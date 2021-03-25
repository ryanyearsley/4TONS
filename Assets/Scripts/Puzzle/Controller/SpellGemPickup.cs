using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemPickup : PoolObject {
	public SpellData spellData;

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer backgroundSpriteRenderer;

	private Sprite backgroundSprite;
	private Sprite highlightedSprite;

	public override void SetupObject () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		backgroundSpriteRenderer = GetComponentInChildren<SpriteRenderer> ();
	}

	public void ReuseSpellGemPickUp (SpellData spellData) {
		this.spellData = spellData;

		backgroundSprite = backgroundSpriteRenderer.sprite;
		highlightedSprite = spellData.puzzlePieceData.puzzlePieceSprite;
		spriteRenderer.sprite = spellData.icon;
		backgroundSpriteRenderer.color = spellData.spellSchoolData.schoolGemColor;
	}

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player1") {
			other.transform.GetComponentInParent<PlayerPuzzleComponent> ().AddSpellGemToInteractable (this);
			backgroundSpriteRenderer.sprite = highlightedSprite;
		}
	}
	private void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player1") {
			other.transform.GetComponentInParent<PlayerPuzzleComponent> ().RemoveSpellGemFromInteractable (this);
			backgroundSpriteRenderer.sprite = backgroundSprite;
		}
	}
}
