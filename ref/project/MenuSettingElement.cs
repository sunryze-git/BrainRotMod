using UnityEngine;

public class MenuSettingElement : MonoBehaviour
{
	private MenuPage parentPage;

	internal int settingElementID;

	private void Start()
	{
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		parentPage.settingElements.Add(this);
		settingElementID = parentPage.settingElements.Count;
	}

	private void OnDestroy()
	{
		if (Object.op_Implicit((Object)(object)parentPage))
		{
			parentPage.settingElements.Remove(this);
		}
	}
}
