using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemUI : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	public void InitializeSpellGemUI (Sprite spellGemSprite) {
		spriteRenderer.sprite = spellGemSprite;
	}
}
