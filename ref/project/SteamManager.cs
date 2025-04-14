using System;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
	[Serializable]
	public class Developer
	{
		public string name;

		public string steamID;
	}

	public static SteamManager instance;

	internal Lobby currentLobby;

	internal Lobby noLobby;

	internal bool joinLobby;

	public GameObject networkConnectPrefab;

	internal AuthTicket steamAuthTicket;

	[Space]
	public List<Developer> developerList;

	internal bool developerMode;

	private void Awake()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			try
			{
				SteamClient.Init(3241660u, true);
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Steamworks failed to initialize. Error: " + ex.Message));
			}
			SteamId steamId = SteamClient.SteamId;
			Debug.Log((object)("STEAM ID: " + ((object)(SteamId)(ref steamId)/*cast due to .constrained prefix*/).ToString()));
			if (!Debug.isDebugBuild)
			{
				return;
			}
			{
				foreach (Developer developer in developerList)
				{
					steamId = SteamClient.SteamId;
					if (((object)(SteamId)(ref steamId)/*cast due to .constrained prefix*/).ToString() == developer.steamID)
					{
						Debug.Log((object)("DEVELOPER MODE: " + developer.name.ToUpper()));
						developerMode = true;
					}
				}
				return;
			}
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void OnEnable()
	{
		SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
		SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
		SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
		SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeft;
		SteamMatchmaking.OnLobbyMemberDataChanged += OnLobbyMemberDataChanged;
		SteamFriends.OnGameOverlayActivated += OnGameOverlayActivated;
	}

	private void Start()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		GetSteamAuthTicket(out steamAuthTicket);
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length < 2)
		{
			return;
		}
		for (int i = 0; i < commandLineArgs.Length - 1; i++)
		{
			if (commandLineArgs[i].ToLower() == "+connect_lobby")
			{
				if (ulong.TryParse(commandLineArgs[i + 1], out var result) && result != 0)
				{
					Debug.Log((object)("Auto-Connecting to lobby: " + result));
					OnGameLobbyJoinRequested(new Lobby(SteamId.op_Implicit(result)), SteamClient.SteamId);
				}
				break;
			}
		}
	}

	private void OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
	{
		Debug.Log((object)("Steam: Lobby member joined: " + ((Friend)(ref _friend)).Name));
		MenuPageLobby.instance.JoiningPlayer(((Friend)(ref _friend)).Name);
	}

	private void OnLobbyMemberLeft(Lobby _lobby, Friend _friend)
	{
		Debug.Log((object)("Steam: Lobby member left: " + ((Friend)(ref _friend)).Name));
	}

	private void OnLobbyMemberDataChanged(Lobby _lobby, Friend _friend)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)" ");
		Debug.Log((object)("Steam: Lobby member data changed for: " + ((Friend)(ref _friend)).Name));
		Debug.Log((object)("I am " + SteamClient.Name));
		Friend owner = ((Lobby)(ref _lobby)).Owner;
		Debug.Log((object)("Current Owner: " + ((Friend)(ref owner)).Name));
		if (PhotonNetwork.IsMasterClient && RunManager.instance.masterSwitched && SteamId.op_Implicit(SteamClient.SteamId) == SteamId.op_Implicit(((Lobby)(ref _lobby)).Owner.Id))
		{
			Debug.Log((object)"I am the new owner and i am locking the lobby.");
			LockLobby();
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)instance == (Object)(object)this)
		{
			CancelSteamAuthTicket();
			SteamClient.Shutdown();
		}
	}

	private async void OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamID)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (SteamId.op_Implicit(((Lobby)(ref _lobby)).Id) == SteamId.op_Implicit(((Lobby)(ref currentLobby)).Id))
		{
			Debug.Log((object)"Steam: Already in this lobby.");
			return;
		}
		SteamId id = ((Lobby)(ref _lobby)).Id;
		Debug.Log((object)("Steam: Game lobby join requested: " + ((object)(SteamId)(ref id)/*cast due to .constrained prefix*/).ToString()));
		await SteamMatchmaking.JoinLobbyAsync(((Lobby)(ref _lobby)).Id);
		if ((Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelMainMenu)
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				player.OutroStartRPC();
			}
			RunManager.instance.lobbyJoin = true;
			RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.LobbyMenu);
		}
		joinLobby = true;
	}

	private void OnLobbyEntered(Lobby _lobby)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((Lobby)(ref currentLobby)).Leave();
		currentLobby = _lobby;
		SteamId id = ((Lobby)(ref _lobby)).Id;
		Debug.Log((object)("Steam: Lobby entered with ID: " + ((object)(SteamId)(ref id)/*cast due to .constrained prefix*/).ToString()));
		Debug.Log((object)("Steam: Region: " + ((Lobby)(ref _lobby)).GetData("Region")));
	}

	private void OnLobbyCreated(Result _result, Lobby _lobby)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)_result == 1)
		{
			SteamId id = ((Lobby)(ref _lobby)).Id;
			Debug.Log((object)("Steam: Lobby created with ID: " + ((object)(SteamId)(ref id)/*cast due to .constrained prefix*/).ToString()));
		}
		else
		{
			Debug.LogError((object)("Steam: Failed to create lobby. Error: " + ((object)(Result)(ref _result)/*cast due to .constrained prefix*/).ToString()));
			NetworkManager.instance.LeavePhotonRoom();
		}
	}

	public async void HostLobby()
	{
		Debug.Log((object)"Steam: Hosting lobby...");
		Lobby? val = await SteamMatchmaking.CreateLobbyAsync(6);
		if (!val.HasValue)
		{
			Debug.LogError((object)"Lobby created but not correctly instantiated.");
			return;
		}
		Lobby value = val.Value;
		((Lobby)(ref value)).SetPrivate();
		value = val.Value;
		((Lobby)(ref value)).SetFriendsOnly();
		value = val.Value;
		((Lobby)(ref value)).SetJoinable(false);
	}

	public void LeaveLobby()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (((Lobby)(ref currentLobby)).IsOwnedBy(SteamClient.SteamId))
		{
			Debug.Log((object)"Steam: Leaving lobby... and ruining it for others.");
			((Lobby)(ref currentLobby)).SetData("BuildName", "");
		}
		else
		{
			Debug.Log((object)"Steam: Leaving lobby...");
		}
		CancelSteamAuthTicket();
		((Lobby)(ref currentLobby)).Leave();
		currentLobby = noLobby;
	}

	public void UnlockLobby()
	{
		Debug.Log((object)"Steam: Unlocking lobby...");
		((Lobby)(ref currentLobby)).SetPrivate();
		((Lobby)(ref currentLobby)).SetFriendsOnly();
		((Lobby)(ref currentLobby)).SetJoinable(true);
	}

	public void LockLobby()
	{
		Debug.Log((object)"Steam: Locking lobby...");
		((Lobby)(ref currentLobby)).SetPrivate();
		((Lobby)(ref currentLobby)).SetFriendsOnly();
		((Lobby)(ref currentLobby)).SetJoinable(false);
	}

	public void SetLobbyData()
	{
		Debug.Log((object)"Steam: Setting lobby data...");
		((Lobby)(ref currentLobby)).SetData("Region", PhotonNetwork.CloudRegion);
		((Lobby)(ref currentLobby)).SetData("BuildName", BuildManager.instance.version.title);
	}

	public void SendSteamAuthTicket()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Sending Steam Auth Ticket...");
		string text = GetSteamAuthTicket(out steamAuthTicket);
		PhotonNetwork.AuthValues = new AuthenticationValues();
		AuthenticationValues authValues = PhotonNetwork.AuthValues;
		SteamId steamId = SteamClient.SteamId;
		authValues.UserId = ((object)(SteamId)(ref steamId)/*cast due to .constrained prefix*/).ToString();
		PhotonNetwork.AuthValues.AuthType = (CustomAuthenticationType)1;
		PhotonNetwork.AuthValues.AddAuthParameter("ticket", text);
	}

	private string GetSteamAuthTicket(out AuthTicket ticket)
	{
		Debug.Log((object)"Getting Steam Auth Ticket...");
		ticket = SteamUser.GetAuthSessionTicket();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ticket.Data.Length; i++)
		{
			stringBuilder.AppendFormat("{0:x2}", ticket.Data[i]);
		}
		return stringBuilder.ToString();
	}

	public void CancelSteamAuthTicket()
	{
		Debug.Log((object)"Cancelling Steam Auth Ticket...");
		if (steamAuthTicket != null)
		{
			steamAuthTicket.Cancel();
		}
	}

	public void OpenSteamOverlayToLobby()
	{
		SteamFriends.OpenOverlay("friends");
	}

	private void OnGameOverlayActivated(bool obj)
	{
		InputManager.instance.ResetInput();
	}
}
