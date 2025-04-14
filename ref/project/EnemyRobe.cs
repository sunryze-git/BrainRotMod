using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRobe : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		TargetPlayer,
		LookUnderStart,
		LookUnder,
		LookUnderAttack,
		LookUnderStop,
		SeekPlayer,
		Attack,
		StuckAttack,
		Stun,
		Leave,
		Despawn
	}

	[Header("References")]
	public EnemyRobeAnim robeAnim;

	internal Enemy enemy;

	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	internal PlayerAvatar targetPlayer;

	private PhotonView photonView;

	private float roamWaitTimer;

	private Vector3 agentDestination;

	private float overrideAgentLerp;

	private Vector3 targetPosition;

	public Transform eyeLocation;

	internal bool isOnScreen;

	internal bool attackImpulse;

	internal bool deathImpulse;

	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	public float idleBreakTimeMax = 90f;

	private float idleBreakTimer;

	internal bool idleBreakTrigger;

	[Space]
	public SpringQuaternion rotationSpring;

	private Quaternion rotationTarget;

	[Space]
	public SpringQuaternion endPieceSpring;

	public Transform endPieceSource;

	public Transform endPieceTarget;

	private float grabAggroTimer;

	private Vector3 lookUnderPositionNavmesh;

	private Vector3 lookUnderPosition;

	internal bool lookUnderAttackImpulse;

	private Vector3 stuckAttackTarget;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		idleBreakTimer = Random.Range(idleBreakTimeMin, idleBreakTimeMax);
	}

	private void Update()
	{
		EndPieceLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (idleBreakTimer >= 0f)
		{
			idleBreakTimer -= Time.deltaTime;
			if (idleBreakTimer <= 0f && CanIdleBreak())
			{
				IdleBreak();
				idleBreakTimer = Random.Range(idleBreakTimeMin, idleBreakTimeMax);
			}
		}
		RotationLogic();
		RigidbodyRotationSpeed();
		if (enemy.IsStunned())
		{
			UpdateState(State.Stun);
		}
		else if (enemy.CurrentState == EnemyState.Despawn)
		{
			UpdateState(State.Despawn);
		}
		switch (currentState)
		{
		case State.Idle:
			StateIdle();
			break;
		case State.Roam:
			StateRoam();
			break;
		case State.Investigate:
			StateInvestigate();
			break;
		case State.TargetPlayer:
			MoveTowardPlayer();
			StateTargetPlayer();
			break;
		case State.LookUnderStart:
			StateLookUnderStart();
			break;
		case State.LookUnder:
			StateLookUnder();
			break;
		case State.LookUnderAttack:
			StateLookUnderAttack();
			break;
		case State.LookUnderStop:
			StateLookUnderStop();
			break;
		case State.SeekPlayer:
			StateSeekPlayer();
			break;
		case State.Spawn:
			StateSpawn();
			break;
		case State.Attack:
			StateAttack();
			break;
		case State.StuckAttack:
			StateStuckAttack();
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
		if (currentState != State.TargetPlayer)
		{
			overrideAgentLerp = 0f;
		}
		if (currentState != State.TargetPlayer && isOnScreen)
		{
			isOnScreen = false;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdateOnScreenRPC", (RpcTarget)1, new object[1] { isOnScreen });
			}
		}
		if (isOnScreen && Object.op_Implicit((Object)(object)targetPlayer) && targetPlayer.isLocal)
		{
			SemiFunc.DoNotLookEffect(((Component)this).gameObject);
		}
	}

	private void StateSpawn()
	{
		if (stateImpulse)
		{
			stateTimer = 3f;
			stateImpulse = false;
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
			if (SemiFunc.EnemyForceLeave(enemy))
			{
				UpdateState(State.Leave);
			}
		}
	}

	private void StateRoam()
	{
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
			stateTimer = 5f;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 5f, 15f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 0f, 999f);
			}
			NavMeshHit val = default(NavMeshHit);
			if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				agentDestination = ((NavMeshHit)(ref val)).position;
			}
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (enemy.Rigidbody.notMovingTimer > 1f)
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
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StateInvestigate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
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

	private void StateTargetPlayer()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
		}
		enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 30f);
		if (Vector3.Distance(enemy.CenterTransform.position, ((Component)targetPlayer).transform.position) < 2f)
		{
			UpdateState(State.Attack);
			return;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.SeekPlayer);
			return;
		}
		if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			enemy.Vision.DisableVision(2f);
			UpdateState(State.SeekPlayer);
		}
		if (stateTimer > 0.5f && targetPlayer.isCrawling && !targetPlayer.isTumbling && Vector3.Distance(enemy.NavMeshAgent.GetPoint(), ((Component)targetPlayer).transform.position) > 0.5f && Vector3.Distance(((Component)targetPlayer).transform.position, targetPlayer.LastNavmeshPosition) < 3f)
		{
			UpdateState(State.LookUnderStart);
		}
	}

	private void StateLookUnderStart()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			lookUnderPosition = ((Component)targetPlayer).transform.position;
			lookUnderPositionNavmesh = targetPlayer.LastNavmeshPosition;
			stateTimer = 2f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.OverrideAgent(3f, 10f, 0.2f);
		enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f);
		enemy.NavMeshAgent.SetDestination(lookUnderPositionNavmesh);
		if (Vector3.Distance(((Component)this).transform.position, lookUnderPositionNavmesh) < 1f)
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

	private void StateLookUnder()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		enemy.Vision.StandOverride(0.25f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(((Component)enemy.Rigidbody).transform.position.x, 0f, ((Component)enemy.Rigidbody).transform.position.z);
		Vector3 val2 = new Vector3(((Component)targetPlayer).transform.position.x, 0f, ((Component)targetPlayer).transform.position.z) - val;
		if (Vector3.Dot(((Vector3)(ref val2)).normalized, ((Component)enemy.Rigidbody).transform.forward) > 0.75f && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)targetPlayer).transform.position) < 2.5f)
		{
			UpdateState(State.LookUnderAttack);
		}
		else if (stateTimer <= 0f)
		{
			UpdateState(State.LookUnderStop);
		}
	}

	private void StateLookUnderAttack()
	{
		if (stateImpulse)
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("LookUnderAttackImpulseRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				LookUnderAttackImpulseRPC();
			}
			stateTimer = 2f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			if (targetPlayer.isDisabled)
			{
				UpdateState(State.LookUnderStop);
			}
			else
			{
				UpdateState(State.LookUnder);
			}
		}
	}

	private void StateLookUnderStop()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.SeekPlayer);
		}
	}

	private void StateSeekPlayer()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 20f;
			stateImpulse = false;
			LevelPoint levelPointAhead = enemy.GetLevelPointAhead(targetPosition);
			if (Object.op_Implicit((Object)(object)levelPointAhead))
			{
				targetPosition = ((Component)levelPointAhead).transform.position;
			}
			enemy.Rigidbody.notMovingTimer = 0f;
		}
		enemy.NavMeshAgent.OverrideAgent(3f, 3f, 0.2f);
		enemy.Rigidbody.OverrideFollowPosition(0.2f, 3f);
		if (Vector3.Distance(((Component)this).transform.position, targetPosition) < 2f)
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

	private void StateAttack()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			attackImpulse = true;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("AttackImpulseRPC", (RpcTarget)1, Array.Empty<object>());
			}
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateTimer = 2f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.Stop(0.2f);
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.SeekPlayer);
			}
		}
	}

	private void StateStuckAttack()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateTimer = 1.5f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.Stop(0.2f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Attack);
		}
	}

	private void StateStun()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
		}
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	private void StateLeave()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
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
		}
		if (enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f || stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateDespawn()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
		}
	}

	private void IdleBreak()
	{
		if (!GameManager.Multiplayer())
		{
			IdleBreakRPC();
		}
		else
		{
			photonView.RPC("IdleBreakRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	internal void UpdateState(State _state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState != _state)
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

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		robeAnim.sfxHurt.Play(((Component)robeAnim).transform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		deathImpulse = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	public void OnVision()
	{
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate || currentState == State.SeekPlayer)
		{
			targetPlayer = enemy.Vision.onVisionTriggeredPlayer;
			UpdateState(State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
		else if (currentState == State.TargetPlayer)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
			{
				stateTimer = Mathf.Max(stateTimer, 1f);
			}
		}
		else if (currentState == State.LookUnderStart)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer && !targetPlayer.isCrawling)
			{
				UpdateState(State.TargetPlayer);
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

	public void OnInvestigate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
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

	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && !(grabAggroTimer > 0f) && currentState == State.Leave)
		{
			grabAggroTimer = 60f;
			targetPlayer = enemy.Vision.onVisionTriggeredPlayer;
			UpdateState(State.TargetPlayer);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
	}

	private bool CanIdleBreak()
	{
		if (currentState != State.Idle && currentState != State.Investigate)
		{
			return currentState == State.Roam;
		}
		return true;
	}

	private void MoveTowardPlayer()
	{
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (enemy.OnScreen.GetOnScreen(targetPlayer))
		{
			flag = true;
			overrideAgentLerp += Time.deltaTime / 4f;
		}
		else
		{
			overrideAgentLerp -= Time.deltaTime / 0.01f;
		}
		if (flag != isOnScreen)
		{
			isOnScreen = flag;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdateOnScreenRPC", (RpcTarget)1, new object[1] { isOnScreen });
			}
		}
		overrideAgentLerp = Mathf.Clamp(overrideAgentLerp, 0f, 1f);
		float num = 25f;
		float num2 = 25f;
		float speed = Mathf.Lerp(enemy.NavMeshAgent.DefaultSpeed, num, overrideAgentLerp);
		float speed2 = Mathf.Lerp(enemy.Rigidbody.positionSpeedChase, num2, overrideAgentLerp);
		enemy.NavMeshAgent.OverrideAgent(speed, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		enemy.Rigidbody.OverrideFollowPosition(1f, speed2);
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
	}

	private void RotationLogic()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.StuckAttack)
		{
			if (Vector3.Distance(stuckAttackTarget, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(stuckAttackTarget - ((Component)enemy.Rigidbody).transform.position);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
		}
		else if (currentState == State.LookUnderStart || currentState == State.LookUnder || currentState == State.LookUnderAttack)
		{
			if (Vector3.Distance(lookUnderPosition, ((Component)this).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(lookUnderPosition - ((Component)this).transform.position);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
		}
		else if (currentState == State.TargetPlayer || currentState == State.Attack)
		{
			if (Object.op_Implicit((Object)(object)targetPlayer) && Vector3.Distance(((Component)targetPlayer).transform.position, ((Component)this).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(((Component)targetPlayer).transform.position - ((Component)this).transform.position);
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
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(rotationSpring, rotationTarget);
	}

	private void EndPieceLogic()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		endPieceSource.rotation = SemiFunc.SpringQuaternionGet(endPieceSpring, endPieceTarget.rotation);
		endPieceTarget.localEulerAngles = new Vector3((0f - enemy.Rigidbody.physGrabObject.rbVelocity.y) * 30f, 0f, 0f);
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
			UpdateState(State.StuckAttack);
		}
		else
		{
			UpdateState(State.Idle);
		}
	}

	private void RigidbodyRotationSpeed()
	{
		if (currentState == State.Roam)
		{
			enemy.Rigidbody.rotationSpeedIdle = 1f;
			enemy.Rigidbody.rotationSpeedChase = 1f;
		}
		else
		{
			enemy.Rigidbody.rotationSpeedIdle = 2f;
			enemy.Rigidbody.rotationSpeedChase = 2f;
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		stateImpulse = true;
		if (currentState == State.Spawn)
		{
			robeAnim.SetSpawn();
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

	[PunRPC]
	private void UpdateOnScreenRPC(bool _onScreen)
	{
		isOnScreen = _onScreen;
	}

	[PunRPC]
	private void AttackImpulseRPC()
	{
		attackImpulse = true;
	}

	[PunRPC]
	private void LookUnderAttackImpulseRPC()
	{
		lookUnderAttackImpulse = true;
	}

	[PunRPC]
	private void IdleBreakRPC()
	{
		idleBreakTrigger = true;
	}
}
