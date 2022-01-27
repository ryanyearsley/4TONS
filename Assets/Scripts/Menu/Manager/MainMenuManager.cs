using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public enum MenuScreen {
	WELCOME, MAIN_MENU, GAUNTLET_CREATE, WIZARD_SELECT, SETTINGS, TUTORIAL, CONTROLS, CREDITS, BLANK, LEADERBOARDS
}

public class MainMenuManager : MonoBehaviour {
	public MenuScreen currentMainMenuScreen { get; private set; }
	public event Action<MenuScreen> OnMenuScreenChangeEvent;
	public event Action<Objective> OnObjectiveSelectEvent;
	public event Action<Player> OnPlayerJoinEvent;
	public event Action<int, WizardSaveData> OnWizardSelectEvent;
	public event Action<WizardSaveData> OnWizardDeleteEvent;

	public Objective selectedObjective = Objective.NOT_SELECTED;
	#region Singleton
	public static MainMenuManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	private void Awake () {
		InitializeSingleton ();
	}

	void Start () {
		Debug.Log ("MainMenuManager: Start");
		StartCoroutine (StartRoutine ());
	}

	public IEnumerator StartRoutine () {
		Time.timeScale = 1;
		Debug.Log ("MainMenuManager: Start routine begin.");
		yield return new WaitForSeconds (0.3f);
		Debug.Log ("MainMenuManager: Animator Fade In.");
		if (PlayerManager.instance.currentPlayers.Count == 0) {
			ChangeMenuScreen (MenuScreen.WELCOME);
		} else {
			Debug.Log ("more than zero players active. Going to gametype select screen.");
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
		}
	}


	public void ChangeMenuScreen (MenuScreen screen) {
		currentMainMenuScreen = screen;
		OnMenuScreenChangeEvent?.Invoke (screen);
		if (screen == MenuScreen.MAIN_MENU) {
			PlayerManager.instance.ClearSelectedWizards ();
			ConfirmObjectiveSelection (Objective.NOT_SELECTED);
		}
	}

	public void PlayerCancel () {
		AudioManager.instance.PlaySound ("Back");
		if (currentMainMenuScreen != MenuScreen.MAIN_MENU)
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
	}
	public void OnPlayerJoin (int controllerIndex) {
		if (currentMainMenuScreen == MenuScreen.WELCOME) {
			Player player = CreateAndRegisterPlayer (controllerIndex);
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
			OnPlayerJoinEvent?.Invoke (player);
		}
	}

	public void OnWizardDelete (WizardSaveData wizardSaveData) {
		OnWizardDeleteEvent?.Invoke (wizardSaveData);
	}

	public Player CreateAndRegisterPlayer (int controllerIndex) {
		Player player = new Player(PlayerManager.instance.currentPlayers.Count, controllerIndex);
		PlayerManager.instance.AddPlayer (player);
		return player;
	}

	public void ConfirmObjectiveSelection (Objective objective) {
		selectedObjective = objective;
		OnObjectiveSelectEvent?.Invoke (objective);
	}



	public void ConfirmPlayerWizardSelection (int playerIndex, WizardSaveData selectedWizard) {
		PlayerManager.instance.ConfirmPlayerWizardSelection (playerIndex, selectedWizard);
		OnWizardSelectEvent?.Invoke (playerIndex, selectedWizard);
	}

	public bool CheckReadyCriteria () {

		if (MainMenuManager.Instance.selectedObjective == Objective.NOT_SELECTED) {
			return false;
		}
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			if (!player.isReady) {
				return false;
			}
		}
		return true;
	}
	public void StartGame() {
		if (CheckReadyCriteria()) {
			if (selectedObjective == Objective.Gauntlet) {
				NERDSTORM.NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
			} else if (selectedObjective == Objective.Zombie_Horde) {
				NERDSTORM.NerdstormSceneManager.instance.LoadZombieHorde ();
			}
		}
	}
}
