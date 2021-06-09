using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IPersistentManager 
{
	void SubscribeToEvents ();
	void UnsubscribeFromEvents ();
	void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode);
}
