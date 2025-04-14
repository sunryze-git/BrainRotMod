using System.Collections;
using UnityEngine;

public class AudioLoopDistance : MonoBehaviour
{
	private AudioSource audioSource;

	private AudioLowPassLogic audioLowPassLogic;

	private float volumeDefault;

	public ParticleSystem[] particles;

	private void Awake()
	{
		audioSource = ((Component)this).GetComponent<AudioSource>();
		audioLowPassLogic = ((Component)this).GetComponent<AudioLowPassLogic>();
		audioLowPassLogic.Setup();
		volumeDefault = audioSource.volume;
		audioSource.volume = 0f;
		AudioLoopDistanceParticle[] componentsInChildren = ((Component)this).GetComponentsInChildren<AudioLoopDistanceParticle>();
		foreach (AudioLoopDistanceParticle audioLoopDistanceParticle in componentsInChildren)
		{
			bool flag = false;
			ParticleSystem[] array = particles;
			for (int j = 0; j < array.Length; j++)
			{
				if ((Object)(object)((Component)array[j]).transform == (Object)(object)((Component)audioLoopDistanceParticle).transform)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogError((object)("Particle not hooked up to Audio: " + ((Object)audioLoopDistanceParticle).name), (Object)(object)((Component)audioLoopDistanceParticle).transform);
			}
		}
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private void Start()
	{
		AudioManager.instance.audioLoopDistances.Add(this);
	}

	private void OnDestroy()
	{
		AudioManager.instance.audioLoopDistances.Remove(this);
	}

	public void Restart()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private IEnumerator Logic()
	{
		yield return (object)new WaitForSeconds(0.1f);
		while (true)
		{
			float _distance = Vector3.Distance(((Component)AudioManager.instance.AudioListener).transform.position, ((Component)this).transform.position);
			if (_distance < audioSource.maxDistance + 5f)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.time = Random.Range(0f, audioSource.clip.length);
					audioSource.Play();
					audioLowPassLogic.Setup();
					ParticleSystem[] array = particles;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play();
					}
				}
				while (audioLowPassLogic.Volume < volumeDefault)
				{
					audioLowPassLogic.Volume += Time.deltaTime;
					yield return null;
				}
			}
			else if (audioSource.isPlaying)
			{
				while (audioLowPassLogic.Volume > 0f)
				{
					audioLowPassLogic.Volume -= Time.deltaTime;
					yield return null;
				}
				audioSource.Stop();
				ParticleSystem[] array = particles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop();
				}
			}
			if (Mathf.Abs(audioSource.maxDistance - _distance) > 20f)
			{
				yield return (object)new WaitForSeconds(Random.Range(3f, 6f));
			}
			else
			{
				yield return (object)new WaitForSeconds(Random.Range(0.5f, 2f));
			}
		}
	}
}
