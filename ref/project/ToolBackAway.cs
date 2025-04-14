using UnityEngine;

public class ToolBackAway : MonoBehaviour
{
	public bool Active;

	public Transform ParentTransform;

	private LayerMask Mask;

	private float RaycastTime = 0.1f;

	private float RaycastTimer;

	public float Length = 1f;

	private float LengthHit;

	private float BackAwayAmount;

	public float BackAwayAmountMax;

	private Vector3 StartPosition;

	public float springFreq = 15f;

	public float springDamping = 0.5f;

	private float target;

	private float current;

	private float velocity;

	private SpringUtils.tDampedSpringMotionParams springParams = new SpringUtils.tDampedSpringMotionParams();

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		StartPosition = ((Component)this).transform.localPosition;
		Mask = SemiFunc.LayerMaskGetVisionObstruct();
	}

	private void FixedUpdate()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!Active)
		{
			return;
		}
		if (RaycastTimer <= 0f)
		{
			RaycastTimer = RaycastTime;
			LengthHit = Length;
			RaycastHit[] array = Physics.RaycastAll(ParentTransform.position, ParentTransform.forward, Length, LayerMask.op_Implicit(Mask));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val = array[i];
				if ((!((Component)((RaycastHit)(ref val)).transform).CompareTag("Player") || !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val)).transform).GetComponent<PlayerController>())) && ((RaycastHit)(ref val)).distance < LengthHit)
				{
					LengthHit = ((RaycastHit)(ref val)).distance;
				}
			}
		}
		else
		{
			RaycastTimer -= Time.fixedDeltaTime;
		}
	}

	private void Update()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			BackAwayAmount = Mathf.Max(0f - BackAwayAmountMax, LengthHit - Length);
		}
		else
		{
			BackAwayAmount = 0f;
		}
		SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.deltaTime, springFreq, springDamping);
		SpringUtils.UpdateDampedSpringMotion(ref current, ref velocity, BackAwayAmount, in springParams);
		((Component)this).transform.localPosition = new Vector3(StartPosition.x, StartPosition.y, StartPosition.z + current);
	}
}
