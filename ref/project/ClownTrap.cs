using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ClownTrap : Trap
{
	public UnityEvent clownTimer;

	public Transform Center;

	private PhysGrabObject physgrabobject;

	private ParticleScriptExplosion particleScriptExplosion;

	private MeshRenderer noseMesh;

	[Space]
	[Header("Clown Components")]
	public GameObject Body;

	public GameObject Arms;

	public GameObject Head;

	public GameObject Nose;

	[Space]
	[Header("Sounds")]
	public Sound NoseSqeak;

	public Sound HeadSpin;

	public Sound ArmRaise;

	public Sound WarningVO1;

	public Sound WarningVO2;

	private AudioSource previousAudioSource;

	public Sound GonnaBlowVO;

	[Space]
	private Quaternion initialClownRotation;

	private int WarningCount = 2;

	private int ExplosionCountDown;

	private bool CountDownActive;

	[Space]
	[Header("Head Spin Animation")]
	public AnimationCurve HeadSpinCurve;

	private bool HeadSpinOneShotActive;

	public float HeadSpinSpeed;

	public float HeadSpinIntensity;

	private float HeadSpinLerp;

	[Space]
	[Header("Arm raise Animation")]
	public AnimationCurve ArmRaiseCurve;

	private bool ArmRaiseActive;

	public float ArmRaiseSpeed;

	public float ArmRaiseIntensity;

	private float ArmRaiseLerp;

	[Space]
	[Header("Nose Squeeze Animation")]
	public AnimationCurve NoseSqueezeCurve;

	private bool NoseSqueezeActive;

	public float NoseSqueezeSpeed;

	public float NoseSqueezeIntensity;

	private float NoseSqueezeLerp;

	protected override void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Start();
		initialClownRotation = Body.transform.localRotation;
		physgrabobject = ((Component)this).GetComponent<PhysGrabObject>();
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		noseMesh = Nose.GetComponent<MeshRenderer>();
	}

	protected override void Update()
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!trapActive)
		{
			return;
		}
		enemyInvestigateRange = 15f;
		if (HeadSpinOneShotActive)
		{
			if (HeadSpinLerp == 0f)
			{
				HeadSpin.Play(physgrabobject.centerPoint);
				enemyInvestigate = true;
			}
			HeadSpinLerp += HeadSpinSpeed * Time.deltaTime;
			if (HeadSpinLerp >= 1f)
			{
				HeadSpinLerp = 0f;
				if (WarningCount > 0)
				{
					WarningCount--;
					switch (WarningCount)
					{
					case 1:
						previousAudioSource = WarningVO1.Play(physgrabobject.centerPoint);
						break;
					case 0:
						if ((Object)(object)previousAudioSource != (Object)null)
						{
							previousAudioSource.Stop();
						}
						previousAudioSource = WarningVO2.Play(physgrabobject.centerPoint);
						break;
					}
					HeadSpinOneShotActive = false;
					HeadSpinDoneLogic();
				}
			}
		}
		Head.transform.localEulerAngles = new Vector3(0f, HeadSpinCurve.Evaluate(HeadSpinLerp) * HeadSpinIntensity, 0f);
		if (ArmRaiseActive)
		{
			if (ArmRaiseLerp == 0f)
			{
				ArmRaise.Play(physgrabobject.centerPoint);
			}
			ArmRaiseLerp += ArmRaiseSpeed * Time.deltaTime;
			if (ArmRaiseLerp >= 1f)
			{
				ArmRaiseLerp = 0f;
				HeadSpinOneShotActive = true;
				if (WarningCount > 0)
				{
					ArmRaiseActive = false;
				}
			}
		}
		Arms.transform.localEulerAngles = new Vector3(0f, 0f, ArmRaiseCurve.Evaluate(ArmRaiseLerp) * ArmRaiseIntensity);
		if (NoseSqueezeActive)
		{
			NoseSqueezeLerp += NoseSqueezeSpeed * Time.deltaTime;
			enemyInvestigate = true;
			if (NoseSqueezeLerp >= 1f)
			{
				NoseSqueezeLerp = 0f;
				if (!CountDownActive)
				{
					((Renderer)noseMesh).material.DisableKeyword("_EMISSION");
				}
				NoseSqueezeActive = false;
			}
		}
		Nose.transform.localScale = new Vector3(1f + NoseSqueezeCurve.Evaluate(NoseSqueezeLerp) * NoseSqueezeIntensity, 1f + NoseSqueezeCurve.Evaluate(NoseSqueezeLerp) * NoseSqueezeIntensity, 1f + NoseSqueezeCurve.Evaluate(NoseSqueezeLerp) * NoseSqueezeIntensity);
		if (CountDownActive)
		{
			float num = ExplosionCountDown / 50;
			float num2 = ExplosionCountDown / 10;
			float num3 = num * Mathf.Sin(Time.time * num2);
			float num4 = num * Mathf.Sin(Time.time * num2 + MathF.PI / 2f);
			Body.transform.localRotation = initialClownRotation * Quaternion.Euler(0f, num3, num4);
			Body.transform.localPosition = new Vector3(Body.transform.localPosition.x, Body.transform.localPosition.y - num3 * 0.005f * Time.deltaTime, Body.transform.localPosition.z);
		}
	}

	private void FixedUpdate()
	{
		if (CountDownActive)
		{
			ExplosionCountDown++;
		}
	}

	public void TrapStop()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		trapActive = false;
		if ((Object)(object)previousAudioSource != (Object)null)
		{
			previousAudioSource.Stop();
		}
		particleScriptExplosion.Spawn(Center.position, 1.5f, 100, 300);
		physgrabobject.dead = true;
	}

	private void TrapActivate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (trapTriggered)
		{
			return;
		}
		NoseSqeak.Play(physgrabobject.centerPoint);
		((Renderer)noseMesh).material.EnableKeyword("_EMISSION");
		NoseSqueezeActive = true;
		ArmRaiseActive = true;
		trapActive = true;
		trapTriggered = true;
		if (WarningCount <= 0)
		{
			if ((Object)(object)previousAudioSource != (Object)null)
			{
				previousAudioSource.Stop();
			}
			previousAudioSource = GonnaBlowVO.Play(physgrabobject.centerPoint);
			ArmRaiseActive = true;
			((Renderer)noseMesh).material.EnableKeyword("_EMISSION");
			clownTimer.Invoke();
			CountDownActive = true;
		}
	}

	private void TouchNoseLogic()
	{
		TrapActivate();
	}

	public void TouchNose()
	{
		if (GameManager.instance.gameMode == 0)
		{
			TouchNoseLogic();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("TouchNoseRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void TouchNoseRPC()
	{
		TouchNoseLogic();
	}

	private void HeadSpinDoneLogic()
	{
		trapTriggered = false;
	}

	private void HeadSpinDone()
	{
		if (GameManager.instance.gameMode == 0)
		{
			HeadSpinDoneLogic();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("HeadSpinDoneRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void HeadSpinDoneRPC()
	{
		HeadSpinDoneLogic();
	}
}
