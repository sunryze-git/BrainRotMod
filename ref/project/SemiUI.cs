using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SemiUI : MonoBehaviour
{
	internal Vector3 initialPosition;

	private float shakeTimeX;

	private float shakeTimeY;

	private float shakeAmountX;

	private float shakeAmountY;

	private float shakeFrequencyX;

	private float shakeFrequencyY;

	private float shakeDurationX;

	private float shakeDurationY;

	public bool animateTheEntireObject;

	[HideInInspector]
	public TextMeshProUGUI uiText;

	private Color originalTextColor;

	private Color originalFontColor;

	private Color originalGlowColor;

	private Color flashColor;

	private float flashColorTime;

	private Material textMaterial;

	internal float hideTimer;

	internal float showTimer;

	private bool uiTextEnabledPrevious;

	private float scaleTime;

	private float scaleAmount;

	private float scaleFrequency;

	private float scaleDuration;

	private Vector3 originalScale;

	[HideInInspector]
	public Transform textRectTransform;

	private AnimationCurve animationCurveWooshAway;

	private AnimationCurve animationCurveWooshIn;

	private AnimationCurve animationCurveInOut;

	private float hideAnimationEvaluation;

	private float showAnimationEvaluation;

	public Vector2 hidePosition = new Vector2(0f, 0f);

	[HideInInspector]
	public Vector2 showPosition = new Vector2(0f, 0f);

	private Vector2 hidePositionCurrent = new Vector2(0f, 0f);

	private bool initialized;

	private Vector2 scootPosition = new Vector2(0f, 0f);

	private float scootTimer = -123f;

	private Vector2 scootPositionPrev = new Vector2(0f, 0f);

	private float scootAnimationEvaluation;

	private Vector2 originalScootPosition = new Vector2(0f, 0f);

	private Vector2 scootPositionCurrent = new Vector2(0f, 0f);

	private List<GameObject> allChildren = new List<GameObject>();

	private float SpringShakeX;

	private float SpringShakeY;

	private float stopScootingTimer;

	private float stopHidingTimer;

	private float stopShowingTimer;

	private float prevShowTimer;

	private float prevHideTimer;

	private float prevScootTimer;

	private float animationEval;

	private float prevStopHidingTimer;

	private float prevStopShowingTimer;

	private float scootEval;

	protected virtual void Start()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		if ((Object)(object)uiText == (Object)null)
		{
			uiText = ((Component)this).GetComponent<TextMeshProUGUI>();
		}
		if ((Object)(object)uiText == (Object)null)
		{
			uiText = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
		}
		initialPosition = ((Component)this).transform.localPosition;
		if (Object.op_Implicit((Object)(object)uiText))
		{
			originalTextColor = ((Graphic)uiText).color;
		}
		if (Object.op_Implicit((Object)(object)uiText))
		{
			originalFontColor = ((TMP_Text)uiText).fontMaterial.GetColor(ShaderUtilities.ID_FaceColor);
		}
		if (Object.op_Implicit((Object)(object)uiText))
		{
			uiTextEnabledPrevious = ((Behaviour)uiText).enabled;
		}
		if (!Object.op_Implicit((Object)(object)textRectTransform))
		{
			textRectTransform = (Transform)(object)((Component)this).GetComponent<RectTransform>();
		}
		originalScale = textRectTransform.localScale;
		if (Object.op_Implicit((Object)(object)uiText))
		{
			originalGlowColor = ((TMP_Text)uiText).fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
		}
		if (!animateTheEntireObject)
		{
			if (showPosition == new Vector2(0f, 0f))
			{
				showPosition = Vector2.op_Implicit(textRectTransform.localPosition);
			}
		}
		else if (showPosition == new Vector2(0f, 0f))
		{
			showPosition = Vector2.op_Implicit(((Component)this).transform.localPosition);
		}
		hidePosition += showPosition;
		((MonoBehaviour)this).StartCoroutine(LateStart());
		if (!animateTheEntireObject)
		{
			textRectTransform.localPosition = Vector2.op_Implicit(hidePosition);
		}
		else
		{
			((Component)this).transform.localPosition = Vector2.op_Implicit(hidePosition);
		}
		hidePositionCurrent = hidePosition;
		hideAnimationEvaluation = 1f;
		if (Object.op_Implicit((Object)(object)uiText) && !animateTheEntireObject)
		{
			((Behaviour)uiText).enabled = false;
		}
		hideTimer = 0.2f;
		allChildren = new List<GameObject>();
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			allChildren.Add(((Component)val).gameObject);
		}
	}

	private void AllChildrenSetActive(bool active)
	{
		foreach (GameObject allChild in allChildren)
		{
			allChild.SetActive(active);
		}
	}

	private IEnumerator LateStart()
	{
		yield return (object)new WaitForSeconds(0.2f);
		animationCurveWooshAway = AssetManager.instance.animationCurveWooshAway;
		animationCurveWooshIn = AssetManager.instance.animationCurveWooshIn;
		animationCurveInOut = AssetManager.instance.animationCurveInOut;
		initialized = true;
	}

	protected virtual void Update()
	{
		if (initialized)
		{
			float deltaTime = Time.deltaTime;
			FlashColorLogic(deltaTime);
			HideAnimationLogic(deltaTime);
			HideTimer(deltaTime);
			SpringScaleLogic(deltaTime);
			ScootPositionLogic(deltaTime);
			SpringShakeLogic(deltaTime);
			UpdatePositionLogic();
			prevShowTimer = showTimer;
			prevHideTimer = hideTimer;
			prevScootTimer = scootTimer;
			prevStopHidingTimer = stopHidingTimer;
			prevStopShowingTimer = stopShowingTimer;
			if (hideTimer >= 0f)
			{
				hideTimer -= deltaTime;
			}
			if (showTimer >= 0f)
			{
				showTimer -= deltaTime;
			}
			if (stopShowingTimer >= 0f)
			{
				stopShowingTimer -= deltaTime;
			}
			if (stopHidingTimer >= 0f)
			{
				stopHidingTimer -= deltaTime;
			}
			if (stopScootingTimer >= 0f)
			{
				stopScootingTimer -= deltaTime;
			}
			if (scootTimer >= 0f)
			{
				scootTimer -= deltaTime;
			}
		}
	}

	public void SemiUISpringScale(float amount, float frequency, float time)
	{
		scaleTime = 0f;
		scaleAmount = amount;
		scaleFrequency = frequency;
		scaleDuration = time;
	}

	private void ScootPositionLogic(float deltaTime)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (scootTimer <= 0f && prevScootTimer <= 0f && scootPositionCurrent != Vector2.zero)
		{
			scootPositionCurrent = Vector2.LerpUnclamped(scootPosition, Vector2.zero, scootEval);
			if (scootEval >= 1f)
			{
				scootPositionCurrent = Vector2.zero;
				scootAnimationEvaluation = 0f;
				scootEval = 0f;
			}
			else
			{
				scootAnimationEvaluation += 4f * deltaTime;
				scootAnimationEvaluation = Mathf.Clamp01(scootAnimationEvaluation);
				scootEval = animationCurveInOut.Evaluate(scootAnimationEvaluation);
			}
		}
		if (!(scootTimer > 0f) || !(prevScootTimer > 0f))
		{
			return;
		}
		stopScootingTimer = 0.1f;
		if (scootPositionCurrent != scootPosition)
		{
			scootPositionCurrent = Vector2.LerpUnclamped(Vector2.zero, scootPosition, scootEval);
			if (scootEval >= 1f)
			{
				scootPositionCurrent = scootPosition;
				scootAnimationEvaluation = 0f;
				scootEval = 0f;
			}
			else
			{
				scootAnimationEvaluation += 4f * deltaTime;
				scootAnimationEvaluation = Mathf.Clamp01(scootAnimationEvaluation);
				scootEval = animationCurveInOut.Evaluate(scootAnimationEvaluation);
			}
		}
	}

	private void UpdatePositionLogic()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!animateTheEntireObject)
		{
			textRectTransform.localPosition = Vector2.op_Implicit(hidePositionCurrent + scootPositionCurrent + new Vector2(SpringShakeX, SpringShakeY));
		}
		else
		{
			((Component)this).transform.localPosition = Vector2.op_Implicit(hidePositionCurrent + scootPositionCurrent + new Vector2(SpringShakeX, SpringShakeY));
		}
	}

	private void SpringScaleLogic(float deltaTime)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (scaleTime < scaleDuration)
		{
			float num = CalculateSpringOffset(scaleTime, scaleAmount, scaleFrequency, scaleDuration);
			Vector3 val = originalScale * (1f + num);
			((Vector3)(ref val))._002Ector(Mathf.Abs(val.x), Mathf.Abs(val.y), Mathf.Abs(val.z));
			if (!animateTheEntireObject)
			{
				textRectTransform.localScale = val;
			}
			else
			{
				((Component)this).transform.localScale = val;
			}
			scaleTime += deltaTime;
		}
		else if (!animateTheEntireObject)
		{
			textRectTransform.localScale = originalScale;
		}
		else
		{
			((Component)this).transform.localScale = originalScale;
		}
	}

	private void HideAnimationLogic(float deltaTime)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if (hideTimer <= 0f && prevHideTimer <= 0f)
		{
			if (showTimer <= 0f && prevShowTimer <= 0f)
			{
				animationEval = 0f;
				showAnimationEvaluation = 0f;
				hideAnimationEvaluation = 0f;
			}
			showTimer = 0.1f;
		}
		if (showTimer > 0f && prevShowTimer > 0f)
		{
			stopShowingTimer = 0.1f;
			if (hidePositionCurrent != showPosition)
			{
				hidePositionCurrent = Vector2.LerpUnclamped(hidePosition + scootPositionCurrent, showPosition, animationEval);
				if (showAnimationEvaluation >= 1f)
				{
					hidePositionCurrent = showPosition;
					showAnimationEvaluation = 0f;
					animationEval = 0f;
				}
				else
				{
					showAnimationEvaluation += 4f * deltaTime;
					showAnimationEvaluation = Mathf.Clamp01(showAnimationEvaluation);
					animationEval = animationCurveWooshIn.Evaluate(showAnimationEvaluation);
				}
			}
		}
		if (!(hideTimer > 0f) || !(prevHideTimer > 0f) || !(showTimer <= 0f) || !(prevShowTimer <= 0f))
		{
			return;
		}
		stopHidingTimer = 0.1f;
		if (hidePositionCurrent != hidePosition)
		{
			hidePositionCurrent = Vector2.LerpUnclamped(showPosition, hidePosition, animationEval);
			if (hideAnimationEvaluation >= 1f)
			{
				hidePositionCurrent = hidePosition;
				hideAnimationEvaluation = 0f;
				animationEval = 0f;
			}
			else
			{
				hideAnimationEvaluation += 4f * deltaTime;
				hideAnimationEvaluation = Mathf.Clamp01(hideAnimationEvaluation);
				animationEval = animationCurveWooshAway.Evaluate(hideAnimationEvaluation);
			}
		}
	}

	private void HideTimer(float deltaTime)
	{
		if (showTimer > 0f && prevShowTimer > 0f && hideTimer <= 0f && prevHideTimer <= 0f)
		{
			if (!animateTheEntireObject)
			{
				if (Object.op_Implicit((Object)(object)uiText) && !((Behaviour)uiText).enabled)
				{
					((Behaviour)uiText).enabled = true;
					AllChildrenSetActive(active: true);
				}
			}
			else
			{
				AllChildrenSetActive(active: true);
			}
			hideTimer = 0f;
			return;
		}
		if (hideTimer <= 0f && prevHideTimer <= 0f && stopHidingTimer <= 0f && prevStopHidingTimer <= 0f && hideAnimationEvaluation == 0f)
		{
			if (!animateTheEntireObject)
			{
				if (Object.op_Implicit((Object)(object)uiText) && !((Behaviour)uiText).enabled)
				{
					((Behaviour)uiText).enabled = true;
					AllChildrenSetActive(active: true);
				}
			}
			else
			{
				AllChildrenSetActive(active: true);
			}
		}
		if (!(hideTimer > 0f) || !(hideAnimationEvaluation >= 1f))
		{
			return;
		}
		if (!animateTheEntireObject)
		{
			if (Object.op_Implicit((Object)(object)uiText) && ((Behaviour)uiText).enabled)
			{
				((Behaviour)uiText).enabled = false;
				AllChildrenSetActive(active: false);
			}
		}
		else
		{
			AllChildrenSetActive(active: false);
		}
	}

	public void SemiUIResetAllShakeEffects()
	{
		shakeTimeX = 0f;
		shakeTimeY = 0f;
		shakeAmountX = 0f;
		shakeAmountY = 0f;
		shakeFrequencyX = 0f;
		shakeFrequencyY = 0f;
		shakeDurationX = 0f;
		shakeDurationY = 0f;
		SpringShakeX = 0f;
		SpringShakeY = 0f;
	}

	private void FlashColorLogic(float deltaTime)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)uiText) && flashColorTime > 0f)
		{
			flashColorTime -= deltaTime;
			((Graphic)uiText).color = flashColor;
			((TMP_Text)uiText).fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, flashColor);
			((TMP_Text)uiText).fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, flashColor);
			if (flashColorTime <= 0f)
			{
				((Graphic)uiText).color = originalTextColor;
				((TMP_Text)uiText).fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, originalFontColor);
				((TMP_Text)uiText).fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, originalGlowColor);
			}
		}
	}

	public void SemiUISpringShakeY(float amount, float frequency, float time)
	{
		shakeTimeY = 0f;
		shakeAmountY = amount;
		shakeFrequencyY = frequency;
		shakeDurationY = time;
	}

	public void SemiUISpringShakeX(float amount, float frequency, float time)
	{
		shakeTimeX = 0f;
		shakeAmountX = amount;
		shakeFrequencyX = frequency;
		shakeDurationX = time;
	}

	public void SemiUITextFlashColor(Color color, float time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		flashColor = color;
		flashColorTime = time;
	}

	private void SpringShakeLogic(float deltaTime)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		if (shakeTimeX < shakeDurationX)
		{
			num = (SpringShakeX = CalculateSpringOffset(shakeTimeX, shakeAmountX, shakeFrequencyX, shakeDurationX));
			shakeTimeX += deltaTime;
		}
		if (shakeTimeY < shakeDurationY)
		{
			num2 = (SpringShakeY = CalculateSpringOffset(shakeTimeY, shakeAmountY, shakeFrequencyY, shakeDurationY));
			shakeTimeY += deltaTime;
		}
		((Component)this).transform.localPosition = initialPosition + new Vector3(num, num2, 0f);
	}

	private float CalculateSpringOffset(float currentTime, float amount, float frequency, float duration)
	{
		float num = currentTime / duration;
		float num2 = frequency * (1f - num);
		return amount * Mathf.Sin(num2 * num * MathF.PI * 2f) * (1f - num);
	}

	public void Hide()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (hideTimer <= 0f && prevHideTimer <= 0f)
		{
			hideAnimationEvaluation = 0f;
			showAnimationEvaluation = 0f;
			animationEval = 0f;
			if (!animateTheEntireObject && Object.op_Implicit((Object)(object)uiText) && !((Behaviour)uiText).enabled)
			{
				((Behaviour)uiText).enabled = false;
				AllChildrenSetActive(active: false);
			}
			hidePositionCurrent = showPosition;
			if (!animateTheEntireObject)
			{
				textRectTransform.localPosition = Vector2.op_Implicit(hidePositionCurrent);
			}
			else
			{
				((Component)this).transform.localPosition = Vector2.op_Implicit(hidePositionCurrent);
			}
		}
		hideTimer = 0.1f;
	}

	public void Show()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (showTimer <= 0f && prevShowTimer <= 0f)
		{
			showAnimationEvaluation = 0f;
			hideAnimationEvaluation = 0f;
			animationEval = 0f;
			if (!animateTheEntireObject)
			{
				if (Object.op_Implicit((Object)(object)uiText) && !((Behaviour)uiText).enabled)
				{
					((Behaviour)uiText).enabled = true;
					AllChildrenSetActive(active: true);
				}
			}
			else
			{
				AllChildrenSetActive(active: true);
			}
			hidePositionCurrent = hidePosition;
			if (!animateTheEntireObject)
			{
				if (Object.op_Implicit((Object)(object)textRectTransform))
				{
					textRectTransform.localPosition = Vector2.op_Implicit(hidePositionCurrent);
				}
			}
			else
			{
				((Component)this).transform.localPosition = Vector2.op_Implicit(hidePositionCurrent);
			}
		}
		showTimer = 0.1f;
	}

	public void SemiUIScoot(Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		scootPosition = position;
		if ((scootTimer <= 0f && prevScootTimer <= 0f) || scootPositionPrev != scootPosition)
		{
			scootEval = 0f;
			scootAnimationEvaluation = 0f;
			scootPositionPrev = scootPosition;
		}
		scootTimer = 0.2f;
	}
}
