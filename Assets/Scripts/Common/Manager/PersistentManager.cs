using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour, IPersistentManager {


	protected virtual void InitializeSingleton() {

	}
	protected virtual void Awake () {
		InitializeSingleton (); 
		DontDestroyOnLoad (this.gameObject);
	}
	protected virtual void Start () {
		SubscribeToEvents ();
		InitializePersistentManager ();
	}
	void OnDisable() {
		UnsubscribeFromEvents ();
	}
	public virtual void InitializePersistentManager() {

	}

	public virtual void SubscribeToEvents () {
		SceneManager.sceneLoaded += SceneLoaded;
	}
	public virtual void UnsubscribeFromEvents () {
		SceneManager.sceneLoaded -= SceneLoaded;
	}
	public virtual void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		UnsubscribeFromEvents ();
		SubscribeToEvents ();
	}

}
