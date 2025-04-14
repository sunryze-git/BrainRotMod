using System.Collections.Generic;
using UnityEngine;

public class MenuPageEsc : MonoBehaviour
{
	public static MenuPageEsc instance;

	internal MenuPage menuPage;

	public GameObject playerMicrophoneVolumeSliderPrefab;

	internal Dictionary<PlayerAvatar, MenuSliderPlayerMicGain> playerMicGainSliders = new Dictionary<PlayerAvatar, MenuSliderPlayerMicGain>();

	private void Start()
	{
		instance = this;
		menuPage = ((Component)this).GetComponent<MenuPage>();
		PlayerGainSlidersUpdate();
	}

	private void Update()
	{
		if (SemiFunc.MenuLevel())
		{
			menuPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	public void ButtonEventContinue()
	{
		menuPage.PageStateSet(MenuPage.PageState.Closing);
	}

	public void PlayerGainSlidersUpdate()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar key in playerMicGainSliders.Keys)
		{
			if (!Object.op_Implicit((Object)(object)key) || !Object.op_Implicit((Object)(object)playerMicGainSliders[key]))
			{
				list.Add(key);
			}
		}
		foreach (PlayerAvatar item in list)
		{
			playerMicGainSliders.Remove(item);
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (!playerMicGainSliders.ContainsKey(player) && !player.isLocal)
			{
				GameObject obj = Object.Instantiate<GameObject>(playerMicrophoneVolumeSliderPrefab, ((Component)this).transform);
				obj.transform.localPosition = new Vector3(340f, 30f, 0f);
				Transform transform = obj.transform;
				transform.localPosition += new Vector3(0f, 42f * (float)playerMicGainSliders.Count, 0f);
				MenuSliderPlayerMicGain component = obj.GetComponent<MenuSliderPlayerMicGain>();
				component.playerAvatar = player;
				component.SliderNameSet(player.playerName + " VOICE");
				playerMicGainSliders.Add(player, component);
			}
		}
	}

	public void ButtonEventSelfDestruct()
	{
		if (SemiFunc.IsMultiplayer())
		{
			ChatManager.instance.PossessSelfDestruction();
		}
		else
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, savingGrace: false);
		}
		MenuManager.instance.PageCloseAll();
	}

	public void ButtonEventQuit()
	{
		RunManager.instance.skipLoadingUI = true;
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.quitApplication = true;
			player.OutroStartRPC();
		}
	}

	public void ButtonEventQuitToMenu()
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.OutroStartRPC();
		}
		NetworkManager.instance.leavePhotonRoom = true;
	}

	public void ButtonEventChangeColor()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Color);
	}

	public void ButtonEventSettings()
	{
		MenuManager.instance.PageSwap(MenuPageIndex.Settings);
	}
}
