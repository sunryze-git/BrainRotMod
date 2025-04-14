using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBang : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		FuseDelay,
		Fuse,
		Move,
		MoveUnder,
		MoveOver,
		MoveBack,
		Stun,
		StunEnd,
		Despawn
	}

	public EnemyBangAnim anim;

	[Space]
	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	internal Enemy enemy;

	internal PhotonView photonView;

	internal int directorIndex;

	private Vector3 moveBackPosition;

	internal bool fuseActive;

	internal float fuseLerp;

	private ParticleScriptExplosion particleScriptExplosion;

	private ParticlePrefabExplosion explosionScript;

	private bool visionPrevious;

	private float visionTimer;

	public SpringQuaternion horizontalRotationSpring;

	private Quaternion horizontalRotationTarget = Quaternion.identity;

	[Space]
	public SpringQuaternion headLookAtSpring;

	public Transform headLookAtTarget;

	public Transform headLookAtSource;

	[Space]
	public ParticleSystem[] deathEffects;

	[Space]
	public GameObject[] headObjects;

	[Space]
	public Transform particleParent;

	public Transform moveOffsetTransform;

	public Transform rotationTransform;

	private Vector3 moveOffsetPosition;

	private float moveOffsetTimer;

	private float moveOffsetSetTimer;

	public AudioSource talkSource;

	public AudioSource stunLoopSource;

	[Space]
	public Sound[] talkSoundsTest;

	[Space]
	public SpringQuaternion talkTopSpring;

	public Transform talkTopSource;

	public Transform talkTopTarget;

	[Space]
	public SpringQuaternion talkBottomSpring;

	public Transform talkBottomSource;

	public Transform talkBottomTarget;

	[Space]
	public float talkBreakerIdleTimeMin = 5f;

	public float talkBreakerIdleTimeMax = 20f;

	[Space]
	public float talkBreakerAttackTimeMin = 2f;

	public float talkBreakerAttackTimeMax = 5f;

	private float talkBreakerTimer;

	private bool explosionTell;

	private float explosionTellThreshold = 0.95f;

	internal float explosionTellFuseThreshold = 0.9f;

	private float talkClipTimer;

	private float talkClipLoudness;

	private int talkClipSampleDataLength = 1024;

	private float[] talkClipSampleData;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		talkClipSampleData = new float[talkClipSampleDataLength];
	}

	private void Update()
	{
		HeadLookAtLogic();
		FuseLogic();
		TalkLogic();
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && LevelGenerator.Instance.Generated)
		{
			if (enemy.IsStunned())
			{
				UpdateState(State.Stun);
			}
			if (enemy.CurrentState == EnemyState.Despawn)
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
			case State.FuseDelay:
				StateFuseDelay();
				break;
			case State.Fuse:
				StateFuse();
				break;
			case State.Move:
				StateMove();
				break;
			case State.MoveUnder:
				StateMoveUnder();
				break;
			case State.MoveOver:
				StateMoveOver();
				break;
			case State.MoveBack:
				StateMoveBack();
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
			TimerLogic();
			RotationLogic();
			MoveOffsetLogic();
		}
	}

	private void StateSpawn()
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
			UpdateState(State.Idle);
		}
	}

	private void StateIdle()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
		}
		Vector3 val = EnemyBangDirector.instance.destinations[directorIndex];
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer || EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackCart)
		{
			val = EnemyBangDirector.instance.attackPosition;
		}
		enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) > 2f)
		{
			UpdateState(State.Roam);
		}
	}

	private void StateRoam()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
		}
		Vector3 val = EnemyBangDirector.instance.destinations[directorIndex];
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer || EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackCart)
		{
			val = EnemyBangDirector.instance.attackPosition;
		}
		enemy.NavMeshAgent.SetDestination(val);
		MoveBackPosition();
		SemiFunc.EnemyCartJump(enemy);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) <= 0.5f)
		{
			UpdateState(State.Idle);
		}
		else if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) <= 2f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateFuseDelay()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(0.1f, 1f);
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
		}
		enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Fuse);
		}
	}

	private void StateFuse()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			FuseSet(_active: true, 0f);
			stateImpulse = false;
			stateTimer = 1.5f;
		}
		enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Move);
		}
	}

	private void StateMove()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		Vector3 val = AttackPositionGet();
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) > 1.5f)
		{
			enemy.NavMeshAgent.SetDestination(val);
		}
		else
		{
			enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
			enemy.NavMeshAgent.Disable(0.1f);
		}
		MoveBackPosition();
		SemiFunc.EnemyCartJump(enemy);
		if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
		{
			UpdateState(State.Idle);
		}
		else
		{
			if (enemy.NavMeshAgent.CanReach(AttackVisionDynamic(), 1f) || !(Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 2f))
			{
				return;
			}
			NavMeshHit val2 = default(NavMeshHit);
			if (!enemy.Jump.jumping && !VisionBlocked() && !NavMesh.SamplePosition(AttackVisionDynamic(), ref val2, 0.5f, -1))
			{
				if (EnemyBangDirector.instance.playerTargetCrawling && Mathf.Abs(AttackVisionDynamic().y - ((Component)enemy.Rigidbody).transform.position.y) < 0.3f)
				{
					UpdateState(State.MoveUnder);
					return;
				}
				if (val.y > ((Component)enemy.Rigidbody).transform.position.y)
				{
					UpdateState(State.MoveOver);
					return;
				}
			}
			if (val.y > ((Component)enemy.Rigidbody).transform.position.y + 0.2f)
			{
				enemy.Jump.StuckTrigger(AttackVisionPositionGet() - enemy.Vision.VisionTransform.position);
			}
		}
	}

	private void StateMoveUnder()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		Vector3 val = AttackPositionGet();
		enemy.NavMeshAgent.Disable(0.1f);
		enemy.Vision.StandOverride(0.25f);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) > 1f)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, val, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		else
		{
			enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		}
		enemy.Jump.StuckDisable(0.5f);
		SemiFunc.EnemyCartJump(enemy);
		NavMeshHit val2 = default(NavMeshHit);
		if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
		{
			UpdateState(State.MoveBack);
		}
		else if (NavMesh.SamplePosition(val, ref val2, 0.5f, -1))
		{
			UpdateState(State.MoveBack);
		}
		else if (VisionBlocked())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.MoveBack);
			}
		}
		else
		{
			EnemyBangDirector.instance.SeeTarget();
			stateTimer = 2f;
		}
	}

	private void StateMoveOver()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 2f;
			stateImpulse = false;
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		enemy.Vision.StandOverride(0.25f);
		Vector3 val = AttackPositionGet();
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, val) > 1f)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, val, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		else
		{
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
			enemy.Rigidbody.DisableFollowPosition(0.1f, 5f);
		}
		SemiFunc.EnemyCartJump(enemy);
		if (AttackVisionDynamic().y > ((Component)enemy.Rigidbody).transform.position.y + 0.3f && !enemy.Jump.jumping)
		{
			Vector3 val2 = AttackVisionDynamic() - ((Component)enemy.Rigidbody).transform.position;
			Vector3 normalized = ((Vector3)(ref val2)).normalized;
			enemy.Jump.StuckTrigger(normalized);
			enemy.Rigidbody.WarpDisable(0.25f);
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, AttackVisionDynamic(), 2f);
		}
		if (enemy.Jump.jumping)
		{
			return;
		}
		NavMeshHit val3 = default(NavMeshHit);
		if (EnemyBangDirector.instance.currentState != EnemyBangDirector.State.AttackPlayer)
		{
			UpdateState(State.MoveBack);
		}
		else if (NavMesh.SamplePosition(val, ref val3, 0.5f, -1))
		{
			UpdateState(State.MoveBack);
		}
		else if (VisionBlocked())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.MoveBack);
			}
		}
		else
		{
			EnemyBangDirector.instance.SeeTarget();
			stateTimer = 2f;
		}
	}

	private void StateMoveBack()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
		}
		enemy.NavMeshAgent.Disable(0.1f);
		if (!enemy.Jump.jumping)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveBackPosition, enemy.NavMeshAgent.DefaultSpeed * Time.deltaTime);
		}
		SemiFunc.EnemyCartJump(enemy);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f && (Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) > 2f || enemy.Rigidbody.notMovingTimer > 2f) && !enemy.Jump.jumping)
		{
			Vector3 val = ((Component)this).transform.position - moveBackPosition;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			enemy.Jump.StuckTrigger(((Component)this).transform.position - moveBackPosition);
			((Component)this).transform.position = ((Component)enemy.Rigidbody).transform.position;
			Transform transform = ((Component)this).transform;
			transform.position += normalized * 2f;
		}
		bool flag = false;
		NavMeshHit val2 = default(NavMeshHit);
		if (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, moveBackPosition) <= 0.2f)
		{
			flag = true;
		}
		else if (NavMesh.SamplePosition(((Component)enemy.Rigidbody).transform.position, ref val2, 0.5f, -1))
		{
			flag = true;
		}
		if (flag)
		{
			if (fuseActive)
			{
				UpdateState(State.Move);
			}
			else
			{
				UpdateState(State.Idle);
			}
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
			UpdateState(State.StunEnd);
		}
	}

	private void StateStunEnd()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.ResetPath();
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			stateImpulse = false;
			stateTimer = 1f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.MoveBack);
		}
	}

	private void StateDespawn()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			if (!fuseActive)
			{
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
				enemy.NavMeshAgent.ResetPath();
			}
			stateImpulse = false;
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			EnemyBangDirector.instance.OnSpawn();
			UpdateState(State.Spawn);
			FuseSet(_active: false, 0f);
		}
	}

	public void OnHurt()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)talkSource).isActiveAndEnabled)
		{
			anim.soundHurt.Play(enemy.CenterTransform.position);
			anim.StunLoopPause(0.5f);
		}
	}

	public void OnDeath()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		anim.soundDeathSFX.Play(enemy.CenterTransform.position);
		anim.soundDeathVO.Play(enemy.CenterTransform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
		if (fuseActive && fuseLerp <= 1f)
		{
			explosionScript = particleScriptExplosion.Spawn(enemy.CenterTransform.position, 0.5f, 15, 10);
			explosionScript.HurtCollider.onImpactEnemy.AddListener(new UnityAction(OnExplodeHitEnemy));
		}
		ParticleSystem[] array = deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void OnInvestigate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			EnemyBangDirector.instance.Investigate(enemy.StateInvestigate.onInvestigateTriggeredPosition);
		}
	}

	public void OnVision()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		EnemyBangDirector.instance.SetTarget(enemy.Vision.onVisionTriggeredPlayer);
		if (currentState == State.Idle || currentState == State.Roam)
		{
			if (!fuseActive)
			{
				UpdateState(State.FuseDelay);
			}
			else
			{
				UpdateState(State.Move);
			}
			EnemyBangDirector.instance.TriggerNearby(((Component)this).transform.position);
		}
	}

	public void OnExplodeHitEnemy()
	{
		if (Object.op_Implicit((Object)(object)explosionScript))
		{
			EnemyBang component = ((Component)explosionScript.HurtCollider.onImpactEnemyEnemy).GetComponent<EnemyBang>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.enemy.Health.healthCurrent = 999;
				component.FuseSet(_active: true, Random.Range(0.96f, 0.98f));
			}
		}
	}

	public void OnImpactLight()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			anim.soundImpactLight.Play(enemy.CenterTransform.position);
		}
	}

	public void OnImpactMedium()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			anim.soundImpactMedium.Play(enemy.CenterTransform.position);
		}
	}

	public void OnImpactHeavy()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.IsStunned())
		{
			anim.soundImpactHeavy.Play(enemy.CenterTransform.position);
		}
	}

	public void UpdateState(State _state)
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
			else
			{
				UpdateStateRPC(currentState);
			}
		}
	}

	private void RotationLogic()
	{
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.StunEnd)
		{
			if (currentState == State.Idle)
			{
				Vector3 position = ((Component)EnemyBangDirector.instance).transform.position;
				if (Vector3.Distance(position, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
				{
					horizontalRotationTarget = Quaternion.LookRotation(position - ((Component)enemy.Rigidbody).transform.position);
					((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
				}
			}
			else if (currentState == State.FuseDelay || currentState == State.Fuse || currentState == State.Move || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.MoveBack)
			{
				if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude < 0.1f)
				{
					Vector3 val = AttackVisionPositionGet();
					if (Vector3.Distance(val, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
					{
						horizontalRotationTarget = Quaternion.LookRotation(val - ((Component)enemy.Rigidbody).transform.position);
						((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
					}
				}
				else
				{
					horizontalRotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.Rigidbody.velocity)).normalized);
					((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
				}
			}
			else if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.1f)
			{
				horizontalRotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.Rigidbody.velocity)).normalized);
				((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
			}
		}
		rotationTransform.rotation = SemiFunc.SpringQuaternionGet(horizontalRotationSpring, horizontalRotationTarget);
	}

	private Vector3 AttackPositionGet()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return EnemyBangDirector.instance.attackPosition;
	}

	private Vector3 AttackVisionPositionGet()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return EnemyBangDirector.instance.attackVisionPosition;
	}

	private Vector3 AttackVisionDynamic()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (EnemyBangDirector.instance.currentState == EnemyBangDirector.State.AttackPlayer)
		{
			return AttackPositionGet();
		}
		return AttackVisionPositionGet();
	}

	private void MoveBackPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)this).transform.position, ((Component)enemy.Rigidbody).transform.position) < 1f)
		{
			moveBackPosition = ((Component)this).transform.position;
		}
	}

	private bool VisionBlocked()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (visionTimer <= 0f)
		{
			visionTimer = 0.1f;
			Vector3 val = AttackVisionPositionGet() - enemy.Vision.VisionTransform.position;
			visionPrevious = Physics.Raycast(enemy.Vision.VisionTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		}
		return visionPrevious;
	}

	private void HeadLookAtLogic()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (currentState == State.Move || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.MoveBack)
		{
			flag = true;
		}
		if (flag)
		{
			Vector3 direction = AttackVisionPositionGet() - headLookAtTarget.position;
			direction = SemiFunc.ClampDirection(direction, headLookAtTarget.forward, 90f);
			headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(headLookAtSpring, Quaternion.LookRotation(direction));
		}
		else
		{
			headLookAtSource.rotation = SemiFunc.SpringQuaternionGet(headLookAtSpring, headLookAtTarget.rotation);
		}
	}

	private void TimerLogic()
	{
		visionTimer -= Time.deltaTime;
	}

	private void FuseLogic()
	{
		if (!fuseActive)
		{
			return;
		}
		if (!EnemyBangDirector.instance.debugNoFuseProgress)
		{
			fuseLerp += Time.deltaTime / 15f;
		}
		fuseLerp = Mathf.Clamp01(fuseLerp);
		if (SemiFunc.IsMasterClientOrSingleplayer() && fuseLerp >= 1f)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ExplodeRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				ExplodeRPC();
			}
			UpdateState(State.Despawn);
			stateImpulse = false;
			enemy.EnemyParent.Despawn();
		}
	}

	private void MoveOffsetLogic()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if ((currentState == State.Move || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.MoveBack) && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, AttackPositionGet()) > 2f)
		{
			moveOffsetTimer = 0.2f;
		}
		else if (currentState == State.Roam && Vector3.Distance(((Component)enemy.Rigidbody).transform.position, EnemyBangDirector.instance.destinations[directorIndex]) <= 2f)
		{
			moveOffsetTimer = 0.2f;
		}
		if (moveOffsetTimer > 0f)
		{
			moveOffsetTimer -= Time.deltaTime;
			if (enemy.Jump.jumping)
			{
				moveOffsetTimer = 0f;
			}
			if (moveOffsetTimer <= 0f)
			{
				moveOffsetPosition = Vector3.zero;
			}
			else
			{
				moveOffsetSetTimer -= Time.deltaTime;
				if (moveOffsetSetTimer <= 0f)
				{
					Vector3 insideUnitSphere = Random.insideUnitSphere;
					Vector3 val = ((Vector3)(ref insideUnitSphere)).normalized * Random.Range(0.5f, 1f);
					val.y = 0f;
					moveOffsetPosition = val;
					moveOffsetSetTimer = Random.Range(0.5f, 2f);
				}
			}
		}
		moveOffsetTransform.localPosition = Vector3.Lerp(moveOffsetTransform.localPosition, moveOffsetPosition, Time.deltaTime * 5f);
	}

	private void TalkLogic()
	{
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && explosionTell)
		{
			bool flag = false;
			bool flag2 = false;
			if (!enemy.Jump.jumping)
			{
				if (currentState == State.Idle || currentState == State.Roam)
				{
					flag = true;
					flag2 = true;
				}
				else if (currentState == State.Move || currentState == State.MoveUnder || currentState == State.MoveOver || currentState == State.MoveBack)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (!flag)
				{
					talkBreakerTimer = Mathf.Min(talkBreakerTimer, talkBreakerAttackTimeMax);
				}
				talkBreakerTimer -= Time.deltaTime;
				if (talkBreakerTimer <= 0f)
				{
					if (flag)
					{
						talkBreakerTimer = Random.Range(talkBreakerIdleTimeMin, talkBreakerIdleTimeMax);
					}
					else
					{
						talkBreakerTimer = Random.Range(talkBreakerAttackTimeMin, talkBreakerAttackTimeMax);
					}
					if (SemiFunc.IsMultiplayer())
					{
						photonView.RPC("TalkBreakerRPC", (RpcTarget)0, new object[1] { flag });
					}
					else
					{
						TalkBreakerRPC(flag);
					}
				}
			}
			else
			{
				talkBreakerTimer = Mathf.Max(talkBreakerTimer, 2f);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (fuseActive)
			{
				if (explosionTell && fuseLerp >= explosionTellThreshold)
				{
					explosionTell = false;
					if (SemiFunc.IsMultiplayer())
					{
						photonView.RPC("ExplosionTellRPC", (RpcTarget)0, Array.Empty<object>());
					}
					else
					{
						ExplosionTellRPC();
					}
				}
			}
			else
			{
				explosionTell = true;
			}
		}
		if (talkClipTimer <= 0f)
		{
			talkClipTimer = 0.01f;
			talkClipLoudness = 0f;
			if (Object.op_Implicit((Object)(object)talkSource.clip) && talkSource.isPlaying)
			{
				talkSource.clip.GetData(talkClipSampleData, talkSource.timeSamples);
				float[] array = talkClipSampleData;
				foreach (float num in array)
				{
					talkClipLoudness += Mathf.Abs(num);
				}
				talkClipLoudness /= talkClipSampleDataLength;
			}
			if (Object.op_Implicit((Object)(object)stunLoopSource.clip) && stunLoopSource.isPlaying)
			{
				stunLoopSource.clip.GetData(talkClipSampleData, stunLoopSource.timeSamples);
				float[] array = talkClipSampleData;
				foreach (float num2 in array)
				{
					talkClipLoudness += Mathf.Abs(num2);
				}
				talkClipLoudness /= talkClipSampleDataLength;
			}
		}
		else
		{
			talkClipTimer -= Time.deltaTime;
		}
		talkTopTarget.localRotation = Quaternion.Euler(talkClipLoudness * -45f, 0f, 0f);
		talkBottomTarget.localRotation = Quaternion.Euler(talkClipLoudness * 90f, 0f, 0f);
		talkTopSource.localRotation = SemiFunc.SpringQuaternionGet(talkTopSpring, talkTopTarget.localRotation);
		talkBottomSource.localRotation = SemiFunc.SpringQuaternionGet(talkBottomSpring, talkBottomTarget.localRotation);
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
	}

	private void FuseSet(bool _active, float _lerp)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("FuseRPC", (RpcTarget)0, new object[2] { _active, _lerp });
		}
		else
		{
			FuseRPC(_active, _lerp);
		}
	}

	[PunRPC]
	private void FuseRPC(bool _active, float _lerp)
	{
		fuseActive = _active;
		fuseLerp = _lerp;
	}

	[PunRPC]
	private void ExplodeRPC()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		ParticleSystem[] array = deathEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		explosionScript = particleScriptExplosion.Spawn(enemy.CenterTransform.position, 1f, 30, 25, 2f);
		explosionScript.HurtCollider.onImpactEnemy.AddListener(new UnityAction(OnExplodeHitEnemy));
	}

	[PunRPC]
	private void TalkBreakerRPC(bool _idle)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_idle)
		{
			anim.soundIdleBreaker.Play(((Component)talkSource).transform.position);
		}
		else
		{
			anim.soundAttackBreaker.Play(((Component)talkSource).transform.position);
		}
	}

	[PunRPC]
	private void ExplosionTellRPC()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		anim.StunLoopPause(2f);
		anim.soundExplosionTell.Play(((Component)talkSource).transform.position);
	}

	[PunRPC]
	public void SetHeadRPC(int _index)
	{
		int num = 0;
		GameObject[] array = headObjects;
		foreach (GameObject val in array)
		{
			if (num == _index)
			{
				val.SetActive(true);
			}
			else
			{
				Object.Destroy((Object)(object)val);
			}
			num++;
		}
	}

	[PunRPC]
	public void SetVoicePitchRPC(float _pitch)
	{
		anim.soundAttackBreaker.Pitch = _pitch;
		anim.soundIdleBreaker.Pitch = _pitch;
		anim.soundHurt.Pitch = _pitch;
		anim.soundDeathVO.Pitch = _pitch;
		anim.soundExplosionTell.Pitch = _pitch;
		anim.soundFuseTell.Pitch = _pitch;
		anim.soundJumpVO.Pitch = _pitch;
		anim.soundLandVO.Pitch = _pitch;
		anim.soundStunIntro.Pitch = _pitch;
		anim.soundStunLoop.Pitch = _pitch;
		anim.soundStunOutro.Pitch = _pitch;
	}
}
