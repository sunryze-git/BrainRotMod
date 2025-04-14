using UnityEngine;
using UnityEngine.UI;

public class InventoryBattery : MonoBehaviour
{
	public int inventorySpot;

	private int batteryState;

	internal RawImage batteryImage;

	private float redBlinkTimer;

	private float batteryShowTimer;

	private Vector3 originalLocalScale;

	private void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		batteryState = 6;
		batteryImage = ((Component)this).GetComponent<RawImage>();
		((Behaviour)batteryImage).enabled = false;
		originalLocalScale = ((Component)this).transform.localScale;
		((Component)this).transform.localScale = Vector3.zero;
	}

	public void BatteryFetch()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)Inventory.instance) && Object.op_Implicit((Object)(object)batteryImage))
		{
			int batteryStateFromInventorySpot = Inventory.instance.GetBatteryStateFromInventorySpot(inventorySpot);
			if (batteryStateFromInventorySpot != -1 && redBlinkTimer == 0f)
			{
				batteryState = batteryStateFromInventorySpot;
				((Graphic)batteryImage).color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	public void BatteryShow()
	{
		batteryShowTimer = 0.2f;
	}

	private void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (batteryShowTimer > 0f)
		{
			batteryShowTimer -= Time.deltaTime;
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, originalLocalScale, Time.deltaTime * 30f);
			((Behaviour)batteryImage).enabled = true;
		}
		else if (((Behaviour)batteryImage).enabled)
		{
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			if (((Component)this).transform.localScale.x < 0.01f)
			{
				((Component)this).transform.localScale = Vector3.zero;
				((Behaviour)batteryImage).enabled = false;
			}
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
			redBlinkTimer += Time.deltaTime;
			if (redBlinkTimer > 0.5f)
			{
				((Graphic)batteryImage).color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				((Graphic)batteryImage).color = new Color(1f, 1f, 1f, 1f);
			}
			if (redBlinkTimer > 1f)
			{
				redBlinkTimer = 0f;
			}
		}
	}
}
