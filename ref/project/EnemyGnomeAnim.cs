using UnityEngine;

public class EnemyGnomeAnim : MonoBehaviour
{
	public Enemy enemy;

	public EnemyGnome enemyGnome;

	internal Animator animator;

	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	private bool attackImpulse;

	private bool stunImpulse;

	private bool jumpImpulse;

	private bool landImpulse;

	internal bool idleBreakerImpulse;

	private bool noticeImpulse;

	[Space]
	public Sound soundFootstep;

	[Space]
	public Sound soundMoveShort;

	public Sound soundMoveLong;

	[Space]
	public Sound soundPickaxeTell;

	public Sound soundPickaxeHit;

	[Space]
	public Sound soundIdleBreaker;

	public Sound soundNotice;

	[Space]
	public Sound soundSpawn;

	public Sound soundDespawn;

	[Space]
	public Sound soundJump;

	public Sound soundLand;

	[Space]
	public Sound soundStun;

	public Sound soundStunOutro;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		else
		{
			animator.speed = 1f;
			if (enemyGnome.currentState == EnemyGnome.State.Stun)
			{
				animator.speed = Mathf.Clamp(((Vector3)(ref enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity)).magnitude, 1f, 3f);
			}
		}
		if (enemyGnome.currentState == EnemyGnome.State.Attack)
		{
			if (attackImpulse)
			{
				animator.SetTrigger("Attack");
				attackImpulse = false;
			}
		}
		else
		{
			attackImpulse = true;
		}
		bool flag = false;
		if (enemyGnome.currentState == EnemyGnome.State.Stun)
		{
			flag = true;
			landImpulse = false;
			if (stunImpulse)
			{
				soundStun.Play(enemy.CenterTransform.position);
				animator.SetTrigger("Stun");
				stunImpulse = false;
			}
			animator.SetBool("Stunned", true);
		}
		else
		{
			animator.SetBool("Stunned", false);
			stunImpulse = true;
		}
		if (!flag && enemy.Jump.jumping)
		{
			if (jumpImpulse)
			{
				animator.SetTrigger("Jump");
				animator.SetBool("Falling", false);
				jumpImpulse = false;
				landImpulse = true;
			}
			else if (enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity.y < 0f)
			{
				animator.SetBool("Falling", true);
			}
		}
		else
		{
			if (landImpulse)
			{
				animator.SetTrigger("Land");
				landImpulse = false;
			}
			animator.SetBool("Falling", false);
			jumpImpulse = true;
		}
		if (idleBreakerImpulse)
		{
			animator.SetTrigger("IdleBreaker");
			idleBreakerImpulse = false;
		}
		if (enemyGnome.currentState == EnemyGnome.State.Notice)
		{
			if (noticeImpulse)
			{
				animator.SetTrigger("Notice");
				noticeImpulse = false;
			}
		}
		else
		{
			noticeImpulse = true;
		}
		if (((Vector3)(ref enemyGnome.enemy.Rigidbody.physGrabObject.rbVelocity)).magnitude > 0.2f || ((Vector3)(ref enemyGnome.enemy.Rigidbody.physGrabObject.rbAngularVelocity)).magnitude > 0.5f)
		{
			animator.SetBool("Moving", true);
		}
		else
		{
			animator.SetBool("Moving", false);
		}
		if (enemyGnome.currentState == EnemyGnome.State.Despawn)
		{
			animator.SetBool("Despawning", true);
		}
		else
		{
			animator.SetBool("Despawning", false);
		}
	}

	public void OnSpawn()
	{
		animator.SetBool("Despawning", false);
		animator.SetBool("Stunned", false);
		animator.Play("Spawn", 0, 0f);
	}

	public void AttackDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemyGnome.UpdateState(EnemyGnome.State.AttackDone);
		}
	}

	public void Footstep()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		soundFootstep.Play(enemy.CenterTransform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, footstep: true, material, Materials.HostType.Enemy);
	}

	public void DespawnSet()
	{
		enemy.EnemyParent.Despawn();
	}

	public void MoveShort()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundMoveShort.Play(enemy.CenterTransform.position);
	}

	public void MoveLong()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundMoveLong.Play(enemy.CenterTransform.position);
	}

	public void PickaxeTell()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundPickaxeTell.Play(enemy.CenterTransform.position);
	}

	public void PickaxeHit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(2f, 2f, 5f, enemy.CenterTransform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 2f, 5f, enemy.CenterTransform.position, 0.05f);
		soundPickaxeHit.Play(enemy.CenterTransform.position);
	}

	public void IdleBreaker()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundIdleBreaker.Play(enemy.CenterTransform.position);
	}

	public void Notice()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundNotice.Play(enemy.CenterTransform.position);
	}

	public void Jump()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundJump.Play(enemy.CenterTransform.position);
	}

	public void Land()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundLand.Play(enemy.CenterTransform.position);
	}

	public void Spawn()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundSpawn.Play(enemy.CenterTransform.position);
	}

	public void Despawn()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundDespawn.Play(enemy.CenterTransform.position);
	}

	public void StunOutro()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundStunOutro.Play(enemy.CenterTransform.position);
	}
}
