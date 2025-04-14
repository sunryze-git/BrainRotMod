using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TruckScreenOpen : MonoBehaviour
{
	public AnimationCurve openScreenCurve;

	private float openScreenCurveTimer;

	private float openScreenYPosOriginal;

	private bool openScreenActive;

	private bool doorDone;

	private bool doorLoopPlaying;

	public Sound doorLoop;

	public Sound doorLoopStart;

	public Sound doorLoopEnd;

	public Sound doorSound;

	private ParticleSystem doorParticles;

	private bool doorClose;

	private float doorOpenPosition = 4.13f;

	private PhotonView photonView;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		openScreenYPosOriginal = ((Component)this).transform.localPosition.y;
		doorParticles = ((Component)this).GetComponentInChildren<ParticleSystem>();
		((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, openScreenYPosOriginal + doorOpenPosition, ((Component)this).transform.localPosition.z);
		((MonoBehaviour)this).StartCoroutine(DelayedClose());
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void TruckScreenOpenStartLogic()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, ((Component)this).transform.position, 0.2f);
		((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, openScreenYPosOriginal, ((Component)this).transform.localPosition.z);
		openScreenCurveTimer = 0f;
		doorLoopStart.Play(((Component)this).transform.position);
		doorLoopPlaying = true;
		doorDone = false;
		doorParticles.Play();
		doorClose = false;
	}

	public void TruckScreenOpenStart()
	{
		if (GameManager.Multiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				photonView.RPC("TruckScreenOpenStartRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			TruckScreenOpenStartLogic();
		}
	}

	[PunRPC]
	private void TruckScreenOpenStartRPC()
	{
		TruckScreenOpenStartLogic();
	}

	private void TruckScreenCloseStart()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		openScreenActive = true;
		GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, ((Component)this).transform.position, 0.2f);
		((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, openScreenYPosOriginal + doorOpenPosition, ((Component)this).transform.localPosition.z);
		openScreenCurveTimer = 0f;
		doorLoopStart.Play(((Component)this).transform.position);
		doorLoopPlaying = true;
		doorDone = false;
		doorParticles.Play();
		doorClose = true;
	}

	private IEnumerator DelayedClose()
	{
		yield return (object)new WaitForSeconds(2f);
		TruckScreenCloseStart();
	}

	private IEnumerator DelayedLevelSwitch()
	{
		yield return (object)new WaitForSeconds(2f);
		RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false);
	}

	private void Update()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		doorLoop.PlayLoop(doorLoopPlaying, 2f, 2f);
		if (!openScreenActive)
		{
			return;
		}
		if (openScreenCurveTimer < 1f)
		{
			openScreenCurveTimer += Time.deltaTime;
			float num = openScreenCurveTimer;
			if (doorClose)
			{
				num = 1f - openScreenCurveTimer;
			}
			((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, openScreenYPosOriginal + openScreenCurve.Evaluate(num) * doorOpenPosition, ((Component)this).transform.localPosition.z);
			if (!(openScreenCurveTimer > 0.8f) || doorDone)
			{
				return;
			}
			GameDirector.instance.CameraImpact.ShakeDistance(6f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			doorDone = true;
			doorLoopEnd.Play(((Component)this).transform.position);
			doorSound.Play(((Component)this).transform.position);
			doorLoopPlaying = false;
			doorParticles.Play();
			if (!doorClose)
			{
				if (!GameManager.Multiplayer())
				{
					((MonoBehaviour)this).StartCoroutine(DelayedLevelSwitch());
				}
				else if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					((MonoBehaviour)this).StartCoroutine(DelayedLevelSwitch());
				}
			}
		}
		else
		{
			openScreenActive = false;
		}
	}
}
