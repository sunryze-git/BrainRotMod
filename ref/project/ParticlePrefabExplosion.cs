using UnityEngine;

public class ParticlePrefabExplosion : MonoBehaviour
{
	public ParticleSystem particleFire;

	public ParticleSystem particleSmoke;

	public Light light;

	public AnimationCurve lightIntensityCurve;

	private float lightIntensityCurveProgress;

	public Gradient lightColorOverTime;

	internal float explosionSize = 1f;

	internal int explosionDamage;

	internal int explosionDamageEnemy;

	private float smokeNullCheckTimer;

	[HideInInspector]
	public float forceMultiplier = 1f;

	[HideInInspector]
	public bool onlyParticleEffect;

	internal bool SkipHurtColliderSetup;

	public HurtCollider HurtCollider;

	private bool HurtColliderActive = true;

	private bool HurtColliderFirstSetup = true;

	private bool HurtColliderSecondSetup = true;

	private float HurtColliderTimer;

	private void Start()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (explosionSize <= 0.25f)
		{
			flag = true;
		}
		MainModule main = particleFire.main;
		float startSpeedMultiplier = ((MainModule)(ref main)).startSpeedMultiplier;
		float startLifetimeMultiplier = ((MainModule)(ref main)).startLifetimeMultiplier;
		((MainModule)(ref main)).startSpeedMultiplier = startSpeedMultiplier * explosionSize;
		((MainModule)(ref main)).startLifetimeMultiplier = startLifetimeMultiplier * explosionSize;
		((MainModule)(ref main)).startLifetimeMultiplier = Mathf.Max(((MainModule)(ref main)).startLifetimeMultiplier, 0.2f);
		((MainModule)(ref main)).startSizeMultiplier = Mathf.Max(((MainModule)(ref main)).startSizeMultiplier, 0.1f);
		if (flag)
		{
			((MainModule)(ref main)).startSpeedMultiplier = ((MainModule)(ref main)).startSpeedMultiplier * 0.5f;
			((MainModule)(ref main)).startLifetimeMultiplier = ((MainModule)(ref main)).startLifetimeMultiplier * 0.5f;
			((MainModule)(ref main)).startSizeMultiplier = ((MainModule)(ref main)).startSizeMultiplier * 0.8f;
		}
		particleFire.Play();
		MainModule main2 = particleSmoke.main;
		startSpeedMultiplier = ((MainModule)(ref main2)).startSpeedMultiplier;
		startLifetimeMultiplier = ((MainModule)(ref main2)).startLifetimeMultiplier;
		((MainModule)(ref main2)).startSpeedMultiplier = startSpeedMultiplier * explosionSize;
		((MainModule)(ref main2)).startLifetimeMultiplier = startLifetimeMultiplier * explosionSize;
		((MainModule)(ref main2)).startLifetimeMultiplier = Mathf.Max(((MainModule)(ref main2)).startLifetimeMultiplier * 1.2f, 2f);
		((MainModule)(ref main2)).startSizeMultiplier = Mathf.Max(((MainModule)(ref main)).startSizeMultiplier * 1.2f, 0.1f);
		particleSmoke.Play();
		((Behaviour)light).enabled = true;
	}

	private void Update()
	{
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if (!onlyParticleEffect && HurtColliderActive)
		{
			HurtColliderTimer += Time.deltaTime;
			if (HurtColliderFirstSetup)
			{
				if (!SkipHurtColliderSetup)
				{
					HurtCollider.playerDamage = explosionDamage;
					HurtCollider.enemyDamage = explosionDamageEnemy;
					HurtCollider.physHitForce = (float)explosionDamage * 0.5f;
					if (explosionDamage >= 50)
					{
						HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
						HurtCollider.physHingeDestroy = true;
					}
					else if (explosionDamage >= 15)
					{
						HurtCollider.physImpact = HurtCollider.BreakImpact.Medium;
					}
					else
					{
						HurtCollider.physImpact = HurtCollider.BreakImpact.Light;
					}
				}
				((Component)HurtCollider).gameObject.SetActive(true);
				((Component)HurtCollider).transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
				HurtColliderFirstSetup = false;
			}
			if (HurtColliderSecondSetup && HurtColliderTimer > 0.2f)
			{
				HurtCollider.playerDamage = 0;
				HurtCollider.playerHitForce *= 0.25f;
				HurtCollider.physHitForce *= 0.25f;
				if (HurtCollider.physImpact > HurtCollider.BreakImpact.None)
				{
					HurtCollider.physImpact--;
				}
				((Component)HurtCollider).transform.localScale = new Vector3(explosionSize * 2f, explosionSize * 2f, explosionSize * 2f);
				HurtColliderSecondSetup = false;
			}
			if (HurtColliderTimer > 1f)
			{
				((Component)HurtCollider).gameObject.SetActive(false);
				HurtColliderActive = false;
			}
		}
		smokeNullCheckTimer += Time.deltaTime;
		if (smokeNullCheckTimer > 1f)
		{
			if (!Object.op_Implicit((Object)(object)particleSmoke))
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			smokeNullCheckTimer = 0f;
		}
		if (((Behaviour)light).enabled)
		{
			float num = explosionSize;
			num = Mathf.Max(num, 0.8f);
			lightIntensityCurveProgress += 0.5f * Time.deltaTime;
			light.intensity = 10f * num * lightIntensityCurve.Evaluate(lightIntensityCurveProgress);
			light.range = 10f * num * lightIntensityCurve.Evaluate(lightIntensityCurveProgress);
			light.color = lightColorOverTime.Evaluate(lightIntensityCurveProgress);
			if (lightIntensityCurveProgress > ((Keyframe)(ref lightIntensityCurve.keys[lightIntensityCurve.length - 1])).time)
			{
				((Behaviour)light).enabled = false;
				lightIntensityCurveProgress = 0f;
			}
		}
	}
}
