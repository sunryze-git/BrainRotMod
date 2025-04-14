using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChargingStation : MonoBehaviour, IPunObservable
{
	public static ChargingStation instance;

	private PhotonView photonView;

	private Transform chargeBar;

	internal float charge = 1f;

	private float chargeScale = 1f;

	private float chargeScaleTarget = 1f;

	internal int chargeInt;

	private int chargeSegments = 6;

	private float chargeRate = 0.05f;

	public AnimationCurve chargeCurve;

	private float chargeCurveTime;

	private Transform chargeArea;

	private float chargeAreaCheckTimer;

	private List<ItemBattery> itemsCharging = new List<ItemBattery>();

	private Transform lockedTransform;

	public GameObject meshObject;

	private Material chargingStationEmissionMaterial;

	private bool isCharging;

	private bool isChargingPrev;

	private Light light1;

	private Light light2;

	public Sound soundStart;

	public Sound soundStop;

	public Sound soundLoop;

	public Transform crystalCylinder;

	public List<Transform> crystals = new List<Transform>();

	public ParticleSystem lightParticle;

	public ParticleSystem fireflyParticles;

	public ParticleSystem bitsParticles;

	public Sound soundPowerCrystalBreak;

	private float crystalCooldown;

	public Item item;

	public GameObject subtleLight;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		chargeRate = 0.05f;
		foreach (Transform item in crystalCylinder)
		{
			Transform val = item;
			crystals.Add(val);
		}
		chargingStationEmissionMaterial = meshObject.GetComponent<Renderer>().material;
		chargeBar = ((Component)this).transform.Find("Charge");
		photonView = ((Component)this).GetComponent<PhotonView>();
		chargeArea = ((Component)this).transform.Find("Charge Area");
		lockedTransform = ((Component)this).transform.Find("Locked");
		light1 = ((Component)((Component)this).transform.Find("Light1")).GetComponent<Light>();
		light2 = ((Component)((Component)this).transform.Find("Light2")).GetComponent<Light>();
		if (!SemiFunc.RunIsShop())
		{
			if (Object.op_Implicit((Object)(object)lockedTransform))
			{
				Object.Destroy((Object)(object)((Component)lockedTransform).gameObject);
			}
		}
		else
		{
			if (Object.op_Implicit((Object)(object)subtleLight))
			{
				Object.Destroy((Object)(object)subtleLight);
			}
			if (Object.op_Implicit((Object)(object)chargeArea))
			{
				Object.Destroy((Object)(object)((Component)chargeArea).gameObject);
			}
			if (Object.op_Implicit((Object)(object)chargeBar))
			{
				Object.Destroy((Object)(object)((Component)chargeBar).gameObject);
			}
			Object.Destroy((Object)(object)((Component)light1).gameObject);
			Object.Destroy((Object)(object)((Component)light2).gameObject);
		}
		charge = 0f;
		chargeScale = 0f;
		chargeScaleTarget = 0f;
		chargeBar.localScale = new Vector3(0f, 1f, 1f);
		chargeInt = SemiFunc.StatGetItemsPurchased("Item Power Crystal");
		if (chargeInt <= 0)
		{
			OutOfCrystalsShutdown();
		}
		int num = StatsManager.instance.runStats["chargingStationCharge"];
		if (chargeInt > num)
		{
			if (chargeInt > chargeSegments)
			{
				chargeInt = chargeSegments;
			}
			charge = (float)chargeInt * (1f / (float)chargeSegments);
			chargeScale = charge;
			chargeScaleTarget = charge;
			chargeBar.localScale = new Vector3(chargeScale, 1f, 1f);
			StatsManager.instance.runStats["chargingStationCharge"] = chargeInt;
		}
		else
		{
			int num2 = num;
			float chargingStationCharge = StatsManager.instance.chargingStationCharge;
			chargeInt = num2;
			if (chargeInt > chargeSegments)
			{
				chargeInt = chargeSegments;
			}
			charge = chargingStationCharge;
			if (charge > (float)chargeInt * (1f / (float)chargeSegments))
			{
				charge = (float)chargeInt * (1f / (float)chargeSegments);
			}
			chargeScale = (float)chargeInt * (1f / (float)chargeSegments);
			chargeScaleTarget = (float)chargeInt * (1f / (float)chargeSegments);
			chargeBar.localScale = new Vector3(chargeScale, 1f, 1f);
			StatsManager.instance.runStats["chargingStationCharge"] = chargeInt;
		}
		((MonoBehaviour)this).StartCoroutine(MissionText());
		while (crystals.Count > chargeInt)
		{
			Object.Destroy((Object)(object)((Component)crystals[0]).gameObject);
			crystals.RemoveAt(0);
			if (crystals.Count == 0)
			{
				OutOfCrystalsShutdown();
				break;
			}
		}
		if (RunManager.instance.levelsCompleted < 1)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void OutOfCrystalsShutdown()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
		((Behaviour)light1).enabled = false;
		((Behaviour)light2).enabled = false;
		Color color = default(Color);
		((Color)(ref color))._002Ector(0.1f, 0.1f, 0.2f);
		subtleLight.GetComponent<Light>().color = color;
	}

	public IEnumerator MissionText()
	{
		while (LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(2f);
		if (SemiFunc.RunIsLobby())
		{
			SemiFunc.UIFocusText("Enjoy the ride, recharge stuff and GEAR UP!", Color.white, AssetManager.instance.colorYellow);
		}
	}

	private void StopCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("StopChargeRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			StopChargeRPC();
		}
	}

	[PunRPC]
	public void StopChargeRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundStop.Play(((Component)this).transform.position);
		isCharging = false;
	}

	private void StartCharge()
	{
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("StartChargeRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			StartChargeRPC();
		}
	}

	[PunRPC]
	public void StartChargeRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundStart.Play(((Component)this).transform.position);
		isCharging = true;
	}

	private void ChargeArea()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (charge <= 0f)
		{
			if (isCharging)
			{
				isChargingPrev = isCharging;
				StopCharge();
				isCharging = false;
			}
			return;
		}
		chargeAreaCheckTimer += Time.deltaTime;
		if (chargeAreaCheckTimer > 0.5f)
		{
			Collider[] array = Physics.OverlapBox(chargeArea.position, chargeArea.localScale / 2f, chargeArea.localRotation, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject()));
			itemsCharging.Clear();
			Collider[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ItemBattery componentInParent = ((Component)array2[i]).GetComponentInParent<ItemBattery>();
				if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.batteryLifeInt < 6 && !itemsCharging.Contains(componentInParent))
				{
					itemsCharging.Add(componentInParent);
				}
			}
			chargeAreaCheckTimer = 0f;
		}
		bool flag = false;
		foreach (ItemBattery item in itemsCharging)
		{
			if (item.batteryLifeInt < 6)
			{
				item.ChargeBattery(((Component)this).gameObject, 30f);
				charge -= chargeRate * Time.deltaTime;
				flag = true;
				if (!isCharging)
				{
					StartCharge();
					isChargingPrev = isCharging;
					isCharging = true;
				}
			}
		}
		if (!flag && isCharging)
		{
			isChargingPrev = isCharging;
			StopCharge();
			isCharging = false;
		}
	}

	private void ChargingEffects()
	{
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (isCharging)
		{
			TutorialDirector.instance.playerUsedChargingStation = true;
			crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, Mathf.PingPong(Time.time * 150f, 5f) - 2.5f);
			int num = 0;
			foreach (Transform crystal in crystals)
			{
				if (Object.op_Implicit((Object)(object)crystal))
				{
					num++;
					float num2 = 0.1f + Mathf.PingPong((Time.time + (float)num) * 5f, 1f);
					Color val = Color.yellow * Mathf.LinearToGammaSpace(num2);
					((Component)crystal).GetComponent<Renderer>().material.SetColor("_EmissionColor", val);
				}
			}
			crystalCooldown = 0f;
			return;
		}
		crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, 0f);
		foreach (Transform crystal2 in crystals)
		{
			if (Object.op_Implicit((Object)(object)crystal2))
			{
				crystalCooldown += Time.deltaTime * 0.5f;
				float num3 = chargeCurve.Evaluate(crystalCooldown);
				float num4 = Mathf.Lerp(1f, 0.1f, num3);
				Color val2 = Color.yellow * Mathf.LinearToGammaSpace(num4);
				((Component)crystal2).GetComponent<Renderer>().material.SetColor("_EmissionColor", val2);
				crystalCylinder.localRotation = Quaternion.Euler(90f, 0f, (Mathf.PingPong(Time.time * 250f, 10f) - 5f) * (1f - num3));
			}
		}
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		soundLoop.PlayLoop(isCharging, 2f, 2f);
		AnimateChargeBar();
		ChargingEffects();
		int count = crystals.Count;
		if (isCharging && count > 0)
		{
			float num = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
			Color val = Color.yellow * Mathf.LinearToGammaSpace(num);
			chargingStationEmissionMaterial.SetColor("_EmissionColor", val);
			if (Object.op_Implicit((Object)(object)light1) && Object.op_Implicit((Object)(object)light2))
			{
				((Behaviour)light1).enabled = true;
				((Behaviour)light2).enabled = true;
				light1.intensity = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
				light2.intensity = 0.5f + Mathf.PingPong(Time.time * 5f, 0.5f);
			}
		}
		else if (Object.op_Implicit((Object)(object)light1) && Object.op_Implicit((Object)(object)light2))
		{
			chargingStationEmissionMaterial.SetColor("_EmissionColor", Color.black);
			((Behaviour)light1).enabled = false;
			((Behaviour)light2).enabled = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !RunManager.instance.restarting)
		{
			ChargeArea();
			float num2 = 1f / (float)chargeSegments;
			int num3 = Mathf.CeilToInt(charge / num2);
			if (num3 > chargeSegments)
			{
				num3 = chargeSegments;
			}
			if (chargeInt != num3)
			{
				chargeInt = num3;
				UpdateChargeBar(chargeInt);
			}
		}
	}

	private void AnimateChargeBar()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)chargeBar) && chargeScale != chargeScaleTarget)
		{
			chargeCurveTime += Time.deltaTime;
			chargeScale = Mathf.Lerp(chargeScale, chargeScaleTarget, chargeCurve.Evaluate(chargeCurveTime));
			chargeBar.localScale = new Vector3(chargeScale, 1f, 1f);
		}
	}

	private void UpdateChargeBar(int segmentPassed)
	{
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpdateChargeBarRPC", (RpcTarget)0, new object[1] { segmentPassed });
		}
		else
		{
			UpdateChargeBarRPC(segmentPassed);
		}
	}

	private void DestroyCrystal()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (crystals.Count >= 1)
		{
			Vector3 position = crystals[0].position + crystals[0].up * 0.1f;
			((Component)lightParticle).transform.position = position;
			((Component)fireflyParticles).transform.position = position;
			((Component)bitsParticles).transform.position = position;
			lightParticle.Play();
			fireflyParticles.Play();
			bitsParticles.Play();
			soundPowerCrystalBreak.Play(position);
			Object.Destroy((Object)(object)((Component)crystals[0]).gameObject);
			crystals.RemoveAt(0);
			if (crystals.Count == 0)
			{
				OutOfCrystalsShutdown();
			}
		}
	}

	[PunRPC]
	public void UpdateChargeBarRPC(int segmentPassed)
	{
		chargeCurveTime = 0f;
		float num = 1f / (float)chargeSegments;
		chargeScaleTarget = (float)segmentPassed * num;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StatsManager.instance.SetItemPurchase(item, StatsManager.instance.GetItemPurchased(item) - 1);
		}
		DestroyCrystal();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			stream.SendNext((object)isCharging);
		}
		else
		{
			isCharging = (bool)stream.ReceiveNext();
		}
	}
}
