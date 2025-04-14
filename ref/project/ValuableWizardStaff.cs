using Photon.Pun;
using UnityEngine;

public class ValuableWizardStaff : MonoBehaviour
{
	private PhotonView photonView;

	private float laserTimer;

	public SemiLaser semiLaser;

	public Transform laserTransform;

	private Rigidbody rb;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		rb = ((Component)this).GetComponent<Rigidbody>();
	}

	private void Update()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (laserTimer > 0f)
		{
			laserTimer -= Time.deltaTime;
			Vector3 endPosition = laserTransform.position + laserTransform.forward * 15f;
			bool isHitting = false;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(laserTransform.position, laserTransform.forward, ref val, 15f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())))
			{
				endPosition = ((RaycastHit)(ref val)).point;
				isHitting = true;
			}
			semiLaser.LaserActive(laserTransform.position, endPosition, isHitting);
		}
	}

	private void FixedUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.IsMasterClientOrSingleplayer() && laserTimer > 0f)
		{
			Vector3 val = -laserTransform.forward * 1000f * Time.fixedDeltaTime;
			rb.AddForce(val, (ForceMode)0);
		}
	}

	public void StaffLaser()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			float num = Random.Range(1f, 4f);
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("StaffLaserRPC", (RpcTarget)0, new object[1] { num });
			}
			else
			{
				StaffLaserRPC(num);
			}
		}
	}

	[PunRPC]
	public void StaffLaserRPC(float _time)
	{
		laserTimer = _time;
	}
}
