using UnityEngine;

public class CameraGlitch : MonoBehaviour
{
	public static CameraGlitch Instance;

	public Camera targetCamera;

	private float targetCameraFOV;

	public Animator Animator;

	public GameObject ActiveParent;

	[Space]
	public int GlitchLongCount;

	public int GlitchShortCount;

	public int GlitchTinyCount;

	[Space]
	public Sound GlitchLong;

	public Sound GlitchShort;

	public Sound GlitchTiny;

	[Space]
	public Sound HurtShort;

	public Sound HurtLong;

	[Space]
	public Sound HealShort;

	public Sound HealLong;

	[Space]
	public Sound Upgrade;

	[Space]
	public Sound doNotLookEffectSound;

	private float doNotLookEffectTimer;

	private float doNotLookEffectImpulseTimer;

	private void Awake()
	{
		Instance = this;
		targetCameraFOV = targetCamera.fieldOfView;
	}

	private void Start()
	{
		ActiveParent.SetActive(false);
	}

	private void Update()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float num = targetCamera.fieldOfView / targetCameraFOV;
		if (num > 1.5f)
		{
			num *= 1.2f;
		}
		if (num < 0.5f)
		{
			num *= 0.8f;
		}
		((Component)this).transform.localScale = new Vector3(num, num, num);
		if (doNotLookEffectTimer > 0f)
		{
			doNotLookEffectSound.PlayLoop(playing: true, 2f, 1f);
			doNotLookEffectTimer -= Time.deltaTime;
			if (doNotLookEffectImpulseTimer <= 0f)
			{
				PlayShort();
				doNotLookEffectImpulseTimer = Random.Range(0.3f, 1f);
			}
			else
			{
				doNotLookEffectImpulseTimer -= Time.deltaTime;
			}
		}
		else
		{
			doNotLookEffectSound.PlayLoop(playing: false, 2f, 1f);
		}
	}

	public void DoNotLookEffectSet()
	{
		doNotLookEffectTimer = 0.1f;
	}

	public void PlayLong()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("Long");
		Animator.SetInteger("Index", Random.Range(0, GlitchLongCount));
		GlitchLong.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.Shake(2f, 0.3f);
	}

	public void PlayShort()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("Short");
		Animator.SetInteger("Index", Random.Range(0, GlitchShortCount));
		GlitchShort.Play(((Component)this).transform.position);
		GameDirector.instance.CameraImpact.Shake(2f, 0.1f);
	}

	public void PlayTiny()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("Tiny");
		Animator.SetInteger("Index", Random.Range(0, GlitchTinyCount));
		GlitchTiny.Play(((Component)this).transform.position);
	}

	public void PlayLongHurt()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("HurtLong");
		Animator.SetInteger("Index", Random.Range(0, GlitchLongCount));
		HurtLong.Play(((Component)this).transform.position);
		GlitchLong.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(5f, 0.2f);
	}

	public void PlayShortHurt()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("HurtShort");
		Animator.SetInteger("Index", Random.Range(0, GlitchShortCount));
		HurtShort.Play(((Component)this).transform.position);
		GlitchShort.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(3f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(3f, 0.2f);
	}

	public void PlayLongHeal()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("HealLong");
		Animator.SetInteger("Index", Random.Range(0, GlitchLongCount));
		HealLong.Play(((Component)this).transform.position);
		GlitchLong.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(2.5f, 0.2f);
	}

	public void PlayShortHeal()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("HealShort");
		Animator.SetInteger("Index", Random.Range(0, GlitchShortCount));
		HealShort.Play(((Component)this).transform.position);
		GlitchShort.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(1.5f, 0.2f);
		GameDirector.instance.CameraImpact.Shake(1.5f, 0.2f);
	}

	public void PlayUpgrade()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Animator.SetTrigger("Upgrade");
		Animator.SetInteger("Index", Random.Range(0, GlitchShortCount));
		Upgrade.Play(((Component)this).transform.position);
		GameDirector.instance.CameraShake.Shake(2f, 0.5f);
		GameDirector.instance.CameraImpact.Shake(2f, 0.5f);
	}
}
