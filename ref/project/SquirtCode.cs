using UnityEngine;

public class SquirtCode : MonoBehaviour
{
	public Transform SquirtPlane1;

	public Transform SquirtPlane2;

	private Vector3 SquirtPlane1OriginalScale;

	private Vector3 SquirtPlane2OriginalScale;

	private Vector3 mainOriginalScale;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SquirtPlane1OriginalScale = SquirtPlane1.localScale;
		SquirtPlane2OriginalScale = SquirtPlane2.localScale;
		mainOriginalScale = ((Component)this).transform.localScale;
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.Rotate(Vector3.right * Time.deltaTime * 800f);
		SquirtPlane1.localScale = new Vector3(SquirtPlane1OriginalScale.x, SquirtPlane1OriginalScale.y, SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f) * 0.15f);
		SquirtPlane2.localScale = new Vector3(SquirtPlane1OriginalScale.x, SquirtPlane1OriginalScale.y, SquirtPlane1OriginalScale.z + Mathf.Sin(Time.time * 50f + 50f) * 0.15f);
		((Component)this).transform.localScale = new Vector3(mainOriginalScale.x + Mathf.Sin(Time.time * 50f) * 0.15f, mainOriginalScale.y, mainOriginalScale.z);
	}
}
