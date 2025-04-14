using Photon.Pun;
using TMPro;
using UnityEngine;

public class DebugServerUI : MonoBehaviour
{
	public TextMeshProUGUI Text;

	private void Start()
	{
		if (GameManager.instance.gameMode == 0)
		{
			((TMP_Text)Text).text = "Local";
		}
		else if (PhotonNetwork.IsMasterClient)
		{
			((TMP_Text)Text).text = "Server";
		}
		else
		{
			((TMP_Text)Text).text = "Client";
		}
	}
}
