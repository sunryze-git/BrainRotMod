using UnityEngine;

public class DebugMovement : MonoBehaviour
{
	public float speed = 1f;

	public float leftRight = 1f;

	public float upDown = 1f;

	private Vector3 startPos;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		startPos = ((Component)this).transform.position;
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = startPos + new Vector3(Mathf.Sin(Time.time * speed), Mathf.Cos(Time.time * 0.5f * speed), Mathf.Cos(Time.time * 0.25f * speed));
	}
}
