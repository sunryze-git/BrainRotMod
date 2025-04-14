using Photon.Pun;
using UnityEngine;

public class PlayerHealthGrab : MonoBehaviour
{
	public Transform followTransform;

	public Transform hideTransform;

	public PlayerAvatar playerAvatar;

	internal StaticGrabObject staticGrabObject;

	private Collider physCollider;

	private bool colliderActive = true;

	private float grabbingTimer;

	[Space]
	public AnimationCurve hideCurve;

	private float hideLerp;

	private void Start()
	{
		physCollider = ((Component)this).GetComponent<Collider>();
		staticGrabObject = ((Component)this).GetComponent<StaticGrabObject>();
		if (playerAvatar.isLocal)
		{
			((Behaviour)staticGrabObject).enabled = false;
		}
	}

	private void Update()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		if (this.playerAvatar.isTumbling || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
		{
			if (hideLerp < 1f)
			{
				hideLerp += Time.deltaTime * 5f;
				hideLerp = Mathf.Clamp(hideLerp, 0f, 1f);
				hideTransform.localScale = new Vector3(1f, hideCurve.Evaluate(hideLerp), 1f);
				if (hideLerp >= 1f)
				{
					((Component)hideTransform).gameObject.SetActive(false);
				}
			}
		}
		else if (hideLerp > 0f)
		{
			if (!((Component)hideTransform).gameObject.activeSelf)
			{
				((Component)hideTransform).gameObject.SetActive(true);
			}
			hideLerp -= Time.deltaTime * 2f;
			hideLerp = Mathf.Clamp(hideLerp, 0f, 1f);
			hideTransform.localScale = new Vector3(1f, hideCurve.Evaluate(hideLerp), 1f);
		}
		bool flag = true;
		if (this.playerAvatar.isDisabled || hideLerp > 0f)
		{
			flag = false;
		}
		if (colliderActive != flag)
		{
			colliderActive = flag;
			physCollider.enabled = colliderActive;
		}
		((Component)this).transform.position = followTransform.position;
		((Component)this).transform.rotation = followTransform.rotation;
		if (!colliderActive || (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient))
		{
			return;
		}
		if (staticGrabObject.playerGrabbing.Count > 0)
		{
			grabbingTimer += Time.deltaTime;
			foreach (PhysGrabber item in staticGrabObject.playerGrabbing)
			{
				if (grabbingTimer >= 1f)
				{
					PlayerAvatar playerAvatar = item.playerAvatar;
					if (this.playerAvatar.playerHealth.health != this.playerAvatar.playerHealth.maxHealth && playerAvatar.playerHealth.health > 10)
					{
						this.playerAvatar.playerHealth.HealOther(10, effect: true);
						playerAvatar.playerHealth.HurtOther(10, Vector3.zero, savingGrace: false);
						playerAvatar.HealedOther();
					}
				}
			}
			if (grabbingTimer >= 1f)
			{
				grabbingTimer = 0f;
			}
		}
		else
		{
			grabbingTimer = 0f;
		}
	}
}
