using UnityEngine;
using UnityEngine.UI;

public class MenuSelectionBoxTop : MonoBehaviour
{
	private RectTransform rectTransform;

	private RawImage rawImage;

	private bool fadeDone;

	private void Start()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		rawImage = ((Component)this).GetComponentInChildren<RawImage>();
	}

	private void Update()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		MenuSelectionBox activeSelectionBox = MenuManager.instance.activeSelectionBox;
		if (Object.op_Implicit((Object)(object)activeSelectionBox))
		{
			((Transform)rectTransform).localPosition = ((Transform)activeSelectionBox.rectTransform).position - ((Component)this).transform.parent.position;
			((Component)this).transform.localScale = ((Transform)activeSelectionBox.rectTransform).localScale;
			((Graphic)rawImage).color = ((Graphic)activeSelectionBox.rawImage).color * 1.5f;
			fadeDone = false;
		}
		else if (!fadeDone)
		{
			((Graphic)rawImage).color = new Color(1f, 1f, 1f, ((Graphic)rawImage).color.a - Time.deltaTime);
			if (((Graphic)rawImage).color.a <= 0f)
			{
				fadeDone = true;
			}
		}
	}
}
