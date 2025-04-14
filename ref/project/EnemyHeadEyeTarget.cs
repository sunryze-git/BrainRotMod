using UnityEngine;

public class EnemyHeadEyeTarget : MonoBehaviour
{
	public Enemy Enemy;

	public Transform Follow;

	[Space]
	public Vector3 Limit;

	public float Speed;

	public bool DebugShow;

	[Space]
	public bool Idle = true;

	public float IdleOffset;

	[Space]
	public float PupilSizeMultiplier;

	public float PupilSizeSpeed;

	public float PupilMinSize;

	public float PupilMaxSize;

	[HideInInspector]
	public float PupilCurrentSize;

	private Camera Camera;

	private void Start()
	{
		Camera = Camera.main;
	}

	private void Update()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!Enemy.CheckChase())
		{
			Idle = true;
		}
		else
		{
			Idle = false;
		}
		if (Idle || !Object.op_Implicit((Object)(object)Enemy.TargetPlayerAvatar))
		{
			((Component)this).transform.position = Follow.position + Follow.forward * IdleOffset;
		}
		else
		{
			((Component)this).transform.position = Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position;
		}
		((Component)this).transform.rotation = Follow.rotation;
		float num = Vector3.Distance(((Component)Enemy).transform.position, ((Component)Camera).transform.position) * PupilSizeMultiplier;
		num = Mathf.Clamp(num, PupilMinSize, PupilMaxSize);
		PupilCurrentSize = Mathf.Lerp(PupilCurrentSize, num, PupilSizeSpeed * Time.deltaTime);
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
