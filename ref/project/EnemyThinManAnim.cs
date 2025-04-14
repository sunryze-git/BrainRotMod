using System;
using UnityEngine;

public class EnemyThinManAnim : MonoBehaviour
{
	internal Animator animator;

	public EnemyThinMan controller;

	public Enemy enemy;

	public GameObject mesh;

	public Light backLight;

	public GameObject tentaclesParent;

	public GameObject extendedTentacles;

	public GameObject extendedPouch;

	public float tentacleSpeed;

	public AnimationCurve tentacleCurve;

	private float[] randomOffsets;

	private bool tentacleBackActive;

	public GameObject tentacleR1;

	public GameObject tentacleR2;

	public GameObject tentacleR3;

	public GameObject tentacleL1;

	public GameObject tentacleL2;

	public GameObject tentacleL3;

	internal bool rattleImpulse;

	public ParticleSystem particleSmoke;

	public ParticleSystem particleSmokeCalmFill;

	public ParticleSystem particleImpact;

	public ParticleSystem particleDirectionalBits;

	[Space]
	public Sound teleportIn;

	public Sound teleportOut;

	[Space]
	public Sound notice;

	[Space]
	public Sound growLoop;

	public Sound zoom;

	public Sound attack;

	public Sound screamLocal;

	public Sound screamGlobal;

	[Space]
	public Sound hurtSound;

	public Sound deathSound;

	private bool despawnImpulse;

	private bool stunImpulse;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
		randomOffsets = new float[6];
		for (int i = 0; i < randomOffsets.Length; i++)
		{
			randomOffsets[i] = Random.Range(0f, MathF.PI * 2f);
		}
	}

	private void Update()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		growLoop.PlayLoop(tentaclesParent.activeSelf, 5f, 5f);
		if (controller.tentacleLerp > 0f)
		{
			backLight.intensity = tentacleCurve.Evaluate(controller.tentacleLerp) * 4f;
		}
		else
		{
			backLight.intensity = 0f;
		}
		if (controller.tentacleLerp > 0f)
		{
			if (!tentaclesParent.activeSelf)
			{
				tentaclesParent.SetActive(true);
			}
			tentaclesParent.transform.localScale = new Vector3(tentacleCurve.Evaluate(controller.tentacleLerp), tentacleCurve.Evaluate(controller.tentacleLerp), tentacleCurve.Evaluate(controller.tentacleLerp));
		}
		else if (tentaclesParent.activeSelf)
		{
			tentaclesParent.SetActive(false);
		}
		if (controller.tentacleLerp > 0f)
		{
			float num = controller.tentacleLerp * 20f;
			float num2 = Mathf.Lerp(10f, 1f, controller.tentacleLerp);
			tentacleR1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[0]) * num, 0f, 0f);
			tentacleR2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[1]) * num, 0f, 0f);
			tentacleR3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[2]) * num, 0f, 0f);
			tentacleL1.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[3]) * num, 0f, 0f);
			tentacleL2.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[4]) * num, 0f, 0f);
			tentacleL3.transform.localRotation = Quaternion.Euler(Mathf.Sin(num2 + randomOffsets[5]) * num, 0f, 0f);
		}
		if (controller.currentState == EnemyThinMan.State.TentacleExtend || controller.currentState == EnemyThinMan.State.Damage)
		{
			if (!extendedTentacles.activeSelf)
			{
				tentaclesParent.SetActive(false);
				extendedPouch.SetActive(true);
				particleSmoke.Play();
				attack.Play(((Component)this).transform.position);
				GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)controller.playerTarget).transform.position, 0.3f);
				GameDirector.instance.CameraImpact.ShakeDistance(10f, 3f, 8f, ((Component)controller.playerTarget).transform.position, 0.1f);
				extendedTentacles.SetActive(true);
			}
			if (Object.op_Implicit((Object)(object)controller.playerTarget))
			{
				float num3 = Vector3.Distance(((Component)controller.playerTarget).transform.position, extendedTentacles.transform.position);
				extendedTentacles.transform.localScale = new Vector3(1f, 1f, num3);
				extendedTentacles.transform.LookAt(controller.playerTarget.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (extendedTentacles.activeSelf)
		{
			Vector3 localScale = extendedTentacles.transform.localScale;
			localScale.z = Mathf.Lerp(localScale.z, 0f, 10f * Time.deltaTime);
			extendedTentacles.transform.localScale = localScale;
			if (extendedTentacles.transform.localScale.z <= 0.1f)
			{
				extendedTentacles.SetActive(false);
				extendedPouch.SetActive(false);
			}
		}
		if (rattleImpulse)
		{
			if (enemy.Health.healthCurrent > 0)
			{
				int num4 = Random.Range(1, 3);
				animator.SetTrigger("Rattle" + num4);
			}
			rattleImpulse = false;
		}
		if (enemy.CurrentState == EnemyState.Despawn && enemy.Health.healthCurrent > 0)
		{
			animator.SetBool("Despawn", true);
			if (despawnImpulse)
			{
				animator.SetTrigger("DespawnTrigger");
				despawnImpulse = false;
			}
		}
		else
		{
			animator.SetBool("Despawn", false);
			despawnImpulse = true;
		}
		if (enemy.IsStunned() && enemy.CurrentState != EnemyState.Despawn && enemy.Health.healthCurrent > 0)
		{
			animator.SetBool("Stun", true);
			if (stunImpulse)
			{
				animator.SetTrigger("StunTrigger");
				stunImpulse = false;
			}
		}
		else
		{
			animator.SetBool("Stun", false);
			stunImpulse = true;
		}
	}

	public void NoticeSet()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Health.healthCurrent < 0)
		{
			return;
		}
		if (controller.playerTarget.isLocal)
		{
			float num = 30f;
			if (Vector3.Distance(((Component)controller.playerTarget).transform.position, ((Component)enemy).transform.position) > 5f)
			{
				num = 20f;
			}
			CameraGlitch.Instance.PlayShort();
			CameraAim.Instance.AimTargetSet(controller.head.transform.position, 0.75f, 2f, ((Component)controller).gameObject, 90);
			CameraZoom.Instance.OverrideZoomSet(num, 0.75f, 3f, 1f, ((Component)controller).gameObject, 50);
			zoom.Play(((Component)this).transform.position);
		}
		animator.SetTrigger("Scream");
	}

	public void OnDeath()
	{
		animator.SetTrigger("Death");
	}

	public void Scream()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		screamLocal.Play(((Component)this).transform.position);
		screamGlobal.Play(((Component)this).transform.position);
	}

	public void DeathEffect()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		particleImpact.Play();
		Quaternion rotation = Quaternion.LookRotation(-((Vector3)(ref enemy.Health.hurtDirection)).normalized);
		((Component)particleDirectionalBits).transform.rotation = rotation;
		particleDirectionalBits.Play();
		deathSound.Play(((Component)this).transform.position);
		enemy.EnemyParent.Despawn();
	}

	public void SetDespawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void DespawnSmoke()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		controller.SmokeEffect(controller.rb.position);
		teleportOut.Play(((Component)this).transform.position);
	}
}
