using UnityEngine;

public class RecordingDirector : MonoBehaviour
{
	internal bool hideUI;

	public static RecordingDirector instance;

	public Light playerLight;

	private float lightHue;

	private void Start()
	{
		if ((Object)(object)instance != (Object)null && (Object)(object)instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		if (!PlayerAvatar.instance.isDisabled)
		{
			((Component)playerLight).transform.position = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform.position;
		}
		if (Input.GetKey((KeyCode)260))
		{
			float num2 = default(float);
			float num3 = default(float);
			float num = default(float);
			Color.RGBToHSV(playerLight.color, ref num, ref num2, ref num3);
			num = (num + Time.deltaTime * 0.2f) % 1f;
			playerLight.color = Color.HSVToRGB(num, 1f, 1f);
		}
		if (Input.GetKey((KeyCode)262))
		{
			float num5 = default(float);
			float num6 = default(float);
			float num4 = default(float);
			Color.RGBToHSV(playerLight.color, ref num4, ref num5, ref num6);
			num4 = (num4 - Time.deltaTime * 0.2f) % 1f;
			playerLight.color = Color.HSVToRGB(num4, 1f, 1f);
		}
		if (Input.GetKey((KeyCode)264))
		{
			Light obj = playerLight;
			obj.range += Time.deltaTime * 30f;
		}
		if (Input.GetKey((KeyCode)258))
		{
			Light obj2 = playerLight;
			obj2.range -= Time.deltaTime * 30f;
		}
		if (Input.GetKey((KeyCode)263))
		{
			Light obj3 = playerLight;
			obj3.intensity += Time.deltaTime * 2f;
		}
		if (Input.GetKey((KeyCode)265))
		{
			Light obj4 = playerLight;
			obj4.intensity -= Time.deltaTime * 2f;
		}
		if (Input.GetKey((KeyCode)256))
		{
			playerLight.intensity = 1f;
			playerLight.range = 10f;
			playerLight.color = Color.white;
		}
		if (!Object.op_Implicit((Object)(object)MenuPageEsc.instance) && !ChatManager.instance.chatActive)
		{
			hideUI = true;
		}
		else
		{
			hideUI = false;
		}
		if (hideUI)
		{
			RenderTextureMain.instance.OverlayDisable();
		}
		FlashlightController.Instance.hideFlashlight = true;
		GameplayManager.instance.OverrideCameraAnimation(0f, 0.2f);
		if (SemiFunc.RunIsLobbyMenu() || SemiFunc.MenuLevel())
		{
			GameDirector.instance.CommandRecordingDirectorToggle();
		}
	}
}
