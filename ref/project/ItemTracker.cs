using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTracker : MonoBehaviour
{
	public enum TrackerType
	{
		Valuable,
		Extraction
	}

	public TrackerType trackerType;

	private float timer;

	private Transform currentTarget;

	private PhysGrabObject currentTargetPhysGrabObject;

	private Rigidbody rb;

	public Transform nozzleTransform;

	private PhysGrabObject physGrabObject;

	public MeshRenderer meshRenderer;

	public AnimationCurve animationCurve;

	private float blipTimer;

	public Sound soundBleep;

	public Sound digitSwap;

	public Sound soundTargetFound;

	public Sound soundTargetLost;

	private ItemToggle itemToggle;

	private ItemBattery itemBattery;

	private PhotonView photonView;

	private bool currentToggleState;

	public Light nozzleLight;

	public MeshRenderer display;

	public TextMeshPro displayText;

	private int prevDigit;

	private float changeDigitTimer;

	private float displayOverrideTimer;

	public Light displayLight;

	private bool hasTarget;

	public Color colorBleep;

	public Color colorBleepOff;

	public Color colorTargetFound;

	public Color colorScreenNeutral;

	private Vector3 targetPosition;

	private float batteryOutTimer;

	private void Start()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		rb = ((Component)this).GetComponent<Rigidbody>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.black);
		((Behaviour)nozzleLight).enabled = false;
		nozzleLight.intensity = 0f;
	}

	private void ValuableTarget()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (trackerType != 0)
		{
			return;
		}
		Vector3 position = nozzleTransform.position;
		hasTarget = false;
		float num = 15f;
		if (!Object.op_Implicit((Object)(object)currentTarget))
		{
			num = 30f;
		}
		Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, num);
		float num2 = float.MaxValue;
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ValuableObject componentInParent = ((Component)array2[i]).gameObject.GetComponentInParent<ValuableObject>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.discovered)
			{
				PhysGrabObject component = ((Component)componentInParent).GetComponent<PhysGrabObject>();
				PhysGrabObjectImpactDetector component2 = ((Component)componentInParent).GetComponent<PhysGrabObjectImpactDetector>();
				float num3 = Vector3.Distance(position, component.midPoint);
				if (num3 < num2 && !component.grabbed && !component2.inCart)
				{
					num2 = num3;
					currentTarget = ((Component)component).transform;
					currentTargetPhysGrabObject = component;
					hasTarget = true;
				}
			}
		}
		if (hasTarget)
		{
			SetTarget(currentTargetPhysGrabObject.photonView.ViewID);
		}
	}

	private void ExtractionTarget()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (trackerType == TrackerType.Extraction)
		{
			hasTarget = false;
			ExtractionPoint extractionPoint = SemiFunc.ExtractionPointGetNearestNotActivated(nozzleTransform.position);
			if (Object.op_Implicit((Object)(object)extractionPoint))
			{
				currentTarget = ((Component)extractionPoint).transform;
				hasTarget = true;
			}
			if (hasTarget)
			{
				SetTarget(((Component)currentTarget).GetComponent<PhotonView>().ViewID);
			}
		}
	}

	private void FindATarget()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && !(itemBattery.batteryLife <= 0f))
		{
			timer += Time.deltaTime;
			if (timer > 2f)
			{
				ValuableTarget();
				ExtractionTarget();
				timer = 0f;
			}
		}
	}

	private void AnimateEmissionToBlack()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!itemToggle.toggleState)
		{
			Color color = ((Renderer)meshRenderer).material.GetColor("_EmissionColor");
			if (color != Color.black)
			{
				((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.Lerp(color, Color.black, Time.deltaTime * 20f));
			}
			if (nozzleLight.intensity > 0f)
			{
				nozzleLight.intensity = Mathf.Lerp(nozzleLight.intensity, 0f, Time.deltaTime * 10f);
			}
			else
			{
				((Behaviour)nozzleLight).enabled = false;
			}
		}
	}

	private void PhysGrabOverrides()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (physGrabObject.grabbed && physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(0.8f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (physGrabObject.grabbed)
		{
			Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			physGrabObject.TurnXYZ(turnX, turnY, identity);
			physGrabObject.OverrideTorqueStrengthX(2f);
			if (Object.op_Implicit((Object)(object)currentTarget) && itemToggle.toggleState)
			{
				physGrabObject.OverrideTorqueStrengthY(0.1f);
			}
			physGrabObject.OverrideGrabVerticalPosition(-0.2f);
		}
		else if (itemToggle.toggleState)
		{
			itemToggle.ToggleItem(toggle: false);
		}
	}

	private void DisplayLogic()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)display).gameObject.activeSelf)
		{
			Vector2 textureOffset = ((Renderer)display).material.GetTextureOffset("_MainTex");
			textureOffset.y += Time.deltaTime * 2f;
			((Renderer)display).material.SetTextureOffset("_MainTex", textureOffset);
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				itemBattery.batteryLife -= Time.deltaTime * 0.5f;
			}
		}
		else if (((TMP_Text)displayText).text != "--")
		{
			((TMP_Text)displayText).text = "--";
		}
		if (displayOverrideTimer >= 0f)
		{
			displayOverrideTimer -= Time.deltaTime;
			if (displayOverrideTimer <= 0f)
			{
				((TMP_Text)displayText).text = "--";
				Color color = colorScreenNeutral;
				color.a = 0.2f;
				((Graphic)displayText).color = colorScreenNeutral;
				((Renderer)display).material.color = color;
				displayLight.color = colorScreenNeutral;
			}
		}
		if (trackerType == TrackerType.Valuable && Object.op_Implicit((Object)(object)currentTargetPhysGrabObject))
		{
			targetPosition = currentTargetPhysGrabObject.midPoint;
		}
		if (trackerType == TrackerType.Extraction && Object.op_Implicit((Object)(object)currentTarget))
		{
			targetPosition = currentTarget.position;
		}
		if (changeDigitTimer <= 0f && displayOverrideTimer <= 0f)
		{
			if (hasTarget && ((Component)display).gameObject.activeSelf)
			{
				int num = Mathf.RoundToInt(Vector3.Distance(nozzleTransform.position, targetPosition));
				if (num != prevDigit)
				{
					changeDigitTimer = 1f;
					digitSwap.Play(((Component)display).transform.position);
					prevDigit = num;
				}
				((TMP_Text)displayText).text = num.ToString();
			}
			else
			{
				((TMP_Text)displayText).text = "--";
			}
		}
		else
		{
			changeDigitTimer -= Time.deltaTime;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (itemToggle.toggleState)
		{
			if (!((Component)display).gameObject.activeSelf)
			{
				((Component)display).gameObject.SetActive(true);
			}
		}
		else if (((Component)display).gameObject.activeSelf)
		{
			((Component)display).gameObject.SetActive(false);
		}
	}

	private void TargetLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!itemToggle.toggleState)
		{
			hasTarget = false;
			currentTarget = null;
			displayOverrideTimer = 0f;
			return;
		}
		if (trackerType == TrackerType.Valuable)
		{
			if (Object.op_Implicit((Object)(object)currentTarget) && ((Component)currentTarget).GetComponent<ValuableObject>().discovered && hasTarget)
			{
				CurrentTargetUpdate(_found: true);
				currentTarget = null;
				hasTarget = false;
			}
			if (!Object.op_Implicit((Object)(object)currentTarget) && hasTarget && physGrabObject.grabbed)
			{
				CurrentTargetUpdate(_found: false);
				hasTarget = false;
			}
		}
		if (trackerType == TrackerType.Extraction)
		{
			if (Object.op_Implicit((Object)(object)currentTarget))
			{
				hasTarget = true;
			}
			else
			{
				hasTarget = false;
			}
		}
	}

	private void Update()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabOverrides();
		if (itemBattery.batteryLifeInt == 0 && itemToggle.toggleState)
		{
			if (!((Component)display).gameObject.activeSelf && itemToggle.toggleState)
			{
				((Component)display).gameObject.SetActive(true);
				batteryOutTimer = 0f;
			}
			if (batteryOutTimer == 0f)
			{
				soundTargetLost.Play(((Component)display).transform.position);
			}
			if (batteryOutTimer > 2f && itemToggle.toggleState)
			{
				itemToggle.ToggleItem(toggle: false);
				((Component)display).gameObject.SetActive(false);
				batteryOutTimer = 0f;
			}
			else
			{
				DisplayColorOverride("X", Color.red, 2f);
				batteryOutTimer += Time.deltaTime;
			}
			return;
		}
		batteryOutTimer = 0f;
		DisplayLogic();
		TargetLogic();
		if (!(displayOverrideTimer > 0f))
		{
			AnimateEmissionToBlack();
			if (itemToggle.toggleState)
			{
				FindATarget();
				Blinking();
			}
		}
	}

	private void Blinking()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		Color color = ((Renderer)meshRenderer).material.GetColor("_EmissionColor");
		Color val = colorBleepOff;
		if (color != val)
		{
			Color val2 = Color.Lerp(color, val, Time.deltaTime * 4f);
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", val2);
			nozzleLight.color = val2;
		}
		if (nozzleLight.intensity < 1f)
		{
			if (!((Behaviour)nozzleLight).enabled)
			{
				((Behaviour)nozzleLight).enabled = true;
			}
			nozzleLight.intensity = Mathf.Lerp(nozzleLight.intensity, 2f, Time.deltaTime * 10f);
		}
		if (hasTarget)
		{
			Vector3 position = nozzleTransform.position;
			float num = 1.5f;
			float num2 = 0.2f;
			blipTimer += Time.deltaTime;
			float num3 = 5f;
			float num4 = 0f;
			float num5 = (Mathf.Clamp(Vector3.Distance(position, targetPosition), num4, num3) - num4) / (num3 - num4);
			float num6 = animationCurve.Evaluate(num5);
			float num7 = Mathf.Lerp(num2, num, num6);
			if (blipTimer > num7)
			{
				blipTimer = 0f;
				soundBleep.Pitch = Mathf.Lerp(1f, 2f, 1f - num6);
				soundBleep.Play(nozzleTransform.position);
				((Renderer)meshRenderer).material.SetColor("_EmissionColor", colorBleep);
				nozzleLight.color = colorBleep;
				((Behaviour)nozzleLight).enabled = true;
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!(itemBattery.batteryLife <= 0f))
		{
			if (!itemToggle.toggleState)
			{
				currentTarget = null;
				hasTarget = false;
			}
			else if (!(displayOverrideTimer > 0f) && SemiFunc.IsMasterClientOrSingleplayer() && hasTarget && physGrabObject.grabbed)
			{
				SemiFunc.PhysLookAtPositionWithForce(rb, ((Component)this).transform, targetPosition, 10f);
				rb.AddForceAtPosition(((Component)this).transform.forward * 1f, nozzleTransform.position, (ForceMode)0);
			}
		}
	}

	private void SetTarget(int photonViewID)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("SetTargetRPC", (RpcTarget)0, new object[1] { photonViewID });
		}
	}

	[PunRPC]
	private void SetTargetRPC(int targetViewID)
	{
		PhysGrabObject component = ((Component)PhotonView.Find(targetViewID)).GetComponent<PhysGrabObject>();
		Transform transform = ((Component)PhotonView.Find(targetViewID)).transform;
		currentTarget = transform;
		if (Object.op_Implicit((Object)(object)component))
		{
			currentTargetPhysGrabObject = component;
		}
		hasTarget = true;
	}

	private void DisplayColorOverride(string _text, Color _color, float _time)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)displayText).text = _text;
		((Graphic)displayText).color = _color;
		displayOverrideTimer = _time;
		displayLight.color = _color;
		_color.a = 0.2f;
		((Renderer)display).material.color = _color;
	}

	private void CurrentTargetUpdate(bool _found)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("CurrentTargetUpdateRPC", (RpcTarget)0, new object[1] { _found });
		}
		else
		{
			CurrentTargetUpdateRPC(_found);
		}
	}

	[PunRPC]
	public void CurrentTargetUpdateRPC(bool _found)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_found)
		{
			soundTargetFound.Play(((Component)display).transform.position);
			DisplayColorOverride("FOUND", colorTargetFound, 2f);
		}
		else
		{
			soundTargetLost.Play(((Component)display).transform.position);
			DisplayColorOverride("NOT FOUND", Color.red, 2f);
		}
		currentTarget = null;
		currentTargetPhysGrabObject = null;
		hasTarget = false;
	}
}
