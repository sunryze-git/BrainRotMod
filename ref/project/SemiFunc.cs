using System;
using System.Collections.Generic;
using System.Globalization;
using Photon.Pun;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public static class SemiFunc
{
	public enum emojiIcon
	{
		drone_heal,
		drone_zero_gravity,
		drone_indestructible,
		drone_feather,
		drone_torque,
		drone_battery,
		orb_heal,
		orb_zero_gravity,
		orb_indestructible,
		orb_feather,
		orb_torque,
		orb_battery,
		orb_magnet,
		grenade_explosive,
		grenade_stun,
		weapon_baseball_bat,
		weapon_sledgehammer,
		weapon_frying_pan,
		weapon_sword,
		weapon_inflatable_hammer,
		item_health_pack_S,
		item_health_pack_M,
		item_health_pack_L,
		item_gun_handgun,
		item_gun_shotgun,
		item_gun_tranq,
		item_valuable_tracker,
		item_extraction_tracker,
		item_grenade_human,
		item_grenade_duct_taped,
		item_rubber_duck,
		item_mine_explosive,
		item_grenade_shockwave,
		item_mine_shockwave,
		item_mine_stun
	}

	public enum itemVolume
	{
		small,
		medium,
		large,
		large_wide,
		power_crystal,
		large_high,
		upgrade,
		healthPack,
		large_plus
	}

	public enum itemType
	{
		drone,
		orb,
		cart,
		item_upgrade,
		player_upgrade,
		power_crystal,
		grenade,
		melee,
		healthPack,
		gun,
		tracker,
		mine,
		pocket_cart
	}

	public enum itemSecretShopType
	{
		none,
		shop_attic
	}

	public enum User
	{
		Walter,
		Axel,
		Robin,
		Jannek,
		Ruben,
		Builder
	}

	public static void EnemyCartJumpReset(Enemy enemy)
	{
		if (enemy.HasJump)
		{
			enemy.Jump.CartJump(0f);
		}
	}

	public static void EnemyCartJump(Enemy enemy)
	{
		if (enemy.HasJump)
		{
			enemy.Jump.CartJump(0.1f);
		}
	}

	public static Vector3 EnemyGetNearestPhysObject(Enemy enemy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabObject physGrabObject = null;
		float num = 9999f;
		Collider[] array = Physics.OverlapSphere(enemy.CenterTransform.position, 3f, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
		for (int i = 0; i < array.Length; i++)
		{
			PhysGrabObject componentInParent = ((Component)array[i]).GetComponentInParent<PhysGrabObject>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !Object.op_Implicit((Object)(object)((Component)componentInParent).GetComponent<EnemyRigidbody>()))
			{
				float num2 = Vector3.Distance(enemy.CenterTransform.position, componentInParent.centerPoint);
				if (num2 < num)
				{
					num = num2;
					physGrabObject = componentInParent;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)physGrabObject))
		{
			return physGrabObject.centerPoint;
		}
		return Vector3.zero;
	}

	public static bool EnemySpawn(Enemy enemy)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float minDistance = 18f;
		float maxDistance = 35f;
		if (EnemyDirector.instance.debugSpawnClose)
		{
			minDistance = 0f;
			maxDistance = 999f;
		}
		LevelPoint levelPoint = enemy.TeleportToPoint(minDistance, maxDistance);
		if (Object.op_Implicit((Object)(object)levelPoint))
		{
			bool flag = ((!enemy.HasRigidbody) ? (!EnemyPhysObjectSphereCheck(((Component)levelPoint).transform.position, 1f)) : (!EnemyPhysObjectBoundingBoxCheck(((Component)enemy).transform.position, ((Component)levelPoint).transform.position, enemy.Rigidbody.rb)));
			enemy.EnemyParent.firstSpawnPointUsed = true;
			if (flag)
			{
				return true;
			}
		}
		enemy.EnemyParent.Despawn();
		enemy.EnemyParent.DespawnedTimerSet(Random.Range(2f, 3f), _min: true);
		return false;
	}

	public static Camera MainCamera()
	{
		return GameDirector.instance.MainCamera;
	}

	public static bool EnemySpawnIdlePause()
	{
		if (EnemyDirector.instance.spawnIdlePauseTimer > 0f)
		{
			return true;
		}
		return false;
	}

	public static bool EnemyForceLeave(Enemy enemy)
	{
		if (enemy.EnemyParent.forceLeave)
		{
			enemy.EnemyParent.forceLeave = false;
			return true;
		}
		return false;
	}

	public static bool OnGroundCheck(Vector3 _position, float _distance, PhysGrabObject _notMe = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit[] array = Physics.RaycastAll(_position, Vector3.down, _distance, LayerMask.GetMask(new string[6] { "Default", "PhysGrabObject", "PhysGrabObjectCart", "PhysGrabObjectHinge", "Enemy", "Player" }));
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit val = array[i];
			PhysGrabObject componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<PhysGrabObject>();
			if (!Object.op_Implicit((Object)(object)componentInParent) || (Object)(object)componentInParent != (Object)(object)_notMe)
			{
				return true;
			}
		}
		return false;
	}

	public static PlayerAvatar PlayerGetFromSteamID(string _steamID)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.steamID == _steamID)
			{
				return player;
			}
		}
		return null;
	}

	public static Transform PlayerGetFaceEyeTransform(PlayerAvatar _player)
	{
		if (!_player.isLocal)
		{
			return _player.playerAvatarVisuals.headLookAtTransform;
		}
		return _player.localCameraTransform;
	}

	public static PlayerAvatar PlayerGetFromName(string _name)
	{
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.playerName == _name)
			{
				return player;
			}
		}
		return null;
	}

	public static Color PlayerGetColorFromSteamID(string _steamID)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		PlayerAvatar playerAvatar = PlayerGetFromSteamID(_steamID);
		if (Object.op_Implicit((Object)(object)playerAvatar))
		{
			return playerAvatar.playerAvatarVisuals.color;
		}
		return Color.black;
	}

	public static void ItemAffectEnemyBatteryDrain(EnemyParent _enemyParent, ItemBattery _itemBattery, float tumbleEnemyTimer, float _deltaTime, float _multiplier = 1f)
	{
		if (!Object.op_Implicit((Object)(object)_enemyParent) || !Object.op_Implicit((Object)(object)_itemBattery))
		{
			return;
		}
		Rigidbody componentInChildren = ((Component)_enemyParent).GetComponentInChildren<Rigidbody>();
		if (!Object.op_Implicit((Object)(object)componentInChildren))
		{
			return;
		}
		Enemy componentInChildren2 = ((Component)_enemyParent).GetComponentInChildren<Enemy>();
		if (!Object.op_Implicit((Object)(object)componentInChildren2))
		{
			return;
		}
		switch ((int)_enemyParent.difficulty)
		{
		case 0:
		{
			float num3 = componentInChildren.mass * 0.5f;
			num3 = Mathf.Clamp(num3, 3f, 4f) * _multiplier;
			_itemBattery.batteryLife -= num3 * _deltaTime;
			if (tumbleEnemyTimer > 1.5f && componentInChildren2.HasStateStunned)
			{
				componentInChildren2.StateStunned.Set(1f);
			}
			break;
		}
		case 1:
		{
			float num2 = componentInChildren.mass * 0.85f;
			num2 = Mathf.Clamp(num2, 5f, 6f) * _multiplier;
			_itemBattery.batteryLife -= num2 * _deltaTime;
			if (tumbleEnemyTimer > 3f && componentInChildren2.HasStateStunned)
			{
				componentInChildren2.StateStunned.Set(1f);
			}
			break;
		}
		case 2:
		{
			float num = componentInChildren.mass * 1f;
			num = Mathf.Clamp(num, 7f, 8f) * _multiplier;
			_itemBattery.batteryLife -= num * _deltaTime;
			if (tumbleEnemyTimer > 4f && componentInChildren2.HasStateStunned)
			{
				componentInChildren2.StateStunned.Set(1f);
			}
			break;
		}
		}
	}

	public static void EnemyInvestigate(Vector3 position, float range)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		EnemyDirector.instance.SetInvestigate(position, range);
	}

	public static int EnemyGetIndex(Enemy _enemy)
	{
		int result = -1;
		if (Object.op_Implicit((Object)(object)_enemy))
		{
			foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
			{
				if ((Object)(object)item.Enemy == (Object)(object)_enemy)
				{
					result = EnemyDirector.instance.enemiesSpawned.IndexOf(item);
					break;
				}
			}
		}
		return result;
	}

	public static Enemy EnemyGetFromIndex(int _enemyIndex)
	{
		Enemy result = null;
		if (_enemyIndex == -1)
		{
			return result;
		}
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			if (EnemyDirector.instance.enemiesSpawned.IndexOf(item) == _enemyIndex)
			{
				result = item.Enemy;
				break;
			}
		}
		return result;
	}

	public static Enemy EnemyGetNearest(Vector3 _position, float _maxDistance, bool _raycast)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Enemy result = null;
		float num = _maxDistance;
		foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
		{
			if (item.DespawnedTimer > 0f)
			{
				continue;
			}
			Vector3 val = item.Enemy.CenterTransform.position - _position;
			if (!(((Vector3)(ref val)).magnitude < num))
			{
				continue;
			}
			if (_raycast)
			{
				bool flag = false;
				RaycastHit[] array = Physics.RaycastAll(_position, val, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(LayerMaskGetVisionObstruct()));
				for (int i = 0; i < array.Length; i++)
				{
					RaycastHit val2 = array[i];
					if (((Component)((RaycastHit)(ref val2)).collider).gameObject.CompareTag("Wall"))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			num = ((Vector3)(ref val)).magnitude;
			result = item.Enemy;
		}
		return result;
	}

	public static bool EnemyPhysObjectSphereCheck(Vector3 _position, float _radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.OverlapSphere(_position, _radius, LayerMask.op_Implicit(LayerMaskGetPhysGrabObject())).Length != 0)
		{
			return true;
		}
		return false;
	}

	public static bool EnemyPhysObjectBoundingBoxCheck(Vector3 _currentPosition, Vector3 _checkPosition, Rigidbody _rigidbody)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(Vector3.zero, Vector3.zero);
		Collider[] componentsInChildren = ((Component)_rigidbody).GetComponentsInChildren<Collider>();
		foreach (Collider val in componentsInChildren)
		{
			if (((Bounds)(ref bounds)).size == Vector3.zero)
			{
				bounds = val.bounds;
			}
			else
			{
				((Bounds)(ref bounds)).Encapsulate(val.bounds);
			}
		}
		Vector3 val2 = _currentPosition - ((Component)_rigidbody).transform.position;
		Vector3 val3 = ((Bounds)(ref bounds)).center - ((Component)_rigidbody).transform.position;
		((Bounds)(ref bounds)).center = _checkPosition - val2 + val3;
		((Bounds)(ref bounds)).size = ((Bounds)(ref bounds)).size * 1.2f;
		componentsInChildren = Physics.OverlapBox(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents, Quaternion.identity, LayerMask.op_Implicit(LayerMaskGetPhysGrabObject()));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((Object)(object)((Component)componentsInChildren[i]).GetComponentInParent<Rigidbody>() != (Object)(object)_rigidbody)
			{
				return true;
			}
		}
		return false;
	}

	public static void DebugDrawBounds(Bounds _bounds, Color _color, float _time)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(((Bounds)(ref _bounds)).min.x, ((Bounds)(ref _bounds)).min.y, ((Bounds)(ref _bounds)).min.z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(((Bounds)(ref _bounds)).max.x, ((Bounds)(ref _bounds)).min.y, ((Bounds)(ref _bounds)).min.z);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(((Bounds)(ref _bounds)).max.x, ((Bounds)(ref _bounds)).min.y, ((Bounds)(ref _bounds)).max.z);
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(((Bounds)(ref _bounds)).min.x, ((Bounds)(ref _bounds)).min.y, ((Bounds)(ref _bounds)).max.z);
		Debug.DrawLine(val, val2, _color, _time);
		Debug.DrawLine(val2, val3, _color, _time);
		Debug.DrawLine(val3, val4, _color, _time);
		Debug.DrawLine(val4, val, _color, _time);
		Vector3 val5 = default(Vector3);
		((Vector3)(ref val5))._002Ector(((Bounds)(ref _bounds)).min.x, ((Bounds)(ref _bounds)).max.y, ((Bounds)(ref _bounds)).min.z);
		Vector3 val6 = default(Vector3);
		((Vector3)(ref val6))._002Ector(((Bounds)(ref _bounds)).max.x, ((Bounds)(ref _bounds)).max.y, ((Bounds)(ref _bounds)).min.z);
		Vector3 val7 = default(Vector3);
		((Vector3)(ref val7))._002Ector(((Bounds)(ref _bounds)).max.x, ((Bounds)(ref _bounds)).max.y, ((Bounds)(ref _bounds)).max.z);
		Vector3 val8 = default(Vector3);
		((Vector3)(ref val8))._002Ector(((Bounds)(ref _bounds)).min.x, ((Bounds)(ref _bounds)).max.y, ((Bounds)(ref _bounds)).max.z);
		Debug.DrawLine(val5, val6, _color, _time);
		Debug.DrawLine(val6, val7, _color, _time);
		Debug.DrawLine(val7, val8, _color, _time);
		Debug.DrawLine(val8, val5, _color, _time);
		Debug.DrawLine(val, val5, _color, _time);
		Debug.DrawLine(val2, val6, _color, _time);
		Debug.DrawLine(val3, val7, _color, _time);
		Debug.DrawLine(val4, val8, _color, _time);
	}

	public static bool DebugUser(User _user)
	{
		if (DebugDev() && Object.op_Implicit((Object)(object)DebugComputerCheck.instance) && DebugComputerCheck.instance.DebugUser == _user)
		{
			return true;
		}
		return false;
	}

	public static bool Axel()
	{
		return DebugUser(User.Axel);
	}

	public static bool Jannek()
	{
		return DebugUser(User.Jannek);
	}

	public static bool Robin()
	{
		return DebugUser(User.Robin);
	}

	public static bool Ruben()
	{
		return DebugUser(User.Ruben);
	}

	public static bool Walter()
	{
		return DebugUser(User.Walter);
	}

	public static bool DebugKeyDown(User _user, KeyCode _input)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DebugUser(_user))
		{
			return Input.GetKeyUp(_input);
		}
		return false;
	}

	public static bool KeyDownAxel(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKeyDown(User.Axel, _input);
	}

	public static bool KeyDownJannek(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKeyDown(User.Jannek, _input);
	}

	public static bool KeyDownRobin(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKeyDown(User.Robin, _input);
	}

	public static bool KeyDownRuben(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKeyDown(User.Ruben, _input);
	}

	public static bool KeyDownWalter(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKeyDown(User.Walter, _input);
	}

	public static bool DebugKey(User _user, KeyCode _input)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DebugUser(_user))
		{
			return Input.GetKey(_input);
		}
		return false;
	}

	public static bool KeyAxel(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKey(User.Axel, _input);
	}

	public static bool KeyJannek(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKey(User.Jannek, _input);
	}

	public static bool KeyRobin(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKey(User.Robin, _input);
	}

	public static bool KeyRuben(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKey(User.Ruben, _input);
	}

	public static bool KeyWalter(KeyCode _input)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DebugKey(User.Walter, _input);
	}

	public static bool DebugDev()
	{
		return SteamManager.instance.developerMode;
	}

	public static float UIMulti()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return HUDCanvas.instance.rect.sizeDelta.y;
	}

	public static string MessageGeneratedGetLeftBehind()
	{
		List<string> list = new List<string> { "You", "They", "My team", "Everyone", "My friends", "The squad", "The group", "All of them", "Those I trusted", "My companions" };
		List<string> list2 = new List<string> { "left", "abandoned", "betrayed", "forgot", "doomed", "deserted", "ditched", "dissed", "discarded", "forgot" };
		List<string> list3 = new List<string>
		{
			"me", "lil old me", "this lil robot", "my life", "my only hope", "my chance", "this poor robot", "my feelings", "our friendship", "my heart",
			"my trust"
		};
		List<string> list4 = new List<string> { "behind", "alone", "in the dark", "without a word", "without warning", "in silence", "without looking back", "with no remorse" };
		List<string> list5 = new List<string> { "I feel lost.", "Why didn't they wait?", "What did I do wrong?", "I can't believe it.", "How could they?", "This can't be happening.", "I'm on my own now.", "They were my only hope.", "I should have known.", "It's so unfair." };
		List<string> list6 = new List<string> { "{subject} {verb} {object}.", "{additional_phrase} {subject} {verb} {object}...", "{subject} {verb} lil me {adverb}.", "{additional_phrase}", "They {verb} me {adverb}...", "Now, {subject} {verb} {object}.", "In the end, {subject} {verb} {object}.", "I can't believe {subject} {verb} {object}...", "{subject} {verb} {object}. {additional_phrase}", "{additional_phrase} {subject} {verb} {object}." };
		int index = Random.Range(0, list6.Count);
		return list6[index].Replace("{subject}", list[Random.Range(0, list.Count)]).Replace("{verb}", list2[Random.Range(0, list2.Count)]).Replace("{object}", list3[Random.Range(0, list3.Count)])
			.Replace("{adverb}", list4[Random.Range(0, list4.Count)])
			.Replace("{additional_phrase}", list5[Random.Range(0, list5.Count)]);
	}

	public static bool MainMenuIsSingleplayer()
	{
		if (MainMenuOpen.instance.MainMenuGetState() == MainMenuOpen.MainMenuGameModeState.SinglePlayer)
		{
			return true;
		}
		return false;
	}

	public static void MenuActionSingleplayerGame(string saveFileName = null)
	{
		RunManager.instance.ResetProgress();
		if (saveFileName != null)
		{
			Debug.Log((object)"Loading save");
			SaveFileLoad(saveFileName);
		}
		else
		{
			SaveFileCreate();
		}
		DataDirector.instance.RunsPlayedAdd();
		if (RunManager.instance.loadLevel == 0)
		{
			RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.RunLevel);
		}
		else
		{
			RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.Shop);
		}
	}

	public static void MenuActionHostGame(string saveFileName = null)
	{
		RunManager.instance.ResetProgress();
		if (saveFileName != null)
		{
			SaveFileLoad(saveFileName);
		}
		else
		{
			SaveFileCreate();
		}
		GameManager.instance.localTest = false;
		RunManager.instance.waitToChangeScene = true;
		RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.LobbyMenu);
		MainMenuOpen.instance.NetworkConnect();
	}

	public static void SaveFileLoad(string saveFileName)
	{
		StatsManager.instance.LoadGame(saveFileName);
	}

	public static void SaveFileDelete(string saveFileName)
	{
		if (!string.IsNullOrEmpty(saveFileName))
		{
			StatsManager.instance.SaveFileDelete(saveFileName);
		}
	}

	public static List<string> SaveFileGetAll()
	{
		return StatsManager.instance.SaveFileGetAll();
	}

	public static void SaveFileCreate()
	{
		StatsManager.instance.SaveFileCreate();
	}

	public static void SaveFileSave()
	{
		StatsManager.instance.SaveFileSave();
	}

	public static bool MainMenuIsMultiplayer()
	{
		if (MainMenuOpen.instance.MainMenuGetState() == MainMenuOpen.MainMenuGameModeState.MultiPlayer)
		{
			return true;
		}
		return false;
	}

	public static void MainMenuSetSingleplayer()
	{
		MainMenuOpen.instance.MainMenuSetState(0);
	}

	public static void MainMenuSetMultiplayer()
	{
		MainMenuOpen.instance.MainMenuSetState(1);
	}

	public static List<PlayerAvatar> PlayerGetAllPlayerAvatarWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.isDisabled)
			{
				continue;
			}
			Vector3 position2 = player.PlayerVisionTarget.VisionTransform.position;
			float num = Vector3.Distance(position, position2);
			if (num > range)
			{
				continue;
			}
			Vector3 val = position2 - position;
			bool flag = false;
			if (doRaycastCheck)
			{
				RaycastHit[] array = Physics.RaycastAll(position, val, num, LayerMask.op_Implicit(layerMask));
				for (int i = 0; i < array.Length; i++)
				{
					RaycastHit val2 = array[i];
					if (((Component)((Component)((RaycastHit)(ref val2)).collider).transform).CompareTag("Wall"))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				list.Add(player);
			}
		}
		return list;
	}

	public static List<PlayerAvatar> PlayerGetAllPlayerAvatarWithinRangeAndVision(float range, Vector3 position, PhysGrabObject _thisPhysGrabObject = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		LayerMask val = LayerMaskGetVisionObstruct();
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.isDisabled)
			{
				continue;
			}
			Vector3 position2 = player.PlayerVisionTarget.VisionTransform.position;
			float num = Vector3.Distance(position, position2);
			if (num > range)
			{
				continue;
			}
			Vector3 val2 = position2 - position;
			bool flag = false;
			RaycastHit[] array = Physics.RaycastAll(position, val2, num, LayerMask.op_Implicit(val));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val3 = array[i];
				if (!((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponentInParent<PhysGrabObject>() == (Object)(object)_thisPhysGrabObject))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				list.Add(player);
			}
		}
		return list;
	}

	public static PlayerAvatar PlayerGetNearestPlayerAvatarWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = range;
		PlayerAvatar result = null;
		List<PlayerAvatar> list = PlayerGetAllPlayerAvatarWithinRange(range, position, doRaycastCheck, layerMask);
		if (list.Count > 0)
		{
			foreach (PlayerAvatar item in list)
			{
				Vector3 position2 = item.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	public static float PlayerNearestDistance(Vector3 position)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		float num = 999f;
		float result = 9f;
		List<PlayerAvatar> playerList = GameDirector.instance.PlayerList;
		if (playerList.Count > 0)
		{
			foreach (PlayerAvatar item in playerList)
			{
				Vector3 position2 = item.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = num;
				}
			}
		}
		return result;
	}

	public static bool PlayerVisionCheck(Vector3 _position, float _range, PlayerAvatar _player, bool _previouslySeen)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return PlayerVisionCheckPosition(_position, _player.PlayerVisionTarget.VisionTransform.position, _range, _player, _previouslySeen);
	}

	public static bool PlayerVisionCheckPosition(Vector3 _startPosition, Vector3 _endPosition, float _range, PlayerAvatar _player, bool _previouslySeen)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (_player.enemyVisionFreezeTimer > 0f)
		{
			return _previouslySeen;
		}
		LayerMask val = LayerMaskGetVisionObstruct();
		Vector3 val2 = _endPosition - _startPosition;
		if (((Vector3)(ref val2)).magnitude > _range)
		{
			return false;
		}
		if (((Vector3)(ref val2)).magnitude < _range)
		{
			_range = ((Vector3)(ref val2)).magnitude;
		}
		RaycastHit[] array = Physics.RaycastAll(_startPosition, val2, _range, LayerMask.op_Implicit(val));
		PlayerAvatar playerAvatar = null;
		Transform val3 = null;
		Transform val4 = null;
		Vector3 val5 = Vector3.zero;
		float num = 1000f;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit val6 = array2[i];
			float num2 = Vector3.Distance(_startPosition, ((RaycastHit)(ref val6)).point);
			if (!(num2 < num))
			{
				continue;
			}
			num = num2;
			val4 = ((RaycastHit)(ref val6)).transform;
			val5 = ((RaycastHit)(ref val6)).point;
			PlayerAvatar playerAvatar2 = null;
			if (((Component)((RaycastHit)(ref val6)).transform).CompareTag("Player"))
			{
				playerAvatar2 = ((Component)((RaycastHit)(ref val6)).transform).GetComponentInParent<PlayerAvatar>();
				if (!Object.op_Implicit((Object)(object)playerAvatar2))
				{
					PlayerController componentInParent = ((Component)((RaycastHit)(ref val6)).transform).GetComponentInParent<PlayerController>();
					if (Object.op_Implicit((Object)(object)componentInParent))
					{
						playerAvatar2 = componentInParent.playerAvatarScript;
					}
				}
			}
			else
			{
				PlayerTumble componentInParent2 = ((Component)((RaycastHit)(ref val6)).transform).GetComponentInParent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)componentInParent2))
				{
					playerAvatar2 = componentInParent2.playerAvatar;
				}
			}
			if (Object.op_Implicit((Object)(object)playerAvatar2) && (Object)(object)playerAvatar2 == (Object)(object)_player)
			{
				playerAvatar = playerAvatar2;
				val3 = ((RaycastHit)(ref val6)).transform;
			}
		}
		if (Object.op_Implicit((Object)(object)playerAvatar) && (Object)(object)val3 == (Object)(object)val4)
		{
			Debug.DrawRay(_startPosition, val2, Color.green, 0.1f);
			return true;
		}
		Debug.DrawRay(_startPosition, val5 - _startPosition, Color.red, 0.1f);
		return false;
	}

	public static void PlayerEyesOverride(PlayerAvatar _player, Vector3 _position, float _time, GameObject _obj)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		_player.playerAvatarVisuals.playerEyes.Override(_position, _time, _obj);
	}

	public static void PlayerEyesOverrideSoft(Vector3 _position, float _time, GameObject _obj, float _radius)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (Vector3.Distance(_position, ((Component)player).transform.position) < _radius)
			{
				player.playerAvatarVisuals.playerEyes.OverrideSoft(_position, _time, _obj);
			}
		}
	}

	public static Transform PlayerGetNearestTransformWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask))
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = range;
		Transform result = null;
		List<PlayerAvatar> list = PlayerGetAllPlayerAvatarWithinRange(range, position, doRaycastCheck, layerMask);
		if (list.Count > 0)
		{
			foreach (PlayerAvatar item in list)
			{
				Vector3 position2 = item.PlayerVisionTarget.VisionTransform.position;
				float num2 = Vector3.Distance(position, position2);
				if (num2 < num)
				{
					num = num2;
					result = item.PlayerVisionTarget.VisionTransform;
				}
			}
		}
		return result;
	}

	public static List<PlayerAvatar> PlayerGetAll()
	{
		return GameDirector.instance.PlayerList;
	}

	public static bool PlayersAllInTruck()
	{
		foreach (PlayerAvatar item in PlayerGetList())
		{
			if (!item.isDisabled && !item.RoomVolumeCheck.inTruck)
			{
				return false;
			}
		}
		return true;
	}

	public static void Command(string _command)
	{
		string text = _command.ToLower();
		switch (text)
		{
		default:
		{
			if (text != null && _command.Length >= 4 && _command.Substring(0, 4) == "/fps" && DebugDev() && int.TryParse(_command.Substring(5), out var result))
			{
				GameDirector.instance.CommandSetFPS(result);
			}
			break;
		}
		case "/cinematic":
			GameDirector.instance.CommandRecordingDirectorToggle();
			break;
		case "/greenscreen":
			GameDirector.instance.CommandGreenScreenToggle();
			break;
		case "/enemy vision":
			if (DebugDev())
			{
				EnemyDirector.instance.debugNoVision = !EnemyDirector.instance.debugNoVision;
			}
			break;
		case "/slow":
			if (DebugDev())
			{
				PlayerController.instance.debugSlow = !PlayerController.instance.debugSlow;
			}
			break;
		case "/recording level":
			if (DebugDev())
			{
				RunManager.instance.ChangeLevel(_completedLevel: true, _levelFailed: false, RunManager.ChangeLevelType.Recording);
			}
			break;
		case "/clear":
			Debug.ClearDeveloperConsole();
			break;
		}
	}

	public static void CursorUnlock(float _time)
	{
		CursorManager.instance.Unlock(_time);
	}

	public static bool OnValidateCheck()
	{
		return false;
	}

	public static void LightAdd(PropLight propLight)
	{
		if (!LightManager.instance.propLights.Contains(propLight))
		{
			LightManager.instance.propLights.Add(propLight);
		}
	}

	public static void LightRemove(PropLight propLight)
	{
		if (LightManager.instance.propLights.Contains(propLight))
		{
			LightManager.instance.propLights.Remove(propLight);
		}
	}

	public static Vector3 EnemyRoamFindPoint(Vector3 _position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = Vector3.zero;
		LevelPoint levelPoint = LevelPointGet(_position, 10f, 25f);
		if (!Object.op_Implicit((Object)(object)levelPoint))
		{
			levelPoint = LevelPointGet(_position, 0f, 999f);
		}
		NavMeshHit val = default(NavMeshHit);
		if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			result = ((NavMeshHit)(ref val)).position;
		}
		return result;
	}

	public static Vector3 EnemyLeaveFindPoint(Vector3 _position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = Vector3.zero;
		LevelPoint levelPoint = LevelPointGetPlayerDistance(_position, 30f, 50f);
		if (!Object.op_Implicit((Object)(object)levelPoint))
		{
			levelPoint = LevelPointGetFurthestFromPlayer(_position, 5f);
		}
		NavMeshHit val = default(NavMeshHit);
		if (Object.op_Implicit((Object)(object)levelPoint) && NavMesh.SamplePosition(((Component)levelPoint).transform.position + Random.insideUnitSphere * 3f, ref val, 5f, -1) && Physics.Raycast(((NavMeshHit)(ref val)).position, Vector3.down, 5f, LayerMask.GetMask(new string[1] { "Default" })))
		{
			result = ((NavMeshHit)(ref val)).position;
		}
		return result;
	}

	public static List<LevelPoint> LevelPointGetWithinDistance(Vector3 pos, float minDist, float maxDist)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			float num = Vector3.Distance(((Component)levelPathPoint).transform.position, pos);
			if (num >= minDist && num <= maxDist)
			{
				list.Add(levelPathPoint);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	public static List<LevelPoint> LevelPointsGetAll()
	{
		return LevelGenerator.Instance.LevelPathPoints;
	}

	public static LevelPoint LevelPointsGetClosestToPlayer()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		List<PlayerAvatar> list = PlayerGetList();
		List<LevelPoint> list2 = LevelPointsGetAll();
		float num = 999f;
		LevelPoint result = null;
		foreach (PlayerAvatar item in list)
		{
			if (item.isDisabled)
			{
				continue;
			}
			Vector3 position = ((Component)item).transform.position;
			foreach (LevelPoint item2 in list2)
			{
				float num2 = Vector3.Distance(position, ((Component)item2).transform.position);
				if (num2 < num)
				{
					num = num2;
					result = item2;
				}
			}
		}
		return result;
	}

	public static List<LevelPoint> LevelPointsGetAllCloseToPlayers()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<PlayerAvatar> list = PlayerGetList();
		List<LevelPoint> list2 = LevelPointsGetAll();
		List<LevelPoint> list3 = new List<LevelPoint>();
		foreach (PlayerAvatar item2 in list)
		{
			float num = 999f;
			LevelPoint item = null;
			if (item2.isDisabled)
			{
				continue;
			}
			Vector3 position = ((Component)item2).transform.position;
			foreach (LevelPoint item3 in list2)
			{
				float num2 = Vector3.Distance(position, ((Component)item3).transform.position);
				if (num2 < num)
				{
					num = num2;
					item = item3;
				}
			}
			list3.Add(item);
		}
		return list3;
	}

	public static List<LevelPoint> LevelPointsGetInPlayerRooms()
	{
		List<PlayerAvatar> list = PlayerGetList();
		List<LevelPoint> list2 = LevelPointsGetAll();
		List<LevelPoint> list3 = new List<LevelPoint>();
		foreach (PlayerAvatar item in list)
		{
			if (item.isDisabled)
			{
				continue;
			}
			foreach (RoomVolume currentRoom in item.RoomVolumeCheck.CurrentRooms)
			{
				foreach (LevelPoint item2 in list2)
				{
					if ((Object)(object)item2.Room == (Object)(object)currentRoom)
					{
						list3.Add(item2);
					}
				}
			}
		}
		return list3;
	}

	public static List<LevelPoint> LevelPointsGetInStartRoom()
	{
		List<LevelPoint> list = LevelPointsGetAll();
		List<LevelPoint> list2 = new List<LevelPoint>();
		foreach (LevelPoint item in list)
		{
			if (item.inStartRoom)
			{
				list2.Add(item);
			}
		}
		return list2;
	}

	public static LevelPoint LevelPointGetPlayerDistance(Vector3 _position, float _minDistance, float _maxDistance, bool _startRoomOnly = false)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if ((_startRoomOnly && !levelPathPoint.inStartRoom) || levelPathPoint.Room.Truck)
			{
				continue;
			}
			float num = 999f;
			bool flag = false;
			Vector3 position = ((Component)levelPathPoint).transform.position;
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (!player.isDisabled)
				{
					float num2 = Vector3.Distance(position, ((Component)player).transform.position);
					if (num2 < num)
					{
						num = num2;
					}
					if (num2 < _maxDistance)
					{
						flag = true;
					}
				}
			}
			if (num > _minDistance && flag)
			{
				list.Add(levelPathPoint);
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}

	public static LevelPoint LevelPointGetFurthestFromPlayer(Vector3 _position, float _minDistance)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		LevelPoint result = null;
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPathPoint.Room.Truck)
			{
				continue;
			}
			float num2 = 999f;
			float num3 = 0f;
			Vector3 position = ((Component)levelPathPoint).transform.position;
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (!player.isDisabled)
				{
					float num4 = Vector3.Distance(position, ((Component)player).transform.position);
					if (num4 < num2)
					{
						num2 = num4;
					}
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
			}
			if (num2 > _minDistance && num3 > num)
			{
				num = num3;
				result = levelPathPoint;
			}
		}
		return result;
	}

	public static void PhysLookAtPositionWithForce(Rigidbody rb, Transform transform, Vector3 position, float forceMultiplier)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = position - transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = Vector3.Cross(transform.forward, normalized);
		float magnitude = ((Vector3)(ref val2)).magnitude;
		((Vector3)(ref val2)).Normalize();
		rb.AddTorque(val2 * magnitude * forceMultiplier);
	}

	public static bool IsNotMasterClient()
	{
		if (GameManager.Multiplayer())
		{
			return !PhotonNetwork.IsMasterClient;
		}
		return false;
	}

	public static bool IsMasterClientOrSingleplayer()
	{
		if (!GameManager.Multiplayer() || !PhotonNetwork.IsMasterClient)
		{
			return !GameManager.Multiplayer();
		}
		return true;
	}

	public static bool IsMasterClient()
	{
		if (GameManager.Multiplayer())
		{
			return PhotonNetwork.IsMasterClient;
		}
		return false;
	}

	public static bool IsMainMenu()
	{
		return Object.op_Implicit((Object)(object)MainMenuOpen.instance);
	}

	public static bool IsMultiplayer()
	{
		return GameManager.instance.gameMode == 1;
	}

	public static bool MenuLevel()
	{
		if (Object.op_Implicit((Object)(object)MainMenuOpen.instance) || Object.op_Implicit((Object)(object)LobbyMenuOpen.instance))
		{
			return true;
		}
		return false;
	}

	public static bool RunIsArena()
	{
		if ((Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelArena)
		{
			return true;
		}
		return false;
	}

	public static void CameraShake(float strength, float duration)
	{
		GameDirector.instance.CameraShake.Shake(strength, duration);
	}

	public static void CameraShakeDistance(Vector3 position, float strength, float duration, float distanceMin, float distanceMax)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraShake.ShakeDistance(strength, distanceMin, distanceMax, position, duration);
	}

	public static void CameraShakeImpact(float strength, float duration)
	{
		GameDirector.instance.CameraImpact.Shake(strength, duration);
	}

	public static void CameraShakeImpactDistance(Vector3 position, float strength, float duration, float distanceMin, float distanceMax)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(strength, distanceMin, distanceMax, position, duration);
	}

	public static Color ColorDifficultyGet(float minValue, float maxValue, float _currentValue)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[4]
		{
			new Color(0f, 1f, 0f),
			new Color(1f, 1f, 0f),
			new Color(1f, 0.5f, 0f),
			new Color(1f, 0f, 0f)
		};
		int num = Mathf.FloorToInt(Mathf.Lerp(0f, (float)(array.Length - 1), Mathf.InverseLerp(minValue, maxValue, _currentValue)));
		float num2 = Mathf.InverseLerp(minValue, maxValue, _currentValue) * (float)(array.Length - 1) - (float)num;
		Color val = array[Mathf.Clamp(num, 0, array.Length - 1)];
		Color val2 = array[Mathf.Clamp(num + 1, 0, array.Length - 1)];
		return Color.Lerp(val, val2, num2);
	}

	public static string TimeToString(float time, bool fancy = false, Color numberColor = default(Color), Color unitColor = default(Color))
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)(time / 3600f);
		int num2 = (int)(time % 3600f / 60f);
		int num3 = (int)(time % 60f);
		string text = "h ";
		string text2 = "m ";
		string text3 = "s";
		if (fancy)
		{
			text = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">h</color> ";
			text2 = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">m</color> ";
			text3 = "</b></color><color=#" + ColorUtility.ToHtmlStringRGBA(unitColor) + ">s</color>";
		}
		string text4 = "";
		if (num > 0)
		{
			if (fancy)
			{
				text4 = text4 + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text4 = text4 + num + text;
		}
		if (num2 > 0 || num > 0)
		{
			if (fancy)
			{
				text4 = text4 + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text4 = text4 + num2 + text2;
		}
		if ((num == 0 && num2 == 0) || fancy)
		{
			if (fancy)
			{
				text4 = text4 + "<color=#" + ColorUtility.ToHtmlStringRGBA(numberColor) + "><b>";
			}
			text4 = text4 + num3 + text3;
		}
		return text4;
	}

	public static List<PhysGrabObject> PhysGrabObjectGetAllWithinRange(float range, Vector3 position, bool doRaycastCheck = false, LayerMask layerMask = default(LayerMask), PhysGrabObject _thisPhysGrabObject = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<PhysGrabObject> list = new List<PhysGrabObject>();
		Collider[] array = Physics.OverlapSphere(position, range, LayerMask.GetMask(new string[1] { "PhysGrabObject" }));
		for (int i = 0; i < array.Length; i++)
		{
			PhysGrabObject componentInParent = ((Component)array[i]).GetComponentInParent<PhysGrabObject>();
			if (!((Object)(object)componentInParent != (Object)null))
			{
				continue;
			}
			bool flag = false;
			if (doRaycastCheck)
			{
				Vector3 val = componentInParent.midPoint - position;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				RaycastHit[] array2 = Physics.RaycastAll(position, normalized, range, LayerMask.op_Implicit(layerMask));
				for (int j = 0; j < array2.Length; j++)
				{
					RaycastHit val2 = array2[j];
					PhysGrabObject componentInParent2 = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<PhysGrabObject>();
					if (!((Object)(object)componentInParent2 == (Object)(object)_thisPhysGrabObject) && !((Object)(object)componentInParent2 == (Object)(object)componentInParent) && ((Object)(object)componentInParent2 == (Object)null || ((Object)(object)componentInParent2 != (Object)null && (Object)(object)componentInParent2 != (Object)(object)componentInParent)))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add(componentInParent);
			}
		}
		return list;
	}

	public static bool LocalPlayerOverlapCheck(float range, Vector3 position, bool doRaycastCheck = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(position, range, LayerMask.op_Implicit(LayerMaskGetVisionObstruct()));
		foreach (Collider val in array)
		{
			PlayerController playerController = null;
			if (((Component)((Component)val).transform).CompareTag("Player"))
			{
				playerController = ((Component)val).GetComponentInParent<PlayerController>();
			}
			else
			{
				PlayerTumble componentInParent = ((Component)val).GetComponentInParent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.playerAvatar.isLocal)
				{
					playerController = PlayerController.instance;
				}
			}
			if (!Object.op_Implicit((Object)(object)playerController))
			{
				continue;
			}
			bool flag = false;
			if (doRaycastCheck)
			{
				Vector3 val2 = ((Component)val).transform.position - position;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				RaycastHit[] array2 = Physics.RaycastAll(position, normalized, range, LayerMask.GetMask(new string[1] { "Default" }));
				for (int j = 0; j < array2.Length; j++)
				{
					RaycastHit val3 = array2[j];
					if (((Component)((RaycastHit)(ref val3)).transform).CompareTag("Wall"))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 PhysFollowPosition(Vector3 _currentPosition, Vector3 _targetPosition, Vector3 _currentVelocity, float _maxSpeed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.ClampMagnitude((_targetPosition - _currentPosition) / Time.fixedDeltaTime, _maxSpeed) - _currentVelocity;
	}

	public static Vector3 PhysFollowRotation(Transform _transform, Quaternion _targetRotation, Rigidbody _rigidbody, float _maxSpeed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		_transform.rotation = Quaternion.RotateTowards(_targetRotation, _transform.rotation, 360f);
		Quaternion val = _targetRotation * Quaternion.Inverse(_transform.rotation);
		float num = default(float);
		Vector3 val2 = default(Vector3);
		((Quaternion)(ref val)).ToAngleAxis(ref num, ref val2);
		((Vector3)(ref val2)).Normalize();
		Vector3 val3 = val2 * num * (MathF.PI / 180f) / Time.fixedDeltaTime;
		val3 -= _rigidbody.angularVelocity;
		Vector3 val4 = _transform.InverseTransformDirection(val3);
		val4 = _rigidbody.inertiaTensorRotation * val4;
		((Vector3)(ref val4)).Scale(_rigidbody.inertiaTensor);
		Vector3 val5 = Quaternion.Inverse(_rigidbody.inertiaTensorRotation) * val4;
		return Vector3.ClampMagnitude(_transform.TransformDirection(val5), _maxSpeed);
	}

	public static Vector3 PhysFollowDirection(Transform _transform, Vector3 _targetDirection, Rigidbody _rigidbody, float _maxSpeed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(Vector3.up, _targetDirection);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Quaternion rotation = _transform.rotation;
		_transform.Rotate(normalized * 100f, (Space)0);
		Quaternion rotation2 = _transform.rotation;
		_transform.rotation = rotation;
		return PhysFollowRotation(((Component)_transform).transform, rotation2, _rigidbody, _maxSpeed);
	}

	public static LevelPoint LevelPointGet(Vector3 _position, float _minDistance, float _maxDistance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		LevelPoint result = null;
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (!levelPathPoint.Room.Truck)
			{
				float num = Vector3.Distance(((Component)levelPathPoint).transform.position, _position);
				if (num >= _minDistance && num <= _maxDistance)
				{
					list.Add(levelPathPoint);
				}
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	public static LevelPoint LevelPointInTargetRoomGet(RoomVolumeCheck _target, float _minDistance, float _maxDistance, LevelPoint ignorePoint = null)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		LevelPoint result = null;
		List<LevelPoint> list = new List<LevelPoint>();
		foreach (LevelPoint levelPathPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			foreach (RoomVolume currentRoom in _target.CurrentRooms)
			{
				if (!((Object)(object)levelPathPoint == (Object)(object)ignorePoint) && (Object)(object)levelPathPoint.Room == (Object)(object)currentRoom)
				{
					float num = Vector3.Distance(((Component)levelPathPoint).transform.position, _target.CheckPosition);
					if (num >= _minDistance && num <= _maxDistance)
					{
						list.Add(levelPathPoint);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	public static bool OnScreen(Vector3 position, float paddWidth, float paddHeight)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		paddWidth = (float)Screen.width * paddWidth;
		paddHeight = (float)Screen.height * paddHeight;
		Vector3 val = CameraUtils.Instance.MainCamera.WorldToScreenPoint(position);
		val.x *= (float)Screen.width / RenderTextureMain.instance.textureWidth;
		val.y *= (float)Screen.height / RenderTextureMain.instance.textureHeight;
		if (val.z > 0f && val.x > 0f - paddWidth && val.x < (float)Screen.width + paddWidth && val.y > 0f - paddHeight && val.y < (float)Screen.height + paddHeight)
		{
			return true;
		}
		return false;
	}

	public static Quaternion ClampRotation(Quaternion _quaternion, Vector3 _bounds)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		_quaternion.x /= _quaternion.w;
		_quaternion.y /= _quaternion.w;
		_quaternion.z /= _quaternion.w;
		_quaternion.w = 1f;
		float num = 114.59156f * Mathf.Atan(_quaternion.x);
		num = Mathf.Clamp(num, 0f - _bounds.x, _bounds.x);
		_quaternion.x = Mathf.Tan(MathF.PI / 360f * num);
		float num2 = 114.59156f * Mathf.Atan(_quaternion.y);
		num2 = Mathf.Clamp(num2, 0f - _bounds.y, _bounds.y);
		_quaternion.y = Mathf.Tan(MathF.PI / 360f * num2);
		float num3 = 114.59156f * Mathf.Atan(_quaternion.z);
		num3 = Mathf.Clamp(num3, 0f - _bounds.z, _bounds.z);
		_quaternion.z = Mathf.Tan(MathF.PI / 360f * num3);
		return ((Quaternion)(ref _quaternion)).normalized;
	}

	public static Vector3 ClampDirection(Vector3 _direction, Vector3 _forward, float _maxAngle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = _direction;
		if (Vector3.Angle(_direction, _forward) > _maxAngle)
		{
			Vector3 val = Vector3.Cross(_forward, _direction);
			result = Quaternion.AngleAxis(_maxAngle, val) * _forward;
		}
		return result;
	}

	public static List<PlayerAvatar> PlayerGetList()
	{
		return GameDirector.instance.PlayerList;
	}

	public static int PhotonViewIDPlayerAvatarLocal()
	{
		return ((Component)PlayerAvatar.instance).GetComponent<PhotonView>().ViewID;
	}

	public static string EmojiText(string inputText)
	{
		inputText = inputText.Replace("{", "<sprite name=");
		inputText = inputText.Replace("}", ">");
		return inputText;
	}

	public static string DollarGetString(int value)
	{
		return value.ToString("#,0", new CultureInfo("de-DE"));
	}

	public static PhysicMaterial PhysicMaterialSticky()
	{
		return AssetManager.instance.physicMaterialStickyExtreme;
	}

	public static PhysicMaterial PhysicMaterialSlippery()
	{
		return AssetManager.instance.physicMaterialSlipperyExtreme;
	}

	public static PhysicMaterial PhysicMaterialSlipperyPlus()
	{
		return AssetManager.instance.physicMaterialSlipperyPlus;
	}

	public static PhysicMaterial PhysicMaterialDefault()
	{
		return AssetManager.instance.physicMaterialDefault;
	}

	public static PhysicMaterial PhysicMaterialPhysGrabObject()
	{
		return AssetManager.instance.physicMaterialPhysGrabObject;
	}

	public static int RunGetLevelsMax()
	{
		return RunManager.instance.levelsMax;
	}

	public static int RunGetLevelsCompleted()
	{
		return RunManager.instance.levelsCompleted;
	}

	public static float RunGetDifficultyMultiplier()
	{
		return (float)RunManager.instance.levelsCompleted / (float)RunManager.instance.levelsMax;
	}

	public static bool PhysGrabObjectIsGrabbed(PhysGrabObject physGrabObject)
	{
		return physGrabObject.grabbed;
	}

	public static List<PhysGrabber> PhysGrabObjectGetPhysGrabbersGrabbing(PhysGrabObject physGrabObject)
	{
		return physGrabObject.playerGrabbing;
	}

	public static List<PlayerAvatar> PhysGrabObjectGetPlayerAvatarsGrabbing(PhysGrabObject physGrabObject)
	{
		List<PlayerAvatar> list = new List<PlayerAvatar>();
		foreach (PhysGrabber item in physGrabObject.playerGrabbing)
		{
			list.Add(item.playerAvatar);
		}
		return list;
	}

	public static bool PhysGrabberLocalIsGrabbing()
	{
		return PhysGrabber.instance.grabbed;
	}

	public static void PhysGrabberLocalForceDrop()
	{
		if (PhysGrabber.instance.grabbed)
		{
			PhysGrabber.instance.OverrideGrabRelease();
		}
	}

	public static void PhysGrabberLocalForceGrab(PhysGrabObject physGrabObject)
	{
		PhysGrabber.instance.OverrideGrab(physGrabObject);
	}

	public static PhysGrabObject PhysGrabberLocalGetGrabbedPhysGrabObject()
	{
		if (!PhysGrabber.instance.grabbed)
		{
			return null;
		}
		return ((Component)PhysGrabber.instance.grabbedObject).GetComponent<PhysGrabObject>();
	}

	public static bool PhysGrabberIsGrabbing(PhysGrabber physGrabber)
	{
		return physGrabber.grabbed;
	}

	public static PhysGrabObject PhysGrabberGetGrabbedPhysGrabObject(PhysGrabber physGrabber)
	{
		if (!physGrabber.grabbed)
		{
			return null;
		}
		return ((Component)physGrabber.grabbedObject).GetComponent<PhysGrabObject>();
	}

	public static void PhysGrabberForceDrop(PhysGrabber physGrabber)
	{
		if (physGrabber.grabbed)
		{
			physGrabber.OverrideGrabRelease();
		}
	}

	public static void PhysGrabberForceGrab(PhysGrabber physGrabber, PhysGrabObject physGrabObject)
	{
		physGrabber.OverrideGrab(physGrabObject);
	}

	public static void PhysGrabberLocalChangeAlpha(float alpha)
	{
		PhysGrabber.instance.ChangeBeamAlpha(alpha);
	}

	public static void LightManagerSetCullTargetTransform(Transform target)
	{
		LightManager.instance.lightCullTarget = target;
		LightManager.instance.UpdateInstant();
	}

	public static string MenuGetSelectableID(GameObject gameObject)
	{
		return "" + ((Object)gameObject).GetInstanceID();
	}

	public static void MenuSelectionBoxTargetSet(MenuPage parentPage, RectTransform rectTransform, Vector2 customOffset = default(Vector2), Vector2 customScale = default(Vector2))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = UIGetRectTransformPositionOnScreen(rectTransform, withScreenMultiplier: false);
		Rect rect = rectTransform.rect;
		float width = ((Rect)(ref rect)).width;
		rect = rectTransform.rect;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(width, ((Rect)(ref rect)).height);
		val += new Vector2(val2.x / 2f, val2.y / 2f) + customOffset;
		MenuSelectableElement component = ((Component)rectTransform).GetComponent<MenuSelectableElement>();
		MenuSelectionBox menuSelectionBox = parentPage.selectionBox;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(0f, 0f);
		bool isInScrollBox = false;
		if (Object.op_Implicit((Object)(object)component) && component.isInScrollBox)
		{
			isInScrollBox = true;
			menuSelectionBox = component.menuScrollBox.menuSelectionBox;
			Transform parent = ((Transform)rectTransform).parent;
			int num = 30;
			while (Object.op_Implicit((Object)(object)parent) && !Object.op_Implicit((Object)(object)((Component)parent).GetComponent<MenuPage>()))
			{
				RectTransform component2 = ((Component)parent).GetComponent<RectTransform>();
				if (Object.op_Implicit((Object)(object)component2) && !Object.op_Implicit((Object)(object)((Component)component2).GetComponent<MenuSelectableElement>()))
				{
					val3 -= new Vector2(((Transform)component2).localPosition.x, ((Transform)component2).localPosition.y);
				}
				parent = parent.parent;
				num--;
				if (num <= 0)
				{
					Debug.LogError((object)(((Object)rectTransform).name + " - Hover FAIL! Could not find a parent page "));
					break;
				}
			}
		}
		val += val3;
		MenuElementAnimations componentInParent = ((Component)rectTransform).GetComponentInParent<MenuElementAnimations>();
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			_ = (float)Screen.width / (float)MenuManager.instance.screenUIWidth;
			_ = (float)Screen.height / (float)MenuManager.instance.screenUIHeight;
			((Component)componentInParent).GetComponent<RectTransform>();
		}
		menuSelectionBox.MenuSelectionBoxSetTarget(Vector2.op_Implicit(val), Vector2.op_Implicit(val2), component.parentPage, isInScrollBox, component.menuScrollBox, customScale);
	}

	public static float MenuGetPitchFromYPos(RectTransform rectTransform)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Lerp(0.5f, 2f, ((Transform)rectTransform).localPosition.y / (float)Screen.height);
	}

	public static Vector2 UIPositionToUIPosition(Vector3 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CameraOverlay.instance.overlayCamera.ScreenToViewportPoint(position) * UIMulti();
		val.x *= 1.015f;
		val.y *= 1.015f;
		val.x /= (float)Screen.width;
		val.y /= (float)Screen.height;
		float num = HUDCanvas.instance.rect.sizeDelta.x / HUDCanvas.instance.rect.sizeDelta.y;
		float num2 = HUDCanvas.instance.rect.sizeDelta.x * num / HUDCanvas.instance.rect.sizeDelta.y;
		val.x *= HUDCanvas.instance.rect.sizeDelta.x * num2;
		val.y *= HUDCanvas.instance.rect.sizeDelta.y * num;
		val.x -= 18f;
		val.y -= 15f;
		return new Vector2(val.x, val.y);
	}

	public static Vector2 UIMousePosToUIPos()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CameraOverlay.instance.overlayCamera.ScreenToViewportPoint(Input.mousePosition) * UIMulti();
		val.x *= 1.015f;
		val.y *= 1.015f;
		val.x /= (float)Screen.width;
		val.y /= (float)Screen.height;
		float num = HUDCanvas.instance.rect.sizeDelta.x / HUDCanvas.instance.rect.sizeDelta.y;
		float num2 = HUDCanvas.instance.rect.sizeDelta.x * num / HUDCanvas.instance.rect.sizeDelta.y;
		val.x *= HUDCanvas.instance.rect.sizeDelta.x * num2;
		val.y *= HUDCanvas.instance.rect.sizeDelta.y * num;
		return new Vector2(val.x, val.y);
	}

	public static Vector2 UIGetRectTransformPositionOnScreen(RectTransform rectTransform, bool withScreenMultiplier = true)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		int num = 1;
		int num2 = 1;
		Vector3 position = ((Transform)rectTransform).position;
		Vector3 position2 = ((Transform)((Component)((Component)rectTransform).GetComponentInParent<MenuPage>()).GetComponent<RectTransform>()).position;
		Vector3 val = position - position2;
		Vector3 val2 = val;
		Rect rect = rectTransform.rect;
		float num3 = ((Rect)(ref rect)).width * rectTransform.pivot.x;
		rect = rectTransform.rect;
		val = val2 - new Vector3(num3, ((Rect)(ref rect)).height * rectTransform.pivot.y, 0f);
		if (withScreenMultiplier)
		{
			val = Vector2.op_Implicit(new Vector2(val.x * (float)num, val.y * (float)num2));
		}
		return Vector2.op_Implicit(val);
	}

	public static Vector2 UIMouseGetLocalPositionWithinRectTransform(RectTransform rectTransform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = UIMousePosToUIPos();
		Vector2 val2 = UIGetRectTransformPositionOnScreen(rectTransform, withScreenMultiplier: false);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(val.x - val2.x, val.y - val2.y);
		Rect rect = rectTransform.rect;
		float num = ((Rect)(ref rect)).width * rectTransform.pivot.x;
		rect = rectTransform.rect;
		float num2 = ((Rect)(ref rect)).height * rectTransform.pivot.y;
		Vector3 lossyScale = ((Transform)rectTransform).lossyScale;
		float num3 = 1f;
		if (lossyScale.y < 1f)
		{
			num3 = 1f + (1f - lossyScale.y);
		}
		if (lossyScale.y > 1f)
		{
			num3 = 1f + (lossyScale.y - 1f);
		}
		((Vector2)(ref val3))._002Ector((val3.x + num) * num3, (val3.y + num2) * num3);
		return val3;
	}

	public static bool UIMouseHover(MenuPage parentPage, RectTransform rectTransform, string menuID, float xPadding = 0f, float yPadding = 0f)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)parentPage.parentPage) && !parentPage.parentPage.pageActive)
		{
			return false;
		}
		Vector2 val = UIMousePosToUIPos();
		if (MenuManager.instance.mouseHoldPosition != Vector2.zero)
		{
			val = MenuManager.instance.mouseHoldPosition;
		}
		int num = 1;
		int num2 = 1;
		MenuScrollBox componentInParent = ((Component)rectTransform).GetComponentInParent<MenuScrollBox>();
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			float num3 = (((Component)componentInParent).transform.position.y - 10f) * (float)num2;
			float num4 = (componentInParent.scrollerEndPosition + 32f) * (float)num2;
			if (val.y > num4 || val.y < num3)
			{
				return false;
			}
		}
		Vector2 val2 = UIGetRectTransformPositionOnScreen(rectTransform, withScreenMultiplier: false);
		float x = val2.x;
		Rect rect = rectTransform.rect;
		float num5 = (x + (((Rect)(ref rect)).xMin - xPadding)) * (float)num;
		float x2 = val2.x;
		rect = rectTransform.rect;
		float num6 = (x2 + (((Rect)(ref rect)).xMax + xPadding)) * (float)num;
		float y = val2.y;
		rect = rectTransform.rect;
		float num7 = (y + (((Rect)(ref rect)).yMin - yPadding)) * (float)num2;
		float y2 = val2.y;
		rect = rectTransform.rect;
		float num8 = (y2 + (((Rect)(ref rect)).yMax + yPadding)) * (float)num2;
		bool flag = false;
		if (val.x >= num5 && val.x <= num6 && val.y >= num7 && val.y <= num8)
		{
			flag = true;
			if (menuID != "-1")
			{
				if (MenuManager.instance.currentMenuID == menuID)
				{
					MenuManager.instance.MenuHover();
				}
				if (MenuManager.instance.currentMenuID == "")
				{
					MenuManager.instance.currentMenuID = menuID;
				}
			}
		}
		else
		{
			flag = false;
			if (menuID != "-1" && MenuManager.instance.currentMenuID == menuID)
			{
				MenuManager.instance.currentMenuID = "";
			}
		}
		if (menuID != "-1")
		{
			if (menuID == MenuManager.instance.currentMenuID)
			{
				return true;
			}
			return false;
		}
		return flag;
	}

	public static void UIHideAim()
	{
		Aim.instance.SetState(Aim.State.Hidden);
	}

	public static void UIHideTumble()
	{
		TumbleUI.instance.Hide();
	}

	public static void UIHideWorldSpace()
	{
		WorldSpaceUIParent.instance.Hide();
	}

	public static void UIHideValuableDiscover()
	{
		ValuableDiscover.instance.Hide();
	}

	public static void UIShowArrow(Vector3 startPosition, Vector3 endPosition, float rotation)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ArrowUI.instance.ArrowShow(startPosition, endPosition, rotation);
	}

	public static void UIShowArrowWorldPosition(Vector3 startPosition, Vector3 endPosition, float rotation)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ArrowUI.instance.ArrowShowWorldPos(startPosition, endPosition, rotation);
	}

	public static void UIBigMessage(string message, string emoji, float size, Color colorMain, Color colorFlash)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		BigMessageUI.instance.BigMessage(message, emoji, size, colorMain, colorFlash);
	}

	public static void UIFocusText(string message, Color colorMain, Color colorFlash, float time = 3f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		MissionUI.instance.MissionText(message, colorMain, colorFlash, time);
	}

	public static void UIItemInfoText(ItemAttributes itemAttributes, string message)
	{
		ItemInfoUI.instance.ItemInfoText(itemAttributes, message);
	}

	public static void UIHideHealth()
	{
		HealthUI.instance.Hide();
	}

	public static void UIHideEnergy()
	{
		EnergyUI.instance.Hide();
	}

	public static void UIHideInventory()
	{
		InventoryUI.instance.Hide();
	}

	public static void UIHideHaul()
	{
		HaulUI.instance.Hide();
	}

	public static void UIHideGoal()
	{
		GoalUI.instance.Hide();
	}

	public static void UIHideCurrency()
	{
		CurrencyUI.instance.Hide();
	}

	public static void UIHideShopCost()
	{
		ShopCostUI.instance.Hide();
	}

	public static void UIShowSpectate()
	{
		if (IsMultiplayer() && Object.op_Implicit((Object)(object)SpectateCamera.instance) && SpectateCamera.instance.CheckState(SpectateCamera.State.Normal) && ((TMP_Text)SpectateNameUI.instance.Text).text != "" && (!Object.op_Implicit((Object)(object)Arena.instance) || Arena.instance.currentState != Arena.States.GameOver))
		{
			SpectateNameUI.instance.Show();
		}
	}

	public static Vector3 UIWorldToCanvasPosition(Vector3 _worldPosition)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rect = HUDCanvas.instance.rect;
		if (OnScreen(_worldPosition, 0.5f, 0.5f))
		{
			Vector3 val = AssetManager.instance.mainCamera.WorldToViewportPoint(_worldPosition);
			((Vector3)(ref val))._002Ector(val.x * rect.sizeDelta.x - rect.sizeDelta.x * 0.5f, val.y * rect.sizeDelta.y - rect.sizeDelta.y * 0.5f, val.z);
			return val;
		}
		return new Vector3((0f - rect.sizeDelta.x) * 2f, (0f - rect.sizeDelta.y) * 2f, 0f);
	}

	public static bool FPSImpulse1()
	{
		return GameDirector.instance.fpsImpulse1;
	}

	public static bool FPSImpulse5()
	{
		return GameDirector.instance.fpsImpulse5;
	}

	public static bool FPSImpulse15()
	{
		return GameDirector.instance.fpsImpulse15;
	}

	public static bool FPSImpulse30()
	{
		return GameDirector.instance.fpsImpulse30;
	}

	public static bool FPSImpulse60()
	{
		return GameDirector.instance.fpsImpulse60;
	}

	public static void LocalPlayerOverrideEnergyUnlimited()
	{
		PlayerController.instance.EnergyCurrent = 100f;
	}

	public static void HUDSpectateSetName(string name)
	{
		SpectateNameUI.instance.SetName(name);
	}

	public static int ValuableGetTotalNumber()
	{
		return ValuableDirector.instance.valuableSpawnAmount;
	}

	public static bool ValuableTrapActivatedDiceRoll(int rarityLevel)
	{
		if (rarityLevel == 1)
		{
			return Random.Range(1, 3) == 1;
		}
		if (rarityLevel == 2)
		{
			return Random.Range(1, 5) == 1;
		}
		if (rarityLevel > 2)
		{
			return Random.Range(1, 10) == 1;
		}
		return false;
	}

	public static LayerMask LayerMaskGetVisionObstruct()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		return LayerMask.op_Implicit(LayerMask.GetMask(new string[6] { "Default", "Player", "PhysGrabObject", "PhysGrabObjectCart", "PhysGrabObjectHinge", "StaticGrabObject" }));
	}

	public static LayerMask LayerMaskGetShouldHits()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		return LayerMask.op_Implicit(LayerMask.GetMask(new string[7] { "Default", "Player", "PhysGrabObject", "PhysGrabObjectCart", "PhysGrabObjectHinge", "StaticGrabObject", "Enemy" }));
	}

	public static LayerMask LayerMaskGetPlayersAndPhysObjects()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return LayerMask.op_Implicit(LayerMask.GetMask(new string[2] { "Player", "PhysGrabObject" }));
	}

	public static LayerMask LayerMaskGetPhysGrabObject()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return LayerMask.op_Implicit(LayerMask.GetMask(new string[4] { "PhysGrabObject", "PhysGrabObjectCart", "PhysGrabObjectHinge", "StaticGrabObject" }));
	}

	public static float BatteryGetChargeRate(int chargeLevel)
	{
		if (chargeLevel == 1)
		{
			return 1f;
		}
		if (chargeLevel == 2)
		{
			return 2f;
		}
		if (chargeLevel >= 3)
		{
			return 5f;
		}
		return 0f;
	}

	public static bool BatteryChargeCondition(ItemBattery battery)
	{
		if (Object.op_Implicit((Object)(object)battery) && ((battery.batteryLife < 100f && battery.batteryActive) || (battery.batteryLife < 99f && !battery.batteryActive)))
		{
			return !battery.isUnchargable;
		}
		return false;
	}

	public static bool InventoryAnyEquipButton()
	{
		if (!InputHold(InputKey.Inventory1) && !InputHold(InputKey.Inventory2))
		{
			return InputHold(InputKey.Inventory3);
		}
		return true;
	}

	public static bool InventoryAnyEquipButtonUp()
	{
		if (!InputUp(InputKey.Inventory1) && !InputUp(InputKey.Inventory2))
		{
			return InputUp(InputKey.Inventory3);
		}
		return true;
	}

	public static bool InventoryAnyEquipButtonDown()
	{
		if (!InputDown(InputKey.Inventory1) && !InputDown(InputKey.Inventory2))
		{
			return InputDown(InputKey.Inventory3);
		}
		return true;
	}

	public static bool LevelGenDone()
	{
		return LevelGenerator.Instance.Generated;
	}

	public static bool RunIsLobbyMenu()
	{
		return (Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelLobbyMenu;
	}

	public static bool RunIsShop()
	{
		return (Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelShop;
	}

	public static bool RunIsLobby()
	{
		return (Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelLobby;
	}

	public static bool RunIsTutorial()
	{
		return (Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelTutorial;
	}

	public static bool RunIsRecording()
	{
		return (Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelRecording;
	}

	public static bool RunIsLevel()
	{
		if ((Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelShop && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelLobby && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelLobbyMenu && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelMainMenu && (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelTutorial)
		{
			return (Object)(object)RunManager.instance.levelCurrent != (Object)(object)RunManager.instance.levelArena;
		}
		return false;
	}

	public static T Singleton<T>(ref T instance, GameObject gameObject) where T : Component
	{
		Debug.Log((object)("Singleton called for type " + typeof(T).Name + " on GameObject " + ((Object)gameObject).name));
		if ((Object)(object)instance == (Object)null)
		{
			Debug.Log((object)("No existing instance found, setting up new instance of " + typeof(T).Name));
			instance = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
			Debug.Log((object)("DontDestroyOnLoad called for " + ((Object)gameObject).name));
			Object.DontDestroyOnLoad((Object)(object)gameObject);
		}
		else if ((Object)(object)((Component)instance).gameObject != (Object)(object)gameObject)
		{
			Debug.Log((object)("Instance already exists for type " + typeof(T).Name + ", destroying game object " + ((Object)gameObject).name));
			Object.Destroy((Object)(object)gameObject);
		}
		else
		{
			Debug.Log((object)("Instance matches the current gameObject " + ((Object)gameObject).name + ", no action needed"));
		}
		Debug.Log((object)("Singleton setup completed for type " + typeof(T).Name + " on GameObject " + ((Object)gameObject).name));
		return instance;
	}

	public static void StatSetBattery(string itemName, int value)
	{
		StatsManager.instance.ItemUpdateStatBattery(itemName, value);
	}

	public static int StatSetRunLives(int value)
	{
		return PunManager.instance.SetRunStatSet("lives", value);
	}

	public static int StatSetRunCurrency(int value)
	{
		return PunManager.instance.SetRunStatSet("currency", value);
	}

	public static int StatSetRunTotalHaul(int value)
	{
		return PunManager.instance.SetRunStatSet("totalHaul", value);
	}

	public static int StatSetRunLevel(int value)
	{
		return PunManager.instance.SetRunStatSet("level", value);
	}

	public static int StatSetSaveLevel(int value)
	{
		return PunManager.instance.SetRunStatSet("save level", value);
	}

	public static int StatSetRunFailures(int value)
	{
		return PunManager.instance.SetRunStatSet("failures", value);
	}

	public static int StatGetItemBattery(string itemName)
	{
		return StatsManager.instance.itemStatBattery[itemName];
	}

	public static int StatGetItemsPurchased(string itemName)
	{
		return StatsManager.instance.itemsPurchased[itemName];
	}

	public static int StatGetRunCurrency()
	{
		return StatsManager.instance.GetRunStatCurrency();
	}

	public static int StatGetRunTotalHaul()
	{
		return StatsManager.instance.GetRunStatTotalHaul();
	}

	public static int StatUpgradeItemBattery(string itemName)
	{
		return PunManager.instance.UpgradeItemBattery(itemName);
	}

	public static void StatSyncAll()
	{
		StatsManager.instance.statsSynced = false;
		PunManager.instance.SyncAllDictionaries();
	}

	public static bool StatsSynced()
	{
		return StatsManager.instance.statsSynced;
	}

	public static void ShopPopulateItemVolumes()
	{
		PunManager.instance.ShopPopulateItemVolumes();
	}

	public static int ShopGetTotalCost()
	{
		return ShopManager.instance.totalCost;
	}

	public static void ShopUpdateCost()
	{
		PunManager.instance.ShopUpdateCost();
	}

	public static void OnLevelGenDone()
	{
		ItemManager.instance.TurnOffIconLightsAgain();
		if (RunIsLobby())
		{
			TutorialDirector.instance.TipsShow();
		}
	}

	public static void OnSceneSwitch(bool _gameOver, bool _leaveGame)
	{
		ItemManager.instance.itemIconLights.SetActive(true);
		if (IsMultiplayer())
		{
			ChatManager.instance.ForceSendMessage(":o");
			ChatManager.instance.ClearAllChatBatches();
		}
		if ((Object)(object)RunManager.instance.levelCurrent == (Object)(object)RunManager.instance.levelLobby)
		{
			TutorialDirector instance = TutorialDirector.instance;
			if (Object.op_Implicit((Object)(object)instance))
			{
				instance.UpdateRoundEnd();
				instance.TipsStore();
			}
		}
		StatsManager.instance.StuffNeedingResetAtTheEndOfAScene();
		TutorialDirector.instance.TipCancel();
		ItemManager.instance.FetchLocalPlayersInventory();
		ItemManager.instance.powerCrystals.Clear();
		if (Object.op_Implicit((Object)(object)ChargingStation.instance) && !_gameOver)
		{
			PunManager.instance.SetRunStatSet("chargingStationCharge", ChargingStation.instance.chargeInt);
		}
		if (IsMasterClientOrSingleplayer() && !_leaveGame && !_gameOver)
		{
			SaveFileSave();
		}
		DataDirector.instance.SaveDeleteCheck(_leaveGame);
		if (!_leaveGame)
		{
			StatSyncAll();
		}
		if (_leaveGame)
		{
			SessionManager.instance.Reset();
		}
	}

	public static PlayerAvatar PlayerAvatarGetFromPhotonID(int photonID)
	{
		PlayerAvatar result = null;
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (player.photonView.ViewID == photonID)
			{
				result = player;
			}
		}
		return result;
	}

	public static PlayerAvatar PlayerAvatarGetFromSteamID(string _steamID)
	{
		PlayerAvatar result = null;
		if (IsMultiplayer())
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (player.steamID == _steamID)
				{
					result = player;
				}
			}
		}
		if (!IsMultiplayer())
		{
			result = PlayerAvatar.instance;
		}
		return result;
	}

	public static PlayerAvatar PlayerAvatarGetFromSteamIDshort(int _steamIDshort)
	{
		PlayerAvatar result = null;
		if (IsMultiplayer())
		{
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (player.steamIDshort == _steamIDshort)
				{
					result = player;
				}
			}
		}
		if (!IsMultiplayer())
		{
			result = PlayerAvatar.instance;
		}
		return result;
	}

	public static PlayerAvatar PlayerAvatarLocal()
	{
		return PlayerAvatar.instance;
	}

	public static string PlayerGetName(PlayerAvatar player)
	{
		if (IsMultiplayer())
		{
			return player.photonView.Owner.NickName;
		}
		return SteamClient.Name;
	}

	public static string PlayerGetSteamID(PlayerAvatar player)
	{
		return player.steamID;
	}

	public static void TruckPopulateItemVolumes()
	{
		PunManager.instance.TruckPopulateItemVolumes();
	}

	public static void LevelSuccessful()
	{
	}

	public static Quaternion SpringQuaternionGet(SpringQuaternion _attributes, Quaternion _targetRotation, float _deltaTime = -1f)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		if (!_attributes.setup)
		{
			_attributes.lastRotation = _targetRotation;
			_attributes.setup = true;
		}
		if (float.IsNaN(_attributes.springVelocity.x))
		{
			_attributes.springVelocity = Vector3.zero;
			_attributes.lastRotation = _targetRotation;
		}
		_targetRotation = Quaternion.RotateTowards(_attributes.lastRotation, _targetRotation, 360f);
		Quaternion val = _targetRotation;
		Quaternion currentX = _attributes.lastRotation * Conjugate(val);
		Vector3 zero = Vector3.zero;
		DampedSpringGeneralSolution(out var _newX, out var _newV, currentX, _attributes.springVelocity - zero, _deltaTime, _attributes.damping, _attributes.speed);
		float magnitude = ((Vector3)(ref _newV)).magnitude;
		if (magnitude * Time.deltaTime > MathF.PI)
		{
			_newV *= MathF.PI / magnitude;
		}
		_attributes.springVelocity = _newV + zero;
		_attributes.lastRotation = _newX * val;
		if (_attributes.clamp && Quaternion.Angle(_attributes.lastRotation, _targetRotation) > _attributes.maxAngle)
		{
			_attributes.lastRotation = Quaternion.RotateTowards(_targetRotation, _attributes.lastRotation, _attributes.maxAngle);
		}
		return _attributes.lastRotation;
	}

	public static float SpringFloatGet(SpringFloat _attributes, float _targetFloat, float _deltaTime = -1f)
	{
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		float currentX = _attributes.lastPosition - _targetFloat;
		DampedSpringGeneralSolution(out var _newX, out var _newV, currentX, _attributes.springVelocity, _deltaTime, _attributes.damping, _attributes.speed);
		float num = _newX;
		_attributes.springVelocity = _newV;
		_attributes.lastPosition = _targetFloat + num;
		if (_attributes.clamp)
		{
			float lastPosition = _attributes.lastPosition;
			_attributes.lastPosition = Mathf.Clamp(_attributes.lastPosition, _attributes.min, _attributes.max);
			if (lastPosition != _attributes.lastPosition)
			{
				_attributes.springVelocity *= -1f;
			}
		}
		return _attributes.lastPosition;
	}

	public static Vector3 SpringVector3Get(SpringVector3 _attributes, Vector3 _targetPosition, float _deltaTime = -1f)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (_deltaTime == -1f)
		{
			_deltaTime = Time.deltaTime;
		}
		Vector3 val = _attributes.lastPosition - _targetPosition;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			DampedSpringGeneralSolution(out var _newX, out var _newV, ((Vector3)(ref val))[i], ((Vector3)(ref _attributes.springVelocity))[i], _deltaTime, _attributes.damping, _attributes.speed);
			((Vector3)(ref zero))[i] = _newX;
			((Vector3)(ref _attributes.springVelocity))[i] = _newV;
		}
		_attributes.lastPosition = _targetPosition + zero;
		if (_attributes.clamp && Vector3.Distance(_attributes.lastPosition, _targetPosition) > _attributes.maxDistance)
		{
			Vector3 val2 = _attributes.lastPosition - _targetPosition;
			_attributes.lastPosition = _targetPosition + ((Vector3)(ref val2)).normalized * _attributes.maxDistance;
		}
		return _targetPosition + zero;
	}

	public static void DampedSpringGeneralSolution(out float _newX, out float _newV, float _currentX, float _currentV, float _time, float _criticality, float _naturalFrequency)
	{
		if (_criticality < 0f)
		{
			_criticality = 0f;
		}
		if (_naturalFrequency <= 0f)
		{
			_naturalFrequency = 1f;
		}
		if (_criticality == 1f)
		{
			float num = _naturalFrequency * _time;
			float num2 = Mathf.Exp(0f - num);
			float num3 = _currentV + _naturalFrequency * _currentX;
			_newX = num2 * (_currentX + num3 * _time);
			_newV = num2 * (num3 * (1f - num) - _naturalFrequency * _currentX);
		}
		else if (_criticality < 1f)
		{
			float num4 = _naturalFrequency * Mathf.Sqrt(1f - _criticality * _criticality);
			float num5 = _criticality * _naturalFrequency;
			float num6 = 1f / num4 * (num5 * _currentX + _currentV);
			float num7 = Mathf.Exp((0f - num5) * _time);
			float num8 = num4 * _time;
			float num9 = Mathf.Cos(num8);
			float num10 = Mathf.Sin(num8);
			_newX = num7 * (_currentX * num9 + num6 * num10);
			_newV = num7 * (num9 * (num6 * num4 - num5 * _currentX) - num10 * (_currentX * num4 + num6 * num5));
		}
		else
		{
			float num11 = Mathf.Sqrt(_criticality * _criticality - 1f);
			float num12 = _naturalFrequency * (num11 - _criticality);
			float num13 = (0f - _naturalFrequency) * (num11 + _criticality);
			float num14 = (num12 * _currentX - _currentV) / (num12 - num13);
			float num15 = _currentX - num14;
			float num16 = Mathf.Exp(num12 * _time);
			float num17 = Mathf.Exp(num13 * _time);
			float num18 = num15 * num16;
			float num19 = num14 * num17;
			_newX = num18 + num19;
			_newV = num12 * num18 + num13 * num19;
		}
	}

	public static void DampedSpringGeneralSolution(out Quaternion _newX, out Vector3 _newV, Quaternion _currentX, Vector3 _currentV, float _time, float _criticality, float _naturalFrequency)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (_criticality < 0f)
		{
			_criticality = 0f;
		}
		if (_naturalFrequency <= 0f)
		{
			_naturalFrequency = 1f;
		}
		if (_criticality == 1f)
		{
			float num = _naturalFrequency * _time;
			float num2 = Mathf.Exp(0f - num);
			Vector3 val = _currentV + ToAngularVelocity(_currentX, 1f / _naturalFrequency);
			_newX = QuaternionScale(ToQuaternionFromAngularVelocityAndTime(val, _time) * _currentX, num2);
			_newV = num2 * (val * (1f - num) - ToAngularVelocity(_currentX, 1f / _naturalFrequency));
		}
		else if (_criticality < 1f)
		{
			float num3 = _naturalFrequency * Mathf.Sqrt(1f - _criticality * _criticality);
			float num4 = _criticality * _naturalFrequency;
			Vector3 val2 = 1f / num3 * (ToAngularVelocity(_currentX, 1f / num4) + _currentV);
			float num5 = Mathf.Exp((0f - num4) * _time);
			float num6 = num3 * _time;
			float num7 = Mathf.Cos(num6);
			float num8 = Mathf.Sin(num6);
			_newX = QuaternionScale(ToQuaternionFromAngularVelocityAndTime(val2, num8) * QuaternionScale(_currentX, num7), num5);
			_newV = num5 * (num7 * (val2 * num3 - ToAngularVelocity(_currentX, 1f / num4)) - num8 * (ToAngularVelocity(_currentX, 1f / num3) + val2 * num4));
		}
		else
		{
			float num9 = Mathf.Sqrt(_criticality * _criticality - 1f);
			float num10 = _naturalFrequency * (num9 - _criticality);
			float num11 = (0f - _naturalFrequency) * (num9 + _criticality);
			Vector3 val3 = (ToAngularVelocity(_currentX, 1f / num10) - _currentV) / (num10 - num11);
			Quaternion val4 = _currentX * Conjugate(ToQuaternionFromAngularVelocityAndTime(val3, 1f));
			float num12 = Mathf.Exp(num10 * _time);
			float num13 = Mathf.Exp(num11 * _time);
			Quaternion val5 = QuaternionScale(val4, num12);
			Vector3 val6 = ToAngularVelocity(val4, 1f / num12);
			Vector3 val7 = val3 * num13;
			Quaternion val8 = ToQuaternionFromAngularVelocityAndTime(val3, num13);
			_newX = val8 * val5;
			_newV = num10 * val6 + num11 * val7;
		}
	}

	public static Vector3 ToAngularVelocity(Quaternion _dQ, float _dT)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ToAngleAndAxis(out var _angleRadians, out var _axis, _dQ);
		return _angleRadians / _dT * _axis;
	}

	public static void ToAngleAndAxis(out float _angleRadians, out Vector3 _axis, Quaternion _Q)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Sqrt(Quaternion.Dot(_Q, _Q));
		_Q.x /= num;
		_Q.y /= num;
		_Q.z /= num;
		_Q.w /= num;
		_axis = new Vector3(_Q.x, _Q.y, _Q.z);
		float magnitude = ((Vector3)(ref _axis)).magnitude;
		if (Mathf.Abs(_Q.w) > 0.99f)
		{
			_angleRadians = 2f * Mathf.Asin(magnitude);
			if (magnitude == 0f)
			{
				_axis = new Vector3(1f, 0f, 0f);
			}
			else
			{
				_axis /= magnitude;
			}
		}
		else
		{
			_angleRadians = 2f * Mathf.Acos(_Q.w);
			_axis /= magnitude;
		}
	}

	public static Quaternion Conjugate(Quaternion q)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static Quaternion QuaternionScale(Quaternion _Q, float _power)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ToAngleAndAxis(out var _angleRadians, out var _axis, _Q);
		return ToQuaternion(_angleRadians * _power, _axis);
	}

	public static Quaternion ToQuaternion(float _angleRadians, Vector3 _axis)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = ((Vector3)(ref _axis)).normalized;
		float num = Mathf.Sin(_angleRadians * 0.5f);
		return new Quaternion(normalized.x * num, normalized.y * num, normalized.z * num, Mathf.Cos(_angleRadians * 0.5f));
	}

	public static Quaternion ToQuaternionFromAngularVelocityAndTime(Vector3 _omega, float _time)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Vector3)(ref _omega)).magnitude * _time;
		if (Mathf.Abs(num) > 1E-15f)
		{
			Vector3 normalized = ((Vector3)(ref _omega)).normalized;
			float num2 = Mathf.Sin(num * 0.5f);
			return new Quaternion(normalized.x * num2, normalized.y * num2, normalized.z * num2, Mathf.Cos(num * 0.5f));
		}
		return Quaternion.identity;
	}

	public static void Log(object message, GameObject gameObject, Color? color = null)
	{
	}

	public static void DoNotLookEffect(GameObject _gameObject, bool _vignette = true, bool _zoom = true, bool _saturation = true, bool _contrast = true, bool _shake = true, bool _glitch = true)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float speedIn = 3f;
		float speedOut = 1f;
		if (_vignette)
		{
			PostProcessing.Instance.VignetteOverride(new Color(0.16f, 0.2f, 0.26f), 0.5f, 1f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_zoom)
		{
			CameraZoom.Instance.OverrideZoomSet(65f, 0.1f, speedIn, speedOut, _gameObject, 150);
		}
		if (_saturation)
		{
			PostProcessing.Instance.SaturationOverride(-25f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_contrast)
		{
			PostProcessing.Instance.ContrastOverride(10f, speedIn, speedOut, 0.1f, _gameObject);
		}
		if (_shake)
		{
			GameDirector.instance.CameraImpact.Shake(15f * Time.deltaTime, 0.1f);
			GameDirector.instance.CameraShake.Shake(15f * Time.deltaTime, 1f);
		}
		if (_glitch)
		{
			CameraGlitch.Instance.DoNotLookEffectSet();
		}
	}

	public static void CameraOverrideStopAim()
	{
		CameraAim.Instance.OverrideAimStop();
	}

	public static ExtractionPoint ExtractionPointGetNearest(Vector3 position)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ExtractionPoint result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject extractionPoint in RoundDirector.instance.extractionPointList)
		{
			float num2 = Vector3.Distance(position, extractionPoint.transform.position);
			if (num2 < num)
			{
				num = num2;
				result = extractionPoint.GetComponent<ExtractionPoint>();
			}
		}
		return result;
	}

	public static ExtractionPoint ExtractionPointGetNearestNotActivated(Vector3 position)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ExtractionPoint result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject extractionPoint in RoundDirector.instance.extractionPointList)
		{
			if (extractionPoint.GetComponent<ExtractionPoint>().currentState == ExtractionPoint.State.Idle)
			{
				float num2 = Vector3.Distance(position, extractionPoint.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = extractionPoint.GetComponent<ExtractionPoint>();
				}
			}
		}
		return result;
	}

	public static float Remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
	{
		float num = Mathf.InverseLerp(origFrom, origTo, value);
		return Mathf.Lerp(targetFrom, targetTo, num);
	}

	public static bool InputDown(InputKey key)
	{
		if (Application.isEditor && (key == InputKey.Back || key == InputKey.Menu))
		{
			key = InputKey.BackEditor;
		}
		return InputManager.instance.KeyDown(key);
	}

	public static bool InputUp(InputKey key)
	{
		return InputManager.instance.KeyUp(key);
	}

	public static bool InputHold(InputKey key)
	{
		return InputManager.instance.KeyHold(key);
	}

	public static Vector2 InputMousePosition()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return InputManager.instance.GetMousePosition();
	}

	public static Vector2 InputMovement()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return InputManager.instance.GetMovement();
	}

	public static float InputMovementX()
	{
		return InputManager.instance.GetMovementX();
	}

	public static float InputMovementY()
	{
		return InputManager.instance.GetMovementY();
	}

	public static float InputScrollY()
	{
		return InputManager.instance.GetScrollY();
	}

	public static float InputMouseX()
	{
		return InputManager.instance.GetMouseX();
	}

	public static float InputMouseY()
	{
		return InputManager.instance.GetMouseY();
	}

	public static void InputDisableMovement()
	{
		InputManager.instance.DisableMovement();
	}

	public static void InputDisableAiming()
	{
		InputManager.instance.DisableAiming();
	}
}
