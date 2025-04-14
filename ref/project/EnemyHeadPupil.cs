using UnityEngine;

public class EnemyHeadPupil : MonoBehaviour
{
	public EnemyHeadEyeTarget EyeTarget;

	public bool Active = true;

	private void Update()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			((Component)this).transform.localScale = new Vector3(EyeTarget.PupilCurrentSize, ((Component)this).transform.localScale.y, EyeTarget.PupilCurrentSize);
		}
	}
}
