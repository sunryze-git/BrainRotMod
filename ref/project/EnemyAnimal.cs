using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimal : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		PlayerNotice,
		GoToPlayer,
		WreakHavoc,
		Leave
	}

	private Enemy enemy;

	private PhotonView photonView;

	public EnemyAnimalAnim enemyAnimalAnim;

	public GameObject welts;

	public State currentState;

	private float havocTimer;

	private LevelPoint ignorePoint;

	private float stateTimer;

	private bool stateImpulse;

	private Vector3 agentDestination;

	private PlayerAvatar playerTarget;

	private float grabAggroTimer;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.PlayerNotice || currentState == State.GoToPlayer || currentState == State.WreakHavoc)
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(((Component)this).transform.position, ((Component)player).transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(player, enemy.Vision.VisionTransform.position, 0.1f, ((Component)this).gameObject);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (grabAggroTimer > 0f)
			{
				grabAggroTimer -= Time.deltaTime;
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
			case State.PlayerNotice:
				StatePlayerNotice();
				PlayerLookAt();
				break;
			case State.GoToPlayer:
				StateGoToPlayer();
				break;
			case State.WreakHavoc:
				StateWreakHavoc();
				break;
			case State.Leave:
				StateLeave();
				break;
			}
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
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateTimer = Random.Range(2f, 6f);
			stateImpulse = false;
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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
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
			stateTimer = 5f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
		}
		if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
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
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
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
			enemy.NavMeshAgent.OverrideAgent(4f, 12f, 0.25f);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 2f)
			{
				SemiFunc.EnemyCartJumpReset(enemy);
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StatePlayerNotice()
	{
		if (stateImpulse)
		{
			stateTimer = 0.5f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.ResetPath();
		enemy.NavMeshAgent.Stop(0.1f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.NavMeshAgent.Stop(0f);
			UpdateState(State.GoToPlayer);
		}
	}

	private void StateGoToPlayer()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		enemy.NavMeshAgent.SetDestination(((Component)playerTarget).transform.position);
		if (stateImpulse)
		{
			stateTimer = 10f;
			stateImpulse = false;
			return;
		}
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)playerTarget).transform.position) < 3f)
		{
			enemy.NavMeshAgent.ResetPath();
			UpdateState(State.WreakHavoc);
			return;
		}
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetDestination()) < 1f && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)playerTarget).transform.position) > 1.5f)
		{
			enemy.Jump.StuckTrigger(((Component)playerTarget).transform.position - ((Component)enemy.Rigidbody).transform.position);
			enemy.Rigidbody.DisableFollowPosition(1f, 10f);
		}
		enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Leave);
		}
	}

	private void StateWreakHavoc()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			havocTimer = 0f;
			stateTimer = 20f;
			stateImpulse = false;
		}
		if (havocTimer <= 0f || Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetDestination()) < 0.25f)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointInTargetRoomGet(playerTarget.RoomVolumeCheck, 1f, 10f, ignorePoint);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointInTargetRoomGet(playerTarget.RoomVolumeCheck, 0f, 999f, ignorePoint);
			}
			NavMeshHit val = default(NavMeshHit);
			if (!Object.op_Implicit((Object)(object)levelPoint) || !NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1))
			{
				UpdateState(State.Leave);
				return;
			}
			if (Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				ignorePoint = levelPoint;
				agentDestination = ((NavMeshHit)(ref val)).position;
				enemy.NavMeshAgent.SetDestination(agentDestination);
			}
			havocTimer = 2f;
		}
		enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		havocTimer -= Time.deltaTime;
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Leave);
		}
	}

	private void StateLeave()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, 25f, 50f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f);
			}
			if (Object.op_Implicit((Object)(object)levelPoint))
			{
				agentDestination = ((Component)levelPoint).transform.position;
			}
			else
			{
				enemy.EnemyParent.SpawnedTimerSet(0f);
			}
			stateTimer = 10f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			enemy.NavMeshAgent.OverrideAgent(6f, 12f, 0.25f);
			if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
			{
				UpdateState(State.Idle);
			}
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			UpdateState(State.Spawn);
		}
		if (((Behaviour)enemyAnimalAnim).isActiveAndEnabled)
		{
			enemyAnimalAnim.SetSpawn();
		}
	}

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		enemyAnimalAnim.hurtSound.Play(((Component)enemyAnimalAnim).transform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		enemyAnimalAnim.particleImpact.Play();
		enemyAnimalAnim.particleBits.Play();
		Quaternion rotation = Quaternion.LookRotation(-((Vector3)(ref enemy.Health.hurtDirection)).normalized);
		((Component)enemyAnimalAnim.particleDirectionalBits).transform.rotation = rotation;
		enemyAnimalAnim.particleDirectionalBits.Play();
		((Component)enemyAnimalAnim.particleLegBits).transform.rotation = rotation;
		enemyAnimalAnim.particleLegBits.Play();
		enemyAnimalAnim.deathSound.Play(((Component)enemyAnimalAnim).transform.position);
		enemy.EnemyParent.Despawn();
	}

	public void OnVision()
	{
		if (currentState != State.Idle && currentState != State.Roam && currentState != State.Investigate)
		{
			return;
		}
		playerTarget = enemy.Vision.onVisionTriggeredPlayer;
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { enemy.Vision.onVisionTriggeredID });
			}
			else
			{
				enemyAnimalAnim.NoticeSet(enemy.Vision.onVisionTriggeredID);
			}
		}
		UpdateState(State.PlayerNotice);
	}

	public void OnInvestigate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Roam || currentState == State.Idle || currentState == State.Investigate)
		{
			UpdateState(State.Investigate);
			agentDestination = enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	public void OnGrabbed()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || grabAggroTimer > 0f || currentState != State.Leave)
		{
			return;
		}
		grabAggroTimer = 60f;
		playerTarget = enemy.Rigidbody.onGrabbedPlayerAvatar;
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			else
			{
				enemyAnimalAnim.NoticeSet(playerTarget.photonView.ViewID);
			}
		}
		UpdateState(State.PlayerNotice);
	}

	private void UpdateState(State _nextState)
	{
		stateTimer = 0f;
		stateImpulse = true;
		currentState = _nextState;
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateStateRPC", (RpcTarget)1, new object[1] { _nextState });
		}
	}

	private void PlayerLookAt()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
		val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
		((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, 50f * Time.deltaTime);
	}

	[PunRPC]
	private void UpdateStateRPC(State _nextState)
	{
		currentState = _nextState;
	}

	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		enemyAnimalAnim.NoticeSet(_playerID);
	}
}
