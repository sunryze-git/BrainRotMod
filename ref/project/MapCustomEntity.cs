using System.Collections;
using UnityEngine;

public class MapCustomEntity : MonoBehaviour
{
	public Transform Parent;

	public SpriteRenderer spriteRenderer;

	public MapCustom mapCustom;

	private float mapCustomHideTimer;

	public IEnumerator Logic()
	{
		while (Object.op_Implicit((Object)(object)Parent) && ((Component)Parent).gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.CustomPositionSet(((Component)this).transform, ((Component)Parent).transform);
			}
			MapLayer layerParent = Map.Instance.GetLayerParent(((Component)Parent).transform.position.y + 1f);
			Color color = spriteRenderer.color;
			if (mapCustomHideTimer > 0f)
			{
				mapCustomHideTimer -= 0.1f;
				color.a = 0f;
			}
			else if (layerParent.layer == Map.Instance.PlayerLayer)
			{
				color.a = 1f;
			}
			else
			{
				color.a = 0.3f;
			}
			spriteRenderer.color = color;
			yield return (object)new WaitForSeconds(0.1f);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void Hide()
	{
		mapCustomHideTimer = 0.5f;
	}
}
