using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPageLobby : MonoBehaviour
{
	public static MenuPageLobby instance;

	internal MenuPage menuPage;

	private float listCheckTimer;

	internal List<PlayerAvatar> lobbyPlayers = new List<PlayerAvatar>();

	internal List<GameObject> listObjects = new List<GameObject>();

	internal List<MenuPlayerListed> menuPlayerListedList = new List<MenuPlayerListed>();

	public GameObject menuPlayerListedPrefab;

	public RectTransform playerListTransform;

	public TextMeshProUGUI roomNameText;

	public TextMeshProUGUI chatPromptText;

	public MenuButton startButton;

	public MenuButton inviteButton;

	public CanvasGroup joiningPlayersCanvasGroup;

	private List<string> joiningPlayers = new List<string>();

	private float joiningPlayersTimer;

	private float joiningPlayersEndTimer;

	private bool joiningPlayer;

	private void Awake()
	{
		instance = this;
		menuPage = ((Component)this).GetComponent<MenuPage>();
		((TMP_Text)roomNameText).text = PhotonNetwork.CloudRegion + " " + PhotonNetwork.CurrentRoom.Name;
		UpdateChatPrompt();
	}

	private void Start()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClient())
		{
			((Component)inviteButton).transform.localPosition = new Vector3(((Component)startButton).transform.localPosition.x + 40f, ((Component)startButton).transform.localPosition.y, ((Component)startButton).transform.localPosition.z);
			((TMP_Text)inviteButton.buttonText).alignment = (TextAlignmentOptions)516;
			((Component)startButton).gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (joiningPlayersTimer > 0f)
		{
			joiningPlayersTimer -= Time.deltaTime;
		}
		else if (joiningPlayers.Count > 0)
		{
			joiningPlayers.Clear();
		}
		if (joiningPlayers.Count > 0 || joiningPlayersEndTimer > 0f)
		{
			joiningPlayer = true;
			joiningPlayersCanvasGroup.alpha = Mathf.Lerp(joiningPlayersCanvasGroup.alpha, 1f, Time.deltaTime * 10f);
			startButton.disabled = true;
		}
		else
		{
			joiningPlayersCanvasGroup.alpha = Mathf.Lerp(joiningPlayersCanvasGroup.alpha, 0f, Time.deltaTime * 10f);
			joiningPlayer = false;
			startButton.disabled = false;
		}
		if (joiningPlayersEndTimer > 0f)
		{
			joiningPlayersEndTimer -= Time.deltaTime;
		}
		listCheckTimer -= Time.deltaTime;
		if (!(listCheckTimer <= 0f))
		{
			return;
		}
		listCheckTimer = 1f;
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		bool flag = false;
		foreach (PlayerAvatar item in list)
		{
			if (!lobbyPlayers.Contains(item) && item.playerAvatarVisuals.colorSet)
			{
				PlayerAdd(item);
				flag = true;
			}
		}
		foreach (PlayerAvatar item2 in lobbyPlayers.ToList())
		{
			if (!list.Contains(item2))
			{
				PlayerRemove(item2);
				flag = true;
			}
		}
		if (flag)
		{
			listObjects.Sort((GameObject a, GameObject b) => a.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID.CompareTo(b.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID));
			for (int i = 0; i < listObjects.Count; i++)
			{
				listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
				listObjects[i].transform.SetSiblingIndex(i);
			}
		}
		foreach (GameObject listObject in listObjects)
		{
			PlayerAvatar playerAvatar = listObject.GetComponent<MenuPlayerListed>().playerAvatar;
			if (Object.op_Implicit((Object)(object)playerAvatar))
			{
				if (playerAvatar.photonView.Owner == PhotonNetwork.MasterClient)
				{
					((TMP_Text)listObject.GetComponent<MenuPlayerListed>().playerName).text = playerAvatar.playerName + " <color=#331100>[HOST]</color>";
				}
				else
				{
					((TMP_Text)listObject.GetComponent<MenuPlayerListed>().playerName).text = playerAvatar.playerName;
				}
				SetPingText(listObject.GetComponent<MenuPlayerListed>().pingText, playerAvatar.playerPing);
			}
		}
	}

	private void PlayerAdd(PlayerAvatar player)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		lobbyPlayers.Add(player);
		GameObject val = Object.Instantiate<GameObject>(menuPlayerListedPrefab, ((Component)this).transform);
		MenuPlayerListed component = val.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		((Transform)((Component)component).GetComponent<RectTransform>()).SetParent((Transform)(object)playerListTransform);
		MenuSliderPlayerMicGain componentInChildren = ((Component)component).GetComponentInChildren<MenuSliderPlayerMicGain>();
		componentInChildren.playerAvatar = player;
		if (player.isLocal)
		{
			Object.Destroy((Object)(object)((Component)componentInChildren).gameObject);
		}
		((Component)component).transform.localPosition = Vector3.zero;
		listObjects.Add(val);
		menuPlayerListedList.Add(component);
		component.listSpot = Mathf.Max(listObjects.Count - 1, 0);
		foreach (string joiningPlayer in joiningPlayers)
		{
			if (player.playerName == joiningPlayer)
			{
				joiningPlayers.Remove(joiningPlayer);
				joiningPlayersEndTimer = 1f;
				break;
			}
		}
	}

	private void PlayerRemove(PlayerAvatar player)
	{
		lobbyPlayers.Remove(player);
		foreach (GameObject listObject in listObjects)
		{
			if ((Object)(object)listObject.GetComponent<MenuPlayerListed>().playerAvatar == (Object)(object)player)
			{
				listObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				listObjects.Remove(listObject);
				menuPlayerListedList.Remove(listObject.GetComponent<MenuPlayerListed>());
				break;
			}
		}
		for (int i = 0; i < listObjects.Count; i++)
		{
			listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}

	private void SetPingText(TextMeshProUGUI text, int ping)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (ping < 50)
		{
			((Graphic)text).color = new Color(0.2f, 0.8f, 0.2f);
		}
		else if (ping < 100)
		{
			((Graphic)text).color = new Color(0.8f, 0.8f, 0.2f);
		}
		else if (ping < 200)
		{
			((Graphic)text).color = new Color(0.8f, 0.4f, 0.2f);
		}
		else
		{
			((Graphic)text).color = new Color(0.8f, 0.2f, 0.2f);
		}
		((TMP_Text)text).text = ping + " ms";
	}

	public void JoiningPlayer(string playerName)
	{
		joiningPlayers.Add(playerName);
		joiningPlayersTimer = 10f;
	}

	public void ChangeColorButton()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Color);
	}

	public void UpdateChatPrompt()
	{
		((TMP_Text)chatPromptText).text = InputManager.instance.InputDisplayReplaceTags("Press [chat] to chat");
	}

	public void ButtonLeave()
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.OutroStartRPC();
		}
		NetworkManager.instance.leavePhotonRoom = true;
	}

	public void ButtonSettings()
	{
		MenuManager.instance.PageOpenOnTop(MenuPageIndex.Settings);
	}

	public void ButtonStart()
	{
		if (joiningPlayer)
		{
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny);
			return;
		}
		PhotonNetwork.CurrentRoom.IsOpen = false;
		SteamManager.instance.LockLobby();
		DataDirector.instance.RunsPlayedAdd();
		if (RunManager.instance.loadLevel == 0)
		{
			RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.RunLevel);
		}
		else
		{
			RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.Shop);
		}
	}

	public void ButtonInvite()
	{
		SteamManager.instance.OpenSteamOverlayToLobby();
	}
}
