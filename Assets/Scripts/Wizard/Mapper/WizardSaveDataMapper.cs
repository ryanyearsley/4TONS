using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSaveDataMapper
{
	public static WizardSaveData MapGameToSaveData (WizardGameData wizardGameData) {

		WizardSaveData wizardSaveData = new WizardSaveData();
		wizardSaveData.wizardName = wizardGameData.wizardName;
		wizardSaveData.spellSchoolDataIndex = wizardGameData.spellSchoolData.schoolIndexStart;
		wizardSaveData.inventorySaveData = new PuzzleSaveData ();
		return wizardSaveData;
	}
}
