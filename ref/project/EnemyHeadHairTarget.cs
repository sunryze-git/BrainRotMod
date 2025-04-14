using UnityEngine;

public class EnemyHeadHairTarget : MonoBehaviour
{
	public Transform Parent;

	private void Start()
	{
		((Component)this).transform.parent = Parent;
	}
}
