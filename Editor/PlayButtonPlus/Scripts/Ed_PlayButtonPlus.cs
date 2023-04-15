using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace uToolKit.Editor {
#if UNITY_EDITOR
	//[InitializeOnLoad]
	public class PlayButtonPlusSOPreProcess {
		//static PlayButtonPlusSOPreProcess() {
			//string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Editor Default Resources", $"{PlayButtonPlusSO.DIRECTORY_NAME}");
			//if (!Directory.Exists(rootPath)) {
			//	Directory.CreateDirectory(rootPath);
			//	AssetDatabase.Refresh();
			//}
			//string assetPath = Path.Combine(rootPath, $"{PlayButtonPlusSO.FILE_NAME}.asset");
			//if (!File.Exists(assetPath)) {
			//	PlayButtonPlusSO asset = ScriptableObject.CreateInstance<PlayButtonPlusSO>();
			//	AssetDatabase.CreateAsset(asset, assetPath);
			//	AssetDatabase.SaveAssets();
			//	AssetDatabase.Refresh();
			//}
		//}
	}

	public class PlayButtonPlusSO : ScriptableObject {
		public const string DIRECTORY_NAME = "PlayButtonPlus";
		public const string FILE_NAME = "PlayButtonPlusSO";

		//void OnEnable() {
		//	EditorApplication.update += Update;
		//}

		//void OnDisable() {
		//	EditorApplication.update -= Update;
		//}

		//void Update() {
		//	// This code will be called once per frame in the main editor loop
		//	Debug.Log("Editor update");
		//}
	}

	public static class Ed_PlayButtonPlusSettings {
		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider() {
			var provider = new SettingsProvider("uToolKit/Play Button+", SettingsScope.User) {
				//label = "Play Button+",
				//activateHandler = (s, e) => {
				//	OnActivate();
				//},
				//deactivateHandler = () => {
				//	OnDeactivate();
				//},
				//guiHandler = (s) => {
				//	OnGUI();
				//}
			};
			return provider;
		}

		private static void OnActivate() {
			Debug.Log("<color=cyan>OnActivate</color>");
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnDeactivate() {
			Debug.Log("<color=cyan>OnDeactivate</color>");
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange obj) {
			Debug.Log("<color=cyan>OnPlayModeStateChanged</color>");
			//switch (obj) {
			//	case PlayModeStateChange.ExitingPlayMode:
			//		EditorSceneManager.playModeStartScene = null;
			//		break;
			//}
		}

		private static void OnGUI() {
			GUILayout.Label("Hi");
		}
	}

	public class Ed_PlayButtonPlus {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables
		//[MenuItem("Project/My Custom Settings")]
		//public static void ShowWindow() {
		//	EditorWindow.GetWindow(typeof(Ed_PlayButtonPlus));
		//}

		//void OnGUI() {
		//	GUILayout.Label("My Custom Settings", EditorStyles.boldLabel);
		//	// Add your custom UI elements here
		//}
		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		//static Ed_PlayButtonPlus() {
		//	EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		//}

		//private static void OnPlayModeStateChanged(PlayModeStateChange obj) {
		//	switch (obj) {
		//		case PlayModeStateChange.ExitingPlayMode:
		//			EditorSceneManager.playModeStartScene = null;
		//			break;
		//	}
		//}

		private static class ToolbarStyles {
			public static readonly GUIStyle textStyle;
			public static readonly GUIStyle commandButtonStyle;

			static ToolbarStyles() {
				textStyle = new GUIStyle(EditorStyles.label) {
					fontSize = 12,
					fontStyle = FontStyle.Normal,
					alignment = TextAnchor.LowerLeft
				};
				commandButtonStyle = new GUIStyle("Command") {
					imagePosition = ImagePosition.ImageAbove,
				};
			}
		}
		private static void OnToolbarGUI() {
			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Bootstrap: ", ToolbarStyles.textStyle, GUILayout.Width(62));
				GUILayout.Space(2);
				GUI.color = EditorApplication.isPlaying ? Color.red : Color.white;
				if (GUILayout.Button(EditorGUIUtility.IconContent($"{(EditorGUIUtility.isProSkin ? "d_" : "")}PlayButton On", "Play"), ToolbarStyles.commandButtonStyle)) {
					if (EditorApplication.isPlaying) {
						EditorApplication.isPlaying = false;
						return;
					}
					if (EditorBuildSettings.scenes.Length == 0) {
						EditorUtility.DisplayDialog("Play", "Error: You must add scenes to the 'Build Settings' window to use this", "Close");
						return;
					}
					EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
					EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes.FirstOrDefault(x => x.path.Contains("Bootstrapper")).path);
					EditorApplication.isPlaying = true;
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
#endif
}
