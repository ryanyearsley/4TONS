using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemPickUpObject : InteractableObject {
	public SpellData spellData;
	public Spell spellCast;

	public InteractableObject interactableComponent;

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
	public override void HighlightInteractable () {
		backgroundSpriteRenderer.sprite = highlightedSprite;
	}
	public override void UnhighlightInteractable () {
		backgroundSpriteRenderer.sprite = backgroundSprite;
	}
}
