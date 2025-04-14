using UnityEngine;

public class CameraJump : MonoBehaviour
{
	public static CameraJump instance;

	internal bool jumpActive;

	public AnimationCurve jumpCurve;

	public float jumpSpeed = 1f;

	private float jumpLerp;

	public Vector3 jumpPosition;

	public Vector3 jumpRotation;

	[Space]
	private bool landActive;

	public AnimationCurve landCurve;

	public float landSpeed = 1f;

	private float landLerp;

	public Vector3 landPosition;

	public Vector3 landRotation;

	private void Awake()
	{
		instance = this;
	}

	public void Jump()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
		GameDirector.instance.CameraShake.Shake(2f, 0.1f);
		jumpActive = true;
		jumpLerp = 0f;
	}

	public void Land()
	{
		if (!landActive)
		{
			GameDirector.instance.CameraImpact.Shake(1f, 0.05f);
			GameDirector.instance.CameraShake.Shake(2f, 0.1f);
			landActive = true;
			landLerp = 0f;
		}
	}

	private void Update()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		Vector3 val2 = Vector3.zero;
		if (jumpActive)
		{
			if (jumpLerp >= 1f)
			{
				jumpActive = false;
				jumpLerp = 0f;
			}
			else
			{
				val += Vector3.LerpUnclamped(Vector3.zero, jumpPosition, jumpCurve.Evaluate(jumpLerp));
				val2 += Vector3.LerpUnclamped(Vector3.zero, jumpRotation, jumpCurve.Evaluate(jumpLerp));
				jumpLerp += jumpSpeed * Time.deltaTime;
			}
		}
		if (landActive)
		{
			if (landLerp >= 1f)
			{
				landActive = false;
				landLerp = 0f;
			}
			else
			{
				val += Vector3.LerpUnclamped(Vector3.zero, landPosition, landCurve.Evaluate(landLerp));
				val2 += Vector3.LerpUnclamped(Vector3.zero, landRotation, landCurve.Evaluate(landLerp));
				landLerp += landSpeed * Time.deltaTime;
			}
		}
		val *= GameplayManager.instance.cameraAnimation;
		val2 *= GameplayManager.instance.cameraAnimation;
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, val, 30f * Time.deltaTime);
		Quaternion localRotation = ((Component)this).transform.localRotation;
		((Component)this).transform.localEulerAngles = val2;
		Quaternion localRotation2 = ((Component)this).transform.localRotation;
		((Component)this).transform.localRotation = localRotation;
		((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, localRotation2, 30f * Time.deltaTime);
	}
}
