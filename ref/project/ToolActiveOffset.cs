using UnityEngine;

public class ToolActiveOffset : MonoBehaviour
{
	public bool Active;

	private bool ActivePrev;

	private bool ActiveCurrent;

	[HideInInspector]
	public float ActiveLerp = 1f;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed = 1.5f;

	[Space]
	public AnimationCurve OutroCurve;

	public float OutroSpeed = 1.5f;

	[Space]
	public Vector3 InactivePosition;

	public Vector3 InactiveRotation;

	[Space]
	public Vector3 ActivePosition;

	public Vector3 ActiveRotation;

	[Space]
	[Header("Sound")]
	public bool MoveSoundAutomatic;

	[HideInInspector]
	public bool MoveSoundManual;

	public Sound MoveSound;

	private void Update()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (Active != ActivePrev && ActiveLerp >= 1f)
		{
			if (MoveSoundAutomatic || MoveSoundManual)
			{
				MoveSoundManual = false;
				MoveSound.Play(((Component)this).transform.position);
			}
			ActiveLerp = 0f;
			ActivePrev = Active;
			ActiveCurrent = Active;
		}
		else
		{
			if (ActiveCurrent)
			{
				ActiveLerp += IntroSpeed * Time.deltaTime;
			}
			else
			{
				ActiveLerp += OutroSpeed * Time.deltaTime;
			}
			ActiveLerp = Mathf.Clamp01(ActiveLerp);
		}
		if (ActiveCurrent)
		{
			((Component)this).transform.localPosition = Vector3.LerpUnclamped(InactivePosition, ActivePosition, IntroCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(InactiveRotation.x, InactiveRotation.y, InactiveRotation.z), Quaternion.Euler(ActiveRotation.x, ActiveRotation.y, ActiveRotation.z), IntroCurve.Evaluate(ActiveLerp));
		}
		else
		{
			((Component)this).transform.localPosition = Vector3.LerpUnclamped(ActivePosition, InactivePosition, OutroCurve.Evaluate(ActiveLerp));
			((Component)this).transform.localRotation = Quaternion.LerpUnclamped(Quaternion.Euler(ActiveRotation.x, ActiveRotation.y, ActiveRotation.z), Quaternion.Euler(InactiveRotation.x, InactiveRotation.y, InactiveRotation.z), OutroCurve.Evaluate(ActiveLerp));
		}
	}
}
