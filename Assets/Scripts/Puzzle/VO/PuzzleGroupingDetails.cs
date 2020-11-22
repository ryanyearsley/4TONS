using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleGroupingType {
    EQUIPPED_STAFF, UNEQUIPPED_STAFF, INVENTORY
}

//Represents an inventory or staff "grouping" of spellgems and tile information.
[SerializeField]
public class PuzzleGroupingDetails {

    public PuzzleGroupingType puzzleType;
    public PuzzleTileInfo[,] map;
    //origin in relation to the puzzle grid (See PuzzleUI)
    public Vector2Int groupingOrigin;


    public CoordinateBounds mapBounds;
    public CoordinateBounds gridBounds;
    public Transform gemParentTransform;
    public Transform castParentTransform;
    public PuzzleSaveDataDictionary puzzleSaveDataDictionary;

	public PuzzleGroupingDetails (PuzzleTileInfo [,] map, Vector2Int groupingOrigin, PuzzleGroupingType puzzleType, PuzzleUI puzzleUI) {
		this.map = map;
		this.groupingOrigin = groupingOrigin;
        this.puzzleType = puzzleType;
        this.gemParentTransform = new GameObject (puzzleType + "_GemParentTransform").transform;
        this.gemParentTransform.parent = puzzleUI.GetComponentInChildren<Grid>().transform.parent;
        this.gemParentTransform.localPosition = Vector3.zero;
        this.castParentTransform = new GameObject (puzzleType + "_CastParentTransform").transform;
        this.castParentTransform.parent = puzzleUI.transform.root;
        this.castParentTransform.localPosition = Vector3.zero;

        puzzleSaveDataDictionary = new PuzzleSaveDataDictionary ();
        GenerateBounds ();
	}

	public void GenerateBounds() {
        Vector2Int maxGridCoords = new Vector2Int(groupingOrigin.x + map.GetLength(0) - 1, groupingOrigin.y + map.GetLength(1) - 1);
        CoordinateBounds gridBounds = new CoordinateBounds(groupingOrigin, maxGridCoords);
        this.gridBounds = gridBounds;
        Vector2Int maxMapCoords = new Vector2Int (map.GetLength (0) - 1, map.GetLength (1) - 1);
        mapBounds = new CoordinateBounds (Vector2Int.zero, maxMapCoords);
    }

    public Vector2Int ConvertGridToMapCell(Vector2Int gridCell) {
        return gridCell - groupingOrigin;
	}
}
