using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreenManager : MonoBehaviour {
	private MainMenuScreen currentScreen;
	public GameObject welcomeScreen;
	public GameObject wizardSelectScreen;
	public GameObject levelSelectScreen;

	[SerializeField]
	private List<PlayerWizardSelectPanelUI> wizardSelectUIs = new List<PlayerWizardSelectPanelUI>();

	void Start () {
		MainMenuManager.Instance.OnMenuScreenChangeEvent += OnScreenChange;
		MainMenuManager.Instance.OnPlayerJoinEvent += OnPlayerJoin;
		OnScreenChange (MainMenuScreen.WELCOME);
	}

	public void OnScreenChange (MainMenuScreen screen) {
		currentScreen = screen;
		Debug.Log ("Screen: " + currentScreen);
		welcomeScreen.SetActive (false);
		wizardSelectScreen.SetActive (false);
		levelSelectScreen.SetActive (false);
		switch (currentScreen) {
			case MainMenuScreen.WELCOME:
				welcomeScreen.SetActive (true);
				break;
			case MainMenuScreen.WIZARD_SELECT:
				wizardSelectScreen.SetActive (true);
				break;
			case MainMenuScreen.LEVEL_SELECT:
				levelSelectScreen.SetActive (true);
				break;
			default:
				welcomeScreen.SetActive (true);
				break;
		}
	}
	public void OnPlayerJoin (Player player) {
		switch (currentScreen) {
			case MainMenuScreen.WELCOME:
				MainMenuManager.Instance.ChangeMenuScreen (MainMenuScreen.WIZARD_SELECT);
				AddWizardSelectPanel (player.playerIndex);
				break;
			case MainMenuScreen.WIZARD_SELECT:
				AddWizardSelectPanel (player.playerIndex);
				break;
			default:
				Debug.Log ("Player unable to join. Not a valid menu phase.");
				break;
		}
	}
	public void AddWizardSelectPanel (int playerIndex) {
		GameObject go = Instantiate (MainMenuConstantsManager.Instance.wizardSelectPanelPrefab);
		go.transform.parent = wizardSelectScreen.transform.GetChild(0);
		PlayerWizardSelectPanelUI wizardSelectPanelUI = go.GetComponent<PlayerWizardSelectPanelUI> ();
		wizardSelectPanelUI.InitializePanel (playerIndex);
	}
}