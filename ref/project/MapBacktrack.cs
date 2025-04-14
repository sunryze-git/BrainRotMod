using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapBacktrack : MonoBehaviour
{
	public GameObject pointPrefab;

	private List<MapBacktrackPoint> points = new List<MapBacktrackPoint>();

	[Space]
	public int amount;

	public float spacing;

	public float pointWait;

	public float resetWait;

	private int currentPoint;

	private Vector3 currentPointPosition;

	private int currentPointCorner;

	private Vector3 truckDestination;

	private NavMeshPath path;

	private void Start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		path = new NavMeshPath();
		for (int i = 0; i < amount; i++)
		{
			GameObject val = Object.Instantiate<GameObject>(pointPrefab, ((Component)this).transform);
			points.Add(val.GetComponent<MapBacktrackPoint>());
			((Object)val.transform).name = $"Point {i}";
		}
		((MonoBehaviour)this).StartCoroutine(Backtrack());
	}

	private IEnumerator Backtrack()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(0.5f);
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPathPoint.Room.Truck)
			{
				truckDestination = ((Component)levelPathPoint).transform.position;
				break;
			}
		}
		Vector3 val2 = default(Vector3);
		while (true)
		{
			Vector3 lastNavmeshPosition = PlayerController.instance.playerAvatarScript.LastNavmeshPosition;
			Vector3 val = lastNavmeshPosition;
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				val = truckDestination;
			}
			else if (Object.op_Implicit((Object)(object)RoundDirector.instance.extractionPointCurrent))
			{
				val = ((Component)RoundDirector.instance.extractionPointCurrent).transform.position;
			}
			bool flag = false;
			if (Map.Instance.Active)
			{
				MapLayer layerParent = Map.Instance.GetLayerParent(lastNavmeshPosition.y + 1f);
				MapLayer layerParent2 = Map.Instance.GetLayerParent(val.y + 1f);
				if (layerParent.layer == layerParent2.layer)
				{
					flag = true;
				}
			}
			if (!Map.Instance.Active || (flag && Vector3.Distance(lastNavmeshPosition, val) < 10f))
			{
				yield return (object)new WaitForSeconds(0.25f);
				continue;
			}
			NavMesh.CalculatePath(lastNavmeshPosition, val, -1, path);
			currentPoint = 0;
			currentPointPosition = lastNavmeshPosition;
			currentPointCorner = 0;
			while (currentPoint < points.Count)
			{
				bool flag2 = false;
				float num = spacing;
				while (!flag2 && currentPointCorner < path.corners.Length)
				{
					float num2 = Vector3.Distance(currentPointPosition, path.corners[currentPointCorner]);
					if (num2 < num)
					{
						currentPointPosition = path.corners[currentPointCorner];
						num -= num2;
						currentPointCorner++;
						continue;
					}
					currentPointPosition = Vector3.Lerp(currentPointPosition, path.corners[currentPointCorner], num / num2);
					if (Map.Instance.GetLayerParent(currentPointPosition.y + 1f).layer == Map.Instance.PlayerLayer)
					{
						points[currentPoint].Show(_sameLayer: true);
					}
					else
					{
						points[currentPoint].Show(_sameLayer: false);
					}
					((Vector3)(ref val2))._002Ector(currentPointPosition.x, 0f, currentPointPosition.z);
					((Component)points[currentPoint]).transform.position = val2 * Map.Instance.Scale + Map.Instance.OverLayerParent.position;
					currentPoint++;
					flag2 = true;
				}
				if (currentPointCorner >= path.corners.Length)
				{
					currentPoint = points.Count;
				}
				yield return (object)new WaitForSeconds(pointWait);
			}
			foreach (MapBacktrackPoint _point in points)
			{
				while (_point.animating)
				{
					yield return (object)new WaitForSeconds(0.05f);
				}
			}
			yield return (object)new WaitForSeconds(pointWait);
		}
	}
}
