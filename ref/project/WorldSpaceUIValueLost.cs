using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIValueLost : WorldSpaceUIChild
{
	internal float timer;

	private float flashTimer = 0.2f;

	private Vector3 scale;

	private TextMeshProUGUI text;

	private Color textColor;

	private float shakeXAmount;

	private float shakeYAmount;

	private float floatY;

	private float shakeTimerX;

	private float shakeXTarget;

	private float shakeX;

	private float shakeTimerY;

	private float shakeYTarget;

	private float shakeY;

	public AnimationCurve curveIntro;

	public AnimationCurve curveOutro;

	private float curveLerp;

	internal int value;

	protected override void Start()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		shakeXAmount = 0.005f;
		shakeYAmount = 0.005f;
		timer = 3f;
		text = ((Component)this).GetComponent<TextMeshProUGUI>();
		textColor = ((Graphic)text).color;
		((Graphic)text).color = Color.white;
		((TMP_Text)text).text = "-$" + SemiFunc.DollarGetString(value);
		scale = ((Component)this).transform.localScale;
		if (value < 1000)
		{
			scale *= 0.75f;
			((Component)this).transform.localScale = scale;
		}
	}

	protected override void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (((Graphic)text).color != textColor)
		{
			flashTimer -= Time.deltaTime;
			if (flashTimer <= 0f && ((Graphic)text).color != textColor)
			{
				((Graphic)text).color = Color.Lerp(((Graphic)text).color, textColor, 20f * Time.deltaTime);
				shakeX = Mathf.Lerp(shakeX, 0f, 20f * Time.deltaTime);
				shakeY = Mathf.Lerp(shakeY, 0f, 20f * Time.deltaTime);
			}
			else
			{
				if (shakeTimerX <= 0f)
				{
					shakeXTarget = Random.Range(0f - shakeXAmount, shakeXAmount);
					shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					shakeTimerX -= Time.deltaTime;
					shakeX = Mathf.Lerp(shakeX, shakeXTarget, 50f * Time.deltaTime);
				}
				if (shakeTimerX <= 0f)
				{
					shakeYTarget = Random.Range(0f - shakeYAmount, shakeYAmount);
					shakeTimerX = Random.Range(0.008f, 0.015f);
				}
				else
				{
					shakeTimerX -= Time.deltaTime;
					shakeY = Mathf.Lerp(shakeY, shakeYTarget, 50f * Time.deltaTime);
				}
			}
		}
		floatY += 0.02f * Time.deltaTime;
		positionOffset = new Vector3(shakeX, shakeY + floatY, 0f);
		timer -= Time.deltaTime;
		if (timer > 0f)
		{
			curveLerp += 10f * Time.deltaTime;
			curveLerp = Mathf.Clamp01(curveLerp);
			((Component)this).transform.localScale = scale * curveIntro.Evaluate(curveLerp);
			return;
		}
		curveLerp -= 5f * Time.deltaTime;
		curveLerp = Mathf.Clamp01(curveLerp);
		((Component)this).transform.localScale = scale * curveOutro.Evaluate(curveLerp);
		if (curveLerp <= 0f)
		{
			WorldSpaceUIParent.instance.valueLostList.Remove(this);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
