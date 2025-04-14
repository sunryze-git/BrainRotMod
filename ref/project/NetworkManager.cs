using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public static NetworkManager instance;

	public float gameTime;

	private float syncInterval = 0.5f;

	private float lastSyncTime;

	public GameObject playerAvatarPrefab;

	private int instantiatedPlayerAvatars;

	private bool LoadingDone;

	internal bool leavePhotonRoom;

	private void Start()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		instance = this;
		if (PhotonNetwork.IsMasterClient)
		{
			lastSyncTime = 0f;
		}
		if (GameManager.instance.gameMode != 1)
		{
			return;
		}
		PhotonNetwork.Instantiate(((Object)playerAvatarPrefab).name, Vector3.zero, Quaternion.identity, (byte)0, (object[])null);
		PhotonNetwork.SerializationRate = 25;
		PhotonNetwork.SendRate = 25;
		bool flag = true;
		PhotonVoiceView[] array = Object.FindObjectsByType<PhotonVoiceView>((FindObjectsSortMode)0);
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			PhotonNetwork.Instantiate("Voice", Vector3.zero, Quaternion.identity, (byte)0, (object[])null);
		}
		((MonoBehaviourPun)this).photonView.RPC("PlayerSpawnedRPC", (RpcTarget)0, Array.Empty<object>());
	}

	[PunRPC]
	public void PlayerSpawnedRPC()
	{
		instantiatedPlayerAvatars++;
	}

	[PunRPC]
	public void AllPlayerSpawnedRPC()
	{
		LevelGenerator.Instance.AllPlayersReady = true;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext((object)lastSyncTime);
			stream.SendNext((object)instantiatedPlayerAvatars);
		}
		else
		{
			gameTime = (float)stream.ReceiveNext();
			instantiatedPlayerAvatars = (int)stream.ReceiveNext();
		}
	}

	private void Update()
	{
		if (GameManager.instance.gameMode != 1)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			if (!LoadingDone && instantiatedPlayerAvatars == PhotonNetwork.CurrentRoom.PlayerCount)
			{
				((MonoBehaviourPun)this).photonView.RPC("AllPlayerSpawnedRPC", (RpcTarget)3, Array.Empty<object>());
				LoadingDone = true;
			}
			gameTime += Time.deltaTime;
			if (Time.time - lastSyncTime > syncInterval)
			{
				lastSyncTime = gameTime;
			}
		}
		else
		{
			gameTime += Time.deltaTime;
		}
	}

	public void LeavePhotonRoom()
	{
		Debug.Log((object)"Leave Photon");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		leavePhotonRoom = false;
		if ((Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelTutorial)
		{
			TutorialDirector.instance.EndTutorial();
		}
		((MonoBehaviour)this).StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Debug.Log((object)("Player left room: " + otherPlayer.NickName));
	}

	public override void OnMasterClientSwitched(Player _newMasterClient)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Master client left...");
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.OutroStartRPC();
		}
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: Host disconnected", "Ok Dang");
		leavePhotonRoom = true;
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)$"Disconnected from server for reason {cause}");
		if ((int)cause == 17 || (int)cause == 9)
		{
			return;
		}
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + ((object)(DisconnectCause)(ref cause)/*cast due to .constrained prefix*/).ToString(), "Ok Dang");
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.OutroStartRPC();
		}
		leavePhotonRoom = true;
	}

	public void DestroyAll()
	{
		if (SemiFunc.IsMultiplayer())
		{
			Debug.Log((object)"Destroyed all network objects.");
			PhotonNetwork.DestroyAll();
		}
	}
}
