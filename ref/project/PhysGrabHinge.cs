using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PhysGrabHinge : MonoBehaviour
{
	public enum BounceEffect
	{
		Light,
		Medium,
		Heavy
	}

	private PhotonView photon;

	public HingeAudio hingeAudio;

	public AudioSource audioSource;

	[Space]
	public Transform hingePoint;

	private Rigidbody hingePointRb;

	private bool hingePointHasRb;

	public float hingeOffsetPositiveThreshold = 15f;

	public float hingeOffsetNegativeThreshold = -15f;

	public float hingeOffsetSpeed = 5f;

	public Vector3 hingeOffsetPositive;

	public Vector3 hingeOffsetNegative;

	private Vector3 hingePointPosition;

	[Space]
	public float hingeBreakShake = 3f;

	[Space]
	public float closeThreshold = 10f;

	public float closeMaxSpeed = 1f;

	public float closeHeavySpeed = 5f;

	public float closeShake = 3f;

	private bool closeHeavy;

	private float closeSpeed;

	internal bool closed = true;

	private float closedForceTimer;

	private bool closing;

	private float closeDisableTimer;

	[Space]
	private float openForceNeeded = 0.04f;

	public float openHeavyThreshold = 3f;

	public float openShake = 3f;

	internal HingeJoint joint;

	private PhysGrabObject physGrabObject;

	private PhysGrabObjectImpactDetector impactDetector;

	private bool moveLoopActive;

	private float moveLoopEndDisableTimer;

	[HideInInspector]
	public Sound moveLoop;

	private Vector3 restPosition;

	private Quaternion restRotation;

	internal bool dead;

	private float deadTimer = 0.1f;

	internal bool broken;

	internal float brokenTimer;

	[Space]
	public float drag;

	[Space]
	public float bounceAmount = 0.2f;

	public BounceEffect bounceEffect = BounceEffect.Medium;

	private Vector3 bounceVelocity;

	private float bounceCooldown;

	[Space]
	public PhysGrabHinge[] wallTagHinges;

	public GameObject[] wallTagObjects;

	private float investigateDelay;

	private float investigateRadius;

	private bool fadeOutFast;

	private void Awake()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		photon = ((Component)this).GetComponent<PhotonView>();
		Sound.CopySound(hingeAudio.moveLoop, moveLoop);
		moveLoop.Source = audioSource;
		joint = ((Component)this).GetComponent<HingeJoint>();
		physGrabObject = ((Component)this).GetComponent<PhysGrabObject>();
		impactDetector = ((Component)this).GetComponent<PhysGrabObjectImpactDetector>();
		impactDetector.particleDisable = true;
		((Joint)joint).anchor = hingePoint.localPosition;
		hingePointRb = ((Component)hingePoint).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)hingePointRb))
		{
			hingePointHasRb = true;
			hingePointPosition = hingePoint.position;
		}
		if (SemiFunc.IsMultiplayer() && SemiFunc.IsNotMasterClient())
		{
			Object.Destroy((Object)(object)joint);
			joint = null;
			hingePointHasRb = false;
		}
		restPosition = ((Component)this).transform.position;
		restRotation = ((Component)this).transform.rotation;
		((MonoBehaviour)this).StartCoroutine(RigidBodyGet());
		((Component)this).gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		foreach (Transform item in ((Component)this).transform)
		{
			((Component)item).gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectHinge");
		}
	}

	private IEnumerator RigidBodyGet()
	{
		while (!physGrabObject.spawned)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		((Component)hingePoint).transform.parent = ((Component)this).transform.parent;
		WallTagSet();
	}

	private void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			closeDisableTimer = 0.1f;
		}
		else if (closing && other.gameObject.CompareTag("Phys Grab Object"))
		{
			closing = false;
		}
	}

	private void OnJointBreak(float breakForce)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			physGrabObject.rb.AddForce(-physGrabObject.rb.velocity * 2f, (ForceMode)1);
			physGrabObject.rb.AddTorque(-physGrabObject.rb.angularVelocity * 10f, (ForceMode)1);
			HingeBreakImpulse();
			broken = true;
		}
	}

	private void FixedUpdate()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		if (broken)
		{
			brokenTimer += Time.fixedDeltaTime;
		}
		if (dead || broken || !physGrabObject.spawned || (GameManager.instance.gameMode != 0 && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		if (GameManager.Multiplayer())
		{
			physGrabObject.photonTransformView.KinematicClientForce(0.1f);
		}
		if (hingePointHasRb)
		{
			if (joint.angle >= hingeOffsetPositiveThreshold)
			{
				Vector3 val = hingePointPosition + hingePoint.TransformDirection(hingeOffsetPositive);
				Vector3 val2 = Vector3.Lerp(((Component)hingePointRb).transform.position, val, hingeOffsetSpeed * Time.fixedDeltaTime);
				if (hingePointRb.position != val2)
				{
					hingePointRb.MovePosition(val2);
				}
			}
			else if (joint.angle <= hingeOffsetNegativeThreshold)
			{
				Vector3 val3 = hingePointPosition + hingePoint.TransformDirection(hingeOffsetNegative);
				Vector3 val4 = Vector3.Lerp(((Component)hingePointRb).transform.position, val3, hingeOffsetSpeed * Time.fixedDeltaTime);
				if (hingePointRb.position != val4)
				{
					hingePointRb.MovePosition(val4);
				}
			}
			else
			{
				Vector3 val5 = Vector3.Lerp(((Component)hingePointRb).transform.position, hingePointPosition, hingeOffsetSpeed * Time.fixedDeltaTime);
				if (closed)
				{
					val5 = hingePointPosition;
				}
				if (hingePointRb.position != val5)
				{
					hingePointRb.MovePosition(val5);
				}
			}
		}
		Vector3 val6;
		if (!closed && closeDisableTimer <= 0f && Object.op_Implicit((Object)(object)joint))
		{
			if (!closing)
			{
				val6 = physGrabObject.rb.angularVelocity;
				Vector3 normalized = ((Vector3)(ref val6)).normalized;
				val6 = -((Joint)joint).axis * joint.angle;
				float num = Vector3.Dot(normalized, ((Vector3)(ref val6)).normalized);
				val6 = physGrabObject.rb.angularVelocity;
				if (((Vector3)(ref val6)).magnitude < closeMaxSpeed && Mathf.Abs(joint.angle) < closeThreshold)
				{
					if (!(num > 0f))
					{
						val6 = physGrabObject.rb.angularVelocity;
						if (!(((Vector3)(ref val6)).magnitude < 0.1f))
						{
							goto IL_0399;
						}
					}
					closeHeavy = false;
					val6 = physGrabObject.rb.angularVelocity;
					closeSpeed = Mathf.Max(((Vector3)(ref val6)).magnitude, 0.2f);
					if (closeSpeed > closeHeavySpeed)
					{
						closeHeavy = true;
					}
					closing = true;
				}
			}
			else if (physGrabObject.playerGrabbing.Count > 0)
			{
				closing = false;
			}
			else
			{
				Vector3 eulerAngles = ((Quaternion)(ref restRotation)).eulerAngles;
				Quaternion rotation = physGrabObject.rb.rotation;
				Vector3 val7 = eulerAngles - ((Quaternion)(ref rotation)).eulerAngles;
				val7 = Vector3.ClampMagnitude(val7, closeSpeed);
				physGrabObject.rb.AddRelativeTorque(val7, (ForceMode)5);
				if (Mathf.Abs(joint.angle) < 2f)
				{
					closedForceTimer = 0.25f;
					closing = false;
					CloseImpulse(closeHeavy);
				}
			}
		}
		goto IL_0399;
		IL_0399:
		if (physGrabObject.playerGrabbing.Count > 0)
		{
			closeDisableTimer = 0.1f;
		}
		else if (closeDisableTimer > 0f)
		{
			closeDisableTimer -= 1f * Time.fixedDeltaTime;
		}
		if (closed)
		{
			if (closedForceTimer > 0f)
			{
				closedForceTimer -= 1f * Time.fixedDeltaTime;
			}
			else
			{
				val6 = physGrabObject.rb.angularVelocity;
				if (((Vector3)(ref val6)).magnitude > openForceNeeded)
				{
					OpenImpulse();
					closeDisableTimer = 2f;
					closing = false;
				}
			}
			if (closed && !physGrabObject.rb.isKinematic && (physGrabObject.rb.position != restPosition || physGrabObject.rb.rotation != restRotation))
			{
				physGrabObject.rb.MovePosition(restPosition);
				physGrabObject.rb.MoveRotation(restRotation);
				physGrabObject.rb.angularVelocity = Vector3.zero;
				physGrabObject.rb.velocity = Vector3.zero;
			}
		}
		if (physGrabObject.playerGrabbing.Count <= 0 && !closing && !closed)
		{
			Vector3 angularVelocity = physGrabObject.rb.angularVelocity;
			if (((Vector3)(ref angularVelocity)).magnitude <= 0.1f && ((Vector3)(ref bounceVelocity)).magnitude > 0.5f && bounceCooldown <= 0f)
			{
				bounceCooldown = 1f;
				physGrabObject.rb.AddTorque(bounceAmount * -((Vector3)(ref bounceVelocity)).normalized, (ForceMode)1);
				if (bounceEffect == BounceEffect.Heavy)
				{
					physGrabObject.heavyImpactImpulse = true;
				}
				else if (bounceEffect == BounceEffect.Medium)
				{
					physGrabObject.mediumImpactImpulse = true;
				}
				else
				{
					physGrabObject.lightImpactImpulse = true;
				}
				moveLoopEndDisableTimer = 1f;
			}
			bounceVelocity = angularVelocity;
		}
		else
		{
			bounceVelocity = Vector3.zero;
		}
		if (bounceCooldown > 0f)
		{
			bounceCooldown -= 1f * Time.fixedDeltaTime;
		}
		if (!closing)
		{
			physGrabObject.OverrideDrag(drag);
			physGrabObject.OverrideAngularDrag(drag);
		}
	}

	private void Update()
	{
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (dead)
		{
			deadTimer -= 1f * Time.deltaTime;
			if (deadTimer <= 0f)
			{
				impactDetector.DestroyObject();
			}
			return;
		}
		if (broken)
		{
			moveLoop.PlayLoop(playing: false, 1f, 1f);
			return;
		}
		if (hingeAudio.moveLoopEnabled)
		{
			if (((Vector3)(ref physGrabObject.rbVelocity)).magnitude > hingeAudio.moveLoopThreshold)
			{
				if (!moveLoopActive)
				{
					fadeOutFast = false;
					moveLoopActive = true;
				}
				moveLoop.PlayLoop(playing: true, hingeAudio.moveLoopFadeInSpeed, hingeAudio.moveLoopFadeOutSpeed);
				moveLoop.LoopPitch = Mathf.Max(moveLoop.Pitch + ((Vector3)(ref physGrabObject.rbVelocity)).magnitude * hingeAudio.moveLoopVelocityMult, 0.1f);
			}
			else
			{
				if (moveLoopActive)
				{
					if (moveLoopEndDisableTimer <= 0f)
					{
						hingeAudio.moveLoopEnd.Play(((Component)moveLoop.Source).transform.position);
						moveLoopEndDisableTimer = 3f;
					}
					moveLoopActive = false;
				}
				if (fadeOutFast)
				{
					moveLoop.PlayLoop(playing: false, hingeAudio.moveLoopFadeInSpeed, 20f);
				}
				else
				{
					moveLoop.PlayLoop(playing: false, hingeAudio.moveLoopFadeInSpeed, hingeAudio.moveLoopFadeOutSpeed);
				}
				moveLoopEndDisableTimer = 0.5f;
			}
			if (moveLoopEndDisableTimer > 0f)
			{
				moveLoopEndDisableTimer -= 1f * Time.deltaTime;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && investigateDelay > 0f)
		{
			investigateDelay -= 1f * Time.deltaTime;
			if (investigateDelay <= 0f && physGrabObject.enemyInteractTimer <= 0f)
			{
				EnemyDirector.instance.SetInvestigate(physGrabObject.midPoint, investigateRadius);
			}
		}
	}

	private void WallTagSet()
	{
		string text = "Untagged";
		if (closed && !broken && !dead)
		{
			text = "Wall";
		}
		if (text == "Wall" && wallTagHinges.Length != 0)
		{
			PhysGrabHinge[] array = wallTagHinges;
			foreach (PhysGrabHinge physGrabHinge in array)
			{
				if (!Object.op_Implicit((Object)(object)physGrabHinge) || !physGrabHinge.closed)
				{
					return;
				}
			}
		}
		if (wallTagObjects.Length == 0)
		{
			return;
		}
		GameObject[] array2 = wallTagObjects;
		foreach (GameObject val in array2)
		{
			if (Object.op_Implicit((Object)(object)val))
			{
				val.tag = text;
			}
		}
	}

	private void EnemyInvestigate(float radius)
	{
		investigateDelay = 0.1f;
		investigateRadius = radius;
	}

	private void CloseImpulse(bool heavy)
	{
		EnemyInvestigate(1f);
		if (GameManager.instance.gameMode == 0)
		{
			CloseImpulseRPC(heavy);
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photon.RPC("CloseImpulseRPC", (RpcTarget)0, new object[1] { heavy });
		}
	}

	[PunRPC]
	private void CloseImpulseRPC(bool heavy)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		fadeOutFast = true;
		GameDirector.instance.CameraImpact.ShakeDistance(closeShake * 0.5f, 3f, 10f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(closeShake, 3f, 10f, ((Component)this).transform.position, 0.1f);
		if (heavy)
		{
			hingeAudio.CloseHeavy.Play(((Component)audioSource).transform.position);
		}
		else
		{
			hingeAudio.Close.Play(((Component)audioSource).transform.position);
		}
		moveLoopEndDisableTimer = 1f;
		closed = true;
		WallTagSet();
	}

	private void OpenImpulse()
	{
		EnemyInvestigate(0.5f);
		if (GameManager.instance.gameMode == 0)
		{
			OpenImpulseRPC();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photon.RPC("OpenImpulseRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void OpenImpulseRPC()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(openShake * 0.5f, 3f, 10f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(openShake, 3f, 10f, ((Component)this).transform.position, 0.1f);
		if (((Vector3)(ref physGrabObject.rbAngularVelocity)).magnitude > openHeavyThreshold)
		{
			hingeAudio.OpenHeavy.Play(((Component)audioSource).transform.position);
		}
		else
		{
			hingeAudio.Open.Play(((Component)audioSource).transform.position);
		}
		closed = false;
		WallTagSet();
	}

	private void HingeBreakImpulse()
	{
		if (GameManager.instance.gameMode == 0)
		{
			HingeBreakRPC();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photon.RPC("HingeBreakRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void HingeBreakRPC()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		GameDirector.instance.CameraImpact.ShakeDistance(hingeBreakShake * 0.5f, 3f, 10f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraShake.ShakeDistance(hingeBreakShake, 3f, 10f, ((Component)this).transform.position, 0.1f);
		hingeAudio.HingeBreak.Play(((Component)audioSource).transform.position);
		physGrabObject.heavyBreakImpulse = true;
		impactDetector.isHinge = false;
		impactDetector.isBrokenHinge = true;
		impactDetector.particleDisable = false;
		broken = true;
		WallTagSet();
		int layer = LayerMask.NameToLayer("PhysGrabObject");
		((Component)this).gameObject.layer = layer;
		foreach (Transform item in ((Component)this).transform)
		{
			((Component)item).gameObject.layer = layer;
		}
	}

	public void DestroyHinge()
	{
		if (GameManager.instance.gameMode == 0)
		{
			DestroyHingeRPC();
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			photon.RPC("DestroyHingeRPC", (RpcTarget)0, Array.Empty<object>());
		}
	}

	[PunRPC]
	private void DestroyHingeRPC()
	{
		dead = true;
		WallTagSet();
	}
}
