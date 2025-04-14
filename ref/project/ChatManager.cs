using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
	public enum PossessChatID
	{
		None,
		LovePotion,
		Ouch,
		SelfDestruct,
		Betrayal,
		SelfDestructCancel
	}

	public enum ChatState
	{
		Inactive,
		Active,
		Possessed,
		Send
	}

	public class PossessMessage
	{
		public PossessChatID possessChatID;

		public string message;

		public float typingSpeed;

		public Color possessColor;

		public float messageDelay;

		public bool sendInTaxmanChat;

		public int sendInTaxmanChatEmojiInt;

		public UnityEvent eventExecutionAfterMessageIsDone;

		public PossessMessage(PossessChatID _possessChatID, string message, float typingSpeed, Color possessColor, float messageDelay, bool sendInTaxmanChat, int sendInTaxmanChatEmojiInt, UnityEvent eventExecutionAfterMessageIsDone)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			possessChatID = _possessChatID;
			this.message = message;
			this.typingSpeed = typingSpeed;
			this.possessColor = possessColor;
			this.messageDelay = messageDelay;
			this.sendInTaxmanChat = sendInTaxmanChat;
			this.sendInTaxmanChatEmojiInt = sendInTaxmanChatEmojiInt;
			this.eventExecutionAfterMessageIsDone = eventExecutionAfterMessageIsDone;
		}
	}

	public class PossessMessageBatch
	{
		public List<PossessMessage> messages = new List<PossessMessage>();

		public int messagePrio;

		public bool isProcessing;

		public PossessMessageBatch(int messagePrio)
		{
			this.messagePrio = messagePrio;
		}
	}

	public static ChatManager instance;

	internal bool chatActive;

	internal bool localPlayerAvatarFetched;

	internal bool textMeshFetched;

	internal PlayerAvatar playerAvatar;

	internal string prevChatMessage = "";

	internal string chatMessage = "";

	public TextMeshProUGUI chatText;

	private float spamTimer;

	private List<string> chatHistory = new List<string>();

	private int chatHistoryIndex;

	private float possessLetterDelay;

	private bool wasPossessed;

	private int wasPossessedPrio;

	private bool betrayalActive;

	internal PossessChatID activePossession;

	internal float activePossessionTimer;

	public PossessChatID currentPossessChatID;

	private List<PossessMessageBatch> possessBatchQueue = new List<PossessMessageBatch>();

	private PossessMessageBatch currentBatch;

	private int currentMessageIndex;

	private bool isScheduling;

	private PossessMessageBatch scheduledBatch;

	private float isSpeakingTimer;

	private ChatState chatState;

	private PossessMessage currentPossessMessage;

	private void Awake()
	{
		if ((Object)(object)instance == (Object)null)
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else if ((Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void SetChatColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)chatText).color = color;
	}

	public void ClearAllChatBatches()
	{
		possessBatchQueue.Clear();
		currentBatch = null;
	}

	public void ForceSendMessage(string _message)
	{
		chatMessage = _message;
		ForceConfirmChat();
	}

	private void CharRemoveEffect()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
		ChatUI.instance.SemiUISpringShakeX(5f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Dud, null, 2f, 1f, soundOnly: true);
	}

	public void AddLetterToChat(string letter)
	{
		prevChatMessage = chatMessage;
		chatMessage += letter;
		((TMP_Text)chatText).text = chatMessage;
	}

	public void ForceConfirmChat()
	{
		StateSet(ChatState.Send);
	}

	private void ChatReset()
	{
		chatMessage = "";
	}

	private void PossessChatLovePotion()
	{
		playerAvatar.OverridePupilSize(3f, 4, 1f, 1f, 15f, 0.3f);
		playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Love, 0.25f, 0);
	}

	private void PossessChatCustomLogic()
	{
		switch (activePossession)
		{
		case PossessChatID.LovePotion:
			PossessChatLovePotion();
			break;
		case PossessChatID.SelfDestruct:
			if (!Object.op_Implicit((Object)(object)playerAvatar))
			{
				return;
			}
			playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case PossessChatID.Betrayal:
			if (!Object.op_Implicit((Object)(object)playerAvatar))
			{
				return;
			}
			playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case PossessChatID.SelfDestructCancel:
			if (!Object.op_Implicit((Object)(object)playerAvatar))
			{
				return;
			}
			playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 0.25f, 0);
			break;
		}
		if (isSpeakingTimer > 0f)
		{
			isSpeakingTimer -= Time.deltaTime;
		}
		if (isSpeakingTimer < 0.2f && Object.op_Implicit((Object)(object)playerAvatar) && Object.op_Implicit((Object)(object)playerAvatar.voiceChat) && Object.op_Implicit((Object)(object)playerAvatar.voiceChat.ttsVoice) && playerAvatar.voiceChat.ttsVoice.isSpeaking)
		{
			isSpeakingTimer = 0.2f;
		}
		if (isSpeakingTimer <= 0f && possessBatchQueue.Count == 0 && currentBatch == null)
		{
			currentPossessChatID = PossessChatID.None;
		}
	}

	public void PossessChatScheduleStart(int messagePrio)
	{
		bool flag = false;
		if (currentBatch != null && messagePrio < currentBatch.messagePrio)
		{
			InterruptCurrentPossessBatch();
			ChatReset();
			flag = true;
		}
		if (currentBatch == null)
		{
			flag = true;
		}
		if (flag)
		{
			isScheduling = true;
			scheduledBatch = new PossessMessageBatch(messagePrio);
		}
	}

	public void PossessChatScheduleEnd()
	{
		if (isScheduling)
		{
			isScheduling = false;
			EnqueuePossessBatch(scheduledBatch);
			scheduledBatch = null;
		}
	}

	public void PossessChat(PossessChatID _possessChatID, string message, float typingSpeed, Color _possessColor, float _messageDelay = 0f, bool sendInTaxmanChat = false, int sendInTaxmanChatEmojiInt = 0, UnityEvent eventExecutionAfterMessageIsDone = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		isSpeakingTimer = 1f;
		PossessMessage item = new PossessMessage(_possessChatID, message, typingSpeed, _possessColor, _messageDelay, sendInTaxmanChat, sendInTaxmanChatEmojiInt, eventExecutionAfterMessageIsDone);
		if (isScheduling)
		{
			scheduledBatch.messages.Add(item);
		}
	}

	private void EnqueuePossessBatch(PossessMessageBatch batch)
	{
		if (currentBatch == null)
		{
			StartPossessBatch(batch);
		}
		else if (batch.messagePrio < currentBatch.messagePrio)
		{
			InterruptCurrentPossessBatch();
			StartPossessBatch(batch);
		}
		else if (batch.messagePrio <= currentBatch.messagePrio)
		{
			possessBatchQueue.Add(batch);
		}
	}

	private void StartPossessBatch(PossessMessageBatch batch)
	{
		currentBatch = batch;
		currentBatch.isProcessing = true;
		currentMessageIndex = 0;
		StartPossessMessage(currentBatch.messages[currentMessageIndex]);
	}

	private void InterruptCurrentPossessBatch()
	{
		ChatReset();
		currentBatch = null;
		possessBatchQueue.Clear();
		wasPossessed = false;
		wasPossessedPrio = 0;
	}

	private void StartPossessMessage(PossessMessage message)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ChatReset();
		possessLetterDelay = 0f;
		SetChatColor(message.possessColor);
		currentPossessMessage = message;
		StateSet(ChatState.Possessed);
		currentPossessChatID = message.possessChatID;
	}

	private void PossessionReset()
	{
		currentPossessChatID = PossessChatID.None;
		currentBatch = null;
		possessBatchQueue.Clear();
		wasPossessed = false;
		wasPossessedPrio = 0;
		ChatReset();
		StateSet(ChatState.Inactive);
	}

	private void TypeEffect(Color _color)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ChatUI.instance.SemiUITextFlashColor(_color, 0.2f);
		ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 2f, 0.2f, soundOnly: true);
	}

	public void TumbleInterruption()
	{
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (!(activePossessionTimer > 0f))
		{
			PossessionReset();
			if (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.voiceChatFetched && playerAvatar.voiceChat.ttsVoice.isSpeaking)
			{
				List<string> list = new List<string>
				{
					"Ouch! Ouch! Ouch!", "Ow! Ow! Ow!", "Oof! Oof! Oof!", "Owie! Wowie! Zowie!", "Ouchie! Ouchie! Ouchie!", "error error error", "system error", "fatal error", "error 404", "runtime error",
					"imma falling", "falling over", "ooooooooh!", "oh nooooo!", "AAAAAAH! AAH!", "AAAAAAAAAAAAAAH!", "AAAAAAAAAAAAAAAAAAAAAAAAAAAH!", "OH! OH! OH!", "AH! AH! AH!"
				};
				int index = Random.Range(0, list.Count);
				string message = list[index];
				PossessChatScheduleStart(3);
				PossessChat(PossessChatID.Ouch, message, 1f, Color.red);
				PossessChatScheduleEnd();
			}
		}
	}

	private void StateInactive()
	{
		ChatUI.instance.Hide();
		chatMessage = "";
		chatActive = false;
		if ((!Object.op_Implicit((Object)(object)MenuManager.instance.currentMenuPage) || (MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Escape && MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Settings)) && SemiFunc.InputDown(InputKey.Chat))
		{
			TutorialDirector.instance.playerChatted = true;
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, null, 1f, 1f, soundOnly: true);
			chatActive = !chatActive;
			StateSet(ChatState.Active);
			chatHistoryIndex = 0;
		}
	}

	private void StateActive()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.InputDown(InputKey.Back))
		{
			StateSet(ChatState.Inactive);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, soundOnly: true);
			return;
		}
		if (Input.GetKeyDown((KeyCode)273) && chatHistory.Count > 0)
		{
			if (chatHistoryIndex > 0)
			{
				chatHistoryIndex--;
			}
			else
			{
				chatHistoryIndex = chatHistory.Count - 1;
			}
			chatMessage = chatHistory[chatHistoryIndex];
			((TMP_Text)chatText).text = chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, soundOnly: true);
		}
		if (Input.GetKeyDown((KeyCode)274) && chatHistory.Count > 0)
		{
			if (chatHistoryIndex < chatHistory.Count - 1)
			{
				chatHistoryIndex++;
			}
			else
			{
				chatHistoryIndex = 0;
			}
			chatMessage = chatHistory[chatHistoryIndex];
			((TMP_Text)chatText).text = chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, soundOnly: true);
		}
		SemiFunc.InputDisableMovement();
		if (SemiFunc.InputDown(InputKey.ChatDelete))
		{
			if (chatMessage.Length > 0)
			{
				chatMessage = chatMessage.Remove(chatMessage.Length - 1);
				((TMP_Text)chatText).text = chatMessage;
				CharRemoveEffect();
			}
		}
		else
		{
			if (chatMessage == "\b")
			{
				chatMessage = "";
			}
			prevChatMessage = chatMessage;
			string text = chatMessage;
			chatMessage += Input.inputString;
			chatMessage = chatMessage.Replace("\n", "");
			if (chatMessage.Length > 50)
			{
				ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
				ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
				ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, soundOnly: true);
				chatMessage = text;
			}
			if (prevChatMessage != chatMessage)
			{
				bool flag = false;
				if (Input.inputString == "\b")
				{
					chatMessage = chatMessage.Remove(Mathf.Max(chatMessage.Length - 2, 0));
					flag = true;
				}
				else
				{
					((TMP_Text)chatText).text = chatMessage;
				}
				chatMessage = chatMessage.Replace("\r", "");
				prevChatMessage = chatMessage;
				if (!flag)
				{
					TypeEffect(Color.yellow);
				}
				else
				{
					CharRemoveEffect();
				}
			}
		}
		if (SemiFunc.InputDown(InputKey.Confirm))
		{
			if (chatMessage != "")
			{
				StateSet(ChatState.Send);
			}
			else
			{
				StateSet(ChatState.Inactive);
			}
		}
		if (Mathf.Sin(Time.time * 10f) > 0f)
		{
			((TMP_Text)chatText).text = chatMessage + "<b>|</b>";
		}
		else
		{
			((TMP_Text)chatText).text = chatMessage;
		}
		if (SemiFunc.InputDown(InputKey.Back))
		{
			StateSet(ChatState.Inactive);
		}
	}

	private void StatePossessed()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		chatActive = true;
		spamTimer = 0f;
		if (currentPossessMessage != null)
		{
			SetChatColor(currentPossessMessage.possessColor);
		}
		if (currentPossessMessage == null)
		{
			currentMessageIndex++;
			if (currentBatch != null && currentMessageIndex < currentBatch.messages.Count)
			{
				StartPossessMessage(currentBatch.messages[currentMessageIndex]);
				return;
			}
			if (currentBatch != null && currentBatch.messages.Count == currentMessageIndex && currentBatch.isProcessing)
			{
				currentBatch.isProcessing = false;
				currentBatch = null;
			}
			if (possessBatchQueue.Count > 0)
			{
				StartPossessBatch(possessBatchQueue[0]);
				possessBatchQueue.RemoveAt(0);
			}
			else
			{
				StateSet(ChatState.Inactive);
				currentBatch = null;
			}
			return;
		}
		bool flag = false;
		if (currentPossessMessage.typingSpeed == -1f)
		{
			flag = true;
		}
		if (possessLetterDelay <= 0f)
		{
			if (currentPossessMessage.message.Length > 0 && !flag)
			{
				string letter = currentPossessMessage.message[0].ToString();
				currentPossessMessage.message = currentPossessMessage.message.Substring(1);
				possessLetterDelay = Random.Range(0.005f, 0.05f);
				TypeEffect(currentPossessMessage.possessColor);
				AddLetterToChat(letter);
			}
			else
			{
				if (isSpeakingTimer > 0f && wasPossessed && wasPossessedPrio <= currentBatch.messagePrio)
				{
					return;
				}
				if (currentPossessMessage.messageDelay > 0f)
				{
					currentPossessMessage.messageDelay -= Time.deltaTime;
					return;
				}
				if (flag)
				{
					chatMessage = currentPossessMessage.message;
				}
				wasPossessed = true;
				if (currentBatch != null)
				{
					wasPossessedPrio = currentBatch.messagePrio;
				}
				StateSet(ChatState.Send);
			}
		}
		else
		{
			possessLetterDelay -= Time.deltaTime * currentPossessMessage.typingSpeed;
			if (currentPossessMessage.typingSpeed == -1f)
			{
				possessLetterDelay = 0f;
			}
		}
	}

	private void SelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		((MonoBehaviour)this).StartCoroutine(SelfDestructCoroutine(delay));
	}

	private void BetrayalSelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		((MonoBehaviour)this).StartCoroutine(SelfDestructCoroutine(delay));
	}

	public void PossessLeftBehind()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerAvatar) && !playerAvatar.isDisabled && !playerAvatar.RoomVolumeCheck.inTruck)
		{
			betrayalActive = true;
			PossessChatScheduleStart(2);
			string message = SemiFunc.MessageGeneratedGetLeftBehind();
			PossessChat(PossessChatID.Betrayal, message, 0.5f, Color.red, 0f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "I need to get to the truck in...", 0.4f, Color.red, 0f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "10...", 0.25f, Color.red, 0f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "9...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "8...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "7...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "6...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "5...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "4...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "3...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "2...", 0.25f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			PossessChat(PossessChatID.Betrayal, "1...", 0.5f, Color.red, 0.3f, sendInTaxmanChat: true, 2);
			UnityEvent val = new UnityEvent();
			val.AddListener(new UnityAction(BetrayalSelfDestruct));
			List<string> list = new List<string> { "betrayal", "i'm sorry", "I failed", "teamwork makes the dream work", "I thought we were friends", "I thought we were a team", "I thought we were in this together" };
			string message2 = list[Random.Range(0, list.Count)];
			PossessChat(PossessChatID.SelfDestruct, message2, 2f, Color.red, 0f, sendInTaxmanChat: true, 2, val);
			PossessChatScheduleEnd();
		}
	}

	public void PossessCancelSelfDestruction()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerAvatar) && !playerAvatar.isDisabled)
		{
			PossessChatScheduleEnd();
			possessBatchQueue.Clear();
			currentBatch = null;
			betrayalActive = false;
			PossessChatScheduleStart(1);
			PossessChat(PossessChatID.SelfDestructCancel, "SELF DESTRUCT SEQUENCE CANCELLED!", 2f, Color.green);
			PossessChatScheduleEnd();
		}
	}

	public void PossessSelfDestruction()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerAvatar) && !playerAvatar.isDisabled)
		{
			PossessChatScheduleStart(-1);
			UnityEvent val = new UnityEvent();
			val.AddListener(new UnityAction(SelfDestruct));
			List<string> list = new List<string>
			{
				"i'm out", "Farewell", "Adieu", "sayonara", "Auf Wiedersehen", "adios", "ciao", "Au Revoir", "hasta la vista", "see You Later",
				"later", "peace OUT", "catch you later", "later gator", "toodles", "bye bye", "bye", "AAAAAAAAAAAAH!", "AAAAAAAAAAAAAAAAAAAAAAAH!", "bye... ... oh?",
				"this will hurt", "it's over for me", "why me?", "I'm sorry", "i see the light", "sad but necessary", "HEJ DÅ!"
			};
			string message = list[Random.Range(0, list.Count)];
			PossessChat(PossessChatID.SelfDestruct, message, 2f, Color.red, 0f, sendInTaxmanChat: true, 2, val);
			PossessChatScheduleEnd();
		}
	}

	private IEnumerator BetrayalSelfDestructCoroutine(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		if (betrayalActive)
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, savingGrace: false);
		}
	}

	private IEnumerator SelfDestructCoroutine(float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		PlayerAvatar.instance.playerHealth.health = 0;
		PlayerAvatar.instance.playerHealth.Hurt(1, savingGrace: false);
	}

	public bool IsPossessed(PossessChatID _possessChatID)
	{
		return activePossession == _possessChatID;
	}

	private void StateSend()
	{
		bool possessed = false;
		if (currentPossessMessage != null && currentPossessMessage.sendInTaxmanChat && Object.op_Implicit((Object)(object)TruckScreenText.instance))
		{
			TruckScreenText.instance.MessageSendCustom(PlayerController.instance.playerSteamID, chatMessage, currentPossessMessage.sendInTaxmanChatEmojiInt);
		}
		if (currentPossessMessage != null)
		{
			possessed = true;
		}
		MessageSend(possessed);
		if (currentPossessMessage != null && currentPossessMessage.eventExecutionAfterMessageIsDone != null)
		{
			currentPossessMessage.eventExecutionAfterMessageIsDone.Invoke();
		}
		currentPossessMessage = null;
		StateSet(ChatState.Possessed);
	}

	private void StateSet(ChatState state)
	{
		chatState = state;
	}

	private void ImportantFetches()
	{
		if (!Object.op_Implicit((Object)(object)chatText))
		{
			textMeshFetched = false;
		}
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			localPlayerAvatarFetched = false;
		}
		if (!textMeshFetched && Object.op_Implicit((Object)(object)ChatUI.instance) && Object.op_Implicit((Object)(object)ChatUI.instance.chatText))
		{
			chatText = ChatUI.instance.chatText;
			textMeshFetched = true;
		}
		if (localPlayerAvatarFetched)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			List<PlayerAvatar> list = SemiFunc.PlayerGetList();
			if (list.Count <= 0)
			{
				return;
			}
			{
				foreach (PlayerAvatar item in list)
				{
					if (item.isLocal)
					{
						playerAvatar = item;
						localPlayerAvatarFetched = true;
						break;
					}
				}
				return;
			}
		}
		playerAvatar = PlayerAvatar.instance;
		localPlayerAvatarFetched = true;
	}

	private void NewLevelResets()
	{
		betrayalActive = false;
		localPlayerAvatarFetched = false;
		textMeshFetched = false;
		PossessionReset();
	}

	private void PossessionActive()
	{
		if (activePossessionTimer <= 0f)
		{
			activePossession = PossessChatID.None;
		}
		if (activePossessionTimer > 0f)
		{
			activePossessionTimer -= Time.deltaTime;
		}
		if (currentPossessChatID != 0 || (activePossession != 0 && isSpeakingTimer > 0f))
		{
			activePossessionTimer = 0.5f;
			activePossession = currentPossessChatID;
		}
	}

	private void Update()
	{
		PossessionActive();
		if (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.isDisabled && (possessBatchQueue.Count > 0 || currentBatch != null))
		{
			InterruptCurrentPossessBatch();
		}
		if (!SemiFunc.IsMultiplayer())
		{
			ChatUI.instance.Hide();
			return;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			NewLevelResets();
			return;
		}
		ImportantFetches();
		PossessChatCustomLogic();
		if (!textMeshFetched || !localPlayerAvatarFetched)
		{
			return;
		}
		switch (chatState)
		{
		case ChatState.Inactive:
			StateInactive();
			break;
		case ChatState.Active:
			StateActive();
			break;
		case ChatState.Possessed:
			StatePossessed();
			break;
		case ChatState.Send:
			StateSend();
			break;
		}
		PossessChatCustomLogic();
		if (!SemiFunc.IsMultiplayer())
		{
			if (chatState != 0)
			{
				StateSet(ChatState.Inactive);
			}
			chatActive = false;
			return;
		}
		if (spamTimer > 0f)
		{
			spamTimer -= Time.deltaTime;
		}
		if (SemiFunc.FPSImpulse15() && betrayalActive && PlayerController.instance.playerAvatarScript.RoomVolumeCheck.inTruck)
		{
			PossessCancelSelfDestruction();
		}
	}

	public bool StateIsActive()
	{
		return chatState == ChatState.Active;
	}

	public bool StateIsPossessed()
	{
		return chatState == ChatState.Possessed;
	}

	public bool StateIsSend()
	{
		return chatState == ChatState.Send;
	}

	public bool StateIsInactive()
	{
		return chatState == ChatState.Inactive;
	}

	private void MessageSend(bool _possessed = false)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!(chatMessage == "") && spamTimer <= 0f)
		{
			playerAvatar.ChatMessageSend(chatMessage, _debugMessage: false);
			if (!_possessed)
			{
				chatHistory.Add(chatMessage);
			}
			if (chatHistory.Count > 20)
			{
				chatHistory.RemoveAt(0);
			}
			chatHistory = chatHistory.AsEnumerable().Reverse().Distinct()
				.Reverse()
				.ToList();
			ChatReset();
			((TMP_Text)chatText).text = chatMessage;
			chatActive = false;
			isSpeakingTimer = 0.2f;
			ChatUI.instance.SemiUITextFlashColor(Color.green, 0.2f);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, soundOnly: true);
			spamTimer = 1f;
		}
	}
}
