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
	public ObjectRegistry (PuzzleData[] inventoryTemplates, SpellSchoolData [] spellSchoolDatas, ZoneData [] zones, ObjectiveData [] objectives, GameDataLegend legend) {

		RegisterPuzzles (inventoryTemplates);

		foreach (SpellSchoolData schoolData in spellSchoolDatas) {
			schoolDataDictionary.Add (schoolData.schoolIndexStart, schoolData);
			RegisterSpells (schoolData);
			RegisterPuzzles (schoolData.staffs);
		}

		foreach (ZoneData zoneData in zones) {
			RegisterTiles (zoneData);
			RegisterEnemies (zoneData, legend);
			RegisterSetPieces (zoneData);
		}

		foreach (ObjectiveData objectiveData in objectives) {
			RegisterObjectivePieces (objectiveData);
		}
	}

	#region dictionary setup

	private void RegisterTiles (ZoneData zoneData) {

		activeTileDictionary.Add (zoneData.primaryFloorTile.id, zoneData.primaryFloorTile.tile);
		activeTileDictionary.Add (zoneData.secondaryFloorTile.id, zoneData.secondaryFloorTile.tile);
		activeTileDictionary.Add (zoneData.underTile.id, zoneData.underTile.tile);
		activeTileDictionary.Add (zoneData.baseTile.id, zoneData.baseTile.tile);
		activeTileDictionary.Add (zoneData.borderTile.id, zoneData.borderTile.tile);
		for (int i = 0; i < zoneData.baseDecorTiles.Count; i++) {
			int id = zoneData.baseDecorTiles[i].id;
			if (!activeTileDictionary.ContainsKey (id))
				activeTileDictionary.Add (id, zoneData.baseDecorTiles [i].tile);
		}
		for (int i = 0; i < zoneData.randomFloorDecorTiles.Count; i++) {
			int id = zoneData.randomFloorDecorTiles[i].id;
			if (!activeTileDictionary.ContainsKey (id))
				activeTileDictionary.Add (id, zoneData.randomFloorDecorTiles [i].tile);
		}
		for (int i = 0; i < zoneData.topDecorTiles.Count; i++) {
			int id = zoneData.topDecorTiles[i].id;
			if (!activeTileDictionary.ContainsKey (id))
				activeTileDictionary.Add (id, zoneData.topDecorTiles [i].tile);
		}

	}
	private void RegisterEnemies (ZoneData worldData, GameDataLegend legend) {
		for (int i = 0; i < worldData.enemyDatas.Count; i++) {
			CreatureData enemyData = worldData.enemyDatas[i];
			if (!activeCreatureDictionary.ContainsKey (enemyData.id)) {
				activeCreatureDictionary.Add (enemyData.id, enemyData);
			}
		}
	}

	private void RegisterSetPieces (ZoneData zoneData) {
		for (int i = 0; i < zoneData.largeSetpieceDatas.Count; i++) {
			SetPieceData setPieceData = zoneData.largeSetpieceDatas[i];
			if (!activeSetpieceDictionary.ContainsKey (setPieceData.id)) {
				activeSetpieceDictionary.Add (setPieceData.id, setPieceData);
			}
		}
	}

	private void RegisterObjectivePieces (ObjectiveData objectiveData) {
		for (int i = 0; i < objectiveData.objectiveSpawnInfos.Length; i++) {
			SetPieceData setPieceData = objectiveData.objectiveSpawnInfos[i].setPieceData;
			if (!activeSetpieceDictionary.ContainsKey (setPieceData.id)) {
				activeSetpieceDictionary.Add (setPieceData.id, setPieceData);
			}
		}
	}

	private void RegisterSpells (SpellSchoolData spellSchoolData) {
		for (int i = 0; i < spellSchoolData.spells.Count; i++) {
			SpellData spellData = spellSchoolData.spells[i];
			activeSpellDictionary.Add (spellData.id, spellData);
		}
	}
	private void RegisterPuzzles (PuzzleData[] puzzleDatas) {
		for (int i = 0; i < puzzleDatas.Length; i++) {
			PuzzleData puzzleData = puzzleDatas[i];
			activePuzzleDictionary.Add (puzzleData.id, puzzleData);
		}
	}

	#endregion
}
