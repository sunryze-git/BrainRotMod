using UnityEngine;

public class ItemGrenadeStun : MonoBehaviour
{
	public Sound soundExplosion;

	public Sound soundTinnitus;

	private Transform stunExplosion;

	private ItemGrenade itemGrenade;

	private void Start()
	{
		stunExplosion = ((Component)((Component)this).GetComponentInChildren<StunExplosion>()).transform;
		((Component)stunExplosion).gameObject.SetActive(false);
		itemGrenade = ((Component)this).GetComponent<ItemGrenade>();
	}

	public void Explosion()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		soundExplosion.Play(((Component)this).transform.position);
		soundTinnitus.Play(((Component)this).transform.position);
		GameObject obj = Object.Instantiate<GameObject>(((Component)stunExplosion).gameObject, ((Component)this).transform.position, ((Component)this).transform.rotation);
		obj.transform.parent = null;
		obj.SetActive(true);
		obj.GetComponent<StunExplosion>().itemGrenade = itemGrenade;
	}
}
