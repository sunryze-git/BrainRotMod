using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
	public static PlayerCollision instance;

	public PlayerController Player;

	public Transform StandCollision;

	public Transform CrouchCollision;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Player.Crouching && CameraCrouchPosition.instance.Active && CameraCrouchPosition.instance.Lerp > 0.5f)
		{
			((Component)this).transform.localScale = CrouchCollision.localScale;
		}
		else
		{
			((Component)this).transform.localScale = StandCollision.localScale;
		}
	}

	public void SetCrouchCollision()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = CrouchCollision.localScale;
	}
}
