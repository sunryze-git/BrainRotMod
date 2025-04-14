using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	[Serializable]
	public class MenuPages
	{
		public MenuPageIndex menuPageIndex;

		public GameObject menuPage;
	}

	public enum MenuState
	{
		Open,
		Closed
	}

	public enum MenuClickEffectType
	{
		Action,
		Confirm,
		Deny,
		Dud,
		Tick
	}

	public static MenuManager instance;

	internal MenuSelectionBox selectionBox;

	internal MenuSelectionBox activeSelectionBox;

	internal List<MenuPlayerHead> playerHeads = new List<MenuPlayerHead>();

	private List<MenuSelectionBox> selectionBoxes = new List<MenuSelectionBox>();

	internal string currentMenuID = "";

	public List<MenuPages> menuPages;

	internal MenuPageIndex currentMenuPageIndex;

	internal MenuPage currentMenuPage;

	internal int currentMenuState;

	private bool stateStart;

	internal MenuButton currentButton;

	internal int fetchSetting;

	private Vector3 soundPosition;

	private float menuHover;

	public Sound soundAction;

	public Sound soundConfirm;

	public Sound soundDeny;

	public Sound soundDud;

	public Sound soundTick;

	public Sound soundHover;

	public Sound soundPageIntro;

	public Sound soundPageOutro;

	public Sound soundWindowPopUp;

	public Sound soundWindowPopUpClose;

	public Sound soundMove;

	internal Vector2 mouseHoldPosition;

	internal int screenUIWidth = 720;

	internal int screenUIHeight = 405;

	internal List<MenuPage> allPages = new List<MenuPage>();

	internal List<MenuPage> inactivePages = new List<MenuPage>();

	internal List<MenuPage> addedPagesOnTop = new List<MenuPage>();

	private bool pagePopUpScheduled;

	private string pagePopUpScheduledHeaderText;

	private Color pagePopUpScheduledHeaderColor;

	private string pagePopUpScheduledBodyText;

	private string pagePopUpScheduledButtonText;

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
		StateSet(MenuState.Closed);
	}

	private void Update()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)PlayerController.instance))
		{
			soundPosition = ((Component)PlayerController.instance).transform.position;
		}
		else
		{
			soundPosition = ((Component)this).transform.position;
		}
		switch (currentMenuState)
		{
		case 0:
			StateOpen();
			stateStart = false;
			break;
		case 1:
			StateClosed();
			stateStart = false;
			break;
		}
		if (Input.GetMouseButton(0))
		{
			if (mouseHoldPosition == Vector2.zero)
			{
				mouseHoldPosition = SemiFunc.UIMousePosToUIPos();
			}
		}
		else
		{
			mouseHoldPosition = Vector2.zero;
		}
	}

	private void FixedUpdate()
	{
		if (menuHover > 0f)
		{
			menuHover -= Time.fixedDeltaTime;
		}
		else
		{
			currentMenuID = "";
		}
	}

	public void SetState(int state)
	{
		currentMenuState = state;
		stateStart = true;
	}

	public void MenuEffectHover(float pitch = -1f, float volume = -1f)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (pitch != -1f)
		{
			soundHover.Pitch = pitch;
		}
		if (volume != -1f)
		{
			soundHover.Volume = volume;
		}
		soundHover.Play(((Component)this).transform.position);
	}

	public void MenuEffectClick(MenuClickEffectType effectType, MenuPage parentPage = null, float pitch = -1f, float volume = -1f, bool soundOnly = false)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		switch (effectType)
		{
		case MenuClickEffectType.Action:
			if (!soundOnly && Object.op_Implicit((Object)(object)activeSelectionBox))
			{
				activeSelectionBox.SetClick(AssetManager.instance.colorYellow);
			}
			if (pitch != -1f)
			{
				soundAction.Pitch = pitch;
			}
			if (volume != -1f)
			{
				soundAction.Volume = volume;
			}
			soundAction.Play(soundPosition);
			break;
		case MenuClickEffectType.Confirm:
			if (!soundOnly && Object.op_Implicit((Object)(object)activeSelectionBox))
			{
				activeSelectionBox.SetClick(Color.green);
			}
			if (pitch != -1f)
			{
				soundConfirm.Pitch = pitch;
			}
			if (volume != -1f)
			{
				soundConfirm.Volume = volume;
			}
			soundConfirm.Play(soundPosition);
			break;
		case MenuClickEffectType.Deny:
			if (!soundOnly && Object.op_Implicit((Object)(object)activeSelectionBox))
			{
				activeSelectionBox.SetClick(Color.red);
			}
			if (pitch != -1f)
			{
				soundDeny.Pitch = pitch;
			}
			if (volume != -1f)
			{
				soundDeny.Volume = volume;
			}
			soundDeny.Play(soundPosition);
			break;
		case MenuClickEffectType.Dud:
			if (!soundOnly)
			{
				activeSelectionBox.SetClick(Color.gray);
			}
			if (pitch != -1f)
			{
				soundDud.Pitch = pitch;
			}
			if (volume != -1f)
			{
				soundDud.Volume = volume;
			}
			soundDud.Play(soundPosition);
			break;
		case MenuClickEffectType.Tick:
			if (!soundOnly)
			{
				Color click = default(Color);
				((Color)(ref click))._002Ector(0f, 0.5f, 1f, 1f);
				if (!Object.op_Implicit((Object)(object)parentPage))
				{
					if (Object.op_Implicit((Object)(object)MenuSelectionBox.instance))
					{
						MenuSelectionBox.instance.SetClick(click);
					}
				}
				else if (Object.op_Implicit((Object)(object)parentPage.selectionBox))
				{
					parentPage.selectionBox.SetClick(click);
				}
			}
			if (pitch != -1f)
			{
				soundTick.Pitch = pitch;
			}
			if (volume != -1f)
			{
				soundTick.Volume = volume;
			}
			soundTick.Play(soundPosition);
			break;
		}
	}

	public void MenuEffectPopUpOpen()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		soundWindowPopUp.Play(soundPosition);
	}

	public void MenuEffectPopUpClose()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		soundWindowPopUpClose.Play(soundPosition);
	}

	public void MenuEffectPageIntro()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		soundPageIntro.Play(soundPosition);
	}

	public void MenuEffectPageOutro()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		soundPageOutro.Play(soundPosition);
	}

	public void MenuEffectMove()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		soundMove.Play(soundPosition);
	}

	private void StateOpen()
	{
		_ = stateStart;
		SemiFunc.CursorUnlock(0.1f);
		PlayerController.instance.InputDisableTimer = 0.1f;
		if (!Object.op_Implicit((Object)(object)currentMenuPage))
		{
			StateSet(MenuState.Closed);
		}
	}

	private void StateClosed()
	{
		_ = stateStart;
		if (Object.op_Implicit((Object)(object)currentMenuPage))
		{
			StateSet(MenuState.Open);
		}
	}

	public void PageAdd(MenuPage menuPage)
	{
		if (!allPages.Contains(menuPage))
		{
			allPages.Add(menuPage);
		}
	}

	public void PageRemove(MenuPage menuPage)
	{
		if (allPages.Contains(menuPage))
		{
			allPages.Remove(menuPage);
		}
	}

	public MenuPage PageOpen(MenuPageIndex menuPageIndex, bool addedPageOnTop = false)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		MenuPages menuPages = this.menuPages.Find((MenuPages x) => x.menuPageIndex == menuPageIndex);
		if (menuPages == null)
		{
			Debug.LogError((object)"Page not found");
			return null;
		}
		GameObject obj = Object.Instantiate<GameObject>(menuPages.menuPage);
		MenuPage component = obj.GetComponent<MenuPage>();
		obj.transform.SetParent(((Component)MenuHolder.instance).transform);
		((Transform)obj.GetComponent<RectTransform>()).localPosition = Vector2.op_Implicit(new Vector2(0f, 0f));
		((Transform)obj.GetComponent<RectTransform>()).localScale = new Vector3(1f, 1f, 1f);
		component.addedPageOnTop = addedPageOnTop;
		if (!addedPageOnTop)
		{
			instance.PageSetCurrent(menuPageIndex, component);
		}
		else
		{
			component.parentPage = currentMenuPage;
		}
		return component;
	}

	public void PageClose(MenuPageIndex menuPageIndex)
	{
		if (menuPages.Find((MenuPages x) => x.menuPageIndex == menuPageIndex) != null)
		{
			MenuPage menuPage = allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex);
			if (!((Object)(object)menuPage == (Object)null))
			{
				menuPage.PageStateSet(MenuPage.PageState.Closing);
				allPages.Remove(menuPage);
			}
		}
	}

	public bool PageCheck(MenuPageIndex menuPageIndex)
	{
		return (Object)(object)allPages.Find((MenuPage x) => x.menuPageIndex == menuPageIndex) != (Object)null;
	}

	public void PageSwap(MenuPageIndex menuPageIndex)
	{
		currentMenuPage.PageStateSet(MenuPage.PageState.Closing);
		PageOpen(menuPageIndex);
	}

	public MenuPage PageOpenOnTop(MenuPageIndex menuPageIndex)
	{
		MenuPage menuPage = currentMenuPage;
		PageInactiveAdd(menuPage);
		currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage2 = PageOpen(menuPageIndex);
		menuPage2.pageIsOnTopOfOtherPage = true;
		menuPage2.pageUnderThisPage = menuPage;
		return menuPage2;
	}

	public void PageAddOnTop(MenuPageIndex menuPageIndex)
	{
		if (!addedPagesOnTop.Contains(currentMenuPage))
		{
			_ = currentMenuPage;
			MenuPage item = PageOpen(menuPageIndex, addedPageOnTop: true);
			if (!addedPagesOnTop.Contains(currentMenuPage))
			{
				addedPagesOnTop.Add(item);
			}
		}
	}

	public void PagePopUpTwoOptions(MenuButtonPopUp menuButtonPopUp, string popUpHeader, Color popUpHeaderColor, string popUpText, string option1Text, string option2Text)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUpTwoOptions;
		MenuPage pageUnderThisPage = currentMenuPage;
		currentMenuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage = PageOpen(menuPageIndex);
		menuPage.pageIsOnTopOfOtherPage = true;
		menuPage.pageUnderThisPage = pageUnderThisPage;
		MenuPageTwoOptions component = ((Component)menuPage).GetComponent<MenuPageTwoOptions>();
		menuPage.menuHeaderName = popUpHeader;
		((TMP_Text)menuPage.menuHeader).text = popUpHeader;
		((Graphic)menuPage.menuHeader).color = popUpHeaderColor;
		component.option1Event = menuButtonPopUp.option1Event;
		component.option2Event = menuButtonPopUp.option2Event;
		((TMP_Text)component.bodyTextMesh).text = popUpText;
		component.option1Button.buttonTextString = option1Text;
		component.option2Button.buttonTextString = option2Text;
	}

	public void MenuHover()
	{
		menuHover = 0.1f;
	}

	public void PageSetCurrent(MenuPageIndex menuPageIndex, MenuPage menuPage)
	{
		currentMenuPageIndex = menuPageIndex;
		currentMenuPage = menuPage;
	}

	public void StateSet(MenuState state)
	{
		currentMenuState = (int)state;
	}

	private void PageInactiveAdd(MenuPage menuPage)
	{
		if (!inactivePages.Contains(menuPage))
		{
			inactivePages.Add(menuPage);
		}
	}

	private void PageInactiveRemove(MenuPage menuPage)
	{
		if (inactivePages.Contains(menuPage))
		{
			inactivePages.Remove(menuPage);
		}
	}

	public void PageReactivatePageUnderThisPage(MenuPage _menuPage)
	{
		if (!((Object)(object)currentMenuPage != (Object)(object)_menuPage) && Object.op_Implicit((Object)(object)currentMenuPage.pageUnderThisPage))
		{
			if (currentMenuPage.pageUnderThisPage.currentPageState == MenuPage.PageState.Inactive)
			{
				currentMenuPage.pageUnderThisPage.PageStateSet(MenuPage.PageState.Activating);
			}
			PageSetCurrent(currentMenuPage.pageUnderThisPage.menuPageIndex, currentMenuPage.pageUnderThisPage);
		}
	}

	public void PageCloseAllExcept(MenuPageIndex menuPageIndex)
	{
		foreach (MenuPage allPage in allPages)
		{
			if (allPage.menuPageIndex != menuPageIndex)
			{
				allPage.PageStateSet(MenuPage.PageState.Closing);
			}
		}
	}

	public void PageCloseAll()
	{
		foreach (MenuPage allPage in allPages)
		{
			allPage.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	public void PageCloseAllAddedOnTop()
	{
		foreach (MenuPage item in addedPagesOnTop)
		{
			item.PageStateSet(MenuPage.PageState.Closing);
		}
	}

	public void PlayerHeadAdd(MenuPlayerHead head)
	{
		if (!playerHeads.Contains(head))
		{
			playerHeads.Add(head);
		}
		for (int i = 0; i < playerHeads.Count; i++)
		{
			if ((Object)(object)playerHeads[i] == (Object)null)
			{
				playerHeads.RemoveAt(i);
			}
		}
	}

	public void SetActiveSelectionBox(MenuSelectionBox selectBox)
	{
		activeSelectionBox = selectBox;
	}

	public void PlayerHeadRemove(MenuPlayerHead head)
	{
		if (playerHeads.Contains(head))
		{
			playerHeads.Remove(head);
		}
		for (int i = 0; i < playerHeads.Count; i++)
		{
			if ((Object)(object)playerHeads[i] == (Object)null)
			{
				playerHeads.RemoveAt(i);
			}
		}
	}

	public void PagePopUpScheduled(string headerText, Color headerColor, string bodyText, string buttonText)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		pagePopUpScheduled = true;
		pagePopUpScheduledHeaderText = headerText;
		pagePopUpScheduledHeaderColor = headerColor;
		pagePopUpScheduledBodyText = bodyText;
		pagePopUpScheduledButtonText = buttonText;
	}

	public void PagePopUpScheduledShow()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (pagePopUpScheduled)
		{
			PagePopUp(pagePopUpScheduledHeaderText, pagePopUpScheduledHeaderColor, pagePopUpScheduledBodyText, pagePopUpScheduledButtonText);
			PagePopUpScheduledReset();
		}
	}

	public void PagePopUpScheduledReset()
	{
		pagePopUpScheduled = false;
	}

	public void SelectionBoxAdd(MenuSelectionBox selectBox)
	{
		if (!selectionBoxes.Contains(selectBox))
		{
			selectionBoxes.Add(selectBox);
		}
		for (int i = 0; i < selectionBoxes.Count; i++)
		{
			if ((Object)(object)selectionBoxes[i] == (Object)null)
			{
				selectionBoxes.RemoveAt(i);
			}
		}
	}

	public void SelectionBoxRemove(MenuSelectionBox selectBox)
	{
		if (selectionBoxes.Contains(selectBox))
		{
			selectionBoxes.Remove(selectBox);
		}
		for (int i = 0; i < selectionBoxes.Count; i++)
		{
			if ((Object)(object)selectionBoxes[i] == (Object)null)
			{
				selectionBoxes.RemoveAt(i);
			}
		}
	}

	public MenuSelectionBox SelectionBoxGetCorrect(MenuPage parentPage, MenuScrollBox menuScrollBox)
	{
		return selectionBoxes.Find((MenuSelectionBox x) => (Object)(object)x.menuPage == (Object)(object)parentPage && (Object)(object)x.menuScrollBox == (Object)(object)menuScrollBox);
	}

	public void PagePopUp(string headerText, Color headerColor, string bodyText, string buttonText)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		MenuPageIndex menuPageIndex = MenuPageIndex.PopUp;
		MenuPage menuPage = currentMenuPage;
		if (currentMenuPage.menuPageIndex == MenuPageIndex.PopUpTwoOptions)
		{
			menuPage = (currentMenuPage = currentMenuPage.pageUnderThisPage);
		}
		menuPage.PageStateSet(MenuPage.PageState.Inactive);
		MenuPage menuPage2 = PageOpen(menuPageIndex);
		menuPage2.pageIsOnTopOfOtherPage = true;
		menuPage2.pageUnderThisPage = menuPage;
		MenuPagePopUp component = ((Component)menuPage2).GetComponent<MenuPagePopUp>();
		menuPage2.menuHeaderName = headerText;
		((TMP_Text)menuPage2.menuHeader).text = headerText;
		((Graphic)menuPage2.menuHeader).color = headerColor;
		((TMP_Text)component.bodyTextMesh).text = bodyText;
		component.okButton.buttonTextString = buttonText;
		currentMenuPage = menuPage2;
		MenuEffectPopUpOpen();
	}
}
