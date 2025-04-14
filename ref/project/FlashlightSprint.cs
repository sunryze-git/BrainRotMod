using UnityEngine;

public class FlashlightSprint : MonoBehaviour
{
	public float Offset;

	public float Speed;

	public PlayerAvatar PlayerAvatar;

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerAvatar.isLocal)
		{
			if (PlayerController.instance.CanSlide)
			{
				((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, new Vector3(0f, 0f, Offset * GameplayManager.instance.cameraAnimation), Speed * Time.deltaTime);
			}
			else
			{
				((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, new Vector3(0f, 0f, 0f), Speed * Time.deltaTime);
			}
		}
	}
}
