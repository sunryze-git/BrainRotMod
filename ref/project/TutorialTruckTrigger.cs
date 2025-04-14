using UnityEngine;

public class TutorialTruckTrigger : MonoBehaviour
{
	private float lockLookTimer;

	public Transform lookTarget;

	private float messageDelay = 1.5f;

	private bool messageSent;

	private bool triggered;

	private void OnTriggerEnter(Collider other)
	{
		if (((Component)other).CompareTag("Player"))
		{
			triggered = true;
		}
	}

	private void Update()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (triggered && ((Component)this).GetComponent<Collider>().enabled)
		{
			lockLookTimer = 0.5f;
			((Component)this).GetComponent<Collider>().enabled = false;
			CameraGlitch.Instance.PlayLong();
		}
		if (lockLookTimer > 0f)
		{
			lockLookTimer -= Time.deltaTime;
			CameraAim.Instance.AimTargetSet(lookTarget.position + Vector3.down, 0.1f, 5f, ((Component)this).gameObject, 90);
		}
		if (!triggered)
		{
			return;
		}
		if (messageDelay > 0f)
		{
			messageDelay -= Time.deltaTime;
		}
		else if (!messageSent)
		{
			TruckScreenText component = ((Component)lookTarget).GetComponent<TruckScreenText>();
			if (!component.isTyping && component.delayTimer <= 0f)
			{
				component.GotoPage(1);
			}
			messageSent = true;
		}
	}
}
