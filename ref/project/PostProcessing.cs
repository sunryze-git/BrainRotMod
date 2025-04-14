using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
	public static PostProcessing Instance;

	private bool setupDone;

	public PostProcessVolume volume;

	internal Grain grain;

	private float grainDisableTimer;

	internal Bloom bloom;

	private float bloomDisableTimer;

	internal ColorGrading colorGrading;

	private float colorGradingSaturation;

	private float colorGradingContrast;

	internal Vignette vignette;

	private Color vignetteColor;

	private float vignetteIntensity;

	private float vignetteSmoothness;

	internal MotionBlur motionBlur;

	internal LensDistortion lensDistortion;

	internal ChromaticAberration chromaticAberration;

	public AnimationCurve introCurve;

	public float introSpeed;

	private float introLerp;

	private float motionBlurDefault;

	private float bloomDefault;

	private float grainIntensityDefault;

	private float grainSizeDefault;

	[Space]
	private bool vignetteOverrideActive;

	private float vignetteOverrideLerp;

	private float vignetteOverrideTimer;

	private float vignetteOverrideSpeedIn;

	private float vignetteOverrideSpeedOut;

	private Color vignetteOverrideColor;

	private float vignetteOverrideIntensity;

	private float vignetteOverrideSmoothness;

	private GameObject vignetteOverrideObject;

	private bool saturationOverrideActive;

	private float saturationOverrideLerp;

	private float saturationOverrideTimer;

	private float saturationOverrideSpeedIn;

	private float saturationOverrideSpeedOut;

	private float saturationOverrideAmount;

	private GameObject saturationOverrideObject;

	private bool contrastOverrideActive;

	private float contrastOverrideLerp;

	private float contrastOverrideTimer;

	private float contrastOverrideSpeedIn;

	private float contrastOverrideSpeedOut;

	private float contrastOverrideAmount;

	private GameObject contrastOverrideObject;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		volume.profile.TryGetSettings<Grain>(ref grain);
		grainSizeDefault = ((ParameterOverride<float>)(object)grain.size).value;
		grainIntensityDefault = ((ParameterOverride<float>)(object)grain.intensity).value;
		((ParameterOverride<float>)(object)grain.intensity).value = 1f;
		volume.profile.TryGetSettings<MotionBlur>(ref motionBlur);
		motionBlurDefault = ((ParameterOverride<float>)(object)motionBlur.shutterAngle).value;
		volume.profile.TryGetSettings<LensDistortion>(ref lensDistortion);
		volume.profile.TryGetSettings<Bloom>(ref bloom);
		volume.profile.TryGetSettings<ColorGrading>(ref colorGrading);
		volume.profile.TryGetSettings<Vignette>(ref vignette);
		volume.profile.TryGetSettings<ChromaticAberration>(ref chromaticAberration);
	}

	private void Update()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!setupDone)
		{
			return;
		}
		Color val = vignetteColor;
		float num = vignetteIntensity;
		float num2 = vignetteSmoothness;
		if (vignetteOverrideActive)
		{
			if (vignetteOverrideTimer > 0f)
			{
				val = Color.Lerp(val, vignetteOverrideColor, vignetteOverrideLerp);
				num = Mathf.Lerp(num, vignetteOverrideIntensity, vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, vignetteOverrideSmoothness, vignetteOverrideLerp);
				vignetteOverrideLerp += vignetteOverrideSpeedIn * Time.deltaTime;
				vignetteOverrideLerp = Mathf.Clamp01(vignetteOverrideLerp);
				vignetteOverrideTimer -= Time.deltaTime;
			}
			else
			{
				val = Color.Lerp(val, vignetteOverrideColor, vignetteOverrideLerp);
				num = Mathf.Lerp(num, vignetteOverrideIntensity, vignetteOverrideLerp);
				num2 = Mathf.Lerp(num2, vignetteOverrideSmoothness, vignetteOverrideLerp);
				vignetteOverrideLerp -= vignetteOverrideSpeedOut * Time.deltaTime;
				if (vignetteOverrideLerp <= 0f)
				{
					vignetteOverrideActive = false;
					vignetteOverrideLerp = 0f;
				}
			}
		}
		((ParameterOverride<Color>)(object)vignette.color).value = val;
		((ParameterOverride<float>)(object)vignette.intensity).value = num;
		((ParameterOverride<float>)(object)vignette.smoothness).value = num2;
		if (saturationOverrideActive)
		{
			if (saturationOverrideTimer > 0f)
			{
				((ParameterOverride<float>)(object)colorGrading.saturation).value = Mathf.Lerp(colorGradingSaturation, saturationOverrideAmount, saturationOverrideLerp);
				saturationOverrideLerp += saturationOverrideSpeedIn * Time.deltaTime;
				saturationOverrideLerp = Mathf.Clamp01(saturationOverrideLerp);
				saturationOverrideTimer -= Time.deltaTime;
			}
			else
			{
				((ParameterOverride<float>)(object)colorGrading.saturation).value = Mathf.Lerp(colorGradingSaturation, saturationOverrideAmount, saturationOverrideLerp);
				saturationOverrideLerp -= saturationOverrideSpeedOut * Time.deltaTime;
				if (saturationOverrideLerp <= 0f)
				{
					((ParameterOverride<float>)(object)colorGrading.saturation).value = colorGradingSaturation;
					saturationOverrideActive = false;
					saturationOverrideLerp = 0f;
				}
			}
		}
		if (contrastOverrideActive)
		{
			if (contrastOverrideTimer > 0f)
			{
				((ParameterOverride<float>)(object)colorGrading.contrast).value = Mathf.Lerp(colorGradingContrast, contrastOverrideAmount, contrastOverrideLerp);
				contrastOverrideLerp += contrastOverrideSpeedIn * Time.deltaTime;
				contrastOverrideLerp = Mathf.Clamp01(contrastOverrideLerp);
				contrastOverrideTimer -= Time.deltaTime;
			}
			else
			{
				((ParameterOverride<float>)(object)colorGrading.contrast).value = Mathf.Lerp(colorGradingContrast, contrastOverrideAmount, contrastOverrideLerp);
				contrastOverrideLerp -= contrastOverrideSpeedOut * Time.deltaTime;
				if (contrastOverrideLerp <= 0f)
				{
					((ParameterOverride<float>)(object)colorGrading.contrast).value = colorGradingContrast;
					contrastOverrideActive = false;
					contrastOverrideLerp = 0f;
				}
			}
		}
		if (bloomDisableTimer > 0f)
		{
			bloomDisableTimer -= Time.deltaTime;
			if (bloomDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Bloom) == 1)
			{
				((PostProcessEffectSettings)bloom).active = true;
			}
		}
		if (grainDisableTimer > 0f)
		{
			grainDisableTimer -= Time.deltaTime;
			if (grainDisableTimer <= 0f && DataDirector.instance.SettingValueFetch(DataDirector.Setting.Grain) == 1)
			{
				((PostProcessEffectSettings)grain).active = true;
			}
		}
	}

	public void SpectateSet()
	{
		((ParameterOverride<float>)(object)motionBlur.shutterAngle).value = 1f;
	}

	public void SpectateReset()
	{
		((ParameterOverride<float>)(object)motionBlur.shutterAngle).value = motionBlurDefault;
	}

	public void Setup()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		((ParameterOverride<float>)(object)colorGrading.temperature).value = LevelGenerator.Instance.Level.ColorTemperature;
		((ParameterOverride<Color>)(object)colorGrading.colorFilter).value = LevelGenerator.Instance.Level.ColorFilter;
		colorGradingSaturation = ((ParameterOverride<float>)(object)colorGrading.saturation).value;
		colorGradingContrast = ((ParameterOverride<float>)(object)colorGrading.contrast).value;
		((ParameterOverride<float>)(object)bloom.intensity).value = LevelGenerator.Instance.Level.BloomIntensity;
		((ParameterOverride<float>)(object)bloom.threshold).value = LevelGenerator.Instance.Level.BloomThreshold;
		((ParameterOverride<Color>)(object)vignette.color).value = LevelGenerator.Instance.Level.VignetteColor;
		vignetteColor = ((ParameterOverride<Color>)(object)vignette.color).value;
		((ParameterOverride<float>)(object)vignette.intensity).value = LevelGenerator.Instance.Level.VignetteIntensity;
		vignetteIntensity = ((ParameterOverride<float>)(object)vignette.intensity).value;
		((ParameterOverride<float>)(object)vignette.smoothness).value = LevelGenerator.Instance.Level.VignetteSmoothness;
		vignetteSmoothness = ((ParameterOverride<float>)(object)vignette.smoothness).value;
		((MonoBehaviour)this).StartCoroutine(Intro());
		setupDone = true;
	}

	private IEnumerator Intro()
	{
		while (GameDirector.instance.currentState < GameDirector.gameState.Main)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		while (introLerp < 1f)
		{
			((ParameterOverride<float>)(object)grain.intensity).value = Mathf.Lerp(0.8f, grainIntensityDefault, introCurve.Evaluate(introLerp));
			((ParameterOverride<float>)(object)grain.size).value = Mathf.Lerp(1.5f, grainSizeDefault, introCurve.Evaluate(introLerp));
			introLerp += introSpeed * Time.deltaTime;
			yield return null;
		}
	}

	public void VignetteOverride(Color _color, float _intensity, float _smoothness, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!vignetteOverrideActive || !((Object)(object)_obj != (Object)(object)vignetteOverrideObject))
		{
			_smoothness = Mathf.Clamp01(_smoothness);
			vignetteOverrideActive = true;
			vignetteOverrideObject = _obj;
			vignetteOverrideTimer = _time;
			vignetteOverrideSpeedIn = _speedIn;
			vignetteOverrideSpeedOut = _speedOut;
			vignetteOverrideColor = _color;
			vignetteOverrideIntensity = _intensity;
			vignetteOverrideSmoothness = _smoothness;
		}
	}

	public void SaturationOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (!saturationOverrideActive || !((Object)(object)_obj != (Object)(object)saturationOverrideObject))
		{
			saturationOverrideActive = true;
			saturationOverrideObject = _obj;
			saturationOverrideTimer = _time;
			saturationOverrideSpeedIn = _speedIn;
			saturationOverrideSpeedOut = _speedOut;
			saturationOverrideAmount = _amount;
		}
	}

	public void ContrastOverride(float _amount, float _speedIn, float _speedOut, float _time, GameObject _obj)
	{
		if (!contrastOverrideActive || !((Object)(object)_obj != (Object)(object)contrastOverrideObject))
		{
			contrastOverrideActive = true;
			contrastOverrideObject = _obj;
			contrastOverrideTimer = _time;
			contrastOverrideSpeedIn = _speedIn;
			contrastOverrideSpeedOut = _speedOut;
			contrastOverrideAmount = _amount;
		}
	}

	public void BloomDisable(float _time)
	{
		bloomDisableTimer = _time;
		((PostProcessEffectSettings)bloom).active = false;
	}

	public void GrainDisable(float _time)
	{
		grainDisableTimer = _time;
		((PostProcessEffectSettings)grain).active = false;
	}
}
