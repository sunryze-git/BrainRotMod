using TMPro;
using UnityEngine;

public class EnergyUI : SemiUI
{
	private TextMeshProUGUI Text;

	public static EnergyUI instance;

	private TextMeshProUGUI textEnergyMax;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		textEnergyMax = ((Component)((Component)this).transform.Find("EnergyMax")).GetComponent<TextMeshProUGUI>();
	}

	protected override void Update()
	{
		base.Update();
		((TMP_Text)Text).text = Mathf.Ceil(PlayerController.instance.EnergyCurrent).ToString();
		((TMP_Text)textEnergyMax).text = "<b><color=orange>/</color></b>" + Mathf.Ceil(PlayerController.instance.EnergyStart);
	}
}
