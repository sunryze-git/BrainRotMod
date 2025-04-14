using UnityEngine;

public class EnemyHeadHair : MonoBehaviour
{
	public Transform Target;

	public bool DebugShow;

	[Space]
	public float PositionSpeed;

	public float RotationSpeed;

	private Vector3 Scale;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Scale = ((Component)this).transform.localScale;
	}

	private void Update()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (PositionSpeed == 0f)
		{
			((Component)this).transform.position = Target.position;
		}
		else
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, Target.position, PositionSpeed * Time.deltaTime);
		}
		((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, Target.rotation, RotationSpeed * Time.deltaTime);
		((Component)this).transform.localScale = new Vector3(Scale.x * Target.lossyScale.x, Scale.y * Target.lossyScale.y, Scale.z * Target.lossyScale.z);
	}

	private void OnValidate()
	{
		if (!SemiFunc.OnValidateCheck())
		{
			MeshRenderer[] componentsInChildren = ((Component)this).gameObject.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Renderer)componentsInChildren[i]).enabled = DebugShow;
			}
		}
	}
}
