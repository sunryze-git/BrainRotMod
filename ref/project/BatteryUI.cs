using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : SemiUI
{
	public static BatteryUI instance;

	private int batteryState;

	public RawImage batteryImage;

	private float redBlinkTimer;

	private float batteryShowTimer;

	private Vector3 originalLocalScale;

	protected override void Start()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		instance = this;
		batteryState = 6;
		originalLocalScale = ((Component)this).transform.localScale;
		((Component)this).transform.localScale = Vector3.zero;
	}

	private void BatteryLogic()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated || !SemiFunc.FPSImpulse15() || !Object.op_Implicit((Object)(object)batteryImage))
		{
			return;
		}
		if (batteryState == 0)
		{
			batteryImage.uvRect = new Rect(-0.006f, -0.921f, 0.4f, 0.2f);
		}
		if (batteryState == 1)
		{
			batteryImage.uvRect = new Rect(0.369f, -0.687f, 0.4f, 0.2f);
		}
		if (batteryState == 2)
		{
			batteryImage.uvRect = new Rect(-0.006f, -0.687f, 0.4f, 0.2f);
		}
		if (batteryState == 3)
		{
			batteryImage.uvRect = new Rect(0.369f, -0.4523f, 0.4f, 0.2f);
		}
		if (batteryState == 4)
		{
			batteryImage.uvRect = new Rect(-0.006f, -0.4523f, 0.4f, 0.2f);
		}
		if (batteryState == 5)
		{
			batteryImage.uvRect = new Rect(0.369f, -0.218f, 0.4f, 0.2f);
		}
		if (batteryState == 6)
		{
			batteryImage.uvRect = new Rect(-0.006f, -0.218f, 0.4f, 0.2f);
		}
		if (batteryState > 6)
		{
			batteryState = 6;
		}
		if (batteryState < 0)
		{
			batteryState = 0;
		}
		if (batteryState <= 3 && SemiFunc.RunIsLobby())
		{
			float num = 1.8f;
			redBlinkTimer += Time.deltaTime * num;
			if (redBlinkTimer > 0.5f)
			{
				((Graphic)batteryImage).color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				Color color = default(Color);
				((Color)(ref color))._002Ector(1f, 0.7f, 0f, 1f);
				((Graphic)batteryImage).color = color;
			}
			if (redBlinkTimer > 1f)
			{
				redBlinkTimer = 0f;
			}
		}
	}

	protected override void Update()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (SemiFunc.RunIsShop())
		{
			Hide();
			return;
		}
		BatteryFetch();
		BatteryLogic();
		if (batteryShowTimer > 0f)
		{
			if (!PhysGrabber.instance.grabbed)
			{
				batteryShowTimer = 0f;
			}
			batteryShowTimer -= Time.deltaTime;
			ItemInfoUI.instance.SemiUIScoot(new Vector2(0f, 20f));
		}
		else
		{
			Hide();
		}
	}

	public void BatteryFetch()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)batteryImage) || !PhysGrabber.instance.grabbed || !SemiFunc.FPSImpulse5())
		{
			return;
		}
		PhysGrabObject grabbedPhysGrabObject = PhysGrabber.instance.grabbedPhysGrabObject;
		if (!Object.op_Implicit((Object)(object)grabbedPhysGrabObject))
		{
			return;
		}
		ItemBattery component = ((Component)grabbedPhysGrabObject).GetComponent<ItemBattery>();
		if (!Object.op_Implicit((Object)(object)component) || (component.onlyShowWhenItemToggleIsOn && !((Component)grabbedPhysGrabObject).GetComponent<ItemToggle>().toggleState))
		{
			return;
		}
		int batteryLifeInt = component.batteryLifeInt;
		if (batteryLifeInt != -1)
		{
			batteryState = batteryLifeInt;
			Color red = default(Color);
			((Color)(ref red))._002Ector(1f, 0.7f, 0f, 1f);
			if (batteryState == 0)
			{
				red = Color.red;
			}
			((Graphic)batteryImage).color = red;
		}
		batteryShowTimer = 1f;
	}
}
