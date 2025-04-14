using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHidden : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		PlayerNotice,
		PlayerGoTo,
		PlayerPickup,
		PlayerMove,
		PlayerRelease,
		PlayerReleaseWait,
		Leave,
		Stun,
		StunEnd,
		Despawn
	}

	[Space]
	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	[Space]
	public Enemy enemy;

	public EnemyHiddenAnim enemyHiddenAnim;

	private PhotonView photonView;

	[Space]
	public Transform playerPickupTransform;

	public AnimationCurve playerPickupCurveUp;

	public AnimationCurve playerPickupCurveSide;

	private float playerPickupLerpUp;

	private float playerPickupLerpSide;

	private Vector3 playerPickupPositionOriginal;

	[Space]
	public SpringQuaternion rotationSpring;

	private Quaternion rotationTarget;

	private Vector3 agentDestination;

	private PlayerAvatar playerTarget;

	private bool agentSet;

	private float grabAggroTimer;

	private float maxMoveTimer;

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		photonView = ((Component)this).GetComponent<PhotonView>();
		playerPickupPositionOriginal = playerPickupTransform.localPosition;
	}

	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (grabAggroTimer > 0f)
			{
				grabAggroTimer -= Time.deltaTime;
			}
			RotationLogic();
			PlayerPickupTransformLogic();
			if (enemy.IsStunned())
			{
				UpdateState(State.Stun);
			}
			if (enemy.CurrentState == EnemyState.Despawn && !enemy.IsStunned())
			{
				UpdateState(State.Despawn);
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
				break;
			case State.PlayerGoTo:
				StatePlayerGoTo();
				break;
			case State.PlayerPickup:
				StatePlayerPickup();
				break;
			case State.PlayerMove:
				StatePlayerMove();
				break;
			case State.PlayerRelease:
				StatePlayerRelease();
				break;
			case State.PlayerReleaseWait:
				StatePlayerReleaseWait();
				break;
			case State.Leave:
				StateLeave();
				break;
			case State.Stun:
				StateStun();
				break;
			case State.StunEnd:
				StateStunEnd();
				break;
			case State.Despawn:
				StateDespawn();
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			PlayerTumbleLogic();
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
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StateInvestigate()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
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
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StatePlayerNotice()
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
			UpdateState(State.PlayerGoTo);
		}
	}

	private void StatePlayerGoTo()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			agentSet = true;
		}
		stateTimer -= Time.deltaTime;
		if (!Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled || stateTimer <= 0f)
		{
			UpdateState(State.Leave);
			return;
		}
		SemiFunc.EnemyCartJump(enemy);
		if (enemy.Jump.jumping)
		{
			enemy.NavMeshAgent.Disable(0.5f);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)playerTarget).transform.position, 5f * Time.deltaTime);
			agentSet = true;
		}
		else if (!enemy.NavMeshAgent.IsDisabled())
		{
			if (!agentSet && enemy.NavMeshAgent.HasPath() && Vector3.Distance(((Component)enemy.Rigidbody).transform.position + Vector3.down * 0.75f, enemy.NavMeshAgent.GetDestination()) < 0.25f)
			{
				enemy.Jump.StuckTrigger(((Component)enemy.Rigidbody).transform.position - ((Component)playerTarget).transform.position);
			}
			enemy.NavMeshAgent.SetDestination(((Component)playerTarget).transform.position);
			enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
			agentSet = false;
		}
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)playerTarget).transform.position) < 1.5f)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.PlayerPickup);
		}
	}

	private void StatePlayerPickup()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
		}
		if (!Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.Leave);
			return;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.PlayerMove);
		}
	}

	private void StatePlayerMove()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			maxMoveTimer = 15f;
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, 50f, 999f);
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
				stateTimer = 0f;
			}
			stateImpulse = false;
		}
		if (enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
		}
		if (!Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.Leave);
			return;
		}
		SemiFunc.EnemyCartJump(enemy);
		enemy.NavMeshAgent.SetDestination(agentDestination);
		enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		enemy.Jump.GapJumpOverride(0.1f, 20f, 20f);
		maxMoveTimer -= Time.deltaTime;
		if (!enemy.NavMeshAgent.HasPath() || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)playerTarget).transform.position) > 5f || stateTimer <= 0f || maxMoveTimer <= 0f)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.PlayerRelease);
		}
	}

	private void StatePlayerRelease()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		stateTimer -= Time.deltaTime;
		if (!Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.Leave);
		}
		else if (stateTimer <= 0f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)playerTarget).transform.position) > 5f)
		{
			UpdateState(State.PlayerReleaseWait);
		}
	}

	private void StatePlayerReleaseWait()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Leave);
		}
	}

	private void StateLeave()
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
		if (enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		SemiFunc.EnemyCartJump(enemy);
		if (Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f || stateTimer <= 0f)
		{
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Idle);
		}
	}

	private void StateStun()
	{
		if (!enemy.IsStunned())
		{
			UpdateState(State.StunEnd);
		}
	}

	private void StateStunEnd()
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

	private void StateDespawn()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			enemy.EnemyParent.Despawn();
			UpdateState(State.Spawn);
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
		enemyHiddenAnim.Hurt();
	}

	public void OnDeath()
	{
		enemyHiddenAnim.Death();
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
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate)
		{
			playerTarget = enemy.Vision.onVisionTriggeredPlayer;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			UpdateState(State.PlayerNotice);
		}
		else if (currentState == State.PlayerGoTo)
		{
			stateTimer = 2f;
		}
	}

	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && !(grabAggroTimer > 0f) && currentState == State.Leave)
		{
			grabAggroTimer = 60f;
			playerTarget = enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			UpdateState(State.PlayerNotice);
		}
	}

	private void UpdateState(State _state)
	{
		if (currentState != _state)
		{
			enemy.Rigidbody.StuckReset();
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.PlayerNotice || currentState == State.PlayerGoTo)
		{
			if (Vector3.Distance(((Component)playerTarget).transform.position, ((Component)this).transform.position) > 0.1f)
			{
				rotationTarget = Quaternion.LookRotation(((Component)playerTarget).transform.position - ((Component)this).transform.position);
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

	private void PlayerTumbleLogic()
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.PlayerPickup || currentState == State.PlayerMove || currentState == State.PlayerRelease) && Object.op_Implicit((Object)(object)playerTarget) && !playerTarget.isDisabled)
		{
			if (!playerTarget.tumble.isTumbling)
			{
				playerTarget.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			}
			playerTarget.tumble.TumbleOverrideTime(3f);
			playerTarget.FallDamageResetSet(0.1f);
			playerTarget.tumble.physGrabObject.OverrideMass(1f);
			playerTarget.tumble.physGrabObject.OverrideAngularDrag(2f);
			playerTarget.tumble.physGrabObject.OverrideDrag(1f);
			playerTarget.tumble.OverrideEnemyHurt(0.1f);
			float num = 1f;
			if (playerTarget.tumble.physGrabObject.playerGrabbing.Count > 0)
			{
				num = 0.5f;
			}
			else if (currentState == State.PlayerRelease || currentState == State.PlayerPickup)
			{
				num = 0.75f;
			}
			Vector3 val = SemiFunc.PhysFollowPosition(((Component)playerTarget.tumble).transform.position, playerPickupTransform.position, playerTarget.tumble.rb.velocity, 10f * num);
			playerTarget.tumble.rb.AddForce(val * (10f * Time.fixedDeltaTime * num), (ForceMode)1);
			Vector3 val2 = SemiFunc.PhysFollowRotation(((Component)playerTarget.tumble).transform, playerPickupTransform.rotation, playerTarget.tumble.rb, 0.2f * num);
			playerTarget.tumble.rb.AddTorque(val2 * (1f * Time.fixedDeltaTime * num), (ForceMode)1);
		}
	}

	private void PlayerPickupTransformLogic()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.PlayerMove || currentState == State.PlayerPickup || currentState == State.PlayerRelease)
		{
			float magnitude = ((Vector3)(ref enemy.Rigidbody.velocity)).magnitude;
			Vector3 angularVelocity = enemy.Rigidbody.rb.angularVelocity;
			float num = (magnitude + ((Vector3)(ref angularVelocity)).magnitude) * 0.5f;
			num = Mathf.Clamp(num, 0f, 1f);
			float num2 = playerPickupCurveUp.Evaluate(playerPickupLerpUp) - 0.5f;
			float num3 = playerPickupCurveSide.Evaluate(playerPickupLerpSide) - 0.5f;
			playerPickupLerpUp += 2f * Time.deltaTime * num;
			if (playerPickupLerpUp > 1f)
			{
				playerPickupLerpUp -= 1f;
			}
			playerPickupLerpSide += 1f * Time.deltaTime * num;
			if (playerPickupLerpSide > 1f)
			{
				playerPickupLerpSide -= 1f;
			}
			playerPickupTransform.localPosition = Vector3.Lerp(playerPickupTransform.localPosition, new Vector3(playerPickupPositionOriginal.x + num3 * 0.2f, playerPickupPositionOriginal.y + num2 * 0.2f, playerPickupPositionOriginal.z), 50f * Time.deltaTime);
		}
		else
		{
			playerPickupLerpSide = 0f;
			playerPickupLerpUp = 0f;
			playerPickupTransform.localPosition = Vector3.Lerp(playerPickupTransform.localPosition, playerPickupPositionOriginal, 10f * Time.deltaTime);
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
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
}
