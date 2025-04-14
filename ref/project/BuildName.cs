using TMPro;
using UnityEngine;

public class BuildName : MonoBehaviour
{
	private void Start()
	{
		((TMP_Text)((Component)this).GetComponent<TextMeshProUGUI>()).text = BuildManager.instance.version.title;
	}
}
