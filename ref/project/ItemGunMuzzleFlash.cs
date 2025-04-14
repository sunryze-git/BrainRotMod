using System.Collections;
using UnityEngine;

public class ItemGunMuzzleFlash : MonoBehaviour
{
	private ParticleSystem smoke;

	private ParticleSystem impact;

	private ParticleSystem sparks;

	private Light shootLight;

	public void ActivateAllEffects()
	{
		((Component)this).gameObject.SetActive(true);
		smoke = ((Component)((Component)this).transform.Find("Particle Smoke")).GetComponent<ParticleSystem>();
		impact = ((Component)((Component)this).transform.Find("Particle Impact")).GetComponent<ParticleSystem>();
		sparks = ((Component)((Component)this).transform.Find("Particle Sparks")).GetComponent<ParticleSystem>();
		shootLight = ((Component)this).GetComponentInChildren<Light>();
		((Component)smoke).gameObject.SetActive(true);
		((Component)impact).gameObject.SetActive(true);
		((Component)sparks).gameObject.SetActive(true);
		((Behaviour)shootLight).enabled = true;
		smoke.Play();
		impact.Play();
		sparks.Play();
		((MonoBehaviour)this).StartCoroutine(MuzzleFlashDestroy());
	}

	private IEnumerator MuzzleFlashDestroy()
	{
		yield return (object)new WaitForSeconds(0.1f);
		while (smoke.isPlaying || impact.isPlaying || sparks.isPlaying || ((Behaviour)shootLight).enabled)
		{
			yield return null;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		if (Object.op_Implicit((Object)(object)shootLight))
		{
			shootLight.intensity = Mathf.Lerp(shootLight.intensity, 0f, Time.deltaTime * 10f);
			if (shootLight.intensity < 0.01f)
			{
				((Behaviour)shootLight).enabled = false;
			}
		}
	}
}
