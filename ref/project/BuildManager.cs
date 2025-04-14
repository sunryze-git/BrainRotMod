using UnityEngine;

public class BuildManager : MonoBehaviour
{
	public static BuildManager instance;

	public Version version;

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			Debug.Log((object)("VERSION: " + version.title));
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
