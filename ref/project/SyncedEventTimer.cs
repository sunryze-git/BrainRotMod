using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class SyncedEventTimer : MonoBehaviour
{
	private PhotonView photonView;

	private float timer;

	public float timerMin = 4f;

	public float timerMax = 5f;

	public UnityEvent onTimerStart;

	public UnityEvent onTimerEnd;

	public UnityEvent onTimerTick;

	private bool singlePlayer;

	private bool isMaster;

	private bool timerActive;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			singlePlayer = true;
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			isMaster = true;
		}
	}

	public void StartTimer()
	{
		if (singlePlayer || isMaster)
		{
			timer = Random.Range(timerMin, timerMax);
			onTimerStart.Invoke();
			((MonoBehaviour)this).StartCoroutine(Timer());
			timerActive = true;
			if (isMaster)
			{
				photonView.RPC("StartTimerRPC", (RpcTarget)1, Array.Empty<object>());
			}
		}
	}

	[PunRPC]
	private void StartTimerRPC()
	{
		timerActive = true;
		onTimerStart.Invoke();
	}

	private IEnumerator Timer()
	{
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		EndTimer();
		if (isMaster)
		{
			photonView.RPC("EndTimerRPC", (RpcTarget)1, Array.Empty<object>());
		}
	}

	private void Update()
	{
		if (timerActive)
		{
			onTimerTick.Invoke();
		}
	}

	[PunRPC]
	private void EndTimerRPC()
	{
		timerActive = false;
		onTimerEnd.Invoke();
	}

	public void EndTimer()
	{
		timerActive = false;
		onTimerEnd.Invoke();
	}
}
