using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialUI : SemiUI
{
	public TextMeshProUGUI Text;

	public Transform progressBar;

	public AnimationCurve scaleInCurve;

	public static TutorialUI instance;

	private string messagePrev = "prev";

	private Color bigMessageColor = Color.white;

	private Color bigMessageFlashColor = Color.white;

	private float messageTimer;

	private float progressBarTarget;

	internal float progressBarCurrent;

	[HideInInspector]
	public float animationCurveEval;

	public VideoPlayer videoPlayer;

	public VideoClip staticVideo;

	public VideoClip nextVideo;

	private string nextText;

	private float bigVideoTimer = 5f;

	public Transform videoTransform;

	public RawImage videoImage;

	public TextMeshProUGUI dummyText;

	public TextMeshProUGUI dummyTextExclamation;

	public Transform dummyTextTransform;

	private float dummyTextAnimationEval;

	private float dummyTextTimer = 30f;

	private string currentDummyText;

	private float hideAllTimer;

	protected override void Start()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		uiText = Text;
		base.Start();
		instance = this;
		videoPlayer.clip = staticVideo;
		((Component)dummyTextTransform).gameObject.SetActive(false);
		dummyTextTimer = 30f;
		((Component)videoTransform).gameObject.SetActive(false);
		((Component)videoPlayer).gameObject.SetActive(false);
		((Component)this).transform.localScale = new Vector3(0f, 1f, 1f);
	}

	public void TutorialText(string message)
	{
		if (!(messageTimer > 0f))
		{
			messageTimer = 0.2f;
			if (message != messagePrev)
			{
				((TMP_Text)Text).text = message;
				SemiUISpringShakeY(20f, 10f, 0.3f);
				SemiUISpringScale(0.4f, 5f, 0.2f);
				messagePrev = message;
			}
		}
	}

	public void SetPage(VideoClip video, string text, string dummyTextString, bool transition = true)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		SemiUISpringShakeY(10f, 8f, 0.5f);
		if (transition)
		{
			((TMP_Text)Text).text = "Good job! <sprite name=creepycrying>";
			videoPlayer.clip = staticVideo;
			nextVideo = video;
			nextText = text;
		}
		else
		{
			((TMP_Text)Text).text = text;
			videoPlayer.clip = video;
			nextVideo = video;
			nextText = text;
		}
		videoPlayer.Play();
		((Component)videoTransform).transform.localScale = new Vector3(1f, 1f, 1f);
		((Graphic)videoImage).color = new Color(1f, 1f, 1f, 1f);
		bigVideoTimer = 7f;
		currentDummyText = dummyTextString;
		((TMP_Text)dummyText).text = dummyTextString;
		dummyTextTimer = 30f;
		dummyTextAnimationEval = 0f;
		((Component)dummyTextTransform).gameObject.SetActive(false);
		((MonoBehaviour)this).StartCoroutine(SwitchPage());
	}

	public void SetTipPage(VideoClip video, string text)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((Component)videoTransform).gameObject.SetActive(true);
		((Component)videoPlayer).gameObject.SetActive(true);
		videoPlayer.clip = video;
		((TMP_Text)Text).text = text;
		videoPlayer.time = 0.0;
		videoPlayer.Play();
		((Component)videoTransform).transform.localScale = new Vector3(1f, 1f, 1f);
		((Graphic)videoImage).color = new Color(1f, 1f, 1f, 1f);
		bigVideoTimer = 6f;
		currentDummyText = "";
		((TMP_Text)dummyText).text = "";
		dummyTextTimer = 30f;
		dummyTextAnimationEval = 0f;
		((Component)dummyTextTransform).gameObject.SetActive(false);
	}

	private IEnumerator SwitchPage()
	{
		yield return (object)new WaitForSeconds(2f);
		if ((Object)(object)videoPlayer.clip != (Object)(object)nextVideo)
		{
			SemiUISpringShakeY(10f, 8f, 0.5f);
		}
		videoPlayer.clip = nextVideo;
		videoPlayer.Play();
		((TMP_Text)Text).text = nextText;
	}

	protected override void Update()
	{
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (hideTimer > 0f && showTimer <= 0f)
		{
			if (hideAllTimer > 0f)
			{
				hideAllTimer -= Time.deltaTime;
				return;
			}
			((Component)dummyTextTransform).gameObject.SetActive(false);
			dummyTextTimer = 30f;
			((Component)videoTransform).gameObject.SetActive(false);
			((Component)videoPlayer).gameObject.SetActive(false);
			return;
		}
		((Component)videoTransform).gameObject.SetActive(true);
		((Component)videoPlayer).gameObject.SetActive(true);
		hideAllTimer = 2f;
		if (dummyTextTimer <= 0f)
		{
			if (currentDummyText != "" && !((Component)dummyTextTransform).gameObject.activeSelf)
			{
				((Component)dummyTextTransform).gameObject.SetActive(true);
				((TMP_Text)dummyText).text = currentDummyText;
				dummyTextAnimationEval = 0f;
				bigVideoTimer = 1f;
			}
			if (dummyTextAnimationEval < 1f)
			{
				dummyTextAnimationEval += Time.deltaTime * 3f;
				dummyTextAnimationEval = Mathf.Clamp01(dummyTextAnimationEval);
				float num = scaleInCurve.Evaluate(dummyTextAnimationEval);
				dummyTextTransform.localPosition = new Vector3(dummyTextTransform.localPosition.x, Mathf.LerpUnclamped(-20f, 20f, num), dummyTextTransform.localPosition.z);
			}
		}
		else
		{
			dummyTextTimer -= Time.deltaTime;
		}
		if (!SemiFunc.RunIsTutorial())
		{
			if (bigVideoTimer > 0f)
			{
				bigVideoTimer -= Time.deltaTime;
				float num2 = 1f;
				((Component)videoTransform).transform.localScale = new Vector3(Mathf.Lerp(((Component)videoTransform).transform.localScale.x, num2, Time.deltaTime * 20f), Mathf.Lerp(((Component)videoTransform).transform.localScale.y, num2, Time.deltaTime * 20f), Mathf.Lerp(((Component)videoTransform).transform.localScale.z, num2, Time.deltaTime * 20f));
				float num3 = 1f;
				((Graphic)videoImage).color = new Color(1f, 1f, 1f, Mathf.Lerp(((Graphic)videoImage).color.a, num3, Time.deltaTime * 20f));
			}
			else
			{
				float num4 = 0.7f;
				((Component)videoTransform).transform.localScale = new Vector3(Mathf.Lerp(((Component)videoTransform).transform.localScale.x, num4, Time.deltaTime * 20f), Mathf.Lerp(((Component)videoTransform).transform.localScale.y, num4, Time.deltaTime * 20f), Mathf.Lerp(((Component)videoTransform).transform.localScale.z, num4, Time.deltaTime * 20f));
				float num5 = 0.5f;
				((Graphic)videoImage).color = new Color(1f, 1f, 1f, Mathf.Lerp(((Graphic)videoImage).color.a, num5, Time.deltaTime * 20f));
			}
		}
		progressBarTarget = TutorialDirector.instance.tutorialProgress;
		animationCurveEval += Time.deltaTime * 3f;
		animationCurveEval = Mathf.Clamp(animationCurveEval, 0f, 1f);
		float num6 = scaleInCurve.Evaluate(animationCurveEval);
		((Component)this).transform.localScale = new Vector3(1f, num6, 1f);
		progressBarCurrent = progressBar.localScale.x;
		progressBar.localScale = new Vector3(Mathf.Lerp(progressBar.localScale.x, progressBarTarget, Time.deltaTime * 20f), 1f, 1f);
		if (currentDummyText == "" || dummyTextTimer > 0f)
		{
			((Component)dummyTextTransform).gameObject.SetActive(false);
		}
	}
}
