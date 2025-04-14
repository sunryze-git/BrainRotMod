using UnityEngine;

public class ItemMineStun : MonoBehaviour
{
	private ItemMine itemMine;

	private PhysGrabObject bitPhysGrabObject;

	private Transform bitTransform;

	private Vector3 startPosition;

	private bool triggered;

	public AnimationCurve jawAnimationCurve;

	private Rigidbody rb;

	private PhysGrabObject physGrabObject;

	public Transform jaw1Tranform;

	public Transform jaw2Tranform;

	private float jawEval;

	private float jaw1CurrentRot;

	private float jaw2CurrentRot;

	public GameObject hurtCollider;

	private bool bite;

	public ParticleSystem particleFlash;

	public ParticleSystem particleLightning;

	private bool chomp;

	public Sound soundChomp;

	public Sound soundElectricity;

	private Quaternion jaw1StartRot;

	private Quaternion jaw2StartRot;

	private void Start()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		itemMine = ((Component)this).GetComponent<ItemMine>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		Quaternion localRotation = jaw1Tranform.localRotation;
		jaw1CurrentRot = ((Quaternion)(ref localRotation)).eulerAngles.x;
		localRotation = jaw2Tranform.localRotation;
		jaw2CurrentRot = ((Quaternion)(ref localRotation)).eulerAngles.x;
		jaw2StartRot = jaw2Tranform.localRotation;
		jaw1StartRot = jaw1Tranform.localRotation;
	}

	private void Reset()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (triggered)
		{
			chomp = false;
			bite = false;
			jawEval = 0f;
			jaw1Tranform.localRotation = jaw1StartRot;
			jaw2Tranform.localRotation = jaw2StartRot;
			Quaternion localRotation = jaw1Tranform.localRotation;
			jaw1CurrentRot = ((Quaternion)(ref localRotation)).eulerAngles.x;
			localRotation = jaw2Tranform.localRotation;
			jaw2CurrentRot = ((Quaternion)(ref localRotation)).eulerAngles.x;
			triggered = false;
			hurtCollider.SetActive(false);
		}
	}

	private void Update()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.grabbed && SemiFunc.IsMasterClientOrSingleplayer())
		{
			Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			bool flag = false;
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if (item.isRotating)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		if (itemMine.state == ItemMine.States.Disarmed)
		{
			Reset();
			if (jawEval > 0f)
			{
				jawEval -= Time.deltaTime * 2f;
				if (jawEval < 0f)
				{
					jawEval = 0f;
				}
				jaw1Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(jaw1CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), jawAnimationCurve.Evaluate(jawEval));
				jaw2Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(jaw2CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), jawAnimationCurve.Evaluate(jawEval));
			}
		}
		if (itemMine.state == ItemMine.States.Armed && jawEval < 1f)
		{
			jawEval += Time.deltaTime * 2f;
			if (jawEval > 1f)
			{
				jawEval = 1f;
			}
			jaw1Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(jaw1CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), jawAnimationCurve.Evaluate(jawEval));
			jaw2Tranform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(jaw2CurrentRot, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), jawAnimationCurve.Evaluate(jawEval));
		}
		if (itemMine.state != ItemMine.States.Triggered)
		{
			return;
		}
		if (jawEval < 1f)
		{
			jawEval += Time.deltaTime * 2f;
			if (jawEval > 1f)
			{
				jawEval = 1f;
			}
			jaw1Tranform.localRotation = Quaternion.Euler(-90f * jawAnimationCurve.Evaluate(jawEval), 0f, 0f);
			jaw2Tranform.localRotation = Quaternion.Euler(90f * jawAnimationCurve.Evaluate(jawEval), 0f, 0f);
			return;
		}
		float num = Mathf.PingPong(Time.time * 5f, 1f);
		float num2 = jawAnimationCurve.Evaluate(num);
		if (num > 0.1f)
		{
			if (!chomp)
			{
				soundChomp.Play(((Component)this).transform.position);
			}
			chomp = true;
		}
		else
		{
			chomp = false;
		}
		if (num > 0.5f)
		{
			if (!bite)
			{
				ElectricityEffect();
			}
			bite = true;
		}
		else
		{
			bite = false;
		}
		jaw1Tranform.localRotation = Quaternion.Euler(-64f * num2, 0f, 0f);
		jaw2Tranform.localRotation = Quaternion.Euler(64f * num2, 0f, 0f);
	}

	private void ElectricityEffect()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		soundElectricity.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		particleLightning.Play();
		particleFlash.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer() && Object.op_Implicit((Object)(object)bitTransform) && Vector3.Distance(((Component)this).transform.position, bitTransform.position) < 1.5f)
		{
			Vector3 insideUnitSphere = Random.insideUnitSphere;
			rb.AddTorque(insideUnitSphere * 1f, (ForceMode)1);
			Vector3 insideUnitSphere2 = Random.insideUnitSphere;
			rb.AddForce(insideUnitSphere2 * 1f, (ForceMode)1);
		}
	}

	private void FixedUpdate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (itemMine.state == ItemMine.States.Armed && Vector3.Angle(((Component)this).transform.up, Vector3.up) > 65f)
		{
			Vector3 right = ((Component)this).transform.right;
			rb.AddTorque(right * Time.fixedDeltaTime * 20f, (ForceMode)0);
		}
		if (!triggered)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)itemMine.triggeredTransform))
		{
			Transform val = SemiFunc.PlayerGetNearestTransformWithinRange(10f, ((Component)this).transform.position, doRaycastCheck: true);
			if (Object.op_Implicit((Object)(object)val))
			{
				itemMine.wasTriggeredByPlayer = true;
				itemMine.triggeredPlayerAvatar = ((Component)val).GetComponentInParent<PlayerAvatar>();
				bitTransform = val;
			}
			else
			{
				Enemy enemy = SemiFunc.EnemyGetNearest(((Component)this).transform.position, 10f, _raycast: true);
				if (Object.op_Implicit((Object)(object)enemy))
				{
					itemMine.wasTriggeredByEnemy = true;
					bitPhysGrabObject = ((Component)enemy).GetComponentInParent<PhysGrabObject>();
					bitTransform = enemy.CenterTransform;
				}
			}
		}
		if (itemMine.wasTriggeredByEnemy || itemMine.wasTriggeredByRigidBody)
		{
			if (!Object.op_Implicit((Object)(object)bitPhysGrabObject))
			{
				itemMine.DestroyMine();
				return;
			}
			if (Object.op_Implicit((Object)(object)bitPhysGrabObject) && !((Component)bitPhysGrabObject).gameObject.activeInHierarchy)
			{
				itemMine.DestroyMine();
				return;
			}
		}
		if (itemMine.wasTriggeredByPlayer)
		{
			if (!Object.op_Implicit((Object)(object)itemMine.triggeredPlayerAvatar) && !Object.op_Implicit((Object)(object)itemMine.triggeredPlayerTumble))
			{
				itemMine.DestroyMine();
				return;
			}
			if (itemMine.triggeredPlayerAvatar.isDisabled)
			{
				itemMine.DestroyMine();
				return;
			}
		}
		if (itemMine.wasTriggeredByPlayer)
		{
			if (Object.op_Implicit((Object)(object)bitTransform) && Object.op_Implicit((Object)(object)itemMine.triggeredPlayerTumble) && !((Behaviour)itemMine.triggeredPlayerTumble).isActiveAndEnabled)
			{
				itemMine.DestroyMine();
				return;
			}
			if (!Object.op_Implicit((Object)(object)bitTransform))
			{
				if (Object.op_Implicit((Object)(object)itemMine.triggeredPlayerAvatar))
				{
					bitTransform = itemMine.triggeredPlayerAvatar.PlayerVisionTarget.VisionTransform;
				}
				if (Object.op_Implicit((Object)(object)itemMine.triggeredPlayerTumble))
				{
					bitTransform = itemMine.triggeredPlayerTumble.playerAvatar.PlayerVisionTarget.VisionTransform;
				}
			}
		}
		if (!Object.op_Implicit((Object)(object)bitTransform))
		{
			itemMine.DestroyMine();
		}
		else if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Vector3 position = ((Component)this).transform.position;
			if (itemMine.wasTriggeredByPlayer)
			{
				position = bitTransform.position;
			}
			if (itemMine.wasTriggeredByEnemy)
			{
				position = bitTransform.position;
			}
			Vector3 val2 = bitTransform.position;
			if (Object.op_Implicit((Object)(object)bitPhysGrabObject))
			{
				val2 = bitPhysGrabObject.midPoint;
			}
			Vector3 val3 = position - new Vector3(val2.x, position.y, val2.z);
			physGrabObject.OverrideZeroGravity();
			Vector3 val4 = SemiFunc.PhysFollowPosition(((Component)this).transform.position, position, rb.velocity, 10f);
			rb.AddForce(val4 * Time.fixedDeltaTime, (ForceMode)1);
			if (val3 != Vector3.zero)
			{
				Vector3 val5 = SemiFunc.PhysFollowRotation(((Component)this).transform, Quaternion.LookRotation(val3), rb, 20f);
				rb.AddTorque(val5 * Time.fixedDeltaTime, (ForceMode)1);
			}
		}
	}

	public void OnTriggered()
	{
		ElectricityEffect();
		jawEval = 0f;
		triggered = true;
		bitTransform = itemMine.triggeredTransform;
		bitPhysGrabObject = itemMine.triggeredPhysGrabObject;
		hurtCollider.SetActive(true);
	}
}
