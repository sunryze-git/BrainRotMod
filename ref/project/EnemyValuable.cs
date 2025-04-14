using System.Collections.Generic;
using UnityEngine;

public class EnemyValuable : MonoBehaviour
{
	private PhysGrabObjectImpactDetector impactDetector;

	private float indestructibleTimer = 5f;

	public MeshRenderer outerMeshRenderer;

	private Material outerMaterial;

	private int fresnelPowerIndex;

	private int fresnelColorIndex;

	[Space]
	private float fresnelPowerDefault;

	public float fresnelPowerIndestructible;

	[Space]
	private Color fresnelColorDefault;

	public Color fresnelColorIndestructible;

	[Space]
	public AnimationCurve indestructibleCurve;

	private float indestructibleLerp;

	[Space]
	public Transform innerTransform;

	public Transform speckSmallTransform;

	public Transform speckBigTransform;

	[Space]
	public List<ParticleSystem> particleSystems;

	private void Start()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		impactDetector = ((Component)this).GetComponentInChildren<PhysGrabObjectImpactDetector>();
		impactDetector.indestructibleSpawnTimer = 0.1f;
		outerMaterial = ((Renderer)outerMeshRenderer).material;
		fresnelPowerIndex = Shader.PropertyToID("_FresnelPower");
		fresnelColorIndex = Shader.PropertyToID("_FresnelColor");
		fresnelPowerDefault = outerMaterial.GetFloat(fresnelPowerIndex);
		outerMaterial.SetFloat(fresnelPowerIndex, fresnelPowerIndestructible);
		fresnelColorDefault = outerMaterial.GetColor(fresnelColorIndex);
		outerMaterial.SetColor(fresnelColorIndex, fresnelColorIndestructible);
		EnemyDirector.instance.AddEnemyValuable(this);
	}

	private void Update()
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (indestructibleTimer > 0f)
		{
			indestructibleTimer -= Time.deltaTime;
			if (indestructibleTimer <= 0f)
			{
				impactDetector.destroyDisable = false;
			}
		}
		else if (indestructibleLerp < 1f)
		{
			float num = Mathf.Lerp(fresnelPowerIndestructible, fresnelPowerDefault, indestructibleCurve.Evaluate(indestructibleLerp));
			outerMaterial.SetFloat(fresnelPowerIndex, num);
			Color val = Color.Lerp(fresnelColorIndestructible, fresnelColorDefault, indestructibleCurve.Evaluate(indestructibleLerp));
			outerMaterial.SetColor(fresnelColorIndex, val);
			indestructibleLerp += 2f * Time.deltaTime;
		}
		innerTransform.Rotate(((Component)this).transform.up * 60f * Time.deltaTime);
		speckSmallTransform.Rotate(((Component)this).transform.up * 100f * Time.deltaTime);
		speckBigTransform.Rotate(-((Component)this).transform.up * 20f * Time.deltaTime);
	}

	public void Destroy()
	{
		impactDetector.DestroyObject();
	}

	public void DestroyImpulse()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			((Component)particleSystem).gameObject.SetActive(true);
			((Component)particleSystem).transform.parent = null;
			MainModule main = particleSystem.main;
			((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
		}
	}
}
