using UnityEngine;

public class PhysGrabObjectBoxCollider : MonoBehaviour
{
	public bool drawGizmos = true;

	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;

	public bool unEquipCollider;

	private void Start()
	{
		if (unEquipCollider)
		{
			BoxCollider component = ((Component)this).GetComponent<BoxCollider>();
			if (Object.op_Implicit((Object)(object)component))
			{
				((Collider)component).enabled = false;
			}
		}
	}

	private void OnDrawGizmos()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!drawGizmos)
		{
			return;
		}
		BoxCollider component = ((Component)this).GetComponent<BoxCollider>();
		if (!((Object)(object)component == (Object)null))
		{
			Color color = default(Color);
			((Color)(ref color))._002Ector(0f, 1f, 0f, 1f * gizmoTransparency);
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(0f, 1f, 0f, 0.2f * gizmoTransparency);
			if (unEquipCollider)
			{
				color2 = (color = new Color(0f, 0.5f, 0f, 1f * gizmoTransparency));
				color2.a = 0.2f * gizmoTransparency;
			}
			Gizmos.color = color;
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawWireCube(component.center, component.size);
			Gizmos.color = color2;
			Gizmos.DrawCube(component.center, component.size);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
