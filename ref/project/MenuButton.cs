using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	private enum ButtonState
	{
		Hover,
		Clicked,
		Normal
	}

	public string buttonTextString = "BUTTON";

	internal TextMeshProUGUI buttonText;

	public bool customHoverArea;

	public bool doButtonEffect = true;

	public bool holdLogic = true;

	private Button button;

	internal bool hovering;

	private float hoverTimer;

	private float clickTimer;

	internal bool clicked;

	private float buttonPitch = 1f;

	private string originalText;

	private RectTransform rectTransform;

	public bool hasHold;

	private float holdTimer;

	private float clickFrequency = 0.2f;

	private float clickFrequencyTicker;

	private MenuSelectableElement menuSelectableElement;

	private float buttonPadding;

	private MenuPage parentPage;

	private MenuButtonPopUp menuButtonPopUp;

	public bool middleAlignFix;

	public bool customColors;

	[Header("Custom Colors")]
	public Color colorNormal;

	public Color colorHover;

	public Color colorClick;

	private int buttonState = 2;

	private bool buttonStateStart;

	private float buttonTextSelectedScootPos = 1f;

	private Vector3 buttonTextSelectedOriginalPos;

	internal bool disabled;

	private void Awake()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Invalid comparison between Unknown and I4
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		if (!customColors)
		{
			colorNormal = Color.gray;
			colorHover = Color.white;
			colorClick = AssetManager.instance.colorYellow;
		}
		menuButtonPopUp = ((Component)this).GetComponent<MenuButtonPopUp>();
		menuSelectableElement = ((Component)this).GetComponent<MenuSelectableElement>();
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		button = ((Component)this).GetComponent<Button>();
		buttonText = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
		buttonTextSelectedOriginalPos = ((TMP_Text)buttonText).transform.localPosition;
		if (buttonTextString != "BUTTON")
		{
			((TMP_Text)buttonText).text = buttonTextString;
		}
		originalText = ((TMP_Text)buttonText).text;
		Vector2 sizeDelta = rectTransform.sizeDelta;
		buttonPitch = SemiFunc.MenuGetPitchFromYPos(rectTransform);
		float fontSize = ((TMP_Text)buttonText).fontSize;
		((TMP_Text)buttonText).fontSize = fontSize;
		((TMP_Text)buttonText).enableAutoSizing = false;
		TextAlignmentOptions alignment = ((TMP_Text)buttonText).alignment;
		((TMP_Text)buttonText).alignment = (TextAlignmentOptions)4097;
		buttonPadding = 0f;
		Vector2 sizeDelta2 = rectTransform.sizeDelta;
		rectTransform.sizeDelta = new Vector2(((TMP_Text)buttonText).GetPreferredValues(originalText, 0f, 0f).x + buttonPadding, ((TMP_Text)buttonText).GetPreferredValues(originalText, 0f, 0f).y + buttonPadding / 2f);
		((TMP_Text)buttonText).alignment = alignment;
		if ((int)alignment == 4098)
		{
			((TMP_Text)buttonText).enableAutoSizing = true;
		}
		if (middleAlignFix)
		{
			RectTransform obj = rectTransform;
			((Transform)obj).position = ((Transform)obj).position + new Vector3((sizeDelta2.x - rectTransform.sizeDelta.x) / 2f, 0f, 0f);
			((TMP_Text)buttonText).enableAutoSizing = false;
		}
		if (customHoverArea)
		{
			rectTransform.sizeDelta = sizeDelta;
		}
	}

	private void Update()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)((Selectable)button).image).color = new Color(0f, 0f, 0f, 0f);
		HoverLogic();
		switch (buttonState)
		{
		case 2:
			ButtonNormal();
			buttonStateStart = false;
			break;
		case 0:
			ButtonHover();
			buttonStateStart = false;
			break;
		case 1:
			ButtonClicked();
			buttonStateStart = false;
			break;
		}
		if (hoverTimer > 0f)
		{
			hoverTimer -= Time.deltaTime;
		}
		void ButtonClicked()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			_ = buttonStateStart;
			HoldTimer();
			((Graphic)buttonText).color = colorClick;
		}
		void ButtonHover()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (buttonStateStart)
			{
				MenuManager.instance.MenuEffectHover(buttonPitch);
			}
			HoldTimer();
			((Graphic)buttonText).color = colorHover;
		}
		void ButtonNormal()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			_ = buttonStateStart;
			holdTimer = 0f;
			((TMP_Text)buttonText).transform.localPosition = buttonTextSelectedOriginalPos;
			((Graphic)buttonText).color = colorNormal;
		}
	}

	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck() && buttonTextString != "BUTTON")
		{
			buttonText = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
			if (((TMP_Text)buttonText).text != buttonTextString)
			{
				((TMP_Text)buttonText).text = buttonTextString;
			}
			if (((Object)((Component)this).gameObject).name != "Menu Button - " + buttonTextString)
			{
				((Object)((Component)this).gameObject).name = "Menu Button - " + buttonTextString;
			}
		}
	}

	private void HoverLogic()
	{
		int num = 0;
		if (!customHoverArea)
		{
			num = 10;
		}
		if (SemiFunc.UIMouseHover(parentPage, rectTransform, menuSelectableElement.menuID, num))
		{
			if (!hovering)
			{
				OnHoverStart();
				hovering = true;
			}
			hoverTimer = 0.01f;
		}
		if (hovering || (clicked && hovering))
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnSelect();
				holdTimer = 0f;
				clickTimer = 0.2f;
			}
			if (hasHold)
			{
				if (Input.GetMouseButton(0))
				{
					holdTimer += Time.deltaTime;
				}
				else
				{
					holdTimer = 0f;
					clickFrequencyTicker = 0f;
					clickFrequency = 0.2f;
				}
			}
		}
		if (clickTimer > 0f)
		{
			clickTimer -= Time.deltaTime;
			clicked = true;
		}
		else
		{
			if (clicked)
			{
				OnSelectEnd();
			}
			clicked = false;
		}
		if (hoverTimer <= 0f)
		{
			if (hovering)
			{
				OnHoverEnd();
			}
			hovering = false;
		}
		if (hoverTimer > 0f)
		{
			_ = hovering;
			OnHovering();
			hovering = true;
		}
	}

	private void ButtonStateSet(int state)
	{
		buttonState = state;
		buttonStateStart = true;
	}

	private void OnHoverStart()
	{
		ButtonStateSet(0);
		buttonStateStart = true;
	}

	private void OnHovering()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)buttonText).transform.localPosition = new Vector3(buttonTextSelectedOriginalPos.x, buttonTextSelectedOriginalPos.y + buttonTextSelectedScootPos, buttonTextSelectedOriginalPos.z);
		Vector2 sizeDelta = rectTransform.sizeDelta;
		_ = new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f, 0f) + (((Component)this).transform.localPosition - new Vector3(buttonPadding / 2f, 0f, 0f));
		SemiFunc.MenuSelectionBoxTargetSet(parentPage, rectTransform);
	}

	private void OnHoverEnd()
	{
		ButtonStateSet(2);
		buttonStateStart = true;
	}

	private void OnSelect()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (disabled)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny);
			return;
		}
		if (doButtonEffect)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, parentPage);
		}
		ButtonStateSet(1);
		if (!Object.op_Implicit((Object)(object)menuButtonPopUp))
		{
			((UnityEvent)button.onClick).Invoke();
		}
		else
		{
			MenuManager.instance.PagePopUpTwoOptions(menuButtonPopUp, menuButtonPopUp.headerText, menuButtonPopUp.headerColor, menuButtonPopUp.bodyText, menuButtonPopUp.option1Text, menuButtonPopUp.option2Text);
		}
	}

	private void OnSelectEnd()
	{
		if (!hovering)
		{
			ButtonStateSet(2);
		}
		else
		{
			ButtonStateSet(0);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnHoverStart();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnHoverEnd();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnSelect();
	}

	private void HoldTimer()
	{
		if (holdLogic && holdTimer > 0.5f)
		{
			if (clickFrequencyTicker <= 0f)
			{
				OnSelect();
				clickFrequencyTicker = clickFrequency;
				clickFrequency -= clickFrequency * 0.2f;
				clickFrequency = Mathf.Clamp(clickFrequency, 0.025f, 0.2f);
			}
			else
			{
				clickFrequencyTicker -= Time.deltaTime;
			}
		}
	}
}
