using UnityEngine;

public class StartRoom : MonoBehaviour
{
	private void Start()
	{
		((Component)this).transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}
}
