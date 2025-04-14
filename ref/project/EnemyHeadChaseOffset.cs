using UnityEngine;

public class EnemyHeadChaseOffset : MonoBehaviour
{
	public Enemy Enemy;

	[Space]
	public Vector3 Offset;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed;

	[Space]
	public AnimationCurve OutroCurve;

	public float OutroSpeed;

	private float Lerp;

	private bool Active;

	private void Update()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy.CurrentState == EnemyState.Chase || Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			if (Lerp <= 0f || Lerp >= 1f)
			{
				Active = true;
			}
		}
		else if (Lerp <= 0f || Lerp >= 1f)
		{
			Active = false;
		}
		if (Active)
		{
			Lerp += Time.deltaTime * IntroSpeed;
		}
		else
		{
			Lerp -= Time.deltaTime * OutroSpeed;
		}
		Lerp = Mathf.Clamp01(Lerp);
		if (Active)
		{
			((Component)this).transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(Offset), IntroCurve.Evaluate(Lerp));
		}
		else
		{
			((Component)this).transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Euler(Offset), OutroCurve.Evaluate(Lerp));
		}
	}
}
