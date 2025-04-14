using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBangDirector : MonoBehaviour, IPunObservable
{
	public enum State
	{
		Idle,
		Leave,
		ChangeDestination,
		Investigate,
		AttackSet,
		AttackPlayer,
		AttackCart
	}

	public static EnemyBangDirector instance;

	public bool debugDraw;

	public bool debugOneOnly;

	public bool debugShortIdle;

	public bool debugLongIdle;

	public bool debugNoFuseProgress;

	[Space]
	public List<EnemyBang> units = new List<EnemyBang>();

	internal List<Vector3> destinations = new List<Vector3>();

	[Space]
	public State currentState = State.ChangeDestination;

	private bool stateImpulse = true;

	private float stateTimer;

	internal bool setup;

	internal PlayerAvatar playerTarget;

	internal bool playerTargetCrawling;

	internal Vector3 attackPosition;

	internal Vector3 attackVisionPosition;

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
				debugNoFuseProgress = false;
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
		int num = -1;
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			EnemyBang component = ((Component)item.Enemy).GetComponent<EnemyBang>();
			if (!Object.op_Implicit((Object)(object)component))
			{
				continue;
			}
			if (debugOneOnly && units.Count > 0)
			{
				Object.Destroy((Object)(object)((Component)component.enemy.EnemyParent).gameObject);
				continue;
			}
			units.Add(component);
			destinations.Add(Vector3.zero);
			component.directorIndex = units.IndexOf(component);
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (num == -1)
				{
					num = Random.Range(0, component.headObjects.Length);
				}
				if (SemiFunc.IsMultiplayer())
				{
					component.photonView.RPC("SetHeadRPC", (RpcTarget)0, new object[1] { num });
				}
				else
				{
					component.SetHeadRPC(num);
				}
				num++;
				if (num >= component.headObjects.Length)
				{
					num = 0;
				}
				float num2 = Random.Range(0.8f, 1.25f);
				if (SemiFunc.IsMultiplayer())
				{
					component.photonView.RPC("SetVoicePitchRPC", (RpcTarget)0, new object[1] { num2 });
				}
				else
				{
					component.SetVoicePitchRPC(num2);
				}
			}
		}
		foreach (EnemyBang unit in units)
		{
			EnemyBangFuse[] componentsInChildren = ((Component)unit.enemy.EnemyParent).GetComponentsInChildren<EnemyBangFuse>(true);
			foreach (EnemyBangFuse obj in componentsInChildren)
			{
				obj.controller = unit;
				obj.particleParent.parent = unit.particleParent;
				obj.setup = true;
			}
		}
		setup = true;
	}

	private void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!setup)
		{
			return;
		}
		if (debugDraw)
		{
			Debug.DrawRay(((Component)this).transform.position, Vector3.up * 2f, Color.green);
			foreach (EnemyBang unit in units)
			{
				Debug.DrawRay(destinations[units.IndexOf(unit)], Vector3.up * 2f, Color.yellow);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			switch (currentState)
			{
			case State.Idle:
				StateIdle();
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
			case State.AttackCart:
				StateAttackCart();
				break;
			case State.Leave:
				StateLeave();
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
		if (stateImpulse)
		{
			stateImpulse = false;
			UpdateState(State.AttackPlayer);
		}
	}

	private void StateAttackPlayer()
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
		}
		PauseSpawnedTimers();
		if (Object.op_Implicit((Object)(object)playerTarget) && !playerTarget.isDisabled)
		{
			playerTargetCrawling = playerTarget.isCrawling;
			if (stateTimer > 0.5f)
			{
				attackPosition = ((Component)playerTarget).transform.position;
				attackVisionPosition = playerTarget.PlayerVisionTarget.VisionTransform.position;
				if (!playerTargetCrawling)
				{
					attackVisionPosition += Vector3.up * 0.25f;
				}
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

	private void StateAttackCart()
	{
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
			float num = 360f / (float)units.Count;
			NavMeshHit val3 = default(NavMeshHit);
			foreach (EnemyBang unit in units)
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
				destinations[units.IndexOf(unit)] = value;
				Transform transform = ((Component)this).transform;
				Quaternion rotation = ((Component)this).transform.rotation;
				transform.rotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y + num, 0f);
			}
			return true;
		}
		return false;
	}

	private void LeaveCheck()
	{
		bool flag = false;
		foreach (EnemyBang unit in units)
		{
			if (SemiFunc.EnemyForceLeave(unit.enemy))
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
		foreach (EnemyBang unit in units)
		{
			unit.enemy.EnemyParent.DespawnedTimerSet(unit.enemy.EnemyParent.DespawnedTimer - 30f);
		}
	}

	public void Investigate(Vector3 _position)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != State.Investigate)
		{
			SetPosition(_position);
			UpdateState(State.Investigate);
		}
	}

	public void SetTarget(PlayerAvatar _player)
	{
		if (currentState != State.AttackSet && currentState != State.AttackPlayer && currentState != State.AttackCart)
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

	public void TriggerNearby(Vector3 _position)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		foreach (EnemyBang unit in units)
		{
			if (Vector3.Distance(((Component)unit).transform.position, _position) < 2f)
			{
				unit.OnVision();
			}
		}
	}

	private void PauseSpawnedTimers()
	{
		foreach (EnemyBang unit in units)
		{
			unit.enemy.EnemyParent.SpawnedTimerPause(0.1f);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)attackVisionPosition);
		}
		else
		{
			attackVisionPosition = (Vector3)stream.ReceiveNext();
		}
	}
}
