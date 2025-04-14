using Photon.Pun;
using TMPro;
using UnityEngine;

public class MapToolController : MonoBehaviour
{
	public static MapToolController instance;

	internal bool Active;

	private bool ActivePrev;

	internal PhotonView photonView;

	public PlayerAvatar PlayerAvatar;

	[Space]
	public Transform FollowTransform;

	public Transform FollowTransformClient;

	[Space]
	public Transform ControllerTransform;

	public Transform VisualTransform;

	public Transform PlayerLookTarget;

	public Transform HideTransform;

	[Space]
	private float DisplayJointAngleDiff;

	private float DisplayJointAnglePreviousX;

	[Space]
	public float MoveMultiplier = 0.5f;

	public float FadeAmount = 0.5f;

	public float BobMultiplier = 0.1f;

	[Space]
	public Sound SoundStart;

	public Sound SoundStop;

	public Sound SoundLoop;

	[Space]
	public MeshRenderer DisplayMesh;

	public Material DisplayMaterial;

	public Material DisplayMaterialClient;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed;

	public AnimationCurve OutroCurve;

	public float OutroSpeed;

	internal float HideLerp;

	private float HideScale;

	[Space]
	public Transform displaySpringTransform;

	public Transform displaySpringTransformTarget;

	public SpringQuaternion displaySpring;

	[Space]
	public Transform mainSpringTransform;

	public Transform mainSpringTransformTarget;

	public SpringQuaternion mainSpring;

	private bool mapToggled;

	private void Start()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		((Component)VisualTransform).gameObject.SetActive(false);
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			((Renderer)DisplayMesh).material = DisplayMaterial;
			((Component)this).transform.parent.parent = FollowTransform;
			((Component)this).transform.parent.localPosition = Vector3.zero;
			((Component)this).transform.parent.localRotation = Quaternion.identity;
			return;
		}
		((Renderer)DisplayMesh).material = DisplayMaterialClient;
		Transform[] componentsInChildren = ((Component)VisualTransform).GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			((Component)componentsInChildren[i]).gameObject.layer = LayerMask.NameToLayer("Triggers");
		}
		SoundStart.SpatialBlend = 1f;
		SoundStop.SpatialBlend = 1f;
		SoundLoop.SpatialBlend = 1f;
	}

	private void Update()
	{
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			if (!PlayerAvatar.isDisabled && !PlayerAvatar.isTumbling && !CameraAim.Instance.AimTargetActive && !SemiFunc.MenuLevel())
			{
				if (InputManager.instance.InputToggleGet(InputKey.Map))
				{
					if (SemiFunc.InputDown(InputKey.Map))
					{
						mapToggled = !mapToggled;
					}
				}
				else
				{
					mapToggled = false;
				}
				if ((SemiFunc.InputHold(InputKey.Map) || mapToggled || Map.Instance.debugActive) && !PlayerController.instance.sprinting)
				{
					if (HideLerp >= 1f)
					{
						Active = true;
					}
				}
				else
				{
					mapToggled = false;
					if (HideLerp <= 0f)
					{
						Active = false;
					}
				}
			}
			else
			{
				Active = false;
			}
			if (Active)
			{
				StatsUI.instance.Show();
				ItemInfoUI.instance.Hide();
				ItemInfoExtraUI.instance.Hide();
				if (((TMP_Text)MissionUI.instance.Text).text != "")
				{
					MissionUI.instance.Show();
				}
			}
		}
		if (Active != ActivePrev)
		{
			ActivePrev = Active;
			if (GameManager.Multiplayer() && photonView.IsMine)
			{
				photonView.RPC("SetActiveRPC", (RpcTarget)1, new object[1] { Active });
			}
			if (Active)
			{
				if (!GameManager.Multiplayer() || photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(active: true);
				}
				((Component)VisualTransform).gameObject.SetActive(true);
				SoundStart.Play(((Component)this).transform.position);
			}
			else
			{
				if (!GameManager.Multiplayer() || photonView.IsMine)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.1f);
					Map.Instance.ActiveSet(active: false);
				}
				SoundStop.Play(((Component)this).transform.position);
			}
		}
		float num = 90f;
		if (GameManager.Multiplayer() && !photonView.IsMine)
		{
			num = 0f;
		}
		float num2 = 1f;
		if (GameManager.Multiplayer() && !photonView.IsMine)
		{
			num2 = 2f;
		}
		if (Active)
		{
			if (HideLerp > 0f)
			{
				HideLerp -= Time.deltaTime * IntroSpeed * num2;
			}
			HideScale = Mathf.LerpUnclamped(1f, 0f, IntroCurve.Evaluate(HideLerp));
			HideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(num, 0f, 0f), IntroCurve.Evaluate(HideLerp));
		}
		else
		{
			if (HideLerp < 1f)
			{
				HideLerp += Time.deltaTime * OutroSpeed * num2;
				if (HideLerp > 1f)
				{
					((Component)VisualTransform).gameObject.SetActive(false);
				}
			}
			HideScale = Mathf.LerpUnclamped(1f, 0f, OutroCurve.Evaluate(HideLerp));
		}
		if ((!GameManager.Multiplayer() || photonView.IsMine) && Active)
		{
			PlayerController.instance.MoveMult(MoveMultiplier, 0.1f);
			CameraTopFade.Instance.Set(FadeAmount, 0.1f);
			CameraBob.Instance.SetMultiplier(BobMultiplier, 0.1f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.05f, 2f, 2f, ((Component)this).gameObject, 100);
			CameraNoise.Instance.Override(0.025f, 0.25f);
			Aim.instance.SetState(Aim.State.Hidden);
		}
		Vector3 val = Vector3.one;
		if (GameManager.Multiplayer() && !photonView.IsMine)
		{
			((Component)this).transform.parent.position = FollowTransformClient.position;
			((Component)this).transform.parent.rotation = FollowTransformClient.rotation;
			val = FollowTransformClient.localScale;
			mainSpringTransform.rotation = SemiFunc.SpringQuaternionGet(mainSpring, mainSpringTransformTarget.rotation);
		}
		((Component)this).transform.parent.localScale = Vector3.Lerp(((Component)this).transform.parent.localScale, val * HideScale, Time.deltaTime * 20f);
		displaySpringTransform.rotation = SemiFunc.SpringQuaternionGet(displaySpring, displaySpringTransformTarget.rotation);
		if (Active)
		{
			DisplayJointAngleDiff = (DisplayJointAnglePreviousX - displaySpringTransform.localRotation.x) * 50f;
			DisplayJointAngleDiff = Mathf.Clamp(DisplayJointAngleDiff, -0.1f, 0.1f);
			DisplayJointAnglePreviousX = displaySpringTransform.localRotation.x;
			SoundLoop.LoopPitch = Mathf.Lerp(SoundLoop.LoopPitch, 1f - DisplayJointAngleDiff, Time.deltaTime * 10f);
		}
		SoundLoop.PlayLoop(Active, 5f, 5f);
	}

	[PunRPC]
	public void SetActiveRPC(bool active)
	{
		Active = active;
	}
}
