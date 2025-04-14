using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuSlider : MonoBehaviour
{
	[Serializable]
	public class CustomOption
	{
		[Space(25f)]
		[Header("____ Custom Option ____")]
		public string customOptionText;

		public UnityEvent onOption;

		public int customValueInt;
	}

	public string elementName = "Element Name";

	public TextMeshProUGUI elementNameText;

	public Transform sliderBG;

	public Transform barSize;

	public Transform barPointer;

	public RectTransform barSizeRectTransform;

	public Transform settingsBar;

	public Transform extraBar;

	private int settingSegments;

	public int startValue;

	public int endValue;

	public string stringAtStartOfValue;

	public string stringAtEndOfValue;

	internal int currentValue;

	internal int prevCurrentValue;

	internal bool valueChangedImpulse;

	public int buttonSegmentJump = 1;

	public int pointerSegmentJump = 1;

	internal float settingsValue = 1f;

	internal float prevSettingsValue = 1f;

	public TextMeshProUGUI segmentText;

	public TextMeshProUGUI segmentMaskText;

	public RectTransform maskRectTransform;

	public bool wrapAround;

	public bool hasBar = true;

	public bool hasCustomOptions;

	private MenuSelectableElement menuSelectableElement;

	private bool hovering;

	private RectTransform rectTransform;

	private float sneakyOffsetBecauseIWasLazy = 3f;

	private MenuPage parentPage;

	internal MenuSetting menuSetting;

	private DataDirector.Setting setting;

	private bool inputSetting;

	private MenuInputPercentSetting inputPercentSetting;

	private Vector3 originalPosition;

	private Vector3 originalPositionBarSize;

	private Vector3 originalPositionBarBG;

	private RectTransform barBGRectTransform;

	private string prevSettingString = "";

	private bool hasBigSettingText;

	internal MenuBigSettingText bigSettingText;

	private int customValue;

	private int customValueNull = -123456;

	public bool hasCustomValues;

	private bool startPositionSetup;

	[Space]
	public UnityEvent onChange;

	public List<CustomOption> customOptions;

	private float extraBarActiveTimer;

	public void Start()
	{
		inputPercentSetting = ((Component)this).GetComponent<MenuInputPercentSetting>();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		if (Object.op_Implicit((Object)(object)inputPercentSetting))
		{
			inputSetting = true;
		}
		menuSetting = ((Component)this).GetComponent<MenuSetting>();
		if (Object.op_Implicit((Object)(object)menuSetting))
		{
			menuSetting.FetchValues();
			int settingValue = menuSetting.settingValue;
			if (hasCustomOptions)
			{
				int indexFromCustomValue = GetIndexFromCustomValue(menuSetting.settingValue);
				menuSetting.settingValue = indexFromCustomValue;
				settingValue = menuSetting.settingValue;
			}
			settingsValue = (float)settingValue / 100f;
			setting = menuSetting.setting;
			elementName = menuSetting.settingName;
			((TMP_Text)elementNameText).text = elementName;
		}
		bigSettingText = ((Component)this).GetComponentInChildren<MenuBigSettingText>();
		if (Object.op_Implicit((Object)(object)bigSettingText))
		{
			hasBigSettingText = true;
		}
		prevSettingString = "";
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		if (Object.op_Implicit((Object)(object)elementNameText))
		{
			((TMP_Text)elementNameText).text = elementName;
		}
		settingSegments = endValue - startValue;
		menuSelectableElement = ((Component)this).GetComponent<MenuSelectableElement>();
		if (hasCustomOptions)
		{
			settingSegments = Mathf.Max(customOptions.Count - 1, 1);
			startValue = 0;
			endValue = customOptions.Count - 1;
			buttonSegmentJump = 1;
			settingsValue = settingsValue / (float)settingSegments * 100f;
		}
		barSizeRectTransform = ((Component)barSize).GetComponent<RectTransform>();
		if (hasCustomOptions)
		{
			if (Mathf.Max(customOptions.Count - 1, 1) != settingSegments)
			{
				Debug.LogWarning((object)"Segment text count is not equal to setting segments count");
			}
			else
			{
				int index = Mathf.RoundToInt(settingsValue * (float)settingSegments);
				string text = customOptions[index].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				((TMP_Text)segmentText).text = text;
			}
		}
		else
		{
			currentValue = Mathf.RoundToInt(Mathf.Lerp((float)startValue, (float)endValue, settingsValue));
			((TMP_Text)segmentText).text = stringAtStartOfValue + currentValue + stringAtEndOfValue;
		}
		((TMP_Text)segmentText).enableAutoSizing = false;
		((TMP_Text)segmentMaskText).enableAutoSizing = false;
		if (!hasBar && Object.op_Implicit((Object)(object)segmentText))
		{
			Object.Destroy((Object)(object)((Component)segmentText).gameObject);
		}
		SetStartPositions();
	}

	public int GetIndexFromCustomValue(int value)
	{
		int result = 0;
		for (int i = 0; i < customOptions.Count; i++)
		{
			if (customOptions[i].customValueInt == value)
			{
				return i;
			}
		}
		return result;
	}

	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			elementNameText = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
			((TMP_Text)elementNameText).text = elementName;
			((Object)((Component)this).gameObject).name = "Slider - " + elementName;
		}
	}

	public void SetStartPositions()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (!startPositionSetup)
		{
			startPositionSetup = true;
			((Transform)barSizeRectTransform).localPosition = new Vector3(((Transform)barSizeRectTransform).localPosition.x + sneakyOffsetBecauseIWasLazy, ((Transform)barSizeRectTransform).localPosition.y, ((Transform)barSizeRectTransform).localPosition.z);
			originalPosition = ((Transform)rectTransform).position;
			originalPositionBarBG = ((Transform)((Component)sliderBG).GetComponent<RectTransform>()).position;
			originalPositionBarSize = ((Component)barSizeRectTransform).transform.position;
			originalPosition = new Vector3(originalPosition.x, originalPosition.y - 1.01f, originalPosition.z);
			barBGRectTransform = ((Component)sliderBG).GetComponent<RectTransform>();
		}
	}

	public string CustomOptionGetCurrentString()
	{
		return customOptions[currentValue].customOptionText;
	}

	public void CustomOptionAdd(string optionText, UnityEvent onOption)
	{
		customOptions.Add(new CustomOption
		{
			customOptionText = optionText,
			onOption = onOption
		});
		settingSegments = Mathf.Max(customOptions.Count - 1, 1);
		startValue = 0;
		endValue = customOptions.Count - 1;
		buttonSegmentJump = 1;
	}

	private void Update()
	{
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		if (hasBigSettingText && prevSettingString != ((TMP_Text)segmentText).text)
		{
			int index = Mathf.RoundToInt(settingsValue * (float)settingSegments);
			((TMP_Text)bigSettingText.textMeshPro).text = customOptions[index].customOptionText;
			prevSettingString = ((TMP_Text)segmentText).text;
		}
		if (prevCurrentValue != currentValue || valueChangedImpulse)
		{
			valueChangedImpulse = false;
			float num = Mathf.Round(settingsValue * 100f);
			if (customOptions.Count > 0)
			{
				num = Mathf.RoundToInt(settingsValue * (float)settingSegments);
			}
			if (Object.op_Implicit((Object)(object)menuSetting))
			{
				if (!hasCustomOptions)
				{
					DataDirector.instance.SettingValueSet(setting, (int)num);
				}
				else if (hasCustomValues)
				{
					CustomOption customOption = customOptions[currentValue];
					DataDirector.instance.SettingValueSet(setting, customOption.customValueInt);
					customOptions[currentValue].onOption.Invoke();
				}
				else
				{
					DataDirector.instance.SettingValueSet(setting, (int)num);
				}
			}
			if (inputSetting)
			{
				InputManager.instance.inputPercentSettings[inputPercentSetting.setting] = (int)num;
			}
			onChange.Invoke();
			prevCurrentValue = currentValue;
		}
		if (extraBarActiveTimer > 0f)
		{
			extraBarActiveTimer -= Time.deltaTime;
		}
		else if (((Component)extraBar).gameObject.activeSelf)
		{
			((Component)extraBar).gameObject.SetActive(false);
		}
		if (hasBar)
		{
			settingsBar.localScale = Vector3.Lerp(settingsBar.localScale, new Vector3(settingsValue, settingsBar.localScale.y, settingsBar.localScale.z), 20f * Time.deltaTime);
			maskRectTransform.sizeDelta = new Vector2(barSizeRectTransform.sizeDelta.x * settingsValue, maskRectTransform.sizeDelta.y);
		}
		Vector3 mousePosition = Input.mousePosition;
		float num2 = (float)(Screen.width / MenuManager.instance.screenUIWidth) * 1.05f;
		float num3 = (float)(Screen.height / MenuManager.instance.screenUIHeight) * 1f;
		((Vector3)(ref mousePosition))._002Ector(mousePosition.x / num2, mousePosition.y / num3, 0f);
		if (SemiFunc.UIMouseHover(parentPage, barSizeRectTransform, menuSelectableElement.menuID, 5f, 5f))
		{
			if (!hovering)
			{
				MenuManager.instance.MenuEffectHover(SemiFunc.MenuGetPitchFromYPos(rectTransform));
			}
			hovering = true;
			int num4 = 10;
			new Vector3(((Transform)barSizeRectTransform).localPosition.x + barSizeRectTransform.sizeDelta.x / 2f - sneakyOffsetBecauseIWasLazy, ((Transform)barSizeRectTransform).localPosition.y + (float)(num4 / 2), ((Transform)barSizeRectTransform).localPosition.z);
			new Vector2(barSizeRectTransform.sizeDelta.x + (float)num4, barSizeRectTransform.sizeDelta.y + (float)num4);
			SemiFunc.MenuSelectionBoxTargetSet(parentPage, barSizeRectTransform, new Vector2(-3f, 0f), new Vector2(20f, 10f));
			if (hasBar)
			{
				PointerLogic(mousePosition);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, parentPage);
				OnIncrease();
			}
		}
		else
		{
			hovering = false;
			if (((Component)barPointer).gameObject.activeSelf)
			{
				barPointer.localPosition = new Vector3(-999f, barPointer.localPosition.y, barPointer.localPosition.z);
				((Component)barPointer).gameObject.SetActive(false);
			}
		}
		if (Object.op_Implicit((Object)(object)segmentMaskText) && ((TMP_Text)segmentMaskText).text != ((TMP_Text)segmentText).text)
		{
			((TMP_Text)segmentMaskText).text = ((TMP_Text)segmentText).text;
		}
	}

	public void ExtraBarSet(float value)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Component)extraBar).gameObject.activeSelf)
		{
			((Component)extraBar).gameObject.SetActive(true);
		}
		value = Mathf.Clamp(value, 0f, 1f);
		extraBar.localScale = new Vector3(value, extraBar.localScale.y, extraBar.localScale.z);
		extraBarActiveTimer = 0.2f;
	}

	public void SetBar(float value)
	{
		settingsValue = Mathf.Clamp(value, 0f, 1f);
		int num = Mathf.RoundToInt(settingsValue * (float)settingSegments);
		currentValue = Mathf.RoundToInt(Mathf.Lerp((float)startValue, (float)endValue, settingsValue));
		if (hasCustomOptions)
		{
			customValue = GetCustomValue(num);
			if (num < customOptions.Count)
			{
				string text = customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				((TMP_Text)segmentText).text = text;
			}
		}
		else
		{
			((TMP_Text)segmentText).text = stringAtStartOfValue + currentValue + stringAtEndOfValue;
		}
	}

	public int GetCustomValue(int index)
	{
		if (!hasCustomOptions)
		{
			return customValueNull;
		}
		if (customOptions.Count == 0)
		{
			return customValueNull;
		}
		if (index >= customOptions.Count)
		{
			return customValueNull;
		}
		if (index < 0)
		{
			return customValueNull;
		}
		if (!hasCustomValues)
		{
			return customValueNull;
		}
		return customOptions[index].customValueInt;
	}

	private void PointerLogic(Vector3 mouseScreenPosition)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)barPointer))
		{
			return;
		}
		if (!((Component)barPointer).gameObject.activeSelf)
		{
			((Component)barPointer).gameObject.SetActive(true);
		}
		Vector2 val = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(barSizeRectTransform);
		int num = (endValue - startValue) / pointerSegmentJump;
		SemiFunc.UIGetRectTransformPositionOnScreen(barSizeRectTransform);
		float num2 = Mathf.Clamp01(val.x / barSizeRectTransform.sizeDelta.x);
		num2 = Mathf.Round(num2 * (float)num) / (float)num;
		float num3 = Mathf.Clamp(((Transform)barSizeRectTransform).localPosition.x + num2 * barSizeRectTransform.sizeDelta.x, ((Transform)barSizeRectTransform).localPosition.x, ((Transform)barSizeRectTransform).localPosition.x + barSizeRectTransform.sizeDelta.x);
		barPointer.localPosition = new Vector3(num3 - 2f, barPointer.localPosition.y, barPointer.localPosition.z);
		if (Input.GetMouseButton(0))
		{
			prevSettingsValue = settingsValue;
			settingsValue = num2;
			if (prevSettingsValue != settingsValue)
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, parentPage);
			}
			int num4 = Mathf.RoundToInt(settingsValue * (float)num);
			if (hasCustomOptions && num4 < customOptions.Count)
			{
				((TMP_Text)segmentText).text = customOptions[num4].customOptionText;
			}
			else
			{
				((TMP_Text)segmentText).text = stringAtStartOfValue + currentValue + stringAtEndOfValue;
			}
			currentValue = Mathf.RoundToInt(Mathf.Lerp((float)startValue, (float)endValue, settingsValue));
			if (hasCustomOptions)
			{
				UpdateSegmentTextAndValue();
				customValue = GetCustomValue(num4);
			}
		}
	}

	public void UpdateSegmentTextAndValue()
	{
		int num = Mathf.RoundToInt(settingsValue * (float)settingSegments);
		currentValue = Mathf.RoundToInt(Mathf.Lerp((float)startValue, (float)endValue, settingsValue));
		if (hasCustomOptions)
		{
			customValue = GetCustomValue(num);
			if (num < customOptions.Count)
			{
				string text = customOptions[num].customOptionText;
				if (text.Length > 16)
				{
					text = text.Substring(0, 16) + "...";
				}
				((TMP_Text)segmentText).text = text;
			}
		}
		else
		{
			((TMP_Text)segmentText).text = stringAtStartOfValue + currentValue + stringAtEndOfValue;
		}
	}

	public void OnIncrease()
	{
		valueChangedImpulse = true;
		prevSettingsValue = settingsValue;
		float num = settingsValue;
		settingsValue += 1f / (float)settingSegments * (float)buttonSegmentJump;
		if (wrapAround)
		{
			settingsValue = ((num == 1f) ? 0f : Mathf.Clamp01(settingsValue));
		}
		else
		{
			settingsValue = Mathf.Clamp(settingsValue, 0f, 1f);
		}
		UpdateSegmentTextAndValue();
	}

	public void OnDecrease()
	{
		valueChangedImpulse = true;
		prevSettingsValue = settingsValue;
		float num = settingsValue;
		settingsValue -= 1f / (float)settingSegments * (float)buttonSegmentJump;
		if (wrapAround)
		{
			settingsValue = ((settingsValue + num < 0f) ? 1f : Mathf.Clamp01(settingsValue));
		}
		else
		{
			settingsValue = Mathf.Clamp(settingsValue, 0f, 1f);
		}
		UpdateSegmentTextAndValue();
	}

	public void SetBarScaleInstant()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		settingsBar.localScale = new Vector3(settingsValue, settingsBar.localScale.y, settingsBar.localScale.z);
	}
}
