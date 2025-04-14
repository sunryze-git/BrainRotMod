using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAvatar : MonoBehaviour, IPunObservable
{
	public PhotonView photonView;

	public Transform playerTransform;

	public Transform lowPassRaycastPoint;

	public GameObject spectateCamera;

	public Transform spectatePoint;

	public PhysGrabber physGrabber;

	public PlayerPhysPusher playerPhysPusher;

	public PlayerAvatarVisuals playerAvatarVisuals;

	public PlayerHealth playerHealth;

	public FlashlightController flashlightController;

	public FlashlightLightAim flashlightLightAim;

	public MapToolController mapToolController;

	public PlayerDeathEffects playerDeathEffects;

	public PlayerReviveEffects playerReviveEffects;

	public PlayerDeathHead playerDeathHead;

	public PlayerHealthGrab healthGrab;

	public PlayerTumble tumble;

	public PlayerPhysObjectStander physObjectStander;

	private Collider collider;

	internal string playerName;

	internal string steamID;

	[Space]
	public Transform localCameraTransform;

	private Camera localCamera;

	internal Vector3 localCameraPosition = Vector3.zero;

	internal Quaternion localCameraRotation = Quaternion.identity;

	[Space]
	public PlayerVisionTarget PlayerVisionTarget;

	public RoomVolumeCheck RoomVolumeCheck;

	public Materials.MaterialTrigger MaterialTrigger;

	[Space]
	internal bool isLocal;

	internal bool isDisabled;

	internal bool outroDone;

	internal bool spawned;

	private bool spawnImpulse = true;

	private int spawnFrames = 3;

	private bool spawnDoneImpulse = true;

	private Vector3 spawnPosition;

	internal Quaternion spawnRotation;

	internal bool finalHeal;

	internal bool isCrouching;

	internal bool isSprinting;

	internal bool isCrawling;

	internal bool isSliding;

	internal bool isMoving;

	internal bool isGrounded;

	internal bool isTumbling;

	private bool Interact;

	internal Vector3 InputDirection;

	internal Vector3 LastNavmeshPosition;

	internal float LastNavMeshPositionTimer;

	internal PlayerVoiceChat voiceChat;

	internal bool voiceChatFetched;

	private Rigidbody rb;

	internal Vector3 rbVelocity;

	internal Vector3 rbVelocityRaw;

	private float rbDiscreteTimer;

	internal Vector3 clientPosition = Vector3.zero;

	internal Vector3 clientPositionCurrent = Vector3.zero;

	internal float clientPositionDelta;

	internal Quaternion clientRotation = Quaternion.identity;

	internal Quaternion clientRotationCurrent = Quaternion.identity;

	private float clientRotationDelta;

	public Sound jumpSound;

	public Sound extraJumpSound;

	public Sound landSound;

	public Sound slideSound;

	[Space]
	public Sound standToCrouchSound;

	public Sound crouchToStandSound;

	[Space]
	public Sound crouchToCrawlSound;

	public Sound crawlToCrouchSound;

	[Space]
	public Sound deathBuildupSound;

	public Sound deathSound;

	[Space]
	public Sound tumbleStartSound;

	public Sound tumbleStopSound;

	public Sound tumbleBreakFreeSound;

	[Space]
	public Sound truckReturn;

	public Sound truckReturnGlobal;

	internal bool clientPhysRiding;

	internal int clientPhysRidingID;

	internal Vector3 clientPhysRidingPosition;

	internal Transform clientPhysRidingTransform;

	public static PlayerAvatar instance;

	internal bool spectating;

	internal bool deadSet;

	private float deadTime = 0.5f;

	private float deadTimer;

	private float deadVoiceTime = 0.1f;

	private float deadVoiceTimer;

	internal float enemyVisionFreezeTimer;

	private Transform deadEnemyLookAtTransform;

	internal int steamIDshort;

	internal PlayerAvatarCollision playerAvatarCollision;

	internal bool fallDamageResetState;

	private bool fallDamageResetStatePrevious;

	private float fallDamageResetTimer;

	internal int playerPing;

	private float playerPingTimer;

	internal bool quitApplication;

	private float overrrideAnimationSpeedTimer;

	private float overrrideAnimationSpeedTarget;

	private float overrrideAnimationSpeedIn;

	private float overrrideAnimationSpeedOut;

	private float overrideAnimationSpeedLerp;

	private bool overrideAnimationSpeedActive;

	private float overrideAnimationSpeedTime;

	private SpringFloat overridePupilSizeSpring = new SpringFloat();

	private bool overridePupilSizeActive;

	private float overridePupilSizeTimer;

	private float overridePupilSizeTime;

	private float overridePupilSizeMultiplier = 1f;

	private float overridePupilSizeMultiplierTarget = 1f;

	private float overridePupilSpringSpeedIn = 15f;

	private float overridePupilSpringDampIn = 0.3f;

	private float overridePupilSpringSpeedOut = 15f;

	private float overridePupilSpringDampOut = 0.3f;

	private int overridePupilSizePrio;

	internal int upgradeMapPlayerCount;

	internal bool levelAnimationCompleted;

	internal WorldSpaceUIPlayerName worldSpaceUIPlayerName;

	private void Awake()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		rb = ((Component)this).GetComponent<Rigidbody>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		collider = ((Component)this).GetComponentInChildren<Collider>();
		isDisabled = false;
		((Component)this).transform.position = Vector3.zero + Vector3.forward * 2f;
		playerAvatarCollision = ((Component)this).GetComponent<PlayerAvatarCollision>();
		GameDirector.instance.PlayerList.Add(this);
		if (!SemiFunc.IsMultiplayer() || photonView.IsMine)
		{
			isLocal = true;
		}
	}

	private void OnDestroy()
	{
		GameDirector.instance.PlayerList.Remove(this);
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			item.Enemy.PlayerRemoved(photonView.ViewID);
		}
		Object.Destroy((Object)(object)((Component)((Component)this).transform.parent).gameObject);
	}

	private void Start()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		overridePupilSizeSpring.speed = 15f;
		overridePupilSizeSpring.damping = 0.3f;
		localCamera = Camera.main;
		deadTimer = deadTime;
		deadVoiceTimer = deadVoiceTime;
		if (!SemiFunc.IsMultiplayer() || photonView.IsMine)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForSteamID());
			playerTransform = ((Component)PlayerController.instance).transform;
			playerTransform.position = ((Component)this).transform.position;
			PlayerController.instance.playerAvatar = ((Component)this).gameObject;
			PlayerController.instance.playerAvatarScript = ((Component)this).gameObject.GetComponent<PlayerAvatar>();
			if (Object.op_Implicit((Object)(object)instance))
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				return;
			}
			instance = this;
		}
		SoundSetup(jumpSound);
		SoundSetup(extraJumpSound);
		SoundSetup(landSound);
		SoundSetup(slideSound);
		SoundSetup(standToCrouchSound);
		SoundSetup(crouchToStandSound);
		SoundSetup(crouchToCrawlSound);
		SoundSetup(crawlToCrouchSound);
		SoundSetup(tumbleStartSound);
		SoundSetup(tumbleStopSound);
		SoundSetup(tumbleBreakFreeSound);
		AddToStatsManager();
		if (SemiFunc.IsMasterClient() && LevelGenerator.Instance.Generated)
		{
			LevelGenerator.Instance.PlayerSpawn();
		}
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator WaitForSteamID()
	{
		while (steamID == null)
		{
			yield return null;
		}
		if (SemiFunc.IsMultiplayer())
		{
			PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
		else if (!SemiFunc.IsMainMenu())
		{
			PlayerAvatarSetColor(DataDirector.instance.ColorGetBody());
		}
	}

	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return (object)new WaitForSeconds(0.2f);
		if (StatsManager.instance.playerUpgradeMapPlayerCount.ContainsKey(steamID))
		{
			upgradeMapPlayerCount = StatsManager.instance.playerUpgradeMapPlayerCount[steamID];
		}
		WorldSpaceUIParent.instance.PlayerName(this);
	}

	private void AddToStatsManager()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		string text = SemiFunc.PlayerGetName(this);
		string text2 = SteamClient.SteamId.Value.ToString();
		if (GameManager.Multiplayer() && GameManager.instance.localTest)
		{
			int num = 0;
			Player[] playerList = PhotonNetwork.PlayerList;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i].IsLocal)
				{
					text = text + " " + num;
					text2 += num;
				}
				num++;
			}
		}
		if (GameManager.Multiplayer())
		{
			if (photonView.IsMine)
			{
				photonView.RPC("AddToStatsManagerRPC", (RpcTarget)3, new object[2] { text, text2 });
			}
		}
		else
		{
			AddToStatsManagerRPC(text, text2);
		}
	}

	private void FinalHealCheck()
	{
		if (isLocal && (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial()) && SemiFunc.FPSImpulse5() && RoundDirector.instance.allExtractionPointsCompleted && RoomVolumeCheck.inTruck && !finalHeal)
		{
			FinalHeal();
		}
	}

	private void FinalHeal()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("FinalHealRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			FinalHealRPC();
		}
	}

	[PunRPC]
	public void FinalHealRPC()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (!finalHeal)
		{
			if (isLocal)
			{
				playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 2f, 1);
				TruckScreenText.instance.MessageSendCustom("", playerName + " {arrowright}{truck}{check}\n {point}{shades}{pointright}<b><color=#00FF00>+25</color></b>{heart}", 0);
				playerHealth.Heal(25);
			}
			TruckHealer.instance.Heal(this);
			truckReturn.Play(PlayerVisionTarget.VisionTransform.position);
			truckReturnGlobal.Play(PlayerVisionTarget.VisionTransform.position);
			((Component)playerAvatarVisuals.effectGetIntoTruck).gameObject.SetActive(true);
			finalHeal = true;
		}
	}

	[PunRPC]
	public void AddToStatsManagerRPC(string _playerName, string _steamID)
	{
		playerName = _playerName;
		steamID = _steamID;
		if (!SemiFunc.IsMultiplayer() || (SemiFunc.IsMultiplayer() && photonView.IsMine))
		{
			PlayerController.instance.PlayerSetName(playerName, steamID);
		}
		if (Object.op_Implicit((Object)(object)StatsManager.instance))
		{
			StatsManager.instance.PlayerAdd(_steamID, _playerName);
		}
	}

	[PunRPC]
	public void UpdateMyPlayerVoiceChat(int photonViewID)
	{
		voiceChat = ((Component)PhotonView.Find(photonViewID)).GetComponent<PlayerVoiceChat>();
		voiceChat.playerAvatar = this;
		if (voiceChat.TTSinstantiated)
		{
			voiceChat.ttsVoice.playerAvatar = this;
		}
		if (!SemiFunc.MenuLevel())
		{
			voiceChat.ToggleLobby(_toggle: false);
		}
		voiceChatFetched = true;
	}

	[PunRPC]
	public void ResetPhysPusher()
	{
		playerPhysPusher.Reset = true;
	}

	public void SetDisabled()
	{
		if (GameManager.Multiplayer())
		{
			if (photonView.IsMine)
			{
				photonView.RPC("SetDisabledRPC", (RpcTarget)0, Array.Empty<object>());
				PlayerVoiceChat.instance.OverridePitchCancel();
			}
		}
		else
		{
			SetDisabledRPC();
		}
	}

	[PunRPC]
	public void SetDisabledRPC()
	{
		isDisabled = true;
	}

	public void UpdateState(bool isCrouching, bool isSprinting, bool isCrawling, bool isSliding, bool isMoving)
	{
		SetState(isCrouching, isSprinting, isCrawling, isSliding, isMoving);
	}

	private void FixedUpdate()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		OverridePupilSizeTick();
		OverrideAnimationSpeedTick();
		if (SemiFunc.IsMultiplayer())
		{
			playerPingTimer -= Time.deltaTime;
			if (playerPingTimer <= 0f)
			{
				playerPing = PhotonNetwork.GetPing();
				playerPingTimer = 6f;
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			if (!spawned)
			{
				return;
			}
			clientPosition = spawnPosition;
			clientPositionCurrent = spawnPosition;
			clientRotation = spawnRotation;
			clientRotationCurrent = spawnRotation;
			((Component)this).transform.position = spawnPosition;
			((Component)this).transform.rotation = spawnRotation;
			rb.MovePosition(((Component)this).transform.position);
			rb.MoveRotation(((Component)this).transform.rotation);
			if ((Object)(object)PlayerController.instance.playerAvatarScript == (Object)(object)this)
			{
				((Component)PlayerController.instance).transform.position = spawnPosition;
				((Component)PlayerController.instance).transform.rotation = spawnRotation;
			}
			if (!spawnImpulse)
			{
				return;
			}
			if (spawnFrames <= 0)
			{
				if (GameManager.Multiplayer())
				{
					LevelGenerator.Instance.PhotonView.RPC("PlayerSpawnedRPC", (RpcTarget)0, Array.Empty<object>());
				}
				else
				{
					LevelGenerator.Instance.playerSpawned++;
				}
				spawnImpulse = false;
			}
			else
			{
				spawnFrames--;
			}
			return;
		}
		if (spawnDoneImpulse)
		{
			if ((Object)(object)PlayerController.instance.playerAvatarScript == (Object)(object)this)
			{
				if (Object.op_Implicit((Object)(object)TruckScreenText.instance) && !SemiFunc.MenuLevel())
				{
					Vector3 position = ((Component)TruckScreenText.instance).transform.position;
					Quaternion val = Quaternion.LookRotation(position - ((Component)this).transform.position);
					Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
					CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
					CameraAim.Instance.AimTargetSet(position, 0.3f, 4f, ((Component)this).gameObject, 0);
				}
				else
				{
					CameraAim.Instance.CameraAimSpawn(((Quaternion)(ref spawnRotation)).eulerAngles.y);
				}
				if (SemiFunc.MenuLevel())
				{
					PlayerController.instance.rb.isKinematic = false;
				}
			}
			rb.isKinematic = false;
			spawnDoneImpulse = false;
		}
		if (photonView.IsMine || !SemiFunc.IsMultiplayer())
		{
			rbVelocityRaw = PlayerController.instance.rb.velocity;
			rb.MovePosition(((Component)this).transform.position);
			rb.MoveRotation(((Component)this).transform.rotation);
		}
		else
		{
			rb.MovePosition(clientPositionCurrent);
			rb.MoveRotation(clientRotationCurrent);
		}
	}

	private void Update()
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		FinalHealCheck();
		OverrideAnimationSpeedLogic();
		OverridePupilSizeLogic();
		if (GameManager.Multiplayer() && GameDirector.instance.currentState >= GameDirector.gameState.Main)
		{
			if (voiceChatFetched)
			{
				if (!isDisabled)
				{
					((Component)voiceChat).transform.position = Vector3.Lerp(((Component)voiceChat).transform.position, ((Component)PlayerVisionTarget.VisionTransform).transform.position, 30f * Time.deltaTime);
				}
			}
			else if (photonView.IsMine && Object.op_Implicit((Object)(object)PlayerVoiceChat.instance))
			{
				photonView.RPC("UpdateMyPlayerVoiceChat", (RpcTarget)3, new object[1] { PlayerVoiceChat.instance.photonView.ViewID });
			}
		}
		if (photonView.IsMine || GameManager.instance.gameMode == 0)
		{
			if (Object.op_Implicit((Object)(object)playerTransform))
			{
				((Component)this).transform.position = playerTransform.position;
				((Component)this).transform.rotation = playerTransform.rotation;
			}
			localCameraRotation = PlayerController.instance.cameraGameObject.transform.rotation;
			localCameraPosition = PlayerController.instance.cameraGameObject.transform.position;
			localCameraTransform.position = PlayerController.instance.cameraGameObjectLocal.transform.position;
			localCameraTransform.rotation = PlayerController.instance.cameraGameObjectLocal.transform.rotation;
			InputDirection = PlayerController.instance.InputDirection;
		}
		else
		{
			clientPositionCurrent = clientPosition;
			clientRotationCurrent = clientRotation;
			localCameraTransform.position = Vector3.Lerp(localCameraTransform.position, localCameraPosition, 20f * Time.deltaTime);
			localCameraTransform.rotation = Quaternion.Lerp(localCameraTransform.rotation, localCameraRotation, 20f * Time.deltaTime);
		}
		if (deadSet)
		{
			if (isLocal && Object.op_Implicit((Object)(object)deadEnemyLookAtTransform))
			{
				CameraAim.Instance.AimTargetSet(deadEnemyLookAtTransform.position, 1f, 80f, ((Component)deadEnemyLookAtTransform).gameObject, 0);
			}
			deadTimer -= Time.deltaTime;
			if (deadVoiceTimer > 0f)
			{
				deadVoiceTimer -= Time.deltaTime;
				if (deadVoiceTimer <= 0f && voiceChatFetched)
				{
					voiceChat.ToggleLobby(_toggle: true);
				}
			}
			if (deadTimer <= 0f)
			{
				PlayerDeathDone();
			}
		}
		if (Object.op_Implicit((Object)(object)tumble))
		{
			isTumbling = tumble.isTumbling;
		}
		if (isTumbling)
		{
			collider.enabled = false;
		}
		else
		{
			collider.enabled = true;
		}
		LastNavMeshPositionTimer += Time.deltaTime;
		RaycastHit val = default(RaycastHit);
		NavMeshHit val2 = default(NavMeshHit);
		if (Physics.Raycast(((Component)this).transform.position + Vector3.up * 0.1f, Vector3.down, ref val, 2f, LayerMask.GetMask(new string[3] { "Default", "NavmeshOnly", "PlayerOnlyCollision" })) && NavMesh.SamplePosition(((RaycastHit)(ref val)).point, ref val2, 0.5f, -1))
		{
			LastNavmeshPosition = ((NavMeshHit)(ref val2)).position;
			LastNavMeshPositionTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (((Component)this).transform.position.y < -50f)
			{
				PlayerDeath(-1);
			}
			FallDamageResetLogic();
		}
		if (enemyVisionFreezeTimer > 0f)
		{
			enemyVisionFreezeTimer -= Time.deltaTime;
		}
		if (!isLocal || !(PlayerController.instance.CollisionController.fallDistance >= 8f))
		{
			return;
		}
		float fallDistance = PlayerController.instance.CollisionController.fallDistance;
		float num = 5f;
		float num2 = 4f;
		if (fallDistance > num)
		{
			int damage = 5;
			float time = 0.5f;
			if (fallDistance > num + num2 * 4f)
			{
				damage = 100;
				time = 2f;
			}
			else if (fallDistance > num + num2 * 3f)
			{
				damage = 50;
				time = 2f;
			}
			else if (fallDistance > num + num2 * 2f)
			{
				damage = 25;
				time = 3f;
			}
			else if (fallDistance > num + num2)
			{
				damage = 15;
				time = 3f;
			}
			tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			tumble.TumbleOverrideTime(time);
			if (SemiFunc.FPSImpulse15())
			{
				tumble.ImpactHurtSet(0.5f, damage);
			}
		}
	}

	public void SetState(bool crouching, bool sprinting, bool crawling, bool sliding, bool moving)
	{
		isCrouching = crouching;
		isSprinting = sprinting;
		isCrawling = crawling;
		isSliding = sliding;
		isMoving = moving;
	}

	private void OverrideAnimationSpeedActivate(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		if (isLocal)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("OverrideAnimationSpeedActivateRPC", (RpcTarget)0, new object[5] { active, _speedMulti, _in, _out, _time });
			}
			else
			{
				OverrideAnimationSpeedActivateRPC(active, _speedMulti, _in, _out, _time);
			}
		}
	}

	[PunRPC]
	public void OverrideAnimationSpeedActivateRPC(bool active, float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		overrideAnimationSpeedActive = active;
		overrrideAnimationSpeedTimer = _time;
		overrrideAnimationSpeedTarget = _speedMulti;
		overrrideAnimationSpeedIn = _in;
		overrrideAnimationSpeedOut = _out;
		overrideAnimationSpeedTime = _time;
	}

	public void OverrideAnimationSpeed(float _speedMulti, float _in, float _out, float _time = 0.1f)
	{
		float num = overrrideAnimationSpeedTarget;
		overrrideAnimationSpeedTimer = _time;
		overrrideAnimationSpeedTarget = _speedMulti;
		overrrideAnimationSpeedIn = _in;
		overrrideAnimationSpeedOut = _out;
		overrideAnimationSpeedTime = _time;
		if (SemiFunc.IsMultiplayer() && (!overrideAnimationSpeedActive || num != _speedMulti))
		{
			OverrideAnimationSpeedActivate(active: true, _speedMulti, _in, _out, _time);
		}
	}

	private void OverrideAnimationSpeedTick()
	{
		if (overrrideAnimationSpeedTimer > 0f)
		{
			overrrideAnimationSpeedTimer -= Time.fixedDeltaTime;
			if (overrrideAnimationSpeedTimer <= 0f && SemiFunc.IsMultiplayer() && overrideAnimationSpeedActive)
			{
				OverrideAnimationSpeedActivate(active: false, overrrideAnimationSpeedTarget, overrrideAnimationSpeedIn, overrrideAnimationSpeedOut, overrideAnimationSpeedTime);
			}
		}
	}

	private void OverrideAnimationSpeedLogic()
	{
		if (Object.op_Implicit((Object)(object)playerAvatarVisuals) && (!(overrrideAnimationSpeedTimer <= 0f) || playerAvatarVisuals.animationSpeedMultiplier != 1f))
		{
			if (!isLocal && overrideAnimationSpeedActive)
			{
				OverrideAnimationSpeed(overrrideAnimationSpeedTarget, overrrideAnimationSpeedIn, overrrideAnimationSpeedOut, overrideAnimationSpeedTime);
			}
			if (overrrideAnimationSpeedTimer > 0f)
			{
				overrideAnimationSpeedLerp = Mathf.Lerp(overrideAnimationSpeedLerp, 1f, Time.deltaTime * overrrideAnimationSpeedIn);
			}
			else
			{
				overrideAnimationSpeedLerp = Mathf.Lerp(overrideAnimationSpeedLerp, 0f, Time.deltaTime * overrrideAnimationSpeedOut);
			}
			playerAvatarVisuals.animationSpeedMultiplier = Mathf.Lerp(1f, overrrideAnimationSpeedTarget, overrideAnimationSpeedLerp);
			if (playerAvatarVisuals.animationSpeedMultiplier > 0.98f)
			{
				playerAvatarVisuals.animationSpeedMultiplier = 1f;
			}
		}
	}

	private void OverridePupilSizeActivate(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		if (isLocal)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("OverridePupilSizeActivateRPC", (RpcTarget)0, new object[8] { active, _multiplier, _prio, springSpeedIn, dampIn, springSpeedOut, dampOut, _time });
			}
			else
			{
				OverridePupilSizeActivateRPC(active, _multiplier, _prio, springSpeedIn, dampIn, springSpeedOut, dampOut, _time);
			}
		}
	}

	[PunRPC]
	public void OverridePupilSizeActivateRPC(bool active, float _multiplier, int _prio, float springSpeedIn, float dampIn, float springSpeedOut, float dampOut, float _time = 0.1f)
	{
		overridePupilSizeActive = active;
		overridePupilSizeMultiplier = _multiplier;
		overridePupilSizeMultiplierTarget = _multiplier;
		overridePupilSizePrio = _prio;
		overridePupilSpringSpeedIn = springSpeedIn;
		overridePupilSpringDampIn = dampIn;
		overridePupilSpringSpeedOut = springSpeedOut;
		overridePupilSpringDampOut = dampOut;
		overridePupilSizeTime = _time;
	}

	public void OverridePupilSize(float _multiplier, int _prio, float springSpeedIn, float springDampIn, float springSpeedOut, float springDampOut, float _time = 0.1f)
	{
		if (!(overridePupilSizeTimer > 0f) || _prio >= overridePupilSizePrio)
		{
			float num = overridePupilSizeMultiplierTarget;
			overridePupilSizeMultiplier = _multiplier;
			overridePupilSizeMultiplierTarget = _multiplier;
			overridePupilSizePrio = _prio;
			overridePupilSpringSpeedIn = springSpeedIn;
			overridePupilSpringDampIn = springDampIn;
			overridePupilSpringSpeedOut = springSpeedOut;
			overridePupilSpringDampOut = springDampOut;
			overridePupilSizeTime = _time;
			overridePupilSizeTimer = _time;
			if (SemiFunc.IsMultiplayer() && (!overridePupilSizeActive || num != _multiplier))
			{
				OverridePupilSizeActivate(active: true, _multiplier, _prio, springSpeedIn, springDampIn, springSpeedOut, springDampOut, _time);
			}
		}
	}

	private void OverridePupilSizeTick()
	{
		if (overridePupilSizeTimer > 0f)
		{
			overridePupilSizeTimer -= Time.fixedDeltaTime;
			if (overridePupilSizeTimer <= 0f && SemiFunc.IsMultiplayer() && overridePupilSizeActive)
			{
				OverridePupilSizeActivate(active: false, overridePupilSizeMultiplierTarget, overridePupilSizePrio, overridePupilSpringSpeedIn, overridePupilSpringDampIn, overridePupilSpringSpeedOut, overridePupilSpringDampOut, overridePupilSizeTime);
			}
		}
	}

	private void OverridePupilSizeLogic()
	{
		if (Object.op_Implicit((Object)(object)playerAvatarVisuals))
		{
			if (!isLocal && overridePupilSizeActive)
			{
				OverridePupilSize(overridePupilSizeMultiplierTarget, overridePupilSizePrio, overridePupilSpringSpeedIn, overridePupilSpringDampIn, overridePupilSpringSpeedOut, overridePupilSpringDampOut, overridePupilSizeTime);
			}
			if (overridePupilSizeTimer > 0f)
			{
				overridePupilSizeSpring.speed = overridePupilSpringSpeedIn;
				overridePupilSizeSpring.damping = overridePupilSpringDampIn;
				playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(overridePupilSizeSpring, overridePupilSizeMultiplierTarget);
			}
			else
			{
				overridePupilSizeSpring.speed = overridePupilSpringSpeedOut;
				overridePupilSizeSpring.damping = overridePupilSpringDampOut;
				playerAvatarVisuals.playerEyes.pupilSizeMultiplier = SemiFunc.SpringFloatGet(overridePupilSizeSpring, 1f);
			}
		}
	}

	public void SetSpectate()
	{
		Object.Instantiate<GameObject>(spectateCamera).GetComponent<SpectateCamera>().SetDeath(spectatePoint);
		spectating = true;
	}

	public void SoundSetup(Sound _sound)
	{
		if (photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	public void EnemyVisionFreezeTimerSet(float _time)
	{
		enemyVisionFreezeTimer = _time;
	}

	public void FlashlightFlicker(float _multiplier)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("FlashlightFlickerRPC", (RpcTarget)0, new object[1] { _multiplier });
		}
		else
		{
			FlashlightFlickerRPC(_multiplier);
		}
	}

	[PunRPC]
	public void FlashlightFlickerRPC(float _multiplier)
	{
		flashlightController.FlickerSet(_multiplier);
	}

	public void Slide()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		slideSound.Play(((Component)this).transform.position);
		if (!GameManager.Multiplayer())
		{
			Materials.Instance.Slide(((Component)this).transform.position, MaterialTrigger, 0f, isPlayer: true);
			return;
		}
		Materials.Instance.Slide(((Component)this).transform.position, MaterialTrigger, 0f, isPlayer: true);
		photonView.RPC("SlideRPC", (RpcTarget)1, Array.Empty<object>());
	}

	[PunRPC]
	private void SlideRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		slideSound.Play(((Component)this).transform.position);
		Materials.Instance.Slide(((Component)this).transform.position, MaterialTrigger, 1f, isPlayer: false);
	}

	public void Jump(bool _powerupEffect)
	{
		if (GameManager.instance.gameMode == 0)
		{
			JumpRPC(_powerupEffect);
			return;
		}
		photonView.RPC("JumpRPC", (RpcTarget)0, new object[1] { _powerupEffect });
	}

	[PunRPC]
	private void JumpRPC(bool _powerupEffect)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		playerAvatarVisuals.JumpImpulse();
		jumpSound.Play(((Component)this).transform.position);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(((Component)this).transform.position, Vector3.down, Materials.SoundType.Heavy, footstep: true, MaterialTrigger, hostType);
		if (_powerupEffect)
		{
			extraJumpSound.Play(((Component)this).transform.position);
			playerAvatarVisuals.PowerupJumpEffect();
		}
	}

	public void Land()
	{
		if (GameManager.instance.gameMode == 0)
		{
			LandRPC();
		}
		else
		{
			photonView.RPC("LandRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void LandRPC()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position + Vector3.up * 0.1f, Vector3.down, ref val, 0.25f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject())))
		{
			PhysGrabObject component = ((Component)((RaycastHit)(ref val)).transform).GetComponent<PhysGrabObject>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.mediumBreakImpulse = true;
				return;
			}
		}
		EnemyDirector.instance.SetInvestigate(((Component)this).transform.position + Vector3.up * 0.2f, 10f);
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		landSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(((Component)this).transform.position, Vector3.down, Materials.SoundType.Heavy, footstep: true, MaterialTrigger, hostType);
		Vector3 position = PlayerVisionTarget.VisionTransform.position;
		if (isLocal)
		{
			position = localCameraPosition;
		}
		SemiFunc.PlayerEyesOverrideSoft(position, 2f, ((Component)this).gameObject, 5f);
	}

	public void Footstep(Materials.SoundType soundType)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)RecordingDirector.instance))
		{
			return;
		}
		Materials.HostType hostType = Materials.HostType.LocalPlayer;
		if (!isLocal)
		{
			hostType = Materials.HostType.OtherPlayer;
		}
		Materials.Instance.Impulse(((Component)this).transform.position, Vector3.down, soundType, footstep: true, MaterialTrigger, hostType);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (soundType)
			{
			case Materials.SoundType.Heavy:
				EnemyDirector.instance.SetInvestigate(((Component)this).transform.position + Vector3.up * 0.2f, 5f);
				break;
			case Materials.SoundType.Medium:
				EnemyDirector.instance.SetInvestigate(((Component)this).transform.position + Vector3.up * 0.2f, 1f);
				break;
			}
		}
	}

	public void StandToCrouch()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!isSprinting)
		{
			AudioSource obj = standToCrouchSound.Play(((Component)this).transform.position);
			obj.pitch *= playerAvatarVisuals.animationSpeedMultiplier;
		}
	}

	private float GetPitchMulti()
	{
		return Mathf.Clamp(playerAvatarVisuals.animationSpeedMultiplier, 0.5f, 1.5f);
	}

	public void CrouchToStand()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AudioSource obj = crouchToStandSound.Play(((Component)this).transform.position);
		float pitchMulti = GetPitchMulti();
		obj.pitch *= pitchMulti;
	}

	public void CrouchToCrawl()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!isSliding && !isSprinting)
		{
			AudioSource obj = crouchToCrawlSound.Play(((Component)this).transform.position);
			float pitchMulti = GetPitchMulti();
			obj.pitch *= pitchMulti;
		}
	}

	public void CrawlToCrouch()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!isSliding && !isSprinting)
		{
			AudioSource obj = crawlToCrouchSound.Play(((Component)this).transform.position);
			float pitchMulti = GetPitchMulti();
			obj.pitch *= pitchMulti;
		}
	}

	public void TumbleStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AudioSource obj = tumbleStartSound.Play(((Component)this).transform.position);
		float pitchMulti = GetPitchMulti();
		obj.pitch *= pitchMulti;
	}

	public void TumbleStop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AudioSource obj = tumbleStopSound.Play(((Component)this).transform.position);
		float pitchMulti = GetPitchMulti();
		obj.pitch *= pitchMulti;
	}

	public void TumbleBreakFree()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AudioSource obj = tumbleBreakFreeSound.Play(((Component)this).transform.position);
		obj.pitch *= GetPitchMulti();
		playerAvatarVisuals.TumbleBreakFreeEffect();
	}

	public void PlayerGlitchShort()
	{
		if (GameManager.instance.gameMode == 0)
		{
			CameraGlitch.Instance.PlayShort();
		}
		else
		{
			photonView.RPC("PlayerGlitchShortRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void PlayerGlitchShortRPC()
	{
		if (photonView.IsMine)
		{
			CameraGlitch.Instance.PlayShort();
		}
	}

	public void Spawn(Vector3 position, Quaternion rotation)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			SpawnRPC(position, rotation);
			return;
		}
		photonView.RPC("SpawnRPC", (RpcTarget)0, new object[2] { position, rotation });
	}

	[PunRPC]
	private void SpawnRPC(Vector3 position, Quaternion rotation)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)photonView))
		{
			photonView = ((Component)this).GetComponent<PhotonView>();
		}
		if (!Object.op_Implicit((Object)(object)rb))
		{
			rb = ((Component)this).GetComponent<Rigidbody>();
		}
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			((Component)PlayerController.instance).transform.position = position;
			((Component)PlayerController.instance).transform.rotation = rotation;
		}
		rb.position = position;
		rb.rotation = rotation;
		((Component)this).transform.position = position;
		((Component)this).transform.rotation = rotation;
		clientPosition = position;
		clientPositionCurrent = position;
		clientRotation = rotation;
		clientRotationCurrent = rotation;
		spawnPosition = position;
		spawnRotation = rotation;
		playerAvatarVisuals.visualPosition = position;
		spawned = true;
	}

	public void PlayerDeath(int enemyIndex)
	{
		if (!deadSet)
		{
			if (GameManager.instance.gameMode == 0)
			{
				PlayerDeathRPC(enemyIndex);
				return;
			}
			photonView.RPC("PlayerDeathRPC", (RpcTarget)0, new object[1] { enemyIndex });
		}
	}

	[PunRPC]
	public void PlayerDeathRPC(int enemyIndex)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		playerHealth.Death();
		deadSet = true;
		if (!isLocal)
		{
			deathBuildupSound.Play(((Component)this).transform.position);
		}
		if (!isLocal)
		{
			return;
		}
		deadEnemyLookAtTransform = null;
		Enemy enemy = SemiFunc.EnemyGetFromIndex(enemyIndex);
		if (Object.op_Implicit((Object)(object)enemy))
		{
			if (Object.op_Implicit((Object)(object)enemy.KillLookAtTransform))
			{
				deadEnemyLookAtTransform = enemy.KillLookAtTransform;
			}
			else
			{
				Debug.LogError((object)("Enemy has no kill look at transform..." + ((Object)enemy).name));
			}
		}
		physGrabber.ReleaseObject();
		if (Object.op_Implicit((Object)(object)playerTransform))
		{
			((Component)playerTransform.parent).gameObject.SetActive(false);
		}
		CameraGlitch.Instance.PlayLongHurt();
		GameDirector.instance.DeathStart();
	}

	private void PlayerDeathDone()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsTutorial())
		{
			TutorialDirector.instance.deadPlayer = true;
		}
		if (isDisabled)
		{
			return;
		}
		isDisabled = true;
		if (GameManager.Multiplayer())
		{
			if (!isLocal)
			{
				if (Object.op_Implicit((Object)(object)SpectateCamera.instance))
				{
					SpectateCamera.instance.UpdatePlayer(this);
				}
			}
			else
			{
				physGrabber.ReleaseObject();
				if (SemiFunc.IsMultiplayer())
				{
					if (!SemiFunc.RunIsArena() && Inventory.instance.physGrabber.photonView.ViewID == physGrabber.photonView.ViewID)
					{
						Inventory.instance.ForceUnequip();
					}
				}
				else
				{
					Inventory.instance.ForceUnequip();
				}
			}
		}
		deathSound.Play(((Component)this).transform.position);
		playerDeathHead.Trigger();
		playerDeathEffects.Trigger();
		((Component)this).gameObject.SetActive(false);
	}

	public void OutroStart()
	{
		if (GameManager.instance.gameMode == 0)
		{
			OutroStartRPC();
		}
		else
		{
			photonView.RPC("OutroStartRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	public void OutroStartRPC()
	{
		if (isLocal)
		{
			GameDirector.instance.OutroStart();
		}
	}

	public void OutroDone()
	{
		if (quitApplication)
		{
			Application.Quit();
		}
		else if (NetworkManager.instance.leavePhotonRoom)
		{
			NetworkManager.instance.LeavePhotonRoom();
		}
		else if (GameManager.instance.gameMode == 0)
		{
			OutroDoneRPC();
		}
		else
		{
			photonView.RPC("OutroDoneRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	public void OutroDoneRPC()
	{
		outroDone = true;
	}

	public void ForceImpulse(Vector3 _force)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer())
		{
			ForceImpulseRPC(_force);
			return;
		}
		photonView.RPC("ForceImpulseRPC", (RpcTarget)0, new object[1] { _force });
	}

	[PunRPC]
	private void ForceImpulseRPC(Vector3 _force)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			PlayerController.instance.ForceImpulse(_force);
		}
	}

	public void PlayerAvatarSetColor(int colorIndex)
	{
		if (!GameManager.Multiplayer())
		{
			SetColorRPC(colorIndex);
			return;
		}
		photonView.RPC("SetColorRPC", (RpcTarget)3, new object[1] { colorIndex });
	}

	[PunRPC]
	private void SetColorRPC(int colorIndex)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (isLocal)
		{
			DataDirector.instance.ColorSetBody(colorIndex);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetPlayerColor(steamID, colorIndex);
		}
		playerAvatarVisuals.SetColor(colorIndex);
	}

	public void Revive(bool _revivedByTruck = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			ReviveRPC(_revivedByTruck);
			return;
		}
		photonView.RPC("ReviveRPC", (RpcTarget)0, new object[1] { _revivedByTruck });
	}

	[PunRPC]
	public void ReviveRPC(bool _revivedByTruck)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)playerDeathHead))
		{
			Debug.LogError((object)"Tried to revive without death head...");
			return;
		}
		TutorialDirector.instance.playerRevived = true;
		if (_revivedByTruck)
		{
			TruckHealer.instance.Heal(this);
		}
		Vector3 position = playerDeathHead.physGrabObject.centerPoint - Vector3.up * 0.25f;
		Vector3 eulerAngles = ((Component)playerDeathHead.physGrabObject).transform.eulerAngles;
		if (SemiFunc.RunIsTutorial())
		{
			position = Vector3.zero + Vector3.up * 2f - Vector3.right * 5f;
			((Component)playerDeathHead).transform.position = position;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			tumble.physGrabObject.Teleport(position, ((Component)this).transform.rotation);
		}
		((Component)this).transform.position = position;
		clientPositionCurrent = ((Component)this).transform.position;
		clientPosition = ((Component)this).transform.position;
		clientPhysRiding = false;
		((Component)this).gameObject.SetActive(true);
		((Component)playerAvatarVisuals).gameObject.SetActive(true);
		((Component)playerAvatarVisuals).transform.position = ((Component)this).transform.position;
		playerAvatarVisuals.visualPosition = ((Component)this).transform.position;
		playerAvatarVisuals.Revive();
		isDisabled = false;
		playerDeathHead.Reset();
		playerDeathEffects.Reset();
		playerReviveEffects.Trigger();
		deadSet = false;
		deadTimer = deadTime;
		deadVoiceTimer = deadVoiceTime;
		if (Object.op_Implicit((Object)(object)voiceChat))
		{
			voiceChat.ToggleLobby(_toggle: false);
		}
		playerAvatarCollision.SetCrouch();
		playerHealth.SetMaterialGreen();
		if (isLocal)
		{
			playerHealth.HealOther(1, effect: true);
			playerTransform.position = ((Component)this).transform.position;
			((Component)playerTransform.parent).gameObject.SetActive(true);
			CameraAim.Instance.CameraAimSpawn(eulerAngles.y);
			GameDirector.instance.Revive();
			SpectateCamera.instance.StopSpectate();
			PlayerController.instance.Revive(eulerAngles);
			CameraGlitch.Instance.PlayLongHeal();
		}
		else if (!_revivedByTruck && SemiFunc.RunIsLevel())
		{
			PlayerAvatar playerAvatarScript = PlayerController.instance.playerAvatarScript;
			if (!playerAvatarScript.isDisabled && Vector3.Distance(((Component)playerAvatarScript).transform.position, ((Component)this).transform.position) < 10f && playerAvatarScript.playerHealth.health >= 50 && !TutorialDirector.instance.playerHealed && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialHealing, 1))
			{
				TutorialDirector.instance.ActivateTip("Healing", 0.5f, _interrupt: false);
			}
		}
		RoomVolumeCheck.CheckSet();
	}

	private void FallDamageResetLogic()
	{
		if (fallDamageResetTimer > 0f)
		{
			fallDamageResetTimer -= Time.deltaTime;
			fallDamageResetState = true;
		}
		else
		{
			fallDamageResetState = false;
		}
		if (fallDamageResetState != fallDamageResetStatePrevious)
		{
			fallDamageResetStatePrevious = fallDamageResetState;
			if (!GameManager.Multiplayer())
			{
				FallDamageResetUpdateRPC(fallDamageResetState);
				return;
			}
			photonView.RPC("FallDamageResetUpdateRPC", (RpcTarget)0, new object[1] { fallDamageResetState });
		}
	}

	public void FallDamageResetSet(float _time)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			fallDamageResetTimer = _time;
		}
	}

	[PunRPC]
	private void FallDamageResetUpdateRPC(bool _state)
	{
		fallDamageResetState = _state;
	}

	private void ChatMessageSpeak(string _message, bool crouching)
	{
		if (Object.op_Implicit((Object)(object)voiceChat) && Object.op_Implicit((Object)(object)voiceChat.ttsVoice))
		{
			voiceChat.ttsVoice.TTSSpeakNow(_message, crouching);
		}
	}

	public void ChatMessageSend(string _message, bool _debugMessage)
	{
		if (!_debugMessage)
		{
			foreach (PlayerVoiceChat voiceChat in RunManager.instance.voiceChats)
			{
				if (!voiceChat.recordingEnabled)
				{
					return;
				}
			}
		}
		bool flag = isCrouching;
		SemiFunc.Command(_message);
		if (!SemiFunc.IsMultiplayer())
		{
			ChatMessageSpeak(_message, flag);
			return;
		}
		if (isDisabled)
		{
			flag = true;
		}
		photonView.RPC("ChatMessageSendRPC", (RpcTarget)0, new object[2] { _message, flag });
	}

	[PunRPC]
	public void ChatMessageSendRPC(string _message, bool crouching)
	{
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			ChatMessageSpeak(_message, crouching);
		}
	}

	public void LoadingLevelAnimationCompleted()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("LoadingLevelAnimationCompletedRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			LoadingLevelAnimationCompletedRPC();
		}
	}

	[PunRPC]
	public void LoadingLevelAnimationCompletedRPC()
	{
		levelAnimationCompleted = true;
	}

	public void HealedOther()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("HealedOtherRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	public void HealedOtherRPC()
	{
		if (isLocal)
		{
			TutorialDirector.instance.playerHealed = true;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)isCrouching);
			stream.SendNext((object)isSprinting);
			stream.SendNext((object)isCrawling);
			stream.SendNext((object)isSliding);
			stream.SendNext((object)isMoving);
			stream.SendNext((object)isGrounded);
			stream.SendNext((object)Interact);
			stream.SendNext((object)InputDirection);
			stream.SendNext((object)PlayerController.instance.VelocityRelative);
			stream.SendNext((object)rbVelocityRaw);
			stream.SendNext((object)((Component)PlayerController.instance).transform.position);
			stream.SendNext((object)((Component)PlayerController.instance).transform.rotation);
			stream.SendNext((object)localCameraPosition);
			stream.SendNext((object)localCameraRotation);
			stream.SendNext((object)PlayerController.instance.CollisionGrounded.physRiding);
			stream.SendNext((object)PlayerController.instance.CollisionGrounded.physRidingID);
			stream.SendNext((object)PlayerController.instance.CollisionGrounded.physRidingPosition);
			stream.SendNext((object)flashlightLightAim.clientAimPoint);
			stream.SendNext((object)playerPing);
			return;
		}
		isCrouching = (bool)stream.ReceiveNext();
		isSprinting = (bool)stream.ReceiveNext();
		isCrawling = (bool)stream.ReceiveNext();
		isSliding = (bool)stream.ReceiveNext();
		isMoving = (bool)stream.ReceiveNext();
		isGrounded = (bool)stream.ReceiveNext();
		Interact = (bool)stream.ReceiveNext();
		InputDirection = (Vector3)stream.ReceiveNext();
		rbVelocity = (Vector3)stream.ReceiveNext();
		rbVelocityRaw = (Vector3)stream.ReceiveNext();
		clientPosition = (Vector3)stream.ReceiveNext();
		clientRotation = (Quaternion)stream.ReceiveNext();
		clientPositionDelta = Vector3.Distance(clientPositionCurrent, clientPosition);
		clientRotationDelta = Quaternion.Angle(clientRotationCurrent, clientRotation);
		localCameraPosition = (Vector3)stream.ReceiveNext();
		localCameraRotation = (Quaternion)stream.ReceiveNext();
		clientPhysRiding = (bool)stream.ReceiveNext();
		clientPhysRidingID = (int)stream.ReceiveNext();
		clientPhysRidingPosition = (Vector3)stream.ReceiveNext();
		if (clientPhysRiding)
		{
			PhotonView val = PhotonView.Find(clientPhysRidingID);
			if (Object.op_Implicit((Object)(object)val))
			{
				clientPhysRidingTransform = ((Component)val).transform;
			}
			else
			{
				clientPhysRiding = false;
			}
		}
		playerAvatarVisuals.PhysRidingCheck();
		flashlightLightAim.clientAimPoint = (Vector3)stream.ReceiveNext();
		playerPing = (int)stream.ReceiveNext();
	}
}
