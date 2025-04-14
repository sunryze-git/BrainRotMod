using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PunManager : MonoBehaviour
{
	internal PhotonView photonView;

	internal StatsManager statsManager;

	private ShopManager shopManager;

	private ItemManager itemManager;

	public static PunManager instance;

	private List<Hashtable> syncData = new List<Hashtable>();

	public PhotonLagSimulationGui lagSimulationGui;

	private int totalHaul;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		statsManager = StatsManager.instance;
		shopManager = ShopManager.instance;
		itemManager = ItemManager.instance;
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	public void SetItemName(string name, ItemAttributes itemAttributes, int photonViewID)
	{
		if (photonViewID != -1 && SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("SetItemNameRPC", (RpcTarget)0, new object[2] { name, photonViewID });
			}
			else
			{
				SetItemNameLOGIC(name, photonViewID, itemAttributes);
			}
		}
	}

	private void SetItemNameLOGIC(string name, int photonViewID, ItemAttributes _itemAttributes = null)
	{
		if (photonViewID == -1 && SemiFunc.IsMultiplayer())
		{
			return;
		}
		ItemAttributes itemAttributes = _itemAttributes;
		if (SemiFunc.IsMultiplayer())
		{
			itemAttributes = ((Component)PhotonView.Find(photonViewID)).GetComponent<ItemAttributes>();
		}
		if ((Object)(object)_itemAttributes == (Object)null && !SemiFunc.IsMultiplayer())
		{
			return;
		}
		itemAttributes.instanceName = name;
		ItemBattery component = ((Component)itemAttributes).GetComponent<ItemBattery>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.SetBatteryLife(statsManager.itemStatBattery[name]);
		}
		ItemEquippable component2 = ((Component)itemAttributes).GetComponent<ItemEquippable>();
		if (!Object.op_Implicit((Object)(object)component2))
		{
			return;
		}
		int spot = 0;
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		int hashCode = name.GetHashCode();
		bool flag = false;
		PlayerAvatar playerAvatar = null;
		foreach (PlayerAvatar item in list)
		{
			string steamID = item.steamID;
			if (StatsManager.instance.playerInventorySpot1[steamID] == hashCode && StatsManager.instance.playerInventorySpot1Taken[steamID] == 0)
			{
				spot = 0;
				flag = true;
				playerAvatar = item;
				StatsManager.instance.playerInventorySpot1Taken[steamID] = 1;
				break;
			}
			if (StatsManager.instance.playerInventorySpot2[steamID] == hashCode && StatsManager.instance.playerInventorySpot2Taken[steamID] == 0)
			{
				spot = 1;
				flag = true;
				playerAvatar = item;
				StatsManager.instance.playerInventorySpot2Taken[steamID] = 1;
				break;
			}
			if (StatsManager.instance.playerInventorySpot3[steamID] == hashCode && StatsManager.instance.playerInventorySpot3Taken[steamID] == 0)
			{
				spot = 2;
				flag = true;
				playerAvatar = item;
				StatsManager.instance.playerInventorySpot3Taken[steamID] = 1;
				break;
			}
		}
		if (flag)
		{
			int requestingPlayerId = -1;
			if (SemiFunc.IsMultiplayer())
			{
				requestingPlayerId = playerAvatar.photonView.ViewID;
			}
			component2.RequestEquip(spot, requestingPlayerId);
		}
	}

	public void CrownPlayerSync(string _steamID)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.IsMultiplayer())
		{
			photonView.RPC("CrownPlayerRPC", (RpcTarget)3, new object[1] { _steamID });
		}
	}

	[PunRPC]
	public void CrownPlayerRPC(string _steamID)
	{
		SessionManager.instance.crownedPlayerSteamID = _steamID;
		PlayerCrownSet component = Object.Instantiate<GameObject>(SessionManager.instance.crownPrefab).GetComponent<PlayerCrownSet>();
		component.crownOwnerFetched = true;
		component.crownOwnerSteamID = _steamID;
		StatsManager.instance.UpdateCrown(_steamID);
	}

	[PunRPC]
	public void SetItemNameRPC(string name, int photonViewID)
	{
		SetItemNameLOGIC(name, photonViewID);
	}

	public void ShopUpdateCost()
	{
		int num = 0;
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes shopping in ShopManager.instance.shoppingList)
		{
			if (Object.op_Implicit((Object)(object)shopping))
			{
				shopping.roomVolumeCheck.CheckSet();
				if (!shopping.roomVolumeCheck.inExtractionPoint)
				{
					list.Add(shopping);
				}
				else
				{
					num += shopping.value;
				}
			}
			else
			{
				list.Add(shopping);
			}
		}
		foreach (ItemAttributes item in list)
		{
			ShopManager.instance.shoppingList.Remove(item);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("UpdateShoppingCostRPC", (RpcTarget)0, new object[1] { num });
			}
			else
			{
				UpdateShoppingCostRPC(num);
			}
		}
	}

	private void Update()
	{
		if (SemiFunc.FPSImpulse5() && SemiFunc.IsMultiplayer() && SemiFunc.IsMasterClient() && totalHaul != RoundDirector.instance.totalHaul)
		{
			totalHaul = RoundDirector.instance.totalHaul;
			photonView.RPC("SyncHaul", (RpcTarget)1, new object[1] { totalHaul });
		}
	}

	[PunRPC]
	public void SyncHaul(int value)
	{
		RoundDirector.instance.totalHaul = value;
	}

	[PunRPC]
	public void UpdateShoppingCostRPC(int value)
	{
		ShopManager.instance.totalCost = value;
	}

	public void ShopPopulateItemVolumes()
	{
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		int spawnCount = 0;
		int spawnCount2 = 0;
		int spawnCount3 = 0;
		int spawnCount4 = 0;
		foreach (KeyValuePair<SemiFunc.itemSecretShopType, List<ItemVolume>> secretItemVolume in ShopManager.instance.secretItemVolumes)
		{
			List<ItemVolume> value = secretItemVolume.Value;
			foreach (ItemVolume item in value)
			{
				if (ShopManager.instance.potentialSecretItems.ContainsKey(secretItemVolume.Key))
				{
					_ = ShopManager.instance.potentialSecretItems[secretItemVolume.Key];
					if (Random.Range(0, 3) == 0 && Object.op_Implicit((Object)(object)item))
					{
						SpawnShopItem(item, ShopManager.instance.potentialSecretItems[secretItemVolume.Key], ref spawnCount, isSecret: true);
					}
				}
			}
			foreach (ItemVolume item2 in value)
			{
				if (Object.op_Implicit((Object)(object)item2))
				{
					Object.Destroy((Object)(object)((Component)item2).gameObject);
				}
			}
		}
		foreach (ItemVolume itemVolume in shopManager.itemVolumes)
		{
			if (shopManager.potentialItems.Count == 0 && shopManager.potentialItemConsumables.Count == 0)
			{
				break;
			}
			if ((spawnCount >= shopManager.itemSpawnTargetAmount || !SpawnShopItem(itemVolume, shopManager.potentialItems, ref spawnCount)) && (spawnCount2 >= shopManager.itemConsumablesAmount || !SpawnShopItem(itemVolume, shopManager.potentialItemConsumables, ref spawnCount2)))
			{
				if (spawnCount3 < shopManager.itemUpgradesAmount)
				{
					SpawnShopItem(itemVolume, shopManager.potentialItemUpgrades, ref spawnCount3);
				}
				if (spawnCount4 < shopManager.itemHealthPacksAmount)
				{
					SpawnShopItem(itemVolume, shopManager.potentialItemHealthPacks, ref spawnCount4);
				}
			}
		}
		foreach (ItemVolume itemVolume2 in shopManager.itemVolumes)
		{
			Object.Destroy((Object)(object)((Component)itemVolume2).gameObject);
		}
	}

	private bool SpawnShopItem(ItemVolume itemVolume, List<Item> itemList, ref int spawnCount, bool isSecret = false)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		for (int num = itemList.Count - 1; num >= 0; num--)
		{
			Item item = itemList[num];
			if (item.itemVolume == itemVolume.itemVolume)
			{
				((Component)ShopManager.instance.itemRotateHelper).transform.parent = ((Component)itemVolume).transform;
				((Component)ShopManager.instance.itemRotateHelper).transform.localRotation = item.spawnRotationOffset;
				Quaternion rotation = ((Component)ShopManager.instance.itemRotateHelper).transform.rotation;
				((Component)ShopManager.instance.itemRotateHelper).transform.parent = ((Component)ShopManager.instance).transform;
				string text = "Items/" + ((Object)item.prefab).name;
				if (SemiFunc.IsMultiplayer())
				{
					PhotonNetwork.InstantiateRoomObject(text, ((Component)itemVolume).transform.position, rotation, (byte)0, (object[])null);
				}
				else
				{
					Object.Instantiate<GameObject>(item.prefab, ((Component)itemVolume).transform.position, rotation);
				}
				itemList.RemoveAt(num);
				if (!isSecret)
				{
					spawnCount++;
				}
				return true;
			}
		}
		return false;
	}

	public void TruckPopulateItemVolumes()
	{
		ItemManager.instance.spawnedItems.Clear();
		if (SemiFunc.IsNotMasterClient())
		{
			return;
		}
		List<ItemVolume> list = new List<ItemVolume>(itemManager.itemVolumes);
		List<Item> list2 = new List<Item>(itemManager.purchasedItems);
		while (list.Count > 0 && list2.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < list2.Count; i++)
			{
				Item item = list2[i];
				ItemVolume itemVolume = list.Find((ItemVolume v) => v.itemVolume == item.itemVolume);
				if (Object.op_Implicit((Object)(object)itemVolume))
				{
					SpawnItem(item, itemVolume);
					list.Remove(itemVolume);
					list2.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		foreach (ItemVolume itemVolume2 in itemManager.itemVolumes)
		{
			Object.Destroy((Object)(object)((Component)itemVolume2).gameObject);
		}
	}

	private void SpawnItem(Item item, ItemVolume volume)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		((Component)ShopManager.instance.itemRotateHelper).transform.parent = ((Component)volume).transform;
		((Component)ShopManager.instance.itemRotateHelper).transform.localRotation = item.spawnRotationOffset;
		Quaternion rotation = ((Component)ShopManager.instance.itemRotateHelper).transform.rotation;
		((Component)ShopManager.instance.itemRotateHelper).transform.parent = ((Component)ShopManager.instance).transform;
		if (SemiFunc.IsMasterClient())
		{
			PhotonNetwork.InstantiateRoomObject("Items/" + ((Object)item.prefab).name, ((Component)volume).transform.position, rotation, (byte)0, (object[])null);
		}
		else if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(item.prefab, ((Component)volume).transform.position, rotation);
		}
	}

	public void AddingItem(string itemName, int index, int photonViewID, ItemAttributes itemAttributes)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("AddingItemRPC", (RpcTarget)0, new object[3] { itemName, index, photonViewID });
		}
		else
		{
			AddingItemLOGIC(itemName, index, photonViewID, itemAttributes);
		}
	}

	private void AddingItemLOGIC(string itemName, int index, int photonViewID, ItemAttributes itemAttributes = null)
	{
		if (!StatsManager.instance.item.ContainsKey(itemName))
		{
			StatsManager.instance.item.Add(itemName, index);
			StatsManager.instance.itemStatBattery.Add(itemName, 100);
			StatsManager.instance.takenItemNames.Add(itemName);
		}
		else
		{
			Debug.LogWarning((object)("Item " + itemName + " already exists in the dictionary"));
		}
		SetItemNameLOGIC(itemName, photonViewID, itemAttributes);
	}

	[PunRPC]
	public void AddingItemRPC(string itemName, int index, int photonViewID)
	{
		AddingItemLOGIC(itemName, index, photonViewID);
	}

	public void UpdateStat(string dictionaryName, string key, int value)
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("UpdateStatRPC", (RpcTarget)0, new object[3] { dictionaryName, key, value });
		}
		else
		{
			UpdateStatRPC(dictionaryName, key, value);
		}
	}

	[PunRPC]
	public void UpdateStatRPC(string dictionaryName, string key, int value)
	{
		StatsManager.instance.DictionaryUpdateValue(dictionaryName, key, value);
	}

	public int SetRunStatSet(string statName, int value)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				statsManager.runStats[statName] = value;
				photonView.RPC("SetRunStatRPC", (RpcTarget)1, new object[2] { statName, value });
			}
			else
			{
				statsManager.runStats[statName] = value;
			}
		}
		return statsManager.runStats[statName];
	}

	[PunRPC]
	public void SetRunStatRPC(string statName, int value)
	{
		statsManager.runStats[statName] = value;
	}

	public int UpgradeItemBattery(string itemName)
	{
		statsManager.itemBatteryUpgrades[itemName]++;
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradeItemBatteryRPC", (RpcTarget)1, new object[2]
			{
				itemName,
				statsManager.itemBatteryUpgrades[itemName]
			});
		}
		return statsManager.itemBatteryUpgrades[itemName];
	}

	[PunRPC]
	public void UpgradeItemBatteryRPC(string itemName, int value)
	{
		statsManager.itemBatteryUpgrades[itemName] = value;
	}

	public int UpgradePlayerHealth(string playerName)
	{
		statsManager.playerUpgradeHealth[playerName]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateHealthRightAway(playerName);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerHealthRPC", (RpcTarget)1, new object[2]
			{
				playerName,
				statsManager.playerUpgradeHealth[playerName]
			});
		}
		return statsManager.playerUpgradeHealth[playerName];
	}

	[PunRPC]
	public void UpgradePlayerHealthRPC(string playerName, int value)
	{
		statsManager.playerUpgradeHealth[playerName] = value;
		UpdateHealthRightAway(playerName);
	}

	private void UpdateHealthRightAway(string playerName)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(playerName);
		if ((Object)(object)playerAvatar == (Object)(object)SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.playerHealth.maxHealth += 20;
			playerAvatar.playerHealth.Heal(20, effect: false);
		}
	}

	public int UpgradePlayerEnergy(string _steamID)
	{
		statsManager.playerUpgradeStamina[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateEnergyRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerEnergyRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeStamina[_steamID]
			});
		}
		return statsManager.playerUpgradeStamina[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerEnergyRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeStamina[_steamID] = value;
		UpdateEnergyRightAway(_steamID);
	}

	private void UpdateEnergyRightAway(string _steamID)
	{
		if ((Object)(object)SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == (Object)(object)SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.EnergyStart += 10f;
			PlayerController.instance.EnergyCurrent = PlayerController.instance.EnergyStart;
		}
	}

	public int UpgradePlayerExtraJump(string _steamID)
	{
		statsManager.playerUpgradeExtraJump[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateExtraJumpRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerExtraJumpRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeExtraJump[_steamID]
			});
		}
		return statsManager.playerUpgradeExtraJump[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerExtraJumpRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeExtraJump[_steamID] = value;
		UpdateExtraJumpRightAway(_steamID);
	}

	private void UpdateExtraJumpRightAway(string _steamID)
	{
		if ((Object)(object)SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == (Object)(object)SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.JumpExtra++;
		}
	}

	public int UpgradeMapPlayerCount(string _steamID)
	{
		statsManager.playerUpgradeMapPlayerCount[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateMapPlayerCountRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradeMapPlayerCountRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeMapPlayerCount[_steamID]
			});
		}
		return statsManager.playerUpgradeMapPlayerCount[_steamID];
	}

	[PunRPC]
	public void UpgradeMapPlayerCountRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeMapPlayerCount[_steamID] = value;
		UpdateMapPlayerCountRightAway(_steamID);
	}

	private void UpdateMapPlayerCountRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if ((Object)(object)playerAvatar == (Object)(object)SemiFunc.PlayerAvatarLocal())
		{
			playerAvatar.upgradeMapPlayerCount++;
		}
	}

	public int UpgradePlayerTumbleLaunch(string _steamID)
	{
		statsManager.playerUpgradeLaunch[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateTumbleLaunchRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerTumbleLaunchRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeLaunch[_steamID]
			});
		}
		return statsManager.playerUpgradeLaunch[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerTumbleLaunchRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeLaunch[_steamID] = value;
		UpdateTumbleLaunchRightAway(_steamID);
	}

	private void UpdateTumbleLaunchRightAway(string _steamID)
	{
		SemiFunc.PlayerAvatarGetFromSteamID(_steamID).tumble.tumbleLaunch++;
	}

	public int UpgradePlayerSprintSpeed(string _steamID)
	{
		statsManager.playerUpgradeSpeed[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateSprintSpeedRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerSprintSpeedRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeSpeed[_steamID]
			});
		}
		return statsManager.playerUpgradeSpeed[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerSprintSpeedRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeSpeed[_steamID] = value;
		UpdateSprintSpeedRightAway(_steamID);
	}

	private void UpdateSprintSpeedRightAway(string _steamID)
	{
		if ((Object)(object)SemiFunc.PlayerAvatarGetFromSteamID(_steamID) == (Object)(object)SemiFunc.PlayerAvatarLocal())
		{
			PlayerController.instance.SprintSpeed += 1f;
			PlayerController.instance.SprintSpeedUpgrades += 1f;
		}
	}

	public int UpgradePlayerGrabStrength(string _steamID)
	{
		statsManager.playerUpgradeStrength[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateGrabStrengthRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerGrabStrengthRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeStrength[_steamID]
			});
		}
		return statsManager.playerUpgradeStrength[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerGrabStrengthRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeStrength[_steamID] = value;
		UpdateGrabStrengthRightAway(_steamID);
	}

	private void UpdateGrabStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar.physGrabber.grabStrength += 0.2f;
		}
	}

	public int UpgradePlayerThrowStrength(string _steamID)
	{
		statsManager.playerUpgradeThrow[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateThrowStrengthRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerThrowStrengthRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeThrow[_steamID]
			});
		}
		return statsManager.playerUpgradeThrow[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerThrowStrengthRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeThrow[_steamID] = value;
		UpdateGrabStrengthRightAway(_steamID);
	}

	private void UpdateThrowStrengthRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar.physGrabber.throwStrength += 0.3f;
		}
	}

	public int UpgradePlayerGrabRange(string _steamID)
	{
		statsManager.playerUpgradeRange[_steamID]++;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			UpdateGrabRangeRightAway(_steamID);
		}
		if (SemiFunc.IsMasterClient())
		{
			photonView.RPC("UpgradePlayerGrabRangeRPC", (RpcTarget)1, new object[2]
			{
				_steamID,
				statsManager.playerUpgradeRange[_steamID]
			});
		}
		return statsManager.playerUpgradeRange[_steamID];
	}

	[PunRPC]
	public void UpgradePlayerGrabRangeRPC(string _steamID, int value)
	{
		statsManager.playerUpgradeRange[_steamID] = value;
		UpdateGrabRangeRightAway(_steamID);
	}

	private void UpdateGrabRangeRightAway(string _steamID)
	{
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(_steamID);
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar.physGrabber.grabRange += 1f;
		}
	}

	public void SyncAllDictionaries()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		StatsManager.instance.statsSynced = true;
		if (!SemiFunc.IsMultiplayer() || !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		syncData.Clear();
		Hashtable val = new Hashtable();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, int>> dictionaryOfDictionary in statsManager.dictionaryOfDictionaries)
		{
			string key = dictionaryOfDictionary.Key;
			((Dictionary<object, object>)(object)val).Add((object)key, (object)ConvertToHashtable(dictionaryOfDictionary.Value));
			num++;
			num2++;
			num3++;
			list.Add(key);
			if (num > 3 || num2 == statsManager.dictionaryOfDictionaries.Count)
			{
				syncData.Add(val);
				list.Clear();
				num = 0;
			}
		}
		for (int i = 0; i < syncData.Count; i++)
		{
			bool flag = i == syncData.Count - 1;
			Hashtable val2 = syncData[i];
			photonView.RPC("ReceiveSyncData", (RpcTarget)1, new object[2] { val2, flag });
		}
		syncData.Clear();
	}

	private Hashtable ConvertToHashtable(Dictionary<string, int> dictionary)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		Hashtable val = new Hashtable();
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			((Dictionary<object, object>)(object)val).Add((object)item.Key, (object)item.Value);
		}
		return val;
	}

	private Dictionary<K, V> ConvertToDictionary<K, V>(Hashtable hashtable)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<K, V> dictionary = new Dictionary<K, V>();
		DictionaryEntryEnumerator enumerator = hashtable.GetEnumerator();
		try
		{
			while (((DictionaryEntryEnumerator)(ref enumerator)).MoveNext())
			{
				DictionaryEntry current = ((DictionaryEntryEnumerator)(ref enumerator)).Current;
				dictionary.Add((K)current.Key, (V)current.Value);
			}
			return dictionary;
		}
		finally
		{
			((IDisposable)(DictionaryEntryEnumerator)(ref enumerator)/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[PunRPC]
	public void ReceiveSyncData(Hashtable data, bool finalChunk)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, int>> dictionaryOfDictionary in statsManager.dictionaryOfDictionaries)
		{
			string key = dictionaryOfDictionary.Key;
			if (((Dictionary<object, object>)(object)data).ContainsKey((object)key))
			{
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			Dictionary<string, int> dictionary = statsManager.dictionaryOfDictionaries[item];
			DictionaryEntryEnumerator enumerator3 = ((Hashtable)data[(object)item]).GetEnumerator();
			try
			{
				while (((DictionaryEntryEnumerator)(ref enumerator3)).MoveNext())
				{
					DictionaryEntry current2 = ((DictionaryEntryEnumerator)(ref enumerator3)).Current;
					string key2 = (string)current2.Key;
					int value = (int)current2.Value;
					dictionary[key2] = value;
				}
			}
			finally
			{
				((IDisposable)(DictionaryEntryEnumerator)(ref enumerator3)/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (finalChunk)
		{
			StatsManager.instance.statsSynced = true;
		}
	}
}
