using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhysGrabber : MonoBehaviour, IPunObservable
{
	private enum ColorState
	{
		Orange,
		Green,
		Purple
	}

	private Camera playerCamera;

	[HideInInspector]
	public float grabRange = 4f;

	[HideInInspector]
	public float grabReleaseDistance = 8f;

	public static PhysGrabber instance;

	[Space]
	[HideInInspector]
	public float minDistanceFromPlayer = 1f;

	[HideInInspector]
	public float maxDistanceFromPlayer = 2.5f;

	[Space]
	public PhysGrabBeam physGrabBeamComponent;

	public GameObject physGrabBeam;

	public Transform physGrabPoint;

	public Transform physGrabPointPuller;

	public Transform physGrabPointPlane;

	private GameObject physGrabPointVisual1;

	private GameObject physGrabPointVisual2;

	internal Vector3 grabbedcObjectPrevCamRelForward;

	internal Vector3 grabbedObjectPrevCamRelUp;

	internal PhysGrabObject grabbedPhysGrabObject;

	internal int grabbedPhysGrabObjectColliderID;

	internal Collider grabbedPhysGrabObjectCollider;

	internal StaticGrabObject grabbedStaticGrabObject;

	internal Rigidbody grabbedObject;

	[HideInInspector]
	public Transform grabbedObjectTransform;

	[HideInInspector]
	public float physGrabPointPullerDampen = 80f;

	[HideInInspector]
	public float springConstant = 0.9f;

	[HideInInspector]
	public float dampingConstant = 0.5f;

	[HideInInspector]
	public float forceConstant = 4f;

	[HideInInspector]
	public float forceMax = 4f;

	private bool physGrabBeamActive;

	[HideInInspector]
	public PhotonView photonView;

	[HideInInspector]
	public bool isLocal;

	[HideInInspector]
	public bool grabbed;

	internal float grabDisableTimer;

	[HideInInspector]
	public Vector3 physGrabPointPosition;

	[HideInInspector]
	public Vector3 physGrabPointPullerPosition;

	[HideInInspector]
	public PlayerAvatar playerAvatar;

	[HideInInspector]
	public Vector3 localGrabPosition;

	[HideInInspector]
	public Vector3 cameraRelativeGrabbedForward;

	[HideInInspector]
	public Vector3 cameraRelativeGrabbedUp;

	[HideInInspector]
	public Vector3 cameraRelativeGrabbedRight;

	private Transform physGrabPointVisualRotate;

	[HideInInspector]
	public Transform physGrabPointVisualGrid;

	[HideInInspector]
	public GameObject physGrabPointVisualGridObject;

	private List<GameObject> physGrabPointVisualGridObjects = new List<GameObject>();

	private int prevColorState = -1;

	[HideInInspector]
	public int colorState;

	private float colorStateOverrideTimer;

	[Space]
	public LayerMask maskLayers;

	internal bool healing;

	internal ItemAttributes currentlyLookingAtItemAttributes;

	internal PhysGrabObject currentlyLookingAtPhysGrabObject;

	internal StaticGrabObject currentlyLookingAtStaticGrabObject;

	[Space]
	public Material physGrabBeamMaterial;

	public Material physGrabBeamMaterialBatteryCharge;

	[HideInInspector]
	public bool physGrabForcesDisabled;

	[HideInInspector]
	public float initialPressTimer;

	private bool overrideGrab;

	private bool overrideGrabRelease;

	private PhysGrabObject overrideGrabTarget;

	private float physGrabBeamAlpha = 1f;

	private float physGrabBeamAlphaChangeTo = 1f;

	private float physGramBeamAlphaTimer;

	private float physGrabBeamAlphaChangeProgress;

	private float physGrabBeamAlphaOriginal;

	private float overrideGrabDistance;

	private float overrideGrabDistanceTimer;

	private float overrideDisableRotationControlsTimer;

	private bool overrideDisableRotationControls;

	private LayerMask mask;

	private float grabCheckTimer;

	internal float pullerDistance;

	[Space]
	public Transform grabberAudioTransform;

	public Sound startSound;

	public Sound loopSound;

	public Sound stopSound;

	private float physRotatingTimer;

	internal Quaternion physRotation;

	private Quaternion physRotationBase;

	[HideInInspector]
	public Vector3 mouseTurningVelocity;

	[HideInInspector]
	public float grabStrength = 1f;

	[HideInInspector]
	public float throwStrength;

	internal bool debugStickyGrabber;

	[HideInInspector]
	public float stopRotationTimer;

	[HideInInspector]
	public Quaternion nextPhysRotation;

	[HideInInspector]
	public bool isRotating;

	private float isRotatingTimer;

	internal bool isPushing;

	internal bool isPulling;

	private float isPushingTimer;

	private float isPullingTimer;

	private float prevPullerDistance;

	private bool prevGrabbed;

	private bool toggleGrab;

	private float toggleGrabTimer;

	private float overrideGrabPointTimer;

	private Transform overrideGrabPointTransform;

	private void Start()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		((MonoBehaviour)this).StartCoroutine(LateStart());
		physRotation = Quaternion.identity;
		physRotationBase = Quaternion.identity;
		mask = LayerMask.op_Implicit(LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) - LayerMask.GetMask(new string[1] { "Player" }));
		playerAvatar = ((Component)this).GetComponent<PlayerAvatar>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || photonView.IsMine)
		{
			isLocal = true;
			instance = this;
		}
		foreach (Transform item in physGrabPoint)
		{
			Transform val = item;
			if (((Object)val).name == "Visual1")
			{
				physGrabPointVisual1 = ((Component)val).gameObject;
				foreach (Transform item2 in val)
				{
					Transform val2 = item2;
					if (((Object)val2).name == "Visual2")
					{
						physGrabPointVisual2 = ((Component)val2).gameObject;
					}
				}
			}
			if (((Object)val).name == "Rotate")
			{
				physGrabPointVisualRotate = val;
				((Component)val).GetComponent<PhysGrabPointRotate>().physGrabber = this;
			}
			if (!(((Object)val).name == "Grid"))
			{
				continue;
			}
			physGrabPointVisualGrid = val;
			foreach (Transform item3 in val)
			{
				Transform val3 = item3;
				physGrabPointVisualGridObject = ((Component)val3).gameObject;
				physGrabPointVisualGridObject.SetActive(false);
			}
		}
		physGrabPoint.SetParent((Transform)null, true);
		PhysGrabPointDeactivate();
		((Component)physGrabPointPuller).gameObject.SetActive(false);
		physGrabBeam.transform.SetParent((Transform)null, false);
		physGrabBeam.transform.position = Vector3.zero;
		physGrabBeam.transform.rotation = Quaternion.identity;
		physGrabBeam.SetActive(false);
		physGrabBeamAlphaOriginal = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material.color.a;
		SoundSetup(startSound);
		SoundSetup(loopSound);
		SoundSetup(stopSound);
		if (isLocal)
		{
			playerCamera = Camera.main;
			PlayerController.instance.physGrabPoint = physGrabPoint;
			physGrabPointPlane.SetParent((Transform)null, false);
			physGrabPointPlane.position = Vector3.zero;
			physGrabPointPlane.rotation = Quaternion.identity;
			physGrabPointPlane.SetParent(((Component)CameraAim.Instance).transform, false);
			physGrabPointPlane.localPosition = Vector3.zero;
			physGrabPointPlane.localRotation = Quaternion.identity;
		}
	}

	private void OnDestroy()
	{
		Object.Destroy((Object)(object)physGrabBeam);
	}

	public void OverrideGrabDistance(float dist)
	{
		prevPullerDistance = pullerDistance;
		pullerDistance = dist;
		overrideGrabDistance = dist;
		overrideGrabDistanceTimer = 0.1f;
	}

	private void OverrideGrabDistanceTick()
	{
		if (overrideGrabDistanceTimer > 0f)
		{
			overrideGrabDistanceTimer -= Time.deltaTime;
		}
		else if (overrideGrabDistanceTimer != -123f)
		{
			overrideGrabDistance = 0f;
			overrideGrabDistanceTimer = -123f;
		}
	}

	private IEnumerator LateStart()
	{
		while (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		string _steamID = SemiFunc.PlayerGetSteamID(playerAvatar);
		yield return (object)new WaitForSeconds(0.2f);
		while (!StatsManager.instance.playerUpgradeStrength.ContainsKey(_steamID))
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		if (!SemiFunc.MenuLevel())
		{
			grabStrength += (float)StatsManager.instance.playerUpgradeStrength[_steamID] * 0.2f;
			throwStrength += (float)StatsManager.instance.playerUpgradeThrow[_steamID] * 0.3f;
			grabRange += (float)StatsManager.instance.playerUpgradeRange[_steamID] * 1f;
		}
	}

	public void SoundSetup(Sound _sound)
	{
		if (!SemiFunc.IsMultiplayer() || photonView.IsMine)
		{
			_sound.SpatialBlend = 0f;
			return;
		}
		_sound.Volume *= 0.5f;
		_sound.VolumeRandom *= 0.5f;
		_sound.SpatialBlend = 1f;
	}

	public void OverrideDisableRotationControls()
	{
		overrideDisableRotationControls = true;
		overrideDisableRotationControlsTimer = 0.1f;
	}

	private void OverrideDisableRotationControlsTick()
	{
		if (overrideDisableRotationControlsTimer > 0f)
		{
			overrideDisableRotationControlsTimer -= Time.fixedDeltaTime;
			if (overrideDisableRotationControlsTimer <= 0f)
			{
				overrideDisableRotationControls = false;
			}
		}
	}

	public void OverrideGrab(PhysGrabObject target)
	{
		overrideGrab = true;
		overrideGrabTarget = target;
	}

	public void OverrideGrabPoint(Transform grabPoint)
	{
		overrideGrabPointTransform = grabPoint;
		overrideGrabPointTimer = 0.1f;
	}

	public void OverrideGrabRelease()
	{
		overrideGrabRelease = true;
		overrideGrab = false;
		overrideGrabTarget = null;
	}

	public void GrabberHeal()
	{
		if (!healing)
		{
			photonView.RPC("HealStart", (RpcTarget)0, Array.Empty<object>());
		}
	}

	private void ColorStateSetColor(Color mainColor, Color emissionColor)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		Material material = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material;
		Material material2 = ((Renderer)physGrabPointVisual1.GetComponent<MeshRenderer>()).material;
		Material material3 = ((Renderer)physGrabPointVisual2.GetComponent<MeshRenderer>()).material;
		Material material4 = ((Renderer)((Component)physGrabPointVisualRotate).GetComponent<MeshRenderer>()).material;
		Light grabberLight = playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberLight;
		Material material5 = ((Renderer)playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[0].GetComponent<MeshRenderer>()).material;
		Material material6 = ((Renderer)playerAvatar.playerAvatarVisuals.playerAvatarRightArm.grabberOrbSpheres[1].GetComponent<MeshRenderer>()).material;
		if (Object.op_Implicit((Object)(object)material))
		{
			material.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material))
		{
			material.SetColor("_EmissionColor", emissionColor);
		}
		if (Object.op_Implicit((Object)(object)material2))
		{
			material2.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material2))
		{
			material2.SetColor("_EmissionColor", emissionColor);
		}
		if (Object.op_Implicit((Object)(object)material3))
		{
			material3.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material3))
		{
			material3.SetColor("_EmissionColor", emissionColor);
		}
		if (Object.op_Implicit((Object)(object)material4))
		{
			material4.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material4))
		{
			material4.SetColor("_EmissionColor", emissionColor);
		}
		if (Object.op_Implicit((Object)(object)grabberLight))
		{
			grabberLight.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material5))
		{
			material5.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material5))
		{
			material5.SetColor("_EmissionColor", emissionColor);
		}
		if (Object.op_Implicit((Object)(object)material6))
		{
			material6.color = mainColor;
		}
		if (Object.op_Implicit((Object)(object)material6))
		{
			material6.SetColor("_EmissionColor", emissionColor);
		}
	}

	public void OverrideColorToGreen(float time = 0.1f)
	{
		colorState = 1;
		colorStateOverrideTimer = time;
	}

	public void OverrideColorToPurple(float time = 0.1f)
	{
		colorState = 2;
		colorStateOverrideTimer = time;
	}

	private void ColorStates()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if (prevColorState == colorState)
		{
			return;
		}
		prevColorState = colorState;
		Color mainColor = default(Color);
		((Color)(ref mainColor))._002Ector(1f, 0.1856f, 0f, 0.15f);
		Color emissionColor = default(Color);
		((Color)(ref emissionColor))._002Ector(1f, 0.1856f, 0f, 1f);
		if (colorState == 0)
		{
			if (!Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
			{
				((Color)(ref mainColor))._002Ector(1f, 0.1856f, 0f, 0.15f);
			}
			else
			{
				((Color)(ref mainColor))._002Ector(1f, 0.1856f, 0f, 1f);
			}
			((Color)(ref emissionColor))._002Ector(1f, 0.1856f, 0f, 1f);
			ColorStateSetColor(mainColor, emissionColor);
		}
		else if (colorState == 1)
		{
			if (!Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
			{
				((Color)(ref mainColor))._002Ector(0f, 1f, 0f, 0.15f);
			}
			else
			{
				((Color)(ref mainColor))._002Ector(0f, 1f, 0f, 1f);
			}
			((Color)(ref emissionColor))._002Ector(0f, 1f, 0f, 1f);
			ColorStateSetColor(mainColor, emissionColor);
		}
		else if (colorState == 2)
		{
			if (!Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
			{
				((Color)(ref mainColor))._002Ector(1f, 0f, 1f, 0.15f);
			}
			else
			{
				((Color)(ref mainColor))._002Ector(1f, 0f, 1f, 1f);
			}
			((Color)(ref emissionColor))._002Ector(1f, 0f, 1f, 1f);
			ColorStateSetColor(mainColor, emissionColor);
		}
	}

	private void ColorStateTick()
	{
		if (colorStateOverrideTimer > 0f)
		{
			colorStateOverrideTimer -= Time.fixedDeltaTime;
		}
		else
		{
			colorState = 0;
		}
	}

	[PunRPC]
	private void HealStart()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material = physGrabBeamMaterialBatteryCharge;
		((Renderer)physGrabPointVisual1.GetComponent<MeshRenderer>()).material = physGrabBeamMaterialBatteryCharge;
		((Renderer)physGrabPointVisual2.GetComponent<MeshRenderer>()).material = physGrabBeamMaterialBatteryCharge;
		physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = new Vector2(-5f, 0f);
		physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material;
		healing = true;
	}

	private void ResetBeam()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (healing)
		{
			((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material = physGrabBeamMaterial;
			((Renderer)physGrabPointVisual1.GetComponent<MeshRenderer>()).material = physGrabBeamMaterial;
			((Renderer)physGrabPointVisual2.GetComponent<MeshRenderer>()).material = physGrabBeamMaterial;
			physGrabBeam.GetComponent<PhysGrabBeam>().scrollSpeed = physGrabBeam.GetComponent<PhysGrabBeam>().originalScrollSpeed;
			physGrabBeam.GetComponent<PhysGrabBeam>().lineMaterial = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material;
			healing = false;
		}
	}

	public void ChangeBeamAlpha(float alpha)
	{
		if (physGramBeamAlphaTimer == -123f)
		{
			physGrabBeamAlpha = physGrabBeamAlphaOriginal;
		}
		physGrabBeamAlphaChangeTo = alpha;
		physGramBeamAlphaTimer = 0.1f;
		physGrabBeamAlphaChangeProgress = 0f;
	}

	private void TickerBeamAlphaChange()
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (physGramBeamAlphaTimer > 0f)
		{
			physGrabBeamAlpha = Mathf.Lerp(physGrabBeamAlpha, physGrabBeamAlphaChangeTo, physGrabBeamAlphaChangeProgress);
			if (physGrabBeamAlphaChangeProgress < 1f)
			{
				physGrabBeamAlphaChangeProgress += 4f * Time.deltaTime;
				Material material = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material;
				material.SetColor("_Color", new Color(material.color.r, material.color.g, material.color.b, physGrabBeamAlpha));
				Material material2 = ((Renderer)physGrabPointVisual1.GetComponent<MeshRenderer>()).material;
				Material material3 = ((Renderer)physGrabPointVisual2.GetComponent<MeshRenderer>()).material;
				material2.SetColor("_Color", new Color(material2.color.r, material2.color.g, material2.color.b, physGrabBeamAlpha));
				material3.SetColor("_Color", new Color(material3.color.r, material3.color.g, material3.color.b, physGrabBeamAlpha));
			}
		}
		else if (physGramBeamAlphaTimer != -123f)
		{
			physGrabBeamAlphaChangeProgress = 0f;
			Material material4 = ((Renderer)physGrabBeam.GetComponent<LineRenderer>()).material;
			material4.SetColor("_Color", new Color(material4.color.r, material4.color.g, material4.color.b, physGrabBeamAlphaOriginal));
			Material material5 = ((Renderer)physGrabPointVisual1.GetComponent<MeshRenderer>()).material;
			Material material6 = ((Renderer)physGrabPointVisual2.GetComponent<MeshRenderer>()).material;
			material5.SetColor("_Color", new Color(material5.color.r, material5.color.g, material5.color.b, physGrabBeamAlphaOriginal));
			material6.SetColor("_Color", new Color(material6.color.r, material6.color.g, material6.color.b, physGrabBeamAlphaOriginal));
			physGramBeamAlphaTimer = -123f;
		}
		if (physGramBeamAlphaTimer > 0f)
		{
			physGramBeamAlphaTimer -= Time.deltaTime;
		}
	}

	public Quaternion GetRotationInput()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.AngleAxis(mouseTurningVelocity.y, Vector3.right);
		Quaternion val2 = Quaternion.AngleAxis(0f - mouseTurningVelocity.x, Vector3.up);
		Quaternion val3 = Quaternion.AngleAxis(mouseTurningVelocity.z, Vector3.forward);
		return val2 * val * val3;
	}

	private void ObjectTurning()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			return;
		}
		if (!grabbed)
		{
			mouseTurningVelocity = Vector3.zero;
			((Component)physGrabPointVisualGrid).gameObject.SetActive(false);
			isRotating = false;
			return;
		}
		if (Object.op_Implicit((Object)(object)physGrabPointVisualGrid) && Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			physGrabPointVisualGrid.position = grabbedPhysGrabObject.midPoint;
		}
		if (((Vector3)(ref mouseTurningVelocity)).magnitude > 0.01f)
		{
			mouseTurningVelocity = Vector3.Lerp(mouseTurningVelocity, Vector3.zero, 1f * Time.deltaTime);
		}
		else
		{
			mouseTurningVelocity = Vector3.zero;
		}
		cameraRelativeGrabbedForward = ((Vector3)(ref cameraRelativeGrabbedForward)).normalized;
		cameraRelativeGrabbedUp = ((Vector3)(ref cameraRelativeGrabbedUp)).normalized;
		bool flag = false;
		if (isLocal && SemiFunc.InputHold(InputKey.Rotate))
		{
			flag = true;
		}
		if (flag)
		{
			float axis = Input.GetAxis("Mouse X");
			float axis2 = Input.GetAxis("Mouse Y");
			Vector3 val = new Vector3(axis, axis2, 0f) * 8f * Time.deltaTime;
			mouseTurningVelocity += val;
			if (isLocal)
			{
				isRotatingTimer = 0.1f;
			}
		}
		if (isRotating)
		{
			((Component)physGrabPointVisualGrid).gameObject.SetActive(true);
			Transform localCameraTransform = playerAvatar.localCameraTransform;
			if (physRotatingTimer <= 0f)
			{
				physRotatingTimer = 0.25f;
				cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(grabbedObjectTransform.forward);
				cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(grabbedObjectTransform.up);
				physGrabPointVisualGrid.rotation = grabbedObjectTransform.rotation;
			}
			physRotatingTimer = 0.25f;
			float mass = grabbedPhysGrabObject.rb.mass;
			float num = 1f / mass;
			num = Mathf.Clamp(num, 0f, 0.5f);
			if (num != 0f)
			{
				grabbedPhysGrabObject.OverrideAngularDrag(40f * num);
			}
			Quaternion val2 = Quaternion.AngleAxis(mouseTurningVelocity.y, localCameraTransform.right);
			Quaternion val3 = Quaternion.AngleAxis(0f - mouseTurningVelocity.x, localCameraTransform.up);
			Quaternion val4 = Quaternion.AngleAxis(mouseTurningVelocity.z, localCameraTransform.forward);
			Quaternion val5 = val3 * val2 * val4;
			float fixedDeltaTime = Time.fixedDeltaTime;
			float num2 = 10000f * Time.fixedDeltaTime;
			float num3 = Quaternion.Angle(Quaternion.identity, val5);
			if (num3 > num2)
			{
				val5 = Quaternion.Slerp(Quaternion.identity, val5, num2 / num3);
			}
			val5 = Quaternion.Slerp(Quaternion.identity, val5, fixedDeltaTime * 20f);
			physGrabPointVisualGrid.rotation = val5 * physGrabPointVisualGrid.rotation;
			cameraRelativeGrabbedForward = localCameraTransform.InverseTransformDirection(grabbedObjectTransform.forward);
			cameraRelativeGrabbedUp = localCameraTransform.InverseTransformDirection(grabbedObjectTransform.up);
			foreach (PhysGrabber item in grabbedPhysGrabObject.playerGrabbing)
			{
				Transform localCameraTransform2 = item.playerAvatar.localCameraTransform;
				item.cameraRelativeGrabbedForward = localCameraTransform2.InverseTransformDirection(physGrabPointVisualGrid.forward);
				item.cameraRelativeGrabbedUp = localCameraTransform2.InverseTransformDirection(physGrabPointVisualGrid.up);
			}
			((Component)physGrabPointVisualGrid).transform.rotation = Quaternion.Slerp(((Component)physGrabPointVisualGrid).transform.rotation, grabbedObjectTransform.rotation, Time.deltaTime * 10f);
		}
		else
		{
			((Component)physGrabPointVisualGrid).gameObject.SetActive(false);
		}
	}

	private void OverrideGrabPointTimer()
	{
		if (overrideGrabPointTimer > 0f)
		{
			overrideGrabPointTimer -= Time.fixedDeltaTime;
		}
		else
		{
			overrideGrabPointTransform = null;
		}
	}

	private void FixedUpdate()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		OverrideGrabPointTimer();
		OverrideDisableRotationControlsTick();
		if (isLocal)
		{
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
			{
				_ = grabbedPhysGrabObject.isMelee;
			}
			else
				_ = 0;
			if (!overrideDisableRotationControls)
			{
				if (isRotatingTimer > 0f)
				{
					SemiFunc.CameraOverrideStopAim();
					if (!isRotating && Object.op_Implicit((Object)(object)grabbedObjectTransform))
					{
						_ = playerAvatar.localCameraTransform;
						mouseTurningVelocity = Vector3.zero;
					}
					isRotating = true;
				}
				else
				{
					isRotating = false;
				}
			}
		}
		if (stopRotationTimer > 0f)
		{
			stopRotationTimer -= Time.fixedDeltaTime;
		}
		ColorStateTick();
	}

	private void PushingPullingChecker()
	{
		if (overrideGrabDistanceTimer > 0f)
		{
			pullerDistance = overrideGrabDistance;
			prevPullerDistance = pullerDistance;
		}
		if (!grabbed)
		{
			isPushing = false;
			isPulling = false;
			isPushingTimer = 0f;
			isPullingTimer = 0f;
			prevPullerDistance = pullerDistance;
			return;
		}
		if (initialPressTimer > 0f)
		{
			prevPullerDistance = pullerDistance;
			isPushingTimer = 0f;
		}
		if (SemiFunc.InputScrollY() > 0f)
		{
			isPushingTimer = 0.1f;
		}
		if (SemiFunc.InputScrollY() < 0f)
		{
			isPullingTimer = 0.1f;
		}
		if (isPushingTimer > 0f)
		{
			isPushing = true;
			isPushingTimer -= Time.deltaTime;
		}
		else
		{
			isPushing = false;
		}
		if (isPullingTimer > 0f)
		{
			isPulling = true;
			isPullingTimer -= Time.deltaTime;
		}
		else
		{
			isPulling = false;
		}
		prevPullerDistance = pullerDistance;
		if (overrideGrabDistanceTimer > 0f)
		{
			pullerDistance = overrideGrabDistance;
			prevPullerDistance = pullerDistance;
		}
	}

	public void OverridePullDistanceIncrement(float distSpeed)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Transform obj = physGrabPointPlane;
		obj.position += ((Component)playerCamera).transform.forward * distSpeed;
	}

	private void Update()
	{
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		if (isRotatingTimer > 0f)
		{
			isRotatingTimer -= Time.deltaTime;
		}
		PushingPullingChecker();
		ColorStates();
		ObjectTurning();
		if (Object.op_Implicit((Object)(object)grabbedObjectTransform) && ((Object)grabbedObjectTransform).name == ((Object)playerAvatar.healthGrab).name)
		{
			OverrideColorToGreen();
		}
		OverrideGrabDistanceTick();
		TickerBeamAlphaChange();
		if (initialPressTimer > 0f)
		{
			initialPressTimer -= Time.deltaTime;
		}
		if (physRotatingTimer > 0f)
		{
			physRotatingTimer -= Time.deltaTime;
		}
		if (grabbed && Object.op_Implicit((Object)(object)grabbedObjectTransform))
		{
			if (!Object.op_Implicit((Object)(object)overrideGrabPointTransform))
			{
				physGrabPoint.position = grabbedObjectTransform.TransformPoint(localGrabPosition);
			}
			else
			{
				physGrabPoint.position = overrideGrabPointTransform.position;
			}
		}
		if (isLocal)
		{
			bool flag = Object.op_Implicit((Object)(object)grabbedPhysGrabObject) && grabbedPhysGrabObject.isMelee;
			if (!SemiFunc.InputHold(InputKey.Rotate))
			{
				if (InputManager.instance.KeyPullAndPush() > 0f && Vector3.Distance(physGrabPointPuller.position, ((Component)playerCamera).transform.position) < grabRange && !flag)
				{
					Transform obj = physGrabPointPlane;
					obj.position += ((Component)playerCamera).transform.forward * 0.2f;
				}
				if (InputManager.instance.KeyPullAndPush() < 0f && Vector3.Distance(physGrabPointPuller.position, ((Component)playerCamera).transform.position) > minDistanceFromPlayer && !flag)
				{
					Transform obj2 = physGrabPointPlane;
					obj2.position -= ((Component)playerCamera).transform.forward * 0.2f;
				}
			}
			if (overrideGrabDistanceTimer < 0f)
			{
				pullerDistance = Vector3.Distance(physGrabPointPuller.position, ((Component)playerCamera).transform.position);
			}
			if (overrideGrabDistance > 0f)
			{
				Transform visionTransform = playerAvatar.PlayerVisionTarget.VisionTransform;
				physGrabPointPlane.position = visionTransform.position + visionTransform.forward * overrideGrabDistance;
			}
			else
			{
				if (pullerDistance < minDistanceFromPlayer)
				{
					physGrabPointPuller.position = ((Component)playerCamera).transform.position + ((Component)playerCamera).transform.forward * minDistanceFromPlayer;
				}
				if (pullerDistance > maxDistanceFromPlayer)
				{
					physGrabPointPuller.position = ((Component)playerCamera).transform.position + ((Component)playerCamera).transform.forward * maxDistanceFromPlayer;
				}
			}
		}
		else if (overrideGrabDistanceTimer <= 0f)
		{
			pullerDistance = Vector3.Distance(physGrabPointPuller.position, playerAvatar.localCameraPosition);
		}
		grabberAudioTransform.position = physGrabBeamComponent.PhysGrabPointOrigin.position;
		loopSound.PlayLoop(physGrabBeam.gameObject.activeSelf, 10f, 10f);
		if (!isLocal)
		{
			return;
		}
		ShowValue();
		bool flag2 = SemiFunc.InputHold(InputKey.Grab) || toggleGrab;
		if (debugStickyGrabber && !SemiFunc.InputHold(InputKey.Rotate))
		{
			flag2 = true;
		}
		if (InputManager.instance.InputToggleGet(InputKey.Grab))
		{
			if (SemiFunc.InputDown(InputKey.Grab))
			{
				toggleGrab = !toggleGrab;
				if (toggleGrab)
				{
					toggleGrabTimer = 0.1f;
				}
			}
		}
		else
		{
			toggleGrab = false;
		}
		if (toggleGrabTimer > 0f)
		{
			toggleGrabTimer -= Time.deltaTime;
		}
		else if (!grabbed && toggleGrab)
		{
			toggleGrab = false;
		}
		if (overrideGrab && (SemiFunc.InputHold(InputKey.Grab) || toggleGrab))
		{
			overrideGrab = false;
			overrideGrabTarget = null;
		}
		if (overrideGrab)
		{
			flag2 = true;
		}
		if (overrideGrabRelease)
		{
			flag2 = false;
			overrideGrabRelease = false;
		}
		if (PlayerController.instance.InputDisableTimer > 0f)
		{
			flag2 = false;
		}
		bool flag3 = false;
		if (flag2 && !grabbed)
		{
			if (grabDisableTimer <= 0f)
			{
				flag3 = true;
			}
		}
		else if (!flag2 && grabbed)
		{
			ReleaseObject();
		}
		if (LevelGenerator.Instance.Generated && PlayerController.instance.InputDisableTimer <= 0f)
		{
			if (grabCheckTimer <= 0f || flag3)
			{
				grabCheckTimer = 0.02f;
				RayCheck(flag3);
			}
			else
			{
				grabCheckTimer -= Time.deltaTime;
			}
		}
		PhysGrabLogic();
		if (grabDisableTimer > 0f)
		{
			grabDisableTimer -= Time.deltaTime;
		}
	}

	private void PhysGrabLogic()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		grabReleaseDistance = Mathf.Max(grabRange * 2f, 10f);
		if (!grabbed)
		{
			return;
		}
		if (physRotatingTimer > 0f)
		{
			Aim.instance.SetState(Aim.State.Rotate);
		}
		else
		{
			Aim.instance.SetState(Aim.State.Grab);
		}
		if (Vector3.Distance(physGrabPoint.position, ((Component)playerCamera).transform.position) > grabReleaseDistance)
		{
			ReleaseObject();
			return;
		}
		if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			if (!((Behaviour)grabbedPhysGrabObject).enabled || grabbedPhysGrabObject.dead || !Object.op_Implicit((Object)(object)grabbedPhysGrabObjectCollider) || !grabbedPhysGrabObjectCollider.enabled)
			{
				ReleaseObject();
				return;
			}
		}
		else
		{
			if (!Object.op_Implicit((Object)(object)grabbedStaticGrabObject))
			{
				ReleaseObject();
				return;
			}
			if (!((Behaviour)grabbedStaticGrabObject).isActiveAndEnabled || grabbedStaticGrabObject.dead)
			{
				ReleaseObject();
				return;
			}
		}
		physGrabPointPullerPosition = physGrabPointPuller.position;
		PhysGrabStarted();
		PhysGrabBeamActivate();
	}

	private void PhysGrabBeamActivate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			if (!physGrabBeamActive)
			{
				physGrabForcesDisabled = false;
				physGrabBeam.SetActive(true);
				physGrabBeamComponent.physGrabPointPullerSmoothPosition = physGrabPoint.position;
				physGrabBeamActive = true;
				PhysGrabStartEffects();
			}
		}
		else if (!physGrabBeamActive)
		{
			photonView.RPC("PhysGrabBeamActivateRPC", (RpcTarget)0, Array.Empty<object>());
			physGrabBeamActive = true;
		}
	}

	public void ShowValue()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!grabbed || !Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			return;
		}
		ValuableObject component = ((Component)grabbedPhysGrabObject).GetComponent<ValuableObject>();
		if (Object.op_Implicit((Object)(object)component))
		{
			WorldSpaceUIValue.instance.Show(grabbedPhysGrabObject, (int)component.dollarValueCurrent, _cost: false, Vector3.zero);
		}
		else if (SemiFunc.RunIsShop())
		{
			ItemAttributes component2 = ((Component)grabbedPhysGrabObject).GetComponent<ItemAttributes>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				WorldSpaceUIValue.instance.Show(grabbedPhysGrabObject, component2.value, _cost: true, component2.costOffset);
			}
		}
	}

	private void PhysGrabStartEffects()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		startSound.Play(((Component)loopSound.Source).transform.position);
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	private void PhysGrabEndEffects()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		stopSound.Play(((Component)loopSound.Source).transform.position);
		if (!GameManager.Multiplayer() || photonView.IsMine)
		{
			GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
		}
	}

	[PunRPC]
	private void PhysGrabBeamActivateRPC()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabStartEffects();
		initialPressTimer = 0.1f;
		physGrabForcesDisabled = false;
		physGrabBeam.SetActive(true);
		physGrabBeamComponent.physGrabPointPullerSmoothPosition = physGrabPoint.position;
		physGrabBeamActive = true;
		((Component)physGrabPointVisualRotate).GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		PhysGrabPointActivate();
	}

	private void PhysGrabPointDeactivate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		physGrabPointVisualGrid.parent = physGrabPoint;
		physGrabPointVisualRotate.localScale = Vector3.zero;
		((Component)physGrabPointVisualRotate).GetComponent<PhysGrabPointRotate>().animationEval = 0f;
		GridObjectsRemove();
		((Component)physGrabPoint).gameObject.SetActive(false);
	}

	private void PhysGrabPointActivate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)grabbedObjectTransform))
		{
			physGrabPointVisualRotate.localScale = Vector3.zero;
			PhysGrabPointRotate component = ((Component)physGrabPointVisualRotate).GetComponent<PhysGrabPointRotate>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.animationEval = 0f;
				component.rotationActiveTimer = 0f;
			}
			physGrabPointVisualGrid.localPosition = Vector3.zero;
			physGrabPointVisualGrid.parent = null;
			physGrabPointVisualGrid.localScale = Vector3.one;
			grabbedPhysGrabObject = ((Component)grabbedObjectTransform).GetComponent<PhysGrabObject>();
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
			{
				physGrabPointVisualGrid.localRotation = grabbedPhysGrabObject.rb.rotation;
			}
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
			{
				GridObjectsInstantiate();
			}
			((Component)physGrabPointVisualGrid).gameObject.SetActive(false);
			((Component)physGrabPoint).gameObject.SetActive(true);
		}
	}

	[PunRPC]
	private void PhysGrabBeamDeactivateRPC()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		physGrabForcesDisabled = false;
		ResetBeam();
		physGrabBeam.SetActive(false);
		PhysGrabPointDeactivate();
		physGrabBeamActive = false;
		PhysGrabEndEffects();
		physRotation = Quaternion.identity;
	}

	private void PhysGrabBeamDeactivate()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			PhysGrabBeamDeactivateRPC();
		}
		else
		{
			photonView.RPC("PhysGrabBeamDeactivateRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)physGrabPointPullerPosition);
			stream.SendNext((object)physGrabPointPlane.position);
			stream.SendNext((object)mouseTurningVelocity);
			stream.SendNext((object)isRotating);
			stream.SendNext((object)colorState);
		}
		else
		{
			physGrabPointPullerPosition = (Vector3)stream.ReceiveNext();
			physGrabPointPuller.position = physGrabPointPullerPosition;
			physGrabPointPlane.position = (Vector3)stream.ReceiveNext();
			mouseTurningVelocity = (Vector3)stream.ReceiveNext();
			isRotating = (bool)stream.ReceiveNext();
			colorState = (int)stream.ReceiveNext();
		}
	}

	private void PhysGrabStarted()
	{
		if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			grabbedPhysGrabObject.GrabStarted(this);
		}
		else if (Object.op_Implicit((Object)(object)grabbedStaticGrabObject))
		{
			grabbedStaticGrabObject.GrabStarted(this);
		}
		else
		{
			ReleaseObject();
		}
	}

	private void PhysGrabEnded()
	{
		if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			grabbedPhysGrabObject.GrabEnded(this);
		}
		else if (Object.op_Implicit((Object)(object)grabbedStaticGrabObject))
		{
			grabbedStaticGrabObject.GrabEnded(this);
		}
	}

	private void RayCheck(bool _grab)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatar.isDisabled || playerAvatar.isTumbling || playerAvatar.deadSet)
		{
			return;
		}
		float num = 10f;
		if (_grab)
		{
			grabDisableTimer = 0.1f;
		}
		Vector3 val = ((Component)playerCamera).transform.forward;
		if (overrideGrab && Object.op_Implicit((Object)(object)overrideGrabTarget))
		{
			Vector3 val2 = ((Component)overrideGrabTarget).transform.position - ((Component)playerCamera).transform.position;
			val = ((Vector3)(ref val2)).normalized;
		}
		if (!_grab)
		{
			RaycastHit[] array = Physics.SphereCastAll(((Component)playerCamera).transform.position, 1f, val, num, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)2);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val3 = array[i];
				ValuableObject component = ((Component)((RaycastHit)(ref val3)).transform).GetComponent<ValuableObject>();
				if (!Object.op_Implicit((Object)(object)component))
				{
					continue;
				}
				if (!component.discovered)
				{
					Vector3 val4 = ((Component)playerCamera).transform.position - ((RaycastHit)(ref val3)).point;
					RaycastHit[] array2 = Physics.SphereCastAll(((RaycastHit)(ref val3)).point, 0.01f, val4, ((Vector3)(ref val4)).magnitude, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)2);
					bool flag = true;
					RaycastHit[] array3 = array2;
					for (int j = 0; j < array3.Length; j++)
					{
						RaycastHit val5 = array3[j];
						if (!((Component)((RaycastHit)(ref val5)).transform).CompareTag("Player") && (Object)(object)((RaycastHit)(ref val5)).transform != (Object)(object)((RaycastHit)(ref val3)).transform)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						component.Discover(ValuableDiscoverGraphic.State.Discover);
					}
				}
				else
				{
					if (!component.discoveredReminder)
					{
						continue;
					}
					Vector3 val6 = ((Component)playerCamera).transform.position - ((RaycastHit)(ref val3)).point;
					RaycastHit[] array4 = Physics.RaycastAll(((RaycastHit)(ref val3)).point, val6, ((Vector3)(ref val6)).magnitude, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)2);
					bool flag2 = true;
					RaycastHit[] array3 = array4;
					for (int j = 0; j < array3.Length; j++)
					{
						RaycastHit val7 = array3[j];
						if (((Component)((Component)((RaycastHit)(ref val7)).collider).transform).CompareTag("Wall"))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						component.discoveredReminder = false;
						component.Discover(ValuableDiscoverGraphic.State.Reminder);
					}
				}
			}
		}
		RaycastHit val8 = default(RaycastHit);
		if (!Physics.Raycast(((Component)playerCamera).transform.position, val, ref val8, num, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)1))
		{
			return;
		}
		bool flag3 = false;
		flag3 = overrideGrab && !Object.op_Implicit((Object)(object)overrideGrabTarget);
		flag3 = overrideGrab && Object.op_Implicit((Object)(object)overrideGrabTarget) && (Object)(object)((Component)((RaycastHit)(ref val8)).transform).GetComponentInParent<PhysGrabObject>() == (Object)(object)overrideGrabTarget;
		if (!overrideGrab)
		{
			flag3 = true;
		}
		if (!(((Component)((RaycastHit)(ref val8)).collider).CompareTag("Phys Grab Object") && flag3) || ((RaycastHit)(ref val8)).distance > grabRange)
		{
			return;
		}
		if (_grab)
		{
			grabbedPhysGrabObject = ((Component)((RaycastHit)(ref val8)).transform).GetComponent<PhysGrabObject>();
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject) && grabbedPhysGrabObject.grabDisableTimer > 0f)
			{
				return;
			}
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject) && grabbedPhysGrabObject.rb.IsSleeping())
			{
				grabbedPhysGrabObject.OverrideIndestructible(0.5f);
				grabbedPhysGrabObject.OverrideBreakEffects(0.5f);
			}
			grabbedObjectTransform = ((RaycastHit)(ref val8)).transform;
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
			{
				PhysGrabObjectCollider component2 = ((Component)((RaycastHit)(ref val8)).collider).GetComponent<PhysGrabObjectCollider>();
				grabbedPhysGrabObjectCollider = ((RaycastHit)(ref val8)).collider;
				grabbedPhysGrabObjectColliderID = component2.colliderID;
				grabbedStaticGrabObject = null;
			}
			else
			{
				grabbedPhysGrabObject = null;
				grabbedPhysGrabObjectCollider = null;
				grabbedPhysGrabObjectColliderID = 0;
				grabbedStaticGrabObject = ((Component)grabbedObjectTransform).GetComponent<StaticGrabObject>();
				if (!Object.op_Implicit((Object)(object)grabbedStaticGrabObject))
				{
					StaticGrabObject[] componentsInParent = ((Component)grabbedObjectTransform).GetComponentsInParent<StaticGrabObject>();
					foreach (StaticGrabObject staticGrabObject in componentsInParent)
					{
						if ((Object)(object)staticGrabObject.colliderTransform == (Object)(object)((Component)((RaycastHit)(ref val8)).collider).transform)
						{
							grabbedStaticGrabObject = staticGrabObject;
						}
					}
				}
				if (!Object.op_Implicit((Object)(object)grabbedStaticGrabObject) || !((Behaviour)grabbedStaticGrabObject).enabled)
				{
					return;
				}
			}
			PhysGrabPointActivate();
			((Component)physGrabPointPuller).gameObject.SetActive(true);
			grabbedObject = ((RaycastHit)(ref val8)).rigidbody;
			Vector3 val9 = ((RaycastHit)(ref val8)).point;
			if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject) && ((Vector3)(ref grabbedPhysGrabObject.roomVolumeCheck.currentSize)).magnitude < 0.5f)
			{
				Bounds bounds = ((RaycastHit)(ref val8)).collider.bounds;
				val9 = ((Bounds)(ref bounds)).center;
			}
			float num2 = Vector3.Distance(((Component)playerCamera).transform.position, val9);
			Vector3 position = ((Component)playerCamera).transform.position + ((Component)playerCamera).transform.forward * num2;
			physGrabPointPlane.position = position;
			physGrabPointPuller.position = position;
			if (physRotatingTimer <= 0f)
			{
				cameraRelativeGrabbedForward = ((Component)Camera.main).transform.InverseTransformDirection(grabbedObjectTransform.forward);
				cameraRelativeGrabbedUp = ((Component)Camera.main).transform.InverseTransformDirection(grabbedObjectTransform.up);
				cameraRelativeGrabbedRight = ((Component)Camera.main).transform.InverseTransformDirection(grabbedObjectTransform.right);
			}
			if (GameManager.instance.gameMode == 0)
			{
				physGrabPoint.position = val9;
				if (!Object.op_Implicit((Object)(object)grabbedPhysGrabObject) || !Object.op_Implicit((Object)(object)grabbedPhysGrabObject.forceGrabPoint))
				{
					localGrabPosition = grabbedObjectTransform.InverseTransformPoint(val9);
				}
				else
				{
					val9 = grabbedPhysGrabObject.forceGrabPoint.position;
					num2 = 1f;
					position = ((Component)playerCamera).transform.position + ((Component)playerCamera).transform.forward * num2 - ((Component)playerCamera).transform.up * 0.3f;
					physGrabPoint.position = val9;
					physGrabPointPlane.position = position;
					physGrabPointPuller.position = position;
					localGrabPosition = grabbedObjectTransform.InverseTransformPoint(val9);
				}
			}
			else if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
			{
				if (Object.op_Implicit((Object)(object)grabbedPhysGrabObject.forceGrabPoint))
				{
					val9 = grabbedPhysGrabObject.forceGrabPoint.position;
					Quaternion val10 = Quaternion.Euler(45f, 0f, 0f);
					cameraRelativeGrabbedForward = val10 * Vector3.forward;
					cameraRelativeGrabbedUp = val10 * Vector3.up;
					cameraRelativeGrabbedRight = val10 * Vector3.right;
					num2 = 1f;
					position = ((Component)playerCamera).transform.position + ((Component)playerCamera).transform.forward * num2 - ((Component)playerCamera).transform.up * 0.3f;
					if (!Object.op_Implicit((Object)(object)overrideGrabPointTransform))
					{
						physGrabPoint.position = val9;
					}
					else
					{
						physGrabPoint.position = overrideGrabPointTransform.position;
					}
					physGrabPointPlane.position = position;
					physGrabPointPuller.position = position;
				}
				grabbedPhysGrabObject.GrabLink(photonView.ViewID, grabbedPhysGrabObjectColliderID, val9, cameraRelativeGrabbedForward, cameraRelativeGrabbedUp);
			}
			else if (Object.op_Implicit((Object)(object)grabbedStaticGrabObject))
			{
				grabbedStaticGrabObject.GrabLink(photonView.ViewID, val9);
			}
			if (isLocal)
			{
				PlayerController.instance.physGrabObject = ((Component)grabbedObjectTransform).gameObject;
				PlayerController.instance.physGrabActive = true;
			}
			initialPressTimer = 0.1f;
			prevGrabbed = grabbed;
			grabbed = true;
		}
		if (!grabbed)
		{
			bool flag4 = false;
			PhysGrabObject physGrabObject = ((Component)((RaycastHit)(ref val8)).transform).GetComponent<PhysGrabObject>();
			if (!Object.op_Implicit((Object)(object)physGrabObject))
			{
				physGrabObject = ((Component)((RaycastHit)(ref val8)).transform).GetComponentInParent<PhysGrabObject>();
			}
			if (Object.op_Implicit((Object)(object)physGrabObject))
			{
				currentlyLookingAtPhysGrabObject = physGrabObject;
				flag4 = true;
			}
			StaticGrabObject staticGrabObject2 = ((Component)((RaycastHit)(ref val8)).transform).GetComponent<StaticGrabObject>();
			if (!Object.op_Implicit((Object)(object)staticGrabObject2))
			{
				staticGrabObject2 = ((Component)((RaycastHit)(ref val8)).transform).GetComponentInParent<StaticGrabObject>();
			}
			if (Object.op_Implicit((Object)(object)staticGrabObject2) && ((Behaviour)staticGrabObject2).enabled)
			{
				currentlyLookingAtStaticGrabObject = staticGrabObject2;
				flag4 = true;
			}
			ItemAttributes component3 = ((Component)((RaycastHit)(ref val8)).transform).GetComponent<ItemAttributes>();
			if (Object.op_Implicit((Object)(object)component3))
			{
				currentlyLookingAtItemAttributes = component3;
				component3.ShowInfo();
			}
			if (flag4)
			{
				Aim.instance.SetState(Aim.State.Grabbable);
			}
		}
	}

	public void ReleaseObject(float _disableTimer = 0.1f)
	{
		if (!grabbed)
		{
			return;
		}
		overrideGrab = false;
		overrideGrabTarget = null;
		if (Object.op_Implicit((Object)(object)physGrabPoint))
		{
			PhysGrabEnded();
			physGrabPoint.SetParent((Transform)null, true);
			grabbedObject = null;
			grabbedObjectTransform = null;
			prevGrabbed = grabbed;
			grabbed = false;
			if (isLocal)
			{
				PlayerController.instance.physGrabObject = null;
				PlayerController.instance.physGrabActive = false;
			}
			if (Object.op_Implicit((Object)(object)physGrabPoint))
			{
				PhysGrabPointDeactivate();
			}
			if (Object.op_Implicit((Object)(object)physGrabPointPuller))
			{
				((Component)physGrabPointPuller).gameObject.SetActive(false);
			}
			PhysGrabBeamDeactivate();
			grabDisableTimer = 0.1f;
		}
	}

	[PunRPC]
	public void ReleaseObjectRPC(bool physGrabEnded, float _disableTimer = 0.1f)
	{
		if (isLocal)
		{
			if (!physGrabEnded)
			{
				grabbedStaticGrabObject = null;
			}
			ReleaseObject();
			grabDisableTimer = _disableTimer;
		}
	}

	private void GridObjectsInstantiate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabObject physGrabObject = grabbedPhysGrabObject;
		if (((Component)physGrabObject).GetComponent<PhysGrabObjectImpactDetector>().isCart)
		{
			return;
		}
		Quaternion rotation = grabbedPhysGrabObject.rb.rotation;
		grabbedPhysGrabObject.rb.rotation = Quaternion.identity;
		Collider[] componentsInChildren = ((Component)physGrabObject).GetComponentsInChildren<Collider>();
		foreach (Collider val in componentsInChildren)
		{
			if (!val.isTrigger && ((Component)val).gameObject.activeSelf && !(val is MeshCollider))
			{
				GameObject val2 = Object.Instantiate<GameObject>(physGrabPointVisualGridObject);
				val2.SetActive(true);
				SetGridObjectScale(val2.transform, val);
				Quaternion rotation2 = grabbedObjectTransform.rotation;
				physGrabPointVisualGrid.rotation = Quaternion.identity;
				grabbedObjectTransform.rotation = Quaternion.identity;
				physGrabPointVisualGrid.localRotation = Quaternion.identity;
				Vector3 position = ((Component)grabbedPhysGrabObject).transform.position;
				_ = ((Component)grabbedPhysGrabObject).transform.localRotation;
				physGrabPointVisualGrid.position = ((Component)grabbedPhysGrabObject).transform.TransformPoint(grabbedPhysGrabObject.midPointOffset);
				((Component)grabbedPhysGrabObject).transform.position = Vector3.zero;
				Transform transform = val2.transform;
				Bounds bounds = val.bounds;
				transform.position = ((Bounds)(ref bounds)).center;
				val2.transform.rotation = ((Component)val).transform.rotation;
				val2.transform.SetParent(physGrabPointVisualGrid);
				physGrabPointVisualGridObjects.Add(val2);
				grabbedObjectTransform.rotation = rotation2;
				((Component)grabbedPhysGrabObject).transform.position = position;
			}
		}
		grabbedPhysGrabObject.rb.rotation = rotation;
	}

	private void SetGridObjectScale(Transform _itemEquipCubeTransform, Collider _collider)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Component)_collider).transform.rotation;
		((Component)_collider).transform.rotation = Quaternion.identity;
		BoxCollider val = (BoxCollider)(object)((_collider is BoxCollider) ? _collider : null);
		if (val != null)
		{
			_itemEquipCubeTransform.localScale = Vector3.Scale(val.size, ((Component)_collider).transform.lossyScale);
		}
		else
		{
			SphereCollider val2 = (SphereCollider)(object)((_collider is SphereCollider) ? _collider : null);
			if (val2 != null)
			{
				float num = val2.radius * Mathf.Max(new float[3]
				{
					((Component)_collider).transform.lossyScale.x,
					((Component)_collider).transform.lossyScale.y,
					((Component)_collider).transform.lossyScale.z
				}) * 2f;
				_itemEquipCubeTransform.localScale = new Vector3(num, num, num);
			}
			else
			{
				CapsuleCollider val3 = (CapsuleCollider)(object)((_collider is CapsuleCollider) ? _collider : null);
				if (val3 != null)
				{
					float num2 = val3.radius * Mathf.Max(((Component)_collider).transform.lossyScale.x, ((Component)_collider).transform.lossyScale.z) * 2f;
					float num3 = val3.height * ((Component)_collider).transform.lossyScale.y;
					_itemEquipCubeTransform.localScale = new Vector3(num2, num3, num2);
				}
				else
				{
					Bounds bounds = _collider.bounds;
					_itemEquipCubeTransform.localScale = ((Bounds)(ref bounds)).size;
				}
			}
		}
		((Component)_collider).transform.rotation = rotation;
	}

	private void GridObjectsRemove()
	{
		foreach (GameObject physGrabPointVisualGridObject in physGrabPointVisualGridObjects)
		{
			Object.Destroy((Object)(object)physGrabPointVisualGridObject);
		}
		physGrabPointVisualGridObjects.Clear();
	}
}
