using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PhysGrabBeam : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	public Transform PhysGrabPointOrigin;

	public Transform PhysGrabPointOriginClient;

	public Transform PhysGrabPoint;

	public Transform PhysGrabPointPuller;

	public Material greenScreenMaterial;

	private Material originalMaterial;

	[HideInInspector]
	public Vector3 physGrabPointPullerSmoothPosition;

	public float CurveStrength = 1f;

	public int CurveResolution = 20;

	[Header("Texture Scrolling")]
	public Vector2 scrollSpeed = new Vector2(5f, 0f);

	[HideInInspector]
	public Vector2 originalScrollSpeed;

	private LineRenderer lineRenderer;

	[HideInInspector]
	public Material lineMaterial;

	private void Start()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!playerAvatar.isLocal)
		{
			PhysGrabPointOrigin = PhysGrabPointOriginClient;
		}
		originalScrollSpeed = scrollSpeed;
		lineRenderer = ((Component)this).GetComponent<LineRenderer>();
		originalMaterial = ((Renderer)lineRenderer).material;
		lineMaterial = ((Renderer)lineRenderer).material;
	}

	private void LateUpdate()
	{
		DrawCurve();
		ScrollTexture();
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		physGrabPointPullerSmoothPosition = PhysGrabPointPuller.position;
		if (Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
		{
			lineMaterial = greenScreenMaterial;
			((Renderer)((Component)this).GetComponent<LineRenderer>()).material = greenScreenMaterial;
		}
	}

	private void OnDisable()
	{
		lineMaterial = originalMaterial;
		if (Object.op_Implicit((Object)(object)lineRenderer))
		{
			((Renderer)lineRenderer).material = originalMaterial;
		}
	}

	private void DrawCurve()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)PhysGrabPointPuller))
		{
			Vector3[] array = (Vector3[])(object)new Vector3[CurveResolution];
			Vector3 position = PhysGrabPointPuller.position;
			_ = Vector3.zero;
			physGrabPointPullerSmoothPosition = Vector3.Lerp(physGrabPointPullerSmoothPosition, position, Time.deltaTime * 10f);
			Vector3 p = physGrabPointPullerSmoothPosition * CurveStrength;
			for (int i = 0; i < CurveResolution; i++)
			{
				float t = (float)i / ((float)CurveResolution - 1f);
				array[i] = CalculateBezierPoint(t, PhysGrabPointOrigin.position, p, PhysGrabPoint.position);
			}
			lineRenderer.positionCount = CurveResolution;
			lineRenderer.SetPositions(array);
		}
	}

	private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Pow(1f - t, 2f) * p0 + 2f * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
	}

	private void ScrollTexture()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)lineMaterial))
		{
			if (playerAvatar.physGrabber.colorState == 1)
			{
				lineMaterial.mainTextureScale = new Vector2(-1f, 1f);
			}
			else
			{
				lineMaterial.mainTextureScale = new Vector2(1f, 1f);
			}
			Vector2 mainTextureOffset = Time.time * scrollSpeed;
			lineMaterial.mainTextureOffset = mainTextureOffset;
		}
	}
}
