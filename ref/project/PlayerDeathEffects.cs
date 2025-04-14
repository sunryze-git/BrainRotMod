using UnityEngine;

public class PlayerDeathEffects : MonoBehaviour
{
	private bool triggered;

	public PlayerAvatarVisuals playerAvatarVisuals;

	[Space]
	public Transform followTransform;

	public Transform enableTransform;

	[Space]
	public Light deathLight;

	private float deathLightIntensityDefault;

	public ParticleSystem smokeParticles;

	public ParticleSystem fireParticles;

	public ParticleSystem bitWeakParticles;

	public ParticleSystem bitStrongParticles;

	public HurtCollider hurtCollider;

	private float hurtColliderTime = 0.5f;

	private float hurtColliderTimer;

	[Space]
	public Sound deathSound;

	private void Start()
	{
		deathLightIntensityDefault = deathLight.intensity;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = followTransform.position;
		((Component)this).transform.rotation = followTransform.rotation;
		if (!triggered)
		{
			return;
		}
		deathLight.intensity = Mathf.Lerp(deathLight.intensity, 0f, Time.deltaTime * 1f);
		if (smokeParticles.isStopped && bitWeakParticles.isStopped && bitStrongParticles.isStopped && deathLight.intensity < 0.01f)
		{
			((Component)this).gameObject.SetActive(false);
		}
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				((Component)hurtCollider).gameObject.SetActive(false);
			}
		}
	}

	public void Trigger()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		MainModule main = bitWeakParticles.main;
		((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(playerAvatarVisuals.color);
		MainModule main2 = bitStrongParticles.main;
		((MainModule)(ref main2)).startColor = MinMaxGradient.op_Implicit(playerAvatarVisuals.color);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		triggered = true;
		((Component)enableTransform).gameObject.SetActive(true);
		((Component)hurtCollider).gameObject.SetActive(true);
		hurtColliderTimer = hurtColliderTime;
		deathSound.Play(((Component)this).transform.position);
		((Component)smokeParticles).gameObject.SetActive(true);
		smokeParticles.Play();
		((Component)fireParticles).gameObject.SetActive(true);
		fireParticles.Play();
		((Component)bitWeakParticles).gameObject.SetActive(true);
		bitWeakParticles.Play();
		((Component)bitStrongParticles).gameObject.SetActive(true);
		bitStrongParticles.Play();
	}

	public void Reset()
	{
		((Component)this).gameObject.SetActive(true);
		triggered = false;
		deathLight.intensity = deathLightIntensityDefault;
		((Component)enableTransform).gameObject.SetActive(false);
	}
}
