using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuColorSelected : MonoBehaviour
{
	public SpringVector3 positionSpring;

	internal Vector3 selectedPosition;

	public RawImage rawImage;

	private MenuPage parentPage;

	private bool goTime;

	private void Start()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		positionSpring = new SpringVector3();
		positionSpring.speed = 50f;
		positionSpring.damping = 0.55f;
		positionSpring.lastPosition = ((Component)this).transform.position;
	}

	public void SetColor(Color color, Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((MonoBehaviour)this).StartCoroutine(SetColorRoutine(color, position));
	}

	private IEnumerator SetColorRoutine(Color color, Vector3 position)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(0.01f);
		((Graphic)rawImage).color = color;
		selectedPosition = position;
		goTime = true;
	}

	private void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (goTime && parentPage.currentPageState != MenuPage.PageState.Closing)
		{
			((Component)this).transform.position = SemiFunc.SpringVector3Get(positionSpring, selectedPosition + Vector3.up * 0.038f + Vector3.right * 0.046f);
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x + 18f, ((Component)this).transform.position.y + 16f, 1f);
		}
	}
}
