using System;
using UnityEngine;
using UnityEngine.Events;

public class BottleTrap : Trap
{
	public UnityEvent bottleTimer;

	private PhysGrabObject physgrabobject;

	[Space]
	[Header("Bottle Components")]
	public GameObject Bottle;

	public GameObject Cork;

	[Space]
	[Header("Sounds")]
	public Sound Pop;

	public Sound FlyLoop;

	[Space]
	private Quaternion initialBottleRotation;

	private Rigidbody rb;

	private bool LoopPlaying;

	private Vector3 randomTorque;

	private int timeToTwist;

	public ParticleSystem bottleParticleSystem;

	public ParticleSystem corkParticleSystem;

	protected override void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		initialBottleRotation = Bottle.transform.localRotation;
		rb = ((Component)this).GetComponent<Rigidbody>();
		physgrabobject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	protected override void Update()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		FlyLoop.PlayLoop(LoopPlaying, 0.8f, 0.8f);
		if (trapStart)
		{
			TrapActivate();
		}
		if (trapActive)
		{
			enemyInvestigate = true;
			enemyInvestigateRange = 15f;
			LoopPlaying = true;
			float num = 40f;
			float num2 = 1f * Mathf.Sin(Time.time * num);
			float num3 = 1f * Mathf.Sin(Time.time * num + MathF.PI / 2f);
			Bottle.transform.localRotation = initialBottleRotation * Quaternion.Euler(num2, 0f, num3);
			Bottle.transform.localPosition = new Vector3(Bottle.transform.localPosition.x, Bottle.transform.localPosition.y - num2 * 0.005f * Time.deltaTime, Bottle.transform.localPosition.z);
		}
	}

	private void FixedUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (!trapActive || !isLocal)
		{
			return;
		}
		rb.AddForce(-((Component)this).transform.up * 0.45f * 40f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		rb.AddForce(Vector3.up * 0.15f * 10f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		rb.AddTorque(randomTorque * 30f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		if (timeToTwist > 200)
		{
			Vector3 val = Random.insideUnitSphere;
			randomTorque = ((Vector3)(ref val)).normalized * Random.Range(0f, 0.5f);
			timeToTwist = 0;
			val = rb.velocity;
			if (((Vector3)(ref val)).magnitude < 0.5f && !physgrabobject.grabbed)
			{
				rb.AddForce(((Component)this).transform.up * 5f, (ForceMode)1);
				rb.AddTorque(randomTorque * 20f, (ForceMode)1);
			}
		}
	}

	public void TrapStop()
	{
		trapActive = false;
		LoopPlaying = false;
		DeparentAndDestroy(bottleParticleSystem);
	}

	private void DeparentAndDestroy(ParticleSystem particleSystem)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)particleSystem != (Object)null && particleSystem.isPlaying)
		{
			((Component)particleSystem).gameObject.transform.parent = null;
			MainModule main = particleSystem.main;
			((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
			particleSystem.Stop(false);
			particleSystem = null;
		}
	}

	public void IncrementTimeToTwist()
	{
		timeToTwist++;
	}

	public void TrapActivate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!trapTriggered)
		{
			Pop.Play(physgrabobject.centerPoint);
			bottleTimer.Invoke();
			Cork.SetActive(false);
			corkParticleSystem.Play(false);
			bottleParticleSystem.Play(false);
			trapActive = true;
			trapTriggered = true;
		}
	}

	private void OnDestroy()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)bottleParticleSystem))
		{
			((Component)bottleParticleSystem).transform.parent = null;
			bottleParticleSystem.Stop(true);
			MainModule main = bottleParticleSystem.main;
			((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
		}
	}
}
