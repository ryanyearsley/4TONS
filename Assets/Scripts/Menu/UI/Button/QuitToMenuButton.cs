using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class QuitToMenuButton : AbstractButtonClick {

	protected override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		//SaveManager.instance.SaveNewWizardDataJSON (playerOne.wizardSaveData);
		playerOne.wizardSaveData = null;
		playerOne.currentPlayerObject = null;
		playerOne.isReady = false;
		playerOne.isAlive = false;
		SceneManager.LoadScene (0);
	}
}
