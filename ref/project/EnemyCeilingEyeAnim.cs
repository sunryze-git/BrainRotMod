using UnityEngine;

public class EnemyCeilingEyeAnim : MonoBehaviour
{
	public EnemyCeilingEye controller;

	private Animator animator;

	public Enemy enemy;

	public ParticleScriptExplosion particleScriptExplosion;

	public ParticleSystem TeleportParticles;

	public ParticleSystem particleImpact;

	public ParticleSystem particleBits;

	[Header("Sounds")]
	public Sound sfxBlink;

	public Sound sfxDespawn;

	public Sound sfxSpawn;

	public Sound sfxDeath;

	public Sound sfxLaserBuildup;

	public Sound sfxLaserBeam;

	public AudioClip sfxStaringStart;

	public Sound sfxStareLoop;

	public Sound sfxTwitchLoop;

	private bool isPlayingTwitchLoop = true;

	private bool isPlayingStaringLoop = true;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		if (controller.currentState == EnemyCeilingEye.State.HasTarget || controller.currentState == EnemyCeilingEye.State.TargetLost)
		{
			animator.SetBool("hasTarget", true);
		}
		else
		{
			animator.SetBool("hasTarget", false);
		}
		if (controller.enemy.CurrentState == EnemyState.Despawn || controller.currentState == EnemyCeilingEye.State.Move)
		{
			animator.SetBool("despawn", true);
		}
		else
		{
			animator.SetBool("despawn", false);
		}
		SfxStaringLoop();
		if (controller.deathImpulse)
		{
			controller.deathImpulse = false;
			animator.SetTrigger("Death");
		}
	}

	public void SetSpawn()
	{
		animator.Play("Ceiling Eye Spawn", 0, 0f);
	}

	public void SetAttack()
	{
		animator.Play("Ceiling Eye Attack", 0, 0f);
	}

	public void SetDespawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (controller.enemy.CurrentState == EnemyState.Despawn)
			{
				controller.enemy.EnemyParent.Despawn();
			}
			else
			{
				controller.OnSpawn();
			}
		}
	}

	public void AttackFinished()
	{
		controller.enemy.EnemyParent.SpawnedTimerSet(0f);
	}

	public void Explosion()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, -0.5f, 0f);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position + val, Vector3.down, ref val2, 30f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())))
		{
			particleScriptExplosion.Spawn(((RaycastHit)(ref val2)).point, 2f, 50, 50);
		}
	}

	public void DeathEffect()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		particleImpact.Play();
		particleBits.Play();
		sfxDeath.Play(((Component)this).transform.position);
	}

	public void TeleportParticlesStart()
	{
		TeleportParticles.Play();
	}

	public void TeleportParticlesStop()
	{
		TeleportParticles.Stop();
	}

	public void SfxBlink()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxBlink.Play(((Component)this).transform.position);
	}

	public void SfxDespawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxDespawn.Play(((Component)this).transform.position);
	}

	public void SfxSpawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxSpawn.Play(((Component)this).transform.position);
	}

	public void SfxLaserBuildup()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxLaserBuildup.Play(((Component)this).transform.position);
	}

	public void SfxLaserBeam()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		sfxLaserBeam.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void SfxStaringStart()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (controller.currentState == EnemyCeilingEye.State.HasTarget && controller.targetPlayer.isLocal)
		{
			AudioScare.instance.PlayCustom(sfxStaringStart);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void SfxStaringLoop()
	{
		sfxStareLoop.PlayLoop(isPlayingStaringLoop, 0.05f, 0.25f);
		sfxTwitchLoop.PlayLoop(isPlayingTwitchLoop, 0.1f, 0.25f);
		if (controller.currentState == EnemyCeilingEye.State.HasTarget)
		{
			isPlayingStaringLoop = true;
		}
		else
		{
			isPlayingStaringLoop = false;
		}
		if (controller.currentState == EnemyCeilingEye.State.HasTarget && controller.targetPlayer.isLocal)
		{
			isPlayingTwitchLoop = true;
		}
		else
		{
			isPlayingTwitchLoop = false;
		}
	}
}
