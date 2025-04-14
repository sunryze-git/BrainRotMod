using UnityEngine;

public class VideoGreenScreen : MonoBehaviour
{
	private float distFromPlayer = 2f;

	public Transform greenScreenFloor;

	public static VideoGreenScreen instance;

	private void Start()
	{
		instance = this;
	}

	private void Update()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyUp((KeyCode)268))
		{
			if (((Component)greenScreenFloor).GetComponent<Renderer>().material.color == Color.green)
			{
				((Component)greenScreenFloor).GetComponent<Renderer>().material.color = Color.blue;
				((Component)this).GetComponentInChildren<Renderer>().material.color = Color.blue;
			}
			else
			{
				((Component)greenScreenFloor).GetComponent<Renderer>().material.color = Color.green;
				((Component)this).GetComponentInChildren<Renderer>().material.color = Color.green;
			}
		}
		PostProcessing.Instance.VignetteOverride(Color.black, 0f, 1f, 10f, 10f, 0.2f, ((Component)this).gameObject);
		PostProcessing.Instance.BloomDisable(0.2f);
		PostProcessing.Instance.GrainDisable(0.2f);
		GameplayManager.instance.OverrideCameraAnimation(0f, 0.2f);
		GameplayManager.instance.OverrideCameraNoise(0f, 0.2f);
		GameplayManager.instance.OverrideCameraShake(0f, 0.2f);
		RaycastHit[] array = Physics.RaycastAll(((Component)this).transform.position, Vector3.down, 100f, LayerMask.GetMask(new string[1] { "Default" }));
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit val = array[i];
			if (((Component)((RaycastHit)(ref val)).collider).CompareTag("Wall"))
			{
				greenScreenFloor.position = ((RaycastHit)(ref val)).point + Vector3.up * 0.01f;
			}
		}
		Transform headLookAtTransform = PlayerAvatar.instance.playerAvatarVisuals.headLookAtTransform;
		((Component)this).transform.LookAt(headLookAtTransform);
		((Component)this).transform.position = headLookAtTransform.position + headLookAtTransform.forward * distFromPlayer;
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			distFromPlayer -= 0.1f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			distFromPlayer += 0.1f;
		}
		distFromPlayer = Mathf.Clamp(distFromPlayer, 1f, 10f);
	}
}
