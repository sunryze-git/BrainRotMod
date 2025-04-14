using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public static Inventory instance;

	internal readonly List<InventorySpot> inventorySpots = new List<InventorySpot>();

	internal PhysGrabber physGrabber;

	private PlayerController playerController;

	private PlayerAvatar playerAvatar;

	internal bool spotsFeched;

	private void Awake()
	{
		if ((Object)(object)instance != (Object)null && (Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public void InventorySpotAddAtIndex(InventorySpot spot, int index)
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		inventorySpots[index] = spot;
		foreach (InventorySpot inventorySpot in inventorySpots)
		{
			if ((Object)(object)inventorySpot == (Object)null)
			{
				return;
			}
		}
		spotsFeched = true;
	}

	private void Start()
	{
		if (SemiFunc.RunIsArena())
		{
			((Behaviour)this).enabled = false;
		}
		playerController = ((Component)this).GetComponent<PlayerController>();
		for (int i = 0; i < 3; i++)
		{
			inventorySpots.Add(null);
		}
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		physGrabber = playerController.playerAvatarScript.physGrabber;
		playerAvatar = playerController.playerAvatarScript;
	}

	public InventorySpot GetSpotByIndex(int index)
	{
		return inventorySpots[index];
	}

	public bool IsItemEquipped(ItemEquippable item)
	{
		foreach (InventorySpot inventorySpot in inventorySpots)
		{
			if (Object.op_Implicit((Object)(object)inventorySpot) && (Object)(object)inventorySpot.CurrentItem == (Object)(object)item)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSpotOccupied(int index)
	{
		InventorySpot spotByIndex = GetSpotByIndex(index);
		if ((Object)(object)spotByIndex != (Object)null)
		{
			return spotByIndex.IsOccupied();
		}
		return false;
	}

	public List<InventorySpot> GetAllSpots()
	{
		return inventorySpots;
	}

	public int GetFirstFreeInventorySpotIndex()
	{
		List<InventorySpot> allSpots = instance.GetAllSpots();
		for (int i = 0; i < allSpots.Count; i++)
		{
			if (!allSpots[i].IsOccupied())
			{
				return i;
			}
		}
		return -1;
	}

	public int InventorySpotsOccupied()
	{
		int num = 0;
		foreach (InventorySpot inventorySpot in inventorySpots)
		{
			if (Object.op_Implicit((Object)(object)inventorySpot) && inventorySpot.IsOccupied())
			{
				num++;
			}
		}
		return num;
	}

	public void InventoryDropAll(Vector3 dropPosition, int playerViewID)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in inventorySpots)
		{
			if (inventorySpot.IsOccupied())
			{
				ItemEquippable currentItem = inventorySpot.CurrentItem;
				if ((Object)(object)currentItem != (Object)null)
				{
					currentItem.ForceUnequip(dropPosition, playerViewID);
				}
			}
		}
	}

	public int GetBatteryStateFromInventorySpot(int index)
	{
		InventorySpot spotByIndex = GetSpotByIndex(index);
		if ((Object)(object)spotByIndex != (Object)null && spotByIndex.IsOccupied())
		{
			ItemEquippable currentItem = spotByIndex.CurrentItem;
			if ((Object)(object)currentItem != (Object)null)
			{
				ItemBattery component = ((Component)currentItem).GetComponent<ItemBattery>();
				if ((Object)(object)component != (Object)null)
				{
					return component.batteryLifeInt;
				}
			}
		}
		return -1;
	}

	public void ForceUnequip()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		foreach (InventorySpot inventorySpot in inventorySpots)
		{
			if (!inventorySpot.IsOccupied())
			{
				continue;
			}
			ItemEquippable currentItem = inventorySpot.CurrentItem;
			if (Object.op_Implicit((Object)(object)currentItem))
			{
				if (SemiFunc.IsMultiplayer())
				{
					((Component)currentItem).GetComponent<ItemEquippable>().ForceUnequip(playerAvatar.PlayerVisionTarget.VisionTransform.position, physGrabber.photonView.ViewID);
				}
				else
				{
					((Component)currentItem).GetComponent<ItemEquippable>().ForceUnequip(playerAvatar.PlayerVisionTarget.VisionTransform.position, -1);
				}
			}
		}
	}
}
