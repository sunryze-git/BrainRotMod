using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ItemMelee : MonoBehaviour, IPunObservable
{
	private float durabilityDrain = 2.5f;

	public float durabilityDrainOnEnemiesAndPVP = 5f;

	public float hitFreeze = 0.2f;

	public float hitFreezeDelay;

	public float swingDetectSpeedMultiplier = 1f;

	public bool turnWeapon = true;

	public float torqueStrength = 1f;

	public float turnWeaponStrength = 40f;

	public Quaternion customRotation = Quaternion.identity;

	public UnityEvent onHit;

	private Transform hurtCollider;

	private Transform hurtColliderRotation;

	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	private PhysGrabObject physGrabObject;

	private Rigidbody rb;

	private float swingTimer = 0.1f;

	private float hitBoxTimer = 0.1f;

	private TrailRenderer trailRenderer;

	public Sound soundSwingLoop;

	public Sound soundSwing;

	public Sound soundHit;

	private Vector3 prevPosition;

	private float prevPosDistance;

	private float prevPosUpdateTimer;

	private Transform swingPoint;

	private Quaternion swingDirection;

	private PlayerAvatar playerAvatar;

	private float hitSoundDelayTimer;

	private ParticleSystem particleSystem;

	private ParticleSystem particleSystemGroundHit;

	private PhotonView photonView;

	private float swingPitch = 1f;

	private float swingPitchTarget;

	private float swingPitchTargetProgress;

	private float distanceCheckTimer;

	private ItemBattery itemBattery;

	private Vector3 swingStartDirection = Vector3.zero;

	private Transform forceGrabPoint;

	private bool isBroken;

	private Quaternion targetYRotation;

	private Quaternion currentYRotation;

	private float durabilityLossCooldown;

	private Transform meshHealthy;

	private Transform meshBroken;

	private bool isSwinging;

	private bool newSwing;

	private float hitTimer;

	private ItemEquippable itemEquippable;

	private float hitCooldown;

	private float groundHitCooldown;

	private float groundHitSoundTimer;

	private float spawnTimer = 3f;

	private float grabbedTimer;

	private float enemyOrPVPDurabilityLossCooldown;

	private void Start()
	{
		rb = ((Component)this).GetComponent<Rigidbody>();
		hurtCollider = ((Component)((Component)this).GetComponentInChildren<HurtCollider>()).transform;
		hurtColliderRotation = ((Component)this).transform.Find("Hurt Collider Rotation");
		physGrabObjectImpactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		((Component)hurtCollider).gameObject.SetActive(false);
		trailRenderer = ((Component)this).GetComponentInChildren<TrailRenderer>();
		swingPoint = ((Component)this).transform.Find("Swing Point");
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		particleSystem = ((Component)((Component)this).transform.Find("Particles")).GetComponent<ParticleSystem>();
		particleSystemGroundHit = ((Component)((Component)this).transform.Find("Particles Ground Hit")).GetComponent<ParticleSystem>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		forceGrabPoint = ((Component)this).transform.Find("Force Grab Point");
		meshHealthy = ((Component)this).transform.Find("Mesh Healthy");
		meshBroken = ((Component)this).transform.Find("Mesh Broken");
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		if (SemiFunc.RunIsArena())
		{
			HurtCollider component = ((Component)hurtCollider).GetComponent<HurtCollider>();
			component.playerDamage = component.enemyDamage;
		}
	}

	private void DisableHurtBoxWhenEquipping()
	{
		if (itemEquippable.equipTimer > 0f || itemEquippable.unequipTimer > 0f)
		{
			((Component)hurtCollider).gameObject.SetActive(false);
			swingTimer = 0f;
			trailRenderer.emitting = false;
		}
	}

	private void FixedUpdate()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		DisableHurtBoxWhenEquipping();
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
			Quaternion turnY = currentYRotation;
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			physGrabObject.TurnXYZ(turnX, turnY, Quaternion.identity);
		}
		if (itemEquippable.equipTimer > 0f || itemEquippable.unequipTimer > 0f || isBroken)
		{
			return;
		}
		if (prevPosUpdateTimer > 0.1f)
		{
			prevPosition = swingPoint.position;
			prevPosUpdateTimer = 0f;
		}
		else
		{
			prevPosUpdateTimer += Time.fixedDeltaTime;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!flag)
		{
			if (torqueStrength != 1f)
			{
				physGrabObject.OverrideTorqueStrength(torqueStrength);
			}
			if (physGrabObject.grabbed)
			{
				if (itemBattery.batteryLife <= 0f)
				{
					physGrabObject.OverrideTorqueStrength(0.1f);
				}
				physGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSlippery());
			}
		}
		if (flag)
		{
			physGrabObject.OverrideTorqueStrength(4f);
		}
		if (distanceCheckTimer > 0.1f)
		{
			prevPosDistance = Vector3.Distance(prevPosition, swingPoint.position) * 10f * rb.mass;
			distanceCheckTimer = 0f;
		}
		distanceCheckTimer += Time.fixedDeltaTime;
		TurnWeapon();
		Vector3 val = prevPosition - swingPoint.position;
		float num = 1f;
		if (!physGrabObject.grabbed)
		{
			num = 0.5f;
		}
		if (((Vector3)(ref val)).magnitude > num * swingDetectSpeedMultiplier && swingPoint.position - prevPosition != Vector3.zero)
		{
			swingTimer = 0.2f;
			if (!isSwinging)
			{
				newSwing = true;
			}
			swingDirection = Quaternion.LookRotation(swingPoint.position - prevPosition);
		}
	}

	private void TurnWeapon()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (customRotation != Quaternion.identity && !turnWeapon)
		{
			Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
			physGrabObject.TurnXYZ(turnX, customRotation, Quaternion.identity);
		}
		if (turnWeaponStrength != 1f)
		{
			physGrabObject.OverrideTorqueStrengthY(turnWeaponStrength);
		}
		if (!turnWeapon)
		{
			return;
		}
		physGrabObject.OverrideAngularDrag(0f);
		physGrabObject.OverrideDrag(0f);
		if (physGrabObject.grabbed && !Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = ((Component)physGrabObject.playerGrabbing[0]).GetComponent<PlayerAvatar>();
		}
		if (!physGrabObject.grabbed && Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = null;
		}
		if (!physGrabObject.grabbed)
		{
			return;
		}
		_ = Vector3.forward;
		_ = Vector3.up;
		_ = ((Component)playerAvatar).transform;
		Vector3 val = rb.velocity / Time.fixedDeltaTime;
		if (((Vector3)(ref val)).magnitude > 200f)
		{
			Vector3 val2 = ((Component)playerAvatar).transform.InverseTransformDirection(val);
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(val2.x, 0f, val2.z);
			Quaternion val4 = Quaternion.identity;
			if (val3 != Vector3.zero)
			{
				val4 = Quaternion.LookRotation(val3);
			}
			Quaternion val5 = Quaternion.Euler(0f, ((Quaternion)(ref val4)).eulerAngles.y + 90f, 0f);
			val5 = Quaternion.Euler(0f, Mathf.Round(((Quaternion)(ref val5)).eulerAngles.y / 90f) * 90f, 0f);
			if (((Quaternion)(ref val5)).eulerAngles.y == 270f)
			{
				val5 = Quaternion.Euler(0f, 90f, 0f);
			}
			if (((Quaternion)(ref val5)).eulerAngles.y == 180f)
			{
				val5 = Quaternion.Euler(0f, 0f, 0f);
			}
			targetYRotation = val5;
		}
		currentYRotation = Quaternion.Slerp(currentYRotation, targetYRotation, Time.deltaTime * 5f);
	}

	private void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		if (grabbedTimer > 0f)
		{
			grabbedTimer -= Time.deltaTime;
		}
		if (physGrabObject.grabbed)
		{
			grabbedTimer = 1f;
		}
		if (hitFreezeDelay > 0f)
		{
			hitFreezeDelay -= Time.deltaTime;
			if (hitFreezeDelay <= 0f)
			{
				physGrabObject.FreezeForces(hitFreeze, Vector3.zero, Vector3.zero);
			}
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (spawnTimer > 0f)
		{
			prevPosition = swingPoint.position;
			swingTimer = 0f;
			spawnTimer -= Time.deltaTime;
			return;
		}
		if (hitCooldown > 0f)
		{
			hitCooldown -= Time.deltaTime;
		}
		if (enemyOrPVPDurabilityLossCooldown > 0f)
		{
			enemyOrPVPDurabilityLossCooldown -= Time.deltaTime;
		}
		if (groundHitCooldown > 0f)
		{
			groundHitCooldown -= Time.deltaTime;
		}
		if (groundHitSoundTimer > 0f)
		{
			groundHitSoundTimer -= Time.deltaTime;
		}
		DisableHurtBoxWhenEquipping();
		if (itemEquippable.equipTimer > 0f || itemEquippable.unequipTimer > 0f)
		{
			return;
		}
		soundSwingLoop.PlayLoop(((Component)hurtCollider).gameObject.activeSelf, 10f, 10f, 3f);
		if (SemiFunc.IsMultiplayer() && !SemiFunc.IsMasterClient() && isSwinging)
		{
			swingTimer = 0.5f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (itemBattery.batteryLife <= 0f)
			{
				MeleeBreak();
			}
			else
			{
				MeleeFix();
			}
			if (durabilityLossCooldown > 0f)
			{
				durabilityLossCooldown -= Time.deltaTime;
			}
			if (!isBroken)
			{
				if (physGrabObject.grabbedLocal)
				{
					if (itemBattery.batteryActive)
					{
					}
				}
				else
				{
					_ = itemBattery.batteryActive;
				}
			}
		}
		if (isBroken)
		{
			return;
		}
		if (hitSoundDelayTimer > 0f)
		{
			hitSoundDelayTimer -= Time.deltaTime;
		}
		if (swingPitch != swingPitchTarget && swingPitchTargetProgress >= 1f)
		{
			swingPitch = swingPitchTarget;
		}
		Vector3 val = prevPosition - swingPoint.position;
		if (((Vector3)(ref val)).magnitude > 0.1f)
		{
			hurtColliderRotation.LookAt(hurtColliderRotation.position - val, Vector3.up);
			hurtColliderRotation.localEulerAngles = new Vector3(0f, hurtColliderRotation.localEulerAngles.y, 0f);
			hurtColliderRotation.localEulerAngles = new Vector3(0f, Mathf.Round(hurtColliderRotation.localEulerAngles.y / 90f) * 90f, 0f);
		}
		Vector3 val2 = prevPosition - swingPoint.position;
		Vector3 normalized = ((Vector3)(ref swingStartDirection)).normalized;
		Vector3 normalized2 = ((Vector3)(ref val2)).normalized;
		float num = Vector3.Dot(normalized, normalized2);
		double num2 = 0.85;
		if (!physGrabObject.grabbed)
		{
			num2 = 0.1;
		}
		if ((double)num > num2)
		{
			swingTimer = 0f;
		}
		if (isSwinging)
		{
			ActivateHitbox();
		}
		if (hitTimer > 0f)
		{
			hitTimer -= Time.deltaTime;
		}
		if (swingTimer <= 0f)
		{
			if (hitBoxTimer <= 0f)
			{
				((Component)hurtCollider).gameObject.SetActive(false);
			}
			else
			{
				hitBoxTimer -= Time.deltaTime;
			}
			trailRenderer.emitting = false;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				isSwinging = false;
			}
		}
		else
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				isSwinging = true;
			}
			if (hitTimer <= 0f)
			{
				hitBoxTimer = 0.2f;
			}
			swingTimer -= Time.deltaTime;
		}
	}

	public void SwingHit()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			SwingHitRPC(durabilityLoss: true);
			return;
		}
		photonView.RPC("SwingHitRPC", (RpcTarget)0, new object[1] { true });
	}

	public void EnemyOrPVPSwingHit()
	{
		if (enemyOrPVPDurabilityLossCooldown <= 0f)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				EnemyOrPVPSwingHitRPC();
			}
			else
			{
				photonView.RPC("EnemyOrPVPSwingHitRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
	}

	[PunRPC]
	public void EnemyOrPVPSwingHitRPC()
	{
		itemBattery.batteryLife -= durabilityDrainOnEnemiesAndPVP;
		enemyOrPVPDurabilityLossCooldown = 0.1f;
	}

	[PunRPC]
	public void SwingHitRPC(bool durabilityLoss)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (durabilityLoss)
		{
			if (hitCooldown > 0f || hitSoundDelayTimer > 0f)
			{
				return;
			}
			hitSoundDelayTimer = 0.1f;
			hitCooldown = 0.3f;
		}
		else
		{
			if (groundHitCooldown > 0f || groundHitSoundTimer > 0f)
			{
				return;
			}
			groundHitCooldown = 0.3f;
			groundHitSoundTimer = 0.1f;
			flag = true;
		}
		if (!flag)
		{
			soundHit.Pitch = 1f;
			soundHit.Play(((Component)this).transform.position);
			particleSystem.Play();
		}
		else
		{
			soundHit.Pitch = 2f;
			soundHit.Play(((Component)this).transform.position, 0.5f);
			particleSystemGroundHit.Play();
		}
		if (physGrabObject.grabbed && !rb.isKinematic)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		if (hitBoxTimer > 0.05f)
		{
			hitBoxTimer = 0.05f;
		}
		hitTimer = 0.5f;
		if (SemiFunc.IsMasterClientOrSingleplayer() && durabilityLoss && durabilityLossCooldown <= 0f && SemiFunc.RunIsLevel())
		{
			itemBattery.batteryLife -= durabilityDrain;
			durabilityLossCooldown = 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && hitFreeze > 0f && !flag)
		{
			hitFreezeDelay = 0.06f;
		}
		if (onHit != null)
		{
			onHit.Invoke();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 10f, ((Component)this).transform.position, 0.1f);
	}

	public void GroundHit()
	{
		if (!(hitTimer > 0f) && !(hitBoxTimer <= 0f))
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SwingHitRPC(durabilityLoss: false);
				return;
			}
			photonView.RPC("SwingHitRPC", (RpcTarget)0, new object[1] { false });
		}
	}

	private void MeleeBreak()
	{
		if (!isBroken)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				MeleeBreakRPC();
			}
			else if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("MeleeBreakRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
	}

	[PunRPC]
	public void MeleeBreakRPC()
	{
		if (physGrabObject.isMelee)
		{
			particleSystem.Play();
			physGrabObject.isMelee = false;
			physGrabObject.forceGrabPoint = null;
			itemBattery.BatteryToggle(toggle: false);
			isBroken = true;
			((Component)hurtCollider).gameObject.SetActive(false);
			trailRenderer.emitting = false;
			((Component)meshHealthy).gameObject.SetActive(false);
			((Component)meshBroken).gameObject.SetActive(true);
		}
	}

	private void MeleeFix()
	{
		if (isBroken)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				MeleeFixRPC();
			}
			else if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("MeleeFixRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
	}

	[PunRPC]
	public void MeleeFixRPC()
	{
		if (!physGrabObject.isMelee)
		{
			particleSystem.Play();
			physGrabObject.isMelee = true;
			physGrabObject.forceGrabPoint = forceGrabPoint;
			itemBattery.BatteryToggle(toggle: true);
			isBroken = false;
			((Component)meshHealthy).gameObject.SetActive(true);
			((Component)meshBroken).gameObject.SetActive(false);
		}
	}

	public void ActivateHitbox()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (hitTimer > 0f)
		{
			return;
		}
		if (newSwing)
		{
			soundSwing.Play(((Component)this).transform.position);
			swingPitchTarget = prevPosDistance;
			swingPitchTargetProgress = 0f;
			if (Object.op_Implicit((Object)(object)swingPoint))
			{
				swingStartDirection = swingPoint.position - prevPosition;
			}
			swingTimer = 0.4f;
			hitBoxTimer = 0.4f;
			if (grabbedTimer > 0f)
			{
				float num = 150f;
				if (!physGrabObject.grabbed)
				{
					num *= 0.5f;
				}
				rb.AddForceAtPosition(swingDirection * Vector3.forward * num * rb.mass, swingPoint.position);
			}
			newSwing = false;
		}
		if (Object.op_Implicit((Object)(object)hurtCollider))
		{
			((Component)hurtCollider).gameObject.SetActive(true);
		}
		if (Object.op_Implicit((Object)(object)trailRenderer))
		{
			trailRenderer.emitting = true;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext((object)isSwinging);
			return;
		}
		bool num = isSwinging;
		isSwinging = (bool)stream.ReceiveNext();
		if (!num && isSwinging)
		{
			newSwing = true;
			ActivateHitbox();
		}
	}
}
