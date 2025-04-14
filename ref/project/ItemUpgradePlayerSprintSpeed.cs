using UnityEngine;

public class ItemUpgradePlayerSprintSpeed : MonoBehaviour
{
	private ItemToggle itemToggle;

	private void Start()
	{
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
	}

	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerSprintSpeed(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID)));
	}
}
