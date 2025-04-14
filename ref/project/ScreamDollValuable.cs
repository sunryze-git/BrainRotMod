using Photon.Pun;
using UnityEngine;

public class ScreamDollValuable : MonoBehaviour
{
	public enum States
	{
		Idle,
		Active
	}

	private Animator animator;

	private PhysGrabObject physGrabObject;

	private Rigidbody rb;

	public Sound soundScreamLoop;

	private PhotonView photonView;

	internal States currentState;

	private bool stateStart;

	private bool loopPlaying;

	private void StateActive()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
		}
		loopPlaying = true;
		animator.SetBool("active", true);
		((Behaviour)animator).enabled = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 100) < 7)
			{
				rb.AddForce(Random.insideUnitSphere * 3f, (ForceMode)1);
				rb.AddTorque(Random.insideUnitSphere * 7f, (ForceMode)1);
			}
			Quaternion turnX = Quaternion.Euler(0f, 180f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
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
				physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
			if (!physGrabObject.grabbed)
			{
				SetState(States.Idle);
			}
		}
		if (physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverridePullDistanceIncrement(-1f * Time.fixedDeltaTime);
		}
	}

	private void StateIdle()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		loopPlaying = false;
		animator.SetBool("active", false);
		if (SemiFunc.IsMasterClientOrSingleplayer() && physGrabObject.grabbed)
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

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		animator = ((Component)this).GetComponent<Animator>();
	}

	private void Update()
	{
		soundScreamLoop.PlayLoop(loopPlaying, 5f, 5f);
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

	public void EnemyInvestigate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 20f);
		}
	}

	public void StopAnimator()
	{
		((Behaviour)animator).enabled = false;
	}

	public void OnHurtColliderHitEnemy()
	{
		physGrabObject.heavyBreakImpulse = true;
	}
}
