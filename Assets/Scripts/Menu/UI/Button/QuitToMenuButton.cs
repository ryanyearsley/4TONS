using UnityEngine.SceneManagement;
using System.Collections.Generic;
using NERDSTORM;
public class QuitToMenuButton : AbstractButtonClick {
	GauntletGameOverPanelUI gameOverPanelUI;

	protected override void Awake()
	{
		base.Awake();
	}

	public override void OnClick()
	{
		PlayerManager.instance.ClearSelectedWizards();
		NerdstormSceneManager.instance.LoadMenu();
	}
}
