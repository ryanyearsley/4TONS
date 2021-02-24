using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[Serializable]
public class MapSpawnPoints {

	public SpawnPoint playerSpawnSetPiecePoint;
	public SpawnPoint nextPortalSpawnPoint;
	public List <SpawnPoint> playerSpawnPoints = new List<SpawnPoint>();
	public List <SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();
	public List<SpellGemSpawnPoint> spellGemSpawnPoints = new List<SpellGemSpawnPoint>();
}
