using UnityEngine;

public class PropLight : MonoBehaviour
{
	public bool levelLight = true;

	internal bool turnedOff;

	[Range(0f, 2f)]
	public float lightRangeMultiplier = 1f;

	internal Light lightComponent;

	internal float originalIntensity;

	internal Behaviour halo;

	internal bool hasHalo;

	private void Awake()
	{
		lightComponent = ((Component)this).GetComponent<Light>();
		originalIntensity = lightComponent.intensity;
		ref Behaviour reference = ref halo;
		Component component = ((Component)this).GetComponent("Halo");
		reference = (Behaviour)(object)((component is Behaviour) ? component : null);
		if (Object.op_Implicit((Object)(object)halo))
		{
			hasHalo = true;
		}
	}

	private void Start()
	{
		if (LevelGenerator.Instance.Generated)
		{
			SemiFunc.LightAdd(this);
		}
	}
}
