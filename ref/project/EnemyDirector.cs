using System.Collections.Generic;
using UnityEngine;

public class EnemyDirector : MonoBehaviour
{
	public enum ExtractionsDoneState
	{
		StartRoom,
		PlayerRoom
	}

	private ExtractionsDoneState extractionsDoneState;

	private float extractionDoneStateTimer;

	private bool extractionDoneStateImpulse = true;

	public static EnemyDirector instance;

	internal bool debugNoVision;

	internal EnemySetup[] debugEnemy;

	internal float debugEnemyEnableTime;

	internal float debugEnemyDisableTime;

	internal bool debugEasyGrab;

	internal bool debugSpawnClose;

	internal bool debugDespawnClose;

	internal bool debugInvestigate;

	internal bool debugShortActionTimer;

	internal bool debugNoSpawnedPause;

	internal bool debugNoSpawnIdlePause;

	internal bool debugNoGrabMaxTime;

	public List<EnemySetup> enemiesDifficulty1;

	public List<EnemySetup> enemiesDifficulty2;

	public List<EnemySetup> enemiesDifficulty3;

	[Space]
	public AnimationCurve spawnIdlePauseCurve;

	[Space]
	public AnimationCurve amountCurve1;

	private int amountCurve1Value;

	public AnimationCurve amountCurve2;

	private int amountCurve2Value;

	public AnimationCurve amountCurve3;

	private int amountCurve3Value;

	internal int totalAmount;

	private List<EnemySetup> enemyList = new List<EnemySetup>();

	private int enemyListIndex;

	[Space]
	public float despawnedDecreaseMinutes;

	public float despawnedDecreasePercent;

	internal float despawnedTimeMultiplier = 1f;

	private float despawnedDecreaseTimer;

	private float investigatePointTimer;

	private float investigatePointTime = 3f;

	private float enemyActionAmount;

	internal float spawnIdlePauseTimer;

	[Space]
	public List<EnemyParent> enemiesSpawned;

	internal List<EnemyValuable> enemyValuables = new List<EnemyValuable>();

	internal List<LevelPoint> enemyFirstSpawnPoints = new List<LevelPoint>();

	private void Awake()
	{
		instance = this;
		despawnedDecreaseTimer = 60f * despawnedDecreaseMinutes;
	}

	private void Start()
	{
		spawnIdlePauseTimer = 60f * Random.Range(2f, 3f) * spawnIdlePauseCurve.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		if (Random.Range(0, 100) < 20)
		{
			spawnIdlePauseTimer *= Random.Range(0.1f, 0.25f);
		}
		spawnIdlePauseTimer = Mathf.Max(spawnIdlePauseTimer, 1f);
	}

	private void Update()
	{
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		if (LevelGenerator.Instance.Generated && spawnIdlePauseTimer > 0f)
		{
			spawnIdlePauseTimer -= Time.deltaTime;
			if (spawnIdlePauseTimer <= 0f)
			{
				foreach (EnemyParent item in enemiesSpawned)
				{
					if (!item.firstSpawnPointUsed)
					{
						spawnIdlePauseTimer = 2f;
					}
				}
			}
			if (debugNoSpawnIdlePause)
			{
				spawnIdlePauseTimer = 0f;
			}
		}
		despawnedDecreaseTimer -= Time.deltaTime;
		if (despawnedDecreaseTimer <= 0f)
		{
			despawnedTimeMultiplier -= despawnedDecreasePercent;
			if (despawnedTimeMultiplier < 0f)
			{
				despawnedTimeMultiplier = 0f;
			}
			despawnedDecreaseTimer = 60f * despawnedDecreaseMinutes;
		}
		if (RoundDirector.instance.allExtractionPointsCompleted)
		{
			foreach (EnemyParent item2 in enemiesSpawned)
			{
				if (item2.DespawnedTimer > 30f)
				{
					item2.DespawnedTimerSet(0f);
				}
			}
			if (investigatePointTimer <= 0f)
			{
				if (extractionsDoneState == ExtractionsDoneState.StartRoom)
				{
					enemyActionAmount = 0f;
					despawnedTimeMultiplier = 0f;
					if (extractionDoneStateImpulse)
					{
						extractionDoneStateTimer = 10f;
						extractionDoneStateImpulse = false;
						foreach (EnemyParent item3 in enemiesSpawned)
						{
							if (!item3.Spawned)
							{
								continue;
							}
							bool flag = false;
							foreach (PlayerAvatar item4 in SemiFunc.PlayerGetList())
							{
								if (!item4.isDisabled && Vector3.Distance(((Component)item3.Enemy).transform.position, ((Component)item4).transform.position) < 25f)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								item3.SpawnedTimerPause(0f);
								item3.SpawnedTimerSet(0f);
							}
						}
					}
					investigatePointTimer = investigatePointTime;
					List<LevelPoint> list = SemiFunc.LevelPointsGetInStartRoom();
					if (list.Count > 0)
					{
						SemiFunc.EnemyInvestigate(((Component)list[Random.Range(0, list.Count)]).transform.position, 100f);
					}
					extractionDoneStateTimer -= investigatePointTime;
					if (extractionDoneStateTimer <= 0f)
					{
						extractionsDoneState = ExtractionsDoneState.PlayerRoom;
					}
				}
				else
				{
					List<LevelPoint> list2 = SemiFunc.LevelPointsGetInPlayerRooms();
					if (list2.Count > 0)
					{
						SemiFunc.EnemyInvestigate(((Component)list2[Random.Range(0, list2.Count)]).transform.position, 100f);
					}
					investigatePointTimer = investigatePointTime;
					investigatePointTime = Mathf.Min(investigatePointTime + 2f, 30f);
				}
			}
			else
			{
				investigatePointTimer -= Time.deltaTime;
			}
		}
		float num = 0f;
		foreach (EnemyParent item5 in enemiesSpawned)
		{
			if (!item5.Spawned || !item5.playerClose || item5.forceLeave)
			{
				continue;
			}
			bool flag2 = false;
			foreach (PlayerAvatar item6 in SemiFunc.PlayerGetList())
			{
				foreach (RoomVolume currentRoom in item6.RoomVolumeCheck.CurrentRooms)
				{
					foreach (RoomVolume currentRoom2 in item5.currentRooms)
					{
						if ((Object)(object)currentRoom == (Object)(object)currentRoom2)
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			if (flag2)
			{
				float num2 = 0f;
				num2 = ((item5.difficulty == EnemyParent.Difficulty.Difficulty3) ? (num2 + 2f) : ((item5.difficulty != EnemyParent.Difficulty.Difficulty2) ? (num2 + 0.5f) : (num2 + 1f)));
				num += num2 * item5.actionMultiplier;
			}
		}
		if (num > 0f)
		{
			enemyActionAmount += num * Time.deltaTime;
		}
		else
		{
			enemyActionAmount -= 0.1f * Time.deltaTime;
			enemyActionAmount = Mathf.Max(0f, enemyActionAmount);
		}
		float num3 = 120f;
		if (debugShortActionTimer)
		{
			num3 = 5f;
		}
		if (!(enemyActionAmount > num3))
		{
			return;
		}
		enemyActionAmount = 0f;
		LevelPoint levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(((Component)this).transform.position, 5f);
		if (Object.op_Implicit((Object)(object)levelPoint))
		{
			SetInvestigate(((Component)levelPoint).transform.position, float.MaxValue);
		}
		if (RoundDirector.instance.allExtractionPointsCompleted && extractionsDoneState == ExtractionsDoneState.PlayerRoom)
		{
			investigatePointTimer = 60f;
		}
		foreach (EnemyParent item7 in enemiesSpawned)
		{
			if (item7.Spawned)
			{
				item7.forceLeave = true;
			}
		}
	}

	public void AmountSetup()
	{
		amountCurve3Value = (int)amountCurve3.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		amountCurve2Value = (int)amountCurve2.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		amountCurve1Value = (int)amountCurve1.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		for (int i = 0; i < amountCurve3Value; i++)
		{
			PickEnemies(enemiesDifficulty3);
		}
		for (int j = 0; j < amountCurve2Value; j++)
		{
			PickEnemies(enemiesDifficulty2);
		}
		for (int k = 0; k < amountCurve1Value; k++)
		{
			PickEnemies(enemiesDifficulty1);
		}
		totalAmount = amountCurve1Value + amountCurve2Value + amountCurve3Value;
	}

	private void PickEnemies(List<EnemySetup> _enemiesList)
	{
		int num = DataDirector.instance.SettingValueFetch(DataDirector.Setting.RunsPlayed);
		_enemiesList.Shuffle();
		EnemySetup item = null;
		float num2 = -1f;
		foreach (EnemySetup _enemies in _enemiesList)
		{
			if ((_enemies.levelsCompletedCondition && (RunManager.instance.levelsCompleted < _enemies.levelsCompletedMin || RunManager.instance.levelsCompleted > _enemies.levelsCompletedMax)) || num < _enemies.runsPlayed)
			{
				continue;
			}
			int num3 = 0;
			foreach (EnemySetup item2 in RunManager.instance.enemiesSpawned)
			{
				if ((Object)(object)item2 == (Object)(object)_enemies)
				{
					num3++;
				}
			}
			float num4 = 100f;
			if (Object.op_Implicit((Object)(object)_enemies.rarityPreset))
			{
				num4 = _enemies.rarityPreset.chance;
			}
			float num5 = Mathf.Max(0f, num4 - 30f * (float)num3);
			float num6 = Random.Range(0f, num5);
			if (num6 > num2)
			{
				item = _enemies;
				num2 = num6;
			}
		}
		enemyList.Add(item);
	}

	public EnemySetup GetEnemy()
	{
		EnemySetup enemySetup = enemyList[enemyListIndex];
		enemyListIndex++;
		int num = 0;
		foreach (EnemySetup item in RunManager.instance.enemiesSpawned)
		{
			if ((Object)(object)item == (Object)(object)enemySetup)
			{
				num++;
			}
		}
		int num2 = 4;
		while (num < 8 && num2 > 0)
		{
			RunManager.instance.enemiesSpawned.Add(enemySetup);
			num++;
			num2--;
		}
		return enemySetup;
	}

	public void FirstSpawnPointAdd(EnemyParent _enemyParent)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		List<LevelPoint> list = SemiFunc.LevelPointsGetAll();
		float num = 0f;
		LevelPoint levelPoint = null;
		foreach (LevelPoint item in list)
		{
			float num2 = Vector3.Distance(((Component)item).transform.position, ((Component)LevelGenerator.Instance.LevelPathTruck).transform.position);
			foreach (LevelPoint enemyFirstSpawnPoint in enemyFirstSpawnPoints)
			{
				if ((Object)(object)enemyFirstSpawnPoint == (Object)(object)item)
				{
					num2 = 0f;
					break;
				}
			}
			if (num2 > num)
			{
				num = num2;
				levelPoint = item;
			}
		}
		if (Object.op_Implicit((Object)(object)levelPoint))
		{
			_enemyParent.firstSpawnPoint = levelPoint;
			enemyFirstSpawnPoints.Add(levelPoint);
		}
	}

	public void DebugResult()
	{
	}

	public void SetInvestigate(Vector3 position, float radius)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (debugInvestigate)
		{
			Object.Instantiate<GameObject>(AssetManager.instance.debugEnemyInvestigate, position, Quaternion.identity).GetComponent<DebugEnemyInvestigate>().radius = radius;
		}
		foreach (EnemyParent item in enemiesSpawned)
		{
			if (!item.Spawned)
			{
				if (radius >= 15f)
				{
					item.DisableDecrease(5f);
				}
			}
			else if (item.Enemy.HasStateInvestigate && Vector3.Distance(position, ((Component)item.Enemy).transform.position) / item.Enemy.StateInvestigate.rangeMultiplier < radius)
			{
				item.Enemy.StateInvestigate.Set(position);
			}
		}
	}

	public void AddEnemyValuable(EnemyValuable _newValuable)
	{
		List<EnemyValuable> list = new List<EnemyValuable>();
		foreach (EnemyValuable enemyValuable2 in enemyValuables)
		{
			if (!Object.op_Implicit((Object)(object)enemyValuable2))
			{
				list.Add(enemyValuable2);
			}
		}
		foreach (EnemyValuable item in list)
		{
			enemyValuables.Remove(item);
		}
		enemyValuables.Add(_newValuable);
		if (enemyValuables.Count > 10)
		{
			EnemyValuable enemyValuable = enemyValuables[0];
			enemyValuables.RemoveAt(0);
			enemyValuable.Destroy();
		}
	}
}
