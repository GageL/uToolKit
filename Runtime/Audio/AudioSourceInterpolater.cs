using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace uToolKit.Runtime {
	public class AudioSourceInterpolater : MonoBehaviour {
		#region Public/Private Variables
		[Required]
		[SerializeField] private AudioSource targetSource;
		#endregion

		#region Runtime Variables
		private Tween audioTween;
		#endregion

		#region Native Methods
		private void Awake() {
			targetSource = this.GetComponent<AudioSource>();
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("Runtime Debug")]
		[Button]
		public void WindUp(float target, float duration, Ease ease, Action OnStart = null, Action OnComplete = null) {
			if (targetSource == null) { return; }
			if (audioTween != null) {
				audioTween.Kill();
			}
			audioTween = targetSource.DOFade(target, duration).SetEase(ease).
				OnStart(delegate {
					OnStart?.Invoke();
				}).
				OnComplete(delegate {
					OnComplete?.Invoke();
				});
		}

		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("Runtime Debug")]
		[Button]
		public void WindDown(float target, float duration, Ease ease, bool stopOnComplete, Action OnStart = null, Action OnComplete = null) {
			if (targetSource == null) { return; }
			if (audioTween != null) {
				audioTween.Kill();
			}
			audioTween = targetSource.DOFade(target, duration).SetEase(ease).
				OnStart(delegate {
					OnStart?.Invoke();
				}).
				OnComplete(delegate {
					if (stopOnComplete) {
						targetSource.Stop();
					}
					OnComplete?.Invoke();
				});
		}
		#endregion

		#region Private Methods

		#endregion
	}
}