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

	private MapDetails currentLevelDetails;

	[SerializeField]
	private WorldData worldData;
	[SerializeField]
	private GameDataLegend legend;
	[SerializeField]
	private ObjectRegistry objectRegistry;

	[SerializeField]
	private Tilemap tilemap;

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
		this.worldData = gameContext.worldData;
		this.legend = ConstantsManager.instance.legend;
		CreateLevelPools (worldData);

	}
	private void CreateLevelPools (WorldData worldData) {
		//PoolManager.instance.CreatePoolGeneric (worldData.playerData.spawnObjectPrefab, worldData.playerData.poolSize);
		//objective pools
		PoolManager.instance.CreateObjectPool (worldData.playerSpawnSetpieceSpawnInfo.setPieceData.spawnObjectPrefab, worldData.playerSpawnSetpieceSpawnInfo.setPieceData.poolSize);
		PoolManager.instance.CreateObjectPool (worldData.nextLevelPortalSpawnInfo.setPieceData.spawnObjectPrefab, worldData.nextLevelPortalSpawnInfo.setPieceData.poolSize);
		//player pools
		PoolManager.instance.CreatePlayerPool (ConstantsManager.instance.playerWizardTemplatePrefab, 4);
		
		//creature pools
		foreach (CreatureData enemyData in worldData.enemyDatas) {
			PoolManager.instance.CreateCreaturePool (enemyData.spawnObjectPrefab, enemyData.poolSize);
		}
		PoolManager.instance.CreateObjectPool (ConstantsManager.instance.spellGemPickupPrefab, 15);
	}

	#endregion
	#region events/subscriptions


	public void OnLoadLevel (int levelIndex) {
		StartCoroutine (LoadLevelRoutine (levelIndex));
	}
	public void OnLevelEnd(int levelIndex) {

	}
	#endregion
	#region public utility methods

	public IEnumerator LoadLevelRoutine(int levelIndex) {
		GenerateMapDetails (worldData, levelIndex);
		yield return new WaitForSeconds (0.5f);
		BuildFloor (levelIndex);
		yield return new WaitForSeconds (0.5f);
		if (levelIndex == 0)
			CreatePlayerObjects ();
		else
			MovePlayerToNextStage (levelIndex);

		yield return new WaitForSeconds (0.5f);
		SpawnSpellGems (levelIndex);
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
		return tilemap.GetCellCenterWorld ((Vector3Int)cartCoordinate);
	}
	public void GenerateMapDetails (WorldData worldData, int floorIndex) {
		if (floorIndex < worldData.mapDatas.Length) {
			MapData nextMapData = worldData.mapDatas[floorIndex];
			MapDetails generatingMapDetails = null;
			if (nextMapData.isGenerated) {
				generatingMapDetails = MapGenerator.instance.GenerateMap (worldData, legend, floorIndex);
			} else if (nextMapData.mapFile != null) {
				int[,] map = MapUtility.DeserializeLevelFile (nextMapData.mapFile);
				generatingMapDetails = new MapDetails (worldData, nextMapData, floorIndex, MapUtility.ConvertMapToTileInfo (map), null);
			}
			mapDetailDictionary.Add (floorIndex, generatingMapDetails);
			currentLevelDetails = generatingMapDetails;
		}
	}

	public void BuildFloor (int floorIndex) {
		if (mapDetailDictionary.ContainsKey (floorIndex)) {
			MapDetails buildingMapDetails = mapDetailDictionary[floorIndex];
			Tile[] tileSet = worldData.tileset.ToArray ();
			MapTileInfo[,] mapTileInfo = buildingMapDetails.mapTileInfo;
			int yLength = mapTileInfo.GetLength(1);
			int xLength = mapTileInfo.GetLength(0);
			Vector2Int origin = buildingMapDetails.floorOrigin;
			Vector3 floorPrefabSpawnPosition = tilemap.GetCellCenterWorld((Vector3Int)origin);
			Instantiate (buildingMapDetails.mapData.floorPrefab, floorPrefabSpawnPosition, Quaternion.identity); ;
			Transform parent = tilemap.transform.parent;
			GameObject tilemapObject = tilemap.transform.gameObject;
			for (int y = 0; y < mapTileInfo.GetLength (1); y++) {
				for (int x = 0; x < mapTileInfo.GetLength (0); x++) {
					int tileValue = mapTileInfo[x, y].value;
					if (objectRegistry.activeTileDictionary.ContainsKey (tileValue)) {
						tilemap.SetTile (new Vector3Int (x + origin.x, y + origin.y, 0), objectRegistry.activeTileDictionary [tileValue]);
						buildingMapDetails.mapTileInfo [x, y].walkable = false;
					} else if (objectRegistry.activeSetpieceDictionary.ContainsKey (tileValue)) {
						Vector3 position = tilemap.GetCellCenterWorld (new Vector3Int (x + origin.x, y + origin.y, 0));
						PoolManager.instance.ReuseObject (objectRegistry.activeSetpieceDictionary [tileValue].spawnObjectPrefab, position, Quaternion.identity);
						
					}
				}
			}
			GenerateFloorNodes (buildingMapDetails);
		}
	}


	//NOTE: Players are only spawned on first stage. They are relocated to following stages.
	public void CreatePlayerObjects () {
		MapDetails currentMapDetails = mapDetailDictionary[0];
		foreach (Player player in PlayerManager.instance.currentPlayers) {
			Vector2Int spawnCoordinate = currentMapDetails.spawnPoints.playerSpawnPoints[player.playerIndex].spawnCoordinate;
			Vector3 spawnPosition = ConvertIsoCoordToScene(spawnCoordinate);
			GameObject playerObject = PoolManager.instance.ReusePlayerObject(spawnPosition, player);
			player.isAlive = true;
			UIManager.Instance.playerUIs [player.playerIndex].InitializePlayerUI (player);
			if (PlayerManager.instance.currentPlayers.Count == 1) {
				PlayerManagement.CameraController.instance.SetCameraDynamic (playerObject.GetComponent<MovementComponent> ());
			}
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
			currentMapDetails.remainingEnemies.Add (enemyObject.GetComponent<GauntletObjectiveComponent>());
		}
		currentMapDetails.totalEnemiesCount = currentMapDetails.remainingEnemies.Count;
	}

	public void SpawnSpellGems (int floorIndex) {

		MapDetails currentMapDetails = mapDetailDictionary[floorIndex];
		int spellGemCount = 0;

		foreach (SpellGemSpawnPoint spellGemSpawnPoint in currentMapDetails.spawnPoints.spellGemSpawnPoints) {
			Vector3 spellGemSpawnPosition = ConvertIsoCoordToScene(spellGemSpawnPoint.spawnCoordinate + currentMapDetails.floorOrigin);
			GameObject spellGemGo = PoolManager.instance.ReuseObject(ConstantsManager.instance.spellGemPickupPrefab, spellGemSpawnPosition, Quaternion.identity);
			spellGemGo.transform.position = spellGemSpawnPosition;
			SpellGemPickup spellGemPickUp = spellGemGo.GetComponent<SpellGemPickup>();
			spellGemPickUp.ReuseSpellGemPickUp (spellGemSpawnPoint.spellData);
			spellGemCount++;
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
				//bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius, unwalkableMask));

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

	void OnDrawGizmos () {
		if (nodes != null && displayGridGizmos) {

			Vector3 gridPosition = grid.GetCellCenterWorld(Vector3Int.one);

			Gizmos.DrawCube (gridPosition, Vector3.one * nodeRadius * 2);
			for (int x = 0; x < nodes.GetLength (0); x++) {
				for (int y = 0; y < nodes.GetLength (1); y++) {
					PathfindingNode node = nodes[x, y];
					//Debug.Log (" map coord: " + new Vector2Int(x, y) + ", world position: " + node.worldPosition);
					if (currentLevelDetails.mapTileInfo [x,y].isSpawnConflict) {
						Debug.Log ("level manager gizmo: spawn conflict at tile");
						Gizmos.color = Color.red;
						Gizmos.DrawCube (node.worldPosition, Vector3.one * (nodeRadius * 2));
					}
					else {
						Gizmos.color = (node.walkable) ? Color.white : Color.black;
						Gizmos.DrawCube (node.worldPosition, Vector3.one * (nodeRadius * 2));
					}
				}
			}
		}
	}
	#endregion
}

