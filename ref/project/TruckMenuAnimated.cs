using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TruckMenuAnimated : MonoBehaviour
{
	public Transform antennaMiddleTransform;

	public Transform antennaMiddleTarget;

	public SpringQuaternion antennaMiddleSpring;

	[Space(20f)]
	public Transform frontPanelTransform;

	public Transform frontPanelTarget;

	public SpringQuaternion frontPanelSpring;

	[Space(20f)]
	public Transform windowRightTransform;

	public Transform windowRightTarget;

	public SpringQuaternion windowRightSpring;

	[Space(20f)]
	public Transform windowLeftTransform;

	public Transform windowLeftTarget;

	public SpringQuaternion windowLeftSpring;

	[Space(20f)]
	public Transform dishTransform;

	public Transform dishTarget;

	public SpringQuaternion dishSpring;

	[Space(20f)]
	public Transform antennaBackTransform;

	public Transform antennaBackTarget;

	public SpringQuaternion antennaBackSpring;

	private Animator animator;

	private PhotonView photonView;

	private float breakerCooldown;

	private float breakerCooldownMin = 8f;

	private float breakerCooldownMax = 16f;

	private List<string> breakerTriggers = new List<string>();

	private int breakerTriggerIndex;

	public ParticleSystem particleSkeletonBitsFirst;

	public ParticleSystem particleSkeletonSmokeFirst;

	public ParticleSystem particleSkeletonBitsLast;

	public ParticleSystem particleSkeletonSmokeLast;

	public Sound soundLoop;

	[Space]
	public Sound soundSwerve;

	public Sound soundSpeedUp;

	public Sound soundSlowDown;

	[Space]
	public Sound soundBodyRustleLong01;

	public Sound soundBodyRustleLong02;

	public Sound soundBodyRustleLong03;

	[Space]
	public Sound soundBodyRustleShort01;

	public Sound soundBodyRustleShort02;

	public Sound soundBodyRustleShort03;

	[Space]
	public Sound soundSkeletonHit;

	public Sound soundSkeletonHitSkull;

	[Space]
	public Sound soundSwerveFast01;

	public Sound soundSwerveFast02;

	[Space]
	public Sound soundFirePass;

	public Sound soundFirePassSwerve01;

	public Sound soundFirePassSwerve02;

	private void Start()
	{
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
		photonView = ((Component)this).GetComponent<PhotonView>();
		breakerCooldown = Random.Range(breakerCooldownMin, breakerCooldownMax);
		breakerTriggers.Add("SpeedUp");
		breakerTriggers.Add("SlowDown");
		breakerTriggers.Add("Swerve");
		breakerTriggers.Add("SkeletonHit");
		breakerTriggers.Add("TruckPass");
		breakerTriggers.Shuffle();
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		antennaMiddleTransform.rotation = SemiFunc.SpringQuaternionGet(antennaMiddleSpring, antennaMiddleTarget.rotation);
		frontPanelTransform.rotation = SemiFunc.SpringQuaternionGet(frontPanelSpring, frontPanelTarget.rotation);
		frontPanelTransform.localEulerAngles = new Vector3(0f, frontPanelTransform.localEulerAngles.y, 0f);
		windowRightTransform.rotation = SemiFunc.SpringQuaternionGet(windowRightSpring, windowRightTarget.rotation);
		windowRightTransform.localEulerAngles = new Vector3(windowRightTransform.localEulerAngles.x, 0f, 0f);
		windowLeftTransform.rotation = SemiFunc.SpringQuaternionGet(windowLeftSpring, windowLeftTarget.rotation);
		windowLeftTransform.localEulerAngles = new Vector3(windowLeftTransform.localEulerAngles.x, 0f, 0f);
		dishTransform.rotation = SemiFunc.SpringQuaternionGet(dishSpring, dishTarget.rotation);
		antennaBackTransform.rotation = SemiFunc.SpringQuaternionGet(antennaBackSpring, antennaBackTarget.rotation);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Idle"))
			{
				breakerCooldown -= Time.deltaTime;
				if (breakerCooldown <= 0f)
				{
					BreakerTrigger();
				}
			}
		}
		soundLoop.PlayLoop(playing: true, 2f, 2f);
	}

	private void BreakerTrigger()
	{
		breakerCooldown = Random.Range(breakerCooldownMin, breakerCooldownMax);
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("BreakerTriggerRPC", (RpcTarget)0, new object[1] { breakerTriggers[breakerTriggerIndex] });
		}
		else
		{
			BreakerTriggerRPC(breakerTriggers[breakerTriggerIndex]);
		}
		breakerTriggerIndex++;
		if (breakerTriggerIndex >= breakerTriggers.Count)
		{
			string text = breakerTriggers[breakerTriggers.Count - 1];
			breakerTriggers.Shuffle();
			while (breakerTriggers[0] == text)
			{
				breakerTriggers.Shuffle();
			}
			breakerTriggerIndex = 0;
		}
	}

	[PunRPC]
	private void BreakerTriggerRPC(string _triggerName)
	{
		if (Object.op_Implicit((Object)(object)animator))
		{
			animator.SetTrigger(_triggerName);
		}
	}

	public void SkeletonHitFirstImpulse()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSkeletonHit.Play(((Component)soundLoop.Source).transform.position);
		particleSkeletonBitsFirst.Play();
		particleSkeletonSmokeFirst.Play();
		GameDirector.instance.CameraShake.Shake(1f, 0.3f);
		GameDirector.instance.CameraImpact.Shake(0.5f, 0.1f);
	}

	public void SkeletonHitLastImpulse()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSkeletonHitSkull.Play(((Component)soundLoop.Source).transform.position);
		particleSkeletonBitsLast.Play();
		particleSkeletonSmokeLast.Play();
	}

	public void PlaySwerve()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSwerve.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlaySpeedUp()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSpeedUp.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlaySlowDown()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSlowDown.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleLong01()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleLong01.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleLong02()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleLong02.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleLong03()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleLong03.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleShort01()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleShort01.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleShort02()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleShort02.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayBodyRustleShort03()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundBodyRustleShort03.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlaySwerveFast01()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSwerveFast01.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlaySwerveFast02()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundSwerveFast02.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayFirePass()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundFirePass.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayFirePassSwerve01()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundFirePassSwerve01.Play(((Component)soundLoop.Source).transform.position);
	}

	public void PlayFirePassSwerve02()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		soundFirePassSwerve02.Play(((Component)soundLoop.Source).transform.position);
	}

	private void OnDrawGizmos()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.matrix = antennaMiddleTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 6f);
		Gizmos.matrix = frontPanelTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 1.5f);
		Gizmos.matrix = windowRightTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = windowLeftTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * -2.5f);
		Gizmos.matrix = dishTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 4f);
		Gizmos.matrix = antennaBackTarget.localToWorldMatrix;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 8f);
	}
}
