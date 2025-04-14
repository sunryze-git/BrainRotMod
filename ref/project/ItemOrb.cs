using System.Collections.Generic;
using UnityEngine;

public class ItemOrb : MonoBehaviour
{
	public enum OrbType
	{
		Constant,
		Pulse
	}

	[HideInInspector]
	public SemiFunc.emojiIcon emojiIcon;

	public Texture orbIcon;

	private Material orbEffect;

	public float orbRadius = 1f;

	private float orbRadiusOriginal = 1f;

	private float orbRadiusMultiplier = 1f;

	private Transform orbTransform;

	private Transform orbInnerTransform;

	private ItemToggle itemToggle;

	[HideInInspector]
	public float batteryDrainRate = 0.1f;

	[HideInInspector]
	public bool itemActive;

	private Transform sphereEffectTransform;

	private float sphereEffectScaleLerp;

	private PhysGrabObject physGrabObject;

	internal List<PhysGrabObject> objectAffected = new List<PhysGrabObject>();

	internal bool localPlayerAffected;

	private Transform sphereCheckTransform;

	private float sphereCheckTimer;

	private List<Transform> spherePieces = new List<Transform>();

	private Transform sphereCore;

	[HideInInspector]
	public ColorPresets colorPresets;

	public BatteryDrainPresets batteryDrainPreset;

	[HideInInspector]
	public Color orbColor;

	private Color orbColorLight;

	[HideInInspector]
	public Color batteryColor;

	private ItemBattery itemBattery;

	private float onNoBatteryTimer;

	private ItemEquippable itemEquippable;

	private ItemAttributes itemAttributes;

	public Sound soundOrbBoot;

	public Sound soundOrbShutdown;

	public Sound soundOrbLoop;

	public OrbType orbType;

	public bool targetValuables = true;

	public bool targetPlayers = true;

	public bool targetEnemies = true;

	public bool targetNonValuables = true;

	private ITargetingCondition customTargetingCondition;

	private void Start()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		customTargetingCondition = ((Component)this).GetComponent<ITargetingCondition>();
		itemBattery = ((Component)this).GetComponent<ItemBattery>();
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		emojiIcon = itemAttributes.emojiIcon;
		colorPresets = itemAttributes.colorPreset;
		orbColor = colorPresets.GetColorMain();
		orbColorLight = colorPresets.GetColorLight();
		batteryColor = orbColorLight;
		itemBattery.batteryColor = batteryColor;
		batteryDrainRate = batteryDrainPreset.batteryDrainRate;
		itemBattery.batteryDrainRate = batteryDrainRate;
		itemEquippable.itemEmoji = emojiIcon.ToString();
		ItemLight component = ((Component)this).GetComponent<ItemLight>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.itemLight.color = orbColor;
		}
		Transform val = ((Component)this).transform.Find("Item Orb Mesh/Top/Piece1/Orb Icon");
		if (Object.op_Implicit((Object)(object)val))
		{
			((Component)val).GetComponent<Renderer>().material.SetTexture("_EmissionMap", orbIcon);
			((Component)val).GetComponent<Renderer>().material.SetColor("_EmissionColor", orbColor);
		}
		Transform val2 = null;
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val3 = item;
			if (((Object)val3).name == "Item Orb Mesh")
			{
				val2 = val3;
			}
		}
		if ((Object)(object)val2 == (Object)null)
		{
			Debug.LogWarning((object)("Item Orb Mesh not found in" + ((Object)((Component)this).gameObject).name));
		}
		foreach (Transform item2 in val2)
		{
			Transform val4 = item2;
			foreach (Transform item3 in val4)
			{
				Transform val5 = item3;
				if (((Object)val5).name.Contains("Piece"))
				{
					spherePieces.Add(val5);
					((Component)val5).GetComponent<Renderer>().material.SetColor("_EmissionColor", orbColor);
				}
			}
			if (((Object)val4).name.Contains("Core"))
			{
				sphereCore = val4;
				((Component)val4).GetComponent<Renderer>().material.SetColor("_EmissionColor", orbColorLight);
			}
		}
		sphereEffectTransform = ((Component)this).transform.Find("sphere effect");
		Material material = ((Component)((Component)this).transform.Find("sphere effect/AreaEffect/effect")).GetComponent<Renderer>().material;
		Material material2 = ((Component)((Component)this).transform.Find("sphere effect/AreaEffect/outline_inside")).GetComponent<Renderer>().material;
		Material material3 = ((Component)((Component)this).transform.Find("sphere effect/AreaEffect/outline")).GetComponent<Renderer>().material;
		Color val6 = orbColorLight;
		((Color)(ref val6))._002Ector(val6.r, val6.g, val6.b, 0.5f);
		Color val7 = default(Color);
		((Color)(ref val7))._002Ector(orbColor.r, orbColor.g, orbColor.b, 0.1f);
		if (Object.op_Implicit((Object)(object)material))
		{
			material.SetColor("_Color", val7);
		}
		if (Object.op_Implicit((Object)(object)material2))
		{
			material2.SetColor("_EdgeColor", val6);
		}
		if (Object.op_Implicit((Object)(object)material3))
		{
			material3.SetColor("_EdgeColor", val6);
		}
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		((Component)sphereEffectTransform).transform.localScale = new Vector3(0f, 0f, 0f);
		((Component)sphereEffectTransform).gameObject.SetActive(false);
		orbRadiusOriginal = orbRadius;
		physGrabObject.clientNonKinematic = true;
	}

	private void Update()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		soundOrbLoop.PlayLoop(itemActive, 0.5f, 0.5f);
		if (!itemActive)
		{
			onNoBatteryTimer = 0f;
		}
		if (orbType == OrbType.Constant)
		{
			OrbConstantLogic();
		}
		if (orbType == OrbType.Pulse)
		{
			OrbPulseLogic();
		}
		bool num = itemActive;
		itemActive = itemToggle.toggleState;
		orbRadius = orbRadiusOriginal * orbRadiusMultiplier;
		if (num != itemActive)
		{
			SphereAnimatePiecesBack();
			itemBattery.batteryActive = itemActive;
			if (itemActive)
			{
				soundOrbBoot.Play(((Component)this).transform.position);
			}
			else
			{
				soundOrbShutdown.Play(((Component)this).transform.position);
			}
		}
		if (itemActive)
		{
			((Component)sphereEffectTransform).gameObject.SetActive(true);
			if (itemBattery.batteryLife > 0f)
			{
				OrbAnimateAppear();
			}
			SphereAnimatePieces();
			if (itemBattery.batteryLife <= 0f)
			{
				onNoBatteryTimer += Time.deltaTime;
				if (onNoBatteryTimer >= 1.5f)
				{
					itemToggle.ToggleItem(toggle: false);
					onNoBatteryTimer = 0f;
				}
			}
		}
		else
		{
			OrbAnimateDisappear();
		}
		if (((Component)sphereEffectTransform).gameObject.activeSelf)
		{
			sphereEffectTransform.rotation = Quaternion.identity;
		}
	}

	private void SphereAnimatePieces()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (Transform spherePiece in spherePieces)
		{
			float num2 = Mathf.Sin(Time.time * 50f + (float)num) * 0.1f;
			spherePiece.localScale = new Vector3(1f + num2, 1f + num2, 1f + num2);
			num++;
		}
		float num3 = Mathf.Sin(Time.time * 30f) * 0.2f;
		sphereCore.localScale = new Vector3(1f + num3, 1f + num3, 1f + num3);
	}

	private void SphereAnimatePiecesBack()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform spherePiece in spherePieces)
		{
			spherePiece.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void OrbConstantLogic()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		if (itemBattery.batteryLifeInt == 0)
		{
			objectAffected.Clear();
			localPlayerAffected = false;
		}
		else
		{
			if (!itemActive)
			{
				return;
			}
			sphereCheckTimer += Time.deltaTime;
			if (!(sphereCheckTimer > 0.1f))
			{
				return;
			}
			objectAffected.Clear();
			sphereCheckTimer = 0f;
			if (itemBattery.batteryLife <= 0f)
			{
				return;
			}
			if (targetEnemies || targetNonValuables || targetValuables)
			{
				objectAffected = SemiFunc.PhysGrabObjectGetAllWithinRange(orbRadius, ((Component)this).transform.position);
				if (!targetEnemies || !targetNonValuables || !targetValuables)
				{
					List<PhysGrabObject> list = new List<PhysGrabObject>();
					foreach (PhysGrabObject item in objectAffected)
					{
						bool flag = customTargetingCondition != null && customTargetingCondition.CustomTargetingCondition(((Component)item).gameObject);
						if (customTargetingCondition == null)
						{
							flag = true;
						}
						if (targetEnemies && item.isEnemy && flag)
						{
							list.Add(item);
						}
						if (targetNonValuables && item.isNonValuable && flag)
						{
							list.Add(item);
						}
						if (targetValuables && item.isValuable && flag)
						{
							list.Add(item);
						}
					}
					objectAffected.Clear();
					objectAffected = list;
				}
			}
			if (targetPlayers)
			{
				localPlayerAffected = SemiFunc.LocalPlayerOverlapCheck(orbRadius, ((Component)this).transform.position);
			}
		}
	}

	private void OrbPulseLogic()
	{
	}

	private void OrbAnimateAppear()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Lerp(0f, orbRadius, sphereEffectScaleLerp);
		sphereEffectTransform.localScale = new Vector3(num, num, num);
		if (sphereEffectScaleLerp < 1f)
		{
			sphereEffectScaleLerp += 10f * Time.deltaTime;
		}
		else
		{
			sphereEffectScaleLerp = 1f;
		}
	}

	private void OrbAnimateDisappear()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)sphereEffectTransform).gameObject.activeSelf)
		{
			float num = Mathf.Lerp(0f, orbRadius, sphereEffectScaleLerp);
			sphereEffectTransform.localScale = new Vector3(num, num, num);
			if (sphereEffectScaleLerp > 0f)
			{
				sphereEffectScaleLerp -= 10f * Time.deltaTime;
				return;
			}
			sphereEffectScaleLerp = 0f;
			((Component)sphereEffectTransform).gameObject.SetActive(false);
		}
	}
}
