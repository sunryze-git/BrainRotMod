using UnityEngine;
using UnityEngine.UI;

public class MenuSliderPointer : MonoBehaviour
{
	private RawImage rawImage;

	private void Start()
	{
		rawImage = ((Component)this).GetComponent<RawImage>();
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, new Vector3(1f, 1f, 1f), 15f * Time.deltaTime);
		((Graphic)rawImage).color = Color.Lerp(((Graphic)rawImage).color, Color.red, 5f * Time.deltaTime);
	}

	public void Tick()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)rawImage))
		{
			((Component)this).transform.localScale = new Vector3(1f, 3f, 1f);
			((Graphic)rawImage).color = new Color(0.5f, 0.5f, 1f, 1f);
		}
	}
}
