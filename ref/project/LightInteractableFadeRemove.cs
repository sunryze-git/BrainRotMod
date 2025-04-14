using UnityEngine;

public class LightInteractableFadeRemove : MonoBehaviour
{
	public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public float fadeDuration = 2f;

	private Light lightComponent;

	private float currentTime;

	private bool isFading;

	private void Start()
	{
		lightComponent = ((Component)this).GetComponent<Light>();
	}

	private void Update()
	{
		if (isFading)
		{
			currentTime += Time.deltaTime;
			float num = currentTime / fadeDuration;
			lightComponent.intensity = fadeCurve.Evaluate(num) * lightComponent.intensity;
			if (currentTime >= fadeDuration)
			{
				Object.Destroy((Object)(object)lightComponent);
				Object.Destroy((Object)(object)this);
			}
		}
	}

	public void StartFading()
	{
		isFading = true;
	}
}
