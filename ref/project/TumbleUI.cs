using UnityEngine;
using UnityEngine.UI;

public class TumbleUI : MonoBehaviour
{
	public static TumbleUI instance;

	private CanvasGroup canvasGroup;

	private bool active;

	private bool activePrevious = true;

	private bool canExit;

	private bool canExitPrevious;

	private bool animating;

	private float animationLerp;

	public Color canNotExitColor1;

	public Color canNotExitColor2;

	public Color canExitColor1;

	public Color canExitColor2;

	[Space]
	public AnimationCurve introCurve;

	public float introSpeed;

	[Space]
	public AnimationCurve outroCurve;

	public float outroSpeed;

	[Space]
	public float updateTime;

	private float updateTimer;

	[Space]
	public GameObject[] parts1;

	public GameObject[] parts2;

	private Image[] images1;

	private Image[] images2;

	[Space]
	public Sound canExitSound;

	private float hideTimer;

	private float hideAlpha;

	private void Awake()
	{
		instance = this;
		canvasGroup = ((Component)this).GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		images1 = (Image[])(object)new Image[parts1.Length];
		int num = 0;
		GameObject[] array = parts1;
		foreach (GameObject val in array)
		{
			images1[num] = val.GetComponent<Image>();
			num++;
		}
		images2 = (Image[])(object)new Image[parts2.Length];
		num = 0;
		array = parts2;
		foreach (GameObject val2 in array)
		{
			images2[num] = val2.GetComponent<Image>();
			num++;
		}
	}

	private void Update()
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PlayerController.instance.playerAvatarScript.isTumbling && !PlayerController.instance.playerAvatarScript.isDisabled)
		{
			active = true;
		}
		else
		{
			active = false;
		}
		if (active != activePrevious)
		{
			activePrevious = active;
			animationLerp = 0f;
			updateTimer = 0f;
			animating = true;
		}
		canExit = true;
		if (active && (PlayerController.instance.playerAvatarScript.tumble.tumbleOverride || PlayerController.instance.tumbleInputDisableTimer > 0f))
		{
			canExit = false;
		}
		if (canExit != canExitPrevious)
		{
			canExitPrevious = canExit;
			if (canExit)
			{
				canExitSound.Play(((Component)this).transform.position);
				animating = true;
				animationLerp = 0.5f;
				updateTimer = 0f;
				Image[] array = images1;
				for (int i = 0; i < array.Length; i++)
				{
					((Graphic)array[i]).color = canExitColor1;
				}
				array = images2;
				for (int i = 0; i < array.Length; i++)
				{
					((Graphic)array[i]).color = canExitColor2;
				}
			}
			else
			{
				Image[] array = images1;
				for (int i = 0; i < array.Length; i++)
				{
					((Graphic)array[i]).color = canNotExitColor2;
				}
				array = images2;
				for (int i = 0; i < array.Length; i++)
				{
					((Graphic)array[i]).color = canNotExitColor1;
				}
			}
		}
		if (animating)
		{
			if (updateTimer <= 0f)
			{
				if (active)
				{
					if (animationLerp == 0f)
					{
						GameObject[] array2 = parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
						array2 = parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(true);
						}
					}
					animationLerp += Time.deltaTime * introSpeed;
					updateTimer = updateTime;
				}
				else
				{
					animationLerp += Time.deltaTime * outroSpeed;
					updateTimer = updateTime;
					if (animationLerp >= 1f)
					{
						GameObject[] array2 = parts1;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
						array2 = parts2;
						for (int i = 0; i < array2.Length; i++)
						{
							array2[i].SetActive(false);
						}
					}
				}
			}
			else
			{
				updateTimer -= Time.deltaTime;
			}
		}
		if (animating)
		{
			if (active)
			{
				((Component)this).transform.localScale = Vector3.LerpUnclamped(Vector3.one * 1.25f, Vector3.one, introCurve.Evaluate(animationLerp));
			}
			else
			{
				((Component)this).transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 1.25f, outroCurve.Evaluate(animationLerp));
			}
			if (animationLerp >= 1f)
			{
				animating = false;
			}
		}
		float num = 1f;
		if (hideTimer > 0f)
		{
			num = 0f;
			hideTimer -= Time.deltaTime;
		}
		hideAlpha = Mathf.Lerp(hideAlpha, num, Time.deltaTime * 20f);
		canvasGroup.alpha = hideAlpha;
	}

	public void Hide()
	{
		hideTimer = 0.1f;
	}
}
