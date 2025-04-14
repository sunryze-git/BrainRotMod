using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : SemiUI
{
	internal TextMeshProUGUI Text;

	public static MissionUI instance;

	private string messagePrev = "prev";

	private Color bigMessageColor = Color.white;

	private Color bigMessageFlashColor = Color.white;

	private float messageTimer;

	private GameObject bigMessageEmojiObject;

	private TextMeshProUGUI emojiText;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		((TMP_Text)Text).text = "";
	}

	public void MissionText(string message, Color colorMain, Color colorFlash, float time = 3f)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!(messageTimer > 0f))
		{
			bigMessageColor = colorMain;
			bigMessageFlashColor = colorFlash;
			messageTimer = time;
			message = "<b>FOCUS > </b>" + message;
			if (message != messagePrev)
			{
				((TMP_Text)Text).fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, bigMessageColor);
				((TMP_Text)Text).fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, bigMessageColor);
				((Graphic)Text).color = bigMessageColor;
				((TMP_Text)Text).text = message;
				SemiUISpringShakeY(20f, 10f, 0.3f);
				SemiUITextFlashColor(bigMessageFlashColor, 0.2f);
				SemiUISpringScale(0.4f, 5f, 0.2f);
				messagePrev = message;
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		if (messageTimer > 0f)
		{
			messageTimer -= Time.deltaTime;
			return;
		}
		messagePrev = "prev";
		Hide();
	}
}
