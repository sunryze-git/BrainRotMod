using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRunner : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		SeekPlayer,
		Sneak,
		Notice,
		AttackPlayer,
		AttackPlayerOver,
		AttackPlayerBackToNavMesh,
		StuckAttackNotice,
		StuckAttack,
		LookUnderStart,
		LookUnder,
		LookUnderStop,
		Stun,
		Leave,
		Despawn
	}

	public SpringQuaternion headSpring;

	public Transform headSpringTarget;

	public Transform headSpringSource;

	public SpringQuaternion rotationSpring;

	private Quaternion rotationTarget;

	public State currentState;

	private bool stateImpulse = true;

	private float stateTimer;

	internal PlayerAvatar targetPlayer;

	public Enemy enemy;

	public EnemyRunnerAnim animator;

	public ParticleSystem hayParticlesBig;

	public ParticleSystem hayParticlesSmall;

	public ParticleSystem bitsParticlesFar;

	public ParticleSystem bitsParticlesShort;

	private PhotonView photonView;

	public HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private Vector3 agentDestination;

	private Vector3 backToNavMeshPosition;

	private Vector3 stuckAttackTarget;

	private float agentSpeed = 3f;

	private float agentSpeedCurrent;

	private float rbCheckHeightOffset = 0.8f;

	private Vector3 targetPosition;

	private float visionTimer;

	private bool visionPrevious;

	public Transform feetTransform;

	private float sampleNavMeshTimer;

	private Vector3 lookUnderPosition;

	private Vector3 lookUnderPositionNavmesh;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		((Component)hurtCollider).gameObject.SetActive(false);
	}

	private void Update()
	{
		HeadSpringUpdate();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (enemy.CurrentState == EnemyState.Despawn && !enemy.IsStunned())
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
			case State.Roam:
				StateRoam();
				break;
			case State.Investigate:
				StateInvestigate();
				break;
			case State.SeekPlayer:
				StateSeekPlayer();
				break;
			case State.Sneak:
				StateSneak();
				break;
			case State.Notice:
				StateNotice();
				break;
			case State.AttackPlayer:
				StateAttackPlayer();
				break;
			case State.AttackPlayerOver:
				StateAttackPlayerOver();
				break;
			case State.AttackPlayerBackToNavMesh:
				StateAttackPlayerBackToNavMesh();
				break;
			case State.StuckAttackNotice:
				StateStuckAttackNotice();
				break;
			case State.StuckAttack:
				StateStuckAttack();
				break;
			case State.LookUnderStart:
				StateLookUnderStart();
				break;
			case State.LookUnder:
				StateLookUnder();
				break;
			case State.LookUnderStop:
				StateLookUnderStop();
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
			RotationLogic();
			TimerLogic();
		}
	}

	public void StateSpawn()
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

	public void StateIdle()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(2f, 6f);
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
			StoreBackToNavMeshPosition();
		}
		if (!SemiFunc.EnemySpawnIdlePause())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Roam);
			}
			if (SemiFunc.EnemyForceLeave(enemy))
			{
				UpdateState(State.Leave);
			}
		}
	}

	public void StateRoam()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
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
				agentDestination = ((NavMeshHit)(ref val)).position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
			StoreBackToNavMeshPosition();
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (enemy.Rigidbody.notMovingTimer > 3f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (!enemy.Jump.jumping && (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f))
			{
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	public void StateInvestigate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			StoreBackToNavMeshPosition();
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f)
			{
				AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 2f)
			{
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	public void StateSeekPlayer()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 20f;
			stateImpulse = false;
			targetPosition = ((Component)this).transform.position;
			LevelPoint levelPointAhead = enemy.GetLevelPointAhead(targetPosition);
			if (Object.op_Implicit((Object)(object)levelPointAhead))
			{
				targetPosition = ((Component)levelPointAhead).transform.position;
			}
			enemy.Rigidbody.notMovingTimer = 0f;
		}
		StoreBackToNavMeshPosition();
		enemy.NavMeshAgent.OverrideAgent(1f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			LevelPoint levelPointAhead2 = enemy.GetLevelPointAhead(targetPosition);
			if (Object.op_Implicit((Object)(object)levelPointAhead2))
			{
				targetPosition = ((Component)levelPointAhead2).transform.position;
			}
		}
		if (enemy.Rigidbody.notMovingTimer >= 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		enemy.NavMeshAgent.SetDestination(targetPosition);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f || enemy.Rigidbody.notMovingTimer > 3f)
		{
			UpdateState(State.Roam);
		}
	}

	public void StateNotice()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 0.9f;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.AttackPlayer);
		}
	}

	public void StateSneak()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.Idle);
			return;
		}
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			enemy.Rigidbody.notMovingTimer = 0f;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
		enemy.NavMeshAgent.OverrideAgent(1f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		StoreBackToNavMeshPosition();
		stateTimer -= Time.deltaTime;
		if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
		}
		else if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(feetTransform.position, enemy.NavMeshAgent.GetPoint()) < 2f || enemy.OnScreen.OnScreenAny)
		{
			UpdateState(State.Notice);
		}
	}

	public void StateAttackPlayer()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.SeekPlayer);
			return;
		}
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			agentSpeedCurrent = 0f;
			targetPosition = ((Component)targetPlayer).transform.position;
			enemy.NavMeshAgent.SetDestination(targetPosition);
			return;
		}
		StoreBackToNavMeshPosition();
		agentSpeedCurrent = Mathf.Lerp(agentSpeedCurrent, agentSpeed, Time.deltaTime * 2f);
		enemy.NavMeshAgent.OverrideAgent(agentSpeedCurrent, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
		stateTimer -= Time.deltaTime;
		if (!enemy.NavMeshAgent.CanReach(((Component)targetPlayer).transform.position, 0.25f) && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			if (((Component)targetPlayer).transform.position.y > ((Component)enemy.Rigidbody).transform.position.y - rbCheckHeightOffset)
			{
				enemy.Jump.StuckTrigger(((Component)targetPlayer).transform.position - enemy.Vision.VisionTransform.position);
			}
			NavMeshHit val = default(NavMeshHit);
			if (!VisionBlocked() && !NavMesh.SamplePosition(((Component)targetPlayer).transform.position, ref val, 0.5f, -1) && ((Component)targetPlayer).transform.position.y > feetTransform.position.y)
			{
				UpdateState(State.AttackPlayerOver);
				return;
			}
		}
		if (!enemy.Jump.jumping && enemy.Rigidbody.notMovingTimer > 2f)
		{
			enemy.Jump.StuckTrigger(((Component)targetPlayer).transform.position - feetTransform.position);
		}
		if (stateTimer > 1.5f && targetPlayer.isCrawling && !targetPlayer.isTumbling && (double)Vector3.Distance(enemy.NavMeshAgent.GetPoint(), ((Component)targetPlayer).transform.position) > 0.5 && Vector3.Distance(((Component)targetPlayer).transform.position, targetPlayer.LastNavmeshPosition) < 2f)
		{
			UpdateState(State.LookUnderStart);
		}
		else if (stateTimer <= 0f)
		{
			UpdateState(State.SeekPlayer);
		}
		else if (targetPlayer.isDisabled)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateAttackPlayerOver()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.AttackPlayerBackToNavMesh);
			return;
		}
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)targetPlayer).transform.position, agentSpeed * Time.deltaTime);
		if (!enemy.Jump.jumping && (((Component)targetPlayer).transform.position.y > ((Component)enemy.Rigidbody).transform.position.y - rbCheckHeightOffset || enemy.Rigidbody.notMovingTimer > 2f))
		{
			enemy.Jump.StuckTrigger(((Component)targetPlayer).transform.position - feetTransform.position);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)targetPlayer).transform.position, agentSpeed);
			enemy.Rigidbody.OverrideFollowRotation(0.5f, 0.25f);
		}
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(((Component)targetPlayer).transform.position, ref val, 0.5f, -1))
		{
			UpdateState(State.AttackPlayerBackToNavMesh);
		}
		else if (VisionBlocked() || targetPlayer.isDisabled || enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f || targetPlayer.isDisabled)
			{
				UpdateState(State.AttackPlayerBackToNavMesh);
			}
		}
	}

	public void StateAttackPlayerBackToNavMesh()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 30f;
		}
		stateTimer -= Time.deltaTime;
		if ((Vector3.Distance(((Component)this).transform.position, feetTransform.position) > 2f || enemy.Rigidbody.notMovingTimer > 2f) && !enemy.Jump.jumping)
		{
			Vector3 val = feetTransform.position - backToNavMeshPosition;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			enemy.Jump.StuckTrigger(normalized);
			((Component)this).transform.position = feetTransform.position;
			Transform transform = ((Component)this).transform;
			transform.position += normalized * 2f;
			enemy.Rigidbody.OverrideFollowRotation(0.5f, 0.25f);
		}
		enemy.NavMeshAgent.Disable(0.1f);
		if (!enemy.Jump.jumping)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, backToNavMeshPosition, agentSpeed * Time.deltaTime);
		}
		NavMeshHit val2 = default(NavMeshHit);
		if (Vector3.Distance(feetTransform.position, backToNavMeshPosition) <= 0.2f || NavMesh.SamplePosition(feetTransform.position, ref val2, 0.5f, -1))
		{
			UpdateState(State.AttackPlayer);
		}
		else if (stateTimer <= 0f)
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
			UpdateState(State.Despawn);
		}
	}

	public void StateStuckAttackNotice()
	{
		if (stateImpulse)
		{
			stateTimer = 0.9f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.StuckAttack);
		}
	}

	public void StateStuckAttack()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(feetTransform.position);
			stateTimer = 1.5f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.Stop(0.2f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateLookUnderStart()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.SeekPlayer);
			return;
		}
		if (stateImpulse)
		{
			lookUnderPosition = ((Component)targetPlayer).transform.position;
			lookUnderPositionNavmesh = targetPlayer.LastNavmeshPosition;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateTimer = 1f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.OverrideAgent(3f, 10f, 0.2f);
		enemy.NavMeshAgent.SetDestination(lookUnderPositionNavmesh);
		if (Vector3.Distance(((Component)this).transform.position, lookUnderPositionNavmesh) < 0.5f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.LookUnder);
			}
		}
		else if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			UpdateState(State.SeekPlayer);
		}
	}

	public void StateLookUnder()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
		}
		stateTimer -= Time.deltaTime;
		enemy.Vision.StandOverride(0.25f);
		if (stateTimer <= 0f)
		{
			UpdateState(State.LookUnderStop);
		}
	}

	public void StateLookUnderStop()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 0.9f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.SeekPlayer);
		}
	}

	public void StateStun()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		StoreBackToNavMeshPosition();
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	public void StateLeave()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
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
			if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				agentDestination = ((NavMeshHit)(ref val)).position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			stateImpulse = false;
		}
		if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			stateTimer -= Time.deltaTime;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		enemy.NavMeshAgent.OverrideAgent(1f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f || stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateDespawn()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			UpdateState(State.Spawn);
		}
	}

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		animator.sfxHurt.Play(((Component)animator).transform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		((Component)hayParticlesBig).transform.position = enemy.CenterTransform.position;
		hayParticlesBig.Play();
		((Component)hayParticlesSmall).transform.position = enemy.CenterTransform.position;
		hayParticlesSmall.Play();
		((Component)bitsParticlesFar).transform.position = enemy.CenterTransform.position;
		bitsParticlesFar.Play();
		((Component)bitsParticlesShort).transform.position = enemy.CenterTransform.position;
		bitsParticlesShort.Play();
		animator.sfxDeath.Play(((Component)animator).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnInvestigate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate)
			{
				agentDestination = enemy.StateInvestigate.onInvestigateTriggeredPosition;
				UpdateState(State.Investigate);
			}
			else if (currentState == State.SeekPlayer)
			{
				targetPosition = enemy.StateInvestigate.onInvestigateTriggeredPosition;
			}
		}
	}

	public void OnVision()
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (currentState == State.Roam || currentState == State.Idle || currentState == State.Investigate || currentState == State.SeekPlayer)
		{
			targetPlayer = enemy.Vision.onVisionTriggeredPlayer;
			if (!enemy.OnScreen.OnScreenAny)
			{
				UpdateState(State.Sneak);
			}
			else
			{
				UpdateState(State.Notice);
			}
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
		else if (currentState == State.AttackPlayer || currentState == State.AttackPlayerOver || currentState == State.Sneak)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
			{
				stateTimer = 2f;
			}
		}
		else if (currentState == State.LookUnderStart)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer && !targetPlayer.isCrawling)
			{
				UpdateState(State.AttackPlayer);
			}
		}
		else if (currentState == State.LookUnder && (Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			if (targetPlayer.isCrawling)
			{
				lookUnderPosition = ((Component)targetPlayer).transform.position;
			}
			else
			{
				UpdateState(State.LookUnderStop);
			}
		}
	}

	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			targetPlayer = enemy.Vision.onVisionTriggeredPlayer;
			SemiLogger.LogAxel("OnGrabbed: " + (object)targetPlayer, null, null);
			UpdateState(State.Notice);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
	}

	private void UpdateState(State _state)
	{
		if (currentState != _state)
		{
			enemy.Rigidbody.notMovingTimer = 0f;
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

	private void AttackNearestPhysObjectOrGoToIdle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		stuckAttackTarget = Vector3.zero;
		if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			stuckAttackTarget = SemiFunc.EnemyGetNearestPhysObject(enemy);
		}
		if (stuckAttackTarget != Vector3.zero)
		{
			UpdateState(State.StuckAttackNotice);
		}
		else
		{
			UpdateState(State.Idle);
		}
	}

	private void HeadSpringUpdate()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		headSpringSource.rotation = SemiFunc.SpringQuaternionGet(headSpring, headSpringTarget.rotation);
	}

	private void RotationLogic()
	{
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.LookUnderStop)
		{
			if (currentState == State.Notice || currentState == State.AttackPlayer || currentState == State.AttackPlayerOver)
			{
				if (Object.op_Implicit((Object)(object)targetPlayer) && Vector3.Distance(((Component)targetPlayer).transform.position, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
				{
					rotationTarget = Quaternion.LookRotation(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position);
					((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
				}
			}
			else if (currentState == State.StuckAttack)
			{
				if (Vector3.Distance(stuckAttackTarget, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
				{
					rotationTarget = Quaternion.LookRotation(stuckAttackTarget - ((Component)enemy.Rigidbody).transform.position);
					((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
				}
			}
			else if (currentState == State.LookUnderStart || currentState == State.LookUnder)
			{
				if (Vector3.Distance(lookUnderPosition, ((Component)this).transform.position) > 0.1f)
				{
					rotationTarget = Quaternion.LookRotation(lookUnderPosition - ((Component)this).transform.position);
					((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
				}
			}
			else
			{
				Vector3 normalized = ((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized;
				if (((Vector3)(ref normalized)).magnitude > 0.1f)
				{
					rotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
					((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
				}
			}
		}
		if (currentState == State.Roam || currentState == State.Investigate)
		{
			rotationSpring.speed = 3f;
		}
		else
		{
			rotationSpring.speed = 10f;
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(rotationSpring, rotationTarget);
	}

	private bool VisionBlocked()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (visionTimer <= 0f && Object.op_Implicit((Object)(object)targetPlayer))
		{
			visionTimer = 0.1f;
			Vector3 val = targetPlayer.PlayerVisionTarget.VisionTransform.position - enemy.Vision.VisionTransform.position;
			visionPrevious = Physics.Raycast(enemy.Vision.VisionTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
		sampleNavMeshTimer -= Time.deltaTime;
	}

	private void StoreBackToNavMeshPosition()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (sampleNavMeshTimer <= 0f)
		{
			sampleNavMeshTimer = 0.5f;
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(((Component)this).transform.position, ref val, 0.5f, -1))
			{
				backToNavMeshPosition = ((NavMeshHit)(ref val)).position;
			}
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		if (currentState == State.Spawn)
		{
			animator.OnSpawn();
		}
	}

	[PunRPC]
	private void TargetPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.photonView.ViewID == _playerID)
			{
				targetPlayer = player;
			}
		}
	}
}
