using System;
using System.Collections.Generic;

[Serializable]
public class PuzzleSaveData : ICloneable {
	public PuzzleData puzzleData;
	public int puzzleDataIndex;
	public SpellGemSaveDataDictionary spellGemSaveDataDictionary;

	#region clones
	public PuzzleSaveData Clone () {
		PuzzleSaveData puzzleSaveData =  (PuzzleSaveData) this.MemberwiseClone();
		if (puzzleSaveData.puzzleData != null) {
			puzzleSaveData.puzzleData = this.puzzleData;
			puzzleSaveData.puzzleDataIndex = puzzleData.id;
			if (spellGemSaveDataDictionary != null)
				this.spellGemSaveDataDictionary = spellGemSaveDataDictionary.CloneDictionary ();
		}
		return puzzleSaveData;
	}

	//Used by Staff pick-ups (clones in the puzzleData SO defaults instead of wizardSaveData)
	public PuzzleSaveData CloneDefault () {
		PuzzleSaveData puzzleSaveData =  (PuzzleSaveData) this.MemberwiseClone();
		puzzleSaveData.puzzleData = this.puzzleData;
		puzzleSaveData.puzzleDataIndex = puzzleData.id;
		if (puzzleData.defaultSpellGemDictionary != null)
			this.spellGemSaveDataDictionary = puzzleData.defaultSpellGemDictionary.CloneDictionary ();
		return puzzleSaveData;
	}
	object ICloneable.Clone () {
		return Clone ();
	}
	#endregion


}
