using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class CleanDirector : MonoBehaviour
{
	[Serializable]
	public class CleaningSpots
	{
		public Interaction.InteractionType InteractionType;

		public int CleaningSpotsMax;
	}

	public static CleanDirector instance;

	public List<GameObject> CleanList = new List<GameObject>();

	public List<CleaningSpots> cleaningSpots;

	internal bool RemoveExcessSpots;

	private PhotonView photonView;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		instance = this;
	}

	private void RandomlyRemoveExcessSpots()
	{
		if (!PhotonNetwork.IsMasterClient && GameManager.instance.gameMode != 0)
		{
			return;
		}
		foreach (CleaningSpots cleaningSpot in cleaningSpots)
		{
			Interaction.InteractionType type = cleaningSpot.InteractionType;
			int cleaningSpotsMax = cleaningSpot.CleaningSpotsMax;
			for (int num = CleanList.Count((GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type); num > cleaningSpotsMax; num--)
			{
				List<GameObject> list = CleanList.Where((GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type).ToList();
				int index = Random.Range(0, list.Count);
				GameObject val = list[index];
				if (GameManager.instance.gameMode == 1)
				{
					if ((Object)(object)val.GetComponent<PhotonView>() == (Object)null)
					{
						Debug.LogWarning((object)("Photon View not found for: " + ((Object)val).name));
					}
					CleanList.Remove(val);
					PhotonNetwork.Destroy(val);
				}
				else
				{
					CleanList.Remove(val);
					Object.Destroy((Object)(object)val);
				}
			}
		}
	}

	private void Update()
	{
		if (!RemoveExcessSpots && GameDirector.instance.currentState >= GameDirector.gameState.Start)
		{
			RandomlyRemoveExcessSpots();
			RemoveExcessSpots = true;
		}
	}
}
