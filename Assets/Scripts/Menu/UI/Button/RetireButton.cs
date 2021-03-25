using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetireButton : AbstractButtonClick
{
	protected override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		SaveManager.instance.SaveInfamousWizard (playerOne.wizardSaveData);
		playerOne.SetPlayerWizardFree ();
		SceneManager.LoadScene (0);//move to gauntlet hub
	}
}
