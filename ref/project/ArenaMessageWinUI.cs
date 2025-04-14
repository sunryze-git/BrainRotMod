using TMPro;
using UnityEngine;

public class ArenaMessageWinUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static ArenaMessageWinUI instance;

	private string messagePrev = "prev";

	private float messageTimer;

	private GameObject bigMessageEmojiObject;

	private TextMeshProUGUI emojiText;

	private VertexGradient originalGradient;

	public GameObject kingObject;

	public GameObject loserObject;

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

	public void ArenaText(string message, bool _kingCrowned = false)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (message != ((TMP_Text)Text).text)
		{
			messageTimer = 0f;
			SemiUIResetAllShakeEffects();
		}
		messageTimer = 0.1f;
		if (_kingCrowned)
		{
			if (!kingObject.activeSelf)
			{
				kingObject.SetActive(true);
				loserObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
				loserObject.SetActive(false);
			}
		}
		else if (!loserObject.activeSelf)
		{
			loserObject.SetActive(true);
			kingObject.transform.localPosition = new Vector3(0f, 3000f, 0f);
			kingObject.SetActive(false);
		}
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
