using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemDrone : MonoBehaviour
{
	public GameObject teleportParticles;

	private ItemAttributes itemAttributes;

	[HideInInspector]
	public SemiFunc.emojiIcon emojiIcon;

	public Texture droneIcon;

	[HideInInspector]
	public ColorPresets colorPreset;

	public BatteryDrainPresets batteryDrainPreset;

	[HideInInspector]
	public float batteryDrainRate = 0.1f;

	[HideInInspector]
	public Color droneColor;

	[HideInInspector]
	public Color batteryColor;

	[HideInInspector]
	public Color beamColor;

	private float checkTimer;

	private Transform magnetTarget;

	[HideInInspector]
	public PhysGrabObject magnetTargetPhysGrabObject;

	[HideInInspector]
	public Rigidbody magnetTargetRigidbody;

	[HideInInspector]
	public bool magnetActive;

	public PlayerTumble playerTumbleTarget;

	private Rigidbody rb;

	private bool attachPointFound;

	private Vector3 attachPoint;

	private float springConstant = 50f;

	private float dampingCoefficient = 5f;

	private float newAttachPointTimer;

	[HideInInspector]
	public bool itemActivated;

	private PhotonView photonView;

	private Vector3 rayHitPosition;

	private Vector3 animatedRayHitPosition;

	private LineBetweenTwoPoints lineBetweenTwoPoints;

	public Transform lineStartPoint;

	private float rayTimer;

	private Transform prevMagnetTarget;

	private Transform droneTransform;

	private List<Transform> dronePyramidTransforms = new List<Transform>();

	private List<Transform> droneTriangleTransforms = new List<Transform>();

	private float lerpAnimationProgress;

	private bool hasBattery = true;

	private ItemBattery itemBattery;

	private float onNoBatteryTimer;

	private bool animationOpen;

	private Transform onSwitchTransform;

	public ItemDroneSounds itemDroneSounds;

	[HideInInspector]
	public Sound soundDroneLoop;

	[HideInInspector]
	public Sound soundDroneBeamLoop;

	public PhysicMaterial physicMaterialSlippery;

	public bool targetValuables;

	public bool targetPlayers;

	public bool targetEnemies;

	public bool targetNonValuables;

	[HideInInspector]
	public Vector3 connectionPoint;

	[HideInInspector]
	public Transform lastPlayerToTouch;

	private PhysGrabObject physGrabObject;

	private float randomNudgeTimer;

	private Collider droneCollider;

	private PhysicMaterial physicMaterialOriginal;

	private ItemToggle itemToggle;

	internal PlayerAvatar playerAvatarTarget;

	private bool targetIsPlayer;

	internal bool targetIsLocalPlayer;

	private ItemEquippable itemEquippable;

	private Camera cameraMain;

	internal PlayerAvatar droneOwner;

	private float teleportSpotTimer;

	private bool hadTarget;

	private bool targetIsEnemy;

	private bool togglePrevious;

	private bool fullReset;

	private bool fullInit;

	private EnemyParent enemyTarget;

	private bool magnetActivePrev;

	private ITargetingCondition customTargetingCondition;

	private void Start()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Expected O, but got Unknown
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Expected O, but got Unknown
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item2 in ((Component)this).transform)
		{
			Transform val = item2;
			if (((Object)val).name == "Particles")
			{
				teleportParticles = ((Component)val).gameObject;
				break;
			}
		}
		customTargetingCondition = ((Component)this).GetComponent<ITargetingCondition>();
		droneCollider = ((Component)this).GetComponentInChildren<Collider>();
		cameraMain = Camera.main;
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemEquippable.itemEmoji = emojiIcon.ToString();
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		lineBetweenTwoPoints = ((Component)this).GetComponent<LineBetweenTwoPoints>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		if (!Object.op_Implicit((Object)(object)itemBattery))
		{
			hasBattery = false;
		}
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		emojiIcon = itemAttributes.emojiIcon;
		colorPreset = itemAttributes.colorPreset;
		droneColor = colorPreset.GetColorMain();
		batteryColor = colorPreset.GetColorLight();
		beamColor = colorPreset.GetColorDark();
		batteryDrainRate = batteryDrainPreset.GetBatteryDrainRate();
		itemBattery.batteryDrainRate = batteryDrainRate;
		itemBattery.batteryColor = batteryColor;
		Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider val2 = componentsInChildren[num];
			physicMaterialOriginal = val2.material;
		}
		Sound.CopySound(itemDroneSounds.DroneLoop, soundDroneLoop);
		Sound.CopySound(itemDroneSounds.DroneBeamLoop, soundDroneBeamLoop);
		ItemLight componentInChildren = ((Component)this).GetComponentInChildren<ItemLight>();
		if (Object.op_Implicit((Object)(object)componentInChildren))
		{
			componentInChildren.itemLight.color = droneColor;
		}
		AudioSource component = ((Component)this).GetComponent<AudioSource>();
		soundDroneLoop.Source = component;
		soundDroneBeamLoop.Source = component;
		foreach (Transform item3 in ((Component)this).transform)
		{
			Transform val3 = item3;
			if (((Object)val3).name == "Drone Icon")
			{
				onSwitchTransform = val3;
				((Component)onSwitchTransform).GetComponent<Renderer>().material.SetTexture("_EmissionMap", droneIcon);
				((Component)onSwitchTransform).GetComponent<Renderer>().material.SetColor("_EmissionColor", droneColor);
			}
			if (!(((Object)val3).name == "Drone"))
			{
				continue;
			}
			droneTransform = val3;
			foreach (Transform item4 in val3)
			{
				Transform val4 = item4;
				if (((Object)val4).name.Contains("Drone Triangle"))
				{
					foreach (Transform item5 in val4)
					{
						Transform item = item5;
						droneTriangleTransforms.Add(item);
					}
				}
				if (!((Object)val4).name.Contains("Drone Pyramid"))
				{
					continue;
				}
				foreach (Transform item6 in val4)
				{
					Transform val5 = item6;
					dronePyramidTransforms.Add(val5);
					((Component)val5).GetComponent<Renderer>().material.SetColor("_EmissionColor", droneColor);
				}
			}
		}
		((Component)droneTransform).GetComponent<Renderer>().material.SetColor("_EmissionColor", droneColor);
		physGrabObject.clientNonKinematic = true;
	}

	private void AnimateDrone()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!itemActivated)
		{
			return;
		}
		lerpAnimationProgress += Time.deltaTime * 10f;
		if (lerpAnimationProgress > 1f)
		{
			lerpAnimationProgress = 1f;
			animationOpen = true;
		}
		float num = 15f;
		if (magnetActive)
		{
			num = 60f;
		}
		foreach (Transform dronePyramidTransform in dronePyramidTransforms)
		{
			float num2 = -33f;
			if (lerpAnimationProgress != 1f)
			{
				dronePyramidTransform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(0f, num2, lerpAnimationProgress), 0f);
				continue;
			}
			float num3 = Mathf.Sin(Time.time * num) * 5f;
			dronePyramidTransform.localRotation = Quaternion.Euler(0f, num2 + num3, 0f);
		}
		foreach (Transform droneTriangleTransform in droneTriangleTransforms)
		{
			float num4 = 45f;
			if (lerpAnimationProgress != 1f)
			{
				droneTriangleTransform.localRotation = Quaternion.Euler(Mathf.Lerp(0f, num4, lerpAnimationProgress), 0f, 0f);
				continue;
			}
			float num5 = Mathf.Sin(Time.time * num / 3f) * 10f;
			droneTriangleTransform.localRotation = Quaternion.Euler(num4 + num5, 0f, 0f);
		}
	}

	private bool TargetFindPlayer()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		playerAvatarTarget = null;
		playerTumbleTarget = null;
		float num = 10000f;
		Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, 1f, LayerMask.GetMask(new string[1] { "Player" }));
		foreach (Collider val in array)
		{
			PlayerAvatar playerAvatar = ((Component)val).GetComponentInParent<PlayerAvatar>();
			if (!Object.op_Implicit((Object)(object)playerAvatar))
			{
				PlayerController componentInParent = ((Component)val).GetComponentInParent<PlayerController>();
				if (Object.op_Implicit((Object)(object)componentInParent))
				{
					playerAvatar = componentInParent.playerAvatarScript;
				}
			}
			if (!Object.op_Implicit((Object)(object)playerAvatar) || (customTargetingCondition != null && !customTargetingCondition.CustomTargetingCondition(((Component)playerAvatar).gameObject)))
			{
				continue;
			}
			float num2 = Vector3.Distance(((Component)this).transform.position, playerAvatar.PlayerVisionTarget.VisionTransform.position);
			if (num2 < num)
			{
				num = num2;
				playerAvatarTarget = playerAvatar;
				targetIsPlayer = true;
				if (playerAvatarTarget.isLocal)
				{
					targetIsLocalPlayer = true;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)playerAvatarTarget))
		{
			Transform val2 = playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			Vector3 newAttachPoint = val2.position;
			if (playerAvatarTarget.isTumbling && Object.op_Implicit((Object)(object)((Component)playerAvatarTarget).transform))
			{
				playerTumbleTarget = playerAvatarTarget.tumble;
				val2 = ((Component)playerTumbleTarget).transform;
				newAttachPoint = playerTumbleTarget.physGrabObject.centerPoint;
			}
			targetIsLocalPlayer = playerAvatarTarget.isLocal;
			targetIsPlayer = true;
			NewRayHitPoint(newAttachPoint, ((Component)playerAvatarTarget).GetComponent<PhotonView>().ViewID, -1, val2);
			attachPoint = rayHitPosition;
			ActivateMagnet();
			return true;
		}
		return false;
	}

	private void GetPlayerTumbleTarget()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)magnetTarget))
		{
			if (Object.op_Implicit((Object)(object)playerTumbleTarget) && !playerTumbleTarget.playerAvatar.isTumbling)
			{
				playerAvatarTarget = playerTumbleTarget.playerAvatar;
				targetIsLocalPlayer = playerAvatarTarget.isLocal;
				targetIsPlayer = true;
				ActivateMagnet();
				playerTumbleTarget = null;
				Transform visionTransform = playerAvatarTarget.PlayerVisionTarget.VisionTransform;
				Vector3 position = visionTransform.position;
				NewRayHitPoint(position, ((Component)playerAvatarTarget).GetComponent<PhotonView>().ViewID, -1, visionTransform);
			}
			if (Object.op_Implicit((Object)(object)playerAvatarTarget) && playerAvatarTarget.isTumbling)
			{
				_ = playerAvatarTarget.PlayerVisionTarget.VisionTransform.position;
				playerTumbleTarget = playerAvatarTarget.tumble;
				_ = ((Component)playerTumbleTarget).transform;
				_ = playerTumbleTarget.physGrabObject.centerPoint;
				targetIsLocalPlayer = false;
				targetIsPlayer = false;
				magnetTarget = ((Component)playerTumbleTarget).transform;
				magnetTargetPhysGrabObject = playerTumbleTarget.physGrabObject;
				playerAvatarTarget = null;
				attachPoint = rayHitPosition;
				ActivateMagnet();
			}
		}
	}

	private void FullReset()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		hadTarget = false;
		magnetTarget = null;
		magnetTargetPhysGrabObject = null;
		magnetTargetRigidbody = null;
		DeactivateMagnet();
		playerTumbleTarget = null;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		attachPoint = Vector3.zero;
		attachPointFound = false;
		rayHitPosition = Vector3.zero;
		animatedRayHitPosition = Vector3.zero;
	}

	private void ToggleOnFullInit()
	{
		if (!fullInit && itemActivated && !togglePrevious)
		{
			fullReset = false;
			fullInit = true;
			togglePrevious = true;
		}
	}

	private void ToggleOffFullReset()
	{
		if (!fullReset && !itemActivated && togglePrevious)
		{
			fullReset = true;
			fullInit = false;
			togglePrevious = false;
		}
	}

	private void ToggleOffIfLostTarget()
	{
	}

	private void ToggleOffIfEnemyTargetIsDead()
	{
		if (!Object.op_Implicit((Object)(object)magnetTarget) || !targetIsEnemy)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)enemyTarget))
		{
			if (!enemyTarget.Spawned && itemToggle.toggleState)
			{
				ForceTurnOff();
				enemyTarget = null;
			}
		}
		else if (itemToggle.toggleState)
		{
			ForceTurnOff();
			enemyTarget = null;
		}
	}

	private void ToggleOffIfPlayerTargetIsDead()
	{
		if (SemiFunc.FPSImpulse5() && targetIsPlayer)
		{
			if (Object.op_Implicit((Object)(object)playerAvatarTarget) && playerAvatarTarget.isDisabled && itemToggle.toggleState)
			{
				ButtonToggleSet(toggle: false);
				playerAvatarTarget = null;
			}
			if (Object.op_Implicit((Object)(object)playerTumbleTarget) && playerTumbleTarget.playerAvatar.isDisabled && itemToggle.toggleState)
			{
				ButtonToggleSet(toggle: false);
				playerTumbleTarget = null;
			}
		}
	}

	private void ForceTurnOff()
	{
		itemBattery.BatteryToggle(toggle: false);
		ButtonToggleSet(toggle: false);
		itemToggle.ToggleItem(toggle: false);
		hadTarget = false;
		itemActivated = false;
	}

	private void Update()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		if (itemEquippable.isEquipped)
		{
			return;
		}
		if (magnetActivePrev != magnetActive)
		{
			BatteryToggle(magnetActive);
			magnetActivePrev = magnetActive;
		}
		if (hadTarget && !magnetActive)
		{
			ForceTurnOff();
		}
		else
		{
			if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
			{
				return;
			}
			soundDroneLoop.PlayLoop(itemActivated, 2f, 2f);
			AnimateDrone();
			if (!itemActivated)
			{
				onNoBatteryTimer = 0f;
			}
			if (itemActivated)
			{
				physGrabObject.impactDetector.canHurtLogic = false;
			}
			else
			{
				physGrabObject.impactDetector.canHurtLogic = true;
			}
			if (itemActivated && magnetActive && Object.op_Implicit((Object)(object)magnetTarget) && !itemEquippable.isEquipped)
			{
				if (rayHitPosition != Vector3.zero && !targetIsPlayer)
				{
					bool flag = false;
					if (Object.op_Implicit((Object)(object)playerTumbleTarget) && playerTumbleTarget.playerAvatar.isLocal)
					{
						flag = true;
					}
					if (!flag)
					{
						animatedRayHitPosition = Vector3.Lerp(animatedRayHitPosition, rayHitPosition, Time.deltaTime * 10f);
						lineBetweenTwoPoints.DrawLine(lineStartPoint.position, magnetTarget.TransformPoint(animatedRayHitPosition));
						connectionPoint = magnetTarget.TransformPoint(animatedRayHitPosition);
					}
					else
					{
						Vector3 val = default(Vector3);
						((Vector3)(ref val))._002Ector(0f, -0.5f, 0f);
						Vector3 point = ((Component)cameraMain).transform.position + val;
						lineBetweenTwoPoints.DrawLine(lineStartPoint.position, point);
						connectionPoint = point;
					}
				}
				else
				{
					animatedRayHitPosition = Vector3.Lerp(animatedRayHitPosition, rayHitPosition, Time.deltaTime * 10f);
					if (!targetIsPlayer)
					{
						lineBetweenTwoPoints.DrawLine(lineStartPoint.position, magnetTargetPhysGrabObject.midPoint);
						connectionPoint = magnetTargetPhysGrabObject.midPoint;
					}
					if (targetIsPlayer)
					{
						Vector3 zero = default(Vector3);
						((Vector3)(ref zero))._002Ector(0f, -0.5f, 0f);
						if (playerAvatarTarget.isTumbling)
						{
							zero = Vector3.zero;
						}
						if (!targetIsLocalPlayer)
						{
							lineBetweenTwoPoints.DrawLine(lineStartPoint.position, magnetTarget.position + zero);
							connectionPoint = magnetTarget.position + zero;
						}
						else
						{
							Vector3 point2 = ((Component)cameraMain).transform.position + zero;
							lineBetweenTwoPoints.DrawLine(lineStartPoint.position, point2);
							connectionPoint = point2;
						}
					}
				}
			}
			if (!itemActivated && !magnetActive && animationOpen)
			{
				lerpAnimationProgress += Time.deltaTime * 10f;
				if (lerpAnimationProgress > 1f)
				{
					lerpAnimationProgress = 1f;
					animationOpen = false;
				}
				foreach (Transform dronePyramidTransform in dronePyramidTransforms)
				{
					float num = 0f;
					dronePyramidTransform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(-33f, num, lerpAnimationProgress), 0f);
				}
				foreach (Transform droneTriangleTransform in droneTriangleTransforms)
				{
					float num2 = 0f;
					droneTriangleTransform.localRotation = Quaternion.Euler(Mathf.Lerp(45f, num2, lerpAnimationProgress), 0f, 0f);
				}
			}
			GetPlayerTumbleTarget();
			if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (itemToggle.toggleState != itemActivated)
			{
				ButtonToggle();
			}
			if (physGrabObject.playerGrabbing.Count == 1)
			{
				lastPlayerToTouch = ((Component)physGrabObject.playerGrabbing[0]).transform;
			}
			if (!itemActivated)
			{
				return;
			}
			springConstant = 40f;
			dampingCoefficient = 10f;
			if (!magnetActive)
			{
				checkTimer += Time.deltaTime;
				if (checkTimer > 0.5f)
				{
					bool flag2 = false;
					if (targetPlayers && !flag2)
					{
						flag2 = TargetFindPlayer();
					}
					if (!flag2 && (targetValuables || targetNonValuables || targetEnemies))
					{
						flag2 = SphereCheck();
					}
					if (flag2)
					{
						hadTarget = true;
						ActivateMagnet();
					}
					checkTimer = 0f;
				}
			}
			else if (!attachPointFound)
			{
				if (!targetIsPlayer)
				{
					if (rayTimer <= 0f)
					{
						FindBeamAttachPosition();
						rayTimer = 0.5f;
					}
					else
					{
						rayTimer -= Time.deltaTime;
					}
				}
			}
			else
			{
				Vector3 velocity = rb.velocity;
				if (((Vector3)(ref velocity)).magnitude > 0.2f)
				{
					newAttachPointTimer += Time.deltaTime;
					if (newAttachPointTimer > 0.5f)
					{
						attachPointFound = false;
						newAttachPointTimer = 0f;
						rayTimer = 0f;
					}
				}
			}
			if (itemActivated && hasBattery && itemBattery.batteryLife <= 0f)
			{
				if (!itemBattery.batteryActive)
				{
					itemBattery.BatteryToggle(toggle: true);
				}
				onNoBatteryTimer += Time.deltaTime;
				if (onNoBatteryTimer >= 1.5f)
				{
					ForceTurnOff();
					onNoBatteryTimer = 0f;
				}
			}
		}
	}

	public void ButtonToggleSet(bool toggle)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			ButtonToggleRPC(toggle);
		}
		else if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("ButtonToggleRPC", (RpcTarget)0, new object[1] { toggle });
		}
	}

	private void FixedUpdate()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		if (!itemActivated)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)magnetTarget))
		{
			ItemEquippable componentInParent = ((Component)magnetTarget).GetComponentInParent<ItemEquippable>();
			if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.isEquipped && magnetActive)
			{
				ForceTurnOff();
				DeactivateMagnet();
			}
		}
		if (itemEquippable.isEquipped)
		{
			if (magnetActive)
			{
				ForceTurnOff();
				DeactivateMagnet();
			}
		}
		else
		{
			if ((!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial()) || (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient) || !itemActivated || !magnetActive)
			{
				return;
			}
			if (!Object.op_Implicit((Object)(object)magnetTarget))
			{
				DeactivateMagnet();
				return;
			}
			if (Vector3.Distance(((Component)this).transform.position, magnetTarget.position) > 4f)
			{
				FindTeleportSpot();
			}
			Collider val = null;
			if (Object.op_Implicit((Object)(object)magnetTarget))
			{
				val = ((Component)magnetTarget).GetComponent<Collider>();
			}
			if (!Object.op_Implicit((Object)(object)playerTumbleTarget) && (!Object.op_Implicit((Object)(object)magnetTarget) || !((Component)magnetTarget).gameObject.activeSelf || !((Component)magnetTarget).gameObject.activeInHierarchy || (Object.op_Implicit((Object)(object)val) && !val.enabled)))
			{
				DeactivateMagnet();
				return;
			}
			physGrabObject.OverrideMaterial(physicMaterialSlippery);
			if (randomNudgeTimer <= 0f)
			{
				if (Vector3.Distance(((Component)this).transform.position, ((Component)magnetTarget).transform.position) > 1.5f)
				{
					Vector3 val2 = ((Component)this).transform.position - ((Component)magnetTarget).transform.position;
					Vector3[] array = (Vector3[])(object)new Vector3[4]
					{
						Vector3.up,
						Vector3.down,
						Vector3.left,
						Vector3.right
					};
					Vector3 val3 = array[Random.Range(0, array.Length)];
					Vector3 val4 = Vector3.Cross(val2, val3);
					Vector3 normalized = ((Vector3)(ref val4)).normalized;
					if (normalized != Vector3.zero)
					{
						rb.AddForce(normalized * 1f, (ForceMode)1);
						rb.AddTorque(normalized * 10f, (ForceMode)1);
					}
				}
				randomNudgeTimer = 0.5f;
			}
			else
			{
				randomNudgeTimer -= Time.fixedDeltaTime;
			}
			if (attachPointFound)
			{
				Vector3 val5 = magnetTarget.TransformPoint(attachPoint) - ((Component)this).transform.position;
				Vector3 val6 = springConstant * val5;
				Vector3 velocity = rb.velocity;
				Vector3 val7 = (0f - dampingCoefficient) * velocity;
				Vector3 val8 = val6 + val7;
				val8 = Vector3.ClampMagnitude(val8, 20f);
				rb.AddForce(val8);
				if (!((Component)magnetTarget).gameObject.activeSelf)
				{
					DeactivateMagnet();
				}
				SemiFunc.PhysLookAtPositionWithForce(rb, ((Component)this).transform, magnetTarget.TransformPoint(rayHitPosition), 1f);
			}
			else
			{
				Vector3 val9 = magnetTarget.position - ((Component)this).transform.position;
				if (((Vector3)(ref val9)).magnitude > 0.8f)
				{
					rb.AddForce(((Vector3)(ref val9)).normalized * 3f);
				}
				else
				{
					Vector3 val10 = -rb.velocity * 0.9f;
					rb.AddForce(val10);
				}
				if (!((Component)magnetTarget).gameObject.activeSelf)
				{
					DeactivateMagnet();
				}
				SemiFunc.PhysLookAtPositionWithForce(rb, ((Component)this).transform, magnetTarget.position, 1f);
			}
		}
	}

	[PunRPC]
	public void TeleportEffectRPC(Vector3 startPosition, Vector3 endPosition)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		itemDroneSounds.DroneRetract.Pitch = 3f;
		itemDroneSounds.DroneRetract.Play(startPosition);
		itemDroneSounds.DroneRetract.Pitch = 4f;
		itemDroneSounds.DroneRetract.Play(endPosition);
		Object.Instantiate<GameObject>(teleportParticles, startPosition, Quaternion.identity);
		Object.Instantiate<GameObject>(teleportParticles, endPosition, Quaternion.identity);
	}

	private void TeleportEffect(Vector3 startPosition, Vector3 endPosition)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("TeleportEffectRPC", (RpcTarget)0, new object[2] { startPosition, endPosition });
			}
		}
		else
		{
			TeleportEffectRPC(startPosition, endPosition);
		}
	}

	private void FindTeleportSpot()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		if (!magnetActive || !Object.op_Implicit((Object)(object)magnetTarget))
		{
			return;
		}
		if (teleportSpotTimer <= 0f)
		{
			ItemEquippable componentInParent = ((Component)magnetTarget).GetComponentInParent<ItemEquippable>();
			if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.isEquipped)
			{
				return;
			}
			Vector3 val = magnetTarget.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
			for (int i = 0; i < 10; i++)
			{
				val = magnetTarget.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				float num = Vector3.Distance(val, magnetTarget.position);
				float num2 = Mathf.Max(0f, num - 0.2f);
				RaycastHit[] array = Physics.RaycastAll(val, magnetTarget.position - val, num2, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
				bool flag = false;
				RaycastHit[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					RaycastHit val2 = array2[j];
					if ((Object)(object)((Component)((RaycastHit)(ref val2)).transform).GetComponentInParent<Rigidbody>() != (Object)(object)((Component)magnetTarget).GetComponentInParent<Rigidbody>() && (Object)(object)((RaycastHit)(ref val2)).transform != (Object)(object)((Component)this).transform)
					{
						flag = true;
						break;
					}
				}
				if (!flag && Physics.OverlapBox(val, new Vector3(0.2f, 0.2f, 0.2f), ((Component)this).transform.rotation, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())).Length == 0)
				{
					TeleportEffect(((Component)this).transform.position, val);
					if (SemiFunc.IsMultiplayer())
					{
						physGrabObject.photonTransformView.Teleport(val, ((Component)this).transform.rotation);
					}
					else
					{
						((Component)this).transform.position = val;
					}
					break;
				}
			}
			teleportSpotTimer = 0.2f;
		}
		else
		{
			teleportSpotTimer -= Time.deltaTime;
		}
	}

	private void DeactivateMagnet()
	{
		if (magnetActive)
		{
			attachPointFound = false;
			playerAvatarTarget = null;
			targetIsPlayer = false;
			targetIsLocalPlayer = false;
			targetIsEnemy = false;
			playerTumbleTarget = null;
			magnetTargetPhysGrabObject = null;
			magnetTargetRigidbody = null;
			MagnetActiveToggle(toggleBool: false);
		}
	}

	private void ActivateMagnet()
	{
		if (!magnetActive)
		{
			MagnetActiveToggle(toggleBool: true);
		}
	}

	private void BatteryToggle(bool activated)
	{
		if (hasBattery)
		{
			itemBattery.batteryActive = activated;
		}
	}

	private void ButtonToggleLogic(bool activated)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		FullReset();
		MagnetActiveToggle(activated);
		droneOwner = SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID);
		lerpAnimationProgress = 0f;
		if (activated)
		{
			((Component)onSwitchTransform).GetComponent<Renderer>().material.SetColor("_EmissionColor", droneColor);
			itemDroneSounds.DroneStart.Play(((Component)this).transform.position);
		}
		else
		{
			if (magnetActive)
			{
				DeactivateMagnet();
			}
			itemDroneSounds.DroneEnd.Play(((Component)this).transform.position);
		}
		itemActivated = activated;
	}

	public void ButtonToggle()
	{
		itemActivated = !itemActivated;
		if (GameManager.instance.gameMode == 0)
		{
			ButtonToggleLogic(itemActivated);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("ButtonToggleRPC", (RpcTarget)0, new object[1] { itemActivated });
		}
	}

	[PunRPC]
	private void ButtonToggleRPC(bool activated)
	{
		ButtonToggleLogic(activated);
	}

	private Transform GetHighestParentWithRigidbody(Transform child)
	{
		if ((Object)(object)((Component)this).GetComponent<Rigidbody>() != (Object)null && (Object)(object)((Component)child).GetComponent<PhotonView>() != (Object)null)
		{
			return child;
		}
		Transform val = child;
		while ((Object)(object)val.parent != (Object)null)
		{
			if ((Object)(object)((Component)val.parent).GetComponent<Rigidbody>() != (Object)null && (Object)(object)((Component)val.parent).GetComponent<PhotonView>() != (Object)null)
			{
				return val.parent;
			}
			val = val.parent;
		}
		return null;
	}

	private void MagnetActiveToggleLogic(bool activated)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		magnetActive = activated;
		lerpAnimationProgress = 0f;
		if (!activated)
		{
			itemDroneSounds.DroneRetract.Play(((Component)this).transform.position);
			rayHitPosition = Vector3.zero;
		}
		else
		{
			itemDroneSounds.DroneDeploy.Play(((Component)this).transform.position);
		}
	}

	public void MagnetActiveToggle(bool toggleBool)
	{
		if (GameManager.instance.gameMode == 0)
		{
			MagnetActiveToggleLogic(toggleBool);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("MagnetActiveToggleRPC", (RpcTarget)0, new object[1] { toggleBool });
		}
	}

	[PunRPC]
	private void MagnetActiveToggleRPC(bool activated)
	{
		MagnetActiveToggleLogic(activated);
	}

	private void NewRayHitPointLogic(Vector3 newRayHitPosition, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)newMagnetTarget))
		{
			magnetTargetPhysGrabObject = ((Component)newMagnetTarget).GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				magnetTarget = ((Component)newMagnetTarget).GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
				targetIsPlayer = false;
				targetIsLocalPlayer = false;
			}
			else
			{
				magnetTarget = newMagnetTarget;
			}
			animatedRayHitPosition = rayHitPosition;
			rayHitPosition = magnetTarget.InverseTransformPoint(newRayHitPosition);
			magnetTargetRigidbody = ((Component)GetHighestParentWithRigidbody(magnetTarget)).GetComponent<Rigidbody>();
			PlayerTumble component = ((Component)magnetTargetRigidbody).GetComponent<PlayerTumble>();
			if (Object.op_Implicit((Object)(object)component))
			{
				if (component.isTumbling)
				{
					playerTumbleTarget = component;
				}
				else
				{
					DeactivateMagnet();
				}
			}
			return;
		}
		magnetTargetPhysGrabObject = ((Component)PhotonView.Find(photonViewId)).gameObject.GetComponent<PhysGrabObject>();
		if (colliderID != -1)
		{
			magnetTarget = ((Component)PhotonView.Find(photonViewId)).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
			targetIsPlayer = false;
			targetIsLocalPlayer = false;
		}
		else
		{
			targetIsPlayer = true;
			playerAvatarTarget = ((Component)PhotonView.Find(photonViewId)).GetComponent<PlayerAvatar>();
			magnetTarget = playerAvatarTarget.PlayerVisionTarget.VisionTransform;
			targetIsLocalPlayer = playerAvatarTarget.isLocal;
			if (((Component)PhotonView.Find(photonViewId)).GetComponent<PlayerAvatar>().isLocal)
			{
				targetIsLocalPlayer = true;
			}
		}
		animatedRayHitPosition = rayHitPosition;
		rayHitPosition = magnetTarget.InverseTransformPoint(newRayHitPosition);
		magnetTargetRigidbody = ((Component)GetHighestParentWithRigidbody(magnetTarget)).GetComponent<Rigidbody>();
	}

	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer())
		{
			NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, newMagnetTarget);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("NewRayHitPointRPC", (RpcTarget)0, new object[3] { newAttachPoint, photonViewId, colliderID });
		}
	}

	[PunRPC]
	private void NewRayHitPointRPC(Vector3 newAttachPoint, int photonViewId, int colliderID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NewRayHitPointLogic(newAttachPoint, photonViewId, colliderID, null);
	}

	private bool SphereCheck()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		playerTumbleTarget = null;
		playerAvatarTarget = null;
		targetIsPlayer = false;
		targetIsEnemy = false;
		targetIsLocalPlayer = false;
		bool result = false;
		if (itemBattery.batteryLife <= 0f)
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, 1f);
		float num = 10000f;
		Collider[] array2 = array;
		RaycastHit val4 = default(RaycastHit);
		foreach (Collider val in array2)
		{
			Transform highestParentWithRigidbody = GetHighestParentWithRigidbody(((Component)val).transform);
			PhysGrabObjectCollider component = ((Component)val).GetComponent<PhysGrabObjectCollider>();
			PhysGrabObject physGrabObject = null;
			bool flag = false;
			bool flag2 = false;
			targetIsEnemy = false;
			if (Object.op_Implicit((Object)(object)highestParentWithRigidbody))
			{
				PhysGrabObjectImpactDetector component2 = ((Component)highestParentWithRigidbody).GetComponent<PhysGrabObjectImpactDetector>();
				physGrabObject = ((Component)highestParentWithRigidbody).GetComponent<PhysGrabObject>();
				if (Object.op_Implicit((Object)(object)component2))
				{
					if (component2.isValuable)
					{
						flag2 = true;
					}
					if (component2.isEnemy)
					{
						flag = true;
						targetIsEnemy = true;
						enemyTarget = ((Component)component2).GetComponentInParent<EnemyParent>();
					}
				}
			}
			bool flag3 = true;
			if (customTargetingCondition != null && Object.op_Implicit((Object)(object)highestParentWithRigidbody))
			{
				flag3 = customTargetingCondition.CustomTargetingCondition(((Component)highestParentWithRigidbody).gameObject);
			}
			bool flag4 = targetValuables && flag2;
			if (!flag4)
			{
				flag4 = targetEnemies && flag;
			}
			if (!flag4)
			{
				flag4 = targetNonValuables && !flag2;
			}
			if (!(Object.op_Implicit((Object)(object)component) && (Object)(object)highestParentWithRigidbody != (Object)(object)((Component)this).transform && Object.op_Implicit((Object)(object)highestParentWithRigidbody) && flag4 && flag3) || !((Component)highestParentWithRigidbody).gameObject.activeSelf)
			{
				continue;
			}
			float num2 = Vector3.Distance(((Component)this).transform.position, physGrabObject.centerPoint);
			if (num2 < num)
			{
				bool flag5 = false;
				Vector3 position = ((Component)this).transform.position;
				Vector3 val2 = physGrabObject.centerPoint - ((Component)this).transform.position;
				Vector3 val3 = physGrabObject.centerPoint - ((Component)this).transform.position;
				if (Physics.Raycast(position, val2, ref val4, ((Vector3)(ref val3)).magnitude, LayerMask.GetMask(new string[1] { "Default" })) && (Object)(object)((Component)((RaycastHit)(ref val4)).collider).transform != (Object)(object)((Component)val).transform && (Object)(object)((Component)((RaycastHit)(ref val4)).collider).transform != (Object)(object)((Component)this).transform)
				{
					flag5 = true;
				}
				if (!flag5)
				{
					num = num2;
					magnetTarget = ((Component)val).transform;
					magnetTargetPhysGrabObject = physGrabObject;
					magnetTargetRigidbody = ((Component)highestParentWithRigidbody).GetComponent<Rigidbody>();
					Vector3 position2 = ((Component)val).transform.position;
					NewRayHitPoint(position2, ((Component)highestParentWithRigidbody).GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					attachPoint = rayHitPosition;
					result = true;
				}
			}
		}
		return result;
	}

	private void FindBeamAttachPosition()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)magnetTarget))
		{
			return;
		}
		Vector3 val = default(Vector3);
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < 6; i++)
		{
			float num = 0.5f;
			((Vector3)(ref val))._002Ector(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num));
			if (Physics.Raycast(((Component)this).transform.position, magnetTarget.position - ((Component)this).transform.position + val, ref val2, 1f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject())))
			{
				Transform highestParentWithRigidbody = GetHighestParentWithRigidbody(((Component)((RaycastHit)(ref val2)).collider).transform);
				PhysGrabObjectCollider component = ((Component)((Component)((RaycastHit)(ref val2)).collider).transform).GetComponent<PhysGrabObjectCollider>();
				if (Object.op_Implicit((Object)(object)component) && (Object)(object)highestParentWithRigidbody == (Object)(object)((Component)magnetTargetPhysGrabObject).transform)
				{
					Vector3 val3 = ((Component)this).transform.position - ((RaycastHit)(ref val2)).point;
					Vector3 normalized = ((Vector3)(ref val3)).normalized;
					NewRayHitPoint(((RaycastHit)(ref val2)).point, ((Component)highestParentWithRigidbody).GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					Vector3 val4 = ((RaycastHit)(ref val2)).point + normalized * 0.5f;
					attachPoint = magnetTarget.InverseTransformPoint(val4);
					attachPointFound = true;
				}
			}
		}
	}

	public void SetTumbleTarget(PlayerTumble tumble)
	{
		if (!SemiFunc.IsMultiplayer())
		{
			SetTumbleTargetRPC(tumble.photonView.ViewID);
		}
		else if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("SetTumbleTargetRPC", (RpcTarget)0, new object[1] { tumble.photonView.ViewID });
		}
	}

	[PunRPC]
	public void SetTumbleTargetRPC(int _photonViewID)
	{
		PhotonView val = PhotonView.Find(_photonViewID);
		if (Object.op_Implicit((Object)(object)val))
		{
			PlayerTumble component = ((Component)val).GetComponent<PlayerTumble>();
			if (Object.op_Implicit((Object)(object)component))
			{
				playerTumbleTarget = component;
			}
		}
	}
}
