using System;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
	public enum Type
	{
		None,
		Wood,
		Rug,
		Tile,
		Stone,
		Catwalk,
		Snow,
		Metal,
		Wetmetal,
		Gravel,
		Grass,
		Water
	}

	public enum SoundType
	{
		Light,
		Medium,
		Heavy
	}

	public enum HostType
	{
		LocalPlayer,
		OtherPlayer,
		Enemy
	}

	[Serializable]
	public class MaterialTrigger
	{
		internal MaterialPreset LastMaterialList;

		internal Type LastMaterialType;

		internal MaterialSlidingLoop SlidingLoopObject;
	}

	public static Materials Instance;

	public LayerMask LayerMask;

	[Space]
	public List<MaterialPreset> MaterialList;

	private MaterialPreset LastMaterialList;

	private void Awake()
	{
		Instance = this;
	}

	public void Impulse(Vector3 origin, Vector3 direction, SoundType soundType, bool footstep, MaterialTrigger materialTrigger, HostType hostType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		Vector3 material = GetMaterial(origin, materialTrigger);
		if (!Object.op_Implicit((Object)(object)LastMaterialList))
		{
			return;
		}
		float volumeMultiplier = 1f;
		float falloffMultiplier = 1f;
		float offscreenVolumeMultiplier = 1f;
		float offscreenFalloffMultiplier = 1f;
		switch (hostType)
		{
		case HostType.OtherPlayer:
			volumeMultiplier = 0.5f;
			break;
		case HostType.Enemy:
			volumeMultiplier = 0.5f;
			falloffMultiplier = 0.5f;
			offscreenVolumeMultiplier = 0.25f;
			offscreenFalloffMultiplier = 0.25f;
			break;
		}
		switch (soundType)
		{
		case SoundType.Light:
			if (footstep)
			{
				if (LastMaterialList.RareFootstepLightMax > 0)
				{
					LastMaterialList.RareFootstepLightCurrent -= 1f;
					if (LastMaterialList.RareFootstepLightCurrent <= 0f)
					{
						LastMaterialList.RareFootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						LastMaterialList.RareFootstepLightCurrent = Random.Range(LastMaterialList.RareFootstepLightMin, LastMaterialList.RareFootstepLightMax);
					}
				}
				LastMaterialList.FootstepLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				break;
			}
			if (LastMaterialList.RareImpactLightMax > 0)
			{
				LastMaterialList.RareImpactLightCurrent -= 1f;
				if (LastMaterialList.RareImpactLightCurrent <= 0f)
				{
					LastMaterialList.RareImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					LastMaterialList.RareImpactLightCurrent = Random.Range(LastMaterialList.RareImpactLightMin, LastMaterialList.RareImpactLightMax);
				}
			}
			LastMaterialList.ImpactLight.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
			break;
		case SoundType.Medium:
			if (footstep)
			{
				if (LastMaterialList.RareFootstepMediumMax > 0)
				{
					LastMaterialList.RareFootstepMediumCurrent -= 1f;
					if (LastMaterialList.RareFootstepMediumCurrent <= 0f)
					{
						LastMaterialList.RareFootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						LastMaterialList.RareFootstepMediumCurrent = Random.Range(LastMaterialList.RareFootstepMediumMin, LastMaterialList.RareFootstepMediumMax);
					}
				}
				LastMaterialList.FootstepMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				break;
			}
			if (LastMaterialList.RareImpactMediumMax > 0)
			{
				LastMaterialList.RareImpactMediumCurrent -= 1f;
				if (LastMaterialList.RareImpactMediumCurrent <= 0f)
				{
					LastMaterialList.RareImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					LastMaterialList.RareImpactMediumCurrent = Random.Range(LastMaterialList.RareImpactMediumMin, LastMaterialList.RareImpactMediumMax);
				}
			}
			LastMaterialList.ImpactMedium.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
			break;
		case SoundType.Heavy:
			if (footstep)
			{
				if (LastMaterialList.RareFootstepHeavyMax > 0)
				{
					LastMaterialList.RareFootstepHeavyCurrent -= 1f;
					if (LastMaterialList.RareFootstepHeavyCurrent <= 0f)
					{
						LastMaterialList.RareFootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
						LastMaterialList.RareFootstepHeavyCurrent = Random.Range(LastMaterialList.RareFootstepHeavyMin, LastMaterialList.RareFootstepHeavyMax);
					}
				}
				LastMaterialList.FootstepHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
				break;
			}
			if (LastMaterialList.RareImpactHeavyMax > 0)
			{
				LastMaterialList.RareImpactHeavyCurrent -= 1f;
				if (LastMaterialList.RareImpactHeavyCurrent <= 0f)
				{
					LastMaterialList.RareImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
					LastMaterialList.RareImpactHeavyCurrent = Random.Range(LastMaterialList.RareImpactHeavyMin, LastMaterialList.RareImpactHeavyMax);
				}
			}
			LastMaterialList.ImpactHeavy.Play(material, volumeMultiplier, falloffMultiplier, offscreenVolumeMultiplier, offscreenFalloffMultiplier);
			break;
		}
	}

	public void Slide(Vector3 origin, MaterialTrigger materialTrigger, float spatialBlend, bool isPlayer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		float volumeMultiplier = 1f;
		if (!isPlayer)
		{
			volumeMultiplier = 0.5f;
		}
		Vector3 material = GetMaterial(origin, materialTrigger);
		if (Object.op_Implicit((Object)(object)LastMaterialList))
		{
			LastMaterialList.SlideOneShot.SpatialBlend = spatialBlend;
			LastMaterialList.SlideOneShot.Play(material, volumeMultiplier);
		}
	}

	public void SlideLoop(Vector3 origin, MaterialTrigger materialTrigger, float spatialBlend, float pitchMultiplier)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = origin;
		bool flag = (Object)(object)materialTrigger.SlidingLoopObject != (Object)null;
		if (!flag || materialTrigger.SlidingLoopObject.getMaterialTimer <= 0f)
		{
			val = GetMaterial(origin, materialTrigger);
			if (flag)
			{
				materialTrigger.SlidingLoopObject.getMaterialTimer = 0.25f;
			}
		}
		if ((Object)(object)materialTrigger.LastMaterialList != (Object)null)
		{
			bool flag2 = false;
			if (!flag)
			{
				flag2 = true;
			}
			else if ((Object)(object)materialTrigger.SlidingLoopObject.material != (Object)(object)materialTrigger.LastMaterialList)
			{
				materialTrigger.SlidingLoopObject = null;
				flag2 = true;
			}
			if (flag2)
			{
				GameObject val2 = Object.Instantiate<GameObject>(AudioManager.instance.AudioMaterialSlidingLoop, val, Quaternion.identity, AudioManager.instance.SoundsParent);
				materialTrigger.SlidingLoopObject = val2.GetComponent<MaterialSlidingLoop>();
				materialTrigger.SlidingLoopObject.material = materialTrigger.LastMaterialList;
			}
			materialTrigger.SlidingLoopObject.activeTimer = 0.1f;
			((Component)materialTrigger.SlidingLoopObject).transform.position = val;
			materialTrigger.SlidingLoopObject.pitchMultiplier = pitchMultiplier;
		}
	}

	private Vector3 GetMaterial(Vector3 origin, MaterialTrigger materialTrigger)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		((Vector3)(ref origin))._002Ector(origin.x, origin.y + 0.1f, origin.z);
		Type _type = materialTrigger.LastMaterialType;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(origin, Vector3.down, ref val, 1f, LayerMask.op_Implicit(LayerMask), (QueryTriggerInteraction)2))
		{
			MaterialSurface component = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<MaterialSurface>();
			if (Object.op_Implicit((Object)(object)component))
			{
				_type = component.Type;
				origin = ((RaycastHit)(ref val)).point;
			}
		}
		LastMaterialList = MaterialList.Find((MaterialPreset x) => x.Type == _type);
		materialTrigger.LastMaterialType = _type;
		materialTrigger.LastMaterialList = LastMaterialList;
		return origin;
	}
}
