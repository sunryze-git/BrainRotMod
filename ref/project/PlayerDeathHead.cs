using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerDeathHead : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	public MeshRenderer headRenderer;

	public ParticleSystem smokeParticles;

	public MapCustom mapCustom;

	public GameObject arenaCrown;

	private float smokeParticleTime = 3f;

	private float smokeParticleTimer;

	private float smokeParticleRateOverTimeDefault;

	private float smokeParticleRateOverTimeCurrent;

	private float smokeParticleRateOverDistanceDefault;

	private float smokeParticleRateOverDistanceCurrent;

	internal PhysGrabObject physGrabObject;

	private PhotonView photonView;

	private RoomVolumeCheck roomVolumeCheck;

	private bool setup;

	private bool triggered;

	private float triggeredTimer;

	internal bool inExtractionPoint;

	private bool inExtractionPointPrevious;

	internal bool inTruck;

	private bool inTruckPrevious;

	[Space]
	public MeshRenderer[] eyeRenderers;

	public Light eyeFlashLight;

	public Color eyeFlashPositiveColor;

	public Color eyeFlashNegativeColor;

	public float eyeFlashStrength;

	public float eyeFlashLightIntensity;

	public Sound eyeFlashPositiveSound;

	public Sound eyeFlashNegativeSound;

	private Material eyeMaterial;

	private int eyeMaterialAmount;

	private int eyeMaterialColor;

	private AnimationCurve eyeFlashCurve;

	private float eyeFlashLerp;

	private bool eyeFlash;

	public AudioClip seenSound;

	private bool serverSeen;

	private float seenCooldownTime = 2f;

	private float seenCooldownTimer;

	private bool localSeen;

	private bool localSeenEffect;

	private float localSeenEffectTime = 2f;

	private float localSeenEffectTimer;

	private float outsideLevelTimer;

	private bool tutorialPossible;

	private float tutorialTimer;

	private float inTruckReviveTimer;

	private Collider[] colliders;

	private void Start()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		roomVolumeCheck = ((Component)this).GetComponent<RoomVolumeCheck>();
		EmissionModule emission = smokeParticles.emission;
		MinMaxCurve val = ((EmissionModule)(ref emission)).rateOverTime;
		smokeParticleRateOverTimeDefault = ((MinMaxCurve)(ref val)).constant;
		emission = smokeParticles.emission;
		val = ((EmissionModule)(ref emission)).rateOverDistance;
		smokeParticleRateOverDistanceDefault = ((MinMaxCurve)(ref val)).constant;
		localSeenEffectTimer = localSeenEffectTime;
		MeshRenderer[] array = eyeRenderers;
		foreach (MeshRenderer val2 in array)
		{
			if (!Object.op_Implicit((Object)(object)eyeMaterial))
			{
				eyeMaterial = ((Renderer)val2).material;
			}
			((Renderer)val2).material = eyeMaterial;
		}
		eyeMaterialAmount = Shader.PropertyToID("_ColorOverlayAmount");
		eyeMaterialColor = Shader.PropertyToID("_ColorOverlay");
		eyeFlashCurve = AssetManager.instance.animationCurveImpact;
		smokeParticleTimer = smokeParticleTime;
		physGrabObject.impactDetector.destroyDisableTeleport = false;
		colliders = ((Component)this).GetComponentsInChildren<Collider>();
		SetColliders(_enabled: false);
		((MonoBehaviour)this).StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (GameManager.Multiplayer())
			{
				photonView.RPC("SetupRPC", (RpcTarget)4, new object[1] { playerAvatar.playerName });
			}
			SetupDone();
			physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.RunIsArena())
			{
				physGrabObject.impactDetector.destroyDisable = false;
			}
			setup = true;
		}
	}

	private IEnumerator SetupClient()
	{
		while (!Object.op_Implicit((Object)(object)physGrabObject))
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		while (!Object.op_Implicit((Object)(object)physGrabObject.impactDetector))
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		while (!Object.op_Implicit((Object)(object)physGrabObject.impactDetector.particles))
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		SetupDone();
	}

	private void SetupDone()
	{
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			Debug.LogError((object)"PlayerDeathHead: PlayerAvatar not found", (Object)(object)((Component)this).gameObject);
			return;
		}
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1) && !playerAvatar.isLocal)
		{
			tutorialPossible = true;
		}
		((Component)this).transform.parent = ((Component)playerAvatar).transform.parent;
		if (SemiFunc.IsMultiplayer() && (Object)(object)playerAvatar == (Object)(object)SessionManager.instance.CrownedPlayerGet())
		{
			arenaCrown.SetActive(true);
		}
	}

	private void Update()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		if (!serverSeen)
		{
			mapCustom.Hide();
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && setup)
		{
			if (!triggered)
			{
				physGrabObject.OverrideDeactivate();
			}
			else if (triggeredTimer > 0f)
			{
				physGrabObject.OverrideDeactivate();
				triggeredTimer -= Time.deltaTime;
				if (triggeredTimer <= 0f)
				{
					physGrabObject.OverrideDeactivateReset();
					physGrabObject.rb.AddForce(playerAvatar.localCameraTransform.up * 2f, (ForceMode)1);
					physGrabObject.rb.AddForce(((Component)physGrabObject).transform.forward * 0.5f, (ForceMode)1);
					physGrabObject.rb.AddTorque(((Component)physGrabObject).transform.right * 0.2f, (ForceMode)1);
				}
			}
		}
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (triggered)
			{
				inExtractionPoint = roomVolumeCheck.inExtractionPoint;
				if (inExtractionPoint != inExtractionPointPrevious)
				{
					if (GameManager.Multiplayer())
					{
						photonView.RPC("FlashEyeRPC", (RpcTarget)0, new object[1] { inExtractionPoint });
					}
					else
					{
						FlashEyeRPC(inExtractionPoint);
					}
					inExtractionPointPrevious = inExtractionPoint;
				}
			}
			else
			{
				inExtractionPoint = false;
				inExtractionPointPrevious = false;
			}
		}
		if (smokeParticles.isPlaying)
		{
			smokeParticleTimer -= Time.deltaTime;
			if (smokeParticleTimer <= 0f)
			{
				smokeParticleRateOverTimeCurrent -= 1f * Time.deltaTime;
				smokeParticleRateOverTimeCurrent = Mathf.Max(smokeParticleRateOverTimeCurrent, 0f);
				smokeParticleRateOverDistanceCurrent -= 10f * Time.deltaTime;
				smokeParticleRateOverDistanceCurrent = Mathf.Max(smokeParticleRateOverDistanceCurrent, 0f);
				EmissionModule emission = smokeParticles.emission;
				((EmissionModule)(ref emission)).rateOverTime = new MinMaxCurve(smokeParticleRateOverTimeCurrent);
				((EmissionModule)(ref emission)).rateOverDistance = new MinMaxCurve(smokeParticleRateOverDistanceCurrent);
				if (smokeParticleRateOverTimeCurrent <= 0f && smokeParticleRateOverDistanceCurrent <= 0f)
				{
					smokeParticles.Stop();
				}
			}
		}
		if (eyeFlash)
		{
			eyeFlashLerp += 2f * Time.deltaTime;
			eyeFlashLerp = Mathf.Clamp01(eyeFlashLerp);
			eyeMaterial.SetFloat(eyeMaterialAmount, eyeFlashCurve.Evaluate(eyeFlashLerp));
			eyeFlashLight.intensity = eyeFlashCurve.Evaluate(eyeFlashLerp) * eyeFlashLightIntensity;
			if (eyeFlashLerp > 1f)
			{
				eyeFlash = false;
				eyeMaterial.SetFloat(eyeMaterialAmount, 0f);
				((Component)eyeFlashLight).gameObject.SetActive(false);
			}
		}
		if (triggered && !localSeen && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			if (seenCooldownTimer > 0f)
			{
				seenCooldownTimer -= Time.deltaTime;
			}
			else
			{
				Vector3 localCameraPosition = PlayerController.instance.playerAvatarScript.localCameraPosition;
				float num = Vector3.Distance(((Component)this).transform.position, localCameraPosition);
				if (num <= 10f && SemiFunc.OnScreen(((Component)this).transform.position, -0.15f, -0.15f))
				{
					Vector3 val = localCameraPosition - ((Component)this).transform.position;
					Vector3 normalized = ((Vector3)(ref val)).normalized;
					RaycastHit val2 = default(RaycastHit);
					if (!Physics.Raycast(physGrabObject.centerPoint, normalized, ref val2, num, LayerMask.GetMask(new string[1] { "Default" })))
					{
						localSeen = true;
						TutorialDirector.instance.playerSawHead = true;
						if (!serverSeen && SemiFunc.RunIsLevel())
						{
							if (SemiFunc.IsMultiplayer())
							{
								photonView.RPC("SeenSetRPC", (RpcTarget)0, new object[1] { true });
							}
							else
							{
								SeenSetRPC(_toggle: true);
							}
							if (PlayerController.instance.deathSeenTimer <= 0f)
							{
								localSeenEffect = true;
								PlayerController.instance.deathSeenTimer = 30f;
								GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
								GameDirector.instance.CameraShake.Shake(2f, 1f);
								AudioScare.instance.PlayCustom(seenSound, 0.3f, 60f);
								ValuableDiscover.instance.New(physGrabObject, ValuableDiscoverGraphic.State.Bad);
							}
						}
					}
				}
			}
		}
		if (localSeenEffect)
		{
			localSeenEffectTimer -= Time.deltaTime;
			CameraZoom.Instance.OverrideZoomSet(75f, 0.1f, 0.25f, 0.25f, ((Component)this).gameObject, 150);
			PostProcessing.Instance.VignetteOverride(Color.black, 0.4f, 1f, 1f, 0.5f, 0.1f, ((Component)this).gameObject);
			PostProcessing.Instance.SaturationOverride(-50f, 1f, 0.5f, 0.1f, ((Component)this).gameObject);
			PostProcessing.Instance.ContrastOverride(5f, 1f, 0.5f, 0.1f, ((Component)this).gameObject);
			GameDirector.instance.CameraImpact.Shake(10f * Time.deltaTime, 0.1f);
			GameDirector.instance.CameraShake.Shake(10f * Time.deltaTime, 1f);
			if (localSeenEffectTimer <= 0f)
			{
				localSeenEffect = false;
			}
		}
		if (triggered && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (roomVolumeCheck.CurrentRooms.Count <= 0)
			{
				outsideLevelTimer += Time.deltaTime;
				if (outsideLevelTimer >= 5f)
				{
					if (RoundDirector.instance.extractionPointActive)
					{
						physGrabObject.Teleport(RoundDirector.instance.extractionPointCurrent.safetySpawn.position, RoundDirector.instance.extractionPointCurrent.safetySpawn.rotation);
					}
					else
					{
						physGrabObject.Teleport(((Component)TruckSafetySpawnPoint.instance).transform.position, ((Component)TruckSafetySpawnPoint.instance).transform.rotation);
					}
				}
			}
			else
			{
				outsideLevelTimer = 0f;
			}
		}
		if (tutorialPossible)
		{
			if (triggered && localSeen)
			{
				tutorialTimer -= Time.deltaTime;
				if (tutorialTimer <= 0f)
				{
					if (!RoundDirector.instance.allExtractionPointsCompleted && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 1))
					{
						TutorialDirector.instance.ActivateTip("Reviving", 0.5f, _interrupt: false);
					}
					tutorialPossible = false;
				}
			}
			else
			{
				tutorialTimer = 5f;
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (RoundDirector.instance.allExtractionPointsCompleted && triggered && !playerAvatar.finalHeal)
		{
			inTruck = roomVolumeCheck.inTruck;
			if (inTruck != inTruckPrevious)
			{
				if (GameManager.Multiplayer())
				{
					photonView.RPC("FlashEyeRPC", (RpcTarget)0, new object[1] { inTruck });
				}
				else
				{
					FlashEyeRPC(inTruck);
				}
				inTruckPrevious = inTruck;
			}
		}
		else
		{
			inTruck = false;
			inTruckPrevious = false;
		}
		if (inTruck)
		{
			inTruckReviveTimer -= Time.deltaTime;
			if (inTruckReviveTimer <= 0f)
			{
				playerAvatar.Revive(_revivedByTruck: true);
			}
		}
		else
		{
			inTruckReviveTimer = 2f;
		}
	}

	private void UpdateColor()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)headRenderer))
		{
			((Renderer)headRenderer).material = playerAvatar.playerHealth.bodyMaterial;
			((Renderer)headRenderer).material.SetFloat(Shader.PropertyToID("_ColorOverlayAmount"), 0f);
			Color color = playerAvatar.playerAvatarVisuals.color;
			PhysObjectParticles particles = physGrabObject.impactDetector.particles;
			Gradient val = new Gradient();
			val.colorKeys = (GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(color, 0f),
				new GradientColorKey(color, 1f)
			};
			particles.gradient = val;
		}
	}

	public void Revive()
	{
		if (triggered && inExtractionPoint)
		{
			playerAvatar.Revive();
		}
	}

	public void Trigger()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		seenCooldownTimer = seenCooldownTime;
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (playerAvatar.isLocal)
			{
				PlayerController.instance.col.enabled = false;
			}
			else
			{
				((Collider)playerAvatar.playerAvatarCollision.Collider).enabled = false;
			}
			Collider[] array = playerAvatar.tumble.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			physGrabObject.Teleport(playerAvatar.playerAvatarCollision.deathHeadPosition, playerAvatar.localCameraTransform.rotation);
			triggeredTimer = 0.1f;
		}
		UpdateColor();
		triggered = true;
		SetColliders(_enabled: true);
		if (Object.op_Implicit((Object)(object)smokeParticles))
		{
			smokeParticles.Play();
		}
		smokeParticleRateOverTimeCurrent = smokeParticleRateOverTimeDefault;
		smokeParticleRateOverDistanceCurrent = smokeParticleRateOverDistanceDefault;
	}

	public void Reset()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		triggered = false;
		smokeParticleTimer = smokeParticleTime;
		localSeenEffectTimer = localSeenEffectTime;
		localSeen = false;
		localSeenEffect = false;
		SetColliders(_enabled: false);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			physGrabObject.Teleport(new Vector3(0f, 3000f, 0f), Quaternion.identity);
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("SeenSetRPC", (RpcTarget)0, new object[1] { false });
			}
			else
			{
				SeenSetRPC(_toggle: false);
			}
		}
	}

	private void SetColliders(bool _enabled)
	{
		Collider[] array = colliders;
		foreach (Collider val in array)
		{
			if (Object.op_Implicit((Object)(object)val))
			{
				val.enabled = _enabled;
			}
		}
	}

	[PunRPC]
	public void SetupRPC(string _playerName)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.playerName == _playerName)
			{
				playerAvatar = player;
				playerAvatar.playerDeathHead = this;
				break;
			}
		}
		((MonoBehaviour)this).StartCoroutine(SetupClient());
	}

	[PunRPC]
	public void FlashEyeRPC(bool _positive)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		inExtractionPoint = _positive;
		if (_positive)
		{
			eyeMaterial.SetColor(eyeMaterialColor, eyeFlashPositiveColor);
			eyeFlashPositiveSound.Play(((Component)this).transform.position);
			eyeFlashLight.color = eyeFlashPositiveColor;
		}
		else
		{
			eyeMaterial.SetColor(eyeMaterialColor, eyeFlashNegativeColor);
			eyeFlashNegativeSound.Play(((Component)this).transform.position);
			eyeFlashLight.color = eyeFlashNegativeColor;
		}
		eyeFlash = true;
		eyeFlashLerp = 0f;
		((Component)eyeFlashLight).gameObject.SetActive(true);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 2f, 8f, ((Component)this).transform.position, 0.25f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 2f, 8f, ((Component)this).transform.position, 0.5f);
	}

	[PunRPC]
	public void SeenSetRPC(bool _toggle)
	{
		serverSeen = _toggle;
	}
}
