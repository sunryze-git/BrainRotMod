using UnityEngine;

public class TextureScroller : MonoBehaviour
{
	public float scrollSpeed = 0.5f;

	private Renderer rend;

	private Vector2 savedOffset;

	private TruckLandscapeScroller truck;

	private void Start()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		rend = ((Component)this).GetComponent<Renderer>();
		savedOffset = rend.material.mainTextureOffset;
		truck = ((Component)this).GetComponentInParent<TruckLandscapeScroller>();
		if ((Object)(object)truck != (Object)null)
		{
			scrollSpeed *= truck.truckSpeed;
		}
	}

	private void Update()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Repeat(Time.time * scrollSpeed, 1f);
		Vector2 mainTextureOffset = default(Vector2);
		((Vector2)(ref mainTextureOffset))._002Ector(num, savedOffset.y);
		rend.material.mainTextureOffset = mainTextureOffset;
	}

	private void OnDisable()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		rend.material.mainTextureOffset = savedOffset;
	}
}
