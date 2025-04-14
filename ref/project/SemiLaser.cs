using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SemiLaser : MonoBehaviour
{
	public enum LaserState
	{
		Intro,
		Active,
		Outro
	}

	public float hurtColliderBeamThickness = 1f;

	public float beamThickness = 1f;

	public float beamHitSize = 1f;

	public float wobbleAmount = 1f;

	public GameObject enableLaser;

	public Transform hitTransform;

	public Transform shootTransform;

	public Transform graceTransform;

	public Light laserSpotLight;

	public Transform hurtColliderRotation;

	public Transform HitParticlesTransform;

	public AudioSource audioSource;

	public AudioSource audioSourceHit;

	public Sound soundLaserStart;

	public Sound soundLaserStartGlobal;

	public Sound soundLaserEnd;

	public Sound soundLaserEndGlobal;

	public Sound soundLaserLoop;

	public Sound soundLaserHitStart;

	public Sound soundLaserHitEnd;

	public Sound soundLaserHitLoop;

	public Sound soundLaserGrace;

	internal LaserState state;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private List<Light> pointLights = new List<Light>();

	private Vector3 startPosition;

	private Vector3 endPosition;

	private List<MeshRenderer> hitMeshRenderers = new List<MeshRenderer>();

	private bool isHitting = true;

	private List<ParticleSystem> hitParticles = new List<ParticleSystem>();

	private List<ParticleSystem> shootParticles = new List<ParticleSystem>();

	private List<ParticleSystem> graceParticles = new List<ParticleSystem>();

	internal HurtCollider hurtCollider;

	private Light hitLight;

	private float hitLightOriginalIntensity;

	private bool isActive;

	private float isActiveTimer;

	private float beamThicknessOriginal;

	private float originalHitLightRange;

	private float laserSpotLightOriginalIntensity;

	private bool hitEnd;

	private bool laserEnd;

	private bool laserStart;

	private Transform audioSourceTransform;

	private Transform audioSourceHitTransform;

	private float graceSoundTimer;

	private Vector3 graceSoundPosition;

	private void Start()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		enableLaser.SetActive(true);
		lineRenderers = ((Component)this).GetComponentsInChildren<LineRenderer>().ToList();
		startPosition = ((Component)this).transform.position;
		endPosition = ((Component)this).transform.position + ((Component)this).transform.forward * 10f;
		pointLights = ((Component)this).GetComponentsInChildren<Light>().ToList();
		pointLights.RemoveAll((Light light) => (int)light.type != 2);
		pointLights.RemoveAll((Light light) => (int)light.shadows > 0);
		hitMeshRenderers = ((Component)hitTransform).GetComponentsInChildren<MeshRenderer>().ToList();
		hitTransform.localScale = Vector3.one * 0.1f;
		((Component)hitTransform).gameObject.SetActive(false);
		hitParticles = ((Component)HitParticlesTransform).GetComponentsInChildren<ParticleSystem>().ToList();
		shootParticles = ((Component)shootTransform).GetComponentsInChildren<ParticleSystem>().ToList();
		hurtCollider = ((Component)this).GetComponentInChildren<HurtCollider>();
		hitLight = ((Component)hitTransform).GetComponentInChildren<Light>();
		hitLightOriginalIntensity = hitLight.intensity;
		graceParticles = ((Component)graceTransform).GetComponentsInChildren<ParticleSystem>().ToList();
		graceTransform.localScale = Vector3.one * beamThickness;
		shootTransform.localScale = Vector3.one * beamThickness;
		hitTransform.localScale = Vector3.one * beamHitSize * beamThickness;
		enableLaser.SetActive(false);
		beamThicknessOriginal = beamThickness;
		originalHitLightRange = hitLight.range;
		audioSourceTransform = ((Component)audioSource).transform;
		audioSourceHitTransform = ((Component)audioSourceHit).transform;
	}

	public void LaserActive(Vector3 _startPosition, Vector3 _endPosition, bool _isHitting)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!enableLaser.activeSelf)
		{
			enableLaser.SetActive(true);
		}
		startPosition = _startPosition;
		endPosition = _endPosition;
		isHitting = _isHitting;
		isActiveTimer = 0.1f;
	}

	private void ActiveTimer()
	{
		if (isActiveTimer <= 0f)
		{
			isActive = false;
			HitParticles(_play: false);
			ShootParticles(_play: false);
		}
		if (isActiveTimer > 0f)
		{
			isActive = true;
			isActiveTimer -= Time.fixedDeltaTime;
		}
	}

	private void FixedUpdate()
	{
		ActiveTimer();
	}

	private void LaserActiveIntroOutro()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!isActive)
		{
			beamThickness = Mathf.Lerp(beamThickness, 0f, Time.deltaTime * 10f);
			((Component)hurtCollider).gameObject.SetActive(false);
			if (beamThickness < 0.01f)
			{
				enableLaser.SetActive(false);
				beamThickness = 0f;
			}
			if (!laserEnd)
			{
				soundLaserEnd.Play(audioSourceTransform.position);
				soundLaserEndGlobal.Play(audioSourceTransform.position);
				laserEnd = true;
			}
			laserStart = false;
		}
		else
		{
			laserEnd = false;
			if (!laserStart)
			{
				soundLaserStart.Play(audioSourceTransform.position);
				soundLaserStartGlobal.Play(audioSourceTransform.position);
				laserStart = true;
			}
			beamThickness = Mathf.Lerp(beamThickness, 1f, Time.deltaTime * 10f);
			if (beamThickness > 0.95f)
			{
				((Component)hurtCollider).gameObject.SetActive(true);
			}
		}
	}

	private void LaserPositioning()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = startPosition;
		hitTransform.LookAt(startPosition);
		shootTransform.LookAt(endPosition);
		graceTransform.LookAt(endPosition);
		shootTransform.position = startPosition;
		hitTransform.position = endPosition + hitTransform.forward * 0.3f;
		audioSourceHitTransform.position = hitTransform.position;
		HitParticlesTransform.position = hitTransform.position;
		HitParticlesTransform.LookAt(startPosition);
		((Component)hurtCollider).transform.localScale = new Vector3(hurtColliderBeamThickness, hurtColliderBeamThickness, Vector3.Distance(startPosition, endPosition));
		((Component)hurtCollider).transform.localPosition = new Vector3((0f - ((Component)hurtCollider).transform.localScale.x) / 2f, (0f - ((Component)hurtCollider).transform.localScale.y) / 2f, 0f);
		((Component)hurtColliderRotation).transform.LookAt(endPosition);
		((Component)laserSpotLight).transform.LookAt(endPosition);
		laserSpotLight.range = Vector3.Distance(startPosition, endPosition) * 1.5f;
		laserSpotLight.intensity = laserSpotLightOriginalIntensity * beamThickness;
	}

	private void Update()
	{
		soundLaserLoop.PlayLoop(enableLaser.activeSelf, 2f, 2f);
		soundLaserHitLoop.PlayLoop(isHitting && enableLaser.activeSelf, 2f, 2f);
		if (enableLaser.activeSelf)
		{
			LaserPositioning();
			AudioSourcePositioning();
			LaserEffectGrace();
			LaserEffectLine();
			LaserEffectIsHitting();
			LaserActiveIntroOutro();
		}
	}

	private void LaserEffectGrace()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (graceSoundTimer > 0f)
		{
			graceSoundTimer -= Time.deltaTime;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		RaycastHit[] array = Physics.SphereCastAll(startPosition, hurtColliderBeamThickness, shootTransform.forward, Vector3.Distance(startPosition, endPosition), LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit val = array[i];
			foreach (ParticleSystem graceParticle in graceParticles)
			{
				if (((RaycastHit)(ref val)).point != Vector3.zero && isActive)
				{
					((Component)graceParticle).transform.position = ((RaycastHit)(ref val)).point;
					graceParticle.Emit(3);
					float num = Vector3.Distance(graceSoundPosition, ((RaycastHit)(ref val)).point);
					if (graceSoundTimer <= 0f || num > 1f)
					{
						soundLaserGrace.Play(((RaycastHit)(ref val)).point);
						graceSoundPosition = ((RaycastHit)(ref val)).point;
						graceSoundTimer = 0.15f;
					}
				}
			}
		}
	}

	private void AudioSourcePositioning()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)AudioListenerFollow.instance).transform;
		Vector3 val = endPosition - startPosition;
		float num = Vector3.Dot(transform.position - startPosition, val) / ((Vector3)(ref val)).sqrMagnitude;
		num = Mathf.Clamp01(num);
		Vector3 position = startPosition + val * num;
		audioSourceTransform.position = position;
	}

	private void LaserEffectLine()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.035f * wobbleAmount + (beamThicknessOriginal - beamThickness) * 0.02f;
		int num2 = Mathf.CeilToInt(Vector3.Distance(startPosition, endPosition) * 2f);
		foreach (LineRenderer lineRenderer in lineRenderers)
		{
			Vector3[] array = (Vector3[])(object)new Vector3[num2];
			for (int i = 0; i < num2; i++)
			{
				float num3 = (float)i / (float)num2;
				array[i] = Vector3.Lerp(startPosition, endPosition, num3);
				ref Vector3 reference = ref array[i];
				reference += Vector3.right * Mathf.Sin(Time.time * 60f + (float)i) * num;
				ref Vector3 reference2 = ref array[i];
				reference2 += Vector3.up * Mathf.Cos(Time.time * 60f + (float)i) * num;
			}
			((Renderer)lineRenderer).material.mainTextureOffset = new Vector2((0f - Time.time) * 30f, 0f);
			lineRenderer.widthMultiplier = (Mathf.PingPong(Time.time * 60f, 0.4f) + 0.8f) * beamThickness;
			lineRenderer.positionCount = num2;
			lineRenderer.SetPositions(array);
			if (isHitting)
			{
				lineRenderer.endWidth = 0.4f * beamThickness;
			}
			else
			{
				lineRenderer.endWidth = 0f;
			}
		}
		float num4 = 4f;
		float num5 = 4f;
		int count = pointLights.Count;
		float num6 = Vector3.Distance(startPosition, endPosition);
		Vector3 val = endPosition - startPosition;
		_ = ((Vector3)(ref val)).normalized;
		int num7 = Mathf.Min(count, Mathf.CeilToInt(num6 / 2f));
		for (int j = 0; j < count; j++)
		{
			if (j < num7)
			{
				int num8 = num7 - 1;
				if (num8 <= 0)
				{
					num8 = 1;
				}
				float num9 = (float)j / (float)num8;
				Vector3 val2 = Vector3.Lerp(startPosition, endPosition, num9);
				((Component)pointLights[j]).transform.position = Vector3.Lerp(((Component)pointLights[j]).transform.position, val2, Time.deltaTime * 20f);
				if (!((Behaviour)pointLights[j]).enabled)
				{
					((Component)pointLights[j]).transform.position = val2;
					((Behaviour)pointLights[j]).enabled = true;
					pointLights[j].range = 0f;
				}
				pointLights[j].range = Mathf.Lerp(pointLights[j].range, num5, Time.deltaTime * 10f) * beamThickness;
				pointLights[j].intensity = Mathf.PingPong(Time.time * 20f, 2f) + num4;
			}
			else
			{
				pointLights[j].range = Mathf.Lerp(pointLights[j].range, 0f, Time.deltaTime * 8f);
				if (pointLights[j].range < 0.05f)
				{
					((Behaviour)pointLights[j]).enabled = false;
				}
			}
		}
	}

	private void HitParticles(bool _play)
	{
		if (!isActive)
		{
			_play = false;
		}
		foreach (ParticleSystem hitParticle in hitParticles)
		{
			if (_play)
			{
				hitParticle.Play();
			}
			else
			{
				hitParticle.Stop();
			}
		}
	}

	private void ShootParticles(bool _play)
	{
		if (!isActive)
		{
			_play = false;
		}
		foreach (ParticleSystem shootParticle in shootParticles)
		{
			if (_play)
			{
				shootParticle.Play();
			}
			else
			{
				shootParticle.Stop();
			}
		}
	}

	private void LaserEffectIsHitting()
	{
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		if (isHitting)
		{
			HitParticles(_play: true);
			ShootParticles(_play: true);
			if (!((Component)hitTransform).gameObject.activeSelf)
			{
				((Component)hitTransform).gameObject.SetActive(true);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, hitTransform.position, 0.1f);
				GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, hitTransform.position, 0.1f);
				hitTransform.localScale = Vector3.zero;
				hitLight.intensity = 0f;
				soundLaserHitStart.Play(hitTransform.position);
				hitEnd = false;
			}
			GameDirector.instance.CameraShake.ShakeDistance(8f, 0f, 6f, hitTransform.position, 0.1f);
			hitTransform.localScale = Vector3.Lerp(hitTransform.localScale, Vector3.one, Time.deltaTime * 40f) * beamHitSize * beamThickness;
			hitLight.intensity = Mathf.Lerp(hitLight.intensity, hitLightOriginalIntensity, Time.deltaTime * 40f) * beamThickness;
			Light obj = hitLight;
			obj.intensity += Mathf.Sin(Time.time * 40f) * 0.5f * beamThickness;
			hitLight.range = originalHitLightRange * beamThickness;
			int num = 0;
			float num2 = 0.15f;
			float num3 = 60f;
			{
				Vector3 val = default(Vector3);
				foreach (MeshRenderer hitMeshRenderer in hitMeshRenderers)
				{
					float num4 = 1.5f;
					num4 = ((num != 0) ? 1.55f : 0.85f);
					((Vector3)(ref val))._002Ector(Mathf.Sin(Time.time * num3) * num2 + 1f, Mathf.Cos(Time.time * num3) * num2 + 1f, Mathf.Sin(Time.time * num3) * num2 + 1f);
					((Component)hitMeshRenderer).transform.localScale = new Vector3(val.x * num4, val.y * num4, val.z * num4);
					((Renderer)hitMeshRenderer).material.mainTextureOffset = new Vector2(Time.time * 10f, 0f);
					((Renderer)hitMeshRenderer).material.mainTextureScale = new Vector2(Mathf.Sin(Time.time * 20f) * 0.4f + 1f, Mathf.Cos(Time.time * 10f) * 0.4f + 1f);
					num++;
				}
				return;
			}
		}
		if (!hitEnd)
		{
			soundLaserHitEnd.Play(hitTransform.position);
			hitEnd = true;
		}
		hitTransform.localScale = Vector3.Lerp(hitTransform.localScale, Vector3.zero, Time.deltaTime * 40f) * beamHitSize * beamThickness;
		hitLight.intensity = Mathf.Lerp(hitLight.intensity, 0f, Time.deltaTime * 40f) / beamThickness;
		if (hitTransform.localScale.x < 0.01f)
		{
			HitParticles(_play: false);
			ShootParticles(_play: false);
			((Component)hitTransform).gameObject.SetActive(false);
		}
	}
}
