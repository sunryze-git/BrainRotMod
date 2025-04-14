using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemySlowWalker : MonoBehaviour, IPunObservable
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		Notice,
		GoToPlayer,
		Sneak,
		Attack,
		StuckAttack,
		LookUnderStart,
		LookUnderIntro,
		LookUnder,
		LookUnderAttack,
		LookUnderStop,
		Stun,
		Leave,
		Despawn
	}

	public State currentState;

	public float stateTimer;

	public EnemySlowWalkerAnim animator;

	public ParticleSystem particleDeathImpact;

	public ParticleSystem particleDeathBitsFar;

	public ParticleSystem particleDeathBitsShort;

	public ParticleSystem particleDeathSmoke;

	public SpringQuaternion horizontalRotationSpring;

	private Quaternion rotationTarget;

	private bool stateImpulse = true;

	internal PlayerAvatar targetPlayer;

	public Enemy enemy;

	private PhotonView photonView;

	private Vector3 agentDestination;

	private Vector3 backToNavMeshPosition;

	private Vector3 stuckAttackTarget;

	private Vector3 targetPosition;

	private float visionTimer;

	private bool visionPrevious;

	public Transform feetTransform;

	private float grabAggroTimer;

	private int attackCount;

	private Vector3 lookUnderPosition;

	private Vector3 lookUnderLookAtPosition;

	private Vector3 lookUnderPositionNavmesh;

	internal bool lookUnderAttackImpulse;

	public Transform lookAtTransform;

	private float visionDotStandingDefault;

	internal bool attackOffsetActive;

	public Transform attackOffsetTransform;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		visionDotStandingDefault = enemy.Vision.VisionDotStanding;
	}

	private void Update()
	{
		HeadLookAt();
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
			case State.Notice:
				StateNotice();
				break;
			case State.GoToPlayer:
				StateGoToPlayer();
				break;
			case State.Sneak:
				StateSneak();
				break;
			case State.Attack:
				StateAttack();
				break;
			case State.StuckAttack:
				StateStuckAttack();
				break;
			case State.LookUnderStart:
				StateLookUnderStart();
				break;
			case State.LookUnderIntro:
				StateLookUnderIntro();
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
			VisionDotLogic();
			AttackOffsetLogic();
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
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
			enemy.NavMeshAgent.Warp(feetTransform.position);
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

	public void StateRoam()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
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
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (enemy.Rigidbody.notMovingTimer > 3f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f)
			{
				AttackNearestPhysObjectOrGoToIdle();
				return;
			}
			if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
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
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
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
			if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
			{
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	public void StateNotice()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.GoToPlayer);
		}
	}

	public void StateGoToPlayer()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.Idle);
			return;
		}
		if (stateImpulse)
		{
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
			stateTimer = 10f;
		}
		enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(feetTransform.position, enemy.NavMeshAgent.GetPoint()) < 8f && stateTimer > 1.5f && enemy.Jump.timeSinceJumped > 1f)
		{
			UpdateState(State.Attack);
		}
		else if (stateTimer > 9f && targetPlayer.isCrawling && !targetPlayer.isTumbling && Vector3.Distance(enemy.NavMeshAgent.GetPoint(), ((Component)targetPlayer).transform.position) > 0.5f && Vector3.Distance(((Component)targetPlayer).transform.position, targetPlayer.LastNavmeshPosition) < 3f)
		{
			UpdateState(State.LookUnderStart);
		}
		else if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
		}
	}

	public void StateSneak()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
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
		enemy.NavMeshAgent.OverrideAgent(0.8f, 30f, 0.2f);
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(feetTransform.position, enemy.NavMeshAgent.GetPoint()) < 5f)
		{
			UpdateState(State.GoToPlayer);
		}
		else if (enemy.OnScreen.OnScreenAny)
		{
			UpdateState(State.Notice);
		}
	}

	public void StateAttack()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 5f;
			attackCount++;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateStuckAttack()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(feetTransform.position);
			stateTimer = 3f;
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
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.Idle);
			return;
		}
		if (stateImpulse)
		{
			lookUnderPosition = ((Component)targetPlayer).transform.position;
			lookUnderLookAtPosition = lookUnderPosition;
			lookUnderPositionNavmesh = targetPlayer.LastNavmeshPosition;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateTimer = 1f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.SetDestination(lookUnderPositionNavmesh);
		if (Vector3.Distance(((Component)this).transform.position, lookUnderPositionNavmesh) < 0.5f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.LookUnderIntro);
			}
		}
		else if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateLookUnderIntro()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.LookUnder);
		}
	}

	public void StateLookUnder()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
		}
		stateTimer -= Time.deltaTime;
		enemy.Vision.StandOverride(0.25f);
		if (targetPlayer.isCrawling && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)targetPlayer).transform.position) < 3f && Vector3.Dot(lookAtTransform.forward, ((Component)targetPlayer).transform.position - lookAtTransform.position) > 0.5f)
		{
			UpdateState(State.LookUnderAttack);
		}
		else if (stateTimer <= 0f)
		{
			UpdateState(State.LookUnderStop);
		}
	}

	public void StateLookUnderAttack()
	{
		if (stateImpulse)
		{
			stateTimer = 0.6f;
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

	public void StateLookUnderStop()
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

	public void StateStun()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			if (!enemy.Rigidbody.grabbed)
			{
				enemy.Rigidbody.rb.AddTorque(-((Component)this).transform.right * 15f, (ForceMode)1);
			}
			stateImpulse = false;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	public void StateLeave()
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
		if (enemy.Rigidbody.notMovingTimer > 3f)
		{
			stateTimer -= Time.deltaTime;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f || stateTimer <= 0f)
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
		((Component)particleDeathImpact).transform.position = enemy.CenterTransform.position;
		particleDeathImpact.Play();
		((Component)particleDeathBitsFar).transform.position = enemy.CenterTransform.position;
		particleDeathBitsFar.Play();
		((Component)particleDeathBitsShort).transform.position = enemy.CenterTransform.position;
		particleDeathBitsShort.Play();
		((Component)particleDeathSmoke).transform.position = enemy.CenterTransform.position;
		particleDeathSmoke.Play();
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
		if (SemiFunc.IsMasterClientOrSingleplayer() && (currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate))
		{
			agentDestination = enemy.StateInvestigate.onInvestigateTriggeredPosition;
			UpdateState(State.Investigate);
		}
	}

	public void OnVision()
	{
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			return;
		}
		if (currentState == State.Roam || currentState == State.Idle || currentState == State.Investigate)
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
		else if (currentState == State.GoToPlayer || currentState == State.Sneak)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
			{
				stateTimer = 10f;
			}
		}
		else if (currentState == State.LookUnderStart)
		{
			if ((Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer && !targetPlayer.isCrawling)
			{
				UpdateState(State.GoToPlayer);
			}
		}
		else if ((currentState == State.LookUnder || currentState == State.LookUnderIntro || currentState == State.LookUnderAttack) && (Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			if (targetPlayer.isCrawling)
			{
				lookUnderLookAtPosition = ((Component)targetPlayer).transform.position;
			}
			else
			{
				UpdateState(State.LookUnderStop);
			}
		}
	}

	public void OnGrabbed()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer() || grabAggroTimer > 0f || currentState != State.Leave)
		{
			return;
		}
		grabAggroTimer = 60f;
		PlayerAvatar onGrabbedPlayerAvatar = enemy.Rigidbody.onGrabbedPlayerAvatar;
		if (((Component)onGrabbedPlayerAvatar).transform.position.y - ((Component)enemy).transform.position.y > 1.15f || ((Component)onGrabbedPlayerAvatar).transform.position.y - ((Component)enemy).transform.position.y < -1f)
		{
			return;
		}
		targetPlayer = onGrabbedPlayerAvatar;
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
			else
			{
				NoticeRPC(targetPlayer.photonView.ViewID);
			}
		}
		UpdateState(State.Notice);
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

	private void RotationLogic()
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Notice)
		{
			if (Object.op_Implicit((Object)(object)targetPlayer) && Vector3.Distance(((Component)targetPlayer).transform.position, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
		}
		else if (currentState == State.LookUnderStart || currentState == State.LookUnderIntro || currentState == State.LookUnder || currentState == State.LookUnderAttack)
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
		if (currentState == State.Attack)
		{
			if (Object.op_Implicit((Object)(object)targetPlayer) && Vector3.Distance(((Component)targetPlayer).transform.position, ((Component)enemy.Rigidbody).transform.position) > 0.1f && stateTimer > 2.5f)
			{
				rotationTarget = Quaternion.LookRotation(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position);
				((Quaternion)(ref rotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotationTarget)).eulerAngles.y, 0f);
			}
			horizontalRotationSpring.speed = 15f;
			horizontalRotationSpring.damping = 0.8f;
		}
		else
		{
			horizontalRotationSpring.speed = 5f;
			horizontalRotationSpring.damping = 0.7f;
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(horizontalRotationSpring, rotationTarget);
	}

	private void HeadLookAt()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.LookUnder || currentState == State.LookUnderAttack)
		{
			Vector3 direction = lookUnderLookAtPosition - lookAtTransform.position;
			direction = SemiFunc.ClampDirection(direction, ((Component)this).transform.forward, 60f);
			Quaternion localRotation = lookAtTransform.localRotation;
			lookAtTransform.rotation = Quaternion.LookRotation(direction);
			Quaternion localRotation2 = lookAtTransform.localRotation;
			((Quaternion)(ref localRotation2)).eulerAngles = new Vector3(0f, ((Quaternion)(ref localRotation2)).eulerAngles.y, 0f);
			lookAtTransform.localRotation = Quaternion.Lerp(localRotation, localRotation2, Time.deltaTime * 10f);
			enemy.Vision.VisionTransform.rotation = lookAtTransform.rotation;
		}
		else
		{
			lookAtTransform.localRotation = Quaternion.Lerp(lookAtTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
			enemy.Vision.VisionTransform.localRotation = Quaternion.identity;
		}
		animator.SpringLogic();
	}

	private void VisionDotLogic()
	{
		if (currentState == State.LookUnder)
		{
			enemy.Vision.VisionDotStanding = 0f;
		}
		else
		{
			enemy.Vision.VisionDotStanding = visionDotStandingDefault;
		}
	}

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
	}

	private void AttackOffsetLogic()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.Attack)
		{
			attackOffsetActive = false;
		}
		if (attackOffsetActive)
		{
			attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(attackOffsetTransform.localPosition.z, 1.5f, Time.deltaTime * 4f));
			enemy.Rigidbody.OverrideFollowPosition(0.2f, 5f, 40f);
		}
		else
		{
			attackOffsetTransform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(attackOffsetTransform.localPosition.z, 0f, Time.deltaTime * 1f));
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
			UpdateState(State.StuckAttack);
		}
		else
		{
			UpdateState(State.Idle);
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

	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)lookUnderLookAtPosition);
		}
		else
		{
			lookUnderLookAtPosition = (Vector3)stream.ReceiveNext();
		}
	}
}
