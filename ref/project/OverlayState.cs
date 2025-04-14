using UnityEngine;

public class OverlayState : MonoBehaviour
{
	public GameObject Play;

	public GameObject Stop;

	public GameObject Rewind;

	[Space]
	public RewindEffect RewindEffect;

	private void Update()
	{
		if (RewindEffect.PlayRewind)
		{
			Play.SetActive(false);
			Stop.SetActive(false);
			Rewind.SetActive(true);
		}
		else if (GameDirector.instance.currentState < GameDirector.gameState.Outro)
		{
			Play.SetActive(true);
			Stop.SetActive(false);
			Rewind.SetActive(false);
		}
		else
		{
			Play.SetActive(false);
			Stop.SetActive(true);
			Rewind.SetActive(false);
		}
	}
}
