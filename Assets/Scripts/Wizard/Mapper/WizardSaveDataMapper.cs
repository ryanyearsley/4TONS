using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSaveDataMapper
{
	public static WizardSaveData MapGameToSaveData (WizardGameData wizardGameData) {

		WizardSaveData wizardSaveData = new WizardSaveData();
		wizardSaveData.wizardName = wizardGameData.wizardName;
		wizardSaveData.spellSchoolData = wizardGameData.spellSchoolData;
		wizardSaveData.wizardData = wizardGameData.spellSchoolData.wizardData;
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey(PuzzleKey.INVENTORY)) {
			wizardSaveData.inventorySaveData = (wizardGameData.puzzleGameDataDictionary [PuzzleKey.INVENTORY].MapToSaveData());
		}
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			wizardSaveData.primaryStaffSaveData = (wizardGameData.puzzleGameDataDictionary [PuzzleKey.PRIMARY_STAFF].MapToSaveData ());
		}
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			wizardSaveData.secondaryStaffSaveData = (wizardGameData.puzzleGameDataDictionary [PuzzleKey.SECONDARY_STAFF].MapToSaveData ());
		}
		return wizardSaveData;
	}
}
