using UnityEngine;

public class VaseExplosionTest : MonoBehaviour
{
	public Transform Center;

	private ParticleScriptExplosion particleScriptExplosion;

	private void Start()
	{
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
	}

	public void Explosion()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		particleScriptExplosion.Spawn(Center.position, 1f, 10, 10);
	}
}
