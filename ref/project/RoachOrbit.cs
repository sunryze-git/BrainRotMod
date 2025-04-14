using System;
using Photon.Pun;
using UnityEngine;

public class RoachOrbit : MonoBehaviour
{
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	public float radius = 5f;

	public float rotationSpeed = 1f;

	public float noiseScale = 1f;

	public float noiseSpeed = 0.5f;

	public float noiseScale2 = 0.5f;

	public float noiseSpeed2 = 1f;

	private Vector3 startPosition;

	private float noiseOffsetX;

	private float noiseOffsetZ;

	private float noiseOffsetX2;

	private float noiseOffsetZ2;

	private PhotonView photonView;

	public Sound squashSound;

	public Sound roachLoopSound;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		startPosition = ((Component)this).transform.position;
		noiseOffsetX = ((Component)this).transform.position.x;
		noiseOffsetZ = ((Component)this).transform.position.z;
		noiseOffsetX2 = ((Component)this).transform.position.x * 1.5f;
		noiseOffsetZ2 = ((Component)this).transform.position.z * 1.5f;
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	[PunRPC]
	private void SquashRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		squashSound.Play(((Component)this).transform.position);
		Object.Instantiate<GameObject>(roachSmashPrefab, ((Component)this).transform.position, Quaternion.identity);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void Squash()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			squashSound.Play(((Component)this).transform.position);
			Object.Instantiate<GameObject>(roachSmashPrefab, ((Component)this).transform.position, Quaternion.identity);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		else
		{
			photonView.RPC("SquashRPC", (RpcTarget)3, Array.Empty<object>());
		}
	}

	private void Update()
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		roachLoopSound.PlayLoop(playing: true, 1f, 2f);
		float num = ((GameManager.instance.gameMode != 0) ? NetworkManager.instance.gameTime : Time.time);
		float num2 = num * noiseSpeed;
		float num3 = Mathf.PerlinNoise(noiseOffsetX + num2 * noiseScale, 0f) * 2f - 1f;
		float num4 = Mathf.PerlinNoise(0f, noiseOffsetZ + num2 * noiseScale) * 2f - 1f;
		num2 = num * noiseSpeed2;
		float num5 = Mathf.PerlinNoise(noiseOffsetX2 + num2 * noiseScale2, 0f) * 2f - 1f;
		float num6 = Mathf.PerlinNoise(0f, noiseOffsetZ2 + num2 * noiseScale2) * 2f - 1f;
		float num7 = (num3 + num5) / 2f;
		float num8 = (num4 + num6) / 2f;
		Vector3 val = startPosition + new Vector3(num7, 0f, num8) * radius;
		Vector3 val2 = val - ((Component)this).transform.position;
		((Component)this).transform.position = val;
		if (val2 != Vector3.zero)
		{
			Quaternion val3 = Quaternion.LookRotation(val2);
			Quaternion val4 = Quaternion.Euler(0f, -90f, 0f);
			val3 *= val4;
			((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val3, rotationSpeed * Time.deltaTime);
		}
	}
}
