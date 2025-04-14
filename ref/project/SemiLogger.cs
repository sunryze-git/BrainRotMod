using UnityEngine;

public class SemiLogger : MonoBehaviour
{
	[HideInCallstack]
	public static void Log(SemiFunc.User _user, object _message, GameObject _obj = null, Color? color = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.DebugUser(_user))
		{
			string arg = ColorUtility.ToHtmlStringRGB((Color)(((_003F?)color) ?? Color.Lerp(Color.gray, Color.white, 0.4f)));
			Debug.Log((object)$"<color=#{arg}>{_message}</color>", (Object)(object)_obj);
		}
	}

	[HideInCallstack]
	public static void LogAxel(object _message, GameObject _obj = null, Color? color = null)
	{
		Log(SemiFunc.User.Axel, _message, _obj, color);
	}

	[HideInCallstack]
	public static void LogJannek(object _message, GameObject _obj = null, Color? color = null)
	{
		Log(SemiFunc.User.Jannek, _message, _obj, color);
	}

	[HideInCallstack]
	public static void LogRobin(object _message, GameObject _obj = null, Color? color = null)
	{
		Log(SemiFunc.User.Robin, _message, _obj, color);
	}

	[HideInCallstack]
	public static void LogRuben(object _message, GameObject _obj = null, Color? color = null)
	{
		Log(SemiFunc.User.Ruben, _message, _obj, color);
	}

	[HideInCallstack]
	public static void LogWalter(object _message, GameObject _obj = null, Color? color = null)
	{
		Log(SemiFunc.User.Walter, _message, _obj, color);
	}
}
