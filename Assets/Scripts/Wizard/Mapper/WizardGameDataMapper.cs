using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardGameDataMapper
{

	//DOES NOT MAP DICTIONARY (LoadPuzzle methods on PuzzleComponents instead validate the SaveData)
	public static WizardGameData MapWizardSaveToGameData(WizardSaveData wizardSaveData) {
		//Note: enrich SaveData before mapping.
		WizardGameData wizardGameData = new WizardGameData();
		wizardGameData.wizardName = wizardSaveData.wizardName;
		wizardGameData.spellSchoolData = wizardSaveData.spellSchoolData;
		wizardGameData.puzzleGameDataDictionary = new PuzzleGameDataDictionary ();
		wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.INVENTORY, new PuzzleGameData (wizardSaveData.inventorySaveData.puzzleData, PuzzleKey.INVENTORY));
		if (wizardSaveData.primaryStaffSaveData.puzzleData != null) {
			wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.PRIMARY_STAFF, new PuzzleGameData (wizardSaveData.primaryStaffSaveData.puzzleData, PuzzleKey.PRIMARY_STAFF));
		}
		if (wizardSaveData.secondaryStaffSaveData.puzzleData != null) {
			wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.SECONDARY_STAFF, new PuzzleGameData (wizardSaveData.secondaryStaffSaveData.puzzleData, PuzzleKey.SECONDARY_STAFF));
		}
		//note: dictionaries are not copied yet. That is done through validation step with savedata on object.
		return wizardGameData;
	}

	public static SpellGemGameData MapSpellGemSaveToGameData(SpellGemSaveData spellGemSaveData) {

		SpellGemGameData spellGemGameData = new SpellGemGameData();
		spellGemGameData.spellData = spellGemSaveData.spellData;
		spellGemGameData.spellGemOriginCoordinate = spellGemSaveData.spellGemOriginCoordinate;
		spellGemGameData.spellGemRotation = spellGemSaveData.spellGemRotation;
		spellGemGameData.spellBindIndex = spellGemSaveData.spellBindIndex;
		return spellGemGameData;
	}



}
