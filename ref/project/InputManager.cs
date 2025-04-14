using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputManager : MonoBehaviour
{
	public static InputManager instance;

	private Dictionary<InputKey, InputAction> inputActions;

	private Dictionary<InputKey, bool> inputToggle;

	internal Dictionary<InputPercentSetting, int> inputPercentSettings;

	private Dictionary<MovementDirection, int> movementBindingIndices;

	private Dictionary<InputKey, List<string>> defaultBindingPaths;

	private Dictionary<InputKey, bool> defaultInputToggleStates;

	private Dictionary<InputPercentSetting, int> defaultInputPercentSettings;

	internal float disableMovementTimer;

	internal float disableAimingTimer;

	internal float mouseSensitivity = 0.1f;

	private Dictionary<string, InputKey> tagDictionary = new Dictionary<string, InputKey>();

	private void Awake()
	{
		if ((Object)(object)instance == (Object)null)
		{
			instance = this;
			Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			InitializeInputs();
			StoreDefaultBindings();
		}
		else
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	private void Start()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		InputSystem.settings.backgroundBehavior = (BackgroundBehavior)1;
		tagDictionary.Add("[move]", InputKey.Movement);
		tagDictionary.Add("[jump]", InputKey.Jump);
		tagDictionary.Add("[grab]", InputKey.Grab);
		tagDictionary.Add("[grab2]", InputKey.Rotate);
		tagDictionary.Add("[sprint]", InputKey.Sprint);
		tagDictionary.Add("[crouch]", InputKey.Crouch);
		tagDictionary.Add("[map]", InputKey.Map);
		tagDictionary.Add("[inventory1]", InputKey.Inventory1);
		tagDictionary.Add("[inventory2]", InputKey.Inventory2);
		tagDictionary.Add("[inventory3]", InputKey.Inventory3);
		tagDictionary.Add("[tumble]", InputKey.Tumble);
		tagDictionary.Add("[interact]", InputKey.Interact);
		tagDictionary.Add("[push]", InputKey.Push);
		tagDictionary.Add("[pull]", InputKey.Pull);
		tagDictionary.Add("[chat]", InputKey.Chat);
		ES3.DeleteFile("DefaultKeyBindings.es3");
		if (!ES3.FileExists(new ES3Settings("DefaultKeyBindings.es3", new Enum[1] { (Enum)(object)(Location)0 })))
		{
			SaveDefaultKeyBindings();
		}
		if (!ES3.FileExists(new ES3Settings("CurrentKeyBindings.es3", new Enum[1] { (Enum)(object)(Location)0 })))
		{
			SaveCurrentKeyBindings();
		}
		LoadKeyBindings("CurrentKeyBindings.es3");
	}

	private void FixedUpdate()
	{
		float num = Mathf.Min(Time.fixedDeltaTime, 0.05f);
		if (disableMovementTimer > 0f)
		{
			disableMovementTimer -= num;
		}
		if (disableAimingTimer > 0f)
		{
			disableAimingTimer -= num;
		}
	}

	private void InitializeInputs()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Expected O, but got Unknown
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Expected O, but got Unknown
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Expected O, but got Unknown
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Expected O, but got Unknown
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Expected O, but got Unknown
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Expected O, but got Unknown
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Expected O, but got Unknown
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Expected O, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Expected O, but got Unknown
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Expected O, but got Unknown
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Expected O, but got Unknown
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Expected O, but got Unknown
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Expected O, but got Unknown
		inputActions = new Dictionary<InputKey, InputAction>();
		movementBindingIndices = new Dictionary<MovementDirection, int>();
		inputToggle = new Dictionary<InputKey, bool>();
		InputAction val = new InputAction("Movement", (InputActionType)0, (string)null, (string)null, (string)null, (string)null);
		CompositeSyntax val2 = InputActionSetupExtensions.AddCompositeBinding(val, "2DVector", (string)null, (string)null);
		((CompositeSyntax)(ref val2)).With("Up", "<Keyboard>/w", (string)null, (string)null);
		((CompositeSyntax)(ref val2)).With("Down", "<Keyboard>/s", (string)null, (string)null);
		((CompositeSyntax)(ref val2)).With("Left", "<Keyboard>/a", (string)null, (string)null);
		((CompositeSyntax)(ref val2)).With("Right", "<Keyboard>/d", (string)null, (string)null);
		inputActions[InputKey.Movement] = val;
		ReadOnlyArray<InputBinding> bindings = val.bindings;
		for (int i = 0; i < bindings.Count; i++)
		{
			InputBinding val3 = bindings[i];
			if (((InputBinding)(ref val3)).isPartOfComposite)
			{
				switch (((InputBinding)(ref val3)).name.ToLower())
				{
				case "up":
					movementBindingIndices[MovementDirection.Up] = i;
					break;
				case "down":
					movementBindingIndices[MovementDirection.Down] = i;
					break;
				case "left":
					movementBindingIndices[MovementDirection.Left] = i;
					break;
				case "right":
					movementBindingIndices[MovementDirection.Right] = i;
					break;
				}
			}
		}
		InputAction val4 = new InputAction("Scroll", (InputActionType)0, (string)null, (string)null, (string)null, (string)null);
		InputActionSetupExtensions.AddBinding(val4, "<Mouse>/scroll/y", (string)null, (string)null, (string)null);
		inputActions[InputKey.Scroll] = val4;
		InputAction value = new InputAction("Jump", (InputActionType)0, "<Keyboard>/space", (string)null, (string)null, (string)null);
		inputActions[InputKey.Jump] = value;
		value = new InputAction("Use", (InputActionType)0, "<Keyboard>/e", (string)null, (string)null, (string)null);
		inputActions[InputKey.Interact] = value;
		value = new InputAction("MouseInput", (InputActionType)0, "<Pointer>/position", (string)null, (string)null, (string)null);
		inputActions[InputKey.MouseInput] = value;
		InputAction val5 = new InputAction("Push", (InputActionType)0, (string)null, (string)null, (string)null, (string)null);
		InputActionSetupExtensions.AddBinding(val5, "<Mouse>/scroll/y", (string)null, (string)null, (string)null);
		inputActions[InputKey.Push] = val5;
		InputAction val6 = new InputAction("Pull", (InputActionType)0, (string)null, (string)null, (string)null, (string)null);
		InputActionSetupExtensions.AddBinding(val6, "<Mouse>/scroll/y", (string)null, (string)null, (string)null);
		inputActions[InputKey.Pull] = val6;
		value = new InputAction("Menu", (InputActionType)0, "<Keyboard>/escape", (string)null, (string)null, (string)null);
		inputActions[InputKey.Menu] = value;
		value = new InputAction("Back", (InputActionType)0, "<Keyboard>/escape", (string)null, (string)null, (string)null);
		inputActions[InputKey.Back] = value;
		value = new InputAction("BackEditor", (InputActionType)0, "<Keyboard>/F1", (string)null, (string)null, (string)null);
		inputActions[InputKey.BackEditor] = value;
		value = new InputAction("Chat", (InputActionType)0, "<Keyboard>/t", (string)null, (string)null, (string)null);
		inputActions[InputKey.Chat] = value;
		value = new InputAction("Map", (InputActionType)0, "<Keyboard>/tab", (string)null, (string)null, (string)null);
		inputActions[InputKey.Map] = value;
		value = new InputAction("Confirm", (InputActionType)0, "<Keyboard>/enter", (string)null, (string)null, (string)null);
		inputActions[InputKey.Confirm] = value;
		value = new InputAction("Grab", (InputActionType)0, "<Mouse>/leftButton", (string)null, (string)null, (string)null);
		inputActions[InputKey.Grab] = value;
		value = new InputAction("Rotate", (InputActionType)0, "<Mouse>/rightButton", (string)null, (string)null, (string)null);
		inputActions[InputKey.Rotate] = value;
		value = new InputAction("Crouch", (InputActionType)0, "<Keyboard>/ctrl", (string)null, (string)null, (string)null);
		inputActions[InputKey.Crouch] = value;
		value = new InputAction("Chat Delete", (InputActionType)0, "<Keyboard>/backspace", (string)null, (string)null, (string)null);
		inputActions[InputKey.ChatDelete] = value;
		value = new InputAction("Tumble", (InputActionType)0, "<Keyboard>/q", (string)null, (string)null, (string)null);
		inputActions[InputKey.Tumble] = value;
		value = new InputAction("Sprint", (InputActionType)0, "<Keyboard>/leftShift", (string)null, (string)null, (string)null);
		inputActions[InputKey.Sprint] = value;
		value = new InputAction("MouseDelta", (InputActionType)0, "<Pointer>/delta", (string)null, (string)null, (string)null);
		inputActions[InputKey.MouseDelta] = value;
		value = new InputAction("Inventory1", (InputActionType)0, "<Keyboard>/1", (string)null, (string)null, (string)null);
		inputActions[InputKey.Inventory1] = value;
		value = new InputAction("Inventory2", (InputActionType)0, "<Keyboard>/2", (string)null, (string)null, (string)null);
		inputActions[InputKey.Inventory2] = value;
		value = new InputAction("Inventory3", (InputActionType)0, "<Keyboard>/3", (string)null, (string)null, (string)null);
		inputActions[InputKey.Inventory3] = value;
		value = new InputAction("SpectateNext", (InputActionType)0, "<Mouse>/rightButton", (string)null, (string)null, (string)null);
		inputActions[InputKey.SpectateNext] = value;
		value = new InputAction("SpectatePrevious", (InputActionType)0, "<Mouse>/leftButton", (string)null, (string)null, (string)null);
		inputActions[InputKey.SpectatePrevious] = value;
		value = new InputAction("PushToTalk", (InputActionType)0, "<Keyboard>/v", (string)null, (string)null, (string)null);
		inputActions[InputKey.PushToTalk] = value;
		inputToggle.Add(InputKey.Sprint, value: false);
		inputToggle.Add(InputKey.Crouch, value: false);
		inputToggle.Add(InputKey.Map, value: false);
		inputToggle.Add(InputKey.Grab, value: false);
		inputPercentSettings = new Dictionary<InputPercentSetting, int>();
		inputPercentSettings[InputPercentSetting.MouseSensitivity] = 50;
		foreach (InputAction value2 in inputActions.Values)
		{
			value2.Enable();
		}
	}

	private void StoreDefaultBindings()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		defaultBindingPaths = new Dictionary<InputKey, List<string>>();
		defaultInputToggleStates = new Dictionary<InputKey, bool>();
		defaultInputPercentSettings = new Dictionary<InputPercentSetting, int>();
		foreach (InputKey key in inputActions.Keys)
		{
			InputAction obj = inputActions[key];
			List<string> list = new List<string>();
			Enumerator<InputBinding> enumerator2 = obj.bindings.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InputBinding current2 = enumerator2.Current;
					list.Add(((InputBinding)(ref current2)).path);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
			}
			defaultBindingPaths[key] = list;
		}
		foreach (KeyValuePair<InputKey, bool> item in inputToggle)
		{
			defaultInputToggleStates[item.Key] = item.Value;
		}
		foreach (KeyValuePair<InputPercentSetting, int> inputPercentSetting in inputPercentSettings)
		{
			defaultInputPercentSettings[inputPercentSetting.Key] = inputPercentSetting.Value;
		}
	}

	public void SaveDefaultKeyBindings()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = defaultBindingPaths;
		keyBindingSaveData.inputToggleStates = defaultInputToggleStates;
		keyBindingSaveData.inputPercentSettings = defaultInputPercentSettings;
		ES3Settings val = new ES3Settings("DefaultKeyBindings.es3", new Enum[1] { (Enum)(object)(Location)0 });
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, val);
	}

	public void SaveCurrentKeyBindings()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = new Dictionary<InputKey, List<string>>();
		keyBindingSaveData.inputToggleStates = new Dictionary<InputKey, bool>(inputToggle);
		keyBindingSaveData.inputPercentSettings = new Dictionary<InputPercentSetting, int>(inputPercentSettings);
		foreach (InputKey key in inputActions.Keys)
		{
			InputAction obj = inputActions[key];
			List<string> list = new List<string>();
			Enumerator<InputBinding> enumerator2 = obj.bindings.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					InputBinding current2 = enumerator2.Current;
					list.Add(string.IsNullOrEmpty(((InputBinding)(ref current2)).overridePath) ? ((InputBinding)(ref current2)).path : ((InputBinding)(ref current2)).overridePath);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
			}
			keyBindingSaveData.bindingOverrides[key] = list;
		}
		ES3Settings val = new ES3Settings("CurrentKeyBindings.es3", new Enum[1] { (Enum)(object)(Location)0 });
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, val);
	}

	public void LoadKeyBindings(string filename)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		try
		{
			ES3Settings val = new ES3Settings(filename, new Enum[1] { (Enum)(object)(Location)0 });
			if (ES3.FileExists(val))
			{
				KeyBindingSaveData saveData = ES3.Load<KeyBindingSaveData>("KeyBindings", val);
				ApplyLoadedKeyBindings(saveData);
			}
			else
			{
				Debug.LogWarning((object)("Keybindings file not found: " + filename));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Failed to load keybindings: " + ex.Message));
		}
	}

	private void ApplyLoadedKeyBindings(KeyBindingSaveData saveData)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		foreach (InputKey key in saveData.bindingOverrides.Keys)
		{
			if (!inputActions.TryGetValue(key, out var value))
			{
				continue;
			}
			List<string> list = saveData.bindingOverrides[key];
			value.Disable();
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				if (!string.IsNullOrEmpty(text) && value.bindings.Count > i)
				{
					InputActionRebindingExtensions.ApplyBindingOverride(value, i, text);
				}
			}
			value.Enable();
		}
		if (saveData.inputToggleStates != null)
		{
			foreach (KeyValuePair<InputKey, bool> inputToggleState in saveData.inputToggleStates)
			{
				inputToggle[inputToggleState.Key] = inputToggleState.Value;
			}
		}
		if (saveData.inputPercentSettings == null)
		{
			return;
		}
		foreach (KeyValuePair<InputPercentSetting, int> inputPercentSetting in saveData.inputPercentSettings)
		{
			inputPercentSettings[inputPercentSetting.Key] = inputPercentSetting.Value;
		}
	}

	public void ResetKeyToDefault(InputKey key)
	{
		if (inputActions.TryGetValue(key, out var value))
		{
			value.Disable();
			List<string> list = defaultBindingPaths[key];
			for (int i = 0; i < list.Count; i++)
			{
				InputActionRebindingExtensions.ApplyBindingOverride(value, i, list[i]);
			}
			value.Enable();
			if (defaultInputToggleStates.ContainsKey(key))
			{
				inputToggle[key] = defaultInputToggleStates[key];
			}
			if (defaultInputPercentSettings.ContainsKey((InputPercentSetting)key))
			{
				inputPercentSettings[(InputPercentSetting)key] = defaultInputPercentSettings[(InputPercentSetting)key];
			}
		}
		else
		{
			Debug.LogWarning((object)("InputKey not found: " + key));
		}
	}

	public bool KeyDown(InputKey key)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble || key == InputKey.Inventory1 || key == InputKey.Inventory2 || key == InputKey.Inventory3 || key == InputKey.Interact) && disableMovementTimer > 0f)
		{
			return false;
		}
		if (inputActions.TryGetValue(key, out var value))
		{
			return key switch
			{
				InputKey.Push => value.ReadValue<Vector2>().y > 0f, 
				InputKey.Pull => value.ReadValue<Vector2>().y < 0f, 
				_ => value.WasPressedThisFrame(), 
			};
		}
		return false;
	}

	public bool KeyUp(InputKey key)
	{
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble) && disableMovementTimer > 0f)
		{
			return false;
		}
		if (inputActions.TryGetValue(key, out var value))
		{
			if (key == InputKey.Push || key == InputKey.Pull)
			{
				return false;
			}
			return value.WasReleasedThisFrame();
		}
		return false;
	}

	public float KeyPullAndPush()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		InputBinding val;
		if (inputActions.TryGetValue(InputKey.Push, out var value))
		{
			val = value.bindings[0];
			if (((InputBinding)(ref val)).effectivePath.EndsWith("scroll/y"))
			{
				if (value.ReadValue<float>() > 0f)
				{
					return value.ReadValue<float>();
				}
			}
			else if (value.IsPressed())
			{
				return 1f;
			}
		}
		if (inputActions.TryGetValue(InputKey.Pull, out var value2))
		{
			val = value2.bindings[0];
			if (((InputBinding)(ref val)).effectivePath.EndsWith("scroll/y"))
			{
				if (value2.ReadValue<float>() < 0f)
				{
					return value2.ReadValue<float>();
				}
			}
			else if (value2.IsPressed())
			{
				return -1f;
			}
		}
		return 0f;
	}

	public InputAction GetAction(InputKey key)
	{
		if (inputActions.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public InputAction GetMovementAction()
	{
		if (inputActions.TryGetValue(InputKey.Movement, out var value))
		{
			return value;
		}
		return null;
	}

	public int GetMovementBindingIndex(MovementDirection direction)
	{
		if (movementBindingIndices.TryGetValue(direction, out var value))
		{
			return value;
		}
		return -1;
	}

	public bool KeyHold(InputKey key)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble) && disableMovementTimer > 0f)
		{
			return false;
		}
		if (inputActions.TryGetValue(key, out var value))
		{
			return key switch
			{
				InputKey.Push => value.ReadValue<Vector2>().y > 0f, 
				InputKey.Pull => value.ReadValue<Vector2>().y < 0f, 
				_ => value.IsPressed(), 
			};
		}
		return false;
	}

	public float GetMovementX()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (disableMovementTimer > 0f)
		{
			return 0f;
		}
		if (inputActions.TryGetValue(InputKey.Movement, out var value))
		{
			return value.ReadValue<Vector2>().x;
		}
		return 0f;
	}

	public float GetScrollY()
	{
		if (inputActions.TryGetValue(InputKey.Scroll, out var value))
		{
			return value.ReadValue<float>();
		}
		return 0f;
	}

	public float GetMovementY()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (disableMovementTimer > 0f)
		{
			return 0f;
		}
		if (inputActions.TryGetValue(InputKey.Movement, out var value))
		{
			return value.ReadValue<Vector2>().y;
		}
		return 0f;
	}

	public Vector2 GetMovement()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (disableMovementTimer > 0f)
		{
			return Vector2.zero;
		}
		if (inputActions.TryGetValue(InputKey.Movement, out var value))
		{
			return value.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	public float GetMouseX()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (disableAimingTimer > 0f)
		{
			return 0f;
		}
		if (inputActions.TryGetValue(InputKey.MouseDelta, out var value))
		{
			return value.ReadValue<Vector2>().x * mouseSensitivity;
		}
		return 0f;
	}

	public float GetMouseY()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (disableAimingTimer > 0f)
		{
			return 0f;
		}
		if (inputActions.TryGetValue(InputKey.MouseDelta, out var value))
		{
			return value.ReadValue<Vector2>().y * mouseSensitivity;
		}
		return 0f;
	}

	public Vector2 GetMousePosition()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (disableAimingTimer > 0f)
		{
			return Vector2.zero;
		}
		if (inputActions.TryGetValue(InputKey.MouseInput, out var value))
		{
			return value.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	public void Rebind(InputKey key, string newBinding)
	{
		if (inputActions.TryGetValue(key, out var value))
		{
			InputActionRebindingExtensions.ApplyBindingOverride(value, newBinding, (string)null, (string)null);
		}
	}

	public void RebindMovementKey(MovementDirection direction, string newBinding)
	{
		if (inputActions.TryGetValue(InputKey.Movement, out var value))
		{
			if (movementBindingIndices.TryGetValue(direction, out var value2))
			{
				InputActionRebindingExtensions.ApplyBindingOverride(value, value2, newBinding);
			}
			else
			{
				Debug.LogWarning((object)$"Binding index for {direction} not found.");
			}
		}
	}

	public void DisableMovement()
	{
		disableMovementTimer = 0.1f;
	}

	public void DisableAiming()
	{
		disableAimingTimer = 0.1f;
	}

	public void InputToggleRebind(InputKey key, bool toggle)
	{
		inputToggle[key] = toggle;
	}

	public bool InputToggleGet(InputKey key)
	{
		return inputToggle[key];
	}

	public string GetKeyString(InputKey key)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		InputAction action = GetAction(key);
		if (action == null)
		{
			return null;
		}
		InputBinding val = action.bindings[0];
		return ((InputBinding)(ref val)).effectivePath;
	}

	public string GetMovementKeyString(MovementDirection direction)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		InputAction movementAction = GetMovementAction();
		int movementBindingIndex = GetMovementBindingIndex(direction);
		if (movementAction == null)
		{
			return null;
		}
		InputBinding val = movementAction.bindings[movementBindingIndex];
		return ((InputBinding)(ref val)).effectivePath;
	}

	public string InputDisplayGet(InputKey _inputKey, MenuKeybind.KeyType _keyType, MovementDirection _movementDirection)
	{
		switch (_keyType)
		{
		case MenuKeybind.KeyType.InputKey:
		{
			InputAction action = GetAction(_inputKey);
			if (action != null)
			{
				int bindingIndex = 0;
				return InputDisplayGetString(action, bindingIndex);
			}
			break;
		}
		case MenuKeybind.KeyType.MovementKey:
		{
			InputAction movementAction = GetMovementAction();
			int movementBindingIndex = GetMovementBindingIndex(_movementDirection);
			if (movementAction != null && movementBindingIndex >= 0)
			{
				return InputDisplayGetString(movementAction, movementBindingIndex);
			}
			break;
		}
		}
		return "Unassigned";
	}

	public string InputDisplayGetString(InputAction action, int bindingIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		InputBinding val = action.bindings[bindingIndex];
		string text = InputControlPath.ToHumanReadableString(((InputBinding)(ref val)).effectivePath, (HumanReadableStringOptions)2, (InputControl)null);
		bool flag = false;
		if (text.EndsWith("Scroll/Y"))
		{
			text = "Mouse Scroll";
			flag = true;
		}
		if (((InputBinding)(ref val)).effectivePath.Contains("Mouse") && !flag)
		{
			text = InputDisplayMouseStringGet(((InputBinding)(ref val)).effectivePath);
		}
		return text.ToUpper();
	}

	private string InputDisplayMouseStringGet(string path)
	{
		if (path.Contains("leftButton"))
		{
			return "Mouse Left";
		}
		if (path.Contains("rightButton"))
		{
			return "Mouse Right";
		}
		if (path.Contains("middleButton"))
		{
			return "Mouse Middle";
		}
		if (path.Contains("press"))
		{
			return "Mouse Press";
		}
		if (path.Contains("backButton"))
		{
			return "Mouse Back";
		}
		if (path.Contains("forwardButton"))
		{
			return "Mouse Forward";
		}
		if (path.Contains("button"))
		{
			int num = path.IndexOf("button");
			string text = path.Substring(num + "button".Length);
			return "Mouse " + text;
		}
		return "Mouse Button";
	}

	public string InputDisplayReplaceTags(string _text)
	{
		foreach (KeyValuePair<string, InputKey> item in tagDictionary)
		{
			string text = "";
			if (item.Value == InputKey.Movement)
			{
				text = InputDisplayGet(item.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Up);
				text += InputDisplayGet(item.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Left);
				text += InputDisplayGet(item.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Down);
				text += InputDisplayGet(item.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Right);
			}
			else
			{
				text = InputDisplayGet(item.Value, MenuKeybind.KeyType.InputKey, MovementDirection.Up);
			}
			_text = _text.Replace(item.Key, "<u><b>" + text + "</b></u>");
		}
		return _text;
	}

	public void ResetInput()
	{
		InputSystem.ResetHaptics();
		InputSystem.ResetDevice((InputDevice)(object)Keyboard.current, false);
		foreach (KeyValuePair<InputKey, InputAction> inputAction in inputActions)
		{
			inputAction.Value.Reset();
		}
	}
}
