using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
	public AudioSource Source;

	public AudioClip[] Sounds;

	private AudioLowPassLogic LowPassLogic;

	private bool HasLowPassLogic;

	public AudioManager.AudioType Type;

	[Range(0f, 1f)]
	public float Volume = 0.5f;

	[Range(0f, 1f)]
	public float VolumeRandom = 0.1f;

	[Range(0f, 5f)]
	public float Pitch = 1f;

	[Range(0f, 2f)]
	public float PitchRandom = 0.1f;

	[Range(0f, 1f)]
	public float SpatialBlend = 1f;

	[Range(0f, 5f)]
	public float Doppler = 1f;

	[Range(0f, 1f)]
	public float ReverbMix = 1f;

	[Range(0f, 5f)]
	public float FalloffMultiplier = 1f;

	[Space]
	[Range(0f, 1f)]
	public float OffscreenVolume = 1f;

	[Range(0f, 1f)]
	public float OffscreenFalloff = 1f;

	[Space]
	public List<Collider> LowPassIgnoreColliders = new List<Collider>();

	private AudioClip LoopClip;

	internal float LoopVolume;

	internal float LoopVolumeCurrent;

	internal float LoopVolumeFinal;

	internal float LoopPitch;

	internal float LoopFalloff;

	internal float LoopFalloffFinal;

	private float LoopOffScreenTime = 0.25f;

	private float LoopOffScreenTimer;

	private bool LoopOffScreen;

	private float LoopOffScreenVolume;

	private float LoopOffScreenFalloff;

	internal float StartTimeOverride = 999999f;

	private bool AudioInfoFetched;

	public AudioSource Play(Vector3 position, float volumeMultiplier = 1f, float falloffMultiplier = 1f, float offscreenVolumeMultiplier = 1f, float offscreenFalloffMultiplier = 1f)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		if (Sounds.Length == 0)
		{
			return null;
		}
		AudioClip val = Sounds[Random.Range(0, Sounds.Length)];
		float num = Pitch + Random.Range(0f - PitchRandom, PitchRandom);
		AudioSource val2 = Source;
		if (!Object.op_Implicit((Object)(object)val2))
		{
			GameObject val3 = AudioManager.instance.AudioDefault;
			switch (Type)
			{
			case AudioManager.AudioType.HighFalloff:
				val3 = AudioManager.instance.AudioHighFalloff;
				break;
			case AudioManager.AudioType.Footstep:
				val3 = AudioManager.instance.AudioFootstep;
				break;
			case AudioManager.AudioType.MaterialImpact:
				val3 = AudioManager.instance.AudioMaterialImpact;
				break;
			case AudioManager.AudioType.Cutscene:
				val3 = AudioManager.instance.AudioCutscene;
				break;
			case AudioManager.AudioType.AmbienceBreaker:
				val3 = AudioManager.instance.AudioAmbienceBreaker;
				break;
			case AudioManager.AudioType.LowFalloff:
				val3 = AudioManager.instance.AudioLowFalloff;
				break;
			case AudioManager.AudioType.Global:
				val3 = AudioManager.instance.AudioGlobal;
				break;
			case AudioManager.AudioType.HigherFalloff:
				val3 = AudioManager.instance.AudioHigherFalloff;
				break;
			case AudioManager.AudioType.Attack:
				val3 = AudioManager.instance.AudioAttack;
				break;
			case AudioManager.AudioType.Persistent:
				val3 = AudioManager.instance.AudioPersistent;
				break;
			}
			GameObject val4 = Object.Instantiate<GameObject>(val3, position, Quaternion.identity, AudioManager.instance.SoundsParent);
			((Object)val4.gameObject).name = ((Object)val).name;
			val2 = val4.GetComponent<AudioSource>();
			if ((Object)(object)val3 != (Object)(object)AudioManager.instance.AudioPersistent)
			{
				Object.Destroy((Object)(object)val4, val.length / num);
			}
		}
		else if (!((Behaviour)val2).enabled)
		{
			return null;
		}
		AudioSource obj = val2;
		obj.minDistance *= FalloffMultiplier;
		AudioSource obj2 = val2;
		obj2.minDistance *= falloffMultiplier;
		AudioSource obj3 = val2;
		obj3.maxDistance *= FalloffMultiplier;
		AudioSource obj4 = val2;
		obj4.maxDistance *= falloffMultiplier;
		val2.clip = Sounds[Random.Range(0, Sounds.Length)];
		val2.volume = (Volume + Random.Range(0f - VolumeRandom, VolumeRandom)) * volumeMultiplier;
		if (SpatialBlend > 0f && (OffscreenVolume * offscreenVolumeMultiplier < 1f || OffscreenFalloff * offscreenFalloffMultiplier < 1f) && !SemiFunc.OnScreen(((Component)val2).transform.position, 0.1f, 0.1f))
		{
			AudioSource obj5 = val2;
			obj5.volume *= OffscreenVolume * offscreenVolumeMultiplier;
			AudioSource obj6 = val2;
			obj6.minDistance *= OffscreenFalloff * offscreenFalloffMultiplier;
			AudioSource obj7 = val2;
			obj7.maxDistance *= OffscreenFalloff * offscreenFalloffMultiplier;
		}
		val2.spatialBlend = SpatialBlend;
		val2.reverbZoneMix = ReverbMix;
		val2.dopplerLevel = Doppler;
		val2.pitch = num;
		val2.loop = false;
		if (SpatialBlend > 0f)
		{
			StartLowPass(val2);
		}
		val2.Play();
		return val2;
	}

	public void Stop()
	{
		Source.Stop();
	}

	private void StartLowPass(AudioSource source)
	{
		LowPassLogic = ((Component)source).GetComponent<AudioLowPassLogic>();
		if (Object.op_Implicit((Object)(object)LowPassLogic))
		{
			if (LowPassIgnoreColliders.Count > 0)
			{
				LowPassLogic.LowPassIgnoreColliders.AddRange(LowPassIgnoreColliders);
			}
			LowPassLogic.Setup();
			HasLowPassLogic = true;
		}
	}

	public void PlayLoop(bool playing, float fadeInSpeed, float fadeOutSpeed, float pitchMultiplier = 1f)
	{
		if (!AudioInfoFetched)
		{
			LoopClip = Sounds[Random.Range(0, Sounds.Length)];
			Source.clip = LoopClip;
		}
		if (playing)
		{
			if (!Source.isPlaying)
			{
				LoopVolume = Volume + Random.Range(0f - VolumeRandom, VolumeRandom);
				LoopPitch = Pitch + Random.Range(0f - PitchRandom, PitchRandom);
				LoopVolumeCurrent = 0f;
				LoopVolumeFinal = LoopVolumeCurrent;
				Source.volume = LoopVolumeCurrent;
				Source.pitch = LoopPitch * pitchMultiplier;
				Source.spatialBlend = SpatialBlend;
				Source.reverbZoneMix = ReverbMix;
				Source.dopplerLevel = Doppler;
				AudioSource source = Source;
				source.minDistance *= FalloffMultiplier;
				AudioSource source2 = Source;
				source2.maxDistance *= FalloffMultiplier;
				LoopFalloff = Source.maxDistance;
				Source.time = Random.Range(0f, Source.clip.length);
				if (StartTimeOverride != 999999f)
				{
					Source.time = StartTimeOverride;
				}
				Source.loop = true;
				StartLowPass(Source);
				Source.Play();
			}
			else
			{
				LoopVolumeCurrent += fadeInSpeed * Time.deltaTime;
				LoopVolumeCurrent = Mathf.Clamp(LoopVolumeCurrent, 0f, LoopVolume);
				LoopOffScreenLogic();
				Source.pitch = LoopPitch * pitchMultiplier;
				if (HasLowPassLogic)
				{
					LowPassLogic.Volume = LoopVolumeFinal;
				}
				else
				{
					Source.volume = LoopVolumeFinal;
				}
			}
		}
		else if (Source.isPlaying)
		{
			LoopVolumeCurrent -= fadeOutSpeed * Time.deltaTime;
			LoopVolumeCurrent = Mathf.Clamp(LoopVolumeCurrent, 0f, LoopVolume);
			LoopOffScreenLogic();
			Source.pitch = LoopPitch * pitchMultiplier;
			if (HasLowPassLogic)
			{
				LowPassLogic.Volume = LoopVolumeFinal;
			}
			else
			{
				Source.volume = LoopVolumeFinal;
			}
			if (LoopVolumeFinal <= 0f)
			{
				Source.Stop();
			}
		}
	}

	private void LoopOffScreenLogic()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		LoopVolumeFinal = LoopVolumeCurrent;
		if (!(SpatialBlend > 0f) || (!(OffscreenVolume < 1f) && !(OffscreenFalloff < 1f)))
		{
			return;
		}
		if (LoopOffScreenTimer <= 0f)
		{
			LoopOffScreenTimer = LoopOffScreenTime;
			LoopOffScreen = !SemiFunc.OnScreen(((Component)Source).transform.position, 0.1f, 0.1f);
		}
		else
		{
			LoopOffScreenTimer -= Time.deltaTime;
		}
		if (OffscreenVolume < 1f)
		{
			if (LoopOffScreen)
			{
				LoopOffScreenVolume = Mathf.Lerp(LoopOffScreenVolume, OffscreenVolume, 15f * Time.deltaTime);
			}
			else
			{
				LoopOffScreenVolume = Mathf.Lerp(LoopOffScreenVolume, 1f, 15f * Time.deltaTime);
			}
			LoopVolumeFinal *= LoopOffScreenVolume;
		}
		if (OffscreenFalloff < 1f)
		{
			if (LoopOffScreen)
			{
				LoopFalloffFinal = Mathf.Lerp(LoopFalloffFinal, LoopFalloff * OffscreenFalloff, 15f * Time.deltaTime);
			}
			else
			{
				LoopFalloffFinal = Mathf.Lerp(LoopFalloffFinal, LoopFalloff, 15f * Time.deltaTime);
			}
			if (HasLowPassLogic)
			{
				LowPassLogic.Falloff = LoopFalloffFinal;
			}
			else
			{
				Source.maxDistance = LoopFalloffFinal;
			}
		}
	}

	public static void CopySound(Sound from, Sound to)
	{
		to.Source = from.Source;
		to.Sounds = from.Sounds;
		to.Type = from.Type;
		to.Volume = from.Volume;
		to.VolumeRandom = from.VolumeRandom;
		to.Pitch = from.Pitch;
		to.PitchRandom = from.PitchRandom;
		to.SpatialBlend = from.SpatialBlend;
		to.ReverbMix = from.ReverbMix;
		to.Doppler = from.Doppler;
	}
}
