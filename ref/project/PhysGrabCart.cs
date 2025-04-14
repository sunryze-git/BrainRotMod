using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhysGrabCart : MonoBehaviour
{
	public enum State
	{
		Locked,
		Dragged,
		Handled
	}

	public bool isSmallCart;

	public GameObject smallCartHurtCollider;

	internal State currentState;

	internal State previousState = State.Handled;

	public TextMeshPro displayText;

	public Transform handlePoint;

	private PhysGrabObject physGrabObject;

	internal Rigidbody rb;

	public float stabilizationForce = 100f;

	private Vector3 hitPoint;

	private PhotonView photonView;

	internal bool cartActive;

	private bool cartActivePrevious;

	public GameObject buttonObject;

	private List<Collider> capsuleColliders = new List<Collider>();

	private List<Collider> cartInside = new List<Collider>();

	public PhysicMaterial physMaterialSlippery;

	public PhysicMaterial physMaterialSticky;

	public PhysicMaterial physMaterialALilSlippery;

	public PhysicMaterial physMaterialNormal;

	private Vector3 velocityRef;

	internal bool cartBeingPulled;

	private float playerInteractionTimer;

	private PhysGrabObjectGrabArea physGrabObjectGrabArea;

	private MeshRenderer cartMesh;

	public MeshRenderer[] grabMesh;

	private List<Material> grabMaterial = new List<Material>();

	[Space]
	public PhysGrabInCart physGrabInCart;

	internal Transform inCart;

	internal Vector3 actualVelocity;

	internal Vector3 actualVelocityLastPosition;

	private Vector3 lastPosition;

	internal List<PhysGrabObject> itemsInCart = new List<PhysGrabObject>();

	internal int itemsInCartCount;

	internal int haulCurrent;

	private float objectInCartCheckTimer = 0.5f;

	private int haulPrevious;

	private float haulUpdateEffectTimer;

	private bool deductedFromHaul;

	private bool resetHaulText;

	private Color originalHaulColor;

	public Sound soundHaulIncrease;

	public Sound soundHaulDecrease;

	[Space]
	public Sound soundLocked;

	public Sound soundDragged;

	public Sound soundHandled;

	private bool thirtyFPSUpdate;

	private float thirtyFPSUpdateTimer;

	private float autoTurnOffTimer;

	private float draggedTimer;

	public Transform cartGrabPoint;

	private ItemEquippable itemEquippable;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		originalHaulColor = ((Graphic)displayText).color;
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		rb.mass = 8f;
		inCart = ((Component)this).transform.Find("In Cart");
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			Transform val2 = val.Find("Semi Box Collider");
			if (((Object)val).name.Contains("Inside"))
			{
				cartInside.Add(((Component)val2).GetComponent<Collider>());
			}
			if (Object.op_Implicit((Object)(object)val2) && (((Component)((Component)val2).GetComponent<Collider>()).gameObject.layer == LayerMask.NameToLayer("PhysGrabObject") || ((Component)((Component)val2).GetComponent<Collider>()).gameObject.layer == LayerMask.NameToLayer("Default")))
			{
				((Component)((Component)val2).GetComponent<Collider>()).gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectCart");
			}
			if (((Object)val).name.Contains("Cart Mesh"))
			{
				cartMesh = ((Component)val).GetComponent<MeshRenderer>();
			}
			if (((Object)val).name.Contains("Cart Wall Collider"))
			{
				((Component)val2).GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
			}
			if (((Object)val).name.Contains("Capsule"))
			{
				capsuleColliders.Add(((Component)val).GetComponent<Collider>());
			}
		}
		photonView = ((Component)this).GetComponent<PhotonView>();
		physGrabObjectGrabArea = ((Component)this).GetComponent<PhysGrabObjectGrabArea>();
		MeshRenderer[] array = grabMesh;
		foreach (MeshRenderer val3 in array)
		{
			grabMaterial.Add(((Renderer)val3).material);
		}
	}

	private void ObjectsInCart()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.PlayerNearestDistance(((Component)this).transform.position) > 12f)
		{
			return;
		}
		if (objectInCartCheckTimer > 0f)
		{
			objectInCartCheckTimer -= Time.deltaTime;
		}
		else
		{
			Collider[] array = Physics.OverlapBox(inCart.position, inCart.localScale / 2f, inCart.rotation);
			itemsInCart.Clear();
			haulPrevious = haulCurrent;
			itemsInCartCount = 0;
			haulCurrent = 0;
			Collider[] array2 = array;
			foreach (Collider val in array2)
			{
				if (((Component)val).gameObject.layer != LayerMask.NameToLayer("PhysGrabObject"))
				{
					continue;
				}
				PhysGrabObject componentInParent = ((Component)val).GetComponentInParent<PhysGrabObject>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !itemsInCart.Contains(componentInParent))
				{
					itemsInCart.Add(componentInParent);
					ValuableObject componentInParent2 = ((Component)val).GetComponentInParent<ValuableObject>();
					if (Object.op_Implicit((Object)(object)componentInParent2))
					{
						haulCurrent += (int)componentInParent2.dollarValueCurrent;
					}
					itemsInCartCount++;
				}
			}
			objectInCartCheckTimer = 0.5f;
		}
		if (haulPrevious != haulCurrent)
		{
			haulUpdateEffectTimer = 0.3f;
			if (haulCurrent > haulPrevious)
			{
				deductedFromHaul = false;
				soundHaulIncrease.Play(displayText.transform.position);
			}
			else
			{
				deductedFromHaul = true;
				soundHaulDecrease.Play(displayText.transform.position);
			}
			haulPrevious = haulCurrent;
		}
		if (haulUpdateEffectTimer > 0f)
		{
			haulUpdateEffectTimer -= Time.deltaTime;
			haulUpdateEffectTimer = Mathf.Max(0f, haulUpdateEffectTimer);
			Color color = Color.white;
			if (deductedFromHaul)
			{
				color = Color.red;
			}
			((Graphic)displayText).color = color;
			if (thirtyFPSUpdate)
			{
				((TMP_Text)displayText).text = GlitchyText();
			}
			resetHaulText = false;
		}
		else if (!resetHaulText)
		{
			((Graphic)displayText).color = originalHaulColor;
			SetHaulText();
			resetHaulText = true;
		}
	}

	private void SetHaulText()
	{
		string text = "<color=#bd4300>$</color>";
		((TMP_Text)displayText).text = text + SemiFunc.DollarGetString(Mathf.Max(0, haulCurrent));
	}

	private void ThirtyFPS()
	{
		if (thirtyFPSUpdateTimer > 0f)
		{
			thirtyFPSUpdateTimer -= Time.deltaTime;
			thirtyFPSUpdateTimer = Mathf.Max(0f, thirtyFPSUpdateTimer);
		}
		else
		{
			thirtyFPSUpdate = true;
			thirtyFPSUpdateTimer = 1f / 30f;
		}
	}

	private string GlitchyText()
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			bool flag = false;
			if (Random.Range(0, 4) == 0 && i <= 5)
			{
				text += "TAX";
				i += 2;
				flag = true;
			}
			if (Random.Range(0, 3) == 0 && !flag)
			{
				text += "$";
				flag = true;
			}
			if (!flag)
			{
				text += Random.Range(0, 10);
			}
		}
		return text;
	}

	private void StateMessages()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.RunIsShop() && physGrabObject.grabbedLocal)
		{
			if (currentState == State.Handled)
			{
				Color color = default(Color);
				((Color)(ref color))._002Ector(0.2f, 0.8f, 0.1f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: STRONG", color);
			}
			if (currentState == State.Dragged)
			{
				Color color2 = default(Color);
				((Color)(ref color2))._002Ector(1f, 0.46f, 0f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: WEAK", color2);
			}
		}
	}

	private void SmallCartLogic()
	{
		if (!isSmallCart || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (itemEquippable.isEquipping)
		{
			if (!smallCartHurtCollider.activeSelf)
			{
				smallCartHurtCollider.SetActive(true);
			}
		}
		else if (!smallCartHurtCollider.activeSelf)
		{
			smallCartHurtCollider.SetActive(false);
		}
		if (currentState == State.Locked)
		{
			CartMassOverride(8f);
			physGrabObject.OverrideMaterial(physMaterialSticky);
		}
	}

	private void Update()
	{
		if (Object.op_Implicit((Object)(object)itemEquippable) && itemEquippable.isUnequipping)
		{
			return;
		}
		ThirtyFPS();
		ObjectsInCart();
		StateMessages();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			AutoTurnOff();
			StateLogic();
			if (playerInteractionTimer > 0f)
			{
				playerInteractionTimer -= Time.deltaTime;
			}
			thirtyFPSUpdate = false;
		}
	}

	private void FixedUpdate()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)physGrabObjectGrabArea) && physGrabObjectGrabArea.listOfAllGrabbers.Count > 0)
		{
			CartSteer();
		}
		else
		{
			cartBeingPulled = false;
		}
		if (!LevelGenerator.Instance.Generated || (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		Quaternion rotation;
		if (rb.IsSleeping())
		{
			rotation = ((Component)this).transform.rotation;
			if (!(Mathf.Abs(((Quaternion)(ref rotation)).eulerAngles.x) > 0.05f))
			{
				rotation = ((Component)this).transform.rotation;
				if (!(Mathf.Abs(((Quaternion)(ref rotation)).eulerAngles.z) > 0.05f))
				{
					goto IL_0189;
				}
			}
			rotation = ((Component)this).transform.rotation;
			Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			rb.MoveRotation(Quaternion.Euler(eulerAngles));
			rb.angularVelocity = new Vector3(0f, rb.angularVelocity.y, 0f);
		}
		else if (!rb.isKinematic)
		{
			rotation = ((Component)this).transform.rotation;
			Vector3 eulerAngles2 = ((Quaternion)(ref rotation)).eulerAngles;
			eulerAngles2.x = 0f;
			eulerAngles2.z = 0f;
			rb.MoveRotation(Quaternion.Euler(eulerAngles2));
			rb.angularVelocity = new Vector3(0f, rb.angularVelocity.y, 0f);
		}
		goto IL_0189;
		IL_0189:
		actualVelocity = (((Component)this).transform.position - actualVelocityLastPosition) / Time.fixedDeltaTime;
		actualVelocityLastPosition = ((Component)this).transform.position;
	}

	private void AutoTurnOff()
	{
		if (physGrabObject.playerGrabbing.Count <= 0)
		{
			cartActive = false;
		}
	}

	private void CartMassOverride(float mass)
	{
		physGrabObject.OverrideMass(mass);
	}

	private void CartSteer()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		List<PhysGrabber> listOfAllGrabbers = physGrabObjectGrabArea.listOfAllGrabbers;
		foreach (PhysGrabber item in listOfAllGrabbers)
		{
			if (Object.op_Implicit((Object)(object)item))
			{
				if (item.isLocal)
				{
					TutorialDirector.instance.playerUsedCart = true;
				}
				item.OverrideGrabPoint(cartGrabPoint);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		float num9 = default(float);
		Vector3 val9 = default(Vector3);
		foreach (PhysGrabber item2 in listOfAllGrabbers)
		{
			if (!Object.op_Implicit((Object)(object)item2))
			{
				continue;
			}
			Bounds bounds = ((Collider)((Component)inCart).GetComponent<BoxCollider>()).bounds;
			if (!((Bounds)(ref bounds)).Contains(((Component)item2).transform.position))
			{
				float num = 1f;
				float num2 = 1f;
				Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
				CartMassOverride(4f);
				if ((Object)(object)item2 == (Object)(object)PhysGrabber.instance)
				{
					SemiFunc.PhysGrabberLocalChangeAlpha(0.1f);
				}
				if (!cartActive && item2.initialPressTimer > 0f)
				{
					cartActive = true;
				}
				if (!cartActive || (Object)(object)item2 != (Object)(object)listOfAllGrabbers[0])
				{
					break;
				}
				cartBeingPulled = true;
				item2.physGrabForcesDisabled = true;
				float num3 = 2f;
				float num4 = 2.5f;
				if (isSmallCart)
				{
					num3 = 1.5f;
					num4 = 2f;
				}
				float num5 = 5f;
				Vector3 val = PlayerController.instance.rb.velocity;
				if (!item2.isLocal)
				{
					val = item2.playerAvatar.rbVelocityRaw;
				}
				bool flag = Vector3.Dot(val, ((Component)this).transform.forward) > 0f;
				if (item2.playerAvatar.isSprinting)
				{
					num5 = 7f;
				}
				if (item2.playerAvatar.isSprinting && flag)
				{
					num3 = 3f;
					num4 = 4f;
				}
				float num6 = Mathf.Clamp(Vector3.Dot(component.velocity, ((Component)item2).transform.forward) / num5, 0f, 1f);
				float num7 = Mathf.Lerp(num3, num4, num6);
				Vector3 val2 = ((Component)item2).transform.rotation * Vector3.back;
				Vector3 val3 = ((Component)item2.playerAvatar).transform.position - val2 * num7;
				float num8 = Mathf.Clamp(Vector3.Distance(((Component)this).transform.position, val3 / 1f), 0f, 1f);
				Vector3 val4 = val3 - ((Component)this).transform.position;
				Vector3 val5 = ((Vector3)(ref val4)).normalized * 5f * num8;
				val5 = Vector3.ClampMagnitude(val5, 5f);
				float y = component.velocity.y;
				component.velocity = Vector3.MoveTowards(component.velocity, val5, num8 * 2f);
				component.velocity = new Vector3(component.velocity.x, y, component.velocity.z) * num;
				component.velocity = Vector3.ClampMagnitude(component.velocity, 5f);
				Quaternion val6 = Quaternion.LookRotation(((Component)item2).transform.position - ((Component)this).transform.position, Vector3.up);
				val6 = Quaternion.Euler(0f, ((Quaternion)(ref val6)).eulerAngles.y + 180f, 0f);
				Quaternion rotation = component.rotation;
				Quaternion val7 = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
				Quaternion val8 = val6 * Quaternion.Inverse(val7);
				((Quaternion)(ref val8)).ToAngleAxis(ref num9, ref val9);
				if (num9 > 180f)
				{
					num9 -= 360f;
				}
				float num10 = Mathf.Clamp(Mathf.Abs(num9) / 180f, 0.2f, 1f) * 20f;
				num10 = Mathf.Clamp(num10, 0f, 4f);
				Vector3 val10 = MathF.PI / 180f * num9 * ((Vector3)(ref val9)).normalized * num10;
				val10 = Vector3.ClampMagnitude(val10, 4f);
				component.angularVelocity = Vector3.MoveTowards(component.angularVelocity, val10, num10) * num2;
				component.angularVelocity = Vector3.ClampMagnitude(component.angularVelocity, 4f);
			}
		}
	}

	private void StateLogic()
	{
		if (LevelGenerator.Instance.Generated)
		{
			if (cartActive != cartActivePrevious)
			{
				cartActivePrevious = cartActive;
			}
			if (physGrabObject.playerGrabbing.Count > 0)
			{
				draggedTimer += Time.deltaTime;
			}
			else
			{
				draggedTimer = 0f;
			}
			if (cartActive)
			{
				currentState = State.Handled;
			}
			else if (draggedTimer > 0.25f)
			{
				currentState = State.Dragged;
			}
			else
			{
				currentState = State.Locked;
			}
			if (currentState != previousState)
			{
				previousState = currentState;
				StateSwitch(currentState);
			}
		}
	}

	private void StateSwitch(State _state)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("StateSwitchRPC", (RpcTarget)0, new object[1] { _state });
		}
		else
		{
			StateSwitchRPC(_state);
		}
	}

	[PunRPC]
	private void StateSwitchRPC(State _state)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		currentState = _state;
		if (currentState == State.Locked)
		{
			soundLocked.Play(((Component)this).transform.position);
			((Renderer)cartMesh).material.SetColor("_EmissionColor", Color.red);
			foreach (Material item in grabMaterial)
			{
				Color red = Color.red;
				item.SetColor("_EmissionColor", red);
				item.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider capsuleCollider in capsuleColliders)
			{
				capsuleCollider.material = physMaterialNormal;
			}
			{
				foreach (Collider item2 in cartInside)
				{
					item2.material = physMaterialNormal;
				}
				return;
			}
		}
		if (currentState == State.Dragged)
		{
			soundDragged.Play(((Component)this).transform.position);
			Material material = ((Renderer)cartMesh).material;
			Color val = default(Color);
			((Color)(ref val))._002Ector(1f, 0.46f, 0f);
			material.SetColor("_EmissionColor", val);
			foreach (Material item3 in grabMaterial)
			{
				item3.SetColor("_EmissionColor", val);
				item3.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider capsuleCollider2 in capsuleColliders)
			{
				capsuleCollider2.material = physMaterialALilSlippery;
			}
			{
				foreach (Collider item4 in cartInside)
				{
					item4.material = physMaterialALilSlippery;
				}
				return;
			}
		}
		soundHandled.Play(((Component)this).transform.position);
		((Renderer)cartMesh).material.SetColor("_EmissionColor", Color.green);
		int num = 0;
		foreach (Material item5 in grabMaterial)
		{
			item5.SetColor("_EmissionColor", Color.green);
			if (num == 1)
			{
				item5.mainTextureOffset = new Vector2(0.5f, 0f);
			}
			num++;
		}
		foreach (Collider capsuleCollider3 in capsuleColliders)
		{
			capsuleCollider3.material = physMaterialSlippery;
		}
		foreach (Collider item6 in cartInside)
		{
			item6.material = SemiFunc.PhysicMaterialPhysGrabObject();
		}
	}
}
