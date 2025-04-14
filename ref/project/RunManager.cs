using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
	public enum ChangeLevelType
	{
		Normal,
		RunLevel,
		Tutorial,
		LobbyMenu,
		MainMenu,
		Shop,
		Recording
	}

	public enum SaveLevel
	{
		Lobby,
		Shop
	}

	public static RunManager instance;

	internal int saveLevel;

	internal int loadLevel;

	internal Level debugLevel;

	internal bool skipMainMenu;

	internal bool localMultiplayerTest;

	internal bool runStarted;

	internal RunManagerPUN runManagerPUN;

	public int levelsCompleted;

	public Level levelCurrent;

	internal Level levelPrevious;

	private Level previousRunLevel;

	internal bool restarting;

	internal bool restartingDone;

	internal int levelsMax = 10;

	[Space]
	public Level levelMainMenu;

	public Level levelLobbyMenu;

	public Level levelLobby;

	public Level levelShop;

	public Level levelTutorial;

	public Level levelRecording;

	public Level levelArena;

	public List<Level> levels;

	internal int runLives = 3;

	internal bool levelFailed;

	internal bool waitToChangeScene;

	internal bool lobbyJoin;

	internal bool masterSwitched;

	internal bool gameOver;

	internal bool allPlayersDead;

	[Space]
	public List<EnemySetup> enemiesSpawned;

	private List<EnemySetup> enemiesSpawnedToDelete = new List<EnemySetup>();

	internal bool skipLoadingUI = true;

	internal Color loadingFadeColor = Color.black;

	internal float loadingAnimationTime;

	internal List<PlayerVoiceChat> voiceChats = new List<PlayerVoiceChat>();

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		levelPrevious = levelCurrent;
	}

	private void Update()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (LevelGenerator.Instance.Generated && !SteamClient.IsValid && !((Behaviour)SteamManager.instance).enabled)
		{
			Debug.LogError((object)"Steam not initialized. Quitting game.");
			Application.Quit();
		}
		if (SemiFunc.DebugDev())
		{
			if (Input.GetKeyDown((KeyCode)284))
			{
				if (SemiFunc.RunIsArena())
				{
					ChangeLevel(_completedLevel: true, _levelFailed: true);
				}
				else
				{
					ChangeLevel(_completedLevel: true, _levelFailed: false);
				}
			}
			if (!restarting && Object.op_Implicit((Object)(object)ChatManager.instance) && !ChatManager.instance.chatActive && Input.GetKeyDown((KeyCode)8))
			{
				ResetProgress();
				RestartScene();
				if ((Object)(object)levelCurrent != (Object)(object)levelTutorial)
				{
					SemiFunc.OnSceneSwitch(gameOver, _leaveGame: false);
				}
			}
		}
		if (restarting)
		{
			RestartScene();
		}
		if (restarting || !runStarted || GameDirector.instance.PlayerList.Count <= 0 || SemiFunc.RunIsArena() || GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		bool flag = true;
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (!player.isDisabled)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			allPlayersDead = true;
			if (Object.op_Implicit((Object)(object)SpectateCamera.instance) && SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
			{
				ChangeLevel(_completedLevel: false, _levelFailed: true);
			}
		}
	}

	private void OnApplicationQuit()
	{
		DataDirector.instance.SaveDeleteCheck(_leaveGame: true);
	}

	public void ChangeLevel(bool _completedLevel, bool _levelFailed, ChangeLevelType _changeLevelType = ChangeLevelType.Normal)
	{
		if ((!SemiFunc.MenuLevel() && !SemiFunc.IsMasterClientOrSingleplayer()) || restarting)
		{
			return;
		}
		gameOver = false;
		if (_levelFailed && (Object)(object)levelCurrent != (Object)(object)levelLobby && (Object)(object)levelCurrent != (Object)(object)levelShop)
		{
			if ((Object)(object)levelCurrent == (Object)(object)levelArena)
			{
				ResetProgress();
				if (SemiFunc.IsMultiplayer())
				{
					levelCurrent = levelLobbyMenu;
				}
				else
				{
					SetRunLevel();
				}
				gameOver = true;
			}
			else
			{
				levelCurrent = levelArena;
			}
		}
		if (!gameOver && (Object)(object)levelCurrent != (Object)(object)levelArena)
		{
			switch (_changeLevelType)
			{
			case ChangeLevelType.RunLevel:
				SetRunLevel();
				break;
			case ChangeLevelType.LobbyMenu:
				levelCurrent = levelLobbyMenu;
				break;
			case ChangeLevelType.MainMenu:
				levelCurrent = levelMainMenu;
				break;
			case ChangeLevelType.Tutorial:
				levelCurrent = levelTutorial;
				break;
			case ChangeLevelType.Recording:
				levelCurrent = levelRecording;
				break;
			case ChangeLevelType.Shop:
				levelCurrent = levelShop;
				break;
			default:
				if ((Object)(object)levelCurrent == (Object)(object)levelMainMenu || (Object)(object)levelCurrent == (Object)(object)levelLobbyMenu)
				{
					levelCurrent = levelLobby;
				}
				else if (_completedLevel && (Object)(object)levelCurrent != (Object)(object)levelLobby && (Object)(object)levelCurrent != (Object)(object)levelShop)
				{
					previousRunLevel = levelCurrent;
					levelsCompleted++;
					SemiFunc.StatSetRunLevel(levelsCompleted);
					SemiFunc.LevelSuccessful();
					levelCurrent = levelShop;
				}
				else if ((Object)(object)levelCurrent == (Object)(object)levelLobby)
				{
					SetRunLevel();
				}
				else if ((Object)(object)levelCurrent == (Object)(object)levelShop)
				{
					levelCurrent = levelLobby;
				}
				break;
			}
		}
		if (Object.op_Implicit((Object)(object)debugLevel) && (Object)(object)levelCurrent != (Object)(object)levelMainMenu && (Object)(object)levelCurrent != (Object)(object)levelLobbyMenu)
		{
			levelCurrent = debugLevel;
		}
		if (GameManager.Multiplayer())
		{
			runManagerPUN.photonView.RPC("UpdateLevelRPC", (RpcTarget)4, new object[3]
			{
				((Object)levelCurrent).name,
				levelsCompleted,
				gameOver
			});
		}
		Debug.Log((object)("Changed level to: " + ((Object)levelCurrent).name));
		if ((Object)(object)levelCurrent == (Object)(object)levelShop)
		{
			saveLevel = 1;
		}
		else
		{
			saveLevel = 0;
		}
		SemiFunc.StatSetSaveLevel(saveLevel);
		RestartScene();
		if (_changeLevelType != ChangeLevelType.Tutorial)
		{
			SemiFunc.OnSceneSwitch(gameOver, _leaveGame: false);
		}
	}

	public void RestartScene()
	{
		if (!restarting)
		{
			restarting = true;
			if (!Object.op_Implicit((Object)(object)GameDirector.instance))
			{
				return;
			}
			{
				foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
				{
					player.OutroStart();
				}
				return;
			}
		}
		if (restartingDone)
		{
			return;
		}
		bool flag = true;
		if (!Object.op_Implicit((Object)(object)GameDirector.instance))
		{
			flag = false;
		}
		else
		{
			foreach (PlayerAvatar player2 in GameDirector.instance.PlayerList)
			{
				if (!player2.outroDone)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (gameOver)
		{
			NetworkManager.instance.DestroyAll();
			gameOver = false;
		}
		if (lobbyJoin)
		{
			lobbyJoin = false;
			restartingDone = true;
			SceneManager.LoadSceneAsync("LobbyJoin");
		}
		else if (!waitToChangeScene)
		{
			restartingDone = true;
			if (!GameManager.Multiplayer())
			{
				SceneManager.LoadSceneAsync("Main");
			}
			else if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.LoadLevel("Reload");
			}
		}
	}

	public void UpdateLevel(string _levelName, int _levelsCompleted, bool _gameOver)
	{
		if (Object.op_Implicit((Object)(object)LobbyMenuOpen.instance))
		{
			DataDirector.instance.RunsPlayedAdd();
		}
		SemiFunc.OnSceneSwitch(_gameOver, _leaveGame: false);
		levelsCompleted = _levelsCompleted;
		SemiFunc.StatSetRunLevel(levelsCompleted);
		if (_levelName == ((Object)levelLobbyMenu).name)
		{
			levelCurrent = levelLobbyMenu;
		}
		else if (_levelName == ((Object)levelLobby).name)
		{
			levelCurrent = levelLobby;
		}
		else if (_levelName == ((Object)levelShop).name)
		{
			levelCurrent = levelShop;
		}
		else if (_levelName == ((Object)levelArena).name)
		{
			levelCurrent = levelArena;
		}
		else if (_levelName == ((Object)levelRecording).name)
		{
			levelCurrent = levelRecording;
		}
		else
		{
			foreach (Level level in levels)
			{
				if (((Object)level).name == _levelName)
				{
					levelCurrent = level;
					break;
				}
			}
		}
		Debug.Log((object)("updated level to: " + ((Object)levelCurrent).name));
	}

	public void ResetProgress()
	{
		if (Object.op_Implicit((Object)(object)StatsManager.instance))
		{
			StatsManager.instance.ResetAllStats();
		}
		levelsCompleted = 0;
		loadLevel = 0;
	}

	public void EnemiesSpawnedRemoveStart()
	{
		enemiesSpawnedToDelete.Clear();
		foreach (EnemySetup item in enemiesSpawned)
		{
			bool flag = false;
			foreach (EnemySetup item2 in enemiesSpawnedToDelete)
			{
				if ((Object)(object)item == (Object)(object)item2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				enemiesSpawnedToDelete.Add(item);
			}
		}
	}

	public void EnemiesSpawnedRemoveEnd()
	{
		foreach (EnemySetup item in enemiesSpawnedToDelete)
		{
			enemiesSpawned.Remove(item);
		}
	}

	public void SetRunLevel()
	{
		levelCurrent = previousRunLevel;
		while ((Object)(object)levelCurrent == (Object)(object)previousRunLevel)
		{
			levelCurrent = levels[Random.Range(0, levels.Count)];
		}
	}

	public IEnumerator LeaveToMainMenu()
	{
		while ((int)PhotonNetwork.NetworkingClient.State != 14 && (int)PhotonNetwork.NetworkingClient.State != 0)
		{
			yield return null;
		}
		Debug.Log((object)"Leave to Main Menu");
		SemiFunc.OnSceneSwitch(_gameOver: false, _leaveGame: true);
		levelCurrent = levelMainMenu;
		SceneManager.LoadSceneAsync("Reload");
		yield return null;
	}
}
