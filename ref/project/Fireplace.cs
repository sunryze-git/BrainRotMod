using UnityEngine;

public class Fireplace : MonoBehaviour
{
	public bool isLit;

	public bool isCornerFireplace;

	public GameObject fire;

	private Vector3 fireOffset = new Vector3(0f, 0.028f, 0.249f);

	private void Awake()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (isLit)
		{
			GameObject val = Object.Instantiate<GameObject>(fire, ((Component)this).transform.position, ((Component)this).transform.rotation);
			val.transform.parent = ((Component)this).transform;
			val.transform.localRotation = Quaternion.identity;
			if (isCornerFireplace)
			{
				fireOffset = new Vector3(0.824f, 0.028f, 0.824f);
				Transform transform = val.transform;
				transform.localRotation *= Quaternion.Euler(0f, 45f, 0f);
			}
			val.transform.localPosition = fireOffset;
		}
	}
}
