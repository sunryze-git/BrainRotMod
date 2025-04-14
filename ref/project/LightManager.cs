using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightManager : MonoBehaviour
{
	[HideInInspector]
	public Transform lightCullTarget;

	public float checkDistance = 5f;

	public float fadeTimeMin = 1f;

	public float fadeTimeMax = 1f;

	public AnimationCurve fadeCurve;

	public AnimationCurve fadeCullCurve;

	public static LightManager instance;

	internal List<PropLight> propLights = new List<PropLight>();

	private List<PropLightEmission> propEmissions = new List<PropLightEmission>();

	private Vector3 lastCheckPos;

	private float lastYRotation;

	internal int activeLightsAmount;

	internal bool updateInstant;

	internal float updateInstantTimer;

	[Space]
	[Header("Sounds")]
	public Sound lampFlickerSound;

	private bool turnOffLights;

	private bool turningOffLights;

	private bool turningOffEmissions;

	private float logicUpdateTimer;

	private bool setup;

	private bool debugActive;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		debugActive = SemiFunc.DebugDev();
	}

	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main || !Object.op_Implicit((Object)(object)PlayerAvatar.instance))
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)lightCullTarget))
		{
			lightCullTarget = ((Component)PlayerAvatar.instance).transform;
		}
		LogicUpdate();
		if (debugActive)
		{
			int num = 0;
			foreach (PropLight propLight in propLights)
			{
				if (Object.op_Implicit((Object)(object)propLight) && ((Component)propLight).gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			activeLightsAmount = num;
		}
		if (updateInstant)
		{
			updateInstantTimer -= Time.deltaTime;
			if (updateInstantTimer <= 0f)
			{
				updateInstant = false;
			}
		}
		if (RoundDirector.instance.allExtractionPointsCompleted && !turnOffLights)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			turningOffLights = true;
			((MonoBehaviour)this).StartCoroutine(TurnOffLights());
			turningOffEmissions = true;
			((MonoBehaviour)this).StartCoroutine(TurnOffEmissions());
			turnOffLights = true;
		}
	}

	private IEnumerator TurnOffLights()
	{
		int _lightsPerFrame = 5;
		int _lightsPerFrameCounter = 0;
		foreach (PropLight item in propLights.ToList())
		{
			if (Object.op_Implicit((Object)(object)item) && item.levelLight)
			{
				item.lightComponent.intensity = 0f;
				item.originalIntensity = 0f;
				if (item.hasHalo)
				{
					item.halo.enabled = false;
				}
				item.turnedOff = true;
				_lightsPerFrameCounter++;
				if (_lightsPerFrameCounter >= _lightsPerFrame)
				{
					_lightsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		turningOffLights = false;
	}

	private IEnumerator TurnOffEmissions()
	{
		int _emissionsPerFrame = 5;
		int _emissionsPerFrameCounter = 0;
		foreach (PropLightEmission item in propEmissions.ToList())
		{
			if (Object.op_Implicit((Object)(object)item) && item.levelLight)
			{
				item.material.SetColor("_EmissionColor", Color.black);
				item.originalEmission = Color.black;
				item.turnedOff = true;
				_emissionsPerFrameCounter++;
				if (_emissionsPerFrameCounter >= _emissionsPerFrame)
				{
					_emissionsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		turningOffEmissions = false;
	}

	private void Setup()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		setup = true;
		if (Object.op_Implicit((Object)(object)lightCullTarget))
		{
			lastCheckPos = lightCullTarget.position;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Prop Lights");
		foreach (GameObject val in array)
		{
			if (Object.op_Implicit((Object)(object)val))
			{
				PropLight component = val.GetComponent<PropLight>();
				if (Object.op_Implicit((Object)(object)component))
				{
					propLights.Add(component);
				}
				else
				{
					Debug.LogError((object)("PropLight component not found in " + ((Object)val).name), (Object)(object)val);
				}
			}
		}
		array = GameObject.FindGameObjectsWithTag("Prop Emission");
		foreach (GameObject val2 in array)
		{
			if (Object.op_Implicit((Object)(object)val2))
			{
				PropLightEmission component2 = val2.GetComponent<PropLightEmission>();
				if (Object.op_Implicit((Object)(object)component2))
				{
					propEmissions.Add(component2);
				}
				else
				{
					Debug.LogError((object)("PropLightEmission component not found in " + ((Object)val2).name), (Object)(object)val2);
				}
			}
		}
		foreach (PropLight propLight in propLights)
		{
			if (!propLight.turnedOff)
			{
				HandleLightActivation(propLight);
			}
		}
		foreach (PropLightEmission propEmission in propEmissions)
		{
			if (!propEmission.turnedOff)
			{
				HandleEmissionActivation(propEmission);
			}
		}
	}

	private void LogicUpdate()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!setup)
		{
			Setup();
		}
		if (logicUpdateTimer > 0f)
		{
			logicUpdateTimer -= Time.deltaTime;
		}
		else if (Object.op_Implicit((Object)(object)lightCullTarget))
		{
			bool flag = false;
			if (Mathf.Abs(lightCullTarget.eulerAngles.y - lastYRotation) >= 20f)
			{
				lastYRotation = lightCullTarget.eulerAngles.y;
				flag = true;
			}
			if (!turningOffLights && !turningOffEmissions && (Vector3.Distance(lastCheckPos, lightCullTarget.position) >= checkDistance || flag))
			{
				logicUpdateTimer = 0.5f;
				UpdateLights();
			}
		}
	}

	private void UpdateLights()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		lastCheckPos = lightCullTarget.position;
		List<PropLight> list = new List<PropLight>();
		foreach (PropLight propLight in propLights)
		{
			if (Object.op_Implicit((Object)(object)propLight))
			{
				HandleLightActivation(propLight);
			}
			else
			{
				list.Add(propLight);
			}
		}
		foreach (PropLight item in list)
		{
			propLights.Remove(item);
		}
		List<PropLightEmission> list2 = new List<PropLightEmission>();
		foreach (PropLightEmission propEmission in propEmissions)
		{
			if (Object.op_Implicit((Object)(object)propEmission))
			{
				HandleEmissionActivation(propEmission);
			}
			else
			{
				list2.Add(propEmission);
			}
		}
		foreach (PropLightEmission item2 in list2)
		{
			propEmissions.Remove(item2);
		}
	}

	public void RemoveLight(PropLight PropLight)
	{
		if (Object.op_Implicit((Object)(object)PropLight) && propLights.Contains(PropLight))
		{
			propLights.Remove(PropLight);
		}
	}

	private void HandleLightActivation(PropLight propLight)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)lightCullTarget))
		{
			return;
		}
		Vector3 position = ((Component)propLight).transform.position;
		Vector3 position2 = lightCullTarget.position;
		bool flag = Vector3.Dot(((Component)propLight).transform.position - lightCullTarget.position, lightCullTarget.forward) <= -0.25f;
		if (Object.op_Implicit((Object)(object)SpectateCamera.instance))
		{
			flag = false;
			if (SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				position.y = 0f;
				position2.y = 0f;
			}
		}
		float num = Vector3.Distance(position, position2);
		float num2 = GraphicsManager.instance.lightDistance * propLight.lightRangeMultiplier;
		if (((Component)propLight).gameObject.activeInHierarchy && ((num >= num2 && !flag) || (num >= num2 * 0.8f && flag)))
		{
			((MonoBehaviour)this).StartCoroutine(FadeLightIntensity(propLight, 0f, Random.Range(fadeTimeMin, fadeTimeMax), delegate
			{
				((Component)propLight).gameObject.SetActive(false);
			}));
		}
		else if (!((Component)propLight).gameObject.activeInHierarchy && num < num2)
		{
			((Component)propLight).gameObject.SetActive(true);
			propLight.lightComponent.intensity = 0f;
			((MonoBehaviour)this).StartCoroutine(FadeLightIntensity(propLight, propLight.originalIntensity, Random.Range(fadeTimeMin, fadeTimeMax)));
		}
	}

	public void UpdateInstant()
	{
		if (setup)
		{
			updateInstant = true;
			updateInstantTimer = 0.1f;
			UpdateLights();
		}
	}

	private void HandleEmissionActivation(PropLightEmission _propLightEmission)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)lightCullTarget))
		{
			Vector3 position = ((Component)_propLightEmission).transform.position;
			Vector3 position2 = lightCullTarget.position;
			if (Object.op_Implicit((Object)(object)SpectateCamera.instance) && SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				position.y = 0f;
				position2.y = 0f;
			}
			if (Vector3.Distance(position, position2) >= GraphicsManager.instance.lightDistance)
			{
				((MonoBehaviour)this).StartCoroutine(FadeEmissionIntensity(_propLightEmission, Color.black, Random.Range(fadeTimeMin, fadeTimeMax)));
			}
			else
			{
				((MonoBehaviour)this).StartCoroutine(FadeEmissionIntensity(_propLightEmission, _propLightEmission.originalEmission, Random.Range(fadeTimeMin, fadeTimeMax)));
			}
		}
	}

	private IEnumerator FadeLightIntensity(PropLight propLight, float targetIntensity, float duration, Action onComplete = null)
	{
		if (!Object.op_Implicit((Object)(object)propLight) || !Object.op_Implicit((Object)(object)propLight.lightComponent))
		{
			yield break;
		}
		float startTime = Time.time;
		float startIntensity = propLight.lightComponent.intensity;
		while (Time.time - startTime < duration && !updateInstant)
		{
			if (!Object.op_Implicit((Object)(object)propLight) || !Object.op_Implicit((Object)(object)propLight.lightComponent))
			{
				yield break;
			}
			float num = (Time.time - startTime) / duration;
			propLight.lightComponent.intensity = Mathf.Lerp(startIntensity, targetIntensity, fadeCullCurve.Evaluate(num));
			yield return null;
		}
		if (Object.op_Implicit((Object)(object)propLight) && Object.op_Implicit((Object)(object)propLight.lightComponent))
		{
			propLight.lightComponent.intensity = targetIntensity;
			if (Mathf.Approximately(targetIntensity, 0f) && ((Component)propLight).gameObject.CompareTag("Prop Lights"))
			{
				((Component)propLight).gameObject.SetActive(false);
			}
			onComplete?.Invoke();
		}
	}

	private IEnumerator FadeEmissionIntensity(PropLightEmission _propLightEmission, Color targetColor, float duration)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)_propLightEmission))
		{
			yield break;
		}
		float startTime = Time.time;
		Color startColor = _propLightEmission.material.GetColor("_EmissionColor");
		while (Time.time - startTime < duration && !updateInstant)
		{
			if (!Object.op_Implicit((Object)(object)_propLightEmission))
			{
				yield break;
			}
			float num = (Time.time - startTime) / duration;
			_propLightEmission.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, fadeCullCurve.Evaluate(num)));
			yield return null;
		}
		if (Object.op_Implicit((Object)(object)_propLightEmission))
		{
			_propLightEmission.material.SetColor("_EmissionColor", targetColor);
		}
	}
}
