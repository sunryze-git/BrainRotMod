using UnityEngine;

public class CleanSpotIdentifier : MonoBehaviour
{
	public Interaction.InteractionType InteractionType;

	private void Start()
	{
		CleanDirector.instance.CleanList.Add(((Component)this).gameObject);
	}

	private void Update()
	{
	}
}
