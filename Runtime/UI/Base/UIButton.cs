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
	public class UIButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
		#region Public/Private Variables
		public enum TransitionType {
			None,
			Color,
			Sprite
		}
		public enum TargetGraphicType {
			Image,
			Text
		}
		public enum StateType {
			Idle,
			Highlighted,
			Pressed,
			Selected,
			SelectedHighlighted,
			SelectedPressed,
			Disabled
		}

		[Serializable]
		public class Element {
			public Graphic TargetGraphic { get => targetGraphic; }
			public TargetGraphicType GraphicType { get => graphicType; }
			public TransitionType Transition { get => transition; }
			[Serializable]
			public class CustomColorBlock {
				public Color NormalColor;
				public Color HighlightedColor;
				public Color PressedColor;
				public Color SelectedColor;
				public Color SelectedHighlightedColor;
				public Color SelectedPressedColor;
				public Color DisabledColor;
				public float ColorMultiplier;
				public float FadeDuration;
			}
			public CustomColorBlock ColorTransitions { get => colorTransitions; }
			[Serializable]
			public class CustomSpriteState {
				public Sprite NormalSprite;
				public Sprite HighlightedSprite;
				public Sprite PressedSprite;
				public Sprite SelectedSprite;
				public Sprite SelectedHighlightedSprite;
				public Sprite SelectedPressedSprite;
				public Sprite DisabledSprite;
			}
			public CustomSpriteState SpriteTransitions { get => spriteTransitions; }

			[Required]
			[HideIf("@transition == TransitionType.None")]
			[SerializeField] private Graphic targetGraphic;
			[InfoBox("You must have an Image target in order to use a sprite transition", InfoMessageType.Error, "@transition == TransitionType.Sprite && graphicType == TargetGraphicType.Text")]
			[HideIf("@transition == TransitionType.None")]
			[SerializeField] private TargetGraphicType graphicType = TargetGraphicType.Image;
			[SerializeField] private TransitionType transition = TransitionType.None;
			[ShowIf("@transition == TransitionType.Color")]
			[SerializeField]
			private CustomColorBlock colorTransitions = new CustomColorBlock() {
				NormalColor = new Color(1f, 1f, 1f),
				HighlightedColor = new Color(.8f, .8f, .8f),
				PressedColor = new Color(.65f, .65f, .65f),
				SelectedColor = new Color(1f, 1f, 1f),
				SelectedHighlightedColor = new Color(1f, 1f, 1f),
				SelectedPressedColor = new Color(1f, 1f, 1f),
				DisabledColor = new Color(1f, 1f, 1f, 0.3f),
				ColorMultiplier = 1f,
				FadeDuration = .1f
			};
			[ShowIf("@transition == TransitionType.Sprite && graphicType == TargetGraphicType.Image")]
			[SerializeField] private CustomSpriteState spriteTransitions;
		}
		public StateType CurrentState { get => currentState; }
		public bool IsSelectable { get => isSelectable; }

		[SerializeField] private bool startDisabled = false;
		[SerializeField] private bool isSelectable = false;
		[ShowIf("@isSelectable == true")]
		[Indent(1)]
		[SerializeField] private bool startSelected = false;
		[SerializeField] private bool isAnimation = false;

		[ShowIf("@isAnimation == false")]
		[SerializeField] private Element[] elements;

		public Animator Animator { get => animator; }
		[ShowIf("@isAnimation == true")]
		[Required]
		[SerializeField] private Animator animator;
		[ShowIf("@isAnimation == true")]
		[SerializeField] private AnimationTriggers animationTransitions;

		[FoldoutGroup("Events")]
		public UnityEvent PointerEnterEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerExitEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerDownEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerUpEvent;
		[FoldoutGroup("Events")]
		public UnityEvent PointerClickEvent;

		[SerializeField] private bool doLog = false;
		#endregion

		#region Runtime Variables
		public static UIButton CurrentHoveredButton;
		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("UIButton Runtime Debug")]
		[SerializeField] private StateType currentState;
		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("UIButton Runtime Debug")]
		[SerializeField] private bool isSelected;
		#endregion

		#region Native Methods
		protected override void Awake() {
			base.Awake();
			InitializeElements();
		}
		#endregion

		#region Callback Methods
		public virtual void OnPointerEnter(PointerEventData eventData) {
			CurrentHoveredButton = this;
			if (isSelected) {
				Animate(StateType.SelectedHighlighted);
			} else {
				Animate(StateType.Highlighted);
			}
			PointerEnterEvent?.Invoke();
		}

		public virtual void OnPointerExit(PointerEventData eventData) {
			CurrentHoveredButton = null;
			if (isSelected) {
				Animate(StateType.Selected);
			} else {
				Animate(StateType.Idle);
			}
			PointerExitEvent?.Invoke();
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			if (isSelected) {
				Animate(StateType.SelectedPressed);
			} else {
				Animate(StateType.Pressed);
			}
			PointerDownEvent?.Invoke();
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (isSelected) {
				Animate(StateType.Selected);
			} else {
				Animate(StateType.Idle);
			}
			PointerUpEvent?.Invoke();
		}

		public virtual void OnPointerClick(PointerEventData eventData) {
			PointerClickEvent?.Invoke();
		}
		#endregion

		#region Static Methods

		#endregion

		#region Public Methods
		[HideInEditorMode]
		[GUIColor(.82f, .40f, .34f)]
		[FoldoutGroup("UIButton Runtime Debug")]
		[Button]
		public void ForceState(StateType state) {
			Animate(state, true);
		}
		#endregion

		#region Private Methods
		private void InitializeElements() {
			Log($"{this.GetType().Name} | InitializeElements | {this.gameObject.name}");
			if (startDisabled) {
				Animate(StateType.Disabled);
			} else {
				if (startSelected) {
					Animate(StateType.Selected);
				} else {
					Animate(StateType.Idle);
				}
			}
		}

		private void Animate(StateType state, bool isForced = false) {
			Log($"{this.GetType().Name} | Animate | {this.gameObject.name} | state: {state}");
			if (currentState == StateType.Disabled && !isForced) {
				Log($"{this.GetType().Name} | Animate | {this.gameObject.name} | Disable, negating state '{state}'");
				return;
			}
			switch (state) {
				case StateType.Idle:
					if (isSelectable) {
						isSelected = false;
					}
					currentState = StateType.Idle;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.normalTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									if (CurrentHoveredButton == this) {
										currentState = StateType.Highlighted;
										elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.HighlightedColor, elements[i].ColorTransitions.FadeDuration);
									} else {
										elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.NormalColor, elements[i].ColorTransitions.FadeDuration);
									}
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (CurrentHoveredButton == this) {
										if (!elements[i].SpriteTransitions.HighlightedSprite) { break; }
										currentState = StateType.Highlighted;
										((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.HighlightedSprite;
									} else {
										if (!elements[i].SpriteTransitions.NormalSprite) { break; }
										((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.NormalSprite;
									}
									break;
							}
						}
					}
					break;
				case StateType.Highlighted:
					currentState = StateType.Highlighted;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.highlightedTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.HighlightedColor, elements[i].ColorTransitions.FadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (!elements[i].SpriteTransitions.HighlightedSprite) { break; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.HighlightedSprite;
									break;
							}
						}
					}
					break;
				case StateType.Pressed:
					currentState = StateType.Pressed;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.pressedTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.PressedColor, elements[i].ColorTransitions.FadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (!elements[i].SpriteTransitions.PressedSprite) { break; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.PressedSprite;
									break;
							}
						}
					}
					break;
				case StateType.Selected:
					if (isSelectable) {
						isSelected = true;
						currentState = StateType.Selected;
						if (isAnimation) {
							if (animator == null) { break; }
							if (isForced && state == currentState) { break; }
							animator.SetTrigger(animationTransitions.selectedTrigger);
						} else {
							for (int i = 0; i < elements.Length; i++) {
								if (!elements[i].TargetGraphic) { break; }
								switch (elements[i].Transition) {
									case TransitionType.Color:
										if (CurrentHoveredButton == this) {
											currentState = StateType.SelectedHighlighted;
											elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.SelectedHighlightedColor, elements[i].ColorTransitions.FadeDuration);
										} else {
											elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.SelectedColor, elements[i].ColorTransitions.FadeDuration);
										}
										break;
									case TransitionType.Sprite:
										if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
										if (CurrentHoveredButton == this) {
											if (!elements[i].SpriteTransitions.SelectedHighlightedSprite) { break; }
											currentState = StateType.SelectedHighlighted;
											((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.SelectedHighlightedSprite;
										} else {
											if (!elements[i].SpriteTransitions.SelectedSprite) { break; }
											((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.SelectedSprite;
										}
										break;
								}
							}
						}
					}
					break;
				case StateType.SelectedHighlighted:
					currentState = StateType.SelectedHighlighted;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.selectedTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.SelectedHighlightedColor, elements[i].ColorTransitions.FadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (!elements[i].SpriteTransitions.SelectedHighlightedSprite) { break; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.SelectedHighlightedSprite;
									break;
							}
						}
					}
					break;
				case StateType.SelectedPressed:
					currentState = StateType.SelectedPressed;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.pressedTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.SelectedPressedColor, elements[i].ColorTransitions.FadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (!elements[i].SpriteTransitions.SelectedPressedSprite) { break; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.SelectedPressedSprite;
									break;
							}
						}
					}
					break;
				case StateType.Disabled:
					if (isSelectable) {
						isSelected = false;
					}
					currentState = StateType.Disabled;
					if (isAnimation) {
						if (animator == null) { break; }
						if (isForced && state == currentState) { break; }
						animator.SetTrigger(animationTransitions.disabledTrigger);
					} else {
						for (int i = 0; i < elements.Length; i++) {
							if (!elements[i].TargetGraphic) { break; }
							switch (elements[i].Transition) {
								case TransitionType.Color:
									elements[i].TargetGraphic.DOColor(elements[i].ColorTransitions.DisabledColor, elements[i].ColorTransitions.FadeDuration);
									break;
								case TransitionType.Sprite:
									if (elements[i].GraphicType != TargetGraphicType.Image) { break; }
									if (!elements[i].SpriteTransitions.PressedSprite) { break; }
									((Image)elements[i].TargetGraphic).sprite = elements[i].SpriteTransitions.DisabledSprite;
									break;
							}
						}
					}
					break;
			}
			Log($"{this.GetType().Name} | Animate | {this.gameObject.name} | currentState: {currentState}");
		}
		
		private Graphic TryGetGraphic() {
			Log($"{this.GetType().Name} | TryGetGraphic | {this.gameObject.name}");
			return this.GetComponent<Graphic>();
		}

		private void Log(string message) {
			if (doLog) {
				Debug.Log(message);
			}
		}
		#endregion
	}
}