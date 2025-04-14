using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIPlayerName : WorldSpaceUIChild
{
	public TextMeshProUGUI text;

	internal PlayerAvatar playerAvatar;

	private Vector3 followTarget;

	private float followTargetY;

	private float showTimer;

	private float showTimeTotal;

	private float showTimeTotalResetTimer;

	private void OnDisable()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)text).color = new Color(1f, 1f, 1f, 0f);
	}

	protected override void Update()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!Object.op_Implicit((Object)(object)playerAvatar))
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		if (showTimeTotalResetTimer > 0f)
		{
			showTimeTotalResetTimer -= Time.deltaTime;
			if (showTimeTotalResetTimer <= 0f)
			{
				showTimeTotal = 0f;
			}
		}
		if (Object.op_Implicit((Object)(object)SpectateCamera.instance) || playerAvatar.isDisabled || showTimer <= 0f)
		{
			((Graphic)text).color = Color.Lerp(((Graphic)text).color, new Color(1f, 1f, 1f, 0f), Time.deltaTime * 20f);
		}
		else
		{
			showTimer -= Time.deltaTime;
			((Graphic)text).color = Color.Lerp(((Graphic)text).color, new Color(1f, 1f, 1f, 0.5f), Time.deltaTime * 5f);
		}
		Vector3 position = playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
		position.y = ((Component)playerAvatar.playerAvatarVisuals).transform.position.y;
		if ((Object)(object)playerAvatar == (Object)(object)SessionManager.instance.CrownedPlayerGet())
		{
			position.y += 0.02f;
		}
		followTarget = Vector3.Lerp(followTarget, position, Time.deltaTime * 30f);
		float num = playerAvatar.playerAvatarVisuals.headLookAtTransform.position.y - ((Component)playerAvatar.playerAvatarVisuals).transform.position.y + 0.35f;
		if (Mathf.Abs(followTargetY - num) > 0.2f)
		{
			followTargetY = Mathf.Lerp(followTargetY, num, Time.deltaTime * 20f);
		}
		else
		{
			followTargetY = Mathf.Lerp(followTargetY, num, Time.deltaTime * 3f);
		}
		worldPosition = followTarget + Vector3.up * followTargetY;
		float num2 = Vector3.Distance(worldPosition, ((Component)Camera.main).transform.position);
		((TMP_Text)text).fontSize = 20f - num2;
	}

	public void Show()
	{
		showTimeTotal += 0.25f;
		showTimeTotalResetTimer = 0.5f;
		if (showTimeTotal >= 1f)
		{
			showTimer = 0.5f;
		}
	}
}
