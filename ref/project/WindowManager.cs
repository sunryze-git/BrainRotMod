using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
	public static WindowManager instance;

	[DllImport("user32.dll")]
	public static extern bool SetWindowText(IntPtr hwnd, string lpString);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			SetWindowText(FindWindow(null, "Repo"), "R.E.P.O.");
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
