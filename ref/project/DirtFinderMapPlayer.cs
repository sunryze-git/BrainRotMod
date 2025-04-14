using System.Collections;
using UnityEngine;

public class DirtFinderMapPlayer : MonoBehaviour
{
	public static DirtFinderMapPlayer Instance;

	private Transform PlayerTransform;

	private Vector3 StartOffset;

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Instance = this;
		StartOffset = ((Component)this).transform.position;
	}

	private void OnEnable()
	{
		PlayerTransform = null;
		((MonoBehaviour)this).StartCoroutine(FindPlayer());
	}

	private IEnumerator FindPlayer()
	{
		yield return (object)new WaitForSeconds(0.1f);
		while (!Object.op_Implicit((Object)(object)PlayerTransform))
		{
			if (Object.op_Implicit((Object)(object)PlayerController.instance))
			{
				PlayerTransform = ((Component)PlayerController.instance).transform;
				((MonoBehaviour)this).StartCoroutine(Logic());
			}
			yield return (object)new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator Logic()
	{
		while (true)
		{
			((Component)this).transform.position = ((Component)PlayerTransform).transform.position * Map.Instance.Scale + Map.Instance.OverLayerParent.position + StartOffset;
			((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, 0f, ((Component)this).transform.localPosition.z);
			((Component)this).transform.rotation = PlayerTransform.rotation;
			MapLayer layerParent = Map.Instance.GetLayerParent(PlayerTransform.position.y + 0.01f);
			Map.Instance.PlayerLayer = layerParent.layer;
			yield return (object)new WaitForSeconds(0.1f);
		}
	}
}
