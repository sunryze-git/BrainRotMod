using UnityEngine;

public class ItemGrenadeShockwave : MonoBehaviour
{
	public GameObject shockwavePrefab;

	public void Explosion()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Object.Instantiate<GameObject>(shockwavePrefab, ((Component)this).transform.position, Quaternion.identity);
	}
}
