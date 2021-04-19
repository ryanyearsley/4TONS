using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Basic World Space UI element with single puzzle
public class PuzzleUI : MonoBehaviour {


	[SerializeField]
	public Grid grid;

	[SerializeField]
	public Transform uncommitedSpellGemParentTransform;

	[SerializeField]
	private PuzzleRegion[] regions;

	private Dictionary <PuzzleKey, PuzzleRegion> puzzleRegionDictionary = new Dictionary<PuzzleKey, PuzzleRegion>();

	private GameObject worldSpaceUIObject;

	private void Awake () {
		
	}
	public virtual void SetUpPuzzleUI () {
		worldSpaceUIObject = transform.GetChild (0).gameObject;
		grid = GetComponentInChildren<Grid> ();
		ClearGridChildren ();
		InitializeRegionDictionary ();
	}


	public void InitializeRegionDictionary () {
		puzzleRegionDictionary.Clear ();
		foreach (PuzzleRegion region in regions) {
			region.bounds = new CoordinateBounds (region.origin, region.dimensions);
			puzzleRegionDictionary.Add (region.puzzleKey, region);
		}
	}

	public void UpdateActiveRegion (PuzzleKey key) {
		foreach (PuzzleRegion region in regions) {
			if (!region.alwaysVisible && region.puzzleKey != key) {
				region.HideRegion ();
			} else {
				region.UnhideRegion ();
			}

		}

	}
	public void DisablePuzzleUI () {
		worldSpaceUIObject.SetActive (false);
	}
	public void EnablePuzzleUI () {
		worldSpaceUIObject.SetActive (true);
	}

	public void ClearGridChildren () {
		foreach (Transform child in grid.gameObject.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}

	public PuzzleCursorLocation CalculatePuzzleCursorLocation (Vector3 cursorPosition) {

		for (int i = 0; i < regions.Length; i++) {
			Vector3Int gridCoord = grid.WorldToCell(cursorPosition);
			PuzzleRegion puzzleRegion = regions[i];
			if (puzzleRegion.isActive && puzzleRegion.bounds.isWithinBounds (gridCoord.XY ())) {
				return new PuzzleCursorLocation (puzzleRegion.puzzleKey, gridCoord - (Vector3Int)puzzleRegion.bounds.minCoord);
			}
		}
		return new PuzzleCursorLocation (PuzzleKey.OUTSIDE_BOUNDS, Vector3Int.zero);
	}

	public PuzzleEntity AddPuzzleEntityToPuzzleUI (PuzzleKey key, PuzzleGameData puzzleGameData) {
		if (puzzleRegionDictionary.ContainsKey (key)) {
			PuzzleRegion region = puzzleRegionDictionary[key];
			PuzzleEntity puzzleEntity = puzzleGameData.puzzleEntity;
			if (puzzleEntity == null) {
				GameObject puzzleEntityGo = Instantiate (ConstantsManager.instance.puzzleEntityPrefab);
				puzzleEntity = puzzleEntityGo.GetComponent<PuzzleEntity> ();
				puzzleEntity.SetUpPuzzleEntity (puzzleGameData);
			}
			region.occupantEntity = puzzleEntity;
			puzzleEntity.SetPuzzleEntityGridParent (grid, region.origin);
			return puzzleEntity;
		} else {
			Debug.LogError ("PuzzleUI: Puzzle Entity Add fail. Region occupied.");
			return null;
		}
	}


	//used during initialization
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
		spellGemEntity.transform.localPosition = cellLocalPosition;
		spellGemEntity.Rotate (spellGemGameData.spellGemRotation * 90);
		return spellGemEntity;
	}

	//used when picking up a spell gem in gameplay
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
		spellGemEntity.Rotate (spellGemGameData.spellGemRotation * 90);
		return spellGemEntity;
	}

	public SpellGemEntity MoveSpellGemToUncommited (SpellGemGameData spellGameData) {
		spellGameData.spellGemEntity.transform.parent = uncommitedSpellGemParentTransform;
		return spellGameData.spellGemEntity;
	}
}