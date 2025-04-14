using UnityEngine;

public class ModulePropSwitch : MonoBehaviour
{
	public enum Connection
	{
		Top,
		Right,
		Bot,
		Left
	}

	internal Module Module;

	public GameObject ConnectedParent;

	public GameObject NotConnectedParent;

	private bool Connected;

	[Space(20f)]
	public Connection ConnectionSide;

	[HideInInspector]
	public string DebugState = "...";

	[HideInInspector]
	public bool DebugSwitch;

	public void Setup()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		while (true)
		{
			float num2 = num;
			Quaternion localRotation = ((Component)Module).transform.localRotation;
			if (!(num2 < ((Quaternion)(ref localRotation)).eulerAngles.y))
			{
				break;
			}
			num += 90;
			ConnectionSide++;
			if (ConnectionSide > Connection.Left)
			{
				ConnectionSide = Connection.Top;
			}
		}
		if (ConnectionSide == Connection.Top && Module.ConnectingTop)
		{
			Connected = true;
		}
		else if (ConnectionSide == Connection.Right && Module.ConnectingRight)
		{
			Connected = true;
		}
		else if (ConnectionSide == Connection.Bot && Module.ConnectingBottom)
		{
			Connected = true;
		}
		else if (ConnectionSide == Connection.Left && Module.ConnectingLeft)
		{
			Connected = true;
		}
		if (Connected)
		{
			NotConnectedParent.SetActive(false);
			ConnectedParent.SetActive(true);
		}
		else
		{
			NotConnectedParent.SetActive(true);
			ConnectedParent.SetActive(false);
		}
	}

	public void Toggle()
	{
		if (DebugSwitch)
		{
			DebugSwitch = false;
			DebugState = "Connected";
			NotConnectedParent.SetActive(false);
			ConnectedParent.SetActive(true);
		}
		else
		{
			DebugSwitch = true;
			DebugState = "Not Connected";
			NotConnectedParent.SetActive(true);
			ConnectedParent.SetActive(false);
		}
	}
}
