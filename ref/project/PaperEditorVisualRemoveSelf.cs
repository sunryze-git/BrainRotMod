using UnityEngine;

public class PaperEditorVisualRemoveSelf : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
