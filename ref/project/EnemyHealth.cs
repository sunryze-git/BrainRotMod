using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
	private PhotonView photonView;

	private Enemy enemy;

	public int health = 100;

	internal int healthCurrent;

	private bool deadImpulse;

	internal bool dead;

	private float deadImpulseTimer;

	public float deathFreezeTime = 0.1f;

	public bool impactHurt;

	public int impactLightDamage;

	public int impactMediumDamage;

	public int impactHeavyDamage;

	public bool objectHurt;

	public float objectHurtMultiplier = 1f;

	public bool objectHurtStun = true;

	internal float objectHurtStunTime = 2f;

	public Transform meshParent;

	private List<MeshRenderer> renderers;

	private List<Material> sharedMaterials = new List<Material>();

	internal List<Material> instancedMaterials = new List<Material>();

	public bool spawnValuable = true;

	public int spawnValuableMax = 3;

	internal int spawnValuableCurrent;

	internal Vector3 hurtDirection;

	private bool hurtEffect;

	private AnimationCurve hurtCurve;

	private float hurtLerp;

	public UnityEvent onHurt;

	private bool onHurtImpulse;

	public UnityEvent onDeathStart;

	public UnityEvent onDeath;

	public UnityEvent onObjectHurt;

	internal PlayerAvatar onObjectHurtPlayer;

	private int materialHurtColor;

	private int materialHurtAmount;

	internal float objectHurtDisableTimer;

	private void Awake()
	{
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		enemy = ((Component)this).GetComponent<Enemy>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		healthCurrent = health;
		hurtCurve = AssetManager.instance.animationCurveImpact;
		renderers = new List<MeshRenderer>();
		if (Object.op_Implicit((Object)(object)meshParent))
		{
			renderers.AddRange(((Component)meshParent).GetComponentsInChildren<MeshRenderer>(true));
		}
		foreach (MeshRenderer renderer in renderers)
		{
			Material val = null;
			foreach (Material sharedMaterial in sharedMaterials)
			{
				if (((Object)((Renderer)renderer).sharedMaterial).name == ((Object)sharedMaterial).name)
				{
					val = sharedMaterial;
					((Renderer)renderer).sharedMaterial = instancedMaterials[sharedMaterials.IndexOf(sharedMaterial)];
				}
			}
			if (!Object.op_Implicit((Object)(object)val))
			{
				val = ((Renderer)renderer).sharedMaterial;
				sharedMaterials.Add(val);
				instancedMaterials.Add(((Renderer)renderer).material);
			}
		}
		materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		foreach (Material instancedMaterial in instancedMaterials)
		{
			instancedMaterial.SetColor(materialHurtColor, Color.red);
		}
	}

	private void Update()
	{
		if (hurtEffect)
		{
			hurtLerp += 2.5f * Time.deltaTime;
			hurtLerp = Mathf.Clamp01(hurtLerp);
			foreach (Material instancedMaterial in instancedMaterials)
			{
				instancedMaterial.SetFloat(materialHurtAmount, hurtCurve.Evaluate(hurtLerp));
			}
			if (hurtLerp > 1f)
			{
				hurtEffect = false;
				foreach (Material instancedMaterial2 in instancedMaterials)
				{
					instancedMaterial2.SetFloat(materialHurtAmount, 0f);
				}
			}
		}
		if ((!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient) && deadImpulse)
		{
			deadImpulseTimer -= Time.deltaTime;
			if (deadImpulseTimer <= 0f)
			{
				if (!GameManager.Multiplayer())
				{
					DeathImpulseRPC();
				}
				else
				{
					photonView.RPC("DeathImpulseRPC", (RpcTarget)0, Array.Empty<object>());
				}
			}
		}
		if (objectHurtDisableTimer > 0f)
		{
			objectHurtDisableTimer -= Time.deltaTime;
		}
		if (onHurtImpulse)
		{
			onHurt.Invoke();
			onHurtImpulse = false;
		}
	}

	public void OnSpawn()
	{
		if (hurtEffect)
		{
			hurtLerp = 1f;
			hurtEffect = false;
			foreach (Material instancedMaterial in instancedMaterials)
			{
				instancedMaterial.SetFloat(materialHurtAmount, 0f);
			}
		}
		healthCurrent = health;
		dead = false;
	}

	public void LightImpact()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (impactHurt && enemy.IsStunned() && impactLightDamage > 0)
		{
			Hurt(impactLightDamage, -((Vector3)(ref enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw)).normalized);
		}
	}

	public void MediumImpact()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (impactHurt && enemy.IsStunned() && impactMediumDamage > 0)
		{
			Hurt(impactMediumDamage, -((Vector3)(ref enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw)).normalized);
		}
	}

	public void HeavyImpact()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (impactHurt && enemy.IsStunned() && impactHeavyDamage > 0)
		{
			Hurt(impactHeavyDamage, -((Vector3)(ref enemy.Rigidbody.impactDetector.previousPreviousVelocityRaw)).normalized);
		}
	}

	public void Hurt(int _damage, Vector3 _hurtDirection)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!dead)
		{
			healthCurrent -= _damage;
			if (healthCurrent <= 0)
			{
				healthCurrent = 0;
				Death(_hurtDirection);
			}
			else if (!GameManager.Multiplayer())
			{
				HurtRPC(_damage, _hurtDirection);
			}
			else
			{
				photonView.RPC("HurtRPC", (RpcTarget)0, new object[2] { _damage, _hurtDirection });
			}
		}
	}

	[PunRPC]
	public void HurtRPC(int _damage, Vector3 _hurtDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		hurtDirection = _hurtDirection;
		hurtEffect = true;
		hurtLerp = 0f;
		if (hurtDirection == Vector3.zero)
		{
			hurtDirection = Random.insideUnitSphere;
		}
		onHurtImpulse = true;
	}

	private void Death(Vector3 _deathDirection)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer())
		{
			DeathRPC(_deathDirection);
			return;
		}
		photonView.RPC("DeathRPC", (RpcTarget)0, new object[1] { _deathDirection });
	}

	[PunRPC]
	public void DeathRPC(Vector3 _deathDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		hurtDirection = _deathDirection;
		hurtEffect = true;
		hurtLerp = 0f;
		deadImpulseTimer = deathFreezeTime;
		enemy.Freeze(deathFreezeTime);
		onDeathStart.Invoke();
		deadImpulse = true;
	}

	[PunRPC]
	public void DeathImpulseRPC()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		deadImpulse = false;
		dead = true;
		if (hurtDirection == Vector3.zero)
		{
			hurtDirection = Random.insideUnitSphere;
		}
		onDeath.Invoke();
	}

	public void ObjectHurtDisable(float _time)
	{
		objectHurtDisableTimer = _time;
	}
}
