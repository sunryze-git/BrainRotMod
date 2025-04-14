using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemBattery : MonoBehaviour
{
	public bool isUnchargable;

	public Transform batteryTransform;

	private Camera mainCamera;

	public float upOffset = 0.5f;

	[HideInInspector]
	public bool batteryActive;

	[HideInInspector]
	public float batteryLife = 100f;

	internal int batteryLifeInt = 6;

	private Renderer itemBatteryMaterial;

	private float batteryOutBlinkTimer;

	private PhotonView photonView;

	[HideInInspector]
	public Color batteryColor;

	private float chargeTimer;

	private float chargeRate;

	private List<GameObject> chargerList = new List<GameObject>();

	internal bool isCharging;

	private float chargingBlinkTimer;

	private bool chargingBlink;

	private bool lowBatteryBeep;

	private ItemAttributes itemAttributes;

	private float showTimer;

	private bool showBattery;

	public bool autoDrain = true;

	private ItemEquippable itemEquippable;

	public bool onlyShowWhenItemToggleIsOn;

	public float batteryDrainRate = 1f;

	private float drainRate;

	private float drainTimer;

	private bool tutorialCheck;

	private PhysGrabObject physGrabObject;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		if (!Object.op_Implicit((Object)(object)itemAttributes))
		{
			Debug.LogWarning((object)("ItemBattery.cs: No ItemAttributes found on " + ((Object)((Component)this).gameObject).name));
		}
	}

	private void Start()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		mainCamera = Camera.main;
		itemBatteryMaterial = ((Component)batteryTransform).GetComponentInChildren<Renderer>();
		itemBatteryMaterial.material.SetColor("_Color", batteryColor);
		physGrabObject = ((Component)this).GetComponentInChildren<PhysGrabObject>();
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
		{
			tutorialCheck = true;
		}
	}

	private IEnumerator BatteryInit()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		while (((Component)this).GetComponent<ItemAttributes>().instanceName == null)
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			StatsManager.instance.SetBatteryLevel(itemAttributes.instanceName, 100);
		}
		batteryLife = StatsManager.instance.GetBatteryLevel(itemAttributes.instanceName);
		if (batteryLife > 0f)
		{
			batteryLifeInt = (int)Mathf.Round(batteryLife / 16.6f);
			batteryColor = itemAttributes.colorPreset.GetColorLight();
		}
		else
		{
			batteryLife = 0f;
			batteryLifeInt = 0;
			batteryColor = itemAttributes.colorPreset.GetColorLight();
		}
		BatteryFullPercentChange(batteryLifeInt);
	}

	public void SetBatteryLife(int _batteryLife)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (batteryLife > 0f)
		{
			batteryLife = _batteryLife;
			batteryLifeInt = (int)Mathf.Round(batteryLife / 16.6f);
		}
		else
		{
			batteryLife = 0f;
			batteryLifeInt = 0;
		}
		batteryColor = itemAttributes.colorPreset.GetColorLight();
		BatteryFullPercentChange(batteryLifeInt);
	}

	public void OverrideBatteryShow(float time = 0.1f)
	{
		showTimer = time;
	}

	public void ChargeBattery(GameObject chargerObject, float chargeAmount)
	{
		if (!chargerList.Contains(chargerObject))
		{
			chargerList.Add(chargerObject);
			chargeRate += chargeAmount;
		}
		chargeTimer = 0.1f;
	}

	private void FixedUpdate()
	{
		if (showTimer > 0f)
		{
			showTimer -= Time.fixedDeltaTime;
			showBattery = true;
		}
		else
		{
			showBattery = false;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (chargeTimer > 0f && batteryLife < 99f)
		{
			batteryLife = Mathf.Clamp(batteryLife + chargeRate * Time.fixedDeltaTime, 0f, 100f);
			if (!isCharging)
			{
				BatteryChargeToggle(toggle: true);
			}
			chargeTimer -= Time.fixedDeltaTime;
		}
		else if (chargeRate != 0f)
		{
			chargeRate = 0f;
			chargeTimer = 0f;
			chargerList.Clear();
			BatteryChargeToggle(toggle: false);
		}
		if (drainTimer > 0f && batteryLife > 0f)
		{
			batteryLife = Mathf.Clamp(batteryLife - drainRate * Time.fixedDeltaTime, 0f, 100f);
			drainTimer -= Time.fixedDeltaTime;
		}
		else if (drainRate != 0f)
		{
			drainRate = 0f;
			drainTimer = 0f;
		}
	}

	private void Update()
	{
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		if (itemAttributes.shopItem && SemiFunc.IsMasterClientOrSingleplayer())
		{
			batteryLife = 100f;
		}
		BatteryLookAt();
		BatteryChargingVisuals();
		if (SemiFunc.RunIsLobby() && batteryLifeInt < 6)
		{
			OverrideBatteryShow();
		}
		if (showBattery && !((Component)itemBatteryMaterial).gameObject.activeSelf)
		{
			((Component)itemBatteryMaterial).gameObject.SetActive(true);
			BatteryOffsetTexture(batteryLifeInt);
		}
		if (tutorialCheck && batteryLife <= 0f && SemiFunc.FPSImpulse15() && physGrabObject.playerGrabbing.Count > 0)
		{
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if (item.isLocal && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
				{
					TutorialDirector.instance.ActivateTip("Charging Station", 2f, _interrupt: false);
					tutorialCheck = false;
				}
			}
		}
		if (batteryActive)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && autoDrain && !itemEquippable.isEquipped)
			{
				batteryLife -= batteryDrainRate * Time.deltaTime;
			}
			if (batteryLifeInt <= 1)
			{
				if (batteryLifeInt == 1)
				{
					batteryOutBlinkTimer += Time.deltaTime;
				}
				else
				{
					batteryOutBlinkTimer += 5f * Time.deltaTime;
				}
				if (batteryOutBlinkTimer >= 1f)
				{
					if (!lowBatteryBeep)
					{
						itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.278f));
						if (batteryLifeInt < 1)
						{
							AssetManager.instance.batteryLowBeep.Play(((Component)this).transform.position);
							itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.044f));
						}
						lowBatteryBeep = true;
					}
				}
				else if (lowBatteryBeep)
				{
					itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.278f));
					lowBatteryBeep = false;
				}
				if (batteryOutBlinkTimer >= 2f)
				{
					batteryOutBlinkTimer = 0f;
				}
			}
			if (!((Component)itemBatteryMaterial).gameObject.activeSelf)
			{
				((Component)itemBatteryMaterial).gameObject.SetActive(true);
				BatteryOffsetTexture(batteryLifeInt);
			}
		}
		else if (!showBattery && ((Component)itemBatteryMaterial).gameObject.activeSelf && !isCharging)
		{
			((Component)itemBatteryMaterial).gameObject.SetActive(false);
			BatteryOffsetTexture(batteryLifeInt);
		}
		if (GameManager.instance.gameMode == 0 || (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient))
		{
			if (batteryLifeInt == 0 && batteryLife >= 17f)
			{
				BatteryFullPercentChange(1, charge: true);
			}
			else if (batteryLifeInt == 1 && batteryLife >= 34f)
			{
				BatteryFullPercentChange(2, charge: true);
			}
			else if (batteryLifeInt == 2 && batteryLife >= 50f)
			{
				BatteryFullPercentChange(3, charge: true);
			}
			else if (batteryLifeInt == 3 && batteryLife >= 67f)
			{
				BatteryFullPercentChange(4, charge: true);
			}
			else if (batteryLifeInt == 4 && batteryLife >= 84f)
			{
				BatteryFullPercentChange(5, charge: true);
			}
			else if (batteryLifeInt == 5 && batteryLife >= 99f)
			{
				BatteryFullPercentChange(6, charge: true);
			}
			if (batteryLifeInt == 6 && batteryLife <= 84f)
			{
				BatteryFullPercentChange(5);
			}
			else if (batteryLifeInt == 5 && batteryLife <= 67f)
			{
				BatteryFullPercentChange(4);
			}
			else if (batteryLifeInt == 4 && batteryLife <= 50f)
			{
				BatteryFullPercentChange(3);
			}
			else if (batteryLifeInt == 3 && batteryLife <= 34f)
			{
				BatteryFullPercentChange(2);
			}
			else if (batteryLifeInt == 2 && batteryLife <= 17f)
			{
				BatteryFullPercentChange(1);
			}
			else if (batteryLifeInt == 1 && batteryLife <= 0f)
			{
				BatteryFullPercentChange(0);
			}
		}
	}

	public void RemoveFullBar(int _bars)
	{
		if (!SemiFunc.RunIsShop() && batteryLifeInt > 0)
		{
			batteryLifeInt -= _bars;
			if (batteryLifeInt <= 0)
			{
				batteryLifeInt = 0;
				batteryLife = 0f;
			}
			else
			{
				batteryLife = (float)batteryLifeInt * 16.6f;
			}
			BatteryFullPercentChange(batteryLifeInt);
		}
	}

	public void BatteryToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				photonView.RPC("BatteryToggleRPC", (RpcTarget)0, new object[1] { toggle });
			}
		}
		else
		{
			BatteryToggleRPC(toggle);
		}
	}

	[PunRPC]
	public void BatteryToggleRPC(bool toggle)
	{
		batteryActive = toggle;
	}

	private void BatteryLookAt()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (showBattery || batteryActive || isCharging)
		{
			batteryTransform.LookAt(((Component)mainCamera).transform);
			float num = Vector3.Distance(batteryTransform.position, ((Component)mainCamera).transform.position);
			batteryTransform.localScale = Vector3.one * num * 0.8f;
			if (batteryTransform.localScale.x > 3f)
			{
				batteryTransform.localScale = Vector3.one * 3f;
			}
			batteryTransform.Rotate(0f, 180f, 0f);
			batteryTransform.position = ((Component)this).transform.position + Vector3.up * upOffset;
		}
	}

	private void BatteryChargingVisuals()
	{
		if (!isCharging)
		{
			return;
		}
		if (!((Component)itemBatteryMaterial).gameObject.activeSelf)
		{
			((Component)itemBatteryMaterial).gameObject.SetActive(true);
		}
		chargingBlinkTimer += Time.deltaTime;
		if (chargingBlinkTimer > 0.5f)
		{
			chargingBlink = !chargingBlink;
			if (chargingBlink)
			{
				BatteryOffsetTexture(batteryLifeInt + 1);
			}
			else
			{
				BatteryOffsetTexture(batteryLifeInt);
			}
			chargingBlinkTimer = 0f;
		}
	}

	private void BatteryChargeToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				photonView.RPC("BatteryChargeStartRPC", (RpcTarget)0, new object[1] { toggle });
			}
		}
		else
		{
			BatteryChargeStartRPC(toggle);
		}
	}

	[PunRPC]
	private void BatteryChargeStartRPC(bool toggle)
	{
		isCharging = toggle;
		BatteryOffsetTexture(batteryLifeInt);
	}

	private void BatteryOffsetTexture(int batteryLevel)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)itemBatteryMaterial))
		{
			return;
		}
		Color red = batteryColor;
		switch (batteryLevel)
		{
		case 6:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.745f));
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 5:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.745f));
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 4:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.512f));
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 3:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.512f));
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 2:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.278f));
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 1:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0.375f, 0.278f));
			if (!isCharging)
			{
				red = Color.red;
			}
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		case 0:
			itemBatteryMaterial.material.SetTextureOffset("_MainTex", new Vector2(0f, 0.044f));
			if (!isCharging)
			{
				red = Color.red;
			}
			itemBatteryMaterial.material.SetColor("_Color", red);
			break;
		}
	}

	private void BatteryFullPercentChangeLogic(int batteryLevel, bool charge)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (batteryLifeInt > batteryLevel && batteryLevel == 1 && batteryActive)
		{
			AssetManager.instance.batteryLowWarning.Play(((Component)this).transform.position);
		}
		batteryLifeInt = batteryLevel;
		if (batteryLifeInt != 0)
		{
			batteryLife = (float)batteryLifeInt * 16.6f;
		}
		else
		{
			batteryLife = 0f;
		}
		SemiFunc.StatSetBattery(itemAttributes.instanceName, (int)batteryLife);
		BatteryOffsetTexture(batteryLifeInt);
		if (batteryActive || charge)
		{
			if (charge)
			{
				AssetManager.instance.batteryChargeSound.Play(((Component)this).transform.position);
			}
			else
			{
				AssetManager.instance.batteryDrainSound.Play(((Component)this).transform.position);
			}
		}
	}

	private void BatteryFullPercentChange(int batteryLifeInt, bool charge = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			BatteryFullPercentChangeLogic(batteryLifeInt, charge);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("BatteryFullPercentChangeRPC", (RpcTarget)0, new object[2] { batteryLifeInt, charge });
		}
	}

	[PunRPC]
	private void BatteryFullPercentChangeRPC(int batteryLifeInt, bool charge)
	{
		BatteryFullPercentChangeLogic(batteryLifeInt, charge);
	}

	public void Drain(float amount)
	{
		drainRate = amount;
		drainTimer = 0.1f;
	}
}
