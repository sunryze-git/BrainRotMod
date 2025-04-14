using TMPro;
using UnityEngine;

public class DebugLevelsCompleted : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Update()
	{
		((TMP_Text)Text).text = "Levels Completed: " + RunManager.instance.levelsCompleted;
	}
}
