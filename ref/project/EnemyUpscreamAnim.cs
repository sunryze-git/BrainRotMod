using UnityEngine;

public class EnemyUpscreamAnim : MonoBehaviour
{
	public Enemy enemy;

	public EnemyUpscream controller;

	internal Animator animator;

	public Materials.MaterialTrigger material;

	private bool idleBreakImpulse;

	private bool stunImpulse;

	private bool jumpImpulse;

	public Sound sfxAttackLocal;

	public Sound sfxAttackGlobal;

	public Sound hurtSound;

	public Sound jumpSound;

	public Sound landSound;

	public Sound stepSound;

	public Sound sfxIdleBreak;

	public Sound despawnSound;

	public bool isPlayingTargetPlayerLoop;

	private float currentSpeed;

	private float moveTimer;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		SetAnimationSpeed();
		if (controller.enemy.CurrentState == EnemyState.Despawn)
		{
			animator.SetBool("despawn", true);
		}
		else
		{
			animator.SetBool("despawn", false);
		}
		if (controller.currentState == EnemyUpscream.State.IdleBreak)
		{
			if (idleBreakImpulse)
			{
				animator.SetTrigger("IdleBreak");
				idleBreakImpulse = false;
			}
		}
		else
		{
			idleBreakImpulse = true;
		}
		if (enemy.Jump.jumping)
		{
			animator.SetBool("jumping", true);
			if (jumpImpulse)
			{
				animator.SetTrigger("Jump");
				animator.SetBool("falling", false);
				jumpImpulse = false;
			}
		}
		else
		{
			animator.SetBool("jumping", false);
			jumpImpulse = true;
		}
		if (enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
		{
			animator.SetBool("falling", true);
		}
		else
		{
			animator.SetBool("falling", false);
		}
		if (((Vector3)(ref enemy.Rigidbody.physGrabObject.rbVelocity)).magnitude > 0.1f || ((Vector3)(ref enemy.Rigidbody.physGrabObject.rbAngularVelocity)).magnitude > 0.5f)
		{
			moveTimer = 0.2f;
		}
		if (moveTimer > 0f)
		{
			moveTimer -= Time.deltaTime;
			animator.SetBool("move", true);
		}
		else
		{
			animator.SetBool("move", false);
		}
		if (controller.currentState == EnemyUpscream.State.Stun)
		{
			if (stunImpulse)
			{
				animator.SetTrigger("Stun");
				stunImpulse = false;
			}
			animator.SetBool("stunned", true);
		}
		else
		{
			animator.SetBool("stunned", false);
			stunImpulse = true;
		}
	}

	public void SetSpawn()
	{
		animator.Play("Spawn", 0, 0f);
	}

	public void SetDespawn()
	{
		controller.UpdateState(EnemyUpscream.State.Spawn);
		controller.enemy.EnemyParent.Despawn();
	}

	public void NoticeSet(int _playerID)
	{
		animator.SetTrigger("Notice");
	}

	public void TeleportParticlesStart()
	{
	}

	public void TeleportParticlesStop()
	{
	}

	public void SfxImpactFootstep()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Grounded.grounded)
		{
			stepSound.Play(((Component)this).transform.position);
			Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
		}
	}

	public void SfxIdleBreak()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxIdleBreak.Play(((Component)this).transform.position);
	}

	public void SfxAttack()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		sfxAttackLocal.Play(((Component)this).transform.position);
		sfxAttackGlobal.Play(((Component)this).transform.position);
	}

	public void Jump()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		jumpSound.Play(((Component)this).transform.position);
	}

	public void Land()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		landSound.Play(((Component)this).transform.position);
	}

	public void DespawnSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		despawnSound.Play(((Component)this).transform.position);
	}

	public void AttackImpulse()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if ((!SemiFunc.IsMultiplayer() || SemiFunc.IsMasterClient()) && Object.op_Implicit((Object)(object)controller.targetPlayer))
		{
			Vector3 val = ((Component)controller.targetPlayer).transform.position - ((Component)this).transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			normalized = Vector3.Lerp(normalized, Vector3.up, 0.6f);
			controller.targetPlayer.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			controller.targetPlayer.tumble.TumbleForce(normalized * 45f);
			controller.targetPlayer.tumble.TumbleTorque(-((Component)controller.targetPlayer).transform.right * 45f);
			controller.targetPlayer.tumble.TumbleOverrideTime(3f);
			controller.targetPlayer.tumble.ImpactHurtSet(3f, 10);
		}
	}

	private void SetAnimationSpeed()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Walk"))
		{
			float num = ((Vector3)(ref enemy.Rigidbody.physGrabObject.rbVelocity)).magnitude + ((Vector3)(ref enemy.Rigidbody.physGrabObject.rbAngularVelocity)).magnitude;
			num = Mathf.Clamp(num, 0.5f, 4f);
			animator.speed = num * 0.6f;
		}
		else
		{
			animator.speed = 1f;
		}
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
	}
}
