using TMPro;
using UnityEngine;

public class SpectateNameUI : SemiUI
{
	internal TextMeshProUGUI Text;

	public static SpectateNameUI instance;

	private string spectateName;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
	}

	protected override void Update()
	{
		base.Update();
		Hide();
	}

	public void SetName(string name)
	{
		spectateName = name;
		((TMP_Text)Text).text = spectateName;
	}
}
