using System;
using Photon.Pun;
using UnityEngine;

public class ItemGun : MonoBehaviour
{
	private PhysGrabObject physGrabObject;

	private ItemToggle itemToggle;

	public int numberOfBullets = 1;

	[Range(0f, 65f)]
	public float gunRandomSpread;

	public float gunRange = 50f;

	public float distanceKeep = 0.8f;

	public float gunRecoilForce = 1f;

	public float cameraShakeMultiplier = 1f;

	public float torqueMultiplier = 1f;

	public float grabStrengthMultiplier = 1f;

	public float shootCooldown = 1f;

	public float batteryDrain = 0.1f;

	public bool batteryDrainFullBar;

	public int batteryDrainFullBars = 1;

	[Range(0f, 100f)]
	public float misfirePercentageChange = 50f;

	public AnimationCurve shootLineWidthCurve;

	public float grabVerticalOffset = -0.2f;

	public float aimVerticalOffset = -10f;

	public float investigateRadius = 20f;

	public Transform gunMuzzle;

	public GameObject bulletPrefab;

	public GameObject muzzleFlashPrefab;

	public Transform gunTrigger;

	public Sound soundShoot;

	public Sound soundShootGlobal;

	public Sound soundNoAmmoClick;

	public Sound soundHit;

	private float shootCooldownTimer;

	private ItemBattery itemBattery;

	private PhotonView photonView;

	private PhysGrabObjectImpactDetector impactDetector;

	private bool prevToggleState;

	private AnimationCurve triggerAnimationCurve;

	private float triggerAnimationEval;

	private bool triggerAnimationActive;

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		triggerAnimationCurve = AssetManager.instance.animationCurveClickInOut;
	}

	private void Update()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.grabbed && physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(distanceKeep);
		}
		if (triggerAnimationActive)
		{
			float num = 45f;
			triggerAnimationEval += Time.deltaTime * 4f;
			gunTrigger.localRotation = Quaternion.Euler(num * triggerAnimationCurve.Evaluate(triggerAnimationEval), 0f, 0f);
			if (triggerAnimationEval >= 1f)
			{
				gunTrigger.localRotation = Quaternion.Euler(0f, 0f, 0f);
				triggerAnimationActive = false;
				triggerAnimationEval = 1f;
			}
		}
		UpdateMaster();
	}

	private void UpdateMaster()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (physGrabObject.grabbed)
		{
			Quaternion turnX = Quaternion.Euler(aimVerticalOffset, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if (flag3)
				{
					if (item.playerAvatar.isCrouching || item.playerAvatar.isCrawling)
					{
						flag2 = true;
					}
					flag3 = false;
				}
				if (item.isRotating)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
			float num = grabVerticalOffset;
			if (flag2)
			{
				num += 0.5f;
			}
			physGrabObject.OverrideGrabVerticalPosition(num);
			if (!flag)
			{
				if (grabStrengthMultiplier != 1f)
				{
					physGrabObject.OverrideGrabStrength(grabStrengthMultiplier);
				}
				if (torqueMultiplier != 1f)
				{
					physGrabObject.OverrideTorqueStrength(torqueMultiplier);
				}
				if (itemBattery.batteryLife <= 0f)
				{
					physGrabObject.OverrideTorqueStrength(0.1f);
				}
			}
			if (flag)
			{
				physGrabObject.OverrideAngularDrag(40f);
				physGrabObject.OverrideTorqueStrength(6f);
			}
		}
		if (itemToggle.toggleState != prevToggleState)
		{
			if (shootCooldownTimer <= 0f)
			{
				Shoot();
				shootCooldownTimer = shootCooldown;
			}
			if (itemBattery.batteryLife <= 0f)
			{
				soundNoAmmoClick.Play(((Component)this).transform.position);
				StartTriggerAnimation();
				SemiFunc.CameraShakeImpact(1f, 0.1f);
				physGrabObject.rb.AddForceAtPosition(-gunMuzzle.forward * 1f, gunMuzzle.position, (ForceMode)1);
			}
			prevToggleState = itemToggle.toggleState;
		}
		if (shootCooldownTimer > 0f)
		{
			shootCooldownTimer -= Time.deltaTime;
		}
	}

	public void Misfire()
	{
		if (!physGrabObject.grabbed && !physGrabObject.hasNeverBeenGrabbed && SemiFunc.IsMasterClientOrSingleplayer() && (float)Random.Range(0, 100) < misfirePercentageChange)
		{
			Shoot();
		}
	}

	public void Shoot()
	{
		bool flag = false;
		if (itemBattery.batteryLife <= 0f)
		{
			flag = true;
		}
		if (Random.Range(0, 10000) == 0)
		{
			flag = false;
		}
		if (!flag)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ShootRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				ShootRPC();
			}
		}
	}

	private void MuzzleFlash()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Object.Instantiate<GameObject>(muzzleFlashPrefab, gunMuzzle.position, gunMuzzle.rotation, gunMuzzle).GetComponent<ItemGunMuzzleFlash>().ActivateAllEffects();
	}

	private void StartTriggerAnimation()
	{
		triggerAnimationActive = true;
		triggerAnimationEval = 0f;
	}

	[PunRPC]
	public void ShootRPC()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		float distanceMin = 3f * cameraShakeMultiplier;
		float distanceMax = 16f * cameraShakeMultiplier;
		SemiFunc.CameraShakeImpactDistance(gunMuzzle.position, 5f * cameraShakeMultiplier, 0.1f, distanceMin, distanceMax);
		SemiFunc.CameraShakeDistance(gunMuzzle.position, 0.1f * cameraShakeMultiplier, 0.1f * cameraShakeMultiplier, distanceMin, distanceMax);
		soundShoot.Play(gunMuzzle.position);
		soundShootGlobal.Play(gunMuzzle.position);
		MuzzleFlash();
		StartTriggerAnimation();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (investigateRadius > 0f)
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, investigateRadius);
		}
		physGrabObject.rb.AddForceAtPosition(-gunMuzzle.forward * gunRecoilForce, gunMuzzle.position, (ForceMode)1);
		if (!batteryDrainFullBar)
		{
			itemBattery.batteryLife -= batteryDrain;
		}
		else
		{
			itemBattery.RemoveFullBar(batteryDrainFullBars);
		}
		RaycastHit val4 = default(RaycastHit);
		for (int i = 0; i < numberOfBullets; i++)
		{
			Vector3 endPosition = gunMuzzle.position;
			bool hit = false;
			bool flag = false;
			Vector3 val = gunMuzzle.forward;
			if (gunRandomSpread > 0f)
			{
				float num = Random.Range(0f, gunRandomSpread / 2f);
				float num2 = Random.Range(0f, 360f);
				Vector3 val2 = Vector3.Cross(val, Random.onUnitSphere);
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				Quaternion val3 = Quaternion.AngleAxis(num, normalized);
				val = Quaternion.AngleAxis(num2, val) * val3 * val;
				val = ((Vector3)(ref val)).normalized;
			}
			if (Physics.Raycast(gunMuzzle.position, val, ref val4, gunRange, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) + LayerMask.GetMask(new string[1] { "Enemy" })))
			{
				endPosition = ((RaycastHit)(ref val4)).point;
				hit = true;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				endPosition = gunMuzzle.position + gunMuzzle.forward * gunRange;
				hit = false;
			}
			ShootBullet(endPosition, hit);
		}
	}

	private void ShootBullet(Vector3 _endPosition, bool _hit)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ShootBulletRPC", (RpcTarget)0, new object[2] { _endPosition, _hit });
			}
			else
			{
				ShootBulletRPC(_endPosition, _hit);
			}
		}
	}

	[PunRPC]
	public void ShootBulletRPC(Vector3 _endPosition, bool _hit)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.playerGrabbing.Count > 1)
		{
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				item.OverrideGrabRelease();
			}
		}
		ItemGunBullet component = Object.Instantiate<GameObject>(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation).GetComponent<ItemGunBullet>();
		component.hitPosition = _endPosition;
		component.bulletHit = _hit;
		soundHit.Play(_endPosition);
		component.shootLineWidthCurve = shootLineWidthCurve;
		component.ActivateAll();
	}
}
