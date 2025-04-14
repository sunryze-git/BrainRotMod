using System.Collections;
using UnityEngine;

public class PlayerCollisionGrounded : MonoBehaviour
{
	public static PlayerCollisionGrounded instance;

	public PlayerCollisionController CollisionController;

	internal bool Grounded;

	private float GroundedTimer;

	public LayerMask LayerMask;

	private SphereCollider Collider;

	[HideInInspector]
	public bool physRiding;

	[HideInInspector]
	public int physRidingID;

	[HideInInspector]
	public Vector3 physRidingPosition;

	private bool colliderCheckActive;

	private void Awake()
	{
		instance = this;
		Collider = ((Component)this).GetComponent<SphereCollider>();
	}

	private void Start()
	{
		ColliderCheckActivate();
	}

	private void OnEnable()
	{
		ColliderCheckActivate();
	}

	private void OnDisable()
	{
		colliderCheckActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private void ColliderCheckActivate()
	{
		if (!colliderCheckActive)
		{
			colliderCheckActive = true;
			((MonoBehaviour)this).StartCoroutine(ColliderCheck());
		}
	}

	private IEnumerator ColliderCheck()
	{
		while (true)
		{
			GroundedTimer -= 1f * Time.deltaTime;
			physRiding = false;
			if (CollisionController.GroundedDisableTimer <= 0f)
			{
				Collider[] array = Physics.OverlapSphere(((Component)this).transform.position, Collider.radius, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)1);
				if (array.Length != 0)
				{
					int num = 0;
					if (LevelGenerator.Instance.Generated)
					{
						Collider[] array2 = array;
						foreach (Collider val in array2)
						{
							if (!((Component)val).gameObject.CompareTag("Phys Grab Object"))
							{
								continue;
							}
							PhysGrabObject physGrabObject = ((Component)val).gameObject.GetComponent<PhysGrabObject>();
							if (!Object.op_Implicit((Object)(object)physGrabObject))
							{
								physGrabObject = ((Component)val).gameObject.GetComponentInParent<PhysGrabObject>();
							}
							if (Object.op_Implicit((Object)(object)physGrabObject))
							{
								if (!PlayerController.instance.JumpGroundedObjects.Contains(physGrabObject))
								{
									PlayerController.instance.JumpGroundedObjects.Add(physGrabObject);
								}
								if (Object.op_Implicit((Object)(object)((Component)physGrabObject).GetComponent<PlayerTumble>()))
								{
									num++;
								}
								else if (((Vector3)(ref physGrabObject.roomVolumeCheck.currentSize)).magnitude > 1f)
								{
									physRiding = true;
									physRidingID = physGrabObject.photonView.ViewID;
									physRidingPosition = ((Component)physGrabObject.photonView).transform.InverseTransformPoint(((Component)PlayerController.instance).transform.position);
								}
							}
						}
					}
					if (num != array.Length)
					{
						GroundedTimer = 0.1f;
						Grounded = true;
					}
				}
			}
			if (GroundedTimer < 0f)
			{
				Grounded = false;
			}
			yield return null;
		}
	}

	private void Update()
	{
		CollisionController.Grounded = Grounded;
	}
}
