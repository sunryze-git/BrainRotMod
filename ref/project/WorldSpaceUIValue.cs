using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIValue : WorldSpaceUIChild
{
	public static WorldSpaceUIValue instance;

	private float showTimer;

	private Vector3 scale;

	private int value;

	private TextMeshProUGUI text;

	private Vector3 newWorldPosition;

	private Vector3 offset;

	private PhysGrabObject currentPhysGrabObject;

	public AnimationCurve curveIntro;

	public AnimationCurve curveOutro;

	private float curveLerp;

	[Space]
	public Color colorValue;

	public Color colorCost;

	[Space]
	public float textSizeValue;

	public float textSizeCost;

	private void Awake()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		positionOffset = new Vector3(0f, -0.05f, 0f);
		instance = this;
		scale = ((Component)this).transform.localScale;
		text = ((Component)this).GetComponent<TextMeshProUGUI>();
	}

	protected override void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		worldPosition = Vector3.Lerp(worldPosition, newWorldPosition, 50f * Time.deltaTime);
		if (Object.op_Implicit((Object)(object)currentPhysGrabObject))
		{
			newWorldPosition = currentPhysGrabObject.centerPoint + offset;
		}
		if (showTimer > 0f)
		{
			showTimer -= Time.deltaTime;
			curveLerp += 10f * Time.deltaTime;
			curveLerp = Mathf.Clamp01(curveLerp);
			((Component)this).transform.localScale = scale * curveIntro.Evaluate(curveLerp);
			return;
		}
		curveLerp -= 10f * Time.deltaTime;
		curveLerp = Mathf.Clamp01(curveLerp);
		((Component)this).transform.localScale = scale * curveOutro.Evaluate(curveLerp);
		if (curveLerp <= 0f)
		{
			currentPhysGrabObject = null;
		}
	}

	public void Show(PhysGrabObject _grabObject, int _value, bool _cost, Vector3 _offset)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)currentPhysGrabObject) && !((Object)(object)currentPhysGrabObject == (Object)(object)_grabObject))
		{
			return;
		}
		value = _value;
		if (_cost)
		{
			((TMP_Text)text).text = "-$" + SemiFunc.DollarGetString(value) + "K";
			((TMP_Text)text).fontSize = textSizeCost;
		}
		else
		{
			((TMP_Text)text).text = "$" + SemiFunc.DollarGetString(value);
			((TMP_Text)text).fontSize = textSizeValue;
		}
		showTimer = 0.1f;
		if (!Object.op_Implicit((Object)(object)currentPhysGrabObject))
		{
			offset = _offset;
			currentPhysGrabObject = _grabObject;
			newWorldPosition = currentPhysGrabObject.centerPoint + offset - Vector3.up * 0.1f;
			worldPosition = newWorldPosition;
			if (_cost)
			{
				((Graphic)text).color = colorCost;
			}
			else
			{
				((Graphic)text).color = colorValue;
			}
		}
	}
}
