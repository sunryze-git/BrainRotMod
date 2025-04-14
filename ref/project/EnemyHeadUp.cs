using UnityEngine;

public class EnemyHeadUp : MonoBehaviour
{
	public Enemy enemy;

	private float startPosition;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		startPosition = ((Component)this).transform.localPosition.y;
	}

	private void Update()
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!enemy.NavMeshAgent.IsDisabled() && enemy.CurrentState == EnemyState.Chase && enemy.StateChase.VisionTimer > 0f && !enemy.TargetPlayerAvatar.isDisabled && enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y > startPosition)
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, new Vector3(((Component)this).transform.position.x, enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position.y, ((Component)this).transform.position.z), 1f * Time.deltaTime);
			return;
		}
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, new Vector3(0f, startPosition, 0f), 5f * Time.deltaTime);
		if (enemy.CurrentState == EnemyState.Despawn || enemy.NavMeshAgent.IsDisabled())
		{
			((Component)this).transform.localPosition = new Vector3(0f, startPosition, 0f);
		}
	}
}
