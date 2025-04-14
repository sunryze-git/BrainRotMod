using UnityEngine;

public class StunExplosion : MonoBehaviour
{
	public Light light;

	public AnimationCurve lightCurve;

	private float lightEval;

	private float removeTimer;

	private HurtCollider hurtCollider;

	public ItemGrenade itemGrenade;

	private void Start()
	{
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>();
	}

	public void StunExplosionReset()
	{
		removeTimer = 0f;
		lightEval = 0f;
		((Component)this).gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!((Component)this).gameObject.activeSelf)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)light))
		{
			if (lightEval < 1f)
			{
				light.intensity = 10f * lightCurve.Evaluate(lightEval);
				lightEval += 0.2f * Time.deltaTime;
			}
			else
			{
				light.intensity = 0f;
			}
		}
		if (removeTimer > 0.5f)
		{
			((Component)hurtCollider).gameObject.SetActive(false);
		}
		else
		{
			((Component)hurtCollider).gameObject.SetActive(true);
		}
		removeTimer += Time.deltaTime;
		if (removeTimer >= 20f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
