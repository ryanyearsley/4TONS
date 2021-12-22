using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonClick : AbstractButtonClick {
	[SerializeField]
	private WizardPrebuildData tutorialWizard;
	public override void OnClick () {
		AudioManager.instance.PlaySound ("Confirm");
		MainMenuManager.Instance.ConfirmPlayerWizardSelection (tutorialWizard.wizardSaveData);
		Debug.Log ("Tutorial button clicked. Beginning Tutorial.");
		NERDSTORM.NerdstormSceneManager.instance.LoadTutorial ();
	}
}
