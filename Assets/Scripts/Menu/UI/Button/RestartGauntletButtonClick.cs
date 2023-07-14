using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class RestartGauntletButtonClick : AbstractButtonClick {

	public override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		playerOne.wizardSaveData = playerOne.wizardSaveData.wizardData.gauntletStartData.wizardSaveData.Clone ();
		playerOne.currentPlayerObject = null;
		NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
	}
}
