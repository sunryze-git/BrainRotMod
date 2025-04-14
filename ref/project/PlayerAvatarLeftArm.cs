using UnityEngine;

public class PlayerAvatarLeftArm : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	public Transform leftArmTransform;

	public FlashlightController flashlightController;

	[Space]
	public AnimationCurve poseCurve;

	public float poseSpeed;

	private float poseLerp;

	private Vector3 poseNew;

	private Vector3 poseOld;

	private Vector3 poseCurrent;

	[Space]
	public Vector3 basePose;

	public Vector3 flashlightPose;

	public SpringQuaternion poseSpring;

	private PlayerAvatarVisuals playerAvatarVisuals;

	private float headRotation;

	private void Start()
	{
		playerAvatarVisuals = ((Component)this).GetComponent<PlayerAvatarVisuals>();
	}

	private void Update()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!playerAvatarVisuals.isMenuAvatar && !playerAvatar.playerHealth.hurtFreeze)
		{
			if (flashlightController.currentState > FlashlightController.State.Hidden && flashlightController.currentState < FlashlightController.State.Outro && !playerAvatar.playerAvatarVisuals.animInCrawl)
			{
				SetPose(flashlightPose);
				HeadAnimate(_active: true);
				AnimatePose();
			}
			else
			{
				SetPose(basePose);
				HeadAnimate(_active: false);
				AnimatePose();
			}
		}
	}

	private void HeadAnimate(bool _active)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (_active)
		{
			float num = ((Quaternion)(ref playerAvatar.localCameraRotation)).eulerAngles.x;
			if (num > 90f)
			{
				num -= 360f;
			}
			headRotation = Mathf.Lerp(headRotation, num * 0.5f, 20f * Time.deltaTime);
		}
		else
		{
			headRotation = Mathf.Lerp(headRotation, 0f, 20f * Time.deltaTime);
		}
	}

	private void AnimatePose()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (poseLerp < 1f)
		{
			poseLerp += poseSpeed * Time.deltaTime;
			poseCurrent = Vector3.LerpUnclamped(poseOld, poseNew, poseCurve.Evaluate(poseLerp));
		}
		Quaternion rotation = leftArmTransform.rotation;
		leftArmTransform.localEulerAngles = new Vector3(poseCurrent.x, poseCurrent.y - headRotation, poseCurrent.z);
		Quaternion rotation2 = leftArmTransform.rotation;
		leftArmTransform.rotation = rotation;
		leftArmTransform.rotation = SemiFunc.SpringQuaternionGet(poseSpring, rotation2);
	}

	private void SetPose(Vector3 _poseNew)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (poseNew != _poseNew)
		{
			poseOld = poseCurrent;
			poseNew = _poseNew;
			poseLerp = 0f;
		}
	}
}
