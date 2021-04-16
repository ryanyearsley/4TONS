using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : PoolObject
{
	public Transform trans;

	private void Awake () {
		trans = GetComponent<Transform> ();
	}

	protected virtual void HighlightPickUp() {

	}
	protected virtual void UnhighlightPickUp () {
	}
	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player1") {
			HighlightPickUp ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().AddItemToInteractable (this);
			}
	}
	private void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player1") {
			UnhighlightPickUp ();
			other.transform.GetComponentInParent<PlayerInteractComponent> ().RemoveItemFromInteractable (this);
		}
	}
}
