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

		public void LoadTutorial() {
			LoadSceneByIndex (tutorialSceneIndex);
		}
		public void LoadGauntletTowerScene(Zone towerZone) {
			if (gauntletSceneDictionary.ContainsKey(towerZone)) {
				LoadSceneByIndex (gauntletSceneDictionary [towerZone]);
			}
		}

		public void LoadSceneByIndex(int index) {
			StartCoroutine (LoadSceneRoutine (index));
		}

		public IEnumerator LoadSceneRoutine (int index) {
			sceneTransitionEvent?.Invoke ();
			yield return new WaitForSeconds (1f);//this allows fade-out screen to do it's thing lol
			SceneManager.LoadScene (index);

		}
	}
}
