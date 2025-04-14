using System;
using Photon.Pun;
using UnityEngine;

public class ItemMeleeInflatableHammer : MonoBehaviour
{
	private ParticleScriptExplosion particleScriptExplosion;

	private Transform explosionPosition;

	private PhotonView photonView;

	private void Start()
	{
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		explosionPosition = ((Component)this).transform.Find("Explosion Position");
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	public void OnHit()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && Random.Range(0, 19) == 0)
		{
			if (SemiFunc.IsMultiplayer())
			{
				photonView.RPC("ExplosionRPC", (RpcTarget)0, Array.Empty<object>());
			}
			else
			{
				ExplosionRPC();
			}
		}
	}

	[PunRPC]
	public void ExplosionRPC()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ParticlePrefabExplosion particlePrefabExplosion = particleScriptExplosion.Spawn(explosionPosition.position, 0.5f, 0, 250);
		particlePrefabExplosion.SkipHurtColliderSetup = true;
		particlePrefabExplosion.HurtCollider.playerDamage = 0;
		particlePrefabExplosion.HurtCollider.enemyDamage = 250;
		particlePrefabExplosion.HurtCollider.physImpact = HurtCollider.BreakImpact.Heavy;
		particlePrefabExplosion.HurtCollider.physHingeDestroy = true;
		particlePrefabExplosion.HurtCollider.playerTumbleForce = 30f;
		particlePrefabExplosion.HurtCollider.playerTumbleTorque = 50f;
	}
}
