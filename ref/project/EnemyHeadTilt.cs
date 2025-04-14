using UnityEngine;

public class EnemyHeadTilt : MonoBehaviour
{
	public float Amount = -500f;

	public float MaxAmount = 20f;

	public float Speed = 10f;

	private Vector3 ForwardPrev;

	private void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(Vector3.Cross(ForwardPrev, ((Component)this).transform.forward).y * Amount, 0f - MaxAmount, MaxAmount);
		((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, Quaternion.Euler(0f, 0f, num), Speed * Time.deltaTime);
		ForwardPrev = ((Component)this).transform.forward;
	}
}
