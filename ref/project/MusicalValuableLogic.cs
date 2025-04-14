using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicalValuableLogic : MonoBehaviour
{
	private PhotonView photonView;

	[FormerlySerializedAs("pianoKeysStart")]
	public Transform musicKeysStart;

	[FormerlySerializedAs("pianoKeysEnd")]
	public Transform musicKeysEnd;

	[Range(0f, 1f)]
	public float volume = 0.25f;

	[Range(0f, 3f)]
	public float lowKeyAmpAmount;

	[FormerlySerializedAs("pitchShift")]
	public bool hasPitchShift;

	public float pitchShiftAmount = 1f;

	public int numberOfOctaves = 6;

	public List<Sound> musicKeys;

	private PhysGrabObject physGrabObject;

	private PhysGrabObjectGrabArea grabArea;

	private int numberOfKeys = 108;

	private Dictionary<AudioSource, PhysGrabber> currentlyPlayedKeys = new Dictionary<AudioSource, PhysGrabber>();

	private bool grabbedByLocalPlayer;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		grabArea = ((Component)this).GetComponent<PhysGrabObjectGrabArea>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		numberOfKeys = musicKeys.Count * numberOfOctaves;
	}

	private void RemovePhysGrabberFromDictionary(PhysGrabber _physGrabber)
	{
		foreach (KeyValuePair<AudioSource, PhysGrabber> item in currentlyPlayedKeys.ToList())
		{
			AudioSource key = item.Key;
			if ((Object)(object)item.Value == (Object)(object)_physGrabber)
			{
				key.priority = 50;
				currentlyPlayedKeys.Remove(key);
				break;
			}
		}
	}

	private void UpdateGrabbedByLocalPlayerGrabRelease()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			RemovePhysGrabberFromDictionary(PhysGrabber.instance);
			return;
		}
		photonView.RPC("UpdateGrabbedByThisPhysGrabberGrabReleaseRPC", (RpcTarget)0, new object[1] { PhysGrabber.instance.photonView.ViewID });
	}

	[PunRPC]
	public void UpdateGrabbedByThisPhysGrabberGrabReleaseRPC(int physGrabberPhotonViewID)
	{
		PhysGrabber component = ((Component)PhotonView.Find(physGrabberPhotonViewID)).GetComponent<PhysGrabber>();
		RemovePhysGrabberFromDictionary(component);
	}

	private void PitchShiftLogic()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (PhysGrabber.instance.grabbed && (Object)(object)PhysGrabber.instance.grabbedPhysGrabObject == (Object)(object)physGrabObject)
		{
			grabbedByLocalPlayer = true;
		}
		else
		{
			if (grabbedByLocalPlayer)
			{
				UpdateGrabbedByLocalPlayerGrabRelease();
			}
			grabbedByLocalPlayer = false;
		}
		foreach (KeyValuePair<AudioSource, PhysGrabber> item in currentlyPlayedKeys.ToList())
		{
			AudioSource key = item.Key;
			PhysGrabber value = item.Value;
			if (!Object.op_Implicit((Object)(object)key) || (Object)(object)value == (Object)null)
			{
				currentlyPlayedKeys.Remove(key);
				continue;
			}
			Vector3 physGrabPointPullerPosition = value.physGrabPointPullerPosition;
			Vector3 position = value.physGrabPoint.position;
			float forceMax = value.forceMax;
			Vector3 val = Vector3.ClampMagnitude(physGrabPointPullerPosition - position, forceMax) * 10f;
			float num = Mathf.Clamp(1f + ((Vector3)(ref val)).magnitude / forceMax, 1f, 1f + pitchShiftAmount);
			key.pitch = Mathf.Lerp(key.pitch, num, Time.deltaTime * 10f);
			key.priority = 20;
		}
	}

	private void Update()
	{
		if (hasPitchShift)
		{
			PitchShiftLogic();
		}
	}

	public void MusicKeyPressed()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		int num = numberOfKeys;
		PlayerAvatar latestGrabber = grabArea.GetLatestGrabber();
		Vector3 position = latestGrabber.physGrabber.physGrabPoint.position;
		Vector3 position2 = musicKeysStart.position;
		Vector3 position3 = musicKeysEnd.position;
		Vector3 val = position3 - position2;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float num2 = Vector3.Dot(position - position2, normalized);
		float num3 = Vector3.Distance(position2, position3);
		int num4;
		if (num2 <= 0f)
		{
			num4 = 0;
		}
		else if (num2 >= num3)
		{
			num4 = num - 1;
		}
		else
		{
			float num5 = num3 / (float)num;
			num4 = (int)(num2 / num5);
		}
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("MusicKeyPressedRPC", (RpcTarget)0, new object[2]
			{
				num4,
				latestGrabber.physGrabber.photonView.ViewID
			});
		}
		else
		{
			MusicKeyPressedRPC(num4);
		}
	}

	[PunRPC]
	public void MusicKeyPressedRPC(int keyIndex, int grabberID = -1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		PlayKey(keyIndex, grabberID);
		SemiFunc.EnemyInvestigate(physGrabObject.midPoint, 25f);
	}

	private void PlayKey(int key, int grabberID = -1)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.05f;
		int num2 = 0;
		int num3 = numberOfKeys / musicKeys.Count;
		for (int i = 0; i < numberOfKeys; i++)
		{
			int index = i % musicKeys.Count;
			if (key >= num2 && key < num2 + num3)
			{
				PhysGrabber physGrabber = null;
				physGrabber = ((grabberID == -1) ? PhysGrabber.instance : ((Component)PhotonView.Find(grabberID)).GetComponent<PhysGrabber>());
				int num4 = 0;
				int num5 = numberOfKeys - 1;
				float num6 = Mathf.Clamp(1f - (float)(key - num4) / (float)(num5 - num4), 0f, 1f) * lowKeyAmpAmount;
				musicKeys[index].Volume = volume * (1f + num6);
				musicKeys[index].Pitch = 1f + (float)(key - num2) * num;
				AudioSource val = musicKeys[index].Play(physGrabObject.midPoint);
				val.priority = 225;
				if (hasPitchShift && Object.op_Implicit((Object)(object)physGrabber))
				{
					currentlyPlayedKeys.Add(val, physGrabber);
				}
			}
			num2 += num3;
		}
	}
}
