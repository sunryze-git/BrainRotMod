using System.Collections;
using UnityEngine;

public class EnemyPlayerDistance : MonoBehaviour
{
	private Enemy Enemy;

	public Transform CheckTransform;

	private bool LogicActive;

	internal float PlayerDistanceLocal = 1000f;

	internal float PlayerDistanceClosest = 1000f;

	private void Start()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
		LogicActive = true;
		((MonoBehaviour)this).StartCoroutine(Logic());
	}

	private void OnDisable()
	{
		LogicActive = false;
		((MonoBehaviour)this).StopAllCoroutines();
	}

	private void OnEnable()
	{
		if (!LogicActive)
		{
			LogicActive = true;
			((MonoBehaviour)this).StartCoroutine(Logic());
		}
	}

	private IEnumerator Logic()
	{
		while (true)
		{
			PlayerDistanceLocal = 999f;
			PlayerDistanceClosest = 999f;
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				float num = Vector3.Distance(CheckTransform.position, player.PlayerVisionTarget.VisionTransform.position);
				if (player.isLocal)
				{
					PlayerDistanceLocal = num;
				}
				if (!player.isDisabled && num < PlayerDistanceClosest)
				{
					PlayerDistanceClosest = num;
				}
			}
			yield return (object)new WaitForSeconds(0.25f);
		}
	}
}
