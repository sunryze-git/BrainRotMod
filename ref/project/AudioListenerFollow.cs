using UnityEngine;

public class AudioListenerFollow : MonoBehaviour
{
	public static AudioListenerFollow instance;

	public Transform TargetPositionTransform;

	public Transform TargetRotationTransform;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		TargetPositionTransform = ((Component)Camera.main).transform;
		TargetRotationTransform = ((Component)Camera.main).transform;
	}

	private void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)TargetPositionTransform))
		{
			if (Object.op_Implicit((Object)(object)SpectateCamera.instance) && SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				((Component)this).transform.position = TargetPositionTransform.position;
			}
			else
			{
				((Component)this).transform.position = TargetPositionTransform.position + TargetPositionTransform.forward * AssetManager.instance.mainCamera.nearClipPlane;
			}
			if (Object.op_Implicit((Object)(object)TargetRotationTransform))
			{
				((Component)this).transform.rotation = TargetRotationTransform.rotation;
			}
		}
	}
}
