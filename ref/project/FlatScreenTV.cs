using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FlatScreenTV : MonoBehaviour
{
	private float timer;

	public Transform regularPlane;

	public Sound regularSound;

	public Sound regularSoundGlobal;

	private bool isActive;

	public Transform regularHurtCollider;

	public Transform visionPoint;

	private PhotonView photonView;

	private StaticGrabObject staticGrabObject;

	private bool broken = true;

	public Transform jumpScare;

	public Transform brokenPlane;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
		staticGrabObject = ((Component)this).GetComponent<StaticGrabObject>();
		((MonoBehaviour)this).StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return (object)new WaitForSeconds(0.1f);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (Random.Range(0, 8) == 0)
			{
				broken = false;
			}
			BrokenOrNot();
		}
	}

	private void BrokenOrNot()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			BrokenOrNotRPC(broken);
			return;
		}
		photonView.RPC("BrokenOrNotRPC", (RpcTarget)0, new object[1] { broken });
	}

	[PunRPC]
	public void BrokenOrNotRPC(bool _broken)
	{
		broken = _broken;
		if (broken)
		{
			((Component)jumpScare).gameObject.SetActive(false);
			((Component)brokenPlane).gameObject.SetActive(true);
		}
		else
		{
			((Component)brokenPlane).gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (timer > 0f)
		{
			if (timer < 1.3f && !((Component)regularHurtCollider).gameObject.activeSelf)
			{
				((Component)regularHurtCollider).gameObject.SetActive(true);
			}
			timer -= Time.deltaTime;
			((Component)regularPlane).GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + Mathf.Sin(timer * 100f) * 0.1f, 1f + Mathf.Sin(timer * 100f) * 0.1f);
			((Component)regularPlane).GetComponent<Renderer>().material.mainTextureOffset = new Vector2((0f - Mathf.Sin(timer * 100f)) * 0.05f, (0f - Mathf.Sin(timer * 100f)) * 0.05f);
			isActive = true;
		}
		else
		{
			if (isActive)
			{
				((Component)regularPlane).gameObject.SetActive(false);
				((Component)regularHurtCollider).gameObject.SetActive(false);
				broken = true;
				BrokenOrNotRPC(broken);
			}
			isActive = false;
		}
	}

	public void actionTime()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				photonView.RPC("actionTimeRPC", (RpcTarget)0, Array.Empty<object>());
			}
		}
		else
		{
			actionTimeRPC();
		}
	}

	[PunRPC]
	public void actionTimeRPC()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!(timer > 0f))
		{
			timer = 1.5f;
			GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, ((Component)this).transform.position, 0.5f);
			GameDirector.instance.CameraImpact.ShakeDistance(8f, 3f, 12f, ((Component)this).transform.position, 0.1f);
			((Component)regularPlane).gameObject.SetActive(true);
			regularSoundGlobal.Play(((Component)this).transform.position);
			regularSound.Play(((Component)this).transform.position);
		}
	}
}
