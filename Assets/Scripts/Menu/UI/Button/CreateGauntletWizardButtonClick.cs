using UnityEngine;

public class CreateGauntletWizardButtonClick : AbstractButtonClick {

	private WizardCreatePanelUI wizardCreatePanelUI;

	protected override void Awake () {
		base.Awake ();
		wizardCreatePanelUI = GetComponentInParent<WizardCreatePanelUI> ();
	}
	public override void OnClick () {
		if (wizardCreatePanelUI.isValidWizard ()) {
			AudioManager.instance.PlaySound ("Confirm");
			WizardSaveData createdWizard = wizardCreatePanelUI.FinalizeWizardCreate();
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (0, createdWizard);
			MainMenuManager.Instance.StartGame ();
		} else {
			AudioManager.instance.PlaySound ("Error");
			//UI displays de
		}
	}
}