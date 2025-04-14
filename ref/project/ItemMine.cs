using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ItemMine : MonoBehaviour
{
	public enum MineType
	{
		None,
		Explosive,
		Shockwave,
		Stun
	}

	public enum States
	{
		Disarmed,
		Arming,
		Armed,
		Disarming,
		Triggering,
		Triggered
	}

	public MineType mineType;

	public Color emissionColor;

	public UnityEvent onTriggered;

	public float armingTime;

	public float triggeringTime;

	private SpringQuaternion triggerSpringQuaternion;

	private Quaternion triggerTargetRotation;

	private bool upsideDown;

	public Transform triggerTransform;

	public LineRenderer triggerLine;

	public ParticleSystem lineParticles;

	private float beepTimer;

	private float checkTimer;

	private ItemMineTrigger itemMineTrigger;

	private ItemEquippable itemEquippable;

	private ItemAttributes itemAttributes;

	private ItemToggle itemToggle;

	[Space(20f)]
	private PhotonView photonView;

	private PhysGrabObject physGrabObject;

	public MeshRenderer meshRenderer;

	public Light lightArmed;

	[Space(20f)]
	public Sound soundArmingBeep;

	public Sound soundArmedBeep;

	public Sound soundDisarmingBeep;

	public Sound soundDisarmedBeep;

	public Sound soundTriggereringBeep;

	private float initialLightIntensity;

	private ParticleScriptExplosion particleScriptExplosion;

	private bool hasBeenGrabbed;

	private Vector3 startPosition;

	private Quaternion startRotation;

	internal Vector3 triggeredPosition;

	internal Transform triggeredTransform;

	internal PlayerAvatar triggeredPlayerAvatar;

	internal PlayerTumble triggeredPlayerTumble;

	internal PhysGrabObject triggeredPhysGrabObject;

	public bool triggeredByRigidBodies = true;

	public bool triggeredByEnemies = true;

	public bool triggeredByPlayers = true;

	public bool triggeredByForces = true;

	public bool destroyAfterTimer;

	public float destroyTimer = 10f;

	internal bool wasTriggeredByEnemy;

	internal bool wasTriggeredByPlayer;

	internal bool wasTriggeredByForce;

	internal bool wasTriggeredByRigidBody;

	internal bool firstLight = true;

	private bool firstLightDone;

	private float secondArmedTimer;

	private bool wasGrabbed;

	private float targetLineLength = 1f;

	private Vector3 prevPos = Vector3.zero;

	private Quaternion prevRot = Quaternion.identity;

	internal States state;

	private bool stateStart = true;

	private float stateTimer;

	private PhysGrabObjectImpactDetector impactDetector;

	private bool mineDestroyed;

	private void Start()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		triggerSpringQuaternion = new SpringQuaternion();
		triggerSpringQuaternion.damping = 0.2f;
		triggerSpringQuaternion.speed = 10f;
		itemAttributes = ((Component)this).GetComponent<ItemAttributes>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		photonView = ((Component)this).GetComponent<PhotonView>();
		lightArmed.color = emissionColor;
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", emissionColor);
		initialLightIntensity = lightArmed.intensity;
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		itemMineTrigger = ((Component)this).GetComponentInChildren<ItemMineTrigger>();
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		startPosition = ((Component)this).transform.position;
		itemEquippable = ((Component)this).GetComponent<ItemEquippable>();
		startRotation = ((Component)this).transform.rotation;
		itemToggle = ((Component)this).GetComponent<ItemToggle>();
	}

	private void StateDisarmed()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			soundDisarmedBeep.Play(((Component)this).transform.position);
			stateStart = false;
			lightArmed.intensity = initialLightIntensity * 3f;
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.green);
			lightArmed.color = Color.green;
			beepTimer = 1f;
		}
		if (firstLight)
		{
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", emissionColor);
			lightArmed.color = emissionColor;
			firstLight = false;
		}
		else if (!firstLightDone)
		{
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.green);
			lightArmed.color = Color.green;
			firstLightDone = true;
		}
		if (lightArmed.intensity > 0f && beepTimer > 0f)
		{
			float num = 1f - beepTimer;
			lightArmed.intensity = Mathf.Lerp(lightArmed.intensity, 0f, num);
			Color val = Color.Lerp(((Renderer)meshRenderer).material.GetColor("_EmissionColor"), Color.black, num);
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", val);
			beepTimer -= Time.deltaTime * 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && itemToggle.toggleState)
		{
			StateSet(States.Arming);
		}
	}

	private void StateArming()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			beepTimer = 1f;
			lightArmed.color = emissionColor;
			Color color = default(Color);
			((Color)(ref color))._002Ector(1f, 0.5f, 0f);
			soundArmingBeep.Play(((Component)this).transform.position);
			ColorSet(color);
		}
		beepTimer -= Time.deltaTime * 4f;
		if (beepTimer <= 0f)
		{
			soundArmingBeep.Play(((Component)this).transform.position);
			Color color2 = default(Color);
			((Color)(ref color2))._002Ector(1f, 0.5f, 0f);
			ColorSet(color2);
			beepTimer = 1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!physGrabObject.grabbed)
			{
				stateTimer += Time.deltaTime;
			}
			else
			{
				stateTimer += Time.deltaTime * 0.25f;
			}
			if (!physGrabObject.grabbed)
			{
				Vector3 velocity = physGrabObject.rb.velocity;
				if (!(((Vector3)(ref velocity)).magnitude > 1f))
				{
					goto IL_0165;
				}
			}
			stateTimer = 0f;
			goto IL_0165;
		}
		goto IL_017a;
		IL_017a:
		if (SemiFunc.IsMasterClientOrSingleplayer() && !itemToggle.toggleState)
		{
			StateSet(States.Disarming);
		}
		return;
		IL_0165:
		if (stateTimer >= armingTime)
		{
			StateSet(States.Armed);
		}
		goto IL_017a;
	}

	private void StateArmed()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			soundArmedBeep.Play(((Component)this).transform.position);
			ColorSet(emissionColor);
			lightArmed.intensity = initialLightIntensity * 8f;
			stateStart = false;
			secondArmedTimer = 2f;
		}
		lightArmed.intensity = Mathf.Lerp(lightArmed.intensity, initialLightIntensity, Time.deltaTime * 4f);
		if (secondArmedTimer > 0f)
		{
			secondArmedTimer -= Time.deltaTime;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && secondArmedTimer <= 0f && triggeredByForces)
		{
			Vector3 velocity = physGrabObject.rb.velocity;
			if (((Vector3)(ref velocity)).magnitude > 0.5f)
			{
				StateSet(States.Triggering);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !itemToggle.toggleState)
		{
			StateSet(States.Disarming);
		}
	}

	private void ColorSet(Color _color)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		lightArmed.intensity = initialLightIntensity;
		lightArmed.color = _color;
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", _color);
	}

	private void StateDisarming()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			beepTimer = 1f;
			soundDisarmingBeep.Play(((Component)this).transform.position);
			ColorSet(emissionColor);
			beepTimer = 1f;
		}
		beepTimer -= Time.deltaTime * 4f;
		if (beepTimer <= 0f)
		{
			soundDisarmingBeep.Play(((Component)this).transform.position);
			ColorSet(Color.green);
			beepTimer = 1f;
		}
		stateTimer += Time.deltaTime;
		if (stateTimer > 0.1f)
		{
			StateSet(States.Disarmed);
		}
	}

	private void StateTriggering()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			beepTimer = 1f;
		}
		beepTimer -= Time.deltaTime * 4f;
		if (beepTimer < 0f)
		{
			soundTriggereringBeep.Play(((Component)this).transform.position);
			ColorSet(emissionColor);
			beepTimer = 1f;
		}
		stateTimer += Time.deltaTime;
		if (stateTimer > triggeringTime)
		{
			StateSet(States.Triggered);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !itemToggle.toggleState)
		{
			StateSet(States.Disarming);
		}
	}

	private void StateTriggered()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			beepTimer = 1f;
			if (!destroyAfterTimer)
			{
				DestroyMine();
			}
			Color color = default(Color);
			((Color)(ref color))._002Ector(0.5f, 0.9f, 1f);
			ColorSet(color);
		}
		stateTimer += Time.deltaTime;
		if (destroyAfterTimer && stateTimer > destroyTimer)
		{
			DestroyMine();
		}
	}

	public void DestroyMine()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.RunIsShop())
		{
			if (!mineDestroyed)
			{
				StatsManager.instance.ItemRemove(itemAttributes.instanceName);
				impactDetector.DestroyObject();
				mineDestroyed = true;
			}
		}
		else
		{
			ResetMine();
			physGrabObject.Teleport(startPosition, startRotation);
		}
	}

	private void ResetMine()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		hasBeenGrabbed = false;
		StateSet(States.Disarmed);
		itemToggle.ToggleItem(toggle: false);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			stateTimer = 0f;
			Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
			if (!component.isKinematic)
			{
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
			}
		}
	}

	private void AnimateLight()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (lightArmed.intensity > 0f && beepTimer > 0f)
		{
			float num = 1f - beepTimer;
			lightArmed.intensity = Mathf.Lerp(lightArmed.intensity, 0f, num);
			Color val = Color.Lerp(((Renderer)meshRenderer).material.GetColor("_EmissionColor"), Color.black, num);
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", val);
		}
	}

	private void Update()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		TriggerRotation();
		TriggerLineVisuals();
		TriggerScaleFixer();
		AnimateLight();
		if (physGrabObject.grabbedLocal && !SemiFunc.RunIsShop())
		{
			PhysGrabber.instance.OverrideGrabDistance(1f);
		}
		if (physGrabObject.grabbed)
		{
			hasBeenGrabbed = true;
		}
		if (itemEquippable.isEquipped && SemiFunc.IsMasterClientOrSingleplayer() && hasBeenGrabbed)
		{
			StateSet(States.Disarmed);
		}
		if (!SemiFunc.RunIsShop())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && wasGrabbed && !physGrabObject.grabbed)
			{
				Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
				if (!component.isKinematic)
				{
					component.velocity *= 0.15f;
				}
			}
			wasGrabbed = physGrabObject.grabbed;
		}
		switch (state)
		{
		case States.Disarmed:
			StateDisarmed();
			break;
		case States.Arming:
			StateArming();
			break;
		case States.Armed:
			StateArmed();
			break;
		case States.Disarming:
			StateDisarming();
			break;
		case States.Triggering:
			StateTriggering();
			break;
		case States.Triggered:
			StateTriggered();
			break;
		}
	}

	[PunRPC]
	public void TriggeredRPC()
	{
		onTriggered.Invoke();
	}

	private void StateSet(States newState)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer() || newState == state)
		{
			return;
		}
		if (newState == States.Triggered)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("TriggeredRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				TriggeredRPC();
			}
		}
		if (SemiFunc.IsMultiplayer())
		{
			photonView.RPC("StateSetRPC", (RpcTarget)0, new object[1] { (int)newState });
		}
		else
		{
			StateSetRPC((int)newState);
		}
	}

	[PunRPC]
	public void StateSetRPC(int newState)
	{
		state = (States)newState;
		stateStart = true;
		stateTimer = 0f;
		beepTimer = 0f;
	}

	private void TriggerScaleFixer()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (state != States.Armed)
		{
			return;
		}
		bool flag = false;
		if (SemiFunc.FPSImpulse30())
		{
			if (Vector3.Distance(prevPos, ((Component)this).transform.position) > 0.01f)
			{
				flag = true;
				prevPos = ((Component)this).transform.position;
			}
			if (Quaternion.Angle(prevRot, ((Component)this).transform.rotation) > 0.01f)
			{
				flag = true;
				prevRot = ((Component)this).transform.rotation;
			}
		}
		if ((!flag && SemiFunc.FPSImpulse1()) || (flag && SemiFunc.FPSImpulse30()))
		{
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(triggerTransform.position, triggerTransform.forward, ref val, 1f, LayerMask.GetMask(new string[1] { "Default" })))
			{
				targetLineLength = ((RaycastHit)(ref val)).distance * 0.8f;
			}
			else
			{
				targetLineLength = 1f;
			}
		}
		triggerTransform.localScale = Mathf.Lerp(triggerTransform.localScale.z, targetLineLength, Time.deltaTime * 8f) * Vector3.one;
	}

	private void TriggerRotation()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		upsideDown = true;
		if (Vector3.Dot(((Component)this).transform.up, Vector3.up) < 0f)
		{
			upsideDown = false;
		}
		if (upsideDown)
		{
			triggerTargetRotation = Quaternion.Euler(-90f, 0f, 0f);
		}
		else
		{
			triggerTargetRotation = Quaternion.Euler(90f, 0f, 0f);
		}
		triggerTransform.localRotation = SemiFunc.SpringQuaternionGet(triggerSpringQuaternion, triggerTargetRotation);
	}

	private void TriggerLineVisuals()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (state == States.Armed)
		{
			((Renderer)triggerLine).material.SetTextureOffset("_MainTex", new Vector2((0f - Time.time) * 2f, 0f));
			if (!((Renderer)triggerLine).enabled)
			{
				((Renderer)triggerLine).enabled = true;
				lineParticles.Play();
			}
			triggerLine.widthMultiplier = Mathf.Lerp(triggerLine.widthMultiplier, 1f, Time.deltaTime * 4f);
		}
		else if (((Renderer)triggerLine).enabled)
		{
			triggerLine.widthMultiplier = Mathf.Lerp(triggerLine.widthMultiplier, 0f, Time.deltaTime * 8f);
			if (triggerLine.widthMultiplier < 0.01f)
			{
				((Renderer)triggerLine).enabled = false;
				lineParticles.Stop();
			}
		}
	}

	public void SetTriggered()
	{
		if (state == States.Armed)
		{
			StateSet(States.Triggering);
		}
	}
}
