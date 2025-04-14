using UnityEngine;

public class ItemOrbMagnet : MonoBehaviour
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
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (!itemOrb.itemActive || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject item in itemOrb.objectAffected)
		{
			if (Object.op_Implicit((Object)(object)item) && (Object)(object)physGrabObject != (Object)(object)item)
			{
				Vector3 val = ((Component)physGrabObject).transform.position - ((Component)item).transform.position;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				val = ((Component)physGrabObject).transform.position - ((Component)item).transform.position;
				if (((Vector3)(ref val)).magnitude > 0.45f)
				{
					item.rb.AddForce(normalized * Mathf.Clamp(item.rb.mass * 10f, 0.2f, 5f));
				}
				item.rb.velocity = physGrabObject.rb.velocity;
				item.OverrideZeroGravity();
			}
		}
	}
}
