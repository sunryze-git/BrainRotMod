using UnityEngine;

public class ItemUpgradePlayerGrabRange : MonoBehaviour
{
	private ItemToggle itemToggle;

	private void Start()
	{
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
	}

	public void Upgrade()
	{
		PunManager.instance.UpgradePlayerGrabRange(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID)));
	}
}
