using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TrapDirector : MonoBehaviourPunCallbacks, IPunObservable
{
	public static TrapDirector instance;

	public bool DebugAllTraps;

	[Space]
	public List<GameObject> TrapList = new List<GameObject>();

	public List<GameObject> SelectedTraps = new List<GameObject>();

	public float TrapCooldown;

	internal bool TrapListUpdated;

	public int TrapCount = 2;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(Generate());
	}

	private void Update()
	{
		if (TrapCooldown > 0f)
		{
			TrapCooldown -= Time.deltaTime;
		}
	}

	private IEnumerator Generate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			UpdateTrapList();
		}
		TrapListUpdated = true;
	}

	private void UpdateTrapList()
	{
		Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
		foreach (GameObject trap in TrapList)
		{
			TrapTypeIdentifier component = trap.GetComponent<TrapTypeIdentifier>();
			if ((Object)(object)component != (Object)null)
			{
				string trapType = component.trapType;
				if (!dictionary.ContainsKey(trapType))
				{
					dictionary[trapType] = new List<GameObject>();
				}
				dictionary[trapType].Add(trap);
			}
		}
		if (DebugAllTraps)
		{
			return;
		}
		foreach (KeyValuePair<string, List<GameObject>> item2 in dictionary)
		{
			if (item2.Value.Count > 0)
			{
				GameObject item = item2.Value[Random.Range(0, item2.Value.Count)];
				SelectedTraps.Add(item);
			}
		}
		foreach (GameObject trap2 in TrapList)
		{
			if (SelectedTraps.Contains(trap2))
			{
				continue;
			}
			TrapTypeIdentifier component2 = trap2.GetComponent<TrapTypeIdentifier>();
			if (!((Object)(object)component2 != (Object)null))
			{
				continue;
			}
			if ((Object)(object)component2.Trigger != (Object)null && component2.OnlyRemoveTrigger)
			{
				if (GameManager.instance.gameMode == 0)
				{
					Object.Destroy((Object)(object)component2.Trigger);
					component2.TriggerRemoved = true;
				}
				else
				{
					trap2.GetComponent<PhotonView>().RPC("DestroyTrigger", (RpcTarget)3, Array.Empty<object>());
				}
			}
			else if (GameManager.instance.gameMode == 0)
			{
				Object.Destroy((Object)(object)trap2);
			}
			else
			{
				trap2.GetComponent<PhotonView>().RPC("DestroyTrap", (RpcTarget)3, Array.Empty<object>());
			}
		}
		while (SelectedTraps.Count > TrapCount)
		{
			int index = Random.Range(0, SelectedTraps.Count);
			GameObject val = SelectedTraps[index];
			SelectedTraps.RemoveAt(index);
			TrapTypeIdentifier component3 = val.GetComponent<TrapTypeIdentifier>();
			if (!((Object)(object)component3 != (Object)null))
			{
				continue;
			}
			if ((Object)(object)component3.Trigger != (Object)null)
			{
				if (GameManager.instance.gameMode == 0)
				{
					Object.Destroy((Object)(object)component3.Trigger);
					component3.TriggerRemoved = true;
				}
				else
				{
					val.GetComponent<PhotonView>().RPC("DestroyTrigger", (RpcTarget)3, Array.Empty<object>());
				}
			}
			else if (GameManager.instance.gameMode == 0)
			{
				Object.Destroy((Object)(object)val);
			}
			else
			{
				val.GetComponent<PhotonView>().RPC("DestroyTrap", (RpcTarget)3, Array.Empty<object>());
			}
		}
	}

	private string RandomType(Dictionary<string, List<GameObject>> trapsByType)
	{
		List<string> list = new List<string>(trapsByType.Keys);
		int index = Random.Range(0, list.Count);
		return list[index];
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext((object)TrapListUpdated);
		}
		else
		{
			TrapListUpdated = (bool)stream.ReceiveNext();
		}
	}
}
