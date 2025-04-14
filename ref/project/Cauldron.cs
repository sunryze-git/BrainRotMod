using System;
using Photon.Pun;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
	public Transform sphereChecker;

	public ParticleSystem sparkParticles;

	public ParticleSystem windParticles;

	public Light lightGreen;

	public GameObject explosion;

	public GameObject hurtCollider;

	private float checkTimer;

	private bool cauldronActive;

	private float explosionTimer;

	private float hurtColliderTimer;

	private PhotonView photonView;

	public Transform liquid;

	private Renderer liquidRenderer;

	public Sound soundExplosion;

	public Sound soundLoop;

	private float safetyTimer;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (Object.op_Implicit((Object)(object)liquid))
		{
			liquidRenderer = ((Component)liquid).GetComponent<Renderer>();
		}
	}

	private void Update()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)liquid))
		{
			return;
		}
		float loopPitch = 1f;
		if (((Component)lightGreen).gameObject.activeSelf)
		{
			loopPitch = 1f + lightGreen.intensity / 8f * 5f;
		}
		soundLoop.LoopPitch = loopPitch;
		soundLoop.PlayLoop(cauldronActive, 1f, 1f);
		float num = 1f;
		if (cauldronActive && ((Component)lightGreen).gameObject.activeSelf)
		{
			num = 1f + lightGreen.intensity * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num / 30f, 0f, 3f, liquid.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && explosionTimer <= 0f) || (SemiFunc.IsMasterClient() && explosionTimer <= 0f))
		{
			checkTimer += Time.deltaTime;
			if (checkTimer > 1f)
			{
				bool flag = cauldronActive;
				cauldronActive = false;
				Collider[] array = Physics.OverlapSphere(sphereChecker.position, sphereChecker.localScale.x * 0.5f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPlayersAndPhysObjects()), (QueryTriggerInteraction)1);
				foreach (Collider val in array)
				{
					if (Object.op_Implicit((Object)(object)val))
					{
						if (Object.op_Implicit((Object)(object)((Component)val).GetComponentInParent<PhysGrabObject>()))
						{
							cauldronActive = true;
							break;
						}
						if (Object.op_Implicit((Object)(object)((Component)val).GetComponentInParent<PlayerAvatar>()))
						{
							cauldronActive = true;
							break;
						}
						if (Object.op_Implicit((Object)(object)((Component)val).GetComponentInParent<PlayerController>()))
						{
							cauldronActive = true;
							break;
						}
					}
				}
				if (cauldronActive && !flag)
				{
					CookStart();
				}
				if (!cauldronActive && flag)
				{
					if (SemiFunc.IsMultiplayer())
					{
						photonView.RPC("EndCookRPC", (RpcTarget)0, Array.Empty<object>());
					}
					else
					{
						EndCookRPC();
					}
				}
				checkTimer = 0f;
			}
		}
		if (explosionTimer > 0f)
		{
			explosionTimer -= Time.deltaTime;
			if (explosionTimer <= 0f && Object.op_Implicit((Object)(object)explosion))
			{
				explosion.gameObject.SetActive(false);
			}
		}
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				if (Object.op_Implicit((Object)(object)hurtCollider))
				{
					hurtCollider.SetActive(false);
				}
			}
			else if (Object.op_Implicit((Object)(object)hurtCollider))
			{
				hurtCollider.SetActive(true);
			}
		}
		if (cauldronActive)
		{
			if (!sparkParticles.isPlaying)
			{
				sparkParticles.Play();
			}
			if (!windParticles.isPlaying)
			{
				windParticles.Play();
			}
			if (!((Component)lightGreen).gameObject.activeSelf)
			{
				((Component)lightGreen).gameObject.SetActive(true);
				lightGreen.intensity = 0f;
				return;
			}
			Light obj = lightGreen;
			obj.intensity += Time.deltaTime * 2f;
			lightGreen.intensity = Mathf.Clamp(lightGreen.intensity, 0f, 8f);
			float num2 = Mathf.Abs(lightGreen.intensity / 8f);
			lightGreen.range = 4f + 1f * Mathf.Sin(Time.time * 10f) * num2;
			if (lightGreen.intensity > 7.5f)
			{
				safetyTimer += Time.deltaTime;
				if (safetyTimer > 3f)
				{
					EndCook();
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					Explosion();
				}
			}
			return;
		}
		if (((Component)lightGreen).gameObject.activeSelf)
		{
			Light obj2 = lightGreen;
			obj2.intensity -= Time.deltaTime * 20f;
			lightGreen.intensity = Mathf.Clamp(lightGreen.intensity, 0f, 8f);
			if (lightGreen.intensity < 0.1f)
			{
				((Component)lightGreen).gameObject.SetActive(false);
			}
		}
		sparkParticles.Stop();
		windParticles.Stop();
	}

	private void Explosion()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("ExplosionRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			ExplosionRPC();
		}
	}

	[PunRPC]
	public void EndCookRPC()
	{
		EndCook();
	}

	private void EndCook()
	{
		cauldronActive = false;
		safetyTimer = 0f;
	}

	[PunRPC]
	public void ExplosionRPC()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, liquid.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, liquid.position, 0.1f);
		soundExplosion.Play(liquid.position);
		explosion.gameObject.SetActive(true);
		hurtCollider.gameObject.SetActive(true);
		explosionTimer = 3f;
		hurtColliderTimer = 0.5f;
		EndCook();
	}

	private void CookStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("CookStartRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			CookStartRPC();
		}
	}

	[PunRPC]
	public void CookStartRPC()
	{
		cauldronActive = true;
	}
}
