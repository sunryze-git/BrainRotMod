using UnityEngine;

public class EnemyHiddenAnim : MonoBehaviour
{
	public enum BreathingState
	{
		None,
		Slow,
		Medium,
		Fast,
		FastNoSound
	}

	public enum FootstepState
	{
		None,
		Standing,
		TwoStep,
		Moving,
		Sprinting,
		TimedSteps
	}

	private BreathingState breathingState;

	private FootstepState footstepState;

	[Space]
	public Enemy enemy;

	public EnemyHidden enemyHidden;

	internal Materials.MaterialTrigger material = new Materials.MaterialTrigger();

	[Space]
	public ParticleSystem particleBreath;

	public ParticleSystem particleBreathFast;

	public ParticleSystem particleBreathConstant;

	private bool breathingCurrent;

	private float breathingTimer;

	[Space]
	public Transform transformFoot;

	public ParticleSystem particleFootstepShapeRight;

	public ParticleSystem particleFootstepShapeLeft;

	public ParticleSystem particleFootstepSmoke;

	private Vector3 footstepPositionPrevious;

	private Vector3 footstepPositionPreviousRight;

	private Vector3 footstepPositionPreviousLeft;

	private int footstepCurrent = 1;

	private float movingTimer;

	private float stopStepTimer;

	private float timedStepsTimer;

	private bool jumpStartImpulse = true;

	private bool jumpStopImpulse;

	[Space]
	public Sound soundBreatheIn;

	public Sound soundBreatheOut;

	[Space]
	public Sound soundBreatheInFast;

	public Sound soundBreatheOutFast;

	[Space]
	public Sound soundFootstep;

	public Sound soundFootstepSprint;

	[Space]
	public Sound soundStunStart;

	private bool soundStunStartImpulse;

	public Sound soundStunLoop;

	public Sound soundStunStop;

	private bool soundStunStopImpulse;

	private float soundStunPauseTimer;

	[Space]
	public Sound soundJump;

	private bool soundJumpImpulse;

	public Sound soundLand;

	private bool soundLandImpulse;

	[Space]
	public Sound soundPlayerPickup;

	private bool soundPlayerPickupImpulse;

	public Sound soundPlayerRelease;

	private bool soundPlayerReleaseImpulse;

	public Sound soundPlayerMove;

	public Sound soundPlayerMoveStop;

	private bool soundPlayerMoveImpulse;

	[Space]
	public Sound soundHurt;

	public Sound soundDeath;

	private void Update()
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		BreathingLogic();
		FootstepLogic();
		if (enemyHidden.currentState == EnemyHidden.State.Stun)
		{
			if (soundStunStartImpulse)
			{
				StopBreathing();
				soundStunStart.Play(((Component)particleBreath).transform.position);
				soundStunStartImpulse = false;
			}
			if (soundStunPauseTimer > 0f)
			{
				if (soundStunStopImpulse)
				{
					soundStunStop.Play(((Component)particleBreath).transform.position);
					soundStunStopImpulse = false;
				}
				soundStunLoop.PlayLoop(playing: false, 2f, 5f);
				particleBreathConstant.Stop();
			}
			else
			{
				soundStunLoop.PlayLoop(playing: true, 2f, 10f);
				particleBreathConstant.Play();
				soundStunStopImpulse = true;
			}
		}
		else
		{
			if (soundStunStopImpulse)
			{
				soundStunStop.Play(((Component)particleBreath).transform.position);
				soundStunStopImpulse = false;
			}
			soundStunLoop.PlayLoop(playing: false, 2f, 5f);
			particleBreathConstant.Stop();
			soundStunStartImpulse = true;
		}
		if (soundStunPauseTimer > 0f)
		{
			soundStunPauseTimer -= Time.deltaTime;
		}
		if (enemy.Jump.jumping)
		{
			if (soundJumpImpulse)
			{
				particleBreath.Play();
				soundJump.Play(((Component)particleBreath).transform.position);
				StopBreathing();
				soundJumpImpulse = false;
			}
			soundLandImpulse = true;
		}
		else
		{
			if (soundLandImpulse)
			{
				particleBreathFast.Play();
				soundLand.Play(((Component)particleBreath).transform.position);
				StopBreathing();
				soundLandImpulse = false;
			}
			soundJumpImpulse = true;
		}
		if (enemyHidden.currentState == EnemyHidden.State.PlayerPickup)
		{
			if (soundPlayerPickupImpulse)
			{
				StopBreathing();
				particleBreath.Play();
				soundPlayerPickup.Play(((Component)particleBreath).transform.position);
				soundPlayerPickupImpulse = false;
			}
		}
		else
		{
			soundPlayerPickupImpulse = true;
		}
		if (enemyHidden.currentState == EnemyHidden.State.PlayerReleaseWait)
		{
			if (soundPlayerReleaseImpulse)
			{
				StopBreathing();
				particleBreath.Play();
				soundPlayerRelease.Play(((Component)particleBreath).transform.position);
				soundPlayerReleaseImpulse = false;
			}
		}
		else
		{
			soundPlayerReleaseImpulse = true;
		}
		if (enemyHidden.currentState == EnemyHidden.State.PlayerMove && !enemy.Jump.jumping)
		{
			soundPlayerMove.PlayLoop(playing: true, 2f, 10f);
			soundPlayerMoveImpulse = true;
			return;
		}
		if (soundPlayerMoveImpulse)
		{
			soundPlayerMoveStop.Play(((Component)particleBreath).transform.position);
			soundPlayerMoveImpulse = false;
		}
		soundPlayerMove.PlayLoop(playing: false, 2f, 10f);
	}

	private void BreathingLogic()
	{
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.Jump.jumping || enemyHidden.currentState == EnemyHidden.State.Stun || enemyHidden.currentState == EnemyHidden.State.PlayerRelease || enemyHidden.currentState == EnemyHidden.State.PlayerReleaseWait || enemyHidden.currentState == EnemyHidden.State.PlayerPickup)
		{
			breathingState = BreathingState.None;
		}
		else if (enemyHidden.currentState == EnemyHidden.State.PlayerMove)
		{
			breathingState = BreathingState.FastNoSound;
		}
		else if (enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || enemyHidden.currentState == EnemyHidden.State.Leave)
		{
			breathingState = BreathingState.Fast;
		}
		else if (enemyHidden.currentState == EnemyHidden.State.Roam || enemyHidden.currentState == EnemyHidden.State.Investigate)
		{
			breathingState = BreathingState.Medium;
		}
		else
		{
			breathingState = BreathingState.Slow;
		}
		if (breathingState == BreathingState.None)
		{
			soundBreatheIn.Stop();
			soundBreatheOut.Stop();
		}
		if (breathingTimer <= 0f)
		{
			if (breathingCurrent)
			{
				breathingCurrent = false;
				if (breathingState != BreathingState.FastNoSound)
				{
					if (breathingState == BreathingState.Fast)
					{
						soundBreatheInFast.Play(((Component)particleBreath).transform.position);
					}
					else
					{
						soundBreatheIn.Play(((Component)particleBreath).transform.position);
					}
				}
				else
				{
					particleBreathFast.Play();
				}
				breathingTimer = 3f;
			}
			else
			{
				breathingCurrent = true;
				if (breathingState != BreathingState.FastNoSound)
				{
					if (breathingState == BreathingState.Fast)
					{
						soundBreatheOutFast.Play(((Component)particleBreath).transform.position);
					}
					else
					{
						soundBreatheOut.Play(((Component)particleBreath).transform.position);
					}
					particleBreath.Play();
				}
				else
				{
					particleBreathFast.Play();
				}
				breathingTimer = 4.5f;
			}
		}
		if (breathingState == BreathingState.Slow)
		{
			breathingTimer -= 1f * Time.deltaTime;
		}
		else if (breathingState == BreathingState.Medium)
		{
			breathingTimer -= 2f * Time.deltaTime;
		}
		else
		{
			breathingTimer -= 5f * Time.deltaTime;
		}
	}

	private void FootstepLogic()
	{
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		if (movingTimer > 0f)
		{
			movingTimer -= Time.deltaTime;
		}
		if ((enemyHidden.currentState == EnemyHidden.State.Roam || enemyHidden.currentState == EnemyHidden.State.Investigate || enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || enemyHidden.currentState == EnemyHidden.State.PlayerMove || enemyHidden.currentState == EnemyHidden.State.Leave) && ((Vector3)(ref enemy.Rigidbody.velocity)).magnitude > 0.5f)
		{
			movingTimer = 0.25f;
		}
		if (enemyHidden.currentState == EnemyHidden.State.Stun || enemy.Jump.jumping)
		{
			footstepState = FootstepState.None;
		}
		else if (enemyHidden.currentState == EnemyHidden.State.StunEnd || enemyHidden.currentState == EnemyHidden.State.PlayerNotice)
		{
			footstepState = FootstepState.TimedSteps;
		}
		else if (movingTimer > 0f)
		{
			if (enemyHidden.currentState == EnemyHidden.State.PlayerGoTo || enemyHidden.currentState == EnemyHidden.State.PlayerMove || enemyHidden.currentState == EnemyHidden.State.Leave)
			{
				footstepState = FootstepState.Sprinting;
			}
			else
			{
				footstepState = FootstepState.Moving;
			}
		}
		else if (footstepState == FootstepState.Moving)
		{
			footstepState = FootstepState.TwoStep;
		}
		else if (footstepState != FootstepState.TwoStep)
		{
			footstepState = FootstepState.Standing;
		}
		if (enemy.Jump.jumping)
		{
			if (jumpStartImpulse)
			{
				jumpStopImpulse = true;
				jumpStartImpulse = false;
				FootstepSet();
				FootstepSet();
			}
		}
		else if (jumpStopImpulse)
		{
			jumpStopImpulse = false;
			jumpStartImpulse = true;
			FootstepSet();
			FootstepSet();
		}
		if ((footstepState == FootstepState.Moving || footstepState == FootstepState.Sprinting) && Vector3.Distance(transformFoot.position, footstepPositionPrevious) > 1f)
		{
			FootstepSet();
		}
		if (footstepState == FootstepState.TimedSteps)
		{
			if (timedStepsTimer <= 0f)
			{
				timedStepsTimer = 0.25f;
				FootstepSet();
			}
			else
			{
				timedStepsTimer -= Time.deltaTime;
			}
		}
		else
		{
			timedStepsTimer = 0f;
		}
		if (footstepState == FootstepState.TwoStep)
		{
			if (stopStepTimer == -1f)
			{
				FootstepSet();
				stopStepTimer = 0.25f;
				return;
			}
			stopStepTimer -= Time.deltaTime;
			if (stopStepTimer <= 0f)
			{
				footstepState = FootstepState.Standing;
				FootstepSet();
				stopStepTimer = -1f;
			}
		}
		else
		{
			stopStepTimer = -1f;
		}
	}

	private void FootstepSet()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = transformFoot.right * (-0.3f * (float)footstepCurrent);
		Vector3 val2 = Random.insideUnitSphere * 0.15f;
		val2.y = 0f;
		RaycastHit val3 = default(RaycastHit);
		if (Physics.Raycast(transformFoot.position + val + val2, Vector3.down * 2f, ref val3, 3f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			ParticleSystem val4 = particleFootstepShapeRight;
			Vector3 val5 = footstepPositionPreviousRight;
			if (footstepCurrent == 1)
			{
				val4 = particleFootstepShapeLeft;
				val5 = footstepPositionPreviousLeft;
			}
			if (Vector3.Distance(val5, ((RaycastHit)(ref val3)).point) > 0.2f)
			{
				((Component)val4).transform.position = ((RaycastHit)(ref val3)).point + Vector3.up * 0.02f;
				((Component)val4).transform.eulerAngles = new Vector3(0f, transformFoot.eulerAngles.y, 0f);
				val4.Play();
				((Component)particleFootstepSmoke).transform.position = ((Component)val4).transform.position;
				((Component)particleFootstepSmoke).transform.rotation = ((Component)val4).transform.rotation;
				particleFootstepSmoke.Play();
				Materials.Instance.Impulse(((Component)val4).transform.position + Vector3.up * 0.5f, Vector3.down, Materials.SoundType.Medium, footstep: true, material, Materials.HostType.Enemy);
				if (footstepState == FootstepState.Sprinting)
				{
					soundFootstepSprint.Play(((Component)val4).transform.position);
				}
				else
				{
					soundFootstep.Play(((Component)val4).transform.position);
				}
				if (footstepCurrent == 1)
				{
					footstepPositionPreviousLeft = ((RaycastHit)(ref val3)).point;
				}
				else
				{
					footstepPositionPreviousRight = ((RaycastHit)(ref val3)).point;
				}
				footstepCurrent *= -1;
			}
		}
		footstepPositionPrevious = transformFoot.position;
	}

	public void StopBreathing()
	{
		soundBreatheIn.Stop();
		soundBreatheInFast.Stop();
		soundBreatheOut.Stop();
		soundBreatheOutFast.Stop();
	}

	public void StunPause()
	{
		soundStunPauseTimer = 1f;
	}

	public void Hurt()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		StopBreathing();
		StunPause();
		soundHurt.Play(((Component)particleBreath).transform.position);
	}

	public void Death()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		particleBreathConstant.Stop();
		StopBreathing();
		StunPause();
		soundDeath.Play(((Component)particleBreath).transform.position);
	}
}
