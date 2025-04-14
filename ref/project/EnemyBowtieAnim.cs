using Photon.Pun;
using UnityEngine;

public class EnemyBowtieAnim : MonoBehaviour
{
	public Transform followTarget;

	public EnemyBowtie controller;

	public Enemy enemy;

	public Materials.MaterialTrigger material;

	internal Animator animator;

	private bool attackImpulse;

	private bool stunImpulse;

	private bool jumpImpulse;

	private bool landImpulse;

	private bool noticeImpulse;

	[Space]
	public ParticleSystem particleBits;

	public ParticleSystem particleImpact;

	public ParticleSystem particleDirectionalBits;

	public ParticleSystem particleEyes;

	public ParticleSystem particleDespawnSpark;

	public ParticleSystem particleYell;

	public ParticleSystem particleYellSmall;

	public ParticleSystem particleStompL;

	public ParticleSystem particleStompR;

	private float soundStunPauseTimer;

	private float soundGroanPauseTimer;

	[Space]
	public Sound footstepSound;

	public Sound footstepSmallSound;

	[Space]
	public Sound moveShortSound;

	public Sound moveLongSound;

	public Sound GroanLoopSound;

	[Space]
	public Sound despawnSound;

	public Sound despawnSparkSound;

	[Space]
	public Sound jumpSound;

	public Sound landSound;

	public Sound noticeSound;

	[Space]
	public Sound yellStartSound;

	public Sound yellStartSoundGlobal;

	public Sound yellEndSound;

	public Sound yellEndSoundGlobal;

	public Sound YellLoopSound;

	public Sound YellLoopSoundGlobal;

	private bool yell;

	[Space]
	public Sound stunSound;

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
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
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
		if (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.1f)
		{
			animator.SetBool("move", true);
		}
		else
		{
			animator.SetBool("move", false);
		}
		if (controller.currentState == EnemyBowtie.State.Leave)
		{
			animator.SetBool("leaving", true);
		}
		else
		{
			animator.SetBool("leaving", false);
		}
		if (controller.currentState == EnemyBowtie.State.Idle || controller.currentState == EnemyBowtie.State.Roam || controller.currentState == EnemyBowtie.State.Investigate)
		{
			if (soundGroanPauseTimer > 0f)
			{
				StopGroaning();
			}
			else
			{
				GroanLoopSound.PlayLoop(playing: true, 5f, 5f);
			}
		}
		else
		{
			StopGroaning();
		}
		if (soundGroanPauseTimer > 0f)
		{
			soundGroanPauseTimer -= Time.deltaTime;
		}
		if (controller.currentState == EnemyBowtie.State.Yell)
		{
			animator.SetBool("yell", true);
			yell = true;
		}
		else
		{
			animator.SetBool("yell", false);
			yell = false;
			particleYell.Stop();
			particleYellSmall.Stop();
		}
		YellLoopSound.PlayLoop(yell, 5f, 5f);
		YellLoopSoundGlobal.PlayLoop(yell, 5f, 5f);
		if (controller.currentState == EnemyBowtie.State.Despawn)
		{
			animator.SetBool("despawn", true);
			particleYell.Stop();
			particleYellSmall.Stop();
		}
		else
		{
			animator.SetBool("despawn", false);
		}
		bool flag = false;
		if (controller.currentState == EnemyBowtie.State.Stun)
		{
			flag = true;
			stunSound.PlayLoop(playing: true, 2f, 5f);
			landImpulse = false;
			if (stunImpulse)
			{
				StopGroaning();
				stunSound.Play(enemy.CenterTransform.position);
				animator.SetTrigger("Stun Impulse");
				stunImpulse = false;
			}
			animator.SetBool("stunned", true);
			if (soundStunPauseTimer > 0f)
			{
				StopStunSound();
			}
			else
			{
				stunSound.PlayLoop(playing: true, 2f, 5f);
			}
		}
		else
		{
			StopStunSound();
			animator.SetBool("stunned", false);
			stunImpulse = true;
		}
		if (soundStunPauseTimer > 0f)
		{
			soundStunPauseTimer -= Time.deltaTime;
		}
		if (!flag && enemy.Jump.jumping)
		{
			if (jumpImpulse)
			{
				animator.SetTrigger("Jump Impulse");
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
				animator.SetTrigger("Land Impulse");
				landImpulse = false;
			}
			animator.SetBool("falling", false);
			jumpImpulse = true;
		}
	}

	public void OnSpawn()
	{
		animator.SetBool("stunned", false);
		animator.Play("Spawn", 0, 0f);
	}

	public void NoticeSet(int _playerID)
	{
		animator.SetTrigger("Notice");
	}

	public void DespawnStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		despawnSound.Play(((Component)this).transform.position);
	}

	public void Despawn()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		particleDespawnSpark.Play();
		despawnSparkSound.Play(((Component)this).transform.position);
		enemy.EnemyParent.Despawn();
	}

	public void Footstep()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		footstepSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, footstep: true, material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.1f);
	}

	public void FootstepSmall()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		footstepSmallSound.Play(((Component)this).transform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint, Vector3.down, Materials.SoundType.Heavy, footstep: true, material, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(1.5f, 3f, 10f, ((Component)this).transform.position, 0.3f);
		GameDirector.instance.CameraImpact.ShakeDistance(1.5f, 3f, 10f, ((Component)this).transform.position, 0.1f);
	}

	public void StompLeft()
	{
		particleStompL.Play();
	}

	public void StompRight()
	{
		particleStompR.Play();
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

	public void Notice()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		noticeSound.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void YellStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		yellStartSound.Play(((Component)this).transform.position);
		yellStartSoundGlobal.Play(((Component)this).transform.position);
		particleYell.Play();
		particleYellSmall.Play();
	}

	public void yellShake()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 12f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 12f, ((Component)this).transform.position, 0.1f);
	}

	public void EnemyInvestigate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 15f);
		}
	}

	public void YellStop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		yellEndSound.Play(((Component)this).transform.position);
		yellEndSoundGlobal.Play(((Component)this).transform.position);
	}

	public void StopStunSound()
	{
		stunSound.PlayLoop(playing: false, 2f, 5f);
	}

	public void StopGroaning()
	{
		GroanLoopSound.PlayLoop(playing: false, 5f, 5f);
	}

	public void StunPause()
	{
		soundStunPauseTimer = 0.5f;
	}

	public void GroanPause()
	{
		soundGroanPauseTimer = 1f;
	}
}
