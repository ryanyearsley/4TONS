using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapDetails
{
    public int floorIndex;
    public bool stageComplete;
    public Vector2Int floorOrigin;

    //coordinates on grid (think world space)
    public CoordinateBounds gridBounds;
    //coordinates on map (think local space)
    public CoordinateBounds mapBounds;
    public WorldData worldData;
    public MapData mapData;
    public MapTileInfo[,] mapTileInfo;
    public MapSpawnPoints spawnPoints;


    public List<AIStateController> remainingEnemies = new List<AIStateController>();
    public int totalEnemiesCount;

    public MapDetails (WorldData worldData, MapData mapData, int floorIndex, MapTileInfo [,] mapTileInfo, MapSpawnPoints spawnPoints) {
        this.worldData = worldData;
        this.mapData = mapData;
        this.floorIndex = floorIndex;
        this.floorOrigin = floorIndex * Vector2Int.one * 80;
        this.mapTileInfo = mapTileInfo;
        this.spawnPoints = spawnPoints;
        GenerateBounds ();
    }

    public void GenerateBounds () {
        Vector2Int maxGridCoords = new Vector2Int(floorOrigin.x + mapTileInfo.GetLength(0) - 1, floorOrigin.y + mapTileInfo.GetLength(1) - 1);
        this.gridBounds = new CoordinateBounds(floorOrigin, maxGridCoords);
        Vector2Int maxMapCoords = new Vector2Int (mapTileInfo.GetLength (0) - 1, mapTileInfo.GetLength (1) - 1);
        this.mapBounds = new CoordinateBounds (Vector2Int.zero, maxMapCoords);
    }
}
