using System.Collections;
using UnityEngine;

public class EnemyGrounded : MonoBehaviour
{
	public Enemy enemy;

	internal bool grounded;

	public BoxCollider boxCollider;

	private bool logicActive;

	private float groundedDisableTimer;

	private void Awake()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		enemy.Grounded = this;
		enemy.HasGrounded = true;
		if (!((Collider)boxCollider).isTrigger)
		{
			Debug.LogError((object)("EnemyGrounded: Collider is not a trigger on " + ((Object)enemy.EnemyParent).name));
		}
		if (((Component)boxCollider).transform.localScale != Vector3.one)
		{
			Debug.LogError((object)("EnemyGrounded: Scale is not 1 on " + ((Object)enemy.EnemyParent).name));
		}
		if (((Component)boxCollider).transform.localPosition != Vector3.zero)
		{
			Debug.LogError((object)("EnemyGrounded: Position is not 0 on " + ((Object)enemy.EnemyParent).name));
		}
		((MonoBehaviour)this).StartCoroutine(ColliderCheck());
	}

	private void OnDisable()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		logicActive = false;
	}

	private void OnEnable()
	{
		if (!logicActive)
		{
			((MonoBehaviour)this).StartCoroutine(ColliderCheck());
		}
	}

	private IEnumerator ColliderCheck()
	{
		logicActive = true;
		yield return (object)new WaitForSeconds(0.1f);
		while (true)
		{
			grounded = false;
			Vector3 val = ((Component)boxCollider).transform.TransformVector(boxCollider.size * 0.5f);
			val.x = Mathf.Abs(val.x);
			val.y = Mathf.Abs(val.y);
			val.z = Mathf.Abs(val.z);
			Bounds bounds = ((Collider)boxCollider).bounds;
			Collider[] array = Physics.OverlapBox(((Bounds)(ref bounds)).center, val, ((Component)boxCollider).transform.rotation, LayerMask.GetMask(new string[4] { "Default", "PhysGrabObject", "PhysGrabObjectHinge", "PhysGrabObjectCart" }), (QueryTriggerInteraction)1);
			if (array.Length != 0)
			{
				Collider[] array2 = array;
				foreach (Collider val2 in array2)
				{
					if (Object.op_Implicit((Object)(object)((Component)val2).GetComponentInParent<EnemyRigidbody>()))
					{
						continue;
					}
					if (enemy.HasJump && enemy.Jump.surfaceJump)
					{
						EnemyJumpSurface component = ((Component)val2).GetComponent<EnemyJumpSurface>();
						if (Object.op_Implicit((Object)(object)component))
						{
							Vector3 val3 = ((Component)enemy).transform.forward;
							if (enemy.HasRigidbody)
							{
								val3 = ((Component)enemy).transform.position - ((Component)enemy.Rigidbody).transform.position;
							}
							if (Vector3.Dot(((Component)component).transform.TransformDirection(component.jumpDirection), val3) > 0.5f)
							{
								enemy.Jump.SurfaceJumpTrigger(((Component)component).transform.TransformDirection(component.jumpDirection));
							}
						}
					}
					if (groundedDisableTimer <= 0f)
					{
						grounded = true;
					}
				}
			}
			if (enemy.HasJump && Object.op_Implicit((Object)(object)enemy.Jump))
			{
				groundedDisableTimer -= 0.05f;
				yield return (object)new WaitForSeconds(0.05f);
			}
			else
			{
				groundedDisableTimer -= 0.25f;
				yield return (object)new WaitForSeconds(0.25f);
			}
		}
	}

	public void GroundedDisable(float _time)
	{
		groundedDisableTimer = _time;
	}
}
