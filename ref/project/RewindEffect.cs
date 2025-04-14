using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewindEffect : MonoBehaviour
{
	public static RewindEffect Instance;

	public Timecode Timecode;

	[HideInInspector]
	public List<Timecode.TimeSnapshot> TimeSnapshots;

	[Space]
	public int maxScreenshots = 50;

	public float movementThreshold = 3f;

	public Transform PlayerTransfrom;

	private List<Texture2D> screenshots = new List<Texture2D>();

	public GameObject RewindEffectUI;

	private Vector3 lastScreenshotPosition;

	[HideInInspector]
	public bool PlayRewind;

	public float rewindDuration = 1.5f;

	private bool RewindEnd;

	private bool FirstStep = true;

	public GameObject RewindLines;

	public RawImage RenderTextureMain;

	public Sound RewindStartSound;

	public Sound RewindEndSound;

	public Sound RewindLoopSound;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		RewindEffectUI.SetActive(false);
		RewindLines.SetActive(false);
	}

	private void Update()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (PlayRewind)
		{
			GameDirector.instance.SetDisableInput(0.5f);
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
		}
		RewindLoopSound.PlayLoop(PlayRewind, 0.9f, 0.9f);
		if (!PlayRewind && !RewindEnd && Vector3.Distance(PlayerTransfrom.position, lastScreenshotPosition) > movementThreshold)
		{
			if (!FirstStep)
			{
				CaptureScreenshot();
				lastScreenshotPosition = PlayerTransfrom.position;
			}
			else
			{
				ClearScreenshots();
				FirstStep = false;
			}
		}
	}

	private void CaptureScreenshot()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (screenshots.Count >= maxScreenshots)
		{
			Object.Destroy((Object)(object)screenshots[0]);
			screenshots.RemoveAt(0);
			TimeSnapshots.RemoveAt(0);
		}
		Texture texture = RenderTextureMain.texture;
		RenderTexture val = new RenderTexture(128, 72, 24);
		val.Create();
		Graphics.Blit(texture, val);
		RenderTexture.active = val;
		Texture2D val2 = new Texture2D(128, 72, (TextureFormat)3, false);
		val2.ReadPixels(new Rect(0f, 0f, 128f, 72f), 0, 0);
		val2.Apply();
		RenderTexture.active = null;
		screenshots.Add(val2);
		TimeSnapshots.Add(Timecode.GetSnapshot());
		Object.Destroy((Object)(object)val);
	}

	public void PlayRewindEffect()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (screenshots.Count >= 10)
		{
			RewindStartSound.Play(((Component)this).transform.position);
			PlayRewind = true;
			RewindLines.SetActive(true);
			RewindEffectUI.SetActive(true);
			((MonoBehaviour)this).StartCoroutine(RewindCoroutine());
		}
		else
		{
			RewindEnding();
		}
	}

	private IEnumerator RewindCoroutine()
	{
		Image rewindImage = RewindEffectUI.GetComponent<Image>();
		((Behaviour)rewindImage).enabled = true;
		float displayTimePerScreenshot = rewindDuration / (float)screenshots.Count;
		for (int i = screenshots.Count - 1; i >= 0; i--)
		{
			Timecode.SetTime(TimeSnapshots[i]);
			Texture2D screenshot = screenshots[i];
			Sprite sprite = (rewindImage.sprite = Sprite.Create(screenshot, new Rect(0f, 0f, (float)((Texture)screenshot).width, (float)((Texture)screenshot).height), new Vector2(0.5f, 0.5f)));
			yield return (object)new WaitForSeconds(displayTimePerScreenshot);
			Object.Destroy((Object)(object)sprite);
			Object.Destroy((Object)(object)screenshot);
		}
		RewindEnding();
	}

	public void ClearScreenshots()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		foreach (Texture2D screenshot in screenshots)
		{
			Object.Destroy((Object)(object)screenshot);
		}
		screenshots.Clear();
		lastScreenshotPosition = PlayerTransfrom.position;
	}

	public void RewindEnding()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Timecode.SetToStartSnapshot();
		RewindEndSound.Play(((Component)this).transform.position);
		PlayRewind = false;
		ClearScreenshots();
		RewindLines.SetActive(false);
		lastScreenshotPosition = PlayerTransfrom.position;
		FirstStep = true;
	}
}
