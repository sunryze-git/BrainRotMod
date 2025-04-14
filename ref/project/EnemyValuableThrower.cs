using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyValuableThrower : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		PlayerNotice,
		GetValuable,
		GoToTarget,
		PickUpTarget,
		TargetPlayer,
		Throw,
		Leave
	}

	private PhotonView photonView;

	public State currentState;

	private bool stateImpulse;

	public float stateTimer;

	private Vector3 agentDestination;

	private int attacks;

	[Space]
	public EnemyValuableThrowerAnim anim;

	public Transform pickupTargetParent;

	public Transform pickupTarget;

	private Enemy enemy;

	private PlayerAvatar playerTarget;

	private PhysGrabObject valuableTarget;

	private Vector3 pickUpPosition;

	private float grabAggroTimer;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		enemy = ((Component)this).GetComponent<Enemy>();
	}

	private void Update()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.GetValuable || currentState == State.GoToTarget || currentState == State.PickUpTarget || currentState == State.TargetPlayer || currentState == State.Throw)
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(((Component)this).transform.position, ((Component)player).transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(player, enemy.Vision.VisionTransform.position, 0.1f, ((Component)this).gameObject);
				}
			}
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (grabAggroTimer > 0f)
		{
			grabAggroTimer -= Time.deltaTime;
		}
		if (!enemy.IsStunned() && LevelGenerator.Instance.Generated)
		{
			switch (currentState)
			{
			case State.Spawn:
				StateSpawn();
				break;
			case State.Idle:
				StateIdle();
				AgentVelocityRotation();
				break;
			case State.Roam:
				StateRoam();
				AgentVelocityRotation();
				break;
			case State.Investigate:
				StateInvestigate();
				AgentVelocityRotation();
				break;
			case State.PlayerNotice:
				StatePlayerNotice();
				PlayerLookAt();
				break;
			case State.GetValuable:
				StateGetValuable();
				break;
			case State.GoToTarget:
				ValuableFailsafe();
				TargetFailsafe();
				AgentVelocityRotation();
				StateGoToTarget();
				break;
			case State.PickUpTarget:
				DropOnStun();
				TargetFailsafe();
				ValuableTargetFollow();
				StatePickUpTarget();
				break;
			case State.TargetPlayer:
				DropOnStun();
				TargetFailsafe();
				PlayerLookAt();
				ValuableTargetFollow();
				StateTargetPlayer();
				break;
			case State.Throw:
				DropOnStun();
				TargetFailsafe();
				PlayerLookAt();
				ValuableTargetFollow();
				StateThrow();
				break;
			case State.Leave:
				AgentVelocityRotation();
				StateLeave();
				break;
			}
			pickupTargetParent.position = ((Component)enemy.Rigidbody).transform.position;
			Quaternion rotation = ((Component)enemy.Rigidbody).transform.rotation;
			Quaternion val = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
			pickupTargetParent.rotation = Quaternion.Slerp(pickupTargetParent.rotation, val, 5f * Time.deltaTime);
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
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
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
			if ((stateTimer <= 0f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, agentDestination) < 2f) && !enemy.Jump.jumping)
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
			UpdateState(State.GetValuable);
		}
	}

	private void StateGetValuable()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.GetValuable)
		{
			return;
		}
		valuableTarget = null;
		PhysGrabObject physGrabObject = null;
		PhysGrabObject physGrabObject2 = null;
		float num = 999f;
		float num2 = 999f;
		Collider[] array = Physics.OverlapSphere(((Component)playerTarget).transform.position, 10f, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
		NavMeshHit val = default(NavMeshHit);
		for (int i = 0; i < array.Length; i++)
		{
			ValuableObject componentInParent = ((Component)array[i]).GetComponentInParent<ValuableObject>();
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.volumeType > ValuableVolume.Type.Big)
			{
				continue;
			}
			float num3 = Vector3.Distance(((Component)playerTarget).transform.position, ((Component)componentInParent).transform.position);
			if (NavMesh.SamplePosition(((Component)componentInParent).transform.position, ref val, 1f, -1))
			{
				if (num3 < num2)
				{
					num2 = num3;
					physGrabObject2 = componentInParent.physGrabObject;
				}
			}
			else if (num3 < num)
			{
				num = num3;
				physGrabObject = componentInParent.physGrabObject;
			}
		}
		if (Object.op_Implicit((Object)(object)physGrabObject2))
		{
			valuableTarget = physGrabObject2;
		}
		else if (Object.op_Implicit((Object)(object)physGrabObject))
		{
			valuableTarget = physGrabObject;
		}
		if (!Object.op_Implicit((Object)(object)valuableTarget))
		{
			UpdateState(State.Leave);
		}
		else
		{
			UpdateState(State.GoToTarget);
		}
	}

	private void StateGoToTarget()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned() || !Object.op_Implicit((Object)(object)valuableTarget))
		{
			return;
		}
		enemy.NavMeshAgent.SetDestination(((Component)valuableTarget).transform.position);
		if (stateImpulse)
		{
			stateTimer = 5f;
			stateImpulse = false;
			return;
		}
		SemiFunc.EnemyCartJump(enemy);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, ((Component)valuableTarget).transform.position) < 1.25f)
		{
			enemy.NavMeshAgent.ResetPath();
			SemiFunc.EnemyCartJumpReset(enemy);
			UpdateState(State.PickUpTarget);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetDestination()) < 1f)
		{
			if (stateTimer <= 0f)
			{
				enemy.Jump.StuckReset();
				UpdateState(State.Leave);
			}
			else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, valuableTarget.centerPoint) > 1.5f)
			{
				enemy.Jump.StuckTrigger(((Component)valuableTarget).transform.position - ((Component)enemy.Rigidbody).transform.position);
				enemy.Rigidbody.DisableFollowPosition(1f, 10f);
			}
		}
		if (enemy.Rigidbody.notMovingTimer > 2f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Leave);
			}
		}
	}

	private void StatePickUpTarget()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.PickUpTarget)
		{
			return;
		}
		if (stateImpulse)
		{
			foreach (PhysGrabber item in valuableTarget.playerGrabbing.ToList())
			{
				if (!SemiFunc.IsMultiplayer())
				{
					item.ReleaseObject();
					continue;
				}
				item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 0.1f });
			}
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			Quaternion val = Quaternion.LookRotation(pickUpPosition - ((Component)enemy.Rigidbody).transform.position);
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			((Component)this).transform.rotation = Quaternion.RotateTowards(((Component)this).transform.rotation, val, 180f * Time.deltaTime);
			pickUpPosition = valuableTarget.midPoint;
			stateTimer = 999f;
			stateImpulse = false;
		}
		if (stateTimer <= 0f)
		{
			UpdateState(State.TargetPlayer);
		}
	}

	private void StateTargetPlayer()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.TargetPlayer)
		{
			return;
		}
		if (stateImpulse)
		{
			stateTimer = 10f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		Vector3 val = playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position;
		RaycastHit val2 = default(RaycastHit);
		bool flag = Physics.Raycast(((Component)enemy.Rigidbody).transform.position, val, ref val2, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
		if (flag && (((Component)((RaycastHit)(ref val2)).transform).CompareTag("Player") || Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val2)).transform).GetComponent<PlayerTumble>())))
		{
			flag = false;
		}
		if (!flag && Vector3.Distance(((Component)this).transform.position, ((Component)playerTarget).transform.position) < 3f)
		{
			enemy.NavMeshAgent.SetDestination(((Component)this).transform.position - ((Component)this).transform.forward * 3f);
		}
		else if (flag || Vector3.Distance(((Component)this).transform.position, ((Component)playerTarget).transform.position) > 5f)
		{
			enemy.NavMeshAgent.SetDestination(((Component)playerTarget).transform.position);
		}
		else
		{
			enemy.NavMeshAgent.ResetPath();
			if (stateTimer <= 8f)
			{
				UpdateState(State.Throw);
			}
		}
		if (stateTimer <= 0f)
		{
			UpdateState(State.Throw);
		}
	}

	private void StateThrow()
	{
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			attacks++;
			stateTimer = 3f;
			stateImpulse = false;
		}
		if (!Object.op_Implicit((Object)(object)valuableTarget))
		{
			stateTimer = Mathf.Clamp(stateTimer, stateTimer, 1f);
		}
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
				UpdateState(State.GetValuable);
			}
		}
	}

	private void StateLeave()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
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
			stateTimer = 10f;
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
		if (((Behaviour)anim).isActiveAndEnabled)
		{
			anim.OnSpawn();
		}
	}

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		anim.hurtSound.Play(((Component)anim).transform.position);
	}

	public void OnDeath()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		anim.particleImpact.Play();
		anim.particleBits.Play();
		((Component)anim.particleDirectionalBits).transform.rotation = Quaternion.LookRotation(-((Vector3)(ref enemy.Health.hurtDirection)).normalized);
		anim.particleDirectionalBits.Play();
		anim.deathSound.Play(((Component)anim).transform.position);
		enemy.EnemyParent.Despawn();
	}

	public void OnVisionTriggered()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || (currentState != State.Idle && currentState != State.Roam && currentState != State.Investigate))
		{
			return;
		}
		if ((Object)(object)playerTarget != (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			playerTarget = enemy.Vision.onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)1, new object[1] { playerTarget.photonView.ViewID });
			}
		}
		if (!enemy.IsStunned())
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("NoticeRPC", (RpcTarget)0, new object[1] { enemy.Vision.onVisionTriggeredID });
			}
			else
			{
				anim.NoticeSet(enemy.Vision.onVisionTriggeredID);
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
		if ((Object)(object)playerTarget != (Object)(object)enemy.Rigidbody.onGrabbedPlayerAvatar)
		{
			playerTarget = enemy.Rigidbody.onGrabbedPlayerAvatar;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)1, new object[1] { playerTarget.photonView.ViewID });
			}
		}
		UpdateState(State.PlayerNotice);
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
	}

	private void UpdateState(State _state)
	{
		currentState = _state;
		stateImpulse = true;
		stateTimer = 0f;
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateStateRPC", (RpcTarget)0, new object[1] { currentState });
		}
	}

	private void ValuableTargetFollow()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)valuableTarget))
		{
			return;
		}
		if (Vector3.Distance(((Component)valuableTarget).transform.position, pickupTarget.position) > 2f)
		{
			valuableTarget = null;
			UpdateState(State.Leave);
			return;
		}
		Vector3 midPoint = valuableTarget.midPoint;
		midPoint.y = ((Component)valuableTarget).transform.position.y;
		Vector3 position = pickupTarget.position;
		valuableTarget.OverrideZeroGravity();
		valuableTarget.OverrideMass(0.5f);
		valuableTarget.OverrideIndestructible();
		valuableTarget.OverrideBreakEffects(0.1f);
		if (Mathf.Abs(midPoint.y - position.y) > 0.25f)
		{
			Vector3 val = ((Component)enemy.Rigidbody).transform.position + ((Component)enemy.Rigidbody).transform.forward;
			((Vector3)(ref position))._002Ector(val.x, position.y, val.z);
		}
		Vector3 val2 = SemiFunc.PhysFollowPosition(midPoint, position, valuableTarget.rb.velocity, 5f);
		valuableTarget.rb.AddForce(val2 * (5f * Time.fixedDeltaTime), (ForceMode)1);
		Vector3 val3 = SemiFunc.PhysFollowRotation(((Component)valuableTarget).transform, pickupTarget.rotation, valuableTarget.rb, 0.5f);
		valuableTarget.rb.AddTorque(val3 * (5f * Time.fixedDeltaTime), (ForceMode)1);
	}

	private void AgentVelocityRotation()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).magnitude > 0.05f)
		{
			Quaternion val = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, 5f * Time.deltaTime);
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

	private void ValuableFailsafe()
	{
		if (!Object.op_Implicit((Object)(object)valuableTarget))
		{
			UpdateState(State.GetValuable);
		}
	}

	private void TargetFailsafe()
	{
		if (!Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
		{
			UpdateState(State.Leave);
		}
	}

	private void DropOnStun()
	{
		if (enemy.IsStunned())
		{
			UpdateState(State.GoToTarget);
		}
	}

	public void ResetStateTimer()
	{
		stateTimer = 0f;
	}

	public void Throw()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)valuableTarget) || !Object.op_Implicit((Object)(object)playerTarget))
		{
			return;
		}
		foreach (PhysGrabber item in valuableTarget.playerGrabbing.ToList())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				item.ReleaseObject();
				continue;
			}
			item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 0.1f });
		}
		Vector3 val = playerTarget.PlayerVisionTarget.VisionTransform.position - valuableTarget.centerPoint;
		val = Vector3.Lerp(((Component)this).transform.forward, val, 0.5f);
		valuableTarget.ResetMass();
		float num = 20f * valuableTarget.rb.mass;
		num = Mathf.Min(num, 100f);
		valuableTarget.ResetIndestructible();
		valuableTarget.rb.AddForce(val * num, (ForceMode)1);
		valuableTarget.rb.AddTorque(((Component)valuableTarget).transform.right * 0.5f, (ForceMode)1);
		valuableTarget.impactDetector.PlayerHurtMultiplier(5f, 2f);
		valuableTarget = null;
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
