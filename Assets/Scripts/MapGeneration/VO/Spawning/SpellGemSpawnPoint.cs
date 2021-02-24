using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemSpawnPoint : SpawnPoint
{
	public SpellData spellData;

	public SpellGemSpawnPoint (Vector2Int coord, SpawnObjectData spawnObjectData, SpellData spellData) : base (coord, null) {
		this.spawnCoordinate = coord;
		this.spawnObjectData = spawnObjectData;
		this.spellData = spellData;
		this.value = spellData.id;
	}
}
