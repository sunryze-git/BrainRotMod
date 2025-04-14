using UnityEngine;

public class PlayerAvatarMenuHover : MonoBehaviour
{
	private RectTransform rectTransform;

	private MenuPage parentPage;

	public Transform pointer;

	private bool startClick;

	private Vector2 mouseClickPos;

	public PlayerAvatarMenu playerAvatarMenu;

	private MenuElementHover menuElementHover;

	private void Start()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		menuElementHover = ((Component)this).GetComponent<MenuElementHover>();
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(rectTransform);
		pointer.localPosition = new Vector3(val.x * 0.98f, val.y * 1.035f, 0f) / SemiFunc.UIMulti() * 2.23f;
		Transform obj = pointer;
		obj.localPosition += new Vector3(-0.065f, -0.06f, 0f);
		((Renderer)((Component)pointer).GetComponent<MeshRenderer>()).enabled = false;
		if (SemiFunc.InputHold(InputKey.Grab) && menuElementHover.isHovering)
		{
			if (!startClick)
			{
				startClick = true;
				mouseClickPos = val;
			}
			Vector2 val2 = (val - mouseClickPos) * 25f;
			playerAvatarMenu.Rotate(new Vector3(0f, 0f - val2.x, 0f));
		}
		else
		{
			startClick = false;
		}
	}
}
