using UnityEngine;

public class Fire : MonoBehaviour
{
	public PropLight propLight;

	[Space]
	public Sound soundHit;

	private void Update()
	{
		if (propLight.turnedOff)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	public void OnHit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		soundHit.Play(((Component)this).transform.position);
	}
}
