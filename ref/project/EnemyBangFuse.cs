using UnityEngine;

public class EnemyBangFuse : MonoBehaviour
{
	internal EnemyBang controller;

	internal bool setup;

	private bool active;

	private float delayTimer;

	public GameObject tipObject;

	public Transform glowTransform;

	private float glowLerp = 1f;

	private float glowSpeed;

	private Vector3 glowTargetOld;

	private Vector3 glowTargetNew;

	[Space]
	public Transform particleParent;

	public ParticleSystem particleFire;

	public ParticleSystem particleSpark;

	private float sparkTimer;

	[Space]
	public AnimationCurve glowCurve;

	public AnimationCurve stiffCurve;

	public AnimationCurve shrinkCurve;

	[Space]
	public SpringQuaternion botSpring;

	public Transform botTransformSource;

	public Transform botTransformTarget;

	[Space]
	public SpringQuaternion topSpring;

	public Transform topTransformSource;

	public Transform topTransformTarget;

	private void Awake()
	{
		tipObject.SetActive(false);
		delayTimer = Random.Range(0.25f, 0.6f);
	}

	private void Update()
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		if (!setup)
		{
			return;
		}
		if (controller.fuseActive)
		{
			if (!active)
			{
				if (delayTimer <= 0f)
				{
					tipObject.SetActive(true);
					particleFire.Play();
					SparkPlay();
					active = true;
				}
				else
				{
					delayTimer -= Time.deltaTime;
				}
			}
		}
		else if (active)
		{
			tipObject.SetActive(false);
			particleFire.Stop();
			active = false;
			delayTimer = Random.Range(0.25f, 0.6f);
		}
		float num = stiffCurve.Evaluate(controller.fuseLerp);
		float num2 = shrinkCurve.Evaluate(controller.fuseLerp);
		Vector3 direction = botTransformTarget.position - (botTransformTarget.position + Vector3.up);
		direction = SemiFunc.ClampDirection(direction, ((Component)this).transform.forward, Mathf.Lerp(30f, 0f, num));
		botTransformTarget.rotation = Quaternion.RotateTowards(botTransformTarget.rotation, Quaternion.LookRotation(direction), 800f * Time.deltaTime);
		Vector3 direction2 = topTransformTarget.position - (topTransformTarget.position + Vector3.up);
		direction2 = SemiFunc.ClampDirection(direction2, botTransformTarget.forward, Mathf.Lerp(90f, 0f, num));
		topTransformTarget.rotation = Quaternion.RotateTowards(topTransformTarget.rotation, Quaternion.LookRotation(direction2), 800f * Time.deltaTime);
		botTransformSource.rotation = SemiFunc.SpringQuaternionGet(botSpring, botTransformTarget.rotation);
		topTransformSource.rotation = SemiFunc.SpringQuaternionGet(topSpring, topTransformTarget.rotation);
		botSpring.damping = Mathf.Lerp(0.3f, 0.8f, num);
		botSpring.speed = Mathf.Lerp(10f, 15f, num);
		botSpring.maxAngle = Mathf.Lerp(90f, 5f, num);
		topSpring.damping = Mathf.Lerp(0.3f, 0.8f, num);
		topSpring.speed = Mathf.Lerp(10f, 15f, num);
		topSpring.maxAngle = Mathf.Lerp(90f, 5f, num);
		botTransformSource.localPosition = Vector3.Lerp(Vector3.zero, -Vector3.forward * 0.1f, num2);
		if (active)
		{
			controller.anim.FuseLoop();
			particleParent.position = tipObject.transform.position;
			glowLerp += glowSpeed * Time.deltaTime;
			glowLerp = Mathf.Clamp01(glowLerp);
			if (glowLerp >= 1f)
			{
				glowTargetOld = glowTransform.localScale;
				glowTargetNew = Vector3.one * Random.Range(0.75f, 1.25f);
				glowSpeed = Random.Range(5f, 30f);
				glowLerp = 0f;
			}
			glowTransform.localScale = Vector3.Lerp(glowTargetOld, glowTargetNew, glowCurve.Evaluate(glowLerp));
			if (controller.fuseLerp >= controller.explosionTellFuseThreshold)
			{
				if (sparkTimer <= 0f)
				{
					sparkTimer = 0.2f;
					SparkPlay();
				}
				else
				{
					sparkTimer -= Time.deltaTime;
				}
			}
			else
			{
				sparkTimer = 0f;
			}
		}
		else
		{
			glowTargetOld = Vector3.one * 5f;
			glowTargetNew = Vector3.one;
			glowSpeed = 5f;
			glowLerp = 0f;
		}
	}

	public void SparkPlay()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		particleSpark.Play();
		controller.anim.soundFuseIgnite.Play(tipObject.transform.position);
	}

	private void OnDisable()
	{
		particleFire.Stop();
	}
}
