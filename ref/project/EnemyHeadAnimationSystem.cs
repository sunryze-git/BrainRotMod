using System;
using Photon.Pun;
using UnityEngine;

public class EnemyHeadAnimationSystem : MonoBehaviour
{
	private PhotonView PhotonView;

	public Enemy Enemy;

	public Animator Animator;

	public Materials.MaterialTrigger MaterialTrigger;

	public EnemyRigidbody EnemyRigidbody;

	public AnimatedOffset LookUnderOffset;

	public EnemyHeadFloat EnemyHeadFloat;

	public ParticleSystem TeleportParticlesTop;

	public ParticleSystem TeleportParticlesBot;

	public EnemyTriggerAttack EnemyTriggerAttack;

	[Space]
	public Sound ChaseBegin;

	public Sound ChaseBeginGlobal;

	public Sound ChaseBeginToChase;

	public Sound ChaseToIdle;

	public bool ChaseLoopActive;

	public Sound ChaseLoop;

	public Sound ChaseLoop2;

	public Sound TeethChatter;

	public Sound MoveLong;

	public Sound MoveShort;

	public Sound BiteStart;

	public Sound BiteEnd;

	public Sound Spawn;

	public Sound Despawn;

	public Sound Hurt;

	[Space]
	public float IdleTeethTimeMin;

	public float IdleTeethTimeMax;

	private float IdleTeethTime;

	private bool IdleTeethTrigger;

	private bool IdleBiteTrigger;

	private int AnimatorIdle;

	private int AnimatorChaseBegin;

	private int AnimatorChase;

	private int AnimatorIdleTeeth;

	private int AnimatorIdleBite;

	private int AnimatorChaseBite;

	private int AnimatorDespawn;

	private int AnimatorSpawn;

	private bool Idle;

	private void Awake()
	{
		PhotonView = ((Component)this).GetComponent<PhotonView>();
		Animator.keepAnimatorStateOnDisable = true;
		IdleTeethTime = Random.Range(IdleTeethTimeMin, IdleTeethTimeMax);
		AnimatorIdle = Animator.StringToHash("Idle");
		AnimatorIdleTeeth = Animator.StringToHash("IdleTeeth");
		AnimatorIdleBite = Animator.StringToHash("IdleBite");
		AnimatorChaseBite = Animator.StringToHash("ChaseBite");
		AnimatorChaseBegin = Animator.StringToHash("ChaseBegin");
		AnimatorChase = Animator.StringToHash("Chase");
		AnimatorDespawn = Animator.StringToHash("Despawn");
		AnimatorSpawn = Animator.StringToHash("Spawn");
	}

	public void SetChaseBeginToChase()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		ChaseBeginToChase.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void SetChaseToIdle()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		ChaseToIdle.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void SetChaseBegin()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		ChaseBegin.Play(((Component)this).transform.position);
		ChaseBeginGlobal.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void PlayTeethChatter()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TeethChatter.Play(((Component)this).transform.position);
	}

	public void MaterialImpact()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Materials.Instance.Impulse(((Component)this).transform.position, Vector3.down, Materials.SoundType.Heavy, footstep: false, MaterialTrigger, Materials.HostType.Enemy);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void Slide()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Materials.Instance.Slide(((Component)this).transform.position, MaterialTrigger, 1f, isPlayer: false);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void PlayMoveLong()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MoveLong.Play(((Component)this).transform.position);
	}

	public void PlayMoveShort()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MoveShort.Play(((Component)this).transform.position);
	}

	public void AttackStuckPhysObject()
	{
		if (!IdleBiteTrigger)
		{
			IdleBiteTrigger = true;
			IdleBite();
		}
	}

	private void Update()
	{
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy.CurrentState == EnemyState.Spawn || Enemy.CurrentState == EnemyState.Roaming || Enemy.CurrentState == EnemyState.Investigate || Enemy.CurrentState == EnemyState.ChaseEnd || Enemy.IsStunned())
		{
			if (Enemy.MasterClient && IdleTeethTime > 0f)
			{
				IdleTeethTime -= Time.deltaTime;
				if (IdleTeethTime <= 0f)
				{
					IdleTeeth();
					IdleTeethTime = Random.Range(IdleTeethTimeMin, IdleTeethTimeMax);
				}
			}
			if (IdleTeethTrigger)
			{
				Animator.SetTrigger(AnimatorIdleTeeth);
				IdleTeethTrigger = false;
			}
			Animator.SetBool(AnimatorIdle, true);
		}
		else
		{
			Animator.SetBool(AnimatorIdle, false);
			if (IdleTeethTime < IdleTeethTimeMin)
			{
				IdleTeethTime = Random.Range(IdleTeethTimeMin, IdleTeethTimeMax);
			}
		}
		if (Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			Animator.SetBool(AnimatorChaseBegin, true);
		}
		else
		{
			Animator.SetBool(AnimatorChaseBegin, false);
		}
		if (Enemy.CurrentState == EnemyState.Chase || Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			Animator.SetBool(AnimatorChase, true);
		}
		else
		{
			Animator.SetBool(AnimatorChase, false);
		}
		AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Chase Bite"))
		{
			EnemyTriggerAttack.Attack = false;
		}
		else if (EnemyTriggerAttack.Attack)
		{
			ChaseBiteTrigger();
		}
		if (Enemy.CurrentState == EnemyState.LookUnder && Enemy.StateLookUnder.WaitDone)
		{
			EnemyRigidbody.OverrideFollowPosition(0.1f, 50f);
			EnemyRigidbody.OverrideFollowRotation(0.1f, 2f);
			LookUnderOffset.Active(0.1f);
			EnemyHeadFloat.Disable(0.5f);
		}
		ChaseLoop.PlayLoop(ChaseLoopActive, 5f, 5f);
		ChaseLoop2.PlayLoop(ChaseLoopActive, 5f, 5f);
		if (Enemy.CurrentState == EnemyState.Despawn || Enemy.Health.dead)
		{
			currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Despawn"))
			{
				currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
				if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Chase Bite"))
				{
					Animator.SetTrigger(AnimatorDespawn);
				}
			}
		}
		if (Enemy.CurrentState == EnemyState.Spawn)
		{
			currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Spawn"))
			{
				Animator.SetTrigger(AnimatorSpawn);
			}
		}
	}

	private void IdleTeeth()
	{
		if (GameManager.instance.gameMode == 0)
		{
			IdleTeethRPC();
		}
		else
		{
			PhotonView.RPC("IdleTeethRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void IdleTeethRPC()
	{
		IdleTeethTrigger = true;
	}

	private void ChaseBiteTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			ChaseBiteTriggerRPC();
		}
		else
		{
			PhotonView.RPC("ChaseBiteTriggerRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void ChaseBiteTriggerRPC()
	{
		Animator.SetTrigger(AnimatorChaseBite);
	}

	public void PlayChaseBiteStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BiteStart.Play(((Component)this).transform.position);
	}

	public void PlayChaseBiteImpact()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Enemy.Rigidbody.GrabRelease();
		BiteEnd.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		EnemyRigidbody.DisableFollowPosition(0.25f, 1f);
		EnemyRigidbody.DisableFollowRotation(0.25f, 2f);
		EnemyRigidbody.rb.AddForce(((Component)EnemyRigidbody).transform.forward * 10f, (ForceMode)1);
	}

	public void PlayBiteStart()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BiteStart.Play(((Component)this).transform.position);
	}

	private void IdleBite()
	{
		if (GameManager.instance.gameMode == 0)
		{
			IdleBiteRPC();
		}
		else
		{
			PhotonView.RPC("IdleBiteRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void IdleBiteRPC()
	{
		Animator.SetTrigger(AnimatorIdleBite);
	}

	public void IdleBiteImpact()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		BiteEnd.Play(((Component)this).transform.position);
		EnemyRigidbody.DisableFollowPosition(1f, 1f);
		EnemyRigidbody.DisableFollowRotation(1f, 1f);
		EnemyRigidbody.rb.AddForce(((Component)EnemyRigidbody).transform.forward * 10f, (ForceMode)1);
	}

	public void IdleBiteDone()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Enemy.AttackStuckPhysObject.Reset();
		IdleBiteTrigger = false;
		EnemyRigidbody.DisableFollowPosition(0.2f, 1f);
		EnemyRigidbody.DisableFollowRotation(0.5f, 1f);
		EnemyRigidbody.rb.AddForce(-((Component)EnemyRigidbody).transform.forward * 2f, (ForceMode)1);
	}

	public void OnSpawn()
	{
		Animator.Play("Spawn", 0, 0f);
	}

	public void PlayDespawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Despawn.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void PlaySpawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Spawn.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
	}

	public void TeleportParticlesStart()
	{
		TeleportParticlesTop.Play();
		TeleportParticlesBot.Play();
	}

	public void TeleportParticlesStop()
	{
		TeleportParticlesTop.Stop();
		TeleportParticlesBot.Stop();
	}

	private void DespawnSet()
	{
		if (GameManager.instance.gameMode == 0)
		{
			DespawnSetRPC();
		}
		else
		{
			PhotonView.RPC("DespawnSetRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void DespawnSetRPC()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			Enemy.StateDespawn.Despawn();
		}
	}
}
