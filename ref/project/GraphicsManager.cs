using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsManager : MonoBehaviour
{
	public static GraphicsManager instance;

	internal float lightDistance;

	internal float shadowDistance;

	internal float gamma;

	public AnimationCurve gammaCurve;

	internal bool glitchLoop;

	private float fullscreenCheckTimer;

	private FullScreenMode windowMode;

	private bool windowFullscreen;

	private float firstSetupTimer = 1f;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Invalid comparison between Unknown and I4
		if (firstSetupTimer > 0f)
		{
			firstSetupTimer -= Time.deltaTime;
			if (firstSetupTimer <= 0f)
			{
				UpdateAll();
			}
		}
		else if (fullscreenCheckTimer <= 0f)
		{
			fullscreenCheckTimer = 1f;
			if (Screen.fullScreenMode != windowMode || Screen.fullScreen != windowFullscreen)
			{
				if ((int)Screen.fullScreenMode == 1)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 0);
				}
				else if ((int)Screen.fullScreenMode == 3)
				{
					DataDirector.instance.SettingValueSet(DataDirector.Setting.WindowMode, 1);
				}
				UpdateWindowMode(_setResolution: false);
				if (Object.op_Implicit((Object)(object)GraphicsButtonWindowMode.instance))
				{
					GraphicsButtonWindowMode.instance.UpdateSlider();
				}
			}
		}
		else
		{
			fullscreenCheckTimer -= Time.deltaTime;
		}
	}

	public void UpdateAll()
	{
		UpdateVsync();
		UpdateMaxFPS();
		UpdateLightDistance();
		UpdateShadowQuality();
		UpdateShadowDistance();
		UpdateMotionBlur();
		UpdateLensDistortion();
		UpdateBloom();
		UpdateChromaticAberration();
		UpdateGrain();
		UpdateWindowMode(_setResolution: false);
		UpdateRenderSize();
		UpdateGlitchLoop();
		UpdateGamma();
	}

	public void UpdateVsync()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Vsync) == 1)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
	}

	public void UpdateMaxFPS()
	{
		Application.targetFrameRate = DataDirector.instance.SettingValueFetch(DataDirector.Setting.MaxFPS);
	}

	public void UpdateLightDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LightDistance))
		{
		case 0:
			lightDistance = 10f;
			break;
		case 1:
			lightDistance = 15f;
			break;
		case 2:
			lightDistance = 20f;
			break;
		case 3:
			lightDistance = 25f;
			break;
		case 4:
			lightDistance = 30f;
			break;
		}
		LightManager.instance.UpdateInstant();
	}

	public void UpdateShadowQuality()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowQuality))
		{
		case 0:
			QualitySettings.shadowResolution = (ShadowResolution)0;
			break;
		case 1:
			QualitySettings.shadowResolution = (ShadowResolution)1;
			break;
		case 2:
			QualitySettings.shadowResolution = (ShadowResolution)2;
			break;
		case 3:
			QualitySettings.shadowResolution = (ShadowResolution)3;
			break;
		}
	}

	public void UpdateShadowDistance()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ShadowDistance))
		{
		case 0:
			shadowDistance = 5f;
			break;
		case 1:
			shadowDistance = 10f;
			break;
		case 2:
			shadowDistance = 15f;
			break;
		case 3:
			shadowDistance = 20f;
			break;
		case 4:
			shadowDistance = 25f;
			break;
		}
		QualitySettings.shadowDistance = shadowDistance;
	}

	public void UpdateMotionBlur()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.MotionBlur) == 1)
		{
			((PostProcessEffectSettings)PostProcessing.Instance.motionBlur).active = true;
		}
		else
		{
			((PostProcessEffectSettings)PostProcessing.Instance.motionBlur).active = false;
		}
	}

	public void UpdateLensDistortion()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.LensEffect) == 1)
		{
			((PostProcessEffectSettings)PostProcessing.Instance.lensDistortion).active = true;
		}
		else
		{
			((PostProcessEffectSettings)PostProcessing.Instance.lensDistortion).active = false;
		}
	}

	public void UpdateBloom()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
		{
			((PostProcessEffectSettings)PostProcessing.Instance.bloom).active = true;
		}
		else
		{
			((PostProcessEffectSettings)PostProcessing.Instance.bloom).active = false;
		}
	}

	public void UpdateChromaticAberration()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.ChromaticAberration) == 1)
		{
			((PostProcessEffectSettings)PostProcessing.Instance.chromaticAberration).active = true;
		}
		else
		{
			((PostProcessEffectSettings)PostProcessing.Instance.chromaticAberration).active = false;
		}
	}

	public void UpdateGrain()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
		{
			((PostProcessEffectSettings)PostProcessing.Instance.grain).active = true;
		}
		else
		{
			((PostProcessEffectSettings)PostProcessing.Instance.grain).active = false;
		}
	}

	public void UpdateWindowMode(bool _setResolution)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.WindowMode))
		{
		case 0:
		{
			windowMode = (FullScreenMode)1;
			windowFullscreen = true;
			Resolution currentResolution = Screen.currentResolution;
			int width = ((Resolution)(ref currentResolution)).width;
			currentResolution = Screen.currentResolution;
			Screen.SetResolution(width, ((Resolution)(ref currentResolution)).height, windowFullscreen);
			break;
		}
		case 1:
		{
			windowMode = (FullScreenMode)3;
			windowFullscreen = false;
			if (!_setResolution)
			{
				break;
			}
			List<Resolution> list = new List<Resolution>();
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution item = resolutions[i];
				if ((float)((Resolution)(ref item)).width / (float)((Resolution)(ref item)).height == 1.7777778f)
				{
					list.Add(item);
				}
			}
			Resolution val = Screen.resolutions[Screen.resolutions.Length - 1];
			if (list.Count > 0)
			{
				val = list[list.Count / 2];
			}
			Screen.SetResolution(((Resolution)(ref val)).width, ((Resolution)(ref val)).height, windowFullscreen);
			break;
		}
		}
		fullscreenCheckTimer = 1f;
	}

	public void UpdateRenderSize()
	{
		switch (DataDirector.instance.SettingValueFetch(DataDirector.Setting.RenderSize))
		{
		case 0:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthLarge;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightLarge;
			break;
		case 1:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthMedium;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightMedium;
			break;
		case 2:
			RenderTextureMain.instance.textureWidthOriginal = RenderTextureMain.instance.textureWidthSmall;
			RenderTextureMain.instance.textureHeightOriginal = RenderTextureMain.instance.textureHeightSmall;
			break;
		}
		RenderTextureMain.instance.ResetResolution();
	}

	public void UpdateGlitchLoop()
	{
		if (DataDirector.instance.SettingValueFetch(DataDirector.Setting.GlitchLoop) == 1)
		{
			glitchLoop = true;
		}
		else
		{
			glitchLoop = false;
		}
	}

	public void UpdateGamma()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		gamma = DataDirector.instance.SettingValueFetch(DataDirector.Setting.Gamma);
		((ParameterOverride<Vector4>)(object)PostProcessing.Instance.colorGrading.gamma).value = new Vector4(0f, 0f, 0f, gammaCurve.Evaluate(gamma / 100f));
	}
}
