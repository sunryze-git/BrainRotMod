using System.Collections;
using UnityEngine;

public class DirtFinderMapScanlines : MonoBehaviour
{
	public float Speed;

	public float MaxZ;

	private void OnEnable()
	{
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private IEnumerator Logic()
	{
		while (true)
		{
			Transform transform = ((Component)this).transform;
			transform.localPosition += new Vector3(0f, 0f, Speed);
			if (((Component)this).transform.localPosition.z < MaxZ)
			{
				((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, ((Component)this).transform.localPosition.y, 0f);
				Transform transform2 = ((Component)this).transform;
				transform2.localPosition += new Vector3(0f, 0f, Speed);
			}
			yield return (object)new WaitForSeconds(0.1f);
		}
	}
}
