﻿using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections;


/*
 When the player enters puzzle state, this UI object is enabled/displayed over player's head.
-Contains Inventory display
-Contains Staff display
-Contains Display of active spells/passives
*/

public class PuzzleUI : MonoBehaviour {


	[SerializeField]
	public Tile staffTile;
	[SerializeField]
	private Tilemap puzzleTilemap;
	[SerializeField]
	private Transform highlightedBlockTransform;
	public Transform uncommitedSpellGemParentTransform;
	[SerializeField]
	private TMP_Text highlightedSpellNameText;
	[SerializeField]
	private TMP_Text highlightedSpellDescriptionText;
	[SerializeField]
	private TMP_Text staffNameText;


	public void InitializePuzzleUI (WizardSaveData wizardSaveData, PuzzleGroupingDetails inventoryDetails, PuzzleGroupingDetails staffDetails) {
		staffNameText.text = wizardSaveData.primaryStaffSaveData.puzzleData.puzzleName;
		PuzzleFactory.BuildInventoryUI (inventoryDetails.map, inventoryDetails.groupingOrigin, puzzleTilemap, staffTile);
		PuzzleFactory.BuildStaffUI (staffDetails.map, staffDetails.groupingOrigin, puzzleTilemap, staffTile);
	}


	public Vector2Int RoundCursorLocationToNearestPuzzleSlot (Vector3 cursorPosition) {
		Vector3Int cursorHighlightedCellPosition = puzzleTilemap.WorldToCell (cursorPosition);
		highlightedBlockTransform.localPosition = puzzleTilemap.GetCellCenterLocal (cursorHighlightedCellPosition);
		return cursorHighlightedCellPosition.XY ();
	}
	public void RoundSpellGemEntityLocationToNearestTile (SpellGemEntity spellGemEntity, Vector2Int highlightedGridCellCoordinate) {
		spellGemEntity.transform.localPosition = puzzleTilemap.GetCellCenterLocal ((Vector3Int)highlightedGridCellCoordinate);
	}
	//creates a new SpellGemEntity if necessary. otherwise re-uses the old one.

	public void RotateSpellGemEntity (SpellGemEntity spellGemEntity, int rotation) {
		spellGemEntity.transform.rotation = Quaternion.Euler (0, 0, rotation);

	}

	public void ErrorFlashSpellGemEntity (SpellGemEntity spellGemEntity) {
		StartCoroutine (ErrorFlashRoutine (spellGemEntity));
	}

	private IEnumerator ErrorFlashRoutine (SpellGemEntity spellGemEntity) {
		float errorFlashTimer = 1f;
		bool isRed = false;
		while (errorFlashTimer > 0f) {
			if (isRed) {
				spellGemEntity.SetNormalColor ();
				isRed = false;
			} else {
				spellGemEntity.SetErrorColor ();
				isRed = true;
			}
			yield return new WaitForSeconds (0.1f);
			errorFlashTimer -= 0.1f;
		}
		if (isRed == true) {
			spellGemEntity.SetNormalColor ();
		}
	}
	public SpellGemEntity AddSpellGemToPuzzleUI (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		SpellGemEntity spellGemEntity = spellSaveData.spellGemEntity;
		if (spellSaveData.spellGemEntity == null) {
			GameObject spellGemEntityGo = Instantiate (ConstantsManager.instance.spellGemUIPrefab);
			spellGemEntityGo.name = "SpellGem_" + spellSaveData.spellData.spellName;
			spellGemEntity = spellGemEntityGo.GetComponent<SpellGemEntity> ();
			spellGemEntity.InitializeSpellGemUI (spellSaveData.spellData);
			spellSaveData.spellGemEntity = spellGemEntity;
		}
		spellGemEntity.gameObject.transform.parent = details.gemParentTransform;
		Vector3Int spellGemCoordinate = new Vector3Int (details.groupingOrigin.x + spellSaveData.spellGemOriginCoordinate.x, details.groupingOrigin.y + spellSaveData.spellGemOriginCoordinate.y, 0);
		Vector3 cellLocalPosition = puzzleTilemap.GetCellCenterLocal (spellGemCoordinate);
		spellGemEntity.transform.localPosition = cellLocalPosition;
		RotateSpellGemEntity (spellSaveData.spellGemEntity, spellSaveData.spellGemRotation * 90);
		return spellGemEntity;
	}

	public void MoveSpellGemToUncommited (SpellSaveData spellSaveData) {
		spellSaveData.spellGemEntity.transform.parent = uncommitedSpellGemParentTransform;
	}

	public void UpdateHighlightedSpellGemInformation (SpellData spellData) {
		highlightedSpellNameText.text = spellData.spellName;
		highlightedSpellDescriptionText.text = spellData.description;
	}
	public void ClearHighlightedSpellGemInformation () {
		highlightedSpellNameText.text = "";
		highlightedSpellDescriptionText.text = "";
	}
}
