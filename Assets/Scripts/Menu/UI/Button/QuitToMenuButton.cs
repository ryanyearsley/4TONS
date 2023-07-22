using UnityEngine.SceneManagement;
using System.Collections.Generic;
using NERDSTORM;
public class QuitToMenuButton : AbstractButtonClick {
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
