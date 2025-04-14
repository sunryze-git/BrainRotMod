using UnityEngine;

[CreateAssetMenu(fileName = "Color Preset", menuName = "Semi Presets/Color Preset")]
public class ColorPresets : ScriptableObject
{
	public Color colorMain;

	public Color colorLight;

	public Color colorDark;

	public Color GetColorMain()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return colorMain;
	}

	public Color GetColorLight()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return colorLight;
	}

	public Color GetColorDark()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return colorDark;
	}
}
