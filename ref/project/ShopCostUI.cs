using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCostUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static ShopCostUI instance;

	public int animatedValue;

	private Color originalColor;

	private int currentValue;

	private int prevValue;

	protected override void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		originalColor = ((Graphic)Text).color;
	}

	protected override void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		string text = "";
		int num = 0;
		if (SemiFunc.RunIsShop())
		{
			num = SemiFunc.ShopGetTotalCost();
			text = SemiFunc.DollarGetString(num);
			if (num > 0)
			{
				((TMP_Text)Text).text = "-$" + text + "K";
				((Graphic)Text).color = originalColor;
			}
			else
			{
				Hide();
			}
			currentValue = num;
			if (currentValue != prevValue)
			{
				Color color = Color.white;
				if (currentValue > prevValue)
				{
					color = Color.red;
				}
				SemiUISpringShakeY(20f, 10f, 0.3f);
				SemiUITextFlashColor(color, 0.2f);
				SemiUISpringScale(0.4f, 5f, 0.2f);
				prevValue = currentValue;
			}
		}
		if (!SemiFunc.RunIsShop())
		{
			Hide();
		}
		if (showTimer > 0f && SemiFunc.RunIsLevel())
		{
			((TMP_Text)Text).text = "+$" + animatedValue + "K";
			((Graphic)Text).color = Color.green;
		}
	}
}
