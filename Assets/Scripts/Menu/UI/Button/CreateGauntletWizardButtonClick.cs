using UnityEngine;

public class CreateGauntletWizardButtonClick : AbstractButtonClick {

	private WizardCreatePanelUI wizardCreatePanelUI;

	protected override void Awake () {
		base.Awake ();
		wizardCreatePanelUI = GetComponentInParent<WizardCreatePanelUI> ();
	}
	protected override void OnClick () {
		if (wizardCreatePanelUI.isValidWizard ()) {
			Debug.Log ("Confirmation button clicked, saving wizard");
			WizardSaveData createdWizard = wizardCreatePanelUI.FinalizeWizardCreate();
			Debug.Log ("Created wizard name: " + createdWizard.wizardName);
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (createdWizard);
			NERDSTORM.NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
			AudioManager.instance.PlaySound ("Confirm");
		} else {
			Debug.Log ("Cannot confirm: no name entered.");
		}
	}
}