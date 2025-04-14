using UnityEngine;
using UnityEngine.Serialization;

public class SemiIconMaker : MonoBehaviour
{
	[FormerlySerializedAs("camera")]
	public Camera iconCamera;

	public RenderTexture renderTexture;

	public bool iconCameraPlacementDone;

	public Sprite CreateIconFromRenderTexture()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)renderTexture))
		{
			Debug.LogError((object)"RenderTexture is null");
			return null;
		}
		if (!ItemManager.instance.firstIcon)
		{
			Light[] componentsInChildren = ((Component)this).GetComponentsInChildren<Light>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Behaviour)componentsInChildren[i]).enabled = false;
			}
		}
		else
		{
			ItemManager.instance.firstIcon = false;
		}
		Transform transform = ((Component)((Component)this).GetComponentInParent<ItemAttributes>()).transform;
		Vector3 position = transform.position;
		transform.position = new Vector3(-1000f, -1000f, -1000f);
		Texture2D val = new Texture2D(((Texture)renderTexture).width, ((Texture)renderTexture).height);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		RenderSettings.fog = false;
		Color ambientLight = RenderSettings.ambientLight;
		RenderSettings.ambientLight = Color.white;
		iconCamera.Render();
		RenderSettings.fog = true;
		RenderSettings.ambientLight = ambientLight;
		val.ReadPixels(new Rect(0f, 0f, (float)((Texture)renderTexture).width, (float)((Texture)renderTexture).height), 0, 0);
		val.Apply();
		RenderTexture.active = active;
		transform.position = position;
		Sprite result = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
		((Component)this).gameObject.SetActive(false);
		return result;
	}
}
