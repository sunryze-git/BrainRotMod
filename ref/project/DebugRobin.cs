using Photon.Pun;
using UnityEngine;

public class DebugRobin : MonoBehaviour
{
	private Transform playerTransform;

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown((KeyCode)287))
		{
			SpawnObject(AssetManager.instance.surplusValuableMedium, ((Component)this).transform.position + ((Component)this).transform.forward * 2f, "Valuables/");
		}
		if (Input.GetKeyDown((KeyCode)285))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Hurt(10, savingGrace: true);
		}
		if (Input.GetKeyDown((KeyCode)286))
		{
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(10);
		}
	}

	private void SpawnObject(GameObject _object, Vector3 _position, string _path)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMultiplayer())
		{
			Object.Instantiate<GameObject>(_object, _position, Quaternion.identity);
		}
		else if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			PhotonNetwork.InstantiateRoomObject(_path + ((Object)_object).name, _position, Quaternion.identity, (byte)0, (object[])null);
		}
	}
}
