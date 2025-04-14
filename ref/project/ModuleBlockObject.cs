using UnityEngine;

public class ModuleBlockObject : MonoBehaviour
{
	private void Start()
	{
		((Component)this).transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}
}
