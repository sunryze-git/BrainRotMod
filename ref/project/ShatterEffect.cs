using UnityEngine;

public class ShatterEffect : MonoBehaviour
{
	public ParticleSystem partSystem;

	public ParticleSystem particleSystemSmoke;

	[Range(0f, 100f)]
	public float particleAmountMultiplier = 50f;

	public Gradient particleColors;

	public Transform ParticleEmissionBox;

	[Space]
	public Sound ShatterSound;

	private MainModule mainModule;

	private EmissionModule emissionModule;

	private ShapeModule shapeModule;

	private ShapeModule shapeModuleSmoke;

	private MainModule mainModuleSmoke;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		mainModule = partSystem.main;
		emissionModule = partSystem.emission;
		shapeModule = partSystem.shape;
		mainModuleSmoke = particleSystemSmoke.main;
		shapeModuleSmoke = particleSystemSmoke.shape;
		SetupParticleSystem();
	}

	private void SetupParticleSystem()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ref EmissionModule reference = ref emissionModule;
		EmissionModule emission = partSystem.emission;
		((EmissionModule)(ref reference)).rateOverTimeMultiplier = ((EmissionModule)(ref emission)).rateOverTimeMultiplier * (particleAmountMultiplier / 100f);
		((ShapeModule)(ref shapeModule)).scale = ParticleEmissionBox.localScale;
		((ShapeModule)(ref shapeModuleSmoke)).scale = ParticleEmissionBox.localScale;
	}

	public void SpawnParticles(Vector3 direction)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		((Component)partSystem).transform.rotation = Quaternion.LookRotation(direction);
		((Component)particleSystemSmoke).transform.rotation = Quaternion.LookRotation(direction);
		((Component)partSystem).transform.position = ParticleEmissionBox.position;
		((ShapeModule)(ref shapeModule)).scale = ParticleEmissionBox.localScale;
		((MainModule)(ref mainModule)).startColor = MinMaxGradient.op_Implicit(particleColors);
		partSystem.Play();
		((Component)partSystem).transform.SetParent((Transform)null);
		particleSystemSmoke.Play();
		((Component)particleSystemSmoke).transform.SetParent((Transform)null);
		ShatterSound.Play(((Component)this).transform.position);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
