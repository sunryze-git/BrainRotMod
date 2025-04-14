using UnityEngine;

public class SpringTentacle : MonoBehaviour
{
	public SpringQuaternion springStart;

	public Transform springStartTarget;

	public Transform springStartSource;

	[Space]
	public SpringQuaternion springMid;

	public Transform springMidTarget;

	public Transform springMidSource;

	[Space]
	public SpringQuaternion springEnd;

	public Transform springEndTarget;

	public Transform springEndSource;

	private float offsetX;

	private float offsetY;

	private void Start()
	{
		offsetX = Random.Range(0f, 100f);
		offsetY = Random.Range(0f, 100f);
	}

	private void Update()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		((Component)springStartTarget).transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f + offsetX) * 5f, Mathf.Sin(Time.time * 5f + offsetY) * 10f, 0f);
		springStartSource.rotation = SemiFunc.SpringQuaternionGet(springStart, ((Component)springStartTarget).transform.rotation);
		springMidSource.rotation = SemiFunc.SpringQuaternionGet(springMid, ((Component)springMidTarget).transform.rotation);
		springEndSource.rotation = SemiFunc.SpringQuaternionGet(springEnd, ((Component)springEndTarget).transform.rotation);
	}
}
