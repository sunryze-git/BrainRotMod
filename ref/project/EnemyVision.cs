using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class EnemyVision : MonoBehaviour
{
	private Enemy Enemy;

	internal bool HasVision;

	internal float DisableTimer;

	internal float StandOverrideTimer;

	private float VisionTimer;

	private float VisionCheckTime = 0.25f;

	public Transform VisionTransform;

	[Header("Base")]
	public float VisionDistance = 10f;

	public Dictionary<int, int> VisionsTriggered = new Dictionary<int, int>();

	public Dictionary<int, bool> VisionTriggered = new Dictionary<int, bool>();

	[Header("Close")]
	public float VisionDistanceClose = 3.5f;

	public float VisionDistanceCloseCrouch = 2f;

	[Header("Dot")]
	public float VisionDotStanding = 0.4f;

	public float VisionDotCrouch = 0.6f;

	public float VisionDotCrawl = 0.9f;

	[Header("Phys Object Vision")]
	public bool PhysObjectVision = true;

	private float PhysObjectVisionRadius = 10f;

	public float PhysObjectVisionRadiusOverride = -1f;

	public float PhysObjectVisionDot = 0.4f;

	[Header("Triggers")]
	public int VisionsToTrigger = 4;

	public int VisionsToTriggerCrouch = 10;

	public int VisionsToTriggerCrawl = 20;

	[Header("Events")]
	public UnityEvent onVisionTriggered;

	internal int onVisionTriggeredID;

	internal PlayerAvatar onVisionTriggeredPlayer;

	internal bool onVisionTriggeredCulled;

	internal bool onVisionTriggeredNear;

	internal float onVisionTriggeredDistance;

	private bool VisionLogicActive;

	private void Awake()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
		((MonoBehaviour)this).StartCoroutine(Vision());
		VisionLogicActive = true;
	}

	private void OnDisable()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		VisionLogicActive = false;
	}

	private void OnEnable()
	{
		if (!VisionLogicActive)
		{
			((MonoBehaviour)this).StartCoroutine(Vision());
			VisionLogicActive = true;
		}
	}

	public void PlayerAdded(int photonID)
	{
		VisionsTriggered.TryAdd(photonID, 0);
		VisionTriggered.TryAdd(photonID, value: false);
	}

	public void PlayerRemoved(int photonID)
	{
		VisionsTriggered.Remove(photonID);
		VisionTriggered.Remove(photonID);
	}

	private IEnumerator Vision()
	{
		VisionLogicActive = true;
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		while (VisionsTriggered.Count == 0)
		{
			yield return (object)new WaitForSeconds(VisionCheckTime);
		}
		while (true)
		{
			if (DisableTimer > 0f || EnemyDirector.instance.debugNoVision)
			{
				DisableTimer -= Time.deltaTime;
				foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
				{
					int viewID = player.photonView.ViewID;
					VisionTriggered[viewID] = false;
					VisionsTriggered[viewID] = 0;
				}
				yield return null;
				continue;
			}
			if (!Enemy.HasStateChaseBegin || Enemy.CurrentState != EnemyState.ChaseBegin)
			{
				HasVision = false;
				bool[] array = new bool[GameDirector.instance.PlayerList.Count];
				if (PhysObjectVision)
				{
					float num = PhysObjectVisionRadius;
					if (PhysObjectVisionRadiusOverride > 0f)
					{
						num = PhysObjectVisionRadiusOverride;
					}
					Collider[] array2 = Physics.OverlapSphere(VisionTransform.position, num, LayerMask.op_Implicit(SemiFunc.LayerMaskGetPhysGrabObject()));
					foreach (Collider val in array2)
					{
						if (!((Component)val).CompareTag("Phys Grab Object"))
						{
							continue;
						}
						PhysGrabObject componentInParent = ((Component)val).GetComponentInParent<PhysGrabObject>();
						if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.playerGrabbing.Count <= 0)
						{
							continue;
						}
						Vector3 val2 = componentInParent.centerPoint - VisionTransform.position;
						RaycastHit[] array3 = Physics.RaycastAll(VisionTransform.position, val2, ((Vector3)(ref val2)).magnitude, LayerMask.op_Implicit(Enemy.VisionMask));
						bool flag = true;
						if (array3.Length != 0)
						{
							RaycastHit[] array4 = array3;
							for (int j = 0; j < array4.Length; j++)
							{
								RaycastHit val3 = array4[j];
								if (((Component)((RaycastHit)(ref val3)).transform).CompareTag("Phys Grab Object") || ((Component)((RaycastHit)(ref val3)).transform).CompareTag("Enemy"))
								{
									PhysGrabObject componentInParent2 = ((Component)((RaycastHit)(ref val3)).transform).GetComponentInParent<PhysGrabObject>();
									if (Object.op_Implicit((Object)(object)componentInParent2) && ((Object)(object)componentInParent2 == (Object)(object)componentInParent || (Enemy.HasRigidbody && (Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponentInParent<EnemyRigidbody>() == (Object)(object)Enemy.Rigidbody)))
									{
										continue;
									}
								}
								flag = false;
							}
						}
						if (!flag || !(Vector3.Dot(VisionTransform.forward, ((Vector3)(ref val2)).normalized) >= PhysObjectVisionDot))
						{
							continue;
						}
						int num2 = 0;
						foreach (PlayerAvatar player2 in GameDirector.instance.PlayerList)
						{
							if ((Object)(object)player2 == (Object)(object)componentInParent.playerGrabbing[0].playerAvatar)
							{
								array[num2] = true;
							}
							num2++;
						}
					}
				}
				int num3 = 0;
				foreach (PlayerAvatar player3 in GameDirector.instance.PlayerList)
				{
					bool flag2 = false;
					if (player3.isDisabled)
					{
						continue;
					}
					int viewID2 = player3.photonView.ViewID;
					if (player3.enemyVisionFreezeTimer > 0f)
					{
						if (VisionTriggered[viewID2])
						{
							VisionTrigger(viewID2, player3, culled: false, playerNear: false);
						}
						num3++;
						continue;
					}
					VisionTriggered[viewID2] = false;
					float num4 = Vector3.Distance(VisionTransform.position, ((Component)player3).transform.position);
					if (!array[num3] && num4 > VisionDistance)
					{
						continue;
					}
					bool flag3 = player3.isCrawling;
					bool flag4 = player3.isCrouching;
					if (player3.isTumbling)
					{
						flag4 = true;
						flag3 = false;
					}
					if (StandOverrideTimer > 0f)
					{
						flag4 = false;
						flag3 = false;
					}
					Transform val4 = null;
					Transform val5 = null;
					Vector3 val6 = ((Component)player3.PlayerVisionTarget.VisionTransform).transform.position - VisionTransform.position;
					Collider[] array5 = Physics.OverlapSphere(VisionTransform.position, 0.01f, LayerMask.op_Implicit(Enemy.VisionMask));
					if (array5.Length != 0)
					{
						Collider[] array2 = array5;
						foreach (Collider val7 in array2)
						{
							if (!((Component)((Component)val7).transform).CompareTag("Enemy"))
							{
								if (((Component)((Component)val7).transform).CompareTag("Player"))
								{
									val4 = ((Component)val7).transform;
								}
								if (Object.op_Implicit((Object)(object)((Component)((Component)val7).transform).GetComponentInParent<PlayerTumble>()))
								{
									val4 = ((Component)val7).transform;
								}
								if (!Object.op_Implicit((Object)(object)((Component)((Component)val7).transform).GetComponentInParent<EnemyRigidbody>()))
								{
									val5 = ((Component)val7).transform;
								}
							}
						}
					}
					if (!Object.op_Implicit((Object)(object)val5))
					{
						RaycastHit[] array6 = Physics.RaycastAll(VisionTransform.position, val6, VisionDistance, LayerMask.op_Implicit(Enemy.VisionMask));
						float num5 = 1000f;
						RaycastHit[] array4 = array6;
						for (int i = 0; i < array4.Length; i++)
						{
							RaycastHit val8 = array4[i];
							if (((Component)((RaycastHit)(ref val8)).transform).CompareTag("Enemy"))
							{
								continue;
							}
							if (((Component)((RaycastHit)(ref val8)).transform).CompareTag("Player"))
							{
								val4 = ((RaycastHit)(ref val8)).transform;
							}
							if (!Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val8)).transform).GetComponentInParent<EnemyRigidbody>()))
							{
								if (Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val8)).transform).GetComponentInParent<PlayerTumble>()))
								{
									val4 = ((RaycastHit)(ref val8)).transform;
								}
								float num6 = Vector3.Distance(VisionTransform.position, ((RaycastHit)(ref val8)).point);
								if (num6 < num5)
								{
									num5 = num6;
									val5 = ((RaycastHit)(ref val8)).transform;
								}
							}
						}
					}
					if (array[num3] || (Object.op_Implicit((Object)(object)val4) && (Object)(object)val4 == (Object)(object)val5))
					{
						float num7 = Vector3.Dot(VisionTransform.forward, ((Vector3)(ref val6)).normalized);
						bool flag5 = false;
						if (flag4)
						{
							if (num4 <= VisionDistanceCloseCrouch)
							{
								flag5 = true;
							}
						}
						else if (num4 <= VisionDistanceClose)
						{
							flag5 = true;
						}
						if (flag5)
						{
							VisionsTriggered[viewID2] = VisionsToTrigger;
						}
						bool flag6 = false;
						if (flag3 && Enemy.CurrentState != EnemyState.LookUnder)
						{
							if (num7 >= VisionDotCrawl)
							{
								flag6 = true;
							}
						}
						else if (flag4 && Enemy.CurrentState != EnemyState.LookUnder)
						{
							if (num7 >= VisionDotCrouch)
							{
								flag6 = true;
							}
						}
						else if (num7 >= VisionDotStanding)
						{
							flag6 = true;
						}
						if (array[num3] || flag6 || flag5)
						{
							flag2 = true;
							bool flag7 = false;
							if (flag3 && Enemy.CurrentState != EnemyState.LookUnder)
							{
								if (VisionsTriggered[viewID2] >= VisionsToTriggerCrawl)
								{
									flag7 = true;
								}
							}
							else if (flag4 && Enemy.CurrentState != EnemyState.LookUnder)
							{
								if (VisionsTriggered[viewID2] >= VisionsToTriggerCrouch)
								{
									flag7 = true;
								}
							}
							else if (VisionsTriggered[viewID2] >= VisionsToTrigger)
							{
								flag7 = true;
							}
							bool culled = false;
							if (Enemy.HasOnScreen)
							{
								if (GameManager.instance.gameMode == 0)
								{
									if (Enemy.OnScreen.CulledLocal)
									{
										culled = true;
									}
								}
								else if (Enemy.OnScreen.CulledPlayer[player3.photonView.ViewID])
								{
									culled = true;
								}
							}
							if (flag7 || flag5)
							{
								VisionTrigger(viewID2, player3, culled, flag5);
							}
						}
					}
					if (flag2)
					{
						VisionsTriggered[viewID2]++;
					}
					else
					{
						VisionsTriggered[viewID2] = 0;
					}
					num3++;
				}
			}
			if (StandOverrideTimer > 0f)
			{
				StandOverrideTimer -= VisionCheckTime;
			}
			yield return (object)new WaitForSeconds(VisionCheckTime);
		}
	}

	public void VisionTrigger(int playerID, PlayerAvatar player, bool culled, bool playerNear)
	{
		VisionTriggered[playerID] = true;
		VisionsTriggered[playerID] = Mathf.Max(VisionsTriggered[playerID], VisionsToTrigger);
		onVisionTriggeredID = playerID;
		onVisionTriggeredPlayer = player;
		onVisionTriggeredCulled = culled;
		onVisionTriggeredNear = playerNear;
		onVisionTriggered.Invoke();
	}

	public void DisableVision(float time)
	{
		DisableTimer = time;
	}

	public void StandOverride(float time)
	{
		StandOverrideTimer = time;
	}
}
