using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public static ItemManager instance;

	public List<ItemVolume> itemVolumes;

	public List<Item> purchasedItems = new List<Item>();

	public List<PhysGrabObject> powerCrystals = new List<PhysGrabObject>();

	public List<ItemAttributes> spawnedItems = new List<ItemAttributes>();

	public List<string> localPlayerInventory = new List<string>();

	internal bool firstIcon = true;

	public GameObject itemIconLights;

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(TurnOffIconLights());
	}

	public void TurnOffIconLightsAgain()
	{
		((MonoBehaviour)this).StartCoroutine(TurnOffIconLights());
	}

	private IEnumerator TurnOffIconLights()
	{
		if (SemiFunc.RunIsShop() || SemiFunc.MenuLevel())
		{
			itemIconLights.SetActive(false);
			yield break;
		}
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			itemIconLights.SetActive(false);
			yield break;
		}
		while (spawnedItems.Exists((ItemAttributes x) => !x.hasIcon))
		{
			yield return (object)new WaitForSeconds(0.2f);
		}
		itemIconLights.SetActive(false);
	}

	public void ResetAllItems()
	{
		purchasedItems.Clear();
		powerCrystals.Clear();
	}

	public void ItemsInitialize()
	{
		if (!SemiFunc.RunIsArena() && (SemiFunc.RunIsLevel() || SemiFunc.RunIsLobby() || SemiFunc.RunIsTutorial()))
		{
			GetAllItemVolumesInScene();
			GetPurchasedItems();
			SemiFunc.TruckPopulateItemVolumes();
		}
	}

	public int IsInLocalPlayersInventory(string itemName)
	{
		for (int i = 0; i < localPlayerInventory.Count; i++)
		{
			if (localPlayerInventory[i] == itemName)
			{
				return i;
			}
		}
		return -1;
	}

	public void FetchLocalPlayersInventory()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		localPlayerInventory.Clear();
		Inventory inventory = Inventory.instance;
		if (!((Object)(object)inventory != (Object)null))
		{
			return;
		}
		foreach (InventorySpot allSpot in inventory.GetAllSpots())
		{
			ItemEquippable itemEquippable = allSpot?.CurrentItem;
			if ((Object)(object)itemEquippable != (Object)null)
			{
				ItemAttributes component = ((Component)itemEquippable).GetComponent<ItemAttributes>();
				if ((Object)(object)component != (Object)null)
				{
					localPlayerInventory.Add(component.item.itemName);
				}
			}
		}
	}

	public void GetAllItemVolumesInScene()
	{
		if (!SemiFunc.IsNotMasterClient())
		{
			itemVolumes.Clear();
			ItemVolume[] array = Object.FindObjectsOfType<ItemVolume>();
			foreach (ItemVolume item in array)
			{
				itemVolumes.Add(item);
			}
		}
	}

	public void AddSpawnedItem(ItemAttributes item)
	{
		spawnedItems.Add(item);
	}

	private void GetPurchasedItems()
	{
		purchasedItems.Clear();
		foreach (KeyValuePair<string, int> item2 in StatsManager.instance.itemsPurchased)
		{
			string key = item2.Key;
			int value = item2.Value;
			if (StatsManager.instance.itemDictionary.ContainsKey(key))
			{
				Item item = StatsManager.instance.itemDictionary[key];
				bool num = item.itemType == SemiFunc.itemType.power_crystal && !SemiFunc.RunIsLobby();
				bool flag = item.itemType == SemiFunc.itemType.cart && SemiFunc.RunIsLobby();
				if (!num && !flag && !item.disabled)
				{
					int num2 = Mathf.Clamp(value, 0, StatsManager.instance.itemDictionary[key].maxAmount);
					for (int i = 0; i < num2; i++)
					{
						purchasedItems.Add(item);
					}
				}
			}
			else
			{
				Debug.LogWarning((object)("Item '" + key + "' not found in the itemDictionary."));
			}
		}
	}
}
