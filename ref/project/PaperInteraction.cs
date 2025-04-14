using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PaperInteraction : MonoBehaviour
{
	public List<GameObject> papers;

	[HideInInspector]
	public bool Picked;

	public Transform PaperTransform;

	[HideInInspector]
	public GameObject paperVisual;

	public CleanEffect CleanEffect;

	private PhotonView photonView;

	private bool destructionToMaster;

	private bool destructionToOthers;

	private void Start()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 1)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				int num = Random.Range(0, papers.Count);
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(0f, (float)Random.Range(0, 360), 0f);
				photonView.RPC("SyncPaperVisual", (RpcTarget)3, new object[2] { num, val });
			}
		}
		else
		{
			paperVisual = Object.Instantiate<GameObject>(papers[Random.Range(0, papers.Count)], ((Component)this).transform.position, Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f));
			paperVisual.transform.parent = PaperTransform;
		}
	}

	private void Update()
	{
		if (!Picked)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (!destructionToMaster)
			{
				photonView.RPC("DestroyPaper", (RpcTarget)2, Array.Empty<object>());
				destructionToMaster = true;
			}
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	[PunRPC]
	public void SyncPaperVisual(int randomPaper, Vector3 randomRotation)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		paperVisual = Object.Instantiate<GameObject>(papers[randomPaper], ((Component)this).transform.position, Quaternion.Euler(randomRotation));
		paperVisual.transform.parent = PaperTransform;
	}

	[PunRPC]
	public void DestroyPaper()
	{
		if (!destructionToOthers)
		{
			PhotonNetwork.Destroy(((Component)this).gameObject);
			destructionToOthers = true;
		}
	}
}
