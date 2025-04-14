using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public static CursorManager instance;

	private float unlockTimer;

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Update()
	{
		if (unlockTimer > 0f)
		{
			if (Object.op_Implicit((Object)(object)MenuCursor.instance))
			{
				MenuCursor.instance.Show();
			}
			unlockTimer -= Time.deltaTime;
		}
		else if (unlockTimer != -1234f)
		{
			Cursor.lockState = (CursorLockMode)1;
			unlockTimer = -1234f;
		}
	}

	public void Unlock(float _time)
	{
		Cursor.lockState = (CursorLockMode)0;
		Cursor.visible = false;
		unlockTimer = _time;
	}
}
