using System.Collections;
using UnityEngine;

public class MapValuable : MonoBehaviour
{
	public ValuableObject target;

	[Space]
	public SpriteRenderer spriteRenderer;

	[Space]
	public Sprite spriteSmall;

	public Sprite spriteBig;

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private IEnumerator Logic()
	{
		while (true)
		{
			if (!Map.Instance.Active)
			{
				yield return (object)new WaitForSeconds(0.25f);
				continue;
			}
			if (!Object.op_Implicit((Object)(object)target))
			{
				break;
			}
			Map.Instance.CustomPositionSet(((Component)this).transform, ((Component)target).transform);
			MapLayer layerParent = Map.Instance.GetLayerParent(((Component)target).transform.position.y + 1f);
			Color color = spriteRenderer.color;
			if (layerParent.layer == Map.Instance.PlayerLayer)
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
}
