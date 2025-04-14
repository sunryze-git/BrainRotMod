using UnityEngine;

public class AudioPersistent : MonoBehaviour
{
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = ((Component)this).GetComponent<AudioSource>();
		((Component)this).transform.parent = null;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		if (!audioSource.isPlaying)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
