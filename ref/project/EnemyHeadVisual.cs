using Photon.Pun;
using UnityEngine;

public class EnemyHeadVisual : MonoBehaviour, IPunObservable
{
	public EnemyHeadController Controller;

	public Enemy enemy;

	private float spawnTimer = 1f;

	[Space]
	public Transform FollowPosition;

	public Transform FollowRotation;

	public Transform TargetRotation;

	private float PositionFollowCurrent;

	private float RotationFollowCurrent;

	[Space]
	[Header("Idle")]
	public float PositionFollowIdle;

	public float RotationFollowIdle;

	[Space]
	[Header("Chasing")]
	public float PositionFollowChasing;

	public float RotationFollowChasing;

	private void Update()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (enemy.FreezeTimer > 0f)
		{
			return;
		}
		if (enemy.MasterClient)
		{
			if (enemy.CheckChase())
			{
				PositionFollowCurrent = PositionFollowChasing;
				RotationFollowCurrent = RotationFollowChasing;
			}
			else
			{
				PositionFollowCurrent = PositionFollowIdle;
				RotationFollowCurrent = RotationFollowIdle;
			}
		}
		if (spawnTimer > 0f || enemy.TeleportedTimer > 0f)
		{
			((Component)this).transform.position = FollowPosition.position;
			TargetRotation.rotation = FollowRotation.rotation;
			if (LevelGenerator.Instance.Generated)
			{
				spawnTimer -= Time.deltaTime;
			}
		}
		else
		{
			((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, FollowPosition.position, PositionFollowCurrent * Time.deltaTime);
			TargetRotation.rotation = Quaternion.Lerp(TargetRotation.rotation, FollowRotation.rotation, RotationFollowCurrent * Time.deltaTime);
		}
	}

	public void Spawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = FollowPosition.position;
		TargetRotation.rotation = FollowRotation.rotation;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext((object)PositionFollowCurrent);
			stream.SendNext((object)RotationFollowCurrent);
		}
		else
		{
			PositionFollowCurrent = (float)stream.ReceiveNext();
			RotationFollowCurrent = (float)stream.ReceiveNext();
		}
	}
}
