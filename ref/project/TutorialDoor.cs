using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialDoor : MonoBehaviour
{
	private enum DoorState
	{
		Closed,
		Success,
		Unlock,
		Opening,
		Open
	}

	public int tutorialPage;

	public AnimationCurve animationCurve;

	public AnimationCurve animationCurveDoor;

	private float doorEndYPos;

	private float animationProgress;

	private bool animationDone;

	private int prevState;

	private int currentState;

	private float stateTimer;

	private bool stateStart;

	public Transform latchTransform;

	public Transform screenTransform;

	private bool animationImpactDone;

	public GameObject emojiScreenGlitch;

	public TextMeshPro doorText;

	private string prevEmoji;

	private string currentEmoji;

	private float emojiScreenGlitchTimer;

	private float emojiDelay;

	private bool thirtyFPSUpdate;

	public Sound soundEmojiGlitch;

	private float thirtyFPSUpdateTimer;

	public List<Transform> fillBars = new List<Transform>();

	private float fillBarProgress;

	private float fillBarProgressPrev = -1f;

	private bool moveDone;

	public Transform animationTransform;

	[FormerlySerializedAs("light")]
	public Light doorLight;

	public Transform latchLamp1;

	public Transform latchLamp2;

	public ParticleSystem particlesCeiling;

	public Transform particlesOpen;

	public ParticleSystem particlesUnlock;

	public ParticleSystem particlesLatch1;

	public ParticleSystem particlesLatch2;

	public ParticleSystem particlesDoorSmoke;

	public ParticleSystem particlesBleep1;

	public ParticleSystem particlesBleep2;

	public ParticleSystem lightParticle;

	public ParticleSystem lightParticle2;

	public Sound soundGoUp;

	public Sound soundGoDown;

	public Sound soundSuccess;

	public Sound soundUnlock;

	public Sound soundUnlockEnd;

	public Sound soundLatches;

	public Sound soundLatchesEnd;

	public Sound soundDoorOpen;

	public Sound soundDoorMove;

	public Sound soundSlamCeiling;

	private void Start()
	{
		doorEndYPos = 7.42f;
		doorLight.intensity = 0f;
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
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		StateMachine();
		if (fillBarProgress != fillBarProgressPrev)
		{
			if (fillBarProgress > fillBarProgressPrev)
			{
				soundGoUp.Pitch = 1f + fillBarProgress / 11f;
				soundGoUp.Play(animationTransform.position);
				particlesBleep1.Play();
				particlesBleep2.Play();
			}
			else
			{
				soundGoDown.Pitch = 1f + fillBarProgress / 11f;
				soundGoDown.Play(animationTransform.position);
			}
			fillBarProgressPrev = fillBarProgress;
		}
	}

	private void StateMachine()
	{
		ThirtyFPS();
		switch (currentState)
		{
		case 0:
			StateClosed();
			break;
		case 1:
			StateSuccess();
			break;
		case 2:
			StateUnlock();
			break;
		case 3:
			StateOpening();
			break;
		case 4:
			StateOpen();
			break;
		}
		EmojiScreenGlitchLogic();
		thirtyFPSUpdate = false;
		stateTimer += Time.deltaTime;
		if (stateTimer > 1000000f)
		{
			stateTimer = 0f;
		}
	}

	private void StateSet(int _state)
	{
		prevState = currentState;
		stateTimer = 0f;
		currentState = _state;
		stateStart = true;
		animationDone = false;
		animationProgress = 0f;
		animationImpactDone = false;
	}

	private void EffectEmoji()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, animationTransform.position, 0.1f);
		soundSuccess.Play(animationTransform.position);
		((Component)lightParticle).transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		lightParticle.Play();
		doorLight.color = new Color(1f, 0.5f, 0f, 1f);
		doorLight.range = 10f;
		doorLight.intensity = 4f;
	}

	private void EffectScreenRotateStart()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, animationTransform.position, 0.1f);
		soundUnlock.Play(animationTransform.position);
	}

	private void EffectScreenRotateEnd()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, animationTransform.position, 0.1f);
		soundUnlockEnd.Play(animationTransform.position);
		particlesUnlock.Play();
		((Component)lightParticle).transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		lightParticle.Play();
	}

	private void EffectLatchStart()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, animationTransform.position, 0.1f);
		soundLatches.Play(animationTransform.position);
		((Component)lightParticle).transform.localPosition = new Vector3(-0.82f, 3.3f, 4.57f);
		lightParticle.Play();
		((Component)lightParticle2).transform.localPosition = new Vector3(-0.82f, 3.3f, -4.57f);
		lightParticle2.Play();
		((Renderer)((Component)latchLamp1).GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
		((Renderer)((Component)latchLamp2).GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
	}

	private void EffectLatchEnd()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		soundLatchesEnd.Play(((Component)this).transform.position);
		particlesLatch1.Play();
		particlesLatch2.Play();
		((Component)lightParticle).transform.localPosition = new Vector3(-0.82f, 3.3f, 3.3f);
		lightParticle.Play();
		((Component)lightParticle2).transform.localPosition = new Vector3(-0.82f, 3.3f, -3.3f);
		lightParticle2.Play();
	}

	private void EffectDoorOpenStart()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, animationTransform.position, 0.1f);
		soundDoorOpen.Play(animationTransform.position);
		particlesDoorSmoke.Play();
	}

	private void EffectDoorMove()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundDoorMove.Play(animationTransform.position);
		((Component)particlesOpen).gameObject.SetActive(true);
	}

	private void EffectDoorOpenEnd()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, animationTransform.position, 0.1f);
		soundSlamCeiling.Play(animationTransform.position);
		particlesCeiling.Play();
		Object.Destroy((Object)(object)doorLight);
	}

	private void StateClosed()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (TutorialDirector.instance.currentPage > tutorialPage)
		{
			StateSet(1);
		}
		float num = TutorialUI.instance.progressBarCurrent * 130f;
		fillBarProgress = Mathf.FloorToInt(num / 11f);
		for (int i = 0; i < fillBars.Count; i++)
		{
			fillBars[i].localScale = new Vector3(1f, 1f, Mathf.Clamp01(fillBarProgress / 11f));
		}
	}

	private void EmojiSet(string emoji)
	{
		((TMP_Text)doorText).text = "<size=100>|</size>" + emoji + "<size=100>|</size>";
	}

	private void EmojiScreenGlitch(Color color)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (emojiScreenGlitchTimer <= 0f)
		{
			soundEmojiGlitch.Play(doorText.transform.position);
		}
		emojiScreenGlitchTimer = 0.2f;
		emojiScreenGlitch.SetActive(true);
		((Behaviour)doorText).enabled = false;
		((Renderer)emojiScreenGlitch.GetComponent<MeshRenderer>()).material.SetColor("_EmissionColor", color);
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
		currentEmoji = ((TMP_Text)doorText).text;
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
				((Behaviour)doorText).enabled = true;
			}
		}
	}

	private void StateSuccess()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			EmojiSet("<sprite name=creepycrying>");
			EffectEmoji();
			stateStart = false;
			for (int i = 0; i < fillBars.Count; i++)
			{
				fillBars[i].localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (!animationDone)
		{
			if (animationProgress == 0f)
			{
				EffectScreenRotateStart();
			}
			animationProgress += 2f * Time.deltaTime;
			float num = animationCurve.Evaluate(animationProgress);
			screenTransform.localRotation = Quaternion.Euler(Mathf.LerpUnclamped(0f, 45f, num), 0f, 0f);
			if (animationProgress >= 0.53f && !animationImpactDone)
			{
				EffectScreenRotateEnd();
				animationImpactDone = true;
			}
			if (animationProgress >= 1f)
			{
				animationDone = true;
			}
		}
		if (stateTimer > 1f)
		{
			StateSet(2);
		}
	}

	private void StateUnlock()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			EffectLatchStart();
			stateStart = false;
		}
		animationProgress += 1.5f * Time.deltaTime;
		float num = animationCurve.Evaluate(animationProgress);
		if (animationProgress > 0.53f && !animationImpactDone)
		{
			animationImpactDone = true;
			EffectLatchEnd();
		}
		latchTransform.localScale = new Vector3(latchTransform.localScale.x, latchTransform.localScale.y, Mathf.LerpUnclamped(1f, 0.8f, num));
		if (animationProgress >= 1f)
		{
			animationDone = true;
			StateSet(3);
		}
	}

	private void StateOpening()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			EffectDoorOpenStart();
			stateStart = false;
		}
		if (animationProgress > 0.53f && !moveDone)
		{
			moveDone = true;
			EffectDoorMove();
		}
		animationProgress += Time.deltaTime;
		float num = animationCurveDoor.Evaluate(animationProgress);
		animationTransform.position = new Vector3(animationTransform.position.x, Mathf.LerpUnclamped(0f, doorEndYPos, num), animationTransform.position.z);
		if (animationProgress > 0.53f && !animationImpactDone)
		{
			animationImpactDone = true;
			EffectDoorOpenEnd();
		}
		if (animationProgress >= 1f)
		{
			animationDone = true;
			StateSet(4);
		}
	}

	private void StateOpen()
	{
	}
}
