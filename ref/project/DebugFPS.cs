using TMPro;
using UnityEngine;

public class DebugFPS : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Awake()
	{
	}

	private void Update()
	{
		((TMP_Text)Text).text = "FPS: " + (int)(1f / Time.unscaledDeltaTime);
	}
}
