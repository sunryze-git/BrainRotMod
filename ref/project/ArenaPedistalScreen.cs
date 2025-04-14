using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaPedistalScreen : MonoBehaviour
{
	public TextMeshPro screenText;

	public GameObject glitchObject;

	public MeshRenderer glitchMeshRenderer;

	private float glitchTimer;

	public Light numberLight;

	public SpriteRenderer screenScanLines;

	private void Update()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (glitchTimer > 0f)
		{
			glitchTimer -= Time.deltaTime;
			float num = Mathf.Sin(Time.time * 100f) * 0.1f;
			float num2 = Mathf.Sin(Time.time * 100f) * 0.1f;
			((Renderer)glitchMeshRenderer).material.mainTextureOffset = new Vector2(num, num2);
		}
		else
		{
			glitchObject.SetActive(false);
		}
	}

	public void SwitchNumber(int number, bool finalPlayer = false)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)glitchMeshRenderer))
		{
			((TMP_Text)screenText).text = number.ToString();
			float num = Random.Range(0f, 100f);
			float num2 = Random.Range(0f, 100f);
			((Renderer)glitchMeshRenderer).material.mainTextureOffset = new Vector2(num, num2);
			glitchObject.SetActive(true);
			if (finalPlayer)
			{
				((Graphic)screenText).color = Color.green;
				numberLight.color = Color.green;
				Color color = default(Color);
				((Color)(ref color))._002Ector(0f, 1f, 0f, 0.65f);
				screenScanLines.color = color;
			}
			glitchTimer = 0.2f;
		}
	}
}
