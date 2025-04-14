using System;
using System.Collections.Generic;
using UnityEngine;

public class DataDirector : MonoBehaviour
{
	public enum Setting
	{
		MusicVolume,
		SfxVolume,
		AmbienceVolume,
		MicDevice,
		ProximityVoice,
		Resolution,
		Fullscreen,
		MicVolume,
		TextToSpeechVolume,
		CameraShake,
		CameraAnimation,
		Tips,
		Vsync,
		MasterVolume,
		CameraSmoothing,
		LightDistance,
		Bloom,
		LensEffect,
		MotionBlur,
		MaxFPS,
		ShadowQuality,
		ShadowDistance,
		ChromaticAberration,
		Grain,
		WindowMode,
		RenderSize,
		GlitchLoop,
		AimSensitivity,
		CameraNoise,
		Gamma,
		PlayerNames,
		RunsPlayed,
		PushToTalk,
		TutorialPlayed,
		TutorialJumping,
		TutorialSprinting,
		TutorialSneaking,
		TutorialHiding,
		TutorialTumbling,
		TutorialPushingAndPulling,
		TutorialRotating,
		TutorialReviving,
		TutorialHealing,
		TutorialCartHandling,
		TutorialItemToggling,
		TutorialInventoryFill,
		TutorialMap,
		TutorialChargingStation,
		TutorialOnlyOneExtraction,
		TutorialChat,
		TutorialFinalExtraction,
		TutorialMultipleExtractions,
		TutorialShop
	}

	public enum SettingType
	{
		Audio,
		Gameplay,
		Graphics,
		None
	}

	public static DataDirector instance;

	private string playerBodyColor = "0";

	internal string micDevice = "";

	private Dictionary<Setting, string> settingsName = new Dictionary<Setting, string>();

	private Dictionary<Setting, int> settingsValue = new Dictionary<Setting, int>();

	private Dictionary<Setting, int> defaultSettingsValue = new Dictionary<Setting, int>();

	private Dictionary<SettingType, List<Setting>> settings = new Dictionary<SettingType, List<Setting>>();

	private void Awake()
	{
		if ((Object)(object)instance == (Object)null)
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			InitializeSettings();
			LoadSettings();
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void InitializeSettings()
	{
		SettingAdd(SettingType.Audio, Setting.MasterVolume, "Master Volume", 75);
		SettingAdd(SettingType.Audio, Setting.MusicVolume, "Music Volume", 75);
		SettingAdd(SettingType.Audio, Setting.SfxVolume, "Sfx Volume", 75);
		SettingAdd(SettingType.Audio, Setting.ProximityVoice, "Proximity Voice Volume", 75);
		SettingAdd(SettingType.Audio, Setting.TextToSpeechVolume, "Text to Speech Volume", 75);
		SettingAdd(SettingType.Audio, Setting.MicDevice, "Microphone", 1);
		SettingAdd(SettingType.Audio, Setting.MicVolume, "Microphone Volume", 100);
		SettingAdd(SettingType.Audio, Setting.PushToTalk, "Push to Talk", 0);
		SettingAdd(SettingType.Graphics, Setting.Resolution, "Resolution", 0);
		SettingAdd(SettingType.Graphics, Setting.Fullscreen, "Fullscreen", 0);
		SettingAdd(SettingType.Graphics, Setting.LightDistance, "Light Distance", 3);
		SettingAdd(SettingType.Graphics, Setting.Vsync, "Vsync", 0);
		SettingAdd(SettingType.Graphics, Setting.Bloom, "Bloom", 1);
		SettingAdd(SettingType.Graphics, Setting.ChromaticAberration, "Chromatic Aberration", 1);
		SettingAdd(SettingType.Graphics, Setting.Grain, "Grain", 1);
		SettingAdd(SettingType.Graphics, Setting.MotionBlur, "Motion Blur", 1);
		SettingAdd(SettingType.Graphics, Setting.LensEffect, "Lens Effect", 1);
		SettingAdd(SettingType.Graphics, Setting.GlitchLoop, "Glitch Loop", 1);
		SettingAdd(SettingType.Graphics, Setting.MaxFPS, "Max FPS", -1);
		SettingAdd(SettingType.Graphics, Setting.ShadowQuality, "Shadow Quality", 2);
		SettingAdd(SettingType.Graphics, Setting.ShadowDistance, "Shadow Distance", 3);
		SettingAdd(SettingType.Graphics, Setting.WindowMode, "Window Mode", 0);
		SettingAdd(SettingType.Graphics, Setting.RenderSize, "Pixelation", 2);
		SettingAdd(SettingType.Graphics, Setting.Gamma, "Gamma", 40);
		SettingAdd(SettingType.Gameplay, Setting.Tips, "Tips", 1);
		SettingAdd(SettingType.Gameplay, Setting.AimSensitivity, "Aim Sensitivity", 35);
		SettingAdd(SettingType.Gameplay, Setting.CameraSmoothing, "Camera Smoothing", 80);
		SettingAdd(SettingType.Gameplay, Setting.CameraShake, "Camera Shake", 100);
		SettingAdd(SettingType.Gameplay, Setting.CameraNoise, "Camera Noise", 100);
		SettingAdd(SettingType.Gameplay, Setting.CameraAnimation, "Camera Animation", 4);
		SettingAdd(SettingType.Gameplay, Setting.PlayerNames, "Player Names", 1);
		SettingAdd(SettingType.None, Setting.RunsPlayed, "Runs Played", 0);
		SettingAdd(SettingType.None, Setting.TutorialPlayed, "Tutorial Played", 0);
		SettingAdd(SettingType.None, Setting.TutorialJumping, "Tutorial Jumping", 0);
		SettingAdd(SettingType.None, Setting.TutorialSprinting, "Tutorial Sprinting", 0);
		SettingAdd(SettingType.None, Setting.TutorialSneaking, "Tutorial Sneaking", 0);
		SettingAdd(SettingType.None, Setting.TutorialHiding, "Tutorial Hiding", 0);
		SettingAdd(SettingType.None, Setting.TutorialTumbling, "Tutorial Tumbling", 0);
		SettingAdd(SettingType.None, Setting.TutorialPushingAndPulling, "Tutorial Pushing and Pulling", 0);
		SettingAdd(SettingType.None, Setting.TutorialRotating, "Tutorial Rotating", 0);
		SettingAdd(SettingType.None, Setting.TutorialReviving, "Tutorial Reviving", 0);
		SettingAdd(SettingType.None, Setting.TutorialHealing, "Tutorial Healing", 0);
		SettingAdd(SettingType.None, Setting.TutorialCartHandling, "Tutorial Cart Handling", 0);
		SettingAdd(SettingType.None, Setting.TutorialItemToggling, "Tutorial Item Toggling", 0);
		SettingAdd(SettingType.None, Setting.TutorialInventoryFill, "Tutorial Inventory Fill", 0);
		SettingAdd(SettingType.None, Setting.TutorialMap, "Tutorial Map", 0);
		SettingAdd(SettingType.None, Setting.TutorialChargingStation, "Tutorial Charging Station", 0);
		SettingAdd(SettingType.None, Setting.TutorialOnlyOneExtraction, "Tutorial Only One Extraction", 0);
		SettingAdd(SettingType.None, Setting.TutorialChat, "Tutorial Chat", 0);
		SettingAdd(SettingType.None, Setting.TutorialFinalExtraction, "Tutorial Final Extraction", 0);
		SettingAdd(SettingType.None, Setting.TutorialMultipleExtractions, "Tutorial Multiple Extractions", 0);
		SettingAdd(SettingType.None, Setting.TutorialShop, "Tutorial Shop", 0);
	}

	private void SettingAdd(SettingType settingType, Setting setting, string _name, int value)
	{
		if (settings.ContainsKey(settingType))
		{
			settings[settingType].Add(setting);
		}
		else
		{
			settings[settingType] = new List<Setting> { setting };
		}
		if (settingsName.ContainsKey(setting))
		{
			Debug.LogError((object)("Setting already exists: " + setting.ToString() + " " + _name));
			return;
		}
		settingsName[setting] = _name;
		settingsValue[setting] = value;
		defaultSettingsValue[setting] = value;
	}

	public int SettingValueFetch(Setting setting)
	{
		if (!settingsValue.ContainsKey(setting))
		{
			return 0;
		}
		return settingsValue[setting];
	}

	public float SettingValueFetchFloat(Setting setting)
	{
		return (float)settingsValue[setting] / 100f;
	}

	public void SettingValueSet(Setting setting, int value)
	{
		if (settingsValue.ContainsKey(setting))
		{
			settingsValue[setting] = value;
		}
		else
		{
			Debug.LogWarning((object)("Setting not found: " + setting));
		}
	}

	public string SettingNameGet(Setting setting)
	{
		if (!settingsName.ContainsKey(setting))
		{
			return null;
		}
		return settingsName[setting];
	}

	public void SaveSettings()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		SettingsSaveData settingsSaveData = new SettingsSaveData();
		settingsSaveData.settingsValue = new Dictionary<string, int>();
		foreach (KeyValuePair<Setting, int> item in settingsValue)
		{
			settingsSaveData.settingsValue[item.Key.ToString()] = item.Value;
		}
		ES3Settings val = new ES3Settings("SettingsData.es3", new Enum[1] { (Enum)(object)(Location)0 });
		ES3.Save<SettingsSaveData>("Settings", settingsSaveData, val);
		ES3.Save<string>("PlayerBodyColor", playerBodyColor, val);
		ES3.Save<string>("micDevice", micDevice, val);
	}

	public void ColorSetBody(int colorID)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		string text = colorID.ToString();
		playerBodyColor = text;
		ES3Settings val = new ES3Settings("SettingsData.es3", new Enum[1] { (Enum)(object)(Location)0 });
		ES3.Save<string>("PlayerBodyColor", playerBodyColor, val);
	}

	public int ColorGetBody()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		ES3Settings val = new ES3Settings("SettingsData.es3", new Enum[1] { (Enum)(object)(Location)0 });
		if (ES3.KeyExists("PlayerBodyColor", val))
		{
			playerBodyColor = ES3.Load<string>("PlayerBodyColor", val);
		}
		return int.Parse(playerBodyColor);
	}

	public void LoadSettings()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		try
		{
			ES3Settings val = new ES3Settings("SettingsData.es3", new Enum[1] { (Enum)(object)(Location)0 });
			if (ES3.FileExists(val))
			{
				if (ES3.KeyExists("Settings", val))
				{
					foreach (KeyValuePair<string, int> item in ES3.Load<SettingsSaveData>("Settings", val).settingsValue)
					{
						if (Enum.TryParse<Setting>(item.Key, out var result) && settingsValue.ContainsKey(result))
						{
							settingsValue[result] = item.Value;
						}
					}
				}
				else
				{
					Debug.LogWarning((object)("Key 'Settings' not found in file: " + val.FullPath));
				}
				if (ES3.KeyExists("PlayerBodyColor", val))
				{
					playerBodyColor = ES3.Load<string>("PlayerBodyColor", val);
				}
				if (ES3.KeyExists("micDevice", val))
				{
					micDevice = ES3.Load<string>("micDevice", val);
				}
			}
			else
			{
				SaveSettings();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Failed to load settings: " + ex.Message));
			ES3.DeleteFile("SettingsData.es3");
			SaveSettings();
		}
	}

	public void ResetSettingToDefault(Setting setting)
	{
		if (defaultSettingsValue.ContainsKey(setting))
		{
			settingsValue[setting] = defaultSettingsValue[setting];
		}
		else
		{
			Debug.LogWarning((object)("Default value not found for setting: " + setting));
		}
	}

	public void ResetSettingTypeToDefault(SettingType settingType)
	{
		if (settings.ContainsKey(settingType))
		{
			foreach (Setting item in settings[settingType])
			{
				if (defaultSettingsValue.ContainsKey(item))
				{
					settingsValue[item] = defaultSettingsValue[item];
				}
			}
			return;
		}
		Debug.LogWarning((object)("SettingType not found: " + settingType));
	}

	public void RunsPlayedAdd()
	{
		int value = SettingValueFetch(Setting.RunsPlayed) + 1;
		SettingValueSet(Setting.RunsPlayed, value);
		SaveSettings();
	}

	public void TutorialPlayed()
	{
		SettingValueSet(Setting.TutorialPlayed, 1);
		SaveSettings();
	}

	public void SaveDeleteCheck(bool _leaveGame)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && !((Object)(object)RunManager.instance.levelPrevious == (Object)(object)RunManager.instance.levelTutorial) && !((Object)(object)RunManager.instance.levelPrevious == (Object)(object)RunManager.instance.levelLobbyMenu) && !((Object)(object)RunManager.instance.levelPrevious == (Object)(object)RunManager.instance.levelMainMenu) && !((Object)(object)RunManager.instance.levelPrevious == (Object)(object)RunManager.instance.levelRecording))
		{
			bool flag = false;
			if (SemiFunc.RunIsArena())
			{
				flag = true;
			}
			else if (RunManager.instance.allPlayersDead && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelMainMenu && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelLobbyMenu && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelTutorial && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelLobby && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelShop && (Object)(object)RunManager.instance.levelPrevious != (Object)(object)RunManager.instance.levelRecording)
			{
				flag = true;
			}
			else if (_leaveGame && RunManager.instance.levelsCompleted == 0)
			{
				flag = true;
			}
			if (flag)
			{
				SemiFunc.SaveFileDelete(StatsManager.instance.saveFileCurrent);
			}
		}
	}
}
