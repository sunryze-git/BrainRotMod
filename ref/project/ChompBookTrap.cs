using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ChompBookTrap : Trap
{
	private Animator animator;

	[Space]
	[Header("Book Components")]
	public GameObject closedBookTop;

	public GameObject closedBookBot;

	public GameObject chainLock;

	public GameObject biteBookTop;

	public GameObject biteBookBot;

	[Space]
	[Header("Sounds")]
	public Sound chomp;

	public Sound lockBreak;

	[Space]
	private Quaternion initialBookRotation;

	private Rigidbody rb;

	public ParticleSystem lockParticle;

	private Transform targetTransform;

	private Vector3 playerDirection;

	public int biteAmount;

	private int biteCount;

	private Quaternion lookRotation;

	private float attackedTimer;

	private bool trapStopped;

	protected override void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		initialBookRotation = closedBookTop.transform.localRotation;
		rb = ((Component)this).GetComponent<Rigidbody>();
		animator = ((Component)this).GetComponent<Animator>();
	}

	protected override void Update()
	{
		base.Update();
		if (trapStart)
		{
			TrapActivate();
		}
		if (trapActive)
		{
			physGrabObject.OverrideIndestructible();
			enemyInvestigate = true;
		}
	}

	private void FixedUpdate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && trapActive && Object.op_Implicit((Object)(object)targetTransform))
		{
			Vector3 val = targetTransform.position - physGrabObject.midPoint;
			playerDirection = ((Vector3)(ref val)).normalized;
			Quaternion val2 = Quaternion.LookRotation(targetTransform.position - physGrabObject.midPoint);
			lookRotation = Quaternion.Slerp(lookRotation, val2, Time.deltaTime * 5f);
			Vector3 val3 = SemiFunc.PhysFollowRotation(((Component)this).transform, lookRotation, rb, 0.3f);
			if (physGrabObject.playerGrabbing.Count > 0)
			{
				val3 *= 0.25f;
			}
			rb.AddTorque(val3, (ForceMode)1);
			if (attackedTimer <= 0f)
			{
				Vector3 val4 = SemiFunc.PhysFollowPosition(((Component)this).transform.position, targetTransform.position, rb.velocity, 1.5f);
				rb.AddForce(val4 * 10f * Time.fixedDeltaTime, (ForceMode)1);
			}
			else
			{
				attackedTimer -= Time.fixedDeltaTime;
			}
			physGrabObject.OverrideZeroGravity();
		}
	}

	public void Attack()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (isLocal)
		{
			targetTransform = SemiFunc.PlayerGetNearestTransformWithinRange(10f, physGrabObject.centerPoint, doRaycastCheck: true, LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Default" })));
			if (Object.op_Implicit((Object)(object)targetTransform))
			{
				attackedTimer = 0.5f;
				rb.AddForce(playerDirection * 2f, (ForceMode)1);
			}
			else
			{
				rb.AddForce(Vector3.up * 3f, (ForceMode)1);
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				Vector3 normalized = ((Vector3)(ref insideUnitSphere)).normalized;
				rb.AddForce(normalized * 3f, (ForceMode)1);
				rb.AddTorque(normalized * 1f, (ForceMode)1);
			}
			biteCount++;
			if (biteCount >= biteAmount)
			{
				TrapStop();
			}
		}
	}

	public void ChompSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		chomp.Play(physGrabObject.centerPoint);
	}

	public void StopAnimation()
	{
		if (trapStopped)
		{
			((Behaviour)animator).enabled = false;
		}
	}

	private void TrapStopLogic()
	{
		trapActive = false;
		trapStopped = true;
		DeparentAndDestroy(lockParticle);
	}

	public void TrapStop()
	{
		if (!GameManager.Multiplayer())
		{
			TrapStopRPC();
		}
		else
		{
			photonView.RPC("TrapStopRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void TrapStopRPC()
	{
		TrapStopLogic();
	}

	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)particleSystem) && particleSystem.isPlaying)
		{
			((Component)particleSystem).gameObject.transform.parent = null;
			MainModule main = particleSystem.main;
			((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
			particleSystem.Stop(false);
		}
	}

	public void TrapActivate()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (trapTriggered)
		{
			return;
		}
		foreach (PhysGrabber item in physGrabObject.playerGrabbing.ToList())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				item.ReleaseObjectRPC(physGrabEnded: true, 1f);
				continue;
			}
			item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 1f });
		}
		trapActive = true;
		trapTriggered = true;
		biteBookTop.SetActive(true);
		biteBookBot.SetActive(true);
		closedBookTop.SetActive(false);
		closedBookBot.SetActive(false);
		chainLock.SetActive(false);
		lockBreak.Play(physGrabObject.centerPoint);
		lockParticle.Play(false);
		((Behaviour)animator).enabled = true;
	}
}
