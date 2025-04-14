using UnityEngine;

public class MapObject : MonoBehaviour
{
	public Transform parent;

	public void Hide()
	{
		Transform[] componentsInChildren = ((Component)((Component)this).transform).GetComponentsInChildren<Transform>(true);
		foreach (Transform val in componentsInChildren)
		{
			if ((Object)(object)val != (Object)(object)((Component)this).transform)
			{
				((Component)val).gameObject.SetActive(false);
			}
		}
	}

	public void Show()
	{
		Transform[] componentsInChildren = ((Component)((Component)this).transform).GetComponentsInChildren<Transform>(true);
		foreach (Transform val in componentsInChildren)
		{
			if ((Object)(object)val != (Object)(object)((Component)this).transform)
			{
				((Component)val).gameObject.SetActive(true);
			}
		}
	}
}
