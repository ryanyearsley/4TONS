using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/* Root class for a player wizard save file.
 * Deserialize To/From JSON using this class.
 * ENRICH to update indexes from Constants (before and after loading)
 * Map to/from WizardGameData object for gameplay Loading/Saving
 */
[Serializable]
public class WizardSaveData : ICloneable {
	public string wizardName;
	public SpellSchoolData spellSchoolData;
	public int spellSchoolDataIndex;
	public WizardData wizardData;
	public int wizardDataIndex;
	public PuzzleSaveData primaryStaffSaveData;
	public PuzzleSaveData secondaryStaffSaveData;
	public PuzzleSaveData inventorySaveData;

	public List<WizardTowerStatus> towerProgress = new List<WizardTowerStatus>();

	//utility
	public bool CheckIfPlayerCompleteTower (SpellSchool school) {
		for (int i = 0; i < towerProgress.Count; i++) {
			if (towerProgress[i].school == school && towerProgress [i].completed) {
				return true;
			}
		}
		return false;
	}

	//This is used to copy a PrebuildWizard SO.
	public WizardSaveData Clone () {
		WizardSaveData wizard =  (WizardSaveData) this.MemberwiseClone();
		wizard.primaryStaffSaveData = primaryStaffSaveData.Clone ();
		if (wizard.secondaryStaffSaveData.puzzleData != null)
			wizard.secondaryStaffSaveData = secondaryStaffSaveData.Clone ();
		wizard.inventorySaveData = inventorySaveData.Clone ();
		towerProgress.Add (new WizardTowerStatus (SpellSchool.Dark));
		towerProgress.Add (new WizardTowerStatus (SpellSchool.Light));
		return wizard;
	}

	object ICloneable.Clone () {
		return Clone ();
	}


}

[Serializable]
public class WizardTowerStatus {
	public SpellSchool school;
	public bool completed;

	public WizardTowerStatus(SpellSchool towerSchool) {
		school = towerSchool;
		completed = false;
	}
}
