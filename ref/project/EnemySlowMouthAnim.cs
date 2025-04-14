using System.Collections.Generic;
using UnityEngine;

public class EnemySlowMouthAnim : MonoBehaviour
{
	public enum State
	{
		Idle,
		Puke,
		Stunned,
		Targetting,
		Attached,
		Aggro,
		SpawnDespawn,
		Death,
		Leave
	}

	public Transform visualsTransform;

	private Vector3 startPos;

	private Vector3 prevPos;

	private SpringQuaternion directionRotation;

	public Transform upperJaw;

	public Transform lowerJaw;

	public Transform headTransform;

	private float upperJawStartRot;

	private float lowerJawStartRot;

	public Sound talkLoop;

	private AudioSource audioSource;

	private SpringFloat jawSpring;

	private float jawOpen;

	public List<Transform> eyes = new List<Transform>();

	private Transform eyeTarget;

	public Transform eyesMiddle;

	private Quaternion eyeRotation;

	private SpringQuaternion eyeRotationSpring;

	public Transform particleTransforn;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private bool particlesPlaying;

	public EnemySlowMouth enemySlowMouth;

	private bool stateStart;

	private SpringFloat eyeLeftSpringScale;

	private SpringFloat eyeRightSpringScale;

	private float paddleTimer;

	private float talkVolume;

	public State state;

	private void StateIdle()
	{
		if (stateStart)
		{
			jawOpen = 0f;
			talkVolume = 0f;
			stateStart = false;
		}
		CodeAnimatedTalk();
		AnimateEyes(5f);
		Paddle(5f, 20f);
	}

	private void StatePuke()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
		}
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		upperJaw.localRotation = Quaternion.Euler(upperJawStartRot - jawOpen, 0f, 0f);
		lowerJaw.localRotation = Quaternion.Euler(lowerJawStartRot + jawOpen, 0f, 0f);
		jawOpen = SemiFunc.SpringFloatGet(jawSpring, targetFloat);
		AnimateEyes(30f);
		EyeScaleSitter(6f);
	}

	private void StateStunned()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			jawOpen = 20f;
			stateStart = false;
		}
		AnimateEyes(20f);
		EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		upperJaw.localRotation = Quaternion.Euler(upperJawStartRot - jawOpen, 0f, 0f);
		lowerJaw.localRotation = Quaternion.Euler(lowerJawStartRot + jawOpen, 0f, 0f);
		jawOpen = SemiFunc.SpringFloatGet(jawSpring, targetFloat);
		Paddle(30f, 10f);
	}

	private void StateTargetting()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			jawOpen = 0f;
			stateStart = false;
		}
		AnimateEyes(20f);
		EyeScaleSitter(4f);
		float targetFloat = 30f + Mathf.Sin(Time.time * 40f) * 10f;
		upperJaw.localRotation = Quaternion.Euler(upperJawStartRot - jawOpen, 0f, 0f);
		lowerJaw.localRotation = Quaternion.Euler(lowerJawStartRot + jawOpen, 0f, 0f);
		jawOpen = SemiFunc.SpringFloatGet(jawSpring, targetFloat);
		Paddle(20f, 12f);
	}

	private void StateAttached()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateAggro()
	{
		if (stateStart)
		{
			jawOpen = 0f;
			stateStart = false;
		}
		AnimateEyes(20f);
		EyeScaleSitter(4f);
		CodeAnimatedTalk();
		Paddle(10f, 20f);
	}

	private void StateSpawnDespawn()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
		}
		upperJaw.localRotation = Quaternion.Slerp(upperJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		lowerJaw.localRotation = Quaternion.Slerp(lowerJaw.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
		Paddle(5f, 20f);
	}

	private void StateDeath()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateLeave()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			jawOpen = 0f;
			talkVolume = 0f;
		}
		float targetFloat = 5f + Mathf.Sin(Time.time * 40f) * 5f;
		upperJaw.localRotation = Quaternion.Euler(upperJawStartRot - jawOpen, 0f, 0f);
		lowerJaw.localRotation = Quaternion.Euler(lowerJawStartRot + jawOpen, 0f, 0f);
		jawOpen = SemiFunc.SpringFloatGet(jawSpring, targetFloat);
		Paddle(10f, 20f);
	}

	private void EyesLookAtTarget()
	{
	}

	private void CodeAnimatedTalk(float _multiplier = 1f)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.FPSImpulse15())
		{
			float[] array = new float[1024];
			audioSource.GetSpectrumData(array, 0, (FFTWindow)2);
			float num = array[0] * 50000f * _multiplier;
			if (num > 20f)
			{
				num = 20f;
			}
			talkVolume = num;
			jawSpring.springVelocity += Random.Range(-25f, 0f);
		}
		upperJaw.localRotation = Quaternion.Euler(upperJawStartRot - jawOpen, 0f, 0f);
		lowerJaw.localRotation = Quaternion.Euler(lowerJawStartRot + jawOpen, 0f, 0f);
		jawSpring.damping = 0.2f;
		jawSpring.speed = 25f;
		jawOpen = SemiFunc.SpringFloatGet(jawSpring, talkVolume);
	}

	private void Start()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		eyeLeftSpringScale = new SpringFloat();
		eyeLeftSpringScale.damping = 0.01f;
		eyeLeftSpringScale.speed = 40f;
		eyeRightSpringScale = new SpringFloat();
		eyeRightSpringScale.damping = 0.01f;
		eyeRightSpringScale.speed = 40f;
		eyeRotationSpring = new SpringQuaternion();
		eyeRotationSpring.damping = 0.01f;
		eyeRotationSpring.speed = 40f;
		startPos = ((Component)this).transform.position;
		directionRotation = new SpringQuaternion();
		directionRotation.damping = 0.5f;
		directionRotation.speed = 10f;
		upperJawStartRot = upperJaw.localEulerAngles.x;
		upperJawStartRot = lowerJaw.localEulerAngles.x;
		audioSource = ((Component)this).GetComponent<AudioSource>();
		jawSpring = new SpringFloat();
		jawSpring.damping = 0.12f;
		jawSpring.speed = 12f;
		eyeTarget = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform;
		particleSystems.AddRange(((Component)particleTransforn).GetComponentsInChildren<ParticleSystem>());
	}

	private void StateMachine()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform eye in eyes)
		{
			if ((Object)(object)eye == (Object)(object)eyes[0])
			{
				eye.localScale = Vector3.one * SemiFunc.SpringFloatGet(eyeLeftSpringScale, 1f);
			}
			else
			{
				eye.localScale = Vector3.one * SemiFunc.SpringFloatGet(eyeRightSpringScale, 1f);
			}
		}
		switch (state)
		{
		case State.Idle:
			StateIdle();
			break;
		case State.Puke:
			StatePuke();
			break;
		case State.Stunned:
			StateStunned();
			break;
		case State.Targetting:
			StateTargetting();
			break;
		case State.Attached:
			StateAttached();
			break;
		case State.Aggro:
			StateAggro();
			break;
		case State.SpawnDespawn:
			StateSpawnDespawn();
			break;
		case State.Death:
			StateDeath();
			break;
		case State.Leave:
			StateLeave();
			break;
		}
		if (state != State.SpawnDespawn && state != State.Death)
		{
			if (jawOpen > 15f)
			{
				PlayParticles(_play: true);
			}
			else
			{
				PlayParticles(_play: false);
			}
		}
		else
		{
			PlayParticles(_play: false);
		}
	}

	public void UpdateState(State _state)
	{
		if (state != _state)
		{
			state = _state;
			stateStart = true;
		}
	}

	private void Update()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		StateMachine();
		if (paddleTimer > 0f)
		{
			paddleTimer -= Time.deltaTime;
		}
		if (paddleTimer <= 0f)
		{
			visualsTransform.localRotation = Quaternion.Slerp(visualsTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
		}
		if (talkVolume > 0f)
		{
			talkVolume = Mathf.Lerp(talkVolume, 0f, Time.deltaTime * 5f);
		}
		if (jawOpen > 0f)
		{
			jawOpen = Mathf.Lerp(jawOpen, 0f, Time.deltaTime * 5f);
		}
	}

	private void AnimateEyes(float _eyeJitterAmount)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (SemiFunc.FPSImpulse15())
		{
			SpringQuaternion springQuaternion = eyeRotationSpring;
			springQuaternion.springVelocity += Random.insideUnitSphere * _eyeJitterAmount;
		}
		foreach (Transform eye in eyes)
		{
			eye.localRotation = SemiFunc.SpringQuaternionGet(eyeRotationSpring, Quaternion.identity);
		}
	}

	private void EyeScaleSitter(float _amount)
	{
		eyeLeftSpringScale.springVelocity += Random.Range(0f, _amount);
		eyeRightSpringScale.springVelocity += Random.Range(0f, _amount);
	}

	public void PlayParticles(bool _play)
	{
		if (particlesPlaying == _play)
		{
			return;
		}
		particlesPlaying = _play;
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	private void Paddle(float _speed, float _amount)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Sin(Time.time * _speed) * _amount;
		visualsTransform.localRotation = Quaternion.Euler(0f, num * 2f, 0f);
		paddleTimer = 0.1f;
	}
}
