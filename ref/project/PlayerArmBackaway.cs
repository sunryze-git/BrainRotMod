using UnityEngine;

public class PlayerArmBackaway : MonoBehaviour
{
	public PlayerArmCollision ArmCollision;

	[Space]
	public float BackAwayTarget;

	[Space]
	public float SpringFreq = 15f;

	public float SpringDamping = 0.5f;

	private float SpringTarget;

	private float SpringCurrent;

	private float SpringVelocity;

	private SpringUtils.tDampedSpringMotionParams SpringParams = new SpringUtils.tDampedSpringMotionParams();

	private void Update()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (ArmCollision.Blocked)
		{
			SpringTarget = BackAwayTarget;
		}
		else
		{
			SpringTarget = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref SpringParams, Time.deltaTime, SpringFreq, SpringDamping);
		SpringUtils.UpdateDampedSpringMotion(ref SpringCurrent, ref SpringVelocity, SpringTarget, in SpringParams);
		((Component)this).transform.localPosition = new Vector3(0f, 0f, SpringCurrent);
	}
}
