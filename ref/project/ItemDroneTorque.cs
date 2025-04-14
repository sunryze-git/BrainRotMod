using Photon.Pun;
using UnityEngine;

public class ItemDroneTorque : MonoBehaviour
{
	private ItemDrone itemDrone;

	private PhysGrabObject myPhysGrabObject;

	private ItemEquippable itemEquippable;

	private ItemToggle itemToggle;

	private ItemBattery itemBattery;

	private ItemAttributes itemAttributes;

	private float tumbleEnemyTimer;

	private bool tumbledPlayer;

	private void Start()
	{
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemDrone = ((Component)this).GetComponent<ItemDrone>();
		myPhysGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
	}

	private void RollTowards(Vector3 direction, Rigidbody targetRb)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(Vector3.up, direction);
		Vector3 val2 = ((Vector3)(ref val)).normalized * 6f;
		float num = Mathf.Clamp(3f / targetRb.mass, 1f, 10f);
		val2 *= num;
		targetRb.angularVelocity = val2 / targetRb.mass;
	}

	private void BatteryDrain(float amount)
	{
		itemBattery.batteryLife -= amount * Time.fixedDeltaTime;
	}

	private void FixedUpdate()
	{
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		if (!itemDrone.itemActivated)
		{
			tumbledPlayer = false;
			tumbleEnemyTimer = 0f;
		}
		if (itemEquippable.isEquipped || (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient) || !itemDrone.itemActivated)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)itemDrone.droneOwner) || (Object.op_Implicit((Object)(object)itemDrone.droneOwner) && itemDrone.droneOwner.isDisabled))
		{
			itemToggle.ToggleItem(toggle: false);
			return;
		}
		myPhysGrabObject.OverrideZeroGravity();
		myPhysGrabObject.OverrideDrag(1f);
		myPhysGrabObject.OverrideAngularDrag(10f);
		if (!itemDrone.magnetActive)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)itemDrone.playerAvatarTarget) && !tumbledPlayer)
		{
			if (!itemDrone.playerAvatarTarget.tumble.isTumbling)
			{
				itemDrone.playerAvatarTarget.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			}
			tumbledPlayer = true;
		}
		if (Object.op_Implicit((Object)(object)itemDrone.playerTumbleTarget))
		{
			Vector3 forward = itemDrone.playerTumbleTarget.playerAvatar.localCameraTransform.forward;
			if (SemiFunc.OnGroundCheck(itemDrone.magnetTargetRigidbody.position, 1.5f, itemDrone.magnetTargetPhysGrabObject))
			{
				Vector3 val = SemiFunc.PhysFollowDirection(((Component)itemDrone.magnetTargetRigidbody).transform, forward, itemDrone.magnetTargetRigidbody, 20f);
				itemDrone.magnetTargetRigidbody.AddTorque(val * 2f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
				Vector3 val2 = SemiFunc.PhysFollowPosition(itemDrone.magnetTargetRigidbody.position, itemDrone.magnetTargetRigidbody.position + forward * 2f, itemDrone.magnetTargetRigidbody.velocity, 25f);
				itemDrone.magnetTargetRigidbody.AddForce(val2 * 2f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
				BatteryDrain(2f);
				if (Object.op_Implicit((Object)(object)itemDrone.magnetTargetPhysGrabObject))
				{
					itemDrone.magnetTargetPhysGrabObject.OverrideMaterial(SemiFunc.PhysicMaterialSticky());
				}
			}
		}
		if (!Object.op_Implicit((Object)(object)itemDrone.magnetTargetPhysGrabObject) || Object.op_Implicit((Object)(object)itemDrone.playerAvatarTarget) || Object.op_Implicit((Object)(object)itemDrone.playerTumbleTarget))
		{
			return;
		}
		Rigidbody magnetTargetRigidbody = itemDrone.magnetTargetRigidbody;
		Transform transform = ((Component)itemDrone.droneOwner).transform;
		Vector3 val3;
		if (Object.op_Implicit((Object)(object)transform))
		{
			float num = Vector3.Distance(new Vector3(magnetTargetRigidbody.position.x, 0f, magnetTargetRigidbody.position.z), new Vector3(transform.position.x, 0f, transform.position.z));
			val3 = transform.position - magnetTargetRigidbody.position;
			Vector3 val4 = ((Vector3)(ref val3)).normalized;
			if (itemDrone.magnetTargetPhysGrabObject.isEnemy)
			{
				EnemyParent componentInParent = ((Component)itemDrone.magnetTargetPhysGrabObject).GetComponentInParent<EnemyParent>();
				if (Object.op_Implicit((Object)(object)componentInParent))
				{
					SemiFunc.ItemAffectEnemyBatteryDrain(componentInParent, itemBattery, tumbleEnemyTimer, Time.fixedDeltaTime);
				}
				tumbleEnemyTimer += Time.fixedDeltaTime;
				val4 = -val4;
			}
			float num2 = 2f;
			float num3 = Mathf.Clamp(magnetTargetRigidbody.mass / 1f, 0.2f, 1f);
			float num4 = num3 * 2f;
			if (num < num4)
			{
				num2 = Mathf.Clamp(num - num3, 0f, num4) / num4;
			}
			Vector3 val5 = SemiFunc.PhysFollowDirection(((Component)itemDrone.magnetTargetRigidbody).transform, val4, itemDrone.magnetTargetRigidbody, 10f) * num2;
			itemDrone.magnetTargetRigidbody.AddTorque(val5 * 5f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
			Vector3 val6 = SemiFunc.PhysFollowPosition(itemDrone.magnetTargetRigidbody.position, itemDrone.magnetTargetRigidbody.position + val4, itemDrone.magnetTargetRigidbody.velocity, 10f) * num2;
			itemDrone.magnetTargetRigidbody.AddForce(val6 * 2f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
			itemDrone.magnetTargetPhysGrabObject.OverrideFragility(0.65f);
		}
		else
		{
			val3 = ((Component)this).transform.forward;
			Vector3 val7 = -((Vector3)(ref val3)).normalized;
			Vector3 val8 = SemiFunc.PhysFollowDirection(((Component)itemDrone.magnetTargetRigidbody).transform, val7, itemDrone.magnetTargetRigidbody, 10f);
			itemDrone.magnetTargetRigidbody.AddTorque(val8 * 2f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
			Vector3 val9 = SemiFunc.PhysFollowPosition(itemDrone.magnetTargetRigidbody.position, itemDrone.magnetTargetRigidbody.position + val7, itemDrone.magnetTargetRigidbody.velocity, 10f);
			itemDrone.magnetTargetRigidbody.AddForce(val9 * 1f / itemDrone.magnetTargetRigidbody.mass, (ForceMode)0);
		}
	}
}
