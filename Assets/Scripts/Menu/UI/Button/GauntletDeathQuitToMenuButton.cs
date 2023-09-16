using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public class GauntletDeathQuitToMenuButton : AbstractButtonClick
{
	GauntletGameOverPanelUI gameOverPanelUI;

	protected override void Awake()
	{
		base.Awake();
		gameOverPanelUI = GetComponentInParent<GauntletGameOverPanelUI>();
	}

	public override void OnClick()
	{
		gameOverPanelUI.ConfirmDeathInfo();
		PlayerManager.instance.ClearSelectedWizards();
		NerdstormSceneManager.instance.LoadMenu();
	}
}