using UnityEngine;

public class PlayerReviveEffects : MonoBehaviour
{
	private bool triggered;

	public PlayerAvatar PlayerAvatar;

	public Transform enableTransform;

	[Space]
	public Light reviveLight;

	private float reviveLightIntensityDefault;

	public ParticleSystem impactParticle;

	public ParticleSystem swirlParticle;

	[Space]
	public Sound reviveSound;

	private void Start()
	{
		reviveLightIntensityDefault = reviveLight.intensity;
	}

	private void Update()
	{
		if (triggered)
		{
			reviveLight.intensity = Mathf.Lerp(reviveLight.intensity, 0f, Time.deltaTime * 1f);
			if (impactParticle.isStopped && swirlParticle.isStopped && reviveLight.intensity < 0.01f)
			{
				triggered = false;
				reviveLight.intensity = reviveLightIntensityDefault;
				((Component)enableTransform).gameObject.SetActive(false);
			}
		}
	}

	public void Trigger()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = PlayerAvatar.playerDeathHead.physGrabObject.centerPoint;
		if (SemiFunc.RunIsTutorial())
		{
			((Component)this).transform.position = ((Component)PlayerAvatar.instance).transform.position;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		triggered = true;
		((Component)enableTransform).gameObject.SetActive(true);
		reviveSound.Play(((Component)this).transform.position);
	}
}
