using UnityEngine;

public class DebugUI : MonoBehaviour
{
	public GameObject enableParent;

	private void Start()
	{
		if (!Application.isEditor)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Update()
	{
		if (SemiFunc.DebugDev() && Input.GetKeyDown((KeyCode)282))
		{
			enableParent.SetActive(!enableParent.activeSelf);
		}
	}
}
