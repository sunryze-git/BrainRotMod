using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhysGrabObject : MonoBehaviour, IPunObservable
{
	public bool clientNonKinematic;

	public bool overrideTagsAndLayers = true;

	internal PhotonView photonView;

	internal PhotonTransformView photonTransformView;

	[HideInInspector]
	public Rigidbody rb;

	private bool isMaster;

	internal RoomVolumeCheck roomVolumeCheck;

	internal PhysGrabObjectImpactDetector impactDetector;

	private bool hasImpactDetector;

	internal Vector3 targetPos;

	private float distance;

	internal Quaternion targetRot;

	private float angle;

	internal Vector3 grabDisplacementCurrent;

	[HideInInspector]
	public bool dead;

	[HideInInspector]
	public bool grabbed;

	[HideInInspector]
	public bool grabbedLocal;

	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	[HideInInspector]
	public bool spawned;

	internal PlayerAvatar lastPlayerGrabbing;

	internal float grabbedTimer;

	[HideInInspector]
	public bool lightBreakImpulse;

	[HideInInspector]
	public bool mediumBreakImpulse;

	[HideInInspector]
	public bool heavyBreakImpulse;

	[HideInInspector]
	public bool lightImpactImpulse;

	[HideInInspector]
	public bool mediumImpactImpulse;

	[HideInInspector]
	public bool heavyImpactImpulse;

	[HideInInspector]
	public float enemyInteractTimer;

	internal float angularDragOriginal;

	internal float dragOriginal;

	internal bool isValuable;

	internal bool isEnemy;

	internal bool isPlayer;

	internal bool isMelee;

	internal bool isNonValuable;

	internal bool isKinematic;

	[HideInInspector]
	public float massOriginal;

	private float lastUpdateTime;

	private List<(Vector3 position, double timestamp)> positionBuffer = new List<(Vector3, double)>();

	private List<(Quaternion rotation, double timestamp)> rotationBuffer = new List<(Quaternion, double)>();

	private float gradualLerp;

	private Vector3 prevTargetPos;

	private Quaternion prevTargetRot;

	internal Vector3 rbVelocity = Vector3.zero;

	internal Vector3 rbAngularVelocity = Vector3.zero;

	internal Vector3 currentPosition;

	internal Quaternion currentRotation;

	private bool hasHinge;

	private PhysGrabHinge hinge;

	private float timerZeroGravity;

	private float timerAlterDrag;

	private float alterDragValue;

	private float timerAlterAngularDrag;

	private float alterAngularDragValue;

	private float timerAlterMass;

	private float alterMassValue;

	private float timerAlterMaterial;

	private float timerAlterDeactivate = -123f;

	private float overrideFragilityTimer;

	internal float overrideDisableBreakEffectsTimer;

	private bool isActive = true;

	private PhysicMaterial alterMaterialPrevious;

	private PhysicMaterial alterMaterialCurrent;

	[HideInInspector]
	public Vector3 midPoint;

	[HideInInspector]
	public Vector3 midPointOffset;

	private Vector3 grabRotation;

	private bool isHidden;

	internal float grabDisableTimer;

	internal bool heldByLocalPlayer;

	private CollisionDetectionMode previousCollisionDetectionMode;

	private Camera mainCamera;

	private float timerAlterIndestructible;

	internal Transform forceGrabPoint;

	private MapCustom mapCustom;

	private bool hasMapCustom;

	private bool isCart;

	private PhysGrabCart physGrabCart;

	[HideInInspector]
	public List<Transform> colliders = new List<Transform>();

	[HideInInspector]
	public Vector3 centerPoint;

	public Vector3 camRelForward;

	public Vector3 camRelUp;

	internal bool frozen;

	private float frozenTimer;

	private Vector3 frozenPosition;

	private Quaternion frozenRotation;

	private Vector3 frozenVelocity;

	private Vector3 frozenAngularVelocity;

	private Vector3 frozenForce;

	private Vector3 frozenTorque;

	private float overrideDragGoDownTimer;

	private float overrideAngularDragGoDownTimer;

	private float overrideMassGoDownTimer;

	internal float impactHappenedTimer;

	internal float impactLightTimer;

	internal float impactMediumTimer;

	internal float impactHeavyTimer;

	internal float breakLightTimer;

	internal float breakMediumTimer;

	internal float breakHeavyTimer;

	internal bool hasNeverBeenGrabbed = true;

	[HideInInspector]
	public Vector3 boundingBox;

	internal Vector3 spawnTorque = Vector3.zero;

	private float smoothRotationDelta;

	private bool rbIsSleepingPrevious;

	private float overrideTorqueStrengthX = 1f;

	private float overrideTorqueStrengthXTimer;

	private float overrideTorqueStrengthY = 1f;

	private float overrideTorqueStrengthYTimer;

	private float overrideTorqueStrengthZ = 1f;

	private float overrideTorqueStrengthZTimer;

	private float overrideTorqueStrength = 1f;

	private float overrideTorqueStrengthTimer;

	private float overrideGrabStrength = 1f;

	private float overrideGrabStrengthTimer;

	private float overrideGrabRelativeVerticalPosition;

	private float overrideGrabRelativeVerticalPositionTimer;

	private float overrideGrabRelativeHorizontalPosition;

	private float overrideGrabRelativeHorizontalPositionTimer;

	private void Awake()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		photonTransformView = ((Component)this).GetComponent<PhotonTransformView>();
		if (!Object.op_Implicit((Object)(object)photonTransformView))
		{
			Debug.LogError((object)("No Photon Transform View found on " + ((Object)((Component)this).gameObject).name));
		}
		physGrabCart = ((Component)this).GetComponent<PhysGrabCart>();
		isCart = Object.op_Implicit((Object)(object)physGrabCart);
		forceGrabPoint = ((Component)this).transform.Find("Force Grab Point");
		rb = ((Component)this).GetComponent<Rigidbody>();
		rb.isKinematic = true;
		Transform val = ((Component)this).transform.Find("Center of Mass");
		if (Object.op_Implicit((Object)(object)val))
		{
			rb.centerOfMass = val.localPosition;
		}
		rb.interpolation = (RigidbodyInterpolation)1;
		rb.collisionDetectionMode = (CollisionDetectionMode)2;
		angularDragOriginal = rb.angularDrag;
		dragOriginal = rb.drag;
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		if (Object.op_Implicit((Object)(object)impactDetector))
		{
			hasImpactDetector = true;
		}
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponent<ValuableObject>()))
		{
			isValuable = true;
		}
		mapCustom = ((Component)this).GetComponent<MapCustom>();
		if (Object.op_Implicit((Object)(object)mapCustom))
		{
			hasMapCustom = true;
		}
		Transform[] componentsInParent = ((Component)this).GetComponentsInParent<Transform>();
		foreach (Transform val2 in componentsInParent)
		{
			if (((Object)val2).name.Contains("debug") || ((Object)val2).name.Contains("Debug"))
			{
				spawned = true;
			}
		}
	}

	public void TurnXYZ(Quaternion turnX, Quaternion turnY, Quaternion turnZ)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = turnY * Vector3.forward;
		Vector3 val2 = turnY * Vector3.up;
		val = turnZ * val;
		val2 = turnZ * val2;
		foreach (PhysGrabber item in playerGrabbing)
		{
			item.cameraRelativeGrabbedForward = turnX * val;
			item.cameraRelativeGrabbedUp = turnX * val2;
		}
	}

	public void TorqueToTarget(PhysGrabber player, Quaternion target, float strength, float dampen)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!rb.isKinematic)
		{
			Vector3 val = Vector3.zero;
			Vector3 forward = ((Component)this).transform.forward;
			Vector3 up = ((Component)this).transform.up;
			Vector3 val2 = target * Vector3.forward;
			Vector3 val3 = (player.cameraRelativeGrabbedUp = target * Vector3.up);
			player.cameraRelativeGrabbedForward = val2;
			Vector3 val4 = Vector3.Cross(forward, val2);
			if (((Vector3)(ref val4)).sqrMagnitude > 1E-08f)
			{
				float num = Vector3.Angle(forward, val2);
				val += ((Vector3)(ref val4)).normalized * Mathf.Clamp(num, 0f, 60f);
			}
			Vector3 val5 = Vector3.Cross(up, val3);
			if (((Vector3)(ref val5)).sqrMagnitude > 1E-08f)
			{
				float num2 = Vector3.Angle(up, val3);
				val += ((Vector3)(ref val5)).normalized * Mathf.Clamp(num2, 0f, 60f);
			}
			val *= rb.mass;
			Vector3 val6 = Vector3.ClampMagnitude(val, 60f);
			val = ((Vector3)(ref val6)).normalized;
			if (rb.mass < 1f)
			{
				val *= 0.75f;
			}
			float num3 = Vector3.Angle(((Component)this).transform.forward, val2) / 180f;
			float num4 = Vector3.Angle(((Component)this).transform.up, val3) / 180f;
			float num5 = Mathf.Clamp01(rb.mass);
			float num6 = (num3 + num4) * dampen * num5;
			val *= strength;
			val *= num6;
			Vector3 val7 = val * num5;
			rb.AddTorque(val7, (ForceMode)1);
		}
	}

	private void Start()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Expected O, but got Unknown
		if (!SemiFunc.IsMultiplayer() && Object.op_Implicit((Object)(object)photonTransformView))
		{
			((Behaviour)photonTransformView).enabled = false;
			photonTransformView = null;
		}
		mainCamera = Camera.main;
		isEnemy = Object.op_Implicit((Object)(object)((Component)this).GetComponent<EnemyRigidbody>());
		isMelee = Object.op_Implicit((Object)(object)((Component)this).GetComponent<ItemMelee>());
		if (!isEnemy)
		{
			isNonValuable = Object.op_Implicit((Object)(object)((Component)this).GetComponent<NotValuableObject>());
		}
		Quaternion rotation = ((Component)this).transform.rotation;
		((Component)this).transform.rotation = Quaternion.identity;
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(Vector3.zero, Vector3.zero);
		Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
		bool flag = false;
		Collider[] array = componentsInChildren;
		foreach (Collider val in array)
		{
			if (!val.isTrigger)
			{
				if (flag)
				{
					((Bounds)(ref bounds)).Encapsulate(val.bounds);
					continue;
				}
				bounds = val.bounds;
				flag = true;
			}
		}
		((Component)this).transform.rotation = rotation;
		if (flag)
		{
			boundingBox = ((Bounds)(ref bounds)).size;
			midPointOffset = ((Component)this).transform.InverseTransformPoint(((Bounds)(ref bounds)).center);
		}
		else
		{
			boundingBox = Vector3.one;
			Debug.LogWarning((object)"No colliders found on the object or its children!");
		}
		int num = 0;
		PhysGrabObjectCollider[] componentsInChildren2 = ((Component)this).GetComponentsInChildren<PhysGrabObjectCollider>();
		foreach (PhysGrabObjectCollider physGrabObjectCollider in componentsInChildren2)
		{
			colliders.Add(((Component)physGrabObjectCollider).transform);
			physGrabObjectCollider.colliderID = num;
			num++;
		}
		roomVolumeCheck = ((Component)this).GetComponent<RoomVolumeCheck>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		hinge = ((Component)this).GetComponent<PhysGrabHinge>();
		if (Object.op_Implicit((Object)(object)hinge))
		{
			hasHinge = true;
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				prevTargetPos = ((Component)this).transform.position;
				prevTargetRot = ((Component)this).transform.rotation;
				targetPos = ((Component)this).transform.position;
				targetRot = ((Component)this).transform.rotation;
				isMaster = true;
			}
			if (PhotonNetwork.IsMasterClient && spawned)
			{
				photonView.TransferOwnership(PhotonNetwork.MasterClient);
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (!Object.op_Implicit((Object)(object)((Component)this).GetComponent<EnemyRigidbody>()))
			{
				((MonoBehaviour)this).StartCoroutine(EnableRigidbody());
			}
		}
		else
		{
			rb.collisionDetectionMode = (CollisionDetectionMode)0;
		}
		if (!overrideTagsAndLayers)
		{
			return;
		}
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val2 = item;
			if (!((Component)val2).CompareTag("Cart") && !((Component)val2).CompareTag("Grab Area"))
			{
				if (((Component)val2).gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("Triggers"))
				{
					((Component)val2).gameObject.tag = "Phys Grab Object";
				}
				if (((Component)val2).gameObject.layer != LayerMask.NameToLayer("IgnorePhysGrab") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("CollisionCheck") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("CartWheels") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectHinge") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("PhysGrabObjectCart") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("Triggers") && ((Component)val2).gameObject.layer != LayerMask.NameToLayer("PlayerOnlyCollision"))
				{
					((Component)val2).gameObject.layer = LayerMask.NameToLayer("PhysGrabObject");
				}
			}
		}
	}

	public void OverrideFragility(float multiplier)
	{
		if (Object.op_Implicit((Object)(object)impactDetector) && isValuable)
		{
			overrideFragilityTimer = 0.1f;
			impactDetector.fragilityMultiplier = multiplier;
		}
	}

	private void OverrideTimersTick()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (timerAlterDeactivate > 0f)
		{
			if (isActive)
			{
				((Component)this).transform.position = new Vector3(0f, 3000f, 0f);
			}
			isActive = false;
			rb.detectCollisions = false;
			rb.isKinematic = true;
			if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel() && ((Behaviour)photonTransformView).enabled)
			{
				((Behaviour)photonTransformView).enabled = false;
			}
			timerAlterDeactivate -= Time.fixedDeltaTime;
		}
		else if (timerAlterDeactivate != -123f)
		{
			OverrideDeactivateReset();
		}
		if (Object.op_Implicit((Object)(object)mapCustom) && hasMapCustom && !isActive)
		{
			mapCustom.Hide();
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		OverrideStrengthTick();
		if (overrideMassGoDownTimer > 0f)
		{
			overrideMassGoDownTimer -= Time.fixedDeltaTime;
		}
		if (timerAlterMass > 0f)
		{
			rb.mass = alterMassValue;
			timerAlterMass -= Time.fixedDeltaTime;
		}
		else if (timerAlterMass != -123f)
		{
			if (massOriginal == 0f)
			{
				massOriginal = rb.mass;
			}
			ResetMass();
		}
		if (Object.op_Implicit((Object)(object)impactDetector))
		{
			if (overrideFragilityTimer > 0f)
			{
				overrideFragilityTimer -= Time.fixedDeltaTime;
			}
			else if (overrideFragilityTimer != -123f)
			{
				if (impactDetector.fragilityMultiplier != 1f)
				{
					impactDetector.fragilityMultiplier = 1f;
				}
				overrideFragilityTimer = -123f;
			}
		}
		if (overrideAngularDragGoDownTimer > 0f)
		{
			overrideAngularDragGoDownTimer -= Time.fixedDeltaTime;
		}
		if (timerAlterAngularDrag > 0f)
		{
			rb.angularDrag = alterAngularDragValue;
			timerAlterAngularDrag -= Time.fixedDeltaTime;
		}
		else if (timerAlterAngularDrag != -123f)
		{
			rb.angularDrag = angularDragOriginal;
			timerAlterAngularDrag = -123f;
			alterAngularDragValue = 0f;
		}
		if (overrideDragGoDownTimer > 0f)
		{
			overrideDragGoDownTimer -= Time.fixedDeltaTime;
		}
		if (timerAlterDrag > 0f)
		{
			rb.drag = alterDragValue;
			timerAlterDrag -= Time.fixedDeltaTime;
		}
		else if (timerAlterDrag != -123f)
		{
			rb.drag = dragOriginal;
			timerAlterDrag = -123f;
			alterDragValue = 0f;
		}
		if (timerAlterIndestructible > 0f)
		{
			if (Object.op_Implicit((Object)(object)impactDetector))
			{
				impactDetector.isIndestructible = true;
			}
			timerAlterIndestructible -= Time.fixedDeltaTime;
		}
		else if (timerAlterIndestructible != -123f)
		{
			ResetIndestructible();
		}
		if (timerAlterMaterial > 0f)
		{
			timerAlterMaterial -= Time.fixedDeltaTime;
		}
		else if (timerAlterMaterial != -123f)
		{
			foreach (Transform collider in colliders)
			{
				if (Object.op_Implicit((Object)(object)collider))
				{
					((Component)collider).GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
				}
				else
				{
					colliders.Remove(collider);
				}
			}
			timerAlterMaterial = -123f;
			alterMaterialCurrent = null;
		}
		if (timerZeroGravity > 0f)
		{
			rb.useGravity = false;
			timerZeroGravity -= Time.fixedDeltaTime;
		}
		else if (timerZeroGravity != -123f)
		{
			rb.useGravity = true;
			timerZeroGravity = -123f;
		}
		if ((hasHinge && !hinge.dead && !hinge.broken) || !rb.useGravity)
		{
			return;
		}
		if (grabbed)
		{
			if (timerAlterAngularDrag <= 0f)
			{
				rb.angularDrag = 0.5f;
			}
			if (timerAlterDrag <= 0f)
			{
				rb.drag = 0.5f;
			}
		}
		else
		{
			if (timerAlterAngularDrag <= 0f)
			{
				rb.angularDrag = angularDragOriginal;
			}
			if (timerAlterDrag <= 0f)
			{
				rb.drag = dragOriginal;
			}
		}
	}

	private IEnumerator EnableRigidbody()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(0.1f);
		spawned = true;
		rb.isKinematic = false;
		if (spawnTorque != Vector3.zero)
		{
			rb.AddTorque(spawnTorque, (ForceMode)1);
		}
	}

	private void TickImpactTimers()
	{
		if (impactHappenedTimer > 0f)
		{
			impactHappenedTimer -= Time.fixedDeltaTime;
		}
		if (impactLightTimer > 0f)
		{
			impactLightTimer -= Time.fixedDeltaTime;
		}
		if (impactMediumTimer > 0f)
		{
			impactMediumTimer -= Time.fixedDeltaTime;
		}
		if (impactHeavyTimer > 0f)
		{
			impactHeavyTimer -= Time.fixedDeltaTime;
		}
		if (breakLightTimer > 0f)
		{
			breakLightTimer -= Time.fixedDeltaTime;
		}
		if (breakMediumTimer > 0f)
		{
			breakMediumTimer -= Time.fixedDeltaTime;
		}
		if (breakHeavyTimer > 0f)
		{
			breakHeavyTimer -= Time.fixedDeltaTime;
		}
	}

	public void OverrideGrabStrength(float value, float time = 0.1f)
	{
		overrideGrabStrengthTimer = time;
		overrideGrabStrength = value;
	}

	public void OverrideTorqueStrengthX(float value, float time = 0.1f)
	{
		overrideTorqueStrengthXTimer = time;
		overrideTorqueStrengthX = value;
	}

	public void OverrideTorqueStrengthY(float value, float time = 0.1f)
	{
		overrideTorqueStrengthYTimer = time;
		overrideTorqueStrengthY = value;
	}

	public void OverrideTorqueStrengthZ(float value, float time = 0.1f)
	{
		overrideTorqueStrengthZTimer = time;
		overrideTorqueStrengthZ = value;
	}

	public void OverrideTorqueStrength(float value, float time = 0.1f)
	{
		overrideTorqueStrengthTimer = time;
		overrideTorqueStrength = value;
	}

	public void OverrideStrengthTick()
	{
		if (overrideTorqueStrengthXTimer > 0f)
		{
			overrideTorqueStrengthXTimer -= Time.fixedDeltaTime;
			if (overrideTorqueStrengthXTimer <= 0f)
			{
				overrideTorqueStrengthX = 1f;
			}
		}
		if (overrideTorqueStrengthYTimer > 0f)
		{
			overrideTorqueStrengthYTimer -= Time.fixedDeltaTime;
			if (overrideTorqueStrengthYTimer <= 0f)
			{
				overrideTorqueStrengthY = 1f;
			}
		}
		if (overrideTorqueStrengthZTimer > 0f)
		{
			overrideTorqueStrengthZTimer -= Time.fixedDeltaTime;
			if (overrideTorqueStrengthZTimer <= 0f)
			{
				overrideTorqueStrengthZ = 1f;
			}
		}
		if (overrideTorqueStrengthTimer > 0f)
		{
			overrideTorqueStrengthTimer -= Time.fixedDeltaTime;
			if (overrideTorqueStrengthTimer <= 0f)
			{
				overrideTorqueStrength = 1f;
			}
		}
		if (overrideGrabStrengthTimer > 0f)
		{
			overrideGrabStrengthTimer -= Time.fixedDeltaTime;
			if (overrideGrabStrengthTimer <= 0f)
			{
				overrideGrabStrength = 1f;
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0872: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		TickImpactTimers();
		OverrideTimersTick();
		OverrideGrabRelativePositionTick();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			rbVelocity = rb.velocity;
			rbAngularVelocity = rb.angularVelocity;
			isKinematic = rb.isKinematic;
			if (!isKinematic)
			{
				float num = 40f;
				float num2 = 30f;
				rb.velocity = Vector3.ClampMagnitude(rb.velocity, num);
				rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, num2);
			}
		}
		if (frozenTimer > 0f)
		{
			frozenTimer -= Time.fixedDeltaTime;
			rb.MovePosition(frozenPosition);
			rb.MoveRotation(frozenRotation);
			if (!rb.isKinematic)
			{
				rb.velocity = Vector3.zero;
				rbVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rbAngularVelocity = Vector3.zero;
			}
			return;
		}
		if (frozen)
		{
			rb.AddForce(frozenVelocity, (ForceMode)2);
			rb.AddTorque(frozenAngularVelocity, (ForceMode)2);
			rb.AddForce(frozenForce, (ForceMode)1);
			rb.AddTorque(frozenTorque, (ForceMode)1);
			frozenForce = Vector3.zero;
			frozenTorque = Vector3.zero;
			frozen = false;
			return;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			rbVelocity = rb.velocity;
			rbAngularVelocity = rb.angularVelocity;
		}
		if (playerGrabbing.Count > 0)
		{
			if (hasNeverBeenGrabbed)
			{
				OverrideIndestructible(0.5f);
				hasNeverBeenGrabbed = false;
			}
			grabbed = true;
			heldByLocalPlayer = false;
			if (GameManager.Multiplayer())
			{
				foreach (PhysGrabber item in playerGrabbing)
				{
					if (item.photonView.IsMine)
					{
						heldByLocalPlayer = true;
					}
				}
			}
			else
			{
				heldByLocalPlayer = true;
			}
		}
		else
		{
			heldByLocalPlayer = false;
			grabbed = false;
		}
		if ((GameManager.Multiplayer() && !isMaster) || rb.isKinematic)
		{
			return;
		}
		Vector3 val = Vector3.zero;
		grabDisplacementCurrent = Vector3.zero;
		foreach (PhysGrabber item2 in playerGrabbing)
		{
			float num3 = item2.forceMax;
			Bounds bounds;
			if (isCart)
			{
				bounds = ((Collider)((Component)physGrabCart.inCart).GetComponent<BoxCollider>()).bounds;
				if (((Bounds)(ref bounds)).Contains(((Component)item2).transform.position))
				{
					num3 *= 0.25f;
				}
			}
			item2.grabbedPhysGrabObject = this;
			if (item2.physGrabForcesDisabled)
			{
				continue;
			}
			Vector3 val2 = item2.physGrabPointPullerPosition;
			if (overrideGrabRelativeVerticalPositionTimer != 0f)
			{
				Vector3 up = item2.playerAvatar.localCameraTransform.up;
				val2 += up * overrideGrabRelativeVerticalPosition;
			}
			if (overrideGrabRelativeHorizontalPositionTimer != 0f)
			{
				Vector3 right = item2.playerAvatar.localCameraTransform.right;
				val2 += right * overrideGrabRelativeHorizontalPosition;
			}
			Vector3 val3 = Vector3.ClampMagnitude(val2 - item2.physGrabPoint.position, num3) * 10f;
			val3 = Vector3.ClampMagnitude(val3, num3);
			Vector3 pointVelocity = rb.GetPointVelocity(item2.physGrabPoint.position);
			Vector3 val4 = val3 * item2.springConstant - pointVelocity * item2.dampingConstant;
			val4 = Vector3.ClampMagnitude(val4, num3) * 2f;
			if (isMelee)
			{
				val4 = Vector3.ClampMagnitude(val4, num3) * 4f;
			}
			val4 *= item2.grabStrength;
			val4 *= overrideGrabStrength;
			Vector3 val5 = val4 * item2.forceConstant;
			if (hasHinge && !hinge.dead && !hinge.broken)
			{
				val5 *= 2f;
			}
			foreach (PhysGrabObject physGrabObject in item2.playerAvatar.physObjectStander.physGrabObjects)
			{
				if ((Object)(object)physGrabObject == (Object)(object)this && val5.y > 0f)
				{
					val5.y = 0f;
				}
			}
			if (isCart)
			{
				bounds = ((Collider)((Component)physGrabCart.inCart).GetComponent<BoxCollider>()).bounds;
				if (((Bounds)(ref bounds)).Contains(((Component)item2.playerAvatar).transform.position) && val5.y > 0f)
				{
					val5.y = 0f;
				}
			}
			rb.AddForceAtPosition(val5, item2.physGrabPoint.position);
			grabDisplacementCurrent += val3 * item2.grabStrength;
			if (hasHinge && !hinge.dead && !hinge.broken)
			{
				continue;
			}
			Transform localCameraTransform = item2.playerAvatar.localCameraTransform;
			Vector3 val6 = localCameraTransform.TransformDirection(item2.physRotation * item2.cameraRelativeGrabbedForward);
			Vector3 val7 = localCameraTransform.TransformDirection(item2.physRotation * item2.cameraRelativeGrabbedUp);
			Vector3 forward = ((Component)this).transform.forward;
			Vector3 up2 = ((Component)this).transform.up;
			Vector3 val8 = Vector3.zero;
			Vector3 val9 = Vector3.Cross(forward, val6);
			if (((Vector3)(ref val9)).sqrMagnitude > 1E-08f)
			{
				val8 += ((Vector3)(ref val9)).normalized * Mathf.Clamp(Vector3.Angle(forward, val6), 0f, 60f);
			}
			Vector3 val10 = Vector3.Cross(up2, val7);
			if (((Vector3)(ref val10)).sqrMagnitude > 1E-08f)
			{
				val8 += ((Vector3)(ref val10)).normalized * Mathf.Clamp(Vector3.Angle(up2, val7), 0f, 60f);
			}
			val8 *= rb.mass;
			Vector3 val11 = Vector3.ClampMagnitude(val8, 60f);
			val8 = ((Vector3)(ref val11)).normalized;
			if (rb.mass < 1f)
			{
				val8 *= 0.75f;
			}
			if (isMelee && item2.grabStrength > 0f)
			{
				OverrideMass(massOriginal + item2.grabStrength * 0.2f);
			}
			if (isMelee)
			{
				Vector3 val12 = Vector3.zero;
				Vector3 val13 = Vector3.Cross(((Component)this).transform.forward, val6);
				if (((Vector3)(ref val13)).sqrMagnitude > 1E-08f)
				{
					val12 = ((Vector3)(ref val13)).normalized * Vector3.Angle(((Component)this).transform.forward, val6);
				}
				Vector3 val14 = Vector3.zero;
				Vector3 val15 = Vector3.Cross(((Component)this).transform.up, val7);
				if (((Vector3)(ref val15)).sqrMagnitude > 1E-08f)
				{
					val14 = ((Vector3)(ref val15)).normalized * Vector3.Angle(((Component)this).transform.up, val7);
				}
				val8 = ((Vector3)(ref val12)).normalized + val14 * 3f;
				float num4 = 0.8f;
				val8 *= num4;
			}
			float num5 = Mathf.Clamp01(rb.mass);
			num5 = Mathf.Max(num5, 1f);
			if (massOriginal > 2f)
			{
				num5 *= item2.grabStrength;
			}
			float num6 = 10f;
			if (((Vector3)(ref item2.mouseTurningVelocity)).magnitude > 0.1f && massOriginal > 1f)
			{
				float num7 = Mathf.Max(rb.mass, 0.1f);
				num6 = 2f / num7;
				float num8 = 1f + ((Vector3)(ref boundingBox)).magnitude;
				num6 += num8;
				if (num6 < 1f)
				{
					num6 = 1f;
				}
				if (num6 > 10f)
				{
					num6 = 10f;
				}
				val8 *= num6;
			}
			float num9 = Mathf.Clamp01(rb.mass);
			if (rb.mass > 1f)
			{
				num9 *= 0.9f;
			}
			val8 = (isMelee ? (val8 * 0.05f) : (val8 * 5f));
			val8 *= overrideTorqueStrength;
			float num10 = Vector3.Angle(((Component)this).transform.forward, val6) / 180f;
			float num11 = Vector3.Angle(((Component)this).transform.up, val7) / 180f;
			float num12 = num10 + num11;
			val8 *= num12 * 15f * num9 * Time.fixedDeltaTime;
			Vector3 val16 = ((Component)this).transform.InverseTransformDirection(val8);
			float num13 = overrideTorqueStrengthX;
			float num14 = overrideTorqueStrengthY;
			float num15 = overrideTorqueStrengthZ;
			if (num13 > 1f)
			{
				num13 *= num12;
			}
			if (num14 > 1f)
			{
				num14 *= num12;
			}
			if (num15 > 1f)
			{
				num15 *= num12;
			}
			val16.x *= num13;
			val16.y *= num14;
			val16.z *= num15;
			val8 = ((Component)this).transform.TransformDirection(val16);
			Vector3 val17 = val8 * num9;
			val += val17;
			Rigidbody obj = rb;
			obj.angularVelocity *= 0.9f;
		}
		if (((Vector3)(ref val)).magnitude > 0f)
		{
			rb.AddTorque(val, (ForceMode)1);
		}
	}

	public void OverrideGrabVerticalPosition(float pos)
	{
		overrideGrabRelativeVerticalPosition = pos;
		overrideGrabRelativeVerticalPositionTimer = 0.1f;
	}

	public void OverrideGrabHorizontalPosition(float pos)
	{
		overrideGrabRelativeHorizontalPosition = pos;
		overrideGrabRelativeHorizontalPositionTimer = 0.1f;
	}

	private void OverrideGrabRelativePositionTick()
	{
		if (overrideGrabRelativeHorizontalPositionTimer > 0f)
		{
			overrideGrabRelativeHorizontalPositionTimer -= Time.deltaTime;
			if (overrideGrabRelativeHorizontalPositionTimer <= 0f)
			{
				overrideGrabRelativeHorizontalPosition = 0f;
			}
		}
		if (overrideGrabRelativeVerticalPositionTimer > 0f)
		{
			overrideGrabRelativeVerticalPositionTimer -= Time.deltaTime;
			if (overrideGrabRelativeVerticalPositionTimer <= 0f)
			{
				overrideGrabRelativeVerticalPosition = 0f;
			}
		}
	}

	public void OverrideZeroGravity(float time = 0.1f)
	{
		timerZeroGravity = time;
	}

	public void OverrideDrag(float value, float time = 0.1f)
	{
		timerAlterDrag = time;
		if (alterDragValue <= value)
		{
			alterDragValue = value;
			overrideDragGoDownTimer = 0.1f;
		}
		else if (overrideDragGoDownTimer <= 0f)
		{
			alterDragValue = value;
		}
	}

	public void OverrideAngularDrag(float value, float time = 0.1f)
	{
		timerAlterAngularDrag = time;
		if (alterAngularDragValue <= value)
		{
			alterAngularDragValue = value;
			overrideAngularDragGoDownTimer = 0.1f;
		}
		else if (overrideAngularDragGoDownTimer <= 0f)
		{
			timerAlterAngularDrag = value;
		}
	}

	public void OverrideIndestructible(float time = 0.1f)
	{
		timerAlterIndestructible = time;
	}

	public void OverrideDeactivate(float time = 0.1f)
	{
		timerAlterDeactivate = time;
		rb.isKinematic = true;
	}

	public void OverrideDeactivateReset()
	{
		isActive = true;
		rb.detectCollisions = true;
		if (spawned)
		{
			rb.isKinematic = false;
		}
		if (SemiFunc.IsMultiplayer() && !SemiFunc.MenuLevel())
		{
			((Behaviour)photonTransformView).enabled = true;
		}
		timerAlterDeactivate = -123f;
	}

	public void OverrideBreakEffects(float _time)
	{
		overrideDisableBreakEffectsTimer = _time;
	}

	public void OverrideMaterial(PhysicMaterial material, float time = 0.1f)
	{
		if ((Object)(object)alterMaterialCurrent != (Object)(object)alterMaterialPrevious || (Object)(object)alterMaterialCurrent == (Object)null)
		{
			alterMaterialPrevious = alterMaterialCurrent;
			foreach (Transform collider in colliders)
			{
				if (Object.op_Implicit((Object)(object)collider))
				{
					Collider component = ((Component)collider).GetComponent<Collider>();
					if (Object.op_Implicit((Object)(object)component))
					{
						component.material = material;
					}
				}
				else
				{
					colliders.Remove(collider);
				}
			}
		}
		alterMaterialCurrent = material;
		timerAlterMaterial = time;
	}

	public void ResetIndestructible()
	{
		if (Object.op_Implicit((Object)(object)impactDetector))
		{
			impactDetector.isIndestructible = false;
		}
		timerAlterIndestructible = -123f;
	}

	public void SetPositionLogic(Vector3 _position, Quaternion _rotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		photonTransformView.Teleport(_position, _rotation);
	}

	[PunRPC]
	private void SetPositionRPC(Vector3 position, Quaternion rotation)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer())
		{
			SetPositionLogic(position, rotation);
			return;
		}
		((Component)this).transform.position = position;
		((Component)this).transform.rotation = rotation;
		rb.position = position;
		rb.rotation = rotation;
	}

	public void Teleport(Vector3 position, Quaternion rotation)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonTransformView.Teleport(position, rotation);
				return;
			}
			photonView.RPC("SetPositionRPC", (RpcTarget)2, new object[2] { position, rotation });
		}
		else
		{
			((Component)this).transform.position = position;
			((Component)this).transform.rotation = rotation;
			rb.position = position;
			rb.rotation = rotation;
		}
	}

	public void OverrideMass(float value, float time = 0.1f)
	{
		timerAlterMass = time;
		if (alterMassValue <= value)
		{
			alterMassValue = value;
			overrideMassGoDownTimer = 0.1f;
		}
		else if (overrideMassGoDownTimer <= 0f)
		{
			alterMassValue = value;
		}
	}

	public void ResetMass()
	{
		rb.mass = massOriginal;
		timerAlterMass = -123f;
		alterMassValue = 0f;
	}

	private void Update()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		if (grabbed)
		{
			for (int i = 0; i < playerGrabbing.Count; i++)
			{
				if (!Object.op_Implicit((Object)(object)playerGrabbing[i]))
				{
					playerGrabbing.RemoveAt(i);
				}
			}
		}
		midPoint = ((Component)this).transform.TransformPoint(midPointOffset);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (playerGrabbing.Count > 0)
			{
				lastPlayerGrabbing = playerGrabbing[playerGrabbing.Count - 1].playerAvatar;
				grabbedTimer = 0.5f;
			}
			else if (Object.op_Implicit((Object)(object)lastPlayerGrabbing))
			{
				grabbedTimer -= Time.deltaTime;
				if (grabbedTimer <= 0f)
				{
					lastPlayerGrabbing = null;
				}
			}
			if (enemyInteractTimer > 0f)
			{
				enemyInteractTimer -= Time.deltaTime;
				if (playerGrabbing.Count > 0)
				{
					enemyInteractTimer = 0f;
				}
			}
			if (hasImpactDetector && !impactDetector.isIndestructible)
			{
				if (heavyImpactImpulse)
				{
					impactDetector.ImpactHeavy(150f, centerPoint);
					heavyImpactImpulse = false;
				}
				if (mediumImpactImpulse)
				{
					impactDetector.ImpactMedium(80f, centerPoint);
					mediumImpactImpulse = false;
				}
				if (lightImpactImpulse)
				{
					impactDetector.ImpactLight(20f, centerPoint);
					lightImpactImpulse = false;
				}
				if (heavyBreakImpulse)
				{
					if (isValuable)
					{
						impactDetector.BreakHeavy(centerPoint);
					}
					else
					{
						impactDetector.Break(0f, centerPoint, impactDetector.breakLevelHeavy);
					}
					heavyBreakImpulse = false;
				}
				if (mediumBreakImpulse)
				{
					if (isValuable)
					{
						impactDetector.BreakMedium(centerPoint);
					}
					else
					{
						impactDetector.Break(0f, centerPoint, impactDetector.breakLevelMedium);
					}
					mediumBreakImpulse = false;
				}
				if (lightBreakImpulse)
				{
					if (isValuable)
					{
						impactDetector.BreakLight(centerPoint);
					}
					else
					{
						impactDetector.Break(0f, centerPoint, impactDetector.breakLevelLight);
					}
					lightBreakImpulse = false;
				}
			}
			if (overrideDisableBreakEffectsTimer > 0f)
			{
				overrideDisableBreakEffectsTimer -= Time.deltaTime;
			}
			if (dead && playerGrabbing.Count == 0)
			{
				DestroyPhysGrabObject();
			}
		}
		if (grabDisableTimer > 0f)
		{
			grabDisableTimer -= Time.deltaTime;
		}
		centerPoint = midPoint;
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !(((Component)this).transform.position.y < -50f))
		{
			return;
		}
		if (impactDetector.destroyDisable)
		{
			if (impactDetector.destroyDisableTeleport)
			{
				Teleport(((Component)TruckSafetySpawnPoint.instance).transform.position, ((Component)TruckSafetySpawnPoint.instance).transform.rotation);
			}
		}
		else
		{
			impactDetector.DestroyObject();
		}
	}

	public void EnemyInteractTimeSet()
	{
		enemyInteractTimer = 10f;
	}

	public void FreezeForces(float _time, Vector3 _force, Vector3 _torque)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!rb.isKinematic)
		{
			frozenTimer = _time;
			if (!frozen)
			{
				frozenPosition = ((Component)this).transform.position;
				frozenRotation = ((Component)this).transform.rotation;
				frozenVelocity = rb.velocity;
				frozenAngularVelocity = rb.angularVelocity;
				frozenForce = Vector3.zero;
				frozenTorque = Vector3.zero;
				frozen = true;
			}
			frozenForce += _force;
			frozenTorque += _torque;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	private void OnDestroy()
	{
		if (RoundDirector.instance.dollarHaulList.Contains(((Component)this).gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(((Component)this).gameObject);
		}
	}

	public void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			DestroyPhysGrabObjectRPC();
		}
		else
		{
			photonView.RPC("DestroyPhysGrabObjectRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void OnDisable()
	{
		RoundDirector.instance.PhysGrabObjectRemove(this);
	}

	private void OnEnable()
	{
		RoundDirector.instance.PhysGrabObjectAdd(this);
	}

	public void GrabStarted(PhysGrabber player)
	{
		if (grabbedLocal)
		{
			return;
		}
		grabbedLocal = true;
		if (GameManager.instance.gameMode == 0)
		{
			if (!playerGrabbing.Contains(player))
			{
				playerGrabbing.Add(player);
			}
		}
		else
		{
			photonView.RPC("GrabStartedRPC", (RpcTarget)2, new object[1] { player.photonView.ViewID });
		}
	}

	public void GrabEnded(PhysGrabber player)
	{
		if (!grabbedLocal)
		{
			return;
		}
		grabbedLocal = false;
		if (GameManager.instance.gameMode == 0)
		{
			Throw(player);
			if (playerGrabbing.Contains(player))
			{
				playerGrabbing.Remove(player);
			}
		}
		else
		{
			photonView.RPC("GrabEndedRPC", (RpcTarget)2, new object[1] { player.photonView.ViewID });
		}
	}

	public void GrabLink(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		photonView.RPC("GrabLinkRPC", (RpcTarget)0, new object[5] { playerPhotonID, colliderID, point, cameraRelativeGrabbedForward, cameraRelativeGrabbedUp });
	}

	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, int colliderID, Vector3 point, Vector3 cameraRelativeGrabbedForward, Vector3 cameraRelativeGrabbedUp)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = ((Component)this).transform.InverseTransformPoint(point);
		component.grabbedObjectTransform = ((Component)this).transform;
		component.grabbedPhysGrabObjectColliderID = colliderID;
		component.grabbedPhysGrabObjectCollider = ((Component)FindColliderFromID(colliderID)).GetComponent<Collider>();
		component.grabbed = true;
		Transform localCameraTransform = component.playerAvatar.localCameraTransform;
		if (playerGrabbing.Count != 0)
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(((Component)this).transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(((Component)this).transform.up);
		}
		else
		{
			component.cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(((Component)this).transform.forward);
			component.cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(((Component)this).transform.up);
			camRelForward = ((Component)this).transform.InverseTransformDirection(((Component)this).transform.forward);
			camRelUp = ((Component)this).transform.InverseTransformDirection(((Component)this).transform.up);
		}
		component.cameraRelativeGrabbedForward = ((Vector3)(ref component.cameraRelativeGrabbedForward)).normalized;
		component.cameraRelativeGrabbedUp = ((Vector3)(ref component.cameraRelativeGrabbedUp)).normalized;
		if (component.photonView.IsMine)
		{
			Vector3 localGrabPosition = component.localGrabPosition;
			photonView.RPC("GrabPointSyncRPC", (RpcTarget)0, new object[2] { playerPhotonID, localGrabPosition });
		}
	}

	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>().localGrabPosition = localPointInBox;
	}

	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		if (!playerGrabbing.Contains(component))
		{
			photonView.RPC("GrabPlayerAddRPC", (RpcTarget)0, new object[1] { playerPhotonID });
		}
	}

	[PunRPC]
	private void GrabPlayerAddRPC(int photonViewID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(photonViewID)).GetComponent<PhysGrabber>();
		playerGrabbing.Add(component);
	}

	[PunRPC]
	private void GrabPlayerRemoveRPC(int photonViewID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(photonViewID)).GetComponent<PhysGrabber>();
		playerGrabbing.Remove(component);
	}

	private void Throw(PhysGrabber player)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Max(rb.mass * 1.5f, 1f);
		Vector3 val = Vector3.ClampMagnitude(player.physGrabPointPullerPosition - player.physGrabPoint.position, player.forceMax) * num;
		val *= 0.5f + player.throwStrength;
		rb.AddForce(val, (ForceMode)1);
	}

	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		Throw(component);
		component.grabbed = false;
		if (playerGrabbing.Contains(component))
		{
			photonView.RPC("GrabPlayerRemoveRPC", (RpcTarget)0, new object[1] { playerPhotonID });
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			if (!Object.op_Implicit((Object)(object)impactDetector))
			{
				impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
			}
			if (!Object.op_Implicit((Object)(object)rb))
			{
				rb = ((Component)this).GetComponent<Rigidbody>();
			}
			stream.SendNext((object)rbVelocity);
			stream.SendNext((object)rbAngularVelocity);
			stream.SendNext((object)impactDetector.isSliding);
			stream.SendNext((object)isKinematic);
		}
		else
		{
			if (!Object.op_Implicit((Object)(object)impactDetector))
			{
				impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
			}
			rbVelocity = (Vector3)stream.ReceiveNext();
			rbAngularVelocity = (Vector3)stream.ReceiveNext();
			impactDetector.isSliding = (bool)stream.ReceiveNext();
			isKinematic = (bool)stream.ReceiveNext();
			lastUpdateTime = Time.time;
		}
	}

	public Transform FindColliderFromID(int colliderID)
	{
		foreach (Transform collider in colliders)
		{
			if (((Component)collider).GetComponent<PhysGrabObjectCollider>().colliderID == colliderID)
			{
				return collider;
			}
		}
		return null;
	}
}
