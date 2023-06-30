using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace uToolKit.Editor {
#if UNITY_EDITOR
	public class Ed_ScriptTemplateFormatter : UnityEditor.AssetModificationProcessor {
		#region Public/Private Variables
		private static readonly string CompanyNamePlaceholder = "#COMPANYNAME#";
		private static readonly string ProductNamePlaceholder = "#PRODUCTNAME#";
		private static readonly string CreationDatePlaceholder = "#CREATIONDATE#";

		private static string createdFilePath;
		private static bool shouldProcess;
		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		public static void OnWillCreateAsset(string path) {
			path = path.Replace(".meta", ""); // Remove the ".meta" extension

			if (!path.EndsWith(".cs")) // Process only C# script files
				return;

			createdFilePath = path;
			shouldProcess = true;

			EditorApplication.update += ProcessCreatedAsset;
		}

		private static void ProcessCreatedAsset() {
			if (!shouldProcess)
				return;

			if (!File.Exists(createdFilePath))
				return;

			string content = string.Empty;

			using (StreamReader reader = new StreamReader(createdFilePath)) {
				content = reader.ReadToEnd();
			}

			content = content.Replace(CompanyNamePlaceholder, PlayerSettings.companyName.Replace(" ", ""));
			content = content.Replace(ProductNamePlaceholder, PlayerSettings.productName.Replace(" ", ""));
			content = content.Replace(CreationDatePlaceholder, System.DateTime.Now.ToString());

			using (StreamWriter writer = new StreamWriter(createdFilePath)) {
				writer.Write(content);
			}

			EditorApplication.update -= ProcessCreatedAsset;
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