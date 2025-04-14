using Photon.Pun;
using UnityEngine;

public class DebugAxel : MonoBehaviour
{
	private HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private Transform playerTransform;

	public Sound sound;

	private void Start()
	{
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>(true);
		((Component)hurtCollider).gameObject.SetActive(false);
	}

	private void Update()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown((KeyCode)288))
		{
			SpawnObject(AssetManager.instance.surplusValuableSmall, ((Component)this).transform.position + ((Component)this).transform.forward * 2f, "Valuables/");
		}
		if (Input.GetKeyDown((KeyCode)287))
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 999f);
			foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
			{
				item.playerDeathHead.inExtractionPoint = true;
				item.playerDeathHead.Revive();
			}
		}
		if ((!LevelGenerator.Instance.Generated && Object.op_Implicit((Object)(object)SpectateCamera.instance) && GameDirector.instance.currentState == GameDirector.gameState.Main) || !Object.op_Implicit((Object)(object)PlayerController.instance.playerAvatarScript) || PlayerController.instance.playerAvatarScript.deadSet)
		{
			return;
		}
		((Component)this).transform.position = ((Component)Camera.main).transform.position;
		((Component)this).transform.rotation = ((Component)Camera.main).transform.rotation;
		if (Input.GetKeyDown((KeyCode)283))
		{
			((Component)hurtCollider).gameObject.SetActive(true);
			hurtColliderTimer = 0.2f;
		}
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				((Component)hurtCollider).gameObject.SetActive(false);
			}
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
