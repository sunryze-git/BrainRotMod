using UnityEngine;

public class FlashlightBob : MonoBehaviour
{
	public PlayerAvatar PlayerAvatar;

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerAvatar.isLocal)
		{
			Vector3 positionResult = CameraBob.Instance.positionResult;
			((Component)this).transform.localPosition = new Vector3((0f - positionResult.y) * 0.2f, 0f, 0f);
			((Component)this).transform.localRotation = Quaternion.Euler(0f, positionResult.y * 30f, 0f);
		}
	}
}
