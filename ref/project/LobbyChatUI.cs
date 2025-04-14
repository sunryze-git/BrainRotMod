using TMPro;
using UnityEngine;

public class LobbyChatUI : SemiUI
{
	private TTSVoice ttsVoice;

	private MenuPlayerListed menuPlayerListed;

	private float prevWordTime;

	public bool isSpectate;

	public bool isGameplay;

	public TextMeshProUGUI spectateName;

	private RectTransform rectTransform;

	private float chatOffsetXPos;

	private bool offsetFetched;

	private string prevPlayerName = "";

	protected override void Start()
	{
		base.Start();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		menuPlayerListed = ((Component)this).GetComponentInParent<MenuPlayerListed>();
	}

	protected override void Update()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (isGameplay && (SemiFunc.RunIsLobbyMenu() || (Object.op_Implicit((Object)(object)PlayerAvatar.instance) && PlayerAvatar.instance.isDisabled)))
		{
			((TMP_Text)uiText).text = "";
			return;
		}
		if (Object.op_Implicit((Object)(object)spectateName) && prevPlayerName != ((TMP_Text)spectateName).text)
		{
			offsetFetched = false;
		}
		if (isSpectate)
		{
			SemiUIScoot(new Vector2(-200f + chatOffsetXPos, 0f));
		}
		if (isSpectate && Object.op_Implicit((Object)(object)spectateName) && !offsetFetched)
		{
			float num = ((TMP_Text)spectateName).preferredWidth;
			if (num > 155f)
			{
				num = 155f;
			}
			((Transform)rectTransform).localPosition = ((Transform)((TMP_Text)spectateName).rectTransform).localPosition + new Vector3(num, 25f, 0f);
			chatOffsetXPos = ((Transform)rectTransform).localPosition.x;
			offsetFetched = true;
			prevPlayerName = ((TMP_Text)spectateName).text;
		}
		if (!Object.op_Implicit((Object)(object)ttsVoice))
		{
			if (!isGameplay)
			{
				if (Object.op_Implicit((Object)(object)menuPlayerListed.playerAvatar.voiceChat) && menuPlayerListed.playerAvatar.voiceChat.TTSinstantiated)
				{
					ttsVoice = menuPlayerListed.playerAvatar.voiceChat.ttsVoice;
				}
			}
			else if (Object.op_Implicit((Object)(object)PlayerAvatar.instance) && Object.op_Implicit((Object)(object)PlayerAvatar.instance.voiceChat) && Object.op_Implicit((Object)(object)PlayerAvatar.instance.voiceChat.ttsVoice))
			{
				ttsVoice = PlayerAvatar.instance.voiceChat.ttsVoice;
			}
			return;
		}
		if (prevWordTime != ttsVoice.currentWordTime)
		{
			SemiUITextFlashColor(Color.yellow, 0.2f);
			SemiUISpringShakeY(4f, 5f, 0.2f);
			prevWordTime = ttsVoice.currentWordTime;
			((TMP_Text)uiText).text = ttsVoice.voiceText;
		}
		if (ttsVoice.isSpeaking)
		{
			((TMP_Text)uiText).text = ttsVoice.voiceText;
		}
		else
		{
			((TMP_Text)uiText).text = "";
		}
	}
}
