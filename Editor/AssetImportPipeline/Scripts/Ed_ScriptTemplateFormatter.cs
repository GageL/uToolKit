using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace uToolKit.Editor {
#if UNITY_EDITOR
	public class Ed_ScriptTemplateFormatter : UnityEditor.AssetModificationProcessor {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		public static void OnWillCreateAsset(string path) {
			path = path.Replace(".meta", "");
			int index = path.LastIndexOf(".");
			if (index < 0) { return; }
			string file = path.Substring(index);
			if (file != ".cs" && file != ".js" && file != ".boo") {
				return;
			}
			index = Application.dataPath.LastIndexOf("Assets");
			path = Application.dataPath.Substring(0, index) + path;
			file = System.IO.File.ReadAllText(path);
			file = file.Replace("#COMPANYNAME#", PlayerSettings.companyName.Replace(" ", ""));
			file = file.Replace("#PRODUCTNAME#", PlayerSettings.productName.Replace(" ", ""));
			file = file.Replace("#CREATIONDATE#", System.DateTime.Now + "");
			System.IO.File.WriteAllText(path, file);
			AssetDatabase.Refresh();
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
#endif
}