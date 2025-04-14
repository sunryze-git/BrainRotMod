using System;
using Photon.Pun;
using UnityEngine;

public class FlamethrowerValuable : MonoBehaviour
{
	public enum States
	{
		Full,
		Empty
	}

	public SemiZuperFlames semiFlames;

	public Transform triggerMesh;

	public Transform Center;

	private Vector3 triggerMeshInitialEulerAngles;

	private bool triggerStuck;

	private float triggerStuckTimer = 0.2f;

	public Sound soundFlameEmpty;

	public float fuelTimer;

	private bool fuelCountdownActive;

	private PhotonView photonView;

	private ParticleScriptExplosion particleScriptExplosion;

	public ParticleSystem flameEndSquirt;

	public ParticleSystem flameEndSparks;

	internal States currentState;

	private void Start()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		photonView = ((Component)this).GetComponent<PhotonView>();
		triggerMeshInitialEulerAngles = triggerMesh.localEulerAngles;
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
	}

	private void Update()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (fuelCountdownActive)
		{
			fuelTimer -= Time.deltaTime;
			if (fuelTimer <= 0f)
			{
				fuelTimer = 0f;
				ReleaseTrigger();
				fuelCountdownActive = false;
				SetState(States.Empty);
			}
		}
		if (triggerStuck)
		{
			triggerStuckTimer -= Time.deltaTime;
			if (triggerStuckTimer <= 0f)
			{
				triggerStuckTimer = 0f;
				ReleaseTrigger();
				triggerStuck = false;
			}
		}
	}

	private void GrabTriggerLogic()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SetTriggerMeshPosition(pulled: true);
		if (currentState == States.Empty)
		{
			soundFlameEmpty.Play(((Component)semiFlames).transform.position);
			flameEndSquirt.Play();
			flameEndSparks.Play();
		}
		else
		{
			EnemyDirector.instance.SetInvestigate(((Component)this).transform.position, 5f);
			semiFlames.FlamesActive(((Component)semiFlames).transform.position, ((Component)semiFlames).transform.rotation);
			fuelCountdownActive = true;
		}
	}

	public void GrabTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			GrabTriggerLogic();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("GrabTriggerRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void GrabTriggerRPC()
	{
		GrabTriggerLogic();
	}

	private void ReleaseTriggerLogic()
	{
		SetTriggerMeshPosition(pulled: false);
		if (currentState == States.Empty)
		{
			flameEndSparks.Stop();
			return;
		}
		semiFlames.FlamesInactive();
		fuelCountdownActive = false;
	}

	public void ReleaseTrigger()
	{
		if (GameManager.instance.gameMode == 0)
		{
			ReleaseTriggerLogic();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("ReleaseTriggerRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void ReleaseTriggerRPC()
	{
		ReleaseTriggerLogic();
	}

	[PunRPC]
	public void SetStateRPC(States state)
	{
		currentState = state;
	}

	private void SetState(States state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.IsMultiplayer())
			{
				SetStateRPC(state);
				return;
			}
			photonView.RPC("SetStateRPC", (RpcTarget)0, new object[1] { state });
		}
	}

	public void SetTriggerMeshPosition(bool pulled)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!triggerStuck)
		{
			if (pulled)
			{
				Vector3 localEulerAngles = default(Vector3);
				((Vector3)(ref localEulerAngles))._002Ector(triggerMeshInitialEulerAngles.x, triggerMeshInitialEulerAngles.y, triggerMeshInitialEulerAngles.z - 40f);
				triggerMesh.localEulerAngles = localEulerAngles;
			}
			else
			{
				triggerMesh.localEulerAngles = triggerMeshInitialEulerAngles;
			}
		}
	}

	public void TriggerStuck()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			triggerStuckTimer = 0.2f;
			triggerStuck = true;
			GrabTrigger();
		}
	}

	public void Explode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		particleScriptExplosion.Spawn(Center.position, 0.2f, 10, 20);
	}
}
