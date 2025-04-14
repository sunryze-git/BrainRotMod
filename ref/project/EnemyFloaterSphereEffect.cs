using UnityEngine;

public class EnemyFloaterSphereEffect : MonoBehaviour
{
	public enum FloaterSphereEffectState
	{
		levitate,
		stop,
		smash
	}

	private MeshRenderer meshRenderer;

	private Light lightSphere;

	private FloaterAttackLogic floaterAttack;

	private bool stateStart = true;

	private float originalScale;

	private Color originalLightColor;

	private float originalLightIntensity;

	private float originalLightRange;

	private int myChildNumber;

	private Color originalMaterialColor;

	internal FloaterSphereEffectState state;

	private void Start()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		lightSphere = ((Component)this).GetComponentInChildren<Light>();
		floaterAttack = ((Component)this).GetComponentInParent<FloaterAttackLogic>();
		originalScale = ((Component)this).transform.localScale.x;
		myChildNumber = ((Component)this).transform.GetSiblingIndex();
		originalMaterialColor = ((Renderer)meshRenderer).material.color;
		if (Object.op_Implicit((Object)(object)lightSphere))
		{
			originalLightColor = lightSphere.color;
			originalLightIntensity = lightSphere.intensity;
			originalLightRange = lightSphere.range;
		}
	}

	private void StateMachine()
	{
		switch (state)
		{
		case FloaterSphereEffectState.levitate:
			StateLevitate();
			break;
		case FloaterSphereEffectState.stop:
			StateStop();
			break;
		case FloaterSphereEffectState.smash:
			StateSmash();
			break;
		}
	}

	private void StateLevitate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (stateStart)
		{
			((Component)this).transform.localScale = new Vector3(originalScale, originalScale, originalScale);
			((Renderer)meshRenderer).material.color = originalMaterialColor;
			if (Object.op_Implicit((Object)(object)lightSphere))
			{
				lightSphere.color = originalLightColor;
				lightSphere.intensity = originalLightIntensity;
				lightSphere.range = originalLightRange;
			}
			stateStart = false;
		}
		PulseEffect();
	}

	private void StateStop()
	{
		if (stateStart)
		{
			stateStart = false;
		}
		StopEffect();
	}

	private void StateSmash()
	{
		if (stateStart)
		{
			stateStart = false;
		}
	}

	private void Update()
	{
		StateMachine();
		if (floaterAttack.state == FloaterAttackLogic.FloaterAttackState.levitate || floaterAttack.state == FloaterAttackLogic.FloaterAttackState.start)
		{
			StateSet(FloaterSphereEffectState.levitate);
		}
		if (floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
		{
			StateSet(FloaterSphereEffectState.stop);
		}
		if (floaterAttack.state == FloaterAttackLogic.FloaterAttackState.smash)
		{
			StateSet(FloaterSphereEffectState.smash);
		}
	}

	private void StateSet(FloaterSphereEffectState _state)
	{
		if (state != _state)
		{
			state = _state;
			stateStart = true;
		}
	}

	private void PulseEffect()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)((Component)this).transform.parent).transform.localScale == Vector3.zero)
		{
			return;
		}
		Transform transform = ((Component)this).transform;
		transform.localScale += new Vector3(1f, 1f, 1f) * Time.deltaTime * 2f;
		Color color = ((Renderer)meshRenderer).material.color;
		Vector3 localScale = ((Component)this).transform.localScale;
		if (((Vector3)(ref localScale)).magnitude > 10f)
		{
			color.a -= 1f * Time.deltaTime;
			if (Object.op_Implicit((Object)(object)lightSphere))
			{
				lightSphere.intensity = 4f * color.a;
			}
		}
		((Renderer)meshRenderer).material.color = color;
		if (Object.op_Implicit((Object)(object)lightSphere))
		{
			lightSphere.range = ((Component)this).transform.localScale.x * 2.8f;
		}
		Material material = ((Renderer)meshRenderer).material;
		material.mainTextureOffset += new Vector2(0.1f, 0.1f) * Time.deltaTime;
		if (color.a <= 0f)
		{
			if (Object.op_Implicit((Object)(object)lightSphere))
			{
				lightSphere.intensity = 4f;
			}
			if (Object.op_Implicit((Object)(object)lightSphere))
			{
				lightSphere.range = 0f;
			}
			((Component)this).transform.localScale = Vector3.zero;
			color.a = 1f;
			((Renderer)meshRenderer).material.color = color;
		}
	}

	private void StopEffect()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)lightSphere))
		{
			Color red = Color.red;
			float num = 8f;
			float num2 = 15f;
			lightSphere.color = Color.Lerp(lightSphere.color, red, Time.deltaTime * 10f);
			lightSphere.intensity = Mathf.Lerp(lightSphere.intensity, num, Time.deltaTime * 10f);
			lightSphere.range = Mathf.Lerp(lightSphere.range, num2, Time.deltaTime * 10f);
		}
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, Vector3.one, Time.deltaTime * 10f);
		Transform transform = ((Component)this).transform;
		transform.localScale += new Vector3(0.4f, 0.4f, 0.4f) * Mathf.Sin((Time.time + (float)(myChildNumber * 10)) * (float)myChildNumber * 20f) * (0.1f + (float)myChildNumber / 10f);
		((Renderer)meshRenderer).material.color = Color.Lerp(((Renderer)meshRenderer).material.color, Color.red, Time.deltaTime * 10f);
	}
}
