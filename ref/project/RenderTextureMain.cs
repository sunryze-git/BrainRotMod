using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureMain : MonoBehaviour
{
	public static RenderTextureMain instance;

	private List<Camera> cameras = new List<Camera>();

	public RenderTexture renderTexture;

	[Space]
	public float textureWidthSmall;

	public float textureHeightSmall;

	[Space]
	public float textureWidthMedium;

	public float textureHeightMedium;

	[Space]
	public float textureWidthLarge;

	public float textureHeightLarge;

	internal float textureWidthOriginal;

	internal float textureHeightOriginal;

	internal float textureWidth;

	internal float textureHeight;

	internal float textureResetTimer;

	internal float sizeResetTimer;

	[Space]
	public AnimationCurve shakeCurve;

	private float shakeTimer;

	private bool shakeActive;

	private float shakeX;

	private float shakeXOld;

	private float shakeXNew;

	private float shakeXLerp = 1f;

	private float shakeY;

	private float shakeYOld;

	private float shakeYNew;

	private float shakeYLerp = 1f;

	private Vector3 originalSize;

	public RawImage overlayRawImage;

	private float overlayDisableTimer;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		textureWidthOriginal = textureWidthSmall;
		textureHeightOriginal = textureHeightSmall;
		originalSize = ((Component)this).transform.localScale;
		Camera[] componentsInChildren = ((Component)Camera.main).GetComponentsInChildren<Camera>();
		foreach (Camera item in componentsInChildren)
		{
			cameras.Add(item);
		}
		ResetResolution();
	}

	private void Update()
	{
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		if (shakeActive)
		{
			if (shakeXLerp >= 1f)
			{
				shakeXLerp = 0f;
				shakeXOld = shakeXNew;
				shakeXNew = Random.Range(-5f, 5f);
			}
			else
			{
				shakeXLerp += Time.deltaTime * 100f;
				shakeX = Mathf.Lerp(shakeXOld, shakeXNew, shakeCurve.Evaluate(shakeXLerp));
			}
			if (shakeYLerp >= 1f)
			{
				shakeYLerp = 0f;
				shakeYOld = shakeYNew;
				shakeYNew = Random.Range(-5f, 5f);
			}
			else
			{
				shakeYLerp += Time.deltaTime * 100f;
				shakeY = Mathf.Lerp(shakeYOld, shakeYNew, shakeCurve.Evaluate(shakeYLerp));
			}
			((Component)this).transform.localPosition = new Vector3(shakeX, shakeY, 0f);
			shakeTimer -= Time.deltaTime;
			if (shakeTimer <= 0f)
			{
				((Component)this).transform.localPosition = new Vector3(0f, 0f, 0f);
				shakeActive = false;
			}
		}
		if (sizeResetTimer > 0f)
		{
			sizeResetTimer -= Time.deltaTime;
		}
		else if (((Component)this).transform.localScale != originalSize)
		{
			((Component)this).transform.localScale = originalSize;
		}
		if (textureResetTimer > 0f)
		{
			textureResetTimer -= Time.deltaTime;
		}
		else if (((Texture)renderTexture).width != (int)textureWidthOriginal || ((Texture)renderTexture).height != (int)textureHeightOriginal)
		{
			ResetResolution();
		}
		if (overlayDisableTimer > 0f)
		{
			overlayDisableTimer -= Time.deltaTime;
			if (overlayDisableTimer <= 0f)
			{
				((Behaviour)overlayRawImage).enabled = true;
			}
		}
	}

	public void Shake(float _time)
	{
		shakeActive = true;
		shakeTimer = _time;
	}

	public void ChangeSize(float _width, float _height, float _time)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = new Vector3(_width, _height, 1f);
		sizeResetTimer = _time;
	}

	public void ChangeResolution(float _width, float _height, float _time)
	{
		textureWidth = _width;
		textureHeight = _height;
		SetRenderTexture();
		textureResetTimer = _time;
	}

	public void ResetResolution()
	{
		textureWidth = textureWidthOriginal;
		textureHeight = textureHeightOriginal;
		SetRenderTexture();
	}

	private void SetRenderTexture()
	{
		renderTexture.Release();
		((Texture)renderTexture).width = (int)textureWidth;
		((Texture)renderTexture).height = (int)textureHeight;
		renderTexture.Create();
		cameras[0].targetTexture = renderTexture;
		foreach (Camera camera in cameras)
		{
			((Behaviour)camera).enabled = false;
			((Behaviour)camera).enabled = true;
		}
	}

	private void OnApplicationQuit()
	{
		textureWidth = textureWidthSmall;
		textureHeight = textureHeightSmall;
		SetRenderTexture();
	}

	public void OverlayDisable()
	{
		((Behaviour)overlayRawImage).enabled = false;
		overlayDisableTimer = 0.5f;
	}
}
