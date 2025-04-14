using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuPage : MonoBehaviour
{
	public enum PageState
	{
		Opening,
		Active,
		Closing,
		Inactive,
		Activating,
		Closed
	}

	public string menuHeaderName;

	public MenuPageIndex menuPageIndex;

	private Vector2 originalPosition;

	internal RectTransform rectTransform;

	private Vector2 animateAwayPosition = new Vector2(0f, 0f);

	private Vector2 targetPosition;

	internal float bottomElementYPos;

	internal List<MenuSelectableElement> selectableElements = new List<MenuSelectableElement>();

	public TextMeshProUGUI menuHeader;

	internal bool pageIsOnTopOfOtherPage;

	internal MenuPage pageUnderThisPage;

	internal bool pageActive;

	private float pageActiveTimer;

	internal bool popUpAnimation;

	internal MenuSelectionBox selectionBox;

	private float stateTimer;

	internal bool addedPageOnTop;

	internal MenuPage parentPage;

	public bool disableIntroAnimation;

	public bool disableOutroAnimation;

	internal List<MenuSettingElement> settingElements = new List<MenuSettingElement>();

	internal int currentActiveSettingElement = -1;

	private float activeSettingElementTimer;

	public UnityEvent onPageEnd;

	internal int scrollBoxes;

	private bool stateStart = true;

	internal PageState currentPageState;

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		selectionBox = ((Component)this).GetComponentInChildren<MenuSelectionBox>();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		originalPosition = Vector2.op_Implicit(((Transform)rectTransform).localPosition);
		float x = originalPosition.x;
		float y = originalPosition.y;
		Rect rect = rectTransform.rect;
		animateAwayPosition = new Vector2(x, y - ((Rect)(ref rect)).height);
		RectTransform obj = rectTransform;
		float x2 = originalPosition.x;
		float y2 = originalPosition.y;
		rect = rectTransform.rect;
		((Transform)obj).localPosition = Vector2.op_Implicit(new Vector2(x2, y2 + ((Rect)(ref rect)).height));
		MenuManager.instance.PageAdd(this);
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		if (!Object.op_Implicit((Object)(object)parentPage))
		{
			parentPage = this;
		}
	}

	private void FixedUpdate()
	{
		if (pageActiveTimer <= 0f)
		{
			pageActive = false;
		}
		if (pageActiveTimer > 0f)
		{
			pageActive = true;
			pageActiveTimer -= Time.fixedDeltaTime;
		}
	}

	private void Update()
	{
		switch (currentPageState)
		{
		case PageState.Opening:
			StateOpening();
			stateStart = false;
			break;
		case PageState.Active:
			StateActive();
			stateStart = false;
			break;
		case PageState.Closing:
			StateClosing();
			stateStart = false;
			break;
		case PageState.Inactive:
			StateInactive();
			stateStart = false;
			break;
		case PageState.Activating:
			StateActivating();
			stateStart = false;
			break;
		}
		if (activeSettingElementTimer > 0f)
		{
			activeSettingElementTimer -= Time.deltaTime;
			if (activeSettingElementTimer <= 0f)
			{
				currentActiveSettingElement = -1;
			}
		}
	}

	private void OnEnable()
	{
		ResetPage();
	}

	public void ResetPage()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)rectTransform))
		{
			RectTransform obj = rectTransform;
			float x = originalPosition.x;
			float y = originalPosition.y;
			Rect rect = rectTransform.rect;
			((Transform)obj).localPosition = Vector2.op_Implicit(new Vector2(x, y + ((Rect)(ref rect)).height));
		}
	}

	public void PageStateSet(PageState pageState)
	{
		stateTimer = 0f;
		currentPageState = pageState;
		stateStart = true;
	}

	private void StateOpening()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			if (!popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageIntro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpOpen();
			}
			LockAndHide();
		}
		if (!addedPageOnTop)
		{
			MenuSelectionBox.instance.firstSelection = true;
		}
		if (Vector2.Distance(Vector2.op_Implicit(((Transform)rectTransform).localPosition), originalPosition) < 0.8f)
		{
			PageStateSet(PageState.Active);
		}
		if (!disableIntroAnimation)
		{
			float deltaTime = Time.deltaTime;
			((Transform)rectTransform).localPosition = Vector2.op_Implicit(Vector2.Lerp(Vector2.op_Implicit(((Transform)rectTransform).localPosition), originalPosition, 40f * deltaTime));
		}
		LockAndHide();
	}

	private void StateActive()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart && !disableIntroAnimation)
		{
			((Transform)rectTransform).localPosition = Vector2.op_Implicit(originalPosition);
		}
		if (!disableIntroAnimation)
		{
			((Transform)rectTransform).localPosition = Vector2.op_Implicit(originalPosition);
		}
		PageAddedOnTopLogic();
		MenuSelectionBox instance = MenuSelectionBox.instance;
		if (!Object.op_Implicit((Object)(object)instance) || (Object)(object)instance != (Object)(object)selectionBox)
		{
			selectionBox.Reinstate();
		}
		LockAndHide();
		if (MenuManager.instance.currentMenuPageIndex != menuPageIndex)
		{
			PageStateSet(PageState.Inactive);
		}
		pageActive = true;
		pageActiveTimer = 0.1f;
	}

	public bool SettingElementActiveCheckFree(int index)
	{
		if (currentActiveSettingElement != -1)
		{
			return currentActiveSettingElement == index;
		}
		return true;
	}

	private void StateClosing()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		LockAndHide();
		if (stateStart)
		{
			if (!popUpAnimation)
			{
				MenuManager.instance.MenuEffectPageOutro();
			}
			else
			{
				MenuManager.instance.MenuEffectPopUpClose();
			}
			if ((Object)(object)MenuManager.instance.currentMenuPage == (Object)(object)this)
			{
				MenuManager.instance.currentMenuPage = null;
				MenuManager.instance.PageRemove(this);
			}
		}
		if (Vector2.Distance(Vector2.op_Implicit(((Transform)rectTransform).localPosition), animateAwayPosition) < 0.8f)
		{
			onPageEnd.Invoke();
			MenuManager.instance.PageRemove(this);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		float deltaTime = Time.deltaTime;
		((Transform)rectTransform).localPosition = Vector2.op_Implicit(Vector2.Lerp(Vector2.op_Implicit(((Transform)rectTransform).localPosition), animateAwayPosition, 40f * deltaTime));
	}

	private void StateInactive()
	{
		_ = stateStart;
		if (MenuManager.instance.currentMenuPageIndex == menuPageIndex)
		{
			PageStateSet(PageState.Active);
		}
	}

	private void PageAddedOnTopLogic()
	{
		if (currentPageState == PageState.Opening || currentPageState == PageState.Closing || !addedPageOnTop)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)parentPage))
		{
			if (currentPageState != parentPage.currentPageState)
			{
				PageStateSet(parentPage.currentPageState);
			}
		}
		else if (currentPageState != PageState.Closing)
		{
			PageStateSet(PageState.Closing);
		}
	}

	private void StateActivating()
	{
		_ = stateStart;
		if (stateTimer > 0.1f)
		{
			PageStateSet(PageState.Active);
		}
		stateTimer += Time.deltaTime;
	}

	public void SettingElementActiveSet(int index)
	{
		if (currentActiveSettingElement == -1)
		{
			currentActiveSettingElement = index;
		}
		activeSettingElementTimer = 0.1f;
	}

	private void LockAndHide()
	{
		SemiFunc.UIHideAim();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideGoal();
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideHaul();
		SemiFunc.UIHideCurrency();
		SemiFunc.UIHideShopCost();
		SemiFunc.UIHideTumble();
		SemiFunc.UIHideWorldSpace();
		SemiFunc.UIHideValuableDiscover();
		SemiFunc.CameraOverrideStopAim();
	}
}
