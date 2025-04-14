using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementSaveFile : MonoBehaviour
{
	public Image fadePanel;

	private MenuElementHover menuElementHover;

	private float initialFadeAlpha;

	private MenuPageSaves parentPageSaves;

	internal string saveFileName;

	public TextMeshProUGUI saveFileHeader;

	public TextMeshProUGUI saveFileHeaderLevel;

	public TextMeshProUGUI saveFileHeaderDate;

	public TextMeshProUGUI saveFileInfoRow1;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		menuElementHover = ((Component)this).GetComponent<MenuElementHover>();
		initialFadeAlpha = ((Graphic)fadePanel).color.a;
		parentPageSaves = ((Component)this).GetComponentInParent<MenuPageSaves>();
	}

	private void Update()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (menuElementHover.isHovering)
		{
			Color color = ((Graphic)fadePanel).color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 10f);
			((Graphic)fadePanel).color = color;
			if (SemiFunc.InputDown(InputKey.Confirm) || SemiFunc.InputDown(InputKey.Grab))
			{
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm);
				parentPageSaves.SaveFileSelected(saveFileName);
			}
		}
		else
		{
			Color color2 = ((Graphic)fadePanel).color;
			color2.a = Mathf.Lerp(color2.a, initialFadeAlpha, Time.deltaTime * 10f);
			((Graphic)fadePanel).color = color2;
		}
	}
}
