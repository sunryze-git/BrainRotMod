using System;
using Photon.Pun;
using UnityEngine;

public class VacuumSpot : MonoBehaviour
{
	public GameObject VacuumSpotVisual;

	[HideInInspector]
	public float Amount = 1f;

	public float DecreaseSpeed;

	[HideInInspector]
	public bool CleanDone;

	[Space]
	public Transform PileMesh;

	public MeshRenderer PileRenderer;

	private float PileRendererAlpha;

	[Space]
	public Transform DecalMesh;

	public MeshRenderer DecalRenderer;

	private float DecalRendererAlpha;

	[Space]
	public AnimationCurve ScaleCurve;

	public AnimationCurve AlphaCurve;

	public Light Light;

	private float LightIntensity;

	[HideInInspector]
	public bool Decreasing;

	[HideInInspector]
	public float DecreaseTimer;

	public GameObject CleanEffect;

	private bool multiplayerCleaning;

	private PhotonView photonView;

	private bool cleanInputPrevious;

	public bool cleanInput;

	private bool syncDestroy;

	private void Start()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		LightIntensity = Light.intensity;
		photonView = ((Component)this).GetComponent<PhotonView>();
		PileRendererAlpha = ((Renderer)PileRenderer).material.color.a;
		DecalRendererAlpha = ((Renderer)DecalRenderer).material.color.a;
	}

	[PunRPC]
	private void StartCleaningRPC()
	{
		multiplayerCleaning = true;
	}

	[PunRPC]
	private void StopCleaningRPC()
	{
		multiplayerCleaning = false;
	}

	private void Update()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (DecreaseTimer > 0f)
		{
			Amount -= DecreaseSpeed * Time.deltaTime;
			if (Amount > 0.2f)
			{
				DecreaseTimer -= 1f * Time.deltaTime;
			}
			float num = Mathf.Lerp(0f, 1f, AlphaCurve.Evaluate(Amount));
			Color color = ((Renderer)PileRenderer).material.color;
			color.a = PileRendererAlpha * num;
			((Renderer)PileRenderer).material.color = color;
			Color color2 = ((Renderer)DecalRenderer).material.color;
			color2.a = DecalRendererAlpha * num;
			((Renderer)DecalRenderer).material.color = color2;
			float num2 = Mathf.Lerp(0f, 1f, ScaleCurve.Evaluate(Amount));
			PileMesh.localScale = new Vector3(1f - (1f - num2) * 0.4f, 0.5f + num2 * 0.5f, 1f - (1f - num2) * 0.4f);
			DecalMesh.localScale = new Vector3(1f - (1f - num2) * 0.2f, 1f, 1f - (1f - num2) * 0.2f);
			Light.intensity = LightIntensity * num2;
			if (Amount <= 0f)
			{
				CleanEffect.SetActive(true);
				CleanEffect.GetComponent<CleanEffect>().Clean();
				CleanEffect.transform.parent = null;
				if (GameManager.instance.gameMode == 1)
				{
					if (PhotonNetwork.IsMasterClient && !syncDestroy)
					{
						PhotonNetwork.Destroy(((Component)this).gameObject);
						syncDestroy = true;
					}
				}
				else
				{
					Object.Destroy((Object)(object)((Component)this).gameObject);
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			if (cleanInput && cleanInput != cleanInputPrevious)
			{
				photonView.RPC("StartCleaningRPC", (RpcTarget)0, Array.Empty<object>());
			}
			if (!cleanInput && cleanInput != cleanInputPrevious)
			{
				photonView.RPC("StopCleaningRPC", (RpcTarget)0, Array.Empty<object>());
			}
			cleanInputPrevious = cleanInput;
			cleanInput = multiplayerCleaning;
		}
		if (cleanInput)
		{
			DecreaseTimer = 0.1f;
			cleanInput = false;
		}
	}
}
