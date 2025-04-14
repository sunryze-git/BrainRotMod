using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGnome : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		NoticeDelay,
		Notice,
		Move,
		MoveUnder,
		MoveOver,
		MoveBack,
		AttackMove,
		Attack,
		AttackDone,
		Stun,
		Despawn
	}

	[Space]
	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	private State attackMoveState;

	[Space]
	public Enemy enemy;

	public EnemyGnomeAnim enemyGnomeAnim;

	private PhotonView photonView;

	internal int directorIndex;

	[Space]
	public SpringQuaternion rotationSpring;

	private Quaternion rotationTarget;

	[Space]
	public BoxCollider avoidCollider;

	private float avoidTimer;

	private Vector3 avoidForce;

	[Space]
	public float speedMin = 1f;

	public float speedMax = 2f;

	[Space]
	public Transform backAwayOffset;

	public Transform moveOffsetTransform;

	public Transform rotationTransform;

	private float moveOffsetTimer;

	private float moveOffsetSetTimer;

	private Vector3 moveOffsetPosition;

	private float attackAngle;

	private Vector3 moveBackPosition;

	private float moveBackTimer;

	private bool visionPrevious;

	private float visionTimer;

	internal float attackCooldown;

	private float idleBreakerTimer;

	internal float overlapCheckTimer;

	internal float overlapCheckCooldown;

	internal bool overlapCheckPrevious;

	[Space]
	public ParticleSystem[] deathEffects;

	[Space]
	public Sound soundHurt;

	public Sound soundDeath;

	[Space]
	public Sound soundImpactLight;

	public Sound soundImpactMedium;

	public Sound soundImpactHeavy;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		enemy.NavMeshAgent.DefaultSpeed = Random.Range(speedMin, speedMax);
		enemy.NavMeshAgent.Agent.speed = enemy.NavMeshAgent.DefaultSpeed;
	}

	private void Update()
	{
		if (Object.op_Implicit((Object)(object)EnemyGnomeDirector.instance) && EnemyGnomeDirector.instance.setup && SemiFunc.IsMasterClientOrSingleplayer())
		{
			AvoidLogic();
			RotationLogic();
			BackAwayOffsetLogic();
			MoveOffsetLogic();
			TimerLogic();
			if (enemy.CurrentState == EnemyState.Despawn)
			{
				UpdateState(State.Despawn);
			}
			if (enemy.IsStunned())
			{
				UpdateState(State.Stun);
			}
			switch (currentState)
			{
			case State.Spawn:
				StateSpawn();
				break;
			case State.Idle:
				StateIdle();
				break;
			case State.Move:
				StateMove();
				break;
			case State.Notice:
				StateNotice();
				break;
			case State.NoticeDelay:
				StateNoticeDelay();
				break;
			case State.MoveUnder:
				StateMoveUnder();
				break;
			case State.MoveOver:
				StateMoveOver();
				break;
			case State.MoveBack:
				StateMoveBack();
				break;
			case State.AttackMove:
				StateAttackMove();
				break;
			case State.Attack:
				StateAttack();
				break;
			case State.AttackDone:
				StateAttackDone();
				break;
			case State.Stun:
				StateStun();
				break;
			case State.Despawn:
				StateDespawn();
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && avoidForce != Vector3.zero)
		{
			enemy.Rigidbody.rb.AddForce(avoidForce * 2f, (ForceMode)0);
		}
	}

	private void StateSpawn()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateIdle()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
		}
		enemy.Rigidbody.DisableFollowPosition(0.1f, 0.5f);
		IdleBreakerLogic();
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			UpdateState(State.NoticeDelay);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, EnemyGnomeDirector.instance.destinations[directorIndex]) > 2f)
		{
			UpdateState(State.Move);
		}
	}

	private void StateMove()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		enemy.NavMeshAgent.SetDestination(EnemyGnomeDirector.instance.destinations[directorIndex]);
		MoveBackPosition();
		MoveOffsetSet();
		SemiFunc.EnemyCartJump(enemy);
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer || EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackValuable)
		{
			UpdateState(State.NoticeDelay);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, EnemyGnomeDirector.instance.destinations[directorIndex]) <= 0.2f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, EnemyGnomeDirector.instance.destinations[directorIndex]) <= 2f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateNoticeDelay()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(0f, 1f);
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Notice);
		}
	}

	private void StateNotice()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
			stateTimer = 0.5f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.AttackMove);
		}
	}

	private void StateAttackMove()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		Vector3 destination = AttackPositionLogic();
		enemy.NavMeshAgent.SetDestination(destination);
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		MoveBackPosition();
		MoveOffsetSet();
		SemiFunc.EnemyCartJump(enemy);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			UpdateState(State.Move);
		}
		else if (flag)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Attack);
		}
		else
		{
			if (enemy.NavMeshAgent.CanReach(AttackVisionDynamic(), 1f) || !(Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f))
			{
				return;
			}
			if (AttackPositionLogic().y > ((Component)enemy.Rigidbody).transform.position.y + 0.2f)
			{
				enemy.Jump.StuckTrigger(AttackVisionPosition() - enemy.Vision.VisionTransform.position);
			}
			NavMeshHit val = default(NavMeshHit);
			if (!VisionBlocked() && !NavMesh.SamplePosition(AttackVisionDynamic(), ref val, 0.5f, -1))
			{
				if (Mathf.Abs(AttackVisionDynamic().y - ((Component)enemy.Rigidbody).transform.position.y) < 0.2f)
				{
					UpdateState(State.MoveUnder);
				}
				else if (AttackPositionLogic().y > ((Component)enemy.Rigidbody).transform.position.y)
				{
					UpdateState(State.MoveOver);
				}
			}
		}
	}

	private void StateMoveUnder()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 val = AttackPositionLogic();
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, val, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		MoveOffsetSet();
		SemiFunc.EnemyCartJump(enemy);
		NavMeshHit val2 = default(NavMeshHit);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			UpdateState(State.MoveBack);
		}
		else if (flag)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Attack);
		}
		else if (NavMesh.SamplePosition(val, ref val2, 0.5f, -1))
		{
			UpdateState(State.MoveBack);
		}
		else if (VisionBlocked())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.MoveBack);
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			stateTimer = 2f;
		}
	}

	private void StateMoveOver()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
		}
		bool flag = EnemyGnomeDirector.instance.CanAttack(this);
		Vector3 val = AttackPositionLogic();
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, val, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		MoveOffsetSet();
		SemiFunc.EnemyCartJump(enemy);
		if (AttackVisionDynamic().y > ((Component)enemy.Rigidbody).transform.position.y + 0.2f && !flag)
		{
			enemy.Jump.StuckTrigger(AttackVisionDynamic() - ((Component)enemy.Rigidbody).transform.position);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, AttackVisionDynamic(), 2f);
		}
		NavMeshHit val2 = default(NavMeshHit);
		if (EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackPlayer && EnemyGnomeDirector.instance.currentState != EnemyGnomeDirector.State.AttackValuable)
		{
			UpdateState(State.MoveBack);
		}
		else if (flag)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Attack);
		}
		else if (NavMesh.SamplePosition(val, ref val2, 0.5f, -1))
		{
			UpdateState(State.MoveBack);
		}
		else if (VisionBlocked())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.MoveBack);
			}
		}
		else
		{
			EnemyGnomeDirector.instance.SeeTarget();
			stateTimer = 2f;
		}
	}

	private void StateMoveBack()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		if (!enemy.Jump.jumping)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		MoveOffsetSet();
		SemiFunc.EnemyCartJump(enemy);
		stateTimer -= Time.deltaTime;
		bool num = EnemyGnomeDirector.instance.CanAttack(this);
		if (stateTimer <= 0f && (Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) > 2f || enemy.Rigidbody.notMovingTimer > 2f) && !enemy.Jump.jumping)
		{
			Vector3 val = ((Component)this).transform.position - moveBackPosition;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			enemy.Jump.StuckTrigger(((Component)this).transform.position - moveBackPosition);
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
			Transform transform = ((Component)this).transform;
			transform.position += normalized * 2f;
		}
		NavMeshHit val2 = default(NavMeshHit);
		if (num)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Attack);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, moveBackPosition) <= 0.2f)
		{
			UpdateState(State.AttackMove);
		}
		else if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val2, 0.5f, -1))
		{
			UpdateState(State.AttackMove);
		}
	}

	private void StateAttack()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateTimer = 3f;
			stateImpulse = false;
		}
		if (stateTimer > 0.5f)
		{
			AnimatorStateInfo currentAnimatorStateInfo = enemyGnomeAnim.animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Attack"))
			{
				UpdateState(State.AttackMove);
				return;
			}
		}
		enemy.StuckCount = 0;
		enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.AttackDone);
		}
	}

	private void StateAttackDone()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 1f;
			stateImpulse = false;
		}
		enemy.StuckCount = 0;
		enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			moveBackTimer = 2f;
			attackCooldown = 2f;
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val, 0.5f, -1))
			{
				UpdateState(State.AttackMove);
			}
			else
			{
				UpdateState(attackMoveState);
			}
		}
	}

	private void StateStun()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
		}
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	private void StateDespawn()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			EnemyGnomeDirector.instance.OnSpawn();
			UpdateState(State.Spawn);
		}
	}

	public void OnHurt()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundHurt.Play(enemy.CenterTransform.position);
	}

	public void OnDeath()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		soundDeath.Play(enemy.CenterTransform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.05f);
		ParticleSystem[] array = deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnInvestigate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.Investigate(enemy.StateInvestigate.onInvestigateTriggeredPosition);
		}
	}

	public void OnVision()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyGnomeDirector.instance.SetTarget(enemy.Vision.onVisionTriggeredPlayer);
		}
	}

	public void OnImpactLight()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			soundImpactLight.Play(enemy.CenterTransform.position);
		}
	}

	public void OnImpactMedium()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			soundImpactMedium.Play(enemy.CenterTransform.position);
		}
	}

	public void OnImpactHeavy()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			soundImpactHeavy.Play(enemy.CenterTransform.position);
		}
	}

	public void UpdateState(State _state)
	{
		if (currentState != _state)
		{
			if (_state == State.Attack)
			{
				attackMoveState = currentState;
			}
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

	private void RotationLogic()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Move || currentState == State.Notice || currentState == State.AttackMove || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.MoveBack || currentState == State.Attack)
		{
			if (currentState == State.Notice || ((currentState == State.AttackMove || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.Attack) && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, EnemyGnomeDirector.instance.attackPosition) < 5f))
			{
				Quaternion rotation = rotationTransform.rotation;
				rotationTransform.rotation = Quaternion.LookRotation(AttackVisionPosition() - ((Component)enemy.Rigidbody).transform.position);
				rotationTransform.eulerAngles = new Vector3(0f, rotationTransform.eulerAngles.y, 0f);
				Quaternion rotation2 = rotationTransform.rotation;
				rotationTransform.rotation = rotation;
				rotationTarget = rotation2;
			}
			else
			{
				Vector3 velocity = enemy.Rigidbody.rb.velocity;
				if (((Vector3)(ref velocity)).magnitude > 0.1f)
				{
					Vector3 position = rotationTransform.position;
					Quaternion rotation3 = rotationTransform.rotation;
					rotationTransform.position = ((Component)enemy.Rigidbody).transform.position;
					Transform obj = rotationTransform;
					velocity = enemy.Rigidbody.rb.velocity;
					obj.rotation = Quaternion.LookRotation(((Vector3)(ref velocity)).normalized);
					rotationTransform.eulerAngles = new Vector3(0f, rotationTransform.eulerAngles.y, 0f);
					Quaternion rotation4 = rotationTransform.rotation;
					rotationTransform.position = position;
					rotationTransform.rotation = rotation3;
					rotationTarget = rotation4;
				}
			}
		}
		else if (currentState == State.Idle && Vector3.Distance(((Component)EnemyGnomeDirector.instance).transform.position, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
		{
			Quaternion rotation5 = rotationTransform.rotation;
			rotationTransform.rotation = Quaternion.LookRotation(((Component)EnemyGnomeDirector.instance).transform.position - ((Component)enemy.Rigidbody).transform.position);
			rotationTransform.eulerAngles = new Vector3(0f, rotationTransform.eulerAngles.y, 0f);
			Quaternion rotation6 = rotationTransform.rotation;
			rotationTransform.rotation = rotation5;
			rotationTarget = rotation6;
		}
		rotationTransform.rotation = SemiFunc.SpringQuaternionGet(rotationSpring, rotationTarget);
	}

	private void AvoidLogic()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Move || currentState == State.AttackMove || currentState == State.MoveUnder || currentState == State.MoveOver)
		{
			if (avoidTimer <= 0f)
			{
				avoidForce = Vector3.zero;
				avoidTimer = 0.25f;
				if (enemy.Jump.jumping)
				{
					return;
				}
				Collider[] array = Physics.OverlapBox(((Component)avoidCollider).transform.position, avoidCollider.size / 2f, ((Component)avoidCollider).transform.rotation, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
				for (int i = 0; i < array.Length; i++)
				{
					EnemyRigidbody componentInParent = ((Component)array[i]).GetComponentInParent<EnemyRigidbody>();
					if (Object.op_Implicit((Object)(object)componentInParent))
					{
						EnemyGnome component = ((Component)componentInParent.enemy).GetComponent<EnemyGnome>();
						if (Object.op_Implicit((Object)(object)component))
						{
							Vector3 val = ((Component)this).transform.position - ((Component)component).transform.position;
							Vector3 normalized = ((Vector3)(ref val)).normalized;
							avoidForce += ((Vector3)(ref normalized)).normalized;
						}
					}
				}
			}
			else
			{
				avoidTimer -= Time.deltaTime;
			}
		}
		else
		{
			avoidForce = Vector3.zero;
		}
	}

	private Vector3 AttackPositionLogic()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = EnemyGnomeDirector.instance.attackPosition + new Vector3(Mathf.Cos(attackAngle), 0f, Mathf.Sin(attackAngle)) * 0.7f;
		attackAngle += Time.deltaTime * 1f;
		return result;
	}

	private Vector3 AttackVisionPosition()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return EnemyGnomeDirector.instance.attackVisionPosition;
	}

	private Vector3 AttackVisionDynamic()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (EnemyGnomeDirector.instance.currentState == EnemyGnomeDirector.State.AttackPlayer)
		{
			return AttackPositionLogic();
		}
		return AttackVisionPosition();
	}

	private void MoveBackPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) < 1f)
		{
			moveBackPosition = ((Component)this).transform.position;
		}
	}

	private void BackAwayOffsetLogic()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (moveBackTimer > 0f)
		{
			moveBackTimer -= Time.deltaTime;
			backAwayOffset.localPosition = Vector3.Lerp(backAwayOffset.localPosition, new Vector3(0f, 0f, -1f), Time.deltaTime * 10f);
		}
		else
		{
			backAwayOffset.localPosition = Vector3.Lerp(backAwayOffset.localPosition, Vector3.zero, Time.deltaTime * 10f);
		}
	}

	private void MoveOffsetLogic()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (moveOffsetTimer > 0f)
		{
			moveOffsetTimer -= Time.deltaTime;
			if (enemy.Jump.jumping)
			{
				moveOffsetTimer = 0f;
			}
			if (moveOffsetTimer <= 0f)
			{
				moveOffsetPosition = Vector3.zero;
			}
			else
			{
				moveOffsetSetTimer -= Time.deltaTime;
				if (moveOffsetSetTimer <= 0f)
				{
					Vector3 val = Random.insideUnitSphere * 0.5f;
					val.y = 0f;
					moveOffsetPosition = val;
					moveOffsetSetTimer = Random.Range(0.2f, 1f);
				}
			}
		}
		moveOffsetTransform.localPosition = Vector3.Lerp(moveOffsetTransform.localPosition, moveOffsetPosition, Time.deltaTime * 20f);
	}

	private void MoveOffsetSet()
	{
		moveOffsetTimer = 0.2f;
	}

	private bool VisionBlocked()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (visionTimer <= 0f)
		{
			visionTimer = 0.1f;
			Vector3 val = AttackVisionPosition() - enemy.Vision.VisionTransform.position;
			visionPrevious = Physics.Raycast(enemy.Vision.VisionTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
		attackCooldown -= Time.deltaTime;
		overlapCheckTimer -= Time.deltaTime;
		if (overlapCheckCooldown > 0f)
		{
			overlapCheckCooldown -= Time.deltaTime;
			if (overlapCheckCooldown <= 0f)
			{
				overlapCheckPrevious = false;
			}
		}
	}

	public void IdleBreakerLogic()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (EnemyGnome gnome in EnemyGnomeDirector.instance.gnomes)
		{
			if ((Object)(object)gnome != (Object)(object)this && Vector3.Distance(((Component)this).transform.position, ((Component)gnome).transform.position) < 2f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		if (idleBreakerTimer <= 0f)
		{
			idleBreakerTimer = Random.Range(2f, 15f);
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("IdleBreakerRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				IdleBreakerRPC();
			}
		}
		else
		{
			idleBreakerTimer -= Time.deltaTime;
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		if (currentState == State.Spawn)
		{
			enemyGnomeAnim.OnSpawn();
		}
	}

	[PunRPC]
	private void IdleBreakerRPC()
	{
		enemyGnomeAnim.idleBreakerImpulse = true;
	}
}
