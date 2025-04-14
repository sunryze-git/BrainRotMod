using System.Collections;
using TMPro;
using UnityEngine;

public class AnimateTextPeriods : MonoBehaviour
{
	private TextMeshProUGUI textMesh;

	private string textString;

	private Coroutine animateCoroutine;

	private void Awake()
	{
		textMesh = ((Component)this).GetComponent<TextMeshProUGUI>();
		textString = ((TMP_Text)textMesh).text;
	}

	private void OnEnable()
	{
		animateCoroutine = ((MonoBehaviour)this).StartCoroutine(AnimateDots());
	}

	private IEnumerator AnimateDots()
	{
		while (true)
		{
			((TMP_Text)textMesh).text = textString + "...".Substring(0, Mathf.FloorToInt(Time.unscaledTime * 8f % 4f));
			yield return null;
		}
	}

	private void OnDisable()
	{
		if (animateCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(animateCoroutine);
		}
	}
}
