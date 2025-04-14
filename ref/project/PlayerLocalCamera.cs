using UnityEngine;

public class PlayerLocalCamera : MonoBehaviour
{
	public bool debug;

	private void OnDrawGizmos()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (debug)
		{
			Gizmos.color = new Color(1f, 0f, 0.79f, 0.5f);
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawSphere(Vector3.zero, 0.1f);
			Gizmos.DrawCube(new Vector3(0f, 0f, 0.15f), new Vector3(0.1f, 0.1f, 0.3f));
		}
	}
}
