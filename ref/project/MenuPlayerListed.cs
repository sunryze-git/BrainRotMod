using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayerListed : MonoBehaviour
{
	internal PlayerAvatar playerAvatar;

	internal int listSpot;

	public TextMeshProUGUI playerName;

	public MenuPlayerHead playerHead;

	private RectTransform parentTransform;

	private Vector3 midScreenFocus;

	public TextMeshProUGUI pingText;

	private bool localFetch;

	internal bool isLocal;

	public bool isSpectate = true;

	public GameObject leftCrown;

	public GameObject rightCrown;

	private bool fetchCrown;

	public bool forceCrown;

	private bool crownSetterWasHere;

	private void Start()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		parentTransform = ((Component)((Component)this).transform.parent).GetComponent<RectTransform>();
		((Transform)playerHead.focusPoint).SetParent((Transform)(object)parentTransform);
		((Transform)playerHead.myFocusPoint).SetParent((Transform)(object)parentTransform);
		midScreenFocus = new Vector3((float)(MenuManager.instance.screenUIWidth / 2), (float)(MenuManager.instance.screenUIHeight / 2), 0f) - ((Transform)parentTransform).localPosition - ((Transform)((Component)((Transform)parentTransform).parent).GetComponent<RectTransform>()).localPosition;
		if (forceCrown)
		{
			leftCrown.SetActive(true);
			rightCrown.SetActive(true);
			ForcePlayer(Arena.instance.winnerPlayer);
			TextMeshProUGUI componentInChildren = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
			if (Object.op_Implicit((Object)(object)componentInChildren) && Object.op_Implicit((Object)(object)playerAvatar))
			{
				((TMP_Text)componentInChildren).text = playerAvatar.playerName;
			}
		}
	}

	public void ForcePlayer(PlayerAvatar _playerAvatar)
	{
		playerHead.SetPlayer(_playerAvatar);
		playerAvatar = _playerAvatar;
		localFetch = false;
	}

	private void Update()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.FPSImpulse5() && !crownSetterWasHere && Object.op_Implicit((Object)(object)PlayerCrownSet.instance) && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (Object.op_Implicit((Object)(object)playerAvatar) && PlayerCrownSet.instance.crownOwnerSteamID == playerAvatar.steamID)
			{
				leftCrown.SetActive(true);
				rightCrown.SetActive(true);
			}
			crownSetterWasHere = true;
		}
		if (!localFetch)
		{
			if (Object.op_Implicit((Object)(object)playerAvatar))
			{
				isLocal = playerAvatar.isLocal;
			}
			localFetch = true;
		}
		if (!forceCrown && ((Transform)playerHead.myFocusPoint).localPosition != midScreenFocus)
		{
			((Transform)playerHead.myFocusPoint).localPosition = midScreenFocus;
		}
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			if (!fetchCrown)
			{
				if ((Object)(object)SessionManager.instance.CrownedPlayerGet() == (Object)(object)playerAvatar)
				{
					leftCrown.SetActive(true);
					rightCrown.SetActive(true);
				}
				fetchCrown = true;
			}
			if (isSpectate && ((TMP_Text)playerName).text != playerAvatar.playerName)
			{
				((TMP_Text)playerName).text = playerAvatar.playerName;
			}
			if (playerAvatar.voiceChatFetched && playerAvatar.voiceChat.isTalking)
			{
				Color val = default(Color);
				((Color)(ref val))._002Ector(0.6f, 0.6f, 0.4f);
				((Graphic)playerName).color = Color.Lerp(((Graphic)playerName).color, val, Time.deltaTime * 10f);
			}
			else
			{
				Color val2 = default(Color);
				((Color)(ref val2))._002Ector(0.2f, 0.2f, 0.2f);
				((Graphic)playerName).color = Color.Lerp(((Graphic)playerName).color, val2, Time.deltaTime * 10f);
			}
		}
		if (!forceCrown)
		{
			if ((Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelLobbyMenu)
			{
				((Component)this).transform.localPosition = new Vector3(-23f, (float)(-listSpot * 22), 0f);
			}
			else
			{
				((Component)this).transform.localPosition = new Vector3(0f, (float)(-listSpot * 32), 0f);
			}
		}
	}

	public void MenuPlayerListedOutro()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
