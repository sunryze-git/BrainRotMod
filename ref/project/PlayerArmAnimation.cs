using UnityEngine;

public class PlayerArmAnimation : MonoBehaviour
{
	private PlayerController Player;

	private Animator Animator;

	private int Crouching;

	private int Crawling;

	private PlayerVoice Voice;

	public Sound MoveShort;

	public Sound MoveLong;

	private void Start()
	{
		Player = PlayerController.instance;
		Voice = PlayerVoice.Instance;
		Animator = ((Component)this).GetComponent<Animator>();
		Crouching = Animator.StringToHash("Crouching");
		Crawling = Animator.StringToHash("Crawling");
	}

	private void Update()
	{
		if (Player.Crouching)
		{
			Animator.SetBool(Crouching, true);
		}
		else
		{
			Animator.SetBool(Crouching, false);
			Animator.SetBool(Crawling, false);
		}
		if (Player.Crawling)
		{
			Animator.SetBool(Crawling, true);
		}
		else
		{
			Animator.SetBool(Crawling, false);
		}
	}

	public void PlayCrouchHush()
	{
	}

	public void PlayMoveShort()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MoveShort.Play(((Component)this).transform.position);
	}

	public void PlayMoveLong()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MoveLong.Play(((Component)this).transform.position);
	}
}
