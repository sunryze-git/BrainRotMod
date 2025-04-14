using UnityEngine;

public class PlayerAvatarTalkAnimation : MonoBehaviour
{
	public AudioSource audioSource;

	public PlayerAvatar playerAvatar;

	public GameObject objectToRotate;

	private PlayerAvatarVisuals playerAvatarVisuals;

	[Space]
	public float threshold = 0.01f;

	public float rotationMaxAngle = 45f;

	public float amountMultiplier = 1f;

	private bool audioSourceFetched;

	private void Start()
	{
		playerAvatarVisuals = ((Component)this).GetComponent<PlayerAvatarVisuals>();
	}

	private void Update()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (playerAvatarVisuals.isMenuAvatar && !Object.op_Implicit((Object)(object)playerAvatar))
		{
			playerAvatar = PlayerAvatar.instance;
		}
		if (!GameManager.Multiplayer() || !playerAvatar.voiceChatFetched)
		{
			return;
		}
		if (!audioSourceFetched)
		{
			audioSource = playerAvatar.voiceChat.audioSource;
			audioSourceFetched = true;
		}
		if (Object.op_Implicit((Object)(object)audioSource))
		{
			float num = 0f;
			if (playerAvatar.voiceChat.clipLoudness > 0.005f)
			{
				num = Mathf.Lerp(0f, 0f - rotationMaxAngle, playerAvatar.voiceChat.clipLoudness * 4f);
			}
			objectToRotate.transform.localRotation = Quaternion.Slerp(objectToRotate.transform.localRotation, Quaternion.Euler(num, 0f, 0f), 100f * Time.deltaTime);
		}
	}
}
