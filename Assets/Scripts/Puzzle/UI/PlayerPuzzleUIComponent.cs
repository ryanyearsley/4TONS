using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;


/*
 When the player enters puzzle state, this UI object is enabled/displayed over player's head.
-Contains Inventory display
-Contains Staff display
-Contains Display of active spells/passives
*/

//childed player component responsible for displaying inventory/staffs/spellgems.
//(think animatorComponent of the puzzle mechanic)
public class PlayerPuzzleUIComponent : PlayerComponent {

	private PuzzleUI puzzleUI;
	private CursorController cursorController;

	[SerializeField]
	private Transform highlightedBlockTransform;

	//CANVAS ELEMENTS

	public SpriteRenderer puzzleIconSprite;
	public TMP_Text puzzleNameText;

	private SpellGemEntity currentErrorFlashSpellGemEntity;
	private bool isErrorFlashing = false;

	private PuzzleKey activeStaffKey = PuzzleKey.PRIMARY_STAFF;


	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		puzzleUI = GetComponent<PuzzleUI> ();
		puzzleUI.SetUpPuzzleUI ();
	}
	public override void ReusePlayerComponent (Player player) {
		Debug.Log ("PlayerPuzzleUIComponent: Reusing Player Component");
		LoadPuzzleEntities (playerObject.wizardGameData);

		puzzleUI.DisablePuzzleUI ();
	}

	public override void OnSpawn(Vector3 spawnPosition) {
		cursorController = playerObject.creaturePositions.targetTransform.GetComponent<CursorController> ();
	}
	public void LoadPuzzleEntities (WizardGameData wizardGameData) {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.INVENTORY)) {
			PuzzleGameData inventoryGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.INVENTORY];
			Debug.Log ("PuzzleComponent: Creating Staff PuzzleEntity.");
			inventoryGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.INVENTORY, inventoryGameData);
			LoadSpellGemEntities (inventoryGameData);
		}
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			PuzzleGameData primaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.PRIMARY_STAFF];
			primaryStaffGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.PRIMARY_STAFF, primaryStaffGameData);
			LoadSpellGemEntities (primaryStaffGameData);
		}

		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			PuzzleGameData secondaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.SECONDARY_STAFF];
			secondaryStaffGameData.puzzleEntity = puzzleUI.AddPuzzleEntityToPuzzleUI (PuzzleKey.SECONDARY_STAFF, secondaryStaffGameData);
			LoadSpellGemEntities (secondaryStaffGameData);
		}
	}

	private void LoadSpellGemEntities (PuzzleGameData puzzleGameData) {
		Debug.Log ("Loading player spellgem entities");
		foreach (SpellGemGameData spellGemGameData in puzzleGameData.spellGemGameDataDictionary.Values) {

			Debug.Log ("Adding entity for " + spellGemGameData.spellData.spellName);
			spellGemGameData.spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (puzzleGameData.puzzleEntity, spellGemGameData);
		}
	}


	#region PlayerComponent Events
	public override void OnChangePlayerState (PlayerState playerState) {
		Debug.Log ("player puzzle OnChangePlayerState event");
		switch (playerState) {
			case (PlayerState.DEAD): {
					puzzleUI.DisablePuzzleUI ();
					InterruptFlashRoutine ();
					break;
				}
			case (PlayerState.COMBAT): {
					puzzleUI.DisablePuzzleUI ();
					InterruptFlashRoutine ();
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					puzzleUI.EnablePuzzleUI ();
					puzzleUI.UpdateActiveRegion (playerObject.wizardGameData.currentStaffKey);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					puzzleUI.EnablePuzzleUI ();
					break;
				}
		}
	}

	public override void OnEquipStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
		activeStaffKey = key;
		UpdateStaffInfo (puzzleGameData);
		puzzleUI.UpdateActiveRegion (key);
	}


	public override void OnPickUpStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
		puzzleUI.AddPuzzleEntityToPuzzleUI (key, puzzleGameData);
	}

	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		base.OnDropStaff (region, puzzleGameData);

	}
	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {

		UpdateHighlightedSpellGemInformation (spellGemGameData.spellData);
		spellGemGameData.spellGemEntity = puzzleUI.AddSpellGemToPuzzleUIUncommited (spellGemGameData);
		spellGemGameData.spellGemEntity.SetMovingColor ();
		currentErrorFlashSpellGemEntity = spellGemGameData.spellGemEntity;
	}
	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {

		SpellGemEntity spellGemEntity = puzzleUI.AddSpellGemToPuzzleUI (puzzleGameData.puzzleEntity, spellGemGameData);
		InterruptFlashRoutine ();
		spellGemGameData.spellGemEntity = spellGemEntity;
		spellGemGameData.spellGemEntity.SetNormalColor ();
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		puzzleUI.MoveSpellGemToUncommited (spellGemGameData);
		spellGemGameData.spellGemEntity.SetMovingColor ();
	}
	#endregion
	public Vector2Int RoundCursorLocationToNearestPuzzleSlot (Vector3 cursorPosition) {
		Vector3Int cursorHighlightedCellPosition = puzzleUI.grid.WorldToCell (cursorPosition);
		highlightedBlockTransform.localPosition = puzzleUI.grid.GetCellCenterLocal (cursorHighlightedCellPosition);
		return cursorHighlightedCellPosition.XY ();
	}
	public void RoundSpellGemEntityLocationToNearestTile (SpellGemEntity spellGemEntity, Vector2Int highlightedGridCellCoordinate) {
		spellGemEntity.transform.localPosition = puzzleUI.grid.GetCellCenterLocal ((Vector3Int)highlightedGridCellCoordinate);
	}
	//creates a new SpellGemEntity if necessary. otherwise re-uses the old one.

	public void RotateSpellGemEntity (SpellGemEntity spellGemEntity, int rotation) {
		spellGemEntity.transform.rotation = Quaternion.Euler (0, 0, rotation);
	}

	public void ErrorFlashSpellGemEntity (SpellGemEntity spellGemEntity) {
		currentErrorFlashSpellGemEntity = spellGemEntity;
		StartCoroutine (ErrorFlashRoutine (spellGemEntity));
	}

	private void InterruptFlashRoutine () {
		if (isErrorFlashing) {
			StopCoroutine (ErrorFlashRoutine (currentErrorFlashSpellGemEntity));
			currentErrorFlashSpellGemEntity.SetNormalColor ();
			isErrorFlashing = false;
		}
	}
	public IEnumerator ErrorFlashRoutine (SpellGemEntity spellGemEntity) {
		isErrorFlashing = true;
		float errorFlashTimer = 0.5f;
		bool isRed = false;
		while (errorFlashTimer > 0f) {
			if (isRed) {
				spellGemEntity.SetMovingColor ();
				isRed = false;
			} else {
				spellGemEntity.SetErrorColor ();
				isRed = true;
			}
			yield return new WaitForSeconds (0.1f);
			errorFlashTimer -= 0.1f;
		}
		if (isRed == true) {
			spellGemEntity.SetMovingColor ();
		}
		isErrorFlashing = false;
	}
	public PuzzleCursorLocation CalculatePuzzleCursorLocation (Vector3 cursorPosition) {
		return puzzleUI.CalculatePuzzleCursorLocation (cursorPosition);
	}
 	public void UpdateHighlightedSpellGemInformation (SpellData spellData) {
		cursorController.UpdateToolTipText (spellData.spellName);
	}
	public void ClearHighlightedSpellGemInformation () {
		cursorController.ClearToolTip ();
	}

	public void UpdateStaffInfo (PuzzleGameData puzzleGameData) {
		puzzleIconSprite.sprite = puzzleGameData.puzzleData.puzzleSprite;
		puzzleNameText.text = puzzleGameData.puzzleData.puzzleName;
	}

	public void ClearStaffInfo () {
		puzzleIconSprite.sprite = null;
		puzzleNameText.text = "";
	}
}
