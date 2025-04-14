using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class IceSawValuable : Trap
{
	public enum States
	{
		Idle,
		Active
	}

	public Sound soundBladeLoop;

	public Sound soundBladeStart;

	public Sound soundBladeEnd;

	[Space]
	public Transform meshTransform;

	public UnityEvent sawTimer;

	public Transform blade;

	public AnimationCurve bladeCurve;

	public float bladeSpeed;

	private float bladeMaxSpeed = 1500f;

	private float bladeLerp;

	private float secondsToStart = 2f;

	private float secondsToStop = 2f;

	public HurtCollider hurtCollider;

	public Collider triggerCollider;

	private float overLapBoxCheckTimer;

	public ParticleSystem sparkParticles;

	internal States currentState;

	private bool stateStart;

	[Space]
	private Quaternion initialTankRotation;

	private Animator animator;

	private PhysGrabObject physgrabobject;

	private Rigidbody rb;

	private bool loopPlaying;

	protected override void Start()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		physgrabobject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		animator = ((Component)this).GetComponent<Animator>();
		initialTankRotation = ((Component)meshTransform).transform.localRotation;
	}

	private void FixedUpdate()
	{
		switch (currentState)
		{
		case States.Active:
			StateActive();
			break;
		case States.Idle:
			StateIdle();
			break;
		}
	}

	protected override void Update()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (trapStart)
		{
			TrapActivate();
		}
		soundBladeLoop.PlayLoop(loopPlaying, 5f, 5f);
		blade.Rotate(-Vector3.right * bladeSpeed * Time.deltaTime);
		bladeSpeed = Mathf.Lerp(0f, bladeMaxSpeed, bladeCurve.Evaluate(bladeLerp));
		float num = 0.3f * bladeCurve.Evaluate(bladeLerp);
		float num2 = 60f * bladeCurve.Evaluate(bladeLerp);
		float num3 = num * Mathf.Sin(Time.time * num2);
		float num4 = num * Mathf.Sin(Time.time * num2 + MathF.PI / 2f);
		((Component)meshTransform).transform.localRotation = initialTankRotation * Quaternion.Euler(num3, 0f, num4);
		((Component)meshTransform).transform.localPosition = new Vector3(((Component)meshTransform).transform.localPosition.x, ((Component)meshTransform).transform.localPosition.y - num3 * 0.005f * Time.deltaTime, ((Component)meshTransform).transform.localPosition.z);
		if (currentState == States.Active)
		{
			if (bladeLerp < 1f)
			{
				bladeLerp += Time.deltaTime / secondsToStart;
			}
			else
			{
				bladeLerp = 1f;
			}
		}
		else if (bladeLerp > 0f)
		{
			bladeLerp -= Time.deltaTime / secondsToStop;
		}
		else
		{
			bladeLerp = 0f;
		}
	}

	private void StateActive()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			((Component)hurtCollider).gameObject.SetActive(true);
			soundBladeStart.Play(physgrabobject.centerPoint);
			loopPlaying = true;
			stateStart = false;
		}
		overLapBoxCheckTimer += Time.deltaTime;
		if (overLapBoxCheckTimer >= 0.1f)
		{
			Bounds bounds = triggerCollider.bounds;
			Vector3 val = ((Bounds)(ref bounds)).size * 0.5f;
			val.x *= Mathf.Abs(((Component)this).transform.lossyScale.x);
			val.y *= Mathf.Abs(((Component)this).transform.lossyScale.y);
			val.z *= Mathf.Abs(((Component)this).transform.lossyScale.z);
			bounds = triggerCollider.bounds;
			if (Physics.OverlapBox(((Bounds)(ref bounds)).center, val / 2f, ((Component)triggerCollider).transform.rotation, LayerMask.GetMask(new string[1] { "Default" }), (QueryTriggerInteraction)2).Length != 0)
			{
				Sparks();
			}
			enemyInvestigate = true;
			enemyInvestigateRange = 15f;
			overLapBoxCheckTimer = 0f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			rb.AddTorque(((Component)this).transform.up * 1f * Time.fixedDeltaTime * 30f, (ForceMode)0);
			if (Random.Range(0, 100) < 7)
			{
				rb.AddForce(Random.insideUnitSphere * 0.5f, (ForceMode)1);
				rb.AddTorque(Random.insideUnitSphere * 0.1f, (ForceMode)1);
			}
			if (!physgrabobject.grabbed && !trapActive)
			{
				SetState(States.Idle);
			}
		}
	}

	private void StateIdle()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			((Component)hurtCollider).gameObject.SetActive(false);
			loopPlaying = false;
			soundBladeEnd.Play(physgrabobject.centerPoint);
			stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && (physgrabobject.grabbed || trapActive))
		{
			SetState(States.Active);
		}
	}

	[PunRPC]
	public void SetStateRPC(States state)
	{
		currentState = state;
		stateStart = true;
	}

	private void SetState(States state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SetStateRPC(state);
				return;
			}
			photonView.RPC("SetStateRPC", (RpcTarget)0, new object[1] { state });
		}
	}

	public void TrapActivate()
	{
		if (!trapTriggered)
		{
			sawTimer.Invoke();
			trapActive = true;
			trapTriggered = true;
		}
	}

	public void TrapStop()
	{
		trapActive = false;
	}

	public void Sparks()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		sparkParticles.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			rb.AddForce(((Component)this).transform.right * 2f, (ForceMode)1);
		}
	}

	public void ImpactDamage()
	{
		physGrabObject.lightBreakImpulse = true;
	}
}
