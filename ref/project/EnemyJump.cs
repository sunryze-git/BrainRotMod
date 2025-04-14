using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(EnemyGrounded))]
public class EnemyJump : MonoBehaviour
{
	public Enemy enemy;

	internal bool jumping;

	internal bool jumpingDelay;

	internal bool landDelay;

	internal float jumpCooldown;

	internal float timeSinceJumped;

	[Space]
	public bool warpAgentOnLand;

	[Space]
	public bool surfaceJump = true;

	public float surfaceJumpForceUp = 5f;

	public float surfaceJumpForceSide = 2f;

	private bool surfaceJumpImpulse;

	private Vector3 surfaceJumpDirection;

	private float surfaceJumpDisableTimer;

	[Space]
	public bool stuckJump;

	private float stuckJumpDisableTimer;

	private float cartJumpTimer;

	private float cartJumpCooldown;

	public int stuckJumpCount = 5;

	public float stuckJumpForceUp = 5f;

	public float stuckJumpForceSide = 2f;

	private bool stuckJumpImpulse;

	private Vector3 stuckJumpImpulseDirection;

	[Space]
	public bool gapJump;

	public float gapJumpForceUp = 5f;

	public float gapJumpForceForward = 5f;

	internal bool gapJumpImpulse;

	private float gapJumpOverrideTimer;

	private float gapJumpOverrideUp;

	private float gapJumpOverrideForward;

	public float gapJumpDelay;

	private float gapJumpDelayTimer;

	public float gapLandDelay;

	private float gapLandDelayTimer;

	private bool gapCheckerActive;

	private void Awake()
	{
		enemy.Jump = this;
		enemy.HasJump = true;
		if (gapJump && !gapCheckerActive)
		{
			((MonoBehaviour)this).StartCoroutine(GapChecker());
			gapCheckerActive = true;
		}
	}

	private void Start()
	{
		if (!enemy.HasRigidbody)
		{
			Debug.LogError((object)("EnemyJump: No Rigidbody found on " + ((Object)enemy).name));
			stuckJump = false;
		}
	}

	private void OnDisable()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		gapCheckerActive = false;
	}

	private void OnEnable()
	{
		if (gapJump && !gapCheckerActive)
		{
			((MonoBehaviour)this).StartCoroutine(GapChecker());
			gapCheckerActive = true;
		}
	}

	public void StuckReset()
	{
		stuckJumpImpulse = false;
	}

	public void SurfaceJumpTrigger(Vector3 _direction)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!jumping)
		{
			surfaceJumpImpulse = true;
			surfaceJumpDirection = _direction;
		}
	}

	public void SurfaceJumpDisable(float _time)
	{
		surfaceJumpImpulse = false;
		surfaceJumpDisableTimer = _time;
	}

	public void StuckTrigger(Vector3 _direction)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!jumping)
		{
			stuckJumpImpulse = true;
			stuckJumpImpulseDirection = _direction;
		}
	}

	public void StuckDisable(float _time)
	{
		stuckJumpDisableTimer = _time;
	}

	private IEnumerator GapChecker()
	{
		gapCheckerActive = true;
		while (true)
		{
			if (enemy.Grounded.grounded && enemy.NavMeshAgent.HasPath())
			{
				int num = 8;
				float num2 = 0.5f;
				float num3 = 2f;
				Vector3 forward = ((Component)enemy.Rigidbody).transform.forward;
				forward.y = 0f;
				Vector3 val = enemy.Rigidbody.physGrabObject.centerPoint + forward * num2;
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					if (Physics.Raycast(val, Vector3.down * 0.25f, num3, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())))
					{
						if (flag)
						{
							gapJumpImpulse = true;
						}
					}
					else if (i < 2)
					{
						flag = true;
					}
					val += forward * num2;
				}
			}
			yield return (object)new WaitForSeconds(0.2f);
		}
	}

	private void FixedUpdate()
	{
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		if (!jumping)
		{
			timeSinceJumped += Time.fixedDeltaTime;
		}
		else
		{
			timeSinceJumped = 0f;
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		bool flag = false;
		if (enemy.Rigidbody.grabbed || enemy.IsStunned() || enemy.Rigidbody.teleportedTimer > 0f)
		{
			stuckJumpImpulse = false;
			gapJumpImpulse = false;
			return;
		}
		float num = gapJumpForceUp;
		float num2 = gapJumpForceForward;
		if (gapJumpOverrideTimer > 0f)
		{
			num = gapJumpOverrideUp;
			num2 = gapJumpOverrideForward;
			gapJumpOverrideTimer -= Time.fixedDeltaTime;
		}
		if (gapJumpImpulse && !jumping && jumpCooldown <= 0f)
		{
			if (gapJumpDelayTimer > 0f)
			{
				JumpingDelaySet(_jumpingDelay: true);
				enemy.NavMeshAgent.Stop(0.1f);
				enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f);
				enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
				gapJumpDelayTimer -= Time.fixedDeltaTime;
			}
			else
			{
				enemy.Rigidbody.DisableFollowPosition(0.5f, 10f);
				Vector3 val = ((Component)enemy.Rigidbody).transform.forward * num2;
				val.y = 0f;
				val += Vector3.up * num;
				enemy.Rigidbody.JumpImpulse();
				enemy.Rigidbody.rb.AddForce(val, (ForceMode)1);
				enemy.NavMeshAgent.OverrideAgent(10f, 999f, 0.5f);
				gapJumpImpulse = false;
				stuckJumpImpulse = false;
				flag = true;
			}
		}
		else
		{
			gapJumpDelayTimer = gapJumpDelay;
		}
		if (enemy.TeleportedTimer > 0f)
		{
			StuckDisable(0.5f);
		}
		if (stuckJumpDisableTimer > 0f)
		{
			stuckJumpDisableTimer -= Time.fixedDeltaTime;
			stuckJumpImpulse = false;
		}
		else if (stuckJump)
		{
			if (cartJumpTimer > 0f && enemy.Rigidbody.touchingCartTimer > 0f)
			{
				if (cartJumpCooldown > 0f)
				{
					cartJumpCooldown -= Time.fixedDeltaTime;
				}
				else
				{
					stuckJumpImpulse = true;
					cartJumpCooldown = 2f;
				}
			}
			if (enemy.StuckCount >= stuckJumpCount)
			{
				stuckJumpImpulse = true;
				enemy.StuckCount = 0;
			}
			if (!flag && stuckJumpImpulse && enemy.Grounded.grounded && !jumping && jumpCooldown <= 0f)
			{
				if (stuckJumpImpulseDirection == Vector3.zero)
				{
					stuckJumpImpulseDirection = ((Component)enemy).transform.position - ((Component)enemy.Rigidbody).transform.position;
				}
				Vector3 val2 = ((Vector3)(ref stuckJumpImpulseDirection)).normalized * stuckJumpForceSide;
				val2.y = 0f;
				val2 += Vector3.up * stuckJumpForceUp;
				stuckJumpImpulseDirection = Vector3.zero;
				enemy.Rigidbody.JumpImpulse();
				enemy.Rigidbody.rb.AddForce(val2, (ForceMode)1);
				stuckJumpImpulse = false;
				flag = true;
			}
		}
		if (cartJumpTimer > 0f)
		{
			cartJumpTimer -= Time.fixedDeltaTime;
		}
		if (surfaceJump)
		{
			if (surfaceJumpDisableTimer > 0f)
			{
				surfaceJumpDisableTimer -= Time.fixedDeltaTime;
			}
			else if (!flag && surfaceJumpImpulse && enemy.Grounded.grounded && !jumping && jumpCooldown <= 0f)
			{
				enemy.Rigidbody.DisableFollowPosition(0.2f, 20f);
				enemy.NavMeshAgent.Stop(0.3f);
				Vector3 val3 = surfaceJumpDirection * surfaceJumpForceSide;
				val3.y = 0f;
				enemy.Rigidbody.JumpImpulse();
				enemy.Rigidbody.rb.AddForce(val3 + Vector3.up * surfaceJumpForceUp, (ForceMode)1);
				surfaceJumpImpulse = false;
				flag = true;
			}
		}
		if (!jumping)
		{
			if (flag)
			{
				JumpingDelaySet(_jumpingDelay: false);
				JumpingSet(_jumping: true);
				LandDelaySet(_landDelay: false);
				enemy.Grounded.GroundedDisable(0.1f);
			}
		}
		else if (enemy.Grounded.grounded)
		{
			if (warpAgentOnLand && !enemy.NavMeshAgent.IsDisabled())
			{
				enemy.NavMeshAgent.Warp(((Component)enemy.Rigidbody).transform.position);
			}
			JumpingDelaySet(_jumpingDelay: false);
			JumpingSet(_jumping: false);
			if (gapLandDelay > 0f)
			{
				LandDelaySet(_landDelay: true);
				gapLandDelayTimer = gapLandDelay;
			}
			jumpCooldown = 0.25f;
		}
		if (jumpCooldown > 0f)
		{
			jumpCooldown -= Time.fixedDeltaTime;
			jumpCooldown = Mathf.Max(jumpCooldown, 0f);
			enemy.StuckCount = 0;
			surfaceJumpImpulse = false;
			stuckJumpImpulse = false;
			gapJumpImpulse = false;
		}
		if (gapLandDelayTimer > 0f)
		{
			enemy.NavMeshAgent.Stop(0.1f);
			enemy.Rigidbody.OverrideFollowPosition(0.1f, 0f);
			enemy.Rigidbody.OverrideColliderMaterialStunned(0.1f);
			gapLandDelayTimer -= Time.fixedDeltaTime;
		}
	}

	public void JumpingSet(bool _jumping)
	{
		if (_jumping != jumping)
		{
			if (_jumping)
			{
				enemy.Grounded.grounded = false;
			}
			jumping = _jumping;
			if (GameManager.Multiplayer() && PhotonNetwork.IsMasterClient)
			{
				enemy.Rigidbody.photonView.RPC("JumpingSetRPC", (RpcTarget)1, new object[1] { jumping });
			}
		}
	}

	public void JumpingDelaySet(bool _jumpingDelay)
	{
		if (jumpingDelay != _jumpingDelay)
		{
			jumpingDelay = _jumpingDelay;
			if (SemiFunc.IsMasterClient())
			{
				enemy.Rigidbody.photonView.RPC("JumpingDelaySetRPC", (RpcTarget)1, new object[1] { jumpingDelay });
			}
		}
	}

	public void LandDelaySet(bool _landDelay)
	{
		if (landDelay != _landDelay)
		{
			landDelay = _landDelay;
			if (SemiFunc.IsMasterClient())
			{
				enemy.Rigidbody.photonView.RPC("LandDelaySetRPC", (RpcTarget)1, new object[1] { landDelay });
			}
		}
	}

	public void CartJump(float _time)
	{
		cartJumpTimer = _time;
	}

	public void GapJumpOverride(float _time, float _up, float _forward)
	{
		gapJumpOverrideTimer = _time;
		gapJumpOverrideUp = _up;
		gapJumpOverrideForward = _forward;
	}

	[PunRPC]
	private void JumpingSetRPC(bool _jumping)
	{
		jumping = _jumping;
	}

	[PunRPC]
	private void JumpingDelaySetRPC(bool _jumpingDelay)
	{
		jumpingDelay = _jumpingDelay;
	}

	[PunRPC]
	private void LandDelaySetRPC(bool _landDelay)
	{
		landDelay = _landDelay;
	}
}
