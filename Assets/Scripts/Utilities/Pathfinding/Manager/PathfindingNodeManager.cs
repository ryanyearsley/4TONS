using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Grid))]
public class PathfindingNodeManager : MonoBehaviour, IGameManager {

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public float nodeRadius;
	public TerrainType[] walkableRegions;
	public int obstacleProximityPenalty = 10;

	private LayerMask walkableMask;
	private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int> ();

	private Grid grid;
	private MapDetails currentMapDetails;
	public PathfindingNode[,] nodes;

	private int gridSizeX, gridSizeY;

	private int penaltyMin = int.MaxValue;
	private int penaltyMax = int.MinValue;

	void Start () {
		//InitializeManager (GameManager.instance.gameContext);
	}
	public void InitializeManager (GameContext gameContext) {
		grid = GetComponent<Grid> ();
		currentMapDetails = LevelManager.instance.GetMapDetails (0);
		StartCoroutine (GenerateFloorNodes (currentMapDetails));
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}
	public IEnumerator InitializeManagerRoutine() {
		yield return new WaitForSeconds (0.1f);

	}
	#region events/subscriptions



	public void OnBeginLevel (int levelIndex) {
		Debug.Log ("updating pathfinding nodes. floor index: " + levelIndex);
		currentMapDetails = LevelManager.instance.GetMapDetails (levelIndex);
		StartCoroutine (GenerateFloorNodes (currentMapDetails));
	}
	#endregion

	private IEnumerator GenerateFloorNodes (MapDetails mapDetails) {
		//

		yield return new WaitForSeconds (0.5f);
		foreach (TerrainType region in walkableRegions) {
			walkableMask.value |= region.terrainMask.value;
			walkableRegionsDictionary.Add ((int)Mathf.Log (region.terrainMask.value, 2), region.terrainPenalty);
		}
		CreateNodes (mapDetails);
	}
	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	private void CreateNodes (MapDetails mapDetails) {

		gridSizeX = mapDetails.mapBounds.maxCoord.x;
		gridSizeY = mapDetails.mapBounds.maxCoord.y;
		nodes = new PathfindingNode [gridSizeX, gridSizeY];

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector2Int worldIsoCoordinate = mapDetails.floorOrigin + new Vector2Int (x, y);


				Vector3 worldPoint = grid.GetCellCenterWorld(new Vector3Int(worldIsoCoordinate.x, worldIsoCoordinate.y, 0));
				//print ("Walkable: " + Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));
				bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));

				int movementPenalty = 0;

				Collider2D hitCollider2D = Physics2D.OverlapPoint (worldPoint, walkableMask);

				//print ("hit Collider 2d: " + ((hitCollider2D) ? "true" : "false"));
				if (hitCollider2D != null) {
					walkableRegionsDictionary.TryGetValue (hitCollider2D.gameObject.layer, out movementPenalty);
				}

				if (!walkable) {
					movementPenalty += obstacleProximityPenalty;
				}

				nodes [x, y] = new PathfindingNode (walkable, worldPoint, x, y, movementPenalty);
			}
		}

		BlurPenaltyMap (3);
	}

	private void BlurPenaltyMap (int blurSize) {
		int kernalSize = blurSize * 2 + 1;
		int kernalExtents = (kernalSize - 1) / 2;

		int [,] penaltiesHorizontalPass = new int [gridSizeX, gridSizeY];
		int [,] penaltiesVerticalPass = new int [gridSizeX, gridSizeY];

		for (int y = 0; y < gridSizeY; y++) {
			for (int x = -kernalExtents; x <= kernalExtents; x++) {
				int sampleX = Mathf.Clamp (x, 0, kernalExtents);
				penaltiesHorizontalPass [0, y] += nodes [sampleX, y].movementPenalty;
			}

			for (int x = 1; x < gridSizeX; x++) {
				int removeIndex = Mathf.Clamp (x - kernalExtents - 1, 0, gridSizeX);
				int addIndex = Mathf.Clamp (x + kernalExtents, 0, gridSizeX - 1);

				penaltiesHorizontalPass [x, y] = penaltiesHorizontalPass [x - 1, y] - nodes [removeIndex, y].movementPenalty + nodes [addIndex, y].movementPenalty;
			}
		}

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = -kernalExtents; y <= kernalExtents; y++) {
				int sampleY = Mathf.Clamp (y, 0, kernalExtents);
				penaltiesVerticalPass [x, 0] += penaltiesHorizontalPass [x, sampleY];
			}

			int blurredPenalty = Mathf.RoundToInt ((float) penaltiesVerticalPass [x, 0] / (kernalSize * kernalSize));
			nodes [x, 0].movementPenalty = blurredPenalty;

			for (int y = 1; y < gridSizeY; y++) {
				int removeIndex = Mathf.Clamp (y - kernalExtents - 1, 0, gridSizeY);
				int addIndex = Mathf.Clamp (y + kernalExtents, 0, gridSizeY - 1);

				penaltiesVerticalPass [x, y] = penaltiesVerticalPass [x, y - 1] - penaltiesHorizontalPass [x, removeIndex] + penaltiesHorizontalPass [x, addIndex];
				blurredPenalty = Mathf.RoundToInt ((float)penaltiesVerticalPass [x, y] / (kernalSize * kernalSize));
				nodes [x, y].movementPenalty = blurredPenalty;

				if (blurredPenalty > penaltyMax) {
					penaltyMax = blurredPenalty;
				}
				if (blurredPenalty < penaltyMin) {
					penaltyMin = blurredPenalty;
				}
			}
		}
	}

	public List<PathfindingNode> GetNeighbors (PathfindingNode node) {
		List<PathfindingNode> neighbors = new List<PathfindingNode> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) {
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbors.Add (nodes [checkX, checkY]);
				}
			}
		}
		return neighbors;
	}

	public PathfindingNode NodeFromWorldPoint (Vector3 worldPosition) {
		Vector2Int gridCoordinate = (Vector2Int)grid.WorldToCell (worldPosition);
		Vector2Int mapCoordinate = gridCoordinate - currentMapDetails.floorOrigin;
		//Debug.Log ("grid coord: " + gridCoordinate + ", map coord: " + mapCoordinate);
		if (currentMapDetails.mapBounds.isWithinBounds (mapCoordinate)) {
			return nodes [mapCoordinate.x, mapCoordinate.y];
		} else {
			Debug.Log ("Target is outside of AI pathfinding node bounds.");
			return null;
		}
	}

	void OnDrawGizmos () {
		if (nodes != null && displayGridGizmos) {
			Gizmos.color = Color.red;
			Gizmos.DrawCube (Vector3.zero, Vector3.one * nodeRadius * 2);

			Vector3 gridPosition = grid.GetCellCenterWorld(Vector3Int.one);
			Gizmos.DrawCube (gridPosition, Vector3.one * nodeRadius * 2);
			for (int x = 0; x < nodes.GetLength (0); x++) {
				for (int y = 0; y < nodes.GetLength (1); y++) {
					PathfindingNode node = nodes[x, y];
					//Debug.Log (" map coord: " + new Vector2Int(x, y) + ", world position: " + node.worldPosition);
					Gizmos.color = (node.walkable) ? Color.white : Color.red;
					Gizmos.DrawCube (node.worldPosition, Vector3.one * (nodeRadius * 2));
				}
			}
		}
	}
	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int terrainPenalty;
	}
}