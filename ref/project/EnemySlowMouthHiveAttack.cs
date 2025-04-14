using UnityEngine;

public class EnemySlowMouthHiveAttack : MonoBehaviour
{
	public Transform hitPositionTransform;

	public Transform blobTransform;

	public Transform blobMeshTransform;

	public AnimationCurve flyUpCurve;

	private float curveProgress;

	private Vector3 prevCheckPosition;

	public EnemyParent enemyParent;

	private void Start()
	{
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		Vector3 position2 = hitPositionTransform.position;
		if (curveProgress < 1f)
		{
			curveProgress += Time.deltaTime * 2f;
			Vector3 position3 = Vector3.Lerp(position, position2, curveProgress);
			blobTransform.position = position3;
			float num = flyUpCurve.Evaluate(curveProgress);
			Transform obj = blobTransform;
			obj.position += Vector3.up * 2f * num;
			if (!(Vector3.Distance(prevCheckPosition, blobTransform.position) > 0.5f))
			{
				return;
			}
			Collider[] array = Physics.OverlapSphere(blobTransform.position, blobMeshTransform.localScale.x / 2f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetShouldHits()));
			for (int i = 0; i < array.Length; i++)
			{
				EnemyParent componentInParent = ((Component)array[i]).GetComponentInParent<EnemyParent>();
				if (!Object.op_Implicit((Object)(object)componentInParent) || !((Object)(object)componentInParent == (Object)(object)enemyParent))
				{
					Splat();
					break;
				}
			}
			prevCheckPosition = blobTransform.position;
		}
		else
		{
			curveProgress = 0f;
		}
	}

	private void Splat()
	{
		curveProgress = 0f;
	}
}
