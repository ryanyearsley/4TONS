using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadePanelUI : MonoBehaviour {
	#region Singleton
	public static FadePanelUI instance { get; private set; }
	private void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	[SerializeField]
	private Animator transitionAnimator;
	private void Awake () {
		InitializeSingleton ();//ON DDOL Object
	}
	private void Start () {
		NERDSTORM.NerdstormSceneManager.instance.sceneTransitionEvent += OnSceneTransition;
	}
	private void OnSceneTransition () {
		StartCoroutine (SceneTransitionRoutine ());
	}
	private IEnumerator SceneTransitionRoutine() {
		Debug.Log ("FadePanelUI: Scene transition beginning");
		transitionAnimator.SetTrigger ("FadeOut");
		yield return new WaitForSeconds (1f);
		transitionAnimator.SetTrigger ("FadeIn");
	}
}
