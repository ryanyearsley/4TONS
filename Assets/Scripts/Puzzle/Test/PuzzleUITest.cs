using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapOffsetTest : MonoBehaviour
{
	public PuzzleData testPuzzleData;

	public PuzzleEntity testPuzzleEntity;
	public Grid grid;
	public Tile tile;
	public Tilemap centeredTilemap;
	public Tilemap offsetTilemap;

	private void Awake () {
		//testPuzzleEntity = new PuzzleEntity (testPuzzleData, grid);
	}
}
