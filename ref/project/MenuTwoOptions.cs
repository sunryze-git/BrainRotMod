using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuTwoOptions : MonoBehaviour
{
	public string option1Text = "ON";

	public string option2Text = "OFF";

	public RectTransform optionsBox;

	public RectTransform optionsBoxBehind;

	public Vector3 targetPosition;

	public Vector3 targetScale;

	public DataDirector.Setting setting;

	public bool customEvents = true;

	public bool settingSet;

	public bool customFetch = true;

	public UnityEvent onOption1;

	public UnityEvent onOption2;

	public UnityEvent fetchSetting;

	public TextMeshProUGUI option1TextMesh;

	public TextMeshProUGUI option2TextMesh;

	public bool startSettingFetch = true;

	private bool fetchComplete;

	public string settingName;

	private void Start()
	{
		if (Object.op_Implicit((Object)(object)option1TextMesh))
		{
			((TMP_Text)option1TextMesh).text = option1Text;
		}
		if (Object.op_Implicit((Object)(object)option1TextMesh))
		{
			((TMP_Text)option2TextMesh).text = option2Text;
		}
		StartFetch();
	}

	private void StartFetch()
	{
		if (customEvents && customFetch)
		{
			fetchSetting.Invoke();
		}
		else
		{
			bool flag = DataDirector.instance.SettingValueFetch(setting) == 1;
			startSettingFetch = flag;
		}
		if (startSettingFetch)
		{
			OnOption1();
		}
		else
		{
			OnOption2();
		}
		fetchComplete = true;
	}

	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			if (Object.op_Implicit((Object)(object)option1TextMesh))
			{
				((TMP_Text)option1TextMesh).text = option1Text;
			}
			if (Object.op_Implicit((Object)(object)option1TextMesh))
			{
				((TMP_Text)option2TextMesh).text = option2Text;
			}
			TextMeshProUGUI componentInChildren = ((Component)this).GetComponentInChildren<TextMeshProUGUI>();
			if (Object.op_Implicit((Object)(object)componentInChildren))
			{
				((TMP_Text)componentInChildren).text = settingName;
			}
			((Object)((Component)this).gameObject).name = "Bool Setting - " + settingName;
		}
	}

	private void OnEnable()
	{
		StartFetch();
	}

	private void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)optionsBox))
		{
			((Transform)optionsBox).localPosition = Vector3.Lerp(((Transform)optionsBox).localPosition, targetPosition, 20f * Time.deltaTime);
			((Transform)optionsBox).localScale = Vector3.Lerp(((Transform)optionsBox).localScale, targetScale / 10f, 20f * Time.deltaTime);
			((Transform)optionsBoxBehind).localPosition = Vector3.Lerp(((Transform)optionsBoxBehind).localPosition, targetPosition, 20f * Time.deltaTime);
			((Transform)optionsBoxBehind).localScale = Vector3.Lerp(((Transform)optionsBoxBehind).localScale, new Vector3(targetScale.x + 4f, targetScale.y + 2f, 1f) / 10f, 20f * Time.deltaTime);
		}
	}

	public void SetTarget(Vector3 pos, Vector3 scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = pos;
		targetScale = scale;
	}

	public void OnOption1()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		SetTarget(new Vector3(37.8f, 12.3f, 0f), new Vector3(73f, 22f, 1f));
		if (!fetchComplete)
		{
			return;
		}
		if (customEvents)
		{
			if (settingSet)
			{
				DataDirector.instance.SettingValueSet(setting, 1);
			}
			onOption1.Invoke();
		}
		else
		{
			DataDirector.instance.SettingValueSet(setting, 1);
		}
	}

	public void OnOption2()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		SetTarget(new Vector3(112.644f, 12.3f, 0f), new Vector3(74f, 22f, 1f));
		if (!fetchComplete)
		{
			return;
		}
		if (customEvents)
		{
			if (settingSet)
			{
				DataDirector.instance.SettingValueSet(setting, 0);
			}
			onOption2.Invoke();
		}
		else
		{
			DataDirector.instance.SettingValueSet(setting, 0);
		}
	}
}
