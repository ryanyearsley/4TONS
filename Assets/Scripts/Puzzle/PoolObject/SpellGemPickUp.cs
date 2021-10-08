using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemPickUp : InteractableObject {
	public SpellData spellData;
	public Spell spellCast;

	[SerializeField]
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private SpriteRenderer backgroundSpriteRenderer;

	private Sprite backgroundSprite;
	private Sprite highlightedSprite;//puzzle piece preview


	public void ReuseSpellGemPickUp (SpellData spellData) {
		this.spellData = spellData;
		this.spellCast = Instantiate (spellData.castObject).GetComponent<Spell>();
		spellCast.transform.parent = this.transform;
		spellCast.transform.localPosition = Vector3.zero;
		backgroundSprite = backgroundSpriteRenderer.sprite;
		highlightedSprite = spellData.puzzlePieceData.puzzlePieceSprite;
		spriteRenderer.sprite = spellData.icon;
		backgroundSpriteRenderer.color = spellData.spellSchoolData.schoolColor;
	}
	protected override void HighlightInteractable () {
		backgroundSpriteRenderer.sprite = highlightedSprite;
	}
	protected override void UnhighlightInteractable () {
		backgroundSpriteRenderer.sprite = backgroundSprite;
	}
}
