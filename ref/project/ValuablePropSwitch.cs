using UnityEngine;

public class ValuablePropSwitch : MonoBehaviour
{
	public GameObject ValuableParent;

	public GameObject PropParent;

	internal bool SetupComplete;

	[HideInInspector]
	public string DebugState = "...";

	[HideInInspector]
	public bool DebugSwitch;

	[HideInInspector]
	public string ChildValuableString = "...";

	private void Start()
	{
		ValuableParent.SetActive(true);
		PropParent.SetActive(false);
	}

	public void Setup()
	{
		ValuablePropSwitch[] componentsInParent = ((Component)this).gameObject.GetComponentsInParent<ValuablePropSwitch>(true);
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if ((Object)(object)componentsInParent[i] != (Object)(object)this)
			{
				Debug.LogError((object)"ValuablePropSwitch: Switches inside switches is not supported...", (Object)(object)((Component)this).gameObject);
			}
		}
		if (!Object.op_Implicit((Object)(object)((Component)this).gameObject.GetComponentInChildren<ValuableVolume>(true)))
		{
			Debug.LogError((object)(((Object)((Component)((Component)this).gameObject.GetComponentInParent<Module>()).gameObject).name + "  |  ValuablePropSwitch: No ValuableVolume found in children..."), (Object)(object)((Component)this).gameObject);
			return;
		}
		bool flag = false;
		ValuableObject[] componentsInChildren = ((Component)this).GetComponentsInChildren<ValuableObject>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (((Component)componentsInChildren[i]).gameObject.activeSelf)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			PropParent.SetActive(false);
			ValuableParent.SetActive(true);
		}
		else
		{
			ValuableParent.SetActive(false);
			PropParent.SetActive(true);
		}
		SetupComplete = true;
	}

	public void DebugToggle()
	{
		if (DebugSwitch)
		{
			DebugSwitch = false;
			DebugState = "Valuable Active";
			if ((Object)(object)ValuableParent != (Object)null)
			{
				ValuableParent.SetActive(true);
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
			if ((Object)(object)ValuableParent != (Object)null)
			{
				ValuableParent.SetActive(false);
			}
		}
	}
}
