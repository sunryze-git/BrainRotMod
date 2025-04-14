using UnityEngine;

public class ItemDroneBattery : MonoBehaviour, ITargetingCondition
{
	private ItemDrone itemDrone;

	private PhysGrabObject myPhysGrabObject;

	private ItemEquippable itemEquippable;

	private ItemBattery itemBattery;

	private void Start()
	{
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemDrone = ((Component)this).GetComponent<ItemDrone>();
		myPhysGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	private void Update()
	{
		if (itemEquippable.isEquipped || !SemiFunc.IsMasterClientOrSingleplayer() || !itemDrone.itemActivated)
		{
			return;
		}
		myPhysGrabObject.OverrideZeroGravity();
		myPhysGrabObject.OverrideDrag(1f);
		myPhysGrabObject.OverrideAngularDrag(10f);
		if (itemDrone.magnetActive && Object.op_Implicit((Object)(object)itemDrone.magnetTargetPhysGrabObject))
		{
			ItemBattery component = ((Component)itemDrone.magnetTargetPhysGrabObject).GetComponent<ItemBattery>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.ChargeBattery(((Component)this).gameObject, 5f);
				itemBattery.Drain(5f);
			}
			if (component.batteryLife >= 99f && !component.batteryActive && component.autoDrain)
			{
				itemDrone.MagnetActiveToggle(toggleBool: false);
			}
		}
	}
}
