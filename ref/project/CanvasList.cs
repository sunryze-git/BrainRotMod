using UnityEngine;

public class CanvasList : MonoBehaviour
{
	private void Awake()
	{
		PopulateAvailableTextures();
	}

	public static void PopulateAvailableTextures()
	{
		CanvasAssigner.AvailableTextures.Clear();
		Texture2D[] array = Resources.LoadAll<Texture2D>("Canvas");
		if (array.Length == 0)
		{
			Debug.LogWarning((object)"No textures were loaded from the Resources/Canvas folder.");
		}
		Texture2D[] array2 = array;
		foreach (Texture2D val in array2)
		{
			if ((Object)(object)val != (Object)null)
			{
				CanvasAssigner.AvailableTextures.Add((Texture)(object)val);
			}
			else
			{
				Debug.LogWarning((object)"A texture was found but is not a Texture2D or could not be cast to Texture2D.");
			}
		}
	}
}
