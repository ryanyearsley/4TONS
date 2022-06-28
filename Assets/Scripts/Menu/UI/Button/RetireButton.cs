using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;
using TMPro;

public class RetireButton : AbstractButtonClick
{
	[SerializeField]
	private TMP_Text buttonText;
	private void Start () {
		if (GauntletHubGameManager.instance != null) {
			m_Button.interactable = true;
			buttonText.color = new Color (1, 1, 1, 1);
		} else {
			m_Button.interactable = false;
			buttonText.color = new Color (0, 0, 0, 0.5f);
		}
	}
	public override void OnClick () {
		Player playerOne = PlayerManager.instance.currentPlayers[0];
		WizardSaveData wizardSaveData = WizardSaveDataMapper.MapGameToSaveData(playerOne.currentPlayerObject.wizardGameData);
		WizardSaveDataManager.instance.SaveInfamousWizard (wizardSaveData);
		playerOne.SetPlayerWizardNull ();
		playerOne.currentPlayerObject = null;
		NerdstormSceneManager.instance.LoadMenu();
	}
}
