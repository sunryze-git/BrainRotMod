using UnityEngine;

public class CameraAim : MonoBehaviour
{
	public static CameraAim Instance;

	public CameraTarget camController;

	public Transform playerTransform;

	public float AimSpeedMouse = 1f;

	public float AimSpeedGamepad = 1f;

	private float aimVertical;

	private float aimHorizontal;

	internal float aimSmoothOriginal = 2f;

	private Quaternion playerAim = Quaternion.identity;

	private Vector3 AimTargetPosition = Vector3.zero;

	public AnimationCurve AimTargetCurve;

	[Space]
	public bool AimTargetActive;

	private float AimTargetTimer;

	private float AimTargetSpeed;

	private float AimTargetLerp;

	private GameObject AimTargetObject;

	private int AimTargetPriority = 999;

	private bool AimTargetSoftActive;

	private float AimTargetSoftTimer;

	private float AimTargetSoftStrengthCurrent;

	private float AimTargetSoftStrength;

	private float AimTargetSoftStrengthNoAim;

	private Vector3 AimTargetSoftPosition;

	private GameObject AimTargetSoftObject;

	private int AimTargetSoftPriority = 999;

	private float overrideAimStopTimer;

	internal bool overrideAimStop;

	private float PlayerAimingTimer;

	private float overrideAimSmooth;

	private float overrideAimSmoothTimer;

	private void Awake()
	{
		Instance = this;
	}

	public void AimTargetSet(Vector3 position, float time, float speed, GameObject obj, int priority)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (priority <= AimTargetPriority && (!((Object)(object)obj != (Object)(object)AimTargetObject) || AimTargetLerp == 0f))
		{
			AimTargetActive = true;
			AimTargetObject = obj;
			AimTargetPosition = position;
			AimTargetTimer = time;
			AimTargetSpeed = speed;
			AimTargetPriority = priority;
		}
	}

	public void AimTargetSoftSet(Vector3 position, float time, float strength, float strengthNoAim, GameObject obj, int priority)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (priority <= AimTargetSoftPriority && (!Object.op_Implicit((Object)(object)AimTargetSoftObject) || !((Object)(object)obj != (Object)(object)AimTargetSoftObject)))
		{
			if ((Object)(object)obj != (Object)(object)AimTargetSoftObject)
			{
				PlayerAimingTimer = 0f;
			}
			AimTargetSoftPosition = position;
			AimTargetSoftTimer = time;
			AimTargetSoftStrength = strength;
			AimTargetSoftStrengthNoAim = strengthNoAim;
			AimTargetSoftObject = obj;
			AimTargetSoftPriority = priority;
		}
	}

	public void CameraAimSpawn(float _rotation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		aimHorizontal = _rotation;
		playerAim = Quaternion.Euler(aimVertical, aimHorizontal, 0f);
		((Component)this).transform.localRotation = playerAim;
	}

	public void OverrideAimStop()
	{
		overrideAimStopTimer = 0.2f;
	}

	private void OverrideAimStopTick()
	{
		if (overrideAimStopTimer > 0f)
		{
			overrideAimStop = true;
			overrideAimStopTimer -= Time.deltaTime;
		}
		else
		{
			overrideAimStop = false;
		}
	}

	private void Update()
	{
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		AimSpeedMouse = Mathf.Lerp(0.2f, 4f, GameplayManager.instance.aimSensitivity / 100f);
		if (GameDirector.instance.currentState >= GameDirector.gameState.Main)
		{
			if (!GameDirector.instance.DisableInput && AimTargetTimer <= 0f && !overrideAimStop)
			{
				InputManager.instance.mouseSensitivity = 0.05f;
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(SemiFunc.InputMouseX(), SemiFunc.InputMouseY());
				Vector2 val2 = default(Vector2);
				((Vector2)(ref val2))._002Ector(Input.GetAxis("Gamepad Aim X"), Input.GetAxis("Gamepad Aim Y"));
				val2 = Vector2.zero;
				if (AimTargetSoftTimer > 0f)
				{
					val = ((!(((Vector2)(ref val)).magnitude > 1f)) ? Vector2.zero : ((Vector2)(ref val)).normalized);
					val2 = ((!(((Vector2)(ref val2)).magnitude > 0.1f)) ? Vector2.zero : ((Vector2)(ref val2)).normalized);
				}
				else
				{
					val *= AimSpeedMouse;
					val2 *= AimSpeedGamepad * Time.deltaTime;
				}
				aimHorizontal += ((Vector2)(ref val))[0];
				aimHorizontal += ((Vector2)(ref val2))[0];
				if (aimHorizontal > 360f)
				{
					aimHorizontal -= 360f;
				}
				if (aimHorizontal < -360f)
				{
					aimHorizontal += 360f;
				}
				aimVertical += 0f - ((Vector2)(ref val))[1];
				aimVertical += 0f - ((Vector2)(ref val2))[1];
				aimVertical = Mathf.Clamp(aimVertical, -70f, 80f);
				playerAim = Quaternion.Euler(aimVertical, aimHorizontal, 0f);
				if (GameplayManager.instance.cameraSmoothing != 0f)
				{
					playerAim = Quaternion.RotateTowards(((Component)this).transform.localRotation, playerAim, 10000f * Time.deltaTime);
				}
				if (((Vector2)(ref val2)).magnitude > 0f || ((Vector2)(ref val)).magnitude > 0f)
				{
					PlayerAimingTimer = 0.1f;
				}
			}
			if (PlayerAimingTimer > 0f)
			{
				PlayerAimingTimer -= Time.deltaTime;
			}
			if (AimTargetTimer > 0f)
			{
				AimTargetTimer -= Time.deltaTime;
				AimTargetLerp += Time.deltaTime * AimTargetSpeed;
				AimTargetLerp = Mathf.Clamp01(AimTargetLerp);
			}
			else if (AimTargetLerp > 0f)
			{
				ResetPlayerAim(((Component)this).transform.localRotation);
				AimTargetLerp = 0f;
				AimTargetPriority = 999;
				AimTargetActive = false;
			}
			Quaternion val3 = Quaternion.LerpUnclamped(playerAim, Quaternion.LookRotation(AimTargetPosition - ((Component)this).transform.position), AimTargetCurve.Evaluate(AimTargetLerp));
			if (AimTargetSoftTimer > 0f && AimTargetTimer <= 0f)
			{
				float num = AimTargetSoftStrength;
				if (PlayerAimingTimer <= 0f)
				{
					num = AimTargetSoftStrengthNoAim;
				}
				AimTargetSoftStrengthCurrent = Mathf.Lerp(AimTargetSoftStrengthCurrent, num, 10f * Time.deltaTime);
				Quaternion val4 = Quaternion.LookRotation(AimTargetSoftPosition - ((Component)this).transform.position);
				val3 = Quaternion.Lerp(val3, val4, num * Time.deltaTime);
				AimTargetSoftTimer -= Time.deltaTime;
				if (AimTargetSoftTimer <= 0f)
				{
					AimTargetSoftObject = null;
					AimTargetSoftPriority = 999;
				}
			}
			float num2 = (aimSmoothOriginal = Mathf.Lerp(50f, 8f, GameplayManager.instance.cameraSmoothing / 100f));
			if (overrideAimSmoothTimer > 0f)
			{
				num2 = overrideAimSmooth;
			}
			((Component)this).transform.localRotation = Quaternion.Lerp(((Component)this).transform.localRotation, val3, num2 * Time.deltaTime);
			ResetPlayerAim(val3);
		}
		if (SemiFunc.MenuLevel() && Object.op_Implicit((Object)(object)CameraNoPlayerTarget.instance))
		{
			((Component)this).transform.localRotation = ((Component)CameraNoPlayerTarget.instance).transform.rotation;
		}
		if (overrideAimSmoothTimer > 0f)
		{
			overrideAimSmoothTimer -= Time.deltaTime;
		}
		OverrideAimStopTick();
	}

	private void ResetPlayerAim(Quaternion _rotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (((Quaternion)(ref _rotation)).eulerAngles.x > 180f)
		{
			aimVertical = ((Quaternion)(ref _rotation)).eulerAngles.x - 360f;
		}
		else
		{
			aimVertical = ((Quaternion)(ref _rotation)).eulerAngles.x;
		}
		aimHorizontal = ((Quaternion)(ref _rotation)).eulerAngles.y;
		playerAim = _rotation;
	}

	public void OverrideAimSmooth(float _smooth, float _time)
	{
		overrideAimSmooth = _smooth;
		overrideAimSmoothTimer = _time;
	}
}
