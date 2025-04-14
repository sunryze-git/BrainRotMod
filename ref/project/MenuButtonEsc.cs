using UnityEngine;

public class MenuButtonEsc : MonoBehaviour
{
	private Transform parentTransform;

	private void Start()
	{
		parentTransform = ((Component)((Component)this).GetComponentInParent<MenuPage>()).transform;
	}

	private void Update()
	{
	}
}
