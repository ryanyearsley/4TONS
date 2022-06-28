using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * When button is clicked,
 * Takes info/state of ChooseWizardButtonUI and sends to manager
 * */
public class SelectWizardButtonClick : AbstractButtonClick {

	private WizardSelectPlayerPanelUI playerPanelUI;
	private int playerIndex;
	private LoadedWizardSelectionUI loadedWizardButtonUI;


	protected override void Awake () {
		base.Awake ();
		playerPanelUI = transform.GetComponentInParent<WizardSelectPlayerPanelUI> ();
		playerIndex = playerPanelUI.transform.GetSiblingIndex ();

		loadedWizardButtonUI = GetComponentInParent<LoadedWizardSelectionUI> ();

	}
	public override void OnClick () {
		if (loadedWizardButtonUI.wizardSaveData != null) {
			Debug.Log ("Confirmation button clicked, saving wizard");
			WizardSaveData selectedWizard = loadedWizardButtonUI.wizardSaveData;
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (playerIndex, selectedWizard);
		} else {
			Debug.Log ("Wizard Selection Unsuccessful: No wizard assigned to button.");
		}
	}
}
