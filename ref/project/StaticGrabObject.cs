using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StaticGrabObject : MonoBehaviour
{
	private PhotonView photonView;

	private bool isMaster;

	public Transform colliderTransform;

	[HideInInspector]
	public Vector3 velocity;

	[HideInInspector]
	public bool grabbed;

	public List<PhysGrabber> playerGrabbing = new List<PhysGrabber>();

	[HideInInspector]
	public bool dead;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient)
		{
			isMaster = true;
			photonView.TransferOwnership(PhotonNetwork.MasterClient);
		}
	}

	private void Update()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (grabbed)
		{
			for (int i = 0; i < playerGrabbing.Count; i++)
			{
				if (!Object.op_Implicit((Object)(object)playerGrabbing[i]))
				{
					playerGrabbing.RemoveAt(i);
				}
			}
		}
		if (GameManager.instance.gameMode != 0 && !isMaster)
		{
			return;
		}
		velocity = Vector3.zero;
		foreach (PhysGrabber item in playerGrabbing)
		{
			Vector3 val = (item.physGrabPointPullerPosition - item.physGrabPoint.position) * 5f;
			velocity += val * Time.deltaTime;
		}
		if (dead && playerGrabbing.Count == 0)
		{
			DestroyPhysGrabObject();
		}
	}

	private void OnDisable()
	{
		playerGrabbing.Clear();
		grabbed = false;
	}

	public void GrabLink(int playerPhotonID, Vector3 point)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		photonView.RPC("GrabLinkRPC", (RpcTarget)0, new object[2] { playerPhotonID, point });
	}

	[PunRPC]
	private void GrabLinkRPC(int playerPhotonID, Vector3 point)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		component.physGrabPoint.position = point;
		component.localGrabPosition = colliderTransform.InverseTransformPoint(point);
		component.grabbedObjectTransform = colliderTransform;
		component.grabbed = true;
		if (component.photonView.IsMine)
		{
			Vector3 localPosition = component.physGrabPoint.localPosition;
			photonView.RPC("GrabPointSyncRPC", (RpcTarget)2, new object[2] { playerPhotonID, localPosition });
		}
	}

	[PunRPC]
	private void GrabPointSyncRPC(int playerPhotonID, Vector3 localPointInBox)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>().physGrabPoint.localPosition = localPointInBox;
	}

	public void GrabStarted(PhysGrabber player)
	{
		if (grabbed)
		{
			return;
		}
		grabbed = true;
		if (GameManager.instance.gameMode == 0)
		{
			if (!playerGrabbing.Contains(player))
			{
				playerGrabbing.Add(player);
			}
		}
		else
		{
			photonView.RPC("GrabStartedRPC", (RpcTarget)2, new object[1] { player.photonView.ViewID });
		}
	}

	[PunRPC]
	private void GrabStartedRPC(int playerPhotonID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		if (!playerGrabbing.Contains(component))
		{
			playerGrabbing.Add(component);
		}
	}

	public void GrabEnded(PhysGrabber player)
	{
		if (!grabbed)
		{
			return;
		}
		grabbed = false;
		if (GameManager.instance.gameMode == 0)
		{
			if (playerGrabbing.Contains(player))
			{
				playerGrabbing.Remove(player);
			}
		}
		else
		{
			photonView.RPC("GrabEndedRPC", (RpcTarget)2, new object[1] { player.photonView.ViewID });
		}
	}

	[PunRPC]
	private void GrabEndedRPC(int playerPhotonID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(playerPhotonID)).GetComponent<PhysGrabber>();
		component.grabbed = false;
		if (playerGrabbing.Contains(component))
		{
			playerGrabbing.Remove(component);
		}
	}

	private void DestroyPhysGrabObject()
	{
		if (GameManager.instance.gameMode == 0)
		{
			DestroyPhysObjectFailsafe();
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			photonView.RPC("DestroyPhysGrabObjectRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void DestroyPhysGrabObjectRPC()
	{
		DestroyPhysObjectFailsafe();
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void DestroyPhysObjectFailsafe()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			if (((Component)val).CompareTag("Phys Grab Controller"))
			{
				val.SetParent((Transform)null);
			}
		}
	}
}
