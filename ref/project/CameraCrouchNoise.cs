using UnityEngine;

public class CameraCrouchNoise : MonoBehaviour
{
	private PlayerController Player;

	public AnimNoise AnimNoise;

	public float Strength = 1f;

	public float LerpSpeed = 2f;

	private void Start()
	{
		Player = PlayerController.instance;
		AnimNoise.MasterAmount = 0f;
		((Behaviour)AnimNoise).enabled = false;
	}

	private void Update()
	{
		if (Player.Crouching && !Object.op_Implicit((Object)(object)RecordingDirector.instance))
		{
			((Behaviour)AnimNoise).enabled = true;
			AnimNoise.MasterAmount = Mathf.Lerp(AnimNoise.MasterAmount, Strength * GameplayManager.instance.cameraNoise, Time.deltaTime * LerpSpeed);
		}
		else if (((Behaviour)AnimNoise).enabled)
		{
			AnimNoise.MasterAmount = Mathf.Lerp(AnimNoise.MasterAmount, 0f, Time.deltaTime * LerpSpeed);
			if (AnimNoise.MasterAmount < 0.001f)
			{
				((Behaviour)AnimNoise).enabled = false;
			}
		}
	}
}
