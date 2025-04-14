using UnityEngine;

public class FlashlightLightAim : MonoBehaviour
{
	public PlayerAvatar playerAvatar;

	public Vector3 clientAimPoint;

	private Vector3 clientAimPointCurrent;

	private Light lightComponent;

	private bool setBias;

	private void Start()
	{
		lightComponent = ((Component)this).GetComponent<Light>();
		if (!playerAvatar.isLocal)
		{
			lightComponent.shadowBias = 0.1f;
		}
	}

	private void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		clientAimPointCurrent = Vector3.Lerp(clientAimPointCurrent, clientAimPoint, Time.deltaTime * 20f);
		RaycastHit val = default(RaycastHit);
		if (!playerAvatar.isLocal)
		{
			Vector3 direction = clientAimPointCurrent - ((Component)this).transform.position;
			direction = SemiFunc.ClampDirection(direction, ((Component)this).transform.parent.forward, 45f);
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
		}
		else if (Physics.Raycast(((Component)this).transform.position, ((Component)this).transform.forward, ref val, 100f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())) && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val)).transform).GetComponentInParent<PlayerController>()))
		{
			clientAimPoint = ((RaycastHit)(ref val)).point;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
		Gizmos.DrawSphere(clientAimPointCurrent, 0.1f);
	}
}
