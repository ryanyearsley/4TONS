using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[Serializable]
public class SpawnPoints {
	public List <Vector2Int> playerSpawnPoints = new List<Vector2Int>();
	public List <Vector2Int> enemySpawnPoints = new List<Vector2Int>();
	public List <Vector2Int> itemSpawnPoints = new List<Vector2Int>();

}
