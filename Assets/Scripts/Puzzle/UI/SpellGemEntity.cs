using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemEntity : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();

	}
	public void InitializeSpellGemUI (Sprite spellGemSprite) {
		spriteRenderer.sprite = spellGemSprite;
	}
	public void SetErrorColor() {
		spriteRenderer.color = Color.red;
	}
	public void SetNormalColor () {
		spriteRenderer.color = Color.white;
	}
}
