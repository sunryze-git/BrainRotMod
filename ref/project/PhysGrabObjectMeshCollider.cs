using UnityEngine;

public class PhysGrabObjectMeshCollider : MonoBehaviour
{
	public bool showGizmo = true;

	[Range(0.2f, 1f)]
	public float gizmoAlpha = 1f;

	private void OnDrawGizmos()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (showGizmo)
		{
			Mesh sharedMesh = ((Component)this).GetComponent<MeshCollider>().sharedMesh;
			if ((Object)(object)sharedMesh != (Object)null)
			{
				Gizmos.color = new Color(0f, 1f, 0f, 0.2f * gizmoAlpha);
				Gizmos.DrawMesh(sharedMesh, ((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform.localScale);
				Gizmos.color = new Color(0f, 1f, 0f, 0.4f * gizmoAlpha);
				Gizmos.DrawWireMesh(sharedMesh, ((Component)this).transform.position, ((Component)this).transform.rotation, ((Component)this).transform.localScale);
			}
		}
	}
}
