using UnityEngine;

public class MenuScrollBox : MonoBehaviour
{
	public RectTransform scrollSize;

	public RectTransform scroller;

	public RectTransform scrollHandle;

	public RectTransform scrollBarBackground;

	public GameObject scrollBar;

	internal float scrollAmount;

	private float scrollAmountTarget;

	private float scrollHeight;

	internal MenuPage parentPage;

	private MenuSelectableElement menuSelectableElement;

	public MenuSelectionBox menuSelectionBox;

	internal float scrollerStartPosition;

	internal float scrollerEndPosition;

	private float scrollHandleTargetPosition;

	public MenuElementHover menuElementHover;

	internal bool scrollBoxActive = true;

	public float heightPadding;

	private void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		menuSelectableElement = ((Component)scrollBarBackground).GetComponent<MenuSelectableElement>();
		scrollHandleTargetPosition = ((Transform)scrollHandle).localPosition.y;
		float num = 0f;
		Rect rect;
		foreach (RectTransform item in (Transform)scroller)
		{
			RectTransform val = item;
			rect = val.rect;
			float num2 = ((Rect)(ref rect)).height * val.pivot.y;
			if (((Transform)val).localPosition.y - num2 < num)
			{
				num = ((Transform)val).localPosition.y - num2;
			}
		}
		scrollHeight = Mathf.Abs(num) + heightPadding;
		scrollerStartPosition = scrollHeight + 42f;
		scrollerEndPosition = ((Transform)scroller).localPosition.y;
		bool flag = true;
		float num3 = scrollHeight;
		rect = scrollBarBackground.rect;
		if (num3 < ((Rect)(ref rect)).height)
		{
			scrollBar.SetActive(false);
			flag = false;
		}
		if (flag)
		{
			parentPage.scrollBoxes++;
		}
	}

	private void Update()
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		if (parentPage.scrollBoxes > 1)
		{
			if (menuElementHover.isHovering)
			{
				scrollBoxActive = true;
			}
			else
			{
				scrollBoxActive = false;
			}
		}
		if (!scrollBar.activeSelf || !scrollBoxActive)
		{
			return;
		}
		Rect rect;
		if (Input.GetMouseButton(0) && SemiFunc.UIMouseHover(parentPage, scrollBarBackground, menuSelectableElement.menuID))
		{
			float num = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(scrollBarBackground).y;
			if (num < scrollHandle.sizeDelta.y / 2f)
			{
				num = scrollHandle.sizeDelta.y / 2f;
			}
			float num2 = num;
			rect = scrollBarBackground.rect;
			if (num2 > ((Rect)(ref rect)).height - scrollHandle.sizeDelta.y / 2f)
			{
				rect = scrollBarBackground.rect;
				num = ((Rect)(ref rect)).height - scrollHandle.sizeDelta.y / 2f;
			}
			scrollHandleTargetPosition = num;
		}
		if (SemiFunc.InputMovementY() != 0f || SemiFunc.InputScrollY() != 0f)
		{
			scrollHandleTargetPosition += SemiFunc.InputMovementY() * 20f / (scrollHeight * 0.01f);
			scrollHandleTargetPosition += SemiFunc.InputScrollY() / (scrollHeight * 0.01f);
			if (scrollHandleTargetPosition < scrollHandle.sizeDelta.y / 2f)
			{
				scrollHandleTargetPosition = scrollHandle.sizeDelta.y / 2f;
			}
			float num3 = scrollHandleTargetPosition;
			rect = scrollBarBackground.rect;
			if (num3 > ((Rect)(ref rect)).height - scrollHandle.sizeDelta.y / 2f)
			{
				rect = scrollBarBackground.rect;
				scrollHandleTargetPosition = ((Rect)(ref rect)).height - scrollHandle.sizeDelta.y / 2f;
			}
		}
		((Transform)scrollHandle).localPosition = new Vector3(((Transform)scrollHandle).localPosition.x, Mathf.Lerp(((Transform)scrollHandle).localPosition.y, scrollHandleTargetPosition, Time.deltaTime * 20f), ((Transform)scrollHandle).localPosition.z);
		float y = ((Transform)scrollHandle).localPosition.y;
		rect = scrollBarBackground.rect;
		scrollAmount = y / ((Rect)(ref rect)).height * 1.1f;
		if (scrollAmount < 0f)
		{
			scrollAmount = 0f;
		}
		if (scrollAmount > 1f)
		{
			scrollAmount = 1f;
		}
		((Transform)scroller).localPosition = new Vector3(((Transform)scroller).localPosition.x, Mathf.Lerp(scrollerStartPosition, scrollerEndPosition, scrollAmount), ((Transform)scroller).localPosition.z);
	}
}
