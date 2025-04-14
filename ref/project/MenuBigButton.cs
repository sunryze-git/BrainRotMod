using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBigButton : MonoBehaviour
{
	public enum State
	{
		Main,
		Edit
	}

	public string buttonTitle = "";

	public string buttonName = "NewButton";

	public RawImage mainButtonBG;

	public RawImage behindButtonBG;

	public MenuButton menuButton;

	public TextMeshProUGUI buttonTitleTextMesh;

	private Color mainButtonMainColor;

	private Color behindButtonMainColor;

	public State state;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		behindButtonMainColor = ((Graphic)behindButtonBG).color;
		mainButtonMainColor = ((Graphic)mainButtonBG).color;
	}

	private void Update()
	{
		switch (state)
		{
		case State.Main:
			StateMain();
			break;
		case State.Edit:
			StateEdit();
			break;
		}
	}

	private void StateMain()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (menuButton.hovering)
		{
			Color color = default(Color);
			((Color)(ref color))._002Ector(0.7f, 0.2f, 0f, 1f);
			((Graphic)mainButtonBG).color = color;
			((Graphic)behindButtonBG).color = AssetManager.instance.colorYellow;
		}
		else
		{
			((Graphic)mainButtonBG).color = mainButtonMainColor;
			((Graphic)behindButtonBG).color = behindButtonMainColor;
		}
		if (menuButton.clicked)
		{
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(1f, 0.5f, 0f, 1f);
			((Graphic)mainButtonBG).color = color2;
			((Graphic)behindButtonBG).color = Color.white;
		}
	}

	private void StateEdit()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)menuButton.buttonText).text = "[press new button]";
		if (menuButton.hovering)
		{
			Color color = default(Color);
			((Color)(ref color))._002Ector(0.5f, 0.1f, 0f, 1f);
			((Graphic)mainButtonBG).color = color;
			((Color)(ref color))._002Ector(1f, 0.5f, 0f, 1f);
			((Graphic)behindButtonBG).color = color;
		}
		else
		{
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(0.5f, 0.1f, 0f, 1f);
			((Graphic)mainButtonBG).color = color2;
			((Color)(ref color2))._002Ector(0.7f, 0.2f, 0f, 1f);
			((Graphic)behindButtonBG).color = color2;
		}
		if (menuButton.clicked)
		{
			Color color3 = default(Color);
			((Color)(ref color3))._002Ector(1f, 0.5f, 0f, 1f);
			((Graphic)mainButtonBG).color = color3;
			((Graphic)behindButtonBG).color = Color.white;
		}
	}
}
