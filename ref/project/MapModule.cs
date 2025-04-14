using System.Collections;
using UnityEngine;

public class MapModule : MonoBehaviour
{
	public Module module;

	public AnimationCurve curve;

	public float speed;

	private float curveLerp;

	private bool animating;

	public Transform graphic;

	public void Hide()
	{
		if (!animating)
		{
			animating = true;
			((MonoBehaviour)this).StartCoroutine(HideAnimation());
		}
	}

	private void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Map.Instance.Active)
		{
			((Component)graphic).transform.rotation = ((Component)DirtFinderMapPlayer.Instance).transform.rotation;
			Transform transform = ((Component)graphic).transform;
			Quaternion rotation = ((Component)graphic).transform.rotation;
			float y = ((Quaternion)(ref rotation)).eulerAngles.y;
			rotation = ((Component)graphic).transform.rotation;
			transform.rotation = Quaternion.Euler(new Vector3(90f, y, ((Quaternion)(ref rotation)).eulerAngles.z));
		}
	}

	private IEnumerator HideAnimation()
	{
		while (curveLerp < 1f)
		{
			curveLerp += speed * Time.deltaTime;
			((Component)this).transform.localScale = Vector3.one * curve.Evaluate(curveLerp);
			yield return null;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
