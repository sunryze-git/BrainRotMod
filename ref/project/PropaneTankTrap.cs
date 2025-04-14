using System;
using UnityEngine;
using UnityEngine.Events;

public class PropaneTankTrap : Trap
{
	public UnityEvent tankTimer;

	private PhysGrabObject physgrabobject;

	private ParticleScriptExplosion particleScriptExplosion;

	[Space]
	public GameObject Tank;

	public Transform Center;

	[Space]
	[Header("Sounds")]
	public Sound Pop;

	public Sound FlyLoop;

	public Sound flyStart;

	public Sound flyEnd;

	[Space]
	private Quaternion initialTankRotation;

	private Rigidbody rb;

	private bool LoopPlaying;

	private Vector3 randomTorque;

	private int timeToTwist;

	public HurtCollider hurtCollider;

	public ParticleSystem smokeParticleSystem;

	public ParticleSystem fireParticleSystem;

	public GameObject fireLight;

	protected override void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		initialTankRotation = Tank.transform.localRotation;
		rb = ((Component)this).GetComponent<Rigidbody>();
		physgrabobject = ((Component)this).GetComponent<PhysGrabObject>();
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		((Component)hurtCollider).gameObject.SetActive(false);
	}

	protected override void Update()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
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
			((Component)hurtCollider).gameObject.SetActive(true);
			float num = 40f;
			float num2 = 1f * Mathf.Sin(Time.time * num);
			float num3 = 1f * Mathf.Sin(Time.time * num + MathF.PI / 2f);
			Tank.transform.localRotation = initialTankRotation * Quaternion.Euler(num2, 0f, num3);
			Tank.transform.localPosition = new Vector3(Tank.transform.localPosition.x, Tank.transform.localPosition.y - num2 * 0.005f * Time.deltaTime, Tank.transform.localPosition.z);
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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (!trapActive || !isLocal)
		{
			return;
		}
		rb.AddForce(-((Component)this).transform.forward * 0.3f * 40f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		rb.AddForce(Vector3.up * 0.1f * 10f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		rb.AddTorque(-((Component)this).transform.right * 0.1f * 10f * Time.fixedDeltaTime * 50f, (ForceMode)0);
		if (timeToTwist > 200)
		{
			Vector3 val = Random.insideUnitSphere;
			randomTorque = ((Vector3)(ref val)).normalized * Random.Range(0f, 0.5f);
			timeToTwist = 0;
			val = rb.velocity;
			if (((Vector3)(ref val)).magnitude < 0.5f && !physgrabobject.grabbed)
			{
				rb.AddForce(((Component)this).transform.forward * 5f, (ForceMode)1);
				rb.AddTorque(randomTorque * 20f, (ForceMode)1);
			}
		}
	}

	public void TrapStop()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		trapActive = false;
		flyEnd.Play(physgrabobject.centerPoint);
		LoopPlaying = false;
		((Component)hurtCollider).gameObject.SetActive(false);
		DeparentAndDestroy(smokeParticleSystem);
		DeparentAndDestroy(fireParticleSystem);
		fireLight.SetActive(false);
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
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!trapTriggered)
		{
			tankTimer.Invoke();
			fireParticleSystem.Play(false);
			fireLight.SetActive(true);
			flyStart.Play(physgrabobject.centerPoint);
			trapActive = true;
			trapTriggered = true;
		}
	}

	public void Explode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		particleScriptExplosion.Spawn(Center.position, 0.8f, 50, 100);
		DeparentAndDestroy(smokeParticleSystem);
		DeparentAndDestroy(fireParticleSystem);
	}
}
