using UnityEngine;

public class SledgehammerController : MonoBehaviour
{
	public Transform ControllerTransform;

	public Transform FollowTransform;

	[Space]
	public SledgehammerSwing Swing;

	public SledgehammerHit Hit;

	public SledgehammerTransition Transition;

	private RoachTrigger Roach;

	[Space]
	[Header("Sounds")]
	public Sound SoundMoveLong;

	public Sound SoundMoveShort;

	public Sound SoundSwing;

	public Sound SoundHit;

	public Sound SoundHitOutro;

	private bool OutroAudioPlay = true;

	private LayerMask Mask;

	private Camera MainCamera;

	private bool InteractImpulse;

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		MainCamera = Camera.main;
		Mask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Default" }));
		((Component)Hit).gameObject.SetActive(false);
		((Component)Transition).gameObject.SetActive(false);
		SoundMoveLong.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	private void OnTriggerStay(Collider other)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (!ToolController.instance.ToolHide.Active || !Swing.CanHit)
		{
			return;
		}
		RoachTrigger component = ((Component)other).GetComponent<RoachTrigger>();
		if (Object.op_Implicit((Object)(object)component))
		{
			Vector3 val = ((Component)component).transform.position - ((Component)MainCamera).transform.position;
			_ = ((Vector3)(ref val)).magnitude;
			val = ((Component)component).transform.position - ((Component)MainCamera).transform.position;
			_ = ((Vector3)(ref val)).normalized;
			Vector3 position = ((Component)MainCamera).transform.position;
			val = ((Component)component).transform.position - ((Component)MainCamera).transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			val = ((Component)component).transform.position - ((Component)MainCamera).transform.position;
			RaycastHit val2 = default(RaycastHit);
			if (!Physics.Raycast(position, normalized, ref val2, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(Mask)))
			{
				Roach = component;
				Swing.HitOutro();
				Swing.CanHit = false;
				((Component)Transition).gameObject.SetActive(true);
				Transition.IntroSet();
				((Component)Hit).gameObject.SetActive(true);
				Hit.Spawn(Roach);
			}
		}
	}

	public void HitDone()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		((Component)Transition).gameObject.SetActive(true);
		Transition.OutroSet();
		GameDirector.instance.CameraImpact.Shake(3f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		SoundHitOutro.Play(((Component)this).transform.position);
		((Component)Hit).gameObject.SetActive(false);
	}

	public void IntroDone()
	{
		GameDirector.instance.CameraImpact.Shake(5f, 0f);
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		((Component)Hit).gameObject.SetActive(true);
		Hit.Hit();
		Roach.RoachOrbit.Squash();
	}

	public void OutroDone()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((Component)Swing).gameObject.SetActive(true);
		((Component)Swing.MeshTransform).gameObject.SetActive(true);
		PlayerController.instance.MoveForce(((Component)PlayerController.instance).transform.forward, -5f, 0.25f);
	}

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)Hit).gameObject.activeSelf)
		{
			Vector3 val = ((Component)Hit).transform.position - ((Component)PlayerController.instance).transform.position;
			float magnitude = ((Vector3)(ref val)).magnitude;
			PlayerController.instance.MoveForce(((Component)Hit).transform.position - ((Component)PlayerController.instance).transform.position, magnitude * 30f, 0.01f);
			PlayerController.instance.InputDisable(0.1f);
		}
		if (ToolController.instance.Interact && !Swing.Swinging)
		{
			InteractImpulse = true;
		}
		if (InteractImpulse && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp >= 1f)
		{
			InteractImpulse = false;
			Swing.Swing();
		}
		if (Swing.Swinging || ((Component)Hit).gameObject.activeSelf)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
		}
		((Component)ControllerTransform).transform.position = ((Component)ToolController.instance.ToolTargetParent).transform.position;
		((Component)ControllerTransform).transform.rotation = ((Component)ToolController.instance.ToolTargetParent).transform.rotation;
		FollowTransform.position = ((Component)ToolController.instance.ToolFollow).transform.position;
		FollowTransform.rotation = ((Component)ToolController.instance.ToolFollow).transform.rotation;
		FollowTransform.localScale = ((Component)ToolController.instance.ToolHide).transform.localScale;
		if (OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			SoundMoveLong.Play(FollowTransform.position);
			OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}
}
