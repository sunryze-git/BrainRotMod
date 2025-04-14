using UnityEngine;

public class LineBetweenTwoPoints : MonoBehaviour
{
	public GameObject lineBetweenTwoPoints;

	public bool hasSpheres = true;

	private GameObject line;

	private GameObject sphere1;

	private GameObject sphere2;

	public Color lineColor = Color.white;

	private LineRenderer lineRenderer;

	private Vector3 pointA;

	private Vector3 pointB;

	private float lineRenderLifetime;

	private void Start()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		line = ((Component)lineBetweenTwoPoints.transform.Find("Line")).gameObject;
		sphere1 = ((Component)lineBetweenTwoPoints.transform.Find("Sphere1")).gameObject;
		sphere2 = ((Component)lineBetweenTwoPoints.transform.Find("Sphere2")).gameObject;
		ItemDrone component = ((Component)this).GetComponent<ItemDrone>();
		if (Object.op_Implicit((Object)(object)component))
		{
			lineColor = component.beamColor;
		}
		lineRenderer = line.GetComponent<LineRenderer>();
		lineRenderer.positionCount = 2;
		((Renderer)lineRenderer).enabled = false;
		if (Object.op_Implicit((Object)(object)sphere1))
		{
			((Renderer)sphere1.GetComponent<MeshRenderer>()).enabled = false;
		}
		if (Object.op_Implicit((Object)(object)sphere2))
		{
			((Renderer)sphere2.GetComponent<MeshRenderer>()).enabled = false;
		}
		((Renderer)lineRenderer).material.SetColor("_EmissionColor", lineColor);
		((Renderer)lineRenderer).material.SetColor("_Color", lineColor);
		((Renderer)lineRenderer).material.SetColor("_AlbedoColor", lineColor);
		if (Object.op_Implicit((Object)(object)sphere1))
		{
			((Renderer)sphere1.GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", lineColor);
			((Renderer)sphere1.GetComponent<MeshRenderer>()).material.SetColor("_Color", lineColor);
			((Renderer)sphere1.GetComponent<MeshRenderer>()).material.SetColor("_AlbedoColor", lineColor);
		}
		if (Object.op_Implicit((Object)(object)sphere2))
		{
			((Renderer)sphere2.GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", lineColor);
			((Renderer)sphere2.GetComponent<MeshRenderer>()).material.SetColor("_Color", lineColor);
			((Renderer)sphere2.GetComponent<MeshRenderer>()).material.SetColor("_AlbedoColor", lineColor);
		}
		if (!hasSpheres)
		{
			Object.Destroy((Object)(object)sphere1);
			Object.Destroy((Object)(object)sphere2);
		}
	}

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		float num = 2f;
		((Renderer)lineRenderer).material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		if (Object.op_Implicit((Object)(object)sphere1))
		{
			((Renderer)sphere1.GetComponent<MeshRenderer>()).material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		}
		if (Object.op_Implicit((Object)(object)sphere2))
		{
			((Renderer)sphere2.GetComponent<MeshRenderer>()).material.mainTextureOffset = new Vector2(Time.time * num, 0f);
		}
		float num2 = 0.05f;
		float num3 = 0.025f;
		lineRenderer.startWidth = num2 + Mathf.Sin(Time.time * 5f) * num3;
		lineRenderer.endWidth = num2 + Mathf.Cos(Time.time * 5f) * num3;
		if (!((Renderer)lineRenderer).enabled)
		{
			return;
		}
		if (lineRenderLifetime <= 0f)
		{
			((Renderer)lineRenderer).enabled = false;
			((Component)lineRenderer).gameObject.SetActive(false);
			if (Object.op_Implicit((Object)(object)sphere1))
			{
				((Renderer)sphere1.GetComponent<MeshRenderer>()).enabled = false;
			}
			if (Object.op_Implicit((Object)(object)sphere2))
			{
				((Renderer)sphere2.GetComponent<MeshRenderer>()).enabled = false;
			}
		}
		else
		{
			lineRenderLifetime -= Time.deltaTime;
		}
	}

	public void DrawLine(Vector3 point1, Vector3 point2)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		((Component)lineRenderer).gameObject.SetActive(true);
		((Renderer)lineRenderer).enabled = true;
		if (Object.op_Implicit((Object)(object)sphere1))
		{
			((Renderer)sphere1.GetComponent<MeshRenderer>()).enabled = true;
			sphere1.transform.position = point1;
		}
		if (Object.op_Implicit((Object)(object)sphere2))
		{
			((Renderer)sphere2.GetComponent<MeshRenderer>()).enabled = true;
			sphere2.transform.position = point2;
		}
		float num = 2f;
		if (Object.op_Implicit((Object)(object)sphere1))
		{
			sphere1.transform.localScale = new Vector3(lineRenderer.startWidth * num, lineRenderer.startWidth * num, lineRenderer.startWidth * num);
		}
		if (Object.op_Implicit((Object)(object)sphere2))
		{
			sphere2.transform.localScale = new Vector3(lineRenderer.endWidth * num, lineRenderer.endWidth * num, lineRenderer.endWidth * num);
		}
		lineRenderer.SetPosition(0, point1);
		lineRenderer.SetPosition(1, point2);
		lineRenderLifetime = 0.01f;
	}
}
