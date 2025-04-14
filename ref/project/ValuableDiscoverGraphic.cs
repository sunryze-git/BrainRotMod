using UnityEngine;
using UnityEngine.UI;

public class ValuableDiscoverGraphic : MonoBehaviour
{
	public enum State
	{
		Discover,
		Reminder,
		Bad
	}

	public PhysGrabObject target;

	private State state;

	private Camera mainCamera;

	private RectTransform canvasRect;

	private Vector3[] screenSpaceCorners;

	private bool hidden = true;

	private bool first = true;

	[Space]
	public Color colorDiscoverCorner;

	public Color colorDiscoverMiddle;

	[Space]
	public Color ColorReminderCorner;

	public Color ColorReminderMiddle;

	[Space]
	public Color ColorBadCorner;

	public Color ColorBadMiddle;

	[Space]
	public Sound sound;

	[Space]
	public AnimationCurve introCurve;

	public float introSpeed;

	public AnimationCurve outroCurve;

	public float outroSpeed;

	public float waitTime;

	private float waitTimer;

	private float animLerp;

	[Space]
	public RectTransform middle;

	private Vector2 middleTarget;

	private Vector2 middleTargetNew;

	private Vector2 middleTargetSize;

	private Vector2 middleTargetSizeNew;

	public RectTransform topLeft;

	private Vector2 topLeftTarget;

	private Vector2 topLeftTargetNew;

	public RectTransform topRight;

	private Vector2 topRightTarget;

	private Vector2 topRightTargetNew;

	public RectTransform botLeft;

	private Vector2 botLeftTarget;

	private Vector2 botLeftTargetNew;

	public RectTransform botRight;

	private Vector2 botRightTarget;

	private Vector2 botRightTargetNew;

	private void Start()
	{
		canvasRect = ValuableDiscover.instance.canvasRect;
		mainCamera = Camera.main;
		waitTimer = waitTime;
		if (state == State.Reminder)
		{
			waitTimer = waitTime * 0.5f;
		}
		if (state == State.Bad)
		{
			waitTimer = waitTime * 3f;
		}
	}

	private void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)target))
		{
			bool flag = true;
			Bounds bigBounds = default(Bounds);
			((Bounds)(ref bigBounds))._002Ector(target.centerPoint, Vector3.zero);
			MeshRenderer[] componentsInChildren = ((Component)target).GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer val in componentsInChildren)
			{
				((Bounds)(ref bigBounds)).Encapsulate(((Renderer)val).bounds);
			}
			if (SemiFunc.OnScreen(((Bounds)(ref bigBounds)).center, 0.5f, 0.5f))
			{
				Rect val2 = RendererBoundsInScreenSpace(bigBounds);
				if (((Rect)(ref val2)).width > 2f || ((Rect)(ref val2)).height > 2f)
				{
					topLeftTargetNew = ((Rect)(ref val2)).center;
					topRightTargetNew = ((Rect)(ref val2)).center;
					botLeftTargetNew = ((Rect)(ref val2)).center;
					botRightTargetNew = ((Rect)(ref val2)).center;
					middleTargetNew = ((Rect)(ref val2)).center;
					middleTargetSizeNew = new Vector2(0f, 0f);
				}
				else
				{
					topLeftTargetNew = Vector2.op_Implicit(GetScreenPosition(new Vector3(((Rect)(ref val2)).xMin, ((Rect)(ref val2)).yMax, 0f)));
					topRightTargetNew = Vector2.op_Implicit(GetScreenPosition(new Vector3(((Rect)(ref val2)).xMax, ((Rect)(ref val2)).yMax, 0f)));
					botLeftTargetNew = Vector2.op_Implicit(GetScreenPosition(new Vector3(((Rect)(ref val2)).xMin, ((Rect)(ref val2)).yMin, 0f)));
					botRightTargetNew = Vector2.op_Implicit(GetScreenPosition(new Vector3(((Rect)(ref val2)).xMax, ((Rect)(ref val2)).yMin, 0f)));
					middleTargetNew = Vector2.op_Implicit(GetScreenPosition(Vector2.op_Implicit(((Rect)(ref val2)).center)));
					middleTargetSizeNew = new Vector2(((Rect)(ref val2)).width * 1.9f + 0.025f, ((Rect)(ref val2)).height + 0.025f);
				}
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				if (first)
				{
					if (state == State.Reminder)
					{
						sound.Play(target.centerPoint, 0.3f);
					}
					else
					{
						sound.Play(target.centerPoint);
					}
					((Component)middle).gameObject.SetActive(true);
					((Component)topLeft).gameObject.SetActive(true);
					((Component)topRight).gameObject.SetActive(true);
					((Component)botLeft).gameObject.SetActive(true);
					((Component)botRight).gameObject.SetActive(true);
					first = false;
				}
				if (hidden)
				{
					middleTarget = middleTargetNew;
					middleTargetSize = middleTargetSizeNew;
					topLeftTarget = topLeftTargetNew;
					topRightTarget = topRightTargetNew;
					botLeftTarget = botLeftTargetNew;
					botRightTarget = botRightTargetNew;
					hidden = false;
				}
				middleTarget = Vector2.Lerp(middleTarget, middleTargetNew, 50f * Time.deltaTime);
				middleTargetSize = Vector2.Lerp(middleTargetSize, middleTargetSizeNew, 50f * Time.deltaTime);
				topLeftTarget = Vector2.Lerp(topLeftTarget, topLeftTargetNew, 50f * Time.deltaTime);
				topRightTarget = Vector2.Lerp(topRightTarget, topRightTargetNew, 50f * Time.deltaTime);
				botLeftTarget = Vector2.Lerp(botLeftTarget, botLeftTargetNew, 50f * Time.deltaTime);
				botRightTarget = Vector2.Lerp(botRightTarget, botRightTargetNew, 50f * Time.deltaTime);
			}
			else
			{
				hidden = true;
				topLeftTarget = middleTarget;
				topRightTarget = middleTarget;
				botLeftTarget = middleTarget;
				botRightTarget = middleTarget;
				middleTargetSize = Vector2.zero;
			}
		}
		else
		{
			waitTimer = 0f;
		}
		middle.anchoredPosition = middleTarget;
		if (waitTimer > 0f)
		{
			animLerp = Mathf.Clamp01(animLerp + introSpeed * Time.deltaTime);
			middle.sizeDelta = Vector2.LerpUnclamped(Vector2.zero, middleTargetSize, introCurve.Evaluate(animLerp));
			topLeft.anchoredPosition = Vector2.LerpUnclamped(middleTarget, topLeftTarget, introCurve.Evaluate(animLerp));
			topRight.anchoredPosition = Vector2.LerpUnclamped(middleTarget, topRightTarget, introCurve.Evaluate(animLerp));
			botLeft.anchoredPosition = Vector2.LerpUnclamped(middleTarget, botLeftTarget, introCurve.Evaluate(animLerp));
			botRight.anchoredPosition = Vector2.LerpUnclamped(middleTarget, botRightTarget, introCurve.Evaluate(animLerp));
			if (animLerp >= 1f)
			{
				waitTimer -= Time.deltaTime;
				if (waitTimer <= 0f)
				{
					animLerp = 0f;
				}
			}
		}
		else
		{
			animLerp = Mathf.Clamp01(animLerp + outroSpeed * Time.deltaTime);
			middle.sizeDelta = Vector2.LerpUnclamped(middleTargetSize, Vector2.zero, outroCurve.Evaluate(animLerp));
			topLeft.anchoredPosition = Vector2.LerpUnclamped(topLeftTarget, middleTarget, outroCurve.Evaluate(animLerp));
			topRight.anchoredPosition = Vector2.LerpUnclamped(topRightTarget, middleTarget, outroCurve.Evaluate(animLerp));
			botLeft.anchoredPosition = Vector2.LerpUnclamped(botLeftTarget, middleTarget, outroCurve.Evaluate(animLerp));
			botRight.anchoredPosition = Vector2.LerpUnclamped(botRightTarget, middleTarget, outroCurve.Evaluate(animLerp));
			if (animLerp >= 1f)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
		}
	}

	public void ReminderSetup()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		state = State.Reminder;
		((Graphic)((Component)middle).GetComponent<Image>()).color = ColorReminderMiddle;
		((Graphic)((Component)topLeft).GetComponent<Image>()).color = ColorReminderCorner;
		((Graphic)((Component)topRight).GetComponent<Image>()).color = ColorReminderCorner;
		((Graphic)((Component)botLeft).GetComponent<Image>()).color = ColorReminderCorner;
		((Graphic)((Component)botRight).GetComponent<Image>()).color = ColorReminderCorner;
	}

	public void BadSetup()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		state = State.Bad;
		((Graphic)((Component)middle).GetComponent<Image>()).color = ColorBadMiddle;
		((Graphic)((Component)topLeft).GetComponent<Image>()).color = ColorBadCorner;
		((Graphic)((Component)topRight).GetComponent<Image>()).color = ColorBadCorner;
		((Graphic)((Component)botLeft).GetComponent<Image>()).color = ColorBadCorner;
		((Graphic)((Component)botRight).GetComponent<Image>()).color = ColorBadCorner;
	}

	private Vector3 GetScreenPosition(Vector3 _position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(_position.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f, _position.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f, _position.z) / SemiFunc.UIMulti();
	}

	private Rect RendererBoundsInScreenSpace(Bounds bigBounds)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		if (screenSpaceCorners == null)
		{
			screenSpaceCorners = (Vector3[])(object)new Vector3[8];
		}
		screenSpaceCorners[0] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x + ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y + ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z + ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[1] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x + ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y + ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z - ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[2] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x + ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y - ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z + ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[3] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x + ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y - ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z - ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[4] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x - ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y + ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z + ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[5] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x - ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y + ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z - ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[6] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x - ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y - ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z + ((Bounds)(ref bigBounds)).extents.z));
		screenSpaceCorners[7] = mainCamera.WorldToViewportPoint(new Vector3(((Bounds)(ref bigBounds)).center.x - ((Bounds)(ref bigBounds)).extents.x, ((Bounds)(ref bigBounds)).center.y - ((Bounds)(ref bigBounds)).extents.y, ((Bounds)(ref bigBounds)).center.z - ((Bounds)(ref bigBounds)).extents.z));
		float x = screenSpaceCorners[0].x;
		float y = screenSpaceCorners[0].y;
		float x2 = screenSpaceCorners[0].x;
		float y2 = screenSpaceCorners[0].y;
		for (int i = 1; i < 8; i++)
		{
			if (screenSpaceCorners[i].x < x)
			{
				x = screenSpaceCorners[i].x;
			}
			if (screenSpaceCorners[i].y < y)
			{
				y = screenSpaceCorners[i].y;
			}
			if (screenSpaceCorners[i].x > x2)
			{
				x2 = screenSpaceCorners[i].x;
			}
			if (screenSpaceCorners[i].y > y2)
			{
				y2 = screenSpaceCorners[i].y;
			}
		}
		return Rect.MinMaxRect(x, y, x2, y2);
	}
}
