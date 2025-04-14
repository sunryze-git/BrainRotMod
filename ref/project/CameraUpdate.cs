using System.Collections.Generic;
using UnityEngine;

public class CameraUpdate : MonoBehaviour
{
	public float updateRate = 0.5f;

	private float updateTimer;

	public List<Camera> cams;

	private void Update()
	{
		if (updateTimer <= 0f - updateRate)
		{
			foreach (Camera cam in cams)
			{
				((Behaviour)cam).enabled = true;
			}
			updateTimer += updateRate;
			return;
		}
		foreach (Camera cam2 in cams)
		{
			((Behaviour)cam2).enabled = false;
		}
		updateTimer -= Time.deltaTime;
	}
}
