using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TVCartoonController : MonoBehaviour
{
	public GameObject TVScreen;

	public float runTime = 10f;

	public float TVActiveTimeMin = 20f;

	public float TVActiveTimeMax = 35f;

	private float TVActiveTime;

	private float Timer;

	private float speedMulti = 1f;

	public MeshRenderer TVBackground;

	public GameObject CatObject;

	public GameObject MouseObject;

	public MeshRenderer TVStatic;

	public Light TVLight;

	public AnimationCurve TVStaticCurve;

	public float TVStaticTime = 0.5f;

	public float TVStaticTimer;

	public TrapTV trapTV;

	[Space]
	[Header("___________________ Cartoon Cat Talk ___________________")]
	public float catTalkTime = 10f;

	public float catTalkPauseTime = 2f;

	public int catTalkCount = 2;

	private int catTalkCounter;

	public float catTalkStartScale = 0.2f;

	public float catTalkEndScale = 0.277f;

	public Sound catTalkSound1;

	public Sound catTalkSound2;

	[Space]
	[Header("___________________ Cartoon Mouse Talk ___________________")]
	public float mouseTalkTime = 10f;

	public float mouseTalkPauseTime = 2f;

	public int mouseTalkCount = 2;

	private int mouseTalkCounter;

	public float mouseTalkStartScale = 0.12f;

	public float mouseTalkEndScale = 0.155f;

	public Sound mouseTalkSound1;

	public Sound mouseTalkSound2;

	private int state;

	private int stateRunning;

	private int stateCatTalk = 1;

	private int stateCatTalkPause = 2;

	private int stateMouseTalk = 3;

	private int stateMouseTalkPause = 4;

	private float updateInterval = 1f / 12f;

	private bool TVStaticIntro = true;

	public bool TVStaticOutro;

	public bool isLocal;

	[Space]
	[Header("___________________ TV Sounds ___________________")]
	public Sound LoopSound;

	public Sound StartSound;

	public Sound StopSound;

	[HideInInspector]
	public bool TVActivated;

	[HideInInspector]
	public bool TrapDone;

	private bool TVStart = true;

	private Material CatMaterial;

	private Material MouseMaterial;

	private PhotonView photonView;

	private void Start()
	{
		TVStaticOutro = false;
		TVActivated = false;
		((Renderer)TVBackground).enabled = false;
		((Renderer)TVStatic).enabled = false;
		TVStaticIntro = true;
		TVActiveTime = Random.Range(TVActiveTimeMin, TVActiveTimeMax);
		((Behaviour)TVLight).enabled = false;
		CatObject.SetActive(false);
		MouseObject.SetActive(false);
		CatMaterial = CatObject.GetComponent<Renderer>().material;
		MouseMaterial = MouseObject.GetComponent<Renderer>().material;
		photonView = ((Component)this).GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			isLocal = true;
		}
	}

	private IEnumerator AnimationCoroutine()
	{
		while (true)
		{
			yield return (object)new WaitForSeconds(updateInterval);
			float num = updateInterval;
			if (TrapDone)
			{
				break;
			}
			float num2 = 0.5f;
			if (state == stateCatTalk)
			{
				num2 = 0f;
			}
			if (CatMaterial.mainTextureOffset.x < 1f)
			{
				CatMaterial.mainTextureOffset = new Vector2(CatMaterial.mainTextureOffset.x + 0.33f, num2);
			}
			else
			{
				CatMaterial.mainTextureOffset = new Vector2(0f, num2);
			}
			float num3 = 0.5f;
			if (state == stateMouseTalk)
			{
				num3 = 0f;
			}
			if (CatMaterial.mainTextureOffset.x < 1f)
			{
				MouseMaterial.mainTextureOffset = new Vector2(MouseMaterial.mainTextureOffset.x + 0.33f, num3);
			}
			else
			{
				MouseMaterial.mainTextureOffset = new Vector2(0f, num3);
			}
			if (state == stateRunning)
			{
				Timer += num * speedMulti;
				if (Timer > runTime)
				{
					Timer = 0f;
					state = stateCatTalk;
					catTalkSound1.Play(((Component)this).transform.position);
				}
			}
			if (state == stateCatTalk)
			{
				Timer += num * speedMulti;
				if (Timer > catTalkTime)
				{
					Timer = 0f;
					state = stateCatTalkPause;
					catTalkCounter++;
				}
			}
			if (state == stateCatTalkPause)
			{
				Timer += num * speedMulti;
				if (Timer > catTalkPauseTime)
				{
					Timer = 0f;
					if (catTalkCounter < catTalkCount)
					{
						state = stateCatTalk;
						catTalkSound2.Play(((Component)this).transform.position);
					}
					else
					{
						catTalkCounter = 0;
						state = stateMouseTalk;
						mouseTalkSound1.Play(((Component)this).transform.position);
					}
				}
			}
			if (state == stateMouseTalk)
			{
				Timer += num * speedMulti;
				if (Timer > mouseTalkTime)
				{
					Timer = 0f;
					state = stateMouseTalkPause;
					mouseTalkCounter++;
				}
			}
			if (state != stateMouseTalkPause)
			{
				continue;
			}
			Timer += num * speedMulti;
			if (Timer > mouseTalkPauseTime)
			{
				Timer = 0f;
				if (mouseTalkCounter < mouseTalkCount)
				{
					state = stateMouseTalk;
					mouseTalkSound2.Play(((Component)this).transform.position);
				}
				else
				{
					mouseTalkCounter = 0;
					state = stateRunning;
				}
			}
		}
	}

	private void Update()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		LoopSound.PlayLoop(TVActivated, 0.9f, 0.9f);
		if (!TVActivated)
		{
			return;
		}
		if (isLocal)
		{
			TVActiveTime -= Time.deltaTime;
		}
		if (TVActiveTime <= TVStaticTime)
		{
			_ = TVStaticOutro;
		}
		if (TVStart)
		{
			((Renderer)TVBackground).enabled = true;
			((Behaviour)TVLight).enabled = true;
			TVStart = false;
			StartSound.Play(((Component)this).transform.position);
			CatObject.SetActive(true);
			MouseObject.SetActive(true);
			((MonoBehaviour)this).StartCoroutine(AnimationCoroutine());
		}
		if (TVStaticIntro || TVStaticOutro)
		{
			float num = TVStaticCurve.Evaluate(TVStaticTimer / TVStaticTime);
			TVStaticTimer += 1f * Time.deltaTime * speedMulti;
			if (num > 0.5f)
			{
				((Renderer)TVStatic).enabled = true;
			}
			else
			{
				((Renderer)TVStatic).enabled = false;
			}
			if (TVStaticTimer > TVStaticTime)
			{
				TVStaticIntro = false;
				TVStaticTimer = 0f;
				((Renderer)TVStatic).enabled = false;
			}
		}
		_ = TVActiveTime;
		_ = 0f;
	}
}
