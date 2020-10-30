using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public enum MainMenuScreen {
	WELCOME, WIZARD_SELECT, LEVEL_SELECT, OPTIONS
} 
public class MainMenuManager : MonoBehaviour {
	public MainMenuScreen currentMainMenuScreen { get; private set; }
	public event Action<MainMenuScreen> OnMenuScreenChangeEvent;
	public event Action<Player> OnPlayerJoinEvent;

	#region Singleton
	public static MainMenuManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	private void Awake () {
		InitializeSingleton ();
	}
	public void ChangeMenuScreen(MainMenuScreen screen) {
		currentMainMenuScreen = screen;
		OnMenuScreenChangeEvent?.Invoke (screen);
	}
	public void OnPlayerJoin(int controllerIndex) {
		Player player = CreateAndRegisterPlayer (controllerIndex);
		OnPlayerJoinEvent?.Invoke (player);
	}

	public Player CreateAndRegisterPlayer (int controllerIndex) {
		Player player = new Player();
		player.playerIndex = PlayerManager.Instance.currentPlayers.Count;
		player.controllerIndex = controllerIndex;
		PlayerManager.Instance.AddPlayer (player);
		return player;
	}
	public void ConfirmPlayerWizardSelection(int playerIndex, WizardSaveData selectedWizard) {
		PlayerManager.Instance.ConfirmPlayerWizardSelection (playerIndex, selectedWizard);
		bool waitingForPlayers = false;
		foreach (Player player in PlayerManager.Instance.currentPlayers.Values) {
			if (!player.isReady) {
				waitingForPlayers = true;
			}
		}
		if (waitingForPlayers == false) {
			ChangeMenuScreen (MainMenuScreen.LEVEL_SELECT);
		}
	}
}
