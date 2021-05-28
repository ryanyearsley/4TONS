﻿using System;
using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;

public enum MenuScreen {
	WELCOME, MAIN_MENU, GAUNTLET_CREATE, WIZARD_SELECT, SETTINGS, TUTORIAL, CONTROLS, CREDITS, BLANK
}

public class MainMenuManager : MonoBehaviour {
	public MenuScreen currentMainMenuScreen  { get; private set; }
	public event Action<MenuScreen> OnMenuScreenChangeEvent;
	public event Action<Player> OnPlayerJoinEvent;
	public event Action<WizardSaveData> OnWizardDeleteEvent;


	[SerializeField]
	private Animator transitionAnimator;

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

	public IEnumerator StartRoutine() {
		Time.timeScale = 1;
		Debug.Log ("MainMenuManager: Start routine begin.");
		yield return new WaitForSeconds (0.3f);
		Debug.Log ("MainMenuManager: Animator Fade In.");
		transitionAnimator.SetTrigger ("FadeIn");
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

	public void PlayerCancel() {
		AudioManager.instance.PlaySound ("Back");
		if (currentMainMenuScreen != MenuScreen.MAIN_MENU)
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
	}
	public void OnPlayerJoin(int controllerIndex) {
		if (currentMainMenuScreen == MenuScreen.WELCOME) {
			Player player = CreateAndRegisterPlayer (controllerIndex);
			ChangeMenuScreen (MenuScreen.MAIN_MENU);
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
			ChangeMenuScreen (MenuScreen.BLANK);
		}
	}

	public void LoadScene(int sceneIndex) {
		StartCoroutine (LoadSceneRoutine (sceneIndex));
	}
	public IEnumerator LoadSceneRoutine(int sceneIndex) {
		transitionAnimator.SetTrigger ("FadeOut");
		yield return new WaitForSeconds (1f);
		SceneManager.LoadScene (sceneIndex);
	}
}
