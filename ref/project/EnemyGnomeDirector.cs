using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGnomeDirector : MonoBehaviour
{
	public enum State
	{
		Idle,
		Leave,
		ChangeDestination,
		Investigate,
		AttackSet,
		AttackPlayer,
		AttackValuable
	}

	public static EnemyGnomeDirector instance;

	public bool debugDraw;

	public bool debugOneOnly;

	public bool debugShortIdle;

	public bool debugLongIdle;

	[Space]
	public List<EnemyGnome> gnomes = new List<EnemyGnome>();

	internal List<Vector3> destinations = new List<Vector3>();

	[Space]
	public State currentState = State.ChangeDestination;

	private bool stateImpulse = true;

	private float stateTimer;

	internal bool setup;

	private PlayerAvatar playerTarget;

	private PhysGrabObject valuableTarget;

	internal Vector3 attackPosition;

	internal Vector3 attackVisionPosition;

	private float valuableAttackPositionTimer;

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			if (!Application.isEditor || (SemiFunc.IsMultiplayer() && !GameManager.instance.localTest))
			{
				debugDraw = false;
				debugOneOnly = false;
				debugShortIdle = false;
				debugLongIdle = false;
			}
			((Component)this).transform.parent = LevelGenerator.Instance.EnemyParent.transform;
			((MonoBehaviour)this).StartCoroutine(Setup());
		}
		else
		{
			Object.Destroy((Object)(object)this);
		}
	}

	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			EnemyGnome component = ((Component)item.Enemy).GetComponent<EnemyGnome>();
			if (Object.op_Implicit((Object)(object)component))
			{
				if (debugOneOnly && gnomes.Count > 0)
				{
					Object.Destroy((Object)(object)((Component)component.enemy.EnemyParent).gameObject);
					continue;
				}
				gnomes.Add(component);
				destinations.Add(Vector3.zero);
				component.directorIndex = gnomes.IndexOf(component);
			}
		}
		setup = true;
	}

	private void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!setup)
		{
			return;
		}
		if (debugDraw)
		{
			if (currentState == State.Idle)
			{
				Debug.DrawRay(((Component)this).transform.position, Vector3.up * 2f, Color.green);
				foreach (EnemyGnome gnome in gnomes)
				{
					Debug.DrawRay(destinations[gnomes.IndexOf(gnome)], Vector3.up * 2f, Color.yellow);
				}
			}
			else if (currentState == State.AttackPlayer)
			{
				Debug.DrawRay(attackPosition, Vector3.up * 2f, Color.red);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (currentState)
			{
			case State.Idle:
				StateIdle();
				break;
			case State.Leave:
				StateLeave();
				break;
			case State.ChangeDestination:
				StateChangeDestination();
				break;
			case State.Investigate:
				StateInvestigate();
				break;
			case State.AttackSet:
				StateAttackSet();
				break;
			case State.AttackPlayer:
				StateAttackPlayer();
				break;
			case State.AttackValuable:
				StateAttackValuable();
				break;
			}
		}
	}

	private void StateIdle()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(20f, 30f);
			if (debugShortIdle)
			{
				stateTimer *= 0.5f;
			}
			if (debugLongIdle)
			{
				stateTimer *= 2f;
			}
		}
		if (!SemiFunc.EnemySpawnIdlePause())
		{
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.ChangeDestination);
			}
			LeaveCheck();
		}
	}

	private void StateLeave()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f);
			if (Object.op_Implicit((Object)(object)levelPoint))
			{
				flag = SetPosition(((Component)levelPoint).transform.position);
			}
			if (flag)
			{
				stateImpulse = false;
				UpdateState(State.Idle);
			}
		}
	}

	private void StateChangeDestination()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			bool flag = false;
			LevelPoint levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 10f, 25f);
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGet(((Component)this).transform.position, 0f, 999f);
			}
			if (Object.op_Implicit((Object)(object)levelPoint))
			{
				flag = SetPosition(((Component)levelPoint).transform.position);
			}
			if (flag)
			{
				stateImpulse = false;
				UpdateState(State.Idle);
			}
		}
	}

	private void StateInvestigate()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			UpdateState(State.Idle);
		}
	}

	private void StateAttackSet()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!stateImpulse)
		{
			return;
		}
		stateImpulse = false;
		valuableTarget = null;
		float num = 0f;
		Collider[] array = Physics.OverlapSphere(((Component)playerTarget).transform.position, 3f, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
		for (int i = 0; i < array.Length; i++)
		{
			ValuableObject componentInParent = ((Component)array[i]).GetComponentInParent<ValuableObject>();
			if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.dollarValueCurrent > num)
			{
				num = componentInParent.dollarValueCurrent;
				valuableTarget = componentInParent.physGrabObject;
			}
		}
		if (Object.op_Implicit((Object)(object)valuableTarget))
		{
			UpdateState(State.AttackValuable);
		}
		else
		{
			UpdateState(State.AttackPlayer);
		}
	}

	private void StateAttackPlayer()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
		}
		PauseGnomeSpawnedTimers();
		if (Object.op_Implicit((Object)(object)playerTarget) && !playerTarget.isDisabled)
		{
			if (stateTimer > 0.5f)
			{
				attackPosition = ((Component)playerTarget).transform.position;
				attackVisionPosition = playerTarget.PlayerVisionTarget.VisionTransform.position;
			}
		}
		else
		{
			stateTimer = 0f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			SetPosition(attackPosition);
			UpdateState(State.Idle);
		}
	}

	private void StateAttackValuable()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 10f;
			valuableAttackPositionTimer = 0f;
		}
		PauseGnomeSpawnedTimers();
		if (Object.op_Implicit((Object)(object)valuableTarget))
		{
			if (valuableAttackPositionTimer <= 0f)
			{
				valuableAttackPositionTimer = 0.2f;
				attackPosition = valuableTarget.centerPoint;
				RaycastHit val = default(RaycastHit);
				if (Physics.Raycast(valuableTarget.centerPoint, Vector3.down, ref val, 2f, LayerMask.GetMask(new string[1] { "Default" })))
				{
					attackPosition = ((RaycastHit)(ref val)).point;
				}
			}
			else
			{
				valuableAttackPositionTimer -= Time.deltaTime;
			}
			attackVisionPosition = valuableTarget.centerPoint;
		}
		else
		{
			stateTimer = 0f;
		}
		bool flag = false;
		foreach (EnemyGnome gnome in gnomes)
		{
			if (Vector3.Distance(((Component)gnome.enemy.Rigidbody).transform.position, attackPosition) <= 1f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			stateTimer -= Time.deltaTime;
		}
		bool flag2 = true;
		foreach (EnemyGnome gnome2 in gnomes)
		{
			if (((Behaviour)gnome2).isActiveAndEnabled)
			{
				flag2 = false;
				break;
			}
		}
		if (stateTimer <= 0f || flag2)
		{
			SetPosition(attackPosition);
			UpdateState(State.Idle);
		}
	}

	private void UpdateState(State _state)
	{
		currentState = _state;
		stateImpulse = true;
		stateTimer = 0f;
	}

	private bool SetPosition(Vector3 _initialPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(_initialPosition, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })) && !SemiFunc.EnemyPhysObjectSphereCheck(((NavMeshHit)(ref val)).position, 1f))
		{
			((Component)this).transform.position = ((NavMeshHit)(ref val)).position;
			((Component)this).transform.rotation = Quaternion.identity;
			float num = 360f / (float)gnomes.Count;
			NavMeshHit val3 = default(NavMeshHit);
			foreach (EnemyGnome gnome in gnomes)
			{
				float num2 = 0f;
				Vector3 value = ((Component)this).transform.position;
				Vector3 val2 = ((Component)this).transform.position;
				for (; num2 < 2f; num2 += 0.1f)
				{
					value = val2;
					val2 = ((NavMeshHit)(ref val)).position + ((Component)this).transform.forward * num2;
					if (!NavMesh.SamplePosition(val2, ref val3, 5f, -1) || !Physics.Raycast(val2, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
					{
						break;
					}
					Vector3 val4 = val2 + Vector3.up * 0.5f - (((NavMeshHit)(ref val)).position + Vector3.up * 0.5f);
					Vector3 normalized = ((Vector3)(ref val4)).normalized;
					if (Physics.Raycast(val2 + Vector3.up * 0.5f, normalized, ((Vector3)(ref normalized)).magnitude, LayerMask.GetMask(new string[2] { "Default", "PhysGrabObjectHinge" })) || (num2 > 0.5f && Random.Range(0, 100) < 15))
					{
						break;
					}
				}
				destinations[gnomes.IndexOf(gnome)] = value;
				Transform transform = ((Component)this).transform;
				Quaternion rotation = ((Component)this).transform.rotation;
				transform.rotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y + num, 0f);
			}
			return true;
		}
		return false;
	}

	public void Investigate(Vector3 _position)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.Investigate && currentState != State.AttackSet && currentState != State.AttackPlayer && currentState != State.AttackValuable)
		{
			SetPosition(_position);
			UpdateState(State.Investigate);
		}
	}

	public void SetTarget(PlayerAvatar _player)
	{
		if (currentState != State.AttackSet && currentState != State.AttackPlayer && currentState != State.AttackValuable)
		{
			playerTarget = _player;
			UpdateState(State.AttackSet);
		}
		else if (currentState == State.AttackPlayer && (Object)(object)playerTarget == (Object)(object)_player)
		{
			stateTimer = 2f;
		}
	}

	public void SeeTarget()
	{
		if (currentState == State.AttackPlayer)
		{
			stateTimer = 1f;
		}
	}

	public bool CanAttack(EnemyGnome _gnome)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (_gnome.attackCooldown > 0f || _gnome.enemy.Jump.jumping)
		{
			return false;
		}
		if (currentState == State.AttackPlayer)
		{
			if (Vector3.Distance(((Component)_gnome.enemy.Rigidbody).transform.position, attackPosition) <= 0.7f)
			{
				return true;
			}
		}
		else if (Object.op_Implicit((Object)(object)valuableTarget))
		{
			_gnome.overlapCheckCooldown = 1f;
			if (_gnome.overlapCheckTimer <= 0f)
			{
				_gnome.overlapCheckTimer = 0.5f;
				_gnome.overlapCheckPrevious = false;
				Collider[] array = Physics.OverlapSphere(((Component)_gnome.enemy.Rigidbody).transform.position, 0.7f, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
				for (int i = 0; i < array.Length; i++)
				{
					ValuableObject componentInParent = ((Component)array[i]).GetComponentInParent<ValuableObject>();
					if (Object.op_Implicit((Object)(object)componentInParent) && (Object)(object)componentInParent.physGrabObject == (Object)(object)valuableTarget)
					{
						_gnome.overlapCheckPrevious = true;
					}
				}
			}
			return _gnome.overlapCheckPrevious;
		}
		return false;
	}

	private void PauseGnomeSpawnedTimers()
	{
		foreach (EnemyGnome gnome in gnomes)
		{
			gnome.enemy.EnemyParent.SpawnedTimerPause(0.1f);
		}
	}

	private void LeaveCheck()
	{
		bool flag = false;
		foreach (EnemyGnome gnome in gnomes)
		{
			if (SemiFunc.EnemyForceLeave(gnome.enemy))
			{
				flag = true;
			}
		}
		if (flag)
		{
			UpdateState(State.Leave);
		}
	}

	public void OnSpawn()
	{
		foreach (EnemyGnome gnome in gnomes)
		{
			gnome.enemy.EnemyParent.DespawnedTimerSet(gnome.enemy.EnemyParent.DespawnedTimer - 30f);
		}
	}
}
