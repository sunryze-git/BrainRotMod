using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyThinMan : MonoBehaviour
{
	public enum State
	{
		Stand,
		OnScreen,
		Notice,
		Attack,
		TentacleExtend,
		Damage,
		Despawn,
		Stunned
	}

	private PhotonView photonView;

	private Enemy enemy;

	public EnemyThinManAnim anim;

	public GameObject tentacleR1;

	public GameObject tentacleR2;

	public GameObject tentacleR3;

	public GameObject tentacleL1;

	public GameObject tentacleL2;

	public GameObject tentacleL3;

	public GameObject extendedTentacles;

	public GameObject head;

	public GameObject hurtCollider;

	private float hurtColliderTimer;

	public float tentacleLerp;

	public State currentState;

	private float stateTimer;

	private bool stateImpulse;

	private float tpTimer;

	public Rigidbody rb;

	internal PlayerAvatar playerTarget;

	private bool otherEnemyFetch = true;

	public List<EnemyThinMan> otherEnemies;

	private Vector3 teleportPosition;

	private Vector3 lastTeleportPosition;

	private float teleportTimer;

	private bool teleporting;

	private float teleportRoamTimer;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			switch (currentState)
			{
			case State.Stand:
				StateStand();
				PlayerLookAt();
				break;
			case State.OnScreen:
				StateOnScreen();
				PlayerLookAt();
				break;
			case State.Notice:
				StateNotice();
				PlayerLookAt();
				break;
			case State.Attack:
				StateAttack();
				PlayerLookAt();
				break;
			case State.TentacleExtend:
				StateTentacleExtend();
				PlayerLookAt();
				break;
			case State.Damage:
				StateDamage();
				PlayerLookAt();
				break;
			case State.Despawn:
				StateDespawn();
				break;
			case State.Stunned:
				StateStunned();
				break;
			}
			if (enemy.IsStunned())
			{
				enemy.EnemyParent.SpawnedTimerSet(0f);
				UpdateState(State.Stunned);
			}
		}
		TeleportLogic();
		SetFollowTargetToPosition();
		TentacleLogic();
		LocalEffect();
		HurtColliderLogic();
	}

	private void StateStand()
	{
		if (stateImpulse)
		{
			SetTarget(null);
			stateTimer = 5f;
			stateImpulse = false;
		}
		if (!Object.op_Implicit((Object)(object)playerTarget))
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (!player.isDisabled && enemy.OnScreen.GetOnScreen(player))
				{
					SetTarget(player);
					UpdateState(State.OnScreen);
					return;
				}
			}
		}
		if (!SemiFunc.EnemySpawnIdlePause())
		{
			if (teleportRoamTimer > 0f)
			{
				teleportRoamTimer -= Time.deltaTime;
			}
			else if (Teleport(_spawn: false))
			{
				SetRoamTimer();
			}
			if (SemiFunc.EnemyForceLeave(enemy))
			{
				Teleport(_spawn: false, _leave: true);
			}
		}
	}

	private void StateOnScreen()
	{
		if (stateImpulse)
		{
			tpTimer = Random.Range(0f, 5f);
			stateTimer = 1f;
			stateImpulse = false;
		}
		bool flag = false;
		if (enemy.OnScreen.GetOnScreen(playerTarget))
		{
			stateTimer = 0.2f;
			flag = true;
		}
		if (tpTimer > 0f)
		{
			tpTimer -= Time.deltaTime;
		}
		if (flag)
		{
			if (!(tentacleLerp < 1f))
			{
				UpdateState(State.Notice);
				return;
			}
			if (tentacleLerp > 0.05f && tentacleLerp < 0.15f && tpTimer <= 0f)
			{
				if (Random.Range(0f, 1f) < 0.5f)
				{
					if (Teleport(_spawn: false))
					{
						tpTimer = 5f;
					}
				}
				else
				{
					tpTimer = 5f;
				}
			}
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Stand);
		}
	}

	private void StateNotice()
	{
		if (!GameManager.Multiplayer())
		{
			NoticeRPC();
		}
		else
		{
			photonView.RPC("NoticeRPC", (RpcTarget)0, Array.Empty<object>());
		}
		UpdateState(State.Attack);
	}

	private void StateAttack()
	{
		if (stateImpulse)
		{
			stateTimer = 1f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.TentacleExtend);
		}
	}

	private void StateTentacleExtend()
	{
		if (stateImpulse)
		{
			stateTimer = 0.1f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			UpdateState(State.Damage);
		}
	}

	private void StateDamage()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		playerTarget.playerHealth.HurtOther(30, ((Component)playerTarget).transform.position, savingGrace: false, SemiFunc.EnemyGetIndex(enemy));
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("ActivateHurtColliderRPC", (RpcTarget)0, new object[1] { ((Component)playerTarget).transform.position });
		}
		else
		{
			ActivateHurtColliderRPC(((Component)playerTarget).transform.position);
		}
		UpdateState(State.Despawn);
	}

	private void StateDespawn()
	{
		if (stateImpulse)
		{
			stateTimer = 0.4f;
			stateImpulse = false;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	private void StateStunned()
	{
	}

	public void OnSpawn()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (otherEnemyFetch)
		{
			otherEnemyFetch = false;
			foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
			{
				EnemyThinMan componentInChildren = ((Component)item).GetComponentInChildren<EnemyThinMan>(true);
				if (Object.op_Implicit((Object)(object)componentInChildren) && (Object)(object)componentInChildren != (Object)(object)this)
				{
					otherEnemies.Add(componentInChildren);
				}
			}
		}
		if (SemiFunc.EnemySpawnIdlePause())
		{
			lastTeleportPosition = ((Component)this).transform.position;
			SemiFunc.EnemySpawn(enemy);
			teleportPosition = ((Component)this).transform.position;
			teleporting = true;
			UpdateState(State.Stand);
		}
		else if (Teleport(_spawn: true))
		{
			SetRoamTimer();
			UpdateState(State.Stand);
		}
		else
		{
			enemy.EnemyParent.Despawn();
			enemy.EnemyParent.DespawnedTimerSet(3f);
		}
	}

	public void OnHurt()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		anim.hurtSound.Play(((Component)anim).transform.position);
	}

	private void UpdateState(State _nextState)
	{
		stateTimer = 0f;
		stateImpulse = true;
		currentState = _nextState;
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateStateRPC", (RpcTarget)1, new object[1] { _nextState });
		}
	}

	private bool Teleport(bool _spawn, bool _leave = false)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		List<LevelPoint> list = new List<LevelPoint>();
		if (_leave)
		{
			list.Add(SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f));
		}
		else
		{
			list = SemiFunc.LevelPointGetWithinDistance(((Component)this).transform.position, 3f, 30f);
			if (list == null)
			{
				list = SemiFunc.LevelPointGetWithinDistance(((Component)this).transform.position, 3f, 50f);
				if (list == null)
				{
					list = SemiFunc.LevelPointGetWithinDistance(((Component)this).transform.position, 0f, 999f);
				}
			}
		}
		if (list == null)
		{
			return false;
		}
		bool flag = Random.Range(0, 100) < 3;
		if (Object.op_Implicit((Object)(object)playerTarget))
		{
			flag = Random.Range(0, 100) < 30;
		}
		if (flag && !_leave)
		{
			list = SemiFunc.LevelPointsGetAllCloseToPlayers();
		}
		if (list == null || list.Count <= 0)
		{
			return false;
		}
		LevelPoint levelPoint = list[Random.Range(0, list.Count)];
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)levelPoint).transform.position + Vector3.up * 0.1f, Vector3.up, ref val, 3.5f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			return false;
		}
		foreach (EnemyThinMan otherEnemy in otherEnemies)
		{
			if (((Behaviour)otherEnemy).isActiveAndEnabled && Vector3.Distance(otherEnemy.rb.position, ((Component)levelPoint).transform.position) <= 2f)
			{
				return false;
			}
		}
		if (Vector3.Distance(((Component)levelPoint).transform.position, lastTeleportPosition) < 1f)
		{
			return false;
		}
		if (SemiFunc.EnemyPhysObjectBoundingBoxCheck(((Component)this).transform.position, ((Component)levelPoint).transform.position, enemy.Rigidbody.rb))
		{
			return false;
		}
		lastTeleportPosition = ((Component)this).transform.position;
		teleportPosition = ((Component)levelPoint).transform.position;
		teleporting = true;
		if (!_spawn)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("TeleportEffectRPC", (RpcTarget)0, new object[2] { lastTeleportPosition, true });
			}
			else
			{
				TeleportEffectRPC(lastTeleportPosition, intro: true);
			}
		}
		else
		{
			enemy.EnemyTeleported(teleportPosition);
		}
		return true;
	}

	private void TentacleLogic()
	{
		if (currentState == State.OnScreen)
		{
			tentacleLerp += anim.tentacleSpeed * Time.deltaTime;
		}
		else if (currentState == State.Attack || currentState == State.TentacleExtend)
		{
			if (currentState == State.TentacleExtend)
			{
				tentacleLerp -= 10f * Time.deltaTime;
			}
		}
		else if (currentState == State.Stunned)
		{
			tentacleLerp -= 0.4f * Time.deltaTime;
		}
		else
		{
			tentacleLerp -= anim.tentacleSpeed * 0.5f * Time.deltaTime;
		}
		tentacleLerp = Mathf.Clamp(tentacleLerp, 0f, 1f);
	}

	private void TeleportLogic()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (teleportTimer <= 0f)
		{
			if (teleporting)
			{
				enemy.EnemyTeleported(teleportPosition);
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("TeleportEffectRPC", (RpcTarget)0, new object[2] { teleportPosition, false });
				}
				else
				{
					TeleportEffectRPC(teleportPosition, intro: false);
				}
				teleporting = false;
			}
		}
		else
		{
			teleportTimer -= Time.deltaTime;
		}
	}

	private void PlayerLookAt()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerTarget))
		{
			Quaternion val = Quaternion.LookRotation(playerTarget.PlayerVisionTarget.VisionTransform.position - ((Component)enemy.Rigidbody).transform.position);
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, 50f * Time.deltaTime);
		}
	}

	private void SetFollowTargetToPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Component)enemy).transform.position = teleportPosition;
	}

	public void SmokeEffect(Vector3 pos)
	{
		anim.particleSmokeCalmFill.Play();
	}

	private void Rattle()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		anim.notice.Play(((Component)this).transform.position);
		anim.rattleImpulse = true;
	}

	private void LocalEffect()
	{
		if (currentState == State.OnScreen && Object.op_Implicit((Object)(object)playerTarget) && playerTarget.isLocal)
		{
			SemiFunc.DoNotLookEffect(((Component)this).gameObject, _vignette: true, _zoom: true, _saturation: true, _contrast: true, _shake: true, _glitch: false);
		}
	}

	private void SetRoamTimer()
	{
		teleportRoamTimer = Random.Range(8f, 22f);
	}

	private void HurtColliderLogic()
	{
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				hurtCollider.SetActive(false);
			}
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _nextState)
	{
		currentState = _nextState;
	}

	[PunRPC]
	private void NoticeRPC()
	{
		anim.NoticeSet();
	}

	[PunRPC]
	public void TeleportEffectRPC(Vector3 position, bool intro)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		SmokeEffect(position);
		if (intro)
		{
			anim.teleportIn.Play(((Component)this).transform.position);
		}
		else
		{
			anim.teleportOut.Play(((Component)this).transform.position);
		}
		anim.rattleImpulse = true;
		teleportTimer = 0.1f;
	}

	private void SetTarget(PlayerAvatar _player)
	{
		if (!((Object)(object)_player == (Object)(object)playerTarget))
		{
			playerTarget = _player;
			bool flag = true;
			int num = -1;
			if (!Object.op_Implicit((Object)(object)playerTarget))
			{
				flag = false;
			}
			if (flag)
			{
				Rattle();
				num = playerTarget.photonView.ViewID;
			}
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("SetTargetRPC", (RpcTarget)1, new object[2] { num, flag });
			}
		}
	}

	[PunRPC]
	public void SetTargetRPC(int playerID, bool hasTarget)
	{
		if (!hasTarget)
		{
			playerTarget = null;
			return;
		}
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			if (item.photonView.ViewID == playerID)
			{
				playerTarget = item;
				break;
			}
		}
		Rattle();
	}

	[PunRPC]
	public void ActivateHurtColliderRPC(Vector3 _position)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		hurtCollider.transform.position = _position;
		hurtCollider.transform.rotation = Quaternion.LookRotation(enemy.Vision.VisionTransform.position - _position);
		hurtCollider.SetActive(true);
		hurtColliderTimer = 0.25f;
	}
}
