using Photon.Pun;
using UnityEngine;

public class ItemGrenadeDuctTaped : MonoBehaviour
{
	public GameObject grenadePrefab;

	private ParticleScriptExplosion particleScriptExplosion;

	private PhotonView photonView;

	public Sound soundExplosion;

	public Sound soundExplosionGlobal;

	private void Start()
	{
		particleScriptExplosion = ((Component)this).GetComponent<ParticleScriptExplosion>();
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	public void Explosion()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!SemiFunc.IsMultiplayer())
		{
			Vector3 val = default(Vector3);
			for (int i = 0; i < 3; i++)
			{
				((Vector3)(ref val))._002Ector(0f, 0.2f * (float)i, 0f);
				ItemGrenadeHuman component = Object.Instantiate<GameObject>(grenadePrefab, ((Component)this).transform.position + val, Quaternion.identity).GetComponent<ItemGrenadeHuman>();
				component.Initialize();
				component.Spawn();
			}
		}
		else if (SemiFunc.IsMasterClient())
		{
			Vector3 val2 = default(Vector3);
			for (int j = 0; j < 3; j++)
			{
				((Vector3)(ref val2))._002Ector(0f, 0.2f * (float)j, 0f);
				GameObject obj = PhotonNetwork.Instantiate("Items/Item Grenade Human", ((Component)this).transform.position + val2, Quaternion.identity, (byte)0, (object[])null);
				obj.GetComponent<ItemGrenadeHuman>().Initialize();
				obj.GetComponent<ItemGrenadeHuman>().Spawn();
			}
		}
		particleScriptExplosion.Spawn(((Component)this).transform.position, 0.8f, 50, 100, 4f, onlyParticleEffect: false, disableSound: true);
		soundExplosion.Play(((Component)this).transform.position);
		soundExplosionGlobal.Play(((Component)this).transform.position);
	}
}
