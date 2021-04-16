using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffValidator : MonoBehaviour
{

	public StaffPickUp staffPickUp;




	public PuzzleEntity mockPuzzleGroupingDetails;


	public PuzzleTileInfo[,] map;


	public PuzzleSaveData testSaveData;/*
	private void Awake () {
		map = PuzzleFactory.DeserializePuzzleFile (staffPickUp.puzzleSaveData.puzzleData.puzzleFile);
		staffPickUp.ReuseStaffPickUp (testSaveData);
	}*/
}
