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
	public PlayerDeathInfo deathInfo;

	public List<WizardTowerStatus> towerProgress = new List<WizardTowerStatus>();

	//utility
	public bool CheckIfPlayerCompleteTower (Zone zone) {
		for (int i = 0; i < towerProgress.Count; i++) {
			if (towerProgress[i].zone == zone && towerProgress [i].completed) {
				return true;
			}
		}
		return false;
	}

	//This is used to copy a PrebuildWizard SO.
	public WizardSaveData Clone () {
		WizardSaveData clone =  (WizardSaveData) this.MemberwiseClone();
		clone.primaryStaffSaveData = primaryStaffSaveData.Clone ();
		if (clone.secondaryStaffSaveData.puzzleData != null)
			clone.secondaryStaffSaveData = secondaryStaffSaveData.Clone ();
		clone.inventorySaveData = inventorySaveData.Clone ();
		clone.towerProgress = new List<WizardTowerStatus> ();
		clone.towerProgress.Add (new WizardTowerStatus (Zone.Dark));
		clone.towerProgress.Add (new WizardTowerStatus (Zone.Light));
		return clone;
	}

	object ICloneable.Clone () {
		return Clone ();
	}


}

[Serializable]
public class WizardTowerStatus {
	public Zone zone;
	public bool completed;

	public WizardTowerStatus(Zone zone) {
		this.zone = zone;
		completed = false;
	}
}
