﻿using System;
using System.Collections.Generic;

[Serializable]
public class PuzzleSaveData {
	public PuzzleData puzzleData;
	public int puzzleDataIndex;
	public PuzzleSaveDataDictionary puzzleSaveDataDictionary;

	public PuzzleSaveData(PuzzleData puzzleData, PuzzleSaveDataDictionary dictionary) {
		this.puzzleData = puzzleData;
		if (dictionary != null)
			this.puzzleSaveDataDictionary = dictionary.CloneDictionary ();
	}
}