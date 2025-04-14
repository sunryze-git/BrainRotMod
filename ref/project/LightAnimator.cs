using UnityEngine;

public class LightAnimator : MonoBehaviour
{
	private Light lightComponent;

	private float lightIntensityInit;

	private float lightIntensity;

	public bool lightActive;

	public float introSpeed;

	public float outroSpeed;

	private void Start()
	{
		lightComponent = ((Component)this).GetComponent<Light>();
		lightIntensityInit = lightComponent.intensity;
		if (lightActive)
		{
			lightIntensity = lightIntensityInit;
			lightComponent.intensity = lightIntensity;
			((Behaviour)lightComponent).enabled = true;
		}
		else
		{
			lightIntensity = 0f;
			lightComponent.intensity = lightIntensity;
			((Behaviour)lightComponent).enabled = false;
		}
	}

	private void Update()
	{
		if (lightActive)
		{
			if (!((Behaviour)lightComponent).enabled)
			{
				((Behaviour)lightComponent).enabled = true;
			}
			lightIntensity = Mathf.Clamp(lightIntensity + introSpeed * Time.deltaTime, 0f, lightIntensityInit);
			lightComponent.intensity = lightIntensity;
		}
		else if (((Behaviour)lightComponent).enabled)
		{
			lightIntensity = Mathf.Clamp(lightIntensity - outroSpeed * Time.deltaTime, 0f, lightIntensityInit);
			lightComponent.intensity = lightIntensity;
			if (lightIntensity <= 0f)
			{
				((Behaviour)lightComponent).enabled = false;
			}
		}
	}
}
