using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPageSaves : MonoBehaviour
{
	public RectTransform saveFileInfo;

	public GameObject saveInfoDefault;

	public GameObject saveInfoSelected;

	public TextMeshProUGUI saveFileHeader;

	public TextMeshProUGUI saveFileHeaderDate;

	public TextMeshProUGUI saveFileInfoRow1;

	public TextMeshProUGUI saveFileInfoRow2;

	public TextMeshProUGUI saveFileInfoRow3;

	private Image saveFileInfoPanel;

	public RectTransform Scroller;

	public RectTransform saveFilePosition;

	public GameObject saveFilePrefab;

	internal string currentSaveFileName;

	internal List<MenuElementSaveFile> saveFiles = new List<MenuElementSaveFile>();

	internal float saveFileYOffset;

	public TextMeshProUGUI gameModeHeader;

	private void Start()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		saveFileInfoPanel = ((Component)saveFileInfo).GetComponentInChildren<Image>();
		List<string> list = StatsManager.instance.SaveFileGetAll();
		float num = 0f;
		Color numberColor = default(Color);
		Color unitColor = default(Color);
		foreach (string item in list)
		{
			GameObject val = Object.Instantiate<GameObject>(saveFilePrefab, (Transform)(object)Scroller);
			val.transform.localPosition = ((Transform)saveFilePosition).localPosition;
			val.transform.SetSiblingIndex(3);
			MenuElementSaveFile component = val.GetComponent<MenuElementSaveFile>();
			component.saveFileName = item;
			string text = StatsManager.instance.SaveFileGetTeamName(item);
			string text2 = StatsManager.instance.SaveFileGetDateAndTime(item);
			int num2 = int.Parse(StatsManager.instance.SaveFileGetRunLevel(item)) + 1;
			((TMP_Text)component.saveFileHeaderDate).text = text2;
			string text3 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, num2));
			float time = StatsManager.instance.SaveFileGetTimePlayed(item);
			((TMP_Text)component.saveFileHeaderLevel).text = "<sprite name=truck> <color=#" + text3 + ">" + num2 + "</color>";
			((TMP_Text)component.saveFileHeader).text = text;
			((Color)(ref numberColor))._002Ector(0.1f, 0.4f, 0.8f);
			((Color)(ref unitColor))._002Ector(0.05f, 0.3f, 0.6f);
			((TMP_Text)component.saveFileInfoRow1).text = "<sprite name=clock>  " + SemiFunc.TimeToString(time, fancy: true, numberColor, unitColor);
			val.transform.localPosition = new Vector3(val.transform.localPosition.x, val.transform.localPosition.y + num, val.transform.localPosition.z);
			Rect rect = val.GetComponent<RectTransform>().rect;
			float num3 = ((Rect)(ref rect)).height + 2f;
			num -= num3;
			saveFileYOffset = num3;
			saveFiles.Add(val.GetComponent<MenuElementSaveFile>());
		}
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			((TMP_Text)gameModeHeader).text = "Multiplayer mode";
		}
		else
		{
			((TMP_Text)gameModeHeader).text = "Singleplayer mode";
		}
	}

	public void OnGoBack()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Main);
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (((Graphic)saveFileInfoPanel).color != new Color(0f, 0f, 0f, 1f))
		{
			((Graphic)saveFileInfoPanel).color = Color.Lerp(((Graphic)saveFileInfoPanel).color, new Color(0f, 0f, 0f, 1f), Time.deltaTime * 10f);
		}
	}

	public void OnNewGame()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (saveFiles.Count >= 10)
		{
			MenuManager.instance.PageCloseAllAddedOnTop();
			MenuManager.instance.PagePopUp("Save file limit reached", Color.red, "You can only have 10 save files at a time. Please delete some save files to make room for new ones.", "OK");
		}
		else if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame();
		}
		else
		{
			SemiFunc.MenuActionSingleplayerGame();
		}
	}

	public void OnLoadGame()
	{
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame(currentSaveFileName);
		}
		else
		{
			SemiFunc.MenuActionSingleplayerGame(currentSaveFileName);
		}
	}

	public void OnDeleteGame()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		SemiFunc.SaveFileDelete(currentSaveFileName);
		bool flag = false;
		foreach (MenuElementSaveFile saveFile in saveFiles)
		{
			if (flag && Object.op_Implicit((Object)(object)saveFile))
			{
				RectTransform component = ((Component)saveFile).GetComponent<RectTransform>();
				((Transform)component).localPosition = new Vector3(((Transform)component).localPosition.x, ((Transform)component).localPosition.y + saveFileYOffset, ((Transform)component).localPosition.z);
				MenuElementAnimations component2 = ((Component)saveFile).GetComponent<MenuElementAnimations>();
				component2.UIAniNudgeY();
				component2.UIAniRotate();
				component2.UIAniNewInitialPosition(new Vector2(((Transform)component).localPosition.x, ((Transform)component).localPosition.y));
			}
			if (saveFile.saveFileName == currentSaveFileName)
			{
				Object.Destroy((Object)(object)((Component)saveFile).gameObject);
				flag = true;
			}
		}
		saveFiles.RemoveAll((MenuElementSaveFile x) => (Object)(object)x == (Object)null);
		GoBackToDefaultInfo();
	}

	public void GoBackToDefaultInfo()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		MenuElementAnimations component = ((Component)saveFileInfo).GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX();
		component.UIAniRotate();
		saveInfoDefault.SetActive(true);
		saveInfoSelected.SetActive(false);
		((Graphic)saveFileInfoPanel).color = new Color(0.45f, 0f, 0f, 1f);
	}

	private void InfoPlayerNames(TextMeshProUGUI _textMesh, string _fileName)
	{
		((TMP_Text)_textMesh).text = "";
		List<string> list = StatsManager.instance.SaveFileGetPlayerNames(_fileName);
		list.Sort((string a, string b) => a.Length.CompareTo(b.Length));
		if (list != null)
		{
			int count = list.Count;
			int num = 0;
			foreach (string item in list)
			{
				if (num == count - 1)
				{
					((TMP_Text)_textMesh).text = ((TMP_Text)_textMesh).text + item;
				}
				else if (num == count - 2)
				{
					((TMP_Text)_textMesh).text = ((TMP_Text)_textMesh).text + item + "<color=#444444>   and   </color>";
				}
				else
				{
					((TMP_Text)_textMesh).text = ((TMP_Text)_textMesh).text + item + "<color=#444444>,</color>   ";
				}
				num++;
			}
		}
		if (list == null || (list != null && list.Count == 0))
		{
			((TMP_Text)_textMesh).text = ((TMP_Text)_textMesh).text + "You did it all alone!";
		}
	}

	public void SaveFileSelected(string saveFileName)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		MenuElementAnimations component = ((Component)saveFileInfo).GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX();
		component.UIAniRotate();
		saveInfoDefault.SetActive(false);
		saveInfoSelected.SetActive(true);
		((Graphic)saveFileInfoPanel).color = new Color(0f, 0.1f, 0.25f, 1f);
		string text = StatsManager.instance.SaveFileGetTeamName(saveFileName);
		string text2 = StatsManager.instance.SaveFileGetDateAndTime(saveFileName);
		((TMP_Text)saveFileHeader).text = text;
		((TMP_Text)saveFileHeaderDate).text = text2;
		currentSaveFileName = saveFileName;
		string text3 = "      ";
		float time = StatsManager.instance.SaveFileGetTimePlayed(saveFileName);
		int num = int.Parse(StatsManager.instance.SaveFileGetRunLevel(saveFileName)) + 1;
		string text4 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, num));
		string text5 = StatsManager.instance.SaveFileGetRunCurrency(saveFileName);
		((TMP_Text)saveFileInfoRow1).text = "<sprite name=truck>  <color=#" + text4 + "><b>" + num + "</b></color>";
		TextMeshProUGUI obj = saveFileInfoRow1;
		((TMP_Text)obj).text = ((TMP_Text)obj).text + text3;
		TextMeshProUGUI obj2 = saveFileInfoRow1;
		((TMP_Text)obj2).text = ((TMP_Text)obj2).text + "<sprite name=clock>  " + SemiFunc.TimeToString(time, fancy: true, new Color(0.1f, 0.4f, 0.8f), new Color(0.05f, 0.3f, 0.6f));
		TextMeshProUGUI obj3 = saveFileInfoRow1;
		((TMP_Text)obj3).text = ((TMP_Text)obj3).text + text3;
		string text6 = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
		TextMeshProUGUI val = saveFileInfoRow1;
		((TMP_Text)val).text = ((TMP_Text)val).text + "<sprite name=$$>  <b>" + text5 + "</b><color=#" + text6 + ">k</color>";
		string text7 = SemiFunc.DollarGetString(int.Parse(StatsManager.instance.SaveFileGetTotalHaul(saveFileName)));
		((TMP_Text)saveFileInfoRow2).text = "<color=#" + text6 + "><sprite name=$$$> TOTAL HAUL:      <b></b>$ </color><b>" + text7 + "</b><color=#" + text6 + ">k</color>";
		InfoPlayerNames(saveFileInfoRow3, saveFileName);
	}
}
