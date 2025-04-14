using UnityEngine;

public class ItemShockwave : MonoBehaviour
{
	public MeshRenderer meshRenderer;

	private float startScale = 1f;

	private bool finalScale;

	private Light lightShockwave;

	public ParticleSystem particleSystemWave;

	public ParticleSystem particleSystemSparks;

	public ParticleSystem particleSystemLightning;

	private HurtCollider hurtCollider;

	public Sound soundExplosion;

	public Sound soundExplosionGlobal;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		startScale = ((Component)this).transform.localScale.x;
		lightShockwave = ((Component)this).GetComponentInChildren<Light>();
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>();
		((Renderer)meshRenderer).material.color = Color.white;
		((Component)this).transform.localScale = Vector3.zero;
		soundExplosion.Play(((Component)this).transform.position);
		soundExplosionGlobal.Play(((Component)this).transform.position);
		particleSystemSparks.Play();
		particleSystemLightning.Play();
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.Rotate(Vector3.up, 100f * Time.deltaTime);
		if (((Component)this).transform.localScale.x < startScale)
		{
			Transform transform = ((Component)this).transform;
			transform.localScale += Vector3.one * Time.deltaTime * 20f;
			lightShockwave.intensity = Mathf.Lerp(4f, 35f, Mathf.InverseLerp(0f, startScale, ((Component)this).transform.localScale.x));
			lightShockwave.range = ((Component)this).transform.localScale.x * 3f;
			return;
		}
		if (!finalScale)
		{
			((Component)this).transform.localScale = Vector3.one * startScale;
			((Component)hurtCollider).gameObject.SetActive(false);
			finalScale = true;
			return;
		}
		float num = Mathf.Lerp(((Component)this).transform.localScale.x, startScale * 1.2f, Time.deltaTime * 2f);
		((Component)this).transform.localScale = Vector3.one * num;
		float num2 = Mathf.InverseLerp(startScale, startScale * 1.2f, num);
		Color color = ((Renderer)meshRenderer).material.color;
		color.a = Mathf.Lerp(1f, 0f, num2);
		((Renderer)meshRenderer).material.color = color;
		lightShockwave.intensity = Mathf.Lerp(35f, 0f, num2);
		if (num2 > 0.998f)
		{
			if (Object.op_Implicit((Object)(object)particleSystemSparks))
			{
				((Component)particleSystemSparks).transform.parent = null;
			}
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
