using UnityEngine;

public class EnemyThinManTentacle : MonoBehaviour
{
	public AnimationCurve wiggleCurve;

	private float rotationLerp = 1f;

	private float rotationLerpSpeed;

	private Quaternion rotationStartPos;

	private Quaternion rotationEndPos;

	private float scaleLerp = 1f;

	private float scaleLerpSpeed;

	private Vector3 scaleStartPos;

	private Vector3 scaleEndPos;

	private void Update()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (rotationLerp >= 1f)
		{
			rotationStartPos = ((Component)this).transform.localRotation;
			rotationEndPos = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
			rotationLerpSpeed = Random.Range(1f, 2f);
			rotationLerp = 0f;
		}
		else
		{
			rotationLerp += Time.deltaTime * rotationLerpSpeed;
			((Component)this).transform.localRotation = Quaternion.Lerp(rotationStartPos, rotationEndPos, wiggleCurve.Evaluate(rotationLerp));
		}
		if (scaleLerp >= 1f)
		{
			scaleStartPos = ((Component)this).transform.localScale;
			scaleEndPos = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
			scaleLerpSpeed = Random.Range(2f, 4f);
			scaleLerp = 0f;
		}
		else
		{
			scaleLerp += Time.deltaTime * scaleLerpSpeed;
			((Component)this).transform.localScale = Vector3.Lerp(scaleStartPos, scaleEndPos, wiggleCurve.Evaluate(scaleLerp));
		}
	}
}
