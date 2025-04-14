using UnityEngine;
using UnityEngine.UI;

public class FadeOverlayOver : MonoBehaviour
{
	public static FadeOverlayOver Instance;

	public Image Image;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState == GameDirector.gameState.Load || GameDirector.instance.currentState == GameDirector.gameState.End || GameDirector.instance.currentState == GameDirector.gameState.EndWait)
		{
			((Graphic)Image).color = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, byte.MaxValue));
		}
		else
		{
			((Graphic)Image).color = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)0));
		}
	}
}
