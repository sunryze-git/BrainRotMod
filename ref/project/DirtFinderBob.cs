using UnityEngine;

public class DirtFinderBob : MonoBehaviour
{
	public CameraBob CameraBob;

	public PlayerAvatar PlayerAvatar;

	[Space]
	public float PosZMultiplier = 1f;

	public float SpringFreqPosZ = 15f;

	public float SpringDampingPosZ = 0.5f;

	private float TargetPosZ;

	private float CurrentPosZ;

	private float VelocityPosZ;

	private SpringUtils.tDampedSpringMotionParams SpringParamsPosZ = new SpringUtils.tDampedSpringMotionParams();

	[Space]
	public float PosYMultiplier = 1f;

	public float SpringFreqPosY = 15f;

	public float SpringDampingPosY = 0.5f;

	private float TargetPosY;

	private float CurrentPosY;

	private float VelocityPosY;

	private SpringUtils.tDampedSpringMotionParams SpringParamsPosY = new SpringUtils.tDampedSpringMotionParams();

	private void Start()
	{
		CameraBob = CameraBob.Instance;
	}

	private void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || PlayerAvatar.isLocal)
		{
			TargetPosY = ((Component)CameraBob).transform.localRotation.y * PosYMultiplier;
			SpringUtils.CalcDampedSpringMotionParams(ref SpringParamsPosY, Time.deltaTime, SpringFreqPosY, SpringDampingPosY);
			SpringUtils.UpdateDampedSpringMotion(ref CurrentPosY, ref VelocityPosY, TargetPosY, in SpringParamsPosY);
			TargetPosZ = ((Component)CameraBob).transform.localRotation.z * PosZMultiplier;
			SpringUtils.CalcDampedSpringMotionParams(ref SpringParamsPosZ, Time.deltaTime, SpringFreqPosZ, SpringDampingPosZ);
			SpringUtils.UpdateDampedSpringMotion(ref CurrentPosZ, ref VelocityPosZ, TargetPosZ, in SpringParamsPosZ);
			((Component)this).transform.localPosition = new Vector3(0f, CurrentPosY * 0.0025f + ((Component)CameraJump.instance).transform.localPosition.y, 0f);
			Transform transform = ((Component)this).transform;
			Quaternion localRotation = ((Component)CameraJump.instance).transform.localRotation;
			transform.localRotation = Quaternion.Euler(((Quaternion)(ref localRotation)).eulerAngles.x * 2f, 0f, 0f);
		}
	}
}
