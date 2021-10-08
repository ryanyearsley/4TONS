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

		public int darkTowerSceneIndex;
		public int lightTowerSceneIndex;

		private Dictionary <SpellSchool, int> gauntletSceneDictionary =
			new Dictionary<SpellSchool, int> ();
		private void Awake () {
			InitializeSingleton ();
			DontDestroyOnLoad (this.gameObject);
			gauntletSceneDictionary.Add (SpellSchool.Dark, darkTowerSceneIndex);
			gauntletSceneDictionary.Add (SpellSchool.Light, lightTowerSceneIndex);
		}
		public void LoadMenu() {
			SceneManager.LoadScene (0);
		}

		public void LoadTutorial() {
			SceneManager.LoadScene (1);
		}
		public void LoadGauntletTowerScene(SpellSchool towerSchool) {
			if (gauntletSceneDictionary.ContainsKey(towerSchool)) {
				SceneManager.LoadScene (gauntletSceneDictionary [towerSchool]);
			}
		}

		public void LoadSceneByIndex(int index) {
			SceneManager.LoadScene (index);
		}
	}
}
