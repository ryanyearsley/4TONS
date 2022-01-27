using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSetupBannerUI : AbstractScreenUI
{

	[SerializeField]
	private TMP_Text objectiveText;

	[SerializeField]
	private Image selectedWizardImage;

	[SerializeField]
	private TMP_Text selectedWizardText;


	[SerializeField]
	private Sprite noWizardSelectedSprite;

	[SerializeField]
	private Button fightButton;
	protected override void Start () {
		base.Start ();
		MainMenuManager.Instance.OnWizardSelectEvent += OnWizardSelect;
		MainMenuManager.Instance.OnObjectiveSelectEvent += OnObjectiveSelect;
		ClearObjectiveSelectedUI ();
		ClearWizardSelected ();
		fightButton.interactable = false;
	}

	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		base.OnScreenChange (mainMenuScreen);
		if (mainMenuScreen == MenuScreen.MAIN_MENU) {
			ClearObjectiveSelectedUI ();
			ClearWizardSelected ();
		}
	}
	public void OnObjectiveSelect (Objective objective) {
		UpdateObjectiveSelectedUI (objective);
		CheckReadyCriteria ();
	}

	public void OnWizardSelect(int playerIndex, WizardSaveData wizardSaveData) {
		UpdateWizardSelected (wizardSaveData);
		CheckReadyCriteria ();
	}

	private void ClearObjectiveSelectedUI () {
		objectiveText.text = "N/A";
	}
	private void UpdateObjectiveSelectedUI(Objective objective) {
		objectiveText.text = objective.ToString();
	}

	private void ClearWizardSelected () {
		selectedWizardText.text = "?";
		selectedWizardImage.sprite = noWizardSelectedSprite;
	}


	private void UpdateWizardSelected (WizardSaveData wizardSaveData) {
		selectedWizardText.text = wizardSaveData.wizardName;
		selectedWizardImage.sprite = wizardSaveData.wizardData.portrait;
	}

	private void CheckReadyCriteria () {
		bool ready = MainMenuManager.Instance.CheckReadyCriteria ();
		if (ready) {
			fightButton.interactable = true;
		} else {
			fightButton.interactable = false;
		}
	}
}
