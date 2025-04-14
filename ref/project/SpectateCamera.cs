using System.Linq;
using UnityEngine;

public class SpectateCamera : MonoBehaviour
{
	public enum State
	{
		Death,
		Normal
	}

	public static SpectateCamera instance;

	private State currentState;

	private float stateTimer;

	private bool stateImpulse = true;

	internal PlayerAvatar player;

	private float previousFarClipPlane = 0.01f;

	private float previousFieldOfView;

	private Camera MainCamera;

	private Camera TopCamera;

	private Transform ParentObject;

	private Transform PreviousParent;

	private float cameraFieldOfView = 10f;

	private int currentPlayerListIndex;

	private Transform deathPlayerSpectatePoint;

	private Vector3 deathCameraOffset;

	private float deathCameraNearClipPlane;

	private float deathCurrentY;

	private Vector3 deathVelocity;

	private float deathSpeed = 6f;

	private Vector3 deathFollowPoint;

	private Vector3 deathFollowPointTarget;

	private Vector3 deathFollowPointVelocity;

	private Vector3 deathSmoothLookAtPoint;

	private Quaternion deathSmoothOrbit;

	private Quaternion deathTargetOrbit;

	private bool deathOrbitInstantSet;

	private Vector3 deathPosition;

	public Transform normalTransformPivot;

	public Transform normalTransformDistance;

	private Vector3 normalPreviousPosition;

	private float normalAimHorizontal;

	private float normalAimVertical;

	private float normalMinDistance = 1f;

	private float normalMaxDistance = 3f;

	private float normalDistanceTarget;

	private float normalDistanceCheckTimer;

	private void Awake()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		instance = this;
		normalTransformPivot.parent = null;
		MainCamera = GameDirector.instance.MainCamera;
		Camera[] componentsInChildren = ((Component)MainCamera).GetComponentsInChildren<Camera>();
		foreach (Camera val in componentsInChildren)
		{
			if ((Object)(object)val != (Object)(object)MainCamera)
			{
				TopCamera = val;
			}
		}
		ParentObject = ((Component)CameraNoise.Instance).transform;
		PreviousParent = ParentObject.parent;
		ParentObject.parent = ((Component)this).transform;
		ParentObject.localPosition = Vector3.zero;
		ParentObject.localRotation = Quaternion.identity;
	}

	private void OnDestroy()
	{
		QualitySettings.shadowDistance = 15f;
	}

	private void LateUpdate()
	{
		SemiFunc.UIHideHealth();
		SemiFunc.UIHideEnergy();
		SemiFunc.UIHideInventory();
		SemiFunc.UIHideAim();
		SemiFunc.UIShowSpectate();
		MissionUI.instance.Hide();
		switch (currentState)
		{
		case State.Death:
			StateDeath();
			break;
		case State.Normal:
			StateNormal();
			break;
		}
		RoomVolumeLogic();
	}

	private void StateDeath()
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			((Component)MainCamera).transform.localPosition = new Vector3(0f, 0f, -50f);
			((Component)MainCamera).transform.localRotation = Quaternion.identity;
			MainCamera.nearClipPlane = 0.01f;
			previousFarClipPlane = MainCamera.farClipPlane;
			MainCamera.farClipPlane = 70f;
			MainCamera.farClipPlane = 90f;
			deathCameraNearClipPlane = 70f;
			MainCamera.nearClipPlane = 70f;
			QualitySettings.shadowDistance = 90f;
			RenderSettings.fog = false;
			PostProcessing.Instance.SpectateSet();
			DeathNearClipLogic(_instant: true);
			LightManager.instance.UpdateInstant();
			CameraGlitch.Instance.PlayShort();
			previousFieldOfView = MainCamera.fieldOfView;
			cameraFieldOfView = 8f;
			MainCamera.fieldOfView = 16f;
			TopCamera.fieldOfView = MainCamera.fieldOfView;
			stateImpulse = false;
			stateTimer = 4f;
			AudioManager.instance.AudioListener.TargetPositionTransform = deathPlayerSpectatePoint;
			GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
			GameDirector.instance.CameraShake.Shake(2f, 1f);
		}
		stateTimer -= Time.deltaTime;
		CameraNoise.Instance.Override(0.03f, 0.25f);
		deathCurrentY += SemiFunc.InputMouseX() * CameraAim.Instance.AimSpeedMouse;
		Vector3 position = ((Component)this).transform.position;
		if (Object.op_Implicit((Object)(object)deathPlayerSpectatePoint))
		{
			position = deathPlayerSpectatePoint.position;
		}
		if (CheckState(State.Death))
		{
			position = deathPosition;
		}
		Vector3 val = position;
		Quaternion val2 = Quaternion.Euler(88f, deathCurrentY, 0f);
		deathTargetOrbit = val2;
		float num = Mathf.Lerp(50f, 2.5f, GameplayManager.instance.cameraSmoothing / 100f);
		deathSmoothOrbit = Quaternion.Slerp(deathSmoothOrbit, deathTargetOrbit, num * Time.deltaTime);
		if (deathOrbitInstantSet)
		{
			deathSmoothOrbit = deathTargetOrbit;
			deathOrbitInstantSet = false;
		}
		val2 = deathSmoothOrbit;
		Vector3 val3 = val2 * Vector3.back * 2f;
		deathFollowPointTarget = val;
		deathFollowPoint = Vector3.SlerpUnclamped(deathFollowPoint, deathFollowPointTarget, Time.deltaTime * deathSpeed);
		((Component)this).transform.position = deathFollowPoint + val3;
		deathSmoothLookAtPoint = Vector3.SlerpUnclamped(deathSmoothLookAtPoint, position, Time.deltaTime * deathSpeed);
		((Component)this).transform.LookAt(deathSmoothLookAtPoint);
		MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, cameraFieldOfView, Time.deltaTime * 10f);
		TopCamera.fieldOfView = MainCamera.fieldOfView;
		DeathNearClipLogic(_instant: false);
		ExtractionPoint extractionPointCurrent = RoundDirector.instance.extractionPointCurrent;
		if (Object.op_Implicit((Object)(object)extractionPointCurrent) && extractionPointCurrent.currentState != ExtractionPoint.State.Idle && extractionPointCurrent.currentState != ExtractionPoint.State.Active)
		{
			bool flag = false;
			foreach (PlayerAvatar item in SemiFunc.PlayerGetList())
			{
				if (!item.isDisabled)
				{
					flag = false;
					break;
				}
				if (item.playerDeathHead.inExtractionPoint)
				{
					flag = true;
				}
			}
			if (flag)
			{
				stateTimer = Mathf.Clamp(stateTimer, 0.25f, stateTimer);
			}
		}
		if (!(stateTimer <= 0f))
		{
			return;
		}
		if (SemiFunc.RunIsTutorial())
		{
			foreach (PlayerAvatar item2 in SemiFunc.PlayerGetList())
			{
				item2.Revive();
			}
			return;
		}
		UpdateState(State.Normal);
	}

	private void StateNormal()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		if (stateImpulse)
		{
			PlayerSwitch();
			if (!Object.op_Implicit((Object)(object)player))
			{
				return;
			}
			RenderSettings.fog = true;
			MainCamera.farClipPlane = previousFarClipPlane;
			MainCamera.fieldOfView = previousFieldOfView;
			TopCamera.fieldOfView = MainCamera.fieldOfView;
			((Component)MainCamera).transform.localPosition = Vector3.zero;
			((Component)MainCamera).transform.localRotation = Quaternion.identity;
			AudioManager.instance.AudioListener.TargetPositionTransform = ((Component)MainCamera).transform;
			stateImpulse = false;
		}
		CameraNoise.Instance.Override(0.03f, 0.25f);
		float num = SemiFunc.InputMouseX();
		float num2 = SemiFunc.InputMouseY();
		float num3 = SemiFunc.InputScrollY();
		if (CameraAim.Instance.overrideAimStop)
		{
			num = 0f;
			num2 = 0f;
			num3 = 0f;
		}
		normalAimHorizontal += num * CameraAim.Instance.AimSpeedMouse * 1.5f;
		if (normalAimHorizontal > 360f)
		{
			normalAimHorizontal -= 360f;
		}
		if (normalAimHorizontal < -360f)
		{
			normalAimHorizontal += 360f;
		}
		float num4 = normalAimVertical;
		float num5 = (0f - num2 * CameraAim.Instance.AimSpeedMouse) * 1.5f;
		normalAimVertical += num5;
		normalAimVertical = Mathf.Clamp(normalAimVertical, -70f, 70f);
		if (num3 != 0f)
		{
			normalMaxDistance = Mathf.Clamp(normalMaxDistance - num3 * 0.0025f, normalMinDistance, 6f);
		}
		Vector3 val = normalPreviousPosition;
		if (Object.op_Implicit((Object)(object)player.spectatePoint))
		{
			val = player.spectatePoint.position;
		}
		else if (player.isTumbling)
		{
			val = player.tumble.physGrabObject.centerPoint;
		}
		else if (player.isCrouching && !player.isCrawling)
		{
			val += Vector3.up * 0.3f;
		}
		else if (!player.isCrawling)
		{
			val += Vector3.down * 0.15f;
		}
		normalPreviousPosition = val;
		normalTransformPivot.position = Vector3.Lerp(normalTransformPivot.position, val, 10f * Time.deltaTime);
		Quaternion val2 = Quaternion.Euler(normalAimVertical, normalAimHorizontal, 0f);
		float num6 = Mathf.Lerp(50f, 6.25f, GameplayManager.instance.cameraSmoothing / 100f);
		normalTransformPivot.rotation = Quaternion.Lerp(normalTransformPivot.rotation, val2, num6 * Time.deltaTime);
		Transform obj = normalTransformPivot;
		Quaternion localRotation = normalTransformPivot.localRotation;
		float x = ((Quaternion)(ref localRotation)).eulerAngles.x;
		localRotation = normalTransformPivot.localRotation;
		obj.localRotation = Quaternion.Euler(x, ((Quaternion)(ref localRotation)).eulerAngles.y, 0f);
		bool flag = false;
		float num7 = normalMaxDistance;
		RaycastHit[] array = Physics.SphereCastAll(normalTransformPivot.position, 0.1f, -normalTransformPivot.forward, normalMaxDistance, LayerMask.op_Implicit(SemiFunc.LayerMaskGetVisionObstruct()));
		if (array.Length != 0)
		{
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit val3 = array2[i];
				if (!Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponent<PlayerHealthGrab>()) && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponent<PlayerAvatar>()) && !Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref val3)).transform).GetComponent<PlayerTumble>()))
				{
					num7 = Mathf.Min(num7, ((RaycastHit)(ref val3)).distance);
					if (((Component)((RaycastHit)(ref val3)).transform).CompareTag("Wall"))
					{
						flag = true;
					}
					Bounds bounds = ((RaycastHit)(ref val3)).collider.bounds;
					Vector3 size = ((Bounds)(ref bounds)).size;
					if (((Vector3)(ref size)).magnitude > 2f)
					{
						flag = true;
					}
				}
			}
			normalDistanceTarget = Mathf.Max(normalMinDistance, num7);
		}
		else
		{
			normalDistanceTarget = normalMaxDistance;
		}
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(0f, 0f, 0f - normalDistanceTarget);
		normalTransformDistance.localPosition = Vector3.Lerp(normalTransformDistance.localPosition, val4, Time.deltaTime * 5f);
		float num8 = 0f - normalTransformDistance.localPosition.z;
		Vector3 val5 = normalTransformPivot.position - normalTransformDistance.position;
		float num9 = ((Vector3)(ref val5)).magnitude;
		RaycastHit val6 = default(RaycastHit);
		if (Physics.SphereCast(normalTransformDistance.position, 0.15f, val5, ref val6, normalMaxDistance, LayerMask.GetMask(new string[1] { "PlayerVisuals" }), (QueryTriggerInteraction)2))
		{
			num9 = ((RaycastHit)(ref val6)).distance;
		}
		num9 = num8 - num9 - 0.1f;
		if (flag)
		{
			float num10 = Mathf.Max(num7, num9);
			MainCamera.nearClipPlane = Mathf.Max(num8 - num10, 0.01f);
		}
		else
		{
			MainCamera.nearClipPlane = 0.01f;
		}
		RenderSettings.fogStartDistance = MainCamera.nearClipPlane;
		((Component)this).transform.position = normalTransformDistance.position;
		((Component)this).transform.rotation = normalTransformDistance.rotation;
		if (Object.op_Implicit((Object)(object)player) && ((Component)this).transform.position.y < ((Component)player).transform.position.y + 0.25f && num5 < 0f)
		{
			normalAimVertical = num4;
		}
		if (SemiFunc.InputDown(InputKey.Jump))
		{
			PlayerSwitch();
		}
		if (SemiFunc.InputDown(InputKey.SpectateNext))
		{
			PlayerSwitch();
		}
		if (SemiFunc.InputDown(InputKey.SpectatePrevious))
		{
			PlayerSwitch(_next: false);
		}
		if (Object.op_Implicit((Object)(object)player) && player.voiceChatFetched)
		{
			player.voiceChat.SpatialDisable(0.1f);
		}
	}

	private void UpdateState(State _state)
	{
		if (currentState != _state)
		{
			currentState = _state;
			stateImpulse = true;
			stateTimer = 0f;
		}
	}

	public bool CheckState(State _state)
	{
		return currentState == _state;
	}

	private void PlayerSwitch(bool _next = true)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (GameDirector.instance.PlayerList.All((PlayerAvatar p) => p.isDisabled))
		{
			return;
		}
		int i = 0;
		int num = currentPlayerListIndex;
		for (int count = GameDirector.instance.PlayerList.Count; i < count; i++)
		{
			num = ((!_next) ? ((num - 1 + count) % count) : ((num + 1) % count));
			PlayerAvatar playerAvatar = GameDirector.instance.PlayerList[num];
			if ((Object)(object)player != (Object)(object)playerAvatar && !playerAvatar.isDisabled)
			{
				currentPlayerListIndex = num;
				player = playerAvatar;
				normalTransformPivot.position = player.spectatePoint.position;
				normalAimHorizontal = ((Component)player).transform.eulerAngles.y;
				normalAimVertical = 0f;
				normalTransformPivot.rotation = Quaternion.Euler(normalAimVertical, normalAimHorizontal, 0f);
				Transform obj = normalTransformPivot;
				Quaternion localRotation = normalTransformPivot.localRotation;
				float x = ((Quaternion)(ref localRotation)).eulerAngles.x;
				localRotation = normalTransformPivot.localRotation;
				obj.localRotation = Quaternion.Euler(x, ((Quaternion)(ref localRotation)).eulerAngles.y, 0f);
				normalTransformDistance.localPosition = new Vector3(0f, 0f, -2f);
				((Component)this).transform.position = normalTransformDistance.position;
				((Component)this).transform.rotation = normalTransformDistance.rotation;
				if (SemiFunc.IsMultiplayer())
				{
					SemiFunc.HUDSpectateSetName(player.playerName);
				}
				SemiFunc.LightManagerSetCullTargetTransform(((Component)player).transform);
				CameraGlitch.Instance.PlayTiny();
				GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
				AudioManager.instance.RestartAudioLoopDistances();
				normalMaxDistance = 3f;
				break;
			}
		}
	}

	public void UpdatePlayer(PlayerAvatar deadPlayer)
	{
		if ((Object)(object)deadPlayer == (Object)(object)player)
		{
			SetDeath(deadPlayer.spectatePoint);
		}
	}

	public void StopSpectate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ParentObject.parent = PreviousParent;
		ParentObject.localPosition = Vector3.zero;
		ParentObject.localRotation = Quaternion.identity;
		MainCamera.nearClipPlane = 0.001f;
		MainCamera.farClipPlane = previousFarClipPlane;
		((Component)MainCamera).transform.localPosition = Vector3.zero;
		((Component)MainCamera).transform.localRotation = Quaternion.identity;
		MainCamera.fieldOfView = previousFieldOfView;
		RenderSettings.fog = true;
		RenderSettings.fogStartDistance = 0f;
		PostProcessing.Instance.SpectateReset();
		PlayerAvatar.instance.spectating = false;
		SemiFunc.LightManagerSetCullTargetTransform(((Component)PlayerAvatar.instance).transform);
		AudioManager.instance.AudioListener.TargetPositionTransform = ((Component)MainCamera).transform;
		Object.Destroy((Object)(object)((Component)normalTransformPivot).gameObject);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void DeathNearClipLogic(bool _instant)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckState(State.Death))
		{
			return;
		}
		Vector3 val = ((Component)MainCamera).transform.position - deathSmoothLookAtPoint;
		RaycastHit[] array = Physics.RaycastAll(deathSmoothLookAtPoint, val, ((Vector3)(ref val)).magnitude, LayerMask.GetMask(new string[1] { "Default" }));
		float num = float.PositiveInfinity;
		Vector3 val2 = Vector3.zero;
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit val3 = array2[i];
			if (((Component)((RaycastHit)(ref val3)).transform).CompareTag("Ceiling") && ((RaycastHit)(ref val3)).distance < num)
			{
				num = ((RaycastHit)(ref val3)).distance;
				val2 = ((RaycastHit)(ref val3)).point;
			}
		}
		if (val2 != Vector3.zero)
		{
			Vector3 val4 = ((Component)MainCamera).transform.position - val2;
			deathCameraNearClipPlane = ((Vector3)(ref val4)).magnitude + 0.5f;
		}
		if (_instant)
		{
			MainCamera.nearClipPlane = deathCameraNearClipPlane;
		}
		else
		{
			MainCamera.nearClipPlane = Mathf.Lerp(MainCamera.nearClipPlane, deathCameraNearClipPlane, Time.deltaTime * 10f);
		}
	}

	public void SetDeath(Transform _spectatePoint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		deathPosition = _spectatePoint.position;
		deathPlayerSpectatePoint = _spectatePoint;
		((Component)this).transform.position = _spectatePoint.position;
		((Component)this).transform.rotation = _spectatePoint.rotation;
		deathFollowPoint = deathPosition;
		deathFollowPointTarget = deathPosition;
		deathSmoothLookAtPoint = deathPosition;
		deathOrbitInstantSet = true;
		SemiFunc.LightManagerSetCullTargetTransform(deathPlayerSpectatePoint);
		deathSmoothLookAtPoint = deathPlayerSpectatePoint.position;
		((Component)this).transform.position = deathFollowPointTarget;
		deathFollowPoint = deathFollowPointTarget;
		deathSmoothLookAtPoint = deathPlayerSpectatePoint.position;
		DeathNearClipLogic(_instant: true);
		UpdateState(State.Death);
	}

	private void RoomVolumeLogic()
	{
		RoomVolumeCheck roomVolumeCheck = PlayerController.instance.playerAvatarScript.RoomVolumeCheck;
		roomVolumeCheck.PauseCheckTimer = 1f;
		if (Object.op_Implicit((Object)(object)player))
		{
			RoomVolumeCheck roomVolumeCheck2 = player.RoomVolumeCheck;
			roomVolumeCheck.CurrentRooms.Clear();
			roomVolumeCheck.CurrentRooms.AddRange(roomVolumeCheck2.CurrentRooms);
		}
	}
}
