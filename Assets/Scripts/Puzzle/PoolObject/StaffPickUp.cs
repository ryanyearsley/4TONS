using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StaffPickUp : PickUpObject {

	private Grid grid;
	[SerializeField]
	private SpriteRenderer staffSprite;
	[SerializeField]
	private Vector2Int tilemapOrigin;

	public PuzzleGameData puzzleGameData;

	public override void SetupObject () {
		grid = GetComponent<Grid> ();
	}

	public void ReuseStaffPickUp (PuzzleData puzzleData) {
		this.puzzleGameData = new PuzzleGameData (puzzleData, PuzzleKey.PICK_UP); //initializes data
		staffSprite.sprite = puzzleGameData.puzzleData.puzzleSprite;
		AddPuzzleEntityToPickUp (puzzleGameData);
	}

	public void ReuseStaffPickUpPlayerDrop (PuzzleGameData puzzleGameData) {
		this.puzzleGameData = puzzleGameData;
		puzzleGameData.puzzleKey = PuzzleKey.PICK_UP;
		staffSprite.sprite = puzzleGameData.puzzleData.puzzleIcon;
		AddPuzzleEntityToPickUp (puzzleGameData);
	}

	public void AddPuzzleEntityToPickUp (PuzzleGameData puzzleGameData) {
		PuzzleEntity puzzleEntity = puzzleGameData.puzzleEntity;
		if (puzzleEntity == null) {
			GameObject puzzleEntityGo = Instantiate(ConstantsManager.instance.puzzleEntityPrefab);
			puzzleEntity = puzzleEntityGo.GetComponent<PuzzleEntity> ();
			puzzleEntity.SetUpPuzzleEntity (puzzleGameData);
		}
		puzzleEntity.SetPuzzleEntityGridParent (grid, tilemapOrigin);
	}

	protected override void HighlightPickUp () {
		puzzleGameData.puzzleEntity.tilemap.gameObject.SetActive (true);
	}
	protected override void UnhighlightPickUp () {
		puzzleGameData.puzzleEntity.tilemap.gameObject.SetActive (false);
	}

}
