using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//houses all dictionaries of active objects in the scene.
[Serializable]
public class ObjectRegistry {

	public SchoolDataDictionary schoolDataDictionary = new SchoolDataDictionary();
	public ActiveTileDictionary activeTileDictionary = new ActiveTileDictionary();
	public ActiveObjectDictionary activeSetpieceDictionary = new ActiveObjectDictionary();
	public ActiveObjectDictionary activeCreatureDictionary = new ActiveObjectDictionary();
	public ActiveSpellDictionary activeSpellDictionary = new ActiveSpellDictionary();
	public PuzzleDataDictionary activePuzzleDictionary = new PuzzleDataDictionary();
	public ObjectRegistry (SpellSchoolData[] spellSchoolDatas, GameDataLegend legend) {
		
			foreach (SpellSchoolData schoolData in spellSchoolDatas){
			schoolDataDictionary.Add (schoolData.schoolIndexStart, schoolData);
			WorldData worldData = schoolData.worldData;
			RegisterTiles (worldData, legend);
			RegisterEnemies (worldData, legend);
			RegisterSetPieces (worldData);
			RegisterSpells (worldData.spellSchoolData);
			RegisterPuzzles (worldData.spellSchoolData);
		}
	}

	#region dictionary setup

	private void RegisterTiles (WorldData worldData, GameDataLegend legend) {
		int indexStart = worldData.schoolIndexStart + legend.TILE_INDEX_START;
		for (int i = 0; i < worldData.tileset.Count; i++) {
			int index = indexStart + i;
			activeTileDictionary.Add (index, worldData.tileset [i]);
		}
	}
	private void RegisterEnemies (WorldData worldData, GameDataLegend legend) {
		int enemyIndexStart = worldData.schoolIndexStart + legend.ENEMY_SPAWN_INDEX_START;
		for (int i = 0; i < worldData.enemyDatas.Count; i++) {
			CreatureData enemyData = worldData.enemyDatas[i];
			activeCreatureDictionary.Add (enemyData.id, enemyData);
		}
	}
	private void RegisterSetPieces (WorldData worldData) {
		activeSetpieceDictionary.Add (worldData.playerSpawnSetpieceSpawnInfo.setPieceData.id, worldData.playerSpawnSetpieceSpawnInfo.setPieceData);
		activeSetpieceDictionary.Add (worldData.nextLevelPortalSpawnInfo.setPieceData.id, worldData.nextLevelPortalSpawnInfo.setPieceData);
		for (int i = 0; i < worldData.setPieceDatas.Count; i++) {
			SetPieceData setPieceData = worldData.setPieceDatas[i];
			activeSetpieceDictionary.Add (setPieceData.id, setPieceData);
		}
	}

	private void RegisterSpells (SpellSchoolData spellSchoolData) {
		for (int i = 0; i < spellSchoolData.spells.Count; i++) {
			SpellData spellData = spellSchoolData.spells[i];
			activeSpellDictionary.Add (spellData.id, spellData);
		}
	}
	private void RegisterPuzzles (SpellSchoolData spellSchoolData) {
		for (int i = 0; i < spellSchoolData.schoolStaffs.Length; i++) {
			PuzzleData puzzleData = spellSchoolData.schoolStaffs[i];
			activePuzzleDictionary.Add (puzzleData.id, puzzleData);
		}
	}
	#endregion
}
