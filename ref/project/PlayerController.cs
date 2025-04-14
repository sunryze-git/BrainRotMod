using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	private bool previousCrouchingState;

	private bool previousCrawlingState;

	private bool previousSprintingState;

	private bool previousSlidingState;

	private bool previousMovingState;

	public GameObject playerAvatar;

	public GameObject playerAvatarPrefab;

	public PlayerCollision PlayerCollision;

	[HideInInspector]
	public GameObject physGrabObject;

	[HideInInspector]
	public bool physGrabActive;

	[HideInInspector]
	public Transform physGrabPoint;

	[Space]
	public PlayerCollisionController CollisionController;

	public PlayerCollisionGrounded CollisionGrounded;

	private bool GroundedPrevious;

	public Materials.MaterialTrigger MaterialTrigger;

	[HideInInspector]
	public Rigidbody rb;

	private bool CanLand;

	private float landCooldown;

	[Space]
	public float MoveSpeed = 0.5f;

	public float MoveFriction = 5f;

	[HideInInspector]
	public Vector3 Velocity;

	[HideInInspector]
	public Vector3 VelocityRelative;

	private Vector3 VelocityRelativeNew;

	private bool VelocityIdle;

	private Vector3 VelocityImpulse = Vector3.zero;

	[Space]
	public float SprintSpeed = 1f;

	public float SprintSpeedUpgrades;

	private float SprintSpeedCurrent = 1f;

	[HideInInspector]
	public float SprintSpeedLerp;

	public float SprintAcceleration = 1f;

	private float SprintedTimer;

	private float SprintDrainTimer;

	[Space]
	public float CrouchSpeed = 1f;

	public float CrouchTimeMin = 0.2f;

	private float CrouchActiveTimer;

	private float CrouchInactiveTimer;

	[Space]
	public float SlideTime = 1f;

	public float SlideDecay = 0.1f;

	private float SlideTimer;

	private Vector3 SlideDirection;

	private Vector3 SlideDirectionCurrent;

	internal bool CanSlide;

	[HideInInspector]
	public bool Sliding;

	[Space]
	public float JumpForce = 20f;

	internal int JumpExtra;

	private int JumpExtraCurrent;

	private bool JumpFirst;

	public float CustomGravity = 20f;

	internal bool JumpImpulse;

	private float JumpCooldown;

	private float JumpGroundedBuffer;

	internal List<PhysGrabObject> JumpGroundedObjects = new List<PhysGrabObject>();

	private float JumpInputBuffer;

	public float StepUpForce = 2f;

	public bool DebugNoTumble;

	[Space]
	public bool DebugEnergy;

	public float EnergyStart = 100f;

	[HideInInspector]
	public float EnergyCurrent;

	public float EnergySprintDrain = 1f;

	private float sprintRechargeTimer;

	private float sprintRechargeTime = 1f;

	private float sprintRechargeAmount = 2f;

	[Space(15f)]
	public CameraAim cameraAim;

	public GameObject cameraGameObject;

	public GameObject cameraGameObjectLocal;

	public Transform VisionTarget;

	[HideInInspector]
	public bool CanInteract;

	[HideInInspector]
	public bool moving;

	private float movingResetTimer;

	[HideInInspector]
	public bool sprinting;

	[HideInInspector]
	public bool Crouching;

	[HideInInspector]
	public bool Crawling;

	[HideInInspector]
	public Vector3 InputDirection;

	[HideInInspector]
	public AudioSource AudioSource;

	private Vector3 positionPrevious;

	private Vector3 MoveForceDirection = Vector3.zero;

	private float MoveForceAmount;

	private float MoveForceTimer;

	internal float InputDisableTimer;

	private float MoveMultiplier = 1f;

	private float MoveMultiplierTimer;

	internal string playerName;

	internal string playerSteamID;

	private float overrideSpeedTimer;

	internal float overrideSpeedMultiplier = 1f;

	private float overrideLookSpeedTimer;

	internal float overrideLookSpeedTarget = 1f;

	private float overrideLookSpeedTimeIn = 15f;

	private float overrideLookSpeedTimeOut = 0.3f;

	private float overrideLookSpeedLerp;

	private float overrideLookSpeedProgress;

	private float overrideVoicePitchTimer;

	internal float overrideVoicePitchMultiplier = 1f;

	private float overrideTimeScaleTimer;

	internal float overrideTimeScaleMultiplier = 1f;

	private Vector3 originalVelocity;

	private Vector3 originalAngularVelocity;

	public PlayerAvatar playerAvatarScript;

	[Space]
	public Collider col;

	public PhysicMaterial PhysicMaterialMove;

	public PhysicMaterial PhysicMaterialIdle;

	internal float antiGravityTimer;

	internal float featherTimer;

	internal float deathSeenTimer;

	internal float tumbleInputDisableTimer;

	private float kinematicTimer;

	private bool toggleSprint;

	private bool toggleCrouch;

	private float rbOriginalMass;

	private float rbOriginalDrag;

	private float playerOriginalMoveSpeed;

	private float playerOriginalCustomGravity;

	private float playerOriginalSprintSpeed;

	private float playerOriginalCrouchSpeed;

	internal bool debugSlow;

	private void Awake()
	{
		instance = this;
	}

	public void PlayerSetName(string _playerName, string _steamID)
	{
		playerName = _playerName;
		playerSteamID = _steamID;
	}

	public void MoveForce(Vector3 direction, float amount, float time)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MoveForceDirection = ((Vector3)(ref direction)).normalized;
		MoveForceAmount = amount;
		MoveForceTimer = time;
	}

	public void InputDisable(float time)
	{
		InputDisableTimer = time;
	}

	public void MoveMult(float multiplier, float time)
	{
		MoveMultiplier = multiplier;
		MoveMultiplierTimer = time;
	}

	public void CrouchDisable(float time)
	{
		CrouchInactiveTimer = Mathf.Max(time, CrouchInactiveTimer);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (GameManager.instance.gameMode != 0 && !PhotonNetwork.IsMasterClient && other.gameObject.CompareTag("Phys Grab Object"))
		{
			playerAvatarScript.photonView.RPC("ResetPhysPusher", (RpcTarget)2, Array.Empty<object>());
		}
	}

	private void Start()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		rb = ((Component)this).GetComponent<Rigidbody>();
		rbOriginalMass = rb.mass;
		rbOriginalDrag = rb.drag;
		AudioSource = ((Component)this).GetComponent<AudioSource>();
		positionPrevious = ((Component)this).transform.position;
		Inventory component = ((Component)this).GetComponent<Inventory>();
		if (SemiFunc.RunIsArena())
		{
			((Behaviour)component).enabled = false;
		}
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(playerAvatarPrefab, ((Component)this).transform.position, Quaternion.identity);
		}
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		yield return (object)new WaitForSeconds(0.2f);
		string key = SemiFunc.PlayerGetSteamID(playerAvatarScript);
		if (StatsManager.instance.playerUpgradeStamina.ContainsKey(key))
		{
			EnergyStart += (float)StatsManager.instance.playerUpgradeStamina[key] * 10f;
			SprintSpeed += StatsManager.instance.playerUpgradeSpeed[key];
			SprintSpeedUpgrades += StatsManager.instance.playerUpgradeSpeed[key];
			JumpExtra = StatsManager.instance.playerUpgradeExtraJump[key];
		}
		EnergyCurrent = EnergyStart;
		playerOriginalMoveSpeed = MoveSpeed;
		playerOriginalSprintSpeed = SprintSpeed;
		playerOriginalCrouchSpeed = CrouchSpeed;
		playerOriginalCustomGravity = CustomGravity;
		if (SemiFunc.MenuLevel())
		{
			rb.isKinematic = true;
			((Component)this).gameObject.SetActive(false);
		}
	}

	public void ChangeState()
	{
		playerAvatarScript.UpdateState(Crouching, sprinting, Crawling, Sliding, moving);
	}

	public void ForceImpulse(Vector3 force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		VelocityImpulse += ((Component)this).transform.InverseTransformDirection(force);
	}

	public void AntiGravity(float _timer)
	{
		antiGravityTimer = _timer;
	}

	public void Feather(float _timer)
	{
		featherTimer = _timer;
	}

	public void Kinematic(float _timer)
	{
		kinematicTimer = _timer;
		rb.isKinematic = true;
		rb.collisionDetectionMode = (CollisionDetectionMode)0;
	}

	public void SetCrawl()
	{
		Crouching = true;
		Crawling = true;
		CrouchActiveTimer = CrouchTimeMin;
		sprinting = false;
		Sliding = false;
		moving = false;
		PlayerCollisionStand.instance.SetBlocked();
		CameraCrouchPosition.instance.Lerp = 1f;
		CameraCrouchPosition.instance.Active = true;
		CameraCrouchPosition.instance.ActivePrev = true;
		ChangeState();
	}

	public void OverrideSpeed(float _speedMulti, float _time = 0.1f)
	{
		overrideSpeedTimer = _time;
		overrideSpeedMultiplier = _speedMulti;
	}

	private void OverrideSpeedTick()
	{
		if (overrideSpeedTimer > 0f)
		{
			overrideSpeedTimer -= Time.fixedDeltaTime;
			if (overrideSpeedTimer <= 0f)
			{
				overrideSpeedMultiplier = 1f;
				MoveSpeed = playerOriginalMoveSpeed;
				SprintSpeed = playerOriginalSprintSpeed;
				CrouchSpeed = playerOriginalCrouchSpeed;
			}
		}
	}

	private void OverrideSpeedLogic()
	{
		if (!(overrideSpeedTimer <= 0f))
		{
			MoveSpeed = playerOriginalMoveSpeed * overrideSpeedMultiplier;
			SprintSpeed = playerOriginalSprintSpeed * overrideSpeedMultiplier;
			CrouchSpeed = playerOriginalCrouchSpeed * overrideSpeedMultiplier;
		}
	}

	public void OverrideAnimationSpeed(float _animSpeedMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		playerAvatarScript.OverrideAnimationSpeed(_animSpeedMulti, _timeIn, _timeOut, _time);
	}

	public void OverrideTimeScale(float _timeScaleMulti, float _time = 0.1f)
	{
		overrideTimeScaleTimer = _time;
		overrideTimeScaleMultiplier = _timeScaleMulti;
	}

	private void OverrideTimeScaleTick()
	{
		if (overrideTimeScaleTimer > 0f)
		{
			overrideTimeScaleTimer -= Time.fixedDeltaTime;
			if (overrideTimeScaleTimer <= 0f)
			{
				overrideTimeScaleMultiplier = 1f;
				rb.mass = rbOriginalMass;
				rb.drag = rbOriginalDrag;
				CustomGravity = playerOriginalCustomGravity;
				MoveSpeed = playerOriginalMoveSpeed;
				SprintSpeed = playerOriginalSprintSpeed;
				CrouchSpeed = playerOriginalCrouchSpeed;
				rb.useGravity = true;
			}
		}
	}

	private void OverrideTimeScaleLogic()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!(overrideTimeScaleTimer <= 0f))
		{
			float num = overrideSpeedMultiplier;
			float y = rb.velocity.y;
			rb.velocity = Vector3.Lerp(Vector3.zero, rb.velocity, num);
			rb.velocity = new Vector3(rb.velocity.x, y, rb.velocity.z);
			rb.angularVelocity = Vector3.Lerp(Vector3.zero, rb.angularVelocity, num);
			rb.mass = Mathf.Lerp(0.01f, rbOriginalMass, num);
			rb.drag = Mathf.Lerp((1f + overrideSpeedMultiplier) * 10f, rbOriginalDrag, num);
			CustomGravity = Mathf.Lerp(0.1f, playerOriginalCustomGravity, num);
			MoveSpeed = Mathf.Lerp(0.1f, playerOriginalMoveSpeed, num);
			SprintSpeed = Mathf.Lerp(0.1f, playerOriginalSprintSpeed, num);
			CrouchSpeed = Mathf.Lerp(0.1f, playerOriginalCrouchSpeed, num);
			rb.useGravity = false;
		}
	}

	public void OverrideLookSpeed(float _lookSpeedTarget, float timeIn, float timeOut, float _time = 0.1f)
	{
		overrideLookSpeedTimer = _time;
		overrideLookSpeedTarget = _lookSpeedTarget;
		overrideLookSpeedTimeIn = timeIn;
		overrideLookSpeedTimeOut = timeOut;
	}

	private void OverrideLookSpeedTick()
	{
		if (overrideLookSpeedTimer > 0f)
		{
			overrideLookSpeedTimer -= Time.fixedDeltaTime;
		}
	}

	private void OverrideLookSpeedLogic()
	{
		if (overrideLookSpeedTimer <= 0f && overrideLookSpeedProgress <= 0f)
		{
			return;
		}
		float smooth;
		if (overrideLookSpeedTimer > 0f)
		{
			overrideLookSpeedProgress += Time.fixedDeltaTime / overrideLookSpeedTimeIn;
			overrideLookSpeedProgress = Mathf.Clamp01(overrideLookSpeedProgress);
			overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, overrideLookSpeedProgress);
			smooth = Mathf.Lerp(cameraAim.aimSmoothOriginal, overrideLookSpeedTarget, overrideLookSpeedLerp);
		}
		else
		{
			overrideLookSpeedProgress -= Time.fixedDeltaTime / overrideLookSpeedTimeOut;
			overrideLookSpeedProgress = Mathf.Clamp01(overrideLookSpeedProgress);
			overrideLookSpeedLerp = Mathf.SmoothStep(0f, 1f, overrideLookSpeedProgress);
			smooth = Mathf.Lerp(cameraAim.aimSmoothOriginal, overrideLookSpeedTarget, overrideLookSpeedLerp);
			if (overrideLookSpeedProgress <= 0f)
			{
				smooth = cameraAim.aimSmoothOriginal;
			}
		}
		cameraAim.OverrideAimSmooth(smooth, 0.1f);
	}

	public void OverrideVoicePitch(float _voicePitchMulti, float _timeIn, float _timeOut, float _time = 0.1f)
	{
		if (Object.op_Implicit((Object)(object)playerAvatarScript.voiceChat))
		{
			playerAvatarScript.voiceChat.OverridePitch(_voicePitchMulti, _timeIn, _timeOut, _time);
		}
	}

	private void OverrideVoicePitchTick()
	{
		if (overrideVoicePitchTimer > 0f)
		{
			overrideVoicePitchTimer -= Time.fixedDeltaTime;
			if (overrideVoicePitchTimer <= 0f)
			{
				overrideVoicePitchMultiplier = 1f;
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		OverrideSpeedTick();
		OverrideTimeScaleTick();
		OverrideLookSpeedTick();
		OverrideVoicePitchTick();
		if (kinematicTimer > 0f)
		{
			VelocityImpulse = Vector3.zero;
			rb.isKinematic = true;
			kinematicTimer -= Time.fixedDeltaTime;
			if (kinematicTimer <= 0f)
			{
				rb.isKinematic = false;
			}
			return;
		}
		if (playerAvatarScript.isTumbling)
		{
			((Component)this).transform.position = ((Component)playerAvatarScript.tumble).transform.position + Vector3.down * 0.3f;
		}
		if (Crawling != previousCrawlingState)
		{
			ChangeState();
			previousCrawlingState = Crawling;
		}
		if (Crouching != previousCrouchingState)
		{
			ChangeState();
			previousCrouchingState = Crouching;
		}
		if (sprinting != previousSprintingState)
		{
			ChangeState();
			previousSprintingState = sprinting;
		}
		if (Sliding != previousSlidingState)
		{
			ChangeState();
			previousSlidingState = Sliding;
		}
		if (moving != previousMovingState)
		{
			ChangeState();
			previousMovingState = moving;
		}
		Transform transform = ((Component)this).transform;
		Quaternion localRotation = cameraGameObject.transform.localRotation;
		transform.rotation = Quaternion.Euler(0f, ((Quaternion)(ref localRotation)).eulerAngles.y, 0f);
		Vector3 val;
		if ((SemiFunc.InputHold(InputKey.Sprint) || toggleSprint) && !playerAvatarScript.isTumbling && !Crouching && EnergyCurrent >= 1f)
		{
			val = rb.velocity;
			if (((Vector3)(ref val)).magnitude > 0.01f)
			{
				CanSlide = true;
				TutorialDirector.instance.playerSprinted = true;
				sprinting = true;
				SprintedTimer = 0.5f;
				SprintDrainTimer = 0.2f;
			}
		}
		else
		{
			if (SprintedTimer > 0f)
			{
				SprintedTimer -= Time.fixedDeltaTime;
				if (SprintedTimer <= 0f)
				{
					CanSlide = false;
					SprintedTimer = 0f;
				}
			}
			SprintSpeedLerp = 0f;
			sprinting = false;
		}
		if (SprintDrainTimer > 0f && !DebugEnergy)
		{
			float energySprintDrain = EnergySprintDrain;
			energySprintDrain += SprintSpeedUpgrades;
			EnergyCurrent -= energySprintDrain * Time.fixedDeltaTime;
			EnergyCurrent = Mathf.Max(0f, EnergyCurrent);
			if (EnergyCurrent <= 0f)
			{
				toggleSprint = false;
			}
			SprintDrainTimer -= Time.fixedDeltaTime;
		}
		if ((Crouching && PlayerCollisionStand.instance.CheckBlocked()) || playerAvatarScript.isTumbling)
		{
			TutorialDirector.instance.playerCrawled = true;
			Crawling = true;
		}
		else
		{
			Crawling = false;
		}
		if (playerAvatarScript.isTumbling || (CollisionController.Grounded && (SemiFunc.InputHold(InputKey.Crouch) || toggleCrouch)))
		{
			if (CrouchInactiveTimer <= 0f)
			{
				if (!Crouching)
				{
					CrouchActiveTimer = CrouchTimeMin;
				}
				TutorialDirector.instance.playerCrouched = true;
				Crouching = true;
				sprinting = false;
			}
		}
		else if (Crouching && CrouchActiveTimer <= 0f && !Crawling)
		{
			Crawling = false;
			Crouching = false;
			CrouchInactiveTimer = CrouchTimeMin;
		}
		if (CrouchActiveTimer > 0f)
		{
			CrouchActiveTimer -= Time.fixedDeltaTime;
		}
		if (CrouchInactiveTimer > 0f)
		{
			CrouchInactiveTimer -= Time.fixedDeltaTime;
		}
		if (sprinting || Crouching)
		{
			CanInteract = false;
		}
		else
		{
			CanInteract = true;
		}
		Vector3 val2 = Vector3.zero;
		if (MoveForceTimer > 0f)
		{
			InputDirection = MoveForceDirection;
			MoveForceTimer -= Time.fixedDeltaTime;
			rb.velocity = MoveForceDirection * MoveForceAmount;
		}
		else if (InputDisableTimer <= 0f)
		{
			val = new Vector3(SemiFunc.InputMovementX(), 0f, SemiFunc.InputMovementY());
			InputDirection = ((Vector3)(ref val)).normalized;
			if (GameDirector.instance.DisableInput || playerAvatarScript.isTumbling)
			{
				InputDirection = Vector3.zero;
			}
			if (((Vector3)(ref InputDirection)).magnitude <= 0.1f)
			{
				SprintSpeedLerp = 0f;
			}
			if (MoveMultiplierTimer > 0f)
			{
				InputDirection *= MoveMultiplier;
			}
			if (sprinting)
			{
				SprintSpeedCurrent = Mathf.Lerp(MoveSpeed, SprintSpeed, SprintSpeedLerp);
				SprintSpeedLerp += SprintAcceleration * Time.fixedDeltaTime;
				SprintSpeedLerp = Mathf.Clamp01(SprintSpeedLerp);
				val2 += InputDirection * SprintSpeedCurrent;
				SlideDirection = InputDirection * SprintSpeedCurrent;
				SlideDirectionCurrent = SlideDirection;
				Sliding = false;
			}
			else if (Crouching)
			{
				if (CanSlide)
				{
					playerAvatarScript.Slide();
					if (!DebugEnergy)
					{
						EnergyCurrent -= 5f;
					}
					EnergyCurrent = Mathf.Max(0f, EnergyCurrent);
					CanSlide = false;
					Sliding = true;
					SlideTimer = SlideTime;
				}
				if (SlideTimer > 0f)
				{
					val2 += SlideDirectionCurrent;
					SlideDirectionCurrent -= SlideDirection * SlideDecay * Time.fixedDeltaTime;
					SlideTimer -= Time.fixedDeltaTime;
					if (SlideTimer <= 0f)
					{
						Sliding = false;
					}
				}
				if (debugSlow)
				{
					InputDirection *= 0.2f;
				}
				val2 += InputDirection * CrouchSpeed;
			}
			else
			{
				if (debugSlow)
				{
					InputDirection *= 0.1f;
				}
				val2 += InputDirection * MoveSpeed;
				Sliding = false;
			}
		}
		else
		{
			InputDirection = Vector3.zero;
		}
		if (InputDisableTimer > 0f)
		{
			InputDisableTimer -= Time.fixedDeltaTime;
		}
		if (MoveMultiplierTimer > 0f)
		{
			MoveMultiplierTimer -= Time.fixedDeltaTime;
		}
		if (antiGravityTimer > 0f)
		{
			if (rb.useGravity)
			{
				rb.drag = 2f;
				rb.useGravity = false;
			}
			antiGravityTimer -= Time.fixedDeltaTime;
		}
		else if (!rb.useGravity)
		{
			rb.drag = 0f;
			rb.useGravity = true;
		}
		val2 += VelocityImpulse;
		VelocityRelativeNew += VelocityImpulse;
		VelocityImpulse = Vector3.zero;
		if (VelocityIdle)
		{
			VelocityRelativeNew = val2;
		}
		else
		{
			VelocityRelativeNew = Vector3.Lerp(VelocityRelativeNew, val2, MoveFriction * Time.fixedDeltaTime);
		}
		Vector3 val3 = ((Component)this).transform.InverseTransformDirection(rb.velocity);
		if (((Vector3)(ref VelocityRelativeNew)).magnitude > 0.1f)
		{
			VelocityIdle = false;
			col.material = PhysicMaterialMove;
			VelocityRelative = Vector3.Lerp(VelocityRelative, VelocityRelativeNew, MoveFriction * Time.fixedDeltaTime);
			VelocityRelative.y = val3.y;
			rb.AddRelativeForce(VelocityRelative - val3, (ForceMode)1);
			Velocity = ((Component)this).transform.InverseTransformDirection(VelocityRelative - val3);
		}
		else
		{
			VelocityIdle = true;
			col.material = PhysicMaterialIdle;
			VelocityRelative = Vector3.zero;
			Velocity = rb.velocity;
		}
		if (!CollisionController.Grounded && !JumpImpulse && featherTimer <= 0f)
		{
			if (rb.useGravity)
			{
				rb.AddForce(new Vector3(0f, (0f - CustomGravity) * Time.fixedDeltaTime, 0f), (ForceMode)1);
			}
			else
			{
				rb.AddForce(new Vector3(0f, (0f - CustomGravity * 0.1f) * Time.fixedDeltaTime, 0f), (ForceMode)1);
			}
		}
		if (JumpImpulse)
		{
			bool flag = false;
			foreach (PhysGrabObject jumpGroundedObject in JumpGroundedObjects)
			{
				foreach (PhysGrabber item in jumpGroundedObject.playerGrabbing)
				{
					if ((Object)(object)item.playerAvatar == (Object)(object)playerAvatarScript)
					{
						flag = true;
						item.ReleaseObject();
						item.grabDisableTimer = 1f;
						break;
					}
				}
			}
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(0f, rb.velocity.y, 0f);
			float num = JumpForce;
			if (flag)
			{
				num = JumpForce * 0.5f;
			}
			rb.AddForce(Vector3.up * num - val4, (ForceMode)1);
			JumpCooldown = 0.1f;
			JumpImpulse = false;
			CollisionGrounded.Grounded = false;
			JumpGroundedBuffer = 0f;
			CollisionController.GroundedDisableTimer = 0.1f;
			CollisionController.fallDistance = 0f;
		}
		if (((Vector3)(ref VelocityRelativeNew)).magnitude > 0.1f)
		{
			movingResetTimer = 0.1f;
			moving = true;
		}
		else if (movingResetTimer > 0f)
		{
			movingResetTimer -= Time.fixedDeltaTime;
			if (movingResetTimer <= 0f)
			{
				sprinting = false;
				moving = false;
			}
		}
		if (featherTimer > 0f)
		{
			if (rb.useGravity)
			{
				rb.useGravity = false;
			}
			if (antiGravityTimer <= 0f)
			{
				rb.AddForce(new Vector3(0f, -15f, 0f), (ForceMode)0);
			}
			featherTimer -= Time.fixedDeltaTime;
			if (featherTimer <= 0f)
			{
				rb.useGravity = true;
			}
		}
		OverrideTimeScaleLogic();
		OverrideSpeedLogic();
		OverrideLookSpeedLogic();
		positionPrevious = ((Component)this).transform.position;
	}

	private void Update()
	{
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated || SemiFunc.MenuLevel())
		{
			return;
		}
		if (deathSeenTimer > 0f)
		{
			deathSeenTimer -= Time.deltaTime;
		}
		if (CollisionController.Grounded)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Crouch))
			{
				if (SemiFunc.InputDown(InputKey.Crouch))
				{
					toggleCrouch = !toggleCrouch;
					if (toggleCrouch)
					{
						toggleSprint = false;
					}
				}
			}
			else
			{
				toggleCrouch = false;
			}
		}
		if (!playerAvatarScript.isTumbling)
		{
			if (InputManager.instance.InputToggleGet(InputKey.Sprint))
			{
				if (SemiFunc.InputDown(InputKey.Sprint))
				{
					toggleSprint = !toggleSprint;
					if (toggleSprint)
					{
						toggleCrouch = false;
					}
				}
			}
			else
			{
				toggleSprint = false;
			}
		}
		if (sprinting)
		{
			sprintRechargeTimer = sprintRechargeTime;
			if (SemiFunc.RunIsArena())
			{
				sprintRechargeTimer *= 0.5f;
			}
		}
		else if (sprintRechargeTimer > 0f)
		{
			sprintRechargeTimer -= Time.deltaTime;
		}
		else if (EnergyCurrent < EnergyStart)
		{
			float num = sprintRechargeAmount;
			if (SemiFunc.RunIsArena())
			{
				num *= 5f;
			}
			EnergyCurrent += num * Time.deltaTime;
			if (EnergyCurrent > EnergyStart)
			{
				EnergyCurrent = EnergyStart;
			}
		}
		if (!JumpImpulse)
		{
			if (SemiFunc.InputDown(InputKey.Jump) && !playerAvatarScript.isTumbling && InputDisableTimer <= 0f)
			{
				JumpInputBuffer = 0.25f;
			}
			if (CollisionGrounded.Grounded)
			{
				JumpFirst = true;
				JumpExtraCurrent = JumpExtra;
				JumpGroundedBuffer = 0.25f;
			}
			else if (JumpGroundedBuffer > 0f)
			{
				JumpGroundedBuffer -= Time.deltaTime;
				if (JumpGroundedBuffer <= 0f)
				{
					JumpFirst = false;
				}
			}
			if (JumpInputBuffer > 0f)
			{
				JumpInputBuffer -= Time.deltaTime;
			}
			if (JumpCooldown > 0f)
			{
				JumpCooldown -= Time.deltaTime;
			}
			if (JumpInputBuffer > 0f && (JumpGroundedBuffer > 0f || (!JumpFirst && JumpExtraCurrent > 0)) && JumpCooldown <= 0f)
			{
				if (JumpFirst)
				{
					JumpFirst = false;
					playerAvatarScript.Jump(_powerupEffect: false);
				}
				else
				{
					JumpExtraCurrent--;
					playerAvatarScript.Jump(_powerupEffect: true);
				}
				CameraJump.instance.Jump();
				TutorialDirector.instance.playerJumped = true;
				JumpImpulse = true;
				JumpInputBuffer = 0f;
			}
			if (JumpGroundedBuffer <= 0f && JumpGroundedObjects.Count > 0)
			{
				JumpGroundedObjects.Clear();
			}
		}
		if (landCooldown > 0f)
		{
			landCooldown -= Time.deltaTime;
		}
		if (rb.velocity.y < -4f || (Object.op_Implicit((Object)(object)playerAvatarScript.tumble) && playerAvatarScript.tumble.physGrabObject.rbVelocity.y < -4f))
		{
			CanLand = true;
		}
		if (GroundedPrevious != CollisionController.Grounded)
		{
			if (CollisionController.Grounded && CanLand)
			{
				if (!SemiFunc.MenuLevel() && landCooldown <= 0f)
				{
					landCooldown = 1f;
					CameraJump.instance.Land();
					playerAvatarScript.Land();
				}
				CanLand = false;
			}
			GroundedPrevious = CollisionController.Grounded;
		}
		if (tumbleInputDisableTimer > 0f)
		{
			tumbleInputDisableTimer -= Time.deltaTime;
		}
		if (playerAvatarScript.isTumbling)
		{
			col.enabled = false;
			rb.isKinematic = true;
			bool flag = false;
			if (playerAvatarScript.tumble.notMovingTimer > 0.5f && (Mathf.Abs(SemiFunc.InputMovementX()) > 0f || Mathf.Abs(SemiFunc.InputMovementY()) > 0f))
			{
				flag = true;
			}
			if ((SemiFunc.InputDown(InputKey.Jump) || SemiFunc.InputDown(InputKey.Tumble) || flag) && tumbleInputDisableTimer <= 0f && !playerAvatarScript.tumble.tumbleOverride && InputDisableTimer <= 0f)
			{
				playerAvatarScript.tumble.TumbleRequest(_isTumbling: false, _playerInput: true);
			}
		}
		else
		{
			col.enabled = true;
			rb.isKinematic = false;
			if (SemiFunc.InputDown(InputKey.Tumble) && tumbleInputDisableTimer <= 0f && InputDisableTimer <= 0f)
			{
				TutorialDirector.instance.playerTumbled = true;
				playerAvatarScript.tumble.TumbleRequest(_isTumbling: true, _playerInput: true);
			}
		}
	}

	public void Revive(Vector3 _rotation)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = Quaternion.Euler(0f, _rotation.y, 0f);
		InputDisable(0.5f);
		Kinematic(0.2f);
		SetCrawl();
		CollisionController.ResetFalling();
		VelocityIdle = true;
		col.material = PhysicMaterialIdle;
		VelocityRelative = Vector3.zero;
		Velocity = Vector3.zero;
		EnergyCurrent = EnergyStart;
	}
}
