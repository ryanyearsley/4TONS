using UnityEngine.SceneManagement;
using System.Collections.Generic;
using NERDSTORM;
public class QuitToMenuButton : AbstractButtonClick {

	public override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		//SaveManager.instance.SaveNewWizardDataJSON (playerOne.wizardSaveData);
		playerOne.wizardSaveData = null;
		playerOne.currentPlayerObject = null;
		playerOne.isReady = false;
		playerOne.isAlive = false;
		NerdstormSceneManager.instance.LoadMenu ();
	}
}
