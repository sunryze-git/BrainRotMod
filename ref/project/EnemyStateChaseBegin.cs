using UnityEngine;

public class EnemyStateChaseBegin : MonoBehaviour
{
	private Enemy Enemy;

	private PlayerController Player;

	[HideInInspector]
	public bool Active;

	[Space]
	public float StateTimeMin;

	public float StateTimeMax;

	private float StateTimer;

	[Space]
	internal PlayerAvatar TargetPlayer;

	[HideInInspector]
	public bool LocalEffect;

	[Space]
	public bool Stinger;

	private void Start()
	{
		Enemy = ((Component)this).GetComponent<Enemy>();
		Player = PlayerController.instance;
	}

	private void Update()
	{
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		if (Enemy.CurrentState != EnemyState.ChaseBegin)
		{
			if (Active)
			{
				Active = false;
			}
			return;
		}
		if (!Active)
		{
			if (Enemy.MasterClient)
			{
				Enemy.StateChase.ChaseCanReach = true;
				Enemy.NavMeshAgent.ResetPath();
				StateTimer = Random.Range(StateTimeMin, StateTimeMax);
			}
			TargetPlayer = PlayerController.instance.playerAvatarScript;
			foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
			{
				if (!player.isDisabled && player.photonView.ViewID == Enemy.TargetPlayerViewID)
				{
					TargetPlayer = player;
				}
			}
			foreach (PlayerAvatar player2 in GameDirector.instance.PlayerList)
			{
				if (player2.isDisabled || !player2.isLocal)
				{
					continue;
				}
				if (GameManager.instance.gameMode == 0 || (Object)(object)TargetPlayer == (Object)(object)player2 || Enemy.PlayerRoom.SameLocal || Enemy.OnScreen.OnScreenLocal)
				{
					LocalEffect = true;
					GameDirector.instance.CameraImpact.Shake(5f, 0.25f);
					GameDirector.instance.CameraShake.Shake(3f, 0.5f);
					if (Stinger)
					{
						CameraGlitch.Instance.PlayShort();
						AudioScare.instance.PlayImpact();
					}
				}
				else
				{
					LocalEffect = false;
					GameDirector.instance.CameraImpact.ShakeDistance(5f, 5f, 10f, ((Component)this).transform.position, 0.25f);
					GameDirector.instance.CameraShake.ShakeDistance(3f, 5f, 10f, ((Component)this).transform.position, 0.5f);
				}
			}
			Active = true;
		}
		Enemy.SetChaseTimer();
		if (Enemy.MasterClient)
		{
			Enemy.NavMeshAgent.UpdateAgent(0f, 5f);
			Enemy.NavMeshAgent.Stop(0.1f);
			((Component)this).transform.LookAt(((Component)TargetPlayer).transform.position);
			((Component)this).transform.localEulerAngles = new Vector3(0f, ((Component)this).transform.localEulerAngles.y, 0f);
			StateTimer -= Time.deltaTime;
			if (StateTimer <= 0f)
			{
				Enemy.CurrentState = EnemyState.Chase;
			}
		}
	}
}
