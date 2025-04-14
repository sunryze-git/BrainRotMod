using Photon.Pun;
using UnityEngine;

public class PlayerPhysPusher : MonoBehaviour
{
	private PhotonView PhotonView;

	private Rigidbody Rigidbody;

	public PlayerAvatar Player;

	[Space]
	public Transform ColliderTarget;

	public Transform Collider;

	internal bool Reset;

	private Vector3 PreviousVelocity;

	private void Awake()
	{
		PhotonView = ((Component)this).GetComponent<PhotonView>();
		Rigidbody = ((Component)this).GetComponent<Rigidbody>();
	}

	private void Start()
	{
		if (GameManager.instance.gameMode == 0 || !PhotonNetwork.IsMasterClient || PhotonView.IsMine)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void FixedUpdate()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (Player.isDisabled || Player.isTumbling || ((Vector3)(ref Player.rbVelocity)).magnitude < 0.1f)
		{
			((Component)Collider).gameObject.SetActive(false);
		}
		else
		{
			((Component)Collider).gameObject.SetActive(true);
		}
		float num = Vector3.Distance(((Component)this).transform.position, ColliderTarget.position);
		if ((Reset && num > 0.5f) || num > 1f || ((Vector3)(ref Player.rbVelocity)).magnitude < 0.1f || Vector3.Dot(Player.rbVelocity, PreviousVelocity) < 0.25f)
		{
			Rigidbody.MovePosition(ColliderTarget.position);
			Reset = false;
		}
		Rigidbody.MoveRotation(ColliderTarget.rotation);
		Vector3 val = ((Component)this).transform.InverseTransformDirection(Rigidbody.velocity);
		Rigidbody.AddRelativeForce(Player.rbVelocity - val, (ForceMode)1);
		PreviousVelocity = Player.rbVelocity;
	}

	private void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Collider.localScale = ColliderTarget.localScale;
	}
}
