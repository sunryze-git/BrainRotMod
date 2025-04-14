using UnityEngine;

public class PhysGrabObjectSphereCollider : MonoBehaviour
{
	public bool drawGizmos = true;

	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;

	private void OnDrawGizmos()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (drawGizmos)
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f * gizmoTransparency);
			Gizmos.matrix = Matrix4x4.TRS(((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform.localScale);
			Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f * gizmoTransparency);
			Gizmos.DrawSphere(Vector3.zero, 0.5f);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
