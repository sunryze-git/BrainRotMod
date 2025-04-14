using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVolumeCheck : MonoBehaviour
{
	public List<RoomVolume> CurrentRooms;

	public bool Continuous = true;

	internal float PauseCheckTimer;

	private LayerMask Mask;

	[Space]
	public bool DebugCheckPosition;

	public Vector3 CheckPosition = Vector3.one;

	public Vector3 currentSize = Vector3.one;

	internal bool inTruck;

	internal bool inExtractionPoint;

	private bool player;

	private bool checkActive;

	private void Awake()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)((Component)this).GetComponentInParent<PlayerAvatar>()))
		{
			player = true;
		}
		Mask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "RoomVolume" }));
		CheckStart();
	}

	private void OnEnable()
	{
		CheckStart();
	}

	private void OnDisable()
	{
		checkActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	public void CheckSet()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		inTruck = false;
		inExtractionPoint = false;
		CurrentRooms.Clear();
		Vector3 localScale = currentSize;
		if (localScale == Vector3.zero)
		{
			localScale = ((Component)this).transform.localScale;
		}
		Collider[] array = Physics.OverlapBox(((Component)this).transform.position + ((Component)this).transform.rotation * CheckPosition, localScale / 2f, ((Component)this).transform.rotation, LayerMask.op_Implicit(Mask));
		foreach (Collider val in array)
		{
			RoomVolume roomVolume = ((Component)((Component)val).transform).GetComponent<RoomVolume>();
			if (!Object.op_Implicit((Object)(object)roomVolume))
			{
				roomVolume = ((Component)((Component)val).transform).GetComponentInParent<RoomVolume>();
			}
			if (!CurrentRooms.Contains(roomVolume))
			{
				CurrentRooms.Add(roomVolume);
			}
			if (roomVolume.Truck)
			{
				inTruck = true;
			}
			if (roomVolume.Extraction)
			{
				inExtractionPoint = true;
			}
		}
		if (!player || CurrentRooms.Count <= 0)
		{
			return;
		}
		bool flag = true;
		MapModule mapModule = CurrentRooms[0].MapModule;
		foreach (RoomVolume currentRoom in CurrentRooms)
		{
			if ((Object)(object)mapModule != (Object)(object)currentRoom.MapModule)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			CurrentRooms[0].SetExplored();
		}
	}

	private IEnumerator Check()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		yield return (object)new WaitForSeconds(0.5f);
		while (true)
		{
			if (PauseCheckTimer > 0f)
			{
				PauseCheckTimer -= 0.5f;
				yield return (object)new WaitForSeconds(0.5f);
				continue;
			}
			CheckSet();
			if (Continuous)
			{
				if (player)
				{
					yield return (object)new WaitForSeconds(0.1f);
				}
				else
				{
					yield return (object)new WaitForSeconds(0.5f);
				}
				continue;
			}
			break;
		}
	}

	private void CheckStart()
	{
		if (!checkActive)
		{
			checkActive = true;
			((MonoBehaviour)this).StartCoroutine(Check());
		}
	}
}
