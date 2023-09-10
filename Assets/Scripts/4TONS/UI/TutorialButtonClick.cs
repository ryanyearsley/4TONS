using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtonClick : AbstractButtonClick {
	public override void OnClick () {
		AudioManager.instance.PlaySound ("Confirm");
		MainMenuManager.Instance.ConfirmPlayerWizardSelection (0, ConstantsManager.instance.tutorialWizardData.wizardSaveData);
		Debug.Log ("Tutorial button clicked. Beginning Tutorial.");
		NERDSTORM.NerdstormSceneManager.instance.LoadTutorial ();
	}
}
