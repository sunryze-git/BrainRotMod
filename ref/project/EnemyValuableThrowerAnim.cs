using UnityEngine;

public class EnemyValuableThrowerAnim : MonoBehaviour
{
	public Transform followTarget;

	public EnemyValuableThrower controller;

	public Enemy enemy;

	public Materials.MaterialTrigger material;

	internal Animator animator;

	[Space]
	public ParticleSystem particleBits;

	public ParticleSystem particleImpact;

	public ParticleSystem particleDirectionalBits;

	private bool stun;

	[Space]
	public Sound footstepSound;

	public Sound footstepSmallSound;

	[Space]
	public Sound moveShortSound;

	public Sound moveLongSound;

	[Space]
	public Sound spawnSound;

	public Sound despawnSound;

	[Space]
	public Sound jumpSound;

	public Sound landSound;

	public Sound noticeSound;

	[Space]
	public Sound pickupIntroSound;

	public Sound pickupOutroTellSound;

	public Sound pickupOutroThrowSound;

	[Space]
	public Sound stunSound;

	public Sound stunStopSound;

	[Space]
	public Sound hurtSound;

	public Sound deathSound;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		else
		{
			animator.speed = 1f;
		}
		((Component)this).transform.position = followTarget.position;
		((Component)this).transform.rotation = followTarget.rotation;
		if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.5f)
		{
			animator.SetBool("Move", true);
			animator.SetBool("Move Slow", false);
		}
		else if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.2f)
		{
			animator.SetBool("Move", false);
			animator.SetBool("Move Slow", true);
		}
		else
		{
			animator.SetBool("Move", false);
			animator.SetBool("Move Slow", false);
		}
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			stun = false;
			animator.SetBool("Despawn", true);
		}
		else
		{
			animator.SetBool("Despawn", false);
		}
		if (controller.currentState == EnemyValuableThrower.State.PickUpTarget || controller.currentState == EnemyValuableThrower.State.TargetPlayer)
		{
			animator.SetBool("Pickup", true);
		}
		else
		{
			animator.SetBool("Pickup", false);
		}
		if (enemy.Jump.jumping)
		{
			animator.SetBool("Jumping", true);
		}
		else
		{
			animator.SetBool("Jumping", false);
		}
		if (enemy.IsStunned())
		{
			stun = true;
			animator.SetBool("Stun", true);
		}
		else
		{
			stun = false;
			animator.SetBool("Stun", false);
		}
		if (stun)
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Stun"))
			{
				animator.SetTrigger("Stun Impulse");
			}
		}
		stunSound.PlayLoop(stun, 10f, 10f);
	}

	public void OnSpawn()
	{
		stun = false;
		animator.SetBool("Stun", false);
		animator.Play("Spawn", 0, 0f);
	}

	public void NoticeSet(int _playerID)
	{
		animator.SetTrigger("Notice");
	}

	public void ResetStateTimer()
	{
		controller.ResetStateTimer();
	}

	public void SpawnStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		spawnSound.Play(((Component)this).transform.position);
	}

	public void DespawnStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		despawnSound.Play(((Component)this).transform.position);
	}

	public void Despawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void Throw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		pickupOutroThrowSound.Play(((Component)this).transform.position);
		controller.Throw();
	}

	public void Footstep()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		footstepSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
	}

	public void FootstepSmall()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		footstepSmallSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, footstep: true, material, Materials.HostType.Enemy);
	}

	public void MoveShort()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		moveShortSound.Play(((Component)this).transform.position);
	}

	public void MoveLong()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		moveLongSound.Play(((Component)this).transform.position);
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

	public void PickupIntro()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		pickupIntroSound.Play(((Component)this).transform.position);
	}

	public void PickupOutro()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		pickupOutroTellSound.Play(((Component)this).transform.position);
	}

	public void StunStop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		stunStopSound.Play(((Component)this).transform.position);
	}

	public void Notice()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		noticeSound.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}
}
