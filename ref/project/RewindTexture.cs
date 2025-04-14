using UnityEngine;
using UnityEngine.UI;

public class RewindTexture : MonoBehaviour
{
	public float scrollSpeed = 0.5f;

	private RawImage rawImage;

	private void Start()
	{
		rawImage = ((Component)this).GetComponent<RawImage>();
	}

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Repeat(Time.time * scrollSpeed, 1f);
		Rect uvRect = rawImage.uvRect;
		((Rect)(ref uvRect)).x = num;
		((Rect)(ref uvRect)).y = num;
		rawImage.uvRect = uvRect;
	}
}
