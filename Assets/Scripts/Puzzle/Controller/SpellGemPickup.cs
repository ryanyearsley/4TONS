using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemPickup : MonoBehaviour
{
    public SpellData spellData;

    private SpriteRenderer spriteRenderer;

	void Awake () {

        spriteRenderer = GetComponent<SpriteRenderer> ();
		if (spellData != null) {
            spriteRenderer.sprite = spellData.icon;
        }
	}

    public void InitializeSpellGemPickUp (SpellData spellData) {
        this.spellData = spellData;
        if (spellData != null) {
            spriteRenderer.sprite = spellData.icon;
        }
    }

	private void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Player") {
            other.transform.root.GetComponent<PlayerPuzzleController> ().AddSpellGemToInteractable (this);
        }
    }
    private void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "Player") {
            other.transform.root.GetComponent<PlayerPuzzleController> ().RemoveSpellGemFromInteractable (this);
		}
    }
}
