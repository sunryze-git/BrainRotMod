using System;
using UnityEngine;

public class FloaterLine : MonoBehaviour
{
	public Transform lineTarget;

	private LineRenderer lineRenderer;

	private PhysGrabObject physGrabObject;

	internal FloaterAttackLogic floaterAttack;

	internal bool outro;

	public Material redMaterial;

	internal bool redMaterialSet;

	private void Start()
	{
		lineRenderer = ((Component)this).GetComponent<LineRenderer>();
		lineRenderer.widthMultiplier = 0f;
		physGrabObject = ((Component)lineTarget).GetComponent<PhysGrabObject>();
	}

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)lineTarget))
		{
			Vector3 position = ((Component)this).transform.position;
			Vector3 val = lineTarget.position;
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				val = physGrabObject.midPoint;
			}
			Vector3[] array = (Vector3[])(object)new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 20f;
				array[i] = Vector3.Lerp(position, val, num) + Vector3.up * Mathf.Sin(num * MathF.PI) * 1f;
				float num2 = 1f - Mathf.Abs(num - 0.5f) * 2f;
				float num3 = 1f;
				if (floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
				{
					num2 *= 3f;
					num3 = 2f;
				}
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
		if (floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop && !redMaterialSet)
		{
			((Renderer)lineRenderer).material = redMaterial;
			redMaterialSet = true;
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
			lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, 0f, Time.deltaTime * 2f);
		}
		else
		{
			lineRenderer.widthMultiplier = 0f;
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
