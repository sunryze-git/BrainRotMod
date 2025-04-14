using Photon.Pun;
using UnityEngine;

public class DebugJannek : MonoBehaviour
{
	private HurtCollider hurtCollider;

	private float hurtColliderTimer;

	private Transform playerTransform;

	private void Start()
	{
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>(true);
		((Component)hurtCollider).gameObject.SetActive(false);
	}

	private void Update()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if ((!LevelGenerator.Instance.Generated && Object.op_Implicit((Object)(object)SpectateCamera.instance) && GameDirector.instance.currentState == GameDirector.gameState.Main) || !Object.op_Implicit((Object)(object)PlayerController.instance.playerAvatarScript) || PlayerController.instance.playerAvatarScript.deadSet)
		{
			return;
		}
		((Component)this).transform.position = ((Component)Camera.main).transform.position;
		((Component)this).transform.rotation = ((Component)Camera.main).transform.rotation;
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
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
			PlayerController.instance.playerAvatarScript.playerHealth.Heal(30);
		}
	}
}
