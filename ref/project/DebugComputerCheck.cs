using System.Collections;
using UnityEngine;

public class DebugComputerCheck : MonoBehaviour
{
	public enum StartMode
	{
		Normal,
		SinglePlayer,
		Multiplayer
	}

	public static DebugComputerCheck instance;

	internal bool Active = true;

	public bool DebugDisable;

	public SemiFunc.User DebugUser;

	public string[] computerNames;

	public StartMode Mode;

	public bool LevelDebug = true;

	public Level LevelOverride;

	public GameObject StartRoomOverride;

	public bool ModuleOverrideActive = true;

	public GameObject ModuleOverride;

	public Module.Type ModuleType;

	public bool OnlyOneModule;

	public float LevelSizeMultiplier = 1f;

	public int LevelsCompleted;

	public bool EnemyDebug = true;

	public EnemySetup[] EnemyOverride;

	public float EnemyEnableTimeOverride;

	public float EnemyDisableTimeOverride;

	public bool EnemyDisable;

	public bool EnemyNoVision;

	public bool EnemySpawnClose;

	public bool EnemyDespawnClose;

	public bool EnemyInvestigates;

	public bool EnemyShortActionTimer;

	public bool EnemyNoSpawnedPause;

	public bool EnemyNoSpawnIdlePause;

	public bool EnemyNoGrabMaxTime;

	public bool EnemyEasyGrab;

	public bool PlayerDebug;

	public bool InfiniteEnergy;

	public bool GodMode;

	public bool NoTumbleMode;

	public bool DebugVoice;

	public bool DebugMapActive;

	public bool PowerGrabber;

	public bool StickyGrabber;

	public bool OtherDebug;

	public ValuableDirector.ValuableDebug valuableDebug;

	public bool DisableMusic;

	public bool DisableLoadingLevelAnimation;

	public bool LowHaul;

	public bool SimulateLag;

	public bool StatsDebug;

	public int Currency;

	public bool EmptyBatteries;

	public int StartCrystals = 1;

	public bool BuyAllItems;

	private void Start()
	{
		if (!Application.isEditor)
		{
			Active = false;
			((Component)this).gameObject.SetActive(false);
			return;
		}
		bool flag = false;
		string[] array = computerNames;
		foreach (string text in array)
		{
			if (SystemInfo.deviceName == text)
			{
				instance = this;
				flag = true;
			}
		}
		if (!flag)
		{
			Active = false;
			((Component)this).gameObject.SetActive(false);
			return;
		}
		if (DebugDisable)
		{
			Active = false;
			return;
		}
		if (Mode == StartMode.SinglePlayer)
		{
			RunManager.instance.skipMainMenu = true;
			if ((Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelMainMenu)
			{
				RunManager.instance.SetRunLevel();
			}
		}
		if (Mode == StartMode.Multiplayer)
		{
			RunManager.instance.localMultiplayerTest = true;
		}
		if (PlayerDebug && InfiniteEnergy)
		{
			PlayerController.instance.DebugEnergy = true;
		}
		if (PlayerDebug && NoTumbleMode)
		{
			PlayerController.instance.DebugNoTumble = true;
		}
		if (LevelDebug && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelMainMenu && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelLobbyMenu)
		{
			if (Object.op_Implicit((Object)(object)LevelOverride))
			{
				RunManager.instance.debugLevel = LevelOverride;
				RunManager.instance.levelCurrent = LevelOverride;
			}
			if (LevelsCompleted > 0)
			{
				RunManager.instance.levelsCompleted = LevelsCompleted;
			}
			if (Object.op_Implicit((Object)(object)StartRoomOverride))
			{
				LevelGenerator.Instance.DebugStartRoom = StartRoomOverride;
			}
			LevelGenerator.Instance.DebugLevelSize = LevelSizeMultiplier;
			if (!ModuleOverrideActive)
			{
				ModuleOverride = null;
			}
			if (Object.op_Implicit((Object)(object)ModuleOverride))
			{
				LevelGenerator.Instance.DebugModule = ModuleOverride;
				if (ModuleType == Module.Type.Normal)
				{
					LevelGenerator.Instance.DebugNormal = true;
				}
				else if (ModuleType == Module.Type.Passage)
				{
					LevelGenerator.Instance.DebugPassage = true;
					LevelGenerator.Instance.DebugAmount = 6;
				}
				else if (ModuleType == Module.Type.DeadEnd || ModuleType == Module.Type.Extraction)
				{
					LevelGenerator.Instance.DebugDeadEnd = true;
					LevelGenerator.Instance.DebugAmount = 1;
				}
			}
			if (OnlyOneModule)
			{
				LevelGenerator.Instance.DebugAmount = 1;
			}
		}
		if (OtherDebug)
		{
			ValuableDirector.instance.valuableDebug = valuableDebug;
		}
		if (EnemyDebug)
		{
			if (EnemyDisable)
			{
				LevelGenerator.Instance.DebugNoEnemy = true;
			}
			if (EnemyNoVision)
			{
				EnemyDirector.instance.debugNoVision = true;
			}
			if (EnemyEasyGrab)
			{
				EnemyDirector.instance.debugEasyGrab = true;
			}
			if (EnemySpawnClose)
			{
				EnemyDirector.instance.debugSpawnClose = true;
			}
			if (EnemyDespawnClose)
			{
				EnemyDirector.instance.debugDespawnClose = true;
			}
			if (EnemyInvestigates)
			{
				EnemyDirector.instance.debugInvestigate = true;
			}
			if (EnemyShortActionTimer)
			{
				EnemyDirector.instance.debugShortActionTimer = true;
			}
			if (EnemyNoSpawnedPause)
			{
				EnemyDirector.instance.debugNoSpawnedPause = true;
			}
			if (EnemyNoSpawnIdlePause)
			{
				EnemyDirector.instance.debugNoSpawnIdlePause = true;
			}
			if (EnemyNoGrabMaxTime)
			{
				EnemyDirector.instance.debugNoGrabMaxTime = true;
			}
			if (EnemyOverride.Length != 0)
			{
				EnemyDirector.instance.debugEnemy = EnemyOverride;
				EnemyDirector.instance.debugEnemyEnableTime = EnemyEnableTimeOverride;
				EnemyDirector.instance.debugEnemyDisableTime = EnemyDisableTimeOverride;
			}
		}
		if (OtherDebug && LowHaul)
		{
			RoundDirector.instance.debugLowHaul = true;
		}
		if (OtherDebug && DisableMusic)
		{
			((Component)LevelMusic.instance).gameObject.SetActive(false);
			((Component)ConstantMusic.instance).gameObject.SetActive(false);
			((Component)MusicEnemyNear.instance).gameObject.SetActive(false);
		}
		if (OtherDebug && DisableLoadingLevelAnimation)
		{
			LoadingUI.instance.debugDisableLevelAnimation = true;
		}
		((MonoBehaviour)this).StartCoroutine(StatsUpdate());
		if (PlayerDebug && DebugVoice)
		{
			((MonoBehaviour)this).StartCoroutine(VoiceUpdate());
		}
		if (OtherDebug && SimulateLag)
		{
			((MonoBehaviour)this).StartCoroutine(SimulateLagUpdate());
		}
		((MonoBehaviour)this).StartCoroutine(AfterLevelGen());
		if (PlayerDebug && DebugMapActive)
		{
			Map.Instance.debugActive = true;
		}
		if (PlayerDebug && (StickyGrabber || PowerGrabber))
		{
			((MonoBehaviour)this).StartCoroutine(PhysGrabber());
		}
	}

	private IEnumerator SimulateLagUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		((Behaviour)PunManager.instance.lagSimulationGui).enabled = true;
	}

	private IEnumerator AfterLevelGen()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (EmptyBatteries)
		{
			StatsManager.instance.EmptyAllBatteries();
		}
	}

	private IEnumerator StatsUpdate()
	{
		if (StatsDebug)
		{
			while (!Object.op_Implicit((Object)(object)StatsManager.instance) || !StatsManager.instance.statsSynced || !Object.op_Implicit((Object)(object)PunManager.instance.statsManager))
			{
				yield return (object)new WaitForSeconds(0.1f);
			}
			if (Currency > 0)
			{
				SemiFunc.StatSetRunCurrency(Currency);
			}
			if (BuyAllItems)
			{
				StatsManager.instance.BuyAllItems();
			}
			if (StartCrystals != 1)
			{
				StatsManager.instance.itemsPurchased["Item Power Crystal"] = StartCrystals;
			}
		}
	}

	private IEnumerator VoiceUpdate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		foreach (PlayerAvatar _player in GameDirector.instance.PlayerList)
		{
			while (!Object.op_Implicit((Object)(object)_player.voiceChat))
			{
				yield return (object)new WaitForSeconds(0.1f);
			}
			_player.voiceChat.SetDebug();
		}
	}

	private IEnumerator PhysGrabber()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (StickyGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.debugStickyGrabber = true;
		}
		if (PowerGrabber)
		{
			PlayerController.instance.playerAvatarScript.physGrabber.grabStrength = 5f;
		}
	}
}
