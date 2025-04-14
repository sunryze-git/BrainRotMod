using UnityEngine;

public class DebugEnemyInvestigate : MonoBehaviour
{
	public AnimationCurve animationCurve;

	private float lerp;

	private float alpha = 1f;

	internal float radius = 1f;

	private float radiusCurrent = 2f;

	private void Update()
	{
		lerp += Time.deltaTime;
		radiusCurrent = Mathf.Lerp(0f, radius, animationCurve.Evaluate(lerp));
		if (lerp >= 1f)
		{
			alpha -= Time.deltaTime;
			if (alpha <= 0f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.eulerAngles = Vector3.zero;
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 1f, alpha);
		Gizmos.DrawWireSphere(Vector3.zero, 0.1f);
		Gizmos.color = new Color(1f, 0.62f, 0f, 0.23f * alpha);
		Gizmos.DrawSphere(Vector3.zero, radiusCurrent);
		Gizmos.color = new Color(1f, 0.62f, 0f, alpha);
		Gizmos.DrawWireSphere(Vector3.zero, radiusCurrent);
		((Component)this).transform.localEulerAngles = new Vector3(45f, 0f, 0f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, radiusCurrent);
		((Component)this).transform.localEulerAngles = new Vector3(0f, 45f, 0f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, radiusCurrent);
		((Component)this).transform.localEulerAngles = new Vector3(0f, 0f, 45f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, radiusCurrent);
	}
}
