using UnityEngine;

public class SledgehammerBob : MonoBehaviour
{
	public CameraBob CameraBob;

	[Space]
	public float SpringFreqPosZ = 15f;

	public float SpringDampingPosZ = 0.5f;

	private float TargetPosZ;

	private float CurrentPosZ;

	private float VelocityPosZ;

	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	[Space]
	public float SpringFreqPosY = 15f;

	public float SpringDampingPosY = 0.5f;

	private float TargetPosY;

	private float CurrentPosY;

	private float VelocityPosY;

	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();

	private void Start()
	{
		CameraBob = GameDirector.instance.CameraBob;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		TargetPosY = ((Component)CameraBob).transform.localRotation.z * -500f;
		SpringUtils.CalcDampedSpringMotionParams(ref SpringParamsPosY, Time.deltaTime, SpringFreqPosY, SpringDampingPosY);
		SpringUtils.UpdateDampedSpringMotion(ref CurrentPosY, ref VelocityPosY, TargetPosY, in SpringParamsPosY);
		TargetPosZ = ((Component)CameraBob).transform.localPosition.y * -0.5f;
		SpringUtils.CalcDampedSpringMotionParams(ref SpringParamsPosZ, Time.deltaTime, SpringFreqPosZ, SpringDampingPosZ);
		SpringUtils.UpdateDampedSpringMotion(ref CurrentPosZ, ref VelocityPosZ, TargetPosZ, in SpringParamsPosZ);
		((Component)this).transform.localRotation = Quaternion.Euler(0f - CurrentPosY, 0f, CurrentPosY);
		((Component)this).transform.localPosition = new Vector3(0f, 0f, ((Component)CameraBob).transform.localPosition.y + CurrentPosZ);
	}
}
