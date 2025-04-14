using UnityEngine;

public class ItemMineTrigger : MonoBehaviour
{
	private enum TargetType
	{
		None,
		Enemy,
		RigidBody,
		Player
	}

	private PhysGrabObject parentPhysGrabObject;

	private ItemMine itemMine;

	public bool enemyTrigger;

	private bool targetAcquired;

	private float visionCheckTimer;

	private void Start()
	{
		parentPhysGrabObject = ((Component)this).GetComponentInParent<PhysGrabObject>();
		itemMine = ((Component)this).GetComponentInParent<ItemMine>();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			Object.Destroy((Object)(object)this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!targetAcquired && Object.op_Implicit((Object)(object)itemMine) && itemMine.state == ItemMine.States.Armed && PassesTriggerChecks(other))
		{
			TryAcquireTarget(other);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!targetAcquired && Object.op_Implicit((Object)(object)itemMine) && itemMine.state == ItemMine.States.Armed && PassesTriggerChecks(other))
		{
			visionCheckTimer += Time.deltaTime;
			if (visionCheckTimer > 0.5f)
			{
				visionCheckTimer = 0f;
				TryAcquireTarget(other);
			}
		}
	}

	private bool PassesTriggerChecks(Collider other)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabObject componentInParent = ((Component)other).GetComponentInParent<PhysGrabObject>();
		if (enemyTrigger)
		{
			if (!Object.op_Implicit((Object)(object)componentInParent) || !componentInParent.isEnemy)
			{
				return false;
			}
		}
		else if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.isEnemy && !itemMine.triggeredByEnemies)
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)componentInParent) && !itemMine.triggeredByRigidBodies && !componentInParent.isEnemy && !componentInParent.isPlayer)
		{
			return false;
		}
		PlayerAvatar playerAvatar = ((Component)other).GetComponentInParent<PlayerAvatar>();
		PlayerController componentInParent2 = ((Component)other).GetComponentInParent<PlayerController>();
		if (Object.op_Implicit((Object)(object)componentInParent2))
		{
			playerAvatar = componentInParent2.playerAvatarScript;
		}
		if (Object.op_Implicit((Object)(object)componentInParent) && !itemMine.triggeredByPlayers && (componentInParent.isPlayer || Object.op_Implicit((Object)(object)playerAvatar)))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isEnemy && !componentInParent.grabbed)
		{
			Vector3 val = componentInParent.rb.velocity;
			if (((Vector3)(ref val)).magnitude < 0.1f)
			{
				val = componentInParent.rb.angularVelocity;
				if (((Vector3)(ref val)).magnitude < 0.1f)
				{
					return false;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)(Object.op_Implicit((Object)(object)componentInParent) ? ((Component)componentInParent).GetComponent<PlayerTumble>() : null)) && !itemMine.triggeredByPlayers)
		{
			return false;
		}
		return true;
	}

	private void TryAcquireTarget(Collider other)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (targetAcquired)
		{
			return;
		}
		PhysGrabObject componentInParent = ((Component)other).GetComponentInParent<PhysGrabObject>();
		PlayerAvatar componentInParent2 = ((Component)other).GetComponentInParent<PlayerAvatar>();
		PlayerAccess componentInParent3 = ((Component)other).GetComponentInParent<PlayerAccess>();
		PlayerController playerController = (Object.op_Implicit((Object)(object)componentInParent3) ? ((Component)componentInParent3).GetComponentInChildren<PlayerController>() : null);
		Vector3 position = ((Component)itemMine).transform.position;
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			Vector3 midPoint = componentInParent.midPoint;
			if (!VisionObstruct(position, midPoint, componentInParent))
			{
				if (componentInParent.isEnemy)
				{
					LockOnTarget(TargetType.Enemy, componentInParent, componentInParent2, playerController);
					return;
				}
				if (!componentInParent.isPlayer && (Object)(object)componentInParent != (Object)(object)parentPhysGrabObject)
				{
					LockOnTarget(TargetType.RigidBody, componentInParent, componentInParent2, playerController);
					return;
				}
			}
		}
		if (Object.op_Implicit((Object)(object)componentInParent2))
		{
			Vector3 position2 = componentInParent2.PlayerVisionTarget.VisionTransform.position;
			if (!VisionObstruct(position, position2, null))
			{
				LockOnTarget(TargetType.Player, componentInParent, componentInParent2, playerController);
				return;
			}
		}
		if (!Object.op_Implicit((Object)(object)playerController))
		{
			return;
		}
		componentInParent2 = playerController.playerAvatarScript;
		if (Object.op_Implicit((Object)(object)componentInParent2))
		{
			Vector3 position3 = componentInParent2.PlayerVisionTarget.VisionTransform.position;
			if (!VisionObstruct(position, position3, null))
			{
				LockOnTarget(TargetType.Player, componentInParent, componentInParent2, playerController);
			}
		}
	}

	private void LockOnTarget(TargetType type, PhysGrabObject physObj, PlayerAvatar playerAvatar, PlayerController playerController)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)itemMine))
		{
			return;
		}
		switch (type)
		{
		case TargetType.Enemy:
			itemMine.wasTriggeredByEnemy = true;
			itemMine.triggeredPhysGrabObject = physObj;
			itemMine.triggeredTransform = ((Component)physObj).transform;
			itemMine.triggeredPosition = ((Component)physObj).transform.position;
			break;
		case TargetType.RigidBody:
			itemMine.wasTriggeredByRigidBody = true;
			itemMine.triggeredPhysGrabObject = physObj;
			itemMine.triggeredTransform = ((Component)physObj).transform;
			itemMine.triggeredPosition = ((Component)physObj).transform.position;
			break;
		case TargetType.Player:
			itemMine.wasTriggeredByPlayer = true;
			if (Object.op_Implicit((Object)(object)playerAvatar))
			{
				itemMine.triggeredPlayerAvatar = playerAvatar;
				PlayerTumble tumble = playerAvatar.tumble;
				if (Object.op_Implicit((Object)(object)tumble))
				{
					itemMine.triggeredPlayerTumble = tumble;
					itemMine.triggeredPhysGrabObject = tumble.physGrabObject;
				}
				itemMine.triggeredTransform = playerAvatar.PlayerVisionTarget.VisionTransform;
				itemMine.triggeredPosition = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			}
			else if (Object.op_Implicit((Object)(object)physObj))
			{
				PlayerTumble componentInParent = ((Component)physObj).GetComponentInParent<PlayerTumble>();
				if (Object.op_Implicit((Object)(object)componentInParent))
				{
					itemMine.triggeredPlayerAvatar = componentInParent.playerAvatar;
					itemMine.triggeredPlayerTumble = componentInParent;
					itemMine.triggeredPhysGrabObject = componentInParent.physGrabObject;
					itemMine.triggeredTransform = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform;
					itemMine.triggeredPosition = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform.position;
				}
			}
			break;
		}
		targetAcquired = true;
		itemMine.SetTriggered();
	}

	private bool VisionObstruct(Vector3 start, Vector3 end, PhysGrabObject targetPhysObj)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct());
		Vector3 val = end - start;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float num2 = Vector3.Distance(start, end);
		RaycastHit[] array = Physics.RaycastAll(start, normalized, num2, num);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit val2 = array[i];
			if (((Component)((RaycastHit)(ref val2)).collider).CompareTag("Wall") || ((Component)((RaycastHit)(ref val2)).collider).CompareTag("Ceiling"))
			{
				return true;
			}
		}
		return false;
	}
}
