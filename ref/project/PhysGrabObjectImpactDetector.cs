using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PhysGrabObjectImpactDetector : MonoBehaviour, IPunObservable
{
	public enum ImpactState
	{
		None,
		Light,
		Medium,
		Heavy
	}

	public bool particleDisable;

	[Range(0f, 4f)]
	public float particleMultiplier = 1f;

	[Space]
	public bool playerHurtDisable;

	public bool slidingDisable;

	public bool destroyDisable;

	internal int destroyDisableLaunches;

	internal float destroyDisableLaunchesTimer;

	internal bool destroyDisableTeleport = true;

	public bool indestructibleBreakEffects = true;

	public bool canHurtLogic = true;

	[HideInInspector]
	public PhysObjectParticles particles;

	private List<Transform> colliderTransforms = new List<Transform>();

	private EnemyRigidbody enemyRigidbody;

	[HideInInspector]
	public bool isEnemy;

	internal float enemyInteractionTimer;

	private Rigidbody rb;

	private Materials.MaterialTrigger materialTrigger = new Materials.MaterialTrigger();

	[HideInInspector]
	public float fragility = 50f;

	[HideInInspector]
	public float durability = 100f;

	private float impactLevel1 = 300f;

	private float impactLevel2 = 400f;

	private float impactLevel3 = 500f;

	private float breakLevel1Cooldown;

	private float breakLevel2Cooldown;

	private float breakLevel3Cooldown;

	private float impactLightCooldown;

	private float impactMediumCooldown;

	private float impactHeavyCooldown;

	private Vector3 previousPosition;

	private Vector3 previousRotation;

	private Camera mainCamera;

	private float impactCooldown;

	internal bool isIndestructible;

	internal float impulseTimerDeactivateImpacts = 5f;

	internal float highestVelocity;

	internal float impactForce;

	internal float resetPrevPositionTimer;

	private PhysGrabObject physGrabObject;

	private PhotonView photonView;

	internal bool isHinge;

	internal bool isBrokenHinge;

	private ValuableObject valuableObject;

	private NotValuableObject notValuableObject;

	private bool isNotValuable;

	private bool breakLogic;

	[HideInInspector]
	public bool isValuable;

	private bool collisionsActive;

	private float collisionsActiveTimer;

	private float collisionActivatedBuffer;

	[HideInInspector]
	public bool isSliding;

	private float slidingTimer;

	private float slidingGain;

	private float slidingSpeedThreshold = 0.1f;

	private float slidingAudioSpeed;

	private Vector3 previousSlidingPosition;

	internal Vector3 previousVelocity;

	internal Vector3 previousAngularVelocity;

	internal Vector3 previousVelocityRaw;

	internal Vector3 previousPreviousVelocityRaw;

	private bool impactHappened;

	internal float impactDisabledTimer;

	private Vector3 contactPoint;

	private PhysAudio impactAudio;

	private float impactAudioPitch = 1f;

	private bool audioActive;

	private float colliderVolume;

	private float timerInCart;

	internal int breakLevelHeavy;

	internal int breakLevelMedium = 1;

	internal int breakLevelLight = 2;

	private Vector3 prevPos;

	private Quaternion prevRot;

	private bool isMoving;

	private float breakForce;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	public UnityEvent onAllImpacts;

	public UnityEvent onImpactLight;

	public UnityEvent onImpactMedium;

	public UnityEvent onImpactHeavy;

	[Space(15f)]
	public UnityEvent onAllBreaks;

	public UnityEvent onBreakLight;

	public UnityEvent onBreakMedium;

	public UnityEvent onBreakHeavy;

	[Space(15f)]
	public UnityEvent onDestroy;

	[HideInInspector]
	public bool inCart;

	private bool inCartPrevious;

	[HideInInspector]
	public bool isCart;

	private PhysGrabCart cart;

	private float inCartVolumeMultiplier;

	private float impactCheckTimer;

	private PhysGrabCart currentCart;

	internal float indestructibleSpawnTimer = 5f;

	internal bool isColliding;

	private float isCollidingTimer;

	[HideInInspector]
	public float fragilityMultiplier = 1f;

	[HideInInspector]
	public float impactFragilityMultiplier = 1f;

	private float playerHurtMultiplier = 1f;

	private float playerHurtMultiplierTimer;

	private void Start()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		inCartVolumeMultiplier = 0.6f;
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponent<PhysGrabHinge>()))
		{
			isHinge = true;
		}
		cart = ((Component)this).GetComponent<PhysGrabCart>();
		if (Object.op_Implicit((Object)(object)cart))
		{
			isCart = true;
		}
		enemyRigidbody = ((Component)this).GetComponent<EnemyRigidbody>();
		if (Object.op_Implicit((Object)(object)enemyRigidbody))
		{
			isEnemy = true;
		}
		previousSlidingPosition = ((Component)this).transform.position;
		valuableObject = ((Component)this).GetComponent<ValuableObject>();
		if (Object.op_Implicit((Object)(object)valuableObject))
		{
			isValuable = true;
			breakLogic = true;
			fragility = valuableObject.durabilityPreset.fragility;
			durability = valuableObject.durabilityPreset.durability;
			impactAudio = valuableObject.audioPreset;
			impactAudioPitch = valuableObject.audioPresetPitch;
		}
		else
		{
			notValuableObject = ((Component)this).GetComponent<NotValuableObject>();
			isNotValuable = true;
			if (Object.op_Implicit((Object)(object)notValuableObject))
			{
				if (Object.op_Implicit((Object)(object)notValuableObject.durabilityPreset))
				{
					breakLogic = true;
					fragility = notValuableObject.durabilityPreset.fragility;
					durability = notValuableObject.durabilityPreset.durability;
				}
				impactAudio = notValuableObject.audioPreset;
				impactAudioPitch = notValuableObject.audioPresetPitch;
			}
		}
		if (Object.op_Implicit((Object)(object)impactAudio))
		{
			audioActive = true;
		}
		else
		{
			audioActive = false;
		}
		photonView = ((Component)this).GetComponent<PhotonView>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		mainCamera = Camera.main;
		ColliderGet(((Component)this).transform);
		colliderVolume /= 200000f;
		GameObject val = Object.Instantiate<GameObject>(Resources.Load<GameObject>("Phys Object Particles"), new Vector3(0f, 0f, 0f), Quaternion.identity);
		val.transform.parent = ((Component)this).transform;
		val.transform.localPosition = new Vector3(0f, 0f, 0f);
		particles = val.GetComponent<PhysObjectParticles>();
		particles.multiplier = particleMultiplier;
		if (isValuable)
		{
			particles.gradient = valuableObject.particleColors;
		}
		if (Object.op_Implicit((Object)(object)notValuableObject))
		{
			particles.gradient = notValuableObject.particleColors;
		}
		particles.colliderTransforms = colliderTransforms;
		originalPosition = rb.position;
		originalRotation = rb.rotation;
	}

	private void ColliderGet(Transform transform)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		if (((Component)transform).CompareTag("Phys Grab Object") && Object.op_Implicit((Object)(object)((Component)transform).GetComponent<Collider>()))
		{
			colliderTransforms.Add(transform);
			Bounds bounds = ((Component)((Component)transform).transform).GetComponent<Collider>().bounds;
			float num = ((Bounds)(ref bounds)).size.x * 100f * (((Bounds)(ref bounds)).size.y * 100f) * (((Bounds)(ref bounds)).size.z * 100f);
			if (Object.op_Implicit((Object)(object)((Component)transform).GetComponent<SphereCollider>()))
			{
				num *= 0.55f;
			}
			colliderVolume += num;
		}
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			ColliderGet(transform2);
		}
	}

	[PunRPC]
	private void InCartRPC(bool inCartState)
	{
		inCart = inCartState;
	}

	private void IndestructibleSpawnTimer()
	{
		if (indestructibleSpawnTimer > 0f)
		{
			physGrabObject.OverrideIndestructible();
			indestructibleSpawnTimer -= Time.deltaTime;
		}
	}

	private void Update()
	{
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		IndestructibleSpawnTimer();
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (inCartPrevious != inCart)
		{
			inCartPrevious = inCart;
			if (GameManager.instance.gameMode == 1)
			{
				photonView.RPC("InCartRPC", (RpcTarget)1, new object[1] { inCart });
			}
		}
		if (timerInCart > 0f)
		{
			inCart = true;
			timerInCart -= Time.deltaTime;
		}
		else
		{
			inCart = false;
			currentCart = null;
		}
		if (isValuable && !valuableObject.dollarValueSet)
		{
			return;
		}
		if (isCollidingTimer > 0f)
		{
			isColliding = true;
			isCollidingTimer -= Time.deltaTime;
		}
		else
		{
			isColliding = false;
		}
		Vector3 val2;
		if (isSliding)
		{
			Vector3 val = previousSlidingPosition;
			val.y = 0f;
			Vector3 position = ((Component)this).transform.position;
			position.y = 0f;
			val2 = position - val;
			float num = ((Vector3)(ref val2)).magnitude / Time.deltaTime;
			if (num >= slidingSpeedThreshold)
			{
				slidingAudioSpeed = Mathf.Lerp(slidingAudioSpeed, 1f + num * 0.01f, 10f * Time.deltaTime);
				Materials.Instance.SlideLoop(rb.worldCenterOfMass, materialTrigger, 1f, 1f + slidingAudioSpeed);
			}
			if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
			{
				slidingTimer -= Time.deltaTime;
				if (slidingTimer < 0f)
				{
					isSliding = false;
				}
			}
		}
		previousSlidingPosition = ((Component)this).transform.position;
		if (playerHurtMultiplierTimer > 0f)
		{
			playerHurtMultiplierTimer -= Time.deltaTime;
			if (playerHurtMultiplierTimer <= 0f)
			{
				playerHurtMultiplier = 1f;
			}
		}
		if (physGrabObject.grabbed)
		{
			collisionsActiveTimer = 0.5f;
		}
		val2 = rb.velocity;
		if (!(((Vector3)(ref val2)).magnitude > 0.01f))
		{
			val2 = rb.angularVelocity;
			if (!(((Vector3)(ref val2)).magnitude > 0.1f))
			{
				goto IL_0264;
			}
		}
		collisionsActiveTimer = 0.5f;
		goto IL_0264;
		IL_0264:
		if (collisionsActiveTimer > 0f)
		{
			if (!collisionsActive)
			{
				collisionActivatedBuffer = 0.1f;
			}
			collisionsActive = true;
			collisionsActiveTimer -= Time.deltaTime;
		}
		else
		{
			collisionsActive = false;
		}
		if (collisionActivatedBuffer > 0f)
		{
			collisionActivatedBuffer -= Time.deltaTime;
		}
		if (breakLevel1Cooldown > 0f)
		{
			breakLevel1Cooldown -= Time.deltaTime;
		}
		if (breakLevel2Cooldown > 0f)
		{
			breakLevel2Cooldown -= Time.deltaTime;
		}
		if (breakLevel3Cooldown > 0f)
		{
			breakLevel3Cooldown -= Time.deltaTime;
		}
		if (impactLightCooldown > 0f)
		{
			impactLightCooldown -= Time.deltaTime;
		}
		if (impactMediumCooldown > 0f)
		{
			impactMediumCooldown -= Time.deltaTime;
		}
		if (impactHeavyCooldown > 0f)
		{
			impactHeavyCooldown -= Time.deltaTime;
		}
		if (impactCooldown > 0f)
		{
			impactCooldown -= Time.deltaTime;
		}
		if (impulseTimerDeactivateImpacts > 0f)
		{
			impulseTimerDeactivateImpacts -= Time.deltaTime;
		}
		if (resetPrevPositionTimer > 0f)
		{
			resetPrevPositionTimer -= Time.deltaTime;
			previousPosition = Vector3.zero;
		}
		if (enemyInteractionTimer > 0f)
		{
			enemyInteractionTimer -= Time.deltaTime;
		}
		if (destroyDisableLaunchesTimer > 0f)
		{
			destroyDisableLaunchesTimer -= Time.deltaTime;
			if (destroyDisableLaunchesTimer <= 0f)
			{
				destroyDisableLaunches = 0;
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (inCart && !isEnemy && physGrabObject.playerGrabbing.Count == 0 && Object.op_Implicit((Object)(object)currentCart) && !rb.isKinematic && !Object.op_Implicit((Object)(object)((Component)this).GetComponent<PlayerTumble>()))
		{
			PhysGrabCart component = ((Component)currentCart).GetComponent<PhysGrabCart>();
			if (((Vector3)(ref component.actualVelocity)).magnitude > 1f)
			{
				Vector3 velocity = rb.velocity;
				rb.velocity = Vector3.Lerp(rb.velocity, component.actualVelocity, 30f * Time.fixedDeltaTime);
				if (rb.velocity.y > velocity.y)
				{
					rb.velocity = new Vector3(rb.velocity.x, velocity.y, rb.velocity.z);
				}
			}
		}
		impactHappened = false;
		breakForce = 0f;
		impactForce = 0f;
		if (impactDisabledTimer <= 0f)
		{
			Vector3 val = rb.velocity / Time.fixedDeltaTime;
			Vector3 val2 = rb.angularVelocity / Time.fixedDeltaTime;
			float magnitude = ((Vector3)(ref previousVelocity)).magnitude;
			float num = Mathf.Abs(magnitude - ((Vector3)(ref val)).magnitude);
			float magnitude2 = ((Vector3)(ref previousAngularVelocity)).magnitude;
			float num2 = Mathf.Abs(magnitude2 - ((Vector3)(ref val2)).magnitude);
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Vector3 normalized2 = ((Vector3)(ref previousVelocity)).normalized;
			float num3 = Vector3.Angle(normalized, normalized2);
			Vector3 normalized3 = ((Vector3)(ref val2)).normalized;
			Vector3 normalized4 = ((Vector3)(ref previousAngularVelocity)).normalized;
			float num4 = Vector3.Angle(normalized3, normalized4);
			num *= 1f;
			num2 *= 0.4f * rb.mass;
			num3 *= 0.2f;
			num4 *= 0.02f * rb.mass;
			if ((num > 1f && magnitude > 1f) || (num2 > 1f && magnitude2 > 1f) || (num3 > 1f && magnitude > 1f) || (num4 > 1f && magnitude2 > 1f))
			{
				impactHappened = true;
				float num5 = num * 2f;
				float num6 = Mathf.Max(rb.mass, 1f);
				breakForce += num5 * num6;
			}
			breakForce *= 8f;
			impactForce = breakForce / 8f * impactFragilityMultiplier;
			breakForce = breakForce * (fragility / 100f) * fragilityMultiplier;
			if (impactHappened)
			{
				if (inCart)
				{
					breakForce = 0f;
				}
				if (inCart || isCart)
				{
					impactForce *= 0.3f;
				}
			}
		}
		else
		{
			impactDisabledTimer -= Time.fixedDeltaTime;
		}
		previousPreviousVelocityRaw = previousVelocityRaw;
		previousVelocityRaw = rb.velocity;
		previousVelocity = rb.velocity / Time.fixedDeltaTime;
		previousAngularVelocity = rb.angularVelocity / Time.fixedDeltaTime;
		if (Vector3.Distance(prevPos, ((Component)this).transform.position) > 0.01f || Quaternion.Angle(prevRot, ((Component)this).transform.rotation) > 0.1f)
		{
			isMoving = true;
		}
		prevPos = ((Component)this).transform.position;
		prevRot = ((Component)this).transform.rotation;
	}

	public void ImpactDisable(float time)
	{
		impactDisabledTimer = time;
	}

	private void EnemyInvestigate(float radius)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!(physGrabObject.enemyInteractTimer > 0f) && !inCart && !isCart)
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, radius);
		}
	}

	public void DestroyObject(bool effects = true)
	{
		if (destroyDisable || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		EnemyRigidbody component = ((Component)this).GetComponent<EnemyRigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.enemy.EnemyParent.Despawn();
		}
		else if (!physGrabObject.dead)
		{
			physGrabObject.dead = true;
			EnemyInvestigate(15f);
			if (!SemiFunc.IsMultiplayer())
			{
				DestroyObjectRPC(effects);
				return;
			}
			photonView.RPC("DestroyObjectRPC", (RpcTarget)0, new object[1] { effects });
		}
	}

	[PunRPC]
	public void DestroyObjectRPC(bool effects)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		physGrabObject.dead = true;
		if (effects)
		{
			GameDirector.instance.CameraImpact.ShakeDistance(10f, 1f, 6f, ((Component)this).transform.position, 0.1f);
		}
		if (Object.op_Implicit((Object)(object)particles))
		{
			((Component)particles).transform.parent = null;
			particles.DestroyParticles();
		}
		if (audioActive && effects)
		{
			AudioSource val = impactAudio.destroy.Play(physGrabObject.centerPoint);
			if (Object.op_Implicit((Object)(object)val))
			{
				val.pitch *= impactAudioPitch;
			}
		}
		onDestroy.Invoke();
	}

	public void BreakHeavy(Vector3 contactPoint)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.1f * (1f + 9f * (100f - durability) / 100f);
		bool flag = false;
		if (isValuable || breakLogic)
		{
			float valueLost = 0f;
			if (isValuable)
			{
				valueLost = Mathf.Round(valuableObject.dollarValueOriginal * num);
				valueLost += Mathf.Round(Random.Range((0f - valueLost) * 0.1f, valueLost * 0.1f));
				valueLost = Mathf.Clamp(valueLost, 0f, valuableObject.dollarValueCurrent);
			}
			Break(valueLost, contactPoint, breakLevelHeavy);
			flag = true;
		}
		if (isNotValuable && notValuableObject.hasHealth)
		{
			notValuableObject.Impact(ImpactState.Heavy);
			flag = true;
		}
		if (flag)
		{
			EnemyInvestigate(10f);
		}
		breakLevel3Cooldown = 0.6f;
		breakLevel2Cooldown = 0.4f;
		breakLevel1Cooldown = 0.3f;
	}

	public void BreakMedium(Vector3 contactPoint)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.05f * (1f + 9f * (100f - durability) / 100f);
		bool flag = false;
		if (isValuable || breakLogic)
		{
			float valueLost = 0f;
			if (isValuable)
			{
				valueLost = Mathf.Round(valuableObject.dollarValueOriginal * num);
				valueLost += Mathf.Round(Random.Range((0f - valueLost) * 0.1f, valueLost * 0.1f));
				valueLost = Mathf.Clamp(valueLost, 0f, valuableObject.dollarValueCurrent);
			}
			Break(valueLost, contactPoint, breakLevelMedium);
			flag = true;
		}
		if (isNotValuable && notValuableObject.hasHealth)
		{
			notValuableObject.Impact(ImpactState.Medium);
			flag = true;
		}
		if (flag)
		{
			EnemyInvestigate(5f);
		}
		breakLevel2Cooldown = 0.4f;
		breakLevel1Cooldown = 0.3f;
	}

	public void BreakLight(Vector3 contactPoint)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.01f * (1f + 9f * (100f - durability) / 100f);
		bool flag = false;
		if (isValuable || breakLogic)
		{
			float valueLost = 0f;
			if (isValuable)
			{
				valueLost = Mathf.Round(valuableObject.dollarValueOriginal * num);
				valueLost += Mathf.Round(Random.Range((0f - valueLost) * 0.1f, valueLost * 0.1f));
				valueLost = Mathf.Clamp(valueLost, 0f, valuableObject.dollarValueCurrent);
			}
			Break(valueLost, contactPoint, breakLevelLight);
			flag = true;
		}
		if (isNotValuable && notValuableObject.hasHealth)
		{
			notValuableObject.Impact(ImpactState.Light);
			flag = true;
		}
		if (flag)
		{
			EnemyInvestigate(3f);
		}
		breakLevel1Cooldown = 0.3f;
	}

	internal void Break(float valueLost, Vector3 _contactPoint, int breakLevel)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (isValuable && !isIndestructible && !destroyDisable)
		{
			flag = true;
		}
		if (GameManager.instance.gameMode == 0)
		{
			BreakRPC(valueLost, _contactPoint, breakLevel, flag);
			return;
		}
		photonView.RPC("BreakRPC", (RpcTarget)0, new object[4] { valueLost, _contactPoint, breakLevel, flag });
	}

	private void HealLogic(float healAmount, Vector3 healingPoint)
	{
		valuableObject.dollarValueCurrent += Mathf.Floor(healAmount);
		valuableObject.dollarValueCurrent = Mathf.Clamp(valuableObject.dollarValueCurrent, 0f, valuableObject.dollarValueOriginal);
	}

	public float Heal(float healPercent, Vector3 healingPoint)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		float result = 0f;
		if (isValuable)
		{
			if (GameManager.Multiplayer())
			{
				if (PhotonNetwork.IsMasterClient)
				{
					float num = valuableObject.dollarValueOriginal * healPercent;
					num = Mathf.Clamp(num, 0f, valuableObject.dollarValueOriginal - valuableObject.dollarValueCurrent);
					if (num > 0f)
					{
						photonView.RPC("HealRPC", (RpcTarget)0, new object[1] { valuableObject.dollarValueOriginal * healPercent });
					}
					result = num;
				}
			}
			else
			{
				float num2 = valuableObject.dollarValueOriginal * healPercent;
				num2 = Mathf.Clamp(num2, 0f, valuableObject.dollarValueOriginal - valuableObject.dollarValueCurrent);
				if (num2 > 0f)
				{
					HealLogic(num2, healingPoint);
				}
				result = num2;
			}
		}
		return result;
	}

	[PunRPC]
	private void HealRPC(float healAmount, Vector3 healingPoint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		HealLogic(healAmount, healingPoint);
	}

	private void ResetObject()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		foreach (PhysGrabber item in physGrabObject.playerGrabbing.ToList())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				item.ReleaseObject();
				continue;
			}
			item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 0.1f });
		}
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		valuableObject.dollarValueCurrent = valuableObject.dollarValueOriginal;
		rb.position = originalPosition;
		rb.rotation = originalRotation;
		((Component)this).transform.position = originalPosition;
		AssetManager.instance.soundUnequip.Play(originalPosition);
		BreakEffect(breakLevelLight, originalPosition);
		Vector3 val = ((Component)physGrabObject).transform.TransformPoint(physGrabObject.midPointOffset);
		Object.Instantiate<GameObject>(AssetManager.instance.prefabTeleportEffect, val, Quaternion.identity).transform.localScale = Vector3.one * 2f;
	}

	[PunRPC]
	private void BreakRPC(float valueLost, Vector3 _contactPoint, int breakLevel, bool _loseValue)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (_loseValue)
		{
			if (Object.op_Implicit((Object)(object)valuableObject))
			{
				float dollarValueCurrent = valuableObject.dollarValueCurrent;
				valuableObject.dollarValueCurrent -= valueLost;
				bool flag = false;
				if (valuableObject.dollarValueCurrent < valuableObject.dollarValueOriginal * 0.15f)
				{
					if (!SemiFunc.RunIsTutorial())
					{
						DestroyObject();
					}
					else
					{
						if (Object.op_Implicit((Object)(object)particles))
						{
							particles.DestroyParticles();
						}
						ResetObject();
						ImpactHeavy(1000f, _contactPoint);
					}
					flag = true;
				}
				if (flag)
				{
					valueLost = dollarValueCurrent;
				}
			}
			WorldSpaceUIParent.instance.ValueLostCreate(_contactPoint, (int)valueLost);
		}
		onAllBreaks.Invoke();
		if (breakLevel == breakLevelHeavy)
		{
			onBreakHeavy.Invoke();
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				physGrabObject.heavyBreakImpulse = false;
			}
		}
		if (breakLevel == breakLevelMedium)
		{
			onBreakMedium.Invoke();
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				physGrabObject.mediumBreakImpulse = false;
			}
		}
		if (breakLevel == breakLevelLight)
		{
			onBreakLight.Invoke();
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				physGrabObject.lightBreakImpulse = false;
			}
		}
		BreakEffect(breakLevel, _contactPoint);
	}

	public void BreakEffect(int breakLevel, Vector3 contactPoint)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (!particleDisable && Object.op_Implicit((Object)(object)particles))
		{
			particles.ImpactSmoke(5, contactPoint, colliderVolume);
		}
		if (breakLevel == breakLevelHeavy)
		{
			if (audioActive && Object.op_Implicit((Object)(object)impactAudio))
			{
				impactAudio.breakHeavy.Play(contactPoint);
			}
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 10f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel == breakLevelMedium)
		{
			if (audioActive && Object.op_Implicit((Object)(object)impactAudio))
			{
				AudioSource val = impactAudio.breakMedium.Play(contactPoint);
				if (Object.op_Implicit((Object)(object)val))
				{
					val.pitch *= impactAudioPitch;
				}
			}
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 5f);
			}
			GameDirector.instance.CameraImpact.ShakeDistance(3f, 1f, 6f, contactPoint, 0.1f);
		}
		if (breakLevel != breakLevelLight)
		{
			return;
		}
		if (audioActive && Object.op_Implicit((Object)(object)impactAudio))
		{
			AudioSource val2 = impactAudio.breakLight.Play(contactPoint);
			if (Object.op_Implicit((Object)(object)val2))
			{
				val2.pitch *= impactAudioPitch;
			}
		}
		if (Object.op_Implicit((Object)(object)physGrabObject))
		{
			SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 3f);
		}
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 1f, 6f, contactPoint, 0.1f);
	}

	public void ImpactHeavy(float force, Vector3 contactPoint)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			ImpactHeavyRPC(force, contactPoint);
		}
		else
		{
			photonView.RPC("ImpactHeavyRPC", (RpcTarget)0, new object[2] { force, contactPoint });
		}
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactHeavyTimer = 0.1f;
	}

	[PunRPC]
	private void ImpactHeavyRPC(float force, Vector3 contactPoint)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)physGrabObject))
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 8f);
		if (audioActive && !isHinge && Object.op_Implicit((Object)(object)impactAudio))
		{
			float volumeMultiplier = ImpactSoundGetVolume(force, impactAudio.impactHeavy.Volume);
			AudioSource val = impactAudio.impactHeavy.Play(contactPoint, volumeMultiplier);
			if (Object.op_Implicit((Object)(object)val))
			{
				val.pitch *= impactAudioPitch;
			}
		}
		if (!particleDisable && !inCart && Object.op_Implicit((Object)(object)particles))
		{
			particles.ImpactSmoke(5, contactPoint, colliderVolume);
		}
		onAllImpacts.Invoke();
		onImpactHeavy.Invoke();
		EnemyInvestigate(1f);
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactHeavyTimer = 0.1f;
	}

	public void ImpactMedium(float force, Vector3 contactPoint)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			ImpactMediumRPC(force, contactPoint);
		}
		else
		{
			photonView.RPC("ImpactMediumRPC", (RpcTarget)0, new object[2] { force, contactPoint });
		}
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactMediumTimer = 0.1f;
	}

	[PunRPC]
	private void ImpactMediumRPC(float force, Vector3 contactPoint)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)physGrabObject))
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 5f);
		if (audioActive && !isHinge && Object.op_Implicit((Object)(object)impactAudio))
		{
			float volumeMultiplier = ImpactSoundGetVolume(force, impactAudio.impactMedium.Volume);
			AudioSource val = impactAudio.impactMedium.Play(contactPoint, volumeMultiplier);
			if (Object.op_Implicit((Object)(object)val))
			{
				val.pitch *= impactAudioPitch;
			}
		}
		onImpactMedium.Invoke();
		onAllImpacts.Invoke();
		if (!rb.isKinematic)
		{
			Rigidbody obj = rb;
			obj.angularVelocity *= 0.55f;
		}
		EnemyInvestigate(0.5f);
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactMediumTimer = 0.1f;
	}

	private float ImpactSoundGetVolume(float force, float volume)
	{
		float num = Mathf.Clamp01(force * 0.01f);
		if (inCart)
		{
			num *= inCartVolumeMultiplier;
		}
		return Mathf.Clamp(num, 0.1f, 1f);
	}

	public void ImpactLight(float force, Vector3 contactPoint)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			ImpactLightRPC(force, contactPoint);
		}
		else
		{
			photonView.RPC("ImpactLightRPC", (RpcTarget)0, new object[2] { force, contactPoint });
		}
		EnemyInvestigate(0.2f);
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactLightTimer = 0.1f;
	}

	public void changeInCart()
	{
		timerInCart = 0.1f;
	}

	[PunRPC]
	private void ImpactLightRPC(float force, Vector3 contactPoint)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)physGrabObject))
		{
			return;
		}
		SemiFunc.PlayerEyesOverrideSoft(physGrabObject.centerPoint, 1f, ((Component)this).gameObject, 3f);
		if (audioActive && !isHinge && Object.op_Implicit((Object)(object)impactAudio))
		{
			float num = ImpactSoundGetVolume(force, impactAudio.impactLight.Volume);
			if (inCart)
			{
				num *= inCartVolumeMultiplier;
			}
			AudioSource val = impactAudio.impactLight.Play(contactPoint, num);
			if (Object.op_Implicit((Object)(object)val))
			{
				val.pitch *= impactAudioPitch;
			}
		}
		if (!rb.isKinematic)
		{
			Rigidbody obj = rb;
			obj.angularVelocity *= 0.6f;
		}
		onAllImpacts.Invoke();
		onImpactLight.Invoke();
		physGrabObject.impactHappenedTimer = 0.1f;
		physGrabObject.impactLightTimer = 0.1f;
	}

	private void OnTriggerStay(Collider other)
	{
		if ((GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient) && ((Component)other).CompareTag("Cart"))
		{
			currentCart = ((Component)other).GetComponentInParent<PhysGrabCart>();
			if (Object.op_Implicit((Object)(object)currentCart))
			{
				currentCart.physGrabInCart.Add(physGrabObject);
			}
			changeInCart();
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		if ((GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient) || !collisionsActive || !isMoving)
		{
			return;
		}
		isCollidingTimer = 0.1f;
		Vector3 val;
		if (!isHinge && !slidingDisable && !isCart && !inCart && !isCart && isValuable && valuableObject.volumeType >= ValuableVolume.Type.Medium && Object.op_Implicit((Object)(object)collision.gameObject.GetComponent<MaterialSurface>()))
		{
			val = rb.velocity;
			if (((Vector3)(ref val)).magnitude > slidingSpeedThreshold && (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y) || Mathf.Abs(rb.velocity.z) > Mathf.Abs(rb.velocity.y)))
			{
				isSliding = true;
				slidingTimer = 0.1f;
			}
			PhysGrabObject component = collision.gameObject.GetComponent<PhysGrabObject>();
			if (Object.op_Implicit((Object)(object)component))
			{
				val = component.rb.velocity;
				float magnitude = ((Vector3)(ref val)).magnitude;
				val = rb.velocity;
				if (magnitude > ((Vector3)(ref val)).magnitude * 0.8f)
				{
					isSliding = false;
				}
			}
		}
		Vector3 val2 = Vector3.zero;
		ContactPoint[] contacts = collision.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint val3 = contacts[i];
			val2 += ((ContactPoint)(ref val3)).point;
		}
		if (collision.contacts.Length != 0)
		{
			contactPoint = val2 / (float)collision.contacts.Length;
		}
		else
		{
			contactPoint = Vector3.zero;
		}
		PhysGrabObjectImpactDetector component2 = collision.gameObject.GetComponent<PhysGrabObjectImpactDetector>();
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		bool flag3 = false;
		flag3 = isCart && contactPoint.y < ((Component)this).transform.position.y;
		if (impactHappened && (!isCart || !Object.op_Implicit((Object)(object)component2) || !component2.inCart) && !flag3)
		{
			if (impulseTimerDeactivateImpacts <= 0f)
			{
				if (impactForce > 150f && impactHeavyCooldown <= 0f)
				{
					flag2 = true;
					ImpactHeavy(impactForce, contactPoint);
					impactHeavyCooldown = 0.5f;
					impactMediumCooldown = 0.5f;
					impactLightCooldown = 0.5f;
				}
				if (impactForce > 80f && impactMediumCooldown <= 0f)
				{
					flag2 = true;
					ImpactMedium(impactForce, contactPoint);
					impactMediumCooldown = 0.5f;
					impactLightCooldown = 0.5f;
				}
				if (impactForce > 20f && impactLightCooldown <= 0f)
				{
					flag2 = true;
					ImpactLight(impactForce, contactPoint);
					impactLightCooldown = 0.5f;
				}
			}
			if (indestructibleSpawnTimer <= 0f)
			{
				float num2 = Mathf.Max(rb.mass, 1f);
				if (breakForce > impactLevel3 * num2 && breakLevel3Cooldown <= 0f && !inCart)
				{
					flag = true;
					num = 3;
				}
				if (breakForce > impactLevel2 * num2 && breakLevel2Cooldown <= 0f && !flag && !inCart)
				{
					flag = true;
					num = 2;
				}
				if (breakForce > impactLevel1 * num2 && breakLevel1Cooldown <= 0f && !flag && !inCart)
				{
					flag = true;
					num = 1;
				}
			}
		}
		bool flag4 = false;
		bool flag5 = false;
		if (flag && (!isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag2 && (!isEnemy || this.enemyRigidbody.enemy.IsStunned()))
		{
			flag4 = true;
		}
		if (flag && isBrokenHinge)
		{
			flag5 = true;
		}
		if (!canHurtLogic)
		{
			flag4 = false;
		}
		bool flag6 = false;
		if (flag4 && (flag || (flag2 && isCart)))
		{
			bool flag7 = false;
			PlayerTumble playerTumble = null;
			if (((Component)collision.transform).CompareTag("Player"))
			{
				flag7 = true;
			}
			else
			{
				playerTumble = ((Component)collision.transform).GetComponent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)playerTumble))
				{
					flag7 = true;
				}
			}
			if (flag7 && isCart)
			{
				if (physGrabObject.playerGrabbing.Count <= 0)
				{
					flag7 = false;
				}
				else
				{
					Bounds bounds = ((Collider)((Component)cart.inCart).GetComponent<BoxCollider>()).bounds;
					if (((Bounds)(ref bounds)).Contains(collision.transform.position))
					{
						flag7 = false;
					}
				}
			}
			if (flag7)
			{
				PlayerController componentInParent = ((Component)collision.transform).GetComponentInParent<PlayerController>();
				PlayerAvatar playerAvatar;
				if (Object.op_Implicit((Object)(object)playerTumble))
				{
					playerAvatar = playerTumble.playerAvatar;
				}
				else if (Object.op_Implicit((Object)(object)componentInParent))
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
				else
				{
					playerAvatar = ((Component)collision.transform).GetComponentInParent<PlayerAvatar>();
					if (!Object.op_Implicit((Object)(object)playerAvatar))
					{
						playerAvatar = ((Component)collision.transform).GetComponent<PlayerAvatar>();
					}
					if (!Object.op_Implicit((Object)(object)playerAvatar))
					{
						playerAvatar = ((Component)collision.transform).GetComponentInChildren<PlayerAvatar>();
					}
					if (!Object.op_Implicit((Object)(object)playerAvatar))
					{
						PlayerPhysPusher component3 = ((Component)collision.transform).GetComponent<PlayerPhysPusher>();
						if (Object.op_Implicit((Object)(object)component3))
						{
							playerAvatar = component3.Player;
						}
					}
				}
				bool flag8 = false;
				foreach (PhysGrabber item in physGrabObject.playerGrabbing)
				{
					if ((Object)(object)item.playerAvatar == (Object)(object)playerAvatar)
					{
						flag8 = true;
						break;
					}
				}
				if (Object.op_Implicit((Object)(object)playerAvatar) && !flag8)
				{
					Vector3 val4 = ((Component)playerAvatar.PlayerVisionTarget.VisionTransform).transform.position - contactPoint;
					float magnitude2 = ((Vector3)(ref previousPreviousVelocityRaw)).magnitude;
					Vector3 val5 = Vector3.Lerp(((Vector3)(ref previousPreviousVelocityRaw)).normalized, ((Vector3)(ref val4)).normalized, 0f);
					if (magnitude2 >= 3f)
					{
						PlayerAvatar playerAvatar2 = null;
						RaycastHit[] array = rb.SweepTestAll(val5, 1f, (QueryTriggerInteraction)2);
						for (int i = 0; i < array.Length; i++)
						{
							RaycastHit val6 = array[i];
							playerAvatar2 = ImpactGetPlayer(((RaycastHit)(ref val6)).collider, componentInParent, playerTumble);
							if ((Object)(object)playerAvatar2 == (Object)(object)playerAvatar)
							{
								break;
							}
						}
						if (!Object.op_Implicit((Object)(object)playerAvatar2))
						{
							Collider[] array2 = Physics.OverlapSphere(contactPoint, 0.2f, LayerMask.GetMask(new string[1] { "Player" }));
							foreach (Collider hit in array2)
							{
								playerAvatar2 = ImpactGetPlayer(hit, componentInParent, playerTumble);
								if ((Object)(object)playerAvatar2 == (Object)(object)playerAvatar)
								{
									break;
								}
							}
						}
						if ((Object)(object)playerAvatar2 == (Object)(object)playerAvatar)
						{
							bool flag9 = false;
							float time = 0.1f;
							if (!playerHurtDisable && !isIndestructible && !destroyDisable && isValuable)
							{
								time = 0.15f;
								int damage = Mathf.RoundToInt((float)(5 * num) * (rb.mass * 0.5f) * playerHurtMultiplier);
								playerAvatar.playerHealth.HurtOther(damage, contactPoint, savingGrace: true);
								flag9 = true;
							}
							bool flag10 = false;
							if (isHinge)
							{
								if (magnitude2 >= 3f)
								{
									flag10 = true;
								}
							}
							else if (isCart)
							{
								if (magnitude2 >= 3f)
								{
									flag10 = true;
								}
							}
							else if (magnitude2 >= 6f)
							{
								flag10 = true;
							}
							if (flag9 || flag10)
							{
								if (!Object.op_Implicit((Object)(object)playerTumble))
								{
									playerTumble = playerAvatar.tumble;
								}
								playerTumble.TumbleRequest(_isTumbling: true, _playerInput: false);
								playerTumble.TumbleOverrideTime(2f);
								Vector3 force = ((Vector3)(ref val4)).normalized * 4f * (float)num;
								val = playerAvatar.localCameraPosition - contactPoint;
								Vector3 torque = Vector3.Cross(((Vector3)(ref val)).normalized, ((Component)playerAvatar).transform.forward) * 5f * ((Vector3)(ref force)).magnitude;
								playerTumble.physGrabObject.FreezeForces(time, force, torque);
								playerAvatar.playerHealth.HurtFreezeOverride(time);
								physGrabObject.FreezeForces(time, Vector3.zero, Vector3.zero);
								flag6 = true;
							}
						}
					}
				}
			}
		}
		if ((flag4 || flag5) && !playerHurtDisable && enemyInteractionTimer <= 0f && !isIndestructible && !destroyDisable && (isValuable || isEnemy || flag5) && ((Component)collision.transform).CompareTag("Enemy"))
		{
			EnemyRigidbody component4 = ((Component)collision.transform).GetComponent<EnemyRigidbody>();
			if (Object.op_Implicit((Object)(object)component4) && component4.enemy.HasHealth && component4.enemy.Health.objectHurt && component4.enemy.Health.objectHurtDisableTimer <= 0f)
			{
				Vector3 val7 = component4.physGrabObject.centerPoint - contactPoint;
				float magnitude3 = ((Vector3)(ref previousPreviousVelocityRaw)).magnitude;
				Vector3 val8 = Vector3.Lerp(((Vector3)(ref previousPreviousVelocityRaw)).normalized, ((Vector3)(ref val7)).normalized, 0.5f);
				if (magnitude3 > 2f)
				{
					EnemyRigidbody enemyRigidbody = null;
					RaycastHit[] array = rb.SweepTestAll(val8, 1f, (QueryTriggerInteraction)2);
					for (int i = 0; i < array.Length; i++)
					{
						RaycastHit val9 = array[i];
						enemyRigidbody = ((Component)((RaycastHit)(ref val9)).transform).GetComponent<EnemyRigidbody>();
					}
					if (!Object.op_Implicit((Object)(object)enemyRigidbody))
					{
						Collider[] array2 = Physics.OverlapSphere(contactPoint, 0.2f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject()));
						for (int i = 0; i < array2.Length; i++)
						{
							enemyRigidbody = ((Component)array2[i]).GetComponentInParent<EnemyRigidbody>();
						}
					}
					if ((Object)(object)enemyRigidbody == (Object)(object)component4)
					{
						flag6 = true;
						int num3 = Mathf.RoundToInt((float)(10 * num) * (rb.mass * 0.5f));
						num3 = Mathf.RoundToInt((float)num3 * component4.enemy.Health.objectHurtMultiplier);
						component4.enemy.Health.Hurt(num3, -((Vector3)(ref val7)).normalized);
						if (component4.enemy.Health.onObjectHurt != null)
						{
							if (physGrabObject.grabbedTimer > 0f)
							{
								component4.enemy.Health.onObjectHurtPlayer = physGrabObject.lastPlayerGrabbing;
							}
							else
							{
								component4.enemy.Health.onObjectHurtPlayer = null;
							}
							component4.enemy.Health.onObjectHurt.Invoke();
						}
						Vector3 val10 = ((Vector3)(ref val7)).normalized * (2f * (float)num);
						component4.rb.AddForce(val10, (ForceMode)1);
						Vector3 normalized = ((Vector3)(ref val7)).normalized;
						Vector3 val11 = -((Component)component4.rb).transform.up;
						Vector3 val12 = Vector3.Cross(normalized, val11) * (2f * (float)num);
						component4.rb.AddTorque(val12, (ForceMode)1);
						EnemyType type = component4.enemy.Type;
						if (isValuable)
						{
							if (component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								float mass = valuableObject.physAttributePreset.mass;
								bool flag11 = false;
								if (mass >= 2f)
								{
									flag11 = true;
								}
								else if (type <= EnemyType.Medium)
								{
									flag11 = true;
								}
								if (flag11)
								{
									component4.enemy.StateStunned.Set(2f);
								}
							}
						}
						else if (isBrokenHinge)
						{
							if (type <= EnemyType.Medium && component4.enemy.HasStateStunned && component4.enemy.Health.objectHurtStun)
							{
								component4.enemy.StateStunned.Set(2f);
							}
							DestroyObject();
						}
					}
				}
			}
		}
		if (flag6)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				ImpactEffectRPC(contactPoint);
			}
			else
			{
				photonView.RPC("ImpactEffectRPC", (RpcTarget)0, new object[1] { contactPoint });
			}
		}
		if (!flag || !(physGrabObject.overrideDisableBreakEffectsTimer <= 0f))
		{
			return;
		}
		if ((destroyDisable || isIndestructible || !isValuable) && !indestructibleBreakEffects)
		{
			if (!flag2)
			{
				if (num == 1)
				{
					ImpactLight(impactForce, contactPoint);
				}
				if (num == 2)
				{
					ImpactMedium(impactForce, contactPoint);
				}
				if (num == 3)
				{
					ImpactHeavy(impactForce, contactPoint);
				}
			}
		}
		else
		{
			if (num == 1)
			{
				BreakLight(contactPoint);
			}
			if (num == 2)
			{
				BreakMedium(contactPoint);
			}
			if (num == 3)
			{
				BreakHeavy(contactPoint);
			}
		}
	}

	[PunRPC]
	private void ImpactEffectRPC(Vector3 _position)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		AssetManager.instance.PhysImpactEffect(_position);
	}

	private PlayerAvatar ImpactGetPlayer(Collider _hit, PlayerController _playerController, PlayerTumble _playerTumble)
	{
		PlayerAvatar playerAvatar = null;
		if (Object.op_Implicit((Object)(object)_playerTumble))
		{
			PlayerTumble playerTumble = ((Component)((Component)_hit).transform).GetComponent<PlayerTumble>();
			if (!Object.op_Implicit((Object)(object)playerTumble))
			{
				playerTumble = ((Component)((Component)_hit).transform).GetComponentInParent<PlayerTumble>();
			}
			if (Object.op_Implicit((Object)(object)playerTumble))
			{
				return playerTumble.playerAvatar;
			}
		}
		playerAvatar = ((Component)((Component)_hit).transform).GetComponentInParent<PlayerAvatar>();
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = ((Component)((Component)_hit).transform).GetComponent<PlayerAvatar>();
		}
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = ((Component)((Component)_hit).transform).GetComponentInChildren<PlayerAvatar>();
		}
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			PlayerPhysPusher component = ((Component)((Component)_hit).transform).GetComponent<PlayerPhysPusher>();
			if (Object.op_Implicit((Object)(object)component))
			{
				playerAvatar = component.Player;
			}
		}
		if (!Object.op_Implicit((Object)(object)playerAvatar) && Object.op_Implicit((Object)(object)_playerController))
		{
			PlayerController playerController = ((Component)((Component)_hit).transform).GetComponentInParent<PlayerController>();
			if (!Object.op_Implicit((Object)(object)playerController) && Object.op_Implicit((Object)(object)((Component)((Component)_hit).transform).GetComponentInParent<PlayerCollisionController>()))
			{
				playerController = PlayerController.instance;
			}
			if (Object.op_Implicit((Object)(object)playerController))
			{
				playerAvatar = playerController.playerAvatarScript;
			}
		}
		return playerAvatar;
	}

	public void PlayerHurtMultiplier(float _multiplier, float _time)
	{
		playerHurtMultiplier = _multiplier;
		playerHurtMultiplierTimer = _time;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}
}
