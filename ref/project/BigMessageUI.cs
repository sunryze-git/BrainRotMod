using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigMessageUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static BigMessageUI instance;

	private string bigMessagePrev = "prev";

	private string bigMessage = "big";

	private Color bigMessageColor = Color.white;

	private Color bigMessageFlashColor = Color.white;

	private float bigMessageTimer;

	private string bigMessageEmoji = "";

	private GameObject bigMessageEmojiObject;

	private TextMeshProUGUI emojiText;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		bigMessageEmojiObject = ((Component)((Component)this).transform.Find("Big Message Emoji")).gameObject;
		emojiText = bigMessageEmojiObject.GetComponent<TextMeshProUGUI>();
	}

	public void BigMessage(string message, string emoji, float size, Color colorMain, Color colorFlash)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		bigMessageColor = colorMain;
		bigMessageFlashColor = colorFlash;
		bigMessageTimer = 0.2f;
		bigMessage = message;
		if (bigMessage != bigMessagePrev)
		{
			((TMP_Text)Text).fontSize = size;
			((TMP_Text)Text).fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, bigMessageColor);
			((TMP_Text)Text).fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, bigMessageColor);
			((Graphic)Text).color = bigMessageColor;
			((TMP_Text)Text).text = bigMessage;
			bigMessageEmoji = SemiFunc.EmojiText(emoji);
			((TMP_Text)emojiText).text = bigMessageEmoji;
			SemiUISpringShakeY(20f, 10f, 0.3f);
			SemiUITextFlashColor(bigMessageFlashColor, 0.2f);
			SemiUISpringScale(0.4f, 5f, 0.2f);
			bigMessagePrev = bigMessage;
		}
	}

	protected override void Update()
	{
		base.Update();
		bigMessageEmojiObject.SetActive(((Behaviour)Text).enabled);
		if (bigMessageTimer > 0f)
		{
			bigMessageTimer -= Time.deltaTime;
			return;
		}
		bigMessage = "big";
		bigMessagePrev = "prev";
		Hide();
	}
}
