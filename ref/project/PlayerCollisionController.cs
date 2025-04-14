using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
	public Transform FollowTarget;

	public Vector3 Offset;

	public PlayerCollisionGrounded CollisionGrounded;

	[Space]
	public float GroundedDisableTimer;

	public bool Grounded;

	internal float fallDistance;

	private float fallLastY;

	private float tumbleVelocityTime;

	private float fallLoopPitch;

	private float fallLoopStopTimer;

	private float volume;

	public Sound soundFallLoop;

	private void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (GroundedDisableTimer > 0f)
		{
			GroundedDisableTimer -= Time.deltaTime;
		}
		((Component)this).transform.position = FollowTarget.position + Offset;
		((Component)this).transform.rotation = FollowTarget.rotation;
		if (PlayerController.instance.playerAvatarScript.fallDamageResetState || SemiFunc.MenuLevel())
		{
			ResetFalling();
		}
		PlayerTumble tumble = PlayerController.instance.playerAvatarScript.tumble;
		if (Object.op_Implicit((Object)(object)tumble) && !Grounded && (!tumble.isTumbling || (float)tumble.physGrabObject.playerGrabbing.Count <= 0f))
		{
			if (GameDirector.instance.currentState == GameDirector.gameState.Main && fallLastY - ((Component)this).transform.position.y > 0f)
			{
				fallDistance += Mathf.Abs(((Component)this).transform.position.y - fallLastY);
			}
			fallLastY = ((Component)this).transform.position.y;
			if (PlayerController.instance.featherTimer > 0f || PlayerController.instance.antiGravityTimer > 0f)
			{
				fallDistance = 0f;
			}
		}
		else
		{
			fallLastY = ((Component)this).transform.position.y;
			fallDistance = 0f;
		}
		if (LevelGenerator.Instance.Generated)
		{
			PlayerController.instance.playerAvatarScript.isGrounded = Grounded;
		}
		float num = 0f;
		bool flag = false;
		if (Object.op_Implicit((Object)(object)tumble) && tumble.isTumbling)
		{
			if (((Vector3)(ref tumble.physGrabObject.rbVelocity)).magnitude > 6f)
			{
				tumbleVelocityTime += Time.deltaTime;
				if (tumbleVelocityTime > 0.5f || ((Vector3)(ref tumble.physGrabObject.rbVelocity)).magnitude > 8f)
				{
					if (((Vector3)(ref tumble.physGrabObject.rbVelocity)).magnitude > 15f)
					{
						fallLoopStopTimer = 0f;
					}
					flag = true;
				}
			}
			else
			{
				tumbleVelocityTime = 0f;
			}
			num = Mathf.Clamp(((Vector3)(ref tumble.physGrabObject.rbVelocity)).magnitude / 20f, 0.8f, 1.25f);
		}
		fallLoopPitch = Mathf.Lerp(fallLoopPitch, num, 10f * Time.deltaTime);
		if (fallLoopStopTimer > 0f)
		{
			volume = 0f;
			fallLoopStopTimer -= Time.deltaTime;
			soundFallLoop.PlayLoop(playing: false, 2f, 20f, fallLoopPitch);
			return;
		}
		if (!flag)
		{
			volume = 0f;
		}
		else
		{
			volume = Mathf.Lerp(volume, 1f, 0.75f * Time.deltaTime);
		}
		soundFallLoop.PlayLoop(flag, 5f, 5f, fallLoopPitch);
		soundFallLoop.LoopVolume = volume;
	}

	public void StopFallLoop()
	{
		fallLoopStopTimer = 1f;
	}

	public void ResetFalling()
	{
		StopFallLoop();
		fallDistance = 0f;
	}
}
