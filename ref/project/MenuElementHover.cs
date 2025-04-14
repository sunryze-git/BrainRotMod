using UnityEngine;

public class MenuElementHover : MonoBehaviour
{
	internal bool isHovering;

	private RectTransform rectTransform;

	private MenuSelectableElement menuSelectableElement;

	private MenuPage parentPage;

	private float buttonPitch;

	public bool hasHoverEffect = true;

	internal string menuID = "-1";

	private void Start()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		menuSelectableElement = ((Component)this).GetComponent<MenuSelectableElement>();
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		buttonPitch = SemiFunc.MenuGetPitchFromYPos(rectTransform);
		if (Object.op_Implicit((Object)(object)menuSelectableElement))
		{
			menuID = menuSelectableElement.menuID;
		}
	}

	private void Update()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.UIMouseHover(parentPage, rectTransform, menuID))
		{
			if (!isHovering && hasHoverEffect)
			{
				MenuManager.instance.MenuEffectHover(buttonPitch);
			}
			isHovering = true;
		}
		else if (isHovering)
		{
			isHovering = false;
		}
		if (hasHoverEffect && isHovering)
		{
			SemiFunc.MenuSelectionBoxTargetSet(parentPage, rectTransform);
		}
	}
}
