using UnityEngine;

public class ItemOrbFeather : MonoBehaviour
{
	private ItemOrb itemOrb;

	private PhysGrabObject physGrabObject;

	private void Start()
	{
		itemOrb = ((Component)this).GetComponent<ItemOrb>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	private void Update()
	{
		if (!itemOrb.itemActive)
		{
			return;
		}
		if (itemOrb.localPlayerAffected)
		{
			PlayerController.instance.Feather(0.1f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject item in itemOrb.objectAffected)
		{
			if (!Object.op_Implicit((Object)(object)item) || !((Object)(object)physGrabObject != (Object)(object)item))
			{
				continue;
			}
			PlayerTumble component = ((Component)item).GetComponent<PlayerTumble>();
			if (!Object.op_Implicit((Object)(object)component))
			{
				item.OverrideMass(1f);
				item.OverrideDrag(1f);
				item.OverrideAngularDrag(5f);
				continue;
			}
			component.DisableCustomGravity(0.1f);
			item.OverrideMass(0.05f);
			if (component.playerAvatar.isLocal)
			{
				PlayerController.instance.Feather(0.1f);
			}
		}
	}
}
