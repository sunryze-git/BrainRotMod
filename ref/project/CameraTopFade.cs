using System.Collections;
using UnityEngine;

public class CameraTopFade : MonoBehaviour
{
	public static CameraTopFade Instance;

	public Transform MeshTransform;

	public MeshRenderer Mesh;

	[Space]
	public AnimationCurve Curve;

	public float Speed = 1f;

	private bool Fading;

	private bool Active;

	private float ActiveTimer;

	private float Amount;

	private float AmountCurrent;

	private float AmountStart;

	private float AmountEnd;

	private float LerpAmount;

	private void Awake()
	{
		Instance = this;
	}

	public void Set(float amount, float time)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ActiveTimer = time;
		if (!Active)
		{
			Active = true;
			AmountStart = AmountCurrent;
			AmountEnd = amount;
			LerpAmount = 0f;
		}
		if (!Fading)
		{
			Color color = ((Renderer)Mesh).material.color;
			color.a = 0f;
			((Renderer)Mesh).material.color = color;
			((Component)MeshTransform).gameObject.SetActive(true);
			Fading = true;
			((MonoBehaviour)this).StartCoroutine(Fade());
		}
	}

	private IEnumerator Fade()
	{
		while (Fading)
		{
			if (Active)
			{
				AmountCurrent = Mathf.Lerp(AmountStart, AmountEnd, Curve.Evaluate(LerpAmount));
				LerpAmount += Speed * Time.deltaTime;
				LerpAmount = Mathf.Clamp01(LerpAmount);
				if (ActiveTimer > 0f)
				{
					ActiveTimer -= Time.deltaTime;
				}
				else
				{
					AmountStart = AmountCurrent;
					AmountEnd = 0f;
					Active = false;
					LerpAmount = 0f;
				}
			}
			else
			{
				AmountCurrent = Mathf.Lerp(AmountStart, AmountEnd, Curve.Evaluate(LerpAmount));
				LerpAmount += Speed * Time.deltaTime;
				LerpAmount = Mathf.Clamp01(LerpAmount);
				if (LerpAmount >= 1f)
				{
					Fading = false;
					((Component)MeshTransform).gameObject.SetActive(false);
				}
			}
			Color color = ((Renderer)Mesh).material.color;
			color.a = AmountCurrent;
			((Renderer)Mesh).material.color = color;
			yield return null;
		}
	}
}
