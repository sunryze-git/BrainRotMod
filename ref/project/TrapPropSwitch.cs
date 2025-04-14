using System.Collections;
using UnityEngine;

public class TrapPropSwitch : MonoBehaviour
{
	public GameObject TrapParent;

	public GameObject PropParent;

	[HideInInspector]
	public string DebugState = "...";

	[HideInInspector]
	public bool DebugSwitch;

	private void Start()
	{
		TrapParent.SetActive(true);
		PropParent.SetActive(true);
		((MonoBehaviour)this).StartCoroutine(Setup());
	}

	public IEnumerator Setup()
	{
		while (!TrapDirector.instance.TrapListUpdated)
		{
			yield return (object)new WaitForSeconds(0.5f);
		}
		yield return (object)new WaitForSeconds(0.5f);
		Trap componentInChildren = ((Component)this).GetComponentInChildren<Trap>();
		if ((Object)(object)componentInChildren != (Object)null && ((Component)componentInChildren).gameObject.activeSelf)
		{
			PropParent.gameObject.SetActive(false);
		}
	}

	public void DebugToggle()
	{
		if (DebugSwitch)
		{
			DebugSwitch = false;
			DebugState = "Trap Active";
			if ((Object)(object)TrapParent != (Object)null)
			{
				TrapParent.SetActive(true);
			}
			if ((Object)(object)PropParent != (Object)null)
			{
				PropParent.SetActive(false);
			}
		}
		else
		{
			DebugSwitch = true;
			DebugState = "Prop Active";
			if ((Object)(object)PropParent != (Object)null)
			{
				PropParent.SetActive(true);
			}
			if ((Object)(object)TrapParent != (Object)null)
			{
				TrapParent.SetActive(false);
			}
		}
	}
}
