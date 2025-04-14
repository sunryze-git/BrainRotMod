using UnityEngine;
using UnityEngine.UI;

public class MenuSelectionBox : MonoBehaviour
{
	public static MenuSelectionBox instance;

	internal Vector3 targetPosition;

	internal Vector3 targetScale;

	internal RawImage rawImage;

	internal RectTransform rectTransform;

	internal Vector3 originalPos;

	internal Vector3 originalScale;

	private float activeTargetTimer;

	private float prevPosTimer;

	private Vector3 prevPos;

	private Vector3 currentPos;

	private Vector3 pulsatePos;

	private float clickTimer;

	private Color flashColor;

	internal MenuPage menuPage;

	internal bool firstSelection = true;

	internal bool isInScrollBox;

	internal MenuScrollBox menuScrollBox;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = ((Component)this).transform.localPosition;
		targetScale = ((Component)this).transform.localScale * 100f;
		originalPos = ((Component)this).transform.localPosition;
		originalScale = ((Component)this).transform.localScale;
		rawImage = ((Component)this).GetComponentInChildren<RawImage>();
		menuPage = ((Component)this).GetComponentInParent<MenuPage>();
		menuPage.selectionBox = this;
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		isInScrollBox = false;
		menuScrollBox = ((Component)this).GetComponentInParent<MenuScrollBox>();
		if (Object.op_Implicit((Object)(object)menuScrollBox))
		{
			isInScrollBox = true;
		}
		else
		{
			instance = this;
		}
		MenuManager.instance.SelectionBoxAdd(this);
	}

	private void Update()
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		if (menuPage.currentPageState != MenuPage.PageState.Active && !menuPage.addedPageOnTop)
		{
			((Graphic)rawImage).color = new Color(0.4f, 0.08f, 0.015f, 0f);
			RectTransform component = ((Component)menuPage).GetComponent<RectTransform>();
			Transform transform = ((Component)this).transform;
			Rect rect = component.rect;
			float num = ((Rect)(ref rect)).width / 2f;
			rect = component.rect;
			transform.localPosition = new Vector3(num, ((Rect)(ref rect)).height / 2f, ((Component)this).transform.localPosition.z);
			((Component)this).transform.localScale = new Vector3(0f, 0f, 1f);
			targetScale = ((Component)this).transform.localScale * 100f;
			targetPosition = ((Transform)rectTransform).localPosition;
			activeTargetTimer = 0f;
			return;
		}
		if (prevPosTimer <= 0f)
		{
			prevPos = currentPos;
			currentPos = ((Component)this).transform.localPosition - pulsatePos;
			prevPosTimer = 1f / 120f;
		}
		else
		{
			prevPosTimer -= Time.deltaTime;
		}
		((Transform)rectTransform).localPosition = Vector3.Lerp(((Transform)rectTransform).localPosition, targetPosition, 20f * Time.deltaTime);
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, targetScale / 100f, 20f * Time.deltaTime);
		if (activeTargetTimer > 0f)
		{
			float num2 = 0.5f;
			((Component)this).transform.localScale = ((Component)this).transform.localScale + new Vector3(num2 * 0.01f, num2 * 0.01f, 1f) * Mathf.Sin(Time.time * 20f);
			((Component)this).transform.localPosition = ((Component)this).transform.localPosition + pulsatePos;
			activeTargetTimer -= Time.deltaTime;
			Color val = default(Color);
			((Color)(ref val))._002Ector(0.08f, 0.2f, 0.4f, 0.75f);
			Color val2 = default(Color);
			((Color)(ref val2))._002Ector(0.2f, 0.5f, 1f, 1f);
			if (Vector3.Distance(((Component)this).transform.localPosition, targetPosition) <= 5f)
			{
				prevPos = currentPos;
			}
			((Graphic)rawImage).color = Color.Lerp(val, val2, Vector3.Distance(prevPos, currentPos) * 0.5f);
		}
		else
		{
			Color val3 = default(Color);
			((Color)(ref val3))._002Ector(0.4f, 0.08f, 0.015f, 0f);
			((Graphic)rawImage).color = Color.Lerp(((Graphic)rawImage).color, val3, 10f * Time.deltaTime);
		}
		ClickColorAnimate();
	}

	public void MenuSelectionBoxSetTarget(Vector3 pos, Vector3 scale, MenuPage parentPage, bool _isInScrollBox, MenuScrollBox _menuScrollBox, Vector2 customScale = default(Vector2))
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (_isInScrollBox != isInScrollBox || (_isInScrollBox && (Object)(object)_menuScrollBox != (Object)(object)menuScrollBox))
		{
			MenuSelectionBox menuSelectionBox = MenuManager.instance.SelectionBoxGetCorrect(parentPage, _menuScrollBox);
			if (Object.op_Implicit((Object)(object)menuSelectionBox))
			{
				MenuManager.instance.SetActiveSelectionBox(menuSelectionBox);
				parentPage.selectionBox = menuSelectionBox;
				menuSelectionBox.Reinstate();
				menuSelectionBox.MenuSelectionBoxSetTarget(pos, scale, parentPage, _isInScrollBox, _menuScrollBox, customScale);
			}
			return;
		}
		MenuManager.instance.SetActiveSelectionBox(this);
		if ((Object)(object)instance != (Object)(object)this)
		{
			Reinstate();
		}
		if (firstSelection)
		{
			firstSelection = false;
			((Component)this).transform.localPosition = pos;
			((Component)this).transform.localScale = Vector3.zero;
			targetPosition = pos;
			targetScale = scale;
		}
		else
		{
			((Vector3)(ref pos))._002Ector(pos.x, pos.y, 0f);
			targetPosition = pos;
			targetScale = scale + new Vector3(customScale.x, customScale.y, 0f);
			float num = targetScale.y * 0.2f;
			targetScale += new Vector3(num, num, 0f);
			targetPosition += new Vector3(0f, 0f, 0f);
			activeTargetTimer = 0.2f;
		}
	}

	public void SetClick(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		flashColor = color;
		clickTimer = 1f;
	}

	private void ClickColorAnimate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!(clickTimer <= 0f))
		{
			Color val = flashColor;
			Color val2 = default(Color);
			((Color)(ref val2))._002Ector(0.08f, 0.2f, 0.4f, 0.75f);
			((Graphic)rawImage).color = Color.Lerp(val, val2, 1f - clickTimer);
			clickTimer -= Time.deltaTime * 10f;
		}
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = originalPos;
		((Component)this).transform.localScale = originalScale;
		targetScale = originalScale * 100f;
		targetPosition = originalPos;
	}

	private void OnDestroy()
	{
		MenuManager.instance.SelectionBoxRemove(this);
	}

	public void Reinstate()
	{
		instance = this;
	}
}
