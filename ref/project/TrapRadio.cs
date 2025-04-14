using System;
using UnityEngine;
using UnityEngine.Events;

public class TrapRadio : Trap
{
	public UnityEvent radioTimer;

	public MeshRenderer RadioDisplay;

	public Light RadioLight;

	public Transform RadioMeter;

	public AnimationCurve RadioFlickerCurve;

	public float RadioFlickerTime = 0.5f;

	private float RadioFlickerTimer;

	private bool RadioFlickerIntro = true;

	private bool RadioFlickerOutro;

	[Space]
	[Header("Gramophone Components")]
	public GameObject Radio;

	[Space]
	[Header("Sounds")]
	public Sound RadioStart;

	public Sound RadioEnd;

	public Sound RadioLoop;

	[Space]
	[Header("Radio Animation")]
	public AnimationCurve RadioStartCurve;

	public float RadioStartIntensity;

	public float RadioStartDuration;

	[Space]
	public AnimationCurve RadioEndCurve;

	public float RadioEndIntensity;

	public float RadioEndDuration;

	private bool StartSequence;

	private bool endSequence;

	private float StartSequenceProgress;

	private float EndSequenceProgress;

	private bool RadioPlaying;

	private Quaternion initialRadioRotation;

	private Quaternion initialRadioMeterRotation;

	protected override void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		initialRadioRotation = Radio.transform.localRotation;
		initialRadioMeterRotation = RadioMeter.localRotation;
		((Behaviour)RadioLight).enabled = false;
		((Renderer)RadioDisplay).enabled = false;
	}

	protected override void Update()
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (trapStart)
		{
			RadioTrapActivated();
		}
		RadioLoop.PlayLoop(RadioPlaying, 2f, 2f);
		if (!trapActive)
		{
			return;
		}
		enemyInvestigate = true;
		if (RadioFlickerIntro || RadioFlickerOutro)
		{
			float num = RadioFlickerCurve.Evaluate(RadioFlickerTimer / RadioFlickerTime);
			RadioFlickerTimer += 1f * Time.deltaTime;
			if (num > 0.5f)
			{
				((Behaviour)RadioLight).enabled = true;
				((Renderer)RadioDisplay).enabled = true;
			}
			else
			{
				((Behaviour)RadioLight).enabled = false;
				((Renderer)RadioDisplay).enabled = false;
			}
			if (RadioFlickerTimer > RadioFlickerTime)
			{
				RadioFlickerIntro = false;
				RadioFlickerTimer = 0f;
				if (RadioFlickerOutro)
				{
					((Behaviour)RadioLight).enabled = false;
					((Renderer)RadioDisplay).enabled = false;
				}
				else
				{
					((Behaviour)RadioLight).enabled = true;
					((Renderer)RadioDisplay).enabled = true;
				}
			}
		}
		RadioPlaying = true;
		float num2 = 1f;
		if (StartSequenceProgress == 0f && !StartSequence)
		{
			StartSequence = true;
			RadioStart.Play(physGrabObject.centerPoint);
			((Behaviour)RadioLight).enabled = true;
			((Renderer)RadioDisplay).enabled = true;
		}
		if (StartSequence)
		{
			num2 += RadioStartCurve.Evaluate(StartSequenceProgress) * RadioStartIntensity;
			StartSequenceProgress += Time.deltaTime / RadioStartDuration;
			if (StartSequenceProgress >= 1f)
			{
				StartSequence = false;
			}
		}
		if (endSequence)
		{
			num2 += RadioEndCurve.Evaluate(EndSequenceProgress) * RadioEndIntensity;
			EndSequenceProgress += Time.deltaTime / RadioEndDuration;
			if (EndSequenceProgress >= 1f)
			{
				EndSequenceDone();
			}
		}
		float num3 = 1f * num2;
		float num4 = 40f;
		float num5 = num3 * Mathf.Sin(Time.time * num4);
		float num6 = num3 * Mathf.Sin(Time.time * num4 + MathF.PI / 2f);
		Radio.transform.localRotation = initialRadioRotation * Quaternion.Euler(num5, 0f, num6);
		num4 = 20f;
		float num7 = num3 * Mathf.Sin(Time.time * num4);
		RadioMeter.localRotation = initialRadioMeterRotation * Quaternion.Euler(0f, num7 * 90f, 0f);
		Radio.transform.localPosition = new Vector3(Radio.transform.localPosition.x, Radio.transform.localPosition.y - num5 * 0.005f * Time.deltaTime, Radio.transform.localPosition.z);
	}

	private void EndSequenceDone()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		endSequence = false;
		((Behaviour)RadioLight).enabled = false;
		((Renderer)RadioDisplay).enabled = false;
		RadioPlaying = false;
		trapActive = false;
		Radio.transform.localRotation = initialRadioRotation;
	}

	public void RadioTrapStop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RadioEnd.Play(physGrabObject.centerPoint);
		endSequence = true;
	}

	public void RadioTrapActivated()
	{
		if (!trapTriggered)
		{
			radioTimer.Invoke();
			trapTriggered = true;
			trapActive = true;
		}
	}
}
