using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUpscream : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		PlayerNotice,
		GoToPlayer,
		Attack,
		Leave,
		IdleBreak,
		Stun
	}

	[Header("References")]
	public EnemyUpscreamAnim upscreamAnim;

	internal Enemy enemy;

	public ParticleSystem[] deathEffects;

	public State currentState;

	public State previousState;

	private float stateTimer;

	private bool attackImpulse;

	private bool stateImpulse;

	internal PlayerAvatar targetPlayer;

	private Vector3 targetPosition;

	public Transform visionTransform;

	private float hasVisionTimer;

	private Vector3 agentPoint;

	private float roamWaitTimer;

	private PhotonView photonView;

	[Header("Head")]
	public SpringQuaternion headSpring;

	public Transform headTransform;

	public Transform headIdleTransform;

	[Header("Eyes")]
	public SpringQuaternion eyeLeftSpring;

	[Space(10f)]
	public Transform eyeLeftTransform;

	public Transform eyeLeftIdle;

	public Transform eyeLeftTarget;

	[Space(10f)]
	public SpringQuaternion eyeRightSpring;

	[Space(10f)]
	public Transform eyeRightTransform;

	public Transform eyeRightIdle;

	public Transform eyeRightTarget;

	[Header("Idle Break")]
	public float idleBreakTimeMin = 45f;

	public float idleBreakTimeMax = 90f;

	private float idleBreakTimer;

	private float grabAggroTimer;

	private int attacks;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		HeadLogic();
		EyeLogic();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (grabAggroTimer > 0f)
			{
				grabAggroTimer -= Time.deltaTime;
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
				IdleBreakLogic();
				break;
			case State.Investigate:
				StateInvestigate();
				AgentVelocityRotation();
				break;
			case State.Roam:
				StateRoam();
				AgentVelocityRotation();
				IdleBreakLogic();
				break;
			case State.PlayerNotice:
				StatePlayerNotice();
				break;
			case State.GoToPlayer:
				AgentVelocityRotation();
				StateGoToPlayer();
				break;
			case State.Attack:
				StateAttack();
				break;
			case State.Leave:
				StateLeave();
				break;
			case State.IdleBreak:
				StateIdleBreak();
				break;
			case State.Stun:
				StateStun();
				break;
			}
		}
		if (currentState == State.Attack && Object.op_Implicit((Object)(object)targetPlayer))
		{
			if (targetPlayer.isLocal)
			{
				PlayerController.instance.InputDisable(0.1f);
				CameraAim.Instance.AimTargetSet(visionTransform.position, 0.1f, 5f, ((Component)this).gameObject, 90);
				CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 5f, 5f, ((Component)this).gameObject, 50);
				Color color = default(Color);
				((Color)(ref color))._002Ector(0.4f, 0f, 0f, 1f);
				PostProcessing.Instance.VignetteOverride(color, 0.75f, 1f, 3.5f, 2.5f, 0.5f, ((Component)this).gameObject);
			}
			if (attackImpulse)
			{
				if (targetPlayer.isLocal)
				{
					targetPlayer.physGrabber.ReleaseObject();
					CameraGlitch.Instance.PlayLong();
				}
				attackImpulse = false;
				upscreamAnim.animator.SetTrigger("Attack");
			}
		}
		else
		{
			attackImpulse = true;
		}
	}

	private void StateSpawn()
	{
		if (stateImpulse)
		{
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			if (previousState == State.Spawn)
			{
				stateTimer = 0.5f;
			}
			else
			{
				stateTimer = Random.Range(3f, 8f);
			}
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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetDestination());
		if (stateImpulse || !enemy.NavMeshAgent.HasPath() || num < 1f)
		{
			if (stateImpulse)
			{
				roamWaitTimer = 0f;
				stateImpulse = false;
			}
			if (roamWaitTimer <= 0f)
			{
				stateTimer = 5f;
				roamWaitTimer = Random.Range(0f, 5f);
				LevelPoint levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 5f, 15f);
				if (!Object.op_Implicit((Object)(object)levelPoint))
				{
					levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 0f, 999f);
				}
				NavMeshHit val = default(NavMeshHit);
				if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
				{
					agentPoint = ((NavMeshHit)(ref val)).position;
					enemy.NavMeshAgent.SetDestination(agentPoint);
				}
			}
			else
			{
				roamWaitTimer -= Time.deltaTime;
			}
		}
		else
		{
			SemiFunc.EnemyCartJump(enemy);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
				if (stateTimer <= 0f)
				{
					UpdateState(State.Idle);
				}
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StateInvestigate()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 5f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentPoint);
			SemiFunc.EnemyCartJump(enemy);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentPoint) < 2f)
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 0.5f;
			stateImpulse = false;
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
		}
		enemy.NavMeshAgent.Stop(0.5f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.NavMeshAgent.Stop(0f);
			UpdateState(State.GoToPlayer);
		}
	}

	private void StateGoToPlayer()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		if (!enemy.Jump.jumping)
		{
			enemy.NavMeshAgent.SetDestination(((Component)targetPlayer).transform.position);
		}
		else
		{
			enemy.NavMeshAgent.Disable(0.1f);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)targetPlayer).transform.position, 5f * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(enemy);
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
			return;
		}
		enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)targetPlayer).transform.position) < 1.5f && !enemy.Jump.jumping && !enemy.IsStunned())
		{
			enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.Attack);
			return;
		}
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (stateTimer <= 0f)
			{
				enemy.Jump.StuckReset();
				UpdateState(State.Leave);
			}
			else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)targetPlayer).transform.position) > 1.5f)
			{
				enemy.Jump.StuckTrigger(((Component)targetPlayer).transform.position - ((Component)enemy.Rigidbody).transform.position);
			}
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Leave);
		}
	}

	private void StateAttack()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			attacks++;
			stateTimer = 1.5f;
			stateImpulse = false;
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
		}
		Quaternion val = Quaternion.LookRotation(targetPlayer.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
		val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
		((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, 50f * Time.deltaTime);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			if (attacks >= 3 || Random.Range(0f, 1f) <= 0.1f)
			{
				UpdateState(State.Leave);
			}
			else
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateLeave()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
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
				agentPoint = ((NavMeshHit)(ref val)).position;
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
		SemiFunc.EnemyCartJump(enemy);
		enemy.NavMeshAgent.SetDestination(agentPoint);
		enemy.NavMeshAgent.OverrideAgent(enemy.NavMeshAgent.DefaultSpeed + 2.5f, enemy.NavMeshAgent.DefaultAcceleration + 2.5f, 0.2f);
		enemy.Rigidbody.OverrideFollowPosition(1f, 10f);
		if (Vector3.Distance(((Component)this).transform.position, agentPoint) < 1f || stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateIdleBreak()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateTimer = 2f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	private void StateStun()
	{
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	internal void UpdateState(State _state)
	{
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && currentState != _state)
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdateStateRPC", (RpcTarget)0, new object[1] { _state });
			}
			else
			{
				UpdateStateRPC(_state);
			}
		}
	}

	private void IdleBreakLogic()
	{
		if (idleBreakTimer >= 0f)
		{
			idleBreakTimer -= Time.deltaTime;
			if (idleBreakTimer <= 0f)
			{
				SemiFunc.EnemyCartJumpReset(enemy);
				UpdateState(State.IdleBreak);
				idleBreakTimer = Random.Range(idleBreakTimeMin, idleBreakTimeMax);
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
		upscreamAnim.hurtSound.Play(((Component)upscreamAnim).transform.position);
	}

	public void OnDeath()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem[] array = deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnVision()
	{
		if (currentState == State.Idle || currentState == State.Roam || currentState == State.IdleBreak || currentState == State.Investigate)
		{
			targetPlayer = enemy.Vision.onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
			if (!enemy.IsStunned())
			{
				if (GameManager.Multiplayer())
				{
					photonView.RPC("NoticeSetRPC", (RpcTarget)0, new object[1] { enemy.Vision.onVisionTriggeredID });
				}
				else
				{
					upscreamAnim.NoticeSet(enemy.Vision.onVisionTriggeredID);
				}
			}
			UpdateState(State.PlayerNotice);
		}
		else if (currentState == State.GoToPlayer && (Object)(object)targetPlayer == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			stateTimer = 2f;
		}
	}

	public void OnInvestigate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Roam || currentState == State.Idle || currentState == State.IdleBreak || currentState == State.Investigate)
		{
			UpdateState(State.Investigate);
			agentPoint = enemy.StateInvestigate.onInvestigateTriggeredPosition;
		}
	}

	public void OnGrabbed()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || grabAggroTimer > 0f || currentState != State.Leave)
		{
			return;
		}
		grabAggroTimer = 60f;
		if ((Object)(object)targetPlayer != (Object)(object)enemy.Rigidbody.onGrabbedPlayerAvatar)
		{
			targetPlayer = enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeSetRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
			else
			{
				upscreamAnim.NoticeSet(targetPlayer.photonView.ViewID);
			}
		}
		UpdateState(State.PlayerNotice);
	}

	public void HeadLogic()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Quaternion targetRotation = headIdleTransform.rotation;
		if (Object.op_Implicit((Object)(object)targetPlayer) && (currentState == State.PlayerNotice || currentState == State.GoToPlayer || currentState == State.Attack) && !enemy.IsStunned())
		{
			Vector3 val = targetPlayer.PlayerVisionTarget.VisionTransform.position;
			if (targetPlayer.isLocal)
			{
				val = targetPlayer.localCameraPosition;
			}
			targetRotation = Quaternion.LookRotation(val - headTransform.position);
		}
		headTransform.rotation = SemiFunc.SpringQuaternionGet(headSpring, targetRotation);
	}

	public void EyeLogic()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.PlayerNotice || currentState == State.GoToPlayer || currentState == State.Attack)
		{
			eyeLeftSpring.damping = 0.6f;
			eyeLeftSpring.speed = 15f;
			eyeRightSpring.damping = 0.6f;
			eyeRightSpring.speed = 15f;
			eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(eyeLeftSpring, eyeLeftTarget.rotation);
			eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(eyeRightSpring, eyeRightTarget.rotation);
		}
		else
		{
			eyeLeftSpring.damping = 0.2f;
			eyeLeftSpring.speed = 15f;
			eyeRightSpring.damping = 0.2f;
			eyeRightSpring.speed = 15f;
			eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(eyeLeftSpring, eyeLeftIdle.rotation);
			eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(eyeRightSpring, eyeRightIdle.rotation);
		}
	}

	private void AgentVelocityRotation()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).magnitude > 0.005f)
		{
			Quaternion val = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			float num = 2f;
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, num * Time.deltaTime);
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		previousState = currentState;
		currentState = _state;
		stateImpulse = true;
		stateTimer = 0f;
		if (currentState == State.Spawn)
		{
			upscreamAnim.SetSpawn();
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
	private void NoticeSetRPC(int _playerID)
	{
		upscreamAnim.NoticeSet(_playerID);
	}
}
