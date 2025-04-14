using UnityEngine;

public class EnemyTriggerAttack : MonoBehaviour
{
	public Enemy Enemy;

	public LayerMask VisionMask;

	public Transform VisionTransform;

	private bool TriggerCheckTimerSet;

	private float TriggerCheckTimer;

	internal bool Attack;

	private void OnTriggerStay(Collider other)
	{
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated || TriggerCheckTimer > 0f)
		{
			return;
		}
		TriggerCheckTimerSet = true;
		if (Enemy.CurrentState == EnemyState.Chase || Enemy.CurrentState == EnemyState.LookUnder)
		{
			PlayerTrigger component = ((Component)other).GetComponent<PlayerTrigger>();
			if (Object.op_Implicit((Object)(object)component))
			{
				bool flag = false;
				if (Enemy.CurrentState == EnemyState.LookUnder && Enemy.StateLookUnder.WaitDone)
				{
					flag = true;
				}
				bool chaseCanReach = Enemy.StateChase.ChaseCanReach;
				PlayerAvatar playerAvatar = component.PlayerAvatar;
				if (playerAvatar.isDisabled || (!Enemy.Vision.VisionTriggered[playerAvatar.photonView.ViewID] && !flag))
				{
					return;
				}
				bool flag2 = true;
				bool flag3 = false;
				if (!chaseCanReach || flag)
				{
					flag2 = false;
					flag3 = true;
				}
				Vector3 position = ((Component)playerAvatar.PlayerVisionTarget.VisionTransform).transform.position;
				Vector3 position2 = VisionTransform.position;
				Vector3 val = position - VisionTransform.position;
				Vector3 val2 = position - VisionTransform.position;
				RaycastHit[] array = Physics.RaycastAll(position2, val, ((Vector3)(ref val2)).magnitude, LayerMask.op_Implicit(VisionMask));
				bool flag4 = false;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit val3 = array2[i];
					if (!((Component)((RaycastHit)(ref val3)).transform).CompareTag("Enemy") && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponent<PlayerTumble>()))
					{
						flag4 = true;
					}
				}
				if (flag4)
				{
					if (!flag3)
					{
						flag2 = false;
					}
				}
				else if (flag3)
				{
					flag2 = true;
				}
				if (flag2)
				{
					Attack = true;
				}
			}
		}
		if (Enemy.CurrentState == EnemyState.ChaseBegin)
		{
			return;
		}
		bool flag5 = false;
		int num = 0;
		Vector3 val4 = Vector3.zero;
		PhysGrabObject componentInParent = ((Component)other).GetComponentInParent<PhysGrabObject>();
		StaticGrabObject componentInParent2 = ((Component)other).GetComponentInParent<StaticGrabObject>();
		if (Object.op_Implicit((Object)(object)componentInParent))
		{
			flag5 = true;
			num = componentInParent.playerGrabbing.Count;
			val4 = componentInParent.midPoint;
			if (Object.op_Implicit((Object)(object)((Component)componentInParent).GetComponent<EnemyRigidbody>()))
			{
				flag5 = false;
			}
		}
		else if (Object.op_Implicit((Object)(object)componentInParent2))
		{
			flag5 = true;
			num = componentInParent2.playerGrabbing.Count;
			val4 = ((Component)componentInParent2).transform.position;
		}
		if (!flag5 || num <= 0 || !(Vector3.Distance(((Component)this).transform.position, val4) < Enemy.Vision.VisionDistance))
		{
			return;
		}
		Vector3 val5 = val4 - VisionTransform.position;
		if (!(Vector3.Dot(VisionTransform.forward, ((Vector3)(ref val5)).normalized) > 0.8f))
		{
			return;
		}
		RaycastHit val6 = default(RaycastHit);
		bool num2 = Physics.Raycast(Enemy.Vision.VisionTransform.position, val5, ref val6, ((Vector3)(ref val5)).magnitude, LayerMask.op_Implicit(VisionMask));
		bool flag6 = true;
		if (num2)
		{
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				if ((Object)(object)((Component)((RaycastHit)(ref val6)).collider).GetComponentInParent<PhysGrabObject>() != (Object)(object)componentInParent)
				{
					flag6 = false;
				}
			}
			else if (Object.op_Implicit((Object)(object)componentInParent2) && (Object)(object)((Component)((RaycastHit)(ref val6)).collider).GetComponentInParent<StaticGrabObject>() != (Object)(object)componentInParent2)
			{
				flag6 = false;
			}
		}
		if (flag6 && Enemy.HasStateInvestigate)
		{
			Enemy.StateInvestigate.Set(val4);
		}
	}

	private void Update()
	{
		if (TriggerCheckTimerSet)
		{
			TriggerCheckTimer = 0.2f;
			TriggerCheckTimerSet = false;
		}
		else if (TriggerCheckTimer > 0f)
		{
			TriggerCheckTimer -= Time.deltaTime;
		}
	}
}
