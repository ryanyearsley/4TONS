using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class RestartTutorialButtonClick : AbstractButtonClick
{
	public override void OnClick()
	{
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		WizardPrebuildData starterData = ConstantsManager.instance.tutorialWizardData;
		string wizardName = starterData.wizardSaveData.wizardName;
		PlayerManager.instance.ClearSelectedWizards();
		playerOne.wizardSaveData = starterData.wizardSaveData.Clone();
		playerOne.wizardSaveData.wizardName = wizardName;
		NERDSTORM.NerdstormSceneManager.instance.LoadTutorial();
	}
}
