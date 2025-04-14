using UnityEngine;

public class ToolHide : MonoBehaviour
{
	public ToolController ToolController;

	public AnimationCurve ShowCurve;

	public AnimationCurve ShowScaleCurve;

	public AnimationCurve HideCurve;

	public AnimationCurve HideScaleCurve;

	[HideInInspector]
	public bool Active;

	[HideInInspector]
	public float ActiveLerp;

	private float ShowTimer;

	public void Show()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		ShowTimer = 0.02f;
		((Component)this).transform.localPosition = ToolController.CurrentHidePosition;
		((Component)this).transform.localRotation = Quaternion.Euler(ToolController.CurrentHideRotation.x, ToolController.CurrentHideRotation.y, ToolController.CurrentHideRotation.z);
		ActiveLerp = 0f;
		Active = true;
	}

	public void Hide()
	{
		ActiveLerp = 0f;
		Active = false;
	}

	private void Update()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		if (ActiveLerp < 1f)
		{
			ActiveLerp += ToolController.CurrentHideSpeed * Time.deltaTime;
			ActiveLerp = Mathf.Clamp01(ActiveLerp);
			if (ActiveLerp >= 1f && !Active)
			{
				ToolController.HideTool();
			}
		}
		if (Active)
		{
			((Component)this).transform.localPosition = Vector3.LerpUnclamped(ToolController.CurrentHidePosition, new Vector3(0f, 0f, 0f), ShowCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(ToolController.CurrentHideRotation.x, ToolController.CurrentHideRotation.y, ToolController.CurrentHideRotation.z), Quaternion.Euler(0f, 0f, 0f), ShowCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localScale = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), ShowScaleCurve.Evaluate(ActiveLerp));
		}
		else if (ActiveLerp < 1f)
		{
			((Component)this).transform.localPosition = Vector3.LerpUnclamped(new Vector3(0f, 0f, 0f), ToolController.CurrentHidePosition, HideCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(ToolController.CurrentHideRotation.x, ToolController.CurrentHideRotation.y, ToolController.CurrentHideRotation.z), HideCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localScale = Vector3.LerpUnclamped(new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), HideScaleCurve.Evaluate(ActiveLerp));
		}
		else
		{
			((Component)this).transform.localPosition = Vector3.zero;
			((Component)this).transform.localRotation = Quaternion.identity;
		}
		if (ShowTimer > 0f)
		{
			ShowTimer -= 1f * Time.deltaTime;
			if (ShowTimer <= 0f)
			{
				ToolController.ShowTool();
			}
		}
	}
}
