using UnityEngine;

public class ToolFollowPush : MonoBehaviour
{
	private Vector3 PushPosition;

	private Quaternion PushRotation;

	public float SettleSpeed;

	public void Push(Vector3 position, Quaternion rotation, float amount)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, position, amount);
		((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, rotation, amount);
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, Vector3.zero, SettleSpeed * Time.deltaTime);
		((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, Quaternion.identity, SettleSpeed * Time.deltaTime);
	}
}
