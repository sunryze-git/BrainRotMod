using System.Collections.Generic;
using UnityEngine;

public class ItemLight : MonoBehaviour
{
	public bool alwaysActive;

	public Light itemLight;

	private float lightIntensityOriginal;

	private float lightRangeOriginal;

	private bool showLight = true;

	private PhysGrabObject physGrabObject;

	private bool culledLight;

	public AnimationCurve lightIntensityCurve;

	private float animationCurveEval;

	public List<MeshRenderer> meshRenderers;

	private float fresnelScaleOriginal;

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponentInParent<PhysGrabObject>();
		lightIntensityOriginal = itemLight.intensity;
		lightRangeOriginal = itemLight.range;
		itemLight.intensity = 0f;
		itemLight.range = 0f;
		((Behaviour)itemLight).enabled = false;
		if (meshRenderers.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				if (Object.op_Implicit((Object)(object)meshRenderer) && ((Component)meshRenderer).gameObject.activeSelf && Object.op_Implicit((Object)(object)meshRenderer) && ((Component)meshRenderer).gameObject.activeSelf)
				{
					Material material = ((Renderer)meshRenderer).material;
					fresnelScaleOriginal = material.GetFloat("_FresnelScale");
					break;
				}
			}
		}
		if (alwaysActive)
		{
			((Behaviour)itemLight).enabled = true;
			showLight = true;
			itemLight.intensity = lightIntensityOriginal;
			itemLight.range = lightRangeOriginal;
		}
	}

	private void SetAllFresnel(float _value)
	{
		if (meshRenderers.Count <= 0)
		{
			return;
		}
		foreach (MeshRenderer meshRenderer in meshRenderers)
		{
			if (Object.op_Implicit((Object)(object)meshRenderer) && ((Component)meshRenderer).gameObject.activeSelf)
			{
				((Renderer)meshRenderer).material.SetFloat("_FresnelScale", _value);
			}
		}
	}

	private void Update()
	{
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		if (showLight)
		{
			if (!((Behaviour)itemLight).enabled)
			{
				itemLight.intensity = 0f;
				itemLight.range = 0f;
				animationCurveEval = 0f;
				((Behaviour)itemLight).enabled = true;
			}
			if (itemLight.intensity < lightIntensityOriginal - 0.01f)
			{
				animationCurveEval += Time.deltaTime * 0.05f;
				float num = lightIntensityCurve.Evaluate(animationCurveEval);
				if (meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer in meshRenderers)
					{
						if (Object.op_Implicit((Object)(object)meshRenderer) && ((Component)meshRenderer).gameObject.activeSelf)
						{
							Material material = ((Renderer)meshRenderer).material;
							float @float = material.GetFloat("_FresnelScale");
							material.SetFloat("_FresnelScale", Mathf.Lerp(@float, fresnelScaleOriginal, num));
						}
					}
				}
				itemLight.intensity = Mathf.Lerp(itemLight.intensity, lightIntensityOriginal, num);
				itemLight.range = Mathf.Lerp(itemLight.range, lightRangeOriginal, num);
			}
		}
		else if (((Behaviour)itemLight).enabled)
		{
			animationCurveEval += Time.deltaTime * 1f;
			float num2 = lightIntensityCurve.Evaluate(animationCurveEval);
			itemLight.intensity = Mathf.Lerp(itemLight.intensity, 0f, num2);
			itemLight.range = Mathf.Lerp(itemLight.range, 0f, num2);
			if (meshRenderers.Count > 0)
			{
				foreach (MeshRenderer meshRenderer2 in meshRenderers)
				{
					if (Object.op_Implicit((Object)(object)meshRenderer2) && ((Component)meshRenderer2).gameObject.activeSelf)
					{
						Material material2 = ((Renderer)meshRenderer2).material;
						float float2 = material2.GetFloat("_FresnelScale");
						material2.SetFloat("_FresnelScale", Mathf.Lerp(float2, 0f, num2));
					}
				}
			}
			if (itemLight.intensity < 0.01f)
			{
				animationCurveEval = 0f;
				itemLight.intensity = 0f;
				itemLight.range = 0f;
				if (meshRenderers.Count > 0)
				{
					foreach (MeshRenderer meshRenderer3 in meshRenderers)
					{
						if (Object.op_Implicit((Object)(object)meshRenderer3) && ((Component)meshRenderer3).gameObject.activeSelf)
						{
							((Renderer)meshRenderer3).material.SetFloat("_FresnelScale", 0f);
						}
					}
				}
				((Behaviour)itemLight).enabled = false;
			}
		}
		if (!SemiFunc.FPSImpulse1())
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)SemiFunc.PlayerGetNearestTransformWithinRange(16f, ((Component)this).transform.position)))
		{
			culledLight = false;
		}
		else
		{
			culledLight = true;
		}
		if (!alwaysActive)
		{
			if (!culledLight)
			{
				if (!physGrabObject.grabbed)
				{
					if (!showLight)
					{
						((Behaviour)itemLight).enabled = true;
						showLight = true;
					}
				}
				else
				{
					showLight = false;
				}
			}
			else
			{
				showLight = false;
			}
		}
		else if (culledLight)
		{
			showLight = false;
		}
		else
		{
			showLight = true;
		}
	}
}
