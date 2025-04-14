using System.Collections.Generic;
using UnityEngine;

public class SemiZuperFlames : MonoBehaviour
{
	public enum State
	{
		None,
		FlamesStart,
		Flames,
		FlamesEnd
	}

	public State state;

	public Transform BaseParticlesTransform;

	private bool flamesActive;

	private bool stateStart;

	private bool baseParticlesPlaying;

	public Light flameLight;

	public List<ParticleSystem> pukeParticles = new List<ParticleSystem>();

	public ParticleSystem pukeEnd = new ParticleSystem();

	private List<ParticleSystem> baseParticles = new List<ParticleSystem>();

	public Sound soundPukeStart;

	public Sound soundPukeEnd;

	public Sound soundPukeLoop;

	private void StateNone()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		PlayBaseParticles(_play: false);
		if (flamesActive)
		{
			StateSet(State.FlamesStart);
		}
	}

	private void StateFlamesStart()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			soundPukeStart.Play(((Component)this).transform.position);
			((Behaviour)flameLight).enabled = true;
			flameLight.intensity = 0f;
			PlayAllParticles(_play: true);
			stateStart = false;
		}
		PlayBaseParticles(_play: true);
		StateSet(State.Flames);
	}

	private void StateFlames()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		PlayBaseParticles(_play: true);
		flameLight.intensity = Mathf.Lerp(flameLight.intensity, 1f, Time.deltaTime * 20f);
		Light obj = flameLight;
		obj.intensity += Mathf.Sin(Time.time * 20f) * 0.05f;
		if (!flamesActive)
		{
			StateSet(State.FlamesEnd);
		}
	}

	private void StateFlamesEnd()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			soundPukeEnd.Play(((Component)this).transform.position);
			((Behaviour)flameLight).enabled = false;
			PlayAllParticles(_play: false);
			pukeEnd.Play();
		}
		PlayBaseParticles(_play: false);
		flameLight.intensity = Mathf.Lerp(flameLight.intensity, 0f, Time.deltaTime * 40f);
		if (flameLight.intensity < 0.01f)
		{
			flameLight.intensity = 0f;
			StateSet(State.None);
		}
	}

	private void PlayAllParticles(bool _play)
	{
		foreach (ParticleSystem pukeParticle in pukeParticles)
		{
			if (_play)
			{
				pukeParticle.Play();
			}
			else
			{
				pukeParticle.Stop();
			}
		}
	}

	private void StateMachine()
	{
		bool playing = flamesActive;
		soundPukeLoop.PlayLoop(playing, 2f, 2f);
		switch (state)
		{
		case State.None:
			StateNone();
			break;
		case State.FlamesStart:
			StateFlamesStart();
			break;
		case State.Flames:
			StateFlames();
			break;
		case State.FlamesEnd:
			StateFlamesEnd();
			break;
		}
	}

	private void Start()
	{
		baseParticles = new List<ParticleSystem>(((Component)BaseParticlesTransform).GetComponentsInChildren<ParticleSystem>());
	}

	private void Update()
	{
		StateMachine();
	}

	private void PlayBaseParticles(bool _play)
	{
		if (baseParticlesPlaying == _play)
		{
			return;
		}
		foreach (ParticleSystem baseParticle in baseParticles)
		{
			if (_play)
			{
				baseParticle.Play();
			}
			else
			{
				baseParticle.Stop();
			}
		}
		baseParticlesPlaying = _play;
	}

	private void StateSet(State _state)
	{
		if (state != _state)
		{
			state = _state;
			stateStart = true;
		}
	}

	public void FlamesActive(Vector3 _position, Quaternion _direction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = Vector3.zero;
		((Component)this).transform.localRotation = Quaternion.identity;
		((Component)this).transform.position = _position;
		((Component)this).transform.rotation = _direction;
		flamesActive = true;
	}

	public void FlamesInactive()
	{
		flamesActive = false;
	}
}
