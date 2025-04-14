using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ModuleConnectObject : MonoBehaviourPunCallbacks, IPunObservable
{
	public bool ModuleConnecting;

	public bool MasterSetup;

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(ConnectingCheck());
	}

	private IEnumerator ConnectingCheck()
	{
		while (!MasterSetup)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		((Component)this).transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext((object)ModuleConnecting);
			stream.SendNext((object)MasterSetup);
		}
		else
		{
			ModuleConnecting = (bool)stream.ReceiveNext();
			MasterSetup = (bool)stream.ReceiveNext();
		}
	}
}
