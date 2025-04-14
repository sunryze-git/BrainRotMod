using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : SemiUI
{
	private TextMeshProUGUI Text;

	private TextMeshProUGUI textNumbers;

	private TextMeshProUGUI upgradesHeader;

	public GameObject scanlineObject;

	public static StatsUI instance;

	private Dictionary<string, int> playerUpgrades = new Dictionary<string, int>();

	private float showStatsTimer;

	private bool fetched;

	protected override void Start()
	{
		base.Start();
		Text = ((Component)this).GetComponent<TextMeshProUGUI>();
		instance = this;
		textNumbers = ((Component)((Component)this).transform.Find("StatsNumbers")).GetComponent<TextMeshProUGUI>();
		((TMP_Text)Text).text = "";
		upgradesHeader = ((Component)((Component)this).transform.Find("Upgrades Header")).GetComponent<TextMeshProUGUI>();
		((TMP_Text)textNumbers).text = "";
		((Behaviour)upgradesHeader).enabled = false;
	}

	public void Fetch()
	{
		playerUpgrades = StatsManager.instance.FetchPlayerUpgrades(PlayerController.instance.playerSteamID);
		((TMP_Text)Text).text = "";
		((TMP_Text)textNumbers).text = "";
		((Behaviour)upgradesHeader).enabled = false;
		scanlineObject.SetActive(false);
		foreach (KeyValuePair<string, int> playerUpgrade in playerUpgrades)
		{
			string text = playerUpgrade.Key.ToUpper();
			if (playerUpgrade.Value > 0)
			{
				TextMeshProUGUI text2 = Text;
				((TMP_Text)text2).text = ((TMP_Text)text2).text + text + "\n";
				TextMeshProUGUI obj = textNumbers;
				((TMP_Text)obj).text = ((TMP_Text)obj).text + "<b>" + playerUpgrade.Value + "\n</b>";
			}
		}
		if (((TMP_Text)Text).text != "")
		{
			((Behaviour)upgradesHeader).enabled = true;
			scanlineObject.SetActive(true);
		}
	}

	public void ShowStats()
	{
		SemiUISpringShakeY(20f, 10f, 0.3f);
		SemiUISpringScale(0.4f, 5f, 0.2f);
		showStatsTimer = 5f;
	}

	protected override void Update()
	{
		base.Update();
		Hide();
		if (showStatsTimer > 0f)
		{
			showStatsTimer -= Time.deltaTime;
			Show();
		}
		if (showTimer > 0f)
		{
			if (!fetched)
			{
				Fetch();
			}
		}
		else
		{
			fetched = false;
		}
	}
}
