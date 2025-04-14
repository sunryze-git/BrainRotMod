using UnityEngine;
using UnityEngine.Events;

public class ToyMonkeyTrap : Trap
{
	public UnityEvent toyMonkeyTimer;

	[Space]
	[Header("Components")]
	public Transform head;

	public Transform leftArm;

	public Transform rightArm;

	[Space]
	[Header("Sounds")]
	public Sound cymbal;

	public Sound mechanicalLoop;

	[Space]
	[Header("Animation")]
	public AnimationCurve armAnimationCurve;

	public AnimationCurve headAnimationCurve;

	private float armRotationLerp = 0.5f;

	private float headRotationXLerp;

	private float headRotationZLerp = 0.25f;

	private float headRotationSpeed = 4f;

	private float spinLerp;

	[Space]
	private Rigidbody rb;

	private bool trapPlaying;

	protected override void Start()
	{
		base.Start();
		rb = ((Component)this).GetComponent<Rigidbody>();
	}

	protected override void Update()
	{
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (trapStart)
		{
			ToyMonkeyTrapActivated();
		}
		mechanicalLoop.PlayLoop(trapPlaying, 0.8f, 0.8f);
		if (trapActive)
		{
			enemyInvestigate = true;
			trapPlaying = true;
			if (armRotationLerp < 1f)
			{
				armRotationLerp += Time.deltaTime * 3f;
			}
			if (armRotationLerp >= 1f)
			{
				armRotationLerp = 0f;
				Vector3 val = Vector3.Slerp(Vector3.up, ((Component)this).transform.right, 0.25f);
				cymbal.Play(physGrabObject.centerPoint);
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				_ = ((Vector3)(ref insideUnitSphere)).normalized;
				rb.AddForce(val * 1.3f, (ForceMode)1);
				rb.AddTorque(((Component)this).transform.up * Random.Range(-0.25f, 0.25f), (ForceMode)1);
			}
			float num = Mathf.Lerp(-15f, 40f, armAnimationCurve.Evaluate(armRotationLerp));
			leftArm.localEulerAngles = new Vector3(0f, num, 0f);
			rightArm.localEulerAngles = new Vector3(0f, 0f - num, 0f);
			if (headRotationXLerp < 1f)
			{
				headRotationXLerp += Time.deltaTime * headRotationSpeed;
			}
			if (headRotationXLerp >= 1f)
			{
				headRotationXLerp = 0f;
			}
			float num2 = Mathf.Lerp(-15f, 15f, headAnimationCurve.Evaluate(headRotationXLerp));
			if (headRotationZLerp < 1f)
			{
				headRotationZLerp += Time.deltaTime * headRotationSpeed;
			}
			if (headRotationZLerp >= 1f)
			{
				headRotationZLerp = 0f;
			}
			float num3 = Mathf.Lerp(-15f, 15f, headAnimationCurve.Evaluate(headRotationZLerp));
			head.localEulerAngles = new Vector3(num2, 0f, num3);
		}
	}

	private void FixedUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!trapActive || !isLocal)
		{
			return;
		}
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		Vector3 normalized = ((Vector3)(ref insideUnitSphere)).normalized;
		if (physGrabObject.playerGrabbing.Count == 0)
		{
			if (spinLerp < 1f)
			{
				spinLerp += Time.deltaTime;
			}
			float num = Mathf.Lerp(0f, 0.5f, spinLerp);
			rb.AddTorque(((Component)this).transform.right * num + normalized * 0.05f, (ForceMode)0);
		}
		else
		{
			spinLerp = 0f;
			rb.AddTorque(normalized * 0.5f, (ForceMode)0);
		}
	}

	public void ToyMonkeyTrapStop()
	{
		trapActive = false;
		trapPlaying = false;
	}

	public void ToyMonkeyTrapActivated()
	{
		if (!trapTriggered)
		{
			toyMonkeyTimer.Invoke();
			trapTriggered = true;
			trapActive = true;
		}
	}
}
