using UnityEngine;

public class PhysGrabPointRotate : MonoBehaviour
{
	[HideInInspector]
	public PhysGrabber physGrabber;

	private Quaternion smoothRotation;

	[HideInInspector]
	public float rotationActiveTimer;

	private float rotationSpeed;

	private float offsetX;

	private AnimationCurve popIn;

	private AnimationCurve popOut;

	private MeshRenderer meshRenderer;

	public Material originalMaterial;

	public Material greenScreenMaterial;

	[HideInInspector]
	public float animationEval;

	private void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		popIn = AssetManager.instance.animationCurveWooshIn;
		popOut = AssetManager.instance.animationCurveWooshAway;
		((Component)this).transform.localScale = Vector3.zero;
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		((Renderer)meshRenderer).material = originalMaterial;
	}

	private void OnEnable()
	{
		if (!Object.op_Implicit((Object)(object)meshRenderer))
		{
			meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		}
		if (Object.op_Implicit((Object)(object)meshRenderer))
		{
			if (!Object.op_Implicit((Object)(object)VideoGreenScreen.instance))
			{
				((Renderer)meshRenderer).material = originalMaterial;
			}
			else
			{
				((Renderer)meshRenderer).material = greenScreenMaterial;
			}
		}
	}

	private void Update()
	{
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)physGrabber))
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)physGrabber))
		{
			Vector3 mouseTurningVelocity = physGrabber.mouseTurningVelocity;
			if (physGrabber.isRotating)
			{
				rotationActiveTimer = 0.1f;
			}
			if (rotationActiveTimer > 0f)
			{
				physGrabber.OverrideColorToPurple();
				((Component)this).transform.LookAt(physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform.position);
				animationEval += Time.deltaTime * 2f;
				animationEval = Mathf.Clamp(animationEval, 0f, 1f);
				float num = popIn.Evaluate(animationEval);
				((Component)this).transform.localScale = Vector3.one * 0.5f * num;
				((Component)this).transform.Rotate(0f, 0f, (0f - Mathf.Atan2(mouseTurningVelocity.y, mouseTurningVelocity.x)) * 57.29578f);
				smoothRotation = Quaternion.Slerp(smoothRotation, ((Component)this).transform.rotation, Time.deltaTime * 10f);
				((Component)this).transform.rotation = smoothRotation;
				rotationActiveTimer -= Time.deltaTime;
			}
			else
			{
				animationEval -= Time.deltaTime * 6f;
				animationEval = Mathf.Clamp(animationEval, 0f, 1f);
				float num2 = popOut.Evaluate(1f - animationEval);
				Vector3 val = Vector3.one * 0.5f;
				((Component)this).transform.localScale = val - val * num2;
			}
		}
		Vector3 localScale = ((Component)this).transform.localScale;
		if (((Vector3)(ref localScale)).magnitude < 0.01f)
		{
			((Renderer)meshRenderer).enabled = false;
		}
		else
		{
			((Renderer)meshRenderer).enabled = true;
		}
		float magnitude = ((Vector3)(ref physGrabber.mouseTurningVelocity)).magnitude;
		offsetX -= magnitude * 0.2f * Time.deltaTime;
		((Component)this).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, 0f);
		float num3 = Mathf.Sin(Time.time * 10f) * 0.2f;
		((Component)this).GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f, 1f + num3);
		((Component)this).GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, (0f - num3) * 0.5f);
	}
}
