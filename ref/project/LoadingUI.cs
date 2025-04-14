using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
	public static LoadingUI instance;

	public Image fadeImage;

	public Image fadeBehindImage;

	[Space]
	public TextMeshProUGUI levelNumberText;

	public TextMeshProUGUI levelNameText;

	[Space]
	public Image loadingGraphic01;

	public Image loadingGraphic02;

	public Image loadingGraphic03;

	private Animator animator;

	internal bool levelAnimationStarted;

	internal bool levelAnimationCompleted;

	internal bool debugDisableLevelAnimation;

	[Space]
	public Sound soundTurn;

	public Sound soundRevUp;

	public Sound soundCrash;

	public Sound soundtextLevel;

	public Sound soundtextName;

	private void Awake()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		instance = this;
		((Component)fadeBehindImage).gameObject.SetActive(false);
		animator = ((Component)this).GetComponent<Animator>();
		((Behaviour)animator).enabled = false;
		animator.keepAnimatorStateOnDisable = true;
		if (Object.op_Implicit((Object)(object)RunManager.instance))
		{
			((Graphic)fadeImage).color = RunManager.instance.loadingFadeColor;
			animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
		}
	}

	private void LateUpdate()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (RunManager.instance.skipLoadingUI)
		{
			return;
		}
		float num = Time.deltaTime;
		if (!levelAnimationStarted)
		{
			num = Mathf.Min(num, 0.01f);
		}
		animator.Update(num);
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait || (GameDirector.instance.currentState == GameDirector.gameState.Start && !levelAnimationCompleted))
		{
			((Graphic)fadeImage).color = Color.Lerp(((Graphic)fadeImage).color, new Color(0f, 0f, 0f, 0f), 5f * num);
		}
		else
		{
			((Graphic)fadeImage).color = Color.Lerp(((Graphic)fadeImage).color, Color.black, 20f * num);
		}
		RunManager.instance.loadingFadeColor = ((Graphic)fadeImage).color;
		RunManager runManager = RunManager.instance;
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		runManager.loadingAnimationTime = ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime;
		if (GameDirector.instance.PlayerList.Count <= 0)
		{
			return;
		}
		bool flag = true;
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (!player.levelAnimationCompleted)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			levelAnimationCompleted = true;
		}
	}

	public void StopLoading()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		RunManager.instance.loadingFadeColor = Color.black;
		RunManager.instance.skipLoadingUI = false;
		((Component)this).gameObject.SetActive(false);
	}

	public void StartLoading()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		levelAnimationStarted = false;
		((Component)this).gameObject.SetActive(true);
		((Graphic)fadeImage).color = RunManager.instance.loadingFadeColor;
		animator.Play("Idle", 0, RunManager.instance.loadingAnimationTime);
	}

	public void LevelAnimationStart()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		levelAnimationStarted = true;
		if (!RunManager.instance.skipLoadingUI && !debugDisableLevelAnimation && (SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || SemiFunc.RunIsArena()))
		{
			loadingGraphic01.sprite = LevelGenerator.Instance.Level.LoadingGraphic01;
			loadingGraphic02.sprite = LevelGenerator.Instance.Level.LoadingGraphic02;
			loadingGraphic03.sprite = LevelGenerator.Instance.Level.LoadingGraphic03;
			if (SemiFunc.RunIsShop())
			{
				((TMP_Text)levelNumberText).text = "SHOP";
			}
			else if (SemiFunc.RunIsArena())
			{
				((TMP_Text)levelNumberText).text = "GAME OVER";
				((Graphic)levelNumberText).color = Color.red;
			}
			else
			{
				((TMP_Text)levelNumberText).text = "LEVEL " + (RunManager.instance.levelsCompleted + 1);
			}
			((TMP_Text)levelNameText).text = LevelGenerator.Instance.Level.NarrativeName.ToUpper();
			animator.SetTrigger("Level");
		}
		else
		{
			levelAnimationCompleted = true;
		}
	}

	public void LevelAnimationComplete()
	{
		PlayerController.instance.playerAvatarScript.LoadingLevelAnimationCompleted();
	}

	public void PlayTurn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundTurn.Play(((Component)this).transform.position);
	}

	public void PlayRevUp()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundRevUp.Play(((Component)this).transform.position);
	}

	public void PlayCrash()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundCrash.Play(((Component)this).transform.position);
	}

	public void PlayTextLevel()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundtextLevel.Play(((Component)this).transform.position);
	}

	public void PlayTextName()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundtextName.Play(((Component)this).transform.position);
	}
}
