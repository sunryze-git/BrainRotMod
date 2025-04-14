using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class PhysGrabObjectGrabArea : MonoBehaviour
{
	[Serializable]
	public class GrabArea
	{
		public Transform grabAreaTransform;

		[Space(20f)]
		public UnityEvent grabAreaEventOnStart = new UnityEvent();

		public UnityEvent grabAreaEventOnRelease = new UnityEvent();

		public UnityEvent grabAreaEventOnHolding = new UnityEvent();

		[HideInInspector]
		public bool grabAreaActive;

		[HideInInspector]
		public List<PhysGrabber> listOfGrabbers = new List<PhysGrabber>();

		[HideInInspector]
		public List<Collider> grabAreaColliders = new List<Collider>();
	}

	private PhysGrabObject physGrabObject;

	private StaticGrabObject staticGrabObject;

	private PhotonView photonView;

	[HideInInspector]
	public List<PhysGrabber> listOfAllGrabbers = new List<PhysGrabber>();

	public List<GrabArea> grabAreas = new List<GrabArea>();

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponentInParent<PhysGrabObject>();
		staticGrabObject = ((Component)this).GetComponentInParent<StaticGrabObject>();
		photonView = ((Component)this).GetComponentInParent<PhotonView>();
		foreach (GrabArea grabArea in grabAreas)
		{
			if (Object.op_Implicit((Object)(object)grabArea.grabAreaTransform))
			{
				if (grabArea.grabAreaTransform.childCount == 0)
				{
					Collider component = ((Component)grabArea.grabAreaTransform).GetComponent<Collider>();
					if ((Object)(object)component != (Object)null)
					{
						grabArea.grabAreaColliders.Add(component);
					}
					else
					{
						Debug.LogWarning((object)("Grab area '" + ((Object)grabArea.grabAreaTransform).name + "' is missing a Collider component."));
					}
				}
				else
				{
					Collider[] componentsInChildren = ((Component)grabArea.grabAreaTransform).GetComponentsInChildren<Collider>();
					if (componentsInChildren.Length != 0)
					{
						grabArea.grabAreaColliders.AddRange(componentsInChildren);
					}
					else
					{
						Debug.LogWarning((object)("Grab area '" + ((Object)grabArea.grabAreaTransform).name + "' has children but no colliders."));
					}
				}
			}
			else
			{
				Debug.LogWarning((object)("Grab area in '" + ((Object)((Component)this).gameObject).name + "' has a missing Transform. Please assign it."));
			}
		}
	}

	public PlayerAvatar GetLatestGrabber()
	{
		if (listOfAllGrabbers.Count > 0)
		{
			return listOfAllGrabbers[listOfAllGrabbers.Count - 1].playerAvatar;
		}
		return null;
	}

	private void Update()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		foreach (PhysGrabber item in (Object.op_Implicit((Object)(object)physGrabObject) ? physGrabObject.playerGrabbing : staticGrabObject.playerGrabbing).ToList())
		{
			if (item.initialPressTimer <= 0f)
			{
				continue;
			}
			Vector3 position = item.physGrabPoint.position;
			foreach (GrabArea grabArea in grabAreas)
			{
				if (grabArea.grabAreaColliders.Count == 0)
				{
					continue;
				}
				bool flag = false;
				foreach (Collider grabAreaCollider in grabArea.grabAreaColliders)
				{
					if (grabAreaCollider.ClosestPoint(position) == position)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (!grabArea.listOfGrabbers.Contains(item))
				{
					grabArea.listOfGrabbers.Add(item);
					if (!listOfAllGrabbers.Contains(item))
					{
						listOfAllGrabbers.Add(item);
						UpdateList(add: true, item);
					}
					UnityEvent grabAreaEventOnStart = grabArea.grabAreaEventOnStart;
					if (grabAreaEventOnStart != null)
					{
						grabAreaEventOnStart.Invoke();
					}
				}
				else
				{
					UnityEvent grabAreaEventOnHolding = grabArea.grabAreaEventOnHolding;
					if (grabAreaEventOnHolding != null)
					{
						grabAreaEventOnHolding.Invoke();
					}
				}
				grabArea.grabAreaActive = true;
				break;
			}
		}
		foreach (GrabArea grabArea2 in grabAreas)
		{
			for (int num = grabArea2.listOfGrabbers.Count - 1; num >= 0; num--)
			{
				PhysGrabber physGrabber = grabArea2.listOfGrabbers[num];
				if (!physGrabber.grabbed)
				{
					UpdateList(add: false, physGrabber);
					listOfAllGrabbers.Remove(physGrabber);
					grabArea2.listOfGrabbers.RemoveAt(num);
				}
			}
		}
		foreach (GrabArea grabArea3 in grabAreas)
		{
			if (grabArea3.listOfGrabbers.Count == 0 && grabArea3.grabAreaActive)
			{
				UnityEvent grabAreaEventOnRelease = grabArea3.grabAreaEventOnRelease;
				if (grabAreaEventOnRelease != null)
				{
					grabAreaEventOnRelease.Invoke();
				}
				grabArea3.grabAreaActive = false;
			}
		}
	}

	[PunRPC]
	public void AddToGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = FindGrabberById(grabberId);
		if ((Object)(object)physGrabber != (Object)null && !listOfAllGrabbers.Contains(physGrabber))
		{
			listOfAllGrabbers.Add(physGrabber);
		}
	}

	[PunRPC]
	public void RemoveFromGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = FindGrabberById(grabberId);
		if ((Object)(object)physGrabber != (Object)null)
		{
			listOfAllGrabbers.Remove(physGrabber);
		}
	}

	private PhysGrabber FindGrabberById(int id)
	{
		foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
		{
			PhysGrabber componentInChildren = ((Component)item).GetComponentInChildren<PhysGrabber>();
			if ((Object)(object)componentInChildren != (Object)null && componentInChildren.photonView.ViewID == id)
			{
				return componentInChildren;
			}
		}
		return null;
	}

	private void UpdateList(bool add, PhysGrabber grabber)
	{
		if (SemiFunc.IsMultiplayer() && !((Object)(object)grabber == (Object)null))
		{
			int viewID = grabber.photonView.ViewID;
			if (add)
			{
				photonView.RPC("AddToGrabbersList", (RpcTarget)1, new object[1] { viewID });
			}
			else
			{
				photonView.RPC("RemoveFromGrabbersList", (RpcTarget)1, new object[1] { viewID });
			}
		}
	}
}
