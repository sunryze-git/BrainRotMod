using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerTumble : MonoBehaviour
{
	internal bool setup;

	public PlayerAvatar playerAvatar;

	public Transform followPosition;

	public ParticleSystem impactParticle;

	public Sound impactSound;

	[Space]
	public Collider[] colliders;

	public HurtCollider hurtCollider;

	[Space]
	public float customGravity = 10f;

	private float customGravityOverrideTimer;

	internal Rigidbody rb;

	internal PhysGrabObject physGrabObject;

	internal PhotonView photonView;

	internal bool isTumbling;

	private bool isTumblingPrevious = true;

	internal float tumbleSetTimer;

	internal float notMovingTimer;

	private Vector3 notMovingPositionLast;

	private Vector3 tumbleForce;

	private Vector3 tumbleTorque;

	private float tumbleForceTimer;

	private float tumbleOverrideTimer;

	internal bool tumbleOverride;

	private bool tumbleOverridePrevious;

	private float lookAtLerp;

	[Space]
	public Sound tumbleMoveSound;

	public Sound tumbleLaunchSound;

	private float tumbleMoveSoundTimer;

	private float tumbleMoveSoundSpeed;

	internal int tumbleLaunch;

	private float overrideEnemyHurtTimer;

	private float impactHurtTimer;

	private int impactHurtDamage;

	private float hurtColliderPauseTimer;

	private float breakFreeCooldown;

	private void Awake()
	{
		rb = ((Component)this).GetComponent<Rigidbody>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		if (!SemiFunc.RunIsLobbyMenu())
		{
			((MonoBehaviour)this).StartCoroutine(Setup());
		}
	}

	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("SetupRPC", (RpcTarget)4, new object[1] { playerAvatar.playerName });
			}
			SetupDone();
		}
	}

	private void SetupDone()
	{
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		playerAvatar.SoundSetup(tumbleLaunchSound);
		((Component)this).transform.parent = ((Component)playerAvatar).transform.parent;
		setup = true;
		string key = SemiFunc.PlayerGetSteamID(playerAvatar);
		if (StatsManager.instance.playerUpgradeLaunch.ContainsKey(key))
		{
			tumbleLaunch = StatsManager.instance.playerUpgradeLaunch[SemiFunc.PlayerGetSteamID(playerAvatar)];
		}
	}

	[PunRPC]
	public void SetupRPC(string _playerName)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.playerName == _playerName)
			{
				playerAvatar = player;
				playerAvatar.tumble = this;
				break;
			}
		}
		SetupDone();
	}

	private void Update()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsLobbyMenu() || !physGrabObject.spawned)
		{
			return;
		}
		if (isTumbling)
		{
			rb.isKinematic = false;
		}
		else
		{
			rb.isKinematic = true;
		}
		if (!isTumbling && Object.op_Implicit((Object)(object)playerAvatar))
		{
			Vector3 val = ((Component)playerAvatar).transform.position + Vector3.up * 0.3f;
			Quaternion rotation = ((Component)playerAvatar).transform.rotation;
			rb.MovePosition(val);
			rb.MoveRotation(rotation);
		}
		if (tumbleSetTimer > 0f)
		{
			tumbleSetTimer -= Time.deltaTime;
		}
		if (tumbleMoveSoundTimer > 0f)
		{
			tumbleMoveSoundTimer -= Time.deltaTime;
			tumbleMoveSound.PlayLoop(playing: true, 1f, 1f, tumbleMoveSoundSpeed);
		}
		else
		{
			tumbleMoveSound.PlayLoop(playing: false, 1f, 1f, tumbleMoveSoundSpeed);
		}
		if (isTumbling && playerAvatar.isLocal)
		{
			CameraZoom.Instance.OverrideZoomSet(55f, 0.1f, 1f, 1f, ((Component)this).gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.6f, 0.2f, 2f, 2f, 0.1f, ((Component)this).gameObject);
		}
		bool flag = false;
		if (isTumbling)
		{
			Vector3 rbVelocity = physGrabObject.rbVelocity;
			if (((Vector3)(ref rbVelocity)).magnitude > 4f && !physGrabObject.impactDetector.inCart)
			{
				flag = true;
				((Component)hurtCollider).transform.LookAt(((Component)hurtCollider).transform.position + rbVelocity);
				if (physGrabObject.playerGrabbing.Count == 0 && overrideEnemyHurtTimer <= 0f)
				{
					hurtCollider.enemyLogic = true;
				}
				else
				{
					hurtCollider.enemyLogic = false;
				}
				if (playerAvatar.isLocal)
				{
					hurtCollider.playerLogic = false;
				}
			}
		}
		if (hurtColliderPauseTimer > 0f)
		{
			flag = false;
			hurtColliderPauseTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (!((Component)hurtCollider).gameObject.activeSelf)
			{
				((Component)hurtCollider).gameObject.SetActive(true);
			}
		}
		else if (((Component)hurtCollider).gameObject.activeSelf)
		{
			((Component)hurtCollider).gameObject.SetActive(false);
		}
		if (overrideEnemyHurtTimer > 0f)
		{
			overrideEnemyHurtTimer -= Time.deltaTime;
		}
		if (isTumbling)
		{
			if ((Vector3.Distance(notMovingPositionLast, ((Component)this).transform.position) <= 0.5f || physGrabObject.impactDetector.inCart) && physGrabObject.playerGrabbing.Count <= 0)
			{
				notMovingTimer += Time.deltaTime;
			}
			else
			{
				notMovingTimer = 0f;
				notMovingPositionLast = ((Component)this).transform.position;
			}
		}
		else
		{
			notMovingTimer = 0f;
			notMovingPositionLast = ((Component)this).transform.position;
		}
		if (breakFreeCooldown <= 0f)
		{
			if (physGrabObject.playerGrabbing.Count > 0 && playerAvatar.isLocal && SemiFunc.InputDown(InputKey.Jump))
			{
				breakFreeCooldown = 0.5f;
				TumbleForce(playerAvatar.localCameraTransform.forward * 15f);
				TumbleTorque(((Component)this).transform.right * 10f);
				BreakFree(playerAvatar.localCameraTransform.forward);
			}
		}
		else
		{
			breakFreeCooldown -= Time.deltaTime;
		}
		if (impactHurtTimer > 0f)
		{
			impactHurtTimer -= Time.deltaTime;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (physGrabObject.playerGrabbing.Count > 0)
		{
			TumbleOverrideTime(1f);
		}
		if (tumbleOverrideTimer > 0f)
		{
			tumbleOverrideTimer -= Time.deltaTime;
			tumbleOverride = true;
		}
		else
		{
			tumbleOverride = false;
		}
		if (tumbleOverride != tumbleOverridePrevious)
		{
			if (tumbleOverride)
			{
				TumbleOverride(_active: true);
			}
			else
			{
				TumbleOverride(_active: false);
			}
			tumbleOverridePrevious = tumbleOverride;
		}
		if (isTumbling && playerAvatar.isDisabled)
		{
			TumbleRequest(_isTumbling: false, _playerInput: false);
		}
		if (isTumbling == isTumblingPrevious)
		{
			return;
		}
		if (isTumbling)
		{
			SetPosition();
			Vector3 rbVelocityRaw = playerAvatar.rbVelocityRaw;
			rb.AddForce(rbVelocityRaw, (ForceMode)2);
			Vector3 val2 = Vector3.Cross(Vector3.up, rbVelocityRaw);
			if (((Vector3)(ref val2)).magnitude <= 0f)
			{
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				val2 = ((Vector3)(ref insideUnitSphere)).normalized * 1f;
			}
			rb.AddTorque(val2 * 2f, (ForceMode)2);
		}
		isTumblingPrevious = isTumbling;
	}

	private void FixedUpdate()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (!isTumbling || (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		if (isTumbling && playerAvatar.playerHealth.hurtFreeze && playerAvatar.deadSet)
		{
			physGrabObject.FreezeForces(0.1f, Vector3.zero, Vector3.zero);
			return;
		}
		if (customGravityOverrideTimer > 0f)
		{
			customGravityOverrideTimer -= Time.fixedDeltaTime;
		}
		if (rb.useGravity && physGrabObject.playerGrabbing.Count <= 0 && customGravityOverrideTimer <= 0f)
		{
			rb.AddForce(-Vector3.up * customGravity, (ForceMode)0);
		}
		if (tumbleForceTimer > 0f)
		{
			tumbleForceTimer -= Time.fixedDeltaTime;
		}
		if (tumbleForceTimer <= 0f && !playerAvatar.playerHealth.hurtFreeze)
		{
			if (((Vector3)(ref tumbleForce)).magnitude > 0f)
			{
				rb.AddForce(tumbleForce, (ForceMode)1);
				tumbleForce = Vector3.zero;
			}
			if (((Vector3)(ref tumbleTorque)).magnitude > 0f)
			{
				rb.AddTorque(tumbleTorque, (ForceMode)1);
				tumbleTorque = Vector3.zero;
			}
		}
		if (notMovingTimer > 2f)
		{
			lookAtLerp += 0.5f * Time.fixedDeltaTime;
			lookAtLerp = Mathf.Clamp01(lookAtLerp);
			Vector3 val = SemiFunc.PhysFollowRotation(((Component)this).transform, playerAvatar.localCameraRotation, rb, 5f);
			val = Vector3.Lerp(Vector3.zero, val, 3f * Time.fixedDeltaTime);
			val = Vector3.Lerp(Vector3.zero, val, lookAtLerp);
			rb.AddTorque(val, (ForceMode)1);
		}
		else
		{
			lookAtLerp = 0f;
		}
	}

	public void DisableCustomGravity(float _time)
	{
		customGravityOverrideTimer = _time;
	}

	private void SetPosition()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		rb.isKinematic = false;
		tumbleForceTimer = 0.1f;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	public void OverrideEnemyHurt(float _time)
	{
		overrideEnemyHurtTimer = _time;
	}

	public void HitEnemy()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (playerAvatar.isLocal)
			{
				playerAvatar.playerHealth.Hurt(5, savingGrace: true);
			}
			else
			{
				playerAvatar.playerHealth.HurtOther(5, ((Component)this).transform.position, savingGrace: true);
			}
		}
	}

	public void TumbleImpact()
	{
		if (playerAvatar.isLocal)
		{
			PlayerController.instance.CollisionController.StopFallLoop();
		}
		if (!(hurtColliderPauseTimer > 0f) && (!SemiFunc.IsMultiplayer() || hurtCollider.onImpactPlayerAvatar.photonView.IsMine))
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("TumbleImpactRPC", (RpcTarget)0, new object[1] { hurtCollider.onImpactPlayerAvatar.photonView.ViewID });
			}
			else
			{
				TumbleImpactRPC(hurtCollider.onImpactPlayerAvatar.photonView.ViewID);
			}
		}
	}

	[PunRPC]
	public void TumbleImpactRPC(int _playerID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		float time = 0.15f;
		hurtColliderPauseTimer = 0.5f;
		Vector3 val = Vector3.zero;
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			if (item.photonView.ViewID == _playerID)
			{
				item.playerHealth.HurtFreezeOverride(time);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					Vector3 val2 = ((Component)item).transform.position - ((Component)this).transform.position;
					val = ((Vector3)(ref val2)).normalized;
					item.tumble.physGrabObject.FreezeForces(time, val * 5f, Vector3.zero);
				}
				break;
			}
		}
		((Component)impactParticle).gameObject.SetActive(true);
		((Component)impactParticle).transform.position = Vector3.Lerp(((Component)this).transform.position, ((Component)this).transform.position + val, 0.5f);
		impactSound.Play(((Component)impactParticle).transform.position);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 5f, 15f, ((Component)this).transform.position, 0.5f);
		playerAvatar.playerHealth.HurtFreezeOverride(time);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			physGrabObject.FreezeForces(time, val * -5f, Vector3.zero);
		}
	}

	public void TumbleOverride(bool _active)
	{
		if (!GameManager.Multiplayer())
		{
			TumbleOverrideRPC(_active);
			return;
		}
		photonView.RPC("TumbleOverrideRPC", (RpcTarget)0, new object[1] { _active });
	}

	[PunRPC]
	public void TumbleOverrideRPC(bool _active)
	{
		tumbleOverride = _active;
	}

	public void TumbleOverrideTime(float _time)
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			TumbleOverrideTimeRPC(_time);
			return;
		}
		photonView.RPC("TumbleOverrideTimeRPC", (RpcTarget)2, new object[1] { _time });
	}

	[PunRPC]
	public void TumbleOverrideTimeRPC(float _time)
	{
		tumbleOverrideTimer = _time;
	}

	public void TumbleForce(Vector3 _force)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			TumbleForceRPC(_force);
			return;
		}
		photonView.RPC("TumbleForceRPC", (RpcTarget)2, new object[1] { _force });
	}

	[PunRPC]
	public void TumbleForceRPC(Vector3 _force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		tumbleForce += _force;
	}

	public void TumbleTorque(Vector3 _torque)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			TumbleTorqueRPC(_torque);
			return;
		}
		photonView.RPC("TumbleTorqueRPC", (RpcTarget)2, new object[1] { _torque });
	}

	[PunRPC]
	public void TumbleTorqueRPC(Vector3 _torque)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		tumbleTorque += _torque;
	}

	public void TumbleRequest(bool _isTumbling, bool _playerInput)
	{
		if ((!PlayerController.instance.DebugNoTumble || _playerInput) && !SemiFunc.MenuLevel() && isTumbling != _isTumbling)
		{
			if (!GameManager.Multiplayer())
			{
				TumbleRequestRPC(_isTumbling, _playerInput);
				return;
			}
			photonView.RPC("TumbleRequestRPC", (RpcTarget)2, new object[2] { _isTumbling, _playerInput });
		}
	}

	[PunRPC]
	public void TumbleRequestRPC(bool _isTumbling, bool _playerInput)
	{
		if (!SemiFunc.MenuLevel() && isTumbling != _isTumbling)
		{
			TumbleSet(_isTumbling, _playerInput);
		}
	}

	public void TumbleSet(bool _isTumbling, bool _playerInput)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		isTumbling = _isTumbling;
		SetPosition();
		if (isTumbling)
		{
			rb.isKinematic = false;
			if (tumbleLaunch > 0 && _playerInput)
			{
				Vector3 val = playerAvatar.localCameraTransform.forward * (3f * (float)tumbleLaunch);
				tumbleForce += val;
			}
		}
		else
		{
			rb.isKinematic = true;
			tumbleForce = Vector3.zero;
		}
		if (!GameManager.Multiplayer())
		{
			TumbleSetRPC(isTumbling, _playerInput);
			return;
		}
		photonView.RPC("TumbleSetRPC", (RpcTarget)0, new object[2] { isTumbling, _playerInput });
	}

	[PunRPC]
	public void TumbleSetRPC(bool _isTumbling, bool _playerInput)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatar.isLocal && _isTumbling && !_playerInput)
		{
			ChatManager.instance.TumbleInterruption();
		}
		isTumbling = _isTumbling;
		playerAvatar.isTumbling = isTumbling;
		playerAvatar.EnemyVisionFreezeTimerSet(0.5f);
		Vector3 val = ((Component)playerAvatar).transform.position + Vector3.up * 0.3f;
		Quaternion rotation = ((Component)playerAvatar).transform.rotation;
		if (!rb.isKinematic)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		if (SemiFunc.IsMultiplayer())
		{
			physGrabObject.photonTransformView.Teleport(val, rotation);
		}
		else
		{
			physGrabObject.rb.position = val;
			physGrabObject.rb.rotation = rotation;
		}
		if (playerAvatar.isLocal)
		{
			playerAvatar.physGrabber.ReleaseObject();
			PlayerController.instance.tumbleInputDisableTimer = 1f;
			GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
			GameDirector.instance.CameraShake.Shake(2f, 0.5f);
			CameraPosition.instance.TumbleSet();
		}
		if (isTumbling)
		{
			if (tumbleLaunch > 0 && _playerInput)
			{
				tumbleLaunchSound.Play(((Component)this).transform.position);
				playerAvatar.playerAvatarVisuals.PowerupJumpEffect();
			}
			playerAvatar.TumbleStart();
			tumbleSetTimer = 0.1f;
			if (playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				((Collider)playerAvatar.playerAvatarCollision.Collider).enabled = false;
			}
			Collider[] array = colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
		else
		{
			playerAvatar.TumbleStop();
			if (playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = true;
			}
			else
			{
				((Collider)playerAvatar.playerAvatarCollision.Collider).enabled = true;
			}
			Collider[] array = colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
	}

	public void BreakImpact()
	{
		if ((!SemiFunc.IsMultiplayer() || (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.isLocal)) && impactHurtTimer > 0f)
		{
			PlayerController.instance.CollisionController.ResetFalling();
			playerAvatar.playerHealth.Hurt(impactHurtDamage, savingGrace: true);
			impactHurtTimer = 0f;
		}
	}

	public void ImpactHurtSet(float _time, int _damage)
	{
		if (!GameManager.Multiplayer())
		{
			ImpactHurtSetRPC(_time, _damage);
			return;
		}
		photonView.RPC("ImpactHurtSetRPC", (RpcTarget)0, new object[2] { _time, _damage });
	}

	[PunRPC]
	public void ImpactHurtSetRPC(float _time, int _damage)
	{
		if (impactHurtTimer <= 0f || (impactHurtTimer <= _time && _damage == impactHurtDamage) || _damage > impactHurtDamage)
		{
			impactHurtTimer = _time;
			impactHurtDamage = _damage;
		}
	}

	private void BreakFree(Vector3 _direction)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("BreakFreeRPC", (RpcTarget)0, new object[1] { _direction });
		}
	}

	[PunRPC]
	private void BreakFreeRPC(Vector3 _direction)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, ((Component)this).transform.position, 0.25f);
		playerAvatar.TumbleBreakFree();
		foreach (PhysGrabber item in physGrabObject.playerGrabbing)
		{
			if (item.playerAvatar.isLocal)
			{
				Vector3 val = item.playerAvatar.PlayerVisionTarget.VisionTransform.position - ((Component)this).transform.position;
				if (Vector3.Dot(((Vector3)(ref val)).normalized, _direction) > 0.5f)
				{
					item.OverridePullDistanceIncrement(-1f);
				}
			}
		}
	}

	public void TumbleMoveSoundSet(bool _active, float _speed)
	{
		_speed = 1f - _speed;
		_speed = 1f + _speed * 0.25f;
		tumbleMoveSoundSpeed = _speed;
		tumbleMoveSoundTimer = 0.1f;
	}

	private void OnDrawGizmos()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (isTumbling)
		{
			float num = 0.1f;
			Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.8f);
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one * num);
			Gizmos.color = new Color(0.28f, 1f, 0f, 0.5f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one * num);
		}
	}
}
