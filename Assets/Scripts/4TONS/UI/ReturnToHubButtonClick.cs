using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class ReturnToHubButtonClick : AbstractButtonClick
{
	public override void OnClick () {
		Debug.Log ("ReturnToHubButtonClick: Updating WizardSaveData and returning to hub.");
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		playerOne.wizardSaveData = WizardSaveDataMapper.MapGameToSaveData(playerOne.currentPlayerObject.wizardGameData);
		playerOne.currentPlayerObject = null;
		NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
	}

}
