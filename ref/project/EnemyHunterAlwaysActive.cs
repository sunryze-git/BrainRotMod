using UnityEngine;

public class EnemyHunterAlwaysActive : MonoBehaviour
{
	private bool shootEffectActive;

	public PropLight shootLight;

	private float shootLightIntensity;

	private bool hitEffectActive;

	public PropLight hitLight;

	private float hitLightIntensity;

	private void Start()
	{
		shootLightIntensity = shootLight.lightComponent.intensity;
		hitLightIntensity = hitLight.lightComponent.intensity;
	}

	private void Update()
	{
		if (shootEffectActive)
		{
			Light lightComponent = shootLight.lightComponent;
			lightComponent.intensity -= Time.deltaTime * 20f;
			shootLight.originalIntensity = shootLightIntensity;
			if (shootLight.lightComponent.intensity <= 0f)
			{
				((Behaviour)shootLight.lightComponent).enabled = false;
				shootEffectActive = false;
			}
		}
		if (hitEffectActive)
		{
			Light lightComponent2 = hitLight.lightComponent;
			lightComponent2.intensity -= Time.deltaTime * 20f;
			hitLight.originalIntensity = hitLightIntensity;
			if (hitLight.lightComponent.intensity <= 0f)
			{
				((Behaviour)hitLight.lightComponent).enabled = false;
				hitEffectActive = false;
			}
		}
	}

	public void Trigger()
	{
		shootEffectActive = true;
		hitEffectActive = true;
		((Behaviour)shootLight.lightComponent).enabled = true;
		shootLight.lightComponent.intensity = shootLightIntensity;
		shootLight.originalIntensity = shootLightIntensity;
		((Behaviour)hitLight.lightComponent).enabled = true;
		hitLight.lightComponent.intensity = hitLightIntensity;
		hitLight.originalIntensity = hitLightIntensity;
	}
}
