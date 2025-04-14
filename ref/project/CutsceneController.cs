using UnityEngine;

public class CutsceneController : MonoBehaviour
{
	public Camera Camera;

	private Camera MainCamera;

	public Transform Parent;

	private Transform PreviousParent;

	private float PreviousFOV;

	private bool Active;

	public GameObject ParentEnable;

	[Space]
	public bool CatchCutscene;

	[Space]
	public float StartTimeMin;

	public float StartTimeMax;

	private float StartTimer;

	private bool Started;

	[Space]
	private bool EndActive;

	public float EndTimer;

	[Space]
	public bool DebugLoop;

	private Animator Animator;

	private void Start()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Animator = ((Component)this).GetComponent<Animator>();
		StartTimer = Random.Range(StartTimeMin, StartTimeMax);
		MainCamera = GameDirector.instance.MainCamera;
		PreviousParent = GameDirector.instance.MainCameraParent;
		PreviousFOV = MainCamera.fieldOfView;
		((Component)MainCamera).transform.parent = Parent;
		MainCamera.fieldOfView = Camera.fieldOfView;
		((Component)MainCamera).transform.localPosition = Vector3.zero;
		((Component)MainCamera).transform.localRotation = Quaternion.identity;
		GameDirector.instance.volumeCutsceneOnly.TransitionTo(0.1f);
		((Behaviour)Camera).enabled = false;
		Active = true;
	}

	private void Update()
	{
		if (!Started)
		{
			HUD.instance.Hide();
			VideoOverlay.Instance.Override(0.1f, 1f, 5f);
			StartTimer -= Time.deltaTime;
			if (StartTimer <= 0f || DebugLoop)
			{
				if (DebugLoop || !GameDirectorStatic.CatchCutscenePlayed || Random.Range(0, 3) == 0)
				{
					Animator.SetBool("Play", true);
					Started = true;
				}
				else
				{
					End();
				}
			}
		}
		if (!Active)
		{
			return;
		}
		VideoOverlay.Instance.Override(0.1f, 1f, 5f);
		GameDirector.instance.SetDisableInput(0.5f);
		MainCamera.fieldOfView = Camera.fieldOfView;
		if (EndActive)
		{
			EndTimer -= Time.deltaTime;
			if (DebugLoop || EndTimer <= 0f)
			{
				End();
			}
		}
		else
		{
			VideoOverlay.Instance.Override(0.1f, 0.2f, 5f);
		}
	}

	private void ResetCamera()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((Component)MainCamera).transform.parent = GameDirector.instance.MainCameraParent;
		MainCamera.fieldOfView = PreviousFOV;
		((Component)MainCamera).transform.localPosition = Vector3.zero;
		((Component)MainCamera).transform.localRotation = Quaternion.identity;
	}

	public void EndSet()
	{
		if (!DebugLoop)
		{
			Animator.SetBool("Play", false);
		}
		EndActive = true;
	}

	private void End()
	{
		if (!DebugLoop)
		{
			GameDirectorStatic.CatchCutscenePlayed = true;
			Active = false;
			ResetCamera();
			if (!CatchCutscene)
			{
				GameDirector.instance.volumeOn.TransitionTo(0.1f);
			}
			ParentEnable.SetActive(false);
		}
	}
}
