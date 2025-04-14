using Photon.Pun;
using UnityEngine;

public class TrapTypeIdentifier : MonoBehaviour
{
	public string trapType;

	[Header("Must add the trigger!")]
	public GameObject Trigger;

	public bool OnlyRemoveTrigger;

	[HideInInspector]
	public bool TriggerRemoved;

	private void Start()
	{
		Module componentInParent = ((Component)this).GetComponentInParent<Module>();
		Debug.LogError((object)("Remove + '" + trapType + "' in '" + ((Object)((Component)componentInParent).gameObject).name + "'"));
		TrapDirector.instance.TrapList.Add(((Component)this).gameObject);
	}

	[PunRPC]
	private void DestroyTrigger()
	{
		Object.Destroy((Object)(object)Trigger);
	}

	[PunRPC]
	private void DestroyTrap()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
