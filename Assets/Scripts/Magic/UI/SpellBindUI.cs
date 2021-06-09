using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBindUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] spellBindSprites;

	public void InitializeSpellBindUI() {
		for (int i = 0; i < spellBindSprites.Length; i++) {
			UpdateBindingUI (i, false);
		}
		gameObject.SetActive (false);
	}
	public void UpdateBindingUI (int spellIndex, bool isOccupied) {
		if (isOccupied) {
			SetSpellBindOccupied (spellIndex);
		} else {
			SetSpellBindVacant (spellIndex);
		}
	}
	private void SetSpellBindOccupied (int index) {
		spellBindSprites [index].color = Color.red;
	}
	private void SetSpellBindVacant (int index) {
		spellBindSprites [index].color = Color.green;
	}
}
