using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
	[Serializable]
	public class LevelMusicTrack
	{
		public string name;

		public AudioClip audioClip;

		[Range(0f, 1f)]
		public float volume = 0.8f;
	}

	public static LevelMusic instance;

	private PhotonView photonView;

	private AudioSource audioSource;

	private bool active;

	private bool activePlayed;

	public AnimationCurve fadeCurve;

	private bool interrupt;

	private float fadeInterrupt;

	private float interruptVolume;

	private float interruptVolumeLerp;

	[Space]
	public float cooldownTimeMin;

	public float cooldownTimeMax;

	private float cooldownTime;

	[Space]
	private List<LevelMusicTrack> levelMusicTracksRandom;

	private int trackIndex;

	private void Awake()
	{
		instance = this;
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		cooldownTime = Random.Range(cooldownTimeMin, cooldownTimeMax);
		audioSource = ((Component)this).GetComponent<AudioSource>();
	}

	public void Setup()
	{
		if (!Object.op_Implicit((Object)(object)LevelGenerator.Instance.Level.MusicPreset))
		{
			((Component)this).gameObject.SetActive(false);
			return;
		}
		levelMusicTracksRandom = new List<LevelMusicTrack>(LevelGenerator.Instance.Level.MusicPreset.tracks);
		levelMusicTracksRandom.Shuffle();
	}

	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			Interrupt(10f);
		}
		if (interrupt)
		{
			interruptVolumeLerp += fadeInterrupt * Time.deltaTime;
			audioSource.volume = Mathf.Lerp(interruptVolume, 0f, fadeCurve.Evaluate(interruptVolumeLerp));
			if (audioSource.volume <= 0.01f)
			{
				audioSource.Stop();
				interrupt = false;
			}
		}
		else if (active)
		{
			if (!SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}
			if (!activePlayed)
			{
				audioSource.clip = levelMusicTracksRandom[trackIndex].audioClip;
				audioSource.volume = levelMusicTracksRandom[trackIndex].volume;
				audioSource.PlayScheduled(AudioSettings.dspTime + 0.5);
				if (SemiFunc.IsMultiplayer())
				{
					photonView.RPC("PlayTrack", (RpcTarget)1, new object[1] { ((Object)audioSource.clip).name });
				}
				trackIndex++;
				if (trackIndex >= levelMusicTracksRandom.Count)
				{
					trackIndex = 0;
					levelMusicTracksRandom.Shuffle();
				}
				activePlayed = true;
			}
			else if (!audioSource.isPlaying)
			{
				active = false;
			}
		}
		else if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			cooldownTime -= Time.deltaTime;
			if (cooldownTime <= 0f)
			{
				cooldownTime = Random.Range(cooldownTimeMin, cooldownTimeMax);
				active = true;
				activePlayed = false;
			}
		}
	}

	public void Interrupt(float interruptSpeed)
	{
		if (((Behaviour)this).isActiveAndEnabled)
		{
			if (cooldownTime < cooldownTimeMin)
			{
				cooldownTime = Random.Range(cooldownTimeMin, cooldownTimeMax);
			}
			if (audioSource.isPlaying && (!interrupt || !(interruptSpeed <= fadeInterrupt)))
			{
				interrupt = true;
				fadeInterrupt = interruptSpeed;
				interruptVolume = audioSource.volume;
				interruptVolumeLerp = 0f;
				active = false;
			}
		}
	}

	[PunRPC]
	public void PlayTrack(string _trackName)
	{
		if (!((Behaviour)this).isActiveAndEnabled || audioSource.isPlaying)
		{
			return;
		}
		foreach (LevelMusicTrack item in levelMusicTracksRandom)
		{
			if (((Object)item.audioClip).name == _trackName)
			{
				audioSource.clip = item.audioClip;
				audioSource.volume = item.volume;
				audioSource.PlayScheduled(AudioSettings.dspTime + 0.5);
			}
		}
	}
}
