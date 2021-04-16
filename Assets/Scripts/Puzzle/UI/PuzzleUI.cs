using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleUI : MonoBehaviour {
	[SerializeField]
	private Vector2Int origin = new Vector2Int(-4, -4);
	private Vector2Int boundDimensions = new Vector2Int (8, 8);

	private CoordinateBounds bounds;

	[SerializeField]
	public Grid grid;

	[SerializeField]
	private Transform highlightedBlockTransform;

	public void SetUpPuzzleUI () {
		grid = GetComponentInChildren<Grid> ();
		bounds = new CoordinateBounds (origin, boundDimensions);
		ClearGridChildren ();
	}

	public void ClearGridChildren () {
		foreach (Transform child in grid.gameObject.transform) {
			GameObject.Destroy (child.gameObject);
		}
	}
	public PuzzleEntity AddPuzzleEntityToPuzzleUI (PuzzleKey region, PuzzleGameData puzzleGameData) {
		PuzzleEntity puzzleEntity = puzzleGameData.puzzleEntity;
		if (puzzleEntity == null) {
			Debug.Log ("PuzzleUI: " + region + " PuzzleEntity missing from PuzzleGameData. Creating PuzzleEntity.");
			GameObject puzzleEntityGo = Instantiate (ConstantsManager.instance.puzzleEntityPrefab);
			puzzleEntity = puzzleEntityGo.GetComponent<PuzzleEntity> ();
			puzzleEntity.SetUpPuzzleEntity (puzzleGameData);
		}

		puzzleEntity.SetPuzzleEntityGridParent (grid, origin);

		puzzleEntity.puzzleObjectTrans.parent = grid.transform;
		return puzzleEntity;
	}

	public void RotateSpellGemEntity (SpellGemEntity spellGemEntity, int rotation) {
		spellGemEntity.transform.rotation = Quaternion.Euler (0, 0, rotation);

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
		spellGemEntity.transform.localPosition = cellLocalPosition;
		RotateSpellGemEntity (spellGemGameData.spellGemEntity, spellGemGameData.spellGemRotation * 90);
		return spellGemEntity;
	}

}
