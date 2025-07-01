using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace uToolKit.Runtime {
    [DefaultExecutionOrder(-2)]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFadeControl : UIBehaviour {
        [FoldoutGroup("UIFadeControl")]
        [Required]
        [SerializeField]
        private CanvasGroup canvasGroup;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private bool startShown = false;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private float fadeInDuration = 0.15f;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private Ease fadeInEase = Ease.InOutQuad;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private float fadeOutDuration = 0.15f;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private Ease fadeOutEase = Ease.InOutQuad;
        [FoldoutGroup("UIFadeControl")]
        [SerializeField]
        private bool doLog = false;
        
        #region Public/Private Variables
        [FoldoutGroup("Events")]
        public UnityEvent OnShowStarted;
        [FoldoutGroup("Events")]
        public UnityEvent OnShowCompleted;
        [FoldoutGroup("Events")]
        public UnityEvent OnHideStarted;
        [FoldoutGroup("Events")]
        public UnityEvent OnHideCompleted;

        public float FadeOutDuration {
            get { return fadeOutDuration; }
        }
        public float FadeInDuration {
            get { return fadeInDuration; }
        }
        #endregion

        #region Runtime Variables
        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [SerializeField]
        protected bool isControlShown = false;
        #endregion

        #region Native Methods
#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            TryGetRef();
        }
#endif

        protected override void Start() {
            base.Start();
            if (canvasGroup == null) {
                return;
            }
            InstantControl(startShown);
        }
        #endregion

        #region Callback Methods
        #endregion

        #region Static Methods
        #endregion

        #region Public Methods
        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public void InstantControl(bool show) {
            Log($"{this.GetType().Name} | InstantControl | {this.gameObject.name} | show: {show}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.alpha = show ? 1 : 0;
            isControlShown = show;
            SetValues(show);
            if (show) {
                OnShowStart();
                OnShowStarted?.Invoke();
                OnShowComplete();
                OnShowCompleted?.Invoke();
            } else {
                OnHideStart();
                OnHideStarted?.Invoke();
                OnHideComplete();
                OnHideCompleted?.Invoke();
            }
        }

        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public void Show(Action OnStart = null, Action OnComplete = null) {
            Log($"{this.GetType().Name} | Show | {this.gameObject.name}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.DOFade(1, fadeInDuration).SetEase(fadeInEase).OnStart(() => {
                    OnShowStart();
                    SetValues(true);
                    OnStart?.Invoke();
                    OnShowStarted?.Invoke();
                    Log($"{this.GetType().Name} | Show | OnStart");
                }
            ).OnComplete(() => {
                    OnShowComplete();
                    isControlShown = true;
                    OnComplete?.Invoke();
                    OnShowCompleted?.Invoke();
                    Log($"{this.GetType().Name} | Show | OnComplete");
                }
            );
        }

        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public void ShowSilent() {
            Log($"{this.GetType().Name} | ShowSilent | {this.gameObject.name}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.DOFade(1, fadeInDuration).SetEase(fadeInEase).OnStart(() => {
                    OnShowStart();
                    SetValues(true);
                    OnShowStarted?.Invoke();
                    Log($"{this.GetType().Name} | ShowSilent | OnStart");
                }
            ).OnComplete(() => {
                    OnShowComplete();
                    isControlShown = true;
                    OnShowCompleted?.Invoke();
                    Log($"{this.GetType().Name} | ShowSilent | OnComplete");
                }
            );
        }

        protected virtual void OnShowStart() {}
        protected virtual void OnShowComplete() {}

        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public void Hide(Action OnStart = null, Action OnComplete = null) {
            Log($"{this.GetType().Name} | Hide | {this.gameObject.name}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, fadeOutDuration).SetEase(fadeOutEase).OnStart(() => {
                    OnHideStart();
                    OnStart?.Invoke();
                    OnHideStarted?.Invoke();
                    Log($"{this.GetType().Name} | Hide | OnStart");
                }
            ).OnComplete(() => {
                    OnHideComplete();
                    SetValues(false);
                    isControlShown = false;
                    OnComplete?.Invoke();
                    OnHideCompleted?.Invoke();
                    Log($"{this.GetType().Name} | Hide | OnComplete");
                }
            );
        }

        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public void HideSilent() {
            Log($"{this.GetType().Name} | HideSilent | {this.gameObject.name}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, fadeOutDuration).SetEase(fadeOutEase).OnStart(() => {
                    OnHideStart();
                    OnHideStarted?.Invoke();
                    Log($"{this.GetType().Name} | HideSilent | OnStart");
                }
            ).OnComplete(() => {
                    OnHideComplete();
                    SetValues(false);
                    isControlShown = false;
                    OnHideCompleted?.Invoke();
                    Log($"{this.GetType().Name} | HideSilent | OnComplete");
                }
            );
        }

        protected virtual void OnHideStart() {}
        protected virtual void OnHideComplete() {}

        [HideInEditorMode]
        [GUIColor(.82f, .40f, .34f)]
        [FoldoutGroup("UIFadeControl Runtime Debug")]
        [Button]
        public bool IsShown() {
            return isControlShown;
        }
        #endregion

        #region Private Methods
        private void TryGetRef() {
            if (canvasGroup != null) {
                return;
            }
            canvasGroup = this.GetComponent<CanvasGroup>();
        }

        private void SetValues(bool show) {
            Log($"{this.GetType().Name} | SetValues | {this.gameObject.name}");
            if (canvasGroup == null) {
                return;
            }
            canvasGroup.interactable = show ? true : false;
            canvasGroup.blocksRaycasts = show ? true : false;
        }

        private void Log(string message) {
            if (doLog) {
                Debug.Log(message);
            }
        }
        #endregion
    }
}