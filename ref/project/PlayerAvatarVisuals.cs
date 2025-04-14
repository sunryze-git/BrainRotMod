using UnityEngine;

public class PlayerAvatarVisuals : MonoBehaviour
{
	public bool isMenuAvatar;

	[Space]
	public PlayerAvatar playerAvatar;

	public GameObject meshParent;

	private Animator animator;

	private bool animSprinting;

	private bool animSliding;

	private bool animSlidingImpulse;

	private bool animJumping;

	private bool animJumpingImpulse;

	private float animJumpTimer;

	private float animJumpedTimer;

	private float animFallingTimer;

	internal bool animInCrawl;

	internal bool animTumbling;

	internal PlayerAvatarTalkAnimation playerAvatarTalkAnimation;

	internal PlayerAvatarRightArm playerAvatarRightArm;

	[Space]
	public Transform headUpTransform;

	public Transform headSideTransform;

	public Transform TTSTransform;

	[Space]
	public Transform bodyTopUpTransform;

	public Transform bodyTopSideTransform;

	[Space]
	public GameObject PhysRiderPoint;

	public PlayerEyes playerEyes;

	private GameObject PhysRiderPointInstance;

	[Space]
	public ParticleSystem[] powerupJumpEffect;

	public ParticleSystem[] tumbleBreakFreeEffect;

	public Transform effectGetIntoTruck;

	private float effectGetIntoTruckTimer;

	[Space]
	public GameObject arenaCrown;

	public Transform leanTransform;

	public SpringQuaternion leanSpring;

	private Vector3 leanSpringTargetPrevious;

	[Space]
	public Transform tiltTransform;

	public SpringQuaternion tiltSpring;

	private bool tiltSprinting;

	private float tiltTimer;

	private Vector3 tiltTarget;

	[Space]
	public SpringQuaternion bodySpring;

	[HideInInspector]
	public Quaternion bodySpringTarget;

	public Transform legTwistTransform;

	public SpringQuaternion legTwistSpring;

	private bool legTwistActive;

	public Transform headLookAtTransform;

	public SpringFloat lookUpSpring;

	public SpringQuaternion lookSideSpring;

	public Transform attachPointJawTop;

	public Transform attachPointJawBottom;

	public Transform attachPointTopHeadMiddle;

	private Vector3 positionLast;

	internal Vector3 visualPosition = Vector3.zero;

	private float visualFollowLerp;

	internal float turnDifference;

	internal float turnDirection;

	private float turnPrevious;

	internal float upDifference;

	internal float upDirection;

	private float upPrevious;

	internal float animationSpeedMultiplier = 1f;

	internal float deltaTime;

	internal Color color;

	internal bool colorSet;

	private bool crownSetterWasHere;

	private void Start()
	{
		playerAvatarRightArm = ((Component)this).GetComponentInChildren<PlayerAvatarRightArm>();
		playerAvatarTalkAnimation = ((Component)this).GetComponentInChildren<PlayerAvatarTalkAnimation>();
		animator = ((Component)this).GetComponent<Animator>();
		animator.keepAnimatorStateOnDisable = true;
		if (!isMenuAvatar && (!GameManager.Multiplayer() || (Object.op_Implicit((Object)(object)this.playerAvatar) && this.playerAvatar.photonView.IsMine)))
		{
			((Behaviour)animator).enabled = false;
			meshParent.SetActive(false);
		}
		if (!SemiFunc.IsMultiplayer() || SemiFunc.RunIsArena())
		{
			return;
		}
		PlayerAvatar playerAvatar = SessionManager.instance.CrownedPlayerGet();
		if (!isMenuAvatar)
		{
			if ((Object)(object)playerAvatar == (Object)(object)this.playerAvatar)
			{
				arenaCrown.SetActive(true);
			}
		}
		else if ((Object)(object)playerAvatar == (Object)(object)PlayerAvatar.instance)
		{
			arenaCrown.SetActive(true);
		}
	}

	private void Update()
	{
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_094c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (SemiFunc.FPSImpulse5() && !crownSetterWasHere && Object.op_Implicit((Object)(object)PlayerCrownSet.instance) && PlayerCrownSet.instance.crownOwnerFetched)
		{
			if (Object.op_Implicit((Object)(object)playerAvatar) && PlayerCrownSet.instance.crownOwnerSteamID == playerAvatar.steamID)
			{
				arenaCrown.SetActive(true);
			}
			crownSetterWasHere = true;
		}
		deltaTime = Time.deltaTime * animationSpeedMultiplier;
		deltaTime = Mathf.Max(deltaTime, 0f);
		if (isMenuAvatar)
		{
			MenuAvatarGetColorsFromRealAvatar();
		}
		if (!isMenuAvatar && playerAvatar.isDisabled)
		{
			((Component)this).gameObject.SetActive(false);
			return;
		}
		Vector3 val;
		if (!isMenuAvatar)
		{
			if (!GameManager.Multiplayer() || playerAvatar.photonView.IsMine)
			{
				if (Object.op_Implicit((Object)(object)playerAvatar))
				{
					((Component)this).transform.position = ((Component)playerAvatar).transform.position;
					((Component)this).transform.rotation = ((Component)playerAvatar).transform.rotation;
				}
			}
			else
			{
				if (playerAvatar.isTumbling && Object.op_Implicit((Object)(object)playerAvatar.tumble))
				{
					visualFollowLerp = 0f;
					visualPosition = playerAvatar.tumble.followPosition.position;
					bodySpringTarget = playerAvatar.tumble.followPosition.rotation;
					playerAvatar.clientPosition = visualPosition;
					playerAvatar.clientPositionCurrent = visualPosition;
				}
				else if (!playerAvatar.clientPhysRiding || !Object.op_Implicit((Object)(object)PhysRiderPointInstance))
				{
					float num = Mathf.Lerp(0f, 25f, visualFollowLerp);
					visualFollowLerp = Mathf.Clamp01(visualFollowLerp + 2f * deltaTime);
					visualPosition = Vector3.Lerp(visualPosition, playerAvatar.clientPositionCurrent, num * deltaTime);
				}
				else if (Object.op_Implicit((Object)(object)PhysRiderPointInstance))
				{
					float num2 = Mathf.Lerp(0f, 25f, visualFollowLerp);
					visualFollowLerp = Mathf.Clamp01(visualFollowLerp + 2f * deltaTime);
					visualPosition = Vector3.Lerp(visualPosition, PhysRiderPointInstance.transform.position, num2 * deltaTime);
					playerAvatar.clientPosition = visualPosition;
					playerAvatar.clientPositionCurrent = visualPosition;
				}
				if (!playerAvatar.isTumbling)
				{
					if (animSliding)
					{
						if (animSlidingImpulse && ((Vector3)(ref playerAvatar.rbVelocity)).magnitude > 0.1f)
						{
							val = ((Component)this).transform.TransformDirection(playerAvatar.rbVelocity);
							bodySpringTarget = Quaternion.LookRotation(((Vector3)(ref val)).normalized, Vector3.up);
						}
					}
					else
					{
						bodySpringTarget = playerAvatar.clientRotationCurrent;
					}
					((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(bodySpring, bodySpringTarget, deltaTime);
				}
				else if (playerAvatar.tumble.tumbleSetTimer <= 0f)
				{
					bodySpring.lastRotation = bodySpringTarget;
					((Component)this).transform.rotation = bodySpringTarget;
				}
				((Component)this).transform.position = visualPosition;
				if (playerAvatar.playerHealth.hurtFreeze)
				{
					animator.speed = 0f;
					return;
				}
				turnDifference = Quaternion.Angle(Quaternion.Euler(0f, turnPrevious, 0f), Quaternion.Euler(0f, ((Quaternion)(ref bodySpringTarget)).eulerAngles.y, 0f));
				float num3 = turnPrevious - ((Quaternion)(ref bodySpringTarget)).eulerAngles.y;
				if (Mathf.Abs(num3) < 180f)
				{
					turnDirection = Mathf.Sign(num3);
				}
				if (playerAvatar.isTumbling)
				{
					turnDifference = 0f;
				}
				turnPrevious = ((Quaternion)(ref bodySpringTarget)).eulerAngles.y;
			}
		}
		if (!isMenuAvatar && (!GameManager.Multiplayer() || playerAvatar.photonView.IsMine))
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)playerEyes) && playerEyes.lookAtActive && GameDirector.instance.currentState == GameDirector.gameState.Main && Object.op_Implicit((Object)(object)playerAvatar) && Object.op_Implicit((Object)(object)playerAvatar.PlayerVisionTarget) && Object.op_Implicit((Object)(object)playerAvatar.PlayerVisionTarget.VisionTransform))
		{
			Vector3 val2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			Vector3 forward = playerAvatar.localCameraTransform.forward;
			if (Object.op_Implicit((Object)(object)playerAvatar.tumble) && playerAvatar.tumble.isTumbling)
			{
				forward = ((Component)playerAvatar.tumble).transform.forward;
			}
			if (isMenuAvatar)
			{
				val2 = ((Component)this).transform.position + Vector3.up * 1.5f;
				forward = ((Component)this).transform.forward;
			}
			Vector3 direction = playerEyes.lookAt.position - val2;
			direction = SemiFunc.ClampDirection(direction, forward, 40f);
			headLookAtTransform.rotation = Quaternion.Slerp(headLookAtTransform.rotation, Quaternion.LookRotation(direction), deltaTime * 15f);
		}
		else
		{
			headLookAtTransform.localRotation = Quaternion.Slerp(headLookAtTransform.localRotation, Quaternion.identity, deltaTime * 15f);
		}
		float num4 = 0f;
		if (!playerAvatar.isTumbling && !isMenuAvatar)
		{
			num4 = ((Quaternion)(ref playerAvatar.localCameraRotation)).eulerAngles.x;
			if (num4 > 90f)
			{
				num4 -= 360f;
			}
			if (playerAvatar.isCrawling)
			{
				num4 *= 0.5f;
			}
		}
		float num5 = headLookAtTransform.localEulerAngles.x;
		if (num5 > 90f)
		{
			num5 -= 360f;
		}
		if (isMenuAvatar)
		{
			num5 *= 1.25f;
		}
		num4 += num5;
		float num6 = SemiFunc.SpringFloatGet(lookUpSpring, num4, deltaTime);
		headUpTransform.localRotation = Quaternion.Euler(num6 * 0.5f, 0f, 0f);
		bodyTopUpTransform.localRotation = Quaternion.Euler(num6 * 0.25f, 0f, 0f);
		upDifference = Quaternion.Angle(Quaternion.Euler(upPrevious, 0f, 0f), Quaternion.Euler(headUpTransform.eulerAngles.x, 0f, 0f));
		float num7 = upPrevious - headUpTransform.eulerAngles.x;
		if (Mathf.Abs(num7) < 180f)
		{
			upDirection = Mathf.Sign(num7);
		}
		upPrevious = headUpTransform.eulerAngles.x;
		float num8 = 0f;
		if (turnDifference > 0.5f && turnDirection != 0f)
		{
			num8 = turnDifference * (0f - turnDirection) * 25f;
		}
		Quaternion localRotation = headLookAtTransform.localRotation;
		Quaternion val3 = Quaternion.Euler(0f, ((Quaternion)(ref localRotation)).eulerAngles.y + num8, 0f);
		val3 = Quaternion.Slerp(Quaternion.identity, val3, 0.5f);
		Quaternion val4 = SemiFunc.SpringQuaternionGet(lookSideSpring, val3, deltaTime);
		headSideTransform.localRotation = val4;
		bodyTopSideTransform.localRotation = Quaternion.Slerp(Quaternion.identity, val4, 0.5f);
		Vector3 zero = Vector3.zero;
		if (isMenuAvatar && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance) && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance.rb))
		{
			val = PlayerAvatarMenu.instance.rb.angularVelocity;
			if (Mathf.Abs(((Vector3)(ref val)).magnitude) > 1f)
			{
				zero.z = PlayerAvatarMenu.instance.rb.angularVelocity.y * 0.01f;
				goto IL_099e;
			}
		}
		if (((Vector3)(ref playerAvatar.rbVelocity)).magnitude > 0.1f)
		{
			Vector3 val5 = ((Component)this).transform.TransformDirection(playerAvatar.rbVelocity);
			if (Vector3.Dot(((Vector3)(ref val5)).normalized, ((Component)this).transform.forward) < -0.5f)
			{
				zero.x = -3f;
			}
			if (Vector3.Dot(((Vector3)(ref val5)).normalized, ((Component)this).transform.forward) > 0.5f)
			{
				zero.x = 3f;
			}
			if (Vector3.Dot(((Vector3)(ref val5)).normalized, ((Component)this).transform.right) > 0.5f)
			{
				zero.z = -3f;
			}
			if (Vector3.Dot(((Vector3)(ref val5)).normalized, ((Component)this).transform.right) < -0.5f)
			{
				zero.z = 3f;
			}
		}
		goto IL_099e;
		IL_099e:
		if (tiltSprinting != animSprinting)
		{
			if (tiltSprinting)
			{
				tiltTimer = 0.25f;
				tiltTarget = leanSpringTargetPrevious * 2f;
			}
			else
			{
				tiltTimer = 0.25f;
				tiltTarget = zero * 3f;
			}
			tiltSprinting = animSprinting;
		}
		leanTransform.localRotation = SemiFunc.SpringQuaternionGet(leanSpring, Quaternion.Euler(zero), deltaTime);
		tiltTransform.localRotation = SemiFunc.SpringQuaternionGet(tiltSpring, Quaternion.Euler(tiltTarget), deltaTime);
		if (tiltTimer > 0f)
		{
			tiltTimer -= deltaTime;
			if (tiltTimer <= 0f)
			{
				tiltTarget = Vector3.zero;
			}
		}
		leanSpringTargetPrevious = zero;
		bool flag = false;
		float num9 = 15f;
		float num10 = 0.5f;
		Vector3 val6 = Vector3.zero;
		if (isMenuAvatar && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance) && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance.rb))
		{
			val = PlayerAvatarMenu.instance.rb.angularVelocity;
			if (Mathf.Abs(((Vector3)(ref val)).magnitude) > 1f)
			{
				flag = true;
				num9 = 10f;
				num10 = 0.7f;
				Vector3 val7 = Quaternion.Euler(0f, (0f - PlayerAvatarMenu.instance.rb.angularVelocity.y) * 0.1f, 0f) * Vector3.forward;
				val7.y = 0f;
				val6 = val7;
				goto IL_0ba8;
			}
		}
		if (playerAvatar.isMoving && !animJumping && ((Vector3)(ref playerAvatar.rbVelocity)).magnitude > 0.1f)
		{
			flag = true;
			num9 = 10f;
			num10 = 0.7f;
			Vector3 normalized = ((Vector3)(ref playerAvatar.rbVelocity)).normalized;
			normalized.y = 0f;
			val6 = normalized;
		}
		goto IL_0ba8;
		IL_0ba8:
		if (legTwistActive != flag)
		{
			legTwistActive = flag;
			legTwistSpring.speed = num9;
			legTwistSpring.damping = num10;
		}
		else
		{
			legTwistSpring.speed = Mathf.Lerp(legTwistSpring.speed, num9, deltaTime * 5f);
			legTwistSpring.damping = Mathf.Lerp(legTwistSpring.damping, num10, deltaTime * 5f);
		}
		Quaternion targetRotation = Quaternion.identity;
		if (val6 != Vector3.zero)
		{
			targetRotation = Quaternion.LookRotation(val6, Vector3.up);
		}
		legTwistTransform.localRotation = SemiFunc.SpringQuaternionGet(legTwistSpring, targetRotation, deltaTime);
		AnimationLogic();
	}

	private void MenuAvatarGetColorsFromRealAvatar()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (isMenuAvatar && !Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = PlayerAvatar.instance;
		}
		if (Object.op_Implicit((Object)(object)playerAvatar) && playerAvatar.playerAvatarVisuals.color != color)
		{
			SetColor(-1, playerAvatar.playerAvatarVisuals.color);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy((Object)(object)PhysRiderPointInstance);
	}

	private void AnimationLogic()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		if (isMenuAvatar && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance) && Object.op_Implicit((Object)(object)PlayerAvatarMenu.instance.rb))
		{
			Vector3 angularVelocity = PlayerAvatarMenu.instance.rb.angularVelocity;
			if (Mathf.Abs(((Vector3)(ref angularVelocity)).magnitude) > 1f)
			{
				animator.SetBool("Turning", true);
			}
			else
			{
				animator.SetBool("Turning", false);
			}
			return;
		}
		bool flag = false;
		if (playerAvatar.isTumbling)
		{
			if (!animSprinting && !animTumbling)
			{
				animator.SetTrigger("TumblingImpulse");
				animTumbling = true;
			}
			if ((((Vector3)(ref playerAvatar.tumble.physGrabObject.rbVelocity)).magnitude > 1f && !playerAvatar.tumble.physGrabObject.impactDetector.inCart) || ((Vector3)(ref playerAvatar.tumble.physGrabObject.rbAngularVelocity)).magnitude > 1f)
			{
				animator.SetBool("TumblingMove", true);
				flag = true;
			}
			else
			{
				animator.SetBool("TumblingMove", false);
			}
			animator.SetBool("Tumbling", true);
		}
		else
		{
			animator.SetBool("Tumbling", false);
			animator.SetBool("TumblingMove", false);
			animTumbling = false;
		}
		if (playerAvatar.isCrouching || playerAvatar.isTumbling)
		{
			animator.SetBool("Crouching", true);
		}
		else
		{
			animator.SetBool("Crouching", false);
		}
		if (playerAvatar.isCrawling || playerAvatar.isTumbling)
		{
			animator.SetBool("Crawling", true);
		}
		else
		{
			animator.SetBool("Crawling", false);
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Crouch to Crawl"))
		{
			currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Crawl"))
			{
				currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Crawl Move"))
				{
					currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					if (!((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Slide"))
					{
						animInCrawl = false;
						goto IL_025d;
					}
				}
			}
		}
		animInCrawl = true;
		goto IL_025d;
		IL_025d:
		if (playerAvatar.isMoving && !animJumping)
		{
			animator.SetBool("Moving", true);
		}
		else
		{
			animator.SetBool("Moving", false);
		}
		if (!playerAvatar.isMoving && !animJumping && Mathf.Abs(turnDifference) > 0.25f)
		{
			animator.SetBool("Turning", true);
		}
		else
		{
			animator.SetBool("Turning", false);
		}
		if (playerAvatar.isSprinting && !animJumping && !animTumbling)
		{
			if (!animSprinting && !animSliding)
			{
				animator.SetTrigger("SprintingImpulse");
				animSprinting = true;
			}
			animator.SetBool("Sprinting", true);
		}
		else
		{
			animator.SetBool("Sprinting", false);
			animSprinting = false;
		}
		animSlidingImpulse = false;
		if (playerAvatar.isSliding && !animJumping && !animTumbling)
		{
			if (!animSliding)
			{
				animSlidingImpulse = true;
				animator.SetTrigger("SlidingImpulse");
			}
			animator.SetBool("Sliding", true);
			animSliding = true;
		}
		else
		{
			animator.SetBool("Sliding", false);
			animSliding = false;
		}
		if (animJumping)
		{
			if (animJumpingImpulse)
			{
				animJumpTimer = 0.2f;
				animJumpingImpulse = false;
				animator.SetTrigger("JumpingImpulse");
				animator.SetBool("Jumping", true);
				animator.SetBool("Falling", false);
			}
			else if (playerAvatar.rbVelocityRaw.y < -0.5f && animJumpTimer <= 0f)
			{
				animator.SetBool("Falling", true);
			}
			if (playerAvatar.isGrounded && animJumpTimer <= 0f)
			{
				animJumpedTimer = 0.5f;
				animJumping = false;
			}
			animJumpTimer -= deltaTime;
		}
		else
		{
			animator.SetBool("Jumping", false);
			animator.SetBool("Falling", false);
		}
		if (animJumpedTimer > 0f)
		{
			animJumpedTimer -= deltaTime;
		}
		if (!playerAvatar.isGrounded)
		{
			animFallingTimer += deltaTime;
		}
		else
		{
			animFallingTimer = 0f;
		}
		if (!playerAvatar.isCrawling && !animJumping && !animSliding && !animTumbling && animFallingTimer > 0.25f && animJumpedTimer <= 0f)
		{
			animJumpTimer = 0.2f;
			animJumping = true;
			animJumpingImpulse = false;
			animator.SetTrigger("FallingImpulse");
			animator.SetBool("Jumping", true);
			animator.SetBool("Falling", true);
		}
		if (flag)
		{
			float num = Mathf.Max(((Vector3)(ref playerAvatar.tumble.physGrabObject.rbVelocity)).magnitude, ((Vector3)(ref playerAvatar.tumble.physGrabObject.rbAngularVelocity)).magnitude) * 0.5f;
			num = Mathf.Clamp(num, 0.5f, 1.25f);
			animator.speed = num * animationSpeedMultiplier;
			playerAvatar.tumble.TumbleMoveSoundSet(flag, num);
			return;
		}
		currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName("Sprint"))
		{
			float num2 = 1f + (float)StatsManager.instance.playerUpgradeSpeed[playerAvatar.steamID] * 0.1f;
			animator.speed = num2 * animationSpeedMultiplier;
		}
		else if (playerAvatar.isMoving && playerAvatar.mapToolController.Active)
		{
			animator.speed = 0.5f * animationSpeedMultiplier;
		}
		else
		{
			animator.speed = 1f * animationSpeedMultiplier;
		}
	}

	public void JumpImpulse()
	{
		if (!playerAvatar.isCrawling && !animTumbling)
		{
			animJumpingImpulse = true;
			animJumping = true;
		}
	}

	public void PhysRidingCheck()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (Object)(object)PhysRiderPointInstance != (Object)null;
		if (flag && (Object)(object)PhysRiderPointInstance.transform.parent != (Object)(object)playerAvatar.clientPhysRidingTransform)
		{
			Object.Destroy((Object)(object)PhysRiderPointInstance);
			flag = false;
		}
		if (!flag)
		{
			PhysRiderPointInstance = Object.Instantiate<GameObject>(PhysRiderPoint, Vector3.zero, Quaternion.identity, playerAvatar.clientPhysRidingTransform);
		}
		PhysRiderPointInstance.transform.localPosition = playerAvatar.clientPhysRidingPosition;
	}

	public void SetColor(int _colorIndex, Color _setColor = default(Color))
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Color val;
		if (_colorIndex != -1)
		{
			val = AssetManager.instance.playerColors[_colorIndex];
		}
		else
		{
			val = _setColor;
			flag = true;
		}
		int num = Shader.PropertyToID("_AlbedoColor");
		color = val;
		if (!flag)
		{
			playerAvatar.playerHealth.bodyMaterial.SetColor(num, val);
		}
		else
		{
			PlayerHealth componentInParent = ((Component)this).GetComponentInParent<PlayerHealth>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				componentInParent.bodyMaterial.SetColor(num, val);
			}
		}
		if (SemiFunc.RunIsLobbyMenu() && Object.op_Implicit((Object)(object)MenuPageLobby.instance))
		{
			foreach (MenuPlayerListed menuPlayerListed in MenuPageLobby.instance.menuPlayerListedList)
			{
				if ((Object)(object)menuPlayerListed.playerAvatar == (Object)(object)playerAvatar)
				{
					menuPlayerListed.playerHead.SetColor(val);
					break;
				}
			}
		}
		colorSet = true;
	}

	public void Revive()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		bodySpringTarget = playerAvatar.clientRotationCurrent;
		bodySpring.lastRotation = bodySpringTarget;
		turnPrevious = ((Quaternion)(ref bodySpringTarget)).eulerAngles.y;
		playerAvatar.isCrawling = true;
		playerAvatar.isCrouching = true;
		playerAvatar.isTumbling = false;
		playerAvatar.isMoving = false;
		playerAvatar.isSprinting = false;
		visualFollowLerp = 1f;
		animator.Play("Crawl");
		animInCrawl = true;
		animator.SetBool("Crouching", true);
		animator.SetBool("Crawling", true);
		animator.SetBool("Moving", false);
		animator.SetBool("Sprinting", false);
		animator.SetBool("Sliding", false);
		animator.SetBool("Jumping", false);
		animator.SetBool("Falling", false);
		animator.SetBool("Turning", false);
		animator.SetBool("Tumbling", false);
	}

	public void PowerupJumpEffect()
	{
		ParticleSystem[] array = powerupJumpEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void TumbleBreakFreeEffect()
	{
		ParticleSystem[] array = tumbleBreakFreeEffect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void FootstepLight()
	{
		playerAvatar.Footstep(Materials.SoundType.Light);
	}

	public void FootstepMedium()
	{
		if (!isMenuAvatar)
		{
			playerAvatar.Footstep(Materials.SoundType.Medium);
		}
	}

	public void FootstepHeavy()
	{
		playerAvatar.Footstep(Materials.SoundType.Heavy);
	}

	public void StandToCrouch()
	{
		playerAvatar.StandToCrouch();
	}

	public void CrouchToStand()
	{
		playerAvatar.CrouchToStand();
	}

	public void CrouchToCrawl()
	{
		playerAvatar.CrouchToCrawl();
	}

	public void CrawlToCrouch()
	{
		playerAvatar.CrawlToCrouch();
	}
}
