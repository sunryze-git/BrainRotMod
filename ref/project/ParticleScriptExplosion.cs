using UnityEngine;

public class ParticleScriptExplosion : MonoBehaviour
{
	public ExplosionPreset explosionPreset;

	private GameObject explosionPrefab;

	private void Start()
	{
		explosionPrefab = Resources.Load<GameObject>("Effects/Part Prefab Explosion");
	}

	public ParticlePrefabExplosion Spawn(Vector3 position, float size, int damage, int enemyDamage, float forceMulti = 1f, bool onlyParticleEffect = false, bool disableSound = false, float shakeMultiplier = 1f)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (size < 0.25f)
		{
			if (!disableSound)
			{
				explosionPreset.explosionSoundSmall.Play(position);
				explosionPreset.explosionSoundSmallGlobal.Play(position);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(3f * shakeMultiplier, 3f, 6f, ((Component)this).transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(3f * shakeMultiplier, 3f, 6f, ((Component)this).transform.position, 0.5f);
			}
		}
		else if (size < 0.5f)
		{
			if (!disableSound)
			{
				explosionPreset.explosionSoundMedium.Play(position);
				explosionPreset.explosionSoundMediumGlobal.Play(position);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(5f * shakeMultiplier, 4f, 8f, ((Component)this).transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 4f, 8f, ((Component)this).transform.position, 0.5f);
			}
		}
		else
		{
			if (!disableSound)
			{
				explosionPreset.explosionSoundBig.Play(position);
				explosionPreset.explosionSoundBigGlobal.Play(position);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(10f * shakeMultiplier, 6f, 12f, ((Component)this).transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 6f, 12f, ((Component)this).transform.position, 0.5f);
			}
		}
		ParticlePrefabExplosion component = Object.Instantiate<GameObject>(explosionPrefab, position, Quaternion.identity).GetComponent<ParticlePrefabExplosion>();
		component.forceMultiplier = explosionPreset.explosionForceMultiplier * forceMulti;
		component.explosionSize = size;
		component.explosionDamage = damage;
		component.explosionDamageEnemy = enemyDamage;
		component.lightColorOverTime = explosionPreset.lightColor;
		ColorOverLifetimeModule colorOverLifetime = component.particleFire.colorOverLifetime;
		((ColorOverLifetimeModule)(ref colorOverLifetime)).color = MinMaxGradient.op_Implicit(explosionPreset.explosionColors);
		colorOverLifetime = component.particleSmoke.colorOverLifetime;
		((ColorOverLifetimeModule)(ref colorOverLifetime)).color = MinMaxGradient.op_Implicit(explosionPreset.smokeColors);
		component.particleFire.Play();
		component.particleSmoke.Play();
		((Behaviour)component.light).enabled = true;
		return component;
	}
}
