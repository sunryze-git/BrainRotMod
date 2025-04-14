using UnityEngine;

public class HUDCanvas : MonoBehaviour
{
	public static HUDCanvas instance;

	internal RectTransform rect;

	private void Awake()
	{
		instance = this;
		rect = ((Component)this).GetComponent<RectTransform>();
	}
}
