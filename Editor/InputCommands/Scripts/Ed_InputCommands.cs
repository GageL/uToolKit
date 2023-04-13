using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace uToolKit.Editor {
#if UNITY_EDITOR
	public static class Ed_InputCommands {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		[MenuItem("Window/uToolKit/Editor Commands/Clear Console _F3")]
		static void ClearConsole() {
			Debug.ClearDeveloperConsole();
			var assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));
			var type = assembly.GetType("UnityEditor.LogEntries");
			var method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
			Debug.Log("EditorInputCommands | ClearConsole");
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
#endif
}