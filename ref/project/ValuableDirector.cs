using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ValuableDirector : MonoBehaviour
{
	public enum ValuableDebug
	{
		Normal,
		All,
		None
	}

	public static ValuableDirector instance;

	private PhotonView PhotonView;

	internal ValuableDebug valuableDebug;

	[HideInInspector]
	public bool setupComplete;

	[HideInInspector]
	public bool valuablesSpawned;

	internal int valuableSpawnPlayerReady;

	internal int valuableSpawnAmount;

	internal int valuableTargetAmount = -1;

	internal int switchSetupPlayerReady;

	private string resourcePath = "Valuables/";

	[Space(20f)]
	public AnimationCurve totalMaxAmountCurve;

	private int totalMaxAmount;

	[Space(20f)]
	public AnimationCurve tinyMaxAmountCurve;

	public int tinyChance;

	private int tinyMaxAmount;

	private string tinyPath = "01 Tiny";

	private List<GameObject> tinyValuables = new List<GameObject>();

	private List<ValuableVolume> tinyVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve smallMaxAmountCurve;

	public int smallChance;

	private int smallMaxAmount;

	private string smallPath = "02 Small";

	private List<GameObject> smallValuables = new List<GameObject>();

	private List<ValuableVolume> smallVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve mediumMaxAmountCurve;

	public int mediumChance;

	private int mediumMaxAmount;

	private string mediumPath = "03 Medium";

	private List<GameObject> mediumValuables = new List<GameObject>();

	private List<ValuableVolume> mediumVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve bigMaxAmountCurve;

	public int bigChance;

	private int bigMaxAmount;

	private string bigPath = "04 Big";

	private List<GameObject> bigValuables = new List<GameObject>();

	private List<ValuableVolume> bigVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve wideMaxAmountCurve;

	public int wideChance;

	private int wideMaxAmount;

	private string widePath = "05 Wide";

	private List<GameObject> wideValuables = new List<GameObject>();

	private List<ValuableVolume> wideVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve tallMaxAmountCurve;

	public int tallChance;

	private int tallMaxAmount;

	private string tallPath = "06 Tall";

	private List<GameObject> tallValuables = new List<GameObject>();

	private List<ValuableVolume> tallVolumes = new List<ValuableVolume>();

	[Space]
	public AnimationCurve veryTallMaxAmountCurve;

	public int veryTallChance;

	private int veryTallMaxAmount;

	private string veryTallPath = "07 Very Tall";

	private List<GameObject> veryTallValuables = new List<GameObject>();

	private List<ValuableVolume> veryTallVolumes = new List<ValuableVolume>();

	[Space(20f)]
	public List<ValuableObject> valuableList = new List<ValuableObject>();

	private void Awake()
	{
		instance = this;
		PhotonView = ((Component)this).GetComponent<PhotonView>();
	}

	private void Start()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			((MonoBehaviour)this).StartCoroutine(SetupClient());
		}
	}

	public IEnumerator SetupClient()
	{
		while (valuableTargetAmount == -1)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		while (valuableSpawnAmount < valuableTargetAmount)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		PhotonView.RPC("PlayerReadyRPC", (RpcTarget)0, Array.Empty<object>());
	}

	public IEnumerator SetupHost()
	{
		float num = SemiFunc.RunGetDifficultyMultiplier();
		if (SemiFunc.RunIsArena())
		{
			num = 0.75f;
		}
		totalMaxAmount = Mathf.RoundToInt(totalMaxAmountCurve.Evaluate(num));
		tinyMaxAmount = Mathf.RoundToInt(tinyMaxAmountCurve.Evaluate(num));
		smallMaxAmount = Mathf.RoundToInt(smallMaxAmountCurve.Evaluate(num));
		mediumMaxAmount = Mathf.RoundToInt(mediumMaxAmountCurve.Evaluate(num));
		bigMaxAmount = Mathf.RoundToInt(bigMaxAmountCurve.Evaluate(num));
		wideMaxAmount = Mathf.RoundToInt(wideMaxAmountCurve.Evaluate(num));
		tallMaxAmount = Mathf.RoundToInt(tallMaxAmountCurve.Evaluate(num));
		veryTallMaxAmount = Mathf.RoundToInt(veryTallMaxAmountCurve.Evaluate(num));
		if (SemiFunc.RunIsArena())
		{
			totalMaxAmount /= 2;
			tinyMaxAmount /= 3;
			smallMaxAmount /= 3;
			mediumMaxAmount /= 3;
			bigMaxAmount /= 3;
			wideMaxAmount /= 2;
			tallMaxAmount /= 2;
			veryTallMaxAmount /= 2;
		}
		foreach (LevelValuables valuablePreset in LevelGenerator.Instance.Level.ValuablePresets)
		{
			tinyValuables.AddRange(valuablePreset.tiny);
			smallValuables.AddRange(valuablePreset.small);
			mediumValuables.AddRange(valuablePreset.medium);
			bigValuables.AddRange(valuablePreset.big);
			wideValuables.AddRange(valuablePreset.wide);
			tallValuables.AddRange(valuablePreset.tall);
			veryTallValuables.AddRange(valuablePreset.veryTall);
		}
		List<ValuableVolume> list = Object.FindObjectsOfType<ValuableVolume>(false).ToList();
		tinyVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tiny);
		tinyVolumes.Shuffle();
		smallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Small);
		smallVolumes.Shuffle();
		mediumVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Medium);
		mediumVolumes.Shuffle();
		bigVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Big);
		bigVolumes.Shuffle();
		wideVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Wide);
		wideVolumes.Shuffle();
		tallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tall);
		tallVolumes.Shuffle();
		veryTallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.VeryTall);
		veryTallVolumes.Shuffle();
		if (valuableDebug == ValuableDebug.All)
		{
			totalMaxAmount = list.Count;
			tinyMaxAmount = tinyVolumes.Count;
			smallMaxAmount = smallVolumes.Count;
			mediumMaxAmount = mediumVolumes.Count;
			bigMaxAmount = bigVolumes.Count;
			wideMaxAmount = wideVolumes.Count;
			tallMaxAmount = tallVolumes.Count;
			veryTallMaxAmount = veryTallVolumes.Count;
		}
		if (valuableDebug == ValuableDebug.None || LevelGenerator.Instance.Level.ValuablePresets.Count <= 0)
		{
			totalMaxAmount = 0;
			tinyMaxAmount = 0;
			smallMaxAmount = 0;
			mediumMaxAmount = 0;
			bigMaxAmount = 0;
			wideMaxAmount = 0;
			tallMaxAmount = 0;
			veryTallMaxAmount = 0;
		}
		valuableTargetAmount = 0;
		string[] _names = new string[7] { "Tiny", "Small", "Medium", "Big", "Wide", "Tall", "Very Tall" };
		int[] _maxAmount = new int[7] { tinyMaxAmount, smallMaxAmount, mediumMaxAmount, bigMaxAmount, wideMaxAmount, tallMaxAmount, veryTallMaxAmount };
		List<ValuableVolume>[] _volumes = new List<ValuableVolume>[7] { tinyVolumes, smallVolumes, mediumVolumes, bigVolumes, wideVolumes, tallVolumes, veryTallVolumes };
		string[] _path = new string[7] { tinyPath, smallPath, mediumPath, bigPath, widePath, tallPath, veryTallPath };
		int[] _chance = new int[7] { tinyChance, smallChance, mediumChance, bigChance, wideChance, tallChance, veryTallChance };
		List<GameObject>[] _valuables = new List<GameObject>[7] { tinyValuables, smallValuables, mediumValuables, bigValuables, wideValuables, tallValuables, veryTallValuables };
		int[] _volumeIndex = new int[7];
		for (int _i = 0; _i < totalMaxAmount; _i++)
		{
			float num2 = -1f;
			int num3 = -1;
			for (int i = 0; i < _names.Length; i++)
			{
				if (_volumeIndex[i] < _maxAmount[i] && _volumeIndex[i] < _volumes[i].Count)
				{
					int num4 = Random.Range(0, _chance[i]);
					if ((float)num4 > num2)
					{
						num2 = num4;
						num3 = i;
					}
				}
			}
			if (num3 == -1)
			{
				break;
			}
			ValuableVolume volume = _volumes[num3][_volumeIndex[num3]];
			GameObject valuable = _valuables[num3][Random.Range(0, _valuables[num3].Count)];
			Spawn(valuable, volume, _path[num3]);
			_volumeIndex[num3]++;
			yield return null;
		}
		if (valuableTargetAmount < totalMaxAmount && Object.op_Implicit((Object)(object)DebugComputerCheck.instance) && (!((Behaviour)DebugComputerCheck.instance).enabled || !DebugComputerCheck.instance.LevelDebug || !DebugComputerCheck.instance.ModuleOverrideActive || !Object.op_Implicit((Object)(object)DebugComputerCheck.instance.ModuleOverride)))
		{
			for (int j = 0; j < _names.Length; j++)
			{
				if (_volumeIndex[j] < _maxAmount[j])
				{
					Debug.LogError((object)("Could not spawn enough ''" + _names[j] + "'' valuables!"));
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			PhotonView.RPC("ValuablesTargetSetRPC", (RpcTarget)0, new object[1] { valuableTargetAmount });
		}
		valuableSpawnPlayerReady++;
		while (GameManager.instance.gameMode == 1 && valuableSpawnPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		VolumesAndSwitchSetup();
		while (GameManager.instance.gameMode == 1 && switchSetupPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		setupComplete = true;
	}

	private void Spawn(GameObject _valuable, ValuableVolume _volume, string _path)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(_valuable, ((Component)_volume).transform.position, ((Component)_volume).transform.rotation);
		}
		else
		{
			PhotonNetwork.InstantiateRoomObject(resourcePath + _path + "/" + ((Object)_valuable).name, ((Component)_volume).transform.position, ((Component)_volume).transform.rotation, (byte)0, (object[])null);
		}
		valuableTargetAmount++;
	}

	[PunRPC]
	private void ValuablesTargetSetRPC(int _amount)
	{
		valuableTargetAmount = _amount;
	}

	[PunRPC]
	private void PlayerReadyRPC()
	{
		valuableSpawnPlayerReady++;
	}

	public void VolumesAndSwitchSetup()
	{
		if (GameManager.instance.gameMode == 0)
		{
			VolumesAndSwitchSetupRPC();
		}
		else
		{
			PhotonView.RPC("VolumesAndSwitchSetupRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void VolumesAndSwitchSetupRPC()
	{
		ValuableVolume[] array = Object.FindObjectsOfType<ValuableVolume>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup();
		}
		ValuablePropSwitch[] array2 = Object.FindObjectsOfType<ValuablePropSwitch>(true);
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Setup();
		}
		if (GameManager.instance.gameMode == 0)
		{
			VolumesAndSwitchReadyRPC();
		}
		else
		{
			PhotonView.RPC("VolumesAndSwitchReadyRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void VolumesAndSwitchReadyRPC()
	{
		switchSetupPlayerReady++;
	}
}
