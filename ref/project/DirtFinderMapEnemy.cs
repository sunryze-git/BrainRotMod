using System.Collections;
using UnityEngine;

public class DirtFinderMapEnemy : MonoBehaviour
{
	public Transform Parent;

	public IEnumerator Logic()
	{
		while ((Object)(object)Parent != (Object)null && ((Component)Parent).gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.EnemyPositionSet(((Component)this).transform, ((Component)Parent).transform);
			}
			yield return (object)new WaitForSeconds(0.1f);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
