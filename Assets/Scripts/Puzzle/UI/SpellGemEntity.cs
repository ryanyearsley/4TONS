using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemEntity : MonoBehaviour {
	public SpellData spellData;
	private SpriteRenderer spriteRenderer;

	private Color defaultColor;
	private Color movingColor;

	private void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();

	}

	public void Rotate (int rotation) {
			transform.rotation = Quaternion.Euler (0, 0, rotation);
	}
	public void InitializeSpellGemEntity (SpellData spellData) {
		this.spellData = spellData;
		spriteRenderer.sprite = spellData.puzzlePieceData.puzzlePieceSprite;
		defaultColor = spellData.spellSchoolData.schoolColor;
		movingColor = new Color (defaultColor.r, defaultColor.g, defaultColor.b, 0.75f);
		spriteRenderer.color = defaultColor;
	}
	public void SetErrorColor() {
		spriteRenderer.color = Color.red;
	}
	public void SetNormalColor () {
		spriteRenderer.color = defaultColor;
	}
	public void SetMovingColor() {
		spriteRenderer.color = movingColor;
	}
}
