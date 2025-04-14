using System;
using System.Collections;
using UnityEngine;

public class RandomRotateAndScale : MonoBehaviour
{
	[Space]
	[Header("Spawn")]
	public AnimationCurve spawnScaleCurve;

	public float spawnAnimationLength = 1f;

	[Space]
	[Header("Time before despawn")]
	public float durationBeforeDespawn = 5f;

	[Space]
	[Header("Despawn")]
	public AnimationCurve despawnScaleCurve;

	public float despawnAnimationLength = 1f;

	[Space]
	[Header("Scale")]
	public float scaleMultiplier = 1f;

	private void Start()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		RotateObjectAndChildren();
		((MonoBehaviour)this).StartCoroutine(ScaleAnimation(spawnScaleCurve, spawnAnimationLength, delegate
		{
			((MonoBehaviour)this).StartCoroutine(WaitAndDespawn(durationBeforeDespawn));
		}));
		Transform transform = ((Component)this).transform;
		transform.position += Vector3.up * 0.02f;
		float num = 0.1f;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(new Ray(((Component)this).transform.position, -Vector3.up), ref val, num))
		{
			((Component)this).transform.position = ((RaycastHit)(ref val)).point + Vector3.up * 0.0001f;
		}
	}

	private void RotateObjectAndChildren()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localEulerAngles = ((Component)this).transform.localEulerAngles;
		((Component)this).transform.localRotation = Quaternion.Euler(localEulerAngles.x + 90f, localEulerAngles.y, (float)Random.Range(0, 360));
		foreach (Transform item in ((Component)this).transform)
		{
			Vector3 localEulerAngles2 = item.localEulerAngles;
			item.localRotation = Quaternion.Euler(localEulerAngles2.x, localEulerAngles2.y, (float)Random.Range(0, 360));
		}
	}

	private IEnumerator ScaleAnimation(AnimationCurve curve, float animationLength, Action onComplete)
	{
		float elapsedTime = 0f;
		while (elapsedTime < animationLength)
		{
			elapsedTime += Time.deltaTime;
			float num = elapsedTime / animationLength;
			float num2 = curve.Evaluate(num) * scaleMultiplier;
			((Component)this).transform.localScale = new Vector3(num2, num2, num2);
			yield return null;
		}
		onComplete?.Invoke();
	}

	private IEnumerator WaitAndDespawn(float duration)
	{
		yield return (object)new WaitForSeconds(duration);
		((MonoBehaviour)this).StartCoroutine(ScaleAnimation(despawnScaleCurve, despawnAnimationLength, delegate
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}));
	}
}
