using System.Collections.Generic;
using UnityEngine;

public class PlayerNameChecker : MonoBehaviour
{
	private float checkTimer;

	private void Update()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.currentState != GameDirector.gameState.Main || (Object.op_Implicit((Object)(object)Map.Instance) && Map.Instance.Active) || !GameplayManager.instance.playerNames)
		{
			return;
		}
		if (checkTimer <= 0f)
		{
			checkTimer = 0.25f;
			List<PlayerAvatar> list = new List<PlayerAvatar>();
			Camera main = Camera.main;
			RaycastHit[] array = Physics.SphereCastAll(((Component)main).transform.position, 0.25f, ((Component)main).transform.forward, 15f, LayerMask.GetMask(new string[1] { "PlayerVisuals" }), (QueryTriggerInteraction)2);
			RaycastHit val3 = default(RaycastHit);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit val = array[i];
				PlayerAvatarVisuals componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<PlayerAvatarVisuals>();
				if (!Object.op_Implicit((Object)(object)componentInParent))
				{
					continue;
				}
				PlayerAvatar playerAvatar = componentInParent.playerAvatar;
				if (!list.Contains(playerAvatar))
				{
					Vector3 val2 = ((Component)main).transform.position - ((RaycastHit)(ref val)).point;
					if (!Physics.Raycast(((RaycastHit)(ref val)).point, val2, ref val3, ((Vector3)(ref val2)).magnitude, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) - LayerMask.GetMask(new string[1] { "Player" }), (QueryTriggerInteraction)2))
					{
						playerAvatar.worldSpaceUIPlayerName.Show();
						list.Add(playerAvatar);
					}
				}
			}
		}
		else
		{
			checkTimer -= Time.deltaTime;
		}
	}
}
