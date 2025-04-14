using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyOnScreen : MonoBehaviour
{
	private Enemy Enemy;

	private Camera MainCamera;

	public Transform[] points;

	[Space]
	public float maxDistance = 20f;

	[Space]
	public float paddingWidth = 0.1f;

	public float paddingHeight = 0.1f;

	private bool LogicActive;

	private float OnScreenTimer = 0.25f;

	internal bool OnScreenLocal;

	private bool OnScreenLocalPrevious;

	internal bool CulledLocal;

	private bool CulledLocalPrevious;

	internal bool OnScreenAny;

	internal bool CulledAny;

	internal Dictionary<int, bool> OnScreenPlayer = new Dictionary<int, bool>();

	internal Dictionary<int, bool> CulledPlayer = new Dictionary<int, bool>();

	private void Awake()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
		MainCamera = Camera.main;
		if (points.Length == 0)
		{
			points = (Transform[])(object)new Transform[1];
			points[0] = Enemy.CenterTransform;
		}
		LogicActive = true;
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private void OnEnable()
	{
		if (!LogicActive)
		{
			LogicActive = true;
			((MonoBehaviour)this).StartCoroutine(Logic());
		}
	}

	private void OnDisable()
	{
		LogicActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private IEnumerator Logic()
	{
		while (OnScreenPlayer.Count == 0)
		{
			yield return (object)new WaitForSeconds(OnScreenTimer);
		}
		RaycastHit val3 = default(RaycastHit);
		while (true)
		{
			CulledLocal = true;
			CulledAny = true;
			OnScreenLocal = false;
			OnScreenAny = false;
			Transform[] array = points;
			foreach (Transform val in array)
			{
				if (Vector3.Distance(val.position, ((Component)CameraUtils.Instance.MainCamera).transform.position) <= maxDistance && SemiFunc.OnScreen(val.position, paddingWidth, paddingHeight))
				{
					CulledLocal = false;
					CulledAny = false;
					Vector3 val2 = ((Component)MainCamera).transform.position - val.position;
					float num = Mathf.Min(Vector3.Distance(((Component)MainCamera).transform.position, val.position), 12f);
					if (!Physics.Raycast(val.position, val2, ref val3, num, LayerMask.op_Implicit(Enemy.VisionMask)) || ((Component)((RaycastHit)(ref val3)).transform).CompareTag("Player") || Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponent<PlayerTumble>()))
					{
						OnScreenLocal = true;
						OnScreenAny = true;
					}
				}
				if (OnScreenAny && !CulledAny)
				{
					break;
				}
			}
			if (GameManager.Multiplayer())
			{
				foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
				{
					if (!player.isDisabled && player.photonView.IsMine)
					{
						if (CulledLocal != CulledLocalPrevious || OnScreenLocal != OnScreenLocalPrevious)
						{
							CulledLocalPrevious = CulledLocal;
							OnScreenLocalPrevious = OnScreenLocal;
							OnScreenPlayerUpdate(player.photonView.ViewID, OnScreenLocal, CulledLocal);
						}
						break;
					}
				}
				foreach (PlayerAvatar player2 in GameDirector.instance.PlayerList)
				{
					if (!player2.isDisabled)
					{
						if (OnScreenPlayer[player2.photonView.ViewID])
						{
							OnScreenAny = true;
						}
						if (!CulledPlayer[player2.photonView.ViewID])
						{
							CulledAny = false;
						}
					}
				}
			}
			yield return (object)new WaitForSeconds(OnScreenTimer);
		}
	}

	public bool GetOnScreen(PlayerAvatar _playerAvatar)
	{
		if (!GameManager.Multiplayer())
		{
			return OnScreenLocal;
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if ((Object)(object)player == (Object)(object)_playerAvatar && OnScreenPlayer[player.photonView.ViewID])
			{
				return true;
			}
		}
		return false;
	}

	private void OnScreenPlayerUpdate(int playerID, bool onScreen, bool culled)
	{
		if (GameManager.instance.gameMode == 0)
		{
			OnScreenPlayerUpdateRPC(playerID, onScreen, culled);
			return;
		}
		Enemy.PhotonView.RPC("OnScreenPlayerUpdateRPC", (RpcTarget)0, new object[3] { playerID, onScreen, culled });
	}

	[PunRPC]
	private void OnScreenPlayerUpdateRPC(int playerID, bool onScreen, bool culled)
	{
		CulledPlayer[playerID] = culled;
		OnScreenPlayer[playerID] = onScreen;
	}

	public void PlayerAdded(int photonID)
	{
		OnScreenPlayer.TryAdd(photonID, value: false);
		CulledPlayer.TryAdd(photonID, value: false);
	}

	public void PlayerRemoved(int photonID)
	{
		OnScreenPlayer.Remove(photonID);
		CulledPlayer.Remove(photonID);
	}
}
