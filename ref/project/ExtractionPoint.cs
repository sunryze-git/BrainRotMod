using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtractionPoint : MonoBehaviour
{
	public enum State
	{
		None,
		Idle,
		Active,
		Success,
		Warning,
		Cancel,
		Extracting,
		Complete,
		Surplus,
		TaxReturn
	}

	public Sound soundButton;

	public Sound soundActivate1;

	public Sound soundActivate2;

	public Sound soundActivate3;

	public Sound soundAlarm;

	public Sound soundAlarmGlobal;

	public Sound soundAlarmFinal;

	public Sound soundCancel;

	public Sound soundEmojiGlitch;

	public Sound soundGreenLights;

	public Sound soundLightsOn;

	public Sound soundHaulIncrease;

	public Sound soundHaulDecrease;

	public Sound soundWarningLightsLoop;

	public Sound soundSuccess;

	public Sound soundSuckEnd;

	public Sound soundSuckLoop;

	public Sound soundTubeBuildup;

	public Sound soundTubeSlam;

	public Sound soundTubeSlamGlobal;

	public Sound soundTubeRaise;

	public Sound soundTubeRaiseGlobal;

	public Sound soundTubeRetract;

	public Sound soundTubeHitCeiling;

	public Sound soundTubeHitCeilingGlobal;

	public Sound jingleLocal;

	public Sound jingleGlobal;

	public Sound surplusStateStart;

	public Sound surplusStateIncreaseLoop;

	public Sound surplusStateDoneLevel1;

	public Sound surplusStateDoneLevel2;

	public Sound surplusStateDoneLevel3;

	public Sound surplusStateDoneLevel4;

	public Sound surplusDeductionStart;

	public Sound surplusDeductionLoop;

	public Sound surplusDeductionEnd;

	public Sound completeJingleLocal;

	public Sound completeJingleGlobal;

	public Sound soundPing;

	public Transform soundPingTransform;

	public Sound surplusLightOutroSound;

	public Transform safetySpawn;

	public Transform haulBar;

	private float haulBarTargetScale;

	private bool stateStart = true;

	public TextMeshPro emojiScreen;

	public TextMeshPro haulGoalScreen;

	public TextMeshPro tubeScreenText;

	public Light tubeScreenLight;

	public Light spotlight1;

	public Light spotlight2;

	public Light emojiLight;

	private float tubeScreenChangeTimer;

	private string tubeScreenTextString = "";

	private Color tubeScreenTextColor = Color.white;

	[Space]
	public Light buttonLight;

	public MeshRenderer button;

	public Material buttonOff;

	public StaticGrabObject buttonGrabObject;

	public Material buttonDenyMaterial;

	private bool buttonActive;

	private float buttonDenyCooldown;

	public Transform buttonDenyTransform;

	public AnimationCurve buttonDenyCurve;

	private bool buttonDenyActive;

	private float buttonDenyLerp;

	[Space]
	public Material spotlightOff;

	public Material spotlightOn;

	public Transform extractionTube;

	public Transform spotlightHead1;

	public Transform spotlightHead2;

	private Color spotLightColor;

	private float stateTimer;

	private bool stateEnd;

	public AnimationCurve tubeSlamDown;

	public AnimationCurve buttonPressAnimationCurve;

	private float tubeSlamDownEval;

	private Vector3 tubeStartPosition;

	private bool thirtyFPSUpdate;

	private float thirtyFPSUpdateTimer;

	public Transform platform;

	public Transform ramp;

	private Vector3 rampStartPosition;

	[Space]
	public GameObject hurtColliders;

	public GameObject hurtColliderMain;

	private float hurtColliderMainTimer;

	[Space]
	public GameObject tubeHitParticles;

	private bool tubeHit;

	public ParticleSystem suckParticles;

	public ParticleSystem upParticles;

	public ParticleSystem ceilingParticles;

	private PhotonView photonView;

	public GameObject roomVolume;

	public GameObject emojiScreenGlitch;

	private int amountOfValuables;

	private float suckUpVariableTimer;

	private float suckUpTimeLeft;

	private float haulUpdateEffectTimer;

	private int haulPrevious;

	private int haulCurrent;

	private bool deductedFromHaul;

	private Color originalHaulColor;

	private bool resetHaulText;

	private bool settingState;

	private float spotlight1Delay;

	private float spotlight2Delay;

	private float emojiDelay;

	private float successDelay;

	[Space]
	public Transform surplusSpawnTransform;

	public Light surplusLight;

	public AnimationCurve surplusLightOutro;

	private bool surplusLightActive;

	private float surplusLightIntensity;

	private float surplusLightRange;

	private float surplusLightTimer = 5f;

	private float surplusLightLerp;

	private int haulSurplus;

	private int haulSurplusAnimated;

	private bool haulSurplusAnimatedDone;

	private int surplusLevel;

	private bool surplusIntroText;

	[HideInInspector]
	public int haulGoal;

	private bool cancelExtraction;

	private Vector3 tubeCancelPosition;

	private bool cancelTube;

	private Quaternion spotlight1StartRotation;

	private Quaternion spotlight2StartRotation;

	private Quaternion spotlight1CancelRotation;

	private Quaternion spotlight2CancelRotation;

	private bool cancelSpotlights;

	private float cancelSpotlightEval;

	private float spotlightIntensity;

	private float spotLightRange;

	private float emojiLightIntensity;

	private Color originalEmojiLightColor;

	private float emojiScreenGlitchTimer;

	private string prevEmoji;

	private string currentEmoji;

	private float buttonDelay;

	private float buttonPressEval;

	private bool buttonPressed;

	private Vector3 buttonOriginalPosition;

	private bool tubeHitCeiling;

	private bool haulGoalFetched;

	[HideInInspector]
	public bool isLocked;

	private float suckInRampEval;

	private Material buttonOriginalMaterial;

	private bool isShop;

	private bool taxReturn;

	private bool inStartRoom;

	[Space]
	public Transform shopStation;

	public Transform shopButton;

	private float shopButtonAnimationEval;

	private bool shopButtonAnimation;

	private Vector3 shopButtonOriginalPosition;

	private float initialStateTime;

	private float textBlinkTime;

	private Color textBlinkColor = Color.white;

	private Color textBlinkColorOriginal = Color.white;

	private int extractionHaul;

	private int runCurrencyBefore;

	private State stateSetTo;

	private float soundPingTimer;

	[Space]
	public DirtFinderMapFloor[] mapActive;

	public DirtFinderMapFloor[] mapUsed;

	public DirtFinderMapFloor[] mapInactive;

	internal State currentState = State.Idle;

	private bool isThief;

	private bool isCompletedRightAway;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		shopButtonOriginalPosition = shopButton.localPosition;
		buttonOriginalMaterial = ((Renderer)button).material;
		((Behaviour)spotlight1).enabled = false;
		((Behaviour)spotlight2).enabled = false;
		((Behaviour)emojiLight).enabled = false;
		((Behaviour)emojiScreen).enabled = false;
		((Behaviour)haulGoalScreen).enabled = false;
		spotLightColor = spotlight1.color;
		tubeStartPosition = extractionTube.localPosition;
		rampStartPosition = ramp.localPosition;
		photonView = ((Component)this).GetComponent<PhotonView>();
		originalHaulColor = ((Graphic)haulGoalScreen).color;
		StateSet(State.Idle);
		extractionTube.localPosition = new Vector3(tubeStartPosition.x, 0f, tubeStartPosition.z);
		spotlight1StartRotation = spotlightHead1.rotation;
		spotlight2StartRotation = spotlightHead2.rotation;
		spotlightIntensity = spotlight1.intensity;
		spotLightRange = spotlight1.range;
		emojiLightIntensity = emojiLight.intensity;
		originalEmojiLightColor = emojiLight.color;
		prevEmoji = "Jannek farts on the moon!";
		buttonOriginalPosition = ((Component)button).transform.localPosition;
		RoundDirector.instance.extractionPoints++;
		RoundDirector.instance.extractionPointList.Add(((Component)this).gameObject);
		surplusLightIntensity = surplusLight.intensity;
		surplusLightRange = surplusLight.range;
		surplusLight.intensity = 0f;
		surplusLight.range = 0f;
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponentInParent<StartRoom>()))
		{
			inStartRoom = true;
		}
		((Component)platform).gameObject.SetActive(false);
		((MonoBehaviour)this).StartCoroutine(MapHideOnStart());
		isShop = SemiFunc.RunIsShop();
		if (!isShop)
		{
			Object.Destroy((Object)(object)((Component)shopStation).gameObject);
			return;
		}
		ShopManager.instance.isThief = false;
		ShopManager.instance.extractionPoint = ((Component)this).transform;
		RoundDirector.instance.extractionPointSurplus = 0;
	}

	private IEnumerator MapHideOnStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(1f);
		DirtFinderMapFloor[] array = mapActive;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MapObject.Hide();
		}
		array = mapUsed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].MapObject.Hide();
		}
	}

	public void ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck()
	{
		OnClick();
	}

	public void OnClick()
	{
		if (isLocked || !StateIs(State.Idle))
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsNotMasterClient())
			{
				RoundDirector.instance.RequestExtractionPointActivation(photonView.ViewID);
			}
			if (SemiFunc.IsMasterClient())
			{
				RoundDirector.instance.ExtractionPointActivate(photonView.ViewID);
			}
		}
		else
		{
			ButtonPress();
			RoundDirector.instance.extractionPointActive = true;
			RoundDirector.instance.extractionPointCurrent = this;
			RoundDirector.instance.ExtractionPointsLock(((Component)this).gameObject);
		}
	}

	public void ButtonPress()
	{
		if (StateIs(State.Idle))
		{
			StateSet(State.Active);
		}
	}

	private void SetLightsEmissionColor(Color color)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Material[] materials = ((Renderer)((Component)spotlightHead1).GetComponentInChildren<MeshRenderer>()).materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetColor("_EmissionColor", color);
		}
		materials = ((Renderer)((Component)spotlightHead2).GetComponentInChildren<MeshRenderer>()).materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetColor("_EmissionColor", color);
		}
	}

	private void TextBlink(Color textColorOriginal, Color textColor, float time)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		textBlinkTime = time;
		textBlinkColor = textColor;
		textBlinkColorOriginal = textColorOriginal;
	}

	private void TextBlinkLogic()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (textBlinkTime > 0f)
		{
			textBlinkTime -= Time.deltaTime;
			if (textBlinkTime <= 0f)
			{
				((Graphic)haulGoalScreen).color = textBlinkColorOriginal;
			}
			else
			{
				((Graphic)haulGoalScreen).color = textBlinkColor;
			}
		}
	}

	public void OnShopClick()
	{
		if (StateIs(State.Active))
		{
			if (haulGoal - haulCurrent >= 0 && haulGoal - haulCurrent != haulGoal)
			{
				StateSet(State.Success);
			}
			else
			{
				StateSet(State.Cancel);
			}
		}
	}

	private void ShopButtonAnimation()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (shopButtonAnimation)
		{
			shopButtonAnimationEval += Time.deltaTime * 2f;
			shopButtonAnimationEval = Mathf.Clamp01(shopButtonAnimationEval);
			float num = buttonPressAnimationCurve.Evaluate(shopButtonAnimationEval);
			Color val = default(Color);
			((Color)(ref val))._002Ector(1f, 0.5f, 0f, 1f);
			((Renderer)((Component)shopButton).GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", Color.Lerp(val, Color.white, num));
			num = Mathf.Clamp(num, 0.5f, 1f);
			shopButton.localScale = new Vector3(1f, num, 1f);
			if (shopButtonAnimationEval >= 1f)
			{
				shopButtonAnimation = false;
				shopButtonAnimationEval = 0f;
			}
		}
	}

	public void HitCeiling()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		ceilingParticles.Play();
		soundTubeHitCeiling.Play(extractionTube.position);
		soundTubeHitCeilingGlobal.Play(extractionTube.position);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, extractionTube.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, extractionTube.position, 0.1f);
	}

	private void EmojiScreenGlitch(Color color)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (emojiScreenGlitchTimer <= 0f)
		{
			soundEmojiGlitch.Play(emojiScreen.transform.position);
		}
		emojiScreenGlitchTimer = 0.2f;
		emojiScreenGlitch.SetActive(true);
		((Behaviour)emojiScreen).enabled = false;
		((Renderer)emojiScreenGlitch.GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", color);
	}

	private void HaulGoalSet(int value)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("HaulGoalSetRPC", (RpcTarget)0, new object[1] { value });
			}
		}
		else
		{
			HaulGoalSetRPC(value);
		}
	}

	[PunRPC]
	public void HaulGoalSetRPC(int value)
	{
		haulGoal = value;
		RoundDirector.instance.extractionHaulGoal = value;
		haulGoalFetched = true;
	}

	private void ResetLights()
	{
		spotlight1.range = spotLightRange;
		spotlight2.range = spotLightRange;
		spotlight1.intensity = spotlightIntensity;
		spotlight2.intensity = spotlightIntensity;
		emojiLight.intensity = emojiLightIntensity;
	}

	private void EmojiScreenGlitchLogic()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (emojiDelay > 0f)
		{
			return;
		}
		currentEmoji = ((TMP_Text)emojiScreen).text;
		if (prevEmoji != currentEmoji)
		{
			prevEmoji = currentEmoji;
			EmojiScreenGlitch(Color.yellow);
		}
		if (!(emojiScreenGlitchTimer <= 0f))
		{
			Vector2 textureOffset = ((Renderer)emojiScreenGlitch.GetComponent<MeshRenderer>()).material.GetTextureOffset("_MainTex");
			textureOffset.y += Time.deltaTime * 15f;
			((Renderer)emojiScreenGlitch.GetComponent<MeshRenderer>()).material.SetTextureOffset("_MainTex", textureOffset);
			emojiScreenGlitchTimer -= Time.deltaTime;
			if (thirtyFPSUpdate)
			{
				float num = Random.Range(0.1f, 1f);
				((Renderer)emojiScreenGlitch.GetComponent<MeshRenderer>()).material.SetTextureScale("_MainTex", new Vector2(num, num));
			}
			if (emojiScreenGlitchTimer <= 0f)
			{
				emojiScreenGlitch.SetActive(false);
				((Behaviour)emojiScreen).enabled = true;
			}
		}
	}

	private void HaulInternalStatsUpdate()
	{
		haulPrevious = haulCurrent;
		haulCurrent = RoundDirector.instance.currentHaul + RoundDirector.instance.extractionPointSurplus;
		if (isShop)
		{
			haulCurrent = SemiFunc.ShopGetTotalCost() * 1000;
			haulGoal = SemiFunc.StatGetRunCurrency() * 1000;
		}
	}

	private void HaulChecker()
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		HaulInternalStatsUpdate();
		if (haulPrevious != haulCurrent)
		{
			haulUpdateEffectTimer = 0.3f;
			if (haulCurrent > haulPrevious)
			{
				deductedFromHaul = false;
				soundHaulIncrease.Play(emojiScreen.transform.position);
			}
			else
			{
				deductedFromHaul = true;
				soundHaulDecrease.Play(emojiScreen.transform.position);
			}
			haulPrevious = haulCurrent;
		}
		if (haulUpdateEffectTimer > 0f)
		{
			haulUpdateEffectTimer -= Time.deltaTime;
			haulUpdateEffectTimer = Mathf.Max(0f, haulUpdateEffectTimer);
			Color color = Color.white;
			if (deductedFromHaul)
			{
				color = Color.red;
			}
			if (isShop)
			{
				color = Color.red;
				if (deductedFromHaul)
				{
					color = Color.white;
				}
			}
			((Graphic)haulGoalScreen).color = color;
			if (thirtyFPSUpdate)
			{
				((TMP_Text)haulGoalScreen).text = GlitchyText();
			}
			resetHaulText = false;
		}
		else if (!resetHaulText)
		{
			((Graphic)haulGoalScreen).color = originalHaulColor;
			SetHaulText();
			resetHaulText = true;
		}
		if (!isShop)
		{
			if (haulGoal - haulCurrent <= 0 && haulGoalFetched)
			{
				successDelay -= Time.deltaTime;
				if (successDelay <= 0f)
				{
					StateSet(State.Success);
				}
			}
			else
			{
				successDelay = 1.5f;
			}
		}
		UpdateEmojiText();
	}

	private void UpdateEmojiText()
	{
		if (haulCurrent == 0)
		{
			SetEmojiScreen("<sprite name=:'(>");
			if (isShop)
			{
				SetEmojiScreen("<sprite name=shoppingcart>");
				if (isThief)
				{
					SetEmojiScreen("<sprite name=thief>");
				}
			}
			return;
		}
		float num = (float)haulCurrent / (float)haulGoal;
		string[] array = new string[6] { "<sprite name=:'(>", "<sprite name=:(>", "<sprite name=mellow>", "<sprite name=:)>", "<sprite name=:D>", "<sprite name=cryinglaughing>" };
		if (isShop)
		{
			num = 0f;
			array = new string[1] { "<sprite name=shoppingcart>" };
			if (isThief)
			{
				array = new string[1] { "<sprite name=thief>" };
			}
		}
		int num2 = Mathf.FloorToInt(num * (float)(array.Length - 1));
		num2 = Mathf.Clamp(num2, 0, array.Length - 1);
		SetEmojiScreen(array[num2]);
	}

	private void ThirtyFPS()
	{
		if (thirtyFPSUpdateTimer > 0f)
		{
			thirtyFPSUpdateTimer -= Time.deltaTime;
			thirtyFPSUpdateTimer = Mathf.Max(0f, thirtyFPSUpdateTimer);
		}
		else
		{
			thirtyFPSUpdate = true;
			thirtyFPSUpdateTimer = 1f / 30f;
		}
	}

	private void Update()
	{
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		ShopButtonAnimation();
		bool playing = tubeHit && tubeSlamDownEval > 0.8f && StateIs(State.Extracting);
		soundSuckLoop.PlayLoop(playing, 2f, 2f);
		bool playing2 = StateIs(State.Warning);
		soundWarningLightsLoop.PlayLoop(playing2, 2f, 2f);
		bool playing3 = StateIs(State.Surplus) && !haulSurplusAnimatedDone && !surplusIntroText;
		surplusStateIncreaseLoop.PlayLoop(playing3, 2f, 2f);
		ThirtyFPS();
		HaulBarAnimateScale();
		StateCancel();
		StateIdle();
		StateActive();
		StateSuccess();
		StateSurplus();
		StateWarning();
		StateExtracting();
		StateComplete();
		StateTaxReturn();
		EmojiScreenGlitchLogic();
		TextBlinkLogic();
		SurplusLightLogic();
		TubeScreenTextChangeLogic();
		stateEnd = false;
		thirtyFPSUpdate = false;
		if (stateTimer > 0f)
		{
			if (initialStateTime == 0f)
			{
				initialStateTime = stateTimer;
			}
			stateTimer -= Time.deltaTime;
			stateTimer = Mathf.Max(0f, stateTimer);
		}
		else if (!stateEnd && stateTimer != -123f)
		{
			stateEnd = true;
			stateTimer = -123f;
			initialStateTime = 0f;
		}
		if (stateSetTo != 0)
		{
			currentState = stateSetTo;
			stateStart = true;
			settingState = false;
			stateEnd = false;
			stateSetTo = State.None;
		}
		if (isLocked && ((Behaviour)buttonGrabObject).enabled && buttonGrabObject.playerGrabbing.Count > 0 && buttonDenyCooldown <= 0f)
		{
			foreach (PhysGrabber item in buttonGrabObject.playerGrabbing.ToList())
			{
				if (!SemiFunc.IsMultiplayer())
				{
					item.ReleaseObject();
					continue;
				}
				item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 1.5f });
			}
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ButtonDenyRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				ButtonDenyRPC();
			}
		}
		if (buttonDenyCooldown > 0f)
		{
			buttonDenyCooldown -= Time.deltaTime;
			if (buttonDenyCooldown <= 0f)
			{
				((Behaviour)buttonLight).enabled = false;
				((Renderer)button).material = buttonOff;
				((Behaviour)buttonGrabObject).enabled = true;
			}
		}
		if (buttonDenyActive)
		{
			buttonDenyLerp += 0.75f * Time.deltaTime;
			buttonDenyLerp = Mathf.Clamp01(buttonDenyLerp);
			buttonDenyTransform.localPosition = new Vector3(0f, 0f, -0.06f * buttonDenyCurve.Evaluate(buttonDenyLerp));
			if (buttonDenyLerp >= 1f)
			{
				buttonDenyActive = false;
				buttonDenyLerp = 0f;
			}
		}
	}

	private void StateIdle()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Idle))
		{
			return;
		}
		if (stateStart)
		{
			stateStart = false;
		}
		if (isLocked)
		{
			if (tubeScreenTextString != "LOCKED")
			{
				TubeScreenTextChange("LOCKED", Color.red);
			}
			ButtonToggle(_active: false);
			return;
		}
		Color color = default(Color);
		((Color)(ref color))._002Ector(1f, 0.5f, 0f);
		if (tubeScreenTextString != "READY")
		{
			TubeScreenTextChange("READY", color);
		}
		if (soundPingTimer <= 0f && !SemiFunc.RunIsTutorial() && !SemiFunc.RunIsRecording())
		{
			soundPing.Play(soundPingTransform.position);
			soundPingTimer = 4f;
		}
		else
		{
			soundPingTimer -= Time.deltaTime;
		}
		ButtonToggle(_active: true);
	}

	private void StateActive()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Active))
		{
			return;
		}
		if (stateStart)
		{
			taxReturn = false;
			if (tubeScreenTextString != "ACTIVE")
			{
				TubeScreenTextChange("ACTIVE", Color.green);
			}
			DirtFinderMapFloor[] array = mapInactive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Hide();
			}
			array = mapActive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Show();
			}
			((Component)platform).gameObject.SetActive(true);
			((Behaviour)emojiLight).enabled = true;
			ButtonToggle(_active: false);
			((Behaviour)emojiScreen).enabled = true;
			((Behaviour)haulGoalScreen).enabled = true;
			((Behaviour)spotlight1).enabled = false;
			((Behaviour)spotlight2).enabled = false;
			((Behaviour)emojiLight).enabled = false;
			((Behaviour)emojiScreen).enabled = false;
			roomVolume.SetActive(true);
			tubeSlamDownEval = 0f;
			emojiLight.color = originalEmojiLightColor;
			emojiDelay = 2f;
			ResetLightIntensity();
			spotlight1Delay = 1f;
			spotlight2Delay = 1.5f;
			successDelay = 0f;
			tubeHitCeiling = false;
			ResetLights();
			if (cancelExtraction)
			{
				buttonPressed = false;
				tubeSlamDownEval = 1f;
				cancelExtraction = false;
				spotlight1Delay = 0f;
				spotlight2Delay = 0f;
				emojiDelay = 0f;
			}
			else
			{
				if (!isShop)
				{
					if (!inStartRoom)
					{
						SemiFunc.EnemyInvestigate(((Component)this).transform.position, 20f);
					}
					jingleLocal.Play(((Component)this).transform.position);
					jingleGlobal.Play(((Component)this).transform.position);
				}
				buttonDelay = 0.5f;
				buttonPressed = true;
				GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)button).transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)button).transform.position, 0.1f);
				soundButton.Play(((Component)button).transform.position);
				int value = RoundDirector.instance.haulGoal / RoundDirector.instance.extractionPoints;
				if (isShop)
				{
					value = SemiFunc.StatGetRunCurrency() * 1000;
				}
				HaulGoalSet(value);
			}
			((Graphic)haulGoalScreen).color = originalHaulColor;
			HaulInternalStatsUpdate();
			SetHaulText();
			currentEmoji = ((TMP_Text)emojiScreen).text;
			stateStart = false;
		}
		if (!tubeHitCeiling && buttonPressed && tubeSlamDownEval > 0.8f)
		{
			tubeHitCeiling = true;
			HitCeiling();
		}
		if (buttonPressed)
		{
			if (buttonPressEval < 1f)
			{
				buttonPressEval += 8f * Time.deltaTime;
				buttonPressEval = Mathf.Min(1f, buttonPressEval);
				float num = buttonPressAnimationCurve.Evaluate(buttonPressEval);
				((Component)button).transform.localPosition = new Vector3(buttonOriginalPosition.x, buttonOriginalPosition.y, buttonOriginalPosition.z - 0.1f * num);
			}
			if (emojiDelay > 0f && !isShop)
			{
				SemiFunc.UIBigMessage("EXTRACTION POINT ACTIVATED", "{!}", 25f, Color.white, Color.white);
				SemiFunc.UIFocusText("Fill the extraction point with valuables", Color.white, AssetManager.instance.colorYellow);
			}
		}
		if (buttonDelay >= 0f)
		{
			buttonDelay -= Time.deltaTime;
		}
		if (spotlight1Delay <= 0f)
		{
			if (!((Behaviour)spotlight1).enabled)
			{
				if (buttonPressed)
				{
					soundActivate1.Play(((Component)spotlight1).transform.position);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)spotlight1).transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)spotlight1).transform.position, 0.1f);
				}
				((Behaviour)spotlight1).enabled = true;
				spotlight1.color = spotLightColor;
			}
		}
		else if (buttonDelay <= 0f)
		{
			spotlight1Delay -= Time.deltaTime;
		}
		if (spotlight2Delay <= 0f)
		{
			if (!((Behaviour)spotlight2).enabled)
			{
				if (buttonPressed)
				{
					soundActivate2.Play(((Component)spotlight2).transform.position);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)spotlight2).transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)spotlight2).transform.position, 0.1f);
				}
				((Behaviour)spotlight2).enabled = true;
				spotlight2.color = spotLightColor;
			}
		}
		else if (buttonDelay <= 0f)
		{
			spotlight2Delay -= Time.deltaTime;
		}
		if (emojiDelay <= 0f)
		{
			if (!((Behaviour)emojiLight).enabled)
			{
				if (buttonPressed)
				{
					soundActivate3.Play(emojiScreen.transform.position);
					GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, emojiScreen.transform.position, 0.1f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, emojiScreen.transform.position, 0.1f);
				}
				((Behaviour)emojiLight).enabled = true;
				((Behaviour)emojiScreen).enabled = true;
				SetHaulText();
			}
		}
		else
		{
			if (buttonDelay <= 0f)
			{
				emojiDelay -= Time.deltaTime;
			}
			if (thirtyFPSUpdate)
			{
				((TMP_Text)haulGoalScreen).text = GlitchyText();
			}
		}
		if (tubeSlamDownEval < 1f && buttonDelay <= 0f)
		{
			if (tubeSlamDownEval == 0f)
			{
				tubeHitParticles.SetActive(true);
				upParticles.Play();
				soundTubeRaise.Play(((Component)this).transform.position);
				soundTubeRaiseGlobal.Play(((Component)this).transform.position);
				GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, extractionTube.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, extractionTube.position, 0.1f);
			}
			tubeSlamDownEval += 2f * Time.deltaTime;
			tubeSlamDownEval = Mathf.Min(1f, tubeSlamDownEval);
			float num2 = tubeSlamDown.Evaluate(tubeSlamDownEval);
			extractionTube.localPosition = new Vector3(tubeStartPosition.x, tubeStartPosition.y * num2, tubeStartPosition.z);
		}
		if (!isCompletedRightAway)
		{
			HaulChecker();
		}
	}

	private void HaulBarAnimateScale()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Lerp(haulBar.localScale.x, haulBarTargetScale, Time.deltaTime * 10f);
		num = Mathf.Clamp01(num);
		if (float.IsNaN(num))
		{
			num = 0f;
		}
		haulBar.localScale = new Vector3(num, 1f, 1f);
		haulBar.localScale = new Vector3(Mathf.Min(1f, haulBar.localScale.x), 1f, 1f);
	}

	private void SetHaulText()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!isShop)
		{
			string text = "<color=#bd4300>$</color>";
			((TMP_Text)haulGoalScreen).text = text + SemiFunc.DollarGetString(Mathf.Max(0, haulCurrent));
			haulBarTargetScale = (float)haulCurrent / (float)haulGoal;
			return;
		}
		string text2 = "<color=#bd4300>$</color>";
		((TMP_Text)haulGoalScreen).text = text2 + SemiFunc.DollarGetString(haulGoal - haulCurrent);
		if (haulGoal - haulCurrent < 0)
		{
			((TMP_Text)haulGoalScreen).text = SemiFunc.DollarGetString(haulGoal - haulCurrent);
			((Graphic)haulGoalScreen).color = Color.red;
		}
	}

	private void SetEmojiScreen(string emoji)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)emojiScreen).text = "<size=100>|</size>" + emoji + "<size=100>|</size>";
		((Graphic)emojiScreen).color = new Color(1f, 1f, 1f, 0f);
	}

	private void ShopButtonPushVisualsStart()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (isShop && !shopButtonAnimation)
		{
			shopButton.localScale = new Vector3(1f, 0.1f, 1f);
			soundButton.Play(shopButton.position);
			shopButtonAnimationEval = 0f;
			shopButtonAnimation = true;
		}
	}

	private void StateSuccess()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Success))
		{
			return;
		}
		if (stateStart)
		{
			ShopButtonPushVisualsStart();
			ResetLights();
			ResetLightIntensity();
			SetSpotlightColor(Color.green);
			tubeSlamDownEval = 0f;
			tubeHitParticles.SetActive(false);
			((TMP_Text)haulGoalScreen).text = "$" + SemiFunc.DollarGetString(Mathf.Max(0, RoundDirector.instance.haulGoal - RoundDirector.instance.currentHaul));
			stateStart = false;
			SetEmojiScreen("<sprite name=check>");
			emojiLight.color = Color.green;
			EmojiScreenGlitch(Color.green);
			emojiDelay = 0f;
			soundSuccess.Play(((Component)this).transform.position);
			soundGreenLights.Play(((Component)this).transform.position);
			((TMP_Text)haulGoalScreen).text = "!!!!!!!!!";
			((Graphic)haulGoalScreen).color = Color.green;
			stateTimer = 2f;
			if (isShop)
			{
				stateTimer = 1f;
			}
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
		}
		if (!isCompletedRightAway)
		{
			CancelExtraction();
		}
		if (!stateEnd)
		{
			return;
		}
		if (isShop)
		{
			StateSet(State.Warning);
			return;
		}
		haulSurplus = Mathf.Abs(RoundDirector.instance.currentHaul - haulGoal);
		if (haulSurplus > 0)
		{
			StateSet(State.Surplus);
		}
		else
		{
			StateSet(State.Warning);
		}
	}

	private void StateSurplus()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Surplus))
		{
			return;
		}
		if (stateStart)
		{
			if (RoundDirector.instance.extractionPointsCompleted != RoundDirector.instance.extractionPoints - 1)
			{
				taxReturn = true;
			}
			ResetLights();
			ResetLightIntensity();
			SetSpotlightColor(Color.green);
			SetEmojiScreen("<sprite name=surplus>");
			emojiLight.color = Color.green;
			EmojiScreenGlitch(Color.green);
			emojiDelay = 0f;
			tubeSlamDownEval = 0f;
			tubeHitParticles.SetActive(false);
			stateStart = false;
			surplusStateStart.Play(((Component)this).transform.position);
			soundGreenLights.Play(((Component)this).transform.position);
			((TMP_Text)haulGoalScreen).text = "TAX RETURN";
			((Graphic)haulGoalScreen).color = Color.green;
			haulSurplusAnimated = 0;
			haulSurplusAnimatedDone = false;
			surplusIntroText = true;
			cancelExtraction = true;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
			if (haulSurplus < 1000)
			{
				stateTimer = 4f;
				surplusLevel = 1;
			}
			else if (haulSurplus >= 1000 && haulSurplus < 5000)
			{
				stateTimer = 5f;
				surplusLevel = 2;
			}
			else if (haulSurplus >= 5000 && haulSurplus < 10000)
			{
				stateTimer = 6f;
				surplusLevel = 3;
			}
			else if (haulSurplus >= 10000 && haulSurplus < 20000)
			{
				stateTimer = 7f;
				surplusLevel = 4;
			}
			else if (haulSurplus >= 20000 && haulSurplus < 50000)
			{
				stateTimer = 8f;
				surplusLevel = 4;
			}
			else if (haulSurplus >= 50000)
			{
				stateTimer = 9f;
				surplusLevel = 4;
			}
		}
		if (!isCompletedRightAway)
		{
			CancelExtraction();
		}
		if (!haulSurplusAnimatedDone)
		{
			haulSurplus = Mathf.Abs(RoundDirector.instance.currentHaul - haulGoal);
		}
		float num = 1.5f;
		if (stateTimer < initialStateTime - num)
		{
			surplusIntroText = false;
			if (haulSurplusAnimated < haulSurplus && initialStateTime != 0f)
			{
				float num2 = 2f;
				float num3 = (initialStateTime - num - stateTimer) / (initialStateTime - num - num2);
				num3 = Mathf.Clamp01(num3);
				surplusStateIncreaseLoop.LoopPitch = 1f + num3;
				haulSurplusAnimated = (int)((float)haulSurplus * num3);
				((TMP_Text)haulGoalScreen).text = "+$" + SemiFunc.DollarGetString(haulSurplusAnimated);
			}
			else if (!haulSurplusAnimatedDone && initialStateTime != 0f)
			{
				haulSurplus = Mathf.Abs(haulCurrent - haulGoal);
				SetEmojiScreen("<sprite name=moneyeyes>");
				GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, haulGoalScreen.transform.position, 0.1f);
				haulSurplusAnimatedDone = true;
				TextBlink(Color.green, Color.white, 0.5f);
				if (surplusLevel == 1)
				{
					surplusStateDoneLevel1.Play(((Component)this).transform.position);
				}
				if (surplusLevel == 2)
				{
					surplusStateDoneLevel1.Play(((Component)this).transform.position);
				}
				if (surplusLevel == 3)
				{
					surplusStateDoneLevel1.Play(((Component)this).transform.position);
				}
				if (surplusLevel == 4)
				{
					surplusStateDoneLevel1.Play(((Component)this).transform.position);
				}
			}
		}
		if (stateEnd && !isCompletedRightAway)
		{
			StateSet(State.Warning);
		}
	}

	private void SpawnTaxReturn()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (RoundDirector.instance.extractionPointSurplus > 0)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				GameObject val = AssetManager.instance.surplusValuableSmall;
				if (RoundDirector.instance.extractionPointSurplus > 10000)
				{
					val = AssetManager.instance.surplusValuableBig;
				}
				else if (RoundDirector.instance.extractionPointSurplus > 5000)
				{
					val = AssetManager.instance.surplusValuableMedium;
				}
				GameObject val2 = null;
				val2 = (SemiFunc.IsMultiplayer() ? PhotonNetwork.InstantiateRoomObject("Valuables/" + ((Object)val).name, surplusSpawnTransform.position, Quaternion.identity, (byte)0, (object[])null) : Object.Instantiate<GameObject>(val, surplusSpawnTransform.position, Quaternion.identity));
				val2.GetComponent<ValuableObject>().dollarValueOverride = RoundDirector.instance.extractionPointSurplus;
				val2.GetComponent<PhysGrabObject>().spawnTorque = Random.insideUnitSphere * 0.05f;
			}
			surplusLightActive = true;
			surplusLight.intensity = surplusLightIntensity;
			surplusLight.range = surplusLightRange;
		}
		RoundDirector.instance.extractionPointSurplus = 0;
	}

	private bool CancelExtraction()
	{
		haulCurrent = RoundDirector.instance.currentHaul + RoundDirector.instance.extractionPointSurplus;
		if (isShop)
		{
			haulCurrent = SemiFunc.ShopGetTotalCost() * 1000;
		}
		if (!isShop)
		{
			if (haulGoal - haulCurrent > 0)
			{
				StateSet(State.Cancel);
				return true;
			}
		}
		else if (haulGoal - haulCurrent < 0 || haulGoal - haulCurrent == haulGoal)
		{
			StateSet(State.Cancel);
			return true;
		}
		return false;
	}

	private void ResetLightIntensity()
	{
		emojiLight.intensity = emojiLightIntensity;
		spotlight1.intensity = spotlightIntensity;
		spotlight2.intensity = spotlightIntensity;
	}

	private void StateWarning()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Warning))
		{
			return;
		}
		if (stateStart)
		{
			ResetLights();
			SetSpotlightColor(Color.red);
			ResetLightIntensity();
			spotlight1.intensity = spotlightIntensity * 2f;
			spotlight2.intensity = spotlightIntensity * 2f;
			Light obj = spotlight1;
			obj.range *= 2f;
			Light obj2 = spotlight2;
			obj2.range *= 2f;
			SetEmojiScreen("<sprite name=!>");
			emojiLight.color = Color.red;
			EmojiScreenGlitch(Color.red);
			stateTimer = 3f;
			stateStart = false;
			((TMP_Text)haulGoalScreen).text = "3";
			((Graphic)haulGoalScreen).color = Color.red;
			soundAlarm.Play(emojiScreen.transform.position);
			soundAlarmGlobal.Play(emojiScreen.transform.position);
			SemiFunc.EnemyInvestigate(((Component)this).transform.position, 20f);
		}
		if (stateTimer > 0f)
		{
			string text = Mathf.CeilToInt(stateTimer).ToString();
			if (((TMP_Text)haulGoalScreen).text != text)
			{
				soundAlarm.Play(emojiScreen.transform.position);
				soundAlarmGlobal.Play(emojiScreen.transform.position);
				((TMP_Text)haulGoalScreen).text = text;
			}
		}
		else if (thirtyFPSUpdate)
		{
			((TMP_Text)haulGoalScreen).text = GlitchyText();
		}
		spotlightHead1.Rotate(0f, 500f * Time.deltaTime, 0f);
		spotlightHead2.Rotate(0f, -500f * Time.deltaTime, 0f);
		if (!isCompletedRightAway)
		{
			CancelExtraction();
		}
		if (stateEnd)
		{
			StateSet(State.Extracting);
		}
	}

	private void SetSpotlightColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		spotlight1.color = color;
		spotlight2.color = color;
		SetLightsEmissionColor(color);
		spotlight1.intensity = spotlightIntensity;
		spotlight2.intensity = spotlightIntensity;
	}

	private void StateCancel()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Cancel))
		{
			return;
		}
		if (stateStart)
		{
			ShopButtonPushVisualsStart();
			taxReturn = false;
			SetEmojiScreen("<sprite name=X>");
			emojiLight.color = Color.red;
			emojiLight.intensity = emojiLightIntensity;
			((Graphic)haulGoalScreen).color = Color.red;
			ResetLights();
			spotlight1CancelRotation = spotlightHead1.rotation;
			spotlight2CancelRotation = spotlightHead2.rotation;
			if (spotlight1CancelRotation != spotlight1StartRotation || spotlight2CancelRotation != spotlight2StartRotation)
			{
				cancelSpotlights = true;
			}
			cancelSpotlightEval = 0f;
			ResetLightIntensity();
			SetSpotlightColor(Color.red);
			stateTimer = 1f;
			stateStart = false;
			suckParticles.Stop();
			soundCancel.Play(((Component)this).transform.position);
			cancelExtraction = true;
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, emojiScreen.transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, emojiScreen.transform.position, 0.1f);
			if (cancelTube)
			{
				tubeCancelPosition = extractionTube.localPosition;
				tubeSlamDownEval = 0f;
				stateTimer = 2f;
				soundTubeRetract.Play(((Component)this).transform.position);
				tubeHitCeiling = false;
			}
		}
		if (cancelTube && !tubeHitCeiling && tubeSlamDownEval > 0.8f)
		{
			tubeHitCeiling = true;
			HitCeiling();
		}
		if (thirtyFPSUpdate)
		{
			((TMP_Text)haulGoalScreen).text = GlitchyText();
		}
		if (cancelTube)
		{
			if (tubeSlamDownEval < 1f)
			{
				tubeSlamDownEval += 4f * Time.deltaTime;
				tubeSlamDownEval = Mathf.Min(1f, tubeSlamDownEval);
				float num = tubeSlamDown.Evaluate(tubeSlamDownEval);
				extractionTube.localPosition = new Vector3(tubeStartPosition.x, tubeStartPosition.y * num, tubeStartPosition.z);
			}
			else
			{
				cancelTube = false;
			}
		}
		if (cancelSpotlights)
		{
			if (cancelSpotlightEval < 1f)
			{
				cancelSpotlightEval += 4f * Time.deltaTime;
				cancelSpotlightEval = Mathf.Min(1f, cancelSpotlightEval);
				float num2 = tubeSlamDown.Evaluate(cancelSpotlightEval);
				spotlightHead1.rotation = Quaternion.Lerp(spotlight1CancelRotation, spotlight1StartRotation, num2);
				spotlightHead2.rotation = Quaternion.Lerp(spotlight2CancelRotation, spotlight2StartRotation, num2);
			}
			else
			{
				cancelSpotlights = false;
			}
		}
		if (stateEnd)
		{
			StateSet(State.Active);
		}
	}

	private void StateExtracting()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Extracting))
		{
			return;
		}
		if (stateStart)
		{
			Color color = default(Color);
			((Color)(ref color))._002Ector(1f, 0.5f, 0f);
			TubeScreenTextChange("EXTRACTING", color);
			cancelTube = true;
			ResetLights();
			stateTimer = 5f;
			SetEmojiScreen("<sprite name=creepycrying>");
			tubeHit = false;
			stateStart = false;
			cancelExtraction = false;
			soundTubeBuildup.Play(((Component)this).transform.position);
			soundAlarmFinal.Play(emojiScreen.transform.position);
			tubeSlamDownEval = 0f;
			if (isShop)
			{
				hurtColliderMainTimer = 0.25f;
			}
			else
			{
				hurtColliderMainTimer = 1f;
			}
			CurrencyUI.instance.FetchCurrency();
			if (isShop)
			{
				stateTimer = 2f;
			}
		}
		if (tubeSlamDownEval > 0f && tubeSlamDownEval < 0.8f)
		{
			hurtColliderMain.SetActive(false);
			hurtColliders.SetActive(true);
		}
		else
		{
			hurtColliders.SetActive(false);
		}
		if (tubeSlamDownEval > 0.8f && !tubeHit)
		{
			tubeHitParticles.SetActive(true);
			GameDirector.instance.CameraImpact.ShakeDistance(10f, 3f, 8f, extractionTube.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(10f, 3f, 8f, extractionTube.position, 0.1f);
			suckParticles.Play();
			tubeHit = true;
			amountOfValuables = RoundDirector.instance.dollarHaulList.Count;
			suckUpTimeLeft = stateTimer;
			suckUpVariableTimer = suckUpTimeLeft / (float)amountOfValuables;
			soundTubeSlam.Play(((Component)this).transform.position);
			soundTubeSlamGlobal.Play(((Component)this).transform.position);
			if (!isShop)
			{
				if (!isCompletedRightAway)
				{
					RoundDirector.instance.HaulCheck();
				}
			}
			else
			{
				ShopManager.instance.ShopCheck();
			}
			extractionHaul = haulGoal;
			if (RoundDirector.instance.extractionPointsCompleted == RoundDirector.instance.extractionPoints - 1)
			{
				extractionHaul = haulGoal + haulSurplus;
			}
			if (!isCompletedRightAway && CancelExtraction())
			{
				return;
			}
			if (!cancelExtraction)
			{
				ExtractionPointSurplus();
				RoundDirector.instance.ExtractionCompletedAllCheck();
			}
		}
		if (cancelExtraction)
		{
			return;
		}
		if (tubeHit)
		{
			if (!isShop)
			{
				if (!isCompletedRightAway)
				{
					SemiFunc.UIBigMessage("EXTRACTION POINT COMPLETED", "{check}", 25f, Color.white, Color.white);
				}
				else
				{
					SemiFunc.UIBigMessage("EXTRACTION POINT SKIPPED", "{:O}", 25f, Color.white, Color.white);
				}
				HaulUI.instance.Hide();
				ShopCostUI.instance.Show();
				CurrencyUI.instance.Show();
				float num = stateTimer / suckUpTimeLeft;
				ShopCostUI.instance.animatedValue = Mathf.CeilToInt(Mathf.Lerp(0f, Mathf.Ceil((float)(extractionHaul / 1000)), 1f - num));
			}
			if (suckUpVariableTimer <= 0f)
			{
				if (!isShop)
				{
					DestroyTheFirstPhysObjectsInHaulList();
				}
				else
				{
					DestroyTheFirstPhysObjectsInShopList();
				}
				suckUpVariableTimer = suckUpTimeLeft / (float)amountOfValuables;
			}
			else
			{
				suckUpVariableTimer -= Time.deltaTime;
			}
			if (hurtColliderMainTimer > 0f)
			{
				hurtColliderMainTimer -= Time.deltaTime;
				if (hurtColliderMainTimer <= 0f)
				{
					hurtColliderMain.SetActive(true);
				}
			}
			GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, extractionTube.position, 0.5f);
			GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, extractionTube.position, 0.5f);
		}
		if (tubeSlamDownEval < 1f)
		{
			spotlightHead1.Rotate(0f, 500f * Time.deltaTime, 0f);
			spotlightHead2.Rotate(0f, -500f * Time.deltaTime, 0f);
			if (thirtyFPSUpdate)
			{
				((TMP_Text)haulGoalScreen).text = GlitchyText();
			}
			tubeSlamDownEval += 2f * Time.deltaTime;
			tubeSlamDownEval = Mathf.Min(1f, tubeSlamDownEval);
			float num2 = tubeSlamDown.Evaluate(tubeSlamDownEval);
			extractionTube.localPosition = new Vector3(tubeStartPosition.x, tubeStartPosition.y * (1f - num2), tubeStartPosition.z);
			spotlight1.intensity = 6f * (1f - num2);
			spotlight2.intensity = 6f * (1f - num2);
			emojiLight.intensity = 5f * (1f - num2);
		}
		if (tubeHit && !isShop)
		{
			extractionTube.localPosition = new Vector3(0f, 0f + 0.025f * Mathf.Sin(Time.time * 60f), 0f);
			suckInRampEval += 2f * Time.deltaTime;
			suckInRampEval = Mathf.Min(1f, suckInRampEval);
			float num3 = tubeSlamDown.Evaluate(suckInRampEval);
			ramp.localPosition = new Vector3(rampStartPosition.x, rampStartPosition.y - 0.05f * num3, rampStartPosition.z * (1f - num3));
		}
		if ((double)stateTimer <= 0.3)
		{
			upParticles.Play();
		}
		if (stateEnd)
		{
			hurtColliderMain.SetActive(false);
			if (!taxReturn)
			{
				StateSet(State.Complete);
			}
			else
			{
				StateSet(State.TaxReturn);
			}
			tubeHitParticles.SetActive(false);
			if (!isShop)
			{
				DestroyAllPhysObjectsInHaulList();
				roomVolume.SetActive(false);
			}
			else
			{
				DestroyAllPhysObjectsInShoppingList();
			}
		}
	}

	private string GlitchyText()
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			bool flag = false;
			if (Random.Range(0, 4) == 0 && i <= 5)
			{
				text += "TAX";
				i += 2;
				flag = true;
			}
			if (Random.Range(0, 3) == 0 && !flag)
			{
				text += "$";
				flag = true;
			}
			if (!flag)
			{
				text += Random.Range(0, 10);
			}
		}
		return text;
	}

	private void ThiefPunishment()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.ShopGetTotalCost() <= 0)
		{
			return;
		}
		isThief = true;
		ShopManager.instance.isThief = true;
		SetEmojiScreen("<sprite name=thief>");
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			TruckScreenText.instance.GotoPage(3);
			if (SemiFunc.IsMultiplayer())
			{
				for (int i = 0; i < 5; i++)
				{
					PhotonNetwork.InstantiateRoomObject("Items/Item Grenade Explosive", ((Component)this).transform.position + ((Component)this).transform.up * 0.2f + ((Component)this).transform.up * (0.2f * (float)i), Quaternion.identity, (byte)0, (object[])null);
				}
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					Object.Instantiate(Resources.Load("Items/Item Grenade Explosive"), ((Component)this).transform.position + ((Component)this).transform.up * 0.2f + ((Component)this).transform.up * (0.2f * (float)j), Quaternion.identity);
				}
			}
		}
		SemiFunc.UIFocusText("Avoid the grenades!", Color.red, Color.red);
	}

	private void StateTaxReturn()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (StateIs(State.TaxReturn))
		{
			if (stateStart)
			{
				cancelTube = false;
				tubeHitCeiling = false;
				tubeSlamDownEval = 0f;
				tubeHitParticles.SetActive(true);
				soundSuckEnd.Play(((Component)this).transform.position);
				suckParticles.Stop();
				spotlightHead1.rotation = spotlight1StartRotation;
				spotlightHead2.rotation = spotlight2StartRotation;
				stateStart = false;
				runCurrencyBefore = SemiFunc.StatGetRunCurrency();
				extractionHaul = Mathf.CeilToInt((float)(extractionHaul / 1000));
				SemiFunc.StatSetRunCurrency(runCurrencyBefore + extractionHaul);
				CurrencyUI.instance.FetchCurrency();
				RoundDirector.instance.ExtractionCompleted();
				((Component)platform).gameObject.SetActive(false);
				completeJingleGlobal.Play(((Component)this).transform.position);
				completeJingleLocal.Play(((Component)this).transform.position);
				GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
				stateTimer = 3f;
			}
			CurrencyUI.instance.Show();
			if (!tubeHitCeiling && tubeSlamDownEval > 0.8f)
			{
				tubeHitCeiling = true;
				HitCeiling();
				TubeScreenTextChange("TAX RETURN", Color.green);
				SpawnTaxReturn();
			}
			if (tubeSlamDownEval < 1f)
			{
				tubeSlamDownEval += 2f * Time.deltaTime;
				tubeSlamDownEval = Mathf.Min(1f, tubeSlamDownEval);
				float num = tubeSlamDown.Evaluate(1f - tubeSlamDownEval);
				extractionTube.localPosition = new Vector3(tubeStartPosition.x, 2.2f * (1f - num), tubeStartPosition.z);
			}
			if (stateEnd)
			{
				StateSet(State.Complete);
			}
		}
	}

	private void StateComplete()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		if (!StateIs(State.Complete))
		{
			return;
		}
		if (stateStart)
		{
			TubeScreenTextChange("COMPLETED", Color.green);
			cancelTube = false;
			if (!Object.op_Implicit((Object)(object)shopStation))
			{
				DirtFinderMapFloor[] array = mapActive;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].MapObject.Hide();
				}
				array = mapUsed;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].MapObject.Show();
				}
			}
			suckParticles.Stop();
			tubeSlamDownEval = 0f;
			stateStart = false;
			if (!taxReturn)
			{
				tubeHitParticles.SetActive(true);
				soundSuckEnd.Play(((Component)this).transform.position);
			}
			soundTubeRaise.Play(((Component)this).transform.position);
			soundTubeRaiseGlobal.Play(((Component)this).transform.position);
			tubeHitCeiling = false;
			spotlightHead1.rotation = spotlight1StartRotation;
			spotlightHead2.rotation = spotlight2StartRotation;
			if (!isShop)
			{
				RoundDirector.instance.extractionPointActive = false;
				RoundDirector.instance.ExtractionPointsUnlock();
				if (!taxReturn)
				{
					int num = SemiFunc.StatGetRunTotalHaul();
					runCurrencyBefore = SemiFunc.StatGetRunCurrency();
					extractionHaul = Mathf.CeilToInt((float)(extractionHaul / 1000));
					SemiFunc.StatSetRunCurrency(runCurrencyBefore + extractionHaul);
					SemiFunc.StatSetRunTotalHaul(num + extractionHaul);
					CurrencyUI.instance.FetchCurrency();
					RoundDirector.instance.ExtractionCompleted();
					((Component)platform).gameObject.SetActive(false);
					completeJingleGlobal.Play(((Component)this).transform.position);
					completeJingleLocal.Play(((Component)this).transform.position);
				}
				stateTimer = 3f;
				if (RoundDirector.instance.extractionPointsCompleted < RoundDirector.instance.extractionPoints)
				{
					SemiFunc.UIFocusText("Find the next extraction point", Color.white, AssetManager.instance.colorYellow);
				}
				else
				{
					SemiFunc.UIFocusText("Get back to the truck!", Color.white, AssetManager.instance.colorYellow);
				}
			}
			else
			{
				cancelExtraction = true;
				isThief = false;
				ShopManager.instance.isThief = false;
				stateTimer = 1f;
				ThiefPunishment();
				HaulInternalStatsUpdate();
				SetHaulText();
			}
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		}
		if (!isShop && stateTimer > 0f)
		{
			CurrencyUI.instance.Show();
		}
		if (!tubeHitCeiling && tubeSlamDownEval > 0.8f)
		{
			tubeHitCeiling = true;
			HitCeiling();
		}
		if (tubeSlamDownEval < 1f)
		{
			tubeSlamDownEval += 2f * Time.deltaTime;
			tubeSlamDownEval = Mathf.Min(1f, tubeSlamDownEval);
			float num2 = tubeSlamDown.Evaluate(1f - tubeSlamDownEval);
			if (taxReturn)
			{
				extractionTube.localPosition = new Vector3(tubeStartPosition.x, Mathf.LerpUnclamped(tubeStartPosition.y, 2.2f, num2), tubeStartPosition.z);
			}
			else
			{
				extractionTube.localPosition = new Vector3(tubeStartPosition.x, tubeStartPosition.y * (1f - num2), tubeStartPosition.z);
			}
		}
		if (stateEnd && isShop)
		{
			cancelExtraction = true;
			StateSet(State.Active);
		}
	}

	private void ExtractionPointSurplus()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				int num = CalculateSurplus();
				photonView.RPC("ExtractionPointSurplusRPC", (RpcTarget)0, new object[1] { num });
			}
		}
		else
		{
			int surplus = CalculateSurplus();
			ExtractionPointSurplusRPC(surplus);
		}
	}

	private int CalculateSurplus()
	{
		int num = haulCurrent - haulGoal;
		return Mathf.Max(0, num);
	}

	private void SurplusLightLogic()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!surplusLightActive)
		{
			return;
		}
		if (surplusLightTimer > 0f)
		{
			surplusLightTimer -= Time.deltaTime;
			if (surplusLightTimer <= 0f)
			{
				surplusLightOutroSound.Play(((Component)this).transform.position);
			}
			return;
		}
		surplusLightLerp += 2f * Time.deltaTime;
		surplusLight.intensity = Mathf.Lerp(0f, surplusLightIntensity, surplusLightOutro.Evaluate(surplusLightLerp));
		if (surplusLightLerp >= 1f)
		{
			surplusLight.intensity = 0f;
			surplusLight.range = 0f;
			surplusLightActive = false;
		}
	}

	[PunRPC]
	public void ExtractionPointSurplusRPC(int surplus)
	{
		RoundDirector.instance.extractionPointSurplus = surplus;
	}

	private void StateSet(State newState)
	{
		if (settingState)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient() && stateSetTo == State.None)
			{
				settingState = true;
				photonView.RPC("StateSetRPC", (RpcTarget)0, new object[1] { newState });
			}
		}
		else if (stateSetTo == State.None)
		{
			settingState = true;
			StateSetRPC(newState);
		}
	}

	[PunRPC]
	public void StateSetRPC(State state)
	{
		stateSetTo = state;
		stateTimer = 0f;
		stateEnd = true;
	}

	private bool StateIs(State state)
	{
		return currentState == state;
	}

	private void DestroyTheFirstPhysObjectsInHaulList()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && RoundDirector.instance.dollarHaulList.Count != 0 && Object.op_Implicit((Object)(object)RoundDirector.instance.dollarHaulList[0]) && Object.op_Implicit((Object)(object)RoundDirector.instance.dollarHaulList[0].GetComponent<PhysGrabObject>()))
		{
			RoundDirector.instance.totalHaul += (int)RoundDirector.instance.dollarHaulList[0].GetComponent<ValuableObject>().dollarValueCurrent;
			RoundDirector.instance.dollarHaulList[0].GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
			RoundDirector.instance.dollarHaulList.RemoveAt(0);
		}
	}

	private void DestroyTheFirstPhysObjectsInShopList()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || ShopManager.instance.shoppingList.Count == 0)
		{
			return;
		}
		ItemAttributes itemAttributes = ShopManager.instance.shoppingList[0];
		if (Object.op_Implicit((Object)(object)itemAttributes) && Object.op_Implicit((Object)(object)((Component)itemAttributes).GetComponent<PhysGrabObject>()) && SemiFunc.StatGetRunCurrency() - itemAttributes.value >= 0)
		{
			SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - itemAttributes.value);
			StatsManager.instance.ItemPurchase(itemAttributes.item.itemAssetName);
			if (itemAttributes.item.itemType == SemiFunc.itemType.item_upgrade)
			{
				StatsManager.instance.AddItemsUpgradesPurchased(itemAttributes.item.itemAssetName);
			}
			((Component)itemAttributes).GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
			ShopManager.instance.shoppingList.RemoveAt(0);
		}
		SemiFunc.ShopUpdateCost();
	}

	private void DestroyAllPhysObjectsInShoppingList()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.playerDeathHead.Revive();
		}
		List<ItemAttributes> list = new List<ItemAttributes>();
		foreach (ItemAttributes shopping in ShopManager.instance.shoppingList)
		{
			if (Object.op_Implicit((Object)(object)shopping) && Object.op_Implicit((Object)(object)((Component)shopping).GetComponent<PhysGrabObject>()) && SemiFunc.StatGetRunCurrency() - shopping.value >= 0)
			{
				SemiFunc.StatSetRunCurrency(SemiFunc.StatGetRunCurrency() - ((Component)shopping).GetComponent<ItemAttributes>().value);
				StatsManager.instance.ItemPurchase(shopping.item.itemAssetName);
				if (shopping.item.itemType == SemiFunc.itemType.item_upgrade)
				{
					StatsManager.instance.AddItemsUpgradesPurchased(shopping.item.itemAssetName);
				}
				((Component)shopping).GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
				list.Add(shopping);
			}
		}
		foreach (ItemAttributes item in list)
		{
			ShopManager.instance.shoppingList.Remove(item);
		}
		SemiFunc.ShopUpdateCost();
	}

	private void DestroyAllPhysObjectsInHaulList()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			player.playerDeathHead.Revive();
		}
		foreach (GameObject dollarHaul in RoundDirector.instance.dollarHaulList)
		{
			if (Object.op_Implicit((Object)(object)dollarHaul) && Object.op_Implicit((Object)(object)dollarHaul.GetComponent<PhysGrabObject>()))
			{
				RoundDirector.instance.totalHaul += (int)dollarHaul.GetComponent<ValuableObject>().dollarValueCurrent;
				dollarHaul.GetComponent<PhysGrabObject>().DestroyPhysGrabObject();
			}
		}
	}

	private void TubeScreenTextChange(string text, Color color)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (tubeScreenTextString != text)
		{
			soundActivate2.Play(((Component)this).transform.position);
		}
		tubeScreenTextString = text;
		tubeScreenChangeTimer = 0.2f;
		((Graphic)tubeScreenText).color = Color.white;
		tubeScreenLight.color = Color.white;
		tubeScreenTextColor = color;
	}

	private void TubeScreenTextChangeLogic()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (tubeScreenChangeTimer > 0f)
		{
			tubeScreenChangeTimer -= Time.deltaTime;
			if (thirtyFPSUpdate)
			{
				((TMP_Text)tubeScreenText).text = GlitchyText();
			}
			if (tubeScreenChangeTimer <= 0f)
			{
				((TMP_Text)tubeScreenText).text = tubeScreenTextString;
				((Graphic)tubeScreenText).color = tubeScreenTextColor;
				tubeScreenLight.color = tubeScreenTextColor;
			}
		}
	}

	private void ButtonToggle(bool _active)
	{
		if (buttonActive != _active)
		{
			if (buttonDenyCooldown > 0f)
			{
				buttonDenyCooldown = 0f;
			}
			if (_active)
			{
				((Renderer)button).material = buttonOriginalMaterial;
				((Behaviour)buttonGrabObject).enabled = true;
				((Behaviour)buttonLight).enabled = true;
			}
			else
			{
				((Renderer)button).material = buttonOff;
				((Behaviour)buttonGrabObject).enabled = true;
				((Behaviour)buttonLight).enabled = false;
			}
			if (currentState != State.Idle)
			{
				((Behaviour)buttonGrabObject).enabled = false;
			}
			buttonActive = _active;
		}
	}

	[PunRPC]
	private void ButtonDenyRPC()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		buttonDenyActive = true;
		buttonDenyLerp = 0f;
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, emojiScreen.transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, emojiScreen.transform.position, 0.1f);
		soundCancel.Play(((Component)this).transform.position);
		((Behaviour)buttonLight).enabled = true;
		((Renderer)button).material = buttonDenyMaterial;
		((Behaviour)buttonGrabObject).enabled = false;
		buttonDenyCooldown = 1f;
		PlayerAvatar playerAvatarScript = PlayerController.instance.playerAvatarScript;
		if (!playerAvatarScript.isDisabled && Vector3.Distance(((Component)playerAvatarScript).transform.position, ((Component)this).transform.position) < 10f && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialOnlyOneExtraction, 1))
		{
			TutorialDirector.instance.ActivateTip("Only One Extraction", 2f, _interrupt: false);
		}
	}
}
