using System.Collections;
using UnityEngine;

public class DirtFinderMapDoor : MonoBehaviour
{
	public Transform Target;

	public GameObject DoorPrefab;

	public PhysGrabHinge Hinge;

	private GameObject MapObject;

	public void Start()
	{
		Hinge = ((Component)this).GetComponent<PhysGrabHinge>();
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		MapObject = Map.Instance.AddDoor(this, DoorPrefab);
		while (!Hinge.broken)
		{
			yield return (object)new WaitForSeconds(1f);
		}
		Object.Destroy((Object)(object)MapObject);
	}
}
