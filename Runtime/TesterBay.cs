using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultCompany.uToolKitProject {
	public class TesterBay : MonoBehaviour {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods
		private void Awake() {
			Debug.Log("<color=red>Awake</color>");
		}

		private void Start() {
			Debug.Log("<color=red>Start</color>");
		}
		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		[RuntimeInitializeOnLoadMethod()]
		private static void RuntimeInitializeOnLoadMethod() {
			Debug.Log("<color=yellow>RuntimeInitializeOnLoadMethod</color>");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void AfterSceneLoad() {
			Debug.Log("<color=yellow>AfterSceneLoad</color>");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void BeforeSceneLoad() {
			Debug.Log("<color=yellow>BeforeSceneLoad</color>");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void AfterAssembliesLoaded() {
			Debug.Log("<color=yellow>AfterAssembliesLoaded</color>");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void BeforeSplashScreen() {
			Debug.Log("<color=yellow>BeforeSplashScreen</color>");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SubsystemRegistration() {
			Debug.Log("<color=yellow>SubsystemRegistration</color>");
		}
		#endregion
	}
}