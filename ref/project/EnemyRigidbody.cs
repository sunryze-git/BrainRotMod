using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class EnemyRigidbody : MonoBehaviour
{
	public Enemy enemy;

	public Transform followTarget;

	internal PhysGrabObject physGrabObject;

	internal PhysGrabObjectImpactDetector impactDetector;

	internal PhotonView photonView;

	internal Rigidbody rb;

	internal Vector3 velocity;

	[Space]
	public bool gravity = true;

	public float customGravity;

	[Space]
	public GrabForce grabForceNeeded;

	public float grabTimeNeeded = 0.5f;

	private float grabForceTimer;

	private float grabShakeReleaseTimer;

	public bool grabStun;

	public bool grabOverride;

	public float grabPositionStrength;

	public float grabRotationStrength;

	public float grabStrengthTime = 1f;

	internal float grabStrengthTimer;

	public float grabTimeMax = 3f;

	private float grabTimeMaxRandom;

	private float grabTimeCurrent;

	internal bool grabbed;

	private bool grabbedPrevious;

	[Space]
	public float positionSpeedIdle = 1f;

	public float positionSpeedLerpIdle = 10f;

	public float positionSpeedChase = 2f;

	public float positionSpeedLerpChase = 50f;

	private float positionSpeed;

	private float positionSpeedCurrent;

	internal float positionSpeedLerp = 1f;

	private float positionSpeedLerpCurrent = 1f;

	private Vector3 positionForce;

	[Space]
	public float rotationSpeedIdle = 1f;

	public float rotationSpeedChase = 2f;

	private float rotationSpeed;

	private float rotationSpeedCurrent;

	private float rotationSpeedLerp = 1f;

	[Space]
	public float distanceWarpIdle = 1f;

	public float distanceWarpChase = 2f;

	private float distanceWarp;

	private float timeSinceLastWarp;

	[Space]
	public float notMovingDistance = 1f;

	internal float notMovingTimer;

	private Vector3 lastMovingPosition;

	[Space]
	public bool stunFromFall = true;

	private float stunFromFallTime = 1f;

	private float stunFromFallTimer;

	[Space]
	public AnimationCurve speedResetCurve;

	public float stunResetSpeed = 10f;

	internal float disableFollowPositionTimer;

	internal float disableFollowPositionResetSpeed;

	internal float disableFollowRotationTimer;

	internal float disableFollowRotationResetSpeed;

	internal float disableNoGravityTimer;

	internal float overrideFollowPositionTimer;

	internal float overrideFollowPositionSpeed;

	internal float overrideFollowPositionLerp;

	internal float overrideFollowRotationTimer;

	internal float overrideFollowRotationSpeed;

	private float idleTimer;

	internal float timeSinceStun;

	[Space]
	public PhysicMaterial ColliderMaterialDefault;

	public PhysicMaterial ColliderMaterialDisabled;

	public PhysicMaterial ColliderMaterialStunned;

	public PhysicMaterial ColliderMaterialGrabbed;

	public PhysicMaterial ColliderMaterialJumping;

	private float colliderMaterialStunnedOverrideTimer;

	[Space]
	public Collider playerCollision;

	private bool hasPlayerCollision;

	private bool playerCollisionActive;

	private int materialState = -1;

	internal float teleportedTimer;

	internal float touchingCartTimer;

	internal bool frozen;

	private Vector3 freezeVelocity;

	private Vector3 freezeAngularVelocity;

	private Vector3 freezeForce;

	private Vector3 freezeTorque;

	internal float yOffset;

	[Space]
	public float impactShakeLight = 1f;

	public float impactShakeMedium = 2f;

	public float impactShakeHeavy = 4f;

	public float impactFragility = 1f;

	public UnityEvent onImpactLight;

	public UnityEvent onImpactMedium;

	public UnityEvent onImpactHeavy;

	public UnityEvent onTouchPlayer;

	internal PlayerAvatar onTouchPlayerAvatar;

	public UnityEvent onTouchPlayerGrabbedObject;

	internal PlayerAvatar onTouchPlayerGrabbedObjectAvatar;

	internal PhysGrabObject onTouchPlayerGrabbedObjectPhysObject;

	internal Vector3 onTouchPlayerGrabbedObjectPosition;

	public UnityEvent onTouchPhysObject;

	internal PhysGrabObject onTouchPhysObjectPhysObject;

	internal Vector3 onTouchPhysObjectPosition;

	public UnityEvent onGrabbed;

	internal PlayerAvatar onGrabbedPlayerAvatar;

	internal Vector3 onGrabbedPosition;

	private float warpDisableTimer;

	private EnemyParent enemyParent;

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		enemyParent = ((Component)this).GetComponentInParent<EnemyParent>();
		yOffset = ((Component)this).transform.position.y - followTarget.position.y;
		enemy.Rigidbody = this;
		enemy.HasRigidbody = true;
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		impactDetector.impactFragilityMultiplier = impactFragility;
		if (Object.op_Implicit((Object)(object)playerCollision))
		{
			hasPlayerCollision = true;
			playerCollisionActive = true;
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				playerCollision.enabled = false;
			}
		}
		rb = ((Component)this).GetComponent<Rigidbody>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	public void IdleSet(float time)
	{
		idleTimer = time;
	}

	private void Update()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.playerGrabbing.Count > 0)
		{
			if (physGrabObject.grabbedLocal)
			{
				ItemInfoUI.instance.ItemInfoText(null, enemyParent.enemyName, enemy: true);
			}
			onGrabbedPlayerAvatar = physGrabObject.playerGrabbing[0].playerAvatar;
			onGrabbedPosition = physGrabObject.playerGrabbing[0].physGrabPoint.position;
			onGrabbed.Invoke();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			physGrabObject.enemyInteractTimer = 10f;
			if (touchingCartTimer > 0f)
			{
				touchingCartTimer -= Time.deltaTime;
			}
			positionSpeed = positionSpeedChase;
			rotationSpeed = rotationSpeedChase;
			distanceWarp = distanceWarpChase;
			positionSpeedLerpCurrent = positionSpeedLerpChase;
			if (idleTimer > 0f)
			{
				positionSpeed = positionSpeedIdle;
				rotationSpeed = rotationSpeedIdle;
				distanceWarp = distanceWarpIdle;
				positionSpeedLerpCurrent = positionSpeedLerpIdle;
				idleTimer -= Time.deltaTime;
			}
			if (overrideFollowPositionTimer > 0f)
			{
				positionSpeed = overrideFollowPositionSpeed;
				if (overrideFollowPositionLerp != -1f)
				{
					positionSpeedLerpCurrent = overrideFollowPositionLerp;
				}
				overrideFollowPositionTimer -= Time.deltaTime;
			}
			if (overrideFollowRotationTimer > 0f)
			{
				rotationSpeed = overrideFollowRotationSpeed;
				overrideFollowRotationTimer -= Time.deltaTime;
			}
			if (disableNoGravityTimer > 0f)
			{
				disableNoGravityTimer -= Time.deltaTime;
			}
			else if (!gravity)
			{
				physGrabObject.OverrideZeroGravity();
			}
		}
		if (!enemy.IsStunned())
		{
			impactDetector.ImpactDisable(0.25f);
		}
	}

	private void FixedUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
		if (!frozen)
		{
			velocity = physGrabObject.rbVelocity;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !physGrabObject.spawned)
		{
			return;
		}
		if (teleportedTimer > 0f)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			teleportedTimer -= Time.fixedDeltaTime;
			return;
		}
		if (hasPlayerCollision)
		{
			if (enemy.IsStunned())
			{
				if (playerCollisionActive)
				{
					playerCollisionActive = false;
					playerCollision.enabled = false;
				}
			}
			else if (!playerCollisionActive)
			{
				playerCollisionActive = true;
				playerCollision.enabled = true;
			}
		}
		if (enemy.FreezeTimer > 0f)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			return;
		}
		if (frozen)
		{
			rb.AddForce(freezeVelocity, (ForceMode)2);
			rb.AddTorque(freezeAngularVelocity, (ForceMode)2);
			rb.AddForce(freezeForce, (ForceMode)1);
			rb.AddTorque(freezeTorque, (ForceMode)1);
			freezeForce = Vector3.zero;
			freezeTorque = Vector3.zero;
			frozen = false;
			return;
		}
		bool flag = false;
		if (physGrabObject.playerGrabbing.Count > 0)
		{
			enemy.SetChaseTarget(physGrabObject.playerGrabbing[0].playerAvatar);
			if (((Vector3)(ref physGrabObject.grabDisplacementCurrent)).magnitude >= grabForceNeeded.amount || EnemyDirector.instance.debugEasyGrab)
			{
				grabShakeReleaseTimer = 0f;
				grabForceTimer += Time.fixedDeltaTime;
				if (grabForceTimer >= grabTimeNeeded)
				{
					flag = true;
					if (grabOverride)
					{
						grabStrengthTimer = grabStrengthTime;
					}
				}
			}
			else
			{
				grabShakeReleaseTimer += Time.fixedDeltaTime;
			}
			if (grabShakeReleaseTimer > 3f && enemy.StateStunned.stunTimer <= 0.25f && !grabbed)
			{
				GrabReleaseShake();
			}
			grabTimeCurrent += Time.fixedDeltaTime;
			if (!EnemyDirector.instance.debugNoGrabMaxTime && enemy.StateStunned.stunTimer <= 0.25f && grabTimeCurrent >= grabTimeMaxRandom * (float)physGrabObject.playerGrabbing.Count)
			{
				GrabReleaseShake();
			}
		}
		else
		{
			grabTimeCurrent = 0f;
			grabTimeMaxRandom = grabTimeMax * Random.Range(0.9f, 1.1f);
			grabForceTimer = 0f;
			grabShakeReleaseTimer = 0f;
		}
		Vector3 val;
		if (grabStrengthTimer > 0f)
		{
			flag = true;
			if (grabStun && enemy.HasStateStunned)
			{
				enemy.StateStunned.Set(0.1f);
			}
			val = rb.velocity;
			if (((Vector3)(ref val)).magnitude < 2f)
			{
				grabStrengthTimer -= Time.fixedDeltaTime;
				if (grabStrengthTimer <= 0f)
				{
					GrabReleaseShake();
				}
			}
		}
		if (flag)
		{
			enemy.StuckCount = 0;
			if (enemy.HasJump)
			{
				enemy.Jump.jumpCooldown = 1f;
			}
		}
		if (grabbedPrevious != flag)
		{
			GrabbedSet(flag);
		}
		if (customGravity > 0f && gravity && disableNoGravityTimer <= 0f && rb.useGravity && physGrabObject.playerGrabbing.Count <= 0)
		{
			rb.AddForce(-Vector3.up * customGravity, (ForceMode)0);
		}
		if (grabbed)
		{
			if (materialState != 0)
			{
				Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = ColliderMaterialGrabbed;
				}
				materialState = 0;
			}
		}
		else if (enemy.IsStunned() || colliderMaterialStunnedOverrideTimer > 0f)
		{
			if (materialState != 1)
			{
				Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = ColliderMaterialStunned;
				}
				materialState = 1;
			}
		}
		else if (disableFollowPositionTimer > 0f || disableFollowRotationTimer > 0f)
		{
			if (materialState != 2)
			{
				Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = ColliderMaterialDisabled;
				}
				materialState = 2;
			}
		}
		else if (enemy.HasJump && enemy.Jump.jumping)
		{
			if (materialState != 3)
			{
				Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material = ColliderMaterialJumping;
				}
				materialState = 3;
			}
		}
		else if (materialState != 4)
		{
			Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = ColliderMaterialDefault;
			}
			materialState = 4;
		}
		if (colliderMaterialStunnedOverrideTimer > 0f)
		{
			colliderMaterialStunnedOverrideTimer -= Time.fixedDeltaTime;
		}
		if (disableFollowRotationTimer <= 0f && !enemy.IsStunned())
		{
			rotationSpeedLerp += disableFollowRotationResetSpeed * Time.fixedDeltaTime;
			rotationSpeedLerp = Mathf.Clamp01(rotationSpeedLerp);
			rotationSpeedCurrent = Mathf.Lerp(0f, rotationSpeed, speedResetCurve.Evaluate(rotationSpeedLerp));
			Vector3 val2 = SemiFunc.PhysFollowRotation(((Component)this).transform, followTarget.rotation, rb, rotationSpeedCurrent);
			if (grabStrengthTimer > 0f)
			{
				val2 = Vector3.Lerp(Vector3.zero, val2, grabRotationStrength);
			}
			rb.AddTorque(val2, (ForceMode)1);
		}
		else
		{
			rotationSpeedLerp = 0f;
			disableFollowRotationTimer -= Time.fixedDeltaTime;
		}
		if (disableFollowPositionTimer <= 0f && !enemy.IsStunned())
		{
			timeSinceStun += Time.fixedDeltaTime;
			positionSpeedLerp += disableFollowPositionResetSpeed * Time.fixedDeltaTime;
			positionSpeedLerp = Mathf.Clamp01(positionSpeedLerp);
			positionSpeedCurrent = Mathf.Lerp(0f, positionSpeed, speedResetCurve.Evaluate(positionSpeedLerp));
			Vector3 val3 = SemiFunc.PhysFollowPosition(((Component)rb).transform.position, followTarget.position, rb.velocity, positionSpeedCurrent);
			if (grabStrengthTimer > 0f)
			{
				val3 = Vector3.Lerp(Vector3.zero, val3, grabPositionStrength);
			}
			if ((gravity || disableNoGravityTimer > 0f) && physGrabObject.playerGrabbing.Count <= 0)
			{
				val3.y = 0f;
			}
			val3 = Vector3.Lerp(positionForce, val3, positionSpeedLerpCurrent * Time.fixedDeltaTime);
			rb.AddForce(val3, (ForceMode)1);
		}
		else
		{
			timeSinceStun = 0f;
			positionSpeedLerp = 0f;
			disableFollowPositionTimer -= Time.fixedDeltaTime;
		}
		if (!grabbed && Vector3.Distance(lastMovingPosition, ((Component)this).transform.position) < notMovingDistance)
		{
			notMovingTimer += Time.fixedDeltaTime;
		}
		else
		{
			lastMovingPosition = ((Component)this).transform.position;
			notMovingTimer = 0f;
		}
		if (enemy.HasNavMeshAgent && !grabbed)
		{
			float num = Vector3.Distance(new Vector3(followTarget.position.x, 0f, followTarget.position.z), new Vector3(rb.position.x, 0f, rb.position.z));
			bool flag2 = false;
			if (enemy.HasJump && enemy.Jump.jumping)
			{
				flag2 = true;
			}
			if (warpDisableTimer <= 0f && num >= distanceWarp && !flag2)
			{
				if (enemy.NavMeshAgent.IsDisabled() || enemy.NavMeshAgent.IsStopped())
				{
					((Component)enemy).transform.position = rb.position;
					timeSinceLastWarp = 0f;
					if (LevelGenerator.Instance.Generated && (!enemy.HasAttackPhysObject || !enemy.AttackStuckPhysObject.Active) && notMovingTimer >= 1f)
					{
						enemy.StuckCount++;
					}
				}
				else
				{
					val = enemy.NavMeshAgent.Agent.velocity;
					if (((Vector3)(ref val)).magnitude > 0.1f || num >= distanceWarp * 2f)
					{
						RaycastHit val4 = default(RaycastHit);
						if (Physics.Raycast(rb.position + Vector3.up * 0.1f, Vector3.down, ref val4, 10f, LayerMask.GetMask(new string[3] { "Default", "NavmeshOnly", "PlayerOnlyCollision" })))
						{
							enemy.NavMeshAgent.AgentMove(((RaycastHit)(ref val4)).point);
						}
						else
						{
							enemy.NavMeshAgent.AgentMove(rb.position);
						}
						timeSinceLastWarp = 0f;
						if (LevelGenerator.Instance.Generated && (!enemy.HasAttackPhysObject || !enemy.AttackStuckPhysObject.Active) && notMovingTimer >= 1f)
						{
							enemy.StuckCount++;
						}
					}
				}
			}
			else if (!enemy.NavMeshAgent.IsDisabled() && !enemy.NavMeshAgent.IsStopped())
			{
				timeSinceLastWarp += Time.fixedDeltaTime;
				if (timeSinceLastWarp >= 3f)
				{
					enemy.StuckCount = 0;
				}
			}
		}
		if (warpDisableTimer > 0f)
		{
			warpDisableTimer -= Time.fixedDeltaTime;
		}
		if (stunFromFall && (!enemy.HasJump || !enemy.Jump.jumping) && !grabbed && gravity && disableNoGravityTimer <= 0f && rb.useGravity && (!enemy.HasGrounded || !enemy.Grounded.grounded))
		{
			if (rb.velocity.y < -2f)
			{
				if (stunFromFallTimer >= stunFromFallTime && enemy.HasStateStunned)
				{
					if (!enemy.IsStunned())
					{
						rb.AddTorque(-((Component)this).transform.right * (rb.mass * 0.5f), (ForceMode)1);
					}
					enemy.StateStunned.Set(3f);
				}
				stunFromFallTimer += Time.fixedDeltaTime;
			}
			else
			{
				stunFromFallTimer = 0f;
			}
		}
		else
		{
			stunFromFallTimer = 0f;
		}
	}

	private void OnCollisionStay(Collision other)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer() || enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (other.gameObject.CompareTag("Phys Grab Object"))
		{
			PhysGrabObject physGrabObject = other.gameObject.GetComponent<PhysGrabObject>();
			if (!Object.op_Implicit((Object)(object)physGrabObject))
			{
				physGrabObject = other.gameObject.GetComponentInParent<PhysGrabObject>();
			}
			if (!Object.op_Implicit((Object)(object)physGrabObject))
			{
				return;
			}
			onTouchPhysObjectPhysObject = physGrabObject;
			ContactPoint contact = other.GetContact(0);
			onTouchPhysObjectPosition = ((ContactPoint)(ref contact)).point;
			onTouchPhysObject.Invoke();
			physGrabObject.EnemyInteractTimeSet();
			PhysGrabCart component = ((Component)physGrabObject).GetComponent<PhysGrabCart>();
			if (Object.op_Implicit((Object)(object)component))
			{
				touchingCartTimer = 0.25f;
				foreach (PhysGrabInCart.CartObject item in component.physGrabInCart.inCartObjects.ToList())
				{
					item.physGrabObject.EnemyInteractTimeSet();
				}
			}
			if (enemy.CheckChase())
			{
				if (!(enemy.FreezeTimer <= 0f))
				{
					return;
				}
				Vector3 val = physGrabObject.centerPoint - this.physGrabObject.centerPoint;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				val = followTarget.position - this.physGrabObject.centerPoint;
				if (Vector3.Dot(((Vector3)(ref val)).normalized, normalized) > 0f)
				{
					Vector3 val2 = normalized * 10f;
					val2.y = 5f;
					physGrabObject.rb.AddForce(val2, (ForceMode)1);
					physGrabObject.rb.AddTorque(Random.insideUnitSphere * ((Vector3)(ref val2)).magnitude, (ForceMode)1);
					physGrabObject.lightBreakImpulse = true;
					PhysGrabHinge component2 = ((Component)physGrabObject).GetComponent<PhysGrabHinge>();
					if (Object.op_Implicit((Object)(object)component2) && component2.brokenTimer >= 1.5f)
					{
						component2.DestroyHinge();
					}
					GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 15f, ((Component)this).transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(5f, 5f, 15f, ((Component)this).transform.position, 0.1f);
					rb.AddForce(-normalized * 2f, (ForceMode)1);
					DisableFollowPosition(0.1f, 5f);
				}
			}
			else
			{
				PlayerTumble component3 = ((Component)physGrabObject).GetComponent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)component3))
				{
					onTouchPlayerAvatar = component3.playerAvatar;
					onTouchPlayer.Invoke();
					enemy.SetChaseTarget(component3.playerAvatar);
				}
				else if (physGrabObject.playerGrabbing.Count > 0)
				{
					PlayerAvatar chaseTarget = (onTouchPlayerGrabbedObjectAvatar = physGrabObject.playerGrabbing[0].playerAvatar);
					onTouchPlayerGrabbedObjectPhysObject = physGrabObject;
					contact = other.GetContact(0);
					onTouchPlayerGrabbedObjectPosition = ((ContactPoint)(ref contact)).point;
					onTouchPlayerGrabbedObject.Invoke();
					enemy.SetChaseTarget(chaseTarget);
				}
			}
		}
		else
		{
			if (!other.gameObject.CompareTag("Player"))
			{
				return;
			}
			PlayerController componentInParent = other.gameObject.GetComponentInParent<PlayerController>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				onTouchPlayerAvatar = componentInParent.playerAvatarScript;
				onTouchPlayer.Invoke();
				enemy.SetChaseTarget(componentInParent.playerAvatarScript);
				return;
			}
			PlayerAvatar componentInParent2 = other.gameObject.GetComponentInParent<PlayerAvatar>();
			if (Object.op_Implicit((Object)(object)componentInParent2))
			{
				onTouchPlayerAvatar = componentInParent2;
				onTouchPlayer.Invoke();
				enemy.SetChaseTarget(componentInParent2);
			}
		}
	}

	public void DisableFollowPosition(float time, float resetSpeed)
	{
		disableFollowPositionTimer = time;
		disableFollowPositionResetSpeed = resetSpeed;
	}

	public void DisableFollowRotation(float time, float resetSpeed)
	{
		disableFollowRotationTimer = time;
		disableFollowRotationResetSpeed = resetSpeed;
	}

	public void DisableNoGravity(float time)
	{
		disableNoGravityTimer = time;
	}

	public void OverrideFollowPosition(float time, float speed, float lerp = -1f)
	{
		overrideFollowPositionTimer = time;
		overrideFollowPositionSpeed = speed;
		overrideFollowPositionLerp = lerp;
	}

	public void OverrideFollowRotation(float time, float speed)
	{
		overrideFollowRotationTimer = time;
		overrideFollowRotationSpeed = speed;
	}

	public void Teleport()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		physGrabObject.Teleport(followTarget.position + new Vector3(0f, yOffset, 0f), followTarget.rotation);
		if (!rb.isKinematic)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		freezeForce = Vector3.zero;
		freezeTorque = Vector3.zero;
		frozen = false;
	}

	public void FreezeForces(Vector3 force, Vector3 torque)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!frozen)
		{
			freezeVelocity = rb.velocity;
			freezeAngularVelocity = rb.angularVelocity;
			frozen = true;
		}
		freezeForce += force;
		freezeTorque += torque;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	public void JumpImpulse()
	{
		if (materialState != 3)
		{
			Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = ColliderMaterialJumping;
			}
			materialState = 3;
		}
	}

	public void StuckReset()
	{
		notMovingTimer = 0f;
		enemy.StuckCount = 0;
	}

	public void WarpDisable(float time)
	{
		warpDisableTimer = time;
	}

	public void OverrideColliderMaterialStunned(float _time)
	{
		colliderMaterialStunnedOverrideTimer = _time;
	}

	public void LightImpact()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.HasHealth)
		{
			enemy.Health.LightImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(impactShakeLight, 5f, 15f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(impactShakeLight, 5f, 15f, ((Component)this).transform.position, 0.1f);
		onImpactLight.Invoke();
	}

	public void MediumImpact()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.HasHealth)
		{
			enemy.Health.MediumImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(impactShakeMedium, 5f, 15f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(impactShakeMedium, 5f, 15f, ((Component)this).transform.position, 0.1f);
		onImpactMedium.Invoke();
	}

	public void HeavyImpact()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.HasHealth)
		{
			enemy.Health.HeavyImpact();
		}
		GameDirector.instance.CameraShake.ShakeDistance(impactShakeHeavy, 5f, 15f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(impactShakeHeavy, 5f, 15f, ((Component)this).transform.position, 0.1f);
		onImpactHeavy.Invoke();
	}

	public void GrabRelease()
	{
		bool flag = false;
		foreach (PhysGrabber item in physGrabObject.playerGrabbing.ToList())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				item.ReleaseObject();
			}
			else
			{
				item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 0.1f });
			}
			flag = true;
		}
		if (flag)
		{
			if (GameManager.instance.gameMode == 0)
			{
				GrabReleaseRPC();
			}
			else
			{
				photonView.RPC("GrabReleaseRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
	}

	[PunRPC]
	private void GrabReleaseRPC()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		physGrabObject.grabDisableTimer = 1f;
	}

	private void GrabReleaseShake()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		grabStrengthTimer = 0f;
		GrabbedSet(_grabbed: false);
		float num = 1f * rb.mass;
		rb.AddRelativeTorque(Vector3.up * num, (ForceMode)1);
		GrabRelease();
		DisableFollowRotation(0.5f, 50f);
	}

	private void GrabbedSet(bool _grabbed)
	{
		grabbed = _grabbed;
		grabbedPrevious = _grabbed;
		if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("GrabbedSetRPC", (RpcTarget)0, new object[1] { grabbed });
		}
	}

	[PunRPC]
	private void GrabbedSetRPC(bool _grabbed)
	{
		grabbed = _grabbed;
	}
}
