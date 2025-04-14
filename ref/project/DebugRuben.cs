using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DebugRuben : MonoBehaviour
{
	private PhotonView photonView;

	private HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private Transform playerTransform;

	public List<GameObject> spawnObjects;

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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.KeyDownRuben((KeyCode)287))
		{
			SpawnObject(AssetManager.instance.surplusValuableSmall, ((Component)this).transform.position + ((Component)this).transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben((KeyCode)288))
		{
			SpawnObject(AssetManager.instance.surplusValuableBig, ((Component)this).transform.position + ((Component)this).transform.forward * 2f, "Valuables/");
		}
		if (SemiFunc.KeyDownRuben((KeyCode)286))
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 999f);
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
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(75);
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
