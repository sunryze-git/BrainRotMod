using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventorySpot : SemiUI
{
	public enum SpotState
	{
		Empty,
		Occupied
	}

	[FormerlySerializedAs("SpotIndex")]
	public int inventorySpotIndex;

	private PhotonView photonView;

	private float equipCooldown = 0.2f;

	private float lastEquipTime;

	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private SpotState currentState;

	private InventoryBattery battery;

	internal Image inventoryIcon;

	private bool handleInput;

	public TextMeshProUGUI noItem;

	public ItemEquippable CurrentItem { get; private set; }

	protected override void Start()
	{
		inventoryIcon = ((Component)this).GetComponentInChildren<Image>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		UpdateUI();
		currentState = SpotState.Empty;
		battery = ((Component)this).GetComponentInChildren<InventoryBattery>();
		base.Start();
		uiText = null;
		SetEmoji(null);
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		if (!SemiFunc.MenuLevel() && !SemiFunc.RunIsLobbyMenu() && !SemiFunc.RunIsArena())
		{
			Inventory.instance.InventorySpotAddAtIndex(this, inventorySpotIndex);
		}
	}

	public void SetEmoji(Sprite emoji)
	{
		if (!Object.op_Implicit((Object)(object)emoji))
		{
			((Behaviour)inventoryIcon).enabled = false;
			((Behaviour)noItem).enabled = true;
		}
		else
		{
			((Behaviour)noItem).enabled = false;
			((Behaviour)inventoryIcon).enabled = true;
			inventoryIcon.sprite = emoji;
		}
	}

	public bool IsOccupied()
	{
		return currentState == SpotState.Occupied;
	}

	public void EquipItem(ItemEquippable item)
	{
		if (currentState == SpotState.Empty)
		{
			CurrentItem = item;
			currentState = SpotState.Occupied;
			UpdateUI();
		}
	}

	public void UnequipItem()
	{
		if (currentState == SpotState.Occupied)
		{
			CurrentItem = null;
			currentState = SpotState.Empty;
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		if (currentState == SpotState.Occupied && (Object)(object)CurrentItem != (Object)null)
		{
			SemiUISpringScale(0.5f, 2f, 0.2f);
			SetEmoji(((Component)CurrentItem).GetComponent<ItemAttributes>().icon);
		}
		else
		{
			SetEmoji(null);
			SemiUISpringScale(0.5f, 2f, 0.2f);
		}
	}

	protected override void Update()
	{
		if (SemiFunc.InputDown(InputKey.Inventory1) && inventorySpotIndex == 0)
		{
			HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory2) && inventorySpotIndex == 1)
		{
			HandleInput();
		}
		else if (SemiFunc.InputDown(InputKey.Inventory3) && inventorySpotIndex == 2)
		{
			HandleInput();
		}
		switch (currentState)
		{
		case SpotState.Empty:
			StateEmpty();
			break;
		case SpotState.Occupied:
			StateOccupied();
			break;
		}
		base.Update();
	}

	private void HandleInput()
	{
		if (!SemiFunc.RunIsArena() && !(PlayerController.instance.InputDisableTimer > 0f) && (handleInput || !(Time.time - lastEquipTime < equipCooldown)))
		{
			PhysGrabber.instance.OverrideGrabRelease();
			lastEquipTime = Time.time;
			handleInput = false;
			if (IsOccupied())
			{
				CurrentItem.RequestUnequip();
			}
			else
			{
				AttemptEquipItem();
			}
		}
	}

	private void AttemptEquipItem()
	{
		ItemEquippable itemPlayerIsHolding = GetItemPlayerIsHolding();
		if ((Object)(object)itemPlayerIsHolding != (Object)null)
		{
			itemPlayerIsHolding.RequestEquip(inventorySpotIndex, PhysGrabber.instance.photonView.ViewID);
		}
	}

	private ItemEquippable GetItemPlayerIsHolding()
	{
		if (!PhysGrabber.instance.grabbed)
		{
			return null;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (grabbedPhysGrabObject == null)
		{
			return null;
		}
		return ((Component)grabbedPhysGrabObject).GetComponent<ItemEquippable>();
	}

	private void StateOccupied()
	{
		if (currentState == SpotState.Occupied && Object.op_Implicit((Object)(object)CurrentItem) && Object.op_Implicit((Object)(object)((Component)CurrentItem).GetComponent<ItemBattery>()))
		{
			battery.BatteryFetch();
			battery.BatteryShow();
		}
	}

	private void StateEmpty()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == SpotState.Empty)
		{
			SemiUIScoot(new Vector2(0f, -20f));
		}
	}
}
