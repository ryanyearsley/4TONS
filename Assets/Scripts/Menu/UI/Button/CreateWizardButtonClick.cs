using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWizardButtonClick : AbstractButton {

	private PlayerWizardSelectPanelUI playerWizardSelectPanelUI;
	private WizardCreatePanelUI wizardCreatePanelUI;

	protected override void Awake () {
		base.Awake ();
		playerWizardSelectPanelUI = GetComponentInParent<PlayerWizardSelectPanelUI> ();
		wizardCreatePanelUI = GetComponentInParent<WizardCreatePanelUI> ();
	}
	protected override void OnClick () {
		Debug.Log ("Confirmation button clicked, saving wizard");
		if (wizardCreatePanelUI.isValidWizard ()) {
			WizardSaveData createdWizard = wizardCreatePanelUI.FinalizeWizard();
			SaveManager.instance.SaveNewWizardDataJSON (createdWizard);
			int playerIndex = playerWizardSelectPanelUI.playerIndex;
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (playerIndex, createdWizard);
			playerWizardSelectPanelUI.ChangeWizardSelectPhase (2);
		} else {
			Debug.Log ("Cannot confirm: no name entered.");
		}
	}
}