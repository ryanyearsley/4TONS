using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstantsManager : MonoBehaviour {

	public GameDataLegend legend;
	public SpellSchoolData[] spellSchools;

	public ObjectRegistry objectRegistry;
	//PLAYER CREATION PREFABS
	public GameObject playerWizardTemplatePrefab;



	//STAFF AND PUZZLE PREFABS
	public GameObject spellGemPickupPrefab;
	public GameObject spellGemUIPrefab;
	public GameObject staffPickupPrefab;
	public GameObject puzzleEntityPrefab;
	public Tile puzzleTile;

	public RandomNameData randomWizardNames;

	// Singleton Pattern to access this script with ease
	#region Singleton
	public static ConstantsManager instance;
	void SingletonInitialization() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	void Awake () {
		SingletonInitialization ();
		MapObjectIDsToLegend ();//sets indexes on all objects using the world data and legend.
		objectRegistry = new ObjectRegistry (spellSchools, legend);//registers objects by their assigned ID into dictionaries.
	}

	public void MapObjectIDsToLegend () {
		Dictionary<SpellSchool, int> schoolIndexStartDictionary = new Dictionary<SpellSchool, int>();
		schoolIndexStartDictionary.Add (SpellSchool.Light, legend.LIGHT_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Dark, legend.DARK_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Ice, legend.ICE_INDEX_START);
		schoolIndexStartDictionary.Add (SpellSchool.Fire, legend.FIRE_INDEX_START);

		foreach (SpellSchoolData spellSchoolData in spellSchools) {
			int schoolIndexStart = schoolIndexStartDictionary [spellSchoolData.spellSchool];
			spellSchoolData.schoolIndexStart = schoolIndexStart;

			WorldData worldData = spellSchoolData.worldData;
			worldData.schoolIndexStart = schoolIndexStart;

			for (int i = 0; i < spellSchoolData.spells.Count; i++) {
				spellSchoolData.spells [i].id = schoolIndexStart + legend.SPELLGEM_BLOCK_SPAWN_INDEX_START + i;
			}
			//worldData.playerData.id = schoolIndexStart + gameDataLegend.PLAYER_ONE_SPAWN_INDEX;
			worldData.playerSpawnSetpieceSpawnInfo.setPieceData.id = schoolIndexStart + legend.PLAYER_ONE_SPAWN_INDEX;
			worldData.nextLevelPortalSpawnInfo.setPieceData.id = schoolIndexStart + legend.NEXT_LEVEL_PORTAL_INDEX;

			for (int i = 0; i < worldData.enemyDatas.Count; i++) {
				worldData.enemyDatas [i].id = schoolIndexStart + legend.ENEMY_SPAWN_INDEX_START + i;
			}
			for (int i = 0; i < worldData.setPieceDatas.Count; i++) {
				worldData.setPieceDatas [i].id = schoolIndexStart + legend.MULTI_TILE_SETPIECE_INDEX_START + i;
			}

			for (int i = 0; i < worldData.spellSchoolData.schoolStaffs.Length; i++) {
				worldData.spellSchoolData.schoolStaffs [i].id = schoolIndexStart + legend.PUZZLEDATA_INDEX_START + i + 1;//+1 to offset from startingStaff
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
