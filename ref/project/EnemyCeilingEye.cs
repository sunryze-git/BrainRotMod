using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyCeilingEye : MonoBehaviour
{
	public enum State
	{
		Idle,
		Move,
		TargetLost,
		HasTarget,
		Spawn,
		Despawn
	}

	public CeilingEyeLine eyeBeamLeft;

	public CeilingEyeLine eyeBeamRight;

	public ParticleSystem eyeBeamParticles;

	[Header("References")]
	public Transform eyeTransform;

	public EnemyCeilingEyeAnim eyeAnim;

	internal Enemy enemy;

	private bool otherEnemyFetch = true;

	public List<EnemyCeilingEye> otherEnemies;

	public State currentState;

	private bool stateImpulse;

	private float stateTimer;

	internal PlayerAvatar targetPlayer;

	private PhotonView photonView;

	internal bool deathImpulse;

	private float eyeDamageTimer;

	private float eyeDamageWaitTimer;

	private void Awake()
	{
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Update()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		RotationAnimation();
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			TargetFailSafe();
			switch (currentState)
			{
			case State.Idle:
				StateIdle();
				break;
			case State.Move:
				StateMove();
				break;
			case State.HasTarget:
				StateHasTarget();
				break;
			case State.TargetLost:
				StateTargetLost();
				break;
			case State.Spawn:
				StateSpawn();
				break;
			case State.Despawn:
				StateDespawn();
				break;
			}
		}
		if (currentState == State.HasTarget && Object.op_Implicit((Object)(object)targetPlayer))
		{
			SemiFunc.PlayerEyesOverride(targetPlayer, enemy.CenterTransform.position, 0.1f, ((Component)this).gameObject);
			PlayerAvatar playerAvatar = targetPlayer;
			if (Object.op_Implicit((Object)(object)playerAvatar.voiceChat))
			{
				playerAvatar.voiceChat.OverridePitch(1.25f, 1f, 2f);
			}
			playerAvatar.OverridePupilSize(2f, 5, 5f, 0.5f, 5f, 0.5f);
			playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.CeilingEye, 0.25f, 0);
			if (targetPlayer.isLocal)
			{
				Vector3 val = ((Component)targetPlayer).transform.position - enemy.CenterTransform.position;
				float num = Vector3.Dot(Vector3.down, ((Vector3)(ref val)).normalized);
				float strengthNoAim = 10f;
				if (num > 0.9f)
				{
					strengthNoAim = 5f;
				}
				CameraAim.Instance.AimTargetSoftSet(enemy.CenterTransform.position, 0.1f, 2f, strengthNoAim, ((Component)this).gameObject, 100);
				PostProcessing.Instance.VignetteOverride(Color.black, 0.5f, 1f, 1f, 0.5f, 0.1f, ((Component)this).gameObject);
				CameraZoom.Instance.OverrideZoomSet(40f, 0.1f, 1f, 1f, ((Component)this).gameObject, 50);
			}
			else
			{
				eyeBeamLeft.outro = false;
				eyeBeamRight.outro = false;
				eyeBeamLeft.lineTarget = targetPlayer.playerAvatarVisuals.playerEyes.pupilLeft;
				eyeBeamRight.lineTarget = targetPlayer.playerAvatarVisuals.playerEyes.pupilRight;
			}
		}
		else
		{
			eyeBeamLeft.outro = true;
			eyeBeamRight.outro = true;
		}
	}

	private void StateIdle()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = Random.Range(20f, 60f);
		}
		if (!SemiFunc.EnemySpawnIdlePause())
		{
			if (!enemy.EnemyParent.playerClose)
			{
				stateTimer -= Time.deltaTime;
			}
			if (stateTimer <= 0f)
			{
				UpdateState(State.Move);
			}
		}
	}

	private void StateMove()
	{
	}

	private void StateHasTarget()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			eyeDamageWaitTimer = 3f;
			stateImpulse = false;
		}
		if (eyeDamageWaitTimer <= 0f)
		{
			eyeDamageTimer -= Time.deltaTime;
			if (eyeDamageTimer <= 0f)
			{
				targetPlayer.playerHealth.HurtOther(2, ((Component)targetPlayer).transform.position, savingGrace: false, SemiFunc.EnemyGetIndex(enemy));
				eyeDamageTimer = 1f;
			}
		}
		else
		{
			if (targetPlayer.isDisabled)
			{
				UpdateState(State.TargetLost);
				return;
			}
			eyeDamageWaitTimer -= Time.deltaTime;
		}
		Vector3 position = targetPlayer.PlayerVisionTarget.VisionTransform.position;
		stateTimer -= Time.deltaTime;
		if (SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position, position, 20f, targetPlayer, _previouslySeen: true) || SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position + ((Component)this).transform.right * 0.25f, position + Vector3.down * 0.1f, 20f, targetPlayer, _previouslySeen: true) || SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position - ((Component)this).transform.right * 0.25f, position - Vector3.down * 0.1f, 20f, targetPlayer, _previouslySeen: true) || SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position + ((Component)this).transform.up * 0.25f, position + Vector3.down * 0.1f, 20f, targetPlayer, _previouslySeen: true) || SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position - ((Component)this).transform.up * 0.25f, position - Vector3.down * 0.1f, 20f, targetPlayer, _previouslySeen: true))
		{
			stateTimer = 0.5f;
		}
		if (stateTimer <= 0f)
		{
			UpdateState(State.TargetLost);
		}
	}

	private void StateTargetLost()
	{
		if (stateImpulse)
		{
			stateImpulse = false;
			stateTimer = 3f;
		}
		stateTimer -= Time.deltaTime;
		if (stateTimer <= 0f)
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
			UpdateState(State.Despawn);
		}
	}

	private void StateSpawn()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			if (stateImpulse)
			{
				stateTimer = 5f;
				stateImpulse = false;
			}
			stateTimer -= Time.deltaTime;
			if (stateTimer <= 0f)
			{
				UpdateState(State.Idle);
			}
		}
	}

	private void StateDespawn()
	{
	}

	public void OnSpawn()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (otherEnemyFetch)
		{
			otherEnemyFetch = false;
			foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
			{
				EnemyCeilingEye componentInChildren = ((Component)item).GetComponentInChildren<EnemyCeilingEye>(true);
				if (Object.op_Implicit((Object)(object)componentInChildren) && (Object)(object)componentInChildren != (Object)(object)this)
				{
					otherEnemies.Add(componentInChildren);
				}
			}
		}
		if (!SemiFunc.EnemySpawn(enemy))
		{
			return;
		}
		RaycastHit val = default(RaycastHit);
		if (Physics.SphereCast(((Component)this).transform.position + Vector3.up * 0.1f + new Vector3(0f, 0.5f, 0f), 0.1f, Vector3.up, ref val, 30f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			foreach (EnemyCeilingEye otherEnemy in otherEnemies)
			{
				if (((Behaviour)otherEnemy).isActiveAndEnabled && Vector3.Distance(((Component)otherEnemy).transform.position, ((RaycastHit)(ref val)).point) <= 2f)
				{
					enemy.StateDespawn.Despawn();
					enemy.EnemyParent.DespawnedTimerSet(Random.Range(5f, 10f), _min: true);
					return;
				}
			}
			((Component)this).transform.position = ((RaycastHit)(ref val)).point;
			((Component)this).transform.rotation = Quaternion.LookRotation(((RaycastHit)(ref val)).normal);
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdatePositionRPC", (RpcTarget)0, new object[2]
				{
					((Component)this).transform.position,
					((Component)this).transform.rotation
				});
			}
		}
		UpdateState(State.Spawn);
	}

	public void OnDeath()
	{
		deathImpulse = true;
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemy.EnemyParent.SpawnedTimerSet(0f);
		}
	}

	public void OnVisionTrigger()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.CurrentState == EnemyState.Despawn || (currentState != 0 && currentState != State.TargetLost))
		{
			return;
		}
		PlayerAvatar onVisionTriggeredPlayer = enemy.Vision.onVisionTriggeredPlayer;
		if (!SemiFunc.PlayerVisionCheckPosition(enemy.Vision.VisionTransform.position, onVisionTriggeredPlayer.PlayerVisionTarget.VisionTransform.position, enemy.Vision.VisionDistance, onVisionTriggeredPlayer, _previouslySeen: true))
		{
			return;
		}
		if ((Object)(object)targetPlayer != (Object)(object)onVisionTriggeredPlayer)
		{
			foreach (EnemyCeilingEye otherEnemy in otherEnemies)
			{
				if ((Object)(object)otherEnemy.targetPlayer == (Object)(object)onVisionTriggeredPlayer)
				{
					return;
				}
			}
			targetPlayer = onVisionTriggeredPlayer;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("TargetPlayerRPC", (RpcTarget)0, new object[1] { targetPlayer.photonView.ViewID });
			}
		}
		UpdateState(State.HasTarget);
	}

	public void TargetFailSafe()
	{
		if (currentState != State.HasTarget)
		{
			targetPlayer = null;
		}
		else if (currentState == State.HasTarget && (!Object.op_Implicit((Object)(object)targetPlayer) || targetPlayer.isDisabled))
		{
			UpdateState(State.TargetLost);
		}
	}

	private void UpdateState(State _state)
	{
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && currentState != _state)
		{
			currentState = _state;
			stateImpulse = true;
			stateTimer = 0f;
			if (GameManager.Multiplayer())
			{
				photonView.RPC("UpdateStateRPC", (RpcTarget)0, new object[1] { currentState });
			}
			else
			{
				UpdateStateRPC(currentState);
			}
		}
	}

	public void RotationAnimation()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (currentState == State.Idle)
		{
			eyeTransform.localRotation = Quaternion.Slerp(eyeTransform.localRotation, Quaternion.identity, 5f * Time.deltaTime);
		}
		else if (currentState == State.HasTarget)
		{
			Vector3 val = SemiFunc.ClampDirection(((Component)targetPlayer).transform.position - enemy.CenterTransform.position, ((Component)this).transform.forward, 35f);
			eyeTransform.rotation = Quaternion.RotateTowards(eyeTransform.rotation, Quaternion.LookRotation(val), 360f * Time.deltaTime);
		}
	}

	[PunRPC]
	private void UpdateStateRPC(State _state)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		currentState = _state;
		stateImpulse = true;
		if (currentState == State.HasTarget)
		{
			if (targetPlayer.isLocal)
			{
				targetPlayer.physGrabber.ReleaseObject();
				CameraAim.Instance.AimTargetSet(enemy.CenterTransform.position, 0.5f, 2f, ((Component)this).gameObject, 100);
				CameraGlitch.Instance.PlayLong();
			}
		}
		else if (currentState == State.Spawn && ((Behaviour)eyeAnim).isActiveAndEnabled)
		{
			eyeAnim.SetSpawn();
		}
	}

	[PunRPC]
	private void TargetPlayerRPC(int _playerID)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.photonView.ViewID == _playerID)
			{
				if (player.isLocal)
				{
					player.physGrabber.ReleaseObject(1f);
				}
				targetPlayer = player;
			}
		}
	}

	[PunRPC]
	private void UpdatePositionRPC(Vector3 _position, Quaternion _rotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = _position;
		((Component)this).transform.rotation = _rotation;
	}
}
