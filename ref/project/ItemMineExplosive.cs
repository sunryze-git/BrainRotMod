using UnityEngine;

public class ItemMineExplosive : MonoBehaviour
{
	private ParticleScriptExplosion particleScriptExplosion;

	private void Start()
	{
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
	}

	public void OnTriggered()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		particleScriptExplosion.Spawn(((Component)this).transform.position, 1.2f, 75, 200, 4f);
	}
}
