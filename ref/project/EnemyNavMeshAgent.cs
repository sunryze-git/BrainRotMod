using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshAgent : MonoBehaviour
{
	internal NavMeshAgent Agent;

	internal Vector3 AgentVelocity;

	public bool updateRotation;

	private float StopTimer;

	private float DisableTimer;

	internal float DefaultSpeed;

	internal float DefaultAcceleration;

	private float OverrideTimer;

	private float SetPathTimer;

	private void Awake()
	{
		Agent = ((Component)this).GetComponent<NavMeshAgent>();
		if (!updateRotation)
		{
			Agent.updateRotation = false;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			((Behaviour)Agent).enabled = true;
		}
		else
		{
			((Behaviour)Agent).enabled = false;
		}
		DefaultSpeed = Agent.speed;
		DefaultAcceleration = Agent.acceleration;
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		AgentVelocity = Agent.velocity;
		if (SetPathTimer > 0f)
		{
			SetPathTimer -= Time.deltaTime;
		}
		if (DisableTimer > 0f)
		{
			((Behaviour)Agent).enabled = false;
			DisableTimer -= Time.deltaTime;
			return;
		}
		if (!((Behaviour)Agent).enabled)
		{
			((Behaviour)Agent).enabled = true;
		}
		if (StopTimer > 0f)
		{
			Agent.isStopped = true;
			StopTimer -= Time.deltaTime;
		}
		else if (((Behaviour)Agent).enabled && Agent.isStopped)
		{
			Agent.isStopped = false;
		}
		if (OverrideTimer > 0f)
		{
			OverrideTimer -= Time.deltaTime;
			if (OverrideTimer <= 0f)
			{
				Agent.speed = DefaultSpeed;
				Agent.acceleration = DefaultAcceleration;
			}
		}
	}

	public void OverrideAgent(float speed, float acceleration, float time)
	{
		Agent.speed = speed;
		Agent.acceleration = acceleration;
		OverrideTimer = time;
	}

	public void UpdateAgent(float speed, float acceleration)
	{
		Agent.speed = speed;
		Agent.acceleration = acceleration;
	}

	public void AgentMove(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = Agent.velocity;
		Vector3 destination = Agent.destination;
		if (OnNavmesh(position))
		{
			Warp(position);
			SetDestination(destination);
			Agent.velocity = velocity;
		}
	}

	private bool OnNavmesh(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		return NavMesh.SamplePosition(position, ref val, 5f, -1);
	}

	public void Warp(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)this).transform.position, position) < 1f)
		{
			return;
		}
		if (DisableTimer > 0f)
		{
			((Behaviour)Agent).enabled = true;
		}
		if (OnNavmesh(position))
		{
			Agent.Warp(position);
			if (DisableTimer > 0f)
			{
				((Behaviour)Agent).enabled = false;
			}
		}
	}

	public void ResetPath()
	{
		if (((Behaviour)Agent).enabled && HasPath())
		{
			Agent.ResetPath();
		}
	}

	public bool CanReach(Vector3 _target, float _range)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)Agent).enabled)
		{
			return true;
		}
		if (!Agent.hasPath)
		{
			return true;
		}
		if (Vector3.Distance(GetPoint(), _target) > _range)
		{
			return false;
		}
		return true;
	}

	public void SetDestination(Vector3 position)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)Agent).enabled)
		{
			if (!Agent.hasPath)
			{
				SetPathTimer = 0.1f;
			}
			Agent.SetDestination(position);
		}
	}

	public void Stop(float time)
	{
		if (((Behaviour)Agent).enabled)
		{
			StopTimer = time;
			if (StopTimer == 0f)
			{
				Agent.isStopped = false;
			}
			else
			{
				Agent.isStopped = true;
			}
		}
	}

	public bool IsStopped()
	{
		if (StopTimer > 0f)
		{
			return true;
		}
		return false;
	}

	public void Disable(float time)
	{
		((Behaviour)Agent).enabled = false;
		DisableTimer = time;
	}

	public void Enable()
	{
		if (DisableTimer > 0f)
		{
			((Behaviour)Agent).enabled = true;
			DisableTimer = 0f;
		}
	}

	public bool IsDisabled()
	{
		if (DisableTimer > 0f)
		{
			return true;
		}
		return false;
	}

	public Vector3 GetPoint()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Agent.hasPath)
		{
			return Agent.path.corners[Agent.path.corners.Length - 1];
		}
		return new Vector3(-1000f, 1000f, 1000f);
	}

	public Vector3 GetDestination()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (Agent.hasPath)
		{
			return Agent.destination;
		}
		return ((Component)this).transform.position;
	}

	public bool HasPath()
	{
		if (SetPathTimer > 0f || Agent.hasPath)
		{
			return true;
		}
		return false;
	}

	public NavMeshPath CalculatePath(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NavMeshPath val = new NavMeshPath();
		if (!((Behaviour)Agent).enabled)
		{
			return val;
		}
		Agent.CalculatePath(position, val);
		return val;
	}
}
