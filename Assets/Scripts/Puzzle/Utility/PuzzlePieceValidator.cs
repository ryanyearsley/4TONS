using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceValidator : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private Vector2 puzzleScale = new Vector3 (0.3125f, 0.3125f, 1);

	public PuzzlePieceData currentPuzzlePiece;
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	public void ValidatePuzzlePiece() {
		spriteRenderer.sprite = currentPuzzlePiece.puzzlePieceSprite;
	}

	private void OnDrawGizmosSelected () {
		spriteRenderer.sprite = currentPuzzlePiece.puzzlePieceSprite;
		for (int i = 0; i < currentPuzzlePiece.coordinates.Length; i++) {
			Vector2 coord = currentPuzzlePiece.coordinates [i] * puzzleScale;
			Vector3 position = new Vector3 (coord.x, coord.y, 1f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere (position, 0.15f);
		}
	}
}
