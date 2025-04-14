using System.Collections.Generic;
using UnityEngine;

public class FloaterAttackLogic : MonoBehaviour
{
	public enum FloaterAttackState
	{
		start,
		levitate,
		stop,
		smash,
		end,
		inactive
	}

	public GameObject linePrefab;

	public ParticleSystem upParticle;

	public ParticleSystem downParticle;

	public EnemyFloater controller;

	public PhysGrabObject enemyFloaterPhysGrabObject;

	internal int damage = 50;

	internal FloaterAttackState state = FloaterAttackState.inactive;

	private bool stateStart = true;

	public Transform sphereEffects;

	public Light attackLight;

	private float range = 4f;

	private List<PlayerAvatar> capturedPlayerAvatars = new List<PlayerAvatar>();

	private List<PhysGrabObject> capturedPhysGrabObjects = new List<PhysGrabObject>();

	private List<FloaterLine> floaterLines = new List<FloaterLine>();

	private float checkTimer;

	private int particleCount;

	private float tumblePhysObjectCheckTimer;

	private void StateMachine()
	{
		if (controller.currentState == EnemyFloater.State.ChargeAttack || controller.currentState == EnemyFloater.State.DelayAttack || controller.currentState == EnemyFloater.State.Attack)
		{
			switch (controller.currentState)
			{
			case EnemyFloater.State.ChargeAttack:
				if (state != FloaterAttackState.levitate)
				{
					StateSet(FloaterAttackState.start);
				}
				break;
			case EnemyFloater.State.Stun:
				StateSet(FloaterAttackState.end);
				break;
			}
		}
		else
		{
			StateSet(FloaterAttackState.end);
		}
		switch (state)
		{
		case FloaterAttackState.start:
			StateStart();
			break;
		case FloaterAttackState.levitate:
			StateLevitate();
			break;
		case FloaterAttackState.stop:
			StateStop();
			break;
		case FloaterAttackState.smash:
			StateSmash();
			break;
		case FloaterAttackState.end:
			StateEnd();
			break;
		case FloaterAttackState.inactive:
			StateInactive();
			break;
		}
	}

	private void Reset()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		checkTimer = 0f;
		particleCount = 0;
		tumblePhysObjectCheckTimer = 0f;
		foreach (FloaterLine floaterLine in floaterLines)
		{
			if (Object.op_Implicit((Object)(object)floaterLine))
			{
				floaterLine.outro = true;
			}
		}
		capturedPlayerAvatars.Clear();
		capturedPhysGrabObjects.Clear();
		floaterLines.Clear();
		sphereEffects.localScale = Vector3.zero;
		attackLight.intensity = 0f;
		((Component)sphereEffects).gameObject.SetActive(false);
	}

	private void StateInactive()
	{
		if (stateStart)
		{
			Reset();
			stateStart = false;
		}
	}

	private void StateEnd()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			foreach (FloaterLine floaterLine in floaterLines)
			{
				if (Object.op_Implicit((Object)(object)floaterLine))
				{
					floaterLine.outro = true;
				}
			}
			stateStart = false;
		}
		if (((Component)sphereEffects).gameObject.activeSelf)
		{
			sphereEffects.localScale = Vector3.Lerp(sphereEffects.localScale, Vector3.zero, Time.deltaTime * 20f);
			attackLight.intensity = Mathf.Lerp(attackLight.intensity, 0f, Time.deltaTime * 20f);
			if (sphereEffects.localScale.x < 0.01f)
			{
				StateSet(FloaterAttackState.inactive);
			}
		}
		else
		{
			StateSet(FloaterAttackState.inactive);
		}
	}

	private void StateStart()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			Reset();
			((Component)sphereEffects).gameObject.SetActive(true);
			stateStart = false;
		}
		sphereEffects.localScale = Vector3.Lerp(sphereEffects.localScale, Vector3.one * 1.2f, Time.deltaTime * 6f);
		Light obj = attackLight;
		Vector3 localScale = sphereEffects.localScale;
		obj.intensity = 4f * ((Vector3)(ref localScale)).magnitude;
		if (sphereEffects.localScale.x > 1.19f)
		{
			attackLight.intensity = 4f;
			sphereEffects.localScale = Vector3.one * 1.2f;
			StateSet(FloaterAttackState.levitate);
		}
	}

	private void StateLevitate()
	{
		if (stateStart)
		{
			stateStart = false;
			GetAllWithinRange();
		}
		if (checkTimer > 0.35f)
		{
			GetAllWithinRange();
			checkTimer = 0f;
		}
		checkTimer += Time.deltaTime;
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PlayerAvatar capturedPlayerAvatar in capturedPlayerAvatars)
		{
			capturedPlayerAvatar.tumble.TumbleOverrideTime(2f);
			PlayerTumble(capturedPlayerAvatar);
		}
		foreach (PhysGrabObject capturedPhysGrabObject in capturedPhysGrabObjects)
		{
			if (Object.op_Implicit((Object)(object)capturedPhysGrabObject) && capturedPhysGrabObject.isEnemy)
			{
				Enemy enemy = ((Component)capturedPhysGrabObject).GetComponent<EnemyRigidbody>().enemy;
				if (Object.op_Implicit((Object)(object)enemy) && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
				{
					enemy.StateStunned.Set(4f);
				}
			}
			capturedPhysGrabObject.OverrideZeroGravity();
		}
	}

	private void StateStop()
	{
		if (stateStart)
		{
			checkTimer = 0f;
			stateStart = false;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PlayerAvatar capturedPlayerAvatar in capturedPlayerAvatars)
			{
				if (Object.op_Implicit((Object)(object)capturedPlayerAvatar))
				{
					capturedPlayerAvatar.tumble.TumbleOverrideTime(2f);
				}
			}
		}
		checkTimer += Time.deltaTime;
		if (checkTimer > 0.35f)
		{
			RemoveAllOutOfRange();
			checkTimer = 0f;
		}
	}

	private void StateSmash()
	{
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			GameDirector.instance.CameraShake.ShakeDistance(6f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			foreach (PhysGrabObject capturedPhysGrabObject in capturedPhysGrabObjects)
			{
				if (Object.op_Implicit((Object)(object)capturedPhysGrabObject))
				{
					((Component)downParticle).transform.position = capturedPhysGrabObject.midPoint;
					downParticle.Emit(1);
				}
			}
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				foreach (PlayerAvatar capturedPlayerAvatar in capturedPlayerAvatars)
				{
					if (Object.op_Implicit((Object)(object)capturedPlayerAvatar) && capturedPlayerAvatar.tumble.isTumbling)
					{
						capturedPlayerAvatar.tumble.TumbleOverrideTime(2f);
						capturedPlayerAvatar.tumble.ImpactHurtSet(2f, damage);
					}
				}
				foreach (PhysGrabObject capturedPhysGrabObject2 in capturedPhysGrabObjects)
				{
					if (Object.op_Implicit((Object)(object)capturedPhysGrabObject2) && Object.op_Implicit((Object)(object)capturedPhysGrabObject2) && Object.op_Implicit((Object)(object)capturedPhysGrabObject2.rb) && !capturedPhysGrabObject2.rb.isKinematic)
					{
						capturedPhysGrabObject2.rb.AddForce(Vector3.down * 30f, (ForceMode)1);
					}
				}
			}
			foreach (FloaterLine floaterLine in floaterLines)
			{
				if (Object.op_Implicit((Object)(object)floaterLine))
				{
					floaterLine.outro = true;
				}
			}
			floaterLines.Clear();
			capturedPlayerAvatars.Clear();
			capturedPhysGrabObjects.Clear();
			stateStart = false;
		}
		sphereEffects.localScale = Vector3.Lerp(sphereEffects.localScale, Vector3.zero, Time.deltaTime * 2f);
		if (sphereEffects.localScale.x > 0.5f)
		{
			attackLight.intensity = Mathf.Lerp(attackLight.intensity, 20f, Time.deltaTime * 60f);
		}
		else
		{
			Light obj = attackLight;
			Vector3 localScale = sphereEffects.localScale;
			obj.intensity = 20f * ((Vector3)(ref localScale)).magnitude;
		}
		if (sphereEffects.localScale.x < 0.01f)
		{
			StateSet(FloaterAttackState.inactive);
		}
	}

	private void RemoveAllOutOfRange()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		for (int num = capturedPlayerAvatars.Count - 1; num >= 0; num--)
		{
			PlayerAvatar playerAvatar = capturedPlayerAvatars[num];
			if (!Object.op_Implicit((Object)(object)playerAvatar))
			{
				capturedPlayerAvatars.RemoveAt(num);
			}
			else if (Vector3.Distance(new Vector3(((Component)playerAvatar).transform.position.x, ((Component)this).transform.position.y, ((Component)playerAvatar).transform.position.z), ((Component)this).transform.position) > range * 1.2f)
			{
				capturedPlayerAvatars.RemoveAt(num);
				foreach (FloaterLine floaterLine in floaterLines)
				{
					if (Object.op_Implicit((Object)(object)floaterLine) && (Object)(object)floaterLine.lineTarget == (Object)(object)playerAvatar.PlayerVisionTarget.VisionTransform)
					{
						floaterLine.outro = true;
					}
				}
			}
		}
		for (int num2 = capturedPhysGrabObjects.Count - 1; num2 >= 0; num2--)
		{
			PhysGrabObject physGrabObject = capturedPhysGrabObjects[num2];
			if (!Object.op_Implicit((Object)(object)physGrabObject))
			{
				capturedPhysGrabObjects.RemoveAt(num2);
			}
			else if (Vector3.Distance(new Vector3(((Component)physGrabObject).transform.position.x, ((Component)this).transform.position.y, ((Component)physGrabObject).transform.position.z), ((Component)this).transform.position) > range * 1.2f)
			{
				capturedPhysGrabObjects.RemoveAt(num2);
			}
		}
	}

	private void StateLevitateFixed()
	{
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		if (state != FloaterAttackState.levitate)
		{
			return;
		}
		if (tumblePhysObjectCheckTimer > 1f)
		{
			foreach (PlayerAvatar capturedPlayerAvatar in capturedPlayerAvatars)
			{
				if (capturedPlayerAvatar.tumble.isTumbling)
				{
					PhysGrabObject physGrabObject = capturedPlayerAvatar.tumble.physGrabObject;
					if (!capturedPhysGrabObjects.Contains(physGrabObject))
					{
						capturedPhysGrabObjects.Add(physGrabObject);
					}
				}
			}
			tumblePhysObjectCheckTimer = 0f;
		}
		else
		{
			tumblePhysObjectCheckTimer += Time.fixedDeltaTime;
		}
		foreach (PhysGrabObject capturedPhysGrabObject in capturedPhysGrabObjects)
		{
			if (Object.op_Implicit((Object)(object)capturedPhysGrabObject))
			{
				float num = 10f;
				if (Object.op_Implicit((Object)(object)((Component)capturedPhysGrabObject).GetComponent<PlayerTumble>()))
				{
					num = 20f;
				}
				if (Object.op_Implicit((Object)(object)capturedPhysGrabObject) && Object.op_Implicit((Object)(object)capturedPhysGrabObject.rb) && !capturedPhysGrabObject.rb.isKinematic)
				{
					capturedPhysGrabObject.rb.AddForce(Vector3.up * Time.fixedDeltaTime * num, (ForceMode)0);
					capturedPhysGrabObject.rb.AddTorque(Vector3.up * Time.fixedDeltaTime * 0.2f, (ForceMode)0);
					capturedPhysGrabObject.rb.AddTorque(Vector3.left * Time.fixedDeltaTime * 0.1f, (ForceMode)0);
					capturedPhysGrabObject.rb.velocity = Vector3.Lerp(capturedPhysGrabObject.rb.velocity, new Vector3(0f, capturedPhysGrabObject.rb.velocity.y, 0f), Time.fixedDeltaTime * 2f);
				}
			}
		}
		if (particleCount < capturedPhysGrabObjects.Count)
		{
			if (Object.op_Implicit((Object)(object)capturedPhysGrabObjects[particleCount]))
			{
				Vector3 position = ((Component)capturedPhysGrabObjects[particleCount]).transform.position;
				Vector3 val = Random.insideUnitSphere * 2f;
				val.y = 0f - Mathf.Abs(val.y);
				position += val;
				((Component)upParticle).transform.position = position;
				upParticle.Emit(1);
			}
			particleCount++;
		}
		else
		{
			particleCount = 0;
		}
	}

	private void StateStopFixed()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (state != FloaterAttackState.stop || !SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PhysGrabObject capturedPhysGrabObject in capturedPhysGrabObjects)
		{
			if (!Object.op_Implicit((Object)(object)capturedPhysGrabObject) || !Object.op_Implicit((Object)(object)capturedPhysGrabObject.rb) || capturedPhysGrabObject.rb.isKinematic)
			{
				continue;
			}
			capturedPhysGrabObject.OverrideZeroGravity();
			if (capturedPhysGrabObject.isEnemy)
			{
				Enemy enemy = ((Component)capturedPhysGrabObject).GetComponent<EnemyRigidbody>().enemy;
				if (Object.op_Implicit((Object)(object)enemy) && enemy.HasStateStunned && enemy.Type < EnemyType.Heavy)
				{
					enemy.StateStunned.Set(4f);
				}
			}
			capturedPhysGrabObject.rb.velocity = Vector3.Lerp(capturedPhysGrabObject.rb.velocity, Vector3.zero, Time.deltaTime * 2f);
		}
	}

	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			StateLevitateFixed();
			StateStopFixed();
		}
	}

	private void Update()
	{
		StateMachine();
	}

	public void StateSet(FloaterAttackState _state)
	{
		if (state != _state)
		{
			state = _state;
			stateStart = true;
		}
	}

	public void GetAllWithinRange()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		RemoveAllOutOfRange();
		foreach (PlayerAvatar item in SemiFunc.PlayerGetAllPlayerAvatarWithinRange(range, ((Component)this).transform.position))
		{
			if (!capturedPlayerAvatars.Contains(item))
			{
				capturedPlayerAvatars.Add(item);
				PlayerTumble(item);
				FloaterLine component = Object.Instantiate<GameObject>(linePrefab, ((Component)this).transform.position, Quaternion.identity, ((Component)this).transform).GetComponent<FloaterLine>();
				component.lineTarget = item.PlayerVisionTarget.VisionTransform;
				component.floaterAttack = this;
				floaterLines.Add(component);
			}
		}
		foreach (PhysGrabObject item2 in SemiFunc.PhysGrabObjectGetAllWithinRange(range, ((Component)this).transform.position))
		{
			if (!((Object)(object)item2 == (Object)(object)enemyFloaterPhysGrabObject) && !capturedPhysGrabObjects.Contains(item2))
			{
				capturedPhysGrabObjects.Add(item2);
			}
		}
	}

	private void PlayerTumble(PlayerAvatar _player)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && Object.op_Implicit((Object)(object)_player) && !_player.isDisabled)
		{
			if (!_player.tumble.isTumbling)
			{
				_player.tumble.TumbleRequest(_isTumbling: true, _playerInput: false);
			}
			_player.tumble.TumbleOverrideTime(2f);
		}
	}

	private void OnEnable()
	{
		StateSet(FloaterAttackState.inactive);
	}

	private void OnDisable()
	{
		StateSet(FloaterAttackState.inactive);
		StateInactive();
	}
}
