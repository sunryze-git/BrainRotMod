using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider component = ((Component)this).GetComponent<BoxCollider>();
		Gizmos.color = new Color(0.1f, 1f, 0.4f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawWireCube(component.center, component.size);
		Gizmos.color = new Color(0.1f, 1f, 0.4f, 0.5f);
		Gizmos.DrawCube(component.center, component.size);
	}
}
