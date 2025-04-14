using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SlowWalkerAttack : MonoBehaviour
{
	public enum State
	{
		Idle,
		CheckInitial,
		Implosion,
		Delay,
		CheckAttack,
		Attack
	}

	public Transform vacuumSphere;

	[Space(10f)]
	public GameObject attackVacuumBuildup;

	public GameObject attackVacuumHurtCollider;

	public GameObject attackImpact;

	public GameObject attackImpactHurtColliders;

	private PhotonView photonView;

	[Space(10f)]
	private List<PlayerAvatar> playersBeingVacuumed = new List<PlayerAvatar>();

	private List<PlayerTumble> playerTumbles = new List<PlayerTumble>();

	private List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	private List<ParticleSystem> vacuumParticles = new List<ParticleSystem>();

	private List<ParticleSystem> impactParticles = new List<ParticleSystem>();

	[Space(10f)]
	public Sound soundVacuumImpact;

	public Sound soundVacuumImpactGlobal;

	public Sound soundVacuumBuildup;

	public Sound soundImpact;

	public Sound soundImpactGlobal;

	public PhysGrabObject enemyPhysGrabObject;

	public Enemy enemy;

	internal State currentState;

	private bool stateStart;

	private bool stateFixed;

	private float stateTimer;

	private float hurtColliderTimer;

	public Transform clubHitPoint;

	public GameObject hurtColliderFirstHit;

	private Vector3 foundPosition;

	private bool didFindPosition;

	private Vector3 slowWalkerCenter;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		vacuumParticles.AddRange(attackVacuumBuildup.GetComponentsInChildren<ParticleSystem>());
		impactParticles.AddRange(attackImpact.GetComponentsInChildren<ParticleSystem>());
	}

	private void SuckInListUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)clubHitPoint))
		{
			return;
		}
		((Component)this).transform.position = clubHitPoint.position;
		Vector3 val = slowWalkerCenter;
		Vector3 val2 = clubHitPoint.position - slowWalkerCenter;
		RaycastHit[] array = Physics.RaycastAll(val, ((Vector3)(ref val2)).normalized, 4f, LayerMask.GetMask(new string[1] { "Default" }));
		bool flag = false;
		Vector3 point = slowWalkerCenter;
		float num = float.MaxValue;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit val3 = array2[i];
			if (((Component)((RaycastHit)(ref val3)).collider).gameObject.CompareTag("Wall"))
			{
				float num2 = Vector3.Distance(slowWalkerCenter, ((RaycastHit)(ref val3)).point);
				if (num2 < num)
				{
					num = num2;
					point = ((RaycastHit)(ref val3)).point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			foundPosition = point;
			foundPosition = Vector3.MoveTowards(foundPosition, slowWalkerCenter, 0.2f);
			didFindPosition = true;
		}
		point = slowWalkerCenter;
		num = float.MaxValue;
		RaycastHit[] array3 = Physics.RaycastAll(((Component)this).transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[1] { "Default" }));
		flag = false;
		array2 = array3;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit val4 = array2[i];
			if (((Component)((RaycastHit)(ref val4)).collider).gameObject.CompareTag("Wall"))
			{
				float num3 = Vector3.Distance(((Component)this).transform.position, ((RaycastHit)(ref val4)).point);
				if (num3 < num)
				{
					num = num3;
					point = ((RaycastHit)(ref val4)).point;
					flag = true;
				}
			}
		}
		if (flag)
		{
			foundPosition = point;
			didFindPosition = true;
		}
		if (didFindPosition)
		{
			((Component)this).transform.position = foundPosition;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		physGrabObjects.Clear();
		foreach (PhysGrabObject item in SemiFunc.PhysGrabObjectGetAllWithinRange(vacuumSphere.localScale.x * 0.5f, vacuumSphere.position + Vector3.up * 0.5f))
		{
			RaycastHit[] array4 = Physics.RaycastAll(item.midPoint, ((Component)this).transform.position + Vector3.up * 0.5f - item.midPoint, vacuumSphere.localScale.x, LayerMask.GetMask(new string[1] { "Default" }));
			bool flag2 = false;
			array2 = array4;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit val5 = array2[i];
				if (((Component)((RaycastHit)(ref val5)).collider).gameObject.CompareTag("Wall"))
				{
					flag2 = true;
				}
			}
			if (!flag2 && !item.isPlayer && (Object)(object)item != (Object)(object)enemyPhysGrabObject)
			{
				physGrabObjects.Add(item);
			}
		}
	}

	private void SuckInListPlayerUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = vacuumSphere.position + Vector3.up * 2f;
		Vector3 val2 = val;
		playersBeingVacuumed.Clear();
		List<PlayerAvatar> collection = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(vacuumSphere.localScale.x, val);
		playersBeingVacuumed.AddRange(collection);
		playerTumbles.Clear();
		foreach (PlayerAvatar item in playersBeingVacuumed)
		{
			val = vacuumSphere.position + Vector3.up * 2f;
			Vector3 position = item.PlayerVisionTarget.VisionTransform.position;
			Vector3 val3 = position - val;
			Vector3 normalized = ((Vector3)(ref val3)).normalized;
			float num = Vector3.Distance(val, position);
			RaycastHit[] array = Physics.RaycastAll(val, normalized, num, LayerMask.GetMask(new string[1] { "Default" }));
			bool flag = false;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit val4 = array2[i];
				if (((Component)((RaycastHit)(ref val4)).collider).gameObject.CompareTag("Wall"))
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			if (flag)
			{
				array2 = Physics.RaycastAll(val, Vector3.up, vacuumSphere.localScale.x * 0.25f, LayerMask.GetMask(new string[1] { "Default" }));
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit val5 = array2[i];
					if (((Component)((RaycastHit)(ref val5)).collider).gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					val = val2 + Vector3.up * vacuumSphere.localScale.x * 0.25f;
					val3 = position - val;
					normalized = ((Vector3)(ref val3)).normalized;
					num = Vector3.Distance(val, position);
					RaycastHit[] array3 = Physics.RaycastAll(val, normalized, num, LayerMask.GetMask(new string[1] { "Default" }));
					flag = false;
					array2 = array3;
					for (int i = 0; i < array2.Length; i++)
					{
						RaycastHit val6 = array2[i];
						if (((Component)((RaycastHit)(ref val6)).collider).gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag && !flag2)
			{
				array2 = Physics.RaycastAll(val, Vector3.up, vacuumSphere.localScale.x * 0.5f, LayerMask.GetMask(new string[1] { "Default" }));
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit val7 = array2[i];
					if (((Component)((RaycastHit)(ref val7)).collider).gameObject.CompareTag("Wall"))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					val = val2 + Vector3.up * vacuumSphere.localScale.x * 0.5f;
					val3 = position - val;
					normalized = ((Vector3)(ref val3)).normalized;
					num = Vector3.Distance(val, position);
					RaycastHit[] array4 = Physics.RaycastAll(val, normalized, num, LayerMask.GetMask(new string[1] { "Default" }));
					flag = false;
					array2 = array4;
					for (int i = 0; i < array2.Length; i++)
					{
						RaycastHit val8 = array2[i];
						if (((Component)((RaycastHit)(ref val8)).collider).gameObject.CompareTag("Wall"))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				continue;
			}
			if (item.isTumbling)
			{
				playerTumbles.Add(item.tumble);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					item.tumble.TumbleOverrideTime(2f);
					item.tumble.OverrideEnemyHurt(0.5f);
				}
			}
			if (!item.isDisabled && !item.isTumbling)
			{
				playerTumbles.Add(item.tumble);
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					item.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
					item.tumble.TumbleOverrideTime(2f);
					item.tumble.OverrideEnemyHurt(0.5f);
				}
			}
		}
	}

	private void StateIdle()
	{
		if (stateFixed)
		{
			return;
		}
		if (stateStart)
		{
			didFindPosition = false;
			stateStart = false;
			attackImpactHurtColliders.SetActive(false);
			attackVacuumHurtCollider.SetActive(false);
			hurtColliderFirstHit.SetActive(false);
		}
		if (SemiFunc.FPSImpulse1())
		{
			if (hurtColliderFirstHit.activeSelf)
			{
				hurtColliderFirstHit.SetActive(false);
			}
			if (attackVacuumHurtCollider.activeSelf)
			{
				attackVacuumHurtCollider.SetActive(false);
			}
			if (attackImpactHurtColliders.activeSelf)
			{
				attackImpactHurtColliders.SetActive(false);
			}
		}
	}

	private void StateCheckInitial()
	{
		if (!stateFixed)
		{
			if (stateStart)
			{
				didFindPosition = false;
				SuckInListUpdate();
				stateStart = false;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				StateSet(State.Implosion);
			}
		}
	}

	private void StateImplosion()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		if (stateStart)
		{
			stateTimer = 1.5f;
			hurtColliderTimer = 0.2f;
			attackVacuumHurtCollider.SetActive(true);
			hurtColliderFirstHit.SetActive(true);
			stateStart = false;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 6f, 15f, ((Component)this).transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 6f, 15f, ((Component)this).transform.position, 0.1f);
			ParticlesPlayVacuum();
			soundVacuumImpact.Play(((Component)this).transform.position);
			soundVacuumImpactGlobal.Play(((Component)this).transform.position);
			soundVacuumBuildup.Play(((Component)this).transform.position);
			val = ((Component)this).transform.position - slowWalkerCenter;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			((Component)this).transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
			Quaternion rotation = ((Component)this).transform.rotation;
			float y = ((Quaternion)(ref rotation)).eulerAngles.y;
			((Component)this).transform.rotation = Quaternion.Euler(0f, y, 0f);
			SuckInListPlayerUpdate();
		}
		if (stateFixed)
		{
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			foreach (PlayerTumble playerTumble in playerTumbles)
			{
				if (playerTumble.isTumbling)
				{
					val = vacuumSphere.position - ((Component)playerTumble.physGrabObject).transform.position;
					Vector3 normalized2 = ((Vector3)(ref val)).normalized;
					Rigidbody rb = playerTumble.physGrabObject.rb;
					rb.AddForce(normalized2 * 2500f * Time.fixedDeltaTime, (ForceMode)0);
					Vector3 val2 = SemiFunc.PhysFollowDirection(((Component)rb).transform, normalized2, rb, 10f) * 2f;
					rb.AddTorque(val2 / rb.mass, (ForceMode)0);
				}
			}
			foreach (PhysGrabObject physGrabObject in physGrabObjects)
			{
				if (Object.op_Implicit((Object)(object)physGrabObject))
				{
					val = vacuumSphere.position - ((Component)physGrabObject).transform.position;
					Vector3 normalized3 = ((Vector3)(ref val)).normalized;
					Rigidbody rb2 = physGrabObject.rb;
					rb2.AddForce(normalized3 * 2500f * Time.fixedDeltaTime, (ForceMode)0);
					Vector3 val3 = SemiFunc.PhysFollowDirection(((Component)rb2).transform, normalized3, rb2, 10f) * 2f;
					rb2.AddTorque(val3 / rb2.mass, (ForceMode)0);
				}
			}
		}
		if (stateFixed)
		{
			return;
		}
		if (didFindPosition)
		{
			((Component)this).transform.position = foundPosition;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.Health.ObjectHurtDisable(0.5f);
			if (stateTimer <= 0f)
			{
				StateSet(State.Attack);
				attackVacuumHurtCollider.SetActive(false);
			}
			if (stateTimer < 1f && hurtColliderFirstHit.activeSelf)
			{
				hurtColliderFirstHit.SetActive(false);
			}
			if (SemiFunc.FPSImpulse5())
			{
				SuckInListPlayerUpdate();
			}
			if (hurtColliderTimer > 0f)
			{
				hurtColliderTimer -= Time.deltaTime;
			}
			else
			{
				attackVacuumHurtCollider.SetActive(false);
			}
		}
	}

	private void StateDelay()
	{
		if (!stateFixed && stateStart)
		{
			stateStart = false;
		}
	}

	private void StateCheckAttack()
	{
		if (!stateFixed && stateStart)
		{
			stateStart = false;
		}
	}

	private void StateAttack()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!stateFixed)
		{
			if (stateStart)
			{
				stateStart = false;
				stateTimer = 3.5f;
				GameDirector.instance.CameraImpact.ShakeDistance(8f, 6f, 15f, ((Component)this).transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 6f, 15f, ((Component)this).transform.position, 0.1f);
				ParticlesPlayImpact();
				soundImpact.Play(((Component)this).transform.position);
				soundImpactGlobal.Play(((Component)this).transform.position);
				Vector3 val = ((Component)this).transform.position - slowWalkerCenter;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				((Component)this).transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
				hurtColliderTimer = 0.2f;
				attackImpactHurtColliders.SetActive(true);
			}
			if (stateTimer <= 0f)
			{
				StateSet(State.Idle);
				attackImpactHurtColliders.SetActive(false);
			}
			if (hurtColliderTimer > 0f)
			{
				hurtColliderTimer -= Time.deltaTime;
			}
			else
			{
				attackImpactHurtColliders.SetActive(false);
			}
		}
	}

	private void StateMachine(bool _stateFixed)
	{
		if (_stateFixed)
		{
			stateFixed = true;
		}
		switch (currentState)
		{
		case State.Idle:
			StateIdle();
			break;
		case State.CheckInitial:
			StateCheckInitial();
			break;
		case State.Implosion:
			StateImplosion();
			break;
		case State.Attack:
			StateAttack();
			break;
		}
		if (_stateFixed && stateFixed)
		{
			stateFixed = false;
		}
	}

	public void AttackStart()
	{
		currentState = State.CheckInitial;
		stateStart = true;
	}

	private void Update()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)enemyPhysGrabObject))
		{
			slowWalkerCenter = enemyPhysGrabObject.midPoint;
		}
		if (SemiFunc.FPSImpulse1() && Object.op_Implicit((Object)(object)enemy) && Object.op_Implicit((Object)(object)enemy.EnemyParent) && !enemy.EnemyParent.Spawned && currentState != 0)
		{
			StateSet(State.Idle);
		}
		StateMachine(_stateFixed: false);
		if (stateTimer > 0f)
		{
			stateTimer -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		StateMachine(_stateFixed: true);
	}

	public void StateSet(State state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				StateSetRPC(state);
				return;
			}
			photonView.RPC("StateSetRPC", (RpcTarget)0, new object[1] { state });
		}
	}

	[PunRPC]
	public void StateSetRPC(State state)
	{
		currentState = state;
		stateStart = true;
	}

	private void ParticlesPlayVacuum()
	{
		foreach (ParticleSystem vacuumParticle in vacuumParticles)
		{
			vacuumParticle.Play();
		}
	}

	private void ParticlesPlayImpact()
	{
		foreach (ParticleSystem impactParticle in impactParticles)
		{
			impactParticle.Play();
		}
	}

	private void OnDisable()
	{
		attackVacuumHurtCollider.SetActive(false);
		hurtColliderFirstHit.SetActive(false);
		attackImpactHurtColliders.SetActive(false);
		attackImpact.SetActive(false);
		attackVacuumBuildup.SetActive(false);
	}
}
