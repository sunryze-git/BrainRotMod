using System.Collections;
using UnityEngine;

public class EnemyPlayerRoom : MonoBehaviour
{
	public RoomVolumeCheck RoomVolumeCheck;

	private bool LogicActive;

	internal bool SameAny;

	internal bool SameLocal;

	private void Start()
	{
		LogicActive = true;
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private void OnEnable()
	{
		if (!LogicActive)
		{
			LogicActive = true;
			((MonoBehaviour)this).StartCoroutine(Logic());
		}
	}

	private void OnDisable()
	{
		LogicActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private IEnumerator Logic()
	{
		while (GameDirector.instance.PlayerList.Count == 0)
		{
			yield return (object)new WaitForSeconds(1f);
		}
		while (true)
		{
			SameAny = false;
			SameLocal = false;
			foreach (RoomVolume currentRoom in RoomVolumeCheck.CurrentRooms)
			{
				foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
				{
					foreach (RoomVolume currentRoom2 in player.RoomVolumeCheck.CurrentRooms)
					{
						if ((Object)(object)currentRoom == (Object)(object)currentRoom2)
						{
							if (!player.isDisabled)
							{
								SameAny = true;
							}
							if (player.isLocal)
							{
								SameLocal = true;
								break;
							}
						}
					}
				}
			}
			yield return (object)new WaitForSeconds(1f);
		}
	}
}
