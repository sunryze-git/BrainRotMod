using System.Collections.Generic;
using UnityEngine;

public class EnemySlowMouthAttaching : MonoBehaviour
{
	internal EnemySlowMouth enemySlowMouth;

	public Transform tentaclesTransform;

	public List<Transform> eyeTransforms = new List<Transform>();

	public Transform topJaw;

	public Transform bottomJaw;

	public Transform particleTransform;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private Quaternion startRotationTopJaw;

	private Quaternion startRotationBottomJaw;

	internal Transform targetTransform;

	public PlayerAvatar targetPlayerAvatar;

	private SpringQuaternion springQuaternion;

	private bool isActive;

	private Vector3 startPosition;

	private SpringFloat springFloatScale;

	private float targetScale = 1f;

	public GameObject topJawPrefab;

	public GameObject bottomJawPrefab;

	public GameObject localPlayerJaw;

	[Space(20f)]
	public Sound soundAttachVO;

	public Sound soundAttach;

	private void Start()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		springQuaternion = new SpringQuaternion();
		springQuaternion.damping = 0.5f;
		springQuaternion.speed = 20f;
		springFloatScale = new SpringFloat();
		springFloatScale.damping = 0.5f;
		springFloatScale.speed = 20f;
		startRotationTopJaw = topJaw.localRotation;
		startRotationBottomJaw = bottomJaw.localRotation;
		startPosition = ((Component)this).transform.position;
		particleSystems = new List<ParticleSystem>(((Component)particleTransform).GetComponentsInChildren<ParticleSystem>());
		GoTime();
	}

	public void GoTime()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		PlayParticles(finalPlay: false);
		if (targetPlayerAvatar.isLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		SetTarget(targetPlayerAvatar);
		springFloatScale.springVelocity = 50f;
		isActive = true;
		targetScale = 1f;
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		soundAttachVO.Play(((Component)this).transform.position);
	}

	private void PlayParticles(bool finalPlay)
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			particleSystem.Play();
			if (finalPlay)
			{
				((Component)particleSystem).transform.parent = null;
				Object.Destroy((Object)(object)((Component)particleSystem).gameObject, 4f);
			}
		}
	}

	private void SpawnPlayerJaw()
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (!targetPlayerAvatar.isLocal)
		{
			GameObject obj = Object.Instantiate<GameObject>(topJawPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation);
			GameObject val = Object.Instantiate<GameObject>(bottomJawPrefab, ((Component)this).transform.position, ((Component)this).transform.rotation);
			EnemySlowMouthPlayerAvatarAttached component = obj.GetComponent<EnemySlowMouthPlayerAvatarAttached>();
			component.jawBot = val.transform;
			Transform attachPointJawTop = targetPlayerAvatar.playerAvatarVisuals.attachPointJawTop;
			Transform attachPointJawBottom = targetPlayerAvatar.playerAvatarVisuals.attachPointJawBottom;
			obj.transform.parent = attachPointJawTop;
			component.playerTarget = targetPlayerAvatar;
			component.enemySlowMouth = enemySlowMouth;
			component.semiPuke = val.GetComponentInChildren<SemiPuke>();
			obj.transform.localPosition = Vector3.zero;
			obj.transform.rotation = Quaternion.identity;
			obj.transform.localRotation = Quaternion.identity;
			val.transform.parent = attachPointJawBottom;
			val.transform.localPosition = Vector3.zero;
			val.transform.rotation = Quaternion.identity;
			val.transform.localRotation = Quaternion.identity;
		}
		else
		{
			Transform localCameraTransform = targetPlayerAvatar.localCameraTransform;
			GameObject obj2 = Object.Instantiate<GameObject>(localPlayerJaw, ((Component)this).transform.position, Quaternion.identity, localCameraTransform);
			obj2.transform.localPosition = Vector3.zero;
			obj2.transform.localRotation = Quaternion.identity;
			EnemySlowMouthCameraVisuals component2 = obj2.GetComponent<EnemySlowMouthCameraVisuals>();
			component2.enemySlowMouth = enemySlowMouth;
			component2.playerTarget = targetPlayerAvatar;
		}
	}

	private void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !Object.op_Implicit((Object)(object)targetTransform) || !Object.op_Implicit((Object)(object)targetPlayerAvatar);
		if (!isActive)
		{
			return;
		}
		if (flag)
		{
			Detach();
			return;
		}
		Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - ((Component)this).transform.position);
		((Component)this).transform.rotation = SemiFunc.SpringQuaternionGet(springQuaternion, targetRotation);
		float num = SemiFunc.SpringFloatGet(springFloatScale, targetScale);
		((Component)this).transform.localScale = Vector3.one * num;
		float num2 = Vector3.Distance(((Component)this).transform.position, targetTransform.position);
		float num3 = num2 * 2f;
		if (num3 < 4f)
		{
			num3 = 4f;
		}
		if (num3 > 10f)
		{
			num3 = 10f;
		}
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, targetTransform.position, Time.deltaTime * num3);
		if (num2 < 1f)
		{
			targetScale = 2.5f;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (enemySlowMouth.currentState != EnemySlowMouth.State.Attack)
			{
				if (!targetPlayerAvatar.isDisabled && !enemySlowMouth.IsPossessed())
				{
					AttachToPlayer();
				}
				else
				{
					Detach();
				}
			}
			if (targetPlayerAvatar.isLocal)
			{
				GameDirector.instance.CameraShake.Shake(4f, 0.1f);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!targetPlayerAvatar.isDisabled && !enemySlowMouth.IsPossessed())
		{
			if (num2 < 0.1f)
			{
				AttachToPlayer();
				enemySlowMouth.UpdateState(EnemySlowMouth.State.Attached);
				isActive = false;
			}
		}
		else
		{
			Detach();
		}
		if (targetPlayerAvatar.isLocal)
		{
			CameraAim.Instance.AimTargetSet(((Component)this).transform.position, 0.2f, 20f, ((Component)this).gameObject, 100);
			CameraZoom.Instance.OverrideZoomSet(30f, 0.1f, 8f, 1f, ((Component)this).gameObject, 50);
		}
		tentaclesTransform.localScale = new Vector3(1f + Mathf.Sin(Time.time * 40f) * 0.2f, 1f + Mathf.Sin(Time.time * 60f) * 0.1f, 1f);
		tentaclesTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 20f) * 10f);
		topJaw.localRotation = startRotationTopJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 3f, 0f, 0f);
		bottomJaw.localRotation = startRotationBottomJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 10f, 0f, 0f);
		foreach (Transform eyeTransform in eyeTransforms)
		{
			eyeTransform.localScale = new Vector3(1.5f + Mathf.Sin(Time.time * 40f) * 0.5f, 1.5f + Mathf.Sin(Time.time * 60f) * 0.5f, 1.5f);
		}
	}

	private void AttachToPlayer()
	{
		if (!targetPlayerAvatar.isDisabled && !Object.op_Implicit((Object)(object)((Component)targetPlayerAvatar).GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>()))
		{
			SpawnPlayerJaw();
			Despawn();
		}
	}

	private void Detach()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		soundAttach.Play(((Component)this).transform.position);
		PlayParticles(finalPlay: true);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			enemySlowMouth.detachPosition = ((Component)this).transform.position;
			enemySlowMouth.detachRotation = ((Component)this).transform.rotation;
			enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void Despawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		soundAttach.Play(((Component)this).transform.position);
		PlayParticles(finalPlay: true);
		if (targetPlayerAvatar.isLocal)
		{
			GameDirector.instance.CameraImpact.Shake(8f, 0.1f);
			GameDirector.instance.CameraShake.Shake(5f, 0.1f);
			CameraGlitch.Instance.PlayLong();
		}
		else
		{
			GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, ((Component)this).transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, ((Component)this).transform.position, 0.1f);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void SetTarget(PlayerAvatar _playerAvatar)
	{
		targetTransform = SemiFunc.PlayerGetFaceEyeTransform(_playerAvatar);
		targetPlayerAvatar = _playerAvatar;
	}
}
