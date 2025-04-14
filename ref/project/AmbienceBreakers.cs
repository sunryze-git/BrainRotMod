using System.Collections;
using Photon.Pun;
using UnityEngine;

public class AmbienceBreakers : MonoBehaviour
{
	public static AmbienceBreakers instance;

	private PhotonView photonView;

	private bool isLocal;

	[Space]
	public float minDistance = 8f;

	public float maxDistance = 15f;

	[Space]
	public float cooldownMin = 20f;

	public float cooldownMax = 120f;

	private float cooldownTimer;

	[Space]
	public Sound sound;

	private int presetOverride = -1;

	private int breakerOverride = -1;

	private float updateRate = 0.5f;

	private void Awake()
	{
		instance = this;
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			isLocal = true;
		}
	}

	public void Setup()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		cooldownTimer = Random.Range(cooldownMin, cooldownMax);
		if (!isLocal)
		{
			yield break;
		}
		while (true)
		{
			if (cooldownTimer > 0f)
			{
				cooldownTimer -= updateRate;
			}
			else
			{
				cooldownTimer = Random.Range(cooldownMin, cooldownMax);
				Vector2 insideUnitCircle = Random.insideUnitCircle;
				Vector2 normalized = ((Vector2)(ref insideUnitCircle)).normalized;
				float num = Random.Range(minDistance, maxDistance);
				Vector3 val = ((Component)GameDirector.instance.PlayerList[Random.Range(0, GameDirector.instance.PlayerList.Count)]).transform.position + new Vector3(normalized.x, 0f, normalized.y) * num;
				int num2 = Random.Range(0, LevelGenerator.Instance.Level.AmbiencePresets.Count);
				if (presetOverride != -1)
				{
					num2 = presetOverride;
				}
				presetOverride = -1;
				if (LevelGenerator.Instance.Level.AmbiencePresets[num2].breakers.Count > 0)
				{
					int num3 = Random.Range(0, LevelGenerator.Instance.Level.AmbiencePresets[num2].breakers.Count);
					if (breakerOverride != -1)
					{
						num3 = breakerOverride;
					}
					breakerOverride = -1;
					if (!GameManager.Multiplayer())
					{
						PlaySoundRPC(val, num2, num3);
					}
					else
					{
						photonView.RPC("PlaySoundRPC", (RpcTarget)0, new object[3] { val, num2, num3 });
					}
				}
			}
			yield return (object)new WaitForSeconds(updateRate);
		}
	}

	public void LiveTest(LevelAmbience _presetOverride, LevelAmbienceBreaker _breakerOverride)
	{
		foreach (LevelAmbience ambiencePreset in LevelGenerator.Instance.Level.AmbiencePresets)
		{
			if (!((Object)(object)ambiencePreset == (Object)(object)_presetOverride))
			{
				continue;
			}
			presetOverride = LevelGenerator.Instance.Level.AmbiencePresets.IndexOf(ambiencePreset);
			foreach (LevelAmbienceBreaker breaker in ambiencePreset.breakers)
			{
				if (breaker == _breakerOverride)
				{
					breakerOverride = ambiencePreset.breakers.IndexOf(breaker);
				}
			}
		}
		cooldownTimer = 0f;
	}

	[PunRPC]
	public void PlaySoundRPC(Vector3 _position, int _preset, int _breaker)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		LevelAmbienceBreaker levelAmbienceBreaker = LevelGenerator.Instance.Level.AmbiencePresets[_preset].breakers[_breaker];
		sound.Volume = levelAmbienceBreaker.volume;
		sound.Sounds[0] = levelAmbienceBreaker.sound;
		sound.Play(_position);
	}
}
