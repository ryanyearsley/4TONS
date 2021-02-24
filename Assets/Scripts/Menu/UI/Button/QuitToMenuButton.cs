using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class QuitToMenuButton : AbstractButtonClick {

	protected override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		SaveManager.instance.SaveNewWizardDataJSON (playerOne.currentWizard);
		playerOne.currentWizard = null;
		playerOne.currentPlayerStateController = null;
		playerOne.isReady = false;
		playerOne.isAlive = false;
		SceneManager.LoadScene (0);
	}
}
