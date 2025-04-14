using UnityEngine;

public class PhysGrabObjectCollider : MonoBehaviour
{
	[HideInInspector]
	public int colliderID;

	private PhysGrabObject physGrabObject;

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponentInParent<PhysGrabObject>();
	}

	private void OnDestroy()
	{
		if (Object.op_Implicit((Object)(object)physGrabObject))
		{
			physGrabObject.colliders.Remove(((Component)this).transform);
		}
	}
}
