using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
	[Serializable]
	public class Tool
	{
		public string Name;

		public Interaction.InteractionType InteractionType;

		[Space]
		public GameObject Object;

		public GameObject ObjectParent;

		public GameObject playerAvatarPrefab;

		[Space]
		public Vector3 HidePosition;

		public Vector3 HideRotation;

		public float HideSpeed = 2f;

		[Space]
		public Vector3 OffsetPosition;

		public Vector3 OffsetRotation;

		[Space]
		public Sprite Icon;

		public bool HeadBob = true;

		public float Range = 1f;
	}

	public bool DebugAlwaysInteract;

	[HideInInspector]
	public static ToolController instance;

	[HideInInspector]
	public bool Active;

	private float ActiveTime = 0.25f;

	private float ActiveTimer;

	[HideInInspector]
	public bool Interact;

	private float InteractTimer;

	[Space]
	public float InteractionRange = 4f;

	public float InteractionCheckTime = 0.1f;

	private float InteractionCheckTimer;

	private float RangeCheckTimer;

	private bool RangeCheck = true;

	[HideInInspector]
	public float ForceActiveTimer;

	[HideInInspector]
	public Interaction.InteractionType ActiveInteractionType;

	[HideInInspector]
	public Interaction.InteractionType CurrentInteractionType;

	[HideInInspector]
	public Interaction ActiveInteraction;

	[HideInInspector]
	public Interaction CurrentInteraction;

	[HideInInspector]
	public Vector3 CurrentHidePosition;

	[HideInInspector]
	public Vector3 CurrentHideRotation;

	[HideInInspector]
	public float CurrentHideSpeed;

	[HideInInspector]
	public Sprite CurrentSprite;

	private float CurrentRange;

	private GameObject CurrentObject;

	[HideInInspector]
	public Interaction.InteractionType PreviousInteractionType;

	[Space]
	public ToolFollowPush ToolFollowPush;

	public ToolHide ToolHide;

	public Transform ToolFollow;

	public Transform ToolOffset;

	public ToolFollow ToolHeadbob;

	public Transform ToolTargetParent;

	public Transform FollowTargetTransform;

	private LayerMask Mask;

	private LayerMask VisibilityMask;

	public List<Tool> Tools;

	private Camera MainCamera;

	private bool InteractInput;

	private bool InteractInputDelayed;

	private bool DirtFinderInput;

	private float DisableTimer;

	public PlayerAvatar playerAvatarScript;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		MainCamera = Camera.main;
		Mask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Interaction" }));
		VisibilityMask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Default" }));
	}

	private void Update()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		UpdateInput();
		InteractionCheck();
		UpdateDirtFinder();
		UpdateActive();
		UpdateInteract();
		ToolFollow.position = Vector3.Lerp(ToolFollow.position, FollowTargetTransform.position, 20f * Time.deltaTime);
		ToolFollow.rotation = Quaternion.Lerp(ToolFollow.rotation, FollowTargetTransform.rotation, 20f * Time.deltaTime);
	}

	public void Disable(float time)
	{
		DisableTimer = time;
	}

	private void UpdateInput()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main || PlayerAvatar.instance.isDisabled)
		{
			return;
		}
		if (SemiFunc.InputDown(InputKey.Interact) || InteractInputDelayed)
		{
			if (ActiveInteractionType != 0 && CurrentInteractionType != 0 && CurrentInteractionType != ActiveInteractionType)
			{
				InteractInputDelayed = true;
				InteractInput = false;
			}
			else
			{
				InteractInput = true;
			}
		}
		else
		{
			InteractInput = false;
		}
		if (PlayerController.instance.CanInteract && (Input.GetButton("Dirt Finder") || Input.GetAxis("Dirt Finder") == 1f || GameDirector.instance.LevelCompleted))
		{
			DirtFinderInput = true;
		}
		else
		{
			DirtFinderInput = false;
		}
		if (DisableTimer > 0f)
		{
			DisableTimer -= 1f * Time.deltaTime;
			ActiveTimer = 0f;
			InteractInputDelayed = false;
			InteractInput = false;
			DirtFinderInput = false;
		}
	}

	private void InteractionCheck()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (InteractionCheckTimer <= 0f)
		{
			InteractionCheckTimer = InteractionCheckTime;
			CurrentInteractionType = Interaction.InteractionType.None;
			if (!PlayerController.instance.CanInteract)
			{
				return;
			}
			RaycastHit[] array = Physics.BoxCastAll(((Component)MainCamera).transform.position, new Vector3(0.01f, 0.01f, 0.01f), ((Component)MainCamera).transform.forward, ((Component)MainCamera).transform.rotation, InteractionRange, LayerMask.op_Implicit(Mask));
			if (array.Length == 0)
			{
				return;
			}
			RaycastHit val = default(RaycastHit);
			bool flag = Physics.Raycast(((Component)MainCamera).transform.position, ((Component)MainCamera).transform.forward, ref val, InteractionRange, LayerMask.op_Implicit(VisibilityMask));
			bool flag2 = false;
			Interaction hitPicked = null;
			float num = 360f;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit val2 = array2[i];
				if (flag && ((RaycastHit)(ref val2)).distance > ((RaycastHit)(ref val)).distance)
				{
					continue;
				}
				Interaction interaction = ((Component)((RaycastHit)(ref val2)).transform).GetComponent<Interaction>();
				if ((Object)(object)interaction == (Object)null)
				{
					continue;
				}
				float range = Tools.Find((Tool x) => x.InteractionType == interaction.Type).Range;
				if (((RaycastHit)(ref val2)).distance <= range)
				{
					float num2 = Quaternion.Angle(Quaternion.LookRotation(((RaycastHit)(ref val2)).transform.position - ((Component)MainCamera).transform.position), ((Component)MainCamera).transform.rotation);
					if (num2 < num)
					{
						num = num2;
						hitPicked = interaction;
						flag2 = true;
					}
				}
			}
			if (flag2)
			{
				CurrentInteraction = hitPicked;
				CurrentInteractionType = hitPicked.Type;
				if (CurrentInteractionType == ActiveInteractionType)
				{
					ActiveInteraction = CurrentInteraction;
				}
				CurrentSprite = Tools.Find((Tool x) => x.InteractionType == CurrentInteractionType).Icon;
				CurrentRange = Tools.Find((Tool x) => x.InteractionType == hitPicked.Type).Range;
			}
		}
		else
		{
			InteractionCheckTimer -= 1f * Time.deltaTime;
		}
	}

	private void UpdateDirtFinder()
	{
		if (GameDirector.instance.LevelCompletedDone || (DirtFinderInput && ForceActiveTimer <= 0f))
		{
			CurrentInteractionType = Interaction.InteractionType.DirtFinder;
			CurrentSprite = Tools.Find((Tool x) => x.InteractionType == CurrentInteractionType).Icon;
			if (ActiveInteractionType != Interaction.InteractionType.DirtFinder && ActiveInteractionType != 0)
			{
				DeactivateTool();
			}
			else if (!Active && (Object)(object)CurrentObject == (Object)null)
			{
				ActivateTool();
			}
			if (ActiveInteractionType == Interaction.InteractionType.DirtFinder)
			{
				ActiveTimer = ActiveTime;
			}
		}
	}

	private void UpdateActive()
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		if (ForceActiveTimer > 0f)
		{
			ForceActiveTimer -= 1f * Time.deltaTime;
			ForceActiveTimer = Mathf.Max(ForceActiveTimer, 0f);
		}
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (ActiveInteractionType != Interaction.InteractionType.DirtFinder && (CurrentInteractionType != 0 || ActiveInteractionType != 0) && (InteractInput || ForceActiveTimer > 0f))
			{
				ActiveTimer = ActiveTime;
				if (!Active && (Object)(object)CurrentObject == (Object)null)
				{
					ActivateTool();
				}
			}
			if (Active && ActiveTimer <= 0f)
			{
				DeactivateTool();
			}
			if (Active)
			{
				if ((Object)(object)CurrentInteraction == (Object)null || ActiveInteractionType == Interaction.InteractionType.DirtFinder)
				{
					ActiveTimer -= 1f * Time.deltaTime;
				}
				else if (!Interact)
				{
					if (RangeCheckTimer <= 0f)
					{
						RangeCheck = false;
						Vector3 val = ((Component)CurrentInteraction).transform.position - ((Component)MainCamera).transform.position;
						RaycastHit[] array = Physics.BoxCastAll(((Component)MainCamera).transform.position, new Vector3(0.01f, 0.01f, 0.01f), val, Quaternion.identity, InteractionRange, LayerMask.op_Implicit(Mask));
						if (array.Length != 0)
						{
							RaycastHit[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								RaycastHit val2 = array2[i];
								if ((Object)(object)((Component)((RaycastHit)(ref val2)).transform).GetComponent<Interaction>() == (Object)(object)ActiveInteraction && ((RaycastHit)(ref val2)).distance <= CurrentRange)
								{
									RangeCheck = true;
									break;
								}
							}
						}
						RangeCheckTimer = 0.2f;
					}
					else
					{
						RangeCheckTimer -= 1f * Time.deltaTime;
					}
					if (!RangeCheck)
					{
						ActiveTimer -= 1f * Time.deltaTime;
					}
				}
			}
		}
		if (ActiveInteractionType != 0)
		{
			PlayerController.instance.CrouchDisable(0.5f);
		}
	}

	private void UpdateInteract()
	{
		if (ActiveInteractionType == Interaction.InteractionType.DirtFinder)
		{
			Interact = false;
			return;
		}
		if (CurrentInteractionType == ActiveInteractionType && (InteractInput || DebugAlwaysInteract))
		{
			InteractTimer = 0.25f;
			InteractInputDelayed = false;
		}
		if (InteractTimer > 0f)
		{
			Interact = true;
			InteractTimer -= 1f * Time.deltaTime;
			if (InteractTimer <= 0f)
			{
				Interact = false;
			}
		}
	}

	private void ActivateTool()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		ActiveInteractionType = CurrentInteractionType;
		ActiveInteraction = CurrentInteraction;
		Active = true;
		foreach (Tool tool in Tools)
		{
			if (tool.InteractionType == CurrentInteractionType)
			{
				((Component)ToolOffset).transform.localPosition = tool.OffsetPosition;
				((Component)ToolOffset).transform.localRotation = Quaternion.Euler(tool.OffsetRotation);
				if (tool.HeadBob)
				{
					ToolHeadbob.Activate();
				}
				else
				{
					ToolHeadbob.Deactivate();
				}
				CurrentHidePosition = tool.HidePosition;
				CurrentHideRotation = tool.HideRotation;
				CurrentHideSpeed = tool.HideSpeed;
				break;
			}
		}
		ToolHide.Show();
	}

	public void ShowTool()
	{
		foreach (Tool tool in Tools)
		{
			if (tool.InteractionType == ActiveInteractionType)
			{
				CurrentObject = Object.Instantiate<GameObject>(tool.Object, tool.ObjectParent.transform);
				break;
			}
		}
	}

	private void DeactivateTool()
	{
		Active = false;
		ActiveInteractionType = Interaction.InteractionType.None;
		ActiveInteraction = null;
		ToolHide.Hide();
	}

	public void HideTool()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((Component)ToolOffset).transform.localPosition = Vector3.zero;
		((Component)ToolOffset).transform.localRotation = Quaternion.identity;
		Object.Destroy((Object)(object)CurrentObject);
	}
}
