using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayerHead : MonoBehaviour
{
	internal PlayerAvatar playerAvatar;

	public Transform headRight;

	public Transform headLeft;

	private RectTransform eyesTransform;

	private MenuPlayerListed playerListed;

	private int listSpotPrev = -1;

	private int listSpot;

	private bool left;

	private bool right = true;

	private List<RawImage> allRawImagesInChildren = new List<RawImage>();

	private Vector3 eyesStartPosOriginal;

	private Vector3 eyesStartPos;

	private bool isTalkingPrev;

	internal bool isTalking;

	public RectTransform focusPoint;

	public RectTransform myFocusPoint;

	private RectTransform playerListedTransform;

	private RectTransform rectTransform;

	internal RectTransform headTransform;

	internal float startedTalkingAtTime;

	public bool isWinnerHead;

	private void Start()
	{
		playerListed = ((Component)this).GetComponentInParent<MenuPlayerListed>();
		allRawImagesInChildren.AddRange(((Component)this).GetComponentsInChildren<RawImage>());
		playerListedTransform = ((Component)playerListed).GetComponent<RectTransform>();
		MenuManager.instance.PlayerHeadAdd(this);
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		startedTalkingAtTime = Time.time;
	}

	public void SetColor(Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		foreach (RawImage allRawImagesInChild in allRawImagesInChildren)
		{
			((Graphic)allRawImagesInChild).color = color;
		}
	}

	private void HeadRight()
	{
		((Component)headRight).gameObject.SetActive(true);
		((Component)headLeft).gameObject.SetActive(false);
		eyesTransform = ((Component)headRight.Find("Eyes")).GetComponent<RectTransform>();
		left = false;
		right = true;
		headTransform = ((Component)headRight).GetComponent<RectTransform>();
	}

	private void HeadLeft()
	{
		((Component)headRight).gameObject.SetActive(false);
		((Component)headLeft).gameObject.SetActive(true);
		eyesTransform = ((Component)headLeft.Find("Eyes")).GetComponent<RectTransform>();
		left = true;
		right = false;
		headTransform = ((Component)headLeft).GetComponent<RectTransform>();
	}

	private void Update()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer())
		{
			if (Object.op_Implicit((Object)(object)playerAvatar))
			{
				isTalkingPrev = isTalking;
				if (playerAvatar.voiceChatFetched)
				{
					isTalking = playerAvatar.voiceChat.isTalking;
				}
			}
			else
			{
				playerAvatar = playerListed.playerAvatar;
			}
		}
		if (MenuManager.instance.currentMenuPageIndex == MenuPageIndex.Lobby)
		{
			((Transform)myFocusPoint).localPosition = ((Component)MenuCursor.instance).transform.localPosition - ((Transform)rectTransform).parent.parent.localPosition;
			((Transform)myFocusPoint).localPosition = new Vector3(((Transform)myFocusPoint).localPosition.x + 18f, ((Transform)myFocusPoint).localPosition.y + 15f, 0f);
		}
		if (!isWinnerHead && Object.op_Implicit((Object)(object)headTransform))
		{
			((Transform)focusPoint).localPosition = ((Transform)playerListedTransform).localPosition + ((Transform)rectTransform).localPosition + ((Transform)headTransform).localPosition * ((Transform)rectTransform).localScale.x;
			float length = 12.5f;
			if (left)
			{
				length = -12.5f;
			}
			RectTransform obj = focusPoint;
			((Transform)obj).localPosition = ((Transform)obj).localPosition + new Vector3(LengthDirX(length, ((Transform)headTransform).localEulerAngles.z), LengthDirY(length, ((Transform)headTransform).localEulerAngles.z), 0f);
			length = 6f;
			RectTransform obj2 = focusPoint;
			((Transform)obj2).localPosition = ((Transform)obj2).localPosition + new Vector3(LengthDirX(length, ((Transform)headTransform).localEulerAngles.z + 90f), LengthDirY(length, ((Transform)headTransform).localEulerAngles.z + 90f), 0f);
		}
		if (isTalking != isTalkingPrev)
		{
			if (isTalking)
			{
				startedTalkingAtTime = Time.time;
			}
			isTalkingPrev = isTalking;
		}
		if (!isWinnerHead)
		{
			listSpot = playerListed.listSpot;
			if (listSpot != listSpotPrev)
			{
				if (listSpot % 2 == 0)
				{
					HeadRight();
				}
				else
				{
					HeadLeft();
				}
				listSpotPrev = listSpot;
			}
		}
		if (!isWinnerHead)
		{
			MenuPlayerHead menuPlayerHead = null;
			float num = 0f;
			foreach (MenuPlayerHead playerHead in MenuManager.instance.playerHeads)
			{
				if (!((Object)(object)playerHead == (Object)(object)this) && playerHead.isTalking)
				{
					float num2 = playerHead.startedTalkingAtTime;
					if (num2 > num)
					{
						num = num2;
						menuPlayerHead = playerHead;
					}
				}
			}
			if (Object.op_Implicit((Object)(object)menuPlayerHead))
			{
				float num3 = 10f;
				Vector3 val = ((Transform)menuPlayerHead.focusPoint).localPosition - ((Transform)focusPoint).localPosition;
				val.z = 0f;
				Vector3 val2 = new Vector3(50f, 25f, 0f) + ((Vector3)(ref val)).normalized * num3;
				if (left)
				{
					val2 = new Vector3(-50f, 25f, 0f) + ((Vector3)(ref val)).normalized * num3;
				}
				((Transform)eyesTransform).localPosition = Vector3.Lerp(((Transform)eyesTransform).localPosition, val2, Time.deltaTime * 10f);
			}
			else
			{
				float num4 = 10f;
				Vector3 val3 = ((Transform)myFocusPoint).localPosition - ((Transform)focusPoint).localPosition;
				val3.z = 0f;
				Vector3 val4 = new Vector3(50f, 25f, 0f) + ((Vector3)(ref val3)).normalized * num4;
				if (left)
				{
					val4 = new Vector3(-50f, 25f, 0f) + ((Vector3)(ref val3)).normalized * num4;
				}
				((Transform)eyesTransform).localPosition = Vector3.Lerp(((Transform)eyesTransform).localPosition, val4, Time.deltaTime * 10f);
			}
		}
		if (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.voiceChatFetched)
		{
			if (left)
			{
				float clipLoudness = playerAvatar.voiceChat.clipLoudness;
				headLeft.localEulerAngles = new Vector3(0f, 0f, (0f - clipLoudness) * 200f);
			}
			if (right)
			{
				float clipLoudness2 = playerAvatar.voiceChat.clipLoudness;
				headRight.localEulerAngles = new Vector3(0f, 0f, clipLoudness2 * 200f);
			}
		}
	}

	public void SetPlayer(PlayerAvatar player)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		playerAvatar = player;
		if (allRawImagesInChildren.Count == 0)
		{
			allRawImagesInChildren.AddRange(((Component)this).GetComponentsInChildren<RawImage>());
		}
		foreach (RawImage allRawImagesInChild in allRawImagesInChildren)
		{
			if (Object.op_Implicit((Object)(object)allRawImagesInChild) && Object.op_Implicit((Object)(object)playerAvatar) && Object.op_Implicit((Object)(object)playerAvatar.playerAvatarVisuals))
			{
				((Graphic)allRawImagesInChild).color = playerAvatar.playerAvatarVisuals.color;
			}
		}
	}

	private void OnDestroy()
	{
		MenuManager.instance.PlayerHeadRemove(this);
		Object.Destroy((Object)(object)((Component)focusPoint).gameObject);
		Object.Destroy((Object)(object)((Component)myFocusPoint).gameObject);
	}

	public static float LengthDirX(float length, float direction)
	{
		return length * Mathf.Cos(direction * (MathF.PI / 180f));
	}

	public static float LengthDirY(float length, float direction)
	{
		return length * Mathf.Sin(direction * (MathF.PI / 180f));
	}
}
