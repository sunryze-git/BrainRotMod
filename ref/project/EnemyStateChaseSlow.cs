using UnityEngine;

public class EnemyStateChaseSlow : MonoBehaviour
{
	private Enemy Enemy;

	private bool Active;

	public float Speed;

	public float Acceleration;

	[Space]
	public float StateTimeMin;

	public float StateTimeMax;

	private float StateTimer;

	private void Start()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
	}

	private void Update()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!Enemy.MasterClient)
		{
			return;
		}
		if (Enemy.CurrentState != EnemyState.ChaseSlow)
		{
			if (Active)
			{
				Active = false;
			}
			return;
		}
		if (!Active)
		{
			ChaseAhead();
			StateTimer = Random.Range(StateTimeMin, StateTimeMax);
			Active = true;
		}
		Enemy.SetChaseTimer();
		Enemy.NavMeshAgent.UpdateAgent(Speed, Acceleration);
		if (Vector3.Distance(((Component)this).transform.position, Enemy.NavMeshAgent.Agent.destination) < 1f)
		{
			ChaseAhead();
		}
		StateTimer -= Time.deltaTime;
		if (StateTimer <= 0f)
		{
			Enemy.CurrentState = EnemyState.ChaseEnd;
		}
	}

	private void ChaseAhead()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		LevelPoint levelPointAhead = Enemy.GetLevelPointAhead(Enemy.StateChase.ChasePosition);
		if (Object.op_Implicit((Object)(object)levelPointAhead))
		{
			Enemy.StateChase.ChasePosition = ((Component)levelPointAhead).transform.position;
		}
		Enemy.NavMeshAgent.SetDestination(Enemy.StateChase.ChasePosition);
	}
}
