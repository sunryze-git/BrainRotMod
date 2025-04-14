using UnityEngine;

public class PlayerCrownSet : MonoBehaviour
{
	public static PlayerCrownSet instance;

	internal bool crownOwnerFetched;

	internal string crownOwnerSteamID;

	private void Start()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
