using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aim : MonoBehaviour
{
	public enum State
	{
		Default,
		Grabbable,
		Grab,
		Rotate,
		Hidden
	}

	[Serializable]
	public class AimState
	{
		public State State;

		public Sprite Sprite;

		public Color Color;
	}

	public static Aim instance;

	[Space]
	public AnimationCurve curveIntro;

	public AnimationCurve curveOutro;

	private float animLerp;

	[Space]
	public List<AimState> aimStates;

	private AimState defaultState;

	private Image image;

	private float stateTimer;

	private State currentState;

	private State previousState;

	private Sprite currentSprite;

	private Color currentColor;

	private void Awake()
	{
		instance = this;
		image = ((Component)this).GetComponent<Image>();
		defaultState = aimStates[0];
	}

	private void Update()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if (stateTimer > 0f)
		{
			stateTimer -= 1f * Time.deltaTime;
		}
		else if (currentState != 0)
		{
			animLerp = 0f;
			currentState = State.Default;
			currentSprite = defaultState.Sprite;
			currentColor = defaultState.Color;
		}
		if (currentState == previousState)
		{
			if (animLerp < 1f)
			{
				animLerp += 10f * Time.deltaTime;
				((Component)this).transform.localScale = Vector3.one * curveOutro.Evaluate(animLerp);
			}
		}
		else
		{
			animLerp += 15f * Time.deltaTime;
			((Component)this).transform.localScale = Vector3.one * curveIntro.Evaluate(animLerp);
			if (animLerp >= 1f)
			{
				image.sprite = currentSprite;
				((Graphic)image).color = currentColor;
				previousState = currentState;
				animLerp = 0f;
			}
		}
		if (previousState == currentState)
		{
			if (currentState == State.Rotate)
			{
				Transform transform = ((Component)this).transform;
				Quaternion localRotation = ((Component)this).transform.localRotation;
				transform.localRotation = Quaternion.Euler(0f, 0f, ((Quaternion)(ref localRotation)).eulerAngles.z - 100f * Time.deltaTime);
			}
			else
			{
				((Component)this).transform.localRotation = Quaternion.identity;
			}
		}
	}

	public void SetState(State _state)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (_state == currentState)
		{
			stateTimer = 0.25f;
			return;
		}
		foreach (AimState aimState in aimStates)
		{
			if (aimState.State == _state)
			{
				currentState = aimState.State;
				currentSprite = aimState.Sprite;
				currentColor = aimState.Color;
				animLerp = 0f;
				stateTimer = 0.2f;
				break;
			}
		}
	}
}
