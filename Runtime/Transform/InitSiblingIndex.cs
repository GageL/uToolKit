using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace uToolKit.Runtime {
	public class InitSiblingIndex : MonoBehaviour {
		[Serializable]
		public enum Mode { Awake, Start, Update };

		#region Public/Private Variables

		#endregion

		#region Runtime Variables
		[SerializeField] private bool workInEditor = false;
		[SerializeField] private Mode mode;
		[SerializeField] private bool forceLast = false;
		[InfoBox("Cannot set siblingIndex to a value less than 0", InfoMessageType.Warning, VisibleIf = "@siblingIndex < 0")]
		[SerializeField] private int siblingIndex = 0;
		#endregion

		#region Native Methods
		private void OnValidate() {
			if (!workInEditor) { return; }
			SetSiblingOrder();
		}

		private void Awake() {
			if (mode == Mode.Awake) {
				SetSiblingOrder();
			}
		}

		private void Start() {
			if (mode == Mode.Start) {
				SetSiblingOrder();
			}
		}

		private void Update() {
			if (mode == Mode.Update) {
				SetSiblingOrder();
			}
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		private void SetSiblingOrder() {
			if (siblingIndex < 0) { return; }
			if (forceLast) {
				this.transform.SetAsLastSibling();
			} else {
				if (this.transform.parent.childCount <= siblingIndex) {
					this.transform.SetSiblingIndex(this.transform.parent.childCount - 1);
				} else {
					this.transform.SetSiblingIndex(siblingIndex);
				}
			}
		}
		#endregion
	}
}