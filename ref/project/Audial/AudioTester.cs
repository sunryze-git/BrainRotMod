using UnityEngine;

namespace Audial;

public class AudioTester : MonoBehaviour
{
	[HideInInspector]
	public bool hasAudioSource = true;

	[HideInInspector]
	public AudioSource audioSource;

	public bool playAudio
	{
		set
		{
			((Component)this).gameObject.SendMessage("ClearBuffer");
			((Component)this).gameObject.SendMessage("ResetUtils", (SendMessageOptions)1);
			if (hasAudioSource && (Object)(object)audioSource.clip != (Object)null)
			{
				audioSource.Play();
			}
		}
	}

	public bool stopAudio
	{
		set
		{
			((Component)this).gameObject.SendMessage("ClearBuffer");
			if (hasAudioSource)
			{
				audioSource.Stop();
			}
		}
	}

	public void ClearBuffer()
	{
	}

	public void SetRunEffectInEditMode()
	{
	}
}
