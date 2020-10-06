using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilderController : MonoBehaviour {

	public GameObject selectedIndicator;
	public Vector2Int levelSize = new Vector2Int (14, 14);
	public TilesetData tileset;

	private Transform indicator;
	private UnityEngine.Grid grid;

	private int[,] tileIndexes;
	private GameObject[,] tiles;

	private void Start () {
		indicator = Instantiate (selectedIndicator, Vector3.zero, Quaternion.identity, transform).transform;
		grid = GetComponent<UnityEngine.Grid> ();
		tileIndexes = new int [levelSize.x, levelSize.y];
		tiles = new GameObject [levelSize.x, levelSize.y];
	}

	private void Update () {
		Vector3 cursorScreenPos = Input.mousePosition.SetZ (0);
		Vector3 cursor = Camera.main.ScreenToWorldPoint (cursorScreenPos).SetZ (0);
		Vector3Int cellPosition = grid.WorldToCell (cursor);
		cellPosition.x = Mathf.Clamp (cellPosition.x, 0, levelSize.x - 1);
		cellPosition.y = Mathf.Clamp (cellPosition.y, 0, levelSize.y - 1);
		indicator.position = grid.GetCellCenterWorld (cellPosition);
	}

	private void OnDrawGizmos () {
		if (grid == null)
			grid = GetComponent<UnityEngine.Grid> ();
		Vector3 bottomCorner = grid.CellToWorld (new Vector3Int (0, 0, 0));
		Vector3 topCorner = grid.CellToWorld (new Vector3Int (levelSize.x, levelSize.y, 0));
		Vector3 leftCorner = grid.CellToWorld (new Vector3Int (0, levelSize.y, 0));
		Vector3 rightCorner = grid.CellToWorld (new Vector3Int (levelSize.x, 0, 0));
		Gizmos.color = Color.red;
		Gizmos.DrawLine (bottomCorner, leftCorner);
		Gizmos.DrawLine (leftCorner, topCorner);
		Gizmos.DrawLine (topCorner, rightCorner);
		Gizmos.DrawLine (rightCorner, bottomCorner);
	}
}