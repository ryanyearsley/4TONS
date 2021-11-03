using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StaffPickUpObject : InteractableObject {

	private Grid grid;
	[SerializeField]
	private SpriteRenderer staffSprite;

	
	[SerializeField]
	private Vector2Int tilemapOrigin;

	private PuzzleUI puzzleUI;

	public PuzzleGameData puzzleGameData;
	public override void SetupObject () {
		grid = GetComponent<Grid> ();

		puzzleUI = GetComponentInChildren<PuzzleUI> ();
		puzzleUI.SetUpPuzzleUI ();
	}
	public void ReuseStaffPickUp (PuzzleData puzzleData) {
		puzzleGameData = new PuzzleGameData (puzzleData, PuzzleKey.PICK_UP, puzzleData.defaultSpellGemDictionary); //initializes data
		puzzleGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.PICK_UP, puzzleGameData);
		foreach (SpellGemGameData spellGemGameData in puzzleGameData.spellGemGameDataDictionary.Values) {
			puzzleUI.AddSpellGemToPuzzleUI (puzzleGameData.puzzleEntity, spellGemGameData);
			SpellCastUtility.LoadSpellCastEntity (puzzleGameData, spellGemGameData);
		}
		staffSprite.sprite = puzzleGameData.puzzleData.puzzleSprite;
		puzzleUI.DisablePuzzleUI ();
		//AddPuzzleEntityToPickUp (puzzleGameData);
	}

	public void ReuseStaffPickUpPlayerDrop (PuzzleGameData puzzleGameData) {
		this.puzzleGameData = puzzleGameData;
		puzzleGameData.puzzleKey = PuzzleKey.PICK_UP;
		puzzleGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (puzzleGameData.puzzleKey, puzzleGameData);
		staffSprite.sprite = puzzleGameData.puzzleData.puzzleSprite;
		//AddPuzzleEntityToPickUp (puzzleGameData);
	}
	/*
		public void AddPuzzleEntityToPickUp (PuzzleGameData puzzleGameData) {
			PuzzleEntity puzzleEntity = puzzleGameData.puzzleEntity;
			if (puzzleEntity == null) {
				GameObject puzzleEntityGo = Instantiate(ConstantsManager.instance.puzzleEntityPrefab);
				puzzleEntity = puzzleEntityGo.GetComponent<PuzzleEntity> ();
				puzzleEntity.SetUpPuzzleEntity (puzzleGameData);
			}
			puzzleEntity.SetPuzzleEntityGridParent (grid, tilemapOrigin);
		}*/

	public override void HighlightInteractable () {
		puzzleUI.EnablePuzzleUI ();
		puzzleGameData.puzzleEntity.tilemap.gameObject.SetActive (true);
	}
	public override void UnhighlightInteractable () {
		puzzleUI.DisablePuzzleUI ();
		puzzleGameData.puzzleEntity.tilemap.gameObject.SetActive (false);
	}

}
