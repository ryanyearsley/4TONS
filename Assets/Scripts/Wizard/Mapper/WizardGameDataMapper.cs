using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardGameDataMapper {

	public static WizardGameData MapWizardSaveToGameData (WizardSaveData wizardSaveData) {
		//Note: enrich SaveData before mapping.
		WizardGameData wizardGameData = new WizardGameData();
		wizardGameData.wizardName = wizardSaveData.wizardName;
		wizardGameData.spellSchoolData = wizardSaveData.spellSchoolData;
		wizardGameData.puzzleGameDataDictionary = new PuzzleGameDataDictionary ();
		wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.INVENTORY,
			new PuzzleGameData (wizardSaveData.inventorySaveData.puzzleData, PuzzleKey.INVENTORY,
			wizardSaveData.inventorySaveData.spellGemSaveDataDictionary));
		if (wizardSaveData.primaryStaffSaveData.puzzleData != null) {
			wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.PRIMARY_STAFF,
				new PuzzleGameData (wizardSaveData.primaryStaffSaveData.puzzleData, PuzzleKey.PRIMARY_STAFF,
				wizardSaveData.primaryStaffSaveData.spellGemSaveDataDictionary));
		}
		if (wizardSaveData.secondaryStaffSaveData.puzzleData != null) {
			wizardGameData.puzzleGameDataDictionary.Add (PuzzleKey.SECONDARY_STAFF,
				new PuzzleGameData (wizardSaveData.secondaryStaffSaveData.puzzleData, PuzzleKey.SECONDARY_STAFF,
				wizardSaveData.secondaryStaffSaveData.spellGemSaveDataDictionary));
		}
		//note: dictionaries are not copied yet. That is done through validation step with savedata on object.
		return wizardGameData;
	}

	public static SpellGemGameData MapSpellGemSaveToGameData (SpellGemSaveData spellGemSaveData) {

		SpellGemGameData spellGemGameData = new SpellGemGameData();
		spellGemGameData.spellData = spellGemSaveData.spellData;
		spellGemGameData.spellGemOriginCoordinate = spellGemSaveData.spellGemOriginCoordinate;
		spellGemGameData.spellGemRotation = spellGemSaveData.spellGemRotation;
		spellGemGameData.spellBindIndex = spellGemSaveData.spellBindIndex;
		return spellGemGameData;
	}

	public static void ValidateAndMapGemData (PuzzleGameData puzzleGameData, SpellGemSaveDataDictionary spellGemSaveDataDictionary) {
		puzzleGameData.spellGemGameDataDictionary = new SpellGemGameDataDictionary ();
		foreach (SpellGemSaveData spellGemSaveData in spellGemSaveDataDictionary.Values) {
			SpellGemGameData spellGemGameData = MapSpellGemSaveToGameData(spellGemSaveData);
			if (PuzzleUtility.CheckSpellFitmentEligibility (puzzleGameData, spellGemGameData)) {
				PuzzleUtility.AddSpellGemToPuzzle (puzzleGameData, spellGemGameData);
				if (puzzleGameData.puzzleData.puzzleType != PuzzleType.INVENTORY) {
					puzzleGameData.spellBindingDictionary [spellGemGameData.spellBindIndex] = spellGemGameData.spellCast;
				}

			} else {
				Debug.Log ("PlayerPuzzleComponent: Cannot convert SpellGemSaveData -> Game data. SpellGem does not fit in alleged spot.");
			}
		}
	}



}
