using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoExtraUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static ItemInfoExtraUI instance;

	private string messagePrev = "prev";

	private float messageTimer;

	private GameObject bigMessageEmojiObject;

	private TextMeshProUGUI emojiText;

	private Color textColor;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		((TMP_Text)Text).text = "";
	}

	public void ItemInfoText(string message, Color color)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!(messageTimer > 0f))
		{
			messageTimer = 0.2f;
			if (message != messagePrev)
			{
				((TMP_Text)Text).text = message;
				SemiUISpringShakeY(20f, 10f, 0.3f);
				SemiUISpringScale(0.4f, 5f, 0.2f);
				textColor = color;
				((Graphic)Text).color = textColor;
				messagePrev = message;
			}
		}
	}

	protected override void Update()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!SemiFunc.RunIsShop())
		{
			if (!SemiFunc.RunIsShop())
			{
				((TMP_Text)Text).fontSize = 12f;
			}
			if (messageTimer > 0f)
			{
				messageTimer -= Time.deltaTime;
				ItemInfoUI.instance.SemiUIScoot(new Vector2(0f, 8f));
			}
			else
			{
				((Graphic)Text).color = Color.white;
				messagePrev = "prev";
				Hide();
			}
		}
	}
}
