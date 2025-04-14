using UnityEngine;

public class MapLayer : MonoBehaviour
{
	public int layer;

	internal Vector3 positionStart;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		positionStart = ((Component)this).transform.position;
	}
}
