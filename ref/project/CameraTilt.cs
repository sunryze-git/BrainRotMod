using UnityEngine;

public class CameraTilt : MonoBehaviour
{
	public static CameraTilt Instance;

	public float tiltZ = 250f;

	public float tiltZMax = 10f;

	[Space]
	public float tiltX = 250f;

	public float tiltXMax = 10f;

	[Space]
	public float strafeAmount = 1f;

	public float CrouchMultiplier = 1f;

	private float Amount = 1f;

	private float AmountCurrent = 1f;

	private float previousX;

	private float previousY;

	private Quaternion targetAngle;

	[HideInInspector]
	public float tiltXresult;

	[HideInInspector]
	public float tiltZresult;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.MenuLevel())
		{
			((Component)this).transform.localRotation = Quaternion.identity;
			return;
		}
		if (PlayerController.instance.Crouching)
		{
			AmountCurrent = Mathf.Lerp(AmountCurrent, Amount * CrouchMultiplier, Time.deltaTime * 5f);
		}
		else
		{
			AmountCurrent = Mathf.Lerp(AmountCurrent, Amount, Time.deltaTime * 5f);
		}
		float num = SemiFunc.InputMovementX();
		if (GameDirector.instance.DisableInput || Object.op_Implicit((Object)(object)SpectateCamera.instance) || PlayerController.instance.InputDisableTimer > 0f)
		{
			num = 0f;
		}
		if (((Component)this).transform.rotation.x != previousX && ((Component)this).transform.rotation.y != previousY)
		{
			Quaternion rotation = ((Component)this).transform.rotation;
			if (Mathf.Abs(((Quaternion)(ref rotation)).eulerAngles.y - previousY) < 180f)
			{
				rotation = ((Component)this).transform.rotation;
				if (Mathf.Abs(((Quaternion)(ref rotation)).eulerAngles.x - previousX) < 180f)
				{
					float num2 = previousX;
					rotation = ((Component)this).transform.rotation;
					tiltXresult = (num2 - ((Quaternion)(ref rotation)).eulerAngles.x) / Time.deltaTime * tiltX;
					tiltXresult = Mathf.Clamp(tiltXresult, 0f - tiltXMax, tiltXMax);
					rotation = ((Component)this).transform.rotation;
					tiltZresult = (((Quaternion)(ref rotation)).eulerAngles.y - previousY) / Time.deltaTime * tiltZ + num * strafeAmount;
					tiltZresult = Mathf.Clamp(tiltZresult, 0f - tiltZMax, tiltZMax);
					float num3 = 1f;
					if (Object.op_Implicit((Object)(object)SpectateCamera.instance))
					{
						num3 = 0.1f;
					}
					num3 *= GameplayManager.instance.cameraAnimation;
					targetAngle = Quaternion.Euler(tiltXresult * AmountCurrent * num3, 0f, tiltZresult * AmountCurrent * num3);
				}
			}
			rotation = ((Component)this).transform.rotation;
			previousX = ((Quaternion)(ref rotation)).eulerAngles.x;
			rotation = ((Component)this).transform.rotation;
			previousY = ((Quaternion)(ref rotation)).eulerAngles.y;
		}
		float num4 = 3f;
		if (targetAngle == Quaternion.identity)
		{
			num4 = 10f;
		}
		((Component)this).transform.localRotation = Quaternion.Slerp(((Component)this).transform.localRotation, targetAngle, num4 * Time.deltaTime);
	}
}
