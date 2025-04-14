using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHunter : MonoBehaviour
{
	public enum State
	{
		Spawn,
		Idle,
		Roam,
		Investigate,
		InvestigateWalk,
		Aim,
		Shoot,
		ShootEnd,
		LeaveStart,
		Leave,
		Despawn,
		Stun
	}

	public bool debugSpawn;

	[Space]
	public State currentState;

	private bool stateImpulse;

	internal float stateTimer;

	[Space]
	public Enemy enemy;

	public EnemyHunterAnim enemyHunterAnim;

	public EnemyHunterAlwaysActive enemyHunterAlwaysActive;

	private PhotonView photonView;

	public Transform investigateRayTransform;

	public Transform verticalAimTransform;

	public Transform gunAimTransform;

	public Transform gunTipTransform;

	public HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private bool shootFast;

	[Space]
	public LineRenderer lineRenderer;

	public AnimationCurve lineRendererWidthCurve;

	private float lineRendererLerp;

	private bool lineRendererActive;

	[Space]
	public SpringQuaternion horizontalAimSpring;

	private Quaternion horizontalAimTarget = Quaternion.identity;

	public SpringQuaternion verticalAimSpring;

	private float pitCheckTimer;

	private int shotsFired;

	private int shotsFiredMax = 4;

	private Vector3 leavePosition;

	private Vector3 investigatePoint;

	private Quaternion investigateAimHorizontal = Quaternion.identity;

	private Quaternion investigateAimVertical = Quaternion.identity;

	private Quaternion investigateAimVerticalPrevious = Quaternion.identity;

	private float investigateAimVerticalRPCTimer;

	private bool investigatePointHasTransform;

	private Transform investigatePointTransform;

	private Vector3 investigatePointTransformPrevious;

	private Vector3 investigatePointSpread;

	private Vector3 investigatePointSpreadTarget;

	private float investigatePointSpreadTimer;

	private int leaveInterruptCounter;

	private float leaveInterruptTimer;

	private float tripTimer;

	[Space]
	public Transform shootEffectTransform;

	public List<ParticleSystem> shootEffects;

	[Space]
	public Transform hitEffectTransform;

	public List<ParticleSystem> hitEffects;

	[Space]
	public List<ParticleSystem> deathEffects;

	[Space]
	public Sound soundHurt;

	public Sound soundDeath;

	public Sound soundShoot;

	public Sound soundShootGlobal;

	public Sound soundHit;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (!Application.isEditor || (SemiFunc.IsMultiplayer() && !GameManager.instance.localTest))
		{
			debugSpawn = false;
		}
	}

	private void Update()
	{
		VerticalRotationLogic();
		HurtColliderTimer();
		LineRendererLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.5f && (enemy.Rigidbody.timeSinceStun == 0f || enemy.Rigidbody.timeSinceStun > 3f))
		{
			tripTimer += Time.deltaTime;
			if (enemy.Rigidbody.physGrabObject.rbVelocity.y <= -2f)
			{
				tripTimer = 999f;
			}
		}
		else
		{
			tripTimer = 0f;
		}
		if (tripTimer > 0.5f)
		{
			enemy.StateStunned.Set(2f);
			tripTimer = 0f;
		}
		if (enemy.IsStunned())
		{
			UpdateState(State.Stun);
		}
		if (enemy.CurrentState == EnemyState.Despawn && !enemy.IsStunned())
		{
			UpdateState(State.Despawn);
		}
		ShotsFiredLogic();
		HorizontalRotationLogic();
		LeaveInterruptLogic();
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
		case State.InvestigateWalk:
			StateInvestigateWalk();
			break;
		case State.Aim:
			AimLogic();
			StateAim();
			break;
		case State.Shoot:
			StateShoot();
			break;
		case State.ShootEnd:
			StateShootEnd();
			break;
		case State.LeaveStart:
			StateLeaveStart();
			break;
		case State.Leave:
			StateLeave();
			break;
		case State.Despawn:
			StateDespawn();
			break;
		case State.Stun:
			StateStun();
			break;
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
			stateTimer = Random.Range(2f, 8f);
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
				UpdateState(State.LeaveStart);
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
			stateTimer = 1f;
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
			pitCheckTimer = 0.1f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateImpulse = false;
		}
		else
		{
			if (enemy.Rigidbody.notMovingTimer > 1f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (PitCheckLogic())
			{
				enemy.NavMeshAgent.ResetPath();
			}
			if (stateTimer <= 0f || !enemy.NavMeshAgent.HasPath())
			{
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.LeaveStart);
		}
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

	private void StateStun()
	{
		if (!enemy.IsStunned())
		{
			UpdateState(State.Idle);
		}
	}

	private void StateInvestigate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!stateImpulse)
		{
			return;
		}
		stateImpulse = false;
		float num = 12f;
		Vector3 val = investigatePoint - investigateRayTransform.position;
		bool flag = false;
		if (((Vector3)(ref val)).magnitude < num)
		{
			flag = true;
			RaycastHit[] array = Physics.RaycastAll(investigateRayTransform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val2 = array[i];
				if (!(Vector3.Distance(investigatePoint, ((RaycastHit)(ref val2)).point) < 1f) && !((Component)((RaycastHit)(ref val2)).transform).CompareTag("Player") && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val2)).transform).GetComponent<EnemyRigidbody>()))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			UpdateState(State.Aim);
		}
		else
		{
			UpdateState(State.InvestigateWalk);
		}
	}

	private void StateInvestigateWalk()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.SetDestination(investigatePoint);
			pitCheckTimer = 0f;
			enemy.Rigidbody.notMovingTimer = 0f;
			stateTimer = 1f;
			stateImpulse = false;
		}
		else
		{
			if (enemy.Rigidbody.notMovingTimer > 1f)
			{
				stateTimer -= Time.deltaTime;
			}
			if (PitCheckLogic())
			{
				enemy.NavMeshAgent.ResetPath();
			}
			if (stateTimer <= 0f || !enemy.NavMeshAgent.HasPath())
			{
				UpdateState(State.Idle);
			}
		}
		if (SemiFunc.EnemyForceLeave(enemy))
		{
			UpdateState(State.LeaveStart);
		}
	}

	private void StateAim()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			stateImpulse = false;
			investigatePointSpread = Vector3.zero;
			investigatePointSpreadTarget = Vector3.zero;
			investigatePointSpreadTimer = 0f;
			if (shootFast)
			{
				stateTimer = 0.5f;
			}
			else
			{
				stateTimer = Random.Range(0.25f, 1f);
			}
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Shoot);
		}
	}

	private void StateShoot()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			Vector3 val = gunAimTransform.position + gunAimTransform.forward * 50f;
			float num = 1f;
			if (shootFast)
			{
				num = 1.5f;
			}
			if (Vector3.Distance(((Component)this).transform.position, investigatePoint) > 10f)
			{
				num = 0.5f;
			}
			bool flag = false;
			RaycastHit[] array = Physics.SphereCastAll(gunAimTransform.position, num, gunAimTransform.forward, 50f, LayerMask.GetMask(new string[1] { "Player" }) + LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val2 = array[i];
				PlayerAvatar playerAvatar = null;
				bool flag2 = false;
				if (((Component)((RaycastHit)(ref val2)).transform).gameObject.layer == LayerMask.NameToLayer("Player"))
				{
					flag2 = true;
					PlayerController componentInParent = ((Component)((RaycastHit)(ref val2)).transform).GetComponentInParent<PlayerController>();
					playerAvatar = ((!Object.op_Implicit((Object)(object)componentInParent)) ? ((Component)((RaycastHit)(ref val2)).transform).GetComponentInParent<PlayerAvatar>() : componentInParent.playerAvatarScript);
				}
				else
				{
					PlayerTumble componentInParent2 = ((Component)((RaycastHit)(ref val2)).transform).GetComponentInParent<PlayerTumble>();
					if (Object.op_Implicit((Object)(object)componentInParent2))
					{
						playerAvatar = componentInParent2.playerAvatar;
						flag2 = true;
					}
				}
				if (!flag2)
				{
					continue;
				}
				bool flag3 = true;
				if (((RaycastHit)(ref val2)).point != Vector3.zero)
				{
					Vector3 val3 = ((RaycastHit)(ref val2)).point - gunAimTransform.position;
					RaycastHit[] array2 = Physics.RaycastAll(gunAimTransform.position, val3, ((Vector3)(ref val3)).magnitude, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
					for (int j = 0; j < array2.Length; j++)
					{
						RaycastHit val4 = array2[j];
						if (((Component)((RaycastHit)(ref val4)).transform).gameObject.layer != LayerMask.NameToLayer("Player") && (((Component)((RaycastHit)(ref val4)).transform).gameObject.layer != LayerMask.NameToLayer("PhysGrabObject") || !Object.op_Implicit((Object)(object)((Component)this).GetComponentInParent<PlayerTumble>())))
						{
							flag3 = false;
							break;
						}
					}
				}
				if (flag3)
				{
					if (((RaycastHit)(ref val2)).point != Vector3.zero)
					{
						val = ((RaycastHit)(ref val2)).point;
						flag = true;
					}
					else if (Object.op_Implicit((Object)(object)playerAvatar))
					{
						val = playerAvatar.PlayerVisionTarget.VisionTransform.position;
						flag = true;
					}
					break;
				}
			}
			RaycastHit val5 = default(RaycastHit);
			if (!flag && Physics.Raycast(gunAimTransform.position, gunAimTransform.forward, ref val5, 50f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) + LayerMask.GetMask(new string[1] { "Enemy" })))
			{
				val = ((RaycastHit)(ref val5)).point;
			}
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ShootRPC", (RpcTarget)0, new object[1] { val });
			}
			else
			{
				ShootRPC(val);
			}
			stateImpulse = false;
			stateTimer = 2f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.ShootEnd);
		}
	}

	private void StateShootEnd()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			shotsFired++;
			stateImpulse = false;
			stateTimer = 2f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			if (shotsFired >= shotsFiredMax)
			{
				UpdateState(State.LeaveStart);
			}
			else
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateLeaveStart()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			shotsFired++;
			stateImpulse = false;
			stateTimer = 3f;
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Leave);
		}
	}

	private void StateLeave()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			enemy.NavMeshAgent.ResetPath();
			if (!enemy.EnemyParent.playerClose)
			{
				UpdateState(State.Idle);
				return;
			}
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, 25f, 50f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f);
			}
			NavMeshHit val = default(NavMeshHit);
			if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				leavePosition = ((NavMeshHit)(ref val)).position;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			stateImpulse = false;
			stateTimer = 5f;
		}
		enemy.NavMeshAgent.SetDestination(leavePosition);
		if (enemy.Rigidbody.notMovingTimer > 2f)
		{
			stateTimer -= Time.deltaTime;
		}
		if (PitCheckLogic())
		{
			stateImpulse = true;
		}
		else if (stateTimer <= 0f || Vector3.Distance(((Component)this).transform.position, leavePosition) < 1f)
		{
			UpdateState(State.Idle);
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
		soundHurt.Play(enemy.CenterTransform.position);
		if (SemiFunc.IsMasterClientOrSingleplayer() && currentState == State.Leave)
		{
			UpdateState(State.Idle);
		}
	}

	public void OnDeath()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		soundDeath.Play(enemy.CenterTransform.position);
		foreach (ParticleSystem deathEffect in deathEffects)
		{
			deathEffect.Play();
		}
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.05f);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.Despawn();
		}
	}

	public void OnInvestigate()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !(enemy.Rigidbody.timeSinceStun > 1.5f))
		{
			return;
		}
		if (currentState == State.Idle || currentState == State.Roam || currentState == State.InvestigateWalk || (currentState == State.ShootEnd && shotsFired < shotsFiredMax) || (currentState == State.Leave && leaveInterruptCounter >= 3))
		{
			investigatePoint = enemy.StateInvestigate.onInvestigateTriggeredPosition;
			if (Vector3.Distance(investigatePoint, ((Component)this).transform.position) < 4f)
			{
				shootFast = true;
			}
			else
			{
				shootFast = false;
			}
			InvestigateTransformGet();
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateInvestigationPoint", (RpcTarget)1, new object[1] { investigatePoint });
			}
			UpdateState(State.Investigate);
		}
		else if (currentState == State.Leave && Vector3.Distance(((Component)this).transform.position, enemy.StateInvestigate.onInvestigateTriggeredPosition) < 5f)
		{
			leaveInterruptCounter++;
			leaveInterruptTimer = 3f;
		}
	}

	public void OnTouchPlayer()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && enemy.Rigidbody.timeSinceStun > 1.5f && (currentState == State.Idle || currentState == State.Roam || currentState == State.InvestigateWalk || currentState == State.ShootEnd || currentState == State.LeaveStart || currentState == State.Leave))
		{
			shootFast = true;
			investigatePoint = enemy.Rigidbody.onTouchPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
			investigatePointHasTransform = true;
			investigatePointTransform = enemy.Rigidbody.onTouchPlayerAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateInvestigationPoint", (RpcTarget)1, new object[1] { investigatePoint });
			}
			UpdateState(State.Aim);
		}
	}

	public void OnTouchPlayerGrabbedObject()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && enemy.Rigidbody.timeSinceStun > 1.5f && (currentState == State.Idle || currentState == State.Roam || currentState == State.InvestigateWalk || currentState == State.ShootEnd || currentState == State.LeaveStart || currentState == State.Leave))
		{
			shootFast = true;
			investigatePoint = enemy.Rigidbody.onTouchPlayerGrabbedObjectPosition;
			investigatePointHasTransform = true;
			investigatePointTransform = enemy.Rigidbody.onTouchPlayerGrabbedObjectAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateInvestigationPoint", (RpcTarget)1, new object[1] { investigatePoint });
			}
			UpdateState(State.Aim);
		}
	}

	public void OnGrabbed()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && enemy.Rigidbody.timeSinceStun > 1.5f && (currentState == State.Idle || currentState == State.Roam || currentState == State.InvestigateWalk || currentState == State.ShootEnd || currentState == State.LeaveStart || currentState == State.Leave))
		{
			shootFast = true;
			investigatePoint = enemy.Rigidbody.onGrabbedPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
			investigatePointHasTransform = true;
			investigatePointTransform = enemy.Rigidbody.onGrabbedPlayerAvatar.PlayerVisionTarget.VisionTransform;
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateInvestigationPoint", (RpcTarget)1, new object[1] { investigatePoint });
			}
			UpdateState(State.Aim);
		}
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
			else
			{
				UpdateStateRPC(currentState);
			}
		}
	}

	private void AimLogic()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = investigatePoint;
		if (investigatePointHasTransform)
		{
			Vector3 val2 = investigatePointTransform.position - investigatePointTransformPrevious;
			val2.y = 0f;
			val = investigatePointTransform.position + val2 * 25f;
			investigatePointTransformPrevious = investigatePointTransform.position;
		}
		if (investigatePointSpreadTimer <= 0f)
		{
			Vector3 val3 = Random.insideUnitSphere * Random.Range(0f, 0.5f);
			if (Vector3.Distance(((Component)this).transform.position, val) > 10f)
			{
				val3 = Random.insideUnitSphere * Random.Range(0.5f, 1f);
			}
			investigatePointSpreadTimer = Random.Range(0.1f, 0.5f);
			investigatePointSpreadTarget = val3;
		}
		else
		{
			investigatePointSpreadTimer -= Time.deltaTime;
		}
		investigatePointSpread = Vector3.Lerp(investigatePointSpread, investigatePointSpreadTarget, Time.deltaTime * 20f);
		val += investigatePointSpread;
		float num = 5f;
		if (shootFast)
		{
			num = 20f;
		}
		investigatePoint = Vector3.Lerp(investigatePoint, val, num * Time.deltaTime);
		Vector3 position = ((Component)this).transform.position;
		Transform transform = ((Component)this).transform;
		transform.position += gunAimTransform.position - verticalAimTransform.position;
		Quaternion rotation = ((Component)this).transform.rotation;
		((Component)this).transform.LookAt(investigatePoint);
		((Component)this).transform.eulerAngles = new Vector3(0f, ((Component)this).transform.eulerAngles.y, 0f);
		Quaternion rotation2 = ((Component)this).transform.rotation;
		((Component)this).transform.rotation = rotation;
		((Component)this).transform.position = position;
		investigateAimHorizontal = rotation2;
		Vector3 position2 = verticalAimTransform.position;
		Transform obj = verticalAimTransform;
		obj.position += gunAimTransform.position - verticalAimTransform.position;
		verticalAimTransform.LookAt(investigatePoint);
		float num2 = 45f;
		float x = verticalAimTransform.localEulerAngles.x;
		x = ((!(x < 180f)) ? Mathf.Clamp(x, 360f - num2, 360f) : Mathf.Clamp(x, 0f, num2));
		verticalAimTransform.localEulerAngles = new Vector3(x, 0f, 0f);
		Quaternion localRotation = verticalAimTransform.localRotation;
		verticalAimTransform.position = position2;
		investigateAimVertical = localRotation;
		if (!SemiFunc.IsMultiplayer())
		{
			return;
		}
		if (investigateAimVerticalRPCTimer <= 0f)
		{
			if (investigateAimVerticalPrevious != investigateAimVertical)
			{
				investigateAimVerticalRPCTimer = 1f;
				photonView.RPC("UpdateVerticalAimRPC", (RpcTarget)1, new object[1] { investigateAimVertical });
				investigateAimVerticalPrevious = investigateAimVertical;
			}
		}
		else
		{
			investigateAimVerticalRPCTimer -= Time.deltaTime;
		}
	}

	private void HorizontalRotationLogic()
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Idle || currentState == State.Roam || currentState == State.InvestigateWalk || currentState == State.LeaveStart || currentState == State.Leave)
		{
			horizontalAimSpring.damping = 0.7f;
			horizontalAimSpring.speed = 3f;
			if (((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).magnitude > 0.01f)
			{
				Quaternion rotation = ((Component)this).transform.rotation;
				((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized);
				((Component)this).transform.eulerAngles = new Vector3(0f, ((Component)this).transform.eulerAngles.y, 0f);
				Quaternion rotation2 = ((Component)this).transform.rotation;
				((Component)this).transform.rotation = rotation;
				horizontalAimTarget = rotation2;
			}
		}
		else if (currentState == State.Aim)
		{
			if (shootFast)
			{
				horizontalAimSpring.damping = 0.9f;
				horizontalAimSpring.speed = 30f;
			}
			else
			{
				horizontalAimSpring.damping = 0.8f;
				horizontalAimSpring.speed = 20f;
			}
			horizontalAimTarget = investigateAimHorizontal;
		}
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(horizontalAimSpring, horizontalAimTarget);
	}

	private void VerticalRotationLogic()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Aim || currentState == State.Shoot)
		{
			verticalAimTransform.localRotation = SemiFunc.SpringQuaternionGet(verticalAimSpring, investigateAimVertical);
		}
		else
		{
			verticalAimTransform.localRotation = SemiFunc.SpringQuaternionGet(verticalAimSpring, Quaternion.identity);
		}
	}

	private bool PitCheckLogic()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (pitCheckTimer <= 0f)
		{
			Vector3 normalized = ((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized;
			if (((Vector3)(ref normalized)).magnitude > 0.1f)
			{
				pitCheckTimer = 0.5f;
				Vector3 normalized2 = ((Vector3)(ref enemy.NavMeshAgent.AgentVelocity)).normalized;
				normalized2.y = 0f;
				bool num = Physics.Raycast(((Component)this).transform.position + normalized2 + Vector3.up * 1f, Vector3.down, 5f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
				if (!num)
				{
					enemy.NavMeshAgent.Warp(((Component)this).transform.position - normalized2 * 0.5f);
					enemy.NavMeshAgent.ResetPath();
					enemy.NavMeshAgent.Agent.velocity = Vector3.zero;
				}
				return !num;
			}
		}
		pitCheckTimer -= Time.deltaTime;
		return false;
	}

	private void HurtColliderTimer()
	{
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				((Component)hurtCollider).gameObject.SetActive(false);
			}
		}
	}

	private void LineRendererLogic()
	{
		if (lineRendererActive)
		{
			lineRenderer.widthMultiplier = lineRendererWidthCurve.Evaluate(lineRendererLerp);
			lineRendererLerp += Time.deltaTime * 5f;
			if (lineRendererLerp >= 1f)
			{
				((Component)lineRenderer).gameObject.SetActive(false);
				lineRendererActive = false;
			}
		}
	}

	private void ShotsFiredLogic()
	{
		if (currentState == State.Spawn || currentState == State.Leave)
		{
			shotsFired = 0;
		}
	}

	private void InvestigateTransformGet()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		investigatePointHasTransform = false;
		Collider[] array = Physics.OverlapSphere(investigatePoint, 1.5f, LayerMask.GetMask(new string[1] { "Player" }) + LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
		foreach (Collider val in array)
		{
			if (((Component)val).CompareTag("Player"))
			{
				PlayerController componentInParent = ((Component)val).GetComponentInParent<PlayerController>();
				if (Object.op_Implicit((Object)(object)componentInParent))
				{
					investigatePointHasTransform = true;
					investigatePointTransform = componentInParent.playerAvatarScript.PlayerVisionTarget.VisionTransform;
					continue;
				}
				PlayerAvatar componentInParent2 = ((Component)val).GetComponentInParent<PlayerAvatar>();
				if (Object.op_Implicit((Object)(object)componentInParent2))
				{
					investigatePointHasTransform = true;
					investigatePointTransform = componentInParent2.PlayerVisionTarget.VisionTransform;
				}
			}
			else
			{
				PlayerTumble componentInParent3 = ((Component)val).GetComponentInParent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)componentInParent3))
				{
					investigatePointHasTransform = true;
					investigatePointTransform = componentInParent3.playerAvatar.PlayerVisionTarget.VisionTransform;
				}
			}
		}
	}

	private void LeaveInterruptLogic()
	{
		if (currentState == State.Leave)
		{
			if (leaveInterruptTimer <= 0f)
			{
				leaveInterruptCounter = 0;
			}
			else
			{
				leaveInterruptTimer -= Time.deltaTime;
			}
		}
		else
		{
			leaveInterruptTimer = 0f;
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		currentState = _state;
		if (currentState == State.Spawn)
		{
			enemyHunterAnim.OnSpawn();
		}
	}

	[PunRPC]
	private void UpdateVerticalAimRPC(Quaternion _rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		investigateAimVertical = _rotation;
	}

	[PunRPC]
	private void ShootRPC(Vector3 _hitPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _hitPosition - gunTipTransform.position;
		((Component)lineRenderer).gameObject.SetActive(true);
		lineRenderer.SetPosition(0, gunTipTransform.position);
		lineRenderer.SetPosition(1, gunTipTransform.position + ((Vector3)(ref val)).normalized * 0.5f);
		lineRenderer.SetPosition(2, _hitPosition - ((Vector3)(ref val)).normalized * 0.5f);
		lineRenderer.SetPosition(3, _hitPosition);
		lineRendererActive = true;
		lineRendererLerp = 0f;
		((Component)hurtCollider).transform.position = _hitPosition;
		((Component)hurtCollider).transform.rotation = Quaternion.LookRotation(gunTipTransform.forward);
		((Component)hurtCollider).gameObject.SetActive(true);
		hurtColliderTimer = 0.25f;
		shootEffectTransform.position = gunTipTransform.position;
		shootEffectTransform.rotation = gunTipTransform.rotation;
		foreach (ParticleSystem shootEffect in shootEffects)
		{
			shootEffect.Play();
		}
		hitEffectTransform.position = _hitPosition;
		hitEffectTransform.rotation = gunTipTransform.rotation;
		foreach (ParticleSystem hitEffect in hitEffects)
		{
			hitEffect.Play();
		}
		enemyHunterAlwaysActive.Trigger();
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 15f, gunTipTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 15f, gunTipTransform.position, 0.05f);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, _hitPosition, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, _hitPosition, 0.05f);
		soundShoot.Play(gunTipTransform.position);
		soundShootGlobal.Play(gunTipTransform.position);
		soundHit.Play(_hitPosition);
	}

	[PunRPC]
	private void UpdateInvestigationPoint(Vector3 _point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		investigatePoint = _point;
	}
}
