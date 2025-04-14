using TMPro;
using UnityEngine;

public class DirtFinderMapComplete : MonoBehaviour
{
	public SpriteRenderer FlashRenderer;

	public AnimationCurve FlashCurve;

	public float FlashSpeed;

	private float FlashLerp;

	[Space]
	public TextMeshPro TextTop;

	public TextMeshPro TextBot;

	private float TextDilate;

	private float TextDilateWait;

	private float TextDilateIncrease = 1f;

	[Space]
	public float CompleteTime;

	private void Start()
	{
		GameDirector.instance.CameraImpact.Shake(1f, 0.25f);
		GameDirector.instance.CameraShake.Shake(1f, 0.25f);
	}

	private void Update()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (FlashLerp < 1f)
		{
			FlashLerp += FlashSpeed * Time.deltaTime;
			FlashRenderer.color = Color.Lerp(FlashRenderer.color, new Color(255f, 255f, 255f, 0f), FlashCurve.Evaluate(FlashLerp));
			if (FlashLerp >= 1f)
			{
				FlashLerp = 1f;
				((Component)((Component)FlashRenderer).transform).gameObject.SetActive(false);
			}
		}
		TextDilate += 5f * TextDilateIncrease * Time.deltaTime;
		if (TextDilate >= 1f)
		{
			TextDilate = 1f;
			if (TextDilateWait > 0f)
			{
				TextDilateWait -= Time.deltaTime;
			}
			else
			{
				TextDilateWait = 0.5f;
				TextDilateIncrease = -1f;
			}
		}
		else if (TextDilate <= -1f)
		{
			TextDilate = -1f;
			if (TextDilateWait > 0f)
			{
				TextDilateWait -= Time.deltaTime;
			}
			else
			{
				TextDilateWait = 0.5f;
				TextDilateIncrease = 1f;
			}
		}
		((TMP_Text)TextTop).fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, TextDilate);
		((TMP_Text)TextBot).fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, TextDilate);
		CompleteTime -= Time.deltaTime;
		if (CompleteTime <= 0f)
		{
			CompleteTime = 0f;
			((Component)this).gameObject.SetActive(false);
		}
	}
}
