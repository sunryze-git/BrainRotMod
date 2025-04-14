using UnityEngine;

public class PlayerAvatarRightArm : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	public Transform rightArmTransform;

	public Transform rightArmParentTransform;

	public MapToolController mapToolController;

	public PhysGrabBeam physGrabBeam;

	public AnimationCurve poseCurve;

	public float poseSpeed;

	private float poseLerp;

	private Vector3 poseNew;

	private Vector3 poseOld;

	private Vector3 poseCurrent;

	[Space]
	public Vector3 basePose;

	public Vector3 mapPose;

	public Vector3 grabberPose;

	private float headRotation;

	public Transform grabberTransform;

	public Transform grabberTransformTarget;

	[Space]
	public Material grabberMaterial;

	public Material grabberMaterialHeal;

	[Space]
	public Transform grabberOrb;

	public GameObject[] grabberOrbSpheres;

	[Space]
	public Light grabberLight;

	public Color grabberLightColor;

	public Color grabberLightColorHeal;

	private float grabberLightIntensity;

	private bool grabberHealing;

	[Space]
	public Transform grabberAimTarget;

	[Space]
	public SpringQuaternion grabberSteerSpring;

	public SpringQuaternion grabberClawSpring;

	public SpringFloat grabberReachSpring;

	private float grabberReachTimer;

	private float grabberReachTarget;

	private float grabberReachPrevious;

	private float grabberReachDifference;

	private float grabberReachDifferenceTimer;

	[Space]
	public Transform grabberClawParent;

	public Transform[] grabberClawChildren;

	public AnimationCurve grabberClawHideCurve;

	public AnimationCurve grabberClawChildCurve;

	private float grabberClawLerp;

	private float grabberClawChildLerp;

	private bool grabberClawHidden;

	private float grabberClawRotation;

	private float deltaTime;

	private PlayerAvatarVisuals playerAvatarVisuals;

	private void Start()
	{
		playerAvatarVisuals = ((Component)this).GetComponent<PlayerAvatarVisuals>();
		grabberLightIntensity = grabberLight.intensity;
		if (!GameManager.Multiplayer() || (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.photonView.IsMine))
		{
			((Component)grabberTransform).gameObject.SetActive(false);
			((Behaviour)this).enabled = false;
		}
	}

	private void Update()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatarVisuals.isMenuAvatar)
		{
			return;
		}
		deltaTime = playerAvatarVisuals.deltaTime;
		if (!playerAvatar.playerHealth.hurtFreeze)
		{
			if (mapToolController.Active && !playerAvatar.playerAvatarVisuals.animInCrawl)
			{
				SetPose(mapPose);
				HeadAnimate(_active: true);
				AnimatePose();
			}
			else if (((Component)physGrabBeam).gameObject.activeSelf && (!mapToolController.Active || !playerAvatar.playerAvatarVisuals.animInCrawl))
			{
				SetPose(grabberPose);
				HeadAnimate(_active: false);
				AnimatePose();
			}
			else
			{
				SetPose(basePose);
				HeadAnimate(_active: false);
				AnimatePose();
			}
			GrabberLogic();
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
			headRotation = Mathf.Lerp(headRotation, num * 0.5f, 20f * deltaTime);
		}
		else
		{
			headRotation = Mathf.Lerp(headRotation, 0f, 20f * deltaTime);
		}
	}

	private void AnimatePose()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (poseLerp < 1f)
		{
			poseLerp += poseSpeed * deltaTime;
			poseCurrent = Vector3.LerpUnclamped(poseOld, poseNew, poseCurve.Evaluate(poseLerp));
		}
		rightArmTransform.localEulerAngles = new Vector3(poseCurrent.x, poseCurrent.y + headRotation, poseCurrent.z);
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

	private void GrabberClawLogic()
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatar.physGrabber.physGrabBeam.activeSelf)
		{
			if (grabberClawHidden)
			{
				grabberClawHidden = false;
				((Component)grabberClawParent).gameObject.SetActive(true);
			}
			grabberClawLerp = Mathf.Clamp01(grabberClawLerp + 3f * deltaTime);
		}
		else if (!grabberClawHidden)
		{
			if (grabberClawLerp <= 0f)
			{
				grabberClawHidden = true;
				((Component)grabberClawParent).gameObject.SetActive(false);
				grabberClawRotation = 0f;
			}
			grabberClawLerp = Mathf.Clamp01(grabberClawLerp - 3f * deltaTime);
		}
		if (!grabberClawHidden)
		{
			grabberClawChildLerp = Mathf.Clamp01(grabberClawChildLerp + 1f * deltaTime);
			if (grabberClawChildLerp >= 1f)
			{
				grabberClawChildLerp = 0f;
			}
			Vector3 val = Vector3.LerpUnclamped(new Vector3(60f, 0f, 0f), new Vector3(80f, 0f, 0f), grabberClawChildCurve.Evaluate(grabberClawChildLerp));
			for (int i = 0; i < grabberClawChildren.Length; i++)
			{
				grabberClawChildren[i].localRotation = Quaternion.Euler(val);
			}
			float num = Mathf.LerpUnclamped(500f, 200f, grabberClawChildCurve.Evaluate(grabberClawChildLerp));
			grabberClawRotation += num * deltaTime;
			if (grabberClawRotation > 360f)
			{
				grabberClawRotation -= 360f;
			}
			grabberClawParent.localScale = Vector3.one * grabberClawHideCurve.Evaluate(grabberClawLerp);
			grabberClawParent.localRotation = Quaternion.Euler(0f, 0f, grabberClawRotation);
			grabberOrb.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, grabberClawChildCurve.Evaluate(grabberClawChildLerp));
			grabberLight.intensity = Mathf.Lerp(0f, grabberLightIntensity, grabberClawHideCurve.Evaluate(grabberClawLerp));
		}
	}

	private void GrabberLogic()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		GrabberClawLogic();
		grabberTransform.position = grabberTransformTarget.position;
		grabberTransform.localScale = grabberTransformTarget.localScale;
		grabberTransform.rotation = SemiFunc.SpringQuaternionGet(grabberClawSpring, grabberTransformTarget.rotation, deltaTime);
		Quaternion targetRotation = Quaternion.identity;
		Quaternion localRotation = rightArmParentTransform.localRotation;
		if (((Component)physGrabBeam).gameObject.activeSelf && !mapToolController.Active)
		{
			Vector3 position = grabberAimTarget.position;
			grabberAimTarget.position = physGrabBeam.PhysGrabPointPuller.position;
			float num = 0f;
			if (grabberAimTarget.localPosition.x < -1f)
			{
				num = 1f;
			}
			grabberAimTarget.localPosition = new Vector3(Mathf.Max(grabberAimTarget.localPosition.x, 0.2f), grabberAimTarget.localPosition.y, Mathf.Max(grabberAimTarget.localPosition.z, num));
			grabberAimTarget.position = Vector3.Lerp(position, grabberAimTarget.position, 30f * deltaTime);
			rightArmParentTransform.LookAt(grabberAimTarget);
			targetRotation = rightArmParentTransform.localRotation;
		}
		rightArmParentTransform.localRotation = localRotation;
		rightArmParentTransform.localRotation = SemiFunc.SpringQuaternionGet(grabberSteerSpring, targetRotation, deltaTime);
		grabberReachDifferenceTimer += deltaTime;
		if (grabberReachDifferenceTimer > 1f)
		{
			grabberReachDifference = 0f;
			grabberReachDifferenceTimer = 0f;
		}
		grabberReachDifference += grabberReachPrevious - playerAvatar.physGrabber.pullerDistance;
		grabberReachPrevious = playerAvatar.physGrabber.pullerDistance;
		if (Mathf.Abs(grabberReachDifference) > 1f)
		{
			if (grabberReachDifference < 0f)
			{
				grabberReachTarget = 0.2f;
			}
			else
			{
				grabberReachTarget = -0.2f;
			}
			grabberReachTimer = 0.25f;
			grabberReachDifference = 0f;
		}
		else
		{
			grabberReachTimer -= deltaTime;
			if (grabberReachTimer <= 0f)
			{
				grabberReachTarget = 0f;
			}
		}
		float num2 = SemiFunc.SpringFloatGet(grabberReachSpring, grabberReachTarget, deltaTime);
		rightArmParentTransform.localScale = new Vector3(1f, 1f, 1f + num2);
		if (playerAvatar.physGrabber.healing)
		{
			if (!grabberHealing)
			{
				grabberLight.color = grabberLightColorHeal;
				GameObject[] array = grabberOrbSpheres;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].GetComponent<Renderer>().material = grabberMaterialHeal;
				}
				grabberHealing = true;
			}
		}
		else if (grabberHealing)
		{
			grabberLight.color = grabberLightColor;
			GameObject[] array = grabberOrbSpheres;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().material = grabberMaterial;
			}
			grabberHealing = false;
		}
	}
}
