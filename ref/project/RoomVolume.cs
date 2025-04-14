using System.Collections;
using UnityEngine;

public class RoomVolume : MonoBehaviour
{
	public bool Truck;

	public bool Extraction;

	public Color Color = Color.blue;

	[Space]
	public ReverbPreset ReverbPreset;

	public RoomAmbience RoomAmbience;

	public Module Module;

	public MapModule MapModule;

	private bool Explored;

	private void Start()
	{
		Module = ((Component)this).GetComponentInParent<Module>();
		RoomVolume[] componentsInParent = ((Component)this).GetComponentsInParent<RoomVolume>();
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if ((Object)(object)componentsInParent[i] != (Object)(object)this)
			{
				Object.Destroy((Object)(object)this);
				return;
			}
		}
		((MonoBehaviour)this).StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		yield return (object)new WaitForSeconds(0.1f);
		BoxCollider[] componentsInChildren = ((Component)this).GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider val in componentsInChildren)
		{
			Vector3 val2 = val.size * 0.5f;
			val2.x *= Mathf.Abs(((Component)val).transform.lossyScale.x);
			val2.y *= Mathf.Abs(((Component)val).transform.lossyScale.y);
			val2.z *= Mathf.Abs(((Component)val).transform.lossyScale.z);
			Collider[] array = Physics.OverlapBox(((Component)val).transform.TransformPoint(val.center), val2, ((Component)val).transform.rotation, LayerMask.GetMask(new string[1] { "Other" }), (QueryTriggerInteraction)2);
			for (int j = 0; j < array.Length; j++)
			{
				LevelPoint component = ((Component)((Component)array[j]).transform).GetComponent<LevelPoint>();
				if (Object.op_Implicit((Object)(object)component))
				{
					component.Room = this;
				}
			}
		}
		if (!Extraction && !Truck && !Module.StartRoom && !SemiFunc.RunIsShop())
		{
			componentsInChildren = ((Component)this).GetComponentsInChildren<BoxCollider>();
			foreach (BoxCollider val3 in componentsInChildren)
			{
				Vector3 scale = val3.size * 0.5f;
				scale.x *= Mathf.Abs(((Component)val3).transform.lossyScale.x);
				scale.y *= Mathf.Abs(((Component)val3).transform.lossyScale.y);
				scale.z *= Mathf.Abs(((Component)val3).transform.lossyScale.z);
				Vector3 position = ((Component)val3).transform.TransformPoint(val3.center);
				Quaternion rotation = ((Component)val3).transform.rotation;
				MapModule = Map.Instance.AddRoomVolume(((Component)this).gameObject, position, rotation, scale, Module);
			}
		}
	}

	public void SetExplored()
	{
		if (!Explored)
		{
			Explored = true;
			if (Object.op_Implicit((Object)(object)MapModule))
			{
				MapModule.Hide();
			}
		}
	}
}
