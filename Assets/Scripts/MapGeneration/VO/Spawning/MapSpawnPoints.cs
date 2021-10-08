using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[Serializable]
public class MapSpawnPoints {

	public List<SpawnPoint> objectiveSpawnPoints = new List<SpawnPoint>();
	public List <SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
	public List <SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();
	public List<SpellGemSpawnPoint> spellGemSpawnPoints = new List<SpellGemSpawnPoint>();
	public List <StaffSpawnPoint> staffSpawnPoints = new List<StaffSpawnPoint>();
	public List<SpawnPoint> setPieceSpawnPoints = new List<SpawnPoint>();
}
