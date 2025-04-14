using UnityEngine;

public class MenuSelectableElement : MonoBehaviour
{
	internal string menuID;

	internal RectTransform rectTransform;

	internal MenuPage parentPage;

	internal bool isInScrollBox;

	internal MenuScrollBox menuScrollBox;

	private void Start()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		menuID = SemiFunc.MenuGetSelectableID(((Component)this).gameObject);
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		if (Object.op_Implicit((Object)(object)parentPage))
		{
			parentPage.selectableElements.Add(this);
			if (((Transform)rectTransform).localPosition.y < parentPage.bottomElementYPos)
			{
				parentPage.bottomElementYPos = ((Transform)rectTransform).localPosition.y;
			}
		}
		isInScrollBox = false;
		MenuScrollBox componentInParent = ((Component)this).GetComponentInParent<MenuScrollBox>();
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			isInScrollBox = true;
			menuScrollBox = componentInParent;
		}
	}
}
