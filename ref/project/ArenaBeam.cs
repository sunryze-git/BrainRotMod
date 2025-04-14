using UnityEngine;

public class ArenaBeam : MonoBehaviour
{
	public Transform lineTarget;

	private LineRenderer lineRenderer;

	private PhysGrabObject physGrabObject;

	internal bool outro;

	private void Start()
	{
		lineRenderer = ((Component)this).GetComponent<LineRenderer>();
		lineRenderer.widthMultiplier = 0f;
	}

	private void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)lineTarget))
		{
			Vector3 position = ((Component)this).transform.position;
			Vector3 position2 = lineTarget.position;
			Vector3[] array = (Vector3[])(object)new Vector3[20];
			for (int i = 0; i < 20; i++)
			{
				float num = (float)i / 19f;
				array[i] = Vector3.Lerp(position, position2, num);
				float num2 = 1f - Mathf.Abs(num - 0.3f) * 2f;
				float num3 = 1f;
				ref Vector3 reference = ref array[i];
				reference += Vector3.right * Mathf.Sin(Time.time * (30f * num3) + (float)i) * 0.05f * num2;
				ref Vector3 reference2 = ref array[i];
				reference2 += Vector3.forward * Mathf.Cos(Time.time * (30f * num3) + (float)i) * 0.05f * num2;
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
			lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, 0f, Time.deltaTime * 2f);
		}
		else
		{
			lineRenderer.widthMultiplier = 0f;
			((Component)((Component)this).transform.parent).gameObject.SetActive(false);
		}
	}
}
