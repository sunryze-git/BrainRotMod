using UnityEngine;

public class EnemyHeadCatchCutscene : MonoBehaviour
{
	public Sound BiteBegin;

	public Sound BiteFirst;

	public Sound BiteLast;

	[Space]
	public Sound Music01;

	public Sound Music02;

	[Space]
	public Sound CameraGlitchLong01;

	public Sound CameraGlitchLong02;

	[Space]
	public Sound CameraGlitchShort01;

	public Sound CameraGlitchShort02;

	[Space]
	public Sound GlassBreak01;

	public Sound GlassBreak02;

	public Sound GlassTension;

	public void PlayBiteBegin()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BiteBegin.Play(((Component)this).transform.position);
	}

	public void PlayBiteFirst()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		BiteFirst.Play(((Component)this).transform.position);
		CameraGlitchShort01.Play(((Component)this).transform.position);
		GlassBreak01.Play(((Component)this).transform.position);
	}

	public void PlayBiteLast()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		BiteLast.Play(((Component)this).transform.position);
		CameraGlitchShort02.Play(((Component)this).transform.position);
		GlassBreak02.Play(((Component)this).transform.position);
	}

	public void PlayMusic01()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Music01.Play(((Component)this).transform.position);
	}

	public void PlayMusic02()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Music02.Play(((Component)this).transform.position);
	}

	public void PlayCameraGlitchLong01()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		CameraGlitchLong01.Play(((Component)this).transform.position);
	}

	public void PlayCameraGlitchLong02()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		CameraGlitchLong02.Play(((Component)this).transform.position);
	}

	public void PlayGlassTension()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		GlassTension.Play(((Component)this).transform.position);
	}
}
