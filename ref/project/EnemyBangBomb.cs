using UnityEngine;

public class EnemyBangBomb : MonoBehaviour
{
	public SpringQuaternion spring;

	public Transform source;

	public Transform target;

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		source.rotation = SemiFunc.SpringQuaternionGet(spring, target.rotation);
	}
}
