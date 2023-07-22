using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class RestartGauntletButtonClick : AbstractButtonClick {

	GauntletGameOverPanelUI gameOverPanelUI;

	protected override void Awake()
	{
		base.Awake();
		gameOverPanelUI = GetComponentInParent<GauntletGameOverPanelUI>();
	}

	public override void OnClick () {
		gameOverPanelUI.ConfirmDeathInfo();
		PlayerManager.instance.ClearSelectedWizards(); 
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		playerOne.wizardSaveData = playerOne.wizardSaveData.wizardData.gauntletStartData.wizardSaveData.Clone();
		NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
	}
}
