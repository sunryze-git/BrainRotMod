using UnityEngine;

public class PunVoiceClientLogic : MonoBehaviour
{
	public static PunVoiceClientLogic instance;

	private void Awake()
	{
		Debug.Log((object)"PunVoiceClientLogic Awake");
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else if ((Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
