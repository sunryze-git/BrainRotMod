using UnityEngine;

public class MenuElementAnimations : MonoBehaviour
{
	private SpringFloat springFloatScale;

	private SpringFloat springFloatPosX;

	private SpringFloat springFloatPosY;

	private SpringFloat springFloatRotation;

	private RectTransform rectTransform;

	private Vector2 initialPosition;

	private float initialScale;

	private float initialRotation;

	public bool forceMiddlePivot = true;

	private void Start()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		if (forceMiddlePivot)
		{
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
		initialPosition = rectTransform.anchoredPosition;
		initialScale = ((Transform)rectTransform).localScale.x;
		initialRotation = ((Transform)rectTransform).localEulerAngles.z;
		springFloatPosX = new SpringFloat();
		springFloatPosY = new SpringFloat();
		springFloatScale = new SpringFloat();
		springFloatRotation = new SpringFloat();
		springFloatPosX.lastPosition = initialPosition.x;
		springFloatPosY.lastPosition = initialPosition.y;
		springFloatScale.lastPosition = initialScale;
		springFloatRotation.lastPosition = initialRotation;
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		float num = SemiFunc.SpringFloatGet(springFloatPosX, initialPosition.x);
		float num2 = SemiFunc.SpringFloatGet(springFloatPosY, initialPosition.y);
		float num3 = SemiFunc.SpringFloatGet(springFloatScale, initialScale);
		float num4 = SemiFunc.SpringFloatGet(springFloatRotation, initialRotation);
		rectTransform.anchoredPosition = new Vector2(num, num2);
		((Transform)rectTransform).localScale = new Vector3(num3, num3, 1f);
		((Transform)rectTransform).localEulerAngles = new Vector3(0f, 0f, num4);
		if (Input.GetKeyDown((KeyCode)257))
		{
			UIAniNudgeX();
		}
		if (Input.GetKeyDown((KeyCode)258))
		{
			Debug.Log((object)"Nudge Y");
			UIAniNudgeY();
		}
		if (Input.GetKeyDown((KeyCode)259))
		{
			UIAniScale();
		}
		if (Input.GetKeyDown((KeyCode)260))
		{
			UIAniRotate();
		}
	}

	public void UIAniNewInitialPosition(Vector2 newPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		initialPosition = newPos;
	}

	public void UIAniNudgeX(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		springFloatPosX.damping = dampen;
		springFloatPosX.springVelocity = nudgeForce * 100f;
		springFloatPosX.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	public void UIAniNudgeY(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		springFloatPosY.damping = dampen;
		springFloatPosY.springVelocity = nudgeForce * 100f;
		springFloatPosY.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	public void UIAniScale(float scaleForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		springFloatScale.damping = dampen;
		springFloatScale.springVelocity = scaleForce * 1f;
		springFloatScale.speed = scaleForce * 15f * springStrengthMultiplier;
	}

	public void UIAniRotate(float rotateForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		springFloatRotation.damping = dampen;
		springFloatRotation.springVelocity = rotateForce * 100f;
		springFloatRotation.speed = rotateForce * 15f * springStrengthMultiplier;
	}
}
