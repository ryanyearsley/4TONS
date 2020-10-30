using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * When button is clicked,
 * Takes info/state of ChooseWizardButtonUI and sends to manager
 * */
public class SelectWizardButtonClick : AbstractButton {

	private PlayerWizardSelectPanelUI playerWizardSelectPanelUI;
	private LoadedWizardButtonUI loadedWizardButtonUI;

	protected override void Awake () {
		base.Awake ();
		playerWizardSelectPanelUI = GetComponentInParent<PlayerWizardSelectPanelUI> ();
		loadedWizardButtonUI = GetComponent<LoadedWizardButtonUI> ();

	}
	protected override void OnClick () {
		Debug.Log ("Confirmation button clicked, saving wizard");
		if (!loadedWizardButtonUI.IsVacant ()) {
			WizardSaveData selectedWizard = loadedWizardButtonUI.wizardSaveData;
			int playerIndex = playerWizardSelectPanelUI.playerIndex;
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (playerIndex, selectedWizard);
			playerWizardSelectPanelUI.ChangeWizardSelectPhase (2);
		} else {
			playerWizardSelectPanelUI.ChangeWizardSelectPhase (1);
		}
	}
}
