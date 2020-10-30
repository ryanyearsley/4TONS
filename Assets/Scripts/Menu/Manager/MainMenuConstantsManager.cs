using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuConstantsManager : MonoBehaviour {
	#region Singleton
	public static MainMenuConstantsManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	private void Awake () {
		InitializeSingleton ();
	}
	#endregion
	public GameObject wizardSelectPanelPrefab;
}
