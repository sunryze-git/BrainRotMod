using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
	internal PhotonView PhotonView;

	internal EnemyParent EnemyParent;

	internal bool MasterClient;

	public EnemyType Type = EnemyType.Medium;

	[Space]
	public EnemyState CurrentState;

	private EnemyState PreviousState;

	private int CurrentStateIndex;

	[Space]
	public Transform CenterTransform;

	public Transform KillLookAtTransform;

	public Transform CustomValuableSpawnTransform;

	internal LayerMask VisionMask;

	private Vector3 PositionTarget;

	private float PositionDistance;

	internal int StuckCount;

	internal EnemyVision Vision;

	internal bool HasVision;

	internal EnemyPlayerDistance PlayerDistance;

	internal bool HasPlayerDistance;

	internal EnemyOnScreen OnScreen;

	internal bool HasOnScreen;

	internal EnemyPlayerRoom PlayerRoom;

	internal bool HasPlayerRoom;

	internal EnemyRigidbody Rigidbody;

	internal bool HasRigidbody;

	internal EnemyNavMeshAgent NavMeshAgent;

	internal bool HasNavMeshAgent;

	internal EnemyAttackStuckPhysObject AttackStuckPhysObject;

	internal bool HasAttackPhysObject;

	internal EnemyStateInvestigate StateInvestigate;

	internal bool HasStateInvestigate;

	internal EnemyStateChaseBegin StateChaseBegin;

	internal bool HasStateChaseBegin;

	internal EnemyStateChase StateChase;

	internal bool HasStateChase;

	internal EnemyStateLookUnder StateLookUnder;

	internal bool HasStateLookUnder;

	internal EnemyStateDespawn StateDespawn;

	internal bool HasStateDespawn;

	internal EnemyStateSpawn StateSpawn;

	internal bool HasStateSpawn;

	private bool Stunned;

	internal EnemyStateStunned StateStunned;

	internal bool HasStateStunned;

	internal EnemyGrounded Grounded;

	internal bool HasGrounded;

	internal EnemyJump Jump;

	internal bool HasJump;

	internal EnemyHealth Health;

	internal bool HasHealth;

	internal PlayerAvatar TargetPlayerAvatar;

	internal int TargetPlayerViewID;

	protected internal float TeleportedTimer;

	protected internal Vector3 TeleportPosition;

	[HideInInspector]
	public float FreezeTimer;

	private float ChaseTimer;

	internal float DisableChaseTimer;

	private PhotonTransformView photonTransformView;

	[Space]
	public bool SightingStinger;

	public bool EnemyNearMusic;

	internal Vector3 moveDirection;

	private void Awake()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		photonTransformView = ((Component)((Component)this).transform.parent).GetComponentInChildren<PhotonTransformView>();
		EnemyParent = ((Component)this).GetComponentInParent<EnemyParent>();
		PhotonView = ((Component)this).GetComponent<PhotonView>();
		Vision = ((Component)this).GetComponent<EnemyVision>();
		if (Object.op_Implicit((Object)(object)Vision))
		{
			HasVision = true;
		}
		VisionMask = LayerMask.op_Implicit(LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) + LayerMask.GetMask(new string[1] { "HideTriggers" }));
		PlayerDistance = ((Component)this).GetComponent<EnemyPlayerDistance>();
		if (Object.op_Implicit((Object)(object)PlayerDistance))
		{
			HasPlayerDistance = true;
		}
		OnScreen = ((Component)this).GetComponent<EnemyOnScreen>();
		if (Object.op_Implicit((Object)(object)OnScreen))
		{
			HasOnScreen = true;
		}
		PlayerRoom = ((Component)this).GetComponent<EnemyPlayerRoom>();
		if (Object.op_Implicit((Object)(object)PlayerRoom))
		{
			HasPlayerRoom = true;
		}
		NavMeshAgent = ((Component)this).GetComponent<EnemyNavMeshAgent>();
		if (Object.op_Implicit((Object)(object)NavMeshAgent))
		{
			HasNavMeshAgent = true;
		}
		AttackStuckPhysObject = ((Component)this).GetComponent<EnemyAttackStuckPhysObject>();
		if (Object.op_Implicit((Object)(object)AttackStuckPhysObject))
		{
			HasAttackPhysObject = true;
		}
		StateInvestigate = ((Component)this).GetComponent<EnemyStateInvestigate>();
		if (Object.op_Implicit((Object)(object)StateInvestigate))
		{
			HasStateInvestigate = true;
		}
		StateChaseBegin = ((Component)this).GetComponent<EnemyStateChaseBegin>();
		if (Object.op_Implicit((Object)(object)StateChaseBegin))
		{
			HasStateChaseBegin = true;
		}
		StateChase = ((Component)this).GetComponent<EnemyStateChase>();
		if (Object.op_Implicit((Object)(object)StateChase))
		{
			HasStateChase = true;
		}
		StateLookUnder = ((Component)this).GetComponent<EnemyStateLookUnder>();
		if (Object.op_Implicit((Object)(object)StateLookUnder))
		{
			HasStateLookUnder = true;
		}
		StateDespawn = ((Component)this).GetComponent<EnemyStateDespawn>();
		if (Object.op_Implicit((Object)(object)StateDespawn))
		{
			HasStateDespawn = true;
		}
		StateSpawn = ((Component)this).GetComponent<EnemyStateSpawn>();
		if (Object.op_Implicit((Object)(object)StateSpawn))
		{
			HasStateSpawn = true;
		}
		StateStunned = ((Component)this).GetComponent<EnemyStateStunned>();
		if (Object.op_Implicit((Object)(object)StateStunned))
		{
			HasStateStunned = true;
		}
		Health = ((Component)this).GetComponent<EnemyHealth>();
		if (Object.op_Implicit((Object)(object)Health))
		{
			HasHealth = true;
		}
		if (!Object.op_Implicit((Object)(object)CenterTransform))
		{
			Debug.LogError((object)("Center Transform not set in " + ((Object)((Component)this).gameObject).name), (Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			MasterClient = true;
		}
		else
		{
			MasterClient = false;
		}
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMultiplayer() && !MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = PositionDistance / num;
			Vector3 val = PositionTarget - ((Component)this).transform.position;
			moveDirection = ((Vector3)(ref val)).normalized;
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, PositionTarget, num2 * Time.deltaTime);
		}
		if (MasterClient)
		{
			Stunned = false;
			if (HasStateStunned && StateStunned.stunTimer > 0f)
			{
				Stunned = true;
			}
		}
		if (FreezeTimer > 0f)
		{
			FreezeTimer -= Time.deltaTime;
		}
		if (TeleportedTimer > 0f)
		{
			StuckCount = 0;
			TeleportedTimer -= Time.deltaTime;
		}
		if (ChaseTimer > 0f)
		{
			ChaseTimer -= Time.deltaTime;
		}
		if (DisableChaseTimer > 0f)
		{
			DisableChaseTimer -= Time.deltaTime;
		}
	}

	public void Spawn()
	{
		Stunned = false;
		FreezeTimer = 0f;
	}

	public bool IsStunned()
	{
		return Stunned;
	}

	public void DisableChase(float time)
	{
		DisableChaseTimer = time;
	}

	public void SetChaseTimer()
	{
		ChaseTimer = 0.1f;
	}

	public bool CheckChase()
	{
		return ChaseTimer > 0f;
	}

	public void SetChaseTarget(PlayerAvatar playerAvatar)
	{
		if (!EnemyDirector.instance.debugNoVision && !(DisableChaseTimer > 0f) && HasVision && !playerAvatar.isDisabled)
		{
			Vision.VisionTrigger(playerAvatar.photonView.ViewID, playerAvatar, culled: false, playerNear: false);
			if (HasStateChase && (!CheckChase() || CurrentState == EnemyState.ChaseSlow))
			{
				CurrentState = EnemyState.ChaseBegin;
				TargetPlayerViewID = playerAvatar.photonView.ViewID;
				TargetPlayerAvatar = playerAvatar;
			}
		}
	}

	public LevelPoint TeleportToPoint(float minDistance, float maxDistance)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		LevelPoint levelPoint = null;
		if (SemiFunc.EnemySpawnIdlePause())
		{
			levelPoint = EnemyParent.firstSpawnPoint;
		}
		else
		{
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, minDistance, maxDistance, _startRoomOnly: true);
			}
			if (!Object.op_Implicit((Object)(object)levelPoint))
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(((Component)this).transform.position, minDistance, maxDistance);
			}
		}
		if (Object.op_Implicit((Object)(object)levelPoint))
		{
			TeleportPosition = new Vector3(((Component)levelPoint).transform.position.x, ((Component)levelPoint).transform.position.y, ((Component)levelPoint).transform.position.z);
			EnemyTeleported(TeleportPosition);
		}
		return levelPoint;
	}

	public LevelPoint GetLevelPointAhead(Vector3 currentTargetPosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		LevelPoint result = null;
		Vector3 val = currentTargetPosition - ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		LevelPoint levelPoint = null;
		float num = 1000f;
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (Object.op_Implicit((Object)(object)levelPathPoint))
			{
				float num2 = Vector3.Distance(((Component)levelPathPoint).transform.position, currentTargetPosition);
				if (num2 < num)
				{
					num = num2;
					levelPoint = levelPathPoint;
				}
			}
		}
		if (!Object.op_Implicit((Object)(object)levelPoint))
		{
			return null;
		}
		float num3 = -1f;
		foreach (LevelPoint connectedPoint in levelPoint.ConnectedPoints)
		{
			if (Object.op_Implicit((Object)(object)connectedPoint))
			{
				val = ((Component)connectedPoint).transform.position - ((Component)levelPoint).transform.position;
				Vector3 normalized2 = ((Vector3)(ref val)).normalized;
				float num4 = Vector3.Dot(normalized, normalized2);
				if (num4 > num3)
				{
					num3 = num4;
					result = connectedPoint;
				}
			}
		}
		return result;
	}

	public void Freeze(float time)
	{
		if (GameManager.instance.gameMode == 0)
		{
			FreezeRPC(time);
			return;
		}
		((MonoBehaviourPun)this).photonView.RPC("FreezeRPC", (RpcTarget)0, new object[1] { time });
	}

	[PunRPC]
	public void FreezeRPC(float time)
	{
		FreezeTimer = time;
	}

	public void PlayerAdded(int photonID)
	{
		if (HasVision)
		{
			Vision.PlayerAdded(photonID);
		}
		if (HasOnScreen)
		{
			OnScreen.PlayerAdded(photonID);
		}
	}

	public void PlayerRemoved(int photonID)
	{
		if ((Object)(object)StateChaseBegin != (Object)null && (Object)(object)StateChaseBegin.TargetPlayer != (Object)null && StateChaseBegin.TargetPlayer.photonView.ViewID == photonID)
		{
			StateChaseBegin.TargetPlayer = null;
			CurrentState = EnemyState.Roaming;
		}
		if ((Object)(object)TargetPlayerAvatar != (Object)null && TargetPlayerAvatar.photonView.ViewID == photonID)
		{
			TargetPlayerAvatar = PlayerController.instance.playerAvatarScript;
			TargetPlayerViewID = TargetPlayerAvatar.photonView.ViewID;
		}
		if (HasVision)
		{
			Vision.PlayerRemoved(photonID);
		}
		if (HasOnScreen)
		{
			OnScreen.PlayerRemoved(photonID);
		}
	}

	public void EnemyTeleported(Vector3 teleportPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = teleportPosition;
		if (HasNavMeshAgent)
		{
			NavMeshAgent.Warp(teleportPosition);
		}
		if (HasRigidbody)
		{
			Rigidbody.Teleport();
		}
		if (GameManager.instance.gameMode == 0)
		{
			EnemyTeleportedRPC(teleportPosition);
			return;
		}
		((MonoBehaviourPun)this).photonView.RPC("EnemyTeleportedRPC", (RpcTarget)0, new object[1] { teleportPosition });
	}

	[PunRPC]
	private void EnemyTeleportedRPC(Vector3 teleportPosition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		PositionDistance = 0f;
		PositionTarget = teleportPosition;
		TeleportPosition = teleportPosition;
		((Component)this).transform.position = teleportPosition;
		TeleportedTimer = 1f;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)((Component)this).transform.position);
			stream.SendNext((object)CurrentState);
			stream.SendNext((object)TargetPlayerViewID);
			stream.SendNext((object)Stunned);
			return;
		}
		PositionTarget = (Vector3)stream.ReceiveNext();
		PositionDistance = Vector3.Distance(((Component)this).transform.position, PositionTarget);
		CurrentState = (EnemyState)stream.ReceiveNext();
		TargetPlayerViewID = (int)stream.ReceiveNext();
		Stunned = (bool)stream.ReceiveNext();
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (!player.isDisabled && player.photonView.ViewID == TargetPlayerViewID)
			{
				TargetPlayerAvatar = player;
				break;
			}
		}
	}
}
