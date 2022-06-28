using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWizardButton : AbstractButtonHold
{
	private LoadedWizardSelectionUI loadedWizardButtonUI;


	protected override void Awake () {
		base.Awake ();
		loadedWizardButtonUI = GetComponentInParent<LoadedWizardSelectionUI> ();

	}
	public override void OnLongClick () {
		base.OnLongClick ();
		if (loadedWizardButtonUI.wizardSaveData != null) {
			Debug.Log ("delete button clicked, Deleting wizard " + loadedWizardButtonUI.wizardSaveData.wizardName);
			WizardSaveData selectedWizard = loadedWizardButtonUI.wizardSaveData;
			WizardSaveDataManager.instance.DeleteInfamousWizardData (selectedWizard.wizardName);
			MainMenuManager.Instance.OnWizardDelete (selectedWizard);
		} else {
			Debug.Log ("Wizard Delete Unsuccessful: No wizard assigned to button.");
		}
	}

}
