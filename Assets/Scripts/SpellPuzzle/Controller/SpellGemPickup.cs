using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellGemPickup : MonoBehaviour
{
    [SerializeField]
    private SpellData spellData;

    private SpriteRenderer spriteRenderer;

	void Awake () {

        spriteRenderer = GetComponent<SpriteRenderer> ();
		if (spellData != null) {
            spriteRenderer.sprite = spellData.icon;
        }
	}

	private void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Player") {
            other.transform.root.GetComponent<SpellController> ().AddSpellGemToInteractable (this);
        }
    }
    private void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "Player") {
            other.transform.root.GetComponent<SpellController> ().RemoveSpellGemFromInteractable (this);
		}
    }
}
