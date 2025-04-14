using UnityEngine;
using UnityEngine.AI;

public class EnemyHeadHairLean : MonoBehaviour
{
	public NavMeshAgent Agent;

	[Space]
	public float Amount = -500f;

	public float MaxAmount = 20f;

	[Space]
	public float RandomMin;

	public float RandomMax;

	private float RandomCurrent;

	private float RandomTimer;

	public float RandomTimeMin;

	public float RandomTimeMax;

	[Space]
	public float SpringFreq = 15f;

	public float SpringDamping = 0.5f;

	private float SpringTarget;

	private float SpringCurrent;

	private float SpringVelocity;

	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity;
		if (RandomTimer <= 0f)
		{
			velocity = Agent.velocity;
			if (((Vector3)(ref velocity)).magnitude > 0.1f)
			{
				RandomTimer = Random.Range(RandomTimeMin, RandomTimeMax);
				RandomCurrent = Random.Range(RandomMin, RandomMax);
				goto IL_0069;
			}
		}
		RandomTimer -= Time.deltaTime;
		goto IL_0069;
		IL_0069:
		float equilibriumPos = 0f;
		velocity = Agent.velocity;
		if (((Vector3)(ref velocity)).magnitude > 0.1f)
		{
			velocity = Agent.velocity;
			equilibriumPos = Mathf.Clamp(((Vector3)(ref velocity)).magnitude * Amount, 0f - MaxAmount, MaxAmount) + RandomCurrent;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref SpringParams, Time.deltaTime, SpringFreq, SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref SpringCurrent, ref SpringVelocity, equilibriumPos, in SpringParams);
		((Component)this).transform.localRotation = Quaternion.Euler(SpringCurrent, 0f, 0f);
	}
}
