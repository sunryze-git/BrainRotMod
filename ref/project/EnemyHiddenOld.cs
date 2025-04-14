using System;
using Photon.Pun;
using UnityEngine;

public class EnemyHiddenOld : MonoBehaviour
{
	private enum State
	{
		Roam,
		PlayerNotice,
		GetPlayer,
		GoToTarget,
		PickUpTarget,
		FindFarawayPoint,
		KidnapTarget,
		TauntTarget,
		DropTarget,
		Despawn
	}

	private Vector3 startPosition;

	private Vector3 footStepPosition;

	public Materials.MaterialTrigger material;

	public Transform grounded;

	public Transform footstepParticlesTransform;

	public ParticleSystem footstepParticleSmoke;

	private bool rightFoot = true;

	private Vector3 previousPosition;

	public ParticleSystem footstepParticleFoot;

	private bool isSprinting;

	private int currentState;

	private bool settingState;

	private int stateSetTo = -1;

	private PhotonView photonView;

	private float stateTimer;

	private bool stateEnd;

	private bool stateStart;

	private float initialStateTime;

	private float sprintingTime;

	public ParticleSystem breathParticles;

	private float breathTimer;

	private bool isBreathing;

	private bool breatheIn = true;

	private float breathCycleTimer;

	public Sound soundBreatheIn;

	public Sound soundBreatheOut;

	public Sound soundFootstepWalk;

	public Sound soundFootstepSprint;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		photonView = ((Component)this).GetComponent<PhotonView>();
		startPosition = ((Component)this).transform.position;
		footStepPosition = startPosition;
		previousPosition = startPosition;
	}

	private void Update()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		StateRoam();
		StatePlayerNotice();
		StateGetPlayer();
		StateGoToTarget();
		StatePickUpTarget();
		StateFindFarawayPoint();
		StateKidnapTarget();
		StateTauntTarget();
		StateDropTarget();
		StateDespawn();
		FootstepLogic();
		SprintTick();
		BreathTick();
		Breathing();
		stateEnd = false;
		if (stateTimer > 0f)
		{
			if (initialStateTime == 0f)
			{
				initialStateTime = stateTimer;
			}
			stateTimer -= Time.deltaTime;
			stateTimer = Mathf.Max(0f, stateTimer);
		}
		else if (!stateEnd && stateTimer != -123f)
		{
			stateEnd = true;
			stateTimer = -123f;
			initialStateTime = 0f;
		}
		if (stateSetTo != -1)
		{
			currentState = stateSetTo;
			stateStart = true;
			settingState = false;
			stateEnd = false;
			stateSetTo = -1;
		}
		float num = 0.5f;
		((Component)this).transform.position = startPosition + new Vector3(Mathf.Sin(Time.time * num) * 1f, 0f, Mathf.Cos(Time.time * num) * 1f);
	}

	private void FootstepLogic()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - previousPosition;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		((Component)this).transform.LookAt(((Component)this).transform.position + normalized);
		Debug.DrawRay(((Component)this).transform.position, normalized, Color.green, 0.1f);
		previousPosition = ((Component)this).transform.position;
		float num = 1f;
		if (isSprinting)
		{
			num = 1.8f;
		}
		if (!(Vector3.Distance(footStepPosition, grounded.position) > 0.5f * num))
		{
			return;
		}
		Vector3 val2 = Vector3.Cross(Vector3.up, ((Component)this).transform.forward);
		Vector3 val3 = -val2;
		Vector3 val4 = Vector3.down + (rightFoot ? (val2 * 0.2f) : (val3 * 0.2f));
		val4 += ((Component)this).transform.forward * 0.3f * num;
		rightFoot = !rightFoot;
		Debug.DrawRay(((Component)this).transform.position, val4, Color.red, 0.1f);
		RaycastHit val5 = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position, val4 * 2f, ref val5, 3f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			footStepPosition = grounded.position;
			footstepParticlesTransform.position = ((RaycastHit)(ref val5)).point;
			footstepParticleSmoke.Play();
			((Component)footstepParticlesTransform).transform.LookAt(((Component)this).transform.position + normalized);
			footstepParticleFoot.Play();
			if (isSprinting)
			{
				Materials.Instance.Impulse(((RaycastHit)(ref val5)).point, Vector3.down, Materials.SoundType.Heavy, footstep: true, material, Materials.HostType.Enemy);
				soundFootstepSprint.Play(((RaycastHit)(ref val5)).point);
			}
			else
			{
				Materials.Instance.Impulse(((RaycastHit)(ref val5)).point, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
				soundFootstepWalk.Play(((RaycastHit)(ref val5)).point);
			}
			Quaternion.LookRotation(((Component)this).transform.forward);
			Debug.DrawRay(((Component)this).transform.position, normalized, Color.blue, 2f);
			MainModule main = footstepParticleFoot.main;
			((MainModule)(ref main)).startRotation3D = true;
			Vector2 val6 = default(Vector2);
			((Vector2)(ref val6))._002Ector(((Component)this).transform.forward.x, ((Component)this).transform.forward.z);
			float num2 = Vector2.SignedAngle(Vector2.up, val6) + 90f;
			float num3 = (rightFoot ? (-90f) : 90f) * (MathF.PI / 180f);
			float num4 = (rightFoot ? (-90f) : 90f);
			num4 += num2;
			num4 *= MathF.PI / 180f;
			((MainModule)(ref main)).startRotationX = new MinMaxCurve(num3);
			((MainModule)(ref main)).startRotationY = new MinMaxCurve(num4);
			((MainModule)(ref main)).startRotationZ = new MinMaxCurve(0f);
		}
	}

	private void StateSet(State newState)
	{
		if (settingState)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient() && stateSetTo == -1)
			{
				settingState = true;
				photonView.RPC("StateSetRPC", (RpcTarget)0, new object[1] { (int)newState });
			}
		}
		else if (stateSetTo == -1)
		{
			settingState = true;
			StateSetRPC((int)newState);
		}
	}

	[PunRPC]
	public void StateSetRPC(int state)
	{
		stateSetTo = state;
		stateTimer = 0f;
		stateEnd = true;
	}

	private bool StateIs(State state)
	{
		return currentState == (int)state;
	}

	private void Sprinting()
	{
		sprintingTime = 0.2f;
		isSprinting = true;
	}

	private void SprintTick()
	{
		if (sprintingTime > 0f)
		{
			sprintingTime -= Time.deltaTime;
		}
		else
		{
			isSprinting = false;
		}
	}

	private void Breathing()
	{
		breathTimer = 0.2f;
		isBreathing = true;
	}

	private void BreathTick()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (breathTimer > 0f)
		{
			breathTimer -= Time.deltaTime;
		}
		else
		{
			isBreathing = false;
		}
		if (!isBreathing)
		{
			return;
		}
		breathCycleTimer += Time.deltaTime;
		float num = 3f;
		if (breatheIn)
		{
			num = 4.5f;
		}
		if (breathCycleTimer > num)
		{
			breathCycleTimer = 0f;
			if (breatheIn)
			{
				BreatheIn();
			}
			else
			{
				BreatheOut();
			}
			breatheIn = !breatheIn;
		}
	}

	private void BreatheIn()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("BreatheInRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			BreatheInRPC();
		}
	}

	[PunRPC]
	public void BreatheInRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundBreatheIn.Play(((Component)this).transform.position);
	}

	private void BreatheOut()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("BreatheOutRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			BreatheOutRPC();
		}
	}

	[PunRPC]
	public void BreatheOutRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundBreatheOut.Play(((Component)this).transform.position);
		breathParticles.Play();
	}

	private void StateRoam()
	{
		if (StateIs(State.Roam))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StatePlayerNotice()
	{
		if (StateIs(State.PlayerNotice))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateGetPlayer()
	{
		if (StateIs(State.GetPlayer))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateGoToTarget()
	{
		if (StateIs(State.GoToTarget))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StatePickUpTarget()
	{
		if (StateIs(State.PickUpTarget))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateFindFarawayPoint()
	{
		if (StateIs(State.FindFarawayPoint))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateKidnapTarget()
	{
		if (StateIs(State.KidnapTarget))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateTauntTarget()
	{
		if (StateIs(State.TauntTarget))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateDropTarget()
	{
		if (StateIs(State.DropTarget))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}

	private void StateDespawn()
	{
		if (StateIs(State.Despawn))
		{
			if (stateStart)
			{
				stateStart = false;
			}
			_ = stateEnd;
		}
	}
}
