using UnityEngine;

public class JumpBox : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider component = ((Component)this).GetComponent<BoxCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			Gizmos.color = new Color(0f, 0.66f, 1f);
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawWireCube(component.center, component.size);
			Gizmos.color = new Color(0f, 0.44f, 1f, 0.2f);
			Gizmos.DrawCube(component.center, component.size);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			_ = Vector3.zero;
			Bounds bounds = ((Collider)component).bounds;
			Vector3 center = ((Bounds)(ref bounds)).center;
			Vector3 val = center + ((Component)this).transform.forward * 0.5f;
			Gizmos.DrawLine(center, val);
			Gizmos.DrawLine(val, val + Vector3.LerpUnclamped(-((Component)this).transform.forward, -((Component)this).transform.right, 0.5f) * 0.25f);
			Gizmos.DrawLine(val, val + Vector3.LerpUnclamped(-((Component)this).transform.forward, ((Component)this).transform.right, 0.5f) * 0.25f);
		}
	}
}
