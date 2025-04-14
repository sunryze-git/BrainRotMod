using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public static MenuController instance;

	public GameObject networkConnectPrefab;

	private bool picked;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		SemiFunc.CursorUnlock(0.1f);
		if (!picked)
		{
			if (Input.GetKeyDown((KeyCode)49) || Input.GetKeyDown((KeyCode)350))
			{
				OnSinglePlayerPicked();
			}
			else if (Input.GetKeyDown((KeyCode)50) || Input.GetKeyDown((KeyCode)351))
			{
				OnMultiplayerPicked();
			}
			else if (Input.GetKeyDown((KeyCode)51) || SteamManager.instance.joinLobby)
			{
				OnMultiplayerOnlinePicked();
			}
			else if (Input.GetKeyDown((KeyCode)52))
			{
				OnTutorialPicked();
			}
		}
	}

	public void OnSinglePlayerPicked()
	{
		picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
	}

	public void OnMultiplayerPicked()
	{
		picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(1);
		GameManager.instance.localTest = true;
		Object.Instantiate<GameObject>(networkConnectPrefab);
	}

	public void OnMultiplayerOnlinePicked()
	{
	}

	public void OnTutorialPicked()
	{
		picked = true;
		RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
		RunManager.instance.ResetProgress();
		GameManager.instance.SetGameMode(0);
		SceneManager.LoadScene("Main");
		RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.Tutorial);
	}
}
