using UnityEngine;

public class EnemyHeadEyeTilt : MonoBehaviour
{
	public Transform Follow;

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localRotation = Follow.localRotation;
	}
}
