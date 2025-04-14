using TMPro;
using UnityEngine;

public class MenuSliderPlayerMicGain : MonoBehaviour
{
	internal MenuSlider menuSlider;

	internal PlayerAvatar playerAvatar;

	private float currentValue;

	private bool fetched;

	private void Start()
	{
		menuSlider = ((Component)this).GetComponent<MenuSlider>();
	}

	private void Update()
	{
		if (!SemiFunc.IsMultiplayer() || !playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (currentValue != (float)menuSlider.currentValue && playerAvatar.voiceChatFetched)
		{
			if (!fetched)
			{
				playerAvatar.voiceChat.voiceGain = GameManager.instance.PlayerMicrophoneSettingGet(playerAvatar.steamID);
				menuSlider.settingsValue = playerAvatar.voiceChat.voiceGain;
				menuSlider.currentValue = (int)(playerAvatar.voiceChat.voiceGain * 200f);
				menuSlider.SetBar(menuSlider.settingsValue);
				menuSlider.SetBarScaleInstant();
				fetched = true;
			}
			currentValue = menuSlider.currentValue;
			playerAvatar.voiceChat.voiceGain = currentValue / 200f;
			GameManager.instance.PlayerMicrophoneSettingSet(playerAvatar.steamID, playerAvatar.voiceChat.voiceGain);
		}
		menuSlider.ExtraBarSet(playerAvatar.voiceChat.clipLoudnessNoTTS * 5f);
	}

	public void SliderNameSet(string name)
	{
		menuSlider = ((Component)this).GetComponent<MenuSlider>();
		menuSlider.elementName = name;
		((TMP_Text)menuSlider.elementNameText).text = name;
	}
}
