using UnityEngine;

public class EnemyStateChase : MonoBehaviour
{
	private Enemy Enemy;

	private PlayerController Player;

	private bool Active;

	public float Speed;

	public float Acceleration;

	[Space]
	public float StateTimeMin;

	public float StateTimeMax;

	private float StateTimer;

	[Space]
	public float VisionTime;

	[HideInInspector]
	public float VisionTimer;

	public int VisionsToReset;

	[HideInInspector]
	public Vector3 ChasePosition = Vector3.zero;

	[HideInInspector]
	public bool ChaseCanReach = true;

	private bool ChaseCanReachSet;

	private bool SawPlayerHide;

	internal Vector3 SawPlayerNavmeshPosition;

	internal Vector3 SawPlayerHidePosition;

	private float CantReachTime;

	[Space]
	public bool ChaseOnlyOnNavmesh = true;

	private void Awake()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
		Player = PlayerController.instance;
	}

	private void Update()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		if (!Enemy.MasterClient)
		{
			return;
		}
		if (Enemy.CurrentState != EnemyState.Chase)
		{
			if (Active)
			{
				Active = false;
			}
			return;
		}
		if (!Active)
		{
			Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer = 0f;
			ChasePosition = ((Component)Enemy.TargetPlayerAvatar).transform.position;
			VisionTimer = VisionTime;
			ChaseCanReachSet = false;
			SawPlayerHide = false;
			CantReachTime = 0f;
			StateTimer = Random.Range(StateTimeMin, StateTimeMax);
			Active = true;
		}
		Enemy.SetChaseTimer();
		Enemy.NavMeshAgent.UpdateAgent(Speed, Acceleration);
		if (Enemy.Vision.VisionTriggered[Enemy.TargetPlayerAvatar.photonView.ViewID])
		{
			VisionTimer = VisionTime;
		}
		else if (VisionTimer > 0f)
		{
			VisionTimer -= Time.deltaTime;
		}
		if (VisionTimer > 0f)
		{
			if (ChaseOnlyOnNavmesh || Enemy.TargetPlayerAvatar.LastNavMeshPositionTimer <= 0.25f)
			{
				Enemy.NavMeshAgent.Enable();
				Enemy.NavMeshAgent.SetDestination(Enemy.TargetPlayerAvatar.LastNavmeshPosition);
				if (ChaseCanReachSet)
				{
					Vector3 point = Enemy.NavMeshAgent.GetPoint();
					if (Vector3.Distance(point, ((Component)Enemy.TargetPlayerAvatar).transform.position) > 0.5f)
					{
						ChaseCanReach = false;
					}
					else
					{
						ChaseCanReach = true;
					}
					if (Enemy.TargetPlayerAvatar.isCrawling && !ChaseCanReach)
					{
						SawPlayerHidePosition = ((Component)Enemy.TargetPlayerAvatar).transform.position;
						SawPlayerNavmeshPosition = Enemy.TargetPlayerAvatar.LastNavmeshPosition;
						SawPlayerHide = true;
					}
					ChasePosition = point;
				}
				ChaseCanReachSet = true;
			}
			else
			{
				Enemy.NavMeshAgent.Disable(0.1f);
				((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, ((Component)Enemy.TargetPlayerAvatar).transform.position, Speed * Time.deltaTime);
			}
		}
		else
		{
			if (SawPlayerHide)
			{
				Enemy.CurrentState = EnemyState.LookUnder;
				return;
			}
			Enemy.NavMeshAgent.SetDestination(ChasePosition);
			if (Vector3.Distance(((Component)this).transform.position, ChasePosition) < 1f)
			{
				LevelPoint levelPointAhead = Enemy.GetLevelPointAhead(ChasePosition);
				if (Object.op_Implicit((Object)(object)levelPointAhead))
				{
					Enemy.NavMeshAgent.SetDestination(((Component)levelPointAhead).transform.position);
				}
				ChasePosition = Enemy.NavMeshAgent.GetDestination();
			}
			ChaseCanReach = true;
			ChaseCanReachSet = false;
		}
		if (ChaseCanReach && Enemy.Vision.VisionsTriggered[Enemy.TargetPlayerAvatar.photonView.ViewID] >= VisionsToReset)
		{
			StateTimer = Random.Range(StateTimeMin, StateTimeMax);
		}
		if (!ChaseCanReach)
		{
			CantReachTime += Time.deltaTime;
			if (CantReachTime > 2f)
			{
				Enemy.Vision.VisionsTriggered[Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
				Enemy.CurrentState = EnemyState.ChaseSlow;
				return;
			}
		}
		else
		{
			CantReachTime = 0f;
		}
		StateTimer -= Time.deltaTime;
		if (StateTimer <= 0f)
		{
			Enemy.CurrentState = EnemyState.ChaseSlow;
		}
		if (Enemy.TargetPlayerAvatar.isDisabled)
		{
			Enemy.Vision.VisionsTriggered[Enemy.TargetPlayerAvatar.photonView.ViewID] = 0;
			Enemy.CurrentState = EnemyState.Roaming;
		}
	}
}
