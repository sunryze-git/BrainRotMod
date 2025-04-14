using UnityEngine;
using UnityEngine.Events;

public class WizardBroomValuable : Trap
{
	public enum States
	{
		Idle,
		MoveForward,
		Turn,
		Unstick,
		grabbed,
		Sleep
	}

	public UnityEvent broomTimer;

	private Rigidbody rb;

	private LayerMask visionObstruct;

	private Vector3 rayOffset = new Vector3(-1.92f, 0f, 0f);

	private float rayDistance = 1f;

	private float raycastCooldown = 0.2f;

	private float raycastTimer;

	private PhysGrabObjectImpactDetector impactDetector;

	public GameObject box;

	public GameObject broom;

	public ParticleSystem plankParticles;

	public ParticleSystem bitParticles;

	public Sound broomBoxBreakSound;

	private float stuckTimer = 2f;

	private float unStickTimer;

	private Vector3 randomDirection;

	private Vector3 randomTorque;

	public States currentState;

	private bool stateStart;

	private bool CheckForwardRaycast(out RaycastHit _hit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		_hit = default(RaycastHit);
		if (raycastTimer < raycastCooldown)
		{
			return false;
		}
		raycastTimer = 0f;
		Vector3 val = broom.transform.TransformPoint(rayOffset);
		Vector3 val2 = -broom.transform.right;
		Debug.DrawRay(val, val2, Color.red);
		return Physics.Raycast(val, val2, ref _hit, rayDistance, LayerMask.op_Implicit(visionObstruct));
	}

	protected override void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		rb = ((Component)this).GetComponent<Rigidbody>();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		visionObstruct = SemiFunc.LayerMaskGetVisionObstruct();
	}

	protected override void Update()
	{
		base.Update();
	}

	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (currentState)
			{
			case States.MoveForward:
				StateMoveForward();
				break;
			case States.Turn:
				StateTurn();
				break;
			case States.Unstick:
				StateUnstick();
				break;
			case States.grabbed:
				StateGrabbed();
				break;
			case States.Idle:
				StateIdle();
				break;
			case States.Sleep:
				StateSleep();
				break;
			}
			if (stuckTimer > 0f)
			{
				stuckTimer -= Time.fixedDeltaTime;
			}
			if (physGrabObject.grabbed && trapActive)
			{
				SetState(States.grabbed);
			}
			if (impactDetector.inCart && trapTriggered)
			{
				TrapStop();
			}
		}
	}

	private void StateMoveForward()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
		}
		raycastTimer += Time.fixedDeltaTime;
		physGrabObject.OverrideZeroGravity();
		if (stuckTimer <= 0f)
		{
			Vector3 velocity = rb.velocity;
			if (((Vector3)(ref velocity)).magnitude > 0.1f)
			{
				stuckTimer = Random.Range(0.1f, 2f);
				return;
			}
			SetState(States.Unstick);
		}
		if (CheckForwardRaycast(out var _) || Vector3.Dot(broom.transform.right, Vector3.up) > 0.1f)
		{
			SetState(States.Turn);
		}
		else
		{
			rb.AddForce(-broom.transform.right * 2000f * Time.fixedDeltaTime, (ForceMode)0);
		}
	}

	private void StateTurn()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			rb.AddForce(Vector3.up * 50f * Time.fixedDeltaTime, (ForceMode)1);
		}
		raycastTimer += Time.fixedDeltaTime;
		physGrabObject.OverrideZeroGravity();
		if (CheckForwardRaycast(out var _hit))
		{
			Vector3 val = Vector3.Cross(((RaycastHit)(ref _hit)).normal, broom.transform.right);
			rb.AddTorque(val * 200f * Time.fixedDeltaTime, (ForceMode)0);
			rb.AddForce(broom.transform.right * 500f * Time.fixedDeltaTime, (ForceMode)0);
		}
		else
		{
			SetState(States.MoveForward);
		}
	}

	private void StateUnstick()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			unStickTimer = Random.Range(1f, 2f);
			Vector3 val = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			randomDirection = ((Vector3)(ref val)).normalized;
			rb.AddForce(randomDirection * 500f * Time.fixedDeltaTime, (ForceMode)1);
			val = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			randomTorque = ((Vector3)(ref val)).normalized;
		}
		if (unStickTimer > 0f)
		{
			rb.AddTorque(randomTorque * 5000f * Time.fixedDeltaTime, (ForceMode)0);
			unStickTimer -= Time.fixedDeltaTime;
		}
		else
		{
			stuckTimer = Random.Range(0.1f, 2f);
			SetState(States.MoveForward);
		}
	}

	private void StateIdle()
	{
		stateStart = false;
	}

	private void StateGrabbed()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		physGrabObject.OverrideZeroGravity();
		physGrabObject.OverrideDrag(0.2f);
		physGrabObject.OverrideAngularDrag(0.2f);
		if (!physGrabObject.grabbed)
		{
			SetState(States.MoveForward);
		}
	}

	private void StateSleep()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		if (!impactDetector.inCart && trapTriggered)
		{
			trapActive = true;
			SetState(States.MoveForward);
		}
	}

	public void TrapActivate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!trapTriggered)
		{
			broomBoxBreakSound.Play(physGrabObject.centerPoint);
			plankParticles.Play();
			bitParticles.Play();
			box.SetActive(false);
			broom.SetActive(true);
			trapActive = true;
			trapTriggered = true;
			SetState(States.MoveForward);
		}
	}

	public void TrapStop()
	{
		trapActive = false;
		SetState(States.Sleep);
	}

	private void SetState(States state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			currentState = state;
			stateStart = true;
		}
	}
}
