using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUITTS : WorldSpaceUIChild
{
	internal TextMeshProUGUI text;

	internal float wordTime;

	internal TTSVoice ttsVoice;

	internal Transform followTransform;

	internal PlayerAvatar playerAvatar;

	private float flashTimer = 0.1f;

	private Color textColor = Color.yellow;

	private Color textColorTarget = Color.white;

	private bool flashDone;

	public AnimationCurve curveIntro;

	private float curveLerp;

	internal Vector3 followPosition;

	private float alphaCheckTimer;

	private float textAlphaTarget;

	private float textAlpha;

	private Camera cameraMain;

	private void Awake()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		text = ((Component)this).GetComponent<TextMeshProUGUI>();
		((Graphic)text).color = new Color(textColor.r, textColor.g, textColor.b, 0f);
		cameraMain = Camera.main;
	}

	protected override void Update()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (alphaCheckTimer <= 0f)
		{
			textAlphaTarget = 1f;
			alphaCheckTimer = 0.1f;
			if (!Object.op_Implicit((Object)(object)SpectateCamera.instance) || SpectateCamera.instance.CheckState(SpectateCamera.State.Normal))
			{
				float num = 5f;
				float num2 = 20f;
				float num3 = Vector3.Distance(((Component)cameraMain).transform.position, worldPosition);
				if (num3 > num)
				{
					num3 = Mathf.Clamp(num3, num, num2);
					textAlphaTarget = 1f - (num3 - num) / (num2 - num);
				}
				if (Object.op_Implicit((Object)(object)ttsVoice) && ttsVoice.playerAvatar.voiceChat.lowPassLogicTTS.LowPass)
				{
					textAlphaTarget *= 0.5f;
				}
			}
		}
		else
		{
			alphaCheckTimer -= Time.deltaTime;
		}
		if (!Object.op_Implicit((Object)(object)followTransform) || !Object.op_Implicit((Object)(object)ttsVoice) || !ttsVoice.isSpeaking || !Object.op_Implicit((Object)(object)playerAvatar) || playerAvatar.isDisabled)
		{
			textAlphaTarget = 0f;
			if (textAlpha < 0.01f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				return;
			}
		}
		textAlpha = Mathf.Lerp(textAlpha, textAlphaTarget, 30f * Time.deltaTime);
		if (!flashDone)
		{
			flashTimer -= Time.deltaTime;
			if (flashTimer <= 0f)
			{
				if (textColor != textColorTarget)
				{
					textColor = Color.Lerp(textColor, textColorTarget, 20f * Time.deltaTime);
				}
				else
				{
					flashDone = true;
				}
			}
		}
		((Graphic)text).color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
		if (Object.op_Implicit((Object)(object)followTransform))
		{
			followPosition = Vector3.Lerp(followPosition, followTransform.position, 10f * Time.deltaTime);
		}
		worldPosition = followPosition + curveIntro.Evaluate(curveLerp) * Vector3.up * 0.025f;
		curveLerp += Time.deltaTime * 4f;
		curveLerp = Mathf.Clamp01(curveLerp);
		if (Object.op_Implicit((Object)(object)ttsVoice) && ttsVoice.currentWordTime != wordTime)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
