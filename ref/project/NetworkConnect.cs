using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviourPunCallbacks
{
	public static NetworkConnect instance;

	private bool joinedRoom;

	private string RoomName;

	private bool ConnectedToMasterServer;

	public GameObject punVoiceClient;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		PhotonNetwork.NickName = SteamClient.Name;
		PhotonNetwork.AutomaticallySyncScene = false;
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
		Object.Instantiate<GameObject>(punVoiceClient, Vector3.zero, Quaternion.identity);
		PhotonNetwork.Disconnect();
		((MonoBehaviour)this).StartCoroutine(CreateLobby());
	}

	private IEnumerator CreateLobby()
	{
		while ((int)PhotonNetwork.NetworkingClient.State != 14 && (int)PhotonNetwork.NetworkingClient.State != 0)
		{
			yield return null;
		}
		if (!GameManager.instance.localTest)
		{
			SteamId id = ((Lobby)(ref SteamManager.instance.currentLobby)).Id;
			if (((SteamId)(ref id)).IsValid)
			{
				NetworkConnect networkConnect = this;
				id = ((Lobby)(ref SteamManager.instance.currentLobby)).Id;
				networkConnect.RoomName = ((object)(SteamId)(ref id)/*cast due to .constrained prefix*/).ToString();
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = ((Lobby)(ref SteamManager.instance.currentLobby)).GetData("Region");
				string data = ((Lobby)(ref SteamManager.instance.currentLobby)).GetData("BuildName");
				if (data != BuildManager.instance.version.title)
				{
					if (data != "")
					{
						Debug.Log((object)("Build name mismatch. Leaving lobby. Build name is ''" + data + "''"));
						string bodyText = "Game lobby is using version\n<color=#FDFF00><b>" + data + "</b>";
						MenuManager.instance.PagePopUpScheduled("Wrong Game Version", Color.red, bodyText, "Ok Dang");
					}
					else
					{
						Debug.Log((object)"Lobby closed. Leaving lobby.");
						MenuManager.instance.PagePopUpScheduled("Lobby Closed", Color.red, "The lobby has closed.", "Ok Dang");
					}
					PhotonNetwork.Disconnect();
					SteamManager.instance.LeaveLobby();
					GameManager.instance.SetGameMode(0);
					RunManager.instance.levelCurrent = RunManager.instance.levelMainMenu;
					SceneManager.LoadSceneAsync("Reload");
					yield break;
				}
				Debug.Log((object)("Already in lobby on Network Connect. Connecting to region: " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion));
			}
			else
			{
				Debug.Log((object)"Created lobby on Network Connect.");
				PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
				SteamManager.instance.HostLobby();
				while (true)
				{
					id = ((Lobby)(ref SteamManager.instance.currentLobby)).Id;
					if (((SteamId)(ref id)).IsValid)
					{
						break;
					}
					yield return null;
				}
				NetworkConnect networkConnect2 = this;
				id = ((Lobby)(ref SteamManager.instance.currentLobby)).Id;
				networkConnect2.RoomName = ((object)(SteamId)(ref id)/*cast due to .constrained prefix*/).ToString();
			}
			SteamManager.instance.SendSteamAuthTicket();
		}
		else
		{
			Debug.Log((object)"Local test mode.");
			RunManager.instance.ResetProgress();
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
			RoomName = SteamClient.Name;
		}
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Connected to Master Server");
		if (!GameManager.instance.localTest)
		{
			SteamId id = ((Lobby)(ref SteamManager.instance.currentLobby)).Id;
			if (((SteamId)(ref id)).IsValid && ((Lobby)(ref SteamManager.instance.currentLobby)).IsOwnedBy(SteamClient.SteamId))
			{
				Debug.Log((object)"I am the owner.");
				SteamManager.instance.SetLobbyData();
				TryJoiningRoom();
				return;
			}
		}
		Debug.Log((object)"I am not the owner.");
		TryJoiningRoom();
	}

	private void TryJoiningRoom()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		Debug.Log((object)("Trying to join room: " + RoomName));
		PhotonNetwork.JoinOrCreateRoom(RoomName, new RoomOptions
		{
			MaxPlayers = 6,
			IsVisible = false
		}, TypedLobby.Default, (string[])null);
	}

	public override void OnCreatedRoom()
	{
		Debug.Log((object)("Created room successfully: " + PhotonNetwork.CurrentRoom.Name));
	}

	public override void OnJoinedRoom()
	{
		Debug.Log((object)("Joined room: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CloudRegion));
		joinedRoom = true;
		PhotonNetwork.AutomaticallySyncScene = true;
		RunManager.instance.waitToChangeScene = false;
		if (GameManager.instance.localTest && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Reload");
		}
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogError((object)("Failed to create room: " + message));
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + message, "Ok Dang");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		((MonoBehaviour)this).StartCoroutine(RunManager.instance.LeaveToMainMenu());
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogError((object)("Failed to join room: " + message));
		MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + message, "Ok Dang");
		PhotonNetwork.Disconnect();
		SteamManager.instance.LeaveLobby();
		GameManager.instance.SetGameMode(0);
		((MonoBehaviour)this).StartCoroutine(RunManager.instance.LeaveToMainMenu());
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
		if ((int)cause != 17 && (int)cause != 9)
		{
			MenuManager.instance.PagePopUpScheduled("Disconnected", Color.red, "Cause: " + ((object)(DisconnectCause)(ref cause)/*cast due to .constrained prefix*/).ToString(), "Ok Dang");
			PhotonNetwork.Disconnect();
			SteamManager.instance.LeaveLobby();
			GameManager.instance.SetGameMode(0);
			((MonoBehaviour)this).StartCoroutine(RunManager.instance.LeaveToMainMenu());
		}
	}

	private void OnDestroy()
	{
		if (joinedRoom)
		{
			Debug.Log((object)"Game Mode: Multiplayer");
			GameManager.instance.SetGameMode(1);
		}
		Debug.Log((object)"NetworkConnect destroyed.");
		RunManager.instance.waitToChangeScene = false;
	}
}
