using UnityEngine;

public class LobbyObjectScroller : MonoBehaviour
{
	public float scrollSpeed = 12f;

	public float maxDistanceX = 80f;

	private float offsetX = -22f;

	private TruckLandscapeScroller truck;

	private void Start()
	{
		truck = ((Component)this).GetComponentInParent<TruckLandscapeScroller>();
		if ((Object)(object)truck != (Object)null)
		{
			scrollSpeed *= truck.truckSpeed;
		}
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
		if (((Component)this).transform.position.x > maxDistanceX + offsetX)
		{
			((Component)this).transform.position = new Vector3(0f - maxDistanceX + offsetX, ((Component)this).transform.position.y, ((Component)this).transform.position.z);
		}
	}
}
