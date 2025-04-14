using UnityEngine;

public class FlashlightTilt : MonoBehaviour
{
	public SpringQuaternion spring;

	private void Update()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Quaternion targetRotation = Quaternion.LookRotation(((Component)this).transform.parent.forward, Vector3.up);
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(spring, targetRotation);
	}
}
