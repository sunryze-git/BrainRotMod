using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public enum Effect
	{
		Upgrade
	}

	public enum EyeOverrideState
	{
		None,
		Red,
		Green,
		Love,
		CeilingEye
	}

	public bool isMenuAvatar;

	private PlayerAvatar playerAvatar;

	private PhotonView photonView;

	internal bool hurtFreeze;

	private float hurtFreezeTimer;

	private bool healthSet;

	internal int health = 100;

	private int healthPrevious;

	internal int maxHealth = 100;

	private bool godMode;

	public Transform meshParent;

	public Light eyeLight;

	private List<MeshRenderer> renderers;

	private List<Material> sharedMaterials = new List<Material>();

	internal List<Material> instancedMaterials = new List<Material>();

	private int materialHurtAmount;

	private int materialHurtColor;

	internal Material bodyMaterial;

	internal Material eyeMaterial;

	internal Material pupilMaterial;

	private Material healthMaterial;

	private int healthMaterialAmount;

	public Gradient healthMaterialColor;

	private bool materialEffect;

	private Color materialEffectColor;

	private AnimationCurve materialEffectCurve;

	private float materialEffectLerp;

	public Sound hurtOther;

	public Sound healOther;

	public Sound upgradeOther;

	private float overrideEyeMaterialLerp;

	private float overrideEyeMaterialLerpPrevious;

	private float overrideEyeMaterialTimer;

	private Color overrideEyeMaterialColor;

	private Color overridePupilMaterialColor;

	private Color overrideEyeLightColor;

	private float overrideEyeLightIntensity;

	private bool overrideEyeActive;

	private bool overrideEyeActivePrevious;

	private int overrideEyePriority = -999;

	private EyeOverrideState overrideEyeState;

	private EyeOverrideState overrideEyeStatePrevious;

	private float invincibleTimer;

	private void Awake()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (!isMenuAvatar)
		{
			playerAvatar = ((Component)this).GetComponent<PlayerAvatar>();
		}
		else
		{
			playerAvatar = PlayerAvatar.instance;
		}
		if (!isMenuAvatar && !SemiFunc.RunIsLobbyMenu() && (!GameManager.Multiplayer() || photonView.IsMine))
		{
			((MonoBehaviour)this).StartCoroutine(Fetch());
		}
		materialEffectCurve = AssetManager.instance.animationCurveImpact;
		renderers = new List<MeshRenderer>();
		renderers.AddRange(((Component)meshParent).GetComponentsInChildren<MeshRenderer>(true));
		foreach (MeshRenderer renderer in renderers)
		{
			Material val = null;
			foreach (Material sharedMaterial in sharedMaterials)
			{
				if (((Object)((Renderer)renderer).sharedMaterial).name == ((Object)sharedMaterial).name)
				{
					val = sharedMaterial;
					((Renderer)renderer).sharedMaterial = instancedMaterials[sharedMaterials.IndexOf(sharedMaterial)];
				}
			}
			if (!Object.op_Implicit((Object)(object)val))
			{
				string name = ((Object)((Renderer)renderer).sharedMaterial).name;
				val = ((Renderer)renderer).sharedMaterial;
				sharedMaterials.Add(val);
				instancedMaterials.Add(((Renderer)renderer).material);
				if (name == "Player Avatar - Body")
				{
					bodyMaterial = ((Renderer)renderer).sharedMaterial;
				}
				if (name == "Player Avatar - Health")
				{
					healthMaterial = ((Renderer)renderer).sharedMaterial;
				}
				if (name == "Player Avatar - Eye")
				{
					eyeMaterial = ((Renderer)renderer).sharedMaterial;
				}
				if (name == "Player Avatar - Pupil")
				{
					pupilMaterial = ((Renderer)renderer).sharedMaterial;
				}
			}
		}
		materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		healthMaterialAmount = Shader.PropertyToID("_OffsetX");
	}

	private void Start()
	{
		DebugComputerCheck[] array = Object.FindObjectsOfType<DebugComputerCheck>();
		foreach (DebugComputerCheck debugComputerCheck in array)
		{
			if (debugComputerCheck.Active && debugComputerCheck.PlayerDebug && debugComputerCheck.GodMode)
			{
				godMode = true;
			}
		}
	}

	private IEnumerator Fetch()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		int num = StatsManager.instance.GetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar));
		if (num <= 0)
		{
			num = 1;
		}
		health = num;
		maxHealth = 100 + StatsManager.instance.GetPlayerMaxHealth(SemiFunc.PlayerGetSteamID(playerAvatar));
		health = Mathf.Clamp(health, 0, maxHealth);
		if (SemiFunc.RunIsArena())
		{
			health = maxHealth;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar), health, setInShop: false);
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateHealthRPC", (RpcTarget)1, new object[3] { health, maxHealth, true });
		}
		healthSet = true;
	}

	private void Update()
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatar.isLocal)
		{
			if (overrideEyeMaterialTimer > 0f)
			{
				overrideEyeMaterialTimer -= Time.deltaTime;
				if (!overrideEyeActive)
				{
					overrideEyeActive = true;
				}
			}
			else if (overrideEyeActive)
			{
				overrideEyeActive = false;
				overrideEyePriority = -999;
			}
			if (SemiFunc.IsMultiplayer() && (overrideEyeActive != overrideEyeActivePrevious || overrideEyeState != overrideEyeStatePrevious))
			{
				overrideEyeActivePrevious = overrideEyeActive;
				overrideEyeStatePrevious = overrideEyeState;
				photonView.RPC("EyeMaterialOverrideRPC", (RpcTarget)1, new object[2] { overrideEyeState, overrideEyeActive });
			}
		}
		if (overrideEyeActive)
		{
			overrideEyeMaterialLerp += 3f * Time.deltaTime;
		}
		else
		{
			overrideEyeMaterialLerp -= 3f * Time.deltaTime;
		}
		overrideEyeMaterialLerp = Mathf.Clamp01(overrideEyeMaterialLerp);
		if (overrideEyeMaterialLerp != overrideEyeMaterialLerpPrevious)
		{
			overrideEyeMaterialLerpPrevious = overrideEyeMaterialLerp;
			float num = AssetManager.instance.animationCurveEaseInOut.Evaluate(overrideEyeMaterialLerp);
			eyeMaterial.SetFloat(materialHurtAmount, num);
			eyeMaterial.SetColor(materialHurtColor, overrideEyeMaterialColor);
			pupilMaterial.SetFloat(materialHurtAmount, num);
			pupilMaterial.SetColor(materialHurtColor, overridePupilMaterialColor);
			if (overrideEyeMaterialLerp <= 0f)
			{
				((Component)eyeLight).gameObject.SetActive(false);
			}
			else if (!((Component)eyeLight).gameObject.activeSelf)
			{
				((Component)eyeLight).gameObject.SetActive(true);
			}
			eyeLight.color = overrideEyeLightColor;
			eyeLight.intensity = overrideEyeLightIntensity * num;
		}
		if (materialEffect)
		{
			materialEffectLerp += 2.5f * Time.deltaTime;
			materialEffectLerp = Mathf.Clamp01(materialEffectLerp);
			if (playerAvatar.deadSet && !playerAvatar.isDisabled)
			{
				materialEffectLerp = Mathf.Clamp(materialEffectLerp, 0f, 0.1f);
			}
			foreach (Material instancedMaterial in instancedMaterials)
			{
				if ((Object)(object)instancedMaterial != (Object)(object)eyeMaterial && (Object)(object)instancedMaterial != (Object)(object)pupilMaterial)
				{
					instancedMaterial.SetFloat(materialHurtAmount, materialEffectCurve.Evaluate(materialEffectLerp));
				}
			}
			if (hurtFreeze && materialEffectLerp > 0.2f)
			{
				hurtFreeze = false;
			}
			if (materialEffectLerp >= 1f)
			{
				materialEffect = false;
				foreach (Material instancedMaterial2 in instancedMaterials)
				{
					if ((Object)(object)instancedMaterial2 != (Object)(object)eyeMaterial && (Object)(object)instancedMaterial2 != (Object)(object)pupilMaterial)
					{
						instancedMaterial2.SetFloat(materialHurtAmount, 0f);
					}
				}
			}
			hurtFreezeTimer = 0f;
			if (!overrideEyeActive)
			{
				eyeMaterial.SetFloat(materialHurtAmount, materialEffectCurve.Evaluate(materialEffectLerp));
				eyeMaterial.SetColor(materialHurtColor, Color.white);
				pupilMaterial.SetFloat(materialHurtAmount, materialEffectCurve.Evaluate(materialEffectLerp));
				pupilMaterial.SetColor(materialHurtColor, Color.black);
			}
		}
		else if (hurtFreeze)
		{
			hurtFreezeTimer -= Time.deltaTime;
			if (hurtFreezeTimer <= 0f)
			{
				hurtFreeze = false;
			}
		}
		if (isMenuAvatar)
		{
			health = playerAvatar.playerHealth.health;
		}
		if ((isMenuAvatar || (GameManager.Multiplayer() && !playerAvatar.isLocal)) && healthPrevious != health)
		{
			float num2 = (float)health / (float)maxHealth;
			float num3 = Mathf.Lerp(0.98f, 0f, num2);
			if (num3 <= 0f)
			{
				num3 = -0.5f;
			}
			healthMaterial.SetFloat(healthMaterialAmount, num3);
			int num4 = Shader.PropertyToID("_AlbedoColor");
			int num5 = Shader.PropertyToID("_EmissionColor");
			Color val = healthMaterialColor.Evaluate(num2);
			healthMaterial.SetColor(num4, val);
			val.a = healthMaterial.GetColor(num5).a;
			healthMaterial.SetColor(num5, val);
			healthPrevious = health;
		}
		if (invincibleTimer > 0f)
		{
			invincibleTimer -= Time.deltaTime;
		}
	}

	public void HurtFreezeOverride(float _time)
	{
		hurtFreeze = true;
		hurtFreezeTimer = _time;
	}

	public void Death()
	{
		health = 0;
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar), health, setInShop: false);
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateHealthRPC", (RpcTarget)1, new object[3] { health, maxHealth, true });
		}
	}

	public void MaterialEffectOverride(Effect _effect)
	{
		if (!GameManager.Multiplayer())
		{
			MaterialEffectOverrideRPC((int)_effect);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photonView.RPC("MaterialEffectOverrideRPC", (RpcTarget)0, new object[1] { (int)_effect });
		}
	}

	[PunRPC]
	public void MaterialEffectOverrideRPC(int _effect)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		materialEffect = true;
		materialEffectLerp = 0f;
		Color white = Color.white;
		if (_effect == 0)
		{
			((Color)(ref white))._002Ector(1f, 0.94f, 0f);
			if (!playerAvatar.isLocal)
			{
				upgradeOther.Play(((Component)this).transform.position);
			}
		}
		foreach (Material instancedMaterial in instancedMaterials)
		{
			if ((Object)(object)instancedMaterial != (Object)(object)eyeMaterial && (Object)(object)instancedMaterial != (Object)(object)pupilMaterial)
			{
				instancedMaterial.SetColor(materialHurtColor, white);
			}
		}
	}

	public void Hurt(int damage, bool savingGrace, int enemyIndex = -1)
	{
		if (invincibleTimer > 0f || damage <= 0 || (GameManager.Multiplayer() && !photonView.IsMine) || playerAvatar.deadSet || godMode)
		{
			return;
		}
		if (savingGrace && damage <= 25 && health > 5 && health <= 20)
		{
			health -= damage;
			if (health <= 0)
			{
				health = Random.Range(1, 5);
			}
		}
		else
		{
			health -= damage;
		}
		if (health <= 0)
		{
			playerAvatar.PlayerDeath(enemyIndex);
			health = 0;
			return;
		}
		if ((float)damage >= 25f)
		{
			CameraGlitch.Instance.PlayLongHurt();
		}
		else
		{
			CameraGlitch.Instance.PlayShortHurt();
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar), health, setInShop: false);
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateHealthRPC", (RpcTarget)1, new object[3] { health, maxHealth, true });
		}
	}

	public void HurtOther(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex = -1)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!GameManager.Multiplayer())
		{
			Hurt(damage, savingGrace, enemyIndex);
			return;
		}
		photonView.RPC("HurtOtherRPC", (RpcTarget)0, new object[4] { damage, hurtPosition, savingGrace, enemyIndex });
	}

	[PunRPC]
	public void HurtOtherRPC(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (photonView.IsMine && (hurtPosition == Vector3.zero || Vector3.Distance(((Component)playerAvatar).transform.position, hurtPosition) < 2f))
		{
			Hurt(damage, savingGrace, enemyIndex);
		}
	}

	public void Heal(int healAmount, bool effect = true)
	{
		if (healAmount <= 0 || health == maxHealth || (GameManager.Multiplayer() && !photonView.IsMine) || playerAvatar.isDisabled || godMode)
		{
			return;
		}
		if (effect && health != 0)
		{
			if ((float)healAmount >= 25f)
			{
				CameraGlitch.Instance.PlayLongHeal();
			}
			else
			{
				CameraGlitch.Instance.PlayShortHeal();
			}
		}
		health += healAmount;
		health = Mathf.Clamp(health, 0, maxHealth);
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar), health, setInShop: false);
		if (GameManager.Multiplayer())
		{
			photonView.RPC("UpdateHealthRPC", (RpcTarget)1, new object[3] { health, maxHealth, effect });
		}
	}

	public void HealOther(int healAmount, bool effect)
	{
		if (!GameManager.Multiplayer())
		{
			Heal(healAmount, effect);
			return;
		}
		photonView.RPC("HealOtherRPC", (RpcTarget)0, new object[2] { healAmount, effect });
	}

	[PunRPC]
	public void HealOtherRPC(int healAmount, bool effect)
	{
		if (photonView.IsMine)
		{
			Heal(healAmount, effect);
		}
	}

	[PunRPC]
	public void UpdateHealthRPC(int healthNew, int healthMax, bool effect)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		maxHealth = healthMax;
		if (!healthSet)
		{
			health = healthNew;
			healthSet = true;
		}
		else
		{
			if (effect)
			{
				materialEffect = true;
				if (!playerAvatar.deadSet)
				{
					materialEffectLerp = 0f;
				}
				if (healthNew < health || healthNew == 0)
				{
					hurtOther.Play(((Component)this).transform.position);
					hurtFreeze = true;
					foreach (Material instancedMaterial in instancedMaterials)
					{
						if ((Object)(object)instancedMaterial != (Object)(object)eyeMaterial && (Object)(object)instancedMaterial != (Object)(object)pupilMaterial)
						{
							instancedMaterial.SetColor(materialHurtColor, Color.red);
						}
					}
				}
				else
				{
					if (health != 0)
					{
						healOther.Play(((Component)this).transform.position);
					}
					SetMaterialGreen();
				}
			}
			health = healthNew;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(playerAvatar), health, setInShop: false);
	}

	public void SetMaterialGreen()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		foreach (Material instancedMaterial in instancedMaterials)
		{
			if ((Object)(object)instancedMaterial != (Object)(object)eyeMaterial && (Object)(object)instancedMaterial != (Object)(object)pupilMaterial)
			{
				instancedMaterial.SetColor(materialHurtColor, new Color(0f, 1f, 0.25f));
			}
		}
	}

	private void EyeMaterialSetup()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (overrideEyeMaterialLerp >= 1f)
		{
			overrideEyeMaterialLerp = 0.8f;
		}
		if (overrideEyeState == EyeOverrideState.Red)
		{
			overrideEyeMaterialColor = Color.red;
			overridePupilMaterialColor = Color.white;
			overrideEyeLightColor = Color.red;
			overrideEyeLightIntensity = 5f;
		}
		else if (overrideEyeState == EyeOverrideState.Green)
		{
			overrideEyeMaterialColor = Color.green;
			overridePupilMaterialColor = Color.white;
			overrideEyeLightColor = Color.green;
			overrideEyeLightIntensity = 5f;
		}
		else if (overrideEyeState == EyeOverrideState.Love)
		{
			overrideEyeMaterialColor = new Color(1f, 0f, 0.5f);
			overridePupilMaterialColor = new Color(0.2f, 0f, 0.2f);
			overrideEyeLightColor = new Color(0.4f, 0f, 0f);
			overrideEyeLightIntensity = 1f;
		}
		else if (overrideEyeState == EyeOverrideState.CeilingEye)
		{
			overrideEyeMaterialColor = new Color(1f, 0.4f, 0f);
			overridePupilMaterialColor = new Color(1f, 1f, 0f);
			overrideEyeLightColor = new Color(1f, 0.4f, 0f);
			overrideEyeLightIntensity = 1f;
		}
	}

	public void EyeMaterialOverride(EyeOverrideState _state, float _time, int _priority)
	{
		if (_priority >= overrideEyePriority)
		{
			overrideEyePriority = _priority;
			if (overrideEyeState != _state)
			{
				overrideEyeState = _state;
				EyeMaterialSetup();
			}
			overrideEyeMaterialTimer = _time;
		}
	}

	[PunRPC]
	public void EyeMaterialOverrideRPC(EyeOverrideState _state, bool _active)
	{
		overrideEyeActive = _active;
		if (overrideEyeState != _state)
		{
			overrideEyeState = _state;
			EyeMaterialSetup();
		}
	}

	public void InvincibleSet(float _time)
	{
		invincibleTimer = _time;
	}
}
