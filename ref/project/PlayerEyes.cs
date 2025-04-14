using UnityEngine;

public class PlayerEyes : MonoBehaviour
{
	public bool debugDraw;

	private PlayerAvatarVisuals playerAvatarVisuals;

	private PlayerAvatar playerAvatar;

	private PlayerAvatarRightArm playerAvatarRightArm;

	public Transform menuAvatarPointer;

	public Transform eyeLeft;

	public Transform eyeRight;

	public Transform pupilLeft;

	public Transform pupilRight;

	[Space]
	public Transform targetIdle;

	public Transform targetLead;

	[Space]
	public Transform lookAt;

	public SpringQuaternion springQuaternionLeft;

	public SpringQuaternion springQuaternionRight;

	private Transform otherPhysGrabPoint;

	private float eyeLeadTimer;

	private float eyeSideAmount;

	private float eyeUpAmount;

	internal bool lookAtActive;

	private PlayerAvatar currentTalker;

	private float currentTalkerTime;

	private float currentTalkerTimer;

	private bool overrideActive;

	private float overrideTimer;

	private Vector3 overridePosition;

	private GameObject overrideObject;

	private bool overrideSoftActive;

	private float overrideSoftTimer;

	private Vector3 overrideSoftPosition;

	private GameObject overrideSoftObject;

	private float deltaTime;

	internal float pupilLeftSizeMultiplier = 1f;

	internal float pupilRightSizeMultiplier = 1f;

	internal float pupilSizeMultiplier = 1f;

	private void Start()
	{
		playerAvatarVisuals = ((Component)this).GetComponent<PlayerAvatarVisuals>();
		playerAvatar = playerAvatarVisuals.playerAvatar;
		playerAvatarRightArm = ((Component)this).GetComponent<PlayerAvatarRightArm>();
		if (!playerAvatarVisuals.isMenuAvatar && (!GameManager.Multiplayer() || (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.photonView.IsMine)))
		{
			((Behaviour)this).enabled = false;
		}
	}

	private void MenuLookAt()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatarVisuals.isMenuAvatar)
		{
			Override(menuAvatarPointer.position, 0.1f, ((Component)menuAvatarPointer).gameObject);
		}
	}

	private void LookAtTransform(Transform _otherPhysGrabPoint, bool _softOverride)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		lookAtActive = false;
		if (overrideActive)
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = overridePosition;
		}
		else if (playerAvatarRightArm.mapToolController.Active && playerAvatarRightArm.mapToolController.HideLerp <= 0f)
		{
			((Component)lookAt).transform.position = playerAvatarRightArm.mapToolController.PlayerLookTarget.position;
		}
		else if (((Behaviour)playerAvatarRightArm.physGrabBeam).isActiveAndEnabled && Object.op_Implicit((Object)(object)playerAvatar.physGrabber.grabbedObjectTransform) && !Object.op_Implicit((Object)(object)((Component)playerAvatar.physGrabber.grabbedObjectTransform).GetComponent<PhysGrabCart>()))
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = playerAvatarRightArm.physGrabBeam.PhysGrabPoint.position;
		}
		else if (playerAvatar.healthGrab.staticGrabObject.playerGrabbing.Count > 0)
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = ((Component)playerAvatarVisuals).transform.position + ((Component)playerAvatarVisuals).transform.forward * 2f;
		}
		else if (playerAvatar.isTumbling && playerAvatar.tumble.physGrabObject.playerGrabbing.Count > 0)
		{
			lookAtActive = true;
			Vector3 position = playerAvatar.tumble.physGrabObject.playerGrabbing[0].playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
			if (playerAvatar.isLocal)
			{
				position = playerAvatar.localCameraPosition;
			}
			((Component)lookAt).transform.position = position;
		}
		else if (Object.op_Implicit((Object)(object)_otherPhysGrabPoint))
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = _otherPhysGrabPoint.position;
		}
		else if (_softOverride)
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = overrideSoftPosition;
		}
		else if (Object.op_Implicit((Object)(object)currentTalker))
		{
			Vector3 position2 = currentTalker.playerAvatarVisuals.headLookAtTransform.position;
			if (currentTalker.isLocal)
			{
				position2 = currentTalker.localCameraPosition;
			}
			lookAtActive = true;
			((Component)lookAt).transform.position = position2;
		}
		else if (playerAvatar.isTumbling)
		{
			lookAtActive = true;
			((Component)lookAt).transform.position = ((Component)this).transform.position + playerAvatar.localCameraTransform.forward * 2f;
		}
		else
		{
			((Component)lookAt).transform.position = targetIdle.position;
		}
		((Component)lookAt).transform.rotation = targetIdle.rotation;
	}

	private void ClosestPhysGrabPoint()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		otherPhysGrabPoint = null;
		float num = 6f;
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if ((Object)(object)player != (Object)(object)playerAvatar && player.physGrabber.physGrabBeam.gameObject.activeSelf && Object.op_Implicit((Object)(object)player.physGrabber.grabbedObjectTransform) && !Object.op_Implicit((Object)(object)((Component)player.physGrabber.grabbedObjectTransform).GetComponent<PhysGrabCart>()))
			{
				float num2 = Vector3.Distance(player.physGrabber.physGrabPoint.position, eyeLeft.position);
				if (num2 < num)
				{
					num = num2;
					otherPhysGrabPoint = player.physGrabber.physGrabPoint;
				}
			}
		}
	}

	private void EyesLead()
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatarVisuals.turnDifference > 0.5f && playerAvatarVisuals.turnDirection != 0f)
		{
			eyeSideAmount = playerAvatarVisuals.turnDifference * (0f - playerAvatarVisuals.turnDirection) * 20f;
			eyeLeadTimer = 0.1f;
		}
		if (playerAvatarVisuals.upDifference > 0.5f && playerAvatarVisuals.upDirection != 0f)
		{
			eyeUpAmount = playerAvatarVisuals.upDifference * (0f - playerAvatarVisuals.upDirection) * 20f;
			eyeLeadTimer = 0.1f;
		}
		if (eyeLeadTimer > 0f)
		{
			eyeLeadTimer -= deltaTime;
			Vector3 localEulerAngles = default(Vector3);
			((Vector3)(ref localEulerAngles))._002Ector(eyeUpAmount, eyeSideAmount, 0f);
			Quaternion localRotation = targetLead.localRotation;
			targetLead.localEulerAngles = localEulerAngles;
			Quaternion localRotation2 = targetLead.localRotation;
			targetLead.localRotation = localRotation;
			targetLead.localRotation = Quaternion.Lerp(localRotation, localRotation2, deltaTime * 5f);
		}
		else
		{
			eyeSideAmount = 0f;
			eyeUpAmount = 0f;
			targetLead.localRotation = Quaternion.Lerp(targetLead.localRotation, Quaternion.identity, deltaTime * 20f);
		}
	}

	private void Update()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatarVisuals.isMenuAvatar && !Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = PlayerAvatar.instance;
		}
		if (!playerAvatarVisuals.isMenuAvatar && (!LevelGenerator.Instance.Generated || playerAvatar.playerHealth.hurtFreeze))
		{
			return;
		}
		deltaTime = playerAvatarVisuals.deltaTime;
		MenuLookAt();
		pupilLeft.localScale = Vector3.one * pupilSizeMultiplier * pupilLeftSizeMultiplier;
		pupilRight.localScale = Vector3.one * pupilSizeMultiplier * pupilRightSizeMultiplier;
		EyesLead();
		ClosestPhysGrabPoint();
		if (Object.op_Implicit((Object)(object)currentTalker) && !currentTalker.voiceChat.isTalking)
		{
			currentTalkerTime = 0f;
		}
		foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
		{
			if (!player.isDisabled && Object.op_Implicit((Object)(object)player.voiceChat) && player.voiceChat.isTalking && Vector3.Distance(((Component)this).transform.position, ((Component)player).transform.position) < 6f)
			{
				currentTalkerTimer = Random.Range(2f, 4f);
				if ((Object)(object)player != (Object)(object)playerAvatar && player.voiceChat.isTalkingStartTime > currentTalkerTime)
				{
					currentTalker = player;
					currentTalkerTime = player.voiceChat.isTalkingStartTime;
				}
			}
		}
		if (currentTalkerTimer > 0f)
		{
			currentTalkerTimer -= deltaTime;
			if (currentTalkerTimer <= 0f)
			{
				currentTalker = null;
			}
		}
		bool softOverride = false;
		if (overrideSoftActive)
		{
			softOverride = true;
			if (Object.op_Implicit((Object)(object)overrideSoftObject))
			{
				PlayerAvatar component = overrideSoftObject.GetComponent<PlayerAvatar>();
				if (Object.op_Implicit((Object)(object)component) && (Object)(object)component == (Object)(object)playerAvatar)
				{
					softOverride = false;
				}
				else
				{
					PlayerTumble component2 = overrideSoftObject.GetComponent<PlayerTumble>();
					if (Object.op_Implicit((Object)(object)component2) && (Object)(object)component2.playerAvatar == (Object)(object)playerAvatar)
					{
						softOverride = false;
					}
				}
			}
		}
		LookAtTransform(otherPhysGrabPoint, softOverride);
		if (overrideSoftTimer > 0f)
		{
			overrideSoftTimer -= deltaTime;
			if (overrideSoftTimer <= 0f)
			{
				overrideSoftActive = false;
			}
		}
		if (overrideTimer > 0f)
		{
			overrideTimer -= deltaTime;
			if (overrideTimer <= 0f)
			{
				overrideActive = false;
			}
		}
		EyeLookAt(ref eyeRight, ref springQuaternionRight, _useSpring: true, 50f, 30f);
		EyeLookAt(ref eyeLeft, ref springQuaternionLeft, _useSpring: true, 50f, 30f);
	}

	public void Override(Vector3 _position, float _time, GameObject _obj)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!overrideActive || !((Object)(object)_obj != (Object)(object)overrideObject))
		{
			overrideActive = true;
			overrideObject = _obj;
			overridePosition = _position;
			overrideTimer = _time;
		}
	}

	public void OverrideSoft(Vector3 _position, float _time, GameObject _obj)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!overrideSoftActive || !((Object)(object)_obj != (Object)(object)overrideSoftObject))
		{
			overrideSoftActive = true;
			overrideSoftObject = _obj;
			overrideSoftPosition = _position;
			overrideSoftTimer = _time;
		}
	}

	public void EyeLookAt(ref Transform _eyeTransform, ref SpringQuaternion _springQuaternion, bool _useSpring, float _clampX, float _clamY)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		Quaternion localRotation = _eyeTransform.localRotation;
		Vector3 val = SemiFunc.ClampDirection(lookAt.position - ((Component)_eyeTransform).transform.position, lookAt.forward, _clampX);
		_eyeTransform.rotation = Quaternion.LookRotation(val);
		float num = _eyeTransform.localEulerAngles.y;
		if (num > _clamY && num < 180f)
		{
			num = _clamY;
		}
		else if (num < 360f - _clamY && num > 180f)
		{
			num = 360f - _clamY;
		}
		else if (num < 0f - _clamY)
		{
			num = 0f - _clamY;
		}
		_eyeTransform.localEulerAngles = new Vector3(_eyeTransform.localEulerAngles.x, num, _eyeTransform.localEulerAngles.z);
		Quaternion localRotation2 = _eyeTransform.localRotation;
		_eyeTransform.localRotation = localRotation;
		if (_useSpring)
		{
			_eyeTransform.localRotation = SemiFunc.SpringQuaternionGet(_springQuaternion, localRotation2, deltaTime);
		}
		else
		{
			_eyeTransform.localRotation = localRotation2;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (debugDraw)
		{
			float num = 0.075f;
			Gizmos.color = new Color(1f, 0.93f, 0.99f, 0.6f);
			Gizmos.matrix = lookAt.localToWorldMatrix;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one * num);
			Gizmos.color = new Color(0f, 1f, 0.98f, 0.3f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one * num);
		}
	}
}
