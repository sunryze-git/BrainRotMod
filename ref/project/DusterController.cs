using UnityEngine;

public class DusterController : MonoBehaviour
{
	public Transform FollowTransform;

	public Transform ParentTransform;

	[Space]
	public ToolActiveOffset ToolActiveOffset;

	public DusterDusting DusterDusting;

	public ToolBackAway ToolBackAway;

	private bool Dusting;

	private float DustingTimer;

	[Space]
	public Sound MoveSound;

	private bool OutroAudioPlay = true;

	private void Start()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		MoveSound.Play(((Component)this).transform.position);
	}

	private void Update()
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		if (ToolController.instance.Interact && ToolController.instance.ToolHide.Active && ToolController.instance.ToolHide.ActiveLerp > 0.75f)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (Object.op_Implicit((Object)(object)activeInteraction))
			{
				DirtyPainting component = ((Component)activeInteraction).GetComponent<DirtyPainting>();
				if (Object.op_Implicit((Object)(object)component))
				{
					CanvasHandler canvasHandler = component.CanvasHandler;
					if (Object.op_Implicit((Object)(object)canvasHandler) && canvasHandler.currentState != CanvasHandler.State.Clean)
					{
						DustingTimer = 0.5f;
						if (DusterDusting.ActiveAmount >= 0.1f)
						{
							canvasHandler.cleanInput = true;
							if (!canvasHandler.CleanDone && canvasHandler.fadeMultiplier <= 0.5f)
							{
								canvasHandler.CleanDone = true;
							}
						}
					}
				}
			}
		}
		if (DustingTimer > 0f)
		{
			ToolActiveOffset.Active = true;
			ToolBackAway.Active = true;
			Dusting = true;
			DustingTimer -= Time.deltaTime;
			if (DustingTimer <= 0f)
			{
				Dusting = false;
				ToolActiveOffset.Active = false;
				ToolBackAway.Active = false;
			}
		}
		if (Dusting && ToolActiveOffset.Active && ToolActiveOffset.ActiveLerp >= 0.3f)
		{
			DusterDusting.Active = true;
		}
		else
		{
			DusterDusting.Active = false;
		}
		FollowTransform.position = ((Component)ToolController.instance.ToolFollow).transform.position;
		FollowTransform.rotation = ((Component)ToolController.instance.ToolFollow).transform.rotation;
		FollowTransform.localScale = ((Component)ToolController.instance.ToolHide).transform.localScale;
		((Component)ParentTransform).transform.position = ((Component)ToolController.instance.ToolTargetParent).transform.position;
		((Component)ParentTransform).transform.rotation = ((Component)ToolController.instance.ToolTargetParent).transform.rotation;
		if (OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			MoveSound.Play(FollowTransform.position);
			OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
	}
}
