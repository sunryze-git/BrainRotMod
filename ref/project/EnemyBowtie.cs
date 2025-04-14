using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBowtie : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		PlayerNotice,
		Yell,
		YellEnd,
		Leave,
		Stun,
		Despawn
	}

	private PhotonView photonView;

	public State currentState;

	public float stateTimer;

	private bool stateImpulse;

	[Space]
	public EnemyBowtieAnim anim;

	public Transform hurtColliderLeave;

	[Space]
	public SpringQuaternion headSpring;

	public Transform headTransform;

	public Transform HeadTargetTransform;

	[Space]
	public SpringQuaternion eyeRightSpring;

	public Transform eyeRightTransform;

	public Transform eyeRightTargetTransform;

	[Space]
	public SpringQuaternion eyeLeftSpring;

	public Transform eyeLeftTransform;

	public Transform eyeLeftTargetTransform;

	[Space]
	public SpringQuaternion horizontalRotationSpring;

	private Quaternion horizontalRotationTarget;

	[Space]
	public Transform verticalRotationTransform;

	public SpringQuaternion verticalRotationSpring;

	private Quaternion verticalRotationTarget;

	private float roamWaitTimer;

	private Vector3 agentDestination;

	internal Enemy enemy;

	private PlayerAvatar playerTarget;

	private float grabAggroTimer;

	private int attacks;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		enemy = ((Component)this).GetComponent<Enemy>();
	}

	private void Update()
	{
		HurtColliderLeaveLogic();
		SpringLogic();
		PlayerEyesLogic();
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (grabAggroTimer > 0f)
		{
			grabAggroTimer -= Time.deltaTime;
		}
		if (LevelGenerator.Instance.Generated)
		{
			HorizontalRotationLogic();
			VerticalRotationLogic();
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
			case State.Yell:
				StateYell();
				break;
			case State.YellEnd:
				StateYellEnd();
				break;
			case State.Leave:
				StateLeave();
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
			enemy.NavMeshAgent.SetDestination(agentDestination);
			SemiFunc.EnemyCartJump(enemy);
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
			stateTimer = 1f;
			stateImpulse = false;
		}
		enemy.Jump.SurfaceJumpDisable(0.5f);
		enemy.NavMeshAgent.ResetPath();
		enemy.NavMeshAgent.Stop(0.1f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.NavMeshAgent.Stop(0f);
			UpdateState(State.Yell);
		}
	}

	private void StateYell()
	{
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			stateTimer = 5f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		enemy.Jump.SurfaceJumpDisable(0.5f);
		if (stateTimer <= 0f)
		{
			UpdateState(State.YellEnd);
		}
	}

	private void StateYellEnd()
	{
		if (stateImpulse)
		{
			attacks++;
			stateTimer = 1f;
			stateImpulse = false;
		}
		enemy.Jump.SurfaceJumpDisable(0.5f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			if (attacks >= 3 || Random.Range(0f, 1f) <= 0.3f)
			{
				attacks = 0;
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
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, 30f, 60f);
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
			stateTimer = 5f;
			stateImpulse = false;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (enemy.Rigidbody.notMovingTimer > 2f)
			{
				stateTimer -= Time.deltaTime;
			}
			enemy.NavMeshAgent.OverrideAgent(5f, 10f, 0.25f);
			if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, agentDestination) < 1f)
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateStun()
	{
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	private void StateDespawn()
	{
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			UpdateState(State.Idle);
		}
		if (((Behaviour)anim).isActiveAndEnabled)
		{
			anim.OnSpawn();
		}
	}

	public void OnHurt()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		anim.GroanPause();
		anim.StunPause();
		anim.hurtSound.Play(((Component)anim).transform.position);
		if (currentState == State.Yell)
		{
			UpdateState(State.YellEnd);
		}
	}

	public void OnDeath()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		anim.GroanPause();
		anim.StunPause();
		anim.deathSound.Play(enemy.CenterTransform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		anim.particleImpact.Play();
		anim.particleBits.Play();
		anim.particleEyes.Play();
		((Component)anim.particleDirectionalBits).transform.rotation = Quaternion.LookRotation(-((Vector3)(ref enemy.Health.hurtDirection)).normalized);
		anim.particleDirectionalBits.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnVisionTriggered()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState != State.Idle && currentState != State.Roam && currentState != State.Investigate) || enemy.Jump.jumping || enemy.IsStunned())
		{
			return;
		}
		PlayerAvatar onVisionTriggeredPlayer = enemy.Vision.onVisionTriggeredPlayer;
		if (!(Mathf.Abs(((Component)onVisionTriggeredPlayer).transform.position.y - ((Component)enemy).transform.position.y) > 4f))
		{
			playerTarget = onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { enemy.Vision.onVisionTriggeredID });
			}
			else
			{
				anim.NoticeSet(enemy.Vision.onVisionTriggeredID);
			}
			UpdateState(State.PlayerNotice);
			VerticalAimSet(100f);
		}
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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
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
		playerTarget = onGrabbedPlayerAvatar;
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
			else
			{
				anim.NoticeSet(playerTarget.photonView.ViewID);
			}
		}
		UpdateState(State.PlayerNotice);
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
		}
	}

	private void HorizontalRotationLogic()
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Roam || currentState == State.Investigate || currentState == State.Leave)
		{
			if (((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).magnitude > 0.05f)
			{
				Quaternion val = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
				val = (horizontalRotationTarget = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f));
			}
		}
		else if (currentState == State.PlayerNotice)
		{
			Quaternion val2 = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
			val2 = (horizontalRotationTarget = Quaternion.Euler(0f, ((Quaternion)(ref val2)).eulerAngles.y, 0f));
		}
		else if (currentState == State.Yell)
		{
			Quaternion val3 = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
			val3 = Quaternion.Euler(0f, ((Quaternion)(ref val3)).eulerAngles.y, 0f);
			horizontalRotationTarget = Quaternion.Slerp(horizontalRotationTarget, val3, 1f * Time.deltaTime);
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(horizontalRotationSpring, horizontalRotationTarget);
	}

	private void VerticalRotationLogic()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Yell)
		{
			VerticalAimSet(1f);
			verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(verticalRotationSpring, verticalRotationTarget);
		}
		else
		{
			verticalRotationTransform.localRotation = SemiFunc.SpringQuaternionGet(verticalRotationSpring, Quaternion.identity);
		}
	}

	private void VerticalAimSet(float _lerp)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		verticalRotationTransform.LookAt(((Component)playerTarget).transform);
		float num = 45f;
		float x = verticalRotationTransform.localEulerAngles.x;
		x = ((!(x < 180f)) ? Mathf.Clamp(x, 360f - num, 360f) : Mathf.Clamp(x, 0f, num));
		verticalRotationTransform.localRotation = Quaternion.Euler(x, 0f, 0f);
		Quaternion localRotation = verticalRotationTransform.localRotation;
		verticalRotationTransform.localRotation = Quaternion.identity;
		verticalRotationTarget = Quaternion.Lerp(verticalRotationTarget, localRotation, _lerp * Time.deltaTime);
	}

	private void HurtColliderLeaveLogic()
	{
		if (!enemy.Jump.jumping && currentState == State.Leave)
		{
			((Component)hurtColliderLeave).gameObject.SetActive(true);
		}
		else
		{
			((Component)hurtColliderLeave).gameObject.SetActive(false);
		}
	}

	private void SpringLogic()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		headTransform.rotation = SemiFunc.SpringQuaternionGet(headSpring, HeadTargetTransform.rotation);
		eyeRightTransform.rotation = SemiFunc.SpringQuaternionGet(eyeRightSpring, eyeRightTargetTransform.rotation);
		eyeLeftTransform.rotation = SemiFunc.SpringQuaternionGet(eyeLeftSpring, eyeLeftTargetTransform.rotation);
	}

	private void PlayerEyesLogic()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.PlayerNotice && currentState != State.Yell && currentState != State.YellEnd)
		{
			return;
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (Vector3.Distance(((Component)this).transform.position, ((Component)player).transform.position) < 8f)
			{
				SemiFunc.PlayerEyesOverride(player, enemy.Vision.VisionTransform.position, 0.1f, ((Component)this).gameObject);
			}
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
	}

	[PunRPC]
	private void NoticeRPC(int _playerID)
	{
		anim.NoticeSet(_playerID);
	}
}
