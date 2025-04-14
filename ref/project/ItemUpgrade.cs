using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ItemUpgrade : MonoBehaviour
{
	public UnityEvent upgradeEvent;

	public bool isPlayerUpgrade;

	public ColorPresets colorPreset;

	internal Color beamColor;

	private float checkTimer;

	private Transform magnetTarget;

	internal PhysGrabObject magnetTargetPhysGrabObject;

	internal Rigidbody magnetTargetRigidbody;

	internal bool magnetActive;

	private Rigidbody rb;

	private bool attachPointFound;

	private Vector3 attachPoint;

	private float springConstant = 50f;

	private float dampingCoefficient = 5f;

	private float newAttachPointTimer;

	internal bool itemActivated;

	private PhotonView photonView;

	private Vector3 rayHitPosition;

	private Vector3 animatedRayHitPosition;

	private LineBetweenTwoPoints lineBetweenTwoPoints;

	public Transform lineStartPoint;

	private float rayTimer;

	private Transform prevMagnetTarget;

	private Transform droneTransform;

	private PhysGrabObjectImpactDetector impactDetector;

	private ItemAttributes itemAttributes;

	private Transform particleEffects;

	private Transform onSwitchTransform;

	internal Vector3 connectionPoint;

	internal Transform lastPlayerToTouch;

	private PhysGrabObject physGrabObject;

	private PhysicMaterial physicMaterialOriginal;

	private ItemToggle itemToggle;

	internal PlayerAvatar playerAvatarTarget;

	private bool targetIsPlayer;

	internal bool targetIsLocalPlayer;

	private Camera cameraMain;

	private bool upgradeDone;

	private bool pushedOrPulled;

	private ITargetingCondition customTargetingCondition;

	private void Start()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		customTargetingCondition = ((Component)this).GetComponent<ITargetingCondition>();
		particleEffects = ((Component)this).transform.Find("Particle Effects");
		cameraMain = Camera.main;
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		lineBetweenTwoPoints = ((Component)this).GetComponent<LineBetweenTwoPoints>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		beamColor = colorPreset.GetColorDark();
		Collider[] componentsInChildren = ((Component)this).GetComponentsInChildren<Collider>();
		int num = 0;
		if (num < componentsInChildren.Length)
		{
			Collider val = componentsInChildren[num];
			physicMaterialOriginal = val.material;
		}
		if (SemiFunc.RunIsShop())
		{
			((Behaviour)itemToggle).enabled = false;
		}
		physGrabObject.clientNonKinematic = true;
	}

	private void Update()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.playerGrabbing.Count > 0)
		{
			bool flag = false;
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if (item.isRotating)
				{
					flag = true;
				}
			}
			float dist = 0.5f;
			if (physGrabObject.grabbed)
			{
				if (physGrabObject.grabbedLocal && !pushedOrPulled)
				{
					PhysGrabber.instance.OverrideGrabDistance(dist);
				}
				if (PhysGrabber.instance.isPulling || PhysGrabber.instance.isPushing)
				{
					pushedOrPulled = true;
				}
			}
			else
			{
				pushedOrPulled = false;
			}
			if (!flag && !pushedOrPulled)
			{
				Quaternion turnX = Quaternion.Euler(45f, 0f, 0f);
				Quaternion turnY = Quaternion.Euler(45f, 180f, 0f);
				Quaternion identity = Quaternion.identity;
				physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		else
		{
			pushedOrPulled = false;
		}
		TargetingLogic();
		PlayerUpgradeLogic();
	}

	private void PlayerUpgrade()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!upgradeDone)
		{
			upgradeEvent.Invoke();
			particleEffects.parent = null;
			((Component)particleEffects).gameObject.SetActive(true);
			PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID);
			if (playerAvatar.isLocal)
			{
				StatsUI.instance.Fetch();
				StatsUI.instance.ShowStats();
				CameraGlitch.Instance.PlayUpgrade();
			}
			else
			{
				GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, ((Component)this).transform.position, 0.2f);
			}
			if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
			{
				playerAvatar.playerHealth.MaterialEffectOverride(PlayerHealth.Effect.Upgrade);
			}
			StatsManager.instance.itemsPurchased[itemAttributes.item.itemAssetName] = Mathf.Max(StatsManager.instance.itemsPurchased[itemAttributes.item.itemAssetName] - 1, 0);
			impactDetector.DestroyObject(effects: false);
			upgradeDone = true;
		}
	}

	private void PlayerUpgradeLogic()
	{
		if (isPlayerUpgrade && itemToggle.toggleState)
		{
			PlayerUpgrade();
		}
	}

	private void TargetingLogic()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		if (isPlayerUpgrade)
		{
			return;
		}
		if (magnetActive && !physGrabObject.grabbed)
		{
			DeactivateMagnet();
		}
		if (itemActivated && magnetActive)
		{
			if (Object.op_Implicit((Object)(object)magnetTarget) && !((Component)magnetTarget).gameObject.activeSelf)
			{
				magnetActive = false;
				magnetTarget = null;
			}
			if (Object.op_Implicit((Object)(object)magnetTarget))
			{
				if (rayHitPosition != Vector3.zero && !targetIsPlayer)
				{
					animatedRayHitPosition = Vector3.Lerp(animatedRayHitPosition, rayHitPosition, Time.deltaTime * 10f);
					lineBetweenTwoPoints.DrawLine(lineStartPoint.position, magnetTarget.TransformPoint(animatedRayHitPosition));
					connectionPoint = magnetTarget.TransformPoint(animatedRayHitPosition);
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
						Vector3 val = default(Vector3);
						((Vector3)(ref val))._002Ector(0f, -0.5f, 0f);
						if (!targetIsLocalPlayer)
						{
							lineBetweenTwoPoints.DrawLine(lineStartPoint.position, magnetTarget.position + val);
							connectionPoint = magnetTarget.position + val;
						}
						else
						{
							Vector3 point = ((Component)cameraMain).transform.position + val;
							lineBetweenTwoPoints.DrawLine(lineStartPoint.position, point);
							connectionPoint = point;
						}
					}
				}
			}
		}
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
		if (physGrabObject.grabbed)
		{
			checkTimer += Time.deltaTime;
			if (checkTimer > 0.5f)
			{
				if (SphereCheck())
				{
					ActivateMagnet();
				}
				checkTimer = 0f;
			}
			return;
		}
		if (!attachPointFound)
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
			return;
		}
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

	private void MagnetLogic()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (isPlayerUpgrade || (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient) || !itemActivated || !magnetActive)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)magnetTarget))
		{
			DeactivateMagnet();
			return;
		}
		if (attachPointFound)
		{
			Vector3 val = magnetTarget.TransformPoint(attachPoint) - ((Component)this).transform.position;
			Vector3 val2 = springConstant * val;
			Vector3 velocity = rb.velocity;
			Vector3 val3 = (0f - dampingCoefficient) * velocity;
			Vector3.ClampMagnitude(val2 + val3, 20f);
		}
		else
		{
			Vector3 val4 = magnetTarget.position - ((Component)this).transform.position;
			_ = ((Vector3)(ref val4)).magnitude;
		}
		if (targetIsPlayer)
		{
			if (Vector3.Distance(((Component)this).transform.position, magnetTarget.position) > 1.8f)
			{
				DeactivateMagnet();
			}
		}
		else if (Vector3.Distance(((Component)this).transform.position, magnetTarget.position) > 1f)
		{
			DeactivateMagnet();
		}
		if (Object.op_Implicit((Object)(object)magnetTargetPhysGrabObject))
		{
			magnetTargetPhysGrabObject.OverrideZeroGravity();
			magnetTargetPhysGrabObject.OverrideMass(0.1f);
			magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky());
			Rigidbody obj = magnetTargetRigidbody;
			Vector3 val5 = ((Component)this).transform.position - magnetTarget.position;
			obj.AddForce(((Vector3)(ref val5)).normalized * 1f, (ForceMode)0);
		}
	}

	private void FixedUpdate()
	{
		MagnetLogic();
	}

	private void DeactivateMagnet()
	{
		attachPointFound = false;
		MagnetActiveToggle(toggleBool: false);
	}

	private void ActivateMagnet()
	{
		MagnetActiveToggle(toggleBool: true);
	}

	private void ButtonToggleLogic(bool activated)
	{
		if (!activated && magnetActive)
		{
			DeactivateMagnet();
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
		magnetActive = activated;
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
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		targetIsPlayer = false;
		targetIsLocalPlayer = false;
		if (Object.op_Implicit((Object)(object)newMagnetTarget))
		{
			magnetTargetPhysGrabObject = ((Component)newMagnetTarget).GetComponent<PhysGrabObject>();
			if (colliderID != -1)
			{
				magnetTarget = ((Component)newMagnetTarget).GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
			}
			else
			{
				magnetTarget = newMagnetTarget;
			}
			animatedRayHitPosition = rayHitPosition;
			rayHitPosition = magnetTarget.InverseTransformPoint(newRayHitPosition);
			magnetTargetRigidbody = ((Component)GetHighestParentWithRigidbody(magnetTarget)).GetComponent<Rigidbody>();
			return;
		}
		magnetTargetPhysGrabObject = ((Component)PhotonView.Find(photonViewId)).gameObject.GetComponent<PhysGrabObject>();
		if (colliderID != -1)
		{
			magnetTarget = ((Component)PhotonView.Find(photonViewId)).gameObject.GetComponent<PhysGrabObject>().FindColliderFromID(colliderID);
		}
		else
		{
			targetIsPlayer = true;
			magnetTarget = ((Component)PhotonView.Find(photonViewId)).GetComponent<PlayerAvatar>().PlayerVisionTarget.VisionTransform;
			if (((Component)PhotonView.Find(photonViewId)).GetComponent<PlayerAvatar>().isLocal)
			{
				targetIsLocalPlayer = true;
			}
			playerAvatarTarget = ((Component)PhotonView.Find(photonViewId)).GetComponent<PlayerAvatar>();
		}
		animatedRayHitPosition = rayHitPosition;
		rayHitPosition = magnetTarget.InverseTransformPoint(newRayHitPosition);
		magnetTargetRigidbody = ((Component)GetHighestParentWithRigidbody(magnetTarget)).GetComponent<Rigidbody>();
	}

	private void NewRayHitPoint(Vector3 newAttachPoint, int photonViewId, int colliderID, Transform newMagnetTarget)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, 0.75f);
		float num = 10000f;
		Collider[] array2 = array;
		RaycastHit val2 = default(RaycastHit);
		foreach (Collider val in array2)
		{
			Transform highestParentWithRigidbody = GetHighestParentWithRigidbody(((Component)val).transform);
			PhysGrabObjectCollider component = ((Component)val).GetComponent<PhysGrabObjectCollider>();
			bool flag = false;
			if ((Object)(object)highestParentWithRigidbody != (Object)null)
			{
				PhysGrabObjectImpactDetector component2 = ((Component)highestParentWithRigidbody).GetComponent<PhysGrabObjectImpactDetector>();
				if ((Object)(object)component2 != (Object)null && component2.isValuable)
				{
					flag = true;
				}
			}
			bool flag2 = true;
			if (customTargetingCondition != null && (Object)(object)highestParentWithRigidbody != (Object)null)
			{
				flag2 = customTargetingCondition.CustomTargetingCondition(((Component)highestParentWithRigidbody).gameObject);
			}
			bool flag3 = false;
			if (!flag3)
			{
				flag3 = !flag;
			}
			if (!((Object)(object)component != (Object)null && (Object)(object)highestParentWithRigidbody != (Object)(object)((Component)this).transform && (Object)(object)highestParentWithRigidbody != (Object)null && flag3 && flag2))
			{
				continue;
			}
			float num2 = Vector3.Distance(((Component)this).transform.position, ((Component)val).transform.position);
			if (num2 < num)
			{
				bool flag4 = false;
				if (Physics.Raycast(((Component)this).transform.position, ((Component)val).transform.position - ((Component)this).transform.position, ref val2, 1f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())) && (Object)(object)((Component)((RaycastHit)(ref val2)).collider).transform != (Object)(object)((Component)val).transform && (Object)(object)((Component)((RaycastHit)(ref val2)).collider).transform != (Object)(object)((Component)this).transform)
				{
					flag4 = true;
				}
				if (!flag4)
				{
					num = num2;
					magnetTarget = ((Component)val).transform;
					magnetTargetPhysGrabObject = ((Component)highestParentWithRigidbody).GetComponent<PhysGrabObject>();
					magnetTargetRigidbody = ((Component)highestParentWithRigidbody).GetComponent<Rigidbody>();
					Vector3 position = ((Component)val).transform.position;
					NewRayHitPoint(position, ((Component)highestParentWithRigidbody).GetComponent<PhotonView>().ViewID, component.colliderID, highestParentWithRigidbody);
					attachPoint = rayHitPosition;
					result = true;
				}
			}
		}
		return result;
	}

	private void FindBeamAttachPosition()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
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
				if ((Object)(object)component != (Object)null && (Object)(object)highestParentWithRigidbody == (Object)(object)((Component)magnetTargetPhysGrabObject).transform)
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
}
