﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
	#region Singleton
	public static MapGenerator instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	private ZoneData zoneData;
	private ObjectiveData objectiveData;
	private MapData currentMapData;
	public MapSpawnPoints spawnPoints;

	public SpawnSectorDictionary spawnSectorDictionary = new SpawnSectorDictionary();
	private SpawnSectorInfo fallbackSector = new SpawnSectorInfo(Vector2.one * 0.1f, Vector2.one * 0.9f);
	private LoadingPanelUI loadLog;

	[Header ("Gizmos")]
	public bool drawGizmos;

	[SerializeField]
	private int[,] map;

	private MapTileInfo [,] mapTileInfo;

	void Awake () {
		SingletonInitialization ();
		loadLog = FindObjectOfType<LoadingPanelUI> ();
	}


	public void GenerateMap (ZoneData zoneData, ObjectiveData objectiveData, int floorIndex) {
		this.zoneData = zoneData;
		this.objectiveData = objectiveData;
		this.currentMapData = zoneData.mapDatas [floorIndex];
		map = new int [currentMapData.mapGenerationData.mapSize.x, currentMapData.mapGenerationData.mapSize.y];
		Log ("Generating map..."); 
		Debug.Log("Generating map...");
		RandomFillMap ();
		for (int i = 0; i < currentMapData.mapGenerationData.smoothingIterations; i++) {
			SmoothMap ();
			Log("Smoothing map... " + i);
			Debug.Log("Smoothing map... " + i);
		}
		Log("Processing map...");
		Debug.Log("Processing map...");
		ProcessMap ();

		mapTileInfo = MapConversionUtility.ConvertMapToTileInfo (map);


		Log ("Generating border...");
		Debug.Log("Generating border...");
		GenerateMapBorder (mapTileInfo);
		spawnPoints = new MapSpawnPoints ();
		Vector2Int floorOrigin = floorIndex * Vector2Int.one * 80;
		MapDetails details = new MapDetails(zoneData, currentMapData, floorIndex,  mapTileInfo, spawnPoints);

		CalculateSectors (details);
		int playerCount = 1;
		if (PlayerManager.instance != null) {
			playerCount = PlayerManager.instance.currentPlayers.Count;
		}


		Log ("Generating spawn points...");
		Debug.Log("Generating spawn points...");
		SpawnSectorInfo playerSector = GetSpawnSector(objectiveData.playerSpawnInfo.spawnSector);
		spawnPoints.playerSpawnPoints = SpawnUtility.GenerateCreatureSpawnPoints (details, objectiveData.playerSpawnInfo, playerSector);

		Log ("Generating objectives...");
		Debug.Log("Generating objectives...");
		foreach (SetPieceSpawnInfo objectiveSpawnInfo in objectiveData.objectiveSpawnInfos) {
			SpawnSectorInfo setpieceSector = GetSpawnSector(objectiveSpawnInfo.setPieceSpawnSector);
			spawnPoints.objectiveSpawnPoints.AddRange (SpawnUtility.GenerateSetPieceSpawnPoints (details, objectiveSpawnInfo.setPieceData, objectiveSpawnInfo.spawnCount, setpieceSector, fallbackSector));
		}


		Log ("Generating enemy spawn points...");
		Debug.Log("Generating enemy spawn points...");
		foreach (CreatureSpawnInfo enemySpawnInfo in currentMapData.enemySpawnInfos) {
			if (enemySpawnInfo.spawnCountRange.y != 0) {
				SpawnSectorInfo enemySector = GetSpawnSector(enemySpawnInfo.spawnSector);
				spawnPoints.enemySpawnPoints.AddRange (SpawnUtility.GenerateCreatureSpawnPoints (details, enemySpawnInfo, enemySector));
			}
		}

		//GENERATE PICK-UPS

		Vector2Int floorRollRange = currentMapData.floorRollRange;
		Log ("Rolling for loot...");

		int playerGemRoll = UnityEngine.Random.Range (floorRollRange.x, floorRollRange.y);
		int playerStaffRoll = UnityEngine.Random.Range (floorRollRange.x, floorRollRange.y);

		Log ("Generating spell gem spawn points...");
		List<SpellData> spawningSpellGems = currentMapData.lootTableData.RollForGems (playerGemRoll);
		foreach (SpellData spellData in spawningSpellGems) {
			spawnPoints.spellGemSpawnPoints.AddRange (SpawnUtility.GenerateSpellGemSpawnPoints (details, spellData));
		}

		Log ("Generating staff spawn points...");
		List<PuzzleData> spawningStaves = currentMapData.lootTableData.RollForStaves (playerStaffRoll);
		foreach (PuzzleData staffPuzzleData in spawningStaves) {
			spawnPoints.staffSpawnPoints.AddRange (SpawnUtility.GenerateStaffSpawnPoints (details, staffPuzzleData));
		}

		//CONVERT TO WORLD SKIN
		Log ("Generating setpiece spawn points...");
		if (zoneData.largeSetpieceDatas.Count > 0)
			spawnPoints.objectiveSpawnPoints.AddRange (SpawnUtility.GenerateRandomLargeSetpieceSpawnPoints (details));
		MapGenerationUtility.GenerateRandomSmallSetPieces (details);
		//At this point, map is still 1s and 0s

		//converts base tiles to match zoneData palette.
		List<MapTileInfo> floorTiles = MapConversionUtility.ConvertFloorValueInTileInfo (details, 0, zoneData.primaryFloorTile);
		List<MapTileInfo> baseTiles = MapConversionUtility.ConvertBaseValueInTileInfo (details, 1, zoneData.baseTile);
		List<MapTileInfo> borderTiles = MapConversionUtility.ConvertBaseValueInTileInfo (details, 2, zoneData.borderTile);

		MapGenerationUtility.RandomizeFloor (details, floorTiles);

		Log ("Generating decor...");
		MapGenerationUtility.GenerateFloorDecor (details, floorTiles);
		MapGenerationUtility.ConvertBasesWithoutFloorNeighbors (baseTiles, details);
		//RandomizeFloorTiles
		MapGenerationUtility.GenerateRandomTopDecor (details, baseTiles);

		Log ("Level Generation Success!");
		LevelManager.instance.SetMapDetails (details);

	}

	private void Log (string logLine) {
		if (loadLog != null) {
			loadLog.UpdateLoadingLog (logLine);
		}
	}

	private SpawnSectorInfo GetSpawnSector (SpawnSector sector) {
		if (spawnSectorDictionary.ContainsKey (sector)) {
			return spawnSectorDictionary [sector];
		} else return fallbackSector;
	}


	#region Map Generation VOs
	public struct Coord {
		public int tileX;
		public int tileY;

		public Coord (int tileX, int tileY) {
			this.tileX = tileX;
			this.tileY = tileY;
		}
	}

	public class Room : IComparable<Room> {
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room () {

		}

		public Room (List<Coord> roomTiles, int [,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room> ();

			edgeTiles = new List<Coord> ();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map [x, y] == 1) {
								edgeTiles.Add (tile);
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom () {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom ();
				}
			}
		}

		public static void ConnectRooms (Room roomA, Room roomB) {
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom ();
			} else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom ();
			}
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected (Room otherRoom) {
			return connectedRooms.Contains (otherRoom);
		}

		public int CompareTo (Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
	}
	#endregion
	#region Map Generation Logic
	private void ProcessMap () {
		List<List<Coord>> baseRegions = GetRegions (1);

		//eliminates all clusters of base tiles that are smaller than the minimum island size.
		foreach (List<Coord> baseRegion in baseRegions) {
			if (baseRegion.Count < currentMapData.mapGenerationData.minimumIslandSize) {
				foreach (Coord tile in baseRegion) {
					map [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> floorRegions = GetRegions (0);
		List<Room> survivingRooms = new List<Room> ();


		//Eliminates all floor space that is smaller than the mininumRoomSize.
		foreach (List<Coord> floorRegion in floorRegions) {
			if (floorRegion.Count < currentMapData.mapGenerationData.minimumRoomSize) {
				foreach (Coord tile in floorRegion) {
					map [tile.tileX, tile.tileY] = 1;
				}
			} else {
				survivingRooms.Add (new Room (floorRegion, map));
			}
		}
		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;
		ConnectClosestRooms (survivingRooms);
	}

	private void ConnectClosestRooms (List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {

		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();

		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add (room);
				} else {
					roomListA.Add (room);
				}
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0)
					continue;
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected (roomB))
					continue;

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX - tileB.tileX, 2) + Mathf.Pow (tileA.tileY - tileB.tileY, 2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}

			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms (allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms (allRooms, true);
		}
	}

	private void CreatePassage (Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms (roomA, roomB);

		List<Coord> line = GetLine (tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle (c, currentMapData.mapGenerationData.passageRadius);
		}
	}

	private void DrawCircle (Coord center, int radius) {
		for (int x = -radius; x <= radius; x++) {
			for (int y = -radius; y <= radius; y++) {
				if (x * x + y * y <= radius * radius) {
					int drawX = center.tileX + x;
					int drawY = center.tileY + y;
					if (IsInMapRange (drawX, drawY)) {
						map [drawX, drawY] = 0;
					}
				}
			}
		}
	}

	private List<Coord> GetLine (Coord from, Coord to) {
		List<Coord> line = new List<Coord> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs (dy);
			shortest = Mathf.Abs (dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++) {
			line.Add (new Coord (x, y));

			if (inverted)
				y += step;
			else
				x += step;

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted)
					x += gradientStep;
				else
					y += gradientStep;
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	private Vector3 CoordToWorldPoint (Coord tile) {
		return new Vector3 (
			-currentMapData.mapGenerationData.mapSize.x / 2 + 0.5f + tile.tileX,
			2,
			-currentMapData.mapGenerationData.mapSize.y / 2 + 0.5f + tile.tileY);
	}

	private List<List<Coord>> GetRegions (int tileType) {
		List<List<Coord>> regions = new List<List<Coord>> ();
		int [,] mapFlags = new int [currentMapData.mapGenerationData.mapSize.x, currentMapData.mapGenerationData.mapSize.y];

		for (int x = 0; x < currentMapData.mapGenerationData.mapSize.x; x++) {
			for (int y = 0; y < currentMapData.mapGenerationData.mapSize.y; y++) {
				if (mapFlags [x, y] == 0 && map [x, y] == tileType) {
					List<Coord> newRegion = GetRegionTiles (x,y);
					regions.Add (newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags [tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	private List<Coord> GetRegionTiles (int startX, int startY) {
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[currentMapData.mapGenerationData.mapSize.x, currentMapData.mapGenerationData.mapSize.y];
		int tileType = map[startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue ();
			tiles.Add (tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange (x, y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags [x, y] == 0 && map [x, y] == tileType) {
							mapFlags [x, y] = 1;
							queue.Enqueue (new Coord (x, y));
						}
					}
				}
			}
		}

		return tiles;
	}

	private bool IsInMapRange (int x, int y) {
		return x >= 0 && x < currentMapData.mapGenerationData.mapSize.x && y >= 0 && y < currentMapData.mapGenerationData.mapSize.y;
	}

	private void SmoothMap () {
		for (int x = 0; x < currentMapData.mapGenerationData.mapSize.x; x++) {
			for (int y = 0; y < currentMapData.mapGenerationData.mapSize.y; y++) {
				int neighborWallTiles = GetSurroundingWallCount (x,y);

				if (neighborWallTiles > 4)
					map [x, y] = 1;
				else if (neighborWallTiles < 4)
					map [x, y] = 0;
			}
		}
	}

	private int GetSurroundingWallCount (int gridX, int gridY) {
		int wallCount = 0;
		for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++) {
			for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++) {
				if (IsInMapRange (neighborX, neighborY)) {
					if (neighborX != gridX || neighborY != gridY) {
						wallCount += map [neighborX, neighborY];
					}
				} else {
					wallCount++;
				}
			}
		}

		return wallCount;
	}

	//MapData
	//mapSize
	//mapGenerationData
	private void RandomFillMap () {
		if (!currentMapData.useCustomSeed)
			currentMapData.seed = System.DateTime.Now.Ticks.ToString ();

		System.Random pseudoRandom = new System.Random (currentMapData.seed.GetHashCode ());
		for (int x = 0; x < currentMapData.mapGenerationData.mapSize.x; x++) {
			for (int y = 0; y < currentMapData.mapGenerationData.mapSize.y; y++) {
				if (x == 0 || x == currentMapData.mapGenerationData.mapSize.x - 1 || y == 0 || y == currentMapData.mapGenerationData.mapSize.y - 1) {
					map [x, y] = 1;
				} else {
					map [x, y] = (pseudoRandom.Next (0, 100) < currentMapData.mapGenerationData.randomFillPercent) ? 1 : 0;
				}
			}
		}
	}
	private void GenerateMapBorder (MapTileInfo [,] mapTileInfo) {
		int xLength = mapTileInfo.GetLength(0);
		int yLength = mapTileInfo.GetLength(1);
		for (int x = 0; x < xLength; x++) {
			for (int y = 0; y < yLength; y++) {
				if (x == 0 || x == xLength - 1 || y == 0 || y == yLength - 1) {
					mapTileInfo [x, y].baseValue = 2;
					mapTileInfo [x, y].isSpawnConflict = true;
				}
			}
		}
	}

	private void CalculateSectors (MapDetails details) {
		Vector2Int maxCoord = details.mapBounds.maxCoord;
		foreach (SpawnSectorInfo sector in spawnSectorDictionary.Values) {
			Vector2 minCoordRaw = sector.minNormalized * maxCoord;
			Vector2 maxCoordRaw = sector.maxNormalized * maxCoord;
			sector.minCoord = new Vector2Int (Mathf.RoundToInt (minCoordRaw.x), Mathf.RoundToInt (minCoordRaw.y));
			sector.maxCoord = new Vector2Int (Mathf.RoundToInt (maxCoordRaw.x), Mathf.RoundToInt (maxCoordRaw.y));
		}
		Vector2 fallbackMinRaw = fallbackSector.minNormalized * maxCoord;
		Vector2 fallbackMaxRaw = fallbackSector.maxNormalized * maxCoord;
		fallbackSector.minCoord = new Vector2Int (Mathf.RoundToInt (fallbackMinRaw.x), Mathf.RoundToInt (fallbackMinRaw.y));
		fallbackSector.maxCoord = new Vector2Int (Mathf.RoundToInt (fallbackMaxRaw.x), Mathf.RoundToInt (fallbackMaxRaw.y));
	}

	#endregion


	private void OnDrawGizmosSelected () {
		if (map != null && drawGizmos) {
			for (int x = 0; x < currentMapData.mapGenerationData.mapSize.x; x++) {
				for (int y = 0; y < currentMapData.mapGenerationData.mapSize.y; y++) {
					Vector3 position = new Vector3 (-currentMapData.mapGenerationData.mapSize.x / 2 + x + 0.5f, -currentMapData.mapGenerationData.mapSize.y / 2 + y + 0.5f, 0);
					int tileIndex = map[x, y];
					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
					Gizmos.DrawCube (position, Vector3.one);
				}
			}
		}
	}
}