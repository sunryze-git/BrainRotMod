using System;
using Photon.Pun;
using UnityEngine;

public class ItemHealthPack : MonoBehaviour
{
	public int healAmount;

	private ItemToggle itemToggle;

	private ItemEquippable itemEquippable;

	private ItemAttributes itemAttributes;

	private PhotonView photonView;

	private PhysGrabObject physGrabObject;

	[Space]
	public ParticleSystem[] particles;

	public ParticleSystem[] rejectParticles;

	[Space]
	public PropLight propLight;

	public AnimationCurve lightIntensityCurve;

	private float lightIntensityLerp;

	public MeshRenderer mesh;

	private Material material;

	private Color materialEmissionOriginal;

	private int materialPropertyEmission = Shader.PropertyToID("_EmissionColor");

	[Space]
	public Sound soundUse;

	public Sound soundReject;

	private bool used;

	private void Start()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		material = ((Renderer)mesh).material;
		materialEmissionOriginal = material.GetColor(materialPropertyEmission);
	}

	private void Update()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		LightLogic();
		if (!SemiFunc.IsMasterClientOrSingleplayer() || !itemToggle.toggleState || used)
		{
			return;
		}
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID);
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			return;
		}
		if (playerAvatar.playerHealth.health >= playerAvatar.playerHealth.maxHealth)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("RejectRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				RejectRPC();
			}
			itemToggle.ToggleItem(toggle: false);
			physGrabObject.rb.AddForce(Vector3.up * 2f, (ForceMode)1);
			physGrabObject.rb.AddTorque(-((Component)physGrabObject).transform.right * 0.05f, (ForceMode)1);
			return;
		}
		playerAvatar.playerHealth.HealOther(healAmount, effect: true);
		_ = StatsManager.instance.itemsPurchased[itemAttributes.item.itemAssetName];
		StatsManager.instance.ItemRemove(itemAttributes.instanceName);
		physGrabObject.impactDetector.indestructibleBreakEffects = true;
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("UsedRPC", (RpcTarget)0, Array.Empty<object>());
		}
		else
		{
			UsedRPC();
		}
	}

	private void LightLogic()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (used && lightIntensityLerp < 1f)
		{
			lightIntensityLerp += 1f * Time.deltaTime;
			propLight.lightComponent.intensity = lightIntensityCurve.Evaluate(lightIntensityLerp);
			propLight.originalIntensity = propLight.lightComponent.intensity;
			material.SetColor(materialPropertyEmission, Color.Lerp(Color.black, materialEmissionOriginal, lightIntensityCurve.Evaluate(lightIntensityLerp)));
		}
	}

	[PunRPC]
	private void UsedRPC()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, ((Component)this).transform.position, 0.2f);
		itemToggle.ToggleDisable(_disable: true);
		itemAttributes.DisableUI(_disable: true);
		Object.Destroy((Object)(object)itemEquippable);
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		soundUse.Play(((Component)this).transform.position);
		used = true;
	}

	[PunRPC]
	private void RejectRPC()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		PlayerAvatar playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(itemToggle.playerTogglePhotonID);
		if (playerAvatar.isLocal)
		{
			playerAvatar.physGrabber.ReleaseObjectRPC(physGrabEnded: false, 1f);
		}
		ParticleSystem[] array = rejectParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 1f, 6f, ((Component)this).transform.position, 0.2f);
		soundReject.Play(((Component)this).transform.position);
	}

	public void OnDestroy()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem[] array = particles;
		foreach (ParticleSystem val in array)
		{
			if (Object.op_Implicit((Object)(object)val) && val.isPlaying)
			{
				((Component)val).transform.SetParent((Transform)null);
				MainModule main = val.main;
				((MainModule)(ref main)).stopAction = (ParticleSystemStopAction)2;
			}
		}
		array = rejectParticles;
		foreach (ParticleSystem val2 in array)
		{
			if (Object.op_Implicit((Object)(object)val2) && val2.isPlaying)
			{
				((Component)val2).transform.SetParent((Transform)null);
				MainModule main2 = val2.main;
				((MainModule)(ref main2)).stopAction = (ParticleSystemStopAction)2;
			}
		}
	}
}
