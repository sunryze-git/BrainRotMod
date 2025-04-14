using System;
using UnityEngine;

public class TruckHealerLine : MonoBehaviour
{
	public Transform lineTarget;

	private LineRenderer lineRenderer;

	public AnimationCurve wobbleCurve;

	public AnimationCurve widthCurve;

	private float curveEval;

	internal bool outro;

	private void Awake()
	{
		lineRenderer = ((Component)this).GetComponent<LineRenderer>();
		lineRenderer.widthMultiplier = 0f;
	}

	private void Update()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)lineTarget))
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else if (curveEval < 1f)
		{
			curveEval += Time.deltaTime * 2.5f;
			lineRenderer.widthMultiplier = widthCurve.Evaluate(curveEval);
			if (Object.op_Implicit((Object)(object)lineTarget))
			{
				Vector3 position = ((Component)this).transform.position;
				Vector3 position2 = lineTarget.position;
				Vector3[] array = (Vector3[])(object)new Vector3[20];
				for (int i = 0; i < 20; i++)
				{
					float num = (float)i / 19f;
					array[i] = Vector3.Lerp(position, position2, num) - Vector3.up * Mathf.Sin(num * MathF.PI) * 0.5f;
					float num2 = 1f - Mathf.Abs(num - 0.5f) * 2f;
					float num3 = 1f;
					float num4 = wobbleCurve.Evaluate(num) * 2f;
					ref Vector3 reference = ref array[i];
					reference += Vector3.right * Mathf.Sin(Time.time * (30f * num3) + (float)i) * 0.02f * num2 * num4;
					ref Vector3 reference2 = ref array[i];
					reference2 += Vector3.forward * Mathf.Cos(Time.time * (30f * num3) + (float)i) * 0.02f * num2 * num4;
				}
				((Renderer)lineRenderer).material.mainTextureOffset = new Vector2((0f - Time.time) * 2f, 0f);
				lineRenderer.positionCount = 20;
				lineRenderer.SetPositions(array);
			}
			else
			{
				outro = true;
			}
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
