﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public MapData mapData;
	public SpawnPoints spawnPoints;

	[Header ("Gizmos")]
	public bool drawGizmos;

	private int[,] map;

	private void Awake () {
		GenerateMap ();
	}

	private void Update () {
		if (Input.GetMouseButtonDown (0))
			GenerateMap ();
	}

	private void GenerateMap () {
		map = new int [mapData.mapSize.x, mapData.mapSize.y];
		spawnPoints = new SpawnPoints ();
		RandomFillMap ();

		for (int i = 0; i < mapData.smoothingIterations; i++) {
			SmoothMap ();
		}

		ProcessMap ();

		int[,] borderedMap = new int[mapData.mapSize.x + mapData.borderSize * 2, mapData.mapSize.y + mapData.borderSize * 2];
		for (int x = 0; x < borderedMap.GetLength (0); x++) {
			for (int y = 0; y < borderedMap.GetLength (1); y++) {
				if (x >= mapData.borderSize && x < mapData.mapSize.x + mapData.borderSize && y >= mapData.borderSize && y < mapData.mapSize.y + mapData.borderSize) {
					borderedMap [x, y] = map [x - mapData.borderSize, y - mapData.borderSize];

				} else {
					borderedMap [x, y] = 1;
				}
			}
		}

		spawnPoints.playerSpawnPoints = GenerateSpawnPoints (mapData.playerCount, 101);
		spawnPoints.enemySpawnPoints = GenerateSpawnPoints (mapData.enemyCount, 102);
		spawnPoints.itemSpawnPoints = GenerateSpawnPoints (mapData.itemCount, 103);

		LevelFactory.BuildLevel (map, mapData.tileset.tilePrefabs.ToArray ());
	}
	private float CalculateSpawnPointProbability (Vector2 gridSize, float fillRate, int spawnCount) {
		float probability = new float();

		return probability;
	}

	private void ProcessMap () {
		List<List<Coord>> wallRegions = GetRegions (1);

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < mapData.minimumIslandSize) {
				foreach (Coord tile in wallRegion) {
					map [tile.tileX, tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions (0);
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < mapData.minimumRoomSize) {
				foreach (Coord tile in roomRegion) {
					map [tile.tileX, tile.tileY] = 1;
				}
			} else {
				survivingRooms.Add (new Room (roomRegion, map));
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
			DrawCircle (c, mapData.passageRadius);
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
			-mapData.mapSize.x / 2 + 0.5f + tile.tileX,
			2,
			-mapData.mapSize.y / 2 + 0.5f + tile.tileY);
	}

	private List<List<Coord>> GetRegions (int tileType) {
		List<List<Coord>> regions = new List<List<Coord>> ();
		int [,] mapFlags = new int [mapData.mapSize.x, mapData.mapSize.y];

		for (int x = 0; x < mapData.mapSize.x; x++) {
			for (int y = 0; y < mapData.mapSize.y; y++) {
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
		int[,] mapFlags = new int[mapData.mapSize.x, mapData.mapSize.y];
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
		return x >= 0 && x < mapData.mapSize.x && y >= 0 && y < mapData.mapSize.y;
	}

	private void SmoothMap () {
		for (int x = 0; x < mapData.mapSize.x; x++) {
			for (int y = 0; y < mapData.mapSize.y; y++) {
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

	private void RandomFillMap () {
		if (!mapData.useCustomSeed)
			mapData.seed = System.DateTime.Now.Ticks.ToString ();

		System.Random pseudoRandom = new System.Random (mapData.seed.GetHashCode ());
		for (int x = 0; x < mapData.mapSize.x; x++) {
			for (int y = 0; y < mapData.mapSize.y; y++) {
				if (x == 0 || x == mapData.mapSize.x - 1 || y == 0 || y == mapData.mapSize.y - 1) {
					map [x, y] = 1;
				} else {
					map [x, y] = (pseudoRandom.Next (0, 100) < mapData.randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	private List<Vector2Int> GenerateSpawnPoints (int spawnCount, int spawnIndex) {
		List<Vector2Int> spawnPointCoords = new List<Vector2Int>();
		for (int i = 0; i < spawnCount; i++) {
			bool spawnPointAdded = false;
			while (spawnPointAdded == false) {
				int randomX = UnityEngine.Random.Range (1, mapData.mapSize.x - 1);
				int randomY = UnityEngine.Random.Range (1, mapData.mapSize.y - 1);
				if (map [randomX, randomY] == 0) {//valid empty slot. 
					spawnPointCoords.Add (new Vector2Int (randomX, randomY));
					map [randomX, randomY] = spawnIndex;
					spawnPointAdded = true;
				}
			}
		}
		return spawnPointCoords;
	}

	private void OnDrawGizmos () {
		if (map != null && drawGizmos) {
			for (int x = 0; x < mapData.mapSize.x; x++) {
				for (int y = 0; y < mapData.mapSize.y; y++) {
					Vector3 position = new Vector3 (-mapData.mapSize.x / 2 + x + 0.5f, -mapData.mapSize.y / 2 + y + 0.5f, 0);
					int tileIndex = map[x, y];
					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
					Gizmos.DrawCube (position, Vector3.one);
				}
			}
		}
	}

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
}