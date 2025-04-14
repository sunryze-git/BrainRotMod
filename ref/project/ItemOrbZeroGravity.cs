using UnityEngine;

public class ItemOrbZeroGravity : MonoBehaviour
{
	private ItemOrb itemOrb;

	private PhysGrabObject physGrabObject;

	private ItemBattery itemBattery;

	private void Start()
	{
		itemOrb = ((Component)this).GetComponent<ItemOrb>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
	}

	private void BatteryDrain(float amount)
	{
		itemBattery.batteryLife -= amount * Time.deltaTime;
	}

	private void Update()
	{
		if (!itemOrb.itemActive)
		{
			return;
		}
		if (itemOrb.localPlayerAffected)
		{
			PlayerController.instance.AntiGravity(0.1f);
			BatteryDrain(0.5f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject item in itemOrb.objectAffected)
		{
			if (Object.op_Implicit((Object)(object)item) && (Object)(object)physGrabObject != (Object)(object)item)
			{
				item.OverrideDrag(0.5f);
				item.OverrideAngularDrag(0.5f);
				item.OverrideZeroGravity();
				if (!Object.op_Implicit((Object)(object)((Component)item).GetComponent<PlayerTumble>()))
				{
					BatteryDrain(0.5f);
				}
			}
		}
	}
}
