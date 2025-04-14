using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDuck : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		Notice,
		GoToPlayer,
		GoToPlayerOver,
		GoToPlayerUnder,
		FlyBackToNavmesh,
		FlyBackToNavmeshStop,
		MoveBackToNavmesh,
		AttackStart,
		Transform,
		ChaseNavmesh,
		ChaseTowards,
		ChaseMoveBack,
		DeTransform,
		Leave,
		Stun,
		Despawn
	}

	private PhotonView photonView;

	public State currentState;

	private bool stateImpulse;

	public float stateTimer;

	private float stateTicker;

	private Vector3 targetPosition;

	private float pitCheckTimer;

	private bool pitCheck;

	private Vector3 agentDestination;

	private bool visionPrevious;

	private float visionTimer;

	private Vector3 moveBackPosition;

	private float moveBackTimer;

	private float targetForwardOffset = 1.5f;

	public Transform followOffsetTransform;

	[Space]
	public SpringQuaternion bodySpring;

	public Transform bodyTransform;

	public Transform bodyTargetTransform;

	[Space]
	public EnemyDuckAnim anim;

	public Enemy enemy;

	public ParticleSystem featherParticles;

	private PlayerAvatar playerTarget;

	[Space]
	private Quaternion rotationTarget;

	public SpringQuaternion rotationSpring;

	[Space]
	public SpringQuaternion headLookAtSpring;

	public Transform headLookAtTarget;

	public Transform headLookAtSource;

	private float targetedPlayerTime;

	private float targetedPlayerTimeMax = 120f;

	internal bool idleBreakerTrigger;

	private float chaseTimer;

	private float annoyingJumpPauseTimer;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		bodyTransform.rotation = SemiFunc.SpringQuaternionGet(bodySpring, bodyTargetTransform.rotation);
		HeadLookAtLogic();
		if ((GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient) || !LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (enemy.IsStunned())
		{
			UpdateState(State.Stun);
		}
		else if (enemy.CurrentState == EnemyState.Despawn)
		{
			UpdateState(State.Despawn);
		}
		if (!Object.op_Implicit((Object)(object)playerTarget))
		{
			if (currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder)
			{
				UpdateState(State.Idle);
			}
			else if (currentState == State.ChaseNavmesh || currentState == State.ChaseTowards || currentState == State.ChaseMoveBack || currentState == State.Transform)
			{
				UpdateState(State.DeTransform);
			}
		}
		RotationLogic();
		TimerLogic();
		GravityLogic();
		TargetPositionLogic();
		FollowOffsetLogic();
		FlyBackConditionLogic();
		switch (currentState)
		{
		case State.Spawn:
			StateSpawn();
			break;
		case State.Idle:
			StateIdle();
			break;
		case State.Roam:
			StateRoam();
			break;
		case State.Investigate:
			StateInvestigate();
			break;
		case State.Notice:
			StateNotice();
			break;
		case State.GoToPlayer:
			StateGoToPlayer();
			break;
		case State.GoToPlayerUnder:
			StateGoToPlayerUnder();
			break;
		case State.GoToPlayerOver:
			StateGoToPlayerOver();
			break;
		case State.MoveBackToNavmesh:
			StateMoveBackToNavMesh();
			break;
		case State.FlyBackToNavmesh:
			StateFlyBackToNavmesh();
			break;
		case State.FlyBackToNavmeshStop:
			StateFlyBackToNavmeshStop();
			break;
		case State.AttackStart:
			StateAttackStart();
			break;
		case State.Transform:
			StateTransform();
			break;
		case State.ChaseNavmesh:
			StateChaseNavmesh();
			break;
		case State.ChaseTowards:
			StateChaseTowards();
			break;
		case State.ChaseMoveBack:
			StateChaseMoveBack();
			break;
		case State.DeTransform:
			StateDeTransform();
			break;
		case State.Stun:
			StateStun();
			break;
		case State.Leave:
			StateLeave();
			break;
		case State.Despawn:
			StateDespawn();
			break;
		}
	}

	private void StateSpawn()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateImpulse = false;
			stateTimer = 2f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateIdle()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(2f, 5f);
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		if (!SemiFunc.EnemySpawnIdlePause())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Roam);
			}
			LeaveCheck(_setLeave: true);
		}
	}

	private void StateRoam()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 10f, 25f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 0f, 999f);
			}
			NavMeshHit val = default(NavMeshHit);
			if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				enemy.NavMeshAgent.SetDestination(((NavMeshHit)(ref val)).position);
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			SemiFunc.EnemyCartJump(enemy);
			MoveBackPosition();
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f || !enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(enemy);
				UpdateState(State.Idle);
			}
		}
		LeaveCheck(_setLeave: true);
	}

	private void StateInvestigate()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
			SemiFunc.EnemyCartJump(enemy);
			MoveBackPosition();
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f || !enemy.NavMeshAgent.HasPath())
			{
				SemiFunc.EnemyCartJumpReset(enemy);
				UpdateState(State.Idle);
			}
		}
		LeaveCheck(_setLeave: true);
	}

	private void StateNotice()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
			stateTimer = 1f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.GoToPlayer);
		}
	}

	private void StateGoToPlayer()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
			annoyingJumpPauseTimer = 1f;
		}
		enemy.NavMeshAgent.SetDestination(targetPosition);
		stateTimer -= Time.deltaTime;
		SemiFunc.EnemyCartJump(enemy);
		MoveBackPosition();
		enemy.Vision.StandOverride(0.25f);
		if (stateTimer <= 0f || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.Idle);
			return;
		}
		NavMeshHit val = default(NavMeshHit);
		if (!enemy.NavMeshAgent.CanReach(targetPosition, 1f) && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f && !VisionBlocked() && !NavMesh.SamplePosition(targetPosition, ref val, 0.5f, -1))
		{
			if (playerTarget.isCrawling && Mathf.Abs(targetPosition.y - ((Component)enemy.Rigidbody).transform.position.y) < 0.3f && !enemy.Jump.jumping)
			{
				UpdateState(State.GoToPlayerUnder);
				return;
			}
			if (targetPosition.y > ((Component)enemy.Rigidbody).transform.position.y)
			{
				UpdateState(State.GoToPlayerOver);
				return;
			}
		}
		AnnoyingJump();
		LeaveCheck(_setLeave: true);
	}

	private void StateGoToPlayerUnder()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
			annoyingJumpPauseTimer = 1f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
		SemiFunc.EnemyCartJump(enemy);
		enemy.Vision.StandOverride(0.25f);
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(targetPosition, ref val, 0.5f, -1))
		{
			UpdateState(State.MoveBackToNavmesh);
		}
		else if (VisionBlocked() || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.MoveBackToNavmesh);
			}
		}
		else
		{
			stateTimer = 2f;
		}
		if (LeaveCheck(_setLeave: false))
		{
			UpdateState(State.MoveBackToNavmesh);
		}
	}

	private void StateGoToPlayerOver()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
			annoyingJumpPauseTimer = 1f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
		SemiFunc.EnemyCartJump(enemy);
		enemy.Vision.StandOverride(0.25f);
		if (playerTarget.PlayerVisionTarget.VisionTransform.position.y > ((Component)enemy.Rigidbody).transform.position.y + 1.5f)
		{
			if (!enemy.Jump.jumping)
			{
				Vector3 val = playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				enemy.Jump.StuckTrigger(normalized);
				((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
				((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, 2f);
			}
		}
		else
		{
			AnnoyingJump();
		}
		NavMeshHit val2 = default(NavMeshHit);
		if (NavMesh.SamplePosition(targetPosition, ref val2, 0.5f, -1))
		{
			UpdateState(State.MoveBackToNavmesh);
		}
		else if (VisionBlocked() || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f || enemy.Rigidbody.notMovingTimer > 1f)
			{
				UpdateState(State.MoveBackToNavmesh);
			}
		}
		else
		{
			stateTimer = 2f;
		}
		if (LeaveCheck(_setLeave: false))
		{
			UpdateState(State.MoveBackToNavmesh);
		}
	}

	private void StateMoveBackToNavMesh()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 30f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		if (!enemy.Jump.jumping)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(enemy);
		enemy.Vision.StandOverride(0.25f);
		if ((Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) > 2f || enemy.Rigidbody.notMovingTimer > 2f) && !enemy.Jump.jumping)
		{
			Vector3 val = moveBackPosition - ((Component)enemy.Rigidbody).transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			enemy.Jump.StuckTrigger(normalized);
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
			Transform transform = ((Component)this).transform;
			transform.position += normalized * 2f;
		}
		stateTimer -= Time.deltaTime;
		NavMeshHit val2 = default(NavMeshHit);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, moveBackPosition) <= 0f || NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val2, 0.5f, -1))
		{
			UpdateState(State.GoToPlayer);
		}
		else if (stateTimer <= 0f)
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
			UpdateState(State.Despawn);
		}
	}

	private void StateFlyBackToNavmesh()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 30f;
			stateTicker = 0f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition + Vector3.up * 0.5f, 0.75f * Time.deltaTime);
		enemy.Rigidbody.OverrideFollowPosition(0.1f, 1f);
		enemy.Rigidbody.OverrideFollowRotation(0.1f, 0.25f);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)this).transform.position) > 2f)
		{
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		if (stateTicker <= 0f)
		{
			stateTicker = 0.25f;
			RaycastHit val = default(RaycastHit);
			NavMeshHit val2 = default(NavMeshHit);
			if (Physics.Raycast(((Component)enemy.Rigidbody).transform.position, Vector3.down, ref val, 5f, LayerMask.GetMask(new string[1] { "Default" })) && NavMesh.SamplePosition(((RaycastHit)(ref val)).point, ref val2, 0.5f, -1))
			{
				moveBackPosition = ((NavMeshHit)(ref val2)).position;
				UpdateState(State.FlyBackToNavmeshStop);
				return;
			}
		}
		else
		{
			stateTicker -= Time.deltaTime;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Despawn);
		}
	}

	private void StateFlyBackToNavmeshStop()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateTimer = 1f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateAttackStart()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 0.5f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.Rigidbody.GrabRelease();
			UpdateState(State.Transform);
		}
	}

	private void StateTransform()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1.4f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(((Component)this).transform.position.x, playerTarget.PlayerVisionTarget.VisionTransform.position.y, ((Component)this).transform.position.z);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, val, Time.deltaTime);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.ChaseNavmesh);
		}
	}

	private void StateChaseNavmesh()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
		}
		enemy.NavMeshAgent.OverrideAgent(10f, 10f, 0.1f);
		enemy.NavMeshAgent.SetDestination(((Component)playerTarget).transform.position);
		if (!VisionBlocked())
		{
			UpdateState(State.ChaseTowards);
			return;
		}
		MoveBackPosition();
		ChaseStop();
	}

	private void StateChaseTowards()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
		}
		if (VisionBlocked())
		{
			UpdateState(State.ChaseMoveBack);
			return;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, playerTarget.localCameraPosition + Vector3.down * 0.31f, 5f * Time.deltaTime);
		ChaseStop();
	}

	private void StateChaseMoveBack()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			RaycastHit val = default(RaycastHit);
			NavMeshHit val2 = default(NavMeshHit);
			if (Physics.Raycast(((Component)enemy.Rigidbody).transform.position, Vector3.down, ref val, 5f, LayerMask.GetMask(new string[1] { "Default" })) && NavMesh.SamplePosition(((RaycastHit)(ref val)).point, ref val2, 0.5f, -1))
			{
				moveBackPosition = ((NavMeshHit)(ref val2)).position;
			}
			stateImpulse = false;
			stateTimer = 10f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition, 5f * Time.deltaTime);
		if (Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) > 2f || enemy.Rigidbody.notMovingTimer > 2f)
		{
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		stateTimer -= Time.deltaTime;
		NavMeshHit val3 = default(NavMeshHit);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, moveBackPosition) <= 1f || NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val3, 0.5f, -1))
		{
			UpdateState(State.ChaseNavmesh);
		}
		else
		{
			ChaseStop();
		}
	}

	private void StateDeTransform()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateImpulse = false;
			stateTimer = 2f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.MoveBackToNavmesh);
		}
	}

	private void StateStun()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			return;
		}
		PlayerAvatar playerAvatar = null;
		float num = 999f;
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			float num2 = Vector3.Distance(((Component)this).transform.position, ((Component)item).transform.position);
			if (num2 < 10f && num2 < num)
			{
				num = num2;
				playerAvatar = item;
			}
		}
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			if (Object.op_Implicit((Object)(object)enemy.Vision.onVisionTriggeredPlayer))
			{
				playerTarget = enemy.Vision.onVisionTriggeredPlayer;
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
				}
				UpdateState(State.AttackStart);
			}
		}
		else
		{
			UpdateState(State.Idle);
		}
	}

	private void StateLeave()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, 30f, 50f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f);
			}
			NavMeshHit val = default(NavMeshHit);
			if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 1f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				agentDestination = ((NavMeshHit)(ref val)).position;
				flag = true;
			}
			if (flag)
			{
				enemy.NavMeshAgent.SetDestination(agentDestination);
				enemy.Rigidbody.notMovingTimer = 0f;
				stateImpulse = false;
			}
		}
		else
		{
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			SemiFunc.EnemyCartJump(enemy);
			if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f || stateTimer <= 0f)
			{
				SemiFunc.EnemyCartJumpReset(enemy);
				UpdateState(State.Idle);
			}
		}
	}

	private void StateDespawn()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateImpulse = false;
		}
	}

	public void OnInvestigate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && (currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate))
		{
			agentDestination = enemy.StateInvestigate.onInvestigateTriggeredPosition;
			UpdateState(State.Investigate);
		}
	}

	public void OnVision()
	{
		if ((currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate) && !enemy.Jump.jumping)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				playerTarget = enemy.Vision.onVisionTriggeredPlayer;
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
				}
				UpdateState(State.Notice);
			}
		}
		else if (currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder)
		{
			stateTimer = 2f;
		}
	}

	public void OnHurt()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		anim.soundHurtPauseTimer = 0.5f;
		anim.hurtSound.Play(enemy.CenterTransform.position);
		if (!enemy.IsStunned() && Object.op_Implicit((Object)(object)playerTarget) && (currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder))
		{
			UpdateState(State.AttackStart);
		}
	}

	public void OnDeath()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.05f);
		((Component)featherParticles).transform.position = enemy.CenterTransform.position;
		featherParticles.Play();
		anim.deathSound.Play(enemy.CenterTransform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState != State.AttackStart && currentState != State.Transform && currentState != State.ChaseNavmesh && currentState != State.ChaseTowards && currentState != State.ChaseMoveBack && currentState != State.DeTransform && currentState != State.Stun && currentState != State.Despawn)
		{
			playerTarget = enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			UpdateState(State.AttackStart);
		}
	}

	public void OnObjectHurt()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && Object.op_Implicit((Object)(object)enemy.Health.onObjectHurtPlayer) && currentState != State.AttackStart && currentState != State.Transform && currentState != State.ChaseNavmesh && currentState != State.ChaseTowards && currentState != State.ChaseMoveBack && currentState != State.DeTransform && currentState != State.Stun && currentState != State.Despawn)
		{
			playerTarget = enemy.Health.onObjectHurtPlayer;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			UpdateState(State.AttackStart);
		}
	}

	private void UpdateState(State _state)
	{
		if (currentState != _state)
		{
			currentState = _state;
			stateImpulse = true;
			stateTimer = 0f;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdateStateRPC", (RpcTarget)0, new object[1] { currentState });
			}
			else
			{
				UpdateStateRPC(currentState);
			}
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			UpdateState(State.Spawn);
		}
	}

	public void TargetPositionLogic()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder) && Object.op_Implicit((Object)(object)playerTarget))
		{
			Vector3 val = ((currentState != State.GoToPlayer && currentState != State.GoToPlayerUnder && currentState != State.GoToPlayerOver) ? (((Component)playerTarget).transform.position + ((Component)playerTarget).transform.forward * targetForwardOffset) : (((Component)playerTarget).transform.position + ((Component)playerTarget).transform.forward * 1.5f));
			if (pitCheckTimer <= 0f)
			{
				pitCheckTimer = 0.1f;
				pitCheck = !Physics.Raycast(val + Vector3.up, Vector3.down, 4f, LayerMask.GetMask(new string[1] { "Default" }));
			}
			else
			{
				pitCheckTimer -= Time.deltaTime;
			}
			if (pitCheck)
			{
				val = ((Component)playerTarget).transform.position;
			}
			targetPosition = Vector3.Lerp(targetPosition, val, 20f * Time.deltaTime);
		}
	}

	private void AnnoyingJump()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if (!enemy.Jump.jumping && !(annoyingJumpPauseTimer > 0f) && playerTarget.PlayerVisionTarget.VisionTransform.position.y > ((Component)enemy.Rigidbody).transform.position.y && enemy.Rigidbody.timeSinceStun > 2f)
		{
			Vector3 val = playerTarget.localCameraTransform.position + playerTarget.localCameraTransform.forward;
			((Vector3)(ref val))._002Ector(((Component)enemy.Rigidbody).transform.position.x, val.y, ((Component)enemy.Rigidbody).transform.position.z);
			float num = val.y - enemy.CenterTransform.position.y;
			if (!enemy.OnScreen.GetOnScreen(playerTarget) && num > 1f && !playerTarget.isMoving)
			{
				enemy.Jump.StuckTrigger(targetPosition - enemy.Vision.VisionTransform.position);
			}
		}
	}

	private void RotationLogic()
	{
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerTarget) && (currentState == State.Notice || currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder))
		{
			if ((!VisionBlocked() && !playerTarget.isMoving && ((Vector3)(ref enemy.Rigidbody.velocity)).magnitude < 0.5f) || enemy.Jump.jumping)
			{
				rotationTarget = Quaternion.LookRotation(((Component)playerTarget).transform.position - ((Component)enemy.Rigidbody).transform.position);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
			else if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.Rigidbody.velocity)).normalized);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
		}
		else if (Object.op_Implicit((Object)(object)playerTarget) && (currentState == State.ChaseNavmesh || currentState == State.ChaseTowards || currentState == State.Transform))
		{
			rotationTarget = Quaternion.LookRotation(((Component)playerTarget).transform.position - ((Component)enemy.Rigidbody).transform.position);
			((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(((Quaternion)(ref rotationTarget)).eulerAngles.x, ((Quaternion)(ref rotationTarget)).eulerAngles.y, ((Quaternion)(ref rotationTarget)).eulerAngles.z);
		}
		else if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.1f)
		{
			rotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.Rigidbody.velocity)).normalized);
			if (currentState != State.ChaseMoveBack)
			{
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(rotationSpring, rotationTarget);
	}

	private void GravityLogic()
	{
		if (currentState == State.ChaseNavmesh || currentState == State.ChaseTowards || currentState == State.ChaseMoveBack || currentState == State.Transform || currentState == State.FlyBackToNavmesh)
		{
			enemy.Rigidbody.gravity = false;
		}
		else
		{
			enemy.Rigidbody.gravity = true;
		}
	}

	private void MoveBackPosition()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (moveBackTimer <= 0f)
		{
			moveBackTimer = 0.1f;
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(((Component)this).transform.position, ref val, 0.5f, -1) && Physics.Raycast(((Component)this).transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				moveBackPosition = ((NavMeshHit)(ref val)).position;
			}
		}
	}

	private bool VisionBlocked()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (visionTimer <= 0f)
		{
			visionTimer = 0.25f;
			Vector3 val = playerTarget.PlayerVisionTarget.VisionTransform.position - enemy.CenterTransform.position;
			visionPrevious = Physics.Raycast(enemy.CenterTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
		moveBackTimer -= Time.deltaTime;
		annoyingJumpPauseTimer -= Time.deltaTime;
		if (currentState == State.ChaseNavmesh || currentState == State.ChaseTowards || currentState == State.ChaseMoveBack)
		{
			chaseTimer += Time.deltaTime;
		}
		else
		{
			chaseTimer = 0f;
		}
		if (currentState == State.Spawn)
		{
			targetedPlayerTime = 0f;
		}
		if (currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder)
		{
			targetedPlayerTime += Time.deltaTime;
			return;
		}
		targetedPlayerTime -= 5f * Time.deltaTime;
		targetedPlayerTime = Mathf.Max(0f, targetedPlayerTime);
	}

	private void HeadLookAtLogic()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if ((currentState == State.Notice || currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder) && Object.op_Implicit((Object)(object)playerTarget) && !playerTarget.isDisabled)
		{
			flag = true;
		}
		if (flag)
		{
			Vector3 direction = playerTarget.PlayerVisionTarget.VisionTransform.position - headLookAtTarget.position;
			direction = SemiFunc.ClampDirection(direction, headLookAtTarget.forward, 60f);
			headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(headLookAtSpring, Quaternion.LookRotation(direction));
		}
		else
		{
			headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(headLookAtSpring, headLookAtTarget.rotation);
		}
	}

	private bool LeaveCheck(bool _setLeave)
	{
		if (SemiFunc.EnemyForceLeave(enemy) || targetedPlayerTime >= targetedPlayerTimeMax)
		{
			if (_setLeave)
			{
				UpdateState(State.Leave);
			}
			return true;
		}
		return false;
	}

	public void IdleBreakerSet()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("IdleBreakerSetRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			IdleBreakerSetRPC();
		}
	}

	private void ChaseStop()
	{
		if (chaseTimer >= 10f || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.DeTransform);
		}
	}

	private void FollowOffsetLogic()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.ChaseNavmesh || currentState == State.ChaseMoveBack || currentState == State.FlyBackToNavmesh)
		{
			followOffsetTransform.localPosition = Vector3.Lerp(followOffsetTransform.localPosition, Vector3.up * 0.75f, 5f * Time.deltaTime);
		}
		else if (currentState == State.FlyBackToNavmeshStop)
		{
			followOffsetTransform.localPosition = Vector3.zero;
		}
		else
		{
			followOffsetTransform.localPosition = Vector3.Lerp(followOffsetTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
		}
	}

	private void FlyBackConditionLogic()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate || currentState == State.Notice || currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder || currentState == State.MoveBackToNavmesh) && ((Component)enemy.Rigidbody).transform.position.y - moveBackPosition.y < -4f)
		{
			UpdateState(State.FlyBackToNavmesh);
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		if (currentState == State.Spawn)
		{
			anim.OnSpawn();
		}
	}

	[PunRPC]
	private void UpdatePlayerTargetRPC(int _photonViewID)
	{
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			if (item.photonView.ViewID == _photonViewID)
			{
				playerTarget = item;
				break;
			}
		}
	}

	[PunRPC]
	private void IdleBreakerSetRPC()
	{
		idleBreakerTrigger = true;
	}
}
