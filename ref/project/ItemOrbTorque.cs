using UnityEngine;

public class ItemOrbTorque : MonoBehaviour
{
	private ItemOrb itemOrb;

	private PhysGrabObject physGrabObject;

	private void Start()
	{
		itemOrb = ((Component)this).GetComponent<ItemOrb>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	private void FixedUpdate()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (!itemOrb.itemActive || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject item in itemOrb.objectAffected)
		{
			if (Object.op_Implicit((Object)(object)item) && (Object)(object)physGrabObject != (Object)(object)item)
			{
				float num = Vector3.Distance(new Vector3(item.rb.position.x, 0f, item.rb.position.z), new Vector3(((Component)this).transform.position.x, 0f, ((Component)this).transform.position.z));
				Rigidbody rb = item.rb;
				Vector3 val = ((Component)this).transform.position - rb.position;
				_ = ((Vector3)(ref val)).normalized;
				float num2 = 0.5f;
				if (num < num2)
				{
					_ = Mathf.Clamp(num - 0.2f, 0f, num2) / num2;
				}
				float num3 = 0.5f;
				float num4 = 1f;
				num2 = 1f;
				if (num < num2)
				{
					num4 = Mathf.Clamp(num - 0.5f, 0f, num2) / num2;
					num3 *= num4;
				}
				if (item.isEnemy)
				{
					num3 *= 5f;
				}
				item.OverrideFragility(0.1f);
				item.OverrideMaterial(SemiFunc.PhysicMaterialSticky());
			}
		}
	}
}
