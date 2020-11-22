using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemEntity : MonoBehaviour {
	private SpellData spellData;
	private SpriteRenderer spriteRenderer;

	private Color defaultColor;

	private void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();

	}
	public void InitializeSpellGemUI (SpellData spellData) {
		this.spellData = spellData;
		spriteRenderer.sprite = spellData.spellGemSprite;
		defaultColor = ConstantsManager.instance.spellSchoolDictionary [spellData.spellSchool].schoolGemColor;
		spriteRenderer.color = defaultColor;
	}
	public void SetErrorColor() {
		spriteRenderer.color = Color.red;
	}
	public void SetNormalColor () {
		spriteRenderer.color = defaultColor;
	}
}
