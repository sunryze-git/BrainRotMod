using UnityEngine;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
	public static FadeOverlay Instance;

	public Image Image;

	[Space]
	public AnimationCurve IntroCurve;

	public float IntroSpeed;

	private float IntroLerp;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.Start || GameDirector.instance.currentState == GameDirector.gameState.Outro || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			((Graphic)Image).color = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, byte.MaxValue));
			return;
		}
		IntroLerp += Time.deltaTime * IntroSpeed;
		float num = IntroCurve.Evaluate(IntroLerp);
		((Graphic)Image).color = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)(255f * num)));
	}
}
