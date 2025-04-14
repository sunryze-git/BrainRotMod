using UnityEngine;

public class ArenaLight : MonoBehaviour
{
	internal MeshRenderer meshRenderer;

	internal Light arenaLight;

	private float lightIntensity = 0.5f;

	private void Start()
	{
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		arenaLight = ((Component)this).GetComponentInChildren<Light>();
		lightIntensity = arenaLight.intensity;
	}

	private void Update()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)arenaLight).enabled)
		{
			if (arenaLight.intensity > lightIntensity)
			{
				arenaLight.intensity = Mathf.Lerp(arenaLight.intensity, lightIntensity, Time.deltaTime * 2f);
				Color val = default(Color);
				((Color)(ref val))._002Ector(0.3f, 0f, 0f);
				((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.Lerp(((Renderer)meshRenderer).material.GetColor("_EmissionColor"), val, Time.deltaTime * 2f));
			}
			else
			{
				arenaLight.intensity = lightIntensity;
			}
		}
	}

	public void TurnOnArenaWarningLight()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.red);
		((Behaviour)arenaLight).enabled = true;
	}

	public void PulsateLight()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		arenaLight.intensity = lightIntensity * 2f;
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.red);
	}
}
