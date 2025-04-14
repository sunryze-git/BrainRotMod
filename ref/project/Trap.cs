using System;
using Photon.Pun;
using UnityEngine;

public class Trap : MonoBehaviour
{
	public enum TrapActivateRarityLevel
	{
		no_rarity,
		level1,
		level2,
		level3
	}

	protected PhotonView photonView;

	[HideInInspector]
	public bool enemyInvestigate;

	private bool enemyInvestigatePrev;

	protected float enemyInvestigateRange = 35f;

	private float enemyInvestigateTimer = 1f;

	private float enemyInvestigateTimerMax = 1f;

	[HideInInspector]
	public bool isLocal;

	[HideInInspector]
	public bool trapTriggered;

	[HideInInspector]
	public bool trapActive;

	[HideInInspector]
	public bool trapStart;

	private float trapActivateTimer = 10f;

	protected PhysGrabObject physGrabObject;

	public bool triggerOnTimer;

	public TrapActivateRarityLevel trapActivateRarityLevel;

	protected virtual void Start()
	{
		enemyInvestigateTimer = enemyInvestigateTimerMax;
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			isLocal = true;
		}
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	protected virtual void Update()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!isLocal)
		{
			return;
		}
		if (enemyInvestigate)
		{
			if (!enemyInvestigatePrev)
			{
				enemyInvestigateTimer = enemyInvestigateTimerMax;
			}
			enemyInvestigateTimer += Time.deltaTime;
			if (enemyInvestigateTimer > enemyInvestigateTimerMax)
			{
				EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, enemyInvestigateRange);
				enemyInvestigateTimer = 0f;
			}
		}
		enemyInvestigatePrev = enemyInvestigate;
		enemyInvestigate = false;
		if (!triggerOnTimer)
		{
			return;
		}
		if (physGrabObject.grabbed)
		{
			if (Application.isEditor && (!GameManager.Multiplayer() || GameManager.instance.localTest) && Input.GetKeyDown((KeyCode)98))
			{
				TrapActivateSync();
			}
			if (!(trapActivateTimer > 0f))
			{
				return;
			}
			trapActivateTimer -= Time.deltaTime;
			if (trapActivateTimer <= 0f)
			{
				trapActivateTimer = Random.Range(5f, 15f);
				if (SemiFunc.ValuableTrapActivatedDiceRoll((int)trapActivateRarityLevel))
				{
					TrapActivateSync();
				}
			}
		}
		else
		{
			trapActivateTimer = Random.Range(0f, 15f);
		}
	}

	private void TrapActivateSync()
	{
		if (trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				photonView.RPC("TrapActivateSyncRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			TrapActivateSyncRPC();
		}
	}

	[PunRPC]
	public void TrapActivateSyncRPC()
	{
		if (physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		trapStart = true;
	}

	public void TrapStart()
	{
		if (trapTriggered)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && SemiFunc.ValuableTrapActivatedDiceRoll((int)trapActivateRarityLevel))
			{
				photonView.RPC("TrapStartRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else if (SemiFunc.ValuableTrapActivatedDiceRoll((int)trapActivateRarityLevel))
		{
			TrapStartRPC();
		}
	}

	[PunRPC]
	public void TrapStartRPC()
	{
		if (physGrabObject.grabbedLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		trapStart = true;
	}
}
