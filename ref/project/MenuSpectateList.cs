using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuSpectateList : SemiUI
{
	public GameObject menuPlayerListedPrefab;

	internal List<PlayerAvatar> spectatingPlayers = new List<PlayerAvatar>();

	internal List<GameObject> listObjects = new List<GameObject>();

	private float listCheckTimer;

	protected override void Update()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!SemiFunc.IsMultiplayer())
		{
			Hide();
			return;
		}
		if (!Object.op_Implicit((Object)(object)SpectateCamera.instance))
		{
			Hide();
			return;
		}
		SemiUIScoot(new Vector2(0f, (float)listObjects.Count * 22f));
		listCheckTimer -= Time.deltaTime;
		if (!(listCheckTimer <= 0f))
		{
			return;
		}
		listCheckTimer = 1f;
		List<PlayerAvatar> list = SemiFunc.PlayerGetList();
		bool flag = false;
		foreach (PlayerAvatar item in list)
		{
			if (item.isDisabled)
			{
				if (!spectatingPlayers.Contains(item))
				{
					PlayerAdd(item);
					flag = true;
				}
			}
			else if (spectatingPlayers.Contains(item))
			{
				PlayerRemove(item);
				flag = true;
			}
		}
		foreach (PlayerAvatar item2 in spectatingPlayers.ToList())
		{
			if (!list.Contains(item2))
			{
				PlayerRemove(item2);
				flag = true;
			}
		}
		if (flag)
		{
			listObjects.Sort((GameObject a, GameObject b) => a.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID.CompareTo(b.GetComponent<MenuPlayerListed>().playerAvatar.photonView.ViewID));
			for (int i = 0; i < listObjects.Count; i++)
			{
				listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
				listObjects[i].transform.SetSiblingIndex(i);
			}
		}
	}

	private void PlayerAdd(PlayerAvatar player)
	{
		spectatingPlayers.Add(player);
		GameObject val = Object.Instantiate<GameObject>(menuPlayerListedPrefab, ((Component)this).transform);
		MenuPlayerListed component = val.GetComponent<MenuPlayerListed>();
		component.playerAvatar = player;
		component.playerHead.SetPlayer(player);
		listObjects.Add(val);
		component.listSpot = Mathf.Max(listObjects.Count - 1, 0);
	}

	private void PlayerRemove(PlayerAvatar player)
	{
		spectatingPlayers.Remove(player);
		foreach (GameObject listObject in listObjects)
		{
			if ((Object)(object)listObject.GetComponent<MenuPlayerListed>().playerAvatar == (Object)(object)player)
			{
				listObject.GetComponent<MenuPlayerListed>().MenuPlayerListedOutro();
				listObjects.Remove(listObject);
				break;
			}
		}
		for (int i = 0; i < listObjects.Count; i++)
		{
			listObjects[i].GetComponent<MenuPlayerListed>().listSpot = i;
		}
	}
}
