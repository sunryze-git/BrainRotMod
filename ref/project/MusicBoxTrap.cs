using Photon.Pun;
using UnityEngine;

public class MusicBoxTrap : Trap
{
	public Transform colliderLid;

	public Transform colliderDancers;

	private PhysGrabObject physgrabObject;

	private CollisionFree colliderLidCollision;

	private CollisionFree colliderDancersCollision;

	public Transform MusicBoxRattler;

	public Transform MusicBoxDancerSpin;

	public Transform MusicBoxDancer;

	public Transform MusicBoxLid;

	public Transform PedestalTransform;

	[Space]
	public AnimationCurve MusicBoxLidCurve;

	public AnimationCurve MusicBoxLidRattlerCurve;

	[Space]
	[Header("Sounds")]
	public Sound MusicBoxOpenSound;

	public Sound MusicBoxCloseSound;

	public Sound MusicBoxMusic;

	private float MusicBoxLidDuration = 0.5f;

	private float MusicBoxLidProgress;

	private bool MusicBoxOpenAnimationActive;

	private bool MusicBoxCloseAnimationActive;

	private bool MusicBoxPlaying;

	private bool openTheBox;

	private Rigidbody rb;

	protected override void Start()
	{
		base.Start();
		rb = ((Component)this).GetComponent<Rigidbody>();
		physgrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		((Component)MusicBoxDancer).gameObject.SetActive(false);
		((Component)PedestalTransform).gameObject.SetActive(false);
		colliderLidCollision = ((Component)colliderLid).GetComponent<CollisionFree>();
		colliderDancersCollision = ((Component)colliderDancers).GetComponent<CollisionFree>();
	}

	protected override void Update()
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!trapTriggered && physgrabObject.grabbed)
		{
			trapStart = true;
		}
		if (trapStart && !MusicBoxOpenAnimationActive && !MusicBoxCloseAnimationActive)
		{
			MusicBoxStart();
		}
		MusicBoxMusic.PlayLoop(MusicBoxPlaying, 2f, 2f);
		if (openTheBox && !colliderDancersCollision.colliding && !colliderLidCollision.colliding)
		{
			if (GameManager.instance.gameMode == 0)
			{
				float musicTime = Random.Range(0f, MusicBoxMusic.Source.clip.length);
				OpenTheBox(musicTime);
			}
			else if (PhotonNetwork.IsMasterClient)
			{
				float num = Random.Range(0f, MusicBoxMusic.Source.clip.length);
				photonView.RPC("OpenTheBox", (RpcTarget)0, new object[1] { num });
			}
			openTheBox = false;
		}
		if (MusicBoxOpenAnimationActive)
		{
			MusicBoxPlaying = true;
			MusicBoxLidProgress += Time.deltaTime;
			float num2 = MusicBoxLidCurve.Evaluate(MusicBoxLidProgress / MusicBoxLidDuration);
			MusicBoxLid.localRotation = Quaternion.Euler(-90f + (0f - num2) * 100f, 0f, 0f);
			float num3 = MusicBoxLidRattlerCurve.Evaluate(MusicBoxLidProgress / MusicBoxLidDuration);
			MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, (0f - num3) * 300f);
			PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num2), 1f);
			float num4 = Mathf.Lerp(0.5f, 3f, num2);
			MusicBoxDancer.localScale = new Vector3(num4, num4, num4);
			if (MusicBoxLidProgress >= MusicBoxLidDuration)
			{
				MusicBoxOpenAnimationActive = false;
				MusicBoxLidProgress = 0f;
			}
		}
		if (MusicBoxCloseAnimationActive)
		{
			MusicBoxLidProgress += Time.deltaTime;
			float num5 = 1f - MusicBoxLidCurve.Evaluate(MusicBoxLidProgress / MusicBoxLidDuration);
			MusicBoxLid.localRotation = Quaternion.Euler(-90f + (0f - num5) * 100f, 0f, 0f);
			float num6 = MusicBoxLidRattlerCurve.Evaluate(MusicBoxLidProgress / MusicBoxLidDuration);
			MusicBoxRattler.localRotation = Quaternion.Euler(0f, 0f, (0f - num6) * 300f);
			PedestalTransform.localScale = new Vector3(1f, Mathf.Lerp(0.15f, 1f, num5), 1f);
			float num7 = Mathf.Lerp(0.5f, 3f, num5);
			MusicBoxDancer.localScale = new Vector3(num7, num7, num7);
			if (MusicBoxLidProgress >= MusicBoxLidDuration)
			{
				MusicBoxCloseAnimationActive = false;
				MusicBoxLidProgress = 0f;
				MusicBoxPlaying = false;
				((Component)MusicBoxDancer).gameObject.SetActive(false);
				((Component)PedestalTransform).gameObject.SetActive(false);
			}
		}
		if (!MusicBoxPlaying)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Quaternion turnX = Quaternion.Euler(0f, 180f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			bool flag = false;
			foreach (PhysGrabber item in physGrabObject.playerGrabbing)
			{
				if (item.isRotating)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				physGrabObject.TurnXYZ(turnX, turnY, identity);
			}
		}
		if (Object.op_Implicit((Object)(object)PhysGrabber.instance) && (Object)(object)PhysGrabber.instance.grabbedObject == (Object)(object)rb)
		{
			CameraAim.Instance.AimTargetSoftSet(((Component)physGrabObject).transform.position + ((Component)CameraAim.Instance).transform.right * 100f, 0.01f, 1f, 1f, ((Component)this).gameObject, 100);
			PhysGrabber.instance.OverrideGrabDistance(1f);
		}
		enemyInvestigate = true;
		MusicBoxDancer.Rotate(0f, 0f, 40f * Time.deltaTime);
		MusicBoxDancerSpin.Rotate(0f, 20f * Time.deltaTime, 0f);
		if (!MusicBoxOpenAnimationActive)
		{
			float num8 = Mathf.Sin(Time.time * 50f) * 0.1f + Mathf.Sin(Time.time * 20f) * 0.1f - Mathf.Sin(Time.time * 70f) * 0.1f;
			float num9 = Mathf.Sin(Time.time * 70f) * 0.1f + Mathf.Sin(Time.time * 10f) * 0.1f - Mathf.Sin(Time.time * 50f) * 0.1f;
			MusicBoxRattler.localRotation = Quaternion.Euler((0f - num9) * 5f, 0f, num8 * 5f);
		}
		if (!physgrabObject.grabbed && !MusicBoxOpenAnimationActive)
		{
			MusicBoxStop();
		}
	}

	public void MusicBoxStop()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		MusicBoxPlaying = false;
		MusicBoxCloseAnimationActive = true;
		trapTriggered = false;
		trapStart = false;
		MusicBoxCloseSound.Play(physgrabObject.centerPoint);
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		((Collider)((Component)colliderDancers).GetComponent<BoxCollider>()).isTrigger = true;
		((Component)colliderDancers).tag = "Untagged";
		((Component)colliderDancers).gameObject.layer = 13;
		((Collider)((Component)colliderLid).GetComponent<BoxCollider>()).isTrigger = true;
		((Component)colliderLid).tag = "Untagged";
		((Component)colliderLid).gameObject.layer = 13;
	}

	public void MusicBoxStart()
	{
		if (!trapTriggered)
		{
			trapTriggered = true;
			openTheBox = true;
		}
	}

	[PunRPC]
	private void OpenTheBox(float musicTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		MusicBoxOpenAnimationActive = true;
		MusicBoxOpenSound.Play(physgrabObject.centerPoint);
		((Component)MusicBoxDancer).gameObject.SetActive(true);
		((Component)PedestalTransform).gameObject.SetActive(true);
		openTheBox = false;
		((Collider)((Component)colliderDancers).GetComponent<BoxCollider>()).isTrigger = false;
		((Component)colliderDancers).tag = "Phys Grab Object";
		((Component)colliderDancers).gameObject.layer = 16;
		((Collider)((Component)colliderLid).GetComponent<BoxCollider>()).isTrigger = false;
		((Component)colliderLid).tag = "Phys Grab Object";
		((Component)colliderLid).gameObject.layer = 16;
		MusicBoxMusic.StartTimeOverride = musicTime;
	}
}
