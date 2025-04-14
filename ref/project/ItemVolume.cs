using System.Collections.Generic;
using UnityEngine;

public class ItemVolume : MonoBehaviour
{
	public SemiFunc.itemVolume itemVolume;

	public SemiFunc.itemSecretShopType itemSecretShopType;

	public List<GameObject> volumes = new List<GameObject>();

	private ItemAttributes itemAttributes;

	private void Start()
	{
		itemAttributes = ((Component)this).GetComponentInParent<ItemAttributes>();
		if (Object.op_Implicit((Object)(object)itemAttributes))
		{
			((Component)this).gameObject.tag = "Untagged";
		}
		if (SemiFunc.IsNotMasterClient())
		{
			Object.Destroy((Object)(object)this);
		}
	}

	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		ItemAttributes componentInParent = ((Component)this).GetComponentInParent<ItemAttributes>();
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			if (itemVolume != componentInParent.item.itemVolume)
			{
				itemVolume = componentInParent.item.itemVolume;
			}
			string text = "Item Volume " + itemVolume;
			if (((Object)((Component)this).gameObject).name != text)
			{
				((Object)((Component)this).gameObject).name = text;
			}
		}
	}

	private void OnDrawGizmos()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		ItemAttributes componentInParent = ((Component)this).GetComponentInParent<ItemAttributes>();
		int num = 0;
		foreach (GameObject volume in volumes)
		{
			if (itemVolume == (SemiFunc.itemVolume)num)
			{
				Color color = (Gizmos.color = Color.yellow);
				Gizmos.matrix = Matrix4x4.TRS(volume.transform.position, volume.transform.rotation, volume.transform.localScale);
				Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), Vector3.one);
				color.a = 0.5f;
				Gizmos.color = color;
				if (!Object.op_Implicit((Object)(object)componentInParent))
				{
					Gizmos.DrawCube(Vector3.zero, Vector3.one);
				}
				Gizmos.matrix = Matrix4x4.identity;
			}
			num++;
		}
	}
}
