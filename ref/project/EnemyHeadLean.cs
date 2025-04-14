using UnityEngine;

public class EnemyHeadLean : MonoBehaviour
{
	public Enemy Enemy;

	[Space]
	public float Amount = -500f;

	public float MaxAmount = 20f;

	public float Speed = 10f;

	private void Update()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!(Enemy.FreezeTimer > 0f))
		{
			if (((Vector3)(ref Enemy.NavMeshAgent.AgentVelocity)).magnitude < 0.1f)
			{
				((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 50f * Time.deltaTime);
				return;
			}
			float num = Mathf.Clamp(((Vector3)(ref Enemy.NavMeshAgent.AgentVelocity)).magnitude * Amount, 0f - MaxAmount, MaxAmount);
			((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, Quaternion.Euler(num, 0f, 0f), Speed * Time.deltaTime);
		}
	}
}
