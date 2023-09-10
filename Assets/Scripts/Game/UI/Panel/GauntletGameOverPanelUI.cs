using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GauntletGameOverPanelUI : AbstractPanelUI
{

	private WizardSaveData currentWizardSaveData;
	private string endTimeFormatted;

	[SerializeField]
	private TMP_Text nameText;
	[SerializeField]
	TMP_Text todText;
	[SerializeField]
	TMP_InputField lastWordsInputField;
	[SerializeField]
	RandomLastWordsData lastWords;
	protected override void OnUIChange(GameState gameState)
	{
		if (panelActiveStates.Contains(gameState))
		{
			panelObject.SetActive(true);
			LoadDeathInfo();
		}
		else
		{
			panelObject.SetActive(false);
		}
	}

	private void LoadDeathInfo()
	{
		if (PlayerManager.instance.currentPlayers.Count > 0)
		{
			Player currentPlayer = PlayerManager.instance.currentPlayers[0];
			currentWizardSaveData = WizardSaveDataMapper.MapGameToSaveData(currentPlayer.currentPlayerObject.wizardGameData);
			endTimeFormatted = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			todText.text = endTimeFormatted;
			nameText.text = currentWizardSaveData.wizardName;
			lastWordsInputField.text = lastWords.GetRandomLastWords();
		}
	}

	public void ConfirmDeathInfo()
	{
		string lastWordsTruncated = TruncateLastWords(lastWordsInputField.text);
		Debug.Log("GameOverPanel: Last words: " + lastWordsTruncated);
		currentWizardSaveData.deathInfo = new PlayerDeathInfo(endTimeFormatted, lastWordsTruncated);
		WizardSaveDataManager.instance.SaveDeadWizard(currentWizardSaveData);
	}

	public static string TruncateLastWords(string inputString)
	{
		if (inputString.Length <= 100)
		{
			// If the input string is already shorter than the maxLength, return it as-is.
			return inputString;
		}
		else
		{
			// If the input string is longer than maxLength, shorten it and append "..." at the end.
			return inputString.Substring(0, 97) + "...";
		}
	}
}
