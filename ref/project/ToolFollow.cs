using UnityEngine;

public class ToolFollow : MonoBehaviour
{
	public CameraBob CameraBob;

	private Vector3 StartPosition;

	private Vector3 StartRotation;

	private bool Active;

	public void Activate()
	{
		Active = true;
	}

	public void Deactivate()
	{
		Active = false;
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		StartPosition = ((Component)this).transform.localPosition;
		StartRotation = ((Component)this).transform.localEulerAngles;
	}

	private void Update()
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			((Component)this).transform.localPosition = new Vector3(StartPosition.x, StartPosition.y + ((Component)CameraBob).transform.localPosition.y * 0.1f, StartPosition.z);
			((Component)this).transform.localRotation = Quaternion.Euler(StartRotation.x + ((Component)CameraBob).transform.localPosition.y * 25f, StartRotation.y + ((Component)CameraBob).transform.localEulerAngles.y * 2f, StartRotation.z + ((Component)CameraBob).transform.localEulerAngles.z * 15f);
		}
		else
		{
			((Component)this).transform.localPosition = StartPosition;
			((Component)this).transform.localRotation = Quaternion.Euler(StartRotation);
		}
	}
}
