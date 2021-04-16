
using System;
using System.Collections.Generic;

[Serializable]
public class WizardGameData {

	//Mapped to Save Data
	public string wizardName;
	public SpellSchoolData spellSchoolData;
	public PuzzleGameDataDictionary puzzleGameDataDictionary = new PuzzleGameDataDictionary();

	//in-game only
	public PuzzleKey currentStaffKey = PuzzleKey.PRIMARY_STAFF;

}