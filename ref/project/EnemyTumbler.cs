using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTumbler : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Notice,
		Investigate,
		MoveToPlayer,
		Tell,
		Tumble,
		TumbleEnd,
		BackToNavmesh,
		Leave,
		Stunned,
		Dead,
		Despawn
	}

	public bool debugSpawn;

	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	internal PlayerAvatar targetPlayer;

	public Enemy enemy;

	public EnemyTumblerAnim enemyTumblerAnim;

	private PhotonView photonView;

	public HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private float roamWaitTimer;

	private Vector3 roamPoint;

	private Vector3 backToNavmeshPosition;

	private Vector3 agentDestination;

	private Quaternion lookDirection;

	private float visionTimer;

	private bool visionPrevious;

	private bool groundedPrevious;

	private float hopMoveTimer;

	[Space]
	public SpringQuaternion headSpring;

	public Transform headTransform;

	public Transform headTargetTransform;

	public Transform headTargetCodeTransform;

	[Space]
	public SpringQuaternion hatSpring;

	public Transform hatTransform;

	public Transform hatTargetTransform;

	[Space]
	public SpringQuaternion mainMeshSpring;

	private Quaternion mainMeshTargetRotation;

	private float grabAggroTimer;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			HopLogic();
			RotationLogic();
			if (visionTimer > 0f)
			{
				visionTimer -= Time.deltaTime;
			}
			if (enemy.IsStunned())
			{
				UpdateState(State.Stunned);
			}
			else if (enemy.CurrentState == EnemyState.Despawn)
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
			case State.Notice:
				StateNotice();
				break;
			case State.Investigate:
				StateInvestigate();
				break;
			case State.MoveToPlayer:
				StateMoveToPlayer();
				break;
			case State.Tell:
				StateTell();
				break;
			case State.Tumble:
				StateTumble();
				break;
			case State.TumbleEnd:
				StateTumbleEnd();
				break;
			case State.BackToNavmesh:
				StateBackToNavmesh();
				break;
			case State.Leave:
				StateLeave();
				break;
			case State.Stunned:
				StateStunned();
				break;
			case State.Dead:
				StateDead();
				break;
			case State.Despawn:
				StateDespawn();
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
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
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
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = Random.Range(3f, 8f);
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

	private void StateNotice()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 1f;
			stateImpulse = false;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.MoveToPlayer);
		}
	}

	private void StateInvestigate()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			if (!enemy.Jump.jumping)
			{
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
				enemy.NavMeshAgent.ResetPath();
			}
			enemy.NavMeshAgent.SetDestination(agentDestination);
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
			SemiFunc.EnemyCartJump(enemy);
			if (enemy.Rigidbody.notMovingTimer > 3f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (!enemy.Jump.jumping && (stateTimer <= 0f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetDestination()) < 1f))
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

	private void StateMoveToPlayer()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		agentDestination = ((Component)targetPlayer).transform.position;
		if (enemy.Grounded.grounded)
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
		}
		stateTimer -= Time.deltaTime;
		Vector3 position = ((Component)targetPlayer).transform.position;
		position.y = ((Component)enemy.Rigidbody).transform.position.y;
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, position) < 7f && !enemy.Jump.jumping && !VisionBlocked())
		{
			UpdateState(State.Tell);
		}
		else if (stateTimer <= 0f || enemy.Rigidbody.notMovingTimer > 3f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateTell()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Tumble);
		}
	}

	private void StateTumble()
	{
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			val = Vector3.Lerp(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position, Vector3.up, 0.6f);
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			enemy.Rigidbody.rb.AddForce(normalized * 40f, (ForceMode)1);
			enemy.Rigidbody.rb.AddTorque(((Component)enemy.Rigidbody).transform.right * 8f, (ForceMode)1);
		}
		enemy.NavMeshAgent.Disable(0.1f);
		enemy.Rigidbody.DisableFollowPosition(0.2f, 10f);
		enemy.Rigidbody.DisableFollowRotation(0.2f, 10f);
		val = enemy.Rigidbody.rb.velocity;
		if (((Vector3)(ref val)).magnitude < 1f)
		{
			stateTimer -= Time.deltaTime;
		}
		((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		NavMeshHit val2 = default(NavMeshHit);
		if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val2, 1f, -1))
		{
			backToNavmeshPosition = ((NavMeshHit)(ref val2)).position;
		}
		if (stateTimer <= 0f)
		{
			UpdateState(State.TumbleEnd);
		}
	}

	private void StateTumbleEnd()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 1f;
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.BackToNavmesh);
		}
	}

	private void StateBackToNavmesh()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val, 1f, -1))
		{
			enemy.NavMeshAgent.Warp(((NavMeshHit)(ref val)).position);
			UpdateState(State.Idle);
		}
	}

	private void StateLeave()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
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

	private void StateStunned()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val, 1f, -1))
		{
			backToNavmeshPosition = ((NavMeshHit)(ref val)).position;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		if (!enemy.IsStunned())
		{
			UpdateState(State.BackToNavmesh);
		}
	}

	private void StateDead()
	{
	}

	private void StateDespawn()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		enemyTumblerAnim.sfxHurt.Play(((Component)this).transform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
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
			UpdateState(State.Notice);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
		else if (currentState == State.MoveToPlayer && (Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			stateTimer = 2f;
		}
	}

	public void OnGrabbed()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && !(grabAggroTimer > 0f) && currentState == State.Leave)
		{
			grabAggroTimer = 60f;
			targetPlayer = enemy.Rigidbody.onGrabbedPlayerAvatar;
			UpdateState(State.Notice);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
	}

	public void OnHurtColliderImpactAny()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("OnHurtColliderImpactAnyRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				OnHurtColliderImpactAnyRPC();
			}
		}
	}

	public void OnHurtColliderImpactPlayer()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			OnHurtColliderImpactPlayerRPC(hurtCollider.onImpactPlayerAvatar.photonView.ViewID);
			return;
		}
		photonView.RPC("OnHurtColliderImpactPlayerRPC", (RpcTarget)0, new object[1] { hurtCollider.onImpactPlayerAvatar.photonView.ViewID });
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
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.Notice || currentState == State.MoveToPlayer || currentState == State.Tell) && !VisionBlocked())
		{
			Quaternion val = Quaternion.LookRotation(targetPlayer.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
			val = (mainMeshTargetRotation = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f));
		}
		else
		{
			Vector3 agentVelocity = enemy.NavMeshAgent.AgentVelocity;
			agentVelocity.y = 0f;
			if (((Vector3)(ref agentVelocity)).magnitude > 1f)
			{
				Vector3 velocity = enemy.Rigidbody.rb.velocity;
				Quaternion val2 = Quaternion.LookRotation(((Vector3)(ref velocity)).normalized);
				mainMeshTargetRotation = Quaternion.Euler(0f, ((Quaternion)(ref val2)).eulerAngles.y, 0f);
			}
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(mainMeshSpring, mainMeshTargetRotation);
		headTargetCodeTransform.localEulerAngles = new Vector3(enemy.Rigidbody.rb.velocity.y * 5f, 0f, 0f);
		headTransform.rotation = SemiFunc.SpringQuaternionGet(headSpring, headTargetTransform.rotation);
		hatTransform.rotation = SemiFunc.SpringQuaternionGet(hatSpring, hatTargetTransform.rotation);
	}

	private bool VisionBlocked()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (visionTimer <= 0f)
		{
			visionTimer = 0.1f;
			Vector3 val = targetPlayer.PlayerVisionTarget.VisionTransform.position - enemy.Vision.VisionTransform.position;
			visionPrevious = Physics.Raycast(enemy.Vision.VisionTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void HopLogic()
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = currentState == State.BackToNavmesh;
		if (currentState == State.Roam || currentState == State.Investigate || currentState == State.MoveToPlayer || currentState == State.Leave || flag)
		{
			float num = 1f;
			if (currentState == State.MoveToPlayer)
			{
				num = 2f;
			}
			if (enemy.Grounded.grounded && !enemy.Jump.jumping)
			{
				enemy.NavMeshAgent.Stop(0.1f);
				if (groundedPrevious != enemy.Grounded.grounded)
				{
					if (flag)
					{
						((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
					}
					else
					{
						enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
					}
				}
			}
			if (hopMoveTimer <= 0f)
			{
				Vector3 steeringTarget = enemy.NavMeshAgent.Agent.steeringTarget;
				Vector3 val = enemy.NavMeshAgent.Agent.steeringTarget - enemy.Rigidbody.physGrabObject.centerPoint;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				steeringTarget.y = enemy.Rigidbody.physGrabObject.centerPoint.y;
				val = steeringTarget - enemy.Rigidbody.physGrabObject.centerPoint;
				Vector3 normalized2 = ((Vector3)(ref val)).normalized;
				bool flag2 = false;
				bool flag3 = false;
				int num2 = 10;
				float num3 = 0.5f;
				float num4 = 2f;
				if (!flag)
				{
					Vector3 val2 = enemy.Rigidbody.physGrabObject.centerPoint + normalized2 * num3;
					bool flag4 = false;
					for (int i = 0; i < num2; i++)
					{
						if (Physics.Raycast(val2, Vector3.down, num4, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())))
						{
							if (flag4)
							{
								flag2 = true;
							}
						}
						else
						{
							if (i < 3)
							{
								flag4 = true;
							}
							flag3 = true;
						}
						val2 += normalized2 * num3;
					}
					enemy.NavMeshAgent.Stop(0f);
				}
				if (flag2)
				{
					enemy.Rigidbody.rb.AddForce(Vector3.up * 30f + normalized * 20f, (ForceMode)1);
					enemy.NavMeshAgent.Warp(enemy.Rigidbody.physGrabObject.centerPoint + normalized * 5f);
					hopMoveTimer = 2.25f;
				}
				else if (!flag && Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
				{
					enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, (ForceMode)1);
					enemy.NavMeshAgent.Warp(enemy.NavMeshAgent.GetPoint());
					hopMoveTimer = 0.75f;
				}
				else if (flag3)
				{
					enemy.Rigidbody.rb.AddForce(Vector3.up * 20f, (ForceMode)1);
					enemy.NavMeshAgent.Warp(enemy.Rigidbody.physGrabObject.centerPoint + normalized * 0.5f);
					hopMoveTimer = 0.75f;
				}
				else
				{
					enemy.Rigidbody.rb.AddForce(Vector3.up * 25f + normalized2 * 10f, (ForceMode)1);
					if (flag)
					{
						((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, backToNavmeshPosition, 2f);
					}
					else
					{
						enemy.NavMeshAgent.Warp(enemy.Rigidbody.physGrabObject.centerPoint + normalized * num);
					}
					hopMoveTimer = 1.25f;
				}
				enemy.Jump.JumpingSet(_jumping: true);
				enemy.Rigidbody.WarpDisable(2f);
				enemy.Grounded.GroundedDisable(0.25f);
			}
			else
			{
				hopMoveTimer -= Time.deltaTime;
			}
		}
		groundedPrevious = enemy.Grounded.grounded;
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		if (currentState == State.Spawn)
		{
			enemyTumblerAnim.OnSpawn();
		}
		if (currentState == State.Tumble)
		{
			enemyTumblerAnim.OnTumble();
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
	private void OnHurtColliderImpactAnyRPC()
	{
		enemyTumblerAnim.SfxOnHurtColliderImpactAny();
	}

	[PunRPC]
	private void OnHurtColliderImpactPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			if (item.photonView.ViewID == _playerID)
			{
				item.tumble.OverrideEnemyHurt(3f);
				break;
			}
		}
	}
}
