using UnityEngine;

public class ItemOrbBattery : MonoBehaviour, ITargetingCondition
{
	private ItemOrb itemOrb;

	private PhysGrabObject physGrabObject;

	public bool CustomTargetingCondition(GameObject target)
	{
		return SemiFunc.BatteryChargeCondition(target.GetComponent<ItemBattery>());
	}

	private void Start()
	{
		itemOrb = ((Component)this).GetComponent<ItemOrb>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	private void Update()
	{
		if (!itemOrb.itemActive || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject item in itemOrb.objectAffected)
		{
			if (Object.op_Implicit((Object)(object)item) && (Object)(object)physGrabObject != (Object)(object)item)
			{
				((Component)item).GetComponent<ItemBattery>().ChargeBattery(((Component)this).gameObject, SemiFunc.BatteryGetChargeRate(3));
			}
		}
	}
}
