using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PuzzleType {
	STAFF, INVENTORY, PICK_UP
}

[Serializable]
public class PuzzleEntity : MonoBehaviour {

	public Tile tile;

	//static data
	public PuzzleGameData puzzleGameData;

	//OBJECT REFERENCE
	public CoordinateBounds gridBounds;
	public Transform puzzleObjectTrans;
	public Tilemap tilemap;//visual rendering of tiles

	private void Awake () {
		Debug.Log ("PuzzleEntity: Awake.");
		puzzleObjectTrans = transform;
		tilemap = GetComponent<Tilemap> ();
	}
	public void SetUpPuzzleEntity (PuzzleGameData puzzleGameData) {
		this.puzzleGameData = puzzleGameData;
		this.puzzleObjectTrans.gameObject.name = puzzleGameData.puzzleData.puzzleName + "_PuzzleEntity";
		PuzzleFactory.BuildPuzzleTilemap (puzzleGameData.map, tilemap);
	}

	public void SetPuzzleEntityGridParent(Grid grid, Vector2Int origin) {
		Debug.Log ("PuzzleEntity: SetPuzzleEntityGridParent.");
		this.gridBounds = new CoordinateBounds (origin, puzzleGameData.mapBounds.dimensions);

		this.puzzleObjectTrans.parent = grid.transform;
		this.puzzleObjectTrans.localPosition = grid.CellToLocal ((Vector3Int)origin);

	}
}
