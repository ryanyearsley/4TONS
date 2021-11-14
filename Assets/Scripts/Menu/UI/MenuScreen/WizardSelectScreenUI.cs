using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSelectScreenUI : AbstractScreenUI
{
	[SerializeField]
	private WizardSelectPlayerPanelUI[] wizardSelectPlayerPanels;

	private void Awake () {
		Debug.Log ("wizard select screen awake");
	}

	protected override void Start () {
		base.Start ();
		MainMenuManager.Instance.OnWizardDeleteEvent += OnWizardDelete;
	}


	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		base.OnScreenChange (mainMenuScreen);
		if (screenActiveStates.Contains (mainMenuScreen)) {
			UpdateUIsToPlayerCount (PlayerManager.instance.currentPlayers.Count);
			UpdateWizardSelectPanels ();
		}
	}
	protected override void OnPlayerJoin (Player player) {
		int playerIndex = player.playerIndex;
		UpdateUIsToPlayerCount (playerIndex + 1);
	}

	private void UpdateUIsToPlayerCount (int playerCount) {
		for (int i = 0; i < wizardSelectPlayerPanels.Length; i++) {
			if (playerCount > i) {
				wizardSelectPlayerPanels [i].gameObject.SetActive (true);
			} else {
				wizardSelectPlayerPanels [i].gameObject.SetActive (false);
			}
		}
	}

	private void OnWizardDelete (WizardSaveData wizardSaveData) {
		Debug.Log ("wizard select screen UI receiving delete wizard event...");
		UpdateWizardSelectPanels ();
	}

	private void UpdateWizardSelectPanels() {
		List<WizardSaveData> wizardSaveDatas = WizardSaveDataManager.instance.infamousWizardSaveDatas;

		Debug.Log ("updating wizard select panels. wizard count: " + wizardSaveDatas.Count);
		foreach (WizardSelectPlayerPanelUI panel in wizardSelectPlayerPanels) {
			panel.PopulateLoadedWizardButtons (wizardSaveDatas);
		}
	}
}
