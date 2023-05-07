using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace uToolKit.Runtime {
	[Serializable]
	public class LoggerOptions {
		public float Height;
		public bool IsBoundTop;
		public int MaxEntries;
		//public bool CollapseSame;
	}
	public class LogConsole : MonoBehaviour {
#if ENABLE_UTOOLKIT_CONSOLE
		#region Public/Private Variables
		public static LogConsole Instance;
		public static Action<string> OnCommandSubmit;
		[SerializeField] private GameObject loggerMenu;
		[SerializeField] private ScrollRect scrollRect;
		[SerializeField] private Transform logEntryContainer;
		[SerializeField] private TMP_InputField commandInputField;
		[SerializeField] private Color logTypeColor;
		[SerializeField] private Color warningTypeColor;
		[SerializeField] private Color errorTypeColor;
		[SerializeField] private Button submitCommandButton;
		[SerializeField] private Button clearLogButton;
		[SerializeField] private Button expandButton;
		#endregion

		#region Runtime Variables
		[Header("Runtime Debug")]
		[SerializeField] private LoggerOptions options;
		[SerializeField] private bool didExpand;
		[SerializeField] private bool didCycle;
		[SerializeField] private int logEntries;
		#endregion

		#region Native Methods
		private void Awake() {
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
			SetOptions(new LoggerOptions() {
				Height = 650f,
				IsBoundTop = false,
				MaxEntries = 150
			});
			CloseMenu();
			ClearLogger();
			if (FindObjectOfType<EventSystem>() == null) {
				UnityEngine.Debug.LogWarning("No EventSystem found in scene.");
			}
		}

		private void OnEnable() {
			Application.logMessageReceived += LogMessageReceived;
			commandInputField.onSubmit.AddListener(delegate (string command) { CommandInputFieldSubmit(command); });
			submitCommandButton.onClick.AddListener(delegate { SubmitCommandButtonClick(); });
			clearLogButton.onClick.AddListener(delegate { ClearLogButtonClick(); });
			expandButton.onClick.AddListener(delegate { ExpandButtonClick(); });
			OnCommandSubmit += delegate (string command) { ParseLocalCommand(command); };
		}

		private void Start() {
			//PushSysInfo();
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				CycleMenu();
			}
			if (!loggerMenu.activeSelf) { return; }
			if (commandInputField.isFocused && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
				SubmitCommand();
			}
		}

		private void OnDisable() {
			Application.logMessageReceived -= LogMessageReceived;
			commandInputField.onSubmit.RemoveListener(delegate (string command) { CommandInputFieldSubmit(command); });
			submitCommandButton.onClick.RemoveListener(delegate { SubmitCommandButtonClick(); });
			clearLogButton.onClick.RemoveListener(delegate { ClearLogButtonClick(); });
			expandButton.onClick.RemoveListener(delegate { ExpandButtonClick(); });
			OnCommandSubmit -= delegate (string command) { ParseLocalCommand(command); };
		}
		#endregion

		#region Callback Methods
		private void LogMessageReceived(string condition, string stackTrace, LogType type) {
			string _message = (string.IsNullOrEmpty(condition) || new[] { "Null", "null" }.Contains(condition) ? string.Empty : condition);
			if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) {
				_message += $"\n\nStack Trace:\n{stackTrace}\n-  -  -  -  -  -  -  -  -  -  -  -  -  -  -";
			}
			StackTrace _callTrace = new StackTrace();
			int _frameIndex = _callTrace.GetFrames().Length - 1;
			CreateLogEntry(type).text += $"{_callTrace.GetFrame(_frameIndex).GetMethod().DeclaringType.Name} | {_callTrace.GetFrame(_frameIndex).GetMethod().Name}{(string.IsNullOrEmpty(_message) ? string.Empty : $" | {_message}")}";
		}

		private void CommandInputFieldSubmit(string command) {
			SubmitCommand();
		}

		private void SubmitCommandButtonClick() {
			SubmitCommand();
		}

		private void ClearLogButtonClick() {
			ClearLogger();
		}

		private void ExpandButtonClick() {
			didExpand = true;
			SetRect(loggerMenu.GetComponent<RectTransform>(), 0, 0, 0, 0);
		}

		private void ParseLocalCommand(string command) {
			if (command == "close") {
				CloseMenu();
			} else if (command == "clear") {
				ClearLogger();
			} else if (command == "cycle") {
				CycleMenu();
			} else if (command.Contains("options.height") && command.Contains("=") && command.Split('=').Length == 2 && !string.IsNullOrEmpty(command.Split('=')[1])) {
				if (float.TryParse(command.Split('=')[1], out float height)) {
					SetOptions(new LoggerOptions() {
						Height = height,
						IsBoundTop = options.IsBoundTop,
						MaxEntries = options.MaxEntries
					});
				}
			} else if (command.Contains("options.isboundtop") && command.Contains("=") && command.Split('=').Length == 2 && !string.IsNullOrEmpty(command.Split('=')[1])) {
				if (bool.TryParse(command.Split('=')[1], out bool isBoundTop)) {
					SetOptions(new LoggerOptions() {
						Height = options.Height,
						IsBoundTop = isBoundTop,
						MaxEntries = options.MaxEntries
					});
				}
			} else if (command.Contains("options.maxentries") && command.Contains("=") && command.Split('=').Length == 2 && !string.IsNullOrEmpty(command.Split('=')[1])) {
				if (int.TryParse(command.Split('=')[1], out int maxEntries)) {
					SetOptions(new LoggerOptions() {
						Height = options.Height,
						IsBoundTop = options.IsBoundTop,
						MaxEntries = maxEntries
					});
				}
			} else if (command == "sysinfo") {
				PushSysInfo();
			}
		}
		#endregion

		#region Static Methods
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateCanvas() {
			LogConsole _loggerCanvas = UnityEngine.Object.Instantiate(Resources.Load<LogConsole>(typeof(LogConsole).Name));
			_loggerCanvas.name = typeof(LogConsole).Name;
			_loggerCanvas.GetComponent<Canvas>().sortingOrder = 5000;
		}
		#endregion

		#region Public Methods
		public void CycleMenu() {
			EventSystem.current.SetSelectedGameObject(null);
			if (loggerMenu.activeSelf) {
				if (didExpand) {
					CloseMenu();
					return;
				}
				if (didCycle) {
					CloseMenu();
					return;
				}
				if (!string.IsNullOrEmpty(commandInputField.text) && commandInputField.text[commandInputField.text.Length - 1] == '`') {
					commandInputField.text = commandInputField.text.Remove(commandInputField.text.Length - 1);
				}
				didCycle = true;
				SetRect(loggerMenu.GetComponent<RectTransform>(), 0, options.IsBoundTop ? options.Height : 0, 0, options.IsBoundTop ? 0 : options.Height);
			} else {
				OpenMenu();
			}
		}

		public void OpenMenu() {
			ResetWindow();
			SetRect(loggerMenu.GetComponent<RectTransform>(), 0, options.IsBoundTop ? 0 : options.Height, 0, options.IsBoundTop ? options.Height : 0);
			loggerMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(commandInputField.gameObject);
		}

		public void CloseMenu() {
			ResetWindow();
			loggerMenu.SetActive(false);
		}

		public void ClearLogger() {
			foreach (Transform child in logEntryContainer) {
				if (child.GetSiblingIndex() == 0) { continue; }
				Destroy(child.gameObject);
			}
			logEntries = 0;
			commandInputField.text = string.Empty;
		}

		public void SetOptions(LoggerOptions value) {
			options = value;
			SetRect(loggerMenu.GetComponent<RectTransform>(), 0, options.IsBoundTop ? options.Height : 0, 0, options.IsBoundTop ? 0 : options.Height);
		}
		#endregion

		#region Private Methods
		private TMP_Text CreateLogEntry(LogType type) {
			TMP_Text _logEntry = Instantiate(logEntryContainer.GetChild(0).GetComponent<TMP_Text>(), logEntryContainer);
			_logEntry.gameObject.SetActive(true);
			_logEntry.name = $"Log Entry {logEntries}";
			logEntries++;
			switch (type) {
				case LogType.Error:
					_logEntry.color = errorTypeColor * 2;
					break;
				case LogType.Assert:
					_logEntry.color = errorTypeColor;
					break;
				case LogType.Exception:
					_logEntry.color = errorTypeColor;
					break;
				case LogType.Warning:
					_logEntry.color = warningTypeColor;
					break;
				case LogType.Log:
					_logEntry.color = logTypeColor;
					break;
			}
			_logEntry.text = $"[{logEntries}] [{DateTime.Now.ToString("G")}] :: ";
			if (logEntries > options.MaxEntries) {
				Destroy(logEntryContainer.GetChild(1).gameObject);
			}
			return _logEntry;
		}

		private void SetRect(RectTransform trs, float left, float top, float right, float bottom) {
			trs.offsetMin = new Vector2(left, bottom);
			trs.offsetMax = new Vector2(-right, -top);
		}

		private void SubmitCommand() {
			if (string.IsNullOrEmpty(commandInputField.text)) { return; }
			CreateLogEntry(LogType.Log).text += commandInputField.text;
			OnCommandSubmit?.Invoke(commandInputField.text);
			commandInputField.text = string.Empty;
		}

		private void ResetWindow() {
			logEntryContainer.GetChild(0).gameObject.SetActive(false);
			scrollRect.normalizedPosition = Vector2.zero;
			commandInputField.text = string.Empty;
			didExpand = false;
			didCycle = false;
		}
		
		public void PushSysInfo() {
			UnityEngine.Debug.Log($"= = = = = = = = = | Start System Info | = = = = = = = = =");
			UnityEngine.Debug.Log($"Operating System: {SystemInfo.operatingSystem}");
			UnityEngine.Debug.Log($"Operating System Family: {SystemInfo.operatingSystemFamily}");
			UnityEngine.Debug.Log($"Processor Count: {SystemInfo.processorCount}");
			UnityEngine.Debug.Log($"Processor Type: {SystemInfo.processorType}");
			UnityEngine.Debug.Log($"Processor Frequency: {((float)SystemInfo.processorFrequency / 1000f).Round(2)} GHz");
			UnityEngine.Debug.Log($"System Memory Size: {((float)SystemInfo.systemMemorySize / 1000f).Truncate(0)} GB");
			UnityEngine.Debug.Log($"Device Name: {SystemInfo.deviceName}");
			UnityEngine.Debug.Log($"Device Model: {SystemInfo.deviceModel}");
			UnityEngine.Debug.Log($"Device Type: {SystemInfo.deviceType}");
			UnityEngine.Debug.Log($"Device Unique Identifier: {SystemInfo.deviceUniqueIdentifier}");
			UnityEngine.Debug.Log($"Device Supports Audio: {SystemInfo.supportsAudio}");
			UnityEngine.Debug.Log($"Device Supports RayTracing: {SystemInfo.supportsRayTracing}");
			UnityEngine.Debug.Log($"Device Rendering Threading Mode: {SystemInfo.renderingThreadingMode}");
			UnityEngine.Debug.Log($"Supports Vibration: {SystemInfo.supportsVibration}");
			UnityEngine.Debug.Log($"Supports Accelerometer: {SystemInfo.supportsAccelerometer}");
			UnityEngine.Debug.Log($"Supports Gyroscope: {SystemInfo.supportsGyroscope}");
			UnityEngine.Debug.Log($"Supports Async Compute: {SystemInfo.supportsAsyncCompute}");
			UnityEngine.Debug.Log($"Supports Instancing: {SystemInfo.supportsInstancing}");
			UnityEngine.Debug.Log($"Battery Level: {SystemInfo.batteryLevel}");
			UnityEngine.Debug.Log($"Battery Status: {SystemInfo.batteryStatus}");
			UnityEngine.Debug.Log($"Graphics Device ID: {SystemInfo.graphicsDeviceID}");
			UnityEngine.Debug.Log($"Graphics Device Name: {SystemInfo.graphicsDeviceName}");
			UnityEngine.Debug.Log($"Graphics Device Type: {SystemInfo.graphicsDeviceType}");
			UnityEngine.Debug.Log($"Graphics Device Vendor: {SystemInfo.graphicsDeviceVendor}");
			UnityEngine.Debug.Log($"Graphics Device Vendor ID: {SystemInfo.graphicsDeviceVendorID}");
			UnityEngine.Debug.Log($"Graphics Device Version: {SystemInfo.graphicsDeviceVersion}");
			UnityEngine.Debug.Log($"Graphics Memory Size: {((float)SystemInfo.graphicsMemorySize / 1000f).Truncate(0)} GB");
			UnityEngine.Debug.Log($"Graphics Multi Threaded: {SystemInfo.graphicsMultiThreaded}");
			UnityEngine.Debug.Log($"Graphics Shader Level: {SystemInfo.graphicsShaderLevel}");
			UnityEngine.Debug.Log($"Graphics UV Start: {SystemInfo.graphicsUVStartsAtTop}");
			UnityEngine.Debug.Log($"Max Texture Size: {SystemInfo.maxTextureSize}");
			UnityEngine.Debug.Log($"NPOT Support: {SystemInfo.npotSupport}");
			UnityEngine.Debug.Log($"Supported Render Target Count: {SystemInfo.supportedRenderTargetCount}");
			UnityEngine.Debug.Log($"Supports 3D Textures: {SystemInfo.supports3DTextures}");
			UnityEngine.Debug.Log($"Supports Compute Shaders: {SystemInfo.supportsComputeShaders}");
			UnityEngine.Debug.Log($"Supports Cubemap Array Textures: {SystemInfo.supportsCubemapArrayTextures}");
			UnityEngine.Debug.Log($"Supports Location Service: {SystemInfo.supportsLocationService}");
			UnityEngine.Debug.Log($"Supports Shadows: {SystemInfo.supportsShadows}");
			UnityEngine.Debug.Log($"Supports Sparse Textures: {SystemInfo.supportsSparseTextures}");
			UnityEngine.Debug.Log($"Supports 2D Array Textures: {SystemInfo.supports2DArrayTextures}");
			UnityEngine.Debug.Log($"Supports 3D Render Textures: {SystemInfo.supports3DRenderTextures}");
			UnityEngine.Debug.Log($"Supports Motion Vectors: {SystemInfo.supportsMotionVectors}");
			UnityEngine.Debug.Log($"Supports Raw Shadow Depth Sampling: {SystemInfo.supportsRawShadowDepthSampling}");
			UnityEngine.Debug.Log($"Supports Set Constant Buffer: {SystemInfo.supportsSetConstantBuffer}");
			UnityEngine.Debug.Log($"Supports Texture Wrap Mirror Once: {SystemInfo.supportsTextureWrapMirrorOnce}");
			UnityEngine.Debug.Log($"Supports Graphics Fog: {SystemInfo.supportsGraphicsFence}");
			UnityEngine.Debug.Log($"Supports Multisample Auto Resolve: {SystemInfo.supportsMultisampleAutoResolve}");
			UnityEngine.Debug.Log($"Supports Multisampled Textures: {SystemInfo.supportsMultisampledTextures}");
			UnityEngine.Debug.Log($"Supports Texture Wrap Mirror Once: {SystemInfo.supportsTextureWrapMirrorOnce}");
			UnityEngine.Debug.Log($"NPOT Support: {SystemInfo.npotSupport}");
			UnityEngine.Debug.Log($"Screen DPI: {Screen.dpi}");
			UnityEngine.Debug.Log($"Screen Height: {Screen.height}");
			UnityEngine.Debug.Log($"Screen Width: {Screen.width}");
			UnityEngine.Debug.Log($"Screen Orientation: {Screen.orientation}");
			UnityEngine.Debug.Log($"Screen Resolution: {Screen.currentResolution}");
			UnityEngine.Debug.Log($"Screen Safe Area: {Screen.safeArea}");
			UnityEngine.Debug.Log($"Screen Sleep Timeout: {Screen.sleepTimeout}");
			UnityEngine.Debug.Log($"Screen Autorotate To Portrait: {Screen.autorotateToPortrait}");
			UnityEngine.Debug.Log($"Screen Autorotate To Portrait Upside Down: {Screen.autorotateToPortraitUpsideDown}");
			UnityEngine.Debug.Log($"Screen Autorotate To Landscape Left: {Screen.autorotateToLandscapeLeft}");
			UnityEngine.Debug.Log($"Screen Autorotate To Landscape Right: {Screen.autorotateToLandscapeRight}");
			UnityEngine.Debug.Log($"Screen Brightness {Screen.brightness}");
			UnityEngine.Debug.Log($"Screen Full Screen: {Screen.fullScreen}");
			UnityEngine.Debug.Log($"Screen Full Screen Mode: {Screen.fullScreenMode}");
			UnityEngine.Debug.Log($"Screen Resolutions: {Screen.resolutions.Length}");
			PushScreenResolutions();
			UnityEngine.Debug.Log($"Device HDR Display Support Flags: {SystemInfo.hdrDisplaySupportFlags}");
			UnityEngine.Debug.Log($"= = = = = = = = = | End System Info | = = = = = = = = =");
		}

		private void PushScreenResolutions() {
			foreach (var res in Screen.resolutions) {
				UnityEngine.Debug.Log($"\t{res}");
			}
		}
		#endregion
#endif
	}
}