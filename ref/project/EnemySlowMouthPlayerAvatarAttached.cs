using System.Collections.Generic;
using UnityEngine;

public class EnemySlowMouthPlayerAvatarAttached : MonoBehaviour
{
	public enum State
	{
		Intro,
		Idle,
		Puke,
		Outro
	}

	public Transform jawBot;

	public Transform particles;

	private SpringFloat springFloatScale;

	internal EnemySlowMouth enemySlowMouth;

	internal SemiPuke semiPuke;

	private float scaleTarget = 1f;

	private bool stateStart;

	internal PlayerAvatar playerTarget;

	public List<Transform> eyeTransforms;

	private PlayerVoiceChat playerVoiceChat;

	private float loudnessAdd;

	private SpringFloat loudnessSpring;

	private float loudnessTarget;

	public State state;

	private void Start()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		loudnessSpring = new SpringFloat();
		loudnessSpring.damping = 0.5f;
		loudnessSpring.speed = 20f;
		springFloatScale = new SpringFloat();
		springFloatScale.damping = 0.35f;
		springFloatScale.speed = 10f;
		((Component)this).transform.localScale = Vector3.one * 2f;
		springFloatScale.lastPosition = 2f;
		playerVoiceChat = playerTarget.voiceChat;
	}

	private void StateIntro()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		loudnessTarget = 0f;
		scaleTarget = 1f;
	}

	private void StateIdle()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		loudnessTarget = 0f;
		scaleTarget = 1f;
	}

	private void StatePuke()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
		}
		float num = Mathf.Sin(Time.time * 40f) * 0.05f;
		loudnessTarget = 0.2f + num;
		scaleTarget = 1f;
		semiPuke.PukeActive(((Component)semiPuke).transform.position, playerTarget.localCameraTransform.rotation);
	}

	private void StateOutro()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			((Component)particles).gameObject.SetActive(true);
			stateStart = false;
		}
		loudnessTarget = 0f;
		scaleTarget = 0f;
		if (((Component)this).transform.localScale.x < 0.05f)
		{
			enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			Object.Destroy((Object)(object)((Component)jawBot).gameObject);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void StateMachine()
	{
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

	private void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)playerVoiceChat))
		{
			playerVoiceChat.OverrideClipLoudnessAnimationValue(loudnessAdd);
		}
		else if (Object.op_Implicit((Object)(object)playerTarget))
		{
			playerVoiceChat = playerTarget.voiceChat;
		}
		loudnessAdd = SemiFunc.SpringFloatGet(loudnessSpring, loudnessTarget);
		Quaternion rotation = playerTarget.playerAvatarVisuals.playerEyes.eyeLeft.rotation;
		Quaternion rotation2 = playerTarget.playerAvatarVisuals.playerEyes.eyeRight.rotation;
		eyeTransforms[0].rotation = rotation;
		eyeTransforms[1].rotation = rotation2;
		StateSynchingWithParentEnemy();
		StateMachine();
		((Component)this).transform.localScale = Vector3.one * SemiFunc.SpringFloatGet(springFloatScale, scaleTarget);
		jawBot.localScale = Vector3.one * SemiFunc.SpringFloatGet(springFloatScale, scaleTarget);
	}

	private void StateSynchingWithParentEnemy()
	{
		if (!Object.op_Implicit((Object)(object)enemySlowMouth))
		{
			StateSet(State.Outro);
			return;
		}
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

	private void StateSet(State _state)
	{
		if (state != _state)
		{
			state = _state;
			stateStart = true;
		}
	}

	private void OnDisable()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (playerTarget.isDisabled)
		{
			enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
			enemySlowMouth.detachPosition = playerTarget.localCameraPosition;
			enemySlowMouth.detachRotation = playerTarget.localCameraRotation;
			Object.Destroy((Object)(object)((Component)jawBot).gameObject);
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
