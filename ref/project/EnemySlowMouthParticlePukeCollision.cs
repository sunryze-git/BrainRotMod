using System.Collections.Generic;
using UnityEngine;

public class EnemySlowMouthParticlePukeCollision : MonoBehaviour
{
	public Light pukeLight;

	private ParticleSystem pukeParticles;

	public GameObject hurtCollider;

	private float hurtColliderTimer;

	private Transform parentTransform;

	private Vector3 startPosition;

	public ParticleSystem pukeBubbleParticles;

	public ParticleSystem pukeSplashParticles;

	public ParticleSystem pukeSmokeParticles;

	private void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		pukeParticles = ((Component)this).GetComponent<ParticleSystem>();
		parentTransform = ((Component)this).transform.parent;
		startPosition = ((Component)this).transform.localPosition;
	}

	private void Update()
	{
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if (pukeParticles.isPlaying)
		{
			if (!((Behaviour)pukeLight).enabled)
			{
				((Behaviour)pukeLight).enabled = true;
				pukeLight.intensity = 0f;
			}
			pukeLight.intensity = Mathf.Lerp(pukeLight.intensity, 0.6f, Time.deltaTime * 5f);
			Light obj = pukeLight;
			obj.intensity += Mathf.Sin(Time.time * 40f) * 0.07f;
		}
		else if (((Behaviour)pukeLight).enabled)
		{
			pukeLight.intensity = Mathf.Lerp(pukeLight.intensity, 0f, Time.deltaTime * 1f);
			Light obj2 = pukeLight;
			obj2.intensity += Mathf.Sin(Time.time * 30f) * 0.01f;
			if (pukeLight.intensity < 0.01f)
			{
				((Behaviour)pukeLight).enabled = false;
			}
		}
		if (hurtColliderTimer > 0f)
		{
			hurtColliderTimer -= Time.deltaTime;
			if (hurtColliderTimer <= 0f)
			{
				hurtCollider.SetActive(false);
			}
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		((Component)this).transform.localPosition = startPosition;
		float num = Vector3.Distance(parentTransform.position, ((Component)this).transform.position);
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(parentTransform.position, num * parentTransform.forward, ref val, num, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct())))
		{
			Vector3 point = ((RaycastHit)(ref val)).point;
			if (Vector3.Distance(parentTransform.position, point) < num)
			{
				Vector3 val2 = parentTransform.InverseTransformPoint(point);
				((Component)this).transform.localPosition = new Vector3(0f, 0f, val2.z);
			}
		}
	}

	private void ActivateHurtCollider(Vector3 _direction, Vector3 _position)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		((Component)pukeSmokeParticles).transform.position = _position;
		((Component)pukeSmokeParticles).transform.rotation = Quaternion.LookRotation(_direction);
		pukeSmokeParticles.Emit(1);
		hurtCollider.SetActive(true);
		((Component)pukeBubbleParticles).transform.position = _position;
		((Component)pukeBubbleParticles).transform.rotation = Quaternion.LookRotation(_direction);
		pukeBubbleParticles.Emit(2);
		((Component)pukeSplashParticles).transform.position = _position;
		pukeSplashParticles.Emit(3);
		hurtCollider.transform.rotation = Quaternion.LookRotation(_direction);
		hurtCollider.transform.position = _position;
		hurtColliderTimer = 0.2f;
		_direction.y += 180f;
		((Component)pukeSplashParticles).transform.rotation = Quaternion.LookRotation(_direction);
	}

	private void OnParticleCollision(GameObject other)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		List<ParticleCollisionEvent> list = new List<ParticleCollisionEvent>();
		int collisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(pukeParticles, other, list);
		for (int i = 0; i < collisionEvents; i++)
		{
			ParticleCollisionEvent val = list[i];
			Vector3 intersection = ((ParticleCollisionEvent)(ref val)).intersection;
			Vector3 velocity = ((ParticleCollisionEvent)(ref val)).velocity;
			Vector3 normalized = ((Vector3)(ref velocity)).normalized;
			if (((Vector3)(ref velocity)).magnitude > 3f)
			{
				ActivateHurtCollider(normalized, intersection);
			}
		}
	}
}
