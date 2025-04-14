using Photon.Pun;
using UnityEngine;

public class PlayerAvatarCollision : MonoBehaviourPunCallbacks, IPunObservable
{
	public PlayerAvatar PlayerAvatar;

	private PlayerController PlayerController;

	public Transform CollisionTransform;

	public CapsuleCollider Collider;

	private Vector3 Scale;

	internal Vector3 deathHeadPosition;

	private void Start()
	{
		PlayerController = PlayerController.instance;
	}

	private void Update()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerAvatar.isLocal)
		{
			Scale = ((Component)PlayerController.PlayerCollision).transform.localScale;
			((Collider)Collider).enabled = false;
		}
		CollisionTransform.localScale = Scale;
		deathHeadPosition = CollisionTransform.position + Vector3.up * (Collider.height * CollisionTransform.localScale.y - 0.18f);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (stream.IsWriting)
		{
			stream.SendNext((object)Scale);
		}
		else
		{
			Scale = (Vector3)stream.ReceiveNext();
		}
	}

	public void SetCrouch()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Scale = PlayerCollision.instance.CrouchCollision.localScale;
		CollisionTransform.localScale = Scale;
	}
}
