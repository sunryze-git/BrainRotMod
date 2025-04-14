using System.Collections;
using UnityEngine;

public class ArmIntroController : MonoBehaviour
{
	public bool DebugDisable;

	[Space]
	public Animator Animator;

	public Transform CameraTransform;

	public GameObject Hide;

	[Space]
	public float WaitTimer = 0.25f;

	[Space]
	public Sound MoveShort;

	public Sound MoveLong;

	public Sound GlovePull;

	public Sound GloveSnap;

	public void Start()
	{
		((Behaviour)Animator).enabled = false;
		((Component)this).transform.parent = CameraTransform;
		Hide.SetActive(false);
		((MonoBehaviour)this).StartCoroutine(StartIntro());
	}

	public void Update()
	{
		PlayerController.instance.CrouchDisable(0.1f);
	}

	private IEnumerator StartIntro()
	{
		while (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			yield return null;
		}
		if (DebugDisable)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			yield break;
		}
		yield return (object)new WaitForSeconds(WaitTimer);
		((Behaviour)Animator).enabled = true;
		Hide.SetActive(true);
	}

	public void AnimationDone()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void PlayGlovePull()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GlovePull.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.Shake(0.25f, 1f);
	}

	public void PlayGloveSnap()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GloveSnap.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
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
