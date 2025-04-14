using UnityEngine;

public class EnemyJumpSurface : MonoBehaviour
{
	public Vector3 jumpDirection;

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		Vector3 val = position + ((Component)this).transform.TransformDirection(((Vector3)(ref jumpDirection)).normalized * 0.3f);
		Gizmos.DrawLine(position, val);
		Gizmos.DrawWireSphere(val, 0.03f);
	}
}
