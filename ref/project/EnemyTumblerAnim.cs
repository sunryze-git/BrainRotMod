using UnityEngine;

public class EnemyTumblerAnim : MonoBehaviour
{
	public Enemy enemy;

	public EnemyTumbler enemyTumbler;

	internal Animator animator;

	public Materials.MaterialTrigger material;

	private bool tumble;

	private bool stunned;

	private bool stunImpulse;

	internal bool spawnImpulse;

	private bool jumpImpulse;

	private float jumpedTimer;

	[Header("One Shots")]
	public Sound sfxJump;

	public Sound sfxLand;

	public Sound sfxNotice;

	public Sound sfxCleaverSwing;

	public Sound sfxCharge;

	public Sound sfxHurt;

	public Sound sfxMoveShort;

	public Sound sfxMoveLong;

	public Sound sfxHurtColliderImpactAny;

	[Header("Loops")]
	public Sound sfxStunnedLoop;

	public Sound sfxTumbleLoopLocal;

	public Sound sfxTumbleLoopGlobal;

	public bool showGizmos = true;

	public float gizmoMinDistance = 3f;

	public float gizmoMaxDistance = 8f;

	private void Awake()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
	}

	private void Update()
	{
		if (enemy.Jump.jumping)
		{
			animator.SetBool("jumping", true);
			if (jumpImpulse)
			{
				jumpedTimer = 0f;
				animator.SetTrigger("Jump");
				animator.SetBool("falling", false);
				jumpImpulse = false;
			}
		}
		else
		{
			animator.SetBool("jumping", false);
			jumpImpulse = true;
		}
		jumpedTimer += Time.deltaTime;
		if (jumpedTimer > 0.5f)
		{
			if (enemy.Rigidbody.physGrabObject.rbVelocity.y < -0.1f)
			{
				animator.SetBool("falling", true);
			}
			else
			{
				animator.SetBool("falling", false);
			}
		}
		if (enemyTumbler.currentState == EnemyTumbler.State.Tell)
		{
			animator.SetBool("tell", true);
		}
		else
		{
			animator.SetBool("tell", false);
		}
		if (enemyTumbler.currentState == EnemyTumbler.State.Tumble)
		{
			animator.SetBool("tumble", true);
			tumble = true;
		}
		else
		{
			animator.SetBool("tumble", false);
			tumble = false;
		}
		sfxTumbleLoopLocal.PlayLoop(tumble, 5f, 5f);
		sfxTumbleLoopGlobal.PlayLoop(tumble, 5f, 5f);
		if (enemyTumbler.currentState == EnemyTumbler.State.Stunned)
		{
			if (stunImpulse)
			{
				animator.SetTrigger("Stun");
				stunImpulse = false;
			}
			animator.SetBool("stunned", true);
			stunned = true;
		}
		else
		{
			animator.SetBool("stunned", false);
			stunImpulse = true;
			stunned = false;
		}
		sfxStunnedLoop.PlayLoop(stunned, 5f, 5f);
		if (enemyTumbler.currentState == EnemyTumbler.State.Despawn)
		{
			animator.SetBool("despawning", true);
		}
		else
		{
			animator.SetBool("despawning", false);
		}
	}

	public void OnSpawn()
	{
		animator.SetBool("stunned", false);
		animator.Play("Spawn", 0, 0f);
	}

	private void OnDrawGizmos()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (showGizmos)
		{
			Gizmos.matrix = Matrix4x4.TRS(((Component)this).transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, gizmoMinDistance);
			Gizmos.color = new Color(0.9f, 0f, 0.1f, 0.5f);
			Gizmos.DrawWireSphere(Vector3.zero, gizmoMaxDistance);
		}
	}

	public void SfxOnHurtColliderImpactAny()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxHurtColliderImpactAny.Play(((Component)this).transform.position);
	}

	public void OnTumble()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
	}

	public void Despawn()
	{
		enemy.EnemyParent.Despawn();
	}

	public void SfxJump()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		sfxJump.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.ShakeDistance(1f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(1f, 3f, 8f, ((Component)this).transform.position, 0.5f);
	}

	public void SfxLand()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		sfxLand.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.5f);
	}

	public void SfxNotice()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxNotice.Play(((Component)this).transform.position);
	}

	public void SfxCleaverSwing()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxCleaverSwing.Play(((Component)this).transform.position);
	}

	public void SfxCharge()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxCharge.Play(((Component)this).transform.position);
	}

	public void SfxHurt()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sfxHurt.Play(((Component)this).transform.position);
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
}
