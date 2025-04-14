using TMPro;
using UnityEngine;

public class ItemInfoUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static ItemInfoUI instance;

	private string messagePrev = "prev";

	private float messageTimer;

	private GameObject bigMessageEmojiObject;

	private TextMeshProUGUI emojiText;

	private VertexGradient originalGradient;

	protected override void Start()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		((TMP_Text)Text).text = "";
		originalGradient = ((TMP_Text)Text).colorGradient;
	}

	public void ItemInfoText(ItemAttributes _itemAttributes, string message, bool enemy = false)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		ItemAttributes currentlyLookingAtItemAttributes = PhysGrabber.instance.currentlyLookingAtItemAttributes;
		if (!PhysGrabber.instance.grabbed && Object.op_Implicit((Object)(object)_itemAttributes) && Object.op_Implicit((Object)(object)currentlyLookingAtItemAttributes) && (Object)(object)currentlyLookingAtItemAttributes != (Object)(object)_itemAttributes)
		{
			return;
		}
		if (message != ((TMP_Text)Text).text)
		{
			messageTimer = 0f;
			SemiUIResetAllShakeEffects();
		}
		if (enemy)
		{
			VertexGradient colorGradient = default(VertexGradient);
			((VertexGradient)(ref colorGradient))._002Ector(new Color(1f, 0f, 0f), new Color(1f, 0f, 0f), new Color(1f, 0.1f, 0f), new Color(1f, 0.1f, 0f));
			((TMP_Text)Text).fontSize = 35f;
			((TMP_Text)Text).colorGradient = colorGradient;
		}
		else
		{
			((TMP_Text)Text).colorGradient = originalGradient;
			if (!SemiFunc.RunIsShop())
			{
				((TMP_Text)Text).fontSize = 15f;
			}
		}
		messageTimer = 0.1f;
		if (message != messagePrev)
		{
			((TMP_Text)Text).text = message;
			SemiUISpringShakeY(5f, 5f, 0.3f);
			SemiUISpringScale(0.1f, 2.5f, 0.2f);
			messagePrev = message;
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
