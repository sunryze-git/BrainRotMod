using UnityEngine;

public class TrapColliderScript : MonoBehaviour
{
	[HideInInspector]
	public bool TrapCollision;

	[HideInInspector]
	public float TrapCollisionForce;

	public PlayerAvatar triggerPlayer;

	private void OnTriggerEnter(Collider other)
	{
		PlayerTrigger component = ((Component)other).GetComponent<PlayerTrigger>();
		if (Object.op_Implicit((Object)(object)component) && !GameDirector.instance.LevelCompleted)
		{
			PlayerAvatar playerAvatar = component.PlayerAvatar;
			if (playerAvatar.isLocal && ((Component)other).gameObject.CompareTag("Player") && !GameDirector.instance.LevelEnemyChasing && TrapDirector.instance.TrapCooldown <= 0f && !PlayerController.instance.Crouching)
			{
				TrapCollision = true;
				triggerPlayer = playerAvatar;
			}
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.2f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = new Color(1f, 0.95f, 0f, 0.5f);
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
}
