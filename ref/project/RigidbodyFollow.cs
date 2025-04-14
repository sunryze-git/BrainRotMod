using UnityEngine;

public class RigidbodyFollow : MonoBehaviour
{
	public Transform Target;

	public bool Scale;

	private Rigidbody Rigidbody;

	private void Start()
	{
		Rigidbody = ((Component)this).GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody.position = Target.position;
		Rigidbody.rotation = Target.rotation;
		if (Scale)
		{
			((Component)this).transform.localScale = Target.localScale;
		}
	}
}
