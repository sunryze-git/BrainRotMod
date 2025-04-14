using UnityEngine;

public class EnemyRobePersistent : MonoBehaviour
{
	public EnemyRobe enemyRobe;

	public ParticleSystem particleConstant;

	private void Update()
	{
		if (((Behaviour)enemyRobe).isActiveAndEnabled && enemyRobe.currentState != 0)
		{
			if (!particleConstant.isPlaying)
			{
				particleConstant.Play();
			}
		}
		else if (particleConstant.isPlaying)
		{
			particleConstant.Stop();
		}
	}
}
