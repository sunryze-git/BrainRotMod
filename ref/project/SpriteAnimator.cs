using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public List<Sprite> animationSprites;

	public int framesPerSecond = 12;

	private int currentSpriteIndex;

	private bool isAnimating = true;

	private float secondsPerFrame;

	private void Start()
	{
		if ((Object)(object)spriteRenderer == (Object)null)
		{
			spriteRenderer = ((Component)this).GetComponent<SpriteRenderer>();
		}
		secondsPerFrame = 1f / (float)framesPerSecond;
		((MonoBehaviour)this).StartCoroutine(AnimateSprite());
	}

	private IEnumerator AnimateSprite()
	{
		while (isAnimating)
		{
			spriteRenderer.sprite = animationSprites[currentSpriteIndex];
			currentSpriteIndex = (currentSpriteIndex + 1) % animationSprites.Count;
			yield return (object)new WaitForSeconds(secondsPerFrame);
		}
	}

	private void OnDisable()
	{
		isAnimating = false;
	}
}
