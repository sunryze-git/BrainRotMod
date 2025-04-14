using TMPro;
using UnityEngine;

public class DebugActiveLights : MonoBehaviour
{
	private TextMeshProUGUI text;

	private void Awake()
	{
		text = ((Component)this).GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		((TMP_Text)text).text = "Active Lights: " + LightManager.instance.activeLightsAmount;
	}
}
