using UnityEngine;

public class AudioAttack : MonoBehaviour
{
	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		AudioSource component = ((Component)this).GetComponent<AudioSource>();
		if (Vector3.Distance(((Component)this).transform.position, ((Component)PlayerController.instance).transform.position) < component.maxDistance)
		{
			LevelMusic.instance.Interrupt(10f);
		}
		EnemyDirector.instance.spawnIdlePauseTimer = 0f;
	}
}
