using UnityEngine;

public class EnemyHeadEyeIdle : MonoBehaviour
{
	public EnemyHeadEyeTarget EyeTarget;

	public float Speed;

	[Space]
	public float TimeMin;

	public float TimeMax;

	private float Timer;

	[Space]
	public float MinX;

	public float MaxX;

	private float CurrentX;

	[Space]
	public float MinY;

	public float MaxY;

	private float CurrentY;

	private void Update()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (EyeTarget.Idle)
		{
			if (Timer <= 0f)
			{
				Timer = Random.Range(TimeMin, TimeMax);
				CurrentX = Random.Range(MinX, MaxX);
				CurrentY = Random.Range(MinY, MaxY);
			}
			else
			{
				Timer -= Time.deltaTime;
			}
		}
		else
		{
			CurrentX = 0f;
			CurrentY = 0f;
		}
		((Component)this).transform.localPosition = Vector3.Lerp(((Component)this).transform.localPosition, new Vector3(CurrentX, CurrentY, ((Component)this).transform.localPosition.z), Speed * Time.deltaTime);
	}
}
