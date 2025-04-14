using System.Collections;
using UnityEngine;

public class ValuableVolume : MonoBehaviour
{
	public enum Type
	{
		Tiny,
		Small,
		Medium,
		Big,
		Wide,
		Tall,
		VeryTall
	}

	public Type VolumeType;

	[HideInInspector]
	public Module Module;

	private Mesh MeshTiny;

	private Mesh MeshSmall;

	private Mesh MeshMedium;

	private Mesh MeshBig;

	private Mesh MeshWide;

	private Mesh MeshTall;

	private Mesh MeshVeryTall;

	private void Start()
	{
		Module = ((Component)this).GetComponentInParent<Module>();
	}

	public void Setup()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		ValuablePropSwitch componentInParent = ((Component)this).GetComponentInParent<ValuablePropSwitch>();
		if (Object.op_Implicit((Object)(object)componentInParent) && (Object)(object)((Component)this).transform.parent != (Object)(object)componentInParent.ValuableParent.transform)
		{
			Debug.LogError((object)"Valuable Volume: Child of ValuablePropSwitch but not valuable parent...", (Object)(object)((Component)this).gameObject);
		}
		if (!((Behaviour)this).isActiveAndEnabled)
		{
			return;
		}
		bool flag = true;
		if (Debug.isDebugBuild)
		{
			((MonoBehaviour)this).StartCoroutine(SafetyCheck());
			flag = false;
		}
		Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, 2f);
		foreach (Collider val in array)
		{
			if (((Component)val).gameObject.CompareTag("Phys Grab Object"))
			{
				ValuableObject componentInParent2 = ((Component)((Component)val).transform).GetComponentInParent<ValuableObject>();
				if (Object.op_Implicit((Object)(object)componentInParent2) && componentInParent2.volumeType == VolumeType && Vector3.Distance(((Component)componentInParent2).transform.position, ((Component)this).transform.position) < 0.1f)
				{
					((Component)componentInParent2).transform.parent = ((Component)this).transform.parent;
					break;
				}
			}
		}
		if (flag)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private IEnumerator SafetyCheck()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		Mesh val = null;
		switch (VolumeType)
		{
		case Type.Tiny:
			val = AssetManager.instance.valuableMeshTiny;
			break;
		case Type.Small:
			val = AssetManager.instance.valuableMeshSmall;
			break;
		case Type.Medium:
			val = AssetManager.instance.valuableMeshMedium;
			break;
		case Type.Big:
			val = AssetManager.instance.valuableMeshBig;
			break;
		case Type.Wide:
			val = AssetManager.instance.valuableMeshWide;
			break;
		case Type.Tall:
			val = AssetManager.instance.valuableMeshTall;
			break;
		case Type.VeryTall:
			val = AssetManager.instance.valuableMeshVeryTall;
			break;
		}
		Bounds bounds = val.bounds;
		Vector3 size = ((Bounds)(ref bounds)).size;
		Collider[] array = Physics.OverlapBox(((Component)this).transform.position + ((Component)this).transform.forward * size.z / 2f + ((Component)this).transform.up * size.y / 2f + Vector3.up * 0.01f, size / 2f, ((Component)this).transform.rotation, LayerMask.GetMask(new string[1] { "Default" }), (QueryTriggerInteraction)1);
		if (array.Length != 0)
		{
			Debug.LogError((object)"Valuable Volume: Overlapping colliders:", (Object)(object)((Component)this).gameObject);
			Collider[] array2 = array;
			foreach (Collider val2 in array2)
			{
				Debug.LogError((object)("     " + ((Object)((Component)val2).gameObject).name), (Object)(object)((Component)val2).gameObject);
			}
		}
	}
}
