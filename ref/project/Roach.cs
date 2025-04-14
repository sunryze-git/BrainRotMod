using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Roach : MonoBehaviour
{
	[Space]
	[Header("Orbit Parameters")]
	public float minOrbitDistance = 1f;

	public float maxOrbitDistance = 5f;

	public float minOrbitSpeed = 1f;

	public float maxOrbitSpeed = 5f;

	public float orbitWiggleFrequency = 1f;

	public float speedWiggleFrequency = 1f;

	[Space]
	[Header("Roach Parameters")]
	public float minRoachSpeed = 1f;

	public float maxRoachSpeed = 3f;

	public float roachSpeedFluctuationFrequency = 0.5f;

	public float overshootMultiplier = 1.5f;

	public float turnMultiplier = 0.5f;

	[Space]
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	private Vector3 origin;

	private float currentOrbitDistance;

	private float currentOrbitSpeed;

	private float angle;

	private float roachSpeedTarget;

	private float currentRoachSpeed;

	private Vector3 targetPosition;

	private Vector3 velocity;

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		origin = ((Component)this).transform.position;
		roachSpeedTarget = Random.Range(minRoachSpeed, maxRoachSpeed);
		targetPosition = GetOrbitPoint(angle);
		((Component)this).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		((Component)this).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		((Component)this).transform.rotation = Quaternion.identity;
	}

	private void Update()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		currentOrbitDistance = Mathf.Lerp(minOrbitDistance, maxOrbitDistance, (Mathf.Sin(Time.time * orbitWiggleFrequency) + 1f) * 0.5f);
		currentOrbitSpeed = Mathf.Lerp(minOrbitSpeed, maxOrbitSpeed, (Mathf.Sin(Time.time * speedWiggleFrequency) + 1f) * 0.5f);
		angle += Time.deltaTime * currentOrbitSpeed;
		Vector3 orbitPoint = GetOrbitPoint(angle);
		Vector3 val;
		if (Vector3.Distance(((Component)this).transform.position, targetPosition) < 0.1f)
		{
			val = orbitPoint - targetPosition;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			targetPosition = orbitPoint + normalized * overshootMultiplier;
		}
		currentRoachSpeed = Mathf.Lerp(currentRoachSpeed, roachSpeedTarget, Time.deltaTime * roachSpeedFluctuationFrequency);
		if (Mathf.Abs(currentRoachSpeed - roachSpeedTarget) < 0.1f)
		{
			roachSpeedTarget = Random.Range(minRoachSpeed, maxRoachSpeed);
		}
		val = targetPosition - ((Component)this).transform.position;
		Vector3 val2 = (((Vector3)(ref val)).normalized * currentRoachSpeed - velocity) * turnMultiplier;
		velocity += val2 * Time.deltaTime;
		Transform transform = ((Component)this).transform;
		transform.position += velocity * Time.deltaTime;
		if (velocity != Vector3.zero)
		{
			((Component)this).transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
		}
	}

	private Vector3 GetOrbitPoint(float angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return origin + new Vector3(Mathf.Sin(angle) * currentOrbitDistance, 0f, Mathf.Cos(angle) * currentOrbitDistance);
	}
}
