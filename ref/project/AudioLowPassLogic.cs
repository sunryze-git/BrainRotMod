using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioLowPassLogic : MonoBehaviour
{
	public bool LowPass;

	[Space]
	public bool ForceStart;

	public bool AlwaysActive;

	[Space]
	public bool HasCustomVolume;

	[Range(0f, 1f)]
	public float CustomVolume = 0.5f;

	private float VolumeMultiplier = 0.5f;

	[Space]
	public bool HasCustomFalloff;

	[Range(0f, 1f)]
	public float CustomFalloff = 0.8f;

	private float FalloffMultiplier = 0.8f;

	internal bool Fetch = true;

	private bool First = true;

	private bool LogicActive;

	internal float Falloff;

	private float LowPassMin;

	private float LowPassMax;

	private AudioLowPassFilter AudioLowpassFilter;

	private AudioSource AudioSource;

	private LayerMask LayerMask;

	internal bool volumeFetched;

	internal float Volume;

	internal List<Collider> LowPassIgnoreColliders = new List<Collider>();

	private Transform audioListener;

	private void Start()
	{
		if (ForceStart)
		{
			Setup();
		}
	}

	public void Setup()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (Fetch)
		{
			audioListener = ((Component)AudioListenerFollow.instance).transform;
			AudioLowpassFilter = ((Component)this).GetComponent<AudioLowPassFilter>();
			AudioSource = ((Component)this).GetComponent<AudioSource>();
			LowPassMin = AudioManager.instance.lowpassValueMin;
			LowPassMax = AudioManager.instance.lowpassValueMax;
			if (HasCustomFalloff)
			{
				FalloffMultiplier = CustomFalloff;
			}
			Falloff = AudioSource.maxDistance;
			LayerMask = LayerMask.op_Implicit(LayerMask.GetMask(new string[3] { "Default", "PhysGrabObject", "PhysGrabObjectHinge" }));
			if (!volumeFetched)
			{
				if (HasCustomVolume)
				{
					VolumeMultiplier = CustomVolume;
				}
				volumeFetched = true;
				Volume = AudioSource.volume;
			}
			Fetch = false;
		}
		CheckStart();
	}

	private void Update()
	{
		if (!LogicActive)
		{
			return;
		}
		if (LowPass)
		{
			if (AudioLowpassFilter.cutoffFrequency != LowPassMin || AudioSource.maxDistance != Falloff * FalloffMultiplier || Mathf.Abs(AudioSource.volume - Volume * VolumeMultiplier) > 0.001f)
			{
				AudioLowPassFilter audioLowpassFilter = AudioLowpassFilter;
				audioLowpassFilter.cutoffFrequency -= (LowPassMax - LowPassMin) * 10f * Time.deltaTime;
				AudioLowpassFilter.cutoffFrequency = Mathf.Clamp(AudioLowpassFilter.cutoffFrequency, LowPassMin, LowPassMax);
				float num = (AudioLowpassFilter.cutoffFrequency - LowPassMin) / (LowPassMax - LowPassMin);
				AudioSource.maxDistance = Mathf.Lerp(Falloff * FalloffMultiplier, Falloff, num);
				AudioSource.volume = Mathf.Lerp(Volume * VolumeMultiplier, Volume, num);
			}
		}
		else if (AudioLowpassFilter.cutoffFrequency != LowPassMax || AudioSource.maxDistance != Falloff || Mathf.Abs(AudioSource.volume - Volume) > 0.001f)
		{
			AudioLowPassFilter audioLowpassFilter2 = AudioLowpassFilter;
			audioLowpassFilter2.cutoffFrequency += (LowPassMax - LowPassMin) * 1f * Time.deltaTime;
			AudioLowpassFilter.cutoffFrequency = Mathf.Clamp(AudioLowpassFilter.cutoffFrequency, LowPassMin, LowPassMax);
			float num2 = (AudioLowpassFilter.cutoffFrequency - LowPassMin) / (LowPassMax - LowPassMin);
			AudioSource.maxDistance = Mathf.Lerp(Falloff * FalloffMultiplier, Falloff, num2);
			AudioSource.volume = Mathf.Lerp(Volume * VolumeMultiplier, Volume, num2);
		}
		First = false;
	}

	private void OnEnable()
	{
		if (!Fetch)
		{
			CheckStart();
		}
	}

	private void OnDisable()
	{
		LogicActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private void CheckStart()
	{
		if (!LogicActive)
		{
			First = true;
			if (((Component)this).gameObject.activeSelf)
			{
				((MonoBehaviour)this).StartCoroutine(Check());
			}
		}
	}

	private IEnumerator Check()
	{
		LogicActive = true;
		while (Object.op_Implicit((Object)(object)AudioSource) && (AlwaysActive || !AudioSource.loop || AudioSource.isPlaying || First) && !Fetch)
		{
			if (!Object.op_Implicit((Object)(object)audioListener))
			{
				if (!Object.op_Implicit((Object)(object)AudioListenerFollow.instance))
				{
					yield return null;
					continue;
				}
				audioListener = ((Component)AudioListenerFollow.instance).transform;
			}
			CheckLogic();
			yield return (object)new WaitForSeconds(0.25f);
		}
		LogicActive = false;
		First = true;
	}

	private void CheckLogic()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		LowPass = true;
		bool flag = Object.op_Implicit((Object)(object)SpectateCamera.instance);
		if (!Object.op_Implicit((Object)(object)audioListener) || !Object.op_Implicit((Object)(object)AudioSource) || AudioSource.spatialBlend <= 0f || (flag && SpectateCamera.instance.CheckState(SpectateCamera.State.Death)))
		{
			LowPass = false;
		}
		else
		{
			Vector3 val = audioListener.position - ((Component)this).transform.position;
			if (((Vector3)(ref val)).magnitude < 20f)
			{
				LowPass = false;
				Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, 0.1f, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)2);
				List<Collider> list = new List<Collider>();
				Collider[] array2 = array;
				foreach (Collider val2 in array2)
				{
					if (((Component)((Component)val2).transform).CompareTag("Wall"))
					{
						list.Add(val2);
					}
				}
				RaycastHit[] array3 = Physics.RaycastAll(((Component)this).transform.position, val, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)2);
				for (int i = 0; i < array3.Length; i++)
				{
					RaycastHit val3 = array3[i];
					if (!((Component)((Component)((RaycastHit)(ref val3)).collider).transform).CompareTag("Wall"))
					{
						continue;
					}
					bool flag2 = true;
					foreach (Collider item in list)
					{
						if ((Object)(object)((Component)item).transform == (Object)(object)((Component)((RaycastHit)(ref val3)).collider).transform)
						{
							flag2 = false;
							break;
						}
					}
					if (!flag2)
					{
						continue;
					}
					bool flag3 = false;
					if (LowPassIgnoreColliders.Count > 0)
					{
						foreach (Collider lowPassIgnoreCollider in LowPassIgnoreColliders)
						{
							if (Object.op_Implicit((Object)(object)lowPassIgnoreCollider) && (Object)(object)((Component)lowPassIgnoreCollider).transform == (Object)(object)((Component)((RaycastHit)(ref val3)).collider).transform)
							{
								flag3 = true;
								break;
							}
						}
					}
					if (!flag3)
					{
						LowPass = true;
						break;
					}
				}
			}
		}
		if (!First)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)AudioSource))
		{
			if (LowPass)
			{
				AudioLowpassFilter.cutoffFrequency = LowPassMin;
				AudioSource.maxDistance = Falloff * FalloffMultiplier;
				AudioSource.volume = Volume * VolumeMultiplier;
			}
			else
			{
				AudioLowpassFilter.cutoffFrequency = LowPassMax;
				AudioSource.maxDistance = Falloff;
				AudioSource.volume = Volume;
			}
		}
		First = false;
	}
}
