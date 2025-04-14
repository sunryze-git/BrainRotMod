using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ToiletFun : MonoBehaviour
{
	internal Transform sphereChecker;

	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	internal List<PlayerAvatar> playerAvatars = new List<PlayerAvatar>();

	private PlayerController playerController;

	internal float checkTimer;

	private PhotonView photonView;

	private float toiletCharge;

	private float explosionTimer;

	private float hurtColliderTimer;

	private bool toiletActive;

	public GameObject hurtCollider;

	public Transform explosion;

	public Sound soundLoop;

	public Sound soundExplosion;

	public Sound soundFlush;

	public Sound soundSplash;

	private float safetyTimer;

	public ParticleSystem smallParticles;

	public ParticleSystem bigParticles;

	public ParticleSystem splashBigParticles;

	public ParticleSystem splashSmallParticles;

	private float splashTimer;

	private float randomForceTimer;

	public Rigidbody hingeRigidBody;

	private float hingeRattlingTimer;

	private void Start()
	{
		sphereChecker = ((Component)((Component)this).GetComponentInChildren<SphereCollider>()).transform;
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void FixedUpdate()
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		bool flag = false;
		Vector3 val = default(Vector3);
		foreach (PhysGrabObject physGrabObject in physGrabObjects)
		{
			if (!Object.op_Implicit((Object)(object)physGrabObject))
			{
				continue;
			}
			if (toiletActive)
			{
				Rigidbody component = ((Component)physGrabObject).GetComponent<Rigidbody>();
				if (Object.op_Implicit((Object)(object)component))
				{
					component.AddTorque(Vector3.up * toiletCharge, (ForceMode)1);
					if (randomForceTimer <= 0f)
					{
						((Vector3)(ref val))._002Ector(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
						component.AddForce(val * toiletCharge, (ForceMode)1);
					}
				}
			}
			float num = Vector3.Distance(physGrabObject.midPoint, sphereChecker.position);
			if ((physGrabObject.impactHappenedTimer > 0f || physGrabObject.impactLightTimer > 0f || physGrabObject.impactHeavyTimer > 0f || physGrabObject.impactMediumTimer > 0f) && num < sphereChecker.localScale.x)
			{
				flag = true;
			}
		}
		foreach (PlayerAvatar playerAvatar in playerAvatars)
		{
			if (Object.op_Implicit((Object)(object)playerAvatar) && toiletActive)
			{
				playerAvatar.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
				playerAvatar.tumble.TumbleOverrideTime(10f);
			}
		}
		if (Object.op_Implicit((Object)(object)playerController) && toiletActive)
		{
			playerController.playerAvatarScript.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			playerController.playerAvatarScript.tumble.TumbleOverrideTime(10f);
		}
		if (splashTimer <= 0f && flag)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("SplashRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				Splash();
			}
			splashTimer = 0.5f;
		}
		if (!toiletActive)
		{
			return;
		}
		if (hingeRattlingTimer <= 0f)
		{
			if (Object.op_Implicit((Object)(object)hingeRigidBody) && !((Component)hingeRigidBody).GetComponent<PhysGrabHinge>().broken)
			{
				hingeRigidBody.AddForce(-Vector3.up * 2f * toiletCharge * 0.5f, (ForceMode)1);
			}
			hingeRattlingTimer = 0.1f;
		}
		else
		{
			hingeRattlingTimer -= Time.deltaTime;
		}
	}

	private void Update()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		num = 1f + toiletCharge / 8f * 2f;
		soundLoop.LoopPitch = num;
		soundLoop.PlayLoop(toiletActive, 1f, 1f);
		float num2 = 1f;
		if (toiletActive)
		{
			num2 = 1f + toiletCharge * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num2 / 30f, 0f, 3f, ((Component)this).transform.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && explosionTimer <= 0f) || (SemiFunc.IsMasterClientOrSingleplayer() && explosionTimer <= 0f))
		{
			checkTimer += Time.deltaTime;
			if (checkTimer > 1f)
			{
				physGrabObjects.Clear();
				playerAvatars.Clear();
				playerController = null;
				Collider[] array = Physics.OverlapSphere(sphereChecker.position, sphereChecker.localScale.x * 0.5f, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPlayersAndPhysObjects()), (QueryTriggerInteraction)1);
				foreach (Collider val in array)
				{
					if (Object.op_Implicit((Object)(object)val))
					{
						PhysGrabObject componentInParent = ((Component)val).GetComponentInParent<PhysGrabObject>();
						if (Object.op_Implicit((Object)(object)componentInParent))
						{
							physGrabObjects.Add(componentInParent);
							break;
						}
						if (Object.op_Implicit((Object)(object)((Component)val).GetComponentInParent<PlayerAvatar>()))
						{
							playerAvatars.Add(((Component)val).GetComponentInParent<PlayerAvatar>());
							break;
						}
						if (Object.op_Implicit((Object)(object)((Component)val).GetComponentInParent<PlayerController>()))
						{
							playerController = ((Component)val).GetComponentInParent<PlayerController>();
							break;
						}
					}
				}
				checkTimer = 0f;
			}
		}
		if (splashTimer > 0f)
		{
			splashTimer -= Time.deltaTime;
		}
		if (explosionTimer > 0f)
		{
			explosionTimer -= Time.deltaTime;
			if (explosionTimer <= 0f && Object.op_Implicit((Object)(object)explosion))
			{
				((Component)explosion).gameObject.SetActive(false);
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
		if (toiletActive)
		{
			if (randomForceTimer > 0f)
			{
				randomForceTimer -= Time.deltaTime;
			}
			else
			{
				randomForceTimer = Random.Range(0.5f, 2f);
			}
			if (!smallParticles.isPlaying)
			{
				smallParticles.Play();
			}
			if (!bigParticles.isPlaying)
			{
				bigParticles.Play();
			}
			toiletCharge += Time.deltaTime * 2f;
			toiletCharge = Mathf.Clamp(toiletCharge, 0f, 8f);
			if (toiletCharge > 7.5f)
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
		}
		else
		{
			toiletCharge -= Time.deltaTime * 20f;
			toiletCharge = Mathf.Clamp(toiletCharge, 0f, 8f);
			smallParticles.Stop();
			bigParticles.Stop();
		}
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
		toiletActive = false;
		safetyTimer = 0f;
	}

	[PunRPC]
	public void ExplosionRPC()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		soundExplosion.Play(((Component)this).transform.position);
		((Component)explosion).gameObject.SetActive(true);
		hurtCollider.gameObject.SetActive(true);
		explosionTimer = 3f;
		hurtColliderTimer = 0.5f;
		EndCook();
	}

	private void FlushStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("FlushStartRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			FlushStartRPC();
		}
	}

	[PunRPC]
	public void FlushStartRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundFlush.Play(((Component)this).transform.position);
		toiletActive = true;
	}

	public void Flush()
	{
		if (!toiletActive && SemiFunc.IsMasterClientOrSingleplayer())
		{
			FlushStart();
		}
	}

	[PunRPC]
	public void SplashRPC()
	{
		Splash();
	}

	private void Splash()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		splashBigParticles.Play();
		splashSmallParticles.Play();
		soundSplash.Play(((Component)this).transform.position);
		splashTimer = 1f;
	}
}
