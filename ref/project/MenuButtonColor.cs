using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonColor : MonoBehaviour
{
	internal int colorID;

	internal Color color = Color.white;

	private MenuButton menuButton;

	private MenuPageColor menuPageColor;

	private MenuPage parentPage;

	private bool buttonClicked;

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		List<Color> playerColors = AssetManager.instance.playerColors;
		color = playerColors[colorID];
		menuButton = ((Component)this).GetComponent<MenuButton>();
		menuPageColor = ((Component)this).GetComponentInParent<MenuPageColor>();
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return (object)new WaitForSeconds(0.1f);
		while (parentPage.currentPageState != MenuPage.PageState.Active)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (color == PlayerAvatar.instance.playerAvatarVisuals.color)
		{
			menuPageColor.SetColor(colorID, ((Component)this).GetComponent<RectTransform>());
		}
	}

	private void Update()
	{
		if (menuButton.clicked && !buttonClicked)
		{
			menuPageColor.SetColor(colorID, ((Component)this).GetComponent<RectTransform>());
			PlayerAvatar.instance.PlayerAvatarSetColor(colorID);
			buttonClicked = true;
		}
		if (buttonClicked && !menuButton.clicked)
		{
			buttonClicked = false;
		}
	}
}
