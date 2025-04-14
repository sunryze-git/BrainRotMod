using UnityEngine;

public class SledgehammerHit : MonoBehaviour
{
	public SledgehammerController Controller;

	public Transform LookAtTransform;

	public ToolActiveOffset Intro;

	public Transform MeshTransform;

	[Space]
	public Transform Outro;

	public AnimationCurve OutroCurve;

	private Vector3 OutroPositionStart;

	private Quaternion OutroRotationStart;

	[Space]
	public bool DebugDelayDisable;

	public float DelayTime;

	private float DelayTimer;

	private bool Spawning;

	private RoachTrigger Roach;

	public void Spawn(RoachTrigger roach)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Intro.Active = true;
		Intro.ActiveLerp = 1f;
		Roach = roach;
		((Component)this).transform.position = ((Component)Roach).transform.position;
		((Component)this).transform.LookAt(LookAtTransform);
		DelayTimer = DelayTime;
		((Component)MeshTransform).gameObject.SetActive(false);
		Spawning = true;
	}

	public void Hit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((Component)MeshTransform).gameObject.SetActive(true);
		Controller.SoundHit.Play(((Component)this).transform.position);
		Spawning = false;
	}

	public void Update()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Spawning)
		{
			((Component)this).transform.position = ((Component)Roach.RoachOrbit).transform.position;
			((Component)this).transform.LookAt(LookAtTransform);
		}
		else if (!DebugDelayDisable)
		{
			DelayTimer -= Time.deltaTime;
			if (DelayTimer <= 0f)
			{
				Controller.HitDone();
			}
		}
	}
}
