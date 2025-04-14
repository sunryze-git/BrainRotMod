using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public bool localTest;

	internal Dictionary<string, float> playerMicrophoneSettings = new Dictionary<string, float>();

	public int gameMode { get; private set; }

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			gameMode = 0;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	public void SetGameMode(int mode)
	{
		gameMode = mode;
	}

	public static bool Multiplayer()
	{
		return instance.gameMode == 1;
	}

	public void PlayerMicrophoneSettingSet(string _name, float _value)
	{
		playerMicrophoneSettings[_name] = _value;
	}

	public float PlayerMicrophoneSettingGet(string _name)
	{
		if (playerMicrophoneSettings.ContainsKey(_name))
		{
			return playerMicrophoneSettings[_name];
		}
		return 0.5f;
	}
}
