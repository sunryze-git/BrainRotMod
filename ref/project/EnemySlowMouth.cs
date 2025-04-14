using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemySlowMouth : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Despawn,
		Roam,
		Investigate,
		Stun,
		Leave,
		Notice,
		Attack,
		Attached,
		Puke,
		Detach,
		IdlePuke,
		GoToPlayerOver,
		GoToPlayerUnder,
		MoveBackToNavmesh,
		GoToPlayer,
		Dead
	}

	public GameObject enemySlowMouthAttack;

	public GameObject localCameraMouthPrefab;

	public GameObject enemySlowMouthOnPlayerTop;

	public GameObject enemySlowMouthOnPlayerBot;

	public Transform particles;

	public Transform tentacles;

	private List<SpringTentacle> springTentacles = new List<SpringTentacle>();

	private List<ParticleSystem> spawnParticles = new List<ParticleSystem>();

	private EnemySlowMouthCameraVisuals cameraVisuals;

	private bool movingRight;

	private bool movingLeft = true;

	private float moveThisDirectionTimer;

	private float looseTargetTimer;

	private float looseTargetTime;

	private float randomNudgeTimer;

	[FormerlySerializedAs("state")]
	public State currentState;

	private Vector3 agentDestination;

	public SemiPuke semiPuke;

	private EnemyVision enemyVision;

	public Transform mouthTransform;

	public Collider enemyCollider;

	public Transform enemyVisuals;

	private bool stateImpulse;

	private bool stateStartFixed;

	private float stateTimer;

	private PhotonView photonView;

	private PlayerAvatar attachTarget;

	private Enemy enemy;

	public EnemyRigidbody enemyRigidbody;

	public PhysGrabObject physGrabObject;

	public Transform followTarget;

	public Vector3 followTargetStartPosition;

	public Transform centerTransform;

	public AudioSource audioSourceVO;

	private float idleBreakerVOCooldown = 20f;

	private float idlePukeCooldown = 20f;

	private State idlePukePreviousState;

	public EnemySlowMouthAnim enemySlowMouthAnim;

	private Vector3 followPointPositionPrev;

	private Transform currentTarget;

	private SpringFloat spawnDespawnScaleSpring;

	private Vector3 targetDestination;

	private bool waitForTargettingLoop;

	private float visionTimer;

	private bool visionPrevious;

	private PlayerAvatar playerTarget;

	private float enemyHiddenTimer;

	private Vector3 moveBackPosition;

	private float targetForwardOffset = 1.5f;

	private Vector3 targetPosition;

	private float targetedPlayerTime;

	private float targetedPlayerTimeMax = 10f;

	private float moveBackTimer;

	private Vector3 enemyGroundPosition;

	internal Vector3 detachPosition;

	internal Quaternion detachRotation;

	private float attachedTimer;

	private float possessCooldown;

	private float stuckTimer;

	private Vector3 stuckPosition;

	private float aggroTimer;

	public Sound soundSpawnVO;

	public Sound soundIdleBreakerVO;

	public Sound soundHurtVO;

	public Sound soundDieVO;

	public Sound soundNoticeVO;

	public Sound soundChaseLoopVO;

	public Sound soundDetachVO;

	public Sound soundDetach;

	public Sound soundStunLoopVO;

	private void Start()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		spawnDespawnScaleSpring = new SpringFloat();
		spawnDespawnScaleSpring.damping = 0.5f;
		spawnDespawnScaleSpring.speed = 20f;
		photonView = ((Component)this).GetComponent<PhotonView>();
		enemy = ((Component)this).GetComponent<Enemy>();
		followTargetStartPosition = followTarget.localPosition;
		enemyVision = ((Component)this).GetComponent<EnemyVision>();
		spawnParticles = new List<ParticleSystem>(((Component)particles).GetComponentsInChildren<ParticleSystem>());
		springTentacles = new List<SpringTentacle>(((Component)tentacles).GetComponentsInChildren<SpringTentacle>());
	}

	private void AnimStateIdle()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Idle);
	}

	private void AnimStatePuke()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Puke);
	}

	private void AnimStateStunned()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Stunned);
	}

	private void AnimStateTargetting()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Targetting);
	}

	private void AnimStateAttached()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Attached);
	}

	private void AnimStateAggro()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Aggro);
	}

	private void AnimStateSpawnDespawn()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.SpawnDespawn);
	}

	private void AnimStateDeath()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Death);
	}

	private void AnimStateLeave()
	{
		enemySlowMouthAnim.UpdateState(EnemySlowMouthAnim.State.Leave);
	}

	private void PlaySpawnParticles()
	{
		foreach (ParticleSystem spawnParticle in spawnParticles)
		{
			spawnParticle.Play();
		}
	}

	private void StateSpawn(bool fixedUpdate)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				enemyVisuals.localScale = Vector3.zero;
				stateTimer = 3f;
				stateImpulse = false;
				PlaySpawnParticles();
			}
			float num = SemiFunc.SpringFloatGet(spawnDespawnScaleSpring, 1f);
			enemyVisuals.localScale = Vector3.one * num;
			AnimStateSpawnDespawn();
			if (stateTimer <= 0f)
			{
				enemyVisuals.localScale = Vector3.one;
				UpdateState(State.Idle);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateIdle(bool fixedUpdate)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
				enemy.NavMeshAgent.ResetPath();
				stateTimer = Random.Range(5f, 10f);
			}
			AnimStateIdle();
			if (SemiFunc.IsMasterClientOrSingleplayer() && !IdlePukeLogic(0.1f))
			{
				IdleBreakerVOLogic();
				LookAtVelocityDirection(_moving: false);
				FloatAround();
				if (stateTimer <= 0f)
				{
					UpdateState(State.Roam);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateDespawn(bool fixedUpdate)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				PlaySpawnParticles();
				soundDetach.Play(centerTransform.position);
			}
			float num = SemiFunc.SpringFloatGet(spawnDespawnScaleSpring, 0f);
			enemyVisuals.localScale = Vector3.one * num;
			AnimStateSpawnDespawn();
			if (SemiFunc.IsMasterClientOrSingleplayer() && stateTimer <= 0f)
			{
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
				enemy.NavMeshAgent.ResetPath();
				enemy.EnemyParent.Despawn();
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateRoam(bool fixedUpdate)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				agentDestination = SemiFunc.EnemyRoamFindPoint(((Component)this).transform.position);
				stateTimer = Random.Range(5f, 10f);
				followTarget.localPosition = new Vector3(0f, 1f, 0f);
			}
			AnimStateIdle();
			if (SemiFunc.IsMasterClientOrSingleplayer() && !IdlePukeLogic(0.1f))
			{
				IdleBreakerVOLogic();
				LookAtVelocityDirection(_moving: false);
				StuckLogic();
				enemy.NavMeshAgent.SetDestination(agentDestination);
				if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f || stateTimer <= 0f)
				{
					UpdateState(State.Idle);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateInvestigate(bool fixedUpdate)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				followTarget.localPosition = new Vector3(0f, 1f, 0f);
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			}
			AnimStateIdle();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			LookAtVelocityDirection(_moving: false);
			if (!IdlePukeLogic(0.2f))
			{
				IdleBreakerVOLogic();
				enemy.NavMeshAgent.SetDestination(agentDestination);
				if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
				{
					UpdateState(State.Idle);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateStun(bool fixedUpdate)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
				enemy.NavMeshAgent.ResetPath();
				stateImpulse = false;
			}
			AnimStateStunned();
			foreach (SpringTentacle springTentacle in springTentacles)
			{
				if (SemiFunc.FPSImpulse5())
				{
					springTentacle.springStart.springVelocity = Random.insideUnitSphere * 25f;
					springTentacle.springMid.springVelocity = Random.insideUnitSphere * 25f;
					springTentacle.springEnd.springVelocity = Random.insideUnitSphere * 25f;
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer() && !enemy.IsStunned())
			{
				UpdateState(State.Idle);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateLeave(bool fixedUpdate)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				Vector3 val = SemiFunc.EnemyLeaveFindPoint(((Component)this).transform.position);
				agentDestination = val;
				followTarget.localPosition = new Vector3(0f, 1f, 0f);
				stateTimer = Random.Range(10f, 15f);
				PlaySpawnParticles();
			}
			enemy.NavMeshAgent.SetDestination(agentDestination);
			AnimStateLeave();
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				StuckLogic();
				IdleBreakerVOLogic();
				FastMoving(_lookAtTarget: false);
				enemyRigidbody.OverrideFollowPosition(0.1f, 5f, 10f);
				enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
				if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
				{
					UpdateState(State.Idle);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateNotice(bool fixedUpdate)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				TargettingPlayerStart();
				if (!audioSourceVO.isPlaying)
				{
					soundNoticeVO.Play(centerTransform.position);
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					UpdatePlayerTarget(enemyVision.onVisionTriggeredPlayer);
				}
				stateTimer = 1f;
				stateImpulse = false;
			}
			AnimStateIdle();
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				followTarget.localRotation = Quaternion.LookRotation(currentTarget.position - centerTransform.position, Vector3.up);
				if (stateTimer <= 0f)
				{
					UpdateState(State.GoToPlayer);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateAttack(bool fixedUpdate)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				EnemySlowMouthAttaching component = Object.Instantiate<GameObject>(enemySlowMouthAttack, centerTransform.position, centerTransform.rotation).GetComponent<EnemySlowMouthAttaching>();
				component.targetPlayerAvatar = playerTarget;
				component.enemySlowMouth = this;
				attachedTimer = 0f;
			}
			AnimStateAttached();
			OverrideHideEnemy();
			LookAtVelocityDirection(_moving: false);
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void DetatchLogic()
	{
		if (SemiFunc.FPSImpulse5())
		{
			if (!Object.op_Implicit((Object)(object)playerTarget))
			{
				UpdateState(State.Detach);
			}
			else if (playerTarget.isDisabled)
			{
				UpdateState(State.Detach);
			}
		}
	}

	private void StateAttached(bool fixedUpdate)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				if (attachedTimer <= 0f)
				{
					attachedTimer = Random.Range(20f, 60f);
				}
				stateTimer = Random.Range(1f, 15f);
			}
			OverrideHideEnemy();
			AnimStateAttached();
			PlayerEffects();
			enemy.EnemyParent.SpawnedTimerPause(1f);
			((Component)enemy).transform.position = ((Component)this).transform.position;
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			DetatchLogic();
			if (SemiFunc.FPSImpulse5())
			{
				bool num = Object.op_Implicit((Object)(object)((Component)playerTarget.playerAvatarVisuals).GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>());
				bool flag = Object.op_Implicit((Object)(object)((Component)playerTarget.localCameraTransform).GetComponentInChildren<EnemySlowMouthCameraVisuals>());
				if (!num && !flag)
				{
					if (Object.op_Implicit((Object)(object)currentTarget))
					{
						detachPosition = currentTarget.position;
						detachRotation = currentTarget.rotation;
					}
					UpdateState(State.Detach);
				}
			}
			IsPossessedBySeveral();
			attachedTimer -= Time.deltaTime;
			if (attachedTimer <= 0f || playerTarget.isDisabled)
			{
				if (Object.op_Implicit((Object)(object)currentTarget))
				{
					detachPosition = currentTarget.position;
					detachRotation = currentTarget.rotation;
					UpdateState(State.Detach);
				}
				else
				{
					enemy.EnemyParent.SpawnedTimerPause(0f);
					UpdateState(State.Despawn);
				}
			}
			else if (stateTimer <= 0f)
			{
				UpdateState(State.Puke);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StatePuke(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				stateTimer = Random.Range(0.5f, 3f);
				if (Random.Range(0, 30) == 0)
				{
					stateTimer = 6f;
				}
			}
			PlayerEffects();
			OverrideHideEnemy();
			AnimStateAttached();
			DetatchLogic();
			if (stateTimer <= 0f)
			{
				UpdateState(State.Attached);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateDetach(bool fixedUpdate)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				stateTimer = 0f;
				attachedTimer = 0f;
				possessCooldown = Random.Range(30f, 120f);
				((Component)enemy).transform.position = ((Component)this).transform.position;
			}
			PlayerEffects();
			OverrideHideEnemy();
			AnimStateAttached();
			if (!(stateTimer <= 0f))
			{
				return;
			}
			if (!Object.op_Implicit((Object)(object)playerTarget))
			{
				enemy.EnemyParent.SpawnedTimerPause(0f);
				UpdateState(State.Despawn);
				return;
			}
			Vector3 forward = playerTarget.localCameraTransform.forward;
			Vector3 val = playerTarget.localCameraTransform.position + forward * 0.3f;
			float num = 0.2f;
			if (Physics.SphereCastAll(val, 0.45f, forward, num, LayerMask.GetMask(new string[1] { "Default" })).Length == 0)
			{
				Vector3 position = playerTarget.localCameraTransform.position + forward * num;
				if (Object.op_Implicit((Object)(object)playerTarget) && Object.op_Implicit((Object)(object)playerTarget.tumble))
				{
					playerTarget.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
				}
				physGrabObject.Teleport(position, playerTarget.localCameraTransform.rotation);
				enemy.NavMeshAgent.Warp(position);
				soundDetach.Play(position);
				soundDetachVO.Play(position);
				enemy.EnemyParent.SpawnedTimerPause(2f);
				UpdateState(State.Leave);
			}
			else if (playerTarget.isDisabled)
			{
				enemy.EnemyParent.SpawnedTimerPause(0f);
				UpdateState(State.Despawn);
			}
			else
			{
				stateTimer = 0.25f;
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateIdlePuke(bool fixedUpdate)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				stateTimer = Random.Range(0.5f, 2f);
				if (Random.Range(0, 30) == 0)
				{
					stateTimer = 4f;
				}
			}
			AnimStatePuke();
			semiPuke.PukeActive(mouthTransform.position, mouthTransform.rotation);
			if (stateTimer <= 0f)
			{
				UpdateState(idlePukePreviousState);
			}
		}
		else
		{
			if (stateStartFixed)
			{
				stateStartFixed = false;
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Vector3 val = Random.insideUnitSphere * 80f;
				enemyRigidbody.rb.AddTorque(val * Time.fixedDeltaTime, (ForceMode)0);
				Vector3 val2 = -mouthTransform.forward * 400f;
				enemyRigidbody.rb.AddForce(val2 * Time.fixedDeltaTime, (ForceMode)0);
			}
		}
	}

	private void StateGoToPlayerOver(bool fixedUpdate)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateTimer = 2f;
				stateImpulse = false;
				followTarget.localPosition = Vector3.zero;
			}
			followTarget.localPosition = currentTarget.localPosition;
			AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			FastMoving(_lookAtTarget: true);
			TargettingPlayer();
			if (IdlePukeLogic())
			{
				return;
			}
			AttachToPlayer();
			enemy.NavMeshAgent.Disable(0.1f);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, enemy.NavMeshAgent.DefaultSpeed * 0.5f * Time.deltaTime);
			enemy.Vision.StandOverride(0.25f);
			if (playerTarget.PlayerVisionTarget.VisionTransform.position.y > ((Component)enemy.Rigidbody).transform.position.y + 1.5f)
			{
				((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
				((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, 2f);
			}
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(targetPosition, ref val, 0.5f, -1))
			{
				UpdateState(State.MoveBackToNavmesh);
			}
			else if (VisionBlocked() || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
			{
				if (stateTimer <= 0f || enemy.Rigidbody.notMovingTimer > 1f)
				{
					UpdateState(State.MoveBackToNavmesh);
				}
			}
			else
			{
				stateTimer = 2f;
			}
			if (SemiFunc.EnemyForceLeave(enemy))
			{
				UpdateState(State.MoveBackToNavmesh);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateGoToPlayerUnder(bool fixedUpdate)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateTimer = 2f;
				stateImpulse = false;
				followTarget.localPosition = Vector3.zero;
			}
			AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			FastMoving(_lookAtTarget: true);
			TargettingPlayer();
			AttachToPlayer();
			if (IdlePukeLogic())
			{
				return;
			}
			followTarget.localPosition = new Vector3(0f, 0.2f, 0f);
			enemy.NavMeshAgent.Disable(0.1f);
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetPosition, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
			enemy.Vision.StandOverride(0.25f);
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(targetPosition, ref val, 0.5f, -1))
			{
				UpdateState(State.MoveBackToNavmesh);
			}
			else if (VisionBlocked() || !Object.op_Implicit((Object)(object)playerTarget) || playerTarget.isDisabled)
			{
				if (stateTimer <= 0f)
				{
					UpdateState(State.MoveBackToNavmesh);
				}
			}
			else
			{
				stateTimer = 2f;
			}
			if (SemiFunc.EnemyForceLeave(enemy))
			{
				UpdateState(State.MoveBackToNavmesh);
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateMoveBackToNavmesh(bool fixedUpdate)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
				stateTimer = 30f;
				followTarget.localPosition = Vector3.zero;
			}
			AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			FastMoving(_lookAtTarget: false);
			TargettingPlayer();
			enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
			if (!IdlePukeLogic())
			{
				enemy.NavMeshAgent.Disable(0.1f);
				((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
				enemy.Vision.StandOverride(0.25f);
				if (Vector3.Distance(((Component)this).transform.position, enemyGroundPosition) > 2f || enemy.Rigidbody.notMovingTimer > 2f)
				{
					Vector3 val = moveBackPosition - enemyGroundPosition;
					Vector3 normalized = ((Vector3)(ref val)).normalized;
					((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
					Transform transform = ((Component)this).transform;
					transform.position += normalized * 2f;
				}
				NavMeshHit val2 = default(NavMeshHit);
				if (Vector3.Distance(enemyGroundPosition, moveBackPosition) <= 0f || NavMesh.SamplePosition(enemyGroundPosition, ref val2, 0.5f, -1))
				{
					UpdateState(State.GoToPlayer);
				}
				else if (stateTimer <= 0f)
				{
					enemy.EnemyParent.SpawnedTimerSet(0f);
				}
			}
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateGoToPlayer(bool fixedUpdate)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				followTarget.localPosition = Vector3.zero;
				stateImpulse = false;
				stateTimer = 5f;
				targetedPlayerTime = 0f;
				targetedPlayerTimeMax = Random.Range(8f, 22f);
			}
			AnimStateTargetting();
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			FastMoving(_lookAtTarget: true);
			TargettingPlayer();
			enemy.NavMeshAgent.OverrideAgent(8f, 8f, 0.1f);
			if (IdlePukeLogic())
			{
				return;
			}
			AttachToPlayer();
			enemy.NavMeshAgent.SetDestination(targetPosition);
			MoveBackPosition();
			enemy.Vision.StandOverride(0.25f);
			NavMeshHit val = default(NavMeshHit);
			if (!enemy.NavMeshAgent.CanReach(targetPosition, 1f) && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f && !VisionBlocked() && !NavMesh.SamplePosition(targetPosition, ref val, 0.5f, -1))
			{
				if (playerTarget.isCrawling && Mathf.Abs(targetPosition.y - ((Component)enemy.Rigidbody).transform.position.y) < 0.3f)
				{
					UpdateState(State.GoToPlayerUnder);
					return;
				}
				if (targetPosition.y > ((Component)enemy.Rigidbody).transform.position.y)
				{
					UpdateState(State.GoToPlayerOver);
					return;
				}
			}
			LeaveCheck();
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateDead(bool fixedUpdate)
	{
		if (!fixedUpdate)
		{
			if (stateImpulse)
			{
				stateImpulse = false;
			}
			OverrideHideEnemy();
			AnimStateDeath();
		}
		else if (stateStartFixed)
		{
			stateStartFixed = false;
		}
	}

	private void StateMachine(bool fixedUpdate)
	{
		if (!fixedUpdate && stateTimer > 0f)
		{
			stateTimer -= Time.deltaTime;
		}
		if (fixedUpdate)
		{
			if (enemyHiddenTimer <= 0f && !enemyCollider.enabled)
			{
				PlaySpawnParticles();
				((Component)enemySlowMouthAnim).gameObject.SetActive(true);
				enemyCollider.enabled = true;
			}
			if (enemyHiddenTimer > 0f)
			{
				enemyHiddenTimer -= Time.fixedDeltaTime;
				if (enemyCollider.enabled)
				{
					PlaySpawnParticles();
					((Component)enemySlowMouthAnim).gameObject.SetActive(false);
					enemyCollider.enabled = false;
				}
			}
		}
		switch (currentState)
		{
		case State.Spawn:
			StateSpawn(fixedUpdate);
			break;
		case State.Idle:
			StateIdle(fixedUpdate);
			break;
		case State.Despawn:
			StateDespawn(fixedUpdate);
			break;
		case State.Roam:
			StateRoam(fixedUpdate);
			break;
		case State.Investigate:
			StateInvestigate(fixedUpdate);
			break;
		case State.Stun:
			StateStun(fixedUpdate);
			break;
		case State.Leave:
			StateLeave(fixedUpdate);
			break;
		case State.Notice:
			StateNotice(fixedUpdate);
			break;
		case State.Attack:
			StateAttack(fixedUpdate);
			break;
		case State.Attached:
			StateAttached(fixedUpdate);
			break;
		case State.Puke:
			StatePuke(fixedUpdate);
			break;
		case State.Detach:
			StateDetach(fixedUpdate);
			break;
		case State.IdlePuke:
			StateIdlePuke(fixedUpdate);
			break;
		case State.GoToPlayerOver:
			StateGoToPlayerOver(fixedUpdate);
			break;
		case State.GoToPlayerUnder:
			StateGoToPlayerUnder(fixedUpdate);
			break;
		case State.MoveBackToNavmesh:
			StateMoveBackToNavmesh(fixedUpdate);
			break;
		case State.GoToPlayer:
			StateGoToPlayer(fixedUpdate);
			break;
		}
	}

	public void UpdateState(State newState)
	{
		if (currentState != newState && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateStateRPC", (RpcTarget)0, new object[1] { newState });
			}
			else
			{
				UpdateStateRPC(newState);
			}
		}
	}

	private void UpdatePlayerTarget(PlayerAvatar _player)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { _player.photonView.ViewID });
			}
			else
			{
				UpdatePlayerTargetRPC(_player.photonView.ViewID);
			}
		}
	}

	[PunRPC]
	private void UpdatePlayerTargetRPC(int _photonViewID)
	{
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			if (item.photonView.ViewID == _photonViewID)
			{
				playerTarget = item;
				currentTarget = SemiFunc.PlayerGetFaceEyeTransform(item);
				break;
			}
		}
	}

	[PunRPC]
	public void UpdateStateRPC(State newState)
	{
		currentState = newState;
		stateImpulse = true;
		stateStartFixed = true;
		stateTimer = 0f;
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundHurtVO.Play(centerTransform.position);
	}

	public void OnVision()
	{
		if (currentState == State.Idle || currentState == State.Investigate || currentState == State.Roam)
		{
			UpdateState(State.Notice);
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

	public void OnDeath()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		soundDieVO.Play(centerTransform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.05f);
		PlaySpawnParticles();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
			UpdateState(State.Dead);
		}
	}

	public void OnDespawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		soundDieVO.Play(centerTransform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.05f);
		PlaySpawnParticles();
	}

	private void Update()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.Stun)
		{
			physGrabObject.OverrideZeroGravity();
		}
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			UpdateState(State.Despawn);
		}
		LoopSounds();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (possessCooldown > 0f)
			{
				possessCooldown -= Time.deltaTime;
			}
			if (enemy.IsStunned() && enemyHiddenTimer <= 0f)
			{
				UpdateState(State.Stun);
			}
			enemyGroundPosition = new Vector3(((Component)enemy.Rigidbody).transform.position.x, ((Component)this).transform.position.y, ((Component)enemy.Rigidbody).transform.position.z);
			TargetPositionLogic();
		}
		StateMachine(fixedUpdate: false);
		if (SemiFunc.FPSImpulse30())
		{
			followPointPositionPrev = followTarget.position;
		}
	}

	private void FixedUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Vector3 forward = ((Component)enemy.Rigidbody).transform.forward;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(centerTransform.position, forward, ref val, 1f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())) && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<PhysGrabHinge>()))
			{
				enemyRigidbody.rb.AddForce(-(forward * 600f) * Time.fixedDeltaTime, (ForceMode)0);
			}
		}
		StateMachine(fixedUpdate: true);
	}

	private void RandomNudge(float _force)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			soundIdleBreakerVO.Play(centerTransform.position);
			Vector3 val = centerTransform.up;
			switch (Random.Range(0, 4))
			{
			case 0:
				val = centerTransform.up;
				break;
			case 1:
				val = centerTransform.right;
				break;
			case 2:
				val = -centerTransform.right;
				break;
			case 3:
				val = -centerTransform.up;
				break;
			}
			_ = centerTransform.position + val * 0.5f;
			randomNudgeTimer = 0f;
			enemyRigidbody.rb.AddForce(val * _force, (ForceMode)1);
		}
	}

	private void LookAtVelocityDirection(bool _moving)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Vector3 velocity = enemyRigidbody.rb.velocity;
			Vector3 val = ((Vector3)(ref velocity)).normalized;
			if (_moving)
			{
				val = enemy.moveDirection;
			}
			if (((Vector3)(ref val)).magnitude > 0.001f)
			{
				Quaternion val2 = Quaternion.LookRotation(val, Vector3.up);
				followTarget.localRotation = Quaternion.Slerp(followTarget.localRotation, val2, Time.deltaTime * 2f);
			}
		}
	}

	private void FloatAround()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		followTarget.localPosition = followTargetStartPosition + Vector3.up * Mathf.Sin(Time.time * 0.5f) * 0.5f;
		Transform obj = followTarget;
		obj.localPosition += Vector3.left * Mathf.Sin(Time.time * 0.2f) * 0.3f;
		Transform obj2 = followTarget;
		obj2.localPosition += Vector3.forward * Mathf.Sin(Time.time * 0.2f) * 2f;
	}

	private bool IdlePukeLogic(float _tickSpeed = 1f)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return false;
		}
		if (idlePukeCooldown > 0f)
		{
			idlePukeCooldown -= Time.deltaTime * _tickSpeed;
			return false;
		}
		IdlePukeExecute();
		return true;
	}

	private void IdlePukeExecute()
	{
		if (!audioSourceVO.isPlaying)
		{
			idlePukePreviousState = currentState;
			UpdateState(State.IdlePuke);
			idlePukeCooldown = Random.Range(5f, 10f);
			float force = Random.Range(5f, 20f);
			RandomNudge(force);
		}
	}

	private void IdleBreakerVOLogic()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (idleBreakerVOCooldown > 0f)
			{
				idleBreakerVOCooldown -= Time.deltaTime;
			}
			else
			{
				IdleBreakerVO();
			}
		}
	}

	public void IdleBreakerVO()
	{
		if (!audioSourceVO.isPlaying && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("IdleBreakerVORPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				IdleBreakerVORPC();
			}
			idleBreakerVOCooldown = Random.Range(15f, 45f);
		}
	}

	[PunRPC]
	public void IdleBreakerVORPC()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)enemySlowMouthAnim).enabled && ((Behaviour)audioSourceVO).enabled)
		{
			soundIdleBreakerVO.Play(centerTransform.position);
		}
	}

	private void AttachToTarget()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(((Component)this).transform.position, targetDestination);
		if (!VisionBlocked() && num < 2f)
		{
			UpdateState(State.Attached);
		}
	}

	private void FastMoving(bool _lookAtTarget)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_lookAtTarget)
		{
			followTarget.localRotation = Quaternion.LookRotation(currentTarget.position - centerTransform.position, Vector3.up);
		}
		else
		{
			LookAtVelocityDirection(_moving: false);
		}
		enemyRigidbody.OverrideFollowPosition(0.1f, 4f, 4f);
	}

	private void OverrideHideEnemy()
	{
		enemyHiddenTimer = 0.2f;
	}

	private bool VisionBlocked()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.FPSImpulse5())
		{
			Vector3 val = playerTarget.PlayerVisionTarget.VisionTransform.position - enemy.CenterTransform.position;
			visionPrevious = Physics.Raycast(enemy.CenterTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void LeaveCheck()
	{
		if (SemiFunc.EnemyForceLeave(enemy) || targetedPlayerTime >= targetedPlayerTimeMax)
		{
			UpdateState(State.Leave);
		}
	}

	private void TargettingPlayerStart()
	{
		aggroTimer = Random.Range(8f, 15f);
		looseTargetTimer = 0f;
	}

	private void TargettingPlayer()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		StuckLogic();
		float num = currentTarget.position.y - ((Component)this).transform.position.y;
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		if (num > 2f)
		{
			num = 2f;
		}
		followTarget.localPosition = new Vector3(0f, num, 0f);
		targetedPlayerTime += Time.deltaTime;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			float num2 = 1f;
			if (VisionBlocked())
			{
				num2 = 4f;
			}
			aggroTimer -= Time.deltaTime * num2;
			if ((Object.op_Implicit((Object)(object)playerTarget) && playerTarget.isDisabled) || !Object.op_Implicit((Object)(object)playerTarget) || aggroTimer <= 0f)
			{
				UpdateState(State.Leave);
			}
			if (VisionBlocked())
			{
				looseTargetTimer += Time.deltaTime;
				if (looseTargetTimer > 3f)
				{
					UpdateState(State.Leave);
					looseTargetTimer = 0f;
				}
			}
			else
			{
				looseTargetTimer = 0f;
			}
		}
		AudioSourceSmoothStop();
	}

	public void TargetPositionLogic()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder) && Object.op_Implicit((Object)(object)playerTarget))
		{
			Vector3 val = ((currentState != State.GoToPlayer && currentState != State.GoToPlayerUnder && currentState != State.GoToPlayerOver) ? (((Component)playerTarget).transform.position + ((Component)playerTarget).transform.forward * targetForwardOffset) : (((Component)playerTarget).transform.position + ((Component)playerTarget).transform.forward * 1.5f));
			targetPosition = Vector3.Lerp(targetPosition, val, 20f * Time.deltaTime);
		}
	}

	private void AudioSourceSmoothStop()
	{
		if (audioSourceVO.isPlaying)
		{
			audioSourceVO.volume = Mathf.Lerp(audioSourceVO.volume, 0f, Time.deltaTime * 40f);
			if (audioSourceVO.volume <= 0.01f)
			{
				audioSourceVO.Stop();
				audioSourceVO.volume = 1f;
			}
		}
		else if (audioSourceVO.volume < 1f)
		{
			audioSourceVO.volume = 1f;
		}
	}

	private void LoopSounds()
	{
		bool playing = currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder || currentState == State.MoveBackToNavmesh;
		soundChaseLoopVO.PlayLoop(playing, 2f, 2f);
		bool playing2 = currentState == State.Stun;
		soundStunLoopVO.PlayLoop(playing2, 2f, 2f);
	}

	private void AttachToPlayer()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !Object.op_Implicit((Object)(object)playerTarget) || !Object.op_Implicit((Object)(object)currentTarget) || !(Vector3.Distance(centerTransform.position, currentTarget.position) < 1.8f))
		{
			return;
		}
		if (SemiFunc.FPSImpulse30())
		{
			if (movingLeft)
			{
				enemyRigidbody.rb.AddForce(-centerTransform.right * 5f, (ForceMode)0);
				if (moveThisDirectionTimer <= 0f)
				{
					movingLeft = false;
					movingRight = true;
					moveThisDirectionTimer = Random.Range(0.2f, 3f);
				}
			}
			if (movingRight)
			{
				enemyRigidbody.rb.AddForce(centerTransform.right * 5f, (ForceMode)0);
				if (moveThisDirectionTimer <= 0f)
				{
					movingRight = false;
					movingLeft = true;
					moveThisDirectionTimer = Random.Range(0.2f, 3f);
				}
			}
		}
		if (moveThisDirectionTimer > 0f)
		{
			moveThisDirectionTimer -= Time.deltaTime;
		}
		if (Random.Range(0, 2) == 0 && possessCooldown <= 0f && !IsPossessed())
		{
			UpdateState(State.Attack);
			return;
		}
		IdlePukeExecute();
		if (Random.Range(0, 3) == 0)
		{
			idlePukePreviousState = State.Leave;
		}
	}

	public bool IsPossessed()
	{
		if (!Object.op_Implicit((Object)(object)playerTarget))
		{
			return true;
		}
		bool num = Object.op_Implicit((Object)(object)((Component)playerTarget.playerAvatarVisuals).GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>());
		bool flag = Object.op_Implicit((Object)(object)((Component)playerTarget.localCameraTransform).GetComponentInChildren<EnemySlowMouthCameraVisuals>());
		if (num || flag)
		{
			return true;
		}
		return false;
	}

	private void PlayerEffects()
	{
		if (Object.op_Implicit((Object)(object)playerTarget))
		{
			if (Object.op_Implicit((Object)(object)playerTarget.voiceChat))
			{
				playerTarget.voiceChat.OverridePitch(0.75f, 1f, 2f);
			}
			playerTarget.OverridePupilSize(2f, 4, 1f, 1f, 5f, 0.5f);
		}
	}

	private void StuckLogic()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(((Component)this).transform.position, stuckPosition);
		float num2 = 0.25f;
		if (currentState == State.GoToPlayer || currentState == State.GoToPlayerOver || currentState == State.GoToPlayerUnder)
		{
			num2 = 1.5f;
		}
		if (num > num2)
		{
			stuckPosition = ((Component)this).transform.position;
			stuckTimer = 0f;
			randomNudgeTimer = 0f;
			return;
		}
		stuckTimer += Time.deltaTime;
		randomNudgeTimer += Time.deltaTime;
		if (randomNudgeTimer > 2.5f)
		{
			float force = Random.Range(4f, 10f);
			RandomNudge(force);
			randomNudgeTimer = 0f;
			if (currentState == State.GoToPlayer)
			{
				if (Random.Range(0, 2) == 0)
				{
					UpdateState(State.GoToPlayerUnder);
				}
				else
				{
					UpdateState(State.GoToPlayerOver);
				}
			}
		}
		if (stuckTimer > 5f)
		{
			UpdateState(State.IdlePuke);
			stuckTimer = 0f;
		}
	}

	private void IsPossessedBySeveral()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !SemiFunc.FPSImpulse5() || !Object.op_Implicit((Object)(object)playerTarget))
		{
			return;
		}
		if (playerTarget.isDisabled)
		{
			UpdateState(State.Leave);
		}
		else if (playerTarget.isLocal)
		{
			if (((Component)playerTarget.localCameraTransform).GetComponentsInChildren<EnemySlowMouthCameraVisuals>().Length > 1)
			{
				UpdateState(State.Leave);
			}
		}
		else if (((Component)playerTarget.playerAvatarVisuals).GetComponentsInChildren<EnemySlowMouthPlayerAvatarAttached>().Length > 1)
		{
			UpdateState(State.Leave);
		}
	}

	private void MoveBackPosition()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (moveBackTimer <= 0f)
		{
			moveBackTimer = 0.1f;
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(((Component)this).transform.position, ref val, 0.5f, -1) && Physics.Raycast(((Component)this).transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				moveBackPosition = ((NavMeshHit)(ref val)).position;
			}
		}
	}
}
