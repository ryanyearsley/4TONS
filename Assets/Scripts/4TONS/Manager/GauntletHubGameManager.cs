using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NERDSTORM;

public class GauntletHubGameManager : MonoBehaviour
{
	#region Singleton
	public static GauntletHubGameManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	private void Awake () {
		SingletonInitialization ();
	}
	public void TowerEntered(Zone zone) {
		GameManager.instance.LevelEnd (0);
		NerdstormSceneManager.instance.LoadGauntletTowerScene (zone);
	}

	public void WaflTempleEntered () {
		GameManager.instance.MakeDecision (0);
	}
}
