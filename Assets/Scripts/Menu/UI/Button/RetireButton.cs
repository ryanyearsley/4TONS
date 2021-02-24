using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetireButton : AbstractButtonClick
{
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
