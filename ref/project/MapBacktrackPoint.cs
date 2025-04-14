using System.Collections;
using UnityEngine;

public class MapBacktrackPoint : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public AnimationCurve curve;

	public float speed;

	private float lerp;

	public bool animating;

	private void Awake()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = Vector3.zero;
	}

	public void Show(bool _sameLayer)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Color color = spriteRenderer.color;
		if (_sameLayer)
		{
			color.a = 1f;
		}
		else
		{
			color.a = 0.2f;
		}
		spriteRenderer.color = color;
		((MonoBehaviour)this).StopCoroutine(Animate());
		((MonoBehaviour)this).StartCoroutine(Animate());
	}

	private IEnumerator Animate()
	{
		animating = true;
		lerp = 0f;
		while (true)
		{
			lerp += Time.deltaTime * speed;
			((Component)this).transform.localScale = Vector3.one * curve.Evaluate(lerp);
			if (!(lerp < 1f))
			{
				break;
			}
			yield return (object)new WaitForSeconds(0.05f);
		}
		animating = false;
	}
}
