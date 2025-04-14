using UnityEngine;

public class GizmoBall : MonoBehaviour
{
	public Color color = Color.red;

	public float radius = 0.5f;

	public Vector3 offset;

	private void OnDrawGizmos()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		color.a = 0.5f;
		Gizmos.color = color;
		Gizmos.DrawSphere(((Component)this).transform.position + offset, radius);
		color.a = 1f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(((Component)this).transform.position + offset, radius);
	}
}
