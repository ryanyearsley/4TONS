using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NERDSTORM {
	public class NerdstormSceneManager : MonoBehaviour {

		#region Singleton
		public static NerdstormSceneManager instance { get; private set; }
		private void InitializeSingleton () {
			if (instance == null) {
				instance = this;
			} else if (instance != this) {
				Destroy (gameObject);
			}
		}
		#endregion

		public int menuSceneIndex = 0;
		public int tutorialSceneIndex = 1;
		public int hubSceneIndex = 2; 
		public int darkTowerSceneIndex = 3;
		public int lightTowerSceneIndex = 4;
		public int zombieHordeSceneIndex = 5;

		public event Action sceneTransitionEvent;

		private Dictionary <Zone, int> gauntletSceneDictionary =
			new Dictionary<Zone, int> ();
		private void Awake () {
			InitializeSingleton ();
			DontDestroyOnLoad (this.gameObject);
			gauntletSceneDictionary.Add (Zone.Hub, hubSceneIndex);
			gauntletSceneDictionary.Add (Zone.Dark, darkTowerSceneIndex);
			gauntletSceneDictionary.Add (Zone.Light, lightTowerSceneIndex);
		}
		public void LoadMenu() {
			LoadSceneByIndex (menuSceneIndex);
		}

		public void LoadMenuInstant () {
			LoadSceneInstant (menuSceneIndex);
		}

		public void LoadTutorial() {
			LoadSceneByIndex (tutorialSceneIndex);
		}
		public void LoadZombieHorde () {
			LoadSceneByIndex (zombieHordeSceneIndex);
		}
		public void LoadGauntletTowerScene(Zone towerZone) {
			if (gauntletSceneDictionary.ContainsKey(towerZone)) {
				LoadSceneByIndex (gauntletSceneDictionary [towerZone]);
			}
		}

		public void LoadSceneByIndex(int index) {
			StartCoroutine (LoadSceneRoutine (index));
		}

		public void LoadSceneInstant (int index) {
			//sceneTransitionEvent?.Invoke ();
			SceneManager.LoadScene (index);
		}

		public IEnumerator LoadSceneRoutine (int index) {
			if (GameManager.instance != null && GameManager.instance.isPaused) {
				GameManager.instance.OnPause ();//unpauses game to allow for transition.
			}
			sceneTransitionEvent?.Invoke ();
			yield return new WaitForSeconds (1f);//this allows fade-out screen to do it's thing lol
			SceneManager.LoadScene (index);
		}
	}
}
