using System.Collections.Generic;
using UnityEngine;

public class ArenaPlatform : MonoBehaviour
{
	public enum States
	{
		Idle,
		Warning,
		GoDown,
		End
	}

	private List<ArenaLight> lights;

	internal States currentState;

	private bool stateStart;

	private float stateTimer;

	public MeshRenderer meshRenderer;

	[Space]
	public DirtFinderMapFloor[] map;

	private void Start()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		lights = new List<ArenaLight>();
		lights.AddRange(((Component)this).GetComponentsInChildren<ArenaLight>());
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.black);
	}

	private void StateIdle()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void StateWarning()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			stateStart = false;
			lights.ForEach(delegate(ArenaLight light)
			{
				light.TurnOnArenaWarningLight();
			});
			((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.red);
		}
		Color val = default(Color);
		((Color)(ref val))._002Ector(0.3f, 0f, 0f);
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.Lerp(((Renderer)meshRenderer).material.GetColor("_EmissionColor"), val, Time.deltaTime * 2f));
	}

	private void StateGoDown()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			DirtFinderMapFloor[] array = map;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Hide();
			}
			stateStart = false;
		}
		if (((Component)this).transform.position.y > -60f)
		{
			((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, ((Component)this).transform.position.y - 30f * Time.deltaTime, ((Component)this).transform.position.z);
		}
		else
		{
			StateSet(States.End);
		}
	}

	private void StateEnd()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void StateMachine()
	{
		switch (currentState)
		{
		case States.Idle:
			StateIdle();
			break;
		case States.Warning:
			StateWarning();
			break;
		case States.GoDown:
			StateGoDown();
			break;
		case States.End:
			StateEnd();
			break;
		}
	}

	private void Update()
	{
		StateMachine();
		if (stateTimer > 0f)
		{
			stateTimer -= Time.deltaTime;
		}
	}

	public void PulsateLights()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		lights.ForEach(delegate(ArenaLight light)
		{
			light.PulsateLight();
		});
		((Renderer)meshRenderer).material.SetColor("_EmissionColor", Color.red);
	}

	public void StateSet(States state)
	{
		currentState = state;
		stateStart = true;
	}
}
