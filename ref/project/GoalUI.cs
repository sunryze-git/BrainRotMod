using TMPro;
using UnityEngine;

public class GoalUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static GoalUI instance;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
	}

	protected override void Update()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (SemiFunc.RunIsLevel() || SemiFunc.RunIsTutorial())
		{
			int extractionPoints = RoundDirector.instance.extractionPoints;
			int extractionPointsCompleted = RoundDirector.instance.extractionPointsCompleted;
			((TMP_Text)Text).text = extractionPointsCompleted + "<color=#7D250B> <size=45>/</size> </color><b>" + extractionPoints;
		}
		else
		{
			Hide();
		}
		if (HaulUI.instance.hideTimer > 0f)
		{
			SemiUIScoot(new Vector2(0f, 45f));
		}
	}
}
