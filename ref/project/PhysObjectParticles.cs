using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysObjectParticles : MonoBehaviour
{
	public Gradient gradient;

	public ParticleSystem particleSystemBits;

	public ParticleSystem particleSystemBitsSmall;

	public ParticleSystem particleSystemSmoke;

	private Particle[] particles;

	private bool particlesSpawned;

	private PhysGrabObject physGrabObject;

	internal float multiplier = 1f;

	public List<Transform> colliderTransforms = new List<Transform>();

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	public void DestroyParticles()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.RunIsTutorial())
		{
			((MonoBehaviour)this).StartCoroutine(DestroyParticlesAfterTime(4f));
		}
		foreach (Transform colliderTransform2 in colliderTransforms)
		{
			Vector3 val = colliderTransform2.localScale;
			Quaternion rotation = colliderTransform2.rotation;
			Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
			Transform colliderTransform = colliderTransform2;
			int num = (int)(val.x * 100f * (val.y * 100f) * (val.z * 100f) / 1000f);
			num = (int)((float)Mathf.Clamp(num, 10, 150) * multiplier);
			float num2 = val.x * 100f * (val.y * 100f) * (val.z * 100f) / 30000f;
			if (Object.op_Implicit((Object)(object)((Component)colliderTransform2).GetComponent<SphereCollider>()))
			{
				num2 *= 0.55f;
				val *= 0.4f;
			}
			num2 = Mathf.Clamp(num2, 0.3f, 2f) * multiplier;
			SpawnParticles(num, num2, val, eulerAngles, colliderTransform);
		}
	}

	private IEnumerator DestroyParticlesAfterTime(float time)
	{
		yield return (object)new WaitForSeconds(time);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void ImpactSmoke(int amount, Vector3 position, float size)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		size = Mathf.Clamp(size, 0.7f, 1.5f);
		Vector3 localPosition = ((Component)this).transform.InverseTransformPoint(position);
		((Component)particleSystemSmoke).transform.localPosition = localPosition;
		MainModule main = particleSystemSmoke.main;
		ShapeModule shape = particleSystemSmoke.shape;
		float startSizeMultiplier = ((MainModule)(ref main)).startSizeMultiplier;
		((ShapeModule)(ref shape)).scale = new Vector3(0.2f, 0.2f, 0.2f);
		float num = Mathf.Clamp(size / 4f, 0f, 2f);
		((MainModule)(ref main)).startSpeed = new MinMaxCurve(0f, num);
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier * size;
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier * size;
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier * size;
		particleSystemSmoke.Emit(amount);
		((MainModule)(ref main)).startSizeXMultiplier = startSizeMultiplier;
		((MainModule)(ref main)).startSizeYMultiplier = startSizeMultiplier;
		((MainModule)(ref main)).startSizeZMultiplier = startSizeMultiplier;
	}

	private void SpawnParticles(int bitCount, float size, Vector3 colliderScale, Vector3 colliderRotation, Transform colliderTransform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		_ = Vector3.left * 5f;
		((Component)particleSystemBits).transform.position = colliderTransform.position;
		((Component)particleSystemBitsSmall).transform.position = colliderTransform.position;
		((Component)particleSystemSmoke).transform.position = colliderTransform.position;
		ShapeModule shape = particleSystemBits.shape;
		MainModule main = particleSystemBits.main;
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier * size;
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier * size;
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier * size;
		((ShapeModule)(ref shape)).scale = colliderScale;
		((ShapeModule)(ref shape)).rotation = colliderRotation;
		main = particleSystemBitsSmall.main;
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier * size;
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier * size;
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier * size;
		shape = particleSystemBitsSmall.shape;
		((ShapeModule)(ref shape)).scale = colliderScale;
		((ShapeModule)(ref shape)).rotation = colliderRotation;
		main = particleSystemSmoke.main;
		shape = particleSystemSmoke.shape;
		float startSizeMultiplier = ((MainModule)(ref main)).startSizeMultiplier;
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier * Mathf.Clamp(size, 0.5f, 1.5f);
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier * Mathf.Clamp(size, 0.5f, 1.5f);
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier * Mathf.Clamp(size, 0.5f, 1.5f);
		float num = Mathf.Clamp(size, 0.5f, 2f);
		((MainModule)(ref main)).startSpeed = new MinMaxCurve(0f, num);
		((ShapeModule)(ref shape)).scale = colliderScale;
		((ShapeModule)(ref shape)).rotation = colliderRotation;
		particleSystemBits.Emit(bitCount);
		particleSystemBitsSmall.Emit(bitCount / 3);
		particleSystemSmoke.Emit(bitCount);
		particlesSpawned = true;
		main = particleSystemBits.main;
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier / size;
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier / size;
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier / size;
		main = particleSystemBitsSmall.main;
		((MainModule)(ref main)).startSizeXMultiplier = ((MainModule)(ref main)).startSizeXMultiplier / size;
		((MainModule)(ref main)).startSizeYMultiplier = ((MainModule)(ref main)).startSizeYMultiplier / size;
		((MainModule)(ref main)).startSizeZMultiplier = ((MainModule)(ref main)).startSizeZMultiplier / size;
		main = particleSystemSmoke.main;
		((MainModule)(ref main)).startSizeXMultiplier = startSizeMultiplier;
		((MainModule)(ref main)).startSizeYMultiplier = startSizeMultiplier;
		((MainModule)(ref main)).startSizeZMultiplier = startSizeMultiplier;
	}

	private void LateUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (particlesSpawned)
		{
			MainModule main = particleSystemBits.main;
			int maxParticles = ((MainModule)(ref main)).maxParticles;
			if (particles == null || particles.Length < maxParticles)
			{
				particles = (Particle[])(object)new Particle[maxParticles];
			}
			int num = particleSystemBits.GetParticles(particles);
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)i / (float)num;
				Color val = gradient.Evaluate(num2);
				((Particle)(ref particles[i])).startColor = Color32.op_Implicit(val);
			}
			particleSystemBits.SetParticles(particles, num);
			main = particleSystemBitsSmall.main;
			maxParticles = ((MainModule)(ref main)).maxParticles;
			if (particles == null || particles.Length < maxParticles)
			{
				particles = (Particle[])(object)new Particle[maxParticles];
			}
			num = particleSystemBitsSmall.GetParticles(particles);
			for (int j = 0; j < num; j++)
			{
				float num3 = (float)j / (float)num;
				Color val2 = gradient.Evaluate(num3);
				((Particle)(ref particles[j])).startColor = Color32.op_Implicit(val2);
			}
			particleSystemBitsSmall.SetParticles(particles, num);
			particlesSpawned = false;
		}
	}
}
