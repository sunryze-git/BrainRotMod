using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemGrenade : MonoBehaviour
{
	public Color blinkColor;

	public UnityEvent onDetonate;

	private ItemToggle itemToggle;

	private ItemAttributes itemAttributes;

	internal bool isActive;

	private float grenadeTimer;

	public float tickTime = 3f;

	private PhotonView photonView;

	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	public Sound soundSplinter;

	public Sound soundTick;

	private float splinterAnimationProgress;

	public AnimationCurve splinterAnimationCurve;

	private Transform splinterTransform;

	private Material grenadeEmissionMaterial;

	private ItemEquippable itemEquippable;

	private Vector3 grenadeStartPosition;

	private Quaternion grenadeStartRotation;

	private PhysGrabObject physGrabObject;

	private Vector3 prevPosition;

	[FormerlySerializedAs("isThiefGrenade")]
	[HideInInspector]
	public bool isSpawnedGrenade;

	public GameObject throwLine;

	private Rigidbody rb;

	private float throwLineTimer;

	private TrailRenderer throwLineTrail;

	private void Start()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		physGrabObjectImpactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		splinterTransform = ((Component)this).transform.Find("Splinter");
		GameObject gameObject = ((Component)((Component)this).transform.Find("Mesh")).gameObject;
		grenadeEmissionMaterial = gameObject.GetComponent<Renderer>().material;
		grenadeStartPosition = ((Component)this).transform.position;
		grenadeStartRotation = ((Component)this).transform.rotation;
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		rb = ((Component)this).GetComponent<Rigidbody>();
		throwLineTrail = throwLine.GetComponent<TrailRenderer>();
	}

	private void FixedUpdate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (itemEquippable.isEquipped || itemEquippable.wasEquippedTimer > 0f)
		{
			prevPosition = rb.position;
			return;
		}
		Vector3 val = (rb.position - prevPosition) / Time.fixedDeltaTime;
		Vector3 val2 = rb.position - prevPosition;
		_ = ((Vector3)(ref val2)).normalized;
		prevPosition = rb.position;
		if (!physGrabObject.grabbed && ((Vector3)(ref val)).magnitude > 2f)
		{
			throwLineTimer = 0.2f;
		}
		if (throwLineTimer > 0f)
		{
			throwLineTrail.emitting = true;
			throwLineTimer -= Time.fixedDeltaTime;
		}
		else
		{
			throwLineTrail.emitting = false;
		}
	}

	private void Update()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		soundTick.PlayLoop(isActive, 2f, 2f);
		if (itemEquippable.isEquipped)
		{
			if (isActive)
			{
				isActive = false;
				grenadeTimer = 0f;
				splinterAnimationProgress = 0f;
				itemToggle.ToggleItem(toggle: false);
				splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
				grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
			}
			return;
		}
		if (isActive)
		{
			if (splinterAnimationProgress < 1f)
			{
				splinterAnimationProgress += 5f * Time.deltaTime;
				float num = splinterAnimationCurve.Evaluate(splinterAnimationProgress);
				splinterTransform.localEulerAngles = new Vector3(num * 90f, 0f, 0f);
			}
			float num2 = Mathf.PingPong(Time.time * 8f, 1f);
			Color val = blinkColor * Mathf.LinearToGammaSpace(num2);
			grenadeEmissionMaterial.SetColor("_EmissionColor", val);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (itemToggle.toggleState && !isActive)
		{
			isActive = true;
			TickStart();
		}
		if (isActive)
		{
			grenadeTimer += Time.deltaTime;
			if (grenadeTimer >= tickTime)
			{
				grenadeTimer = 0f;
				TickEnd();
			}
		}
	}

	private void GrenadeReset()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		isActive = false;
		grenadeTimer = 0f;
		throwLine.SetActive(false);
		splinterAnimationProgress = 0f;
		itemToggle.ToggleItem(toggle: false);
		splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
		grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	private void TickStart()
	{
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("TickStartRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			TickStartRPC();
		}
	}

	private void TickEnd()
	{
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("TickEndRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			TickEndRPC();
		}
	}

	[PunRPC]
	private void TickStartRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundSplinter.Play(((Component)this).transform.position);
		isActive = true;
	}

	[PunRPC]
	private void TickEndRPC()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (itemEquippable.isEquipped)
		{
			return;
		}
		onDetonate.Invoke();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.RunIsShop() || isSpawnedGrenade)
			{
				if (!isSpawnedGrenade)
				{
					StatsManager.instance.ItemRemove(itemAttributes.instanceName);
				}
				physGrabObjectImpactDetector.DestroyObject();
			}
			else
			{
				physGrabObject.Teleport(grenadeStartPosition, grenadeStartRotation);
			}
		}
		if (SemiFunc.RunIsShop() && !isSpawnedGrenade)
		{
			GrenadeReset();
		}
	}
}
