using UnityEngine;

public class PhysGrabBeamPoint : MonoBehaviour
{
	public float tileSpeedX = 0.5f;

	public float tileSpeedY = 0.5f;

	public float textureJitterSpeed = 10f;

	public float sphereJitterSpeed = 10f;

	private Vector3 originalScale;

	public Material originalMaterial;

	public Material greenScreenMaterial;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		originalScale = ((Component)this).transform.localScale;
		originalMaterial = ((Component)this).GetComponent<Renderer>().material;
	}

	private void OnEnable()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
		{
			((Component)this).GetComponent<Renderer>().material = greenScreenMaterial;
			{
				foreach (Transform item in ((Component)this).transform)
				{
					((Component)item).GetComponent<Renderer>().material = greenScreenMaterial;
				}
				return;
			}
		}
		((Component)this).GetComponent<Renderer>().material = originalMaterial;
		foreach (Transform item2 in ((Component)this).transform)
		{
			((Component)item2).GetComponent<Renderer>().material = originalMaterial;
		}
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.time * tileSpeedX;
		float num2 = Time.time * tileSpeedY;
		((Component)this).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(num, num2);
		float num3 = Mathf.Sin(Time.time * textureJitterSpeed) * 0.1f;
		((Component)this).GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + num3, 1f + num3);
		foreach (Transform item in ((Component)this).transform)
		{
			((Component)item).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f - num, 0f - num2);
			((Component)item).GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f - num3, 1f - num3);
		}
		float num4 = Mathf.Sin(Time.time * sphereJitterSpeed) * (originalScale.x * 0.3f);
		((Component)this).transform.localScale = (originalScale + new Vector3(num4, num4, num4)) * 0.5f;
	}
}
