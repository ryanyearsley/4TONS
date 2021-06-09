using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstantsManager : PersistentManager {

	public GameDataLegend legend;
	public SpellSchoolData[] spellSchools;
	public WorldData[] worlds;

	public ObjectRegistry objectRegistry;
	//PLAYER CREATION PREFABS
	public GameObject playerWizardTemplatePrefab;



	//STAFF AND PUZZLE PREFABS

	public SpawnObjectData spellGemPickUpData;
	public SpawnObjectData staffPickUpData;
	public GameObject spellGemPickupPrefab;
	public GameObject spellGemUIPrefab;
	public GameObject staffPickupPrefab;
	public GameObject puzzleEntityPrefab;
	public Tile puzzleTile;

	public RandomNameData randomWizardNames;

	public Color validProjectedAoEColor;
	public Color invalidProjectedAoEColor;


	// Singleton Pattern to access this script with ease
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
		objectRegistry = new ObjectRegistry (spellSchools, worlds, legend);//registers objects by their assigned ID into dictionaries.
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


		Dictionary<Zone, int> zoneIndexStartDictionary = new Dictionary<Zone, int>();
		zoneIndexStartDictionary.Add (Zone.Generic, legend.GENERIC_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Light, legend.LIGHT_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Dark, legend.DARK_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Fire, legend.FIRE_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Ice, legend.ICE_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Hub, legend.HUB_INDEX_START);
		zoneIndexStartDictionary.Add (Zone.Tutorial, legend.TUTORIAL_INDEX_START);

		//assign unique index to all worlddata objects
		foreach (WorldData worldData in worlds) {
			int zoneIndexStart = zoneIndexStartDictionary [worldData.zone];
			worldData.schoolIndexStart = zoneIndexStart;
			worldData.playerSpawnSetpieceSpawnInfo.setPieceData.id = zoneIndexStart + legend.PLAYER_ONE_SPAWN_INDEX;
			worldData.nextLevelPortalSpawnInfo.setPieceData.id = zoneIndexStart + legend.NEXT_LEVEL_PORTAL_INDEX;

			for (int i = 0; i < worldData.enemyDatas.Count; i++) {
				worldData.enemyDatas [i].id = zoneIndexStart + legend.ENEMY_SPAWN_INDEX_START + i;
			}
			for (int i = 0; i < worldData.setPieceDatas.Count; i++) {
				worldData.setPieceDatas [i].id = zoneIndexStart + legend.SETPIECE_INDEX_START + i;
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
	Light, Dark, Fire, Ice, Generic, Tutorial, Hub
}

public enum SpellSchool {
	Light, Dark, Fire, Ice, Generic
}
