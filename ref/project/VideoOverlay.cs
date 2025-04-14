using UnityEngine;
using UnityEngine.UI;

public class VideoOverlay : MonoBehaviour
{
	public static VideoOverlay Instance;

	public RawImage RawImage;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed;

	private float IntroLerp;

	[Space]
	public float IdleAlpha;

	public float IdleCooldownMin;

	public float IdleCooldownMax;

	private float IdleCooldown;

	public float IdleTimeMin;

	public float IdleTimeMax;

	private float IdleTimer = 0.1f;

	private float OverrideTimer;

	private float OverrideAmount;

	private float OverrideSpeed;

	private void Awake()
	{
		Instance = this;
	}

	public void Override(float time, float amount, float speed)
	{
		OverrideTimer = time;
		OverrideAmount = amount;
		OverrideSpeed = speed;
	}

	private void Update()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			((Graphic)RawImage).color = Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)5));
			return;
		}
		if ((GameDirector.instance.currentState == GameDirector.gameState.Start && LoadingUI.instance.levelAnimationCompleted) || GameDirector.instance.currentState == GameDirector.gameState.Outro)
		{
			((Graphic)RawImage).color = Color.Lerp(((Graphic)RawImage).color, Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)50)), 20f * Time.deltaTime);
			return;
		}
		if (IntroLerp < 1f)
		{
			IntroLerp += Time.deltaTime * 0.5f;
			float num = IntroCurve.Evaluate(IntroLerp);
			((Graphic)RawImage).color = Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(5f * num)));
			return;
		}
		if (OverrideTimer > 0f)
		{
			OverrideTimer -= Time.deltaTime;
			((Graphic)RawImage).color = Color.Lerp(((Graphic)RawImage).color, Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255f * OverrideAmount))), Time.deltaTime * OverrideSpeed);
			return;
		}
		float num2 = 0f;
		if (IdleTimer > 0f)
		{
			IdleTimer -= Time.deltaTime;
			num2 = IdleAlpha;
			if (!GraphicsManager.instance.glitchLoop || Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
			{
				num2 = 0f;
			}
			if (IdleTimer <= 0f)
			{
				IdleCooldown = Random.Range(IdleCooldownMin, IdleCooldownMax);
			}
		}
		else if (IdleCooldown > 0f)
		{
			IdleCooldown -= Time.deltaTime;
		}
		else
		{
			IdleTimer = Random.Range(IdleTimeMin, IdleTimeMax);
		}
		((Graphic)RawImage).color = Color.Lerp(((Graphic)RawImage).color, Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(100f * num2))), Time.deltaTime * 1f);
	}
}
