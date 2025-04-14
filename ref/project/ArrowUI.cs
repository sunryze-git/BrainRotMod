using UnityEngine;

public class ArrowUI : MonoBehaviour
{
	public static ArrowUI instance;

	public AnimationCurve arrowCurveMove;

	private float arrowCurveMoveEval;

	public AnimationCurve arrowCurveBop;

	private float showArrowTimer;

	private Vector3 startPosition;

	private Vector3 endPosition;

	private float endRotation;

	private bool endShow;

	public MeshRenderer arrowMesh;

	private float bopEval;

	private bool targetWorldPos;

	private Camera mainCamera;

	private void Awake()
	{
		instance = this;
		((Renderer)arrowMesh).enabled = false;
		mainCamera = Camera.main;
	}

	public void ArrowShow(Vector3 startPos, Vector3 endPos, float rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (endPosition != endPos)
		{
			arrowCurveMoveEval = 0f;
			((Component)this).transform.localPosition = startPos;
		}
		startPos.z = 0f;
		endPos.z = 0f;
		targetWorldPos = false;
		startPosition = startPos;
		endPosition = endPos;
		endRotation = rotation;
		showArrowTimer = 0.2f;
	}

	public void ArrowShowWorldPos(Vector3 startPos, Vector3 endPos, float rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (endPosition != endPos)
		{
			arrowCurveMoveEval = 0f;
			((Component)this).transform.position = startPos;
		}
		targetWorldPos = true;
		startPosition = startPos;
		endPosition = endPos;
		endRotation = rotation;
		showArrowTimer = 0.2f;
	}

	private void Update()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		if (targetWorldPos)
		{
			Vector3 val = mainCamera.WorldToScreenPoint(endPosition);
			endPosition = ((Vector3)(ref val)).normalized;
			endPosition.z = 0f;
		}
		bopEval += Time.deltaTime;
		bopEval = Mathf.Clamp01(bopEval);
		float num = arrowCurveBop.Evaluate(bopEval);
		((Component)arrowMesh).transform.localPosition = new Vector3(-51f + -30f * num, 0f, 0f);
		if (bopEval >= 1f)
		{
			bopEval = 0f;
		}
		if (showArrowTimer > 0f)
		{
			((Renderer)arrowMesh).enabled = true;
			endShow = false;
			showArrowTimer -= Time.deltaTime;
			arrowCurveMoveEval += Time.deltaTime;
			arrowCurveMoveEval = Mathf.Clamp01(arrowCurveMoveEval);
			float num2 = arrowCurveMove.Evaluate(arrowCurveMoveEval);
			((Component)this).transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, num2);
			((Component)this).transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(90f, endRotation, num2));
			float num3 = arrowCurveMove.Evaluate(arrowCurveMoveEval * 2f);
			((Component)this).transform.localScale = new Vector3(num3, num3, num3);
			return;
		}
		if (!endShow)
		{
			arrowCurveMoveEval = 0f;
			endShow = true;
		}
		if (arrowCurveMoveEval >= 1f)
		{
			((Renderer)arrowMesh).enabled = false;
			arrowCurveMoveEval = 1f;
			return;
		}
		arrowCurveMoveEval += Time.deltaTime * 4f;
		float num4 = arrowCurveMove.Evaluate(arrowCurveMoveEval);
		((Component)this).transform.localScale = new Vector3(1f - num4, 1f - num4, 1f - num4);
		startPosition = Vector3.zero;
		endPosition = Vector3.one;
	}
}
