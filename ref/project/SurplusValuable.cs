using UnityEngine;

public class SurplusValuable : MonoBehaviour
{
	private PhysGrabObjectImpactDetector impactDetector;

	private float indestructibleTimer = 3f;

	public float coinMultiplier = 1f;

	public ParticleSystem coinParticles;

	public Sound spawnSound;

	private void Start()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		impactDetector = ((Component)this).GetComponentInChildren<PhysGrabObjectImpactDetector>();
		impactDetector.indestructibleSpawnTimer = 0.1f;
		coinParticles.Emit((int)(30f * coinMultiplier));
		spawnSound.Play(((Component)this).transform.position);
	}

	private void Update()
	{
		if (indestructibleTimer > 0f)
		{
			indestructibleTimer -= Time.deltaTime;
			if (indestructibleTimer <= 0f)
			{
				impactDetector.destroyDisable = false;
			}
		}
	}

	public void BreakLight()
	{
		coinParticles.Emit((int)(3f * coinMultiplier));
	}

	public void BreakMedium()
	{
		coinParticles.Emit((int)(5f * coinMultiplier));
	}

	public void BreakHeavy()
	{
		coinParticles.Emit((int)(10f * coinMultiplier));
	}

	public void DestroyImpulse()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		coinParticles.Emit((int)(20f * coinMultiplier));
		((Component)coinParticles).transform.parent = null;
		MainModule main = coinParticles.main;
		((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
	}
}
