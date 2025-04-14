using UnityEngine;

public class SledgehammerTransition : MonoBehaviour
{
	public SledgehammerController Controller;

	private Vector3 PositionStart;

	private Quaternion RotationStart;

	private Vector3 ScaleStart;

	[Space]
	public Transform SwingTarget;

	public Transform HitTarget;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed = 1f;

	[Space]
	public AnimationCurve OutroCurve;

	public float OutroSpeed = 1f;

	private float LerpAmount;

	private bool Intro;

	public void IntroSet()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Intro = true;
		LerpAmount = 0f;
		PositionStart = SwingTarget.position;
		RotationStart = SwingTarget.rotation;
		ScaleStart = SwingTarget.localScale;
		((Component)this).transform.position = PositionStart;
		((Component)this).transform.rotation = RotationStart;
		((Component)this).transform.localScale = ScaleStart;
	}

	public void OutroSet()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Intro = false;
		LerpAmount = 0f;
		PositionStart = HitTarget.position;
		RotationStart = HitTarget.rotation;
		ScaleStart = HitTarget.localScale;
		((Component)this).transform.position = PositionStart;
		((Component)this).transform.rotation = RotationStart;
		((Component)this).transform.localScale = ScaleStart;
	}

	public void Update()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!(LerpAmount < 1f))
		{
			return;
		}
		if (Intro)
		{
			LerpAmount += IntroSpeed * Time.deltaTime;
			((Component)this).transform.position = Vector3.Lerp(PositionStart, HitTarget.position, IntroCurve.Evaluate(LerpAmount));
			((Component)this).transform.rotation = Quaternion.Lerp(RotationStart, HitTarget.rotation, IntroCurve.Evaluate(LerpAmount));
			((Component)this).transform.localScale = Vector3.Lerp(ScaleStart, HitTarget.localScale, IntroCurve.Evaluate(LerpAmount));
		}
		else
		{
			LerpAmount += OutroSpeed * Time.deltaTime;
			((Component)this).transform.position = Vector3.Lerp(PositionStart, SwingTarget.position, OutroCurve.Evaluate(LerpAmount));
			((Component)this).transform.rotation = Quaternion.Lerp(RotationStart, SwingTarget.rotation, OutroCurve.Evaluate(LerpAmount));
			((Component)this).transform.localScale = Vector3.Lerp(ScaleStart, SwingTarget.localScale, OutroCurve.Evaluate(LerpAmount));
		}
		if (LerpAmount >= 1f)
		{
			if (Intro)
			{
				Controller.IntroDone();
			}
			else
			{
				Controller.OutroDone();
			}
			((Component)this).gameObject.SetActive(false);
		}
	}
}
