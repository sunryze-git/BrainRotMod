using UnityEngine;

public class WorldSpaceUIChild : MonoBehaviour
{
	internal Vector3 worldPosition;

	private RectTransform myRect;

	internal Vector3 positionOffset;

	protected virtual void Start()
	{
		myRect = ((Component)this).GetComponent<RectTransform>();
		SetPosition();
	}

	protected virtual void Update()
	{
		SetPosition();
	}

	private void SetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = SemiFunc.UIWorldToCanvasPosition(worldPosition);
		myRect.anchoredPosition = Vector2.op_Implicit(val + positionOffset);
	}
}
