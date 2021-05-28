using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWizardButton : AbstractButtonClick
{
	private LoadedWizardSelectionUI loadedWizardButtonUI;

	protected override void Awake () {
		base.Awake ();
		loadedWizardButtonUI = GetComponentInParent<LoadedWizardSelectionUI> ();

	}
	protected override void OnClick () {
		if (loadedWizardButtonUI.wizardSaveData != null) {
			Debug.Log ("delete button clicked, Deleting wizard");
			WizardSaveData selectedWizard = loadedWizardButtonUI.wizardSaveData;
			WizardSaveDataManager.instance.DeleteWizardData (selectedWizard.wizardName);
			MainMenuManager.Instance.OnWizardDelete (selectedWizard);
		} else {
			Debug.Log ("Wizard Delete Unsuccessful: No wizard assigned to button.");
		}
	}
}
