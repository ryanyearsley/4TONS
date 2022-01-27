using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PlayerManager : PersistentManager {
	#region Singleton
	public static PlayerManager instance { get; private set; }
	private void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	[SerializeField]
	public List<Player> currentPlayers = new List<Player>();

	protected override void Awake () {
		base.Awake ();
		InitializeSingleton ();
	}

	public override void SetUpPersistentManager () {
		if (MainMenuManager.Instance == null && currentPlayers.Count == 0) {
			Debug.Log ("PlayerManager: Spawning test player.");
			//This will create a test player in any scene aside from the menu.
			Player testPlayer = new Player(0, 0);
			testPlayer.isAlive = true;
			testPlayer.wizardSaveData = ConstantsManager.instance.defaultWizardData.wizardSaveData.Clone ();
			currentPlayers.Add (testPlayer);
		}
	}
	public override void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		Debug.Log ("PlayerManager: Scene Loaded");
	}
	public void AddPlayer(Player player) {
		if (currentPlayers.Contains (player)) return;
		currentPlayers.Add (player);
	}

	public void RemovePlayer (Player player) {
		if (currentPlayers.Contains (player)) {
			currentPlayers.Remove (player);
		}
	}
	public void ConfirmPlayerWizardSelection(int playerIndex, WizardSaveData wizardSaveDataClone) {
		if (currentPlayers.Count >= playerIndex) {
			currentPlayers [playerIndex].wizardSaveData = wizardSaveDataClone;
			currentPlayers [playerIndex].isReady = true;
		}
	}
	public void ClearSelectedWizards () {
		foreach (Player player in currentPlayers) {
			player.SetPlayerWizardNull ();
		}
	}

} 

[Serializable]
public class Player {
	public int playerIndex;
	public int controllerIndex;
	public bool isReady;

	public bool isAlive;
	public WizardSaveData wizardSaveData;
	public PlayerObject currentPlayerObject;

	public Player (int playerIndex, int controllerIndex) {
		this.playerIndex = playerIndex;
		this.controllerIndex = controllerIndex;
		isReady = false;
		isAlive = false;
	}

	public void SetPlayerWizardNull() {
		wizardSaveData = null;
		currentPlayerObject = null;
		isReady = false;
		isAlive = false;
	}
}

