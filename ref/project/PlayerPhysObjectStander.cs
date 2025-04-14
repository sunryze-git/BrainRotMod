using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysObjectStander : MonoBehaviour
{
	public LayerMask layerMask;

	private SphereCollider Collider;

	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	private float checkTimer;

	private void Awake()
	{
		Collider = ((Component)this).GetComponent<SphereCollider>();
	}

	private void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (checkTimer <= 0f)
		{
			physGrabObjects.Clear();
			Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, Collider.radius, LayerMask.op_Implicit(layerMask));
			if (array.Length != 0)
			{
				Collider[] array2 = array;
				foreach (Collider val in array2)
				{
					PhysGrabObject physGrabObject = ((Component)val).gameObject.GetComponent<PhysGrabObject>();
					if (!Object.op_Implicit((Object)(object)physGrabObject))
					{
						physGrabObject = ((Component)val).gameObject.GetComponentInParent<PhysGrabObject>();
					}
					if (Object.op_Implicit((Object)(object)physGrabObject))
					{
						physGrabObjects.Add(physGrabObject);
					}
				}
			}
			checkTimer = 0.1f;
		}
		else
		{
			checkTimer -= 1f * Time.deltaTime;
		}
	}
}
