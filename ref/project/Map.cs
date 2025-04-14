using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
	public static Map Instance;

	public bool Active;

	public bool ActivePrevious;

	public GameObject ActiveParent;

	public int PlayerLayer;

	[Space]
	public GameObject LayerPrefab;

	public GameObject ModulePrefab;

	public Transform OverLayerParent;

	[Space]
	public List<MapLayer> Layers = new List<MapLayer>();

	public List<MapModule> MapModules = new List<MapModule>();

	[Space]
	public GameObject EnemyObject;

	public GameObject CustomObject;

	public GameObject ValuableObject;

	[Space]
	public GameObject FloorObject1x1;

	public GameObject FloorObject1x1Diagonal;

	public GameObject FloorObject1x1Curve;

	public GameObject FloorObject1x1CurveInverted;

	[Space]
	public GameObject FloorObject1x05;

	public GameObject FloorObject1x05Diagonal;

	public GameObject FloorObject1x05Curve;

	public GameObject FloorObject1x05CurveInverted;

	[Space]
	public GameObject FloorObject1x025;

	public GameObject FloorObject1x025Diagonal;

	[Space]
	public GameObject RoomVolume;

	public GameObject RoomVolumeOutline;

	[Space]
	public GameObject FloorTruck;

	public GameObject WallTruck;

	[Space]
	public GameObject FloorUsed;

	public GameObject WallUsed;

	[Space]
	public GameObject FloorInactive;

	public GameObject WallInactive;

	[Space]
	public GameObject Wall1x1Object;

	public GameObject Wall1x1DiagonalObject;

	public GameObject Wall1x1CurveObject;

	[Space]
	public GameObject Wall1x05Object;

	public GameObject Wall1x05DiagonalObject;

	public GameObject Wall1x05CurveObject;

	[Space]
	public GameObject Wall1x025Object;

	public GameObject Wall1x025DiagonalObject;

	[Space]
	public GameObject Door1x1Object;

	public GameObject Door1x05Object;

	public GameObject Door1x1DiagonalObject;

	public GameObject Door1x05DiagonalObject;

	public GameObject Door1x2Object;

	public GameObject Door1x1WizardObject;

	public GameObject Door1x1ArcticObject;

	[Space]
	public GameObject DoorBlockedObject;

	public GameObject DoorBlockedWizardObject;

	public GameObject DoorBlockedArcticObject;

	public GameObject DoorDiagonalObject;

	public GameObject StairsObject;

	[Space]
	public float Scale = 0.1f;

	private float LayerHeight = 4f;

	[Space]
	public Transform playerTransformSource;

	public Transform playerTransformTarget;

	[Space]
	public Transform CompletedTransform;

	internal bool debugActive;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		playerTransformSource = ((Component)PlayerController.instance).transform;
		ActiveSet(active: false);
	}

	private void Update()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if (Active != ActivePrevious)
		{
			if (!Active)
			{
				foreach (MapLayer layer in Layers)
				{
					((Component)layer).transform.position = layer.positionStart;
				}
			}
			ActivePrevious = Active;
		}
		if (!Active)
		{
			return;
		}
		foreach (MapLayer layer2 in Layers)
		{
			if (layer2.layer == PlayerLayer)
			{
				((Component)layer2).transform.localPosition = new Vector3(((Component)layer2).transform.localPosition.x, 0f, ((Component)layer2).transform.localPosition.z);
			}
			else if (layer2.layer == PlayerLayer - 1)
			{
				((Component)layer2).transform.localPosition = new Vector3(((Component)layer2).transform.localPosition.x, GetLayerPosition(2).y, ((Component)layer2).transform.localPosition.z);
			}
			else if (layer2.layer == PlayerLayer + 1)
			{
				((Component)layer2).transform.localPosition = new Vector3(((Component)layer2).transform.localPosition.x, GetLayerPosition(3).y, ((Component)layer2).transform.localPosition.z);
			}
			else
			{
				((Component)layer2).transform.localPosition = new Vector3(((Component)layer2).transform.localPosition.x, -5f, ((Component)layer2).transform.localPosition.z);
			}
		}
	}

	public void ActiveSet(bool active)
	{
		Active = active;
		if ((Object)(object)ActiveParent != (Object)null)
		{
			ActiveParent.SetActive(active);
		}
	}

	public void EnemyPositionSet(Transform transformTarget, Transform transformSource)
	{
	}

	public void AddEnemy(Enemy enemy)
	{
	}

	public void CustomPositionSet(Transform transformTarget, Transform transformSource)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		transformTarget.position = ((Component)transformSource).transform.position * Scale + OverLayerParent.position;
		transformTarget.localPosition = new Vector3(transformTarget.localPosition.x, 0f, transformTarget.localPosition.z);
		Quaternion rotation = transformSource.rotation;
		transformTarget.localRotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
	}

	public void AddCustom(MapCustom mapCustom, Sprite sprite, Color color)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(CustomObject, ((Component)OverLayerParent).transform);
		((Object)val.gameObject).name = ((Object)((Component)mapCustom).gameObject).name;
		CustomPositionSet(val.transform, ((Component)mapCustom).transform);
		MapCustomEntity component = val.GetComponent<MapCustomEntity>();
		component.Parent = ((Component)mapCustom).transform;
		component.mapCustom = mapCustom;
		component.spriteRenderer.sprite = sprite;
		component.spriteRenderer.color = color;
		((MonoBehaviour)component).StartCoroutine(component.Logic());
		mapCustom.mapCustomEntity = component;
	}

	public void AddFloor(DirtFinderMapFloor floor)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = null;
		MapLayer layerParent = GetLayerParent(((Component)floor).transform.position.y);
		if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x1, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Diagonal)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x1Diagonal, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x05, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Diagonal)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x05Diagonal, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x05Curve, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve_Inverted)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x05CurveInverted, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x025, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025_Diagonal)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x025Diagonal, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Floor)
		{
			val = Object.Instantiate<GameObject>(FloorTruck, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Wall)
		{
			val = Object.Instantiate<GameObject>(WallTruck, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Floor)
		{
			val = Object.Instantiate<GameObject>(FloorUsed, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Wall)
		{
			val = Object.Instantiate<GameObject>(WallUsed, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Floor)
		{
			val = Object.Instantiate<GameObject>(FloorInactive, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Wall)
		{
			val = Object.Instantiate<GameObject>(WallInactive, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x1Curve, ((Component)layerParent).transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve_Inverted)
		{
			val = Object.Instantiate<GameObject>(FloorObject1x1CurveInverted, ((Component)layerParent).transform);
		}
		((Object)val.gameObject).name = ((Object)((Component)floor).gameObject).name;
		val.transform.localScale = ((Component)floor).transform.localScale;
		val.transform.position = ((Component)floor).transform.position * Scale + ((Component)layerParent).transform.position + GetLayerPosition(layerParent.layer);
		val.transform.rotation = ((Component)floor).transform.rotation;
		MapObjectSetup(((Component)floor).gameObject, val);
	}

	public void AddWall(DirtFinderMapWall wall)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = null;
		MapLayer layerParent = GetLayerParent(((Component)wall).transform.position.y);
		val = ((wall.Type == DirtFinderMapWall.WallType.Door_1x1) ? Object.Instantiate<GameObject>(Door1x1Object, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x2) ? Object.Instantiate<GameObject>(Door1x2Object, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_Blocked) ? Object.Instantiate<GameObject>(DoorBlockedObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Wizard) ? Object.Instantiate<GameObject>(DoorBlockedWizardObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Arctic) ? Object.Instantiate<GameObject>(DoorBlockedArcticObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Stairs) ? Object.Instantiate<GameObject>(StairsObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x05) ? Object.Instantiate<GameObject>(Door1x05Object, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x1_Diagonal) ? Object.Instantiate<GameObject>(Door1x1DiagonalObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x05_Diagonal) ? Object.Instantiate<GameObject>(Door1x05DiagonalObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x05) ? Object.Instantiate<GameObject>(Wall1x05Object, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x025) ? Object.Instantiate<GameObject>(Wall1x025Object, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x05_Diagonal) ? Object.Instantiate<GameObject>(Wall1x05DiagonalObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x025_Diagonal) ? Object.Instantiate<GameObject>(Wall1x025DiagonalObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Diagonal) ? Object.Instantiate<GameObject>(Wall1x1DiagonalObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x1_Wizard) ? Object.Instantiate<GameObject>(Door1x1WizardObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Door_1x1_Arctic) ? Object.Instantiate<GameObject>(Door1x1ArcticObject, ((Component)layerParent).transform) : ((wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Curve) ? Object.Instantiate<GameObject>(Wall1x1CurveObject, ((Component)layerParent).transform) : ((wall.Type != DirtFinderMapWall.WallType.Wall_1x05_Curve) ? Object.Instantiate<GameObject>(Wall1x1Object, ((Component)layerParent).transform) : Object.Instantiate<GameObject>(Wall1x05CurveObject, ((Component)layerParent).transform)))))))))))))))))));
		((Object)val.gameObject).name = ((Object)((Component)wall).gameObject).name;
		val.transform.position = ((Component)wall).transform.position * Scale + ((Component)layerParent).transform.position + GetLayerPosition(layerParent.layer);
		val.transform.rotation = ((Component)wall).transform.rotation;
		val.transform.localScale = ((Component)wall).transform.localScale;
		MapObjectSetup(((Component)wall).gameObject, val);
	}

	public MapModule AddRoomVolume(GameObject _parent, Vector3 _position, Quaternion _rotation, Vector3 _scale, Module _module)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		MapLayer component = ((Component)OverLayerParent).GetComponent<MapLayer>();
		GameObject val = Object.Instantiate<GameObject>(RoomVolume, ((Component)component).transform);
		((Object)val.gameObject).name = "Room Volume";
		val.transform.position = _position * Scale + ((Component)component).transform.position + GetLayerPosition(component.layer);
		val.transform.localPosition = new Vector3(val.transform.localPosition.x, 0f, val.transform.localPosition.z);
		val.transform.rotation = _rotation;
		val.transform.localScale = _scale;
		val.transform.localScale = new Vector3(val.transform.localScale.x, 0.1f, val.transform.localScale.z);
		GameObject val2 = Object.Instantiate<GameObject>(RoomVolumeOutline, ((Component)component).transform);
		val2.transform.position = val.transform.position;
		val2.transform.rotation = val.transform.rotation;
		val2.transform.localScale = new Vector3(val.transform.localScale.x + 0.25f, val.transform.localScale.y, val.transform.localScale.z + 0.25f);
		foreach (MapModule mapModule in MapModules)
		{
			if ((Object)(object)mapModule.module == (Object)(object)_module)
			{
				val.transform.SetParent(((Component)mapModule).transform);
				val2.transform.SetParent(((Component)mapModule).transform);
				return mapModule;
			}
		}
		GameObject val3 = Object.Instantiate<GameObject>(ModulePrefab, ((Component)component).transform);
		MapModule component2 = val3.GetComponent<MapModule>();
		component2.module = _module;
		((Object)val3.gameObject).name = ((Object)((Component)_module).gameObject).name;
		val3.transform.position = ((Component)_module).transform.position * Scale + ((Component)component).transform.position + GetLayerPosition(component.layer);
		MapModules.Add(component2);
		val.transform.SetParent(val3.transform);
		val2.transform.SetParent(val3.transform);
		return component2;
	}

	public void AddValuable(ValuableObject _valuable)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(ValuableObject, ((Component)OverLayerParent).transform);
		((Object)val.gameObject).name = ((Object)((Component)_valuable).gameObject).name;
		val.transform.position = ((Component)_valuable).transform.position * Scale + OverLayerParent.position;
		val.transform.localPosition = new Vector3(val.transform.localPosition.x, 0f, val.transform.localPosition.z);
		MapValuable component = val.GetComponent<MapValuable>();
		component.target = _valuable;
		if (_valuable.volumeType <= ValuableVolume.Type.Medium)
		{
			component.spriteRenderer.sprite = component.spriteSmall;
		}
		else
		{
			component.spriteRenderer.sprite = component.spriteBig;
		}
	}

	public GameObject AddDoor(DirtFinderMapDoor door, GameObject doorPrefab)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		MapLayer layerParent = GetLayerParent(((Component)door).transform.position.y);
		GameObject val = Object.Instantiate<GameObject>(doorPrefab, ((Component)layerParent).transform);
		((Object)val.gameObject).name = ((Object)((Component)door).gameObject).name;
		door.Target = val.transform;
		DirtFinderMapDoorTarget component = val.GetComponent<DirtFinderMapDoorTarget>();
		component.Target = ((Component)door).transform;
		component.Layer = layerParent;
		DoorUpdate(component.HingeTransform, ((Component)door).transform, layerParent);
		return val;
	}

	public void DoorUpdate(Transform transformTarget, Transform transformSource, MapLayer _layer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		transformTarget.position = ((Component)transformSource).transform.position * Scale + ((Component)_layer).transform.position + GetLayerPosition(_layer.layer);
		transformTarget.rotation = transformSource.rotation;
	}

	public MapLayer GetLayerParent(float _positionY)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.FloorToInt((_positionY + 0.1f) / LayerHeight);
		foreach (MapLayer layer in Layers)
		{
			if (layer.layer == num)
			{
				return layer;
			}
		}
		GameObject val = Object.Instantiate<GameObject>(LayerPrefab, ((Component)this).transform);
		MapLayer component = val.GetComponent<MapLayer>();
		component.layer = num;
		Layers.Add(component);
		val.transform.localPosition = new Vector3(val.transform.localPosition.x, LayerHeight * Scale * (float)num, val.transform.localPosition.z);
		((Object)val).name = "Layer " + num;
		Layers = Layers.OrderBy((MapLayer x) => x.layer).ToList();
		Layers.Reverse();
		OverLayerParent.SetSiblingIndex(0);
		int num2 = 1;
		foreach (MapLayer layer2 in Layers)
		{
			((Component)layer2).transform.SetSiblingIndex(num2);
			num2++;
		}
		return component;
	}

	public Vector3 GetLayerPosition(int _layerIndex)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, (0f - LayerHeight * Scale) * (float)_layerIndex, 0f);
	}

	private MapObject MapObjectSetup(GameObject _parent, GameObject _object)
	{
		MapObject component = _object.GetComponent<MapObject>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			Debug.LogError((object)"Map Object missing component!", (Object)(object)_object);
		}
		else
		{
			component.parent = _parent.transform;
			DirtFinderMapFloor component2 = _parent.GetComponent<DirtFinderMapFloor>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.MapObject = component;
			}
		}
		return component;
	}
}
