using System;
using UnityEngine;

public class CeilingEyeLine : MonoBehaviour
{
	public Transform followTransform;

	public Transform lineTarget;

	private LineRenderer lineRenderer;

	internal bool outro;

	private void Awake()
	{
		lineRenderer = ((Component)this).GetComponent<LineRenderer>();
		lineRenderer.widthMultiplier = 0f;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = followTransform.position;
		((Component)this).transform.rotation = followTransform.rotation;
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
				ref Vector3 reference = ref array[i];
				reference += Vector3.right * Mathf.Sin(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
				ref Vector3 reference2 = ref array[i];
				reference2 += Vector3.forward * Mathf.Cos(Time.time * (30f * num3) + (float)i) * 0.02f * num2;
			}
			((Renderer)lineRenderer).material.mainTextureOffset = new Vector2(Time.time * 2f, 0f);
			lineRenderer.positionCount = 20;
			lineRenderer.SetPositions(array);
		}
		else
		{
			outro = true;
		}
		if (!outro)
		{
			if (lineRenderer.widthMultiplier < 0.195f)
			{
				lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, 0.2f, Time.deltaTime * 2f);
			}
			else
			{
				lineRenderer.widthMultiplier = 0.2f;
			}
		}
		else if (lineRenderer.widthMultiplier > 0.005f)
		{
			lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, 0f, Time.deltaTime * 5f);
		}
		else
		{
			lineRenderer.widthMultiplier = 0f;
		}
	}
}
