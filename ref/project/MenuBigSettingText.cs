using TMPro;
using UnityEngine;

public class MenuBigSettingText : MonoBehaviour
{
	internal TextMeshProUGUI textMeshPro;

	private void Start()
	{
		textMeshPro = ((Component)this).GetComponent<TextMeshProUGUI>();
	}
}
