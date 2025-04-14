using UnityEngine;

public class RemoveSphere : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
