using System.Collections;
using UnityEngine;

public class DirtFinderMapDoorTarget : MonoBehaviour
{
	public Transform Target;

	public Transform HingeTransform;

	public MapLayer Layer;

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	public IEnumerator Logic()
	{
		while (Object.op_Implicit((Object)(object)Target) && ((Component)Target).gameObject.activeSelf)
		{
			if (Map.Instance.Active)
			{
				Map.Instance.DoorUpdate(HingeTransform, ((Component)Target).transform, Layer);
			}
			yield return (object)new WaitForSeconds(0.1f);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
