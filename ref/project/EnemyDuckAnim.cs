using Photon.Pun;
using UnityEngine;

public class EnemyDuckAnim : MonoBehaviour
{
	public Enemy enemy;

	internal Animator animator;

	public EnemyDuck controller;

	public Sound quackSound;

	public Sound stunSound;

	public Sound stunStopSound;

	public Sound biteSound;

	public Sound transformSound;

	public Sound jumpSound;

	public Sound footstepSound;

	public Sound mouthExtendSound;

	public Sound mouthRetractSound;

	public Sound attackLoopSound;

	public Sound hurtSound;

	public Sound deathSound;

	public Sound noticeSound;

	public Sound flyFlapSound;

	public Sound flyLoopSound;

	public float soundHurtPauseTimer;

	private bool jumpImpulse;

	private bool flyImpulse;

	private bool landImpulse;

	private bool stunImpulse;

	private bool noticeImpulse;

	private bool transformImpulse;

	private float idleBreakerTimer;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		AnimatorStateInfo currentAnimatorStateInfo;
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		else
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Walk") && !animator.IsInTransition(0))
			{
				animator.speed = Mathf.Clamp(((Vector3)(ref enemy.Rigidbody.velocity)).magnitude + 0.2f, 0.8f, 1.2f);
			}
			else
			{
				animator.speed = 1f;
			}
		}
		if (controller.currentState != EnemyDuck.State.AttackStart && controller.currentState != EnemyDuck.State.Transform && controller.currentState != EnemyDuck.State.ChaseNavmesh && controller.currentState != EnemyDuck.State.ChaseTowards && controller.currentState != EnemyDuck.State.ChaseMoveBack && controller.currentState != EnemyDuck.State.DeTransform)
		{
			if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.1f)
			{
				animator.SetBool("move", true);
			}
			else
			{
				animator.SetBool("move", false);
			}
			if (!enemy.IsStunned())
			{
				if (!enemy.Grounded.grounded && (controller.currentState == EnemyDuck.State.FlyBackToNavmesh || controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop))
				{
					if (flyImpulse)
					{
						animator.SetTrigger("fly");
						animator.SetBool("falling", false);
						flyImpulse = false;
						landImpulse = true;
					}
					else if (controller.currentState == EnemyDuck.State.FlyBackToNavmeshStop)
					{
						animator.SetBool("falling", true);
					}
				}
				else if (enemy.Jump.jumping)
				{
					if (jumpImpulse)
					{
						animator.SetTrigger("jump");
						animator.SetBool("falling", false);
						jumpImpulse = false;
						landImpulse = true;
					}
					else if (controller.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
					{
						animator.SetBool("falling", true);
					}
				}
				else
				{
					if (landImpulse)
					{
						animator.SetTrigger("land");
						landImpulse = false;
					}
					animator.SetBool("falling", false);
					jumpImpulse = true;
					flyImpulse = true;
				}
			}
		}
		if (controller.currentState == EnemyDuck.State.AttackStart)
		{
			if (transformImpulse)
			{
				animator.SetTrigger("transform");
				transformImpulse = false;
			}
		}
		else
		{
			transformImpulse = true;
		}
		if (controller.currentState == EnemyDuck.State.AttackStart || controller.currentState == EnemyDuck.State.Transform || controller.currentState == EnemyDuck.State.ChaseNavmesh || controller.currentState == EnemyDuck.State.ChaseTowards || controller.currentState == EnemyDuck.State.ChaseMoveBack)
		{
			animator.SetBool("move", false);
			animator.SetBool("chase", true);
			if (soundHurtPauseTimer > 0f)
			{
				StopAttackSound();
			}
			else
			{
				attackLoopSound.PlayLoop(playing: true, 5f, 5f);
			}
		}
		else
		{
			StopAttackSound();
			animator.SetBool("chase", false);
		}
		if (controller.currentState == EnemyDuck.State.Notice)
		{
			if (noticeImpulse)
			{
				animator.SetTrigger("notice");
				noticeImpulse = false;
			}
		}
		else
		{
			noticeImpulse = true;
		}
		currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Idle") && !animator.IsInTransition(0))
		{
			idleBreakerTimer += Time.deltaTime;
			if (idleBreakerTimer > 5f)
			{
				idleBreakerTimer = 0f;
				if (Random.Range(0, 100) < 35)
				{
					controller.IdleBreakerSet();
				}
			}
		}
		if (controller.idleBreakerTrigger)
		{
			animator.SetTrigger("idlebreak");
			controller.idleBreakerTrigger = false;
		}
		if (controller.currentState == EnemyDuck.State.Stun)
		{
			landImpulse = false;
			if (stunImpulse)
			{
				animator.SetTrigger("stun");
				stunImpulse = false;
			}
			animator.SetBool("stunned", true);
			if (soundHurtPauseTimer > 0f)
			{
				stunSound.PlayLoop(playing: false, 5f, 2f);
			}
			else
			{
				stunSound.PlayLoop(playing: true, 5f, 5f);
			}
		}
		else
		{
			animator.SetBool("stunned", false);
			stunSound.PlayLoop(playing: false, 5f, 1f);
			if (!stunImpulse)
			{
				stunStopSound.Play(((Component)this).transform.position);
				stunImpulse = true;
			}
		}
		if (controller.currentState == EnemyDuck.State.Despawn)
		{
			animator.SetBool("despawning", true);
		}
		else
		{
			animator.SetBool("despawning", false);
		}
		if (controller.currentState == EnemyDuck.State.FlyBackToNavmesh)
		{
			flyLoopSound.PlayLoop(playing: true, 5f, 2f);
		}
		else
		{
			flyLoopSound.PlayLoop(playing: false, 5f, 2f);
		}
		if (soundHurtPauseTimer > 0f)
		{
			soundHurtPauseTimer -= Time.deltaTime;
		}
	}

	public void OnSpawn()
	{
		animator.Play("Spawn", 0, 0f);
	}

	private void Quack()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		quackSound.Play(((Component)this).transform.position);
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && controller.currentState != EnemyDuck.State.Idle && controller.currentState != EnemyDuck.State.Roam && controller.currentState != EnemyDuck.State.Investigate && controller.currentState != EnemyDuck.State.Leave && controller.currentState != EnemyDuck.State.MoveBackToNavmesh)
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 10f);
		}
	}

	private void BiteSound()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.Rigidbody.GrabRelease();
		}
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		biteSound.Play(((Component)this).transform.position);
	}

	private void TransformSound()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			if (Vector3.Distance(((Component)this).transform.position, ((Component)Camera.main).transform.position) < 10f)
			{
				AudioScare.instance.PlayImpact();
			}
			transformSound.Play(((Component)this).transform.position);
		}
	}

	private void JumpSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		jumpSound.Play(((Component)this).transform.position);
	}

	private void FootstepSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		footstepSound.Play(((Component)this).transform.position);
	}

	private void MouthExtendSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mouthExtendSound.Play(((Component)this).transform.position);
	}

	private void MouthRetractSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		mouthRetractSound.Play(((Component)this).transform.position);
	}

	private void StopAttackSound()
	{
		attackLoopSound.PlayLoop(playing: false, 5f, 5f);
	}

	private void NoticeSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		noticeSound.Play(((Component)this).transform.position);
	}

	private void FlyFlapSound()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		flyFlapSound.Play(((Component)this).transform.position);
	}

	public void Despawn()
	{
		enemy.EnemyParent.Despawn();
	}
}
