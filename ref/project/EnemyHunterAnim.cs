using System.Collections.Generic;
using UnityEngine;

public class EnemyHunterAnim : MonoBehaviour
{
	public Enemy enemy;

	public EnemyHunter enemyHunter;

	internal Animator animator;

	public Materials.MaterialTrigger material;

	private float moveTimer;

	private bool stunImpulse;

	internal bool spawnImpulse;

	[Space]
	public List<ParticleSystem> teleportEffects;

	[Space]
	public Sound soundFootstepShort;

	public Sound soundFootstepLong;

	public Sound soundReload01;

	public Sound soundAimStart;

	public Sound soundAimStartGlobal;

	public Sound soundReload02;

	public Sound soundMoveShort;

	public Sound soundMoveLong;

	public Sound soundGunLong;

	public Sound soundGunShort;

	public Sound soundSpawn;

	public Sound soundDespawn;

	public Sound soundLeaveStart;

	[Space]
	public AudioClip[] aimStartClips;

	public AudioClip[] aimStartGlobalClips;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		else
		{
			animator.speed = 1f;
		}
		if ((enemyHunter.currentState == EnemyHunter.State.Roam || enemyHunter.currentState == EnemyHunter.State.InvestigateWalk || enemyHunter.currentState == EnemyHunter.State.Leave) && (((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.2f || ((Vector3)(ref enemy.Rigidbody.physGrabObject.rbAngularVelocity)).magnitude > 0.25f))
		{
			moveTimer = 0.1f;
		}
		if (moveTimer > 0f)
		{
			moveTimer -= Time.deltaTime;
			animator.SetBool("Moving", true);
		}
		else
		{
			animator.SetBool("Moving", false);
		}
		if (enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			animator.SetBool("Leaving", true);
		}
		else
		{
			animator.SetBool("Leaving", false);
		}
		if (enemyHunter.currentState == EnemyHunter.State.Stun)
		{
			if (stunImpulse)
			{
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
		if (enemyHunter.currentState == EnemyHunter.State.Aim)
		{
			animator.SetBool("Aiming", true);
		}
		else
		{
			animator.SetBool("Aiming", false);
		}
		if (enemyHunter.currentState == EnemyHunter.State.Shoot || enemyHunter.currentState == EnemyHunter.State.ShootEnd)
		{
			animator.SetBool("Shooting", true);
		}
		else
		{
			animator.SetBool("Shooting", false);
		}
		if (enemyHunter.currentState == EnemyHunter.State.Despawn)
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
		animator.SetBool("Stunned", false);
		animator.Play("Spawn", 0, 0f);
	}

	public void TeleportEffect()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 10f, ((Component)this).transform.position, 0.05f);
		foreach (ParticleSystem teleportEffect in teleportEffects)
		{
			teleportEffect.Play();
		}
	}

	public void FootstepShort()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		soundFootstepShort.Play(enemy.CenterTransform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
	}

	public void FootstepLong()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		soundFootstepLong.Play(enemy.CenterTransform.position);
		Materials.Instance.Impulse(enemy.Rigidbody.physGrabObject.centerPoint + Vector3.down * 1f, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
	}

	public void AimStart()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		int num = Random.Range(0, aimStartClips.Length);
		soundAimStart.Sounds[0] = aimStartClips[num];
		soundAimStartGlobal.Sounds[0] = aimStartGlobalClips[num];
		soundAimStart.Play(enemy.CenterTransform.position);
		soundAimStartGlobal.Play(enemy.CenterTransform.position);
	}

	public void Despawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void Reload01()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundReload01.Play(enemy.CenterTransform.position);
	}

	public void Reload02()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundReload02.Play(enemy.CenterTransform.position);
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

	public void GunLong()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundGunLong.Play(enemy.CenterTransform.position);
	}

	public void GunShort()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundGunShort.Play(enemy.CenterTransform.position);
	}

	public void Spawn()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundSpawn.Play(enemy.CenterTransform.position);
	}

	public void DespawnSound()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundDespawn.Play(enemy.CenterTransform.position);
	}

	public void LeaveStartSound()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		soundLeaveStart.Play(enemy.CenterTransform.position);
	}

	public void LeaveStartDone()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && enemyHunter.currentState == EnemyHunter.State.LeaveStart)
		{
			enemyHunter.stateTimer = 0f;
		}
	}
}
