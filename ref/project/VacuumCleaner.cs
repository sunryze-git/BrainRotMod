using UnityEngine;

public class VacuumCleaner : MonoBehaviour
{
	public bool DebugNoSuck;

	[Space]
	public float SuckingTime;

	private float SuckingTimer;

	private bool Sucking;

	[Space]
	public ToolActiveOffset SuckingOffset;

	public VacuumCleanerBag VacuumCleanerBag;

	public ParticleSystem ParticleSystem;

	public Transform FollowTransform;

	public Transform ParentTransform;

	[Space]
	public AnimNoise SuckNoise;

	public float SuckNoiseAmount;

	[Space]
	public Sound IntroSound;

	public Sound OutroSound;

	public Sound LoopSound;

	public Sound LoopSuckSound;

	private float LoopSuckSoundTimer;

	public Sound LoopStartSound;

	public Sound LoopStopSound;

	private bool OutroAudioPlay = true;

	private void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		IntroSound.Play(FollowTransform.position);
		GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		SuckingTimer = 1f;
	}

	private void Update()
	{
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		if (ToolController.instance.Interact)
		{
			Interaction activeInteraction = ToolController.instance.ActiveInteraction;
			if (Object.op_Implicit((Object)(object)activeInteraction))
			{
				VacuumSpotInteraction component = ((Component)activeInteraction).GetComponent<VacuumSpotInteraction>();
				if (Object.op_Implicit((Object)(object)component) && !DebugNoSuck)
				{
					component.VacuumSpot.cleanInput = true;
					LoopSuckSoundTimer = 0.1f;
					if (component.VacuumSpot.Amount > 0.2f)
					{
						SuckingTimer = Mathf.Max(SuckingTimer, SuckingTime);
					}
					else if (!component.VacuumSpot.CleanDone)
					{
						component.VacuumSpot.CleanDone = true;
					}
				}
			}
		}
		if (LoopSuckSoundTimer > 0f)
		{
			LoopSuckSoundTimer -= Time.deltaTime;
			LoopSuckSound.PlayLoop(playing: true, 5f, 5f);
		}
		else
		{
			LoopSuckSound.PlayLoop(playing: false, 5f, 5f);
		}
		if (SuckingTimer > 0f)
		{
			SuckingTimer -= Time.deltaTime;
			if (!Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				Sucking = true;
				VacuumCleanerBag.Active = true;
				SuckingOffset.Active = true;
				if (OutroAudioPlay)
				{
					ParticleSystem.Play();
				}
				LoopStartSound.Play(FollowTransform.position);
			}
			GameDirector.instance.CameraShake.Shake(1f, 0.25f);
			SuckNoise.noiseStrengthDefault = Mathf.Lerp(SuckNoise.noiseStrengthDefault, SuckNoiseAmount, 5f * Time.deltaTime);
		}
		else
		{
			if (Sucking)
			{
				GameDirector.instance.CameraShake.Shake(3f, 0.25f);
				LoopStopSound.Play(FollowTransform.position);
				VacuumCleanerBag.Active = false;
				SuckingOffset.Active = false;
				if (OutroAudioPlay)
				{
					ParticleSystem.Stop();
				}
				Sucking = false;
			}
			SuckNoise.noiseStrengthDefault = Mathf.Lerp(SuckNoise.noiseStrengthDefault, 0f, 5f * Time.deltaTime);
		}
		LoopSound.PlayLoop(Sucking, 0.5f, 0.5f);
		if (OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			if ((Object)(object)ParticleSystem != (Object)null && ParticleSystem.isPlaying)
			{
				((Component)ParticleSystem).gameObject.transform.parent = null;
				MainModule main = ParticleSystem.main;
				((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
				ParticleSystem.Stop();
				ParticleSystem = null;
			}
			OutroSound.Play(FollowTransform.position);
			OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(3f, 0.25f);
		}
		FollowTransform.position = ((Component)ToolController.instance.ToolFollow).transform.position;
		FollowTransform.rotation = ((Component)ToolController.instance.ToolFollow).transform.rotation;
		FollowTransform.localScale = ((Component)ToolController.instance.ToolHide).transform.localScale;
		((Component)ParentTransform).transform.position = ((Component)ToolController.instance.ToolTargetParent).transform.position;
		((Component)ParentTransform).transform.rotation = ((Component)ToolController.instance.ToolTargetParent).transform.rotation;
	}
}
