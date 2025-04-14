using TMPro;
using UnityEngine;

public class MenuButtonHoverArea : MonoBehaviour
{
	private MenuButton menuButton;

	public TextMeshProUGUI text;

	private RectTransform rectTransform;

	private void Start()
	{
		menuButton = ((Component)this).GetComponentInParent<MenuButton>();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
	}

	private void Update()
	{
	}

	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			rectTransform = ((Component)this).GetComponent<RectTransform>();
		}
	}
}
