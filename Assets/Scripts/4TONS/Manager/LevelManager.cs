using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[RequireComponent (typeof (Grid))]
public class LevelManager : MonoBehaviour, IGameManager {

	#region Singleton
	public static LevelManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	private LoadingPanelUI loadLog;
	private const float MAX_GENERATION_TIME = 2f;
	public bool displayCustomSpawnGizmos;
	private MapDetails currentMapDetails;
	private ZoneData zoneData;
	private ObjectiveData objectiveData;
	[SerializeField]
	private ObjectRegistry objectRegistry;

	[SerializeField]
	private Tilemap baseTilemap;
	[SerializeField]
	private Tilemap baseHitboxTilemap;
	private CompositeCollider2D baseCompositeCollider;
	private CompositeCollider2D baseHitboxCompositeCollider;
	[SerializeField]
	private Tilemap floorTilemap;
	[SerializeField]
	private Tilemap floorDecorTilemap;
	[SerializeField]
	private Tilemap topTilemap;

	[SerializeField]
	private Tile baseHitboxTile;

	//int = levelIndex
	private Dictionary<int, MapDetails> mapDetailDictionary = new Dictionary<int, MapDetails>();

	[Header ("Pathfinding")]
	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public float nodeRadius;
	public TerrainType[] walkableRegions;
	public int obstacleProximityPenalty = 10;

	private LayerMask walkableMask;
	private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int> ();

	[SerializeField]
	private Grid grid;
	private MapDetails pathfindingMapDetails;
	public PathfindingNode[,] nodes;

	private int gridSizeX, gridSizeY;

	private int penaltyMin = int.MaxValue;
	private int penaltyMax = int.MinValue;

	#region awake/initialization
	void Awake () {
		SingletonInitialization ();

		loadLog = FindObjectOfType<LoadingPanelUI> ();
		grid = GetComponent<Grid> ();
		baseCompositeCollider = baseTilemap.GetComponent<CompositeCollider2D> ();
		baseHitboxCompositeCollider = baseHitboxTilemap.GetComponent<CompositeCollider2D> ();
	
	}

	void Start () {
		objectRegistry = ConstantsManager.instance.objectRegistry;
		InitializeManager (GameManager.instance.gameContext);
	}

	public void InitializeManager (GameContext gameContext) {
		GameManager.instance.loadLevelEvent += OnLoadLevel;
		GameManager.instance.levelEndEvent += OnLevelEnd;
		this.zoneData = gameContext.zoneData;
		this.objectiveData = gameContext.objectiveData;
		CreateLevelPools (zoneData, objectiveData);
		BackgroundManager.instance.SetUpCameraBackground (zoneData.backgroundGroup);
	}
	private void CreateLevelPools (ZoneData worldData, ObjectiveData objectivedata) {
		//PoolManager.instance.CreatePoolGeneric (worldData.playerData.spawnObjectPrefab, worldData.playerData.poolSize);
		//objective pools

		foreach (SetPieceSpawnInfo objectiveSpawnInfo in objectiveData.objectiveSpawnInfos) {
			PoolManager.instance.CreateObjectPool (objectiveSpawnInfo.setPieceData.spawnObjectPrefab, objectiveSpawnInfo.setPieceData.poolSize);
		}
		//player pools
		PoolManager.instance.CreatePlayerPool (objectiveData.playerObjectData.spawnObjectPrefab, 2);

		//creature pools
		foreach (CreatureData enemyData in worldData.enemyDatas) {
			PoolManager.instance.CreateCreaturePool (enemyData.spawnObjectPrefab, enemyData.poolSize);
		}
		PoolManager.instance.CreateObjectPool (ConstantsManager.instance.spellGemPickUpData.spawnObjectPrefab, 30);
		PoolManager.instance.CreateObjectPool (ConstantsManager.instance.staffPickUpData.spawnObjectPrefab, 15);
	}

	#endregion
	#region events/subscriptions


	public void OnLoadLevel (int levelIndex) {
		StartCoroutine (LoadLevelRoutine (levelIndex));
	}
	public void OnLevelEnd (int levelIndex) {

	}
	#endregion
	#region public utility methods

	public IEnumerator LoadLevelRoutine (int levelIndex) {
		currentMapDetails = null;
		GenerateMapDetails (zoneData, objectiveData, levelIndex);

		float generationTime = 0f;
		while (currentMapDetails == null) {
			yield return new WaitForSeconds (0.25f);
			generationTime++;
			if (generationTime > MAX_GENERATION_TIME) {
				yield break;
			}
		}
		if (currentMapDetails.mapData.mapGenerationData != null)
			BuildFloor (levelIndex);
		yield return new WaitForSeconds (0.25f);
		if (levelIndex == 0)
			CreatePlayerObjects ();
		else
			MovePlayerToNextStage (levelIndex);

		yield return new WaitForSeconds (0.25f);
		SpawnSpellGems (levelIndex);
		SpawnStaffs (levelIndex);
		BuildSetpieces (levelIndex);
		BuildObjectives (levelIndex);
		yield return new WaitForSeconds (0.25f);
		GenerateFloorNodes (currentMapDetails);
		SpawnEnemies (levelIndex);
		yield return new WaitForSeconds (0.25f);
		GameManager.instance.SetLevelLoaded ();
	}

	public void SetMapDetails(MapDetails mapDetails) {
		if (!mapDetailDictionary.ContainsKey (mapDetails.floorIndex)) {
			mapDetailDictionary.Add (mapDetails.floorIndex, mapDetails);
			currentMapDetails = mapDetails;
		}
	}

	public MapDetails GetMapDetails (int currentFloorIndex) {
		if (mapDetailDictionary.ContainsKey (currentFloorIndex)) {
			return mapDetailDictionary [currentFloorIndex];
		} else return null;
	}
	public Vector3 ConvertIsoCoordToScene (Vector2Int cartCoordinate) {
		return baseTilemap.GetCellCenterWorld ((Vector3Int)cartCoordinate);
	}
	public void GenerateMapDetails (ZoneData zoneData, ObjectiveData objectiveData, int floorIndex) {
		if (floorIndex < zoneData.mapDatas.Length) {
			MapData nextMapData = zoneData.mapDatas[floorIndex];
			if (nextMapData.mapGenerationData != null) {
				MapGenerator.instance.GenerateMap (zoneData, objectiveData, floorIndex);
			} else if (nextMapData.customMapData != null) {
				int[,] map = MapConversionUtility.Generate2DArrayFromTilemap(baseTilemap,
					new CoordinateBounds(Vector2Int.zero, Vector2Int.one * 56));
				MapDetails customMapDetails = new MapDetails (zoneData, nextMapData, floorIndex, MapConversionUtility.ConvertMapToTileInfo (map), nextMapData.customMapData.mapSpawnPoints);
				SetMapDetails (customMapDetails);
				GenerateFloorNodes (customMapDetails);
			}
			Debug.Log ("LevelManager: Adding floorIndex " + floorIndex);
		}
	}

	public void BuildFloor (int floorIndex) {
		Log ("Building Tilemaps...");
		if (mapDetailDictionary.ContainsKey (floorIndex)) {
			MapDetails buildingMapDetails = mapDetailDictionary[floorIndex];
			MapTileInfo[,] mapTileInfo = buildingMapDetails.mapTileInfo;
			int yLength = mapTileInfo.GetLength(1);
			int xLength = mapTileInfo.GetLength(0);
			Vector2Int origin = buildingMapDetails.floorOrigin;
			Vector3 floorPrefabSpawnPosition = baseTilemap.GetCellCenterWorld((Vector3Int)origin);
			if (zoneData.floorPrefab != null)
				Instantiate (zoneData.floorPrefab, floorPrefabSpawnPosition, Quaternion.identity); ;
			Transform parent = baseTilemap.transform.parent;
			GameObject tilemapObject = baseTilemap.transform.gameObject;
			//TILEMAP SETS ONLY
			for (int y = 0; y < mapTileInfo.GetLength (1); y++) {
				for (int x = 0; x < mapTileInfo.GetLength (0); x++) {
					MapTileInfo tileInfo = mapTileInfo[x, y];
					if (tileInfo.floorTile != null) {
						floorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), tileInfo.floorTile.tile);
					}
					if (tileInfo.floorDecorTile != null) {
						floorDecorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), tileInfo.floorDecorTile.tile);
					}
					if (tileInfo.baseTile != null) {
						UpdateBaseTilemap (new Vector3Int (x + origin.x, y + origin.y, 0), tileInfo.baseTile.tile);
					}
					if (tileInfo.topDecorTile != null) {
						topTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), tileInfo.topDecorTile.tile);
					} 
				}
			}
		}
	}
	public void UpdateBaseTilemap (Vector3Int coordinate, Tile tile) {
		floorTilemap.SetTile (coordinate, zoneData.primaryFloorTile.tile);
		baseTilemap.SetTile (coordinate, tile);
		baseHitboxTilemap.SetTile (coordinate, baseHitboxTile);
	}

	public void UpdateBase(Vector3Int coordinate, Tile tile) {

	}

	//NOTE: Players are only spawned on first stage. They are relocated to following stages.

	public void BuildSetpieces (int floorIndex) {
		Log ("Spawning setpieces...");
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.setPieceSpawnPoints) {
			Vector3 setpiecePosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			PoolManager.instance.ReuseObject (spawnPoint.spawnObjectData.spawnObjectPrefab, setpiecePosition, Quaternion.identity);
		}
	}
	public void BuildObjectives (int floorIndex) {
		Log ("Spawning objectives...");
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.objectiveSpawnPoints) {
			Vector3 setpiecePosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			PoolManager.instance.ReuseObject (spawnPoint.spawnObjectData.spawnObjectPrefab, setpiecePosition, Quaternion.identity);
		}
	}
	public void CreatePlayerObjects () {
		Log ("Spawning Player...");
		MapDetails currentMapDetails = mapDetailDictionary[0];
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			Vector2Int spawnCoordinate = currentMapDetails.spawnPoints.playerSpawnPoints[player.playerIndex].spawnCoordinate;
			Vector3 spawnPosition = ConvertIsoCoordToScene(spawnCoordinate);
			GameObject playerObject = PoolManager.instance.ReusePlayerObject(spawnPosition, player);
			player.isAlive = true;
			UIManager.Instance.GetPlayerUIFromPlayerIndex(player.playerIndex).InitializePlayerUI (player);
		}
	}

	public void MovePlayerToNextStage (int floorIndex) {
		Log ("Spawning Player...");
		Debug.Log ("moving player to floor " + floorIndex);
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			Vector2Int spawnCoordinate = currentMapDetails.spawnPoints.playerSpawnPoints[player.playerIndex].spawnCoordinate + currentMapDetails.floorOrigin;
			Vector3 spawnPosition = ConvertIsoCoordToScene(spawnCoordinate);
			player.currentPlayerObject.transform.position = spawnPosition;
			player.isAlive = true;
		}
	}
	public void SpawnEnemies (int floorIndex) {
		Log ("Spawning enemies...");
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		List<GauntletObjectiveComponent> remainingEnemies = new List<GauntletObjectiveComponent>();
		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.enemySpawnPoints) {
			Vector3 enemySpawnPosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject enemyObject = PoolManager.instance.ReuseCreatureObject(spawnPoint.spawnObjectData.spawnObjectPrefab, enemySpawnPosition);
		}
	}

	public void SpawnSpellGems (int floorIndex) {

		Log ("Spawning spell gems...");
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		int spellGemCount = 0;

		foreach (SpellGemSpawnPoint spellGemSpawnPoint in currentMapDetails.spawnPoints.spellGemSpawnPoints) {
			Vector3 spellGemSpawnPosition = ConvertIsoCoordToScene(spellGemSpawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject spellGemGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.spellGemPickUpData.spawnObjectPrefab, spellGemSpawnPosition, Quaternion.identity);
			spellGemGo.transform.position = spellGemSpawnPosition;
			SpellGemPickUpObject spellGemPickUp = spellGemGo.GetComponent<SpellGemPickUpObject>();
			spellGemPickUp.ReuseSpellGemPickUp (spellGemSpawnPoint.spellData);
			spellGemCount++;
		}
	}
	public void SpawnStaffs (int floorIndex) {

		Log ("Spawning staffs...");
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (StaffSpawnPoint staffSpawnPoint in currentMapDetails.spawnPoints.staffSpawnPoints) {
			Vector3 staffSpawnPosition = ConvertIsoCoordToScene(staffSpawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject staffPickUpGO = PoolManager.instance.ReuseObject(ConstantsManager.instance.staffPickUpData.spawnObjectPrefab, staffSpawnPosition, Quaternion.identity);
			staffPickUpGO.transform.position = staffSpawnPosition;
			StaffPickUpObject staffPickUp = staffPickUpGO.GetComponent<StaffPickUpObject>();
			staffPickUp.SetupObject ();
			staffPickUp.ReuseStaffPickUp (staffSpawnPoint.puzzleData);
		}
	}

	public void DestroyEnvironment (Vector3 centerPosition, int radius) {
		Vector3Int centerCoordinate = grid.WorldToCell(centerPosition) - ((Vector3Int)currentMapDetails.floorOrigin);
		for (int x = centerCoordinate.x - radius; x < centerCoordinate.x + radius; x++) {
			for (int y = centerCoordinate.y - radius; y < centerCoordinate.y + radius; y++) {
				Vector2Int mapCoordinate = new Vector2Int (x, y);
				if (currentMapDetails.mapBounds.isWithinBounds (mapCoordinate)) {
					Vector3 coordinatePos = grid.GetCellCenterLocal ((Vector3Int)mapCoordinate);
					MapTileInfo mapTileInfo = currentMapDetails.mapTileInfo [x, y];
					if (IsometricCoordinateUtilites.IsoDistanceBetweenPoints (centerPosition, coordinatePos) <= radius
					&& mapTileInfo.floorTile != null
					&& mapTileInfo.baseValue != zoneData.borderTile.id) {
						Vector3Int coordinate = new Vector3Int (x + currentMapDetails.floorOrigin.x, y + currentMapDetails.floorOrigin.y, 0);
						baseTilemap.SetTile (coordinate, null);
						topTilemap.SetTile (coordinate, null);
						mapTileInfo.walkable = true;
						if (zoneData.destructionDebrisObject != null)
							PoolManager.instance.ReuseObject (zoneData.destructionDebrisObject, coordinatePos, Quaternion.identity);
					}
				} else {
					Debug.Log ("LevelManager: Attempting to destroy outside of bounds.");
				}
			}
		}
	}

	private void Log (string logLine) {
		if (loadLog != null) {
			loadLog.UpdateLoadingLog (logLine);
		}
	}

	#endregion

	#region pathfinding methods
	[System.Serializable]
	public class TerrainType {
		public LayerMask terrainMask;
		public int terrainPenalty;
	}

	private void GenerateFloorNodes (MapDetails mapDetails) {
		//
		pathfindingMapDetails = mapDetails;
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

		gridSizeX = mapDetails.mapBounds.maxCoord.x + 1;
		gridSizeY = mapDetails.mapBounds.maxCoord.y + 1;
		nodes = new PathfindingNode [gridSizeX, gridSizeY];

		for (int y = 0; y < gridSizeY; y++) {
			for (int x = 0; x < gridSizeX; x++) {
				Vector2Int worldIsoCoordinate = mapDetails.floorOrigin + new Vector2Int (x, y);
				MapTileInfo tile = mapDetails.mapTileInfo[x, y];

				Vector3 worldPoint = grid.GetCellCenterWorld(new Vector3Int(worldIsoCoordinate.x, worldIsoCoordinate.y, 1));
				Debug.Log ("LevelManager: Checking coordinate " + worldIsoCoordinate +" @ world point " + worldPoint);
					
				if (Physics2D.OverlapCircle (worldPoint, 0.1f, unwalkableMask)) {
					Debug.Log ("LevelManager: Found obstacle @ " + worldIsoCoordinate +", marking unwalkable.");
					tile.walkable = false;
				}

				int movementPenalty = 0;

				Collider2D hitCollider2D = Physics2D.OverlapPoint (worldPoint, walkableMask);

				//print ("hit Collider 2d: " + ((hitCollider2D) ? "true" : "false"));
				if (hitCollider2D != null) {
					Debug.Log ("LevelManager: Found obstacle @ " + worldIsoCoordinate + ", setting movement penalty.");
					walkableRegionsDictionary.TryGetValue (hitCollider2D.gameObject.layer, out movementPenalty);
				}

				if (!tile.walkable) {
					movementPenalty += obstacleProximityPenalty;
				}

				nodes [x, y] = new PathfindingNode (tile.walkable, worldPoint, x, y, movementPenalty);
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
					continue; // self != neighbor
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX < 0 || checkX >= gridSizeX || checkY < 0 || checkY >= gridSizeY)
					continue;//out of bounds
				else {
					PathfindingNode pathfindingNode = nodes [checkX, checkY];
					if (x != 0 && y != 0) {
						//diagonal, validate mutual-neighbors...
						PathfindingNode mutualNeighborLeft = nodes[checkX, node.gridY];
						PathfindingNode mutualNeighborRight = nodes[node.gridX, checkY];
						if (pathfindingNode.walkable && mutualNeighborLeft.walkable && mutualNeighborRight.walkable) {
							neighbors.Add (pathfindingNode);
						}
					} else if (pathfindingNode.walkable) {
						//direct neighbor. Add away.
						neighbors.Add (pathfindingNode);
					}
				}

			}
		}
		return neighbors;
	}

	public PathfindingNode NodeFromWorldPoint (Vector3 worldPosition) {
		Vector2Int gridCoordinate = (Vector2Int)grid.WorldToCell (worldPosition);
		Vector2Int mapCoordinate = gridCoordinate - pathfindingMapDetails.floorOrigin;
		//Debug.Log ("grid coord: " + gridCoordinate + ", map coord: " + mapCoordinate);
		if (pathfindingMapDetails.mapBounds.isWithinBounds (mapCoordinate)) {
			return nodes [mapCoordinate.x, mapCoordinate.y];
		} else {
			Debug.Log ("Target is outside of AI pathfinding node bounds.");
			return null;
		}
	}

	//used by AI to randomly walk around during idle.
	public Vector3 RandomNearbyCoordinatePosition (Vector3 worldPosition, int range) {
		Vector2Int gridCoordinate = (Vector2Int)grid.WorldToCell (worldPosition);
		Vector2Int mapCoordinate = gridCoordinate - pathfindingMapDetails.floorOrigin;
		PathfindingNode randomWalkableNode = null;
		while (randomWalkableNode == null) {
			int randomX = mapCoordinate.x + Random.Range (-range, range);
			int randomY = mapCoordinate.y + Random.Range (-range, range);
			Vector2Int randomNearbyCoordinate = new Vector2Int (randomX, randomY);
			if (pathfindingMapDetails.mapBounds.isWithinBounds (randomNearbyCoordinate)
				&& nodes [randomNearbyCoordinate.x, randomNearbyCoordinate.y].walkable) {
				randomWalkableNode = nodes [randomNearbyCoordinate.x, randomNearbyCoordinate.y];
			}
		}
		return randomWalkableNode.worldPosition;
	}

	[ExecuteInEditMode]
	private void OnDrawGizmos () {
		if (displayCustomSpawnGizmos) {
			zoneData = FindObjectOfType<GameManager> ().gameContext.zoneData;
			foreach (MapData mapData in zoneData.mapDatas)
				if (mapData.customMapData != null) {
					Gizmos.color = Color.cyan;
					Debug.Log ("LevelManager: Drawing Spawn Gizmo at " + mapData.customMapData.mapSpawnPoints.playerSpawnPoints [0].spawnCoordinate);
					Gizmos.DrawSphere (grid.GetCellCenterWorld ((Vector3Int)
						mapData.customMapData.mapSpawnPoints.playerSpawnPoints [0].spawnCoordinate), 0.3f);
					Gizmos.color = Color.blue;
					foreach (SpellGemSpawnPoint spellGemSpawnPoint in mapData.customMapData.mapSpawnPoints.spellGemSpawnPoints) {
						Gizmos.DrawSphere (grid.GetCellCenterWorld ((Vector3Int)
					spellGemSpawnPoint.spawnCoordinate), 0.3f);
					}
					Gizmos.color = Color.green;
					foreach (StaffSpawnPoint staffSpawnPoint in mapData.customMapData.mapSpawnPoints.staffSpawnPoints) {
						Gizmos.DrawSphere (grid.GetCellCenterWorld ((Vector3Int)
					staffSpawnPoint.spawnCoordinate), 0.3f);
					}
					Gizmos.color = Color.red;
					foreach (SpawnPoint enemySpawnPoint in mapData.customMapData.mapSpawnPoints.enemySpawnPoints) {
						Gizmos.DrawSphere (grid.GetCellCenterWorld ((Vector3Int)
					enemySpawnPoint.spawnCoordinate), 0.3f);
					}
				}
		}
	}

	void OnDrawGizmosSelected () {
		if (nodes != null && displayGridGizmos) {

			Vector3 gridPosition = grid.GetCellCenterWorld(Vector3Int.one);

			Gizmos.DrawCube (gridPosition, Vector3.one * nodeRadius * 2);
			for (int x = 0; x < nodes.GetLength (0); x++) {
				for (int y = 0; y < nodes.GetLength (1); y++) {
					PathfindingNode node = nodes[x, y];
					//Debug.Log (" map coord: " + new Vector2Int(x, y) + ", world position: " + node.worldPosition);

					Gizmos.color = (node.walkable) ? Color.white : Color.black;
					Gizmos.DrawCube (node.worldPosition, Vector3.one * (nodeRadius * 2));
				}
			}
		}
	}

	#endregion
}

