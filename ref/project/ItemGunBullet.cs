using System.Collections;
using UnityEngine;

public class ItemGunBullet : MonoBehaviour
{
	private Transform hitEffectTransform;

	private ParticleSystem particleSparks;

	private ParticleSystem particleSmoke;

	private ParticleSystem particleImpact;

	private Light hitLight;

	private LineRenderer shootLine;

	public HurtCollider hurtCollider;

	internal bool bulletHit;

	internal Vector3 hitPosition;

	public float hurtColliderTimer = 0.25f;

	private bool shootLineActive;

	private float shootLineLerp;

	internal AnimationCurve shootLineWidthCurve;

	public void ActivateAll()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.SetActive(true);
		hitEffectTransform = ((Component)this).transform.Find("Hit Effect");
		particleSparks = ((Component)hitEffectTransform.Find("Particle Sparks")).GetComponent<ParticleSystem>();
		particleSmoke = ((Component)hitEffectTransform.Find("Particle Smoke")).GetComponent<ParticleSystem>();
		particleImpact = ((Component)hitEffectTransform.Find("Particle Impact")).GetComponent<ParticleSystem>();
		hitLight = ((Component)hitEffectTransform.Find("Hit Light")).GetComponent<Light>();
		shootLine = ((Component)this).GetComponentInChildren<LineRenderer>();
		Vector3 position = ((Component)this).transform.position;
		Vector3 val = hitPosition - position;
		((Renderer)shootLine).enabled = true;
		shootLine.SetPosition(0, ((Component)this).transform.position);
		shootLine.SetPosition(1, ((Component)this).transform.position + ((Vector3)(ref val)).normalized * 0.5f);
		shootLine.SetPosition(2, hitPosition - ((Vector3)(ref val)).normalized * 0.5f);
		shootLine.SetPosition(3, hitPosition);
		shootLineActive = true;
		shootLineLerp = 0f;
		if (bulletHit)
		{
			((Component)hitEffectTransform).gameObject.SetActive(true);
			((Component)particleSparks).gameObject.SetActive(true);
			((Component)particleSmoke).gameObject.SetActive(true);
			((Component)particleImpact).gameObject.SetActive(true);
			((Behaviour)hitLight).enabled = true;
			((Component)hurtCollider).gameObject.SetActive(true);
			Quaternion rotation = Quaternion.LookRotation(val);
			((Component)hurtCollider).transform.rotation = rotation;
			((Component)hurtCollider).transform.position = hitPosition;
			((Component)hurtCollider).gameObject.SetActive(true);
			hitEffectTransform.position = hitPosition;
			hitEffectTransform.rotation = rotation;
		}
		((MonoBehaviour)this).StartCoroutine(BulletDestroy());
	}

	private IEnumerator BulletDestroy()
	{
		yield return (object)new WaitForSeconds(0.2f);
		while (particleSparks.isPlaying || particleSmoke.isPlaying || particleImpact.isPlaying || ((Behaviour)hitLight).enabled || ((Renderer)shootLine).enabled || ((Component)hurtCollider).gameObject.activeSelf)
		{
			yield return null;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void LineRendererLogic()
	{
		if (shootLineActive)
		{
			shootLine.widthMultiplier = shootLineWidthCurve.Evaluate(shootLineLerp);
			shootLineLerp += Time.deltaTime * 5f;
			if (shootLineLerp >= 1f)
			{
				((Renderer)shootLine).enabled = false;
				((Component)shootLine).gameObject.SetActive(false);
				shootLineActive = false;
			}
		}
	}

	private void Update()
	{
		LineRendererLogic();
		if (!bulletHit)
		{
			return;
		}
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			((Component)hurtCollider).gameObject.SetActive(true);
		}
		else
		{
			((Component)hurtCollider).gameObject.SetActive(false);
		}
		if (Object.op_Implicit((Object)(object)hitLight))
		{
			hitLight.intensity = Mathf.Lerp(hitLight.intensity, 0f, Time.deltaTime * 10f);
			if (hitLight.intensity < 0.01f)
			{
				((Behaviour)hitLight).enabled = false;
			}
		}
	}
}
