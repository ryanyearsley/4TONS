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

	[SerializeField]
	private GameObject worldSpaceUIObject;
	[SerializeField]
	private Vector2Int inventoryOrigin = new Vector2Int(-11, 0);
	[SerializeField]
	private Vector2Int inventoryBoundSize = new Vector2Int (9, 6);
	[SerializeField]
	private Vector2Int staffOrigin = new Vector2Int(2, 0);
	[SerializeField]
	private Vector2Int staffBoundSize =  new Vector2Int (7, 7);

	private CoordinateBounds inventoryBounds;
	private CoordinateBounds staffBounds;

	[SerializeField]
	public Tile staffTile;

	[SerializeField]
	public Grid grid;

	[SerializeField]
	private Transform highlightedBlockTransform;
	public Transform uncommitedSpellGemParentTransform;

	//CANVAS ELEMENTS
	[SerializeField]
	private TMP_Text highlightedSpellNameText;
	[SerializeField]
	private SpriteRenderer equippedStaffSprite;
	[SerializeField]
	private TMP_Text equippedStaffNameText;



	private PuzzleKey activeStaffRegion = PuzzleKey.PRIMARY_STAFF;


	public void DisablePuzzleUI() {
		worldSpaceUIObject.SetActive (false);
	}
	public void EnablePuzzleUI () {
		worldSpaceUIObject.SetActive (true);
	}

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		grid = GetComponentInChildren<Grid> ();
		inventoryBounds = new CoordinateBounds (inventoryOrigin, inventoryBoundSize);
		staffBounds = new CoordinateBounds (staffOrigin, staffBoundSize);
		ClearHighlightedSpellGemInformation ();
	}

	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		activeStaffRegion = region;
		UpdateStaffInfo (puzzleGameData.puzzleData);
		equippedStaffSprite.sprite = puzzleGameData.puzzleData.puzzleSprite;
		equippedStaffNameText.text = puzzleGameData.puzzleData.puzzleName;

	}
	public void ClearGridChildren() {
		foreach (Transform child in grid.gameObject.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	public PuzzleCursorLocation CalculatePuzzleCursorLocation (Vector3 cursorPosition) {

		Vector3Int gridCoord = grid.WorldToCell(cursorPosition);
		highlightedBlockTransform.localPosition = grid.GetCellCenterLocal ((Vector3Int)gridCoord);

		if (staffBounds.isWithinBounds (gridCoord.XY ()))
			return new PuzzleCursorLocation (activeStaffRegion, gridCoord - (Vector3Int)staffBounds.minCoord);
		else if (inventoryBounds.isWithinBounds (gridCoord.XY ()))
			return new PuzzleCursorLocation (PuzzleKey.INVENTORY, gridCoord - (Vector3Int)inventoryBounds.minCoord);
		else
			return new PuzzleCursorLocation (PuzzleKey.OUTSIDE_BOUNDS, Vector3Int.zero);
	}
	public Vector2Int RoundCursorLocationToNearestPuzzleSlot (Vector3 cursorPosition) {
		Vector3Int cursorHighlightedCellPosition = grid.WorldToCell (cursorPosition);
		highlightedBlockTransform.localPosition = grid.GetCellCenterLocal (cursorHighlightedCellPosition);
		return cursorHighlightedCellPosition.XY ();
	}
	public void RoundSpellGemEntityLocationToNearestTile (SpellGemEntity spellGemEntity, Vector2Int highlightedGridCellCoordinate) {
		spellGemEntity.transform.localPosition = grid.GetCellCenterLocal ((Vector3Int)highlightedGridCellCoordinate);
	}
	//creates a new SpellGemEntity if necessary. otherwise re-uses the old one.

	public void RotateSpellGemEntity (SpellGemEntity spellGemEntity, int rotation) {
		spellGemEntity.transform.rotation = Quaternion.Euler (0, 0, rotation);

	}

	public void ErrorFlashSpellGemEntity (SpellGemEntity spellGemEntity) {
		StartCoroutine (ErrorFlashRoutine (spellGemEntity));
	}

	private IEnumerator ErrorFlashRoutine (SpellGemEntity spellGemEntity) {
		float errorFlashTimer = 0.5f;
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

	//called by either OnInit or OnEquip
	public PuzzleEntity AddPuzzleEntityToPuzzleUI (PuzzleKey region, PuzzleGameData puzzleGameData) {
		PuzzleEntity puzzleEntity = puzzleGameData.puzzleEntity;
		if (puzzleEntity == null) {
			Debug.Log ("PuzzleUI: " + region + " PuzzleEntity missing from PuzzleGameData. Creating PuzzleEntity.");
			GameObject puzzleEntityGo = Instantiate (ConstantsManager.instance.puzzleEntityPrefab);
			puzzleEntity = puzzleEntityGo.GetComponent<PuzzleEntity> ();
			puzzleEntity.SetUpPuzzleEntity (puzzleGameData);
		}


		Vector2Int originCoordinate = Vector2Int.zero;
		if (region == PuzzleKey.INVENTORY)
			originCoordinate = inventoryBounds.minCoord;
		else
			originCoordinate = staffBounds.minCoord;

		puzzleEntity.SetPuzzleEntityGridParent (grid, originCoordinate);

		puzzleEntity.puzzleObjectTrans.parent = grid.transform;
		Debug.Log ("PuzzleUI: childed staff to puzzleUI");
		return puzzleEntity;
	}

	public SpellGemEntity AddSpellGemToPuzzleUI (PuzzleEntity puzzleEntity, SpellGemGameData spellGemGameData) {
		SpellGemEntity spellGemEntity = spellGemGameData.spellGemEntity;
		if (spellGemGameData.spellGemEntity == null) {
			GameObject spellGemEntityGo = Instantiate (ConstantsManager.instance.spellGemUIPrefab);
			spellGemEntityGo.name = "SpellGem_" + spellGemGameData.spellData.spellName;
			spellGemEntity = spellGemEntityGo.GetComponent<SpellGemEntity> ();
			spellGemEntity.InitializeSpellGemEntity (spellGemGameData.spellData);
			spellGemGameData.spellGemEntity = spellGemEntity;
		}
		spellGemEntity.gameObject.transform.parent = puzzleEntity.tilemap.transform;
		Vector3Int spellGemCoordinate = new Vector3Int (spellGemGameData.spellGemOriginCoordinate.x, spellGemGameData.spellGemOriginCoordinate.y, 0);
		Vector3 cellLocalPosition = puzzleEntity.tilemap.GetCellCenterLocal (spellGemCoordinate);
		spellGemEntity.transform.localPosition = cellLocalPosition ;
		RotateSpellGemEntity (spellGemGameData.spellGemEntity, spellGemGameData.spellGemRotation * 90);
		return spellGemEntity;
	}
	public SpellGemEntity AddSpellGemToPuzzleUIUncommited (SpellGemGameData spellGemGameData) {
		SpellGemEntity spellGemEntity = spellGemGameData.spellGemEntity;
		if (spellGemGameData.spellGemEntity == null) {
			GameObject spellGemEntityGo = Instantiate (ConstantsManager.instance.spellGemUIPrefab);
			spellGemEntityGo.name = "SpellGem_" + spellGemGameData.spellData.spellName;
			spellGemEntity = spellGemEntityGo.GetComponent<SpellGemEntity> ();
			spellGemEntity.InitializeSpellGemEntity (spellGemGameData.spellData);
			spellGemGameData.spellGemEntity = spellGemEntity;
		}
		spellGemEntity.gameObject.transform.parent = uncommitedSpellGemParentTransform;
		Vector3Int spellGemCoordinate = new Vector3Int (spellGemGameData.spellGemOriginCoordinate.x, spellGemGameData.spellGemOriginCoordinate.y, 0);
		Vector3 cellLocalPosition = grid.GetCellCenterLocal (spellGemCoordinate);
		spellGemEntity.transform.localPosition = cellLocalPosition;
		RotateSpellGemEntity (spellGemGameData.spellGemEntity, spellGemGameData.spellGemRotation * 90);
		return spellGemEntity;
	}

	public SpellGemEntity MoveSpellGemToUncommited (SpellGemGameData spellGameData) {
		spellGameData.spellGemEntity.transform.parent = uncommitedSpellGemParentTransform;
		spellGameData.spellCast.transform.parent = uncommitedSpellGemParentTransform;
		return spellGameData.spellGemEntity;
	}

	public void UpdateHighlightedSpellGemInformation (SpellData spellData) {
		highlightedSpellNameText.text = spellData.spellName;
	}
	public void ClearHighlightedSpellGemInformation () {
		highlightedSpellNameText.text = "";
	}

	public void UpdateStaffInfo(PuzzleData puzzleData) {

		equippedStaffSprite.sprite = puzzleData.puzzleSprite;
		equippedStaffNameText.text = puzzleData.puzzleName;
	}

	public void ClearStaffInfo() {
		equippedStaffSprite.sprite = null;
		equippedStaffNameText.text = "UNARMED";
	}
	/*
		private void OnDrawGizmosSelected () {
			Gizmos.DrawCube
		}*/
}
