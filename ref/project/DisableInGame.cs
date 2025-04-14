using UnityEngine;

public class DisableInGame : MonoBehaviour
{
	private void Start()
	{
		((Component)this).gameObject.SetActive(false);
	}
}
