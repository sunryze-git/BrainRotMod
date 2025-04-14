using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBeamer : MonoBehaviour, IPunObservable
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		AttackStart,
		Attack,
		AttackEnd,
		MeleeStart,
		Melee,
		Seek,
		Leave,
		Stun,
		Despawn
	}

	public State currentState;

	public float stateTimer;

	private bool stateImpulse;

	private float stateTicker;

	internal Enemy enemy;

	internal PhotonView photonView;

	public EnemyBeamerAnim anim;

	public Transform aimVerticalTransform;

	public Transform hatTransform;

	public Transform bottomTransform;

	public SemiLaser laser;

	public Transform laserStartTransform;

	public Transform laserRayTransform;

	private Quaternion aimVerticalTarget;

	private Quaternion aimHorizontalTarget;

	public SpringQuaternion horizontalRotationSpring;

	private Quaternion horizontalRotationTarget = Quaternion.identity;

	[Space]
	public AnimationCurve aimHorizontalCurve;

	public float aimHorizontalSpread;

	public float aimHorizontalSpeed;

	private float aimHorizontalLerp;

	private float aimHorizontalResult;

	internal PlayerAvatar playerTarget;

	private Vector3 agentDestination;

	private Vector3 seekDestination;

	private Vector3 meleeTarget;

	private bool meleePlayer;

	private float laserCooldown;

	private float laserRange = 10f;

	private Vector3 hitPosition;

	private Vector3 hitPositionSmooth;

	private float hitPositionClientDistance;

	private bool hitPositionStartImpulse;

	private bool hitPositionImpact;

	private float hitPositionTimer;

	public ParticleSystem particleDeathSmoke;

	public ParticleSystem particleDeathBody;

	public ParticleSystem particleDeathNose;

	public ParticleSystem particleDeathHat;

	public ParticleSystem particleBottomSmoke;

	internal bool moveFast;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		VerticalAimLogic();
		LaserLogic();
		MoveFastLogic();
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
			case State.Investigate:
				StateInvestigate();
				break;
			case State.AttackStart:
				StateAttackStart();
				break;
			case State.Attack:
				StateAttack();
				break;
			case State.AttackEnd:
				StateAttackEnd();
				break;
			case State.MeleeStart:
				StateMeleeStart();
				break;
			case State.Melee:
				StateMelee();
				break;
			case State.Seek:
				StateSeek();
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
			RotationLogic();
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
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(4f, 8f);
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
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 999f;
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
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (!enemy.Jump.jumping && Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			UpdateState(State.Idle);
		}
		else if (enemy.Rigidbody.notMovingTimer >= 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StateInvestigate()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 999f;
			enemy.Rigidbody.notMovingTimer = 0f;
		}
		else
		{
			enemy.NavMeshAgent.SetDestination(agentDestination);
			if (!enemy.Jump.jumping && (Vector3.Distance(((Component)enemy.Rigidbody).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f || Vector3.Distance(((Component)enemy.Rigidbody).transform.position, agentDestination) < 1f))
			{
				UpdateState(State.Idle);
			}
			else if (enemy.Rigidbody.notMovingTimer >= 3f)
			{
				AttackNearestPhysObjectOrGoToIdle();
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.Leave);
		}
	}

	private void StateAttackStart()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			aimHorizontalResult = 0f;
			stateImpulse = false;
			stateTimer = 1.5f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			seekDestination = ((Component)playerTarget).transform.position;
			aimHorizontalLerp = 0f;
		}
		aimHorizontalResult = Mathf.Lerp(0f, 0f - aimHorizontalSpread, aimHorizontalCurve.Evaluate(aimHorizontalLerp));
		aimHorizontalLerp += 1.5f * Time.deltaTime;
		aimHorizontalTarget = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
		aimHorizontalTarget = Quaternion.Euler(0f, ((Quaternion)(ref aimHorizontalTarget)).eulerAngles.y, 0f);
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Attack);
		}
	}

	private void StateAttack()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 0.5f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			aimHorizontalLerp = 0f;
		}
		aimHorizontalResult = Mathf.Lerp(0f - aimHorizontalSpread, aimHorizontalSpread, aimHorizontalCurve.Evaluate(aimHorizontalLerp));
		aimHorizontalLerp += aimHorizontalSpeed * Time.deltaTime;
		if (aimHorizontalLerp >= 1f)
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.AttackEnd);
			}
		}
	}

	private void StateAttackEnd()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 2f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			aimHorizontalLerp = 0f;
		}
		aimHorizontalResult = Mathf.Lerp(aimHorizontalSpread, 0f, aimHorizontalCurve.Evaluate(aimHorizontalLerp));
		aimHorizontalLerp += 1.5f * Time.deltaTime;
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Seek);
		}
	}

	private void StateMeleeStart()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 0.5f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			seekDestination = meleeTarget;
		}
		if (meleePlayer)
		{
			meleeTarget = ((Component)playerTarget).transform.position;
			seekDestination = meleeTarget;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Melee);
		}
	}

	private void StateMelee()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Seek);
		}
	}

	private void StateSeek()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateTimer = 20f;
			stateImpulse = false;
			enemy.Rigidbody.notMovingTimer = 0f;
			if (Vector3.Distance(((Component)this).transform.position, seekDestination) >= 3f)
			{
				moveFast = true;
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("MoveFastRPC", (RpcTarget)1, new object[1] { moveFast });
				}
			}
		}
		enemy.NavMeshAgent.SetDestination(seekDestination);
		if (moveFast)
		{
			enemy.NavMeshAgent.OverrideAgent(enemy.NavMeshAgent.DefaultSpeed * 2f, enemy.NavMeshAgent.DefaultAcceleration * 2f, 0.1f);
		}
		if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			LevelPoint levelPointAhead = enemy.GetLevelPointAhead(seekDestination);
			if (Object.op_Implicit((Object)(object)levelPointAhead))
			{
				seekDestination = ((Component)levelPointAhead).transform.position;
			}
			if (moveFast)
			{
				moveFast = false;
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("MoveFastRPC", (RpcTarget)1, new object[1] { moveFast });
				}
			}
		}
		if (enemy.Rigidbody.notMovingTimer >= 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
			return;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Idle);
		}
	}

	public void StateLeave()
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
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
			stateTimer = 999f;
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
			enemy.Vision.DisableVision(5f);
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		enemy.NavMeshAgent.SetDestination(agentDestination);
		if (Vector3.Distance(((Component)this).transform.position, enemy.NavMeshAgent.GetPoint()) < 1f)
		{
			UpdateState(State.Idle);
		}
		else if (enemy.Rigidbody.notMovingTimer >= 3f)
		{
			AttackNearestPhysObjectOrGoToIdle();
		}
	}

	private void StateStun()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	private void StateDespawn()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateImpulse = false;
		}
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.EnemySpawn(enemy))
		{
			UpdateState(State.Spawn);
		}
	}

	public void OnVision()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Jump.jumping)
		{
			return;
		}
		if (currentState == State.Roam || currentState == State.Idle || currentState == State.Seek || currentState == State.Leave || currentState == State.Investigate)
		{
			playerTarget = enemy.Vision.onVisionTriggeredPlayer;
			if (Object.op_Implicit((Object)(object)playerTarget) && Vector3.Distance(((Component)this).transform.position, ((Component)playerTarget).transform.position) < 2.5f && Mathf.Abs(((Component)this).transform.position.y - ((Component)playerTarget).transform.position.y) < 1f)
			{
				meleeTarget = ((Component)playerTarget).transform.position;
				meleePlayer = true;
				UpdateState(State.MeleeStart);
			}
			else if (laserCooldown <= 0f)
			{
				UpdateState(State.AttackStart);
			}
			else
			{
				seekDestination = ((Component)playerTarget).transform.position;
				UpdateState(State.Seek);
			}
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdatePlayerTargetRPC", (RpcTarget)0, new object[1] { playerTarget.photonView.ViewID });
			}
		}
		else if ((currentState == State.AttackStart || currentState == State.Attack || currentState == State.AttackEnd) && (Object)(object)playerTarget == (Object)(object)enemy.Vision.onVisionTriggeredPlayer)
		{
			seekDestination = ((Component)playerTarget).transform.position;
		}
	}

	public void OnInvestigate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && (currentState == State.Idle || currentState == State.Roam || currentState == State.Seek || currentState == State.Investigate))
		{
			agentDestination = enemy.StateInvestigate.onInvestigateTriggeredPosition;
			UpdateState(State.Investigate);
		}
	}

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		anim.soundHurt.Play(((Component)anim).transform.position);
		anim.soundHurtPauseTimer = 0.5f;
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		anim.soundDeath.Play(((Component)anim).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, enemy.CenterTransform.position, 0.05f);
		((Component)particleDeathSmoke).transform.position = enemy.CenterTransform.position;
		particleDeathSmoke.Play();
		((Component)particleDeathBody).transform.position = enemy.CenterTransform.position;
		particleDeathBody.Play();
		((Component)particleDeathNose).transform.position = laserStartTransform.position;
		particleDeathNose.Play();
		((Component)particleDeathHat).transform.position = hatTransform.position;
		particleDeathHat.Play();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	private void UpdateState(State _state)
	{
		if (_state != currentState)
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

	private void AttackNearestPhysObjectOrGoToIdle()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		meleeTarget = SemiFunc.EnemyGetNearestPhysObject(enemy);
		if (meleeTarget != Vector3.zero)
		{
			meleePlayer = false;
			UpdateState(State.Melee);
		}
		else
		{
			UpdateState(State.Idle);
		}
	}

	private void RotationLogic()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.AttackStart || currentState == State.Attack || currentState == State.AttackEnd)
		{
			horizontalRotationTarget = Quaternion.Euler(((Quaternion)(ref aimHorizontalTarget)).eulerAngles.x, ((Quaternion)(ref aimHorizontalTarget)).eulerAngles.y + aimHorizontalResult, ((Quaternion)(ref aimHorizontalTarget)).eulerAngles.z);
		}
		else if (currentState == State.MeleeStart)
		{
			if (Vector3.Distance(meleeTarget, ((Component)enemy.Rigidbody).transform.position) > 0.1f)
			{
				horizontalRotationTarget = Quaternion.LookRotation(meleeTarget - ((Component)enemy.Rigidbody).transform.position);
				((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
			}
		}
		else
		{
			Vector3 normalized = ((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized;
			if (((Vector3)(ref normalized)).magnitude > 0.1f)
			{
				horizontalRotationTarget = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
				((Quaternion)(ref horizontalRotationTarget)).eulerAngles = new Vector3(0f, ((Quaternion)(ref horizontalRotationTarget)).eulerAngles.y, 0f);
			}
		}
		if (currentState == State.Spawn || currentState == State.Idle || currentState == State.Roam || currentState == State.Investigate || currentState == State.Leave)
		{
			horizontalRotationSpring.speed = 5f;
			horizontalRotationSpring.damping = 0.7f;
		}
		else if (currentState == State.AttackStart || currentState == State.Attack || currentState == State.AttackEnd)
		{
			horizontalRotationSpring.speed = 15f;
			horizontalRotationSpring.damping = 0.8f;
		}
		else
		{
			horizontalRotationSpring.speed = 10f;
			horizontalRotationSpring.damping = 0.8f;
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(horizontalRotationSpring, horizontalRotationTarget);
	}

	private void VerticalAimLogic()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.AttackStart && currentState != State.Attack && currentState != State.AttackEnd)
		{
			aimVerticalTarget = Quaternion.identity;
		}
		else if (currentState == State.AttackStart || (currentState != State.Attack && aimHorizontalLerp < 0.1f))
		{
			Quaternion val = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - laserRayTransform.position);
			if (aimVerticalTarget == Quaternion.identity)
			{
				aimVerticalTarget = val;
			}
			else
			{
				aimVerticalTarget = Quaternion.Lerp(aimVerticalTarget, val, 2f * Time.deltaTime);
			}
			Quaternion rotation = laserRayTransform.rotation;
			laserRayTransform.rotation = aimVerticalTarget;
			aimVerticalTarget = laserRayTransform.localRotation;
			aimVerticalTarget = Quaternion.Euler(laserRayTransform.eulerAngles.x, 0f, 0f);
			laserRayTransform.rotation = rotation;
		}
		aimVerticalTransform.localRotation = Quaternion.Lerp(aimVerticalTransform.localRotation, aimVerticalTarget, 20f * Time.deltaTime);
		laserRayTransform.localRotation = aimVerticalTarget;
	}

	private void LaserLogic()
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.Attack && currentState != State.AttackStart && currentState != State.AttackEnd)
		{
			laserCooldown -= Time.deltaTime;
		}
		if (currentState == State.Attack)
		{
			laserCooldown = 3f;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				Transform val = laserStartTransform;
				Vector3 val2 = laserRayTransform.position - laserStartTransform.position;
				RaycastHit val3 = default(RaycastHit);
				if (Physics.Raycast(laserStartTransform.position, val2, ref val3, ((Vector3)(ref val2)).magnitude, LayerMask.GetMask(new string[1] { "Default" })))
				{
					val = laserRayTransform;
				}
				else if (Physics.OverlapSphere(laserStartTransform.position, 0.25f, LayerMask.GetMask(new string[1] { "Default" })).Length != 0)
				{
					val = laserRayTransform;
				}
				if (hitPositionTimer <= 0f)
				{
					hitPositionTimer = 0.05f;
					RaycastHit val4 = default(RaycastHit);
					if (Physics.Raycast(val.position, val.forward, ref val4, laserRange, LayerMask.GetMask(new string[1] { "Default" })))
					{
						hitPosition = ((RaycastHit)(ref val4)).point;
						hitPositionImpact = true;
					}
					else
					{
						hitPosition = val.position + val.forward * laserRange;
						hitPositionImpact = false;
					}
				}
				else
				{
					hitPositionTimer -= Time.deltaTime;
				}
				hitPositionSmooth = Vector3.Lerp(hitPositionSmooth, hitPosition, 20f * Time.deltaTime);
			}
			else
			{
				hitPositionSmooth = Vector3.MoveTowards(hitPositionSmooth, hitPosition, hitPositionClientDistance * Time.deltaTime * ((float)PhotonNetwork.SerializationRate * 0.8f));
			}
			if (hitPositionStartImpulse)
			{
				hitPositionSmooth = hitPosition;
				hitPositionStartImpulse = false;
			}
			Vector3 val5 = laserRayTransform.position - hitPosition;
			Vector3 val6 = laserRayTransform.position - hitPositionSmooth;
			if (((Vector3)(ref val5)).magnitude < ((Vector3)(ref val6)).magnitude)
			{
				val6 = Vector3.ClampMagnitude(val6, ((Vector3)(ref val5)).magnitude);
				hitPositionSmooth = laserRayTransform.position - val6;
			}
			laser.LaserActive(laserStartTransform.position, hitPositionSmooth, hitPositionImpact);
		}
		else
		{
			hitPositionTimer = 0f;
			hitPositionStartImpulse = true;
		}
	}

	private void MoveFastLogic()
	{
		if (currentState != State.Seek && moveFast)
		{
			moveFast = false;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("MoveFastRPC", (RpcTarget)1, new object[1] { moveFast });
			}
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

	[PunRPC]
	private void MeleeTriggerRPC()
	{
		anim.meleeImpulse = true;
	}

	[PunRPC]
	private void MoveFastRPC(bool _moveFast)
	{
		moveFast = _moveFast;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)aimVerticalTarget);
			stream.SendNext((object)hitPosition);
			stream.SendNext((object)hitPositionImpact);
		}
		else
		{
			aimVerticalTarget = (Quaternion)stream.ReceiveNext();
			hitPosition = (Vector3)stream.ReceiveNext();
			hitPositionImpact = (bool)stream.ReceiveNext();
			hitPositionClientDistance = Vector3.Distance(hitPositionSmooth, hitPosition);
		}
	}
}
