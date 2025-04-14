using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class EnemyHeadController : MonoBehaviourPunCallbacks, IPunObservable
{
	private Quaternion RotationTarget;

	private float RotationDistance;

	private Enemy Enemy;

	public EnemyHeadVisual Visual;

	public EnemyHeadAnimationSystem AnimationSystem;

	[Space]
	public Transform LookAtTransform;

	public Transform AnimationTransform;

	public Transform HairParent;

	private List<EnemyHeadHair> Hairs;

	[Space]
	public List<GameObject> DeathParticles;

	public Sound DeathSound;

	private void Awake()
	{
		Enemy = ((Component)this).GetComponentInParent<Enemy>();
		Hairs = ((Component)HairParent).GetComponentsInChildren<EnemyHeadHair>().ToList();
	}

	private void Update()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy.CurrentState == EnemyState.Chase || Enemy.CurrentState == EnemyState.ChaseSlow || Enemy.CurrentState == EnemyState.LookUnder)
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (Vector3.Distance(((Component)this).transform.position, ((Component)player).transform.position) < 8f)
				{
					SemiFunc.PlayerEyesOverride(player, Enemy.Vision.VisionTransform.position, 0.1f, ((Component)this).gameObject);
				}
			}
		}
		if (Enemy.TeleportedTimer > 0f)
		{
			foreach (EnemyHeadHair hair in Hairs)
			{
				((Component)hair).transform.position = hair.Target.position;
			}
		}
		if (!Enemy.MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = RotationDistance / num;
			LookAtTransform.rotation = Quaternion.RotateTowards(LookAtTransform.rotation, RotationTarget, num2 * Time.deltaTime);
			return;
		}
		Vector3 val;
		if (Object.op_Implicit((Object)(object)Enemy.AttackStuckPhysObject.TargetObject))
		{
			if ((Object)(object)Enemy.AttackStuckPhysObject != (Object)null)
			{
				Vector3 position = ((Component)Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck).transform.position;
				position += ((Component)Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck).transform.TransformDirection(Enemy.AttackStuckPhysObject.TargetObject.roomVolumeCheck.CheckPosition);
				LookAtTransform.LookAt(position);
			}
		}
		else if (Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			LookAtTransform.LookAt(Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
		}
		else if (Enemy.CurrentState == EnemyState.Chase && Enemy.StateChase.VisionTimer > 0f)
		{
			val = Enemy.NavMeshAgent.Agent.velocity;
			val = ((Vector3)(ref val)).normalized;
			if (((Vector3)(ref val)).magnitude > 0.1f)
			{
				val = Enemy.NavMeshAgent.Agent.velocity;
				Quaternion val2 = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
				LookAtTransform.LookAt(Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
				LookAtTransform.rotation = Quaternion.Lerp(LookAtTransform.rotation, val2, 0.25f);
			}
			else
			{
				LookAtTransform.LookAt(Enemy.TargetPlayerAvatar.PlayerVisionTarget.VisionTransform.position);
			}
		}
		else if (Enemy.CurrentState == EnemyState.LookUnder)
		{
			LookAtTransform.LookAt(Enemy.StateChase.SawPlayerHidePosition);
			LookAtTransform.localEulerAngles = new Vector3(0f, LookAtTransform.localEulerAngles.y, 0f);
		}
		else
		{
			val = Enemy.NavMeshAgent.Agent.velocity;
			if (((Vector3)(ref val)).magnitude > 0.1f)
			{
				Transform lookAtTransform = LookAtTransform;
				val = Enemy.NavMeshAgent.Agent.velocity;
				lookAtTransform.rotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
				LookAtTransform.localEulerAngles = new Vector3(0f, LookAtTransform.localEulerAngles.y, 0f);
			}
		}
		if (Enemy.CurrentState == EnemyState.Despawn)
		{
			Enemy.Rigidbody.DisableFollowPosition(0.1f, 1f);
			Enemy.Rigidbody.DisableFollowRotation(0.1f, 1f);
		}
	}

	public void VisionTriggered()
	{
		if (Enemy.DisableChaseTimer > 0f || Enemy.CurrentState == EnemyState.Chase || Enemy.CurrentState == EnemyState.LookUnder)
		{
			return;
		}
		if (Enemy.CurrentState == EnemyState.ChaseSlow)
		{
			Enemy.CurrentState = EnemyState.Chase;
		}
		else if (Enemy.Vision.onVisionTriggeredCulled && !Enemy.Vision.onVisionTriggeredNear)
		{
			if (Enemy.CurrentState != EnemyState.Sneak)
			{
				if (Random.Range(0f, 100f) <= 30f)
				{
					Enemy.CurrentState = EnemyState.ChaseBegin;
				}
				else
				{
					Enemy.CurrentState = EnemyState.Sneak;
				}
			}
		}
		else if (Enemy.Vision.onVisionTriggeredDistance >= 7f)
		{
			Enemy.CurrentState = EnemyState.Chase;
			Enemy.StateChase.ChaseCanReach = true;
		}
		else
		{
			Enemy.CurrentState = EnemyState.ChaseBegin;
		}
		Enemy.TargetPlayerViewID = Enemy.Vision.onVisionTriggeredPlayer.photonView.ViewID;
		Enemy.TargetPlayerAvatar = Enemy.Vision.onVisionTriggeredPlayer;
	}

	public void OnSpawn()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			SemiFunc.EnemySpawn(Enemy);
		}
		Visual.Spawn();
		if (((Behaviour)AnimationSystem).isActiveAndEnabled)
		{
			AnimationSystem.OnSpawn();
		}
	}

	public void OnHurt()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		AnimationSystem.Hurt.Play(((Component)Enemy.Rigidbody).transform.position);
	}

	public void OnDeath()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Enemy.CurrentState = EnemyState.Despawn;
		}
		foreach (GameObject deathParticle in DeathParticles)
		{
			deathParticle.SetActive(true);
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		DeathSound.Play(((Component)this).transform.position);
		Enemy.EnemyParent.Despawn();
	}

	public void OnStunnedEnd()
	{
		Enemy.CurrentState = EnemyState.Roaming;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)LookAtTransform.rotation);
			return;
		}
		RotationTarget = (Quaternion)stream.ReceiveNext();
		RotationDistance = Quaternion.Angle(((Component)this).transform.rotation, RotationTarget);
	}
}
