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
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		WizardPrebuildData starterData = playerOne.wizardSaveData.wizardData.gauntletStartData;
		string wizardName = playerOne.wizardSaveData.wizardName;
		PlayerManager.instance.ClearSelectedWizards();
		playerOne.wizardSaveData = starterData.wizardSaveData.Clone();
		playerOne.wizardSaveData.wizardName = wizardName;
		NerdstormSceneManager.instance.LoadGauntletTowerScene (Zone.Hub);
	}
}
