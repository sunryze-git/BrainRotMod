using UnityEngine;

public class PlayerAvatarMenu : MonoBehaviour
{
	public static PlayerAvatarMenu instance;

	public Transform cameraAndStuff;

	private MenuPage parentPage;

	private Vector3 startPosition;

	internal Rigidbody rb;

	private Vector3 rotationForce;

	internal PlayerAvatarVisuals playerVisuals;

	private void Awake()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		startPosition = new Vector3(0f, 0f, -2000f);
		if (Object.op_Implicit((Object)(object)instance) && instance.startPosition == startPosition)
		{
			startPosition = new Vector3(0f, 4f, -2000f);
		}
		playerVisuals = ((Component)this).GetComponentInChildren<PlayerAvatarVisuals>();
		instance = this;
	}

	private void Start()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		rb = ((Component)this).GetComponent<Rigidbody>();
		parentPage = ((Component)this).GetComponentInParent<MenuPage>();
		((Component)this).transform.SetParent((Transform)null);
		((Component)this).transform.localScale = Vector3.one;
		((Component)this).transform.position = startPosition;
		cameraAndStuff.SetParent((Transform)null);
		cameraAndStuff.localScale = Vector3.one;
	}

	private void FixedUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		rb.MovePosition(startPosition);
		if (((Vector3)(ref rotationForce)).magnitude > 0.1f)
		{
			rb.AddTorque(rotationForce * Time.fixedDeltaTime);
			rotationForce = Vector3.zero;
			rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, 1f);
		}
	}

	private void Update()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.InputMovementX() > 0.01f || SemiFunc.InputMovementX() < -0.01f)
		{
			float num = (0f - SemiFunc.InputMovementX()) * 3000f;
			Rotate(new Vector3(0f, num, 0f));
		}
		if (!Object.op_Implicit((Object)(object)parentPage))
		{
			Object.Destroy((Object)(object)((Component)cameraAndStuff).gameObject);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	public void Rotate(Vector3 _rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		rotationForce = _rotation;
	}
}
