using UnityEngine;

public class CleanEffectParticleSettings : MonoBehaviour
{
	public ParticleSystem gleamParticles;

	public GameObject cleanEffectParticleRadius;

	public GameObject cleanEffectParticleSize;

	public GameObject cleanEffectParticleAmount;

	private void Start()
	{
		UpdateParticleProperties();
	}

	private void UpdateParticleProperties()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		float num = cleanEffectParticleSize.transform.localScale.x * 10f / 4f;
		float x = cleanEffectParticleRadius.transform.localScale.x;
		float num2 = cleanEffectParticleAmount.transform.localScale.x * 100f / 4f;
		MainModule main = gleamParticles.main;
		float startSizeMultiplier = ((MainModule)(ref main)).startSizeMultiplier;
		((MainModule)(ref main)).startSizeMultiplier = startSizeMultiplier * num;
		ShapeModule shape = gleamParticles.shape;
		((ShapeModule)(ref shape)).radius = x / 2f;
		EmissionModule emission = gleamParticles.emission;
		float rateOverTimeMultiplier = ((EmissionModule)(ref emission)).rateOverTimeMultiplier;
		((EmissionModule)(ref emission)).rateOverTimeMultiplier = rateOverTimeMultiplier * num2;
		Object.Destroy((Object)(object)cleanEffectParticleSize);
		Object.Destroy((Object)(object)cleanEffectParticleRadius);
		Object.Destroy((Object)(object)cleanEffectParticleAmount);
	}
}
