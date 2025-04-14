using System;
using Photon.Pun;
using UnityEngine;

public class FrogTrap : Trap
{
	private PhysGrabObject physgrabobject;

	[Space]
	[Header("Frog Components")]
	public GameObject Frog;

	public GameObject FrogFeet;

	public GameObject FrogCrank;

	[Space]
	[Header("Sounds")]
	public Sound CrankStart;

	public Sound CrankEnd;

	public Sound CrankLoop;

	public Sound Jump;

	[Space]
	public AnimationCurve FrogJumpCurve;

	private float FrogJumpLerp;

	public float FrogJumpSpeed;

	private bool FrogJumpActive;

	public float FrogJumpIntensity;

	private Quaternion initialFrogRotation;

	private Rigidbody rb;

	private bool LoopPlaying;

	private bool everPickedUp;

	private float frogJumpTimer;

	private PhysGrabObjectImpactDetector impactDetector;

	private bool grabbedPrev;

	protected override void Start()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		initialFrogRotation = Frog.transform.localRotation;
		rb = ((Component)this).GetComponent<Rigidbody>();
		physgrabobject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	protected override void Update()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		CrankLoop.PlayLoop(LoopPlaying, 0.8f, 0.8f);
		if (physGrabObject.grabbed)
		{
			if (!grabbedPrev)
			{
				Jump.Play(physgrabobject.centerPoint);
				grabbedPrev = true;
				if (physGrabObject.grabbedLocal)
				{
					PhysGrabber.instance.OverrideGrabDistance(0.8f);
					if (SemiFunc.IsMasterClientOrSingleplayer())
					{
						Quaternion turnX = Quaternion.Euler(45f, 180f, 0f);
						Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
						Quaternion identity = Quaternion.identity;
						physGrabObject.TurnXYZ(turnX, turnY, identity);
					}
				}
			}
			everPickedUp = true;
			LoopPlaying = false;
			if (trapActive)
			{
				TrapStop();
			}
		}
		else
		{
			grabbedPrev = false;
			if (everPickedUp)
			{
				trapStart = true;
			}
		}
		if (trapStart && !impactDetector.inCart)
		{
			TrapActivate();
		}
		if (!trapActive || physGrabObject.grabbed)
		{
			return;
		}
		enemyInvestigate = true;
		LoopPlaying = true;
		if (FrogJumpActive)
		{
			FrogJumpLerp += FrogJumpSpeed * Time.deltaTime;
			if (FrogJumpLerp >= 1f)
			{
				FrogJumpLerp = 0f;
				FrogJumpActive = false;
			}
		}
		FrogFeet.transform.localEulerAngles = new Vector3(0f, 0f, FrogJumpCurve.Evaluate(FrogJumpLerp) * FrogJumpIntensity);
		FrogCrank.transform.Rotate(0f, 0f, 80f * Time.deltaTime);
		float num = 40f;
		float num2 = 1f * Mathf.Sin(Time.time * num);
		float num3 = 1f * Mathf.Sin(Time.time * num + MathF.PI / 2f);
		Frog.transform.localRotation = initialFrogRotation * Quaternion.Euler(num2, 0f, num3);
		Frog.transform.localPosition = new Vector3(Frog.transform.localPosition.x, Frog.transform.localPosition.y - num2 * 0.005f * Time.deltaTime, Frog.transform.localPosition.z);
		if (frogJumpTimer > 0f)
		{
			frogJumpTimer -= Time.deltaTime;
		}
		else if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("FrogJumpRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			FrogJump();
		}
	}

	public void FrogJump()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (impactDetector.inCart)
		{
			TrapStop();
			return;
		}
		frogJumpTimer = Random.Range(0.5f, 0.8f);
		enemyInvestigate = true;
		Jump.Play(physgrabobject.centerPoint);
		FrogJumpActive = true;
		FrogJumpLerp = 0f;
		enemyInvestigateRange = 15f;
		if (isLocal)
		{
			Vector3 insideUnitSphere;
			if (Vector3.Dot(Frog.transform.up, Vector3.up) > 0.5f)
			{
				rb.AddForce(Vector3.up * 1f, (ForceMode)1);
				rb.AddForce(((Component)this).transform.forward * 1.5f, (ForceMode)1);
				insideUnitSphere = Random.insideUnitSphere;
				Vector3 val = ((Vector3)(ref insideUnitSphere)).normalized * Random.Range(0.05f, 0.1f);
				val.z = 0f;
				val.x = 0f;
				rb.AddTorque(val * 0.25f, (ForceMode)1);
			}
			else
			{
				rb.AddForce(Vector3.up * 1f, (ForceMode)1);
				insideUnitSphere = Random.insideUnitSphere;
				Vector3 normalized = ((Vector3)(ref insideUnitSphere)).normalized;
				rb.AddTorque(normalized * 0.03f, (ForceMode)1);
			}
		}
	}

	[PunRPC]
	public void FrogJumpRPC()
	{
		FrogJump();
	}

	public void TrapStop()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		trapActive = false;
		trapStart = false;
		LoopPlaying = false;
		trapTriggered = false;
		CrankEnd.Play(physgrabobject.centerPoint);
	}

	public void TrapActivate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!trapTriggered)
		{
			CrankStart.Play(physgrabobject.centerPoint);
			trapActive = true;
			trapTriggered = true;
			frogJumpTimer = 0f;
		}
	}
}
