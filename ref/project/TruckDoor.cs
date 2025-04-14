using System.Collections;
using UnityEngine;

public class TruckDoor : MonoBehaviour
{
	public Sound doorLoopStart;

	public Sound doorLoopEnd;

	public Sound doorSound;

	private float startYPosition;

	private bool fullyOpen;

	private float doorEval;

	public AnimationCurve doorCurve;

	public Transform doorMesh;

	private float doorDelay = 2f;

	private bool doorOpen;

	private ExtractionPoint extractionPointNearest;

	private float playerInTruckCheckTimer;

	private bool timeToCheck;

	private bool introActivationDone;

	private void Start()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		playerInTruckCheckTimer = 2f;
		startYPosition = ((Component)this).transform.position.y;
		((MonoBehaviour)this).StartCoroutine(DelayedStart());
	}

	private IEnumerator DelayedStart()
	{
		while (!SemiFunc.LevelGenDone())
		{
			yield return (object)new WaitForSeconds(0.3f);
		}
		while (!Object.op_Implicit((Object)(object)extractionPointNearest))
		{
			yield return (object)new WaitForSeconds(0.1f);
			extractionPointNearest = SemiFunc.ExtractionPointGetNearest(((Component)this).transform.position);
		}
		timeToCheck = true;
	}

	private void Update()
	{
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (timeToCheck)
		{
			if (playerInTruckCheckTimer > 0f)
			{
				playerInTruckCheckTimer -= Time.deltaTime;
			}
			else
			{
				playerInTruckCheckTimer = 0.5f;
				if (!introActivationDone && !SemiFunc.PlayersAllInTruck())
				{
					introActivationDone = true;
					if (!TutorialDirector.instance.tutorialActive)
					{
						extractionPointNearest.ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck();
					}
				}
			}
		}
		if (doorDelay > 0f && SemiFunc.LevelGenDone())
		{
			doorDelay -= Time.deltaTime;
		}
		if (doorDelay <= 0f && doorEval < 1f)
		{
			if (!doorOpen)
			{
				doorOpen = true;
				if (SemiFunc.RunIsShop())
				{
					SemiFunc.UIFocusText("Buy stuff in the shop", Color.white, AssetManager.instance.colorYellow);
				}
				GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
				doorLoopStart.Play(((Component)this).transform.position);
			}
			float num = doorCurve.Evaluate(doorEval);
			doorEval += 1.5f * Time.deltaTime;
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, startYPosition + 2.5f * num, ((Component)this).transform.position.z);
		}
		if (doorEval >= 1f && !fullyOpen)
		{
			fullyOpen = true;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			doorLoopEnd.Play(((Component)this).transform.position);
			doorSound.Play(((Component)this).transform.position);
		}
	}
}
