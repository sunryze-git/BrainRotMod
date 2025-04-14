using UnityEngine;

public class PropLightEmission : MonoBehaviour
{
	public bool levelLight = true;

	internal bool turnedOff;

	internal Renderer meshRenderer;

	internal Color originalEmission;

	internal Material material;

	private void Awake()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		meshRenderer = ((Component)this).GetComponent<Renderer>();
		material = meshRenderer.material;
		originalEmission = material.GetColor("_EmissionColor");
	}
}
