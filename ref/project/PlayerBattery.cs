using System;
using Photon.Pun;
using UnityEngine;

public class PlayerBattery : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	private PhotonView photonView;

	private StaticGrabObject staticGrabObject;

	private bool masterCharging;

	private bool isLocal;

	private bool chargeBattery;

	private float chargeRate = 0.5f;

	private float chargeTimer;

	private int amountPlayersGrabbing;

	private int amountPlayersGrabbingPrevious;

	public Transform batteryPlacement;

	public Sound batteryChargeSound;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		staticGrabObject = ((Component)this).GetComponent<StaticGrabObject>();
	}

	private void Update()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if ((isLocal || playerAvatar.isLocal) && !isLocal)
		{
			((Component)this).GetComponent<Collider>().enabled = false;
			((Renderer)((Component)this).GetComponent<MeshRenderer>()).enabled = false;
			isLocal = true;
		}
		((Component)this).transform.position = batteryPlacement.position;
		((Component)this).transform.rotation = batteryPlacement.rotation;
		if (PhotonNetwork.IsMasterClient)
		{
			if (staticGrabObject.playerGrabbing.Count > 0 && !masterCharging)
			{
				masterCharging = true;
				photonView.RPC("BatteryChargeStart", (RpcTarget)0, Array.Empty<object>());
			}
			if (staticGrabObject.playerGrabbing.Count <= 0 && masterCharging)
			{
				masterCharging = false;
				photonView.RPC("BatteryChargeEnd", (RpcTarget)0, Array.Empty<object>());
			}
		}
		if (!chargeBattery)
		{
			return;
		}
		if (chargeTimer < chargeRate)
		{
			chargeTimer += Time.deltaTime;
			return;
		}
		batteryChargeSound.Play(((Component)this).transform.position);
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (PhysGrabber item in staticGrabObject.playerGrabbing)
			{
				_ = item;
			}
			amountPlayersGrabbing = staticGrabObject.playerGrabbing.Count;
			if (amountPlayersGrabbing != amountPlayersGrabbingPrevious)
			{
				photonView.RPC("UpdateAmountPlayersGrabbing", (RpcTarget)1, new object[1] { amountPlayersGrabbing });
				amountPlayersGrabbingPrevious = amountPlayersGrabbing;
			}
		}
		if (playerAvatar.isLocal)
		{
			PlayerController.instance.EnergyCurrent += 1f * (float)amountPlayersGrabbing;
		}
		chargeTimer = 0f;
	}

	[PunRPC]
	private void UpdateAmountPlayersGrabbing(int amount)
	{
		amountPlayersGrabbing = amount;
	}

	[PunRPC]
	private void BatteryChargeStart()
	{
		chargeBattery = true;
	}

	[PunRPC]
	private void BatteryChargeEnd()
	{
		chargeBattery = false;
	}
}
