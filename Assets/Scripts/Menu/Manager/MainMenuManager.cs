using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MenuScreen {
	WELCOME, MAIN_MENU, GAUNTLET_CREATE, WIZARD_SELECT, SETTINGS, TUTORIAL, CONTROLS
}

public class MainMenuManager : MonoBehaviour {
	public MenuScreen currentMainMenuScreen { get; private set; }
	public event Action<MenuScreen> OnMenuScreenChangeEvent;
	public event Action<Player> OnPlayerJoinEvent;
	public event Action<WizardSaveData> OnWizardDeleteEvent;

	#region Singleton
	public static MainMenuManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	private void Awake () {
		InitializeSingleton ();
	}

	IEnumerator Start () {
		yield return new WaitForSeconds (0.05f);
		if (PlayerManager.instance.currentPlayers.Count > 0) {
			Debug.Log ("more than zero players active. Going to gametype select screen.");
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
		} else {
			ChangeMenuScreen (MenuScreen.WELCOME);
		}
	}
	public void ChangeMenuScreen(MenuScreen screen) {
		currentMainMenuScreen = screen;
		OnMenuScreenChangeEvent?.Invoke (screen);
	}
	public void OnPlayerJoin(int controllerIndex) {
		if (currentMainMenuScreen == MenuScreen.WELCOME) {
			Player player = CreateAndRegisterPlayer (controllerIndex);
			MainMenuManager.Instance.ChangeMenuScreen (MenuScreen.MAIN_MENU);
			OnPlayerJoinEvent?.Invoke (player);
		}
	}

	public void OnWizardDelete(WizardSaveData wizardSaveData) {
		OnWizardDeleteEvent?.Invoke (wizardSaveData);
	}

	public Player CreateAndRegisterPlayer (int controllerIndex) {
		Player player = new Player(PlayerManager.instance.currentPlayers.Count, controllerIndex);
		PlayerManager.instance.AddPlayer (player);
		return player;
	}
	public void ConfirmPlayerWizardSelection(WizardSaveData selectedWizard) {
		PlayerManager.instance.ConfirmPlayerWizardSelection (0, selectedWizard);
		bool isEveryoneReady = true;
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			if (!player.isReady) {
				isEveryoneReady = false;
			}
		}
		if (isEveryoneReady == true) {
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
		}
	}
}
