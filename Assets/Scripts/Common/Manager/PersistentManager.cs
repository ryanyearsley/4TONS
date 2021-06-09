using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour, IPersistentManager {

	protected virtual void Awake () {
		//InitializeSingleton (); singleton initialized from inheriting scripts.
		DontDestroyOnLoad (this.gameObject);
	}
	protected virtual void Start () {
		SubscribeToEvents ();
	}
	void OnDisable() {
		//UnsubscribeFromEvents ();
	}

	public void SubscribeToEvents () {
		SceneManager.sceneLoaded += SceneLoaded;
		SettingsManager.instance.updateSettingsEvent += OnUpdateSettings;


	}
	public void UnsubscribeFromEvents () {
		SceneManager.sceneLoaded -= SceneLoaded;
		SettingsManager.instance.updateSettingsEvent -= OnUpdateSettings;

	}
	public virtual void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {

	}
	public virtual void OnUpdateSettings(SettingsData settingsData) {

	}
}
