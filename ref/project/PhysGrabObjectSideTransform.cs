using UnityEngine;

public class PhysGrabObjectSideTransform : MonoBehaviour
{
	[HideInInspector]
	public Vector3 prevPosition;

	[HideInInspector]
	public float velocity;

	private float velocityResetTimer;

	private float impactTimer;

	private MeshRenderer meshRenderer;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		prevPosition = ((Component)this).transform.position;
	}

	private void FixedUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(prevPosition, ((Component)this).transform.position) / Time.fixedDeltaTime * 5f;
		if (num > velocity)
		{
			velocity = num;
			velocityResetTimer = 0.1f;
		}
		if (velocityResetTimer > 0f)
		{
			velocityResetTimer -= Time.fixedDeltaTime;
		}
		else
		{
			velocity = 0f;
		}
		prevPosition = ((Component)this).transform.position;
	}
}
