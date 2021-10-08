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
	public bool displayCustomSpawnGizmos;
	private MapDetails currentLevelDetails;
	private ZoneData zoneData;
	private ObjectiveData objectiveData;
	[SerializeField]
	private ObjectRegistry objectRegistry;

	[SerializeField]
	private Tilemap baseTilemap;
	[SerializeField]
	private Tilemap baseHitboxTilemap;
	[SerializeField]
	private Tilemap floorTilemap;
	[SerializeField]
	private Tilemap floorDecorTilemap;
	[SerializeField]
	private Tilemap topTilemap;

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
		grid = GetComponent<Grid> ();
	}

	void Start () {
		objectRegistry = ConstantsManager.instance.objectRegistry;
		InitializeManager (GameManager.instance.gameContext);
	}

	public void InitializeManager (GameContext gameContext) {
		GameManager.instance.loadLevelEvent += OnLoadLevel;
		GameManager.instance.levelEndEvent += OnLevelEnd;
		this.zoneData = gameContext.worldData;
		this.objectiveData = gameContext.objectiveData;
		baseHitboxTile = ConstantsManager.instance.baseHitboxTile;
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
		GenerateMapDetails (zoneData, objectiveData, levelIndex);
		yield return new WaitForSeconds (0.5f);
		if (currentLevelDetails.mapData.mapGenerationData != null)
			BuildFloor (levelIndex);
		else {
			GenerateFloorNodes (currentLevelDetails);
		}
		yield return new WaitForSeconds (0.5f);
		if (levelIndex == 0)
			CreatePlayerObjects ();
		else
			MovePlayerToNextStage (levelIndex);

		yield return new WaitForSeconds (0.5f);
		SpawnSpellGems (levelIndex);
		SpawnStaffs (levelIndex);
		BuildSetpieces (levelIndex);
		BuildObjectives (levelIndex);
		yield return new WaitForSeconds (0.5f);
		SpawnEnemies (levelIndex);
		yield return new WaitForSeconds (0.5f);

		GameManager.instance.SetLevelLoaded ();

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
			MapDetails generatingMapDetails = null;
			if (nextMapData.mapGenerationData != null) {
				generatingMapDetails = MapGenerator.instance.GenerateMap (zoneData, objectiveData, floorIndex);
			} else if (nextMapData.customMapData != null) {
				int[,] map = MapConversionUtility.Generate2DArrayFromTilemap(baseTilemap,
					new CoordinateBounds(Vector2Int.zero, Vector2Int.one * 56));
				generatingMapDetails = new MapDetails (zoneData, nextMapData, floorIndex, MapConversionUtility.ConvertMapToTileInfo (map), nextMapData.customMapData.mapSpawnPoints);

				GenerateFloorNodes (generatingMapDetails);
			}
			Debug.Log ("LevelManager: Adding floorIndex " + floorIndex);
			mapDetailDictionary.Add (floorIndex, generatingMapDetails);
			currentLevelDetails = generatingMapDetails;
		}
	}

	public void BuildFloor (int floorIndex) {
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
			for (int y = 0; y < mapTileInfo.GetLength (1); y++) {
				for (int x = 0; x < mapTileInfo.GetLength (0); x++) {
					MapTileInfo tileInfo = mapTileInfo[x, y];
					int tileValue = tileInfo.value;
					if (objectRegistry.activeTileDictionary.ContainsKey (tileValue)) {
						if (tileInfo.tileLayer == TileLayer.FLOOR) {
							floorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), objectRegistry.activeTileDictionary [tileValue]);
							mapTileInfo [x, y].walkable = true;
						} else if (tileInfo.tileLayer == TileLayer.FLOOR_DECOR) {
							floorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), zoneData.primaryFloorTile.tile);
							floorDecorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), objectRegistry.activeTileDictionary [tileValue]);
							mapTileInfo [x, y].walkable = true;
						} else if (tileInfo.tileLayer == TileLayer.BASE) {
							UpdateBaseTilemap(new Vector3Int (x +origin.x, y + origin.y, 0), objectRegistry.activeTileDictionary [tileValue]);
							mapTileInfo [x, y].walkable = false;
						} else if (tileInfo.tileLayer == TileLayer.TOP) {
							UpdateBaseTilemap (new Vector3Int (x + origin.x, y + origin.y, 0), zoneData.baseTile.tile);
							topTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), objectRegistry.activeTileDictionary [tileValue]);
							mapTileInfo [x, y].walkable = false;
						}
					} else if (objectRegistry.activeSetpieceDictionary.ContainsKey (tileValue)) {
						Vector3 position = baseTilemap.GetCellCenterWorld (new Vector3Int (x + origin.x, y + origin.y, 0));
						PoolManager.instance.ReuseObject (objectRegistry.activeSetpieceDictionary [tileValue].spawnObjectPrefab, position, Quaternion.identity);
						mapTileInfo [x, y].walkable = false;
					} else {
						floorTilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), zoneData.primaryFloorTile.tile);
						tileInfo.walkable = true;
					}
				}
			}
			GenerateFloorNodes (buildingMapDetails);
		}
	}
	public void UpdateBaseTilemap(Vector3Int coordinate, Tile tile) {
		floorTilemap.SetTile (coordinate, zoneData.underTile.tile);
		baseTilemap.SetTile (coordinate, tile);
		baseHitboxTilemap.SetTile (coordinate, baseHitboxTile);
	}


	//NOTE: Players are only spawned on first stage. They are relocated to following stages.

	public void BuildSetpieces (int floorIndex) {
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.setPieceSpawnPoints) {
			Vector3 setpiecePosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			PoolManager.instance.ReuseObject (spawnPoint.spawnObjectData.spawnObjectPrefab, setpiecePosition, Quaternion.identity);
		}
	}
	public void BuildObjectives (int floorIndex) {
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.objectiveSpawnPoints) {
			Vector3 setpiecePosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			PoolManager.instance.ReuseObject (spawnPoint.spawnObjectData.spawnObjectPrefab, setpiecePosition, Quaternion.identity);
		}
	}
	public void CreatePlayerObjects () {
		Debug.Log ("creating player objects");
		MapDetails currentMapDetails = mapDetailDictionary[0];
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			Vector2Int spawnCoordinate = currentMapDetails.spawnPoints.playerSpawnPoints[player.playerIndex].spawnCoordinate;
			Vector3 spawnPosition = ConvertIsoCoordToScene(spawnCoordinate);
			GameObject playerObject = PoolManager.instance.ReusePlayerObject(spawnPosition, player);
			player.isAlive = true;
			UIManager.Instance.playerUIs [player.playerIndex].InitializePlayerUI (player);
		}
	}

	public void MovePlayerToNextStage (int floorIndex) {
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
		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		List<GauntletObjectiveComponent> remainingEnemies = new List<GauntletObjectiveComponent>();
		foreach (SpawnPoint spawnPoint in currentMapDetails.spawnPoints.enemySpawnPoints) {
			Vector3 enemySpawnPosition = ConvertIsoCoordToScene(spawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject enemyObject = PoolManager.instance.ReuseCreatureObject(spawnPoint.spawnObjectData.spawnObjectPrefab, enemySpawnPosition);
			currentMapDetails.remainingEnemies.Add (enemyObject.GetComponent<GauntletObjectiveComponent> ());
		}
		currentMapDetails.totalEnemiesCount = currentMapDetails.remainingEnemies.Count;
	}

	public void SpawnSpellGems (int floorIndex) {

		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		int spellGemCount = 0;

		foreach (SpellGemSpawnPoint spellGemSpawnPoint in currentMapDetails.spawnPoints.spellGemSpawnPoints) {
			Vector3 spellGemSpawnPosition = ConvertIsoCoordToScene(spellGemSpawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject spellGemGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.spellGemPickUpData.spawnObjectPrefab, spellGemSpawnPosition, Quaternion.identity);
			spellGemGo.transform.position = spellGemSpawnPosition;
			SpellGemPickUp spellGemPickUp = spellGemGo.GetComponent<SpellGemPickUp>();
			spellGemPickUp.ReuseSpellGemPickUp (spellGemSpawnPoint.spellData);
			spellGemCount++;
		}
	}
	public void SpawnStaffs (int floorIndex) {

		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];

		foreach (StaffSpawnPoint staffSpawnPoint in currentMapDetails.spawnPoints.staffSpawnPoints) {
			Vector3 staffSpawnPosition = ConvertIsoCoordToScene(staffSpawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject staffPickUpGO = PoolManager.instance.ReuseObject(ConstantsManager.instance.staffPickUpData.spawnObjectPrefab, staffSpawnPosition, Quaternion.identity);
			staffPickUpGO.transform.position = staffSpawnPosition;
			StaffPickUp staffPickUp = staffPickUpGO.GetComponent<StaffPickUp>();
			staffPickUp.SetupObject ();
			staffPickUp.ReuseStaffPickUp (staffSpawnPoint.puzzleData);
		}
	}

	public void DestroyEnvironment (Vector3 centerPosition, int radius) {
		Vector3Int centerCoordinate = grid.WorldToCell(centerPosition) - ((Vector3Int)currentLevelDetails.floorOrigin);
		for (int x = centerCoordinate.x - radius; x < centerCoordinate.x + radius; x++) {
			for (int y = centerCoordinate.y - radius; y < centerCoordinate.y + radius; y++) {
				Vector2Int mapCoordinate = new Vector2Int (x, y);
				if (currentLevelDetails.mapBounds.isWithinBounds (mapCoordinate)) {
					Vector3 coordinatePos = grid.GetCellCenterLocal ((Vector3Int)mapCoordinate);
					MapTileInfo mapTileInfo = currentLevelDetails.mapTileInfo [x, y];
					if (IsometricCoordinateUtilites.IsoDistanceBetweenPoints (centerPosition, coordinatePos) <= radius
					&& mapTileInfo.tileLayer != TileLayer.FLOOR
					&& mapTileInfo.value != zoneData.borderTile.id) {
						Vector3Int coordinate = new Vector3Int (x + currentLevelDetails.floorOrigin.x, y + currentLevelDetails.floorOrigin.y, 0);
						baseTilemap.SetTile (coordinate, null);
						topTilemap.SetTile (coordinate, null);
						mapTileInfo.tileLayer = TileLayer.FLOOR;
						mapTileInfo.walkable = true;
						if (zoneData.destructionDebrisObject != null)
							PoolManager.instance.ReuseObject (zoneData.destructionDebrisObject, coordinatePos, Quaternion.identity);
					}
				}
				else {
					Debug.Log ("LevelManager: Attempting to destroy outside of bounds.");
				}
			}
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

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector2Int worldIsoCoordinate = mapDetails.floorOrigin + new Vector2Int (x, y);
				MapTileInfo tile = mapDetails.mapTileInfo[x, y];

				Vector3 worldPoint = grid.GetCellCenterWorld(new Vector3Int(worldIsoCoordinate.x, worldIsoCoordinate.y, 0));

				bool walkable = tile.walkable;
				if (Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask) && walkable == true) {
					tile.walkable = false;
					walkable = false;
				}

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
	public Vector3 RandomNearbyCoordinate (Vector3 worldPosition, int range) {
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

