using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentParentManager : MonoBehaviour {
	#region Singleton
	public static PersistentParentManager instance { get; private set; }
	private void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	private void Awake()
	{
		InitializeSingleton();
#if UNITY_EDITOR
		if (Application.isPlaying)
			UnityEditor.SceneVisibilityManager.instance.Show(gameObject, false);
#endif
		DontDestroyOnLoad(gameObject);
	}
}
