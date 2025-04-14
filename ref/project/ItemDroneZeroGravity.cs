using Photon.Pun;
using UnityEngine;

public class ItemDroneZeroGravity : MonoBehaviour
{
	private ItemDrone itemDrone;

	private PhysGrabObject myPhysGrabObject;

	private ItemEquippable itemEquippable;

	private float tumbleEnemyTimer;

	private ItemBattery itemBattery;

	private void Start()
	{
		itemDrone = ((Component)this).GetComponent<ItemDrone>();
		myPhysGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
	}

	private void FixedUpdate()
	{
		if (itemDrone.magnetActive && Object.op_Implicit((Object)(object)itemDrone.magnetTargetPhysGrabObject))
		{
			if (Object.op_Implicit((Object)(object)itemDrone.playerTumbleTarget))
			{
				itemBattery.batteryLife -= 2f * Time.fixedDeltaTime;
				itemDrone.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky());
			}
			EnemyParent componentInParent = ((Component)itemDrone.magnetTargetPhysGrabObject).GetComponentInParent<EnemyParent>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				SemiFunc.ItemAffectEnemyBatteryDrain(componentInParent, itemBattery, tumbleEnemyTimer, Time.fixedDeltaTime);
				tumbleEnemyTimer += Time.fixedDeltaTime;
			}
		}
	}

	private void Update()
	{
		if (!itemDrone.itemActivated)
		{
			tumbleEnemyTimer = 0f;
		}
		if (itemEquippable.isEquipped)
		{
			return;
		}
		if (itemDrone.itemActivated && itemDrone.magnetActive && Object.op_Implicit((Object)(object)itemDrone.playerAvatarTarget) && itemDrone.targetIsLocalPlayer)
		{
			itemBattery.batteryLife -= 2f * Time.deltaTime;
			PlayerController.instance.AntiGravity(0.1f);
		}
		if ((GameManager.instance.gameMode != 1 || PhotonNetwork.IsMasterClient) && itemDrone.itemActivated)
		{
			myPhysGrabObject.OverrideZeroGravity();
			myPhysGrabObject.OverrideDrag(1f);
			myPhysGrabObject.OverrideAngularDrag(10f);
			if (itemDrone.magnetActive && Object.op_Implicit((Object)(object)itemDrone.magnetTargetPhysGrabObject))
			{
				itemDrone.magnetTargetPhysGrabObject.OverrideDrag(0.1f);
				itemDrone.magnetTargetPhysGrabObject.OverrideAngularDrag(0.1f);
				itemDrone.magnetTargetPhysGrabObject.OverrideZeroGravity();
			}
		}
	}
}
