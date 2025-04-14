using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PickerController : MonoBehaviour
{
	public Transform ParentTransform;

	public AnimationCurve PickerStabIntro;

	public AnimationCurve PickerStabOutro;

	public float AnimationSpeedIntro = 1f;

	public float AnimationSpeedOutro = 1f;

	public GameObject AnimatedPicker;

	[Space]
	public GameObject StabPoint;

	public GameObject StabPointChild;

	[Space]
	private LayerMask Mask;

	private Camera MainCamera;

	public GameObject meshObject;

	public MeshRenderer meshRenderer;

	private bool isAnimating;

	private float animationProgress;

	private float ShowTimer = 0.3f;

	private bool stab;

	private GameObject stabObject;

	private List<GameObject> stabObjects = new List<GameObject>();

	private List<float> stabObjectsAngles = new List<float>();

	[Space]
	public Transform PickerPoint;

	public Transform PickerPointEnd;

	public Transform PickerPointAnimate;

	public Transform PickerPointEndAnimate;

	public float StabObjectSpacing = 10f;

	private bool isStabbing;

	private bool introAnimation = true;

	private Quaternion RotationStart;

	private Quaternion RotationEnd;

	private Vector3 PositionStart;

	private Vector3 PositionEnd;

	private Vector3 ScaleStart;

	private Vector3 ScaleEnd;

	[Space]
	public Sound IntroSound;

	public Sound OutroSound;

	public Sound StabSound;

	private bool OutroAudioPlay = true;

	private void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		IntroSound.Play(((Component)this).transform.position);
		AnimatedPicker.SetActive(false);
		StabPoint.SetActive(false);
		MainCamera = Camera.main;
		Mask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Default" }));
		GameDirector.instance.CameraShake.Shake(2f, 0.25f);
	}

	private void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (OutroAudioPlay && !ToolController.instance.ToolHide.Active)
		{
			OutroSound.Play(((Component)this).transform.position);
			OutroAudioPlay = false;
			GameDirector.instance.CameraShake.Shake(2f, 0.25f);
		}
		if (isAnimating)
		{
			AnimatePicker();
		}
		AlignStabObjects();
		if (ShowTimer > 0f)
		{
			ShowTimer -= Time.deltaTime;
		}
		if (ToolController.instance.Interact)
		{
			isStabbing = true;
		}
		Interaction activeInteraction = ToolController.instance.ActiveInteraction;
		if (Object.op_Implicit((Object)(object)activeInteraction) && !isAnimating && ShowTimer <= 0f && ToolController.instance.ToolHide.Active && isStabbing)
		{
			StabPoint.SetActive(true);
			PaperPick component = ((Component)activeInteraction).GetComponent<PaperPick>();
			stabObject = ComponentHolderProtocol.GameObject((Object)(object)component.PaperInteraction);
			StabPoint.transform.position = component.PaperInteraction.PaperTransform.position;
			Vector3 val = StabPoint.transform.position - ((Component)this).transform.position;
			StabPoint.transform.rotation = Quaternion.LookRotation(val);
			GameDirector.instance.CameraShake.Shake(3f, 0.2f);
			StartAnimation();
			isStabbing = false;
		}
		((Component)this).transform.position = ((Component)ToolController.instance.ToolFollow).transform.position;
		((Component)this).transform.rotation = ((Component)ToolController.instance.ToolFollow).transform.rotation;
		((Component)this).transform.localScale = ((Component)ToolController.instance.ToolHide).transform.localScale;
	}

	private void AlignStabObjects()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < stabObjects.Count; i++)
		{
			Vector3 position = PickerPoint.position;
			Vector3 position2 = PickerPointEnd.position;
			if (isAnimating)
			{
				position = PickerPointAnimate.position;
				position2 = PickerPointEndAnimate.position;
			}
			float num = StabObjectSpacing * (float)i;
			float num2 = Vector3.Distance(position, position2);
			float num3 = num / num2;
			stabObjects[i].transform.position = Vector3.Lerp(position, position2, num3);
			stabObjects[i].transform.LookAt(((Component)this).transform.position);
			stabObjects[i].transform.Rotate(90f, 0f, 0f, (Space)1);
			stabObjects[i].transform.Rotate(0f, stabObjectsAngles[i], 0f, (Space)1);
		}
	}

	private void AnimatePicker()
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		if (animationProgress < 2f)
		{
			ToolController.instance.ForceActiveTimer = 0.1f;
			int num = 0;
			if (!introAnimation)
			{
				num = 1;
			}
			if (!introAnimation)
			{
				animationProgress += Time.deltaTime * AnimationSpeedOutro;
				float num2 = PickerStabOutro.Evaluate(animationProgress - (float)num);
				AnimatedPicker.transform.position = Vector3.LerpUnclamped(PositionStart, meshObject.transform.position, num2);
				AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(RotationStart, meshObject.transform.rotation, num2);
				AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(ScaleStart, meshObject.transform.localScale, num2);
			}
			else
			{
				animationProgress += Time.deltaTime * AnimationSpeedIntro;
				float num3 = PickerStabIntro.Evaluate(animationProgress - (float)num);
				AnimatedPicker.transform.position = Vector3.LerpUnclamped(PositionStart, StabPointChild.transform.position, num3);
				AnimatedPicker.transform.rotation = Quaternion.LerpUnclamped(RotationStart, StabPointChild.transform.rotation, num3);
				AnimatedPicker.transform.localScale = Vector3.LerpUnclamped(ScaleStart, StabPointChild.transform.localScale, num3);
			}
			if (animationProgress > 1f && !stab)
			{
				GameDirector.instance.CameraImpact.Shake(3f, 0f);
				PaperInteraction component = stabObject.GetComponent<PaperInteraction>();
				component.Picked = true;
				component.CleanEffect.Clean();
				((Component)component.CleanEffect).transform.parent = null;
				GameObject paperVisual = component.paperVisual;
				((Renderer)paperVisual.GetComponent<MeshRenderer>()).shadowCastingMode = (ShadowCastingMode)0;
				paperVisual.layer = LayerMask.NameToLayer("TopLayer");
				StabSound.Play(paperVisual.transform.position);
				paperVisual.transform.parent = ParentTransform;
				stabObjects.Add(paperVisual);
				stabObjectsAngles.Add(Random.Range(0, 360));
				introAnimation = false;
				stab = true;
				AnimationSet(introAnimation);
				GameDirector.instance.CameraShake.Shake(2f, 0.25f);
			}
		}
		else
		{
			EndAnimation();
		}
	}

	private void AnimationSet(bool intro)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (intro)
		{
			RotationStart = meshObject.transform.rotation;
			PositionStart = meshObject.transform.position;
			ScaleStart = meshObject.transform.localScale;
		}
		else
		{
			RotationStart = StabPointChild.transform.rotation;
			PositionStart = StabPointChild.transform.position;
			ScaleStart = StabPointChild.transform.localScale;
		}
		AnimatedPicker.transform.rotation = RotationStart;
	}

	public void StartAnimation()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		isAnimating = true;
		animationProgress = 0f;
		StabPoint.SetActive(true);
		AnimatedPicker.SetActive(true);
		AnimatedPicker.transform.position = ((Component)this).transform.position;
		AnimatedPicker.transform.rotation = ((Component)this).transform.rotation;
		AnimatedPicker.transform.localScale = ((Component)this).transform.localScale;
		((Renderer)meshRenderer).enabled = false;
		AnimationSet(introAnimation);
	}

	private void EndAnimation()
	{
		stab = false;
		introAnimation = true;
		isAnimating = false;
		StabPoint.SetActive(false);
		AnimatedPicker.SetActive(false);
		((Renderer)meshRenderer).enabled = true;
	}
}
