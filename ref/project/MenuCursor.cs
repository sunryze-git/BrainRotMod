using UnityEngine;

public class MenuCursor : MonoBehaviour
{
	private float showTimer;

	private GameObject mesh;

	public static MenuCursor instance;

	private float overridePosTimer;

	private void Start()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		mesh = ((Component)((Component)this).transform.GetChild(0)).gameObject;
		((Component)this).transform.localScale = Vector3.zero;
		mesh.SetActive(false);
		instance = this;
	}

	public void OverridePosition(Vector3 _position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = new Vector3(_position.x, _position.y, 0f);
		overridePosTimer = 0.1f;
	}

	private void Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (overridePosTimer <= 0f)
		{
			if ((int)Cursor.lockState == 0)
			{
				Vector2 val = SemiFunc.UIMousePosToUIPos();
				((Component)this).transform.localPosition = new Vector3(val.x, val.y, 0f);
			}
		}
		else
		{
			overridePosTimer -= Time.deltaTime;
		}
		if (showTimer > 0f)
		{
			if (!mesh.activeSelf)
			{
				mesh.SetActive(true);
			}
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Vector3.one, Time.deltaTime * 30f);
			showTimer -= Time.deltaTime;
		}
		else if (mesh.activeSelf)
		{
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Vector3.zero, Time.deltaTime * 30f);
			Vector3 localScale = ((Component)this).transform.localScale;
			if (((Vector3)(ref localScale)).magnitude < 0.1f && mesh.activeSelf)
			{
				mesh.SetActive(false);
			}
		}
	}

	public void Show()
	{
		showTimer = 0.01f;
	}
}
