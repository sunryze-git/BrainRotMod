using UnityEngine;

public class EnemyFloaterAnim : MonoBehaviour
{
	private int animStunned = Animator.StringToHash("stunned");

	private int animDespawning = Animator.StringToHash("despawning");

	private int animAttacking = Animator.StringToHash("attacking");

	private int animStun = Animator.StringToHash("Stun");

	private int animNotice = Animator.StringToHash("Notice");

	private int animChargeAttack = Animator.StringToHash("ChargeAttack");

	private int animDelayAttack = Animator.StringToHash("DelayAttack");

	private int animAttack = Animator.StringToHash("Attack");

	public Enemy enemy;

	public EnemyFloater controller;

	public FloaterAttackLogic attackLogic;

	public float springSpeedMultiplier = 1f;

	public float springDampingMultiplier = 1f;

	public SpringQuaternion springHead;

	private float springHeadSpeed;

	private float springHeadDamping;

	public Transform springHeadTarget;

	public Transform springHeadSource;

	public SpringQuaternion springLegL;

	private float springLegLSpeed;

	private float springLegLDamping;

	public Transform springLegLTarget;

	public Transform springLegLSource;

	public SpringQuaternion springLegR;

	private float springLegRSpeed;

	private float springLegRDamping;

	public Transform springLegRTarget;

	public Transform springLegRSource;

	public SpringQuaternion springArmL;

	private float springArmLSpeed;

	private float springArmLDamping;

	public Transform springArmLTarget;

	public Transform springArmLSource;

	public SpringQuaternion springArmR;

	private float springArmRSpeed;

	private float springArmRDamping;

	public Transform springArmRTarget;

	public Transform springArmRSource;

	[Header("One Shots")]
	public Sound sfxChargeAttackStart;

	public Sound sfxDelayAttackLocal;

	public Sound sfxDelayAttackGlobal;

	public Sound sfxAttackUpLocal;

	public Sound sfxAttackUpGlobal;

	public Sound sfxAttackDownLocal;

	public Sound sfxAttackDownGlobal;

	public Sound sfxMoveShort;

	public Sound sfxMoveLong;

	public Sound sfxHurt;

	public Sound sfxDeath;

	[Header("Loops")]
	public Sound sfxChargeAttackLoop;

	public Sound sfxDelayAttackLoop;

	public Sound sfxStunnedLoop;

	[Header("Animation Booleans")]
	public bool sfxChargeAttackLoopPlaying;

	public bool sfxDelayAttackLoopPlaying;

	internal Animator animator;

	private bool idling;

	private bool stunned;

	private bool stunImpulse;

	private bool noticeImpulse;

	private bool delayAttackImpulse;

	private bool attackImpulse;

	private bool chargeAttackImpulse;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
		springHeadSpeed = springHead.speed;
		springHeadDamping = springHead.damping;
		springLegLSpeed = springLegL.speed;
		springLegLDamping = springLegL.damping;
		springLegRSpeed = springLegR.speed;
		springLegRDamping = springLegR.damping;
		springArmLSpeed = springArmL.speed;
		springArmLDamping = springArmL.damping;
		springArmRSpeed = springArmR.speed;
		springArmRDamping = springArmR.damping;
	}

	private void Update()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		SpringLogic();
		if (enemy.Rigidbody.frozen)
		{
			animator.speed = 0f;
		}
		else
		{
			animator.speed = 1f;
		}
		if (controller.currentState == EnemyFloater.State.Stun)
		{
			if (stunImpulse)
			{
				animator.SetTrigger(animStun);
				stunImpulse = false;
			}
			animator.SetBool(animStunned, true);
			stunned = true;
		}
		else
		{
			animator.SetBool(animStunned, false);
			stunImpulse = true;
			stunned = false;
		}
		SfxStunnedLoop();
		if (controller.currentState == EnemyFloater.State.Notice)
		{
			if (noticeImpulse)
			{
				animator.SetTrigger(animNotice);
				noticeImpulse = false;
			}
		}
		else
		{
			noticeImpulse = true;
		}
		if (controller.currentState == EnemyFloater.State.ChargeAttack)
		{
			if (chargeAttackImpulse)
			{
				animator.SetTrigger(animChargeAttack);
				chargeAttackImpulse = false;
				sfxChargeAttackStart.Play(((Component)this).transform.position);
			}
		}
		else
		{
			chargeAttackImpulse = true;
		}
		SfxChargeAttackLoop();
		if (controller.currentState == EnemyFloater.State.DelayAttack)
		{
			if (delayAttackImpulse)
			{
				animator.SetTrigger(animDelayAttack);
				delayAttackImpulse = false;
			}
		}
		else
		{
			delayAttackImpulse = true;
		}
		SfxDelayAttackLoop();
		if (controller.currentState == EnemyFloater.State.Attack)
		{
			if (attackImpulse)
			{
				animator.SetTrigger(animAttack);
				attackImpulse = false;
			}
		}
		else
		{
			attackImpulse = true;
		}
		if (controller.currentState == EnemyFloater.State.Despawn)
		{
			animator.SetBool(animDespawning, true);
		}
		else
		{
			animator.SetBool(animDespawning, false);
		}
	}

	public void OnSpawn()
	{
		animator.Play("Spawn", 0, 0f);
	}

	public void NoticeSet(int _playerID)
	{
		animator.SetTrigger(animNotice);
	}

	private void SpringLogic()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		springHead.speed = springHeadSpeed * springSpeedMultiplier;
		springHead.damping = springHeadDamping * springDampingMultiplier;
		springHeadSource.rotation = SemiFunc.SpringQuaternionGet(springHead, ((Component)springHeadTarget).transform.rotation);
		springLegL.speed = springLegLSpeed * springSpeedMultiplier;
		springLegL.damping = springLegLDamping * springDampingMultiplier;
		springLegLSource.rotation = SemiFunc.SpringQuaternionGet(springLegL, ((Component)springLegLTarget).transform.rotation);
		springLegR.speed = springLegRSpeed * springSpeedMultiplier;
		springLegR.damping = springLegRDamping * springDampingMultiplier;
		springLegRSource.rotation = SemiFunc.SpringQuaternionGet(springLegR, ((Component)springLegRTarget).transform.rotation);
		springArmL.speed = springArmLSpeed * springSpeedMultiplier;
		springArmL.damping = springArmLDamping * springDampingMultiplier;
		springArmLSource.rotation = SemiFunc.SpringQuaternionGet(springArmL, ((Component)springArmLTarget).transform.rotation);
		springArmR.speed = springArmRSpeed * springSpeedMultiplier;
		springArmR.damping = springArmRDamping * springDampingMultiplier;
		springArmRSource.rotation = SemiFunc.SpringQuaternionGet(springArmR, ((Component)springArmRTarget).transform.rotation);
	}

	public void Despawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void DelayAttack()
	{
		attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.stop);
		SfxDelayAttack();
	}

	public void Attack()
	{
		attackLogic.StateSet(FloaterAttackLogic.FloaterAttackState.smash);
	}

	public void SfxDelayAttack()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		sfxDelayAttackLocal.Play(((Component)this).transform.position);
		sfxDelayAttackGlobal.Play(((Component)this).transform.position);
	}

	public void SfxAttackUp()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		sfxAttackUpLocal.Play(((Component)this).transform.position);
		sfxAttackUpGlobal.Play(((Component)this).transform.position);
	}

	public void SfxAttackDown()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		sfxAttackDownLocal.Play(((Component)this).transform.position);
		sfxAttackDownGlobal.Play(((Component)this).transform.position);
	}

	public void SfxMoveShort()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxMoveShort.Play(((Component)this).transform.position);
	}

	public void SfxMoveLong()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxMoveLong.Play(((Component)this).transform.position);
	}

	public void SfxHurt()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxHurt.Play(((Component)this).transform.position);
	}

	public void SfxDeath()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxDeath.Play(((Component)this).transform.position);
	}

	public void SfxChargeAttackLoop()
	{
		sfxChargeAttackLoop.PlayLoop(sfxChargeAttackLoopPlaying, 5f, 5f);
	}

	public void SfxDelayAttackLoop()
	{
		sfxDelayAttackLoop.PlayLoop(sfxDelayAttackLoopPlaying, 5f, 5f);
	}

	public void SfxStunnedLoop()
	{
		sfxStunnedLoop.PlayLoop(stunned, 5f, 5f);
	}
}
