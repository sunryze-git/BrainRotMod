using UnityEngine;

public class EnemyAnimalAnim : MonoBehaviour
{
	private Animator animator;

	public Enemy enemy;

	public EnemyAnimal controller;

	public Materials.MaterialTrigger material;

	private bool stun;

	private bool attack;

	private bool previousAttack;

	private float attackPitch;

	[Space]
	public ParticleSystem particleBits;

	public ParticleSystem particleImpact;

	public ParticleSystem particleDirectionalBits;

	public ParticleSystem particleLegBits;

	public ParticleSystem particleDig;

	[Space]
	public Sound stepSound;

	public Sound stompSound;

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
	public Sound attackStartSound;

	public Sound attackLoop;

	public Sound attackStopSound;

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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Attack") && !animator.IsInTransition(0))
		{
			animator.speed = Mathf.Clamp(((Vector3)(ref enemy.Rigidbody.velocity)).magnitude / 5f, 0.6f, 2f);
			attackPitch = Mathf.Clamp(((Vector3)(ref enemy.Rigidbody.velocity)).magnitude / 4.75f, 0.75f, 1.25f);
			attack = true;
		}
		else
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Walk") && !animator.IsInTransition(0))
			{
				animator.speed = Mathf.Clamp(((Vector3)(ref enemy.Rigidbody.velocity)).magnitude / 2f, 0.8f, 2f);
			}
			else
			{
				animator.speed = 1f;
				attack = false;
			}
		}
		attackLoop.PlayLoop(attack, 5f, 5f, attackPitch);
		if (!previousAttack && attack)
		{
			attackStartSound.Play(((Component)this).transform.position);
			previousAttack = true;
		}
		if (previousAttack && !attack)
		{
			attackStopSound.Play(((Component)this).transform.position);
			previousAttack = false;
		}
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		if (Vector3.Dot(((Component)enemy.Rigidbody).transform.up, Vector3.up) > 0.6f)
		{
			animator.SetBool("upright", true);
		}
		else
		{
			animator.SetBool("upright", false);
		}
		if (enemy.Rigidbody.velocity.y < -2f)
		{
			animator.SetBool("falling", true);
		}
		else
		{
			animator.SetBool("falling", false);
		}
		if (enemy.Jump.jumping)
		{
			animator.SetBool("jump", true);
		}
		else
		{
			animator.SetBool("jump", false);
		}
		if (enemy.IsStunned())
		{
			stun = true;
			animator.SetBool("stun", true);
		}
		else
		{
			stun = false;
			animator.SetBool("stun", false);
		}
		if (stun)
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Stun"))
			{
				animator.SetTrigger("Stun Impulse");
			}
		}
		if (enemy.CurrentState == EnemyState.Despawn)
		{
			animator.SetBool("despawn", true);
		}
		else
		{
			animator.SetBool("despawn", false);
		}
		if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.2f)
		{
			animator.SetBool("move", true);
		}
		else
		{
			animator.SetBool("move", false);
		}
		if (controller.currentState == EnemyAnimal.State.WreakHavoc)
		{
			animator.SetBool("attack", true);
		}
		else
		{
			animator.SetBool("attack", false);
		}
	}

	public void SetSpawn()
	{
		stun = false;
		animator.Play("Spawn", 0, 0f);
	}

	public void SetDespawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void NoticeSet(int _playerID)
	{
		animator.SetTrigger("Notice");
	}

	public void MaterialImpactShake()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		stompSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, footstep: false, material, Materials.HostType.Enemy);
	}

	public void ImpactLight()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Light, footstep: false, material, Materials.HostType.Enemy);
		}
	}

	public void ImpactMedium()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Medium, footstep: false, material, Materials.HostType.Enemy);
		}
	}

	public void ImpactHeavy()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Grounded.grounded)
		{
			Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, footstep: false, material, Materials.HostType.Enemy);
		}
	}

	public void ImpactFootstep()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Grounded.grounded)
		{
			stepSound.Play(((Component)this).transform.position);
			Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, footstep: true, material, Materials.HostType.Enemy);
		}
	}

	public void Dig()
	{
		particleDig.Play();
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

	public void StunStop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		attackStopSound.Play(((Component)this).transform.position);
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
}
