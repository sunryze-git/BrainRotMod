using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemRubberDuck : MonoBehaviour
{
	public Sound soundQuack;

	public Sound soundSqueak;

	public Sound soundDuckLoop;

	public Sound soundDuckExplosion;

	public Sound soundDuckExplosionGlobal;

	private Rigidbody rb;

	private PhotonView photonView;

	private ParticleScriptExplosion particleScriptExplosion;

	private PhysGrabObject physGrabObject;

	public HurtCollider hurtCollider;

	public Transform hurtTransform;

	private float hurtColliderTime;

	private Vector3 prevPosition;

	private bool playDuckLoop;

	private List<TrailRenderer> trails = new List<TrailRenderer>();

	private ItemBattery itemBattery;

	private float trailTimer;

	public GameObject brokenObject;

	public GameObject notBrokenObject;

	private ItemEquippable itemEquippable;

	private float lilQuacksTimer;

	private void Start()
	{
		rb = ((Component)this).GetComponent<Rigidbody>();
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>();
		((Component)hurtCollider).gameObject.SetActive(false);
		photonView = ((Component)this).GetComponent<PhotonView>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		TrailRenderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<TrailRenderer>();
		foreach (TrailRenderer item in componentsInChildren)
		{
			trails.Add(item);
		}
	}

	private void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMasterClientOrSingleplayer() || (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsArena()))
		{
			return;
		}
		Vector3 velocity = rb.velocity;
		if (((Vector3)(ref velocity)).magnitude < 0.1f)
		{
			if (lilQuacksTimer > 0f)
			{
				lilQuacksTimer -= Time.deltaTime;
				return;
			}
			lilQuacksTimer = Random.Range(1f, 3f);
			LilQuackJump();
		}
	}

	private void FixedUpdate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		if (itemEquippable.isEquipped || itemEquippable.wasEquippedTimer > 0f)
		{
			prevPosition = rb.position;
			return;
		}
		if (itemBattery.batteryLifeInt == 0)
		{
			if (!brokenObject.activeSelf)
			{
				brokenObject.SetActive(true);
				notBrokenObject.SetActive(false);
			}
		}
		else if (brokenObject.activeSelf)
		{
			brokenObject.SetActive(false);
			notBrokenObject.SetActive(true);
		}
		Vector3 val = (rb.position - prevPosition) / Time.fixedDeltaTime;
		Vector3 val2 = rb.position - prevPosition;
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		prevPosition = rb.position;
		if (!physGrabObject.grabbed && itemBattery.batteryLife > 0f)
		{
			if (((Vector3)(ref val)).magnitude > 5f)
			{
				playDuckLoop = true;
				trailTimer = 0.2f;
			}
			else
			{
				playDuckLoop = false;
			}
		}
		else
		{
			playDuckLoop = false;
		}
		if (trailTimer > 0f)
		{
			playDuckLoop = true;
			trailTimer -= Time.fixedDeltaTime;
			foreach (TrailRenderer trail in trails)
			{
				trail.emitting = true;
			}
		}
		else
		{
			playDuckLoop = false;
			foreach (TrailRenderer trail2 in trails)
			{
				trail2.emitting = false;
			}
		}
		soundDuckLoop.PlayLoop(playDuckLoop, 2f, 1f);
		if (hurtColliderTime > 0f)
		{
			hurtTransform.forward = normalized;
			if (!((Component)hurtCollider).gameObject.activeSelf)
			{
				((Component)hurtCollider).gameObject.SetActive(true);
				float num = ((Vector3)(ref val)).magnitude * 2f;
				if (num > 50f)
				{
					num = 50f;
				}
				hurtCollider.physHitForce = num;
				hurtCollider.physHitTorque = num;
				hurtCollider.enemyHitForce = num;
				hurtCollider.enemyHitTorque = num;
				hurtCollider.playerTumbleForce = num;
				hurtCollider.playerTumbleTorque = num;
			}
			hurtColliderTime -= Time.fixedDeltaTime;
		}
		else if (((Component)hurtCollider).gameObject.activeSelf)
		{
			((Component)hurtCollider).gameObject.SetActive(false);
		}
	}

	private void LilQuackJump()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (!(itemBattery.batteryLife <= 0f) && !physGrabObject.grabbed)
		{
			rb.AddForce(Vector3.up * 0.5f, (ForceMode)1);
			rb.AddTorque(Random.insideUnitSphere * 2f, (ForceMode)1);
			rb.AddForce(Vector2.op_Implicit(Random.insideUnitCircle * 0.2f), (ForceMode)1);
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("LilQuackJumpRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				LilQuackJumpRPC();
			}
		}
	}

	[PunRPC]
	public void LilQuackJumpRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundQuack.Play(((Component)this).transform.position);
	}

	public void Squeak()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!(itemBattery.batteryLife <= 0f))
		{
			soundSqueak.Play(((Component)this).transform.position);
		}
	}

	public void Quack()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (itemBattery.batteryLife <= 0f || physGrabObject.grabbed)
		{
			return;
		}
		soundQuack.Play(((Component)this).transform.position);
		hurtColliderTime = 0.2f;
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		itemBattery.batteryLife -= 2.5f;
		if (Random.Range(0, 10) == 0)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("QuackRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				QuackRPC();
			}
		}
		Vector3 velocity = rb.velocity;
		if (((Vector3)(ref velocity)).magnitude < 20f)
		{
			rb.velocity *= 5f;
			rb.AddTorque(Random.insideUnitSphere * 40f);
		}
	}

	[PunRPC]
	public void QuackRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		soundDuckExplosionGlobal.Play(((Component)this).transform.position);
		soundDuckExplosion.Play(((Component)this).transform.position);
		ParticlePrefabExplosion particlePrefabExplosion = particleScriptExplosion.Spawn(((Component)this).transform.position, 0.85f, 0, 250, 1f, onlyParticleEffect: false, disableSound: true);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}
}
