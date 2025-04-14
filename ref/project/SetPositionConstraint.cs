using UnityEngine;
using UnityEngine.Animations;

public class SetPositionConstraint : MonoBehaviour
{
	private void Start()
	{
		((Component)this).GetComponent<PositionConstraint>().constraintActive = true;
	}
}
