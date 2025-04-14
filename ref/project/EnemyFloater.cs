using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFloater : MonoBehaviour
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
		ChargeAttack,
		DelayAttack,
		Attack,
		Stun,
		Leave,
		Despawn
	}

	public State currentState;

	public float stateTimer;

	public EnemyFloaterAnim animator;

	public ParticleSystem particleDeathImpact;

	public ParticleSystem particleDeathBitsFar;

	public ParticleSystem particleDeathBitsShort;

	public ParticleSystem particleDeathSmoke;

	public SpringQuaternion rotationSpring;

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

	public Transform followParentTransform;

	public AnimationCurve followParentCurve;

	private float followParentLerp;

	private float grabAggroTimer;

	private int attackCount;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			FloatingAnimation();
			if (enemy.CurrentState == EnemyState.Despawn && !enemy.IsStunned() && currentState == State.Idle)
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
			case State.ChargeAttack:
				StateChargeAttack();
				break;
			case State.DelayAttack:
				StateDelayAttack();
				break;
			case State.Attack:
				StateAttack();
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
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
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
			if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
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
				UpdateState(State.Idle);
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

	public void StateNotice()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
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
			if (Vector3.Distance(feetTransform.position, ((Component)targetPlayer).transform.position) < 2.5f)
			{
				UpdateState(State.ChargeAttack);
			}
			else
			{
				UpdateState(State.GoToPlayer);
			}
		}
	}

	public void StateGoToPlayer()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)targetPlayer))
		{
			UpdateState(State.Idle);
			return;
		}
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		targetPosition = ((Component)targetPlayer).transform.position;
		enemy.NavMeshAgent.SetDestination(targetPosition);
		enemy.NavMeshAgent.OverrideAgent(2f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(feetTransform.position, enemy.NavMeshAgent.GetPoint()) < 2f && stateTimer > 1.5f)
		{
			UpdateState(State.ChargeAttack);
		}
	}

	public void StateSneak()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
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
		enemy.NavMeshAgent.OverrideAgent(1.5f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(feetTransform.position, enemy.NavMeshAgent.GetPoint()) < 2f || enemy.OnScreen.OnScreenAny)
		{
			UpdateState(State.Notice);
		}
	}

	public void StateChargeAttack()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 7f;
			enemy.NavMeshAgent.Warp(feetTransform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.DelayAttack);
		}
	}

	public void StateDelayAttack()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Attack);
		}
	}

	public void StateAttack()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			attackCount++;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			if (attackCount >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				attackCount = 0;
				UpdateState(State.Leave);
			}
			else
			{
				UpdateState(State.Idle);
			}
		}
	}

	public void StateStun()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
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
		enemy.NavMeshAgent.OverrideAgent(1.5f, enemy.NavMeshAgent.DefaultAcceleration, 0.2f);
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		((Component)particleDeathImpact).transform.position = enemy.CenterTransform.position;
		particleDeathImpact.Play();
		((Component)particleDeathBitsFar).transform.position = enemy.CenterTransform.position;
		particleDeathBitsFar.Play();
		((Component)particleDeathBitsShort).transform.position = enemy.CenterTransform.position;
		particleDeathBitsShort.Play();
		((Component)particleDeathSmoke).transform.position = enemy.CenterTransform.position;
		particleDeathSmoke.Play();
		animator.SfxDeath();
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
		else if ((currentState == State.GoToPlayer || currentState == State.Sneak) && (Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			stateTimer = 2f;
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

	private void FloatingAnimation()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.1f;
		float num2 = 0.4f;
		float num3 = followParentCurve.Evaluate(followParentLerp);
		float num4 = Mathf.Lerp(0f - num, num, num3);
		float num5 = 0f;
		Vector3 localPosition = default(Vector3);
		((Vector3)(ref localPosition))._002Ector(followParentTransform.localPosition.x, num4 + num5, followParentTransform.localPosition.z);
		followParentLerp += Time.deltaTime * num2;
		if (followParentLerp > 1f)
		{
			followParentLerp = 0f;
		}
		followParentTransform.localPosition = localPosition;
	}

	private void RotationLogic()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Notice)
		{
			if (Object.op_Implicit((Object)(object)targetPlayer) && Vector3.Distance(((Component)targetPlayer).transform.position, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position);
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

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
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
		animator.NoticeSet(_playerID);
	}
}
