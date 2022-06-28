using UnityEngine;
using UnityEngine.UI;

public class CreateGauntletWizardButtonClick : AbstractButtonClick {

	private WizardCreatePanelUI wizardCreatePanelUI;

	protected override void Awake () {
		base.Awake ();
		wizardCreatePanelUI = GetComponentInParent<WizardCreatePanelUI> ();
	}
	public override void OnClick () {
		Debug.Log ("CreateGauntletWizard.OnClick");
		if (wizardCreatePanelUI.isValidWizard ()) {
			AudioManager.instance.PlaySound ("Confirm");
			WizardSaveData createdWizard = wizardCreatePanelUI.FinalizeWizardCreate();
			MainMenuManager.Instance.ConfirmPlayerWizardSelection (0, createdWizard);
			MainMenuManager.Instance.TryStartGame ();
		} else {
			AudioManager.instance.PlaySound ("Error");
			//UI displays de
		}
	}
}