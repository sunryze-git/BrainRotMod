using UnityEngine;

public class EnvironmentDirector : MonoBehaviour
{
	public static EnvironmentDirector Instance;

	private bool SetupDone;

	private Camera MainCamera;

	[Space]
	public float DarkAdaptationSpeedIn = 5f;

	public float DarkAdaptationSpeedOut = 5f;

	public AnimationCurve DarkAdaptationCurve;

	private float DarkAdaptationLerp;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		MainCamera = Camera.main;
	}

	public void Setup()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		RenderSettings.fogColor = LevelGenerator.Instance.Level.FogColor;
		RenderSettings.fogStartDistance = LevelGenerator.Instance.Level.FogStartDistance;
		RenderSettings.fogEndDistance = LevelGenerator.Instance.Level.FogEndDistance;
		MainCamera.backgroundColor = RenderSettings.fogColor;
		MainCamera.farClipPlane = RenderSettings.fogEndDistance + 1f;
		DarkAdaptationLerp = 0.1f;
		if (LevelGenerator.Instance.Level.AmbiencePresets.Count > 0)
		{
			AmbienceLoop.instance.Setup();
			AmbienceBreakers.instance.Setup();
		}
		else
		{
			Debug.LogError((object)"Level is missing ambience preset!");
		}
		SetupDone = true;
	}

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!SetupDone)
		{
			return;
		}
		Color ambientColor = LevelGenerator.Instance.Level.AmbientColor;
		Color ambientColorAdaptation = LevelGenerator.Instance.Level.AmbientColorAdaptation;
		if (!FlashlightController.Instance.LightActive)
		{
			if (DarkAdaptationLerp < 1f)
			{
				DarkAdaptationLerp += Time.deltaTime * DarkAdaptationSpeedIn;
				DarkAdaptationLerp = Mathf.Clamp01(DarkAdaptationLerp);
				RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, DarkAdaptationCurve.Evaluate(DarkAdaptationLerp));
			}
		}
		else if (DarkAdaptationLerp > 0f)
		{
			DarkAdaptationLerp -= Time.deltaTime * DarkAdaptationSpeedOut;
			DarkAdaptationLerp = Mathf.Clamp01(DarkAdaptationLerp);
			RenderSettings.ambientLight = Color.Lerp(ambientColor, ambientColorAdaptation, DarkAdaptationCurve.Evaluate(DarkAdaptationLerp));
		}
	}
}
