using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldSpaceUIParent : MonoBehaviour
{
	public static WorldSpaceUIParent instance;

	private CanvasGroup canvasGroup;

	[Space]
	public GameObject valueLostPrefab;

	public GameObject TTSPrefab;

	public GameObject playerNamePrefab;

	internal List<WorldSpaceUIValueLost> valueLostList = new List<WorldSpaceUIValueLost>();

	private float hideTimer;

	internal float hideAlpha = 1f;

	private void Awake()
	{
		instance = this;
		canvasGroup = ((Component)this).GetComponent<CanvasGroup>();
	}

	private void Update()
	{
		float num = 1f;
		if (hideTimer > 0f)
		{
			num = 0f;
			hideTimer -= Time.deltaTime;
		}
		hideAlpha = Mathf.Lerp(hideAlpha, num, Time.deltaTime * 20f);
		canvasGroup.alpha = hideAlpha;
	}

	public void ValueLostCreate(Vector3 _worldPosition, int _value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)PlayerController.instance).isActiveAndEnabled && Vector3.Distance(_worldPosition, ((Component)PlayerController.instance).transform.position) > 10f)
		{
			return;
		}
		foreach (WorldSpaceUIValueLost valueLost in valueLostList)
		{
			if (Vector3.Distance(valueLost.worldPosition, _worldPosition) < 1f && valueLost.timer > 0f)
			{
				valueLost.timer = 0f;
				_value += valueLost.value;
			}
		}
		WorldSpaceUIValueLost component = Object.Instantiate<GameObject>(valueLostPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform).GetComponent<WorldSpaceUIValueLost>();
		component.worldPosition = _worldPosition;
		component.value = _value;
		valueLostList.Add(component);
	}

	public void Hide()
	{
		hideTimer = 0.1f;
	}

	public void TTS(PlayerAvatar _player, string _text, float _time)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState == GameDirector.gameState.Main && !_player.isDisabled && !_player.isLocal)
		{
			WorldSpaceUITTS component = Object.Instantiate<GameObject>(TTSPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform).GetComponent<WorldSpaceUITTS>();
			((TMP_Text)component.text).text = _text;
			component.playerAvatar = _player;
			component.followTransform = _player.playerAvatarVisuals.TTSTransform;
			component.worldPosition = component.followTransform.position;
			component.followPosition = component.worldPosition;
			component.wordTime = _time;
			component.ttsVoice = _player.voiceChat.ttsVoice;
		}
	}

	public void PlayerName(PlayerAvatar _player)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_player.isLocal && !SemiFunc.MenuLevel())
		{
			WorldSpaceUIPlayerName component = Object.Instantiate<GameObject>(playerNamePrefab, ((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform).GetComponent<WorldSpaceUIPlayerName>();
			component.playerAvatar = _player;
			((TMP_Text)component.text).text = _player.playerName;
			_player.worldSpaceUIPlayerName = component;
		}
	}
}
