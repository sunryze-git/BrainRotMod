using System.Collections.Generic;
using UnityEngine;

public class MenuPageColor : MonoBehaviour
{
	public GameObject colorButtonPrefab;

	public RectTransform colorButtonHolder;

	public MenuColorSelected menuColorSelected;

	private MenuPage menuPage;

	private void Start()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		menuPage = ((Component)this).GetComponent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < playerColors.Count; i++)
		{
			GameObject obj = Object.Instantiate<GameObject>(colorButtonPrefab, (Transform)(object)colorButtonHolder);
			MenuButtonColor component = obj.GetComponent<MenuButtonColor>();
			MenuButton component2 = obj.GetComponent<MenuButton>();
			component.colorID = i;
			component.color = playerColors[i];
			component2.colorNormal = playerColors[i] + Color.black * 0.5f;
			component2.colorHover = playerColors[i];
			component2.colorClick = playerColors[i] + Color.white * 0.95f;
			RectTransform component3 = obj.GetComponent<RectTransform>();
			((Transform)component3).SetSiblingIndex(0);
			component3.anchoredPosition = new Vector2((float)num, (float)(224 + num2));
			num += 38;
			float num3 = num;
			Rect rect = colorButtonHolder.rect;
			if (num3 > ((Rect)(ref rect)).width)
			{
				num = 0;
				num2 -= 30;
			}
		}
		Object.Destroy((Object)(object)colorButtonPrefab);
	}

	public void SetColor(int colorID, RectTransform buttonTransform)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm);
		menuColorSelected.SetColor(AssetManager.instance.playerColors[colorID], ((Transform)buttonTransform).position);
	}

	public void ConfirmButton()
	{
		MenuManager.instance.PageReactivatePageUnderThisPage(menuPage);
		MenuManager.instance.MenuEffectPopUpClose();
		menuPage.PageStateSet(MenuPage.PageState.Closing);
	}
}
