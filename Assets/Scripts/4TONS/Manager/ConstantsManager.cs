using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstantsManager : PersistentManager {

	public GameDataLegend legend;
	public SpellSchoolData[] spellSchools;
	public ZoneData[] zones;
	public ObjectiveData[] objectives;

	public ObjectRegistry objectRegistry;
	//PLAYER CREATION PREFABS

	//Whenever a player is spawned, this agnostic wizard prefab is used...
	//It is then overwritten by player's WizardSaveData (assigned in PlayerManager).
	public CreatureData playerCreatureData;
	public WizardPrebuildData defaultWizardData;



	//STAFF AND PUZZLE PREFABS


	//USED IN MAP GENERATION
	public SpawnObjectData spellGemPickUpData;
	public SpawnObjectData staffPickUpData;

	//USED IN PLAYER PICK-UP/DROP
	public GameObject spellGemUIPrefab;
	public GameObject puzzleEntityPrefab;

	public Tile puzzleTile;
	public Tile baseHitboxTile;
	public RandomNameData randomWizardNames;

	public Color validProjectedAoEColor;
	public Color invalidProjectedAoEColor;


	#region Singleton
	public static ConstantsManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	protected override void Awake () {
		base.Awake ();
		SingletonInitialization ();
		MapObjectIDsToLegend ();//sets indexes on all objects using the world data and legend.
		objectRegistry = new ObjectRegistry (spellSchools, zones, objectives, legend);//registers objects by their assigned ID into dictionaries.
	}

	public void MapObjectIDsToLegend () {
		Dictionary<SpellSchool, int> schoolIndexStartDictionary = new Dictionary<SpellSchool, int>();
		schoolIndexStartDictionary.Add (SpellSchool.Generic, legend.GENERIC_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Light, legend.LIGHT_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Dark, legend.DARK_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Fire, legend.FIRE_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Ice, legend.ICE_INDEX_START);

		//assigns unique index to all spell data objects.
		foreach (SpellSchoolData spellSchoolData in spellSchools) {
			int schoolIndexStart = schoolIndexStartDictionary [spellSchoolData.spellSchool];
			spellSchoolData.schoolIndexStart = schoolIndexStart;

			for (int i = 0; i < spellSchoolData.spells.Count; i++) {
				spellSchoolData.spells [i].id = schoolIndexStart + legend.SPELL_INDEX_START + i;
			}
			for (int i = 0; i < spellSchoolData.staffs.Length; i++) {
				spellSchoolData.staffs [i].id = schoolIndexStart + legend.STAFF_INDEX_START + i + 1;//+1 to offset from startingStaff
			}
		}
		
		int objectiveCount = 100;
		foreach (ObjectiveData objectiveData in objectives) {
			foreach (SetPieceSpawnInfo objSpawnInfo in objectiveData.objectiveSpawnInfos) {
				objSpawnInfo.setPieceData.id = objectiveCount++;
			}
		}


		Dictionary<Zone, int> zoneIndexStartDictionary = new Dictionary<Zone, int>();
		zoneIndexStartDictionary.Add (Zone.Generic, legend.GENERIC_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Hub, legend.HUB_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Light, legend.LIGHT_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Dark, legend.DARK_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Fire, legend.FIRE_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Ice, legend.ICE_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Tutorial, legend.TUTORIAL_INDEX_START);
		//assign unique index to all worlddata objects
		foreach (ZoneData zoneData in zones) {
			int zoneIndexStart = zoneIndexStartDictionary [zoneData.zone];
			zoneData.schoolIndexStart = zoneIndexStart;
			zoneData.primaryFloorTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.FLOOR_TILE_INDEX_START;
			zoneData.primaryFloorTile.layer = TileLayer.FLOOR;
			zoneData.secondaryFloorTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.FLOOR_TILE_INDEX_START + 1;
			zoneData.secondaryFloorTile.layer = TileLayer.FLOOR;
			zoneData.underTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.FLOOR_TILE_INDEX_START + 2;
			zoneData.underTile.layer = TileLayer.FLOOR;

			zoneData.baseTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.BASE_TILE_INDEX_START;
			zoneData.baseTile.layer = TileLayer.BASE;

			zoneData.baseBlankTopTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.BASE_BLANKTOP_TILE_INDEX_START;
			zoneData.baseBlankTopTile.layer = TileLayer.BASE;

			zoneData.borderTile.id = zoneIndexStart + legend.TILE_INDEX_START + legend.BORDER_TILE_INDEX_START;
			zoneData.borderTile.layer = TileLayer.BASE;

			for (int i = 0; i < zoneData.baseDecorTiles.Count; i++) {
				TileData tileData = zoneData.baseDecorTiles[i];
				tileData.id = zoneIndexStart + legend.TILE_INDEX_START + legend.BASE_DECOR_TILE_INDEX_START + i;
				tileData.layer = TileLayer.BASE;
			}
			if (zoneData.floorData != null) {
				for (int i = 0; i < zoneData.floorData.tiles.Count; i++) {
					TileData tileData = zoneData.floorData.tiles[i];
					tileData.id = zoneIndexStart + legend.TILE_INDEX_START + legend.FLOORS_TILE_INDEX_START + i;
					tileData.layer = TileLayer.FLOOR;
				}
			}

			for (int i = 0; i < zoneData.randomFloorDecorTiles.Count; i++) {
				TileData tileData = zoneData.randomFloorDecorTiles[i];
				tileData.id = zoneIndexStart + legend.TILE_INDEX_START + legend.FLOOR_DECOR_TILE_INDEX_START + i;
				tileData.layer = TileLayer.FLOOR;
			}
			for (int i = 0; i < zoneData.surroundingDecorTiles.Count; i++) {
				TileData tileData = zoneData.surroundingDecorTiles[i];
				tileData.id = zoneIndexStart + legend.TILE_INDEX_START + legend.SURROUNDING_FLOOR_DECOR_TILE_INDEX_START + i;
				tileData.layer = TileLayer.FLOOR;
			}

			for (int i = 0; i < zoneData.topDecorTiles.Count; i++) {
				TileData tileData = zoneData.topDecorTiles[i];
				tileData.id = zoneIndexStart + legend.TILE_INDEX_START + legend.TOP_DECOR_TILE_INDEX_START + i;
				tileData.layer = TileLayer.BASE;
			}


			for (int i = 0; i < zoneData.enemyDatas.Count; i++) {
				zoneData.enemyDatas [i].id = zoneIndexStart + legend.ENEMY_SPAWN_INDEX_START + i;
			}
			for (int i = 0; i < zoneData.largeSetpieceDatas.Count; i++) {
				zoneData.largeSetpieceDatas [i].id = zoneIndexStart + legend.SETPIECE_INDEX_START + i;
			}
		}
	}

	public SpellSchoolData GetSpellSchoolData (int id) {
		if (objectRegistry.schoolDataDictionary.TryGetValue (id, out SpellSchoolData spellSchoolData)) {
			return spellSchoolData;
		} else return null;
	}
	public SpellData GetSpellData (int id) {
		if (objectRegistry.activeSpellDictionary.TryGetValue (id, out SpellData spellData)) {
			return spellData;
		} else return null;
	}
	public PuzzleData GetPuzzleData (int id) {
		if (objectRegistry.activePuzzleDictionary.TryGetValue (id, out PuzzleData puzzleData)) {
			return puzzleData;
		} else return null;
	}


}
public enum Zone {
	Light, Dark, Fire, Ice, Generic, Tutorial, Hub, Test
}

public enum SpellSchool {
	Light, Dark, Fire, Ice, Generic
}
