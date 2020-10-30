using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBuilderManager : MonoBehaviour {

	public static LevelBuilderManager instance;

	public GameObject selectedIndicator;
	public Vector2Int levelSize = new Vector2Int (14, 14);
	public TilesetData tileset;
	public GameObject tileUIPrefab;
	public Transform scrollViewContents;
	public LayerMask buildableMask;

	private Transform indicator;
	private UnityEngine.Grid grid;

	private GameObject currentTile;
	private int currentIndex;
	private int[,] tileIndexes;
	private GameObject[,] tiles;

	private void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	private void Start () {
		indicator = Instantiate (selectedIndicator, Vector3.zero, Quaternion.identity, transform).transform;
		grid = GetComponent<UnityEngine.Grid> ();
		tileIndexes = new int [levelSize.x, levelSize.y];
		tiles = new GameObject [levelSize.x, levelSize.y];
		if (tileset != null) {
			for (int i = 0; i < tileset.Count; i++) {
				GameObject ui = Instantiate (tileUIPrefab, scrollViewContents);
				SpriteRenderer tileSR = tileset[i].GetComponent<TileObject> ().spriteRenderer;
				ui.GetComponent<TilePaletteButtonUI> ().Initialize (i, tileSR.sprite, tileSR.color);
			}
		}
		currentTile = tileset [0];
		currentIndex = 1;
	}

	private void Update () {
		Vector3 cursorScreenPos = Input.mousePosition.SetZ (0);
		Vector3 cursor = Camera.main.ScreenToWorldPoint (cursorScreenPos).SetZ (0);
		Vector3Int cellPosition = grid.WorldToCell (cursor);
		cellPosition.x = Mathf.Clamp (cellPosition.x, 0, levelSize.x - 1);
		cellPosition.y = Mathf.Clamp (cellPosition.y, 0, levelSize.y - 1);
		indicator.position = grid.GetCellCenterWorld (cellPosition);

		if (Input.GetMouseButtonDown (0)) {
			CreateTile (cursor);
		}
	}

	public void CreateTile (Vector3 cursor) {
		if (Physics2D.OverlapPoint (cursor.XY (), buildableMask) != null) {
			Vector3 position = grid.GetCellCenterWorld (grid.WorldToCell (cursor)).SetZ (0);
			Vector2Int indexes = grid.WorldToCell (cursor).XY ();
			tileIndexes [indexes.x, indexes.y] = currentIndex;

			if (tiles [indexes.x, indexes.y] != null)
				Destroy (tiles [indexes.x, indexes.y]);

			tiles [indexes.x, indexes.y] = Instantiate (currentTile, position, Quaternion.identity);
		}
	}

	public void OnSelectTile (int index) {
		currentTile = tileset [index];
		currentIndex = index + 1;
		Debug.Log (currentTile);
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