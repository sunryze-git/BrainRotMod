using UnityEngine;

public class EnemyHeadEyeTremble : MonoBehaviour
{
	public float Speed;

	[Space]
	public float TimeMin;

	public float TimeMax;

	private float TimerX;

	private float TimerY;

	[Space]
	public float Min;

	public float Max;

	private float TargetX;

	private float CurrentX;

	private float TargetY;

	private float CurrentY;

	private void Update()
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (TimerX <= 0f)
		{
			TimerX = Random.Range(TimeMin, TimeMax);
			TargetX = Random.Range(Min, Max);
		}
		else
		{
			TimerX -= Time.deltaTime;
		}
		CurrentX = Mathf.Lerp(CurrentX, TargetX, Speed * Time.deltaTime);
		if (TimerY <= 0f)
		{
			TimerY = Random.Range(TimeMin, TimeMax);
			TargetY = Random.Range(Min, Max);
		}
		else
		{
			TimerY -= Time.deltaTime;
		}
		CurrentY = Mathf.Lerp(CurrentY, TargetY, Speed * Time.deltaTime);
		((Component)this).transform.localRotation = Quaternion.Euler(CurrentX, CurrentY, 0f);
	}
}
