using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScanlines : MonoBehaviour
{
	private TextMeshProUGUI parentText;

	private float originalAlpha;

	private Image image;

	private float changeColorTimer;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		image = ((Component)this).GetComponent<Image>();
		originalAlpha = ((Graphic)image).color.a;
		parentText = ((Component)this).GetComponentInParent<TextMeshProUGUI>();
	}

	private void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)parentText))
		{
			if (changeColorTimer <= 0f)
			{
				Color color = ((Graphic)parentText).color;
				((Graphic)image).color = new Color(color.r, color.g, color.b, originalAlpha);
				changeColorTimer = 0.03f;
			}
			else
			{
				changeColorTimer -= Time.deltaTime;
			}
		}
	}
}
