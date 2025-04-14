using System.Collections.Generic;
using UnityEngine;

public class EnemySlowMouthCameraVisuals : MonoBehaviour
{
	public enum State
	{
		Intro,
		Idle,
		Puke,
		Outro
	}

	internal EnemySlowMouth enemySlowMouth;

	internal State state;

	internal State statePrev;

	public SemiPuke semiPuke;

	public AnimationCurve curveIntroOutro;

	public Transform pukeCapsuleTransform;

	public MeshRenderer pukeCapsuleRenderer;

	private float curveEval;

	private PlayerAvatar playerAvatar;

	public Transform topJawTransform;

	public Transform botJawTransform;

	public GameObject puke;

	private Quaternion topJawStartRotation;

	private Quaternion botJawStartRotation;

	private Quaternion topJawTargetRotation;

	private Quaternion botJawTargetRotation;

	private Vector3 jawStartPosition;

	private Vector3 jawTargetPosition;

	private SpringQuaternion topJawSpring;

	private SpringQuaternion botJawSpring;

	private SpringVector3 jawPositionSpring;

	private bool stateStart = true;

	private float stateTimer;

	private float openAngleTarget = 125f;

	private float pukeTimer;

	internal PlayerAvatar playerTarget;

	public List<ParticleSystem> pukeParticles;

	private void Start()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		jawPositionSpring = new SpringVector3();
		jawPositionSpring.damping = 0.5f;
		jawPositionSpring.speed = 20f;
		topJawSpring = new SpringQuaternion();
		topJawSpring.damping = 0.5f;
		topJawSpring.speed = 20f;
		botJawSpring = new SpringQuaternion();
		botJawSpring.damping = 0.5f;
		botJawSpring.speed = 20f;
		playerAvatar = PlayerAvatar.instance;
		topJawStartRotation = topJawTransform.localRotation;
		botJawStartRotation = botJawTransform.localRotation;
		topJawTargetRotation = topJawStartRotation;
		botJawTargetRotation = topJawStartRotation;
		jawStartPosition = ((Component)this).transform.localPosition;
	}

	private void StateIntro()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateTimer = 1f;
			float num = 125f;
			topJawTransform.localRotation = Quaternion.Euler(num, 0f, 0f);
			botJawTransform.localRotation = Quaternion.Euler(0f - num, 0f, 0f);
			topJawSpring.lastRotation = topJawTransform.localRotation;
			botJawSpring.lastRotation = botJawTransform.localRotation;
			jawTargetPosition = jawStartPosition;
			((Component)this).transform.localPosition = jawTargetPosition + new Vector3(0f, -1f, -1f);
			jawPositionSpring.lastPosition = ((Component)this).transform.localPosition;
			stateStart = false;
		}
		float num2 = curveIntroOutro.Evaluate(1f - stateTimer);
		openAngleTarget = Mathf.LerpUnclamped(125f, 0f, num2);
		topJawTargetRotation = topJawStartRotation * Quaternion.Euler(openAngleTarget, 0f, 0f);
		botJawTargetRotation = botJawStartRotation * Quaternion.Euler(0f - openAngleTarget, 0f, 0f);
		if (stateTimer < 0f)
		{
			StateSet(State.Idle);
		}
	}

	private void StateIdle()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			botJawTargetRotation = botJawStartRotation;
			topJawTargetRotation = topJawStartRotation;
			jawTargetPosition = jawStartPosition;
			stateStart = false;
			if (statePrev == State.Puke)
			{
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
				GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			}
		}
		botJawTargetRotation = botJawStartRotation * Quaternion.Euler(-2f * Mathf.Sin(Time.time * 2f), 0f, 0f);
		topJawTargetRotation = topJawStartRotation * Quaternion.Euler(2f * Mathf.Sin(Time.time * 2f), 0f, 0f);
		if (SemiFunc.IsMultiplayer() && Object.op_Implicit((Object)(object)playerAvatar) && Object.op_Implicit((Object)(object)playerAvatar.voiceChat))
		{
			botJawSpring.speed = 20f;
			topJawSpring.speed = 20f;
			float num = playerAvatar.voiceChat.clipLoudness * 100f;
			num = Mathf.Clamp(num, 0f, 10f);
			topJawTargetRotation *= Quaternion.Euler(topJawStartRotation.x + num, topJawStartRotation.y, topJawStartRotation.z);
			botJawTargetRotation *= Quaternion.Euler(botJawStartRotation.x - num, botJawStartRotation.y, botJawStartRotation.z);
		}
		if (SemiFunc.FPSImpulse15())
		{
			topJawSpring.springVelocity = Random.insideUnitSphere * 0.2f;
			botJawSpring.springVelocity = Random.insideUnitSphere * 0.2f;
		}
	}

	private void StatePuke()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			Vector3 localScale = pukeCapsuleTransform.localScale;
			localScale.x = 0f;
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			pukeCapsuleTransform.localScale = localScale;
		}
		GameDirector.instance.CameraShake.ShakeDistance(4f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		semiPuke.PukeActive(((Component)semiPuke).transform.position, playerTarget.localCameraTransform.rotation);
		pukeTimer = 0.2f;
		botJawTargetRotation = botJawStartRotation * Quaternion.Euler(-25f, 0f, 0f);
		topJawTargetRotation = topJawStartRotation * Quaternion.Euler(25f, 0f, 0f);
		if (SemiFunc.FPSImpulse15())
		{
			topJawSpring.springVelocity = Random.insideUnitSphere * 5f;
			botJawSpring.springVelocity = Random.insideUnitSphere * 5f;
		}
	}

	private void StateOutro()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			stateTimer = 1f;
		}
		float num = curveIntroOutro.Evaluate(1f - stateTimer);
		openAngleTarget = Mathf.LerpUnclamped(0f, 125f, num);
		topJawTargetRotation = topJawStartRotation * Quaternion.Euler(openAngleTarget, 0f, 0f);
		botJawTargetRotation = botJawStartRotation * Quaternion.Euler(0f - openAngleTarget, 0f, 0f);
		jawTargetPosition = Vector3.LerpUnclamped(jawStartPosition, jawStartPosition + new Vector3(0f, 0.2f, -0.4f), num);
		if (stateTimer < 0f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void StateSynchingWithParentEnemy()
	{
		bool flag = enemySlowMouth.currentState == EnemySlowMouth.State.Puke;
		if (enemySlowMouth.currentState == EnemySlowMouth.State.Attached || enemySlowMouth.currentState == EnemySlowMouth.State.Puke || enemySlowMouth.currentState == EnemySlowMouth.State.Detach)
		{
			if (flag)
			{
				StateSet(State.Puke);
			}
			else if (state != 0)
			{
				StateSet(State.Idle);
			}
		}
		else
		{
			StateSet(State.Outro);
		}
	}

	private void StateMachine()
	{
		StateSynchingWithParentEnemy();
		if (stateTimer > 0f)
		{
			stateTimer -= Time.deltaTime;
		}
		switch (state)
		{
		case State.Intro:
			StateIntro();
			break;
		case State.Idle:
			StateIdle();
			break;
		case State.Puke:
			StatePuke();
			break;
		case State.Outro:
			StateOutro();
			break;
		}
	}

	public void StateSet(State newState)
	{
		if (state != newState)
		{
			statePrev = state;
			state = newState;
			stateStart = true;
			stateTimer = 0f;
		}
	}

	private void PukeParticles(bool _play)
	{
		foreach (ParticleSystem pukeParticle in pukeParticles)
		{
			if (_play)
			{
				if (!pukeParticle.isPlaying)
				{
					pukeParticle.Play();
				}
			}
			else if (pukeParticle.isPlaying)
			{
				pukeParticle.Stop();
			}
		}
	}

	private void Update()
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		StateMachine();
		if (pukeTimer > 0f)
		{
			PukeParticles(_play: true);
			pukeTimer -= Time.deltaTime;
			pukeCapsuleTransform.localScale = Vector3.Lerp(pukeCapsuleTransform.localScale, Vector3.one, Time.deltaTime * 5f);
			Transform obj = pukeCapsuleTransform;
			obj.localScale += Vector3.one * Mathf.Sin(Time.time * 30f) * 0.01f;
			Transform obj2 = pukeCapsuleTransform;
			obj2.localScale += Vector3.one * Mathf.Sin(Time.time * 60f) * 0.01f;
			Vector2 textureOffset = ((Renderer)pukeCapsuleRenderer).material.GetTextureOffset("_MainTex");
			textureOffset.x -= Time.deltaTime * 1.5f;
			((Renderer)pukeCapsuleRenderer).material.SetTextureOffset("_MainTex", textureOffset);
			if (!((Component)pukeCapsuleTransform).gameObject.activeSelf)
			{
				pukeCapsuleTransform.localScale = Vector3.zero;
				((Component)pukeCapsuleTransform).gameObject.SetActive(true);
			}
		}
		else
		{
			PukeParticles(_play: false);
			pukeCapsuleTransform.localScale = Vector3.Lerp(pukeCapsuleTransform.localScale, Vector3.zero, Time.deltaTime * 5f);
			if (pukeCapsuleTransform.localScale.x < 0.01f && ((Component)pukeCapsuleTransform).gameObject.activeSelf)
			{
				pukeCapsuleTransform.localScale = Vector3.zero;
				((Component)pukeCapsuleTransform).gameObject.SetActive(false);
			}
		}
		((Component)this).transform.localPosition = SemiFunc.SpringVector3Get(jawPositionSpring, jawTargetPosition);
		topJawTransform.localRotation = SemiFunc.SpringQuaternionGet(topJawSpring, topJawTargetRotation);
		botJawTransform.localRotation = SemiFunc.SpringQuaternionGet(botJawSpring, botJawTargetRotation);
	}
}
