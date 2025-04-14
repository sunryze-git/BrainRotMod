using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class ValuableWizardTimeGlass : MonoBehaviour
{
	public enum States
	{
		Idle,
		Active
	}

	private PhysGrabObject physGrabObject;

	private PhotonView photonView;

	internal States currentState;

	private bool stateStart;

	public Transform particleSystemTransform;

	public ParticleSystem particleSystemSwirl;

	public ParticleSystem particleSystemGlitter;

	public MeshRenderer timeGlassMaterial;

	[FormerlySerializedAs("light")]
	public Light timeGlassLight;

	public Sound soundTimeGlassLoop;

	private float soundPitchLerp;

	private int particleFocus;

	private void StateActive()
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			particleSystemGlitter.Play();
			particleSystemSwirl.Play();
			stateStart = false;
			((Component)timeGlassLight).gameObject.SetActive(true);
		}
		if (!((Component)timeGlassLight).gameObject.activeSelf)
		{
			((Component)timeGlassLight).gameObject.SetActive(true);
		}
		if (((Component)particleSystemTransform).gameObject.activeSelf)
		{
			List<PhysGrabber> playerGrabbing = physGrabObject.playerGrabbing;
			if (playerGrabbing.Count > particleFocus)
			{
				PhysGrabber physGrabber = playerGrabbing[particleFocus];
				if (Object.op_Implicit((Object)(object)physGrabber))
				{
					Transform headLookAtTransform = physGrabber.playerAvatar.playerAvatarVisuals.headLookAtTransform;
					if (Object.op_Implicit((Object)(object)headLookAtTransform))
					{
						particleSystemTransform.LookAt(headLookAtTransform);
					}
					particleFocus++;
				}
				else
				{
					particleFocus = 0;
				}
			}
			else
			{
				particleFocus = 0;
			}
		}
		soundPitchLerp = Mathf.Lerp(soundPitchLerp, 1f, Time.deltaTime * 2f);
		timeGlassLight.intensity = Mathf.Lerp(timeGlassLight.intensity, 4f, Time.deltaTime * 2f);
		Color val = default(Color);
		((Color)(ref val))._002Ector(0.5f, 0f, 1f);
		((Renderer)timeGlassMaterial).material.SetColor("_EmissionColor", val * timeGlassLight.intensity);
		foreach (PhysGrabber item in physGrabObject.playerGrabbing)
		{
			if (Object.op_Implicit((Object)(object)item) && !item.isLocal)
			{
				item.playerAvatar.voiceChat.OverridePitch(0.65f, 1f, 2f);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			physGrabObject.OverrideDrag(20f);
			physGrabObject.OverrideAngularDrag(40f);
			if (!physGrabObject.grabbed)
			{
				SetState(States.Idle);
			}
		}
		if (physGrabObject.grabbedLocal)
		{
			PlayerAvatar instance = PlayerAvatar.instance;
			if (Object.op_Implicit((Object)(object)instance.voiceChat))
			{
				instance.voiceChat.OverridePitch(0.65f, 1f, 2f);
			}
			instance.OverridePupilSize(3f, 4, 1f, 1f, 5f, 0.5f);
			PlayerController.instance.OverrideSpeed(0.5f);
			PlayerController.instance.OverrideLookSpeed(0.5f, 2f, 1f);
			PlayerController.instance.OverrideAnimationSpeed(0.2f, 1f, 2f);
			PlayerController.instance.OverrideTimeScale(0.1f);
			physGrabObject.OverrideTorqueStrength(0.6f);
			CameraZoom.Instance.OverrideZoomSet(50f, 0.1f, 0.5f, 1f, ((Component)this).gameObject, 0);
			PostProcessing.Instance.SaturationOverride(50f, 0.1f, 0.5f, 0.1f, ((Component)this).gameObject);
		}
	}

	private void StateIdle()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			particleSystemGlitter.Stop();
			particleSystemSwirl.Stop();
			stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && physGrabObject.grabbed)
		{
			SetState(States.Active);
		}
		timeGlassLight.intensity = Mathf.Lerp(timeGlassLight.intensity, 0f, Time.deltaTime * 10f);
		soundPitchLerp = Mathf.Lerp(soundPitchLerp, 0f, Time.deltaTime * 10f);
		Color val = default(Color);
		((Color)(ref val))._002Ector(0.5f, 0f, 1f);
		((Renderer)timeGlassMaterial).material.SetColor("_EmissionColor", val * timeGlassLight.intensity);
		if (timeGlassLight.intensity < 0.01f)
		{
			((Component)timeGlassLight).gameObject.SetActive(false);
		}
	}

	[PunRPC]
	public void SetStateRPC(States state)
	{
		currentState = state;
		stateStart = true;
	}

	private void SetState(States state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SetStateRPC(state);
				return;
			}
			photonView.RPC("SetStateRPC", (RpcTarget)0, new object[1] { state });
		}
	}

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		float pitchMultiplier = Mathf.Lerp(2f, 0.5f, soundPitchLerp);
		soundTimeGlassLoop.PlayLoop(currentState == States.Active, 0.8f, 0.8f, pitchMultiplier);
		switch (currentState)
		{
		case States.Active:
			StateActive();
			break;
		case States.Idle:
			StateIdle();
			break;
		}
	}
}
