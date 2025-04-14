using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Strobotnik.Klattersynth;

public class Speech : MonoBehaviour
{
	private struct ScheduledUnit
	{
		public string text;

		public int voiceBaseFrequency;

		public VoicingSource voicingSource;

		public bool bracketsAsPhonemes;

		public SpeechClip pregenClip;
	}

	[Tooltip("When true, speech is real-time generated (played using a single small looping audio clip).\n\nNOTE: Not supported with WebGL, will be auto-disabled in Start() when running in WebGL!")]
	public bool useStreamingMode = true;

	[Tooltip("Maximum amount of speech clips to automatically cache in non-streaming mode.\n(Least recently used are discarded when going over this amount.)")]
	public int maxAutoCachedClips = 10;

	[Tooltip("Base frequency for the synthesized voice.\nCan be runtime-adjusted.")]
	public int voiceBaseFrequency = 220;

	[Tooltip("Type of \"voicing source\".\nCan be runtime-adjusted.")]
	public VoicingSource voicingSource;

	[Tooltip("How many milliseconds to use per one \"speech frame\".")]
	[Range(1f, 100f)]
	public int msPerSpeechFrame = 10;

	[Tooltip("Amount of flutter in voice.")]
	[Range(0f, 200f)]
	public int flutter = 10;

	[Tooltip("Speed of the flutter.")]
	[Range(0.001f, 100f)]
	public float flutterSpeed = 1f;

	private TTSVoice ttsVoice;

	private const int sampleRate = 11025;

	private bool talking;

	private AudioSource audioSrc;

	private SpeechSynth speechSynth;

	private StringBuilder speakSB;

	private List<SpeechClip> cachedSpeechClips;

	private List<ScheduledUnit> scheduled = new List<ScheduledUnit>(5);

	private bool errCheck(bool errorWhenTrue, string logErrorString)
	{
		if (errorWhenTrue)
		{
			if (logErrorString != null)
			{
				Debug.LogError((object)logErrorString, (Object)(object)this);
			}
			return true;
		}
		return false;
	}

	private void cache(SpeechClip sc)
	{
		if (maxAutoCachedClips <= 0)
		{
			return;
		}
		if (cachedSpeechClips == null)
		{
			cachedSpeechClips = new List<SpeechClip>(maxAutoCachedClips);
		}
		else
		{
			int num = cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == sc.hash);
			if (num >= 0)
			{
				cachedSpeechClips[num] = sc;
				return;
			}
			if (cachedSpeechClips.Count >= maxAutoCachedClips)
			{
				cachedSpeechClips.RemoveRange(0, cachedSpeechClips.Count - (maxAutoCachedClips - 1));
			}
		}
		cachedSpeechClips.Add(sc);
	}

	private SpeechClip findFromCache(StringBuilder text, int freq, VoicingSource voicingSrc, bool bracketsAsPhonemes)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (cachedSpeechClips == null)
		{
			return null;
		}
		ulong hash = SpeechSynth.makeHashCode(text, freq, voicingSrc, bracketsAsPhonemes);
		int num = cachedSpeechClips.FindIndex((SpeechClip x) => x.hash == hash);
		if (num < 0)
		{
			return null;
		}
		SpeechClip val = cachedSpeechClips[num];
		if (num < cachedSpeechClips.Count - 1)
		{
			cachedSpeechClips.RemoveAt(num);
			cachedSpeechClips.Add(val);
		}
		return val;
	}

	public void speakNextScheduled()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (scheduled != null && scheduled.Count != 0)
		{
			ScheduledUnit scheduledUnit = scheduled[0];
			scheduled.RemoveAt(0);
			if (scheduledUnit.pregenClip != null)
			{
				speak(scheduledUnit.pregenClip);
			}
			else
			{
				speak(scheduledUnit.voiceBaseFrequency, scheduledUnit.voicingSource, scheduledUnit.text, scheduledUnit.bracketsAsPhonemes);
			}
		}
	}

	public bool isTalking()
	{
		return talking;
	}

	public float getCurrentLoudness()
	{
		return speechSynth.getCurrentLoudness();
	}

	public string getPhonemes()
	{
		return speechSynth.getPhonemes();
	}

	public void speak(SpeechClip pregenSpeech)
	{
		speechSynth.speak(pregenSpeech);
		talking = true;
	}

	public void speak(string text, bool bracketsAsPhonemes = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
	}

	public void speak(int voiceBaseFrequency, VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!errCheck(text == null, "null text"))
		{
			if (speakSB == null)
			{
				speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			speakSB.Length = 0;
			speakSB.Append(text);
			speak(voiceBaseFrequency, voicingSource, speakSB, bracketsAsPhonemes);
		}
	}

	public void speak(StringBuilder text, bool bracketsAsPhonemes = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
	}

	private void VoiceText(StringBuilder text, float wordTime)
	{
		if (Object.op_Implicit((Object)(object)ttsVoice))
		{
			ttsVoice.VoiceText(text.ToString(), wordTime);
		}
	}

	public void speak(int voiceBaseFrequency, VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		VoiceText(text, Time.time);
		text = new StringBuilder(text.ToString().ToLower());
		if (errCheck(text == null, "null text (SB)"))
		{
			return;
		}
		if (!useStreamingMode)
		{
			SpeechClip speechClip = findFromCache(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
			if (speechClip == null)
			{
				pregenerate(out speechClip, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, addToCache: true);
			}
			if (speechClip != null)
			{
				talking = true;
				speechSynth.speak(speechClip);
			}
		}
		else
		{
			talking = true;
			speechSynth.speak(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes);
		}
	}

	public void pregenerate(string text, bool bracketsAsPhonemes = false)
	{
		pregenerate(out var _, text, bracketsAsPhonemes, addToCache: true);
	}

	public void pregenerate(out SpeechClip speechClip, string text, bool bracketsAsPhonemes = false, bool addToCache = false)
	{
		speechClip = null;
		if (!errCheck(text == null, "null text"))
		{
			if (speakSB == null)
			{
				speakSB = new StringBuilder(text.Length * 3 / 2);
			}
			speakSB.Length = 0;
			speakSB.Append(text);
			pregenerate(out speechClip, speakSB, bracketsAsPhonemes, addToCache);
		}
	}

	public void pregenerate(out SpeechClip speechClip, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		pregenerate(out speechClip, voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes, addToCache);
	}

	public void pregenerate(out SpeechClip speechClip, int voiceBaseFrequency, VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false, bool addToCache = false)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		speechClip = null;
		if (!errCheck(text == null, "null text (SB)"))
		{
			speechClip = speechSynth.pregenerate(text, voiceBaseFrequency, voicingSource, bracketsAsPhonemes, true);
			if (speechClip != null && addToCache)
			{
				cache(speechClip);
			}
		}
	}

	public void schedule(SpeechClip speechClip)
	{
		ScheduledUnit item = default(ScheduledUnit);
		item.pregenClip = speechClip;
		scheduled.Add(item);
	}

	public void scheduleClear()
	{
		scheduled.Clear();
	}

	public void schedule(string text, bool bracketsAsPhonemes = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		schedule(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
	}

	public void schedule(StringBuilder text, bool bracketsAsPhonemes = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		schedule(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
	}

	public void schedule(int voiceBaseFrequency, VoicingSource voicingSource, string text, bool bracketsAsPhonemes = false)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!talking && scheduled.Count == 0)
		{
			speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
			return;
		}
		ScheduledUnit item = default(ScheduledUnit);
		item.voiceBaseFrequency = voiceBaseFrequency;
		item.voicingSource = voicingSource;
		item.text = text;
		item.bracketsAsPhonemes = bracketsAsPhonemes;
		item.pregenClip = null;
		scheduled.Add(item);
	}

	public void schedule(int voiceBaseFrequency, VoicingSource voicingSource, StringBuilder text, bool bracketsAsPhonemes = false)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!talking && scheduled.Count == 0)
		{
			speak(voiceBaseFrequency, voicingSource, text, bracketsAsPhonemes);
		}
		else
		{
			schedule(voiceBaseFrequency, voicingSource, text.ToString(), bracketsAsPhonemes);
		}
	}

	public void stop(bool allScheduled = false)
	{
		if (allScheduled)
		{
			scheduled.Clear();
		}
		speechSynth.stop();
	}

	public void cacheClear()
	{
		if (cachedSpeechClips != null)
		{
			cachedSpeechClips.Clear();
		}
	}

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		audioSrc = ((Component)this).GetComponentInParent<AudioSource>();
		speechSynth = new SpeechSynth();
		speechSynth.init(audioSrc, useStreamingMode, 11025, msPerSpeechFrame, flutter, flutterSpeed);
		ttsVoice = ((Component)this).GetComponentInParent<TTSVoice>();
	}

	private void Update()
	{
		talking = speechSynth.update();
		if (!talking)
		{
			speakNextScheduled();
		}
	}

	private void OnDestroy()
	{
		ClearAllData();
	}

	private void ClearAllData()
	{
		stop(allScheduled: true);
		cacheClear();
		scheduleClear();
		if (speakSB != null)
		{
			speakSB.Clear();
		}
		if (Object.op_Implicit((Object)(object)audioSrc))
		{
			audioSrc.Stop();
		}
	}

	private void OnDisable()
	{
		ClearAllData();
	}
}
