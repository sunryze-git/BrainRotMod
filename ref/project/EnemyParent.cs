using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class EnemyParent : MonoBehaviourPunCallbacks, IPunObservable
{
	public enum Difficulty
	{
		Difficulty1,
		Difficulty2,
		Difficulty3
	}

	public string enemyName = "Dinosaur";

	internal bool SetupDone;

	internal bool Spawned = true;

	internal Enemy Enemy;

	[Space]
	public Difficulty difficulty;

	[Space]
	public float actionMultiplier = 1f;

	[Space]
	public GameObject EnableObject;

	[Space]
	public float SpawnedTimeMin;

	public float SpawnedTimeMax;

	[Space]
	public float DespawnedTimeMin;

	public float DespawnedTimeMax;

	[Space]
	public float SpawnedTimer;

	public float DespawnedTimer;

	private float spawnedTimerPauseTimer;

	private float valuableSpawnTimer;

	internal bool playerClose;

	internal bool playerVeryClose;

	internal bool forceLeave;

	internal List<RoomVolume> currentRooms = new List<RoomVolume>();

	internal LevelPoint firstSpawnPoint;

	internal bool firstSpawnPointUsed;

	private void Awake()
	{
		((Component)this).transform.parent = LevelGenerator.Instance.EnemyParent.transform;
		Enemy = ((Component)this).GetComponentInChildren<Enemy>();
		if (EnemyDirector.instance.debugEnemy != null)
		{
			if (EnemyDirector.instance.debugEnemyEnableTime > 0f)
			{
				SpawnedTimeMax = EnemyDirector.instance.debugEnemyEnableTime;
				SpawnedTimeMin = SpawnedTimeMax;
			}
			if (EnemyDirector.instance.debugEnemyDisableTime > 0f)
			{
				DespawnedTimeMax = EnemyDirector.instance.debugEnemyDisableTime;
				DespawnedTimeMin = DespawnedTimeMax;
			}
		}
		((MonoBehaviour)this).StartCoroutine(Setup());
	}

	private void Update()
	{
		if (SemiFunc.FPSImpulse1())
		{
			GetRoomVolume();
		}
	}

	private IEnumerator Setup()
	{
		while (!SetupDone)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		LevelGenerator.Instance.EnemiesSpawned++;
		EnemyDirector.instance.enemiesSpawned.Add(this);
		if (LevelGenerator.Instance.EnemiesSpawned >= LevelGenerator.Instance.EnemiesSpawnTarget)
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
				{
					item.Enemy.PlayerAdded(player.photonView.ViewID);
				}
			}
			if (GameManager.Multiplayer())
			{
				LevelGenerator.Instance.PhotonView.RPC("EnemyReadyRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (Enemy.HasRigidbody)
			{
				float num = ((Component)Enemy.Rigidbody).transform.localPosition.y - ((Component)Enemy).transform.localPosition.y;
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(0f, num, 0f);
				Vector3 position = ((Component)Enemy).transform.position + val;
				Enemy.Rigidbody.rb.position = position;
				Enemy.Rigidbody.rb.rotation = Enemy.Rigidbody.followTarget.rotation;
				Enemy.Rigidbody.physGrabObject.Teleport(position, Enemy.Rigidbody.followTarget.rotation);
				Enemy.Rigidbody.physGrabObject.spawned = true;
				Enemy.Rigidbody.rb.isKinematic = false;
			}
			((MonoBehaviour)this).StartCoroutine(Logic());
			((MonoBehaviour)this).StartCoroutine(PlayerCloseLogic());
		}
	}

	private IEnumerator Logic()
	{
		Despawn();
		DespawnedTimer = Random.Range(2f, 5f);
		while (true)
		{
			if (Spawned)
			{
				if (SpawnedTimer <= 0f)
				{
					if (!playerClose || EnemyDirector.instance.debugDespawnClose)
					{
						Enemy.CurrentState = EnemyState.Despawn;
					}
				}
				else if (spawnedTimerPauseTimer > 0f)
				{
					spawnedTimerPauseTimer -= Time.deltaTime;
				}
				else if (!playerClose || EnemyDirector.instance.debugDespawnClose)
				{
					SpawnedTimer -= Time.deltaTime;
				}
			}
			else if (DespawnedTimer <= 0f)
			{
				Spawn();
			}
			else
			{
				DespawnedTimer -= Time.deltaTime;
			}
			yield return null;
		}
	}

	private IEnumerator PlayerCloseLogic()
	{
		Vector3 val2 = default(Vector3);
		while (true)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
			{
				if (!item.isDisabled)
				{
					Vector3 val = new Vector3(((Component)item).transform.position.x, 0f, ((Component)item).transform.position.z);
					((Vector3)(ref val2))._002Ector(((Component)Enemy).transform.position.x, 0f, ((Component)Enemy).transform.position.z);
					float num = Vector3.Distance(val, val2);
					if (num <= 6f)
					{
						flag2 = true;
						flag = true;
						break;
					}
					if (num <= 20f)
					{
						EnemyDirector.instance.spawnIdlePauseTimer = 0f;
						flag = true;
					}
				}
			}
			playerClose = flag;
			playerVeryClose = flag2;
			if (flag)
			{
				valuableSpawnTimer = 10f;
			}
			else if (valuableSpawnTimer > 0f)
			{
				valuableSpawnTimer -= 1f;
			}
			yield return (object)new WaitForSeconds(1f);
		}
	}

	public void DisableDecrease(float _time)
	{
		DespawnedTimer -= _time;
	}

	public void SpawnedTimerSet(float _time)
	{
		if (Spawned)
		{
			SpawnedTimer = _time;
			if (_time == 0f)
			{
				Enemy.CurrentState = EnemyState.Despawn;
			}
		}
	}

	public void DespawnedTimerSet(float _time, bool _min = false)
	{
		if (!Spawned)
		{
			if (!_min)
			{
				DespawnedTimer = _time;
			}
			else
			{
				DespawnedTimer = Mathf.Min(DespawnedTimer, _time);
			}
		}
	}

	public void SpawnedTimerReset()
	{
		if (Spawned)
		{
			SpawnedTimer = Random.Range(SpawnedTimeMin, SpawnedTimeMax);
			if (Enemy.CurrentState == EnemyState.Despawn)
			{
				Enemy.CurrentState = EnemyState.Roaming;
			}
		}
	}

	public void SpawnedTimerPause(float _time)
	{
		spawnedTimerPauseTimer = Mathf.Max(spawnedTimerPauseTimer, _time);
	}

	public void GetRoomVolume()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		currentRooms.Clear();
		Collider[] array = Physics.OverlapBox(Enemy.CenterTransform.position, Vector3.one / 2f, ((Component)this).transform.rotation, LayerMask.GetMask(new string[1] { "RoomVolume" }));
		foreach (Collider val in array)
		{
			RoomVolume roomVolume = ((Component)((Component)val).transform).GetComponent<RoomVolume>();
			if (!Object.op_Implicit((Object)(object)roomVolume))
			{
				roomVolume = ((Component)((Component)val).transform).GetComponentInParent<RoomVolume>();
			}
			if (!currentRooms.Contains(roomVolume))
			{
				currentRooms.Add(roomVolume);
			}
		}
	}

	private void Spawn()
	{
		SpawnedTimer = Random.Range(SpawnedTimeMin, SpawnedTimeMax);
		Enemy.CurrentState = EnemyState.Spawn;
		if (GameManager.Multiplayer())
		{
			((MonoBehaviourPun)this).photonView.RPC("SpawnRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			SpawnRPC();
		}
	}

	[PunRPC]
	private void SpawnRPC()
	{
		if (Enemy.HasHealth)
		{
			Enemy.Health.OnSpawn();
		}
		if (Enemy.HasStateStunned)
		{
			Enemy.StateStunned.Spawn();
		}
		if (Enemy.HasJump)
		{
			Enemy.Jump.StuckReset();
		}
		Enemy.StuckCount = 0;
		Spawned = true;
		EnableObject.SetActive(true);
		Enemy.StateSpawn.OnSpawn.Invoke();
		Enemy.Spawn();
		if (!EnemyDirector.instance.debugNoSpawnedPause)
		{
			SpawnedTimerPause(Random.Range(3f, 4f) * 60f);
		}
		forceLeave = false;
	}

	public void Despawn()
	{
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Enemy.CurrentState = EnemyState.Despawn;
		DespawnedTimer = Random.Range(DespawnedTimeMin, DespawnedTimeMax) * EnemyDirector.instance.despawnedTimeMultiplier;
		DespawnedTimer = Mathf.Max(DespawnedTimer, 1f);
		if (Enemy.HasRigidbody)
		{
			Enemy.Rigidbody.grabbed = false;
			Enemy.Rigidbody.grabStrengthTimer = 0f;
			Enemy.Rigidbody.GrabRelease();
		}
		if (GameManager.Multiplayer())
		{
			((MonoBehaviourPun)this).photonView.RPC("DespawnRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			DespawnRPC();
		}
		if (!Enemy.HasHealth || !Enemy.Health.spawnValuable || Enemy.Health.healthCurrent > 0)
		{
			return;
		}
		if (valuableSpawnTimer > 0f && Enemy.Health.spawnValuableCurrent < Enemy.Health.spawnValuableMax)
		{
			GameObject val = AssetManager.instance.enemyValuableSmall;
			if (difficulty == Difficulty.Difficulty2)
			{
				val = AssetManager.instance.enemyValuableMedium;
			}
			else if (difficulty == Difficulty.Difficulty3)
			{
				val = AssetManager.instance.enemyValuableBig;
			}
			Transform val2 = Enemy.CustomValuableSpawnTransform;
			if (!Object.op_Implicit((Object)(object)val2))
			{
				val2 = Enemy.CenterTransform;
			}
			if (!SemiFunc.IsMultiplayer())
			{
				Object.Instantiate<GameObject>(val, val2.position, Quaternion.identity);
			}
			else
			{
				PhotonNetwork.InstantiateRoomObject("Valuables/" + ((Object)val).name, val2.position, Quaternion.identity, (byte)0, (object[])null);
			}
			Enemy.Health.spawnValuableCurrent++;
		}
		DespawnedTimer *= 3f;
	}

	[PunRPC]
	private void DespawnRPC()
	{
		Spawned = false;
		EnableObject.SetActive(false);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext((object)SetupDone);
		}
		else
		{
			SetupDone = (bool)stream.ReceiveNext();
		}
	}
}
