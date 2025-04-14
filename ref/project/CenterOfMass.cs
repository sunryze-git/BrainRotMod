using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(((Component)this).transform.position, 0.1f);
	}
}
