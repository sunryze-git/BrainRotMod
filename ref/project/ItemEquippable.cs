using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemEquippable : MonoBehaviourPunCallbacks
{
	public enum ItemState
	{
		Idle,
		Equipping,
		Equipped,
		Unequipping
	}

	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private ItemState currentState;

	public Sprite ItemIcon;

	private InventorySpot equippedSpot;

	private int ownerPlayerId = -1;

	internal bool isEquipped;

	internal bool isEquippedPrev;

	internal float wasEquippedTimer;

	internal bool isUnequipping;

	internal bool isEquipping;

	private float isUnequippingTimer;

	private float isEquippingTimer;

	public LayerMask ObstructionLayers;

	public SemiFunc.emojiIcon itemEmojiIcon;

	internal string itemEmoji;

	internal int inventorySpotIndex;

	internal float unequipTimer;

	internal float equipTimer;

	private const float animationDuration = 0.4f;

	private PhysGrabObject physGrabObject;

	private bool stateStart = true;

	private float itemEquipCubeShowTimer;

	private Vector3 teleportPosition;

	private float forceGrabTimer;

	internal PhysGrabber latestOwner;

	private Rigidbody rb => ((Component)this).GetComponent<Rigidbody>();

	private void Start()
	{
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
	}

	public bool IsEquipped()
	{
		return currentState == ItemState.Equipped;
	}

	private bool CollisionCheck()
	{
		return false;
	}

	public void RequestEquip(int spot, int requestingPlayerId = -1)
	{
		if (!IsEquipped() && currentState != ItemState.Unequipping)
		{
			if (SemiFunc.IsMultiplayer())
			{
				((MonoBehaviourPun)this).photonView.RPC("RPC_RequestEquip", (RpcTarget)2, new object[2] { spot, requestingPlayerId });
			}
			else
			{
				RPC_RequestEquip(spot, -1);
			}
		}
	}

	[PunRPC]
	private void RPC_RequestEquip(int spotIndex, int physGrabberPhotonViewID)
	{
		bool flag = SemiFunc.IsMultiplayer();
		if (currentState == ItemState.Idle)
		{
			if (flag)
			{
				((MonoBehaviourPun)this).photonView.RPC("RPC_UpdateItemState", (RpcTarget)0, new object[3] { 2, spotIndex, physGrabberPhotonViewID });
			}
			else
			{
				RPC_UpdateItemState(2, spotIndex, physGrabberPhotonViewID);
			}
		}
	}

	[PunRPC]
	private void RPC_UpdateItemState(int state, int spotIndex, int ownerId)
	{
		bool num = SemiFunc.IsMultiplayer();
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView obj = PhotonView.Find(ownerId);
			playerAvatar = ((obj != null) ? ((Component)obj).GetComponent<PlayerAvatar>() : null);
		}
		InventorySpot inventorySpot = null;
		if (num)
		{
			if (PhysGrabber.instance.photonView.ViewID == ownerId)
			{
				if (spotIndex != -1)
				{
					inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
				}
			}
			else
			{
				inventorySpot = null;
			}
		}
		else if (spotIndex != -1)
		{
			inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
		}
		bool flag = false;
		if ((Object)(object)inventorySpot == (Object)null)
		{
			flag = true;
		}
		if ((Object)(object)inventorySpot != (Object)null && inventorySpot.IsOccupied())
		{
			flag = true;
		}
		if (Inventory.instance.IsItemEquipped(this))
		{
			flag = true;
		}
		if (state == 2)
		{
			string instanceName = ((Component)this).GetComponent<ItemAttributes>().instanceName;
			StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, instanceName, spotIndex);
			currentState = ItemState.Equipped;
			if (!flag)
			{
				equippedSpot = inventorySpot;
				equippedSpot?.EquipItem(this);
			}
			else
			{
				equippedSpot = null;
			}
		}
		else
		{
			equippedSpot?.UnequipItem();
			equippedSpot = null;
		}
		inventorySpotIndex = spotIndex;
		currentState = (ItemState)state;
		ownerPlayerId = ownerId;
		stateStart = true;
		UpdateVisuals();
	}

	private void IsEquippingAndUnequippingTimer()
	{
		if (isEquippingTimer > 0f)
		{
			if (isEquippingTimer <= 0f)
			{
				isEquipping = false;
			}
			isEquippingTimer -= Time.deltaTime;
		}
		if (isUnequippingTimer > 0f)
		{
			if (isUnequippingTimer <= 0f)
			{
				isUnequipping = false;
			}
			isUnequippingTimer -= Time.deltaTime;
		}
	}

	public void RequestUnequip()
	{
		if (IsEquipped())
		{
			currentState = ItemState.Unequipping;
			if (SemiFunc.IsMultiplayer())
			{
				((MonoBehaviourPun)this).photonView.RPC("RPC_StartUnequip", (RpcTarget)0, new object[1] { ownerPlayerId });
			}
			else
			{
				RPC_StartUnequip(ownerPlayerId);
			}
		}
	}

	[PunRPC]
	private void RPC_StartUnequip(int requestingPlayerId)
	{
		if (ownerPlayerId == requestingPlayerId && (!SemiFunc.IsMultiplayer() || PhysGrabber.instance.photonView.ViewID == ownerPlayerId))
		{
			PerformUnequip(requestingPlayerId);
		}
	}

	private void PerformUnequip(int requestingPlayerId)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		unequipTimer = 0.4f;
		SetRotation();
		currentState = ItemState.Unequipping;
		physGrabObject.OverrideDeactivateReset();
		if (SemiFunc.IsMultiplayer())
		{
			RayHitTestNew(1f);
			((MonoBehaviourPun)this).photonView.RPC("RPC_CompleteUnequip", (RpcTarget)2, new object[2] { requestingPlayerId, teleportPosition });
		}
		else
		{
			RayHitTestNew(1f);
			RPC_CompleteUnequip(requestingPlayerId, teleportPosition);
		}
	}

	private bool RayHitTestNew(float distance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		int num = LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) & ~LayerMask.GetMask(new string[2] { "Ignore Raycast", "CollisionCheck" });
		RaycastHit val = default(RaycastHit);
		if (Object.op_Implicit((Object)(object)Camera.main) && Physics.Raycast(((Component)Camera.main).transform.position, ((Component)Camera.main).transform.forward, ref val, distance, num))
		{
			teleportPosition = ((RaycastHit)(ref val)).point;
		}
		else
		{
			teleportPosition = ((Component)Camera.main).transform.position + ((Component)Camera.main).transform.forward * distance;
		}
		return CollisionCheck();
	}

	private bool RayHitTest(float distance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		int num = LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()) & ~LayerMask.GetMask(new string[2] { "Ignore Raycast", "CollisionCheck" });
		if (Object.op_Implicit((Object)(object)Camera.main))
		{
			RaycastHit val = default(RaycastHit);
			Physics.Raycast(((Component)Camera.main).transform.position, ((Component)Camera.main).transform.forward, ref val, distance, num);
		}
		return CollisionCheck();
	}

	private Vector3 GetUnequipPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	[PunRPC]
	private void RPC_CompleteUnequip(int physGrabberPhotonViewID, Vector3 teleportPos)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		PhysGrabber physGrabber = ((!SemiFunc.IsMultiplayer()) ? PhysGrabber.instance : ((Component)PhotonView.Find(physGrabberPhotonViewID)).GetComponent<PhysGrabber>());
		StatsManager.instance.PlayerInventoryUpdate(physGrabber.playerAvatar.steamID, "", inventorySpotIndex);
		Transform visionTransform = physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform;
		physGrabObject.Teleport(teleportPos, Quaternion.LookRotation(((Component)visionTransform).transform.forward, Vector3.up));
		rb.isKinematic = false;
		int num = (((Object)(object)equippedSpot != (Object)null) ? equippedSpot.inventorySpotIndex : (-1));
		equippedSpot?.UnequipItem();
		equippedSpot = null;
		ownerPlayerId = -1;
		if (SemiFunc.IsMultiplayer())
		{
			((MonoBehaviourPun)this).photonView.RPC("RPC_UpdateItemState", (RpcTarget)0, new object[3] { 3, num, physGrabberPhotonViewID });
		}
		else
		{
			RPC_UpdateItemState(3, num, -1);
		}
	}

	private void UpdateVisuals()
	{
		if (currentState == ItemState.Equipped)
		{
			SetItemActive(isActive: false);
		}
		else if (currentState == ItemState.Idle)
		{
			SetItemActive(isActive: true);
		}
		else if (currentState == ItemState.Unequipping)
		{
			((MonoBehaviour)this).StartCoroutine(AnimateUnequip());
		}
	}

	private void SetItemActive(bool isActive)
	{
	}

	private IEnumerator AnimateUnequip()
	{
		float duration = 0.2f;
		float elapsed = 0f;
		Vector3 originalScale = ((Component)this).transform.localScale;
		Vector3 targetScale = Vector3.one;
		List<Collider> colliders = new List<Collider>();
		colliders.AddRange(((Component)this).GetComponents<Collider>());
		colliders.AddRange(((Component)this).GetComponentsInChildren<Collider>());
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			physGrabObject.OverrideMass(0.1f);
		}
		isUnequipping = true;
		isUnequippingTimer = 0.2f;
		Collider _unequipCollider = null;
		bool _hasUnequipCollider = false;
		foreach (Collider item in colliders)
		{
			PhysGrabObjectBoxCollider component = ((Component)item).GetComponent<PhysGrabObjectBoxCollider>();
			if (Object.op_Implicit((Object)(object)component) && component.unEquipCollider)
			{
				item.enabled = true;
				_hasUnequipCollider = true;
				_unequipCollider = item;
				colliders.Remove(item);
				break;
			}
		}
		if (_hasUnequipCollider)
		{
			foreach (Collider item2 in colliders)
			{
				item2.enabled = false;
			}
		}
		else
		{
			foreach (Collider item3 in colliders)
			{
				item3.enabled = true;
			}
		}
		while (elapsed < duration)
		{
			float num = elapsed / duration;
			((Component)this).transform.localScale = Vector3.Lerp(originalScale, targetScale, num);
			elapsed += Time.deltaTime;
			yield return null;
		}
		if (_hasUnequipCollider)
		{
			_unequipCollider.enabled = false;
			foreach (Collider item4 in colliders)
			{
				item4.enabled = true;
			}
		}
		isUnequipping = false;
		isUnequippingTimer = 0f;
		((Component)this).transform.localScale = targetScale;
		ForceGrab();
		forceGrabTimer = 0.2f;
	}

	private void ForceGrab()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			PhysGrabber.instance.OverrideGrab(physGrabObject);
		}
		else if (PhysGrabber.instance.photonView.ViewID == ownerPlayerId)
		{
			PhysGrabber.instance.OverrideGrab(physGrabObject);
		}
	}

	private IEnumerator AnimateEquip()
	{
		float duration = 0.1f;
		float elapsed = 0f;
		Vector3 originalScale = ((Component)this).transform.localScale;
		Vector3 targetScale = originalScale * 0.01f;
		List<Collider> list = new List<Collider>();
		list.AddRange(((Component)this).GetComponents<Collider>());
		list.AddRange(((Component)this).GetComponentsInChildren<Collider>());
		isEquipping = true;
		isEquippingTimer = 0.2f;
		foreach (Collider item in list)
		{
			item.enabled = false;
		}
		while (elapsed < duration)
		{
			float num = elapsed / duration;
			((Component)this).transform.localScale = Vector3.Lerp(originalScale, targetScale, num);
			elapsed += Time.deltaTime;
			yield return null;
		}
		isEquipping = false;
		isEquippingTimer = 0f;
		((Component)this).transform.localScale = targetScale;
	}

	public void ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != 0)
		{
			dropPosition += Random.insideUnitSphere * 0.2f;
			if (SemiFunc.IsMultiplayer())
			{
				((MonoBehaviourPun)this).photonView.RPC("RPC_ForceUnequip", (RpcTarget)0, new object[2] { dropPosition, physGrabberPhotonViewID });
			}
			else
			{
				RPC_ForceUnequip(dropPosition, physGrabberPhotonViewID);
			}
		}
	}

	[PunRPC]
	private void RPC_ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView obj = PhotonView.Find(physGrabberPhotonViewID);
			playerAvatar = ((obj != null) ? ((Component)obj).GetComponent<PlayerAvatar>() : null);
		}
		if (currentState != 0)
		{
			ownerPlayerId = -1;
			currentState = ItemState.Unequipping;
			StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, "", inventorySpotIndex);
			if (Object.op_Implicit((Object)(object)equippedSpot))
			{
				equippedSpot.UnequipItem();
				equippedSpot = null;
			}
			UpdateVisuals();
			physGrabObject.OverrideDeactivateReset();
			physGrabObject.Teleport(dropPosition, Quaternion.identity);
			((MonoBehaviour)this).StartCoroutine(AnimateUnequip());
			SetItemActive(isActive: true);
		}
	}

	private void WasEquippedTimer()
	{
		if (isEquippedPrev != isEquipped)
		{
			wasEquippedTimer = 0.5f;
			isEquippedPrev = isEquipped;
		}
		if (wasEquippedTimer > 0f)
		{
			wasEquippedTimer -= Time.deltaTime;
		}
	}

	private void Update()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		WasEquippedTimer();
		IsEquippingAndUnequippingTimer();
		switch (currentState)
		{
		case ItemState.Idle:
			StateIdle();
			break;
		case ItemState.Equipping:
			StateEquipping();
			break;
		case ItemState.Equipped:
			StateEquipped();
			break;
		case ItemState.Unequipping:
			StateUnequipping();
			break;
		}
		if (unequipTimer > 0f)
		{
			unequipTimer -= Time.deltaTime;
		}
		if (equipTimer > 0f)
		{
			equipTimer -= Time.deltaTime;
		}
		if (itemEquipCubeShowTimer > 0f)
		{
			itemEquipCubeShowTimer -= Time.deltaTime;
			if (itemEquipCubeShowTimer <= 0f)
			{
				Vector3 localScale = ((Component)this).transform.localScale;
				((Component)this).transform.localScale = Vector3.one;
				((Component)this).transform.localScale = localScale;
			}
		}
	}

	private void StateIdleStart()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateIdle()
	{
		if (currentState != 0)
		{
			return;
		}
		StateIdleStart();
		isEquipped = false;
		if (forceGrabTimer > 0f)
		{
			forceGrabTimer -= Time.deltaTime;
			if (forceGrabTimer <= 0f)
			{
				ForceGrab();
			}
		}
	}

	private void StateEquippingStart()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateEquipping()
	{
		if (currentState == ItemState.Equipping)
		{
			StateEquippingStart();
			currentState = ItemState.Equipped;
			isEquipped = true;
		}
	}

	private void StateEquippedStart()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			AssetManager.instance.soundEquip.Play(physGrabObject.midPoint);
			((MonoBehaviour)this).StartCoroutine(AnimateEquip());
		}
	}

	private void StateEquipped()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (currentState != ItemState.Equipped)
		{
			return;
		}
		StateEquippedStart();
		foreach (PhysGrabber item in physGrabObject.playerGrabbing)
		{
			item.OverrideGrabRelease();
		}
		if (!isEquipped)
		{
			equipTimer = 0.5f;
		}
		isEquipped = true;
		Vector3 localScale = ((Component)physGrabObject).transform.localScale;
		if (((Vector3)(ref localScale)).magnitude < 0.1f)
		{
			physGrabObject.OverrideDeactivate();
		}
	}

	private void StateUnequippingStart()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateUnequipping()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		AssetManager.instance.soundUnequip.Play(physGrabObject.midPoint);
		if (currentState == ItemState.Unequipping)
		{
			currentState = ItemState.Idle;
			isEquipped = false;
		}
	}

	private void SetRotation()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((Component)physGrabObject).transform.rotation = Quaternion.LookRotation(((Component)Camera.main).transform.forward, Vector3.up);
		physGrabObject.rb.rotation = ((Component)physGrabObject).transform.rotation;
	}

	private void OnDestroy()
	{
		if (Object.op_Implicit((Object)(object)equippedSpot))
		{
			equippedSpot.UnequipItem();
		}
	}
}
