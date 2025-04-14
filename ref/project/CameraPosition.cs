using UnityEngine;

public class CameraPosition : MonoBehaviour
{
	public static CameraPosition instance;

	public Transform playerTransform;

	public Vector3 playerOffset;

	public CameraTarget camController;

	public float positionSmooth = 2f;

	private float tumbleSetTimer;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float num = positionSmooth;
		if (tumbleSetTimer > 0f)
		{
			num *= 0.5f;
			tumbleSetTimer -= Time.deltaTime;
		}
		Vector3 val = playerTransform.localPosition + playerOffset;
		if (SemiFunc.MenuLevel() && Object.op_Implicit((Object)(object)CameraNoPlayerTarget.instance))
		{
			val = ((Component)CameraNoPlayerTarget.instance).transform.position;
		}
		((Component)this).transform.localPosition = Vector3.Slerp(((Component)this).transform.localPosition, val, num * Time.deltaTime);
		((Component)this).transform.localRotation = Quaternion.Slerp(((Component)this).transform.localRotation, Quaternion.identity, num * Time.deltaTime);
		if (SemiFunc.MenuLevel())
		{
			((Component)this).transform.localPosition = val;
		}
	}

	public void TumbleSet()
	{
		tumbleSetTimer = 0.5f;
	}
}
