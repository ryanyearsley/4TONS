using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class RetireButton : AbstractButtonClick
{
	protected override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		WizardSaveData wizardSaveData = WizardSaveDataMapper.MapGameToSaveData(playerOne.currentPlayerObject.wizardGameData);
		WizardSaveDataManager.instance.SaveInfamousWizard (wizardSaveData);
		playerOne.SetPlayerWizardNull ();
		playerOne.currentPlayerObject = null;
		NerdstormSceneManager.instance.LoadMenu();
	}
}
