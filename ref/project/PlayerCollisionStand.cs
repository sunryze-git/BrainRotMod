using UnityEngine;

public class PlayerCollisionStand : MonoBehaviour
{
	public static PlayerCollisionStand instance;

	public PlayerCollisionController CollisionController;

	private CapsuleCollider Collider;

	public LayerMask LayerMask;

	public Transform TargetTransform;

	public Vector3 Offset;

	private bool checkActive;

	private float setBlockedTimer;

	private void Awake()
	{
		instance = this;
		Collider = ((Component)this).GetComponent<CapsuleCollider>();
	}

	public bool CheckBlocked()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (setBlockedTimer > 0f)
		{
			return true;
		}
		Vector3 val = ((Component)this).transform.position + Offset + Vector3.up * Collider.radius;
		Vector3 val2 = ((Component)this).transform.position + Offset + Vector3.up * Collider.height - Vector3.up * Collider.radius;
		if (Physics.OverlapCapsule(val, val2, Collider.radius, LayerMask.op_Implicit(LayerMask)).Length != 0)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (setBlockedTimer > 0f)
		{
			setBlockedTimer -= Time.deltaTime;
		}
		((Component)this).transform.position = TargetTransform.position;
	}

	public void SetBlocked()
	{
		setBlockedTimer = 0.25f;
		PlayerCollision.instance.SetCrouchCollision();
	}
}
