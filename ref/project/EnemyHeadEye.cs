using UnityEngine;

public class EnemyHeadEye : MonoBehaviour
{
	public Transform Target;

	public EnemyHeadEyeTarget EyeTarget;

	private float CurrentX;

	private float CurrentY;

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(Target.position - ((Component)this).transform.position);
		((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val, EyeTarget.Speed * Time.deltaTime);
		((Component)this).transform.localRotation = SemiFunc.ClampRotation(((Component)this).transform.localRotation, EyeTarget.Limit);
	}
}
