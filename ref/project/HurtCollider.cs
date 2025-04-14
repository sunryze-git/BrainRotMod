using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class HurtCollider : MonoBehaviour
{
	public enum BreakImpact
	{
		None,
		Light,
		Medium,
		Heavy
	}

	public enum TorqueAxis
	{
		up,
		down,
		left,
		right,
		forward,
		back
	}

	public enum HitType
	{
		Player,
		PhysObject,
		Enemy
	}

	public class Hit
	{
		public HitType hitType;

		public GameObject hitObject;

		public float cooldown;
	}

	public bool playerLogic = true;

	[Space]
	public bool playerKill = true;

	public int playerDamage = 10;

	public float playerDamageCooldown = 0.25f;

	public float playerHitForce;

	public bool playerRayCast;

	public float playerTumbleForce;

	public float playerTumbleTorque;

	public TorqueAxis playerTumbleTorqueAxis = TorqueAxis.down;

	public float playerTumbleTime;

	public float playerTumbleImpactHurtTime;

	public int playerTumbleImpactHurtDamage;

	public bool physLogic = true;

	[Space]
	public bool physDestroy = true;

	public bool physHingeDestroy = true;

	public bool physHingeBreak;

	public BreakImpact physImpact = BreakImpact.Medium;

	public float physDamageCooldown = 0.25f;

	public float physHitForce;

	public float physHitTorque;

	public bool physRayCast;

	public bool enemyLogic = true;

	public Enemy enemyHost;

	[Space]
	[FormerlySerializedAs("enemyDespawn")]
	public bool enemyKill = true;

	public bool enemyStun = true;

	public float enemyStunTime = 2f;

	public EnemyType enemyStunType = EnemyType.Medium;

	public float enemyFreezeTime = 0.1f;

	[Space]
	public BreakImpact enemyImpact = BreakImpact.Medium;

	public int enemyDamage;

	public float enemyDamageCooldown = 0.25f;

	public float enemyHitForce;

	public float enemyHitTorque;

	public bool enemyRayCast;

	public bool enemyHitTriggers = true;

	[Range(0f, 180f)]
	public float hitSpread = 180f;

	public List<PhysGrabObject> ignoreObjects = new List<PhysGrabObject>();

	public UnityEvent onImpactAny;

	public UnityEvent onImpactPlayer;

	internal PlayerAvatar onImpactPlayerAvatar;

	public UnityEvent onImpactPhysObject;

	public UnityEvent onImpactEnemy;

	internal Enemy onImpactEnemyEnemy;

	private Collider Collider;

	private BoxCollider BoxCollider;

	private SphereCollider SphereCollider;

	private bool ColliderIsBox = true;

	private LayerMask LayerMask;

	private LayerMask RayMask;

	internal List<Hit> hits = new List<Hit>();

	private bool colliderCheckRunning;

	private bool cooldownLogicRunning;

	private Vector3 applyForce;

	private Vector3 applyTorque;

	private void Awake()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider = ((Component)this).GetComponent<BoxCollider>();
		if (!Object.op_Implicit((Object)(object)BoxCollider))
		{
			SphereCollider = ((Component)this).GetComponent<SphereCollider>();
			Collider = (Collider)(object)SphereCollider;
			ColliderIsBox = false;
		}
		else
		{
			Collider = (Collider)(object)BoxCollider;
		}
		Collider.isTrigger = true;
		LayerMask = LayerMask.op_Implicit(LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject()) + LayerMask.GetMask(new string[1] { "Player" }) + LayerMask.GetMask(new string[1] { "Default" }) + LayerMask.GetMask(new string[1] { "Enemy" }));
		RayMask = LayerMask.op_Implicit(LayerMask.GetMask(new string[2] { "Default", "PhysGrabObjectHinge" }));
	}

	private void OnEnable()
	{
		if (!colliderCheckRunning)
		{
			colliderCheckRunning = true;
			((MonoBehaviour)this).StartCoroutine(ColliderCheck());
		}
	}

	private void OnDisable()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		colliderCheckRunning = false;
		cooldownLogicRunning = false;
		hits.Clear();
	}

	private IEnumerator CooldownLogic()
	{
		while (hits.Count > 0)
		{
			for (int i = 0; i < hits.Count; i++)
			{
				Hit hit = hits[i];
				hit.cooldown -= Time.deltaTime;
				if (hit.cooldown <= 0f)
				{
					hits.RemoveAt(i);
					i--;
				}
			}
			yield return null;
		}
		cooldownLogicRunning = false;
	}

	private bool CanHit(GameObject hitObject, float cooldown, bool raycast, Vector3 hitPosition, HitType hitType)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		foreach (Hit hit2 in hits)
		{
			if ((Object)(object)hit2.hitObject == (Object)(object)hitObject)
			{
				return false;
			}
		}
		Hit hit = new Hit();
		hit.hitObject = hitObject;
		hit.cooldown = cooldown;
		hit.hitType = hitType;
		hits.Add(hit);
		if (!cooldownLogicRunning)
		{
			((MonoBehaviour)this).StartCoroutine(CooldownLogic());
			cooldownLogicRunning = true;
		}
		if (raycast)
		{
			Bounds bounds = Collider.bounds;
			Vector3 val = hitPosition - ((Bounds)(ref bounds)).center;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			bounds = Collider.bounds;
			float num = Vector3.Distance(hitPosition, ((Bounds)(ref bounds)).center);
			bounds = Collider.bounds;
			RaycastHit[] array = Physics.RaycastAll(((Bounds)(ref bounds)).center, normalized, num, LayerMask.op_Implicit(RayMask), (QueryTriggerInteraction)2);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val2 = array[i];
				if (((Component)((RaycastHit)(ref val2)).collider).gameObject.CompareTag("Wall"))
				{
					PhysGrabObject componentInParent = hitObject.GetComponentInParent<PhysGrabObject>();
					PhysGrabObject componentInParent2 = ((Component)((RaycastHit)(ref val2)).collider).gameObject.GetComponentInParent<PhysGrabObject>();
					if (!Object.op_Implicit((Object)(object)componentInParent) || (Object)(object)componentInParent != (Object)(object)componentInParent2)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private IEnumerator ColliderCheck()
	{
		yield return null;
		while (!Object.op_Implicit((Object)(object)LevelGenerator.Instance) || !LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		while (true)
		{
			Bounds bounds = Collider.bounds;
			Vector3 center = ((Bounds)(ref bounds)).center;
			Collider[] array;
			if (ColliderIsBox)
			{
				Vector3 val = BoxCollider.size * 0.5f;
				val.x *= Mathf.Abs(((Component)this).transform.lossyScale.x);
				val.y *= Mathf.Abs(((Component)this).transform.lossyScale.y);
				val.z *= Mathf.Abs(((Component)this).transform.lossyScale.z);
				array = Physics.OverlapBox(center, val, ((Component)this).transform.rotation, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)2);
			}
			else
			{
				float num = ((Component)this).transform.lossyScale.x * SphereCollider.radius;
				array = Physics.OverlapSphere(center, num, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)2);
			}
			if (array.Length != 0)
			{
				Collider[] array2 = array;
				foreach (Collider val2 in array2)
				{
					if (playerLogic && playerDamageCooldown > 0f && ((Component)val2).gameObject.CompareTag("Player"))
					{
						PlayerAvatar playerAvatar = ((Component)val2).gameObject.GetComponentInParent<PlayerAvatar>();
						if (!Object.op_Implicit((Object)(object)playerAvatar))
						{
							PlayerController componentInParent = ((Component)val2).gameObject.GetComponentInParent<PlayerController>();
							if (Object.op_Implicit((Object)(object)componentInParent))
							{
								playerAvatar = componentInParent.playerAvatarScript;
							}
						}
						if (Object.op_Implicit((Object)(object)playerAvatar))
						{
							PlayerHurt(playerAvatar);
						}
					}
					if (!(enemyDamageCooldown > 0f) && !(physDamageCooldown > 0f) && !(playerDamageCooldown > 0f))
					{
						continue;
					}
					if (((Component)val2).gameObject.CompareTag("Phys Grab Object"))
					{
						PhysGrabObject componentInParent2 = ((Component)val2).gameObject.GetComponentInParent<PhysGrabObject>();
						if (ignoreObjects.Contains(componentInParent2) || !Object.op_Implicit((Object)(object)componentInParent2))
						{
							continue;
						}
						bool flag = false;
						PlayerTumble componentInParent3 = ((Component)val2).gameObject.GetComponentInParent<PlayerTumble>();
						if (Object.op_Implicit((Object)(object)componentInParent3))
						{
							flag = true;
						}
						if (playerLogic && playerDamageCooldown > 0f && flag)
						{
							PlayerHurt(componentInParent3.playerAvatar);
						}
						if (!SemiFunc.IsMasterClientOrSingleplayer())
						{
							continue;
						}
						EnemyRigidbody enemyRigidbody = null;
						if (enemyLogic && !flag)
						{
							enemyRigidbody = ((Component)val2).gameObject.GetComponentInParent<EnemyRigidbody>();
							EnemyHurtRigidbody(enemyRigidbody, componentInParent2);
						}
						if (!physLogic || Object.op_Implicit((Object)(object)enemyRigidbody) || flag || !(physDamageCooldown > 0f) || !CanHit(((Component)componentInParent2).gameObject, physDamageCooldown, physRayCast, componentInParent2.centerPoint, HitType.PhysObject))
						{
							continue;
						}
						bool flag2 = false;
						PhysGrabObjectImpactDetector componentInParent4 = ((Component)val2).gameObject.GetComponentInParent<PhysGrabObjectImpactDetector>();
						if (Object.op_Implicit((Object)(object)componentInParent4))
						{
							if (physHingeDestroy)
							{
								PhysGrabHinge component = ((Component)componentInParent2).GetComponent<PhysGrabHinge>();
								if (Object.op_Implicit((Object)(object)component))
								{
									component.DestroyHinge();
									flag2 = true;
								}
							}
							else if (physHingeBreak)
							{
								PhysGrabHinge component2 = ((Component)componentInParent2).GetComponent<PhysGrabHinge>();
								if (Object.op_Implicit((Object)(object)component2) && Object.op_Implicit((Object)(object)component2.joint))
								{
									((Joint)component2.joint).breakForce = 0f;
									((Joint)component2.joint).breakTorque = 0f;
									flag2 = true;
								}
							}
							if (!flag2)
							{
								if (physDestroy)
								{
									if (!componentInParent4.destroyDisable)
									{
										PhysGrabHinge component3 = ((Component)componentInParent2).GetComponent<PhysGrabHinge>();
										if (Object.op_Implicit((Object)(object)component3))
										{
											component3.DestroyHinge();
										}
										else
										{
											componentInParent4.DestroyObject();
										}
									}
									else
									{
										PhysObjectHurt(componentInParent2, BreakImpact.Heavy, 50f, 30f, apply: true, destroyLaunch: true);
									}
									flag2 = true;
								}
								else if (Object.op_Implicit((Object)(object)componentInParent2) && PhysObjectHurt(componentInParent2, physImpact, physHitForce, physHitTorque, apply: true, destroyLaunch: false))
								{
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							onImpactAny.Invoke();
							onImpactPhysObject.Invoke();
						}
					}
					else
					{
						if (!SemiFunc.IsMasterClientOrSingleplayer() || !enemyLogic)
						{
							continue;
						}
						Enemy componentInParent5 = ((Component)val2).gameObject.GetComponentInParent<Enemy>();
						if (Object.op_Implicit((Object)(object)componentInParent5) && !componentInParent5.HasRigidbody && CanHit(((Component)componentInParent5).gameObject, enemyDamageCooldown, enemyRayCast, ((Component)componentInParent5).transform.position, HitType.Enemy) && EnemyHurt(componentInParent5))
						{
							onImpactAny.Invoke();
							onImpactEnemyEnemy = componentInParent5;
							onImpactEnemy.Invoke();
						}
						if (!enemyHitTriggers)
						{
							continue;
						}
						EnemyParent componentInParent6 = ((Component)val2).gameObject.GetComponentInParent<EnemyParent>();
						if (Object.op_Implicit((Object)(object)componentInParent6))
						{
							EnemyRigidbody componentInChildren = ((Component)componentInParent6).GetComponentInChildren<EnemyRigidbody>();
							if (Object.op_Implicit((Object)(object)componentInChildren))
							{
								EnemyHurtRigidbody(componentInChildren, componentInChildren.physGrabObject);
							}
						}
					}
				}
			}
			yield return (object)new WaitForSeconds(0.05f);
		}
	}

	private void EnemyHurtRigidbody(EnemyRigidbody _enemyRigidbody, PhysGrabObject _physGrabObject)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (enemyDamageCooldown > 0f && Object.op_Implicit((Object)(object)_enemyRigidbody) && CanHit(((Component)_physGrabObject).gameObject, enemyDamageCooldown, enemyRayCast, _physGrabObject.centerPoint, HitType.Enemy) && EnemyHurt(_enemyRigidbody.enemy))
		{
			onImpactAny.Invoke();
			onImpactEnemyEnemy = _enemyRigidbody.enemy;
			onImpactEnemy.Invoke();
		}
	}

	private bool EnemyHurt(Enemy _enemy)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_enemy == (Object)(object)enemyHost)
		{
			return false;
		}
		if (!enemyLogic)
		{
			return false;
		}
		bool flag = false;
		if (enemyKill)
		{
			if (_enemy.HasHealth)
			{
				_enemy.Health.Hurt(_enemy.Health.healthCurrent, ((Component)this).transform.forward);
			}
			else if (_enemy.HasStateDespawn)
			{
				_enemy.EnemyParent.SpawnedTimerSet(0f);
				_enemy.CurrentState = EnemyState.Despawn;
				flag = true;
			}
		}
		if (!flag)
		{
			if (enemyStun && _enemy.HasStateStunned && _enemy.Type <= enemyStunType)
			{
				_enemy.StateStunned.Set(enemyStunTime);
			}
			if (enemyFreezeTime > 0f)
			{
				_enemy.Freeze(enemyFreezeTime);
			}
			if (_enemy.HasRigidbody)
			{
				PhysObjectHurt(_enemy.Rigidbody.physGrabObject, enemyImpact, enemyHitForce, enemyHitTorque, apply: true, destroyLaunch: false);
				if (enemyFreezeTime > 0f)
				{
					_enemy.Rigidbody.FreezeForces(applyForce, applyTorque);
				}
			}
			if (enemyDamage > 0 && _enemy.HasHealth)
			{
				_enemy.Health.Hurt(enemyDamage, ((Vector3)(ref applyForce)).normalized);
			}
		}
		return true;
	}

	private void PlayerHurt(PlayerAvatar _player)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Multiplayer() && !_player.photonView.IsMine)
		{
			return;
		}
		int enemyIndex = SemiFunc.EnemyGetIndex(enemyHost);
		if (playerKill)
		{
			onImpactAny.Invoke();
			onImpactPlayer.Invoke();
			_player.playerHealth.Hurt(_player.playerHealth.health, savingGrace: true, enemyIndex);
		}
		else
		{
			if (!CanHit(((Component)_player).gameObject, playerDamageCooldown, playerRayCast, _player.PlayerVisionTarget.VisionTransform.position, HitType.Player))
			{
				return;
			}
			_player.playerHealth.Hurt(playerDamage, savingGrace: true, enemyIndex);
			bool flag = false;
			Bounds bounds = Collider.bounds;
			Vector3 center = ((Bounds)(ref bounds)).center;
			Vector3 val = _player.PlayerVisionTarget.VisionTransform.position - center;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			normalized = SemiFunc.ClampDirection(normalized, ((Component)this).transform.forward, hitSpread);
			bool flag2 = _player.tumble.isTumbling;
			if (playerTumbleTime > 0f && _player.playerHealth.health > 0)
			{
				_player.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
				_player.tumble.TumbleOverrideTime(playerTumbleTime);
				if (playerTumbleImpactHurtTime > 0f)
				{
					_player.tumble.ImpactHurtSet(playerTumbleImpactHurtTime, playerTumbleImpactHurtDamage);
				}
				flag2 = true;
				flag = true;
			}
			if (flag2 && (playerTumbleForce > 0f || playerTumbleTorque > 0f))
			{
				flag = true;
				if (playerTumbleForce > 0f)
				{
					_player.tumble.TumbleForce(normalized * playerTumbleForce);
				}
				if (playerTumbleTorque > 0f)
				{
					Vector3 val2 = Vector3.zero;
					if (playerTumbleTorqueAxis == TorqueAxis.up)
					{
						val2 = ((Component)_player).transform.up;
					}
					if (playerTumbleTorqueAxis == TorqueAxis.down)
					{
						val2 = -((Component)_player).transform.up;
					}
					if (playerTumbleTorqueAxis == TorqueAxis.right)
					{
						val2 = ((Component)_player).transform.right;
					}
					if (playerTumbleTorqueAxis == TorqueAxis.left)
					{
						val2 = -((Component)_player).transform.right;
					}
					if (playerTumbleTorqueAxis == TorqueAxis.forward)
					{
						val2 = ((Component)_player).transform.forward;
					}
					if (playerTumbleTorqueAxis == TorqueAxis.back)
					{
						val2 = -((Component)_player).transform.forward;
					}
					val = _player.localCameraPosition - center;
					Vector3 torque = Vector3.Cross(((Vector3)(ref val)).normalized, val2) * playerTumbleTorque;
					_player.tumble.TumbleTorque(torque);
				}
			}
			if (!flag2 && playerHitForce > 0f)
			{
				PlayerController.instance.ForceImpulse(normalized * playerHitForce);
			}
			if (playerHitForce > 0f || playerDamage > 0 || flag)
			{
				onImpactPlayerAvatar = _player;
				onImpactAny.Invoke();
				onImpactPlayer.Invoke();
			}
		}
	}

	private bool PhysObjectHurt(PhysGrabObject physGrabObject, BreakImpact impact, float hitForce, float hitTorque, bool apply, bool destroyLaunch)
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		switch (impact)
		{
		case BreakImpact.Light:
			physGrabObject.lightBreakImpulse = true;
			result = true;
			break;
		case BreakImpact.Medium:
			physGrabObject.mediumBreakImpulse = true;
			result = true;
			break;
		case BreakImpact.Heavy:
			physGrabObject.heavyBreakImpulse = true;
			result = true;
			break;
		}
		if (Object.op_Implicit((Object)(object)enemyHost) && impact != 0 && physGrabObject.playerGrabbing.Count <= 0 && !physGrabObject.impactDetector.isEnemy)
		{
			physGrabObject.impactDetector.enemyInteractionTimer = 2f;
		}
		if (hitForce > 0f)
		{
			if (hitForce >= 5f && physGrabObject.playerGrabbing.Count > 0)
			{
				foreach (PhysGrabber item in physGrabObject.playerGrabbing.ToList())
				{
					if (!SemiFunc.IsMultiplayer())
					{
						item.ReleaseObjectRPC(physGrabEnded: true, 2f);
						continue;
					}
					item.photonView.RPC("ReleaseObjectRPC", (RpcTarget)0, new object[2] { false, 1f });
				}
			}
			Bounds bounds = Collider.bounds;
			Vector3 center = ((Bounds)(ref bounds)).center;
			Vector3 val = physGrabObject.centerPoint - center;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			normalized = SemiFunc.ClampDirection(normalized, ((Component)this).transform.forward, hitSpread);
			applyForce = normalized * hitForce;
			val = physGrabObject.centerPoint - center;
			Vector3 normalized2 = ((Vector3)(ref val)).normalized;
			Vector3 val2 = -((Component)physGrabObject).transform.up;
			applyTorque = Vector3.Cross(normalized2, val2) * hitTorque;
			if (apply)
			{
				if (destroyLaunch && !physGrabObject.rb.isKinematic)
				{
					physGrabObject.rb.velocity = Vector3.zero;
					physGrabObject.rb.angularVelocity = Vector3.zero;
					physGrabObject.impactDetector.destroyDisableLaunches++;
					physGrabObject.impactDetector.destroyDisableLaunchesTimer = 10f;
					val = Random.insideUnitSphere;
					Vector3 val3 = ((Vector3)(ref val)).normalized * 4f;
					if (physGrabObject.impactDetector.destroyDisableLaunches >= 3)
					{
						val3 *= 20f;
						physGrabObject.impactDetector.destroyDisableLaunches = 0;
					}
					val3.y = 0f;
					applyForce = (Vector3.up * 20f + val3) * physGrabObject.rb.mass;
					val = Random.insideUnitSphere;
					applyTorque = ((Vector3)(ref val)).normalized * 0.25f * physGrabObject.rb.mass;
				}
				physGrabObject.rb.AddForce(applyForce, (ForceMode)1);
				physGrabObject.rb.AddTorque(applyTorque, (ForceMode)1);
				result = true;
			}
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider component = ((Component)this).GetComponent<BoxCollider>();
		SphereCollider component2 = ((Component)this).GetComponent<SphereCollider>();
		if (Object.op_Implicit((Object)(object)component2) && (((Component)this).transform.localScale.z != ((Component)this).transform.localScale.x || ((Component)this).transform.localScale.z != ((Component)this).transform.localScale.y))
		{
			Debug.LogError((object)"Sphere Collider must be uniform scale");
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 6f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		if (Object.op_Implicit((Object)(object)component))
		{
			Gizmos.DrawWireCube(component.center, component.size);
		}
		if (Object.op_Implicit((Object)(object)component2))
		{
			Gizmos.DrawWireSphere(component2.center, component2.radius);
		}
		Gizmos.color = new Color(1f, 0f, 0.39f, 0.2f);
		if (Object.op_Implicit((Object)(object)component))
		{
			Gizmos.DrawCube(component.center, component.size);
		}
		if (Object.op_Implicit((Object)(object)component2))
		{
			Gizmos.DrawSphere(component2.center, component2.radius);
		}
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 val = Vector3.zero;
		Bounds bounds;
		if (Object.op_Implicit((Object)(object)component))
		{
			bounds = ((Collider)component).bounds;
			val = ((Bounds)(ref bounds)).center;
		}
		if (Object.op_Implicit((Object)(object)component2))
		{
			bounds = ((Collider)component2).bounds;
			val = ((Bounds)(ref bounds)).center;
		}
		Vector3 val2 = val + ((Component)this).transform.forward * 0.5f;
		Gizmos.DrawLine(val, val2);
		Gizmos.DrawLine(val2, val2 + Vector3.LerpUnclamped(-((Component)this).transform.forward, -((Component)this).transform.right, 0.5f) * 0.25f);
		Gizmos.DrawLine(val2, val2 + Vector3.LerpUnclamped(-((Component)this).transform.forward, ((Component)this).transform.right, 0.5f) * 0.25f);
		if (hitSpread < 180f)
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
			Vector3 val3 = Quaternion.AngleAxis(hitSpread, ((Component)this).transform.right) * ((Component)this).transform.forward;
			Vector3 val4 = ((Vector3)(ref val3)).normalized * 1.5f;
			val3 = Quaternion.AngleAxis(0f - hitSpread, ((Component)this).transform.right) * ((Component)this).transform.forward;
			Vector3 val5 = ((Vector3)(ref val3)).normalized * 1.5f;
			val3 = Quaternion.AngleAxis(hitSpread, ((Component)this).transform.up) * ((Component)this).transform.forward;
			Vector3 val6 = ((Vector3)(ref val3)).normalized * 1.5f;
			val3 = Quaternion.AngleAxis(0f - hitSpread, ((Component)this).transform.up) * ((Component)this).transform.forward;
			Vector3 val7 = ((Vector3)(ref val3)).normalized * 1.5f;
			Gizmos.DrawRay(val, val4);
			Gizmos.DrawRay(val, val5);
			Gizmos.DrawRay(val, val6);
			Gizmos.DrawRay(val, val7);
			Gizmos.DrawLineStrip((ReadOnlySpan<Vector3>)(Vector3[]?)(object)new Vector3[4]
			{
				val + val4,
				val + val6,
				val + val5,
				val + val7
			}, true);
		}
		else if (hitSpread > 180f)
		{
			Debug.LogError((object)"Hit Spread cannot be greater than 180 degrees");
		}
	}
}
