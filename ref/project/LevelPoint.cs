using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelPoint : MonoBehaviour
{
	public bool DebugMeshActive = true;

	public Mesh DebugMesh;

	internal bool inStartRoom;

	[Space]
	public bool ModuleConnect;

	public bool Truck;

	private bool ModuleConnected;

	public RoomVolume Room;

	[Space]
	public List<LevelPoint> ConnectedPoints;

	[HideInInspector]
	public List<LevelPoint> AllLevelPoints;

	private void Start()
	{
		LevelGenerator.Instance.LevelPathPoints.Add(this);
		if (Truck)
		{
			LevelGenerator.Instance.LevelPathTruck = this;
		}
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponentInParent<StartRoom>()))
		{
			inStartRoom = true;
		}
		if (ModuleConnect)
		{
			((MonoBehaviour)this).StartCoroutine(ModuleConnectSetup());
		}
		((MonoBehaviour)this).StartCoroutine(NavMeshCheck());
	}

	private IEnumerator NavMeshCheck()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(0.5f);
		bool flag = false;
		NavMeshHit val = default(NavMeshHit);
		if (!NavMesh.SamplePosition(((Component)this).transform.position, ref val, 0.5f, -1))
		{
			flag = true;
			Debug.LogError((object)"Level Point not on Navmesh! Fix!", (Object)(object)((Component)this).gameObject);
		}
		if (!Object.op_Implicit((Object)(object)Room))
		{
			flag = true;
			Debug.LogError((object)"Level Point did not find a room volume!! Fix!!!", (Object)(object)((Component)this).gameObject);
		}
		foreach (LevelPoint connectedPoint in ConnectedPoints)
		{
			if (!Object.op_Implicit((Object)(object)connectedPoint))
			{
				flag = true;
				Debug.LogError((object)"Level Point not fully connected! Fix!!", (Object)(object)((Component)this).gameObject);
				continue;
			}
			bool flag2 = false;
			foreach (LevelPoint connectedPoint2 in connectedPoint.ConnectedPoints)
			{
				if ((Object)(object)connectedPoint2 == (Object)(object)this)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
				Debug.LogError((object)"Level Point not fully connected! Fix!!", (Object)(object)((Component)this).gameObject);
			}
		}
		if (flag && Application.isEditor)
		{
			Object.Instantiate<GameObject>(AssetManager.instance.debugLevelPointError, ((Component)this).transform.position, Quaternion.identity);
		}
	}

	private IEnumerator ModuleConnectSetup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		float num = 999f;
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (!levelPathPoint.ModuleConnect)
			{
				continue;
			}
			float num2 = Vector3.Distance(((Component)this).transform.position, ((Component)levelPathPoint).transform.position);
			if (num2 < 15f && num2 < num && Vector3.Dot(((Component)levelPathPoint).transform.forward, ((Component)this).transform.forward) <= -0.8f)
			{
				Vector3 forward = ((Component)levelPathPoint).transform.forward;
				Vector3 val = ((Component)this).transform.position - ((Component)levelPathPoint).transform.position;
				if (Vector3.Dot(forward, ((Vector3)(ref val)).normalized) > 0.8f)
				{
					num = num2;
					ConnectedPoints.Add(levelPathPoint);
				}
			}
		}
		ModuleConnected = true;
	}
}
